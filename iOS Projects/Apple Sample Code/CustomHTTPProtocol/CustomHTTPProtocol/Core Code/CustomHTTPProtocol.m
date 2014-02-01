/*
     File: CustomHTTPProtocol.m
 Abstract: An NSURLProtocol subclass that overrides the built-in HTTP/HTTPS protocol.
  Version: 1.0
 
 Disclaimer: IMPORTANT:  This Apple software is supplied to you by Apple
 Inc. ("Apple") in consideration of your agreement to the following
 terms, and your use, installation, modification or redistribution of
 this Apple software constitutes acceptance of these terms.  If you do
 not agree with these terms, please do not use, install, modify or
 redistribute this Apple software.
 
 In consideration of your agreement to abide by the following terms, and
 subject to these terms, Apple grants you a personal, non-exclusive
 license, under Apple's copyrights in this original Apple software (the
 "Apple Software"), to use, reproduce, modify and redistribute the Apple
 Software, with or without modifications, in source and/or binary forms;
 provided that if you redistribute the Apple Software in its entirety and
 without modifications, you must retain this notice and the following
 text and disclaimers in all such redistributions of the Apple Software.
 Neither the name, trademarks, service marks or logos of Apple Inc. may
 be used to endorse or promote products derived from the Apple Software
 without specific prior written permission from Apple.  Except as
 expressly stated in this notice, no other rights or licenses, express or
 implied, are granted by Apple herein, including but not limited to any
 patent rights that may be infringed by your derivative works or by other
 works in which the Apple Software may be incorporated.
 
 The Apple Software is provided by Apple on an "AS IS" basis.  APPLE
 MAKES NO WARRANTIES, EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION
 THE IMPLIED WARRANTIES OF NON-INFRINGEMENT, MERCHANTABILITY AND FITNESS
 FOR A PARTICULAR PURPOSE, REGARDING THE APPLE SOFTWARE OR ITS USE AND
 OPERATION ALONE OR IN COMBINATION WITH YOUR PRODUCTS.
 
 IN NO EVENT SHALL APPLE BE LIABLE FOR ANY SPECIAL, INDIRECT, INCIDENTAL
 OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 INTERRUPTION) ARISING IN ANY WAY OUT OF THE USE, REPRODUCTION,
 MODIFICATION AND/OR DISTRIBUTION OF THE APPLE SOFTWARE, HOWEVER CAUSED
 AND WHETHER UNDER THEORY OF CONTRACT, TORT (INCLUDING NEGLIGENCE),
 STRICT LIABILITY OR OTHERWISE, EVEN IF APPLE HAS BEEN ADVISED OF THE
 POSSIBILITY OF SUCH DAMAGE.
 
 Copyright (C) 2013 Apple Inc. All Rights Reserved.
 
 */

#import "CustomHTTPProtocol.h"

#import "CanonicalRequest.h"
#import "CacheStoragePolicy.h"

static BOOL kPreventOnDiskCaching = NO;

@interface CustomHTTPProtocol ()

@property (atomic, retain, readwrite) NSThread *                        clientThread;

@property (atomic, copy,   readwrite) NSArray *                         modes;              // see below
@property (atomic, assign, readwrite) NSTimeInterval                    startTime;          // written by client thread only, read by any thread
@property (atomic, retain, readwrite) NSURLRequest *                    actualRequest;      // client thread only
@property (atomic, retain, readwrite) NSURLConnection *                 connection;         // client thread only
@property (atomic, retain, readwrite) NSURLAuthenticationChallenge *    pendingChallenge;   // main thread only

// The concurrency control on .modes is too complex to explain in a short comment.  It's set up 
// on the client thread in -startLoading and then never modified.  It is, however, read by code 
// running on other threads (specifically the main thread), so we deallocate it in -dealloc 
// rather than in -stopLoading.  We can be sure that it's not read before it's set up because 
// the main thread code that reads it can only be called after -startLoading has started 
// the connection running.

