//
//  Copyright 2009 Scott Lawrence. All rights reserved.
//  This is heavily based on UICatalog from Apple's samples
//
//  This module (and submodules) can be used for any purpose in any app
//  no warranty is expressed or implied.  If it works for you, great.
//  If it breaks, bummer.
//

// NOTE: this should be broken out into two classes;
//		- 1. UI Color Widget - display and handle press
//		- 2. UI Color Selector (most of which should be in ColorBoxPicker

#import <UIKit/UIKit.h>
#import "ColorBoxPicker.h"
#import "ColorTouchPicker.h"

@protocol BLColorDisplayDelegate;

@interface LSColorDisplay : UIButton <UIActionSheetDelegate, ColorBoxPickerDelegate, ColorTouchPickerDelegate> {
	float r,g,b;
	id <BLColorDisplayDelegate> delegate;
	BOOL press;
	int touchBehavior;
	int pickerType;
	
	ColorTouchPicker * ctp;
	ColorBoxPicker * cbp;
	UIColor * aColors[8];
	NSString * aNames[8];
	NSString * legend;
	int maxSetColors;
	int maxSetNames;
//	NSAutoreleasePool * pool;
}
//@property (nonatomic, assign) id delegate;
@property (assign,readonly) float r;
@property (assign,readonly) float g;
@property (assign,readonly) float b;

- (void) setColorWithRed:(float)r green:(float)g blue:(float)b;
- (void) setColorWith:(UIColor *)aColor;
- (UIColor *) getColor;
- (void) pickNewColor;

#define kTouchBehavior_FullPicker	(0)
#define kTouchBehavior_SelectColor	(1)
- (void)setTouchBehavior:(int)behavior;

#define kPickerType_List			(0)
#define kPickerType_1Column			(1)
#define kPickerType_2Columns		(2)
#define kPickerType_Touch			(3)
- (void)setPickerType:(int)ptype;

- (void) setItem:(int)itemNo Name:(NSString *)nam;
- (void) setItem:(int)itemNo Color:(UIColor *)col;
- (void) setItem:(int)itemNo Name:(NSString *)nam Color:(UIColor *)col;
- (void) setItem:(int)itemNo Name:(NSString *)nam ColorStr:(NSString *)col;

- (void) setLegend:(NSString *)text;

@end

@protocol BLColorDisplayDelegate<NSObject>
- (void) colorChanged:(id)sender;
@end

