### AlternateViews ###

===========================================================================
DESCRIPTION:

A sample app that demonstrates how to implement alternate or distinguishing view controllers for each
particular device orientation.  Through the help of the following UIViewController properties,
this can be easily achieved -

	@property(nonatomic,assign) UIModalTransitionStyle modalTransitionStyle;	// for a transition fade
	@property(nonatomic,assign) BOOL wantsFullScreenLayout;   // for any view controller to appear over another

This sample implements two different view controllers: one for portrait and one for landscape.
The portrait view controller listens for device orientations in order to properly swap in and out the
landscape view controller.  It uses the above two properties to achieve the visual cross-fade effect.


===========================================================================
USING THE SAMPLE:

When launched, notice the view says "Portrait".  Rotate the device to 
landscape right or landscape left positions and the view changes to the 
alternate one supporting landscape.

===========================================================================
BUILD REQUIREMENTS:

iOS 6 SDK or later


===========================================================================
RUNTIME REQUIREMENTS:

iOS 4.3 or later


===========================================================================
PACKAGING LIST:

main.m - 
    Main source file for this sample.

CustomNavigationController.{h/m}
    UINavigationController subclass that forwards queries about its
    supported interface orientations to its child view controllers.

AppDelegate.{h/m} - 
    The application's delegate.

PortraitViewController.{h/m} - 
    The secondary UIViewController for portrait device orientation.

LandscapeViewController.{h/m} - 
    The main UIViewController for landscape device orientation.


===========================================================================
CHANGES FROM PREVIOUS VERSIONS:

Version 1.0
- First version.

Version 1.1
- Upgraded project to build with the iOS 4.0 SDK.

Version 1.2
- Updated project to build with the iOS SDK 6.
- Deployment target set to iOS 4.3.
- Now uses ARC.
- Included launch images and missing retina versions of certain icons.


===========================================================================
Copyright (C) 2009-2013 Apple Inc. All rights reserved.