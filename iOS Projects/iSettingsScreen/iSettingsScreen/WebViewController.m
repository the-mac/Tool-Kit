//
//  Copyright 2009 Scott Lawrence. All rights reserved.
//  This is heavily based on UICatalog from Apple's samples
//
//  This module (and submodules) can be used for any purpose in any app
//  no warranty is expressed or implied.  If it works for you, great.
//  If it breaks, bummer.
//

#import "WebViewController.h"
#import "Constants.h"

@implementation WebViewController

@synthesize waitUntilLoad;
@synthesize blackScreenUntilLoad;
@synthesize displayLater;
@synthesize theNavigationBar;

- (id)init
{
	self = [super init];
	if (self)
	{
		// this title will appear in the navigation bar
		self.title = NSLocalizedString(@"WebTitle", @"");
	}
	return self;
}

//- (void)dealloc
//{
//	[myWebView release];
//	[urlField release];
//	
//	[super dealloc];
//}


- (void) fadeInDisplay
{
	[UIView beginAnimations:@"fadein" context:nil];
	[UIView setAnimationBeginsFromCurrentState:YES];
	[UIView setAnimationDuration:0.3];
	[myWebView setAlpha:1.0];
	[UIView commitAnimations];
}

- (void) blankTheDisplay
{
	[myWebView setAlpha:0.0];
}

- (void)loadView:(BOOL)rst withTitle:(NSString *)title
{
	// the base view for this view controller
	UIView *contentView = [[UIView alloc] initWithFrame:[[UIScreen mainScreen] applicationFrame]];
	contentView.backgroundColor = [UIColor whiteColor];
	
	// important for view orientation rotation
	contentView.autoresizesSubviews = YES;
	contentView.autoresizingMask = (UIViewAutoresizingFlexibleWidth | UIViewAutoresizingFlexibleHeight);	
	
	self.view = contentView;
	restrictive = rst;
//	[contentView release];
	
	// now fill it...
	UINavigationBar *aNavigationBar = [[UINavigationBar alloc] initWithFrame:CGRectMake(0.0, 0.0, 320.0, 44.0)];
    aNavigationBar.barStyle = UIBarStyleBlackOpaque;
    self.theNavigationBar = aNavigationBar;
//	[self.view addSubview:aNavigationBar];

//    [aNavigationBar release];
	
	UIBarButtonItem *buttonItem = [[UIBarButtonItem alloc] initWithBarButtonSystemItem:UIBarButtonSystemItemDone
																				target:self 
																				action:@selector(transitionOutAndRemoveView:)];
    UINavigationItem *navigationItem = [[UINavigationItem alloc] initWithTitle:title];
    navigationItem.rightBarButtonItem = buttonItem;
    [theNavigationBar pushNavigationItem:navigationItem animated:NO];
	theNavigationBar.autoresizingMask = UIViewAutoresizingFlexibleWidth;
//    [navigationItem release];
//    [buttonItem release];
	

	CGRect webFrame = [[UIScreen mainScreen] applicationFrame];
	webFrame.origin.y += 44;	// leave from the URL input field and its label
	webFrame.size.height -= 44.0;
	myWebView = [[UIWebView alloc] initWithFrame:webFrame];
	myWebView.backgroundColor = [UIColor blackColor];
//	myWebView.scalesPageToFit = YES;
	myWebView.autoresizingMask = (UIViewAutoresizingFlexibleWidth | UIViewAutoresizingFlexibleHeight);
	myWebView.delegate = self;
	if( blackScreenUntilLoad ) 
	{
		[self blankTheDisplay];
	}
	
	/*
	CGRect textFieldFrame = CGRectMake(kLeftMargin, kTweenMargin, self.view.bounds.size.width - (kLeftMargin * 2.0), kTextFieldHeight);
	urlField = [[UITextField alloc] initWithFrame:textFieldFrame];
    urlField.borderStyle = UITextBorderStyleBezel;
    urlField.textColor = [UIColor blackColor];
    urlField.delegate = self;
    urlField.placeholder = @"<enter a URL>";
    urlField.text = urlString;
	urlField.backgroundColor = [UIColor whiteColor];
	urlField.autoresizingMask = UIViewAutoresizingFlexibleWidth;
	urlField.returnKeyType = UIReturnKeyGo;
	urlField.keyboardType = UIKeyboardTypeURL;	// this makes the keyboard more friendly for typing URLs
	urlField.autocorrectionType = UITextAutocorrectionTypeNo;	// we don't like autocompletion while typing
	urlField.clearButtonMode = UITextFieldViewModeAlways;
	if( !restrictive ) {
		[self.view addSubview:urlField];
	}
	 */
	// now, layer everything together
	[self.view addSubview:myWebView];
	[self.view addSubview:theNavigationBar];
	
	if( !displayLater  &&  !waitUntilLoad )
		[self addViewAndTransitionIn];

}