- (void)cancelPendingChallenge;

@end

@implementation CustomHTTPProtocol

@synthesize modes            = _modes;
@synthesize startTime        = _startTime;
@synthesize clientThread     = _clientThread;
@synthesize actualRequest    = _actualRequest;
@synthesize connection       = _connection;
@synthesize pendingChallenge = _pendingChallenge;

#pragma mark * Subclass specific additions

static id<CustomHTTPProtocolDelegate> sDelegate;          // protected by @synchronized on the class

+ (void)start
    // See comment in header.
{
    [NSURLProtocol registerClass:self];
}

+ (id<CustomHTTPProtocolDelegate>)delegate
    // See comment in header.
{
    id<CustomHTTPProtocolDelegate> result;

    @synchronized (self) {
        result = [[sDelegate retain] autorelease];
    }
    return result;
}

+ (void)setDelegate:(id<CustomHTTPProtocolDelegate>)newValue
    // See comment in header.
{
    @synchronized (self) {
        sDelegate = newValue;
    }
}

+ (void)customHTTPProtocol:(CustomHTTPProtocol *)protocol logWithFormat:(NSString *)format, ...
    // All internal logging calls this routine, which routes the log message to the 
    // delegate.
{
    // protocol may be nil
    id<CustomHTTPProtocolDelegate> delegate;
    
    delegate = [self delegate];
    if ([delegate respondsToSelector:@selector(customHTTPProtocol:logWithFormat:arguments:)]) {
        va_list ap;
        
        va_start(ap, format);
        [delegate customHTTPProtocol:protocol logWithFormat:format arguments:ap];
        va_end(ap);
    }
}

#pragma mark * NSURLProtocol overrides

static NSString * kOurRequestProperty = @"com.apple.dts.CustomHTTPProtocol";

+ (BOOL)canInitWithRequest:(NSURLRequest *)request
    // An override of an NSURLProtocol method.  We claim all HTTPS requests that don't have 
    // kOurRequestProperty attached.
    //
    // This can be called on any thread, so we have to be careful what we touch.
{
    BOOL        result;
    NSURL *     url;
    NSString *  scheme;
    
    url = [request URL];
    assert(url != nil);     // The code won't crash if url is nil, but we still want to know at debug time.
    
    result = ([self propertyForKey:kOurRequestProperty inRequest:request] == nil);
    if ( ! result ) {
        [self customHTTPProtocol:nil logWithFormat:@"decline request %@ (recursive)", request];
    } else {
        scheme = [[url scheme] lowercaseString];
        assert(scheme != nil);
        
        result = [scheme isEqual:@"https"];

        // Flip the following to YES to have all requests go through the custom protocol.
        
        if ( ! result && NO ) {
            result = [scheme isEqual:@"http"];
        }

        if ( ! result ) {
            [self customHTTPProtocol:nil logWithFormat:@"decline request %@ (scheme mismatch)", request];
        } else {
            [self customHTTPProtocol:nil logWithFormat:@"accept request %@", request];
        }
    }
    
    return result;
}

+ (NSURLRequest *)canonicalRequestForRequest:(NSURLRequest *)request
    // An override of an NSURLProtocol method.   Canonicalising a request is quite complex, 
    // so all the heavy lifting has been shuffled off to a separate module.
    //
    // This can be called on any thread, so we have to be careful what we touch.
{
    NSURLRequest *      result;
    
    result = CanonicalRequestForRequest(request);

    [self customHTTPProtocol:nil logWithFormat:@"canonicalized %@ to %@", request, result];
    
    return result;
}

- (id)initWithRequest:(NSURLRequest *)request cachedResponse:(NSCachedURLResponse *)cachedResponse client:(id <NSURLProtocolClient>)client
    // An override of an NSURLProtocol method.   All we do here is log the call.
    //
    // This can be called on any thread, so we have to be careful what we touch.
{
    self = [super initWithRequest:request cachedResponse:cachedResponse client:client];
    if (self != nil) {
        [[self class] customHTTPProtocol:self logWithFormat:@"init for %@ from <%@ %p>", request, [client class], client];
    }
    return self;
}

