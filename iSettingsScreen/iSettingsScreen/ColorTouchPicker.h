//
//  Copyright 2009 Scott Lawrence. All rights reserved.
//  This is heavily based on UICatalog from Apple's samples
//
//  This module (and submodules) can be used for any purpose in any app
//  no warranty is expressed or implied.  If it works for you, great.
//  If it breaks, bummer.
//

#import <UIKit/UIKit.h>

@protocol ColorTouchPickerDelegate;


@interface ColorTouchPicker : UIViewController {
	id <ColorTouchPickerDelegate> delegate;
	UILabel * legendLabel;
	UIColor * theColor;
}

@property (nonatomic, assign) id delegate;

- (void)addViewAndTransitionIn;
- (void)transitionOutAndRemoveView:(id)sender;

- (void) setLegend:(NSString *)text;

@end

@protocol ColorTouchPickerDelegate<NSObject>
- (void) userTouchedInPicker:(id)sender theColor:(UIColor *)color;
@end
