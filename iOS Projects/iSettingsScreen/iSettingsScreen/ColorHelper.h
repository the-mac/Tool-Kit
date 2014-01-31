//
//  Copyright 2009 Scott Lawrence. All rights reserved.
//  This is heavily based on UICatalog from Apple's samples
//
//  This module (and submodules) can be used for any purpose in any app
//  no warranty is expressed or implied.  If it works for you, great.
//  If it breaks, bummer.
//

#import <Foundation/Foundation.h>


@interface ColorHelper : NSObject {

}

// String Methods /////////////////////////////////////////////////////

+ (float) stringValueToFloat:(NSString *)v;  // converts 0.5 or 127(/255) to 0.5

+ (NSString *)stringFromColor:(UIColor *)aColor; // generates "0.5 0.5 0.5" (R G B) from a UIColor
+ (UIColor *)colorFromString:(NSString *)aColor; // converts "0.5 0.5 0.5" or "127 127 127" to a UIColor


// Color Modification Methods /////////////////////////////////////////

+ (UIColor *)darker:(UIColor *)startColor;
+ (UIColor *)lighter:(UIColor *)startColor;

@end
