//
//  Copyright 2009 Scott Lawrence. All rights reserved.
//  This is heavily based on UICatalog from Apple's samples
//
//  This module (and submodules) can be used for any purpose in any app
//  no warranty is expressed or implied.  If it works for you, great.
//  If it breaks, bummer.
//

#import "ColorHelper.h"


@implementation ColorHelper




#pragma mark -
#pragma mark UIColor string functions


+ (float) stringValueToFloat:(NSString *)v
{
	NSArray *dotcomponents = [v componentsSeparatedByString:@"."];
	
	if( [dotcomponents count] == 2 ) {
		// float
		return [v floatValue];
	} 
	return ((float)[v intValue]) / 255.0;
}


+ (NSString *)stringFromColor:(UIColor *)aColor
{
	const CGFloat *rgba = CGColorGetComponents( aColor.CGColor );
	if( !rgba ) return @"0.5, 0.5, 0.5";
	
	return [NSString stringWithFormat:@"%0.3f, %0.3f, %0.3f",  rgba[0], rgba[1], rgba[2]];
}


+ (UIColor *)colorFromString:(NSString *)aColor
{
	if( !aColor ) return[UIColor cyanColor];
	
	// Separate into components by removing commas and spaces  NSString
    NSArray *components = [aColor componentsSeparatedByString:@", "];  
    if ([components count] != 3) return [UIColor blueColor];
	
	// adjust if the color is float [0.0,1.0]  or int [0,255]
	float Fred   = [ColorHelper stringValueToFloat:[components objectAtIndex:0]];
	float Fgreen = [ColorHelper stringValueToFloat:[components objectAtIndex:1]];
	float Fblue  = [ColorHelper stringValueToFloat:[components objectAtIndex:2]];
	
    // Create the color  
    return [UIColor colorWithRed:Fred green:Fgreen blue:Fblue alpha:1.0]; 
}


#pragma mark -
#pragma mark UIColor intensity functions

+ (UIColor *)darker:(UIColor *)startColor
{
	if( !startColor ) return nil;
	
	const CGFloat *rgba = CGColorGetComponents( startColor.CGColor );
	if( !rgba ) return nil;

	
	UIColor * darkerColor = [[UIColor alloc] initWithRed:(rgba[0]/2.0)
												   green:(rgba[1]/2.0)
													blue:(rgba[2]/2.0) 
												   alpha:rgba[3] ];
	return darkerColor;
}

+ (UIColor *)lighter:(UIColor *)startColor
{
	if( !startColor ) return nil;
	
	const CGFloat *rgba = CGColorGetComponents( startColor.CGColor );
	if( !rgba ) return nil;
	
	
	UIColor * lighterColor = [[UIColor alloc] initWithRed:((rgba[0] + 1.0)/2.0)
													green:((rgba[1] + 1.0)/2.0)
													 blue:((rgba[2] + 1.0)/2.0) 
													alpha:rgba[3] ];
	return lighterColor;
}

@end
