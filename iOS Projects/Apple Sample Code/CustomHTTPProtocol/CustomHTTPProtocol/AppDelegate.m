/*
     File: AppDelegate.m
 Abstract: Main app controller.
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

#import "AppDelegate.h"

#import "WebViewController.h"

#import "CredentialsManager.h"

#import "CustomHTTPProtocol.h"

#include <mach/mach.h>              // for mach_thread_self

@interface AppDelegate () <CustomHTTPProtocolDelegate>

@property (nonatomic, retain, readwrite) WebViewController *   viewController;

@end

@implementation AppDelegate

@synthesize window         = _window;
@synthesize navController  = _navController;

@synthesize viewController = _viewController;

static BOOL sAppDelegateLoggingEnabled = YES;

static NSTimeInterval sAppStartTime;                // since reference date

// both of the following are protected by @synchronized on the class

static CFMutableDictionaryRef sThreadToIDMap;       // maps Mach thread ID (as returned by mach_thread_self) to our thread ID (allocated via sNextThreadID)
static NSUInteger             sNextThreadID = 1;

- (void)applicationDidFinishLaunching:(UIApplication *)application
{
    #pragma unused(application)
    assert(self.window != nil);
    assert(self.navController != nil);
    
    sAppStartTime = [NSDate timeIntervalSinceReferenceDate];

    // Prepare the globals needed by our logging code.
    
    sThreadToIDMap = CFDictionaryCreateMutable(NULL, 0, NULL, NULL);
    CFDictionarySetValue(sThreadToIDMap, (const void *) (uintptr_t) mach_thread_self(), (const void *) (uintptr_t) 0);

    // Start up the core code.
    
    [CustomHTTPProtocol setDelegate:self];
    if (YES) {
        [CustomHTTPProtocol start];
    }
    
    // Create the web view controller and set up the UI.  We do this after setting 
    // up the core code in case this triggers any HTTP requests.
    
    self.viewController = [[[WebViewController alloc] init] autorelease];
    assert(self.viewController != nil);
    self.viewController.title = @"CustomHTTPProtocol";
    if (NO) {
        self.viewController.navigationItem.rightBarButtonItem = [[[UIBarButtonItem alloc] initWithTitle:@"Test" style:UIBarButtonItemStyleBordered target:self action:@selector(testAction:)] autorelease];
    }
    [self.navController pushViewController:self.viewController animated:NO];

    [self.window addSubview:self.navController.view];
	[self.window makeKeyAndVisible];
}

- (void)customHTTPProtocol:(CustomHTTPProtocol *)protocol logWithFormat:(NSString *)format arguments:(va_list)argList
    // Called by the CustomHTTPProtocol class to pass us log messages.  We respond by simply 
    // logging the messages
{
    if (sAppDelegateLoggingEnabled) {
        NSTimeInterval  now;
        mach_port_t     machThreadID;
        NSUInteger      threadID;
        NSString *      str;
        char            elapsedStr[16];

        now = [NSDate timeIntervalSinceReferenceDate];

        machThreadID = mach_thread_self();
        @synchronized ([self class]) {
            BOOL            found;
            const void *    value;
            
            found = CFDictionaryGetValueIfPresent(sThreadToIDMap, (const void *) (uintptr_t) machThreadID, &value) != false;
            if (found) {
                threadID = (uintptr_t) value;
            } else {
                threadID = sNextThreadID;
                sNextThreadID += 1;
                CFDictionarySetValue(sThreadToIDMap, (const void *) (uintptr_t) machThreadID, (const void *) (uintptr_t) threadID);
            }
        };
        
        str = [[NSString alloc] initWithFormat:format arguments:argList];
        assert(str != nil);
        
        snprintf(elapsedStr, sizeof(elapsedStr), "+%.1f", (now - sAppStartTime));
        
        if (protocol == nil) {
            fprintf(stderr, "%3zu %s %s\n",    (size_t) threadID, elapsedStr, [str UTF8String]);
        } else {
            fprintf(stderr, "%3zu %s %p %s\n", (size_t) threadID, elapsedStr, protocol, [str UTF8String]);
        }
        
        [str release];
    }
}

- (void)logWithFormat:(NSString *)format, ...
{
    va_list     ap;
    
    va_start(ap, format);
    [self customHTTPProtocol:nil logWithFormat:format arguments:ap];
    va_end(ap);
}

- (BOOL)customHTTPProtocol:(CustomHTTPProtocol *)protocol canAuthenticateAgainstProtectionSpace:(NSURLProtectionSpace *)protectionSpace
    // A CustomHTTPProtocol delegate callback, called when the protocol needs to process 
    // an authentication challenge.  In this case we accept any server trust authentication 
    // challenges.
{
    assert(protocol != nil);
    #pragma unused(protocol)
    assert(protectionSpace != nil);
    
    return [[protectionSpace authenticationMethod] isEqual:NSURLAuthenticationMethodServerTrust];
}

- (void)customHTTPProtocol:(CustomHTTPProtocol *)protocol didReceiveAuthenticationChallenge:(NSURLAuthenticationChallenge *)challenge
    // A CustomHTTPProtocol delegate callback, called when the protocol has an authenticate 
    // challenge that the delegate accepts via -customHTTPProtocol:canAuthenticateAgainstProtectionSpace:. 
    // In this specific case it's only called to handle server trust authentication challenges. 
    // It evaluates the trust based on both the global set of trusted anchors and the list of trusted 
    // anchors returned by the CredentialsManager.
{
    OSStatus            err;
    NSURLCredential *   credential;
    SecTrustRef         trust;
    SecTrustResultType  trustResult;
    
    assert(protocol != nil);
    assert(challenge != nil);
    assert([[[challenge protectionSpace] authenticationMethod] isEqual:NSURLAuthenticationMethodServerTrust]);
    
    credential = nil;

    // Extract the SecTrust object from the challenge, apply our trusted anchors to that 
    // object, and then evaluate the trust.  If it's OK, create a credential and use 
    // that to resolve the authentication challenge.  If anything goes wrong, resolve 
    // the challenge with nil, which continues without a credential, which causes the 
    // connection to fail.
    
    trust = [[challenge protectionSpace] serverTrust];
    if (trust == NULL) {
        assert(NO);
    } else {
        err = SecTrustSetAnchorCertificates(trust, (CFArrayRef) [CredentialsManager sharedManager].trustedAnchors);
        if (err != noErr) {
            assert(NO);
        } else {
            err = SecTrustSetAnchorCertificatesOnly(trust, false);
            if (err != noErr) {
                assert(NO);
            } else {
                err = SecTrustEvaluate(trust, &trustResult);
                if (err != noErr) {
                    assert(NO);
                } else {
                    if ( (trustResult == kSecTrustResultProceed) || (trustResult == kSecTrustResultUnspecified) ) {
                        credential = [NSURLCredential credentialForTrust:trust];
                        assert(credential != nil);
                    }
                }
            }
        }
    }
    
    [protocol resolveAuthenticationChallenge:challenge withCredential:credential];
}

#pragma mark NSURLConnection test

// This code lets you test a vanilla NSURLConnection in addition to the UIWebView test shown by the main 
// app.  This is useful because UIWebView uses NSURLConnection (actually, the private CFNetwork API that 
// underlies NSURLConnection, CFURLConnection) in a unique way, so it's important to test your code 
// with both UIWebView and NSURLConnection.

- (void)testAction:(id)sender
{
    #pragma unused(sender)
    [self logWithFormat:@"test start"];
    (void) [NSURLConnection connectionWithRequest:[NSURLRequest requestWithURL:[NSURL URLWithString:@"http://www.apple.com/"]] delegate:self];
}

- (NSURLRequest *)connection:(NSURLConnection *)connection willSendRequest:(NSURLRequest *)request redirectResponse:(NSURLResponse *)response
{
    #pragma unused(connection)
    [self logWithFormat:@"test willSendRequest:%@ redirectResponse:%@", request, response];
    return request;
}

- (void)connection:(NSURLConnection *)connection didReceiveResponse:(NSURLResponse *)response
{
    #pragma unused(connection)
    #pragma unused(response)
    [self logWithFormat:@"test didReceiveResponse:%@", response];
}

- (void)connection:(NSURLConnection *)connection didReceiveData:(NSData *)data
{
    #pragma unused(connection)
    #pragma unused(data)
    [self logWithFormat:@"test didReceiveData:%zu", (size_t) [data length]];
}

- (NSCachedURLResponse *)connection:(NSURLConnection *)connection willCacheResponse:(NSCachedURLResponse *)cachedResponse
{
    #pragma unused(connection)
    [self logWithFormat:@"test willCacheResponse:%@", cachedResponse];
    return cachedResponse;
}

- (void)connectionDidFinishLoading:(NSURLConnection *)connection
{
    #pragma unused(connection)
    [self logWithFormat:@"test connectionDidFinishLoading"];
}

- (void)connection:(NSURLConnection *)connection didFailWithError:(NSError *)error
{
    #pragma unused(connection)
    #pragma unused(error)
    [self logWithFormat:@"test didFailWithError:%@ / %d", [error domain], (int) [error code]];
}

@end
