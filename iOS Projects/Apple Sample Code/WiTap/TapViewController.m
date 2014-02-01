/*
     File: TapViewController.m
 Abstract: Controls the main tap view.
  Version: 2.0
 
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

#import "TapViewController.h"

#import "TapView.h"

#include <AssertMacros.h>

@interface TapViewController () <TapViewDelegate>

@end

@implementation TapViewController

- (id)init
{
    self = [super initWithNibName:nil bundle:nil];
    if (self != nil) {
        // do nothing
    }
    return self;
}

- (void)loadCloseButton
{
    UIButton *  closeButton;
    
    closeButton = [[UIButton alloc] initWithFrame:CGRectMake(0.0f, 0.0f, 32.0f, 32.0f)];
    closeButton.contentMode = UIViewContentModeCenter;
    closeButton.autoresizingMask = UIViewAutoresizingFlexibleRightMargin | UIViewAutoresizingFlexibleBottomMargin;
    [closeButton setImage:[UIImage imageNamed:@"cross24"] forState:UIControlStateNormal];
    [closeButton addTarget:self action:@selector(closeButtonAction:) forControlEvents:UIControlEventTouchUpInside];
    [self.view addSubview:closeButton];
}

enum {
    kTapViewControllerTapsPerRow = 3
};

// The tap count must be an even multiple of the taps per row otherwise this is never 
// going to work.

check_compile_time((kTapViewControllerTapItemCount % kTapViewControllerTapsPerRow) == 0);

- (void)loadView
{
    // Creates our main view and the tap views embedded within it.  This only creates 
    // the views; the layout is done in viewDidLayoutSubviews.
    
    self.view = [[UIView alloc] initWithFrame:CGRectZero];
    for (NSUInteger x = 0; x < kTapViewControllerTapsPerRow; x++) {
        for (NSUInteger y = 0; y < (kTapViewControllerTapItemCount / kTapViewControllerTapsPerRow); y++) {
            TapView *   tapView;
            
            tapView = [[TapView alloc] initWithFrame:CGRectZero];
            tapView.backgroundColor = [UIColor colorWithHue:((y * kTapViewControllerTapsPerRow + x) / (CGFloat) kTapViewControllerTapItemCount) 
                saturation:0.75 
                brightness:0.75 
                alpha:1.0
            ];
            tapView.tag = (NSInteger) (y * kTapViewControllerTapsPerRow + x + 1);
            tapView.delegate = self;
            [self.view addSubview:tapView];
        }
    }
    
    [self loadCloseButton];
}

- (void)viewDidLayoutSubviews
{
    CGRect      frame;
    
    [super viewDidLayoutSubviews];

    // Lays out the tap views in a grid.
    
    frame = self.view.bounds;
    frame.size.width  /= (CGFloat) kTapViewControllerTapsPerRow;
    frame.size.height /= (CGFloat) kTapViewControllerTapsPerRow;
    for (NSUInteger x = 0; x < kTapViewControllerTapsPerRow; x++) {
        for (NSUInteger y = 0; y < (kTapViewControllerTapItemCount / kTapViewControllerTapsPerRow); y++) {
            [self.view viewWithTag:(NSInteger) (y * kTapViewControllerTapsPerRow + x + 1)].frame = CGRectMake(
                frame.origin.x + x * frame.size.width, 
                frame.origin.y + y * frame.size.height, 
                frame.size.width,
                frame.size.height
            );
        }
    }
    
    // We don't need to position the close button because the autosizing masks do 
    // what we need.
}

- (void)closeButtonAction:(id)sender
{
    #pragma unused(sender)
    if ([self.delegate respondsToSelector:@selector(tapViewControllerDidClose:)]) {
        [self.delegate tapViewControllerDidClose:self];
    }
}

#pragma mark * API exposed to our clients

- (void)remoteTouchDownOnItem:(NSUInteger)tapItemIndex
{
    assert(tapItemIndex < kTapViewControllerTapItemCount);
    if (self.isViewLoaded) {
        ((TapView *)[self.view viewWithTag:tapItemIndex + 1]).remoteTouch = YES;
    }
}

- (void)remoteTouchUpOnItem:(NSUInteger)tapItemIndex
{
    assert(tapItemIndex < kTapViewControllerTapItemCount);
    if (self.isViewLoaded) {
        ((TapView *)[self.view viewWithTag:tapItemIndex + 1]).remoteTouch = NO;
    }
}

- (void)resetTouches
{
    for (NSInteger tag = 1; tag <= kTapViewControllerTapItemCount; tag++) {
        TapView *   tapView;
        
        tapView = ((TapView *) [self.view viewWithTag:tag]);
        assert([tapView isKindOfClass:[TapView class]]);
        [tapView resetTouches];
    }
}

#pragma mark * Tap view delegate callbacks

- (void)tapViewLocalTouchDown:(TapView *)tapView
{
    if ([self.delegate respondsToSelector:@selector(tapViewController:localTouchDownOnItem:)]) {
        assert(tapView.tag != 0);
        assert(tapView.tag <= kTapViewControllerTapItemCount);
        [self.delegate tapViewController:self localTouchDownOnItem:(NSUInteger) ([tapView tag] - 1)];
    }
}

- (void)tapViewLocalTouchUp:(TapView *)tapView
{
    if ([self.delegate respondsToSelector:@selector(tapViewController:localTouchUpOnItem:)]) {
        assert(tapView.tag != 0);
        assert(tapView.tag <= kTapViewControllerTapItemCount);
        [self.delegate tapViewController:self localTouchUpOnItem:(NSUInteger) ([tapView tag] - 1)];
    }
}

@end