- (void)dealloc
    // This can be called on any thread, so we have to be careful what we touch.
{
    [[self class] customHTTPProtocol:self logWithFormat:@"dealloc"];
    [self->_modes release];
    [self->_clientThread release];
    assert(self->_actualRequest == nil);            // we should have cleared it by now
    assert(self->_connection == nil);               // we should have cancelled it by now
    assert(self->_pendingChallenge == nil);         // we should have cancelled it by now
    [super dealloc];
}

- (void)startLoading
    // An override of an NSURLProtocol method.   At this point we kick off the process 
    // of loading the URL via NSURLConnection.
    //
    // The thread that this is called on becomes the client thread.
{
    NSMutableURLRequest *   newRequest;
    NSString *              currentMode;
    
    assert(self.clientThread == nil);           // you can't call -startLoading twice
    assert(self.connection == nil);

    // Calculate our effective run loop modes.  In some circumstances (yes I'm looking at 
    // you UIWebView!) we can be called from a non-standard thread which then runs a 
    // non-standard run loop mode waiting for the request to finish.  We detect this 
    // non-standard mode and add it to the list of run loop modes we use when scheduling 
    // our callbacks.  Exciting huh?
    //
    // For debugging purposes the non-standard mode is "WebCoreSynchronousLoaderRunLoopMode" 
    // but it's better not to hard-code that here.
    
    assert(self.modes == nil);
    currentMode = [[NSRunLoop currentRunLoop] currentMode];
    if ( [currentMode isEqual:NSDefaultRunLoopMode] ) {
        currentMode = nil;
    }
    self.modes = [NSArray arrayWithObjects:NSDefaultRunLoopMode, currentMode, nil];
    assert([self.modes count] > 0);

    // Create new request that's a clone of the request we were initialised with, 
    // except that it has our custom property set on it.
    
    newRequest = [[[self request] mutableCopy] autorelease];
    assert(newRequest != nil);
    
    [[self class] setProperty:[NSNumber numberWithBool:YES] forKey:kOurRequestProperty inRequest:newRequest];

    self.startTime = [NSDate timeIntervalSinceReferenceDate];
    if (currentMode == nil) {
        [[self class] customHTTPProtocol:self logWithFormat:@"start %@", newRequest];
    } else {
        [[self class] customHTTPProtocol:self logWithFormat:@"start %@ (mode %@)", newRequest, currentMode];
    }
    
    // Now create a connection with the new request.  Don't start it immediately because 
    // a) if we start immediately our delegate can be called before -initWithRequest:xxx 
    // returns, and that confuses our asserts, and b) we want to customise the run loop modes.

    self.connection = [[[NSURLConnection alloc] initWithRequest:newRequest delegate:self startImmediately:NO] autorelease];
    assert(self.connection != nil);

    for (NSString * mode in self.modes) {
        [self.connection scheduleInRunLoop:[NSRunLoop currentRunLoop] forMode:mode];
    }
    
    // Latch the actual request we sent down so that we can use it for cache storage 
    // policy determination.
    
    self.actualRequest = newRequest;
    
    // Latch the thread we were called on, primarily for debugging purposes.
    
    self.clientThread = [NSThread currentThread];
    
    // Once everything is ready to go, start the request.
    
    [self.connection start];
}