- (void)loadURL:(NSString *)newURLString
{
	//XXXX NOTE: THIS IS CHANGED TO FORCE EXTERNAL LINK LOADING.  THIS SHOULD BE SELECTABLE IN THE .PLIST!!!
	[myWebView loadRequest:[NSURLRequest requestWithURL:[NSURL URLWithString:newURLString]]];
	
	//[[UIApplication sharedApplication] openURL:[NSURL URLWithString:newURLString]];

}

- (void)loadLocalFile:(NSString *)filepath
{
	NSError * e;
	NSString * filedata = [NSString stringWithContentsOfFile:filepath encoding:NSASCIIStringEncoding error:&e];
	[myWebView loadHTMLString:filedata baseURL:[NSURL fileURLWithPath:[[NSBundle mainBundle] bundlePath] isDirectory:YES]];
}


//- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
//{
	// we support rotation in this view controller
//	return NO;
//}

// this helps dismiss the keyboard when the "Done" button is clicked
- (BOOL)textFieldShouldReturn:(UITextField *)textField
{
	[textField resignFirstResponder];
	[myWebView loadRequest:[NSURLRequest requestWithURL:[NSURL URLWithString:[textField text]]]];
	
	return YES;
}


#pragma mark UIWebView delegate methods

- (BOOL)webView:(UIWebView *)webView shouldStartLoadWithRequest:(NSURLRequest *)request navigationType:(UIWebViewNavigationType)navigationType
{
	// force external loading of embedded links
	if( [[[[request URL] scheme] uppercaseString]isEqualToString:@"FILE"] ) {
		return YES;
	}
	[[UIApplication sharedApplication] openURL:[request URL]];
	return NO;
}

- (void)webViewDidStartLoad:(UIWebView *)webView
{
	// starting the load, show the activity indicator in the status bar
/// 3.0	[UIApplication sharedApplication].isNetworkActivityIndicatorVisible = YES;
}

- (void)webViewDidFinishLoad:(UIWebView *)webView
{
	// finished loading, hide the activity indicator in the status bar
/// 3.0	[UIApplication sharedApplication].isNetworkActivityIndicatorVisible = NO;
	if( displayLater ) return;
	
	if( waitUntilLoad )
	{
		[self addViewAndTransitionIn];
	}
	
	if( blackScreenUntilLoad )
	{
		[self fadeInDisplay];
	}
}


- (void)webView:(UIWebView *)webView didFailLoadWithError:(NSError *)error 
{
	// load error, hide the activity indicator in the status bar
/// 3.0 	[UIApplication sharedApplication].isNetworkActivityIndicatorVisible = NO;

	// report the error inside the webview
	NSString* errorString = [NSString stringWithFormat:
							 @"<html><center><font size=+5 color='red'>An error occurred:<br>%@</font></center></html>",
							 error.localizedDescription];
	[myWebView loadHTMLString:errorString baseURL:nil];
}


- (void)addViewAndTransitionIn
{
	// just in cases
	CGPoint oldCenter = [self.view center];
	oldCenter.x = 160;
	self.view.center = oldCenter;
	
	// and the new one
	CGPoint newCenter = [self.view center];
	newCenter.x += 320;
	[self.view setCenter:newCenter];
	
	[[[UIApplication sharedApplication] keyWindow] addSubview:self.view];
	
	[UIView beginAnimations:@"FadeInWebview" context:nil];
	[UIView setAnimationBeginsFromCurrentState:YES];
	[UIView setAnimationDuration:0.3];
	[UIView setAnimationCurve:UIViewAnimationCurveEaseIn];
	[self.view setCenter:oldCenter];
	[UIView commitAnimations];
}

- (void)transitionOutAndRemoveView:(id)sender
{
	CGPoint newCenter = [self.view center];
	newCenter.x += 320;

	[UIView beginAnimations:@"FadeOutWebview" context:nil];
	[UIView setAnimationBeginsFromCurrentState:YES];
	[UIView setAnimationDelegate:self.view];
	[UIView setAnimationDidStopSelector:@selector( removeFromSuperview )];
	[UIView setAnimationDuration:0.3];
	[UIView setAnimationCurve:UIViewAnimationCurveEaseIn];
	//[self.view setAlpha:0.0];
	[self.view setCenter:newCenter];
	[UIView commitAnimations];
}

@end

