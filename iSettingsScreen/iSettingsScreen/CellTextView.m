//
//  Copyright 2009 Scott Lawrence. All rights reserved.
//  This is heavily based on UICatalog from Apple's samples
//
//  This module (and submodules) can be used for any purpose in any app
//  no warranty is expressed or implied.  If it works for you, great.
//  If it breaks, bummer.
//

#import "CellTextView.h"
#import "Constants.h"

// cell identifier for this custom cell
NSString* kCellTextView_ID = @"CellTextView_ID";

#define kInsertValue	8.0

@implementation CellTextView

@synthesize view;

- (id)initWithFrame:(CGRect)aRect /*reuseIdentifier:(NSString *)identifier*/
{
	self = [super initWithFrame:aRect /*reuseIdentifier:identifier*/];
	if (self)
	{
		// turn off selection use
		self.selectionStyle = UITableViewCellSelectionStyleNone;
	}
	return self;
}

- (void)setView:(UITextView *)inView
{
	view = inView;
//	[self.view retain];
	[self.contentView addSubview:inView];
	[self layoutSubviews];
}

- (void)layoutSubviews
{
	[super layoutSubviews];
	
	CGRect contentRect = [self.contentView bounds];
	
	// inset the text view within the cell
	if (contentRect.size.width > (kInsertValue*2))	// but not if the width is too small
	{
		self.view.frame  = CGRectMake(contentRect.origin.x + kInsertValue,
									  contentRect.origin.y + kInsertValue,
									  contentRect.size.width - (kInsertValue*2),
									  contentRect.size.height - (kInsertValue*2));
	}
}

//- (void)dealloc
//{
//    [view release];
//    [super dealloc];
//}

@end