- (void)stopLoading
    // An override of an NSURLProtocol method.   We cancel our load.
    //
    // Expected to be called on the client thread.
{
    [[self class] customHTTPProtocol:self logWithFormat:@"stop (elapsed %.1f)", [NSDate timeIntervalSinceReferenceDate] - self.startTime];
    
    assert(self.clientThread != nil);           // someone must have called -startLoading

    // Check that we're being stopped on the same thread that we were started 
    // on.  Without this invariant things are going to go badly (for example, 
    // run loop sources that got attached during -startLoading may not get 
    // detached here).
    //
    // I originally had code here to skip over to the client thread but that 
    // actually gets complex when you consider run loop modes, so I've nixed it. 
    // Rather, I rely on our client calling us on the right thread, which is what 
    // the following assert is about.
    
    assert([NSThread currentThread] == self.clientThread);
    
    [self cancelPendingChallenge];
    if (self.connection != nil) {
        [self.connection cancel];
        self.connection = nil;
    }
    self.actualRequest = nil;
    // Don't nil out self.modes; see the comment near the property declaration for a 
    // a discussion of this.
}

#pragma mark * Authentication challenge handling

- (void)didReceiveAuthenticationChallenge:(NSURLAuthenticationChallenge *)challenge
{
    assert(challenge != nil);

    assert([NSThread currentThread] == self.clientThread);

    [[self class] customHTTPProtocol:self logWithFormat:@"challenge %@ received", challenge];

    // Just pass the work off to the main thread.  We do this so that all accesses 
    // to pendingChallenge are done from the main thread, which avoids the need for 
    // extra synchronisation.
    // 
    // Note that we use the default run loop mode here, not the common modes.  We don't want 
    // an authorisation dialog showing up on top of an active menu (-:
    
    [self performSelectorOnMainThread:@selector(mainThreadDidReceiveAuthenticationChallenge:) withObject:challenge waitUntilDone:NO];
}

- (void)mainThreadDidReceiveAuthenticationChallenge:(NSURLAuthenticationChallenge *)challenge
{
    assert(challenge != nil);

    assert([NSThread isMainThread]);
    
    if (self.pendingChallenge != nil) {

        // Our delegate is not expecting a second authentication challenge before resolving the 
        // first.  Likewise, NSURLConnection shouldn't send us a second authentication challenge 
        // before we resolve the first.  If this happens, assert, log, and cancel the challenge.
        //
        // Note that we have to cancel the challenge on the thread on which we received it, 
        // namely, the client thread.

        [[self class] customHTTPProtocol:self logWithFormat:@"challenge %@ cancelled; other challenge pending", challenge];
        assert(NO);
        [self performSelector:@selector(clientThreadCancelAuthenticationChallenge:) onThread:self.clientThread withObject:challenge waitUntilDone:NO modes:self.modes];
    } else {
        id<CustomHTTPProtocolDelegate>  delegate;

        delegate = [[self class] delegate];

        // Remember the challenge in progress.
        
        self.pendingChallenge = challenge;

        // Tell the delegate about it.  It would be weird if the delegate didn't support this 
        // selector (it did return YES from -customHTTPProtocol:canAuthenticateAgainstProtectionSpace: 
        // after all), but if it doesn't then we just cancel the challenge ourselves (or the client 
        // thread, of course).
        
        if ([delegate respondsToSelector:@selector(customHTTPProtocol:canAuthenticateAgainstProtectionSpace:)]) {
            [[self class] customHTTPProtocol:self logWithFormat:@"challenge %@ passed to delegate", challenge];
            [delegate customHTTPProtocol:self didReceiveAuthenticationChallenge:self.pendingChallenge];
        } else {
            self.pendingChallenge = nil;
            [[self class] customHTTPProtocol:self logWithFormat:@"challenge %@ cancelled; no delegate method", challenge];
            assert(NO);
            [self performSelector:@selector(clientThreadCancelAuthenticationChallenge:) onThread:self.clientThread withObject:challenge waitUntilDone:NO modes:self.modes];
        }
    }
}

- (void)clientThreadCancelAuthenticationChallenge:(NSURLAuthenticationChallenge *)challenge
{
    assert(challenge != nil);
    
    assert([NSThread currentThread] == self.clientThread);

    [[challenge sender] cancelAuthenticationChallenge:challenge];
}

