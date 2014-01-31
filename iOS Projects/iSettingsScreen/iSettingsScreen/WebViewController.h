//
//  Copyright 2009 Scott Lawrence. All rights reserved.
//  This is heavily based on UICatalog from Apple's samples
//
//  This module (and submodules) can be used for any purpose in any app
//  no warranty is expressed or implied.  If it works for you, great.
//  If it breaks, bummer.
//

#import <UIKit/UIKit.h>

@interface WebViewController : UIViewController <UITextFieldDelegate, UIWebViewDelegate>
{
	UINavigationBar *theNavigationBar;
	UIWebView	*myWebView;
	UITextField *urlField;
	NSString *urlString;
	BOOL restrictive;
	BOOL waitUntilLoad;
	BOOL blackScreenUntilLoad;
	BOOL displayLater;
}

@property (nonatomic, retain) UINavigationBar *theNavigationBar;
@property BOOL waitUntilLoad;
@property BOOL blackScreenUntilLoad;
@property BOOL displayLater;

- (void)loadView:(BOOL)rst withTitle:(NSString *)title;
- (void)loadURL:(NSString *)newURLString;
- (void)loadLocalFile:(NSString *)filepath;

- (void)addViewAndTransitionIn;
- (void)transitionOutAndRemoveView:(id)sender;

@end
