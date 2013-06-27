//
//  Copyright 2009 Scott Lawrence. All rights reserved.
//  This is heavily based on UICatalog from Apple's samples
//
//  This module (and submodules) can be used for any purpose in any app
//  no warranty is expressed or implied.  If it works for you, great.
//  If it breaks, bummer.
//

#import "SourceCell.h"
#import "Constants.h"

// cell identifier for this custom cell
NSString *kSourceCell_ID = @"SourceCell_ID";

@implementation SourceCell

@synthesize sourceLabel;

- (id)initWithFrame:(CGRect)aRect reuseIdentifier:(NSString *)identifier
{
	if (self = [super initWithFrame:aRect reuseIdentifier:identifier])
	{
		// turn off selection use
		self.selectionStyle = UITableViewCellSelectionStyleNone;
		
		sourceLabel = [[UILabel alloc] initWithFrame:aRect];
		sourceLabel.backgroundColor = [UIColor clearColor];
		sourceLabel.opaque = NO;
		sourceLabel.textAlignment = UITextAlignmentCenter;
		sourceLabel.textColor = [UIColor grayColor];
		sourceLabel.highlightedTextColor = [UIColor blackColor];
		sourceLabel.font = [UIFont systemFontOfSize:12];
		
		[self.contentView addSubview:sourceLabel];
	}
	return self;
}

- (void)layoutSubviews
{
	[super layoutSubviews];
	
	sourceLabel.frame = [self.contentView bounds];
}

//- (void)dealloc
//{
//	[sourceLabel release];
//	
//    [super dealloc];
//}

@end