- (void)cancelPendingChallenge
{
    assert([NSThread currentThread] == self.clientThread);

    // Just pass the work off to the main thread.  We do this so that all accesses 
    // to pendingChallenge are done from the main thread, which avoids the need for 
    // extra synchronisation.

    [self performSelectorOnMainThread:@selector(mainThreadCancelPendingChallenge) withObject:nil waitUntilDone:NO];
}

- (void)mainThreadCancelPendingChallenge
{
    assert([NSThread isMainThread]);
    
    if (self.pendingChallenge == nil) {
        // This is not only not unusual, it's actually very typical.  It happens every time you shut down 
        // the connection.  Ideally I'd like to not even call -mainThreadCancelPendingChallenge when 
        // there's no challenge outstanding, but the synchronisation issues are tricky.  Rather than solve 
        // those, I'm just not going to log in this case.
        //
        // [[self class] customHTTPProtocol:self logWithFormat:@"challenge not cancelled; no challenge pending"];
    } else {
        id<CustomHTTPProtocolDelegate>  delegate;
        NSURLAuthenticationChallenge *  challenge;

        delegate = [[self class] delegate];

        challenge = [[self.pendingChallenge retain] autorelease];
        self.pendingChallenge = nil;
        
        if ([delegate respondsToSelector:@selector(customHTTPProtocol:didCancelAuthenticationChallenge:)]) {
            [[self class] customHTTPProtocol:self logWithFormat:@"challenge %@ cancellation passed to delegate", challenge];
            [delegate customHTTPProtocol:self didCancelAuthenticationChallenge:challenge];
        } else {
            [[self class] customHTTPProtocol:self logWithFormat:@"challenge %@ cancellation failed; no delegate method", challenge];
            // If we managed to send a challenge to the client but can't cancel it, that's bad. 
            // There's nothing we can do at this point except log the problem.
            assert(NO);
        }
    }
}

- (void)resolveAuthenticationChallenge:(NSURLAuthenticationChallenge *)challenge withCredential:(NSURLCredential *)credential
    // See comment in header.
{
    assert(challenge == self.pendingChallenge);
    // credential may be nil
    
    assert([NSThread isMainThread]);
    assert(self.clientThread != nil);
    
    if (challenge != self.pendingChallenge) {
        [[self class] customHTTPProtocol:self logWithFormat:@"challenge resolution mismatch (%@ / %@)", challenge, self.pendingChallenge];
        // This should never happen, and we want to know if it does, at least in the debug build.
        assert(NO);
    } else {
        NSDictionary *  parameters;
        
        // We have to pass two parameters to -clientThreadResolveWithParameters:, which is a pain 
        // because -performSelector:onThread:xxx only supports one.  So pack them into a dictionary. 
        // We can't use GCD here because we're targetting a specific thread.
        
        parameters = [NSDictionary dictionaryWithObjectsAndKeys:
            challenge,  @"challenge", 
            credential, @"credential",          // If credential is nil, it terminates the objects and keys sequence, 
            nil                                 // resulting in a dictionary that just contains the challenge.
        ];
        assert(parameters != nil);
        
        // We clear out our record of the pending challenge and then pass the real work 
        // over to the client thread (which ensures that the challenge is resolved on 
        // the same thread we received it on).
        
        self.pendingChallenge = nil;
        
        [self performSelector:@selector(clientThreadResolveWithParameters:) onThread:self.clientThread withObject:parameters waitUntilDone:NO modes:self.modes];
    }
}

