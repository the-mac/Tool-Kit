/*
     File: PortraitViewController.m
 Abstract: The application view controller used when the device is in portrait orientation.
  Version: 1.2
 
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

#import "PortraitViewController.h"
#import "LandscapeViewController.h"

@interface PortraitViewController ()

// For efficiency, an instance of LandscapeViewController is initialized
// once.  This instance is then reused as needed.
@property (nonatomic, strong) LandscapeViewController *landscapeViewController;

@end


@implementation PortraitViewController

// -------------------------------------------------------------------------------
//	awakeFromNib
// -------------------------------------------------------------------------------
- (void)awakeFromNib
{
    // Instruct the system to generate notifications when the device orientation
    // changes.  This view controller will not receive calls to
    // willRotateToInterfaceOrientation:duration: and
    // didRotateFromInterfaceOrientation: because the system will never attempt
    // to rotate this view controller.  Thus, orientation notifications are the only
    // way to know the orientation changed.
	[[UIDevice currentDevice] beginGeneratingDeviceOrientationNotifications];
	[[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(orientationChanged:)
                                                 name:UIDeviceOrientationDidChangeNotification
                                               object:nil];
}

// -------------------------------------------------------------------------------
//	orientationChanged:
//  Handler for the UIDeviceOrientationDidChangeNotification.
//  See also: awakeFromNib
// -------------------------------------------------------------------------------
- (void)orientationChanged:(NSNotification *)notification
{
    // A delay must be added here, otherwise the new view will be swapped in 
	// too quickly resulting in an animation glitch
    [self performSelector:@selector(updateLandscapeView) withObject:nil afterDelay:0];
}

// -------------------------------------------------------------------------------
//	updateLandscapeView
//  This method contains the logic for presenting and dismissing the
//  LandscapeViewController depending on the current device orientation and
//  whether the LandscapeViewController is presently presented.
//  See also: orientationChanged:
// -------------------------------------------------------------------------------
- (void)updateLandscapeView
{
    // Get the device's current orientation.  By the time the
    // UIDeviceOrientationDidChangeNotification has been posted, this value reflects
    // the new orientation of the device.
    UIDeviceOrientation deviceOrientation = [UIDevice currentDevice].orientation;
    
    if (UIDeviceOrientationIsLandscape(deviceOrientation) && self.presentedViewController == nil)
    // Only take action if the orientation is landscape and presentedViewController is nil (no
    // view controller is presented).  The later check prevents this view controller from trying
    // to present landscapeViewController again if the device rotates from landscape to landscape
    // (the user turns the device 180 degrees).
	{
        // landscapeViewController only should be initialized once.  Subsequent presentations
        // will reuse the same instance.  This improves performance.  Addtionally,
        // landscapeViewController is initialized lazily (when needed).  This is also done
        // for performance.
        if (!self.landscapeViewController)
            self.landscapeViewController = [[LandscapeViewController alloc] initWithNibName:@"LandscapeView" bundle:nil];
        
        // presentViewController:animated:completion: is the modern API for displaying
        // a modal view controller.  However, it is not supported under iOS 4.  In that case,
        // fall back to the old presentModalViewController:animated: API.
        if ([self respondsToSelector:@selector(presentViewController:animated:completion:)])
            [self presentViewController:self.landscapeViewController animated:YES completion:NULL];
        else
            [self presentModalViewController:self.landscapeViewController animated:YES];
    }
	else if (deviceOrientation == UIDeviceOrientationPortrait && self.presentedViewController != nil)
    // Only take action if the orientation is portrait and presentedViewController is not nil (a
    // view controller is presented).
	{
        // dismissViewControllerAnimated:completion: is the modern API for dismissing
        // a modal view controller.  However, it is not supported under iOS 4.  In that case,
        // fall back to the old dismissModalViewControllerAnimated: API.
        if ([self respondsToSelector:@selector(dismissViewControllerAnimated:completion:)])
            [self dismissViewControllerAnimated:YES completion:NULL];
        else
            [self dismissModalViewControllerAnimated:YES];
    }    
}

#pragma mark - Rotation

// -------------------------------------------------------------------------------
//	supportedInterfaceOrientations
//  Support only portrait orientation (iOS 6).
// -------------------------------------------------------------------------------
- (NSUInteger)supportedInterfaceOrientations
{
    return UIInterfaceOrientationMaskPortrait;
}

// -------------------------------------------------------------------------------
//	shouldAutorotateToInterfaceOrientation
//  Support only portrait orientation (IOS 5 and below).
// -------------------------------------------------------------------------------
- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}

#pragma mark - Memory management

// -------------------------------------------------------------------------------
//	dealloc
// -------------------------------------------------------------------------------
- (void)dealloc
{
    [[NSNotificationCenter defaultCenter] removeObserver:self];
    
    // Instruct the system to stop generating device orientation notifications.
    [[UIDevice currentDevice] endGeneratingDeviceOrientationNotifications];
}

@end

