//
//  StringHelper.m
//  PTLog
//
//  Created by Ellen Miner on 1/2/09.
//  Copyright 2009 RaddOnline. All rights reserved.
//

#import "StringHelper.h"


@implementation NSString (StringHelper)

#pragma mark Methods to determine the height of a string for resizeable table cells
- (CGFloat)RAD_textHeightForSystemFontOfSize:(CGFloat)size {
	//Calculate the expected size based on the font and linebreak mode of your label
	CGFloat maxWidth = [UIScreen mainScreen].bounds.size.width - 50;
	CGFloat maxHeight = 9999;
	CGSize maximumLabelSize = CGSizeMake(maxWidth,maxHeight);
	
	CGSize expectedLabelSize = [self sizeWithFont:[UIFont systemFontOfSize:size] 
								   constrainedToSize:maximumLabelSize 
									   lineBreakMode:UILineBreakModeWordWrap]; 
	
	return expectedLabelSize.height;
}

- (CGRect)RAD_frameForCellLabelWithSystemFontOfSize:(CGFloat)size {
	CGFloat width = [UIScreen mainScreen].bounds.size.width - 50;
	CGFloat height = [self RAD_textHeightForSystemFontOfSize:size] + 10.0;
	return CGRectMake(10.0f, 10.0f, width, height);
}

- (void)RAD_resizeLabel:(UILabel *)aLabel WithSystemFontOfSize:(CGFloat)size {
	aLabel.frame = [self RAD_frameForCellLabelWithSystemFontOfSize:size];
	aLabel.text = self;
	[aLabel sizeToFit];
}

- (UILabel *)RAD_newSizedCellLabelWithSystemFontOfSize:(CGFloat)size {
	UILabel *cellLabel = [[UILabel alloc] initWithFrame:[self RAD_frameForCellLabelWithSystemFontOfSize:size]];
	cellLabel.textColor = [UIColor blackColor];
	cellLabel.backgroundColor = [UIColor clearColor];
	cellLabel.textAlignment = UITextAlignmentLeft;
	cellLabel.font = [UIFont systemFontOfSize:size];
	
	cellLabel.text = self; 
	cellLabel.numberOfLines = 0; 
	[cellLabel sizeToFit];
	return cellLabel;
}

@end