- (void)clientThreadResolveWithParameters:(NSDictionary *)parameters
{
    NSURLAuthenticationChallenge *  challenge;
    NSURLCredential *               credential;

    assert([NSThread currentThread] == self.clientThread);
    
    challenge = [parameters objectForKey:@"challenge"];
    assert([challenge isKindOfClass:[NSURLAuthenticationChallenge class]]);

    credential = [parameters objectForKey:@"credential"];
    assert( (credential == nil) || [credential isKindOfClass:[NSURLCredential class]] );

    if (credential == nil) {
        [[self class] customHTTPProtocol:self logWithFormat:@"challenge %@ resolved without credential", challenge];
        [[challenge sender] continueWithoutCredentialForAuthenticationChallenge:challenge];
    } else {
        [[self class] customHTTPProtocol:self logWithFormat:@"challenge %@ resolved with %@", challenge, credential];
        [[challenge sender] useCredential:credential forAuthenticationChallenge:challenge];
    }
}

#pragma mark * NSURLConnection delegate callbacks

- (NSURLRequest *)connection:(NSURLConnection *)connection willSendRequest:(NSURLRequest *)request redirectResponse:(NSURLResponse *)response
    // An NSURLConnection delegate callback.  We use this to tell the client about redirects 
    // (and for a bunch of debugging and logging).
    //
    // Runs on the client thread.
{
    #pragma unused(connection)
    assert(connection == self.connection);
    assert(request != nil);
    // response may be nil

    assert([NSThread currentThread] == self.clientThread);

    if (response == nil) {
        [[self class] customHTTPProtocol:self logWithFormat:@"will send request %@", request];
    } else {
        NSMutableURLRequest *    redirectRequest;

        // If response is not nil this is a redirect so we tell the client.

        [[self class] customHTTPProtocol:self logWithFormat:@"will send request %@ following redirect %@", request, response];

        // The new request was copied from our old request, so it has our magic property.  We actually 
        // have to remove that so that, when the client starts the new request, we see it.  If we 
        // don't do this then we never see the new request and thus don't get a chance to change 
        // its caching behaviour.
        //
        // We also cancel our current connection because the client is going to start a new request for 
        // us anyway.

        assert([[self class] propertyForKey:kOurRequestProperty inRequest:request] != nil);
        
        redirectRequest = [[request mutableCopy] autorelease];
        [[self class] removePropertyForKey:kOurRequestProperty inRequest:redirectRequest];
        
        [[self client] URLProtocol:self wasRedirectedToRequest:redirectRequest redirectResponse:response];
        
        [self.connection cancel];
        [[self client] URLProtocol:self didFailWithError:[NSError errorWithDomain:NSCocoaErrorDomain code:NSUserCancelledError userInfo:nil]];
    }
    
    return request;
}

- (BOOL)connection:(NSURLConnection *)connection canAuthenticateAgainstProtectionSpace:(NSURLProtectionSpace *)protectionSpace
{
    #pragma unused(connection)
    BOOL        result;
    id<CustomHTTPProtocolDelegate> delegate;

    assert(connection == self.connection);
    assert(protectionSpace != nil);

    assert([NSThread currentThread] == self.clientThread);

    // Simple ask our delegate what to do.
    
    delegate = [[self class] delegate];
    
    result = NO;
    if ([delegate respondsToSelector:@selector(customHTTPProtocol:canAuthenticateAgainstProtectionSpace:)]) {
        result = [delegate customHTTPProtocol:self canAuthenticateAgainstProtectionSpace:protectionSpace];
    }

    if (result) {
        [[self class] customHTTPProtocol:self logWithFormat:@"can authenticate %@", protectionSpace];
    } else {
        [[self class] customHTTPProtocol:self logWithFormat:@"cannot authenticate %@", protectionSpace];
    }

    return result;
}

- (void)connection:(NSURLConnection *)connection didReceiveAuthenticationChallenge:(NSURLAuthenticationChallenge *)challenge
{
    #pragma unused(connection)
    assert(connection == self.connection);
    assert(challenge != nil);

    assert([NSThread currentThread] == self.clientThread);

    [[self class] customHTTPProtocol:self logWithFormat:@"received challenge %@", challenge];
    
    // Call -didReceiveAuthenticationChallenge:, which passes the challenge on the delegate.

    [self didReceiveAuthenticationChallenge:challenge];
}

