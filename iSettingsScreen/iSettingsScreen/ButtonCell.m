//
//  Copyright 2009 Scott Lawrence. All rights reserved.
//  This is heavily based on UICatalog from Apple's samples
//
//  This module (and submodules) can be used for any purpose in any app
//  no warranty is expressed or implied.  If it works for you, great.
//  If it breaks, bummer.
//

#import "ButtonCell.h"
#import "Constants.h"

#define kCellHeight	25.0

// cell identifier for this custom cell
NSString *kButtonCell_ID = @"ButtonCell_ID";

@implementation ButtonCell

@synthesize nameLabel;

- (id)initWithFrame:(CGRect)aRect reuseIdentifier:(NSString *)identifier
{
	if (self = [super initWithFrame:aRect reuseIdentifier:identifier])
	{
		// turn off selection use
		self.selectionStyle = UITableViewCellSelectionStyleBlue;
		
		nameLabel = [[UILabel alloc] initWithFrame:aRect];
		nameLabel.backgroundColor = [UIColor clearColor];
		nameLabel.opaque = NO;
		nameLabel.textColor = [UIColor darkGrayColor];
		nameLabel.highlightedTextColor = [UIColor whiteColor];
		nameLabel.font = [UIFont boldSystemFontOfSize:18];
		nameLabel.textAlignment = UITextAlignmentCenter;
		[self.contentView addSubview:nameLabel];
	}
	return self;
}

- (void)layoutSubviews
{	
	[super layoutSubviews];
    CGRect contentRect = [self.contentView bounds];
	
	// In this example we will never be editing, but this illustrates the appropriate pattern
	CGRect frame = CGRectMake(contentRect.origin.x + kCellLeftOffset, kCellTopOffset, 
							  contentRect.size.width-(kCellLeftOffset*2), kCellHeight);
	nameLabel.frame = frame;
}

//- (void)dealloc
//{
//	[nameLabel release];
//	
//    [super dealloc];
//}

- (void)setSelected:(BOOL)selected animated:(BOOL)animated
{
	[super setSelected:selected animated:animated];

	// when the selected state changes, set the highlighted state of the lables accordingly
	nameLabel.highlighted = selected;
}

@end
