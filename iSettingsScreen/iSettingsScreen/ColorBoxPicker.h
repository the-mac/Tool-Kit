//
//  Copyright 2009 Scott Lawrence. All rights reserved.
//  This is heavily based on UICatalog from Apple's samples
//
//  This module (and submodules) can be used for any purpose in any app
//  no warranty is expressed or implied.  If it works for you, great.
//  If it breaks, bummer.
//

#import <UIKit/UIKit.h>

@protocol ColorBoxPickerDelegate;

// Layout Type
#define kBP_Undefined		(0)
#define kBP_OneColumn		(1)
#define kBP_TwoColumns		(2)

@interface ColorBoxPicker : UIViewController /*<BLColorDisplayDelegate> */{
	id <ColorBoxPickerDelegate> delegate;
	int nColors;
	NSMutableArray * lscds;
}

@property (nonatomic, assign) id delegate;
- (id)initWithLayoutType:(int)layoutType;

- (void)setColorArray:(UIColor **)carr ofSize:(int)sz;

- (void)addViewAndTransitionIn;
- (void)transitionOutAndRemoveView:(id)sender;

@end

@protocol ColorBoxPickerDelegate<NSObject>
- (void) userSelectedInPicker:(id)sender theColor:(UIColor *)color;
@end
