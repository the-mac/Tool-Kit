/*
     File: WebViewController.m
 Abstract: Main web view controller.
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

#import "WebViewController.h"

#import "CredentialsManager.h"

#include <Security/Security.h>

@interface WebViewController () <UIWebViewDelegate>

// private properties

@property (nonatomic, retain, readwrite) NSURLRequest *     installRequest;
@property (nonatomic, retain, readwrite) NSURLConnection *  installConnection;
@property (nonatomic, retain, readwrite) NSMutableData *    installData;

// forward declarations

- (void)displaySites;
- (void)displayError:(NSError *)error;
- (NSURL *)failingURLForError:(NSError *)error;
- (void)installFromURL:(NSURL *)url;
- (void)installStopWithError:(NSError *)error;

@end

@implementation WebViewController

@synthesize webView = _webView;
@synthesize installRequest = _installRequest;
@synthesize installConnection = _installConnection;
@synthesize installData = _installData;

- (id)init
{
    self = [super initWithNibName:@"WebViewController" bundle:nil];
    if (self != nil) {
        self.navigationItem.leftBarButtonItem = [[[UIBarButtonItem alloc] initWithTitle:@"Sites" style:UIBarButtonItemStyleBordered target:self action:@selector(sitesAction:)] autorelease];
    }
    return self;
}

- (void)dealloc
{
    [self->_webView release];

    // All of these should be nil because the connection retains its delegate (that is, us) 
    // until it completes, and we clean these up when the connection completes.
    
    assert(self->_installRequest == nil);
    assert(self->_installConnection == nil);
    assert(self->_installData == nil);

    [super dealloc];
}

- (IBAction)sitesAction:(id)sender
{
    #pragma unused(sender)

    // If we're currently downloading an anchor to install, stop that now.
    
    if (self.installConnection != nil) {
        [self installStopWithError:[NSError errorWithDomain:NSCocoaErrorDomain code:NSUserCancelledError userInfo:nil]];
    }
    
    // Display the list of sites that the user can choose from.
    
    [self displaySites];
}

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    assert(self.webView != nil);
    assert(self.webView.delegate == self);
    [self displaySites];
}

- (void)viewDidUnload
{
    [super viewDidUnload];
    self.webView.delegate = nil;
    self.webView = nil;
}

#pragma mark * Web view delegate callbacks

// When we want to display the anchor install UI, we point the web view at some HTML 
// (derived from "anchorInstall.html") that contains a HTML form.  When the user taps the 
// Install button, the form posts to a URL.  We then catch that URL in 
// -webView:shouldStartLoadWithRequest:navigationType: and start the install.  We give 
// that URL a special prefix, kAnchorInstallSchemePrefix, to make it easy to recognise. 
// For example, if we display the install UI for <http://www.cacert.org/certs/root.der>, 
// the URL that gets POSTed is <x-anchor-install-http://www.cacert.org/certs/root.der>. 
//
// Note that we use a prefix rather than a custom scheme so as to preserver the previous 
// scheme, which might be important (for example, "http" vs "https", or perhaps even 
// "ftp").

static NSString * kAnchorInstallSchemePrefix = @"x-anchor-install-";

- (BOOL)webView:(UIWebView *)webView shouldStartLoadWithRequest:(NSURLRequest *)request navigationType:(UIWebViewNavigationType)navigationType
    // A UIWebView delegate callback, called when the web view starts loading a 
    // new page.  We detect the web view trying to load one of our anchor install 
    // URLs and start loading it directly via NSURLConnection.  In that case we 
    // also tell the web view to display the "Installing..." UI.
{
    BOOL                allowLoad;
    NSMutableString *   installURLString;
    NSURL *             installURL;
    
    assert(webView == self.webView);
    #pragma unused(webView)
    assert(request != nil);
    
    NSLog(@"%@ %d", [request URL], navigationType);
    
    // If it's one of our special anchor install URLs...
    
    if ( [[[[request URL] scheme] lowercaseString] hasPrefix:kAnchorInstallSchemePrefix] ) {

        // Start downloading the anchor using NSURLConnection.  Before we call the install 
        // code (-installFromURL:) we have to calculate the install URL by stripping the 
        // prefix off the request URL.

        installURLString = [[[[request URL] absoluteString] mutableCopy] autorelease];
        assert(installURLString != nil);
        
        [installURLString replaceCharactersInRange:NSMakeRange(0, [kAnchorInstallSchemePrefix length]) withString:@""];

        installURL = [NSURL URLWithString:installURLString];
        assert(installURL != nil);
        
        [self installFromURL:installURL];
        
        allowLoad = NO;
    } else {
        allowLoad = YES;
    }

    return allowLoad;
}

- (void)webView:(UIWebView *)webView didFailLoadWithError:(NSError *)error
    // A UIWebView delegate callback, called when the web view encounters an error. 
    // We check for an error indicating that the user navigated to a certificate 
    // and, if we see it, point the web view to our anchor install page 
    // (derived from "anchorInstall.html").
{
    NSURL *     failingURL;
    NSString *  domain;
    NSInteger   code;
    BOOL        handled;
    
    assert(webView == self.webView);
    #pragma unused(webView)
    assert(error != nil);

    handled = NO;
    
    // Extract information from the error.
    
    domain  = [error domain];
    code    = [error code];
    failingURL = [self failingURLForError:error];

    assert(domain != nil);

    // If we get an error from WebKit saying that it couldn't load the data 
    // (WebKitErrorFrameLoadInterruptedByPolicyChange) and the URL looks like a 
    // certificate, kick off the anchor install UI.
    
    if ( [domain isEqual:@"WebKitErrorDomain"] && (code == 102) && (failingURL != nil) ) {
        NSString *          failingURLExtension;
        NSString *          anchorInstallPath;
        NSString *          anchorInstallTemplate;
        NSMutableString *   installURLString;
        NSURL *             installURL;

        assert([failingURL scheme] != nil);     // If the user has no scheme, adding kAnchorInstallSchemePrefix would be 
                                                // completely bogus.  This shouldn't never happen, but the assert makes sure.
    
        failingURLExtension = [[[[failingURL absoluteString] lastPathComponent] pathExtension] lowercaseString];
        if ( (failingURLExtension != nil) && ([failingURLExtension isEqual:@"cer"] || [failingURLExtension isEqual:@"der"]) ) {
        
            // Get the contents of "anchorInstall.html" and substitute the failing URL and the 
            // install URL into the text.  Simple substitution like this is fine in this case 
            // because the incoming texts are known good URLs, and thus don't need any form 
            // of quoting.
            
            anchorInstallPath = [[NSBundle mainBundle] pathForResource:@"anchorInstall" ofType:@"html"];
            assert(anchorInstallPath != nil);
            
            anchorInstallTemplate  = [NSString stringWithContentsOfFile:anchorInstallPath usedEncoding:NULL error:NULL];
            assert(anchorInstallTemplate != nil);

            // Calculate installURL, that is, the failing URL without the prefix 
            // (kAnchorInstallSchemePrefix).

            installURLString = [[[failingURL absoluteString] mutableCopy] autorelease];
            assert(installURLString != nil);
            
            [installURLString replaceCharactersInRange:NSMakeRange(0, 0) withString:kAnchorInstallSchemePrefix];

            installURL = [NSURL URLWithString:installURLString];
            assert(installURL != nil);

            assert(failingURL != nil);
            
            // Get the web view to load the anchor install UI.  Make sure that we give it a 
            // valid base URL so that page-relative URLs within the page work properly.
            
            [self.webView loadHTMLString:[NSString stringWithFormat:anchorInstallTemplate, failingURL, installURL] baseURL:[NSURL fileURLWithPath:anchorInstallPath]];
            handled = YES;
        }
    } else if ( [domain isEqual:NSURLErrorDomain] && (code == NSURLErrorCancelled) ) {
        // UIWebView sends us NSURLErrorCancelled errors when things fail that aren't critical, so for the moment 
        // we just ignore them.
        handled = YES;
    }
    
    // If we didn't handle the error as a special case, point the web view at our error page.
    
    if ( ! handled) {
        [self displayError:error];
    }    
}

#pragma mark * Web view utilities

- (void)displaySites
    // Tells the web view to load "root.html", our initial start page.
{
    NSURL *     rootURL;

    rootURL = [[NSBundle mainBundle] URLForResource:@"root" withExtension:@"html"];
    assert(rootURL != nil);
    
    [self.webView loadRequest:[NSURLRequest requestWithURL:rootURL]];
}

- (void)didStartInstall
    // Tell the anchor install page that we've started installing an anchor.  
    // It responds by display its "Installing..." UI.
{
    (void) [self.webView stringByEvaluatingJavaScriptFromString:@"didStartInstall()"];
}

- (void)didFinishInstall
    // Tell the anchor install page that we've successfully installed an anchor.  
    // It responds by display its "Installed" UI.  Note that there's equivalent 
    // error notification; errors result in a redirect to our error page (see 
    // the -displayError: method).
{
    (void) [self.webView stringByEvaluatingJavaScriptFromString:@"didFinishInstall()"];
}

- (void)displayError:(NSError *)error
    // Tells the web view to load "error.html", our standard error page, 
    // parameterising it with the error domain, code and failing URL.
{
    NSURL *     failingURL;
    NSString *  failingURLString;
    NSString *  errorPath;
    NSString *  errorTemplate;
    
    assert(error != nil);

    failingURL = [self failingURLForError:error];
    if (failingURL == nil) {
        failingURLString = @"n/a";
    } else {
        assert([failingURL isKindOfClass:[NSURL class]]);
        assert([failingURL scheme] != nil);
        failingURLString = [failingURL absoluteString];
        assert(failingURLString != nil);
    }

    errorPath = [[NSBundle mainBundle] pathForResource:@"error" ofType:@"html"];
    assert(errorPath != nil);

    errorTemplate = [NSString stringWithContentsOfFile:errorPath usedEncoding:NULL error:NULL];
    assert(errorTemplate != nil);
    
    [self.webView loadHTMLString:[NSString stringWithFormat:errorTemplate, failingURLString, [error domain], (size_t) [error code]] baseURL:[NSURL fileURLWithPath:errorPath]];
}

#pragma mark * Error utilities

static NSString * kInstallErrorDomain = @"kInstallErrorDomain";

enum {
    // positive numbers are HTTP status codes
    kInstallErrorUnsupportedMIMEType   = -1, 
    kInstallErrorCertificateDataTooBig = -2,
    kInstallErrorCertificateDataBad    = -3
};

- (NSError *)installErrorWithCode:(NSInteger)code
    // Returns an error object in the domain kInstallErrorDomain with the 
    // specified error code and the failing URL set from the current install 
    // request ([self.installRequest URL]).
{
    NSURL *                 url;
    NSString *              urlStr;
    NSMutableDictionary *   userInfo;
        
    url = [self.installRequest URL];
    urlStr = nil;
    if (url != nil) {
        urlStr = [url absoluteString];
    }

    if ( (url == nil) && (urlStr == nil) ) {
        userInfo = nil;
    } else {
        userInfo = [NSMutableDictionary dictionary];
        assert(userInfo != nil);
        
        if (url != nil) {
            [userInfo setObject:url forKey:NSURLErrorFailingURLErrorKey];
        }
        if (urlStr != nil) {
            [userInfo setObject:urlStr forKey:NSURLErrorFailingURLStringErrorKey];
        }
    }

    return [NSError errorWithDomain:kInstallErrorDomain code:code userInfo:userInfo];
}

- (NSURL *)failingURLForError:(NSError *)error
    // Extracts the failing URL from an NSError by way of the NSURLErrorFailingURLErrorKey 
    // and NSURLErrorFailingURLStringErrorKey values in the error's user info dictionary.
{
    NSURL *         result;
    NSDictionary *  userInfo;
    
    assert(error != nil);
    
    result = nil;
    
    userInfo = [error userInfo];
    if (userInfo != nil) {
        result = [userInfo objectForKey:NSURLErrorFailingURLErrorKey];
        assert( (result == nil) || [result isKindOfClass:[NSURL class]] );
        
        if (result == nil) {
            NSString *  urlStr;
            
            urlStr = [userInfo objectForKey:NSURLErrorFailingURLStringErrorKey];
            assert( (urlStr == nil) || [urlStr isKindOfClass:[NSString class]] );
            if (urlStr != nil) {
                assert([urlStr isKindOfClass:[NSString class]]);
                
                result = [NSURL URLWithString:urlStr];
            }
        }
    }
    
    return result;
}

#pragma mark * Anchor certificate fetch and install

enum {
    kInstallCertificateDataMaxLength     = 256 * 1024,
    kInstallCertificateDataDefaultLength = 64 * 1024
};

- (void)installFromURL:(NSURL *)url
    // Starts the process to download and install an anchor certificate.
{
    assert(url != nil);
    
    if (self.installConnection == nil) {
        assert(self.installRequest == nil);
        assert(self.installData == nil);
        
        // Start the connection to download and install the anchor certificate.
        
        self.installRequest = [NSURLRequest requestWithURL:url];
        assert(self.installRequest != nil);
        
        self.installConnection = [NSURLConnection connectionWithRequest:self.installRequest delegate:self];
        assert(self.installConnection != nil);

        [self didStartInstall];
    } else {
        assert(NO);     // We shouldn't be able to get a second install going until the first is complete.
    }
}

- (void)connection:(NSURLConnection *)connection didReceiveResponse:(NSURLResponse *)response
    // An NSURLConnection delegate callback, called when the connection starts to receive 
    // a response.  This checks that the response is reasonable and, if not, shuts down the 
    // connection.
{
    NSError *   error;
    
    assert(connection == self.installConnection);
    #pragma unused(connection)
    assert(response != nil);
    assert(self.installRequest != nil);
    
    // Check the HTTP status code of the response.
    
    error = nil;
    if ( [response isKindOfClass:[NSHTTPURLResponse class]] ) {
        NSHTTPURLResponse * httpResponse;
        
        httpResponse = (NSHTTPURLResponse *) response;
        
        if ( ([httpResponse statusCode] / 100) != 2) {
            error = [self installErrorWithCode:[httpResponse statusCode]];
        }
    }

    // Check the content type of the response.

    if (error == nil) {
        static NSSet * sSupportedMIMETypes;

        if (sSupportedMIMETypes == nil) {
            sSupportedMIMETypes = [[NSSet alloc] initWithObjects:@"application/x-x509-ca-cert", @"application/pkix-cert", nil];
        }
        if ( ! [sSupportedMIMETypes containsObject:[response MIMEType]] ) {
            error = [self installErrorWithCode:kInstallErrorUnsupportedMIMEType];
        }
    }
    
    // Check the expected length of the response.  We the length is unknown, use a default.  
    // If the length is too big, fail.  If we have a known length that's within our limit, 
    // create the data buffer with that length to avoid us having to resize it.
    
    if (error == nil) {
        long long       responseLength;
        
        responseLength = [response expectedContentLength];
        
        if (responseLength == NSURLResponseUnknownLength) {
            self.installData = [NSMutableData dataWithCapacity:kInstallCertificateDataDefaultLength];
        } else if (responseLength > kInstallCertificateDataMaxLength) {
            error = [self installErrorWithCode:kInstallErrorCertificateDataTooBig];
        } else {
            self.installData = [NSMutableData dataWithCapacity: (NSUInteger) responseLength];
        }
        assert( (error == nil) == (self.installData != nil) );
    }
    
    // Stop the connection if anything was bogus.
    
    if (error != nil) {
        [self installStopWithError:error];
    }
}

- (void)connection:(NSURLConnection *)connection didReceiveData:(NSData *)data
    // An NSURLConnection delegate callback, called as the connection receives data. 
    // This just appends the data to our installData buffer.
{
    assert(connection == self.installConnection);
    #pragma unused(connection)
    assert(data != nil);
    
    // If the incoming data would push us over the maximum certificate size, 
    // fail.  Otherwise just appending the incoming data to our buffer.
    
    if (([self.installData length] + [data length]) > kInstallCertificateDataMaxLength) {
        [self installStopWithError:[self installErrorWithCode:kInstallErrorCertificateDataTooBig]];
    } else {
        [self.installData appendData:data];
    }
}

- (void)connectionDidFinishLoading:(NSURLConnection *)connection
    // An NSURLConnection delegate callback, called when the connection completes 
    // successfully.  This validates the data in the data buffer and then installs 
    // it as an anchor certificate.
{
    NSError *           error;
    SecCertificateRef   anchor;
    
    assert(connection == self.installConnection);
    #pragma unused(connection)
    
    // Try to create a certificate from the data we downloaded.  If that 
    // succeeds, tell the credentials manager.
    
    error = nil;
    anchor = SecCertificateCreateWithData(NULL, (CFDataRef) self.installData);
    if (anchor == nil) {
        error = [self installErrorWithCode:kInstallErrorCertificateDataBad];
    }
    if (error == nil) {
        [[CredentialsManager sharedManager] addTrustedAnchor:anchor];
    }
    
    // Clean up the installation.  For debugging purposes only (specifically, 
    // to make it easy to see the download animation UI), you can enable a delay.
    
    if (NO) {
        [self performSelector:@selector(installStopWithError:) withObject:error afterDelay:5.0];
    } else {
        [self installStopWithError:error];
    }
    
    if (anchor != NULL) {
        CFRelease(anchor);
    }
}

- (void)connection:(NSURLConnection *)connection didFailWithError:(NSError *)error
    // An NSURLConnection delegate callback, called when the connection completes 
    // successfully.  This just stops the install, resulting in the error being 
    // displayed in the web view.
{
    assert(connection == self.installConnection);
    #pragma unused(connection)
    assert(error != nil);
    [self installStopWithError:error];
}

- (void)installStopWithError:(NSError *)error
    // Stops and cleans up the install process and:
    //
    // o if there's no error, tells the anchor install page currently being displayed 
    //   by the web view to switch to the "Installed" UI
    //
    // o if there's an error, tells the web view to display it
{
    // error may be nil
    
    [self.installConnection cancel];
    self.installConnection = nil;
    self.installRequest = nil;
    self.installData = nil;
    
    if (error == nil) {
        [self didFinishInstall];
    } else {
        [self displayError:error];
    }
}

@end