- (void)connection:(NSURLConnection *)connection didReceiveResponse:(NSURLResponse *)response
    // An NSURLConnection delegate callback.  We pass this on to the client.
    //
    // Runs on the client thread.
{
    #pragma unused(connection)
    NSURLCacheStoragePolicy cacheStoragePolicy;

    assert(connection == self.connection);
    assert(response != nil);

    assert([NSThread currentThread] == self.clientThread);

    // Pass the call on to our client.  The only tricky thing is that we have to decide on a 
    // cache storage policy, which is based on the actual request we issued, not the request 
    // we were given.
    
    if ([response isKindOfClass:[NSHTTPURLResponse class]]) {
        cacheStoragePolicy = CacheStoragePolicyForRequestAndResponse(self.actualRequest, (NSHTTPURLResponse *) response);
    } else {
        assert(NO);
        cacheStoragePolicy = NSURLCacheStorageNotAllowed;
    }

    // If we're forcing in in-memory caching only, override the cache storage policy.
    
    if (kPreventOnDiskCaching) {
        if (cacheStoragePolicy == NSURLCacheStorageAllowed) {
            cacheStoragePolicy = NSURLCacheStorageAllowedInMemoryOnly;
        }
    }

    [[self class] customHTTPProtocol:self logWithFormat:@"received response %@ with cache storage policy %zu", response, (size_t) cacheStoragePolicy];
    
    [[self client] URLProtocol:self didReceiveResponse:response cacheStoragePolicy:cacheStoragePolicy];
}

- (NSCachedURLResponse *)connection:(NSURLConnection *)connection willCacheResponse:(NSCachedURLResponse *)cachedResponse
{
    #pragma unused(connection)
    assert(connection == self.connection);
    assert(cachedResponse != nil);

    if (kPreventOnDiskCaching) {
        [[self class] customHTTPProtocol:self logWithFormat:@"will not cache response"];
    } else {
        [[self class] customHTTPProtocol:self logWithFormat:@"will cache response"];
    }

    return cachedResponse;
}

- (void)connection:(NSURLConnection *)connection didReceiveData:(NSData *)data
    // An NSURLConnection delegate callback.  We pass this on to the client.
    //
    // Runs on the client thread.
{
    #pragma unused(connection)
    assert(connection == self.connection);
    assert(data != nil);

    assert([NSThread currentThread] == self.clientThread);

    // Just pass the call on to our client.

    if (NO) {
        [[self class] customHTTPProtocol:self logWithFormat:@"received %zu bytes of data", (size_t) [data length]];
    }
    [[self client] URLProtocol:self didLoadData:data];
}

- (void)connectionDidFinishLoading:(NSURLConnection *)connection
    // An NSURLConnection delegate callback.  We pass this on to the client.
    //
    // Runs on the client thread.
{
    #pragma unused(connection)
    assert(connection == self.connection);

    assert([NSThread currentThread] == self.clientThread);

    [[self class] customHTTPProtocol:self logWithFormat:@"success"];

    // Just pass the call on to our client.

    [[self client] URLProtocolDidFinishLoading:self];
    
    // We don't need to clean up the connection here; the system will call 
    // our -stopLoading for that.
}

- (void)connection:(NSURLConnection *)connection didFailWithError:(NSError *)error
    // An NSURLConnection delegate callback.  We pass this on to the client.
    //
    // Runs on the client thread.
{
    #pragma unused(connection)
    assert(connection == self.connection);
    assert(error != nil);

    assert([NSThread currentThread] == self.clientThread);

    [[self class] customHTTPProtocol:self logWithFormat:@"error %@ / %d", [error domain], (int) [error code]];

    // Just pass the call on to our client.

    [[self client] URLProtocol:self didFailWithError:error];

    // We don't need to clean up the connection here; the system will call 
    // our -stopLoading for that.
}

@end
