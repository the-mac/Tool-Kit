//
//  Copyright 2009 Scott Lawrence. All rights reserved.
//  This is heavily based on UICatalog from Apple's samples
//
//  This module (and submodules) can be used for any purpose in any app
//  no warranty is expressed or implied.  If it works for you, great.
//  If it breaks, bummer.
//

#import "LSColorDisplay.h"
#import "LlamaSettings.h"

@implementation LSColorDisplay

@synthesize r,g,b;
//@synthesize delegate;



#pragma mark -
#pragma mark Touch checking - for changing the display box


- (void)touchesBegan:(NSSet *)touches withEvent:(UIEvent *)event
{
	press = YES;
	[self setNeedsDisplay];
	[super touchesBegan:touches withEvent:event];
}

- (void)touchesEnded:(NSSet *)touches withEvent:(UIEvent *)event
{
	press = NO;
	[self setNeedsDisplay];
	[super touchesEnded:touches withEvent:event];
}


#pragma mark -
#pragma mark do stuff when we're pressed
- (void)colorAction:(id)sender
{
	if( touchBehavior == kTouchBehavior_FullPicker ) {
		// were we in a box picker context?  If so, tell the sender we have a new color
		[(LSColorDisplay *)sender pickNewColor];
	}
	if( touchBehavior == kTouchBehavior_SelectColor ) {
		// nope. jsut a single item, send it back
//		[self.delegate colorChanged:self];
	}
}

#pragma mark -
#pragma mark initalization

- (void) initializeStuff
{
//	pool = [[NSAutoreleasePool alloc] init];
	
	[self setBackgroundColor:[UIColor clearColor]];
	[self addTarget:self action:@selector(colorAction:) forControlEvents:UIControlEventTouchUpInside];
	press = NO;
	touchBehavior = kTouchBehavior_FullPicker;
	
	// set colors for the selector - basic primaries
	[self setItem:0 Name:@"White"  ColorStr:@"1.0, 1.0, 1.0"];
	[self setItem:1 Name:@"Red"    ColorStr:@"1.0, 0.0, 0.0"];
	[self setItem:2 Name:@"Orange" ColorStr:@"1.0, 0.5, 1.0"];
	[self setItem:3 Name:@"Yellow" ColorStr:@"1.0, 1.0, 0.0"];
	[self setItem:4 Name:@"Green"  ColorStr:@"0.0, 1.0, 0.0"];
	[self setItem:5 Name:@"Cyan"   ColorStr:@"0.0, 1.0, 1.0"];
	[self setItem:6 Name:@"Blue"   ColorStr:@"0.0, 0.0, 1.0"];
	[self setItem:7 Name:@"Purple" ColorStr:@"1.0, 0.0, 1.0"];
	maxSetColors = 0;
	maxSetNames = 0;
	legend = @"Touch to pick a color";
}

- (id)initWithFrame:(CGRect)frame {
    if (self = [super initWithFrame:frame]) {
        // Initialization code
		[self initializeStuff];
    }
    return self;
}


//- (void)dealloc {
//	int x;
//	for( x=0 ; x<8 ; x++ ){
//		[aColors[x] release];
//		[aNames[x] release];
//	}
//    [super dealloc];
//}

- (void) setTouchBehavior:(int)behav
{
	touchBehavior = behav;
}

- (void) setPickerType:(int)pick
{
	pickerType = pick;
}

- (void) setLegend:(NSString *)text
{
	legend = text;
}

#pragma mark -
#pragma mark draw ourselves

#define B_INSET		(1)


- (void)drawRect:(CGRect)rect {
    // Drawing code
	CGRect drawbox = rect;
	drawbox.origin.x += B_INSET;
	drawbox.origin.y += B_INSET;
	drawbox.size.width -= B_INSET*2;
	drawbox.size.height -= B_INSET*2;

	CGContextRef theContext = UIGraphicsGetCurrentContext();
	if( [self isEnabled] ) {
		CGContextSetRGBFillColor( theContext, r, g, b, 1.0 );
	} else {
		CGContextSetRGBFillColor( theContext, (r+1.0)*3/4, (g+1.0)*3/4, (b+1.0)*3/4, 1.0 );
	}
	CGContextFillRect( theContext, drawbox );

	
	CGContextSetLineWidth( theContext, 2.0 );
	
	// if it's not enabled, just draw a box
	if( ![self isEnabled] )
	{
		CGContextSetRGBStrokeColor( theContext, 0.5, 0.5, 0.5, 0.3 );
		CGContextMoveToPoint( theContext, drawbox.origin.x, drawbox.origin.y );
		CGContextAddRect( theContext, drawbox );
		CGContextStrokePath( theContext );
		return;
	}
	
	// okay... it is enabled, draw a 3D look
	CGContextSetLineCap(theContext, kCGLineCapRound );
	
	// West and North lines
	CGContextMoveToPoint( theContext, drawbox.origin.x, drawbox.origin.y + drawbox.size.height );
	CGContextAddLineToPoint( theContext, drawbox.origin.x, drawbox.origin.y );
	CGContextAddLineToPoint( theContext, drawbox.origin.x + drawbox.size.width, drawbox.origin.y );
	if( press ) {
		CGContextSetRGBStrokeColor( theContext, 0.2, 0.2, 0.2, 1.0 );
	} else {
		CGContextSetRGBStrokeColor( theContext, 0.8, 0.8, 0.8, 1.0 );
	}
	CGContextStrokePath( theContext );
	
	// South and East lines
	
	CGContextMoveToPoint( theContext, drawbox.origin.x, drawbox.origin.y + drawbox.size.height );
	CGContextAddLineToPoint( theContext, drawbox.origin.x + drawbox.size.width, drawbox.origin.y + drawbox.size.height );
	CGContextAddLineToPoint( theContext, drawbox.origin.x + drawbox.size.width, drawbox.origin.y );
	if( !press ) {
		CGContextSetRGBStrokeColor( theContext, 0.2, 0.2, 0.2, 1.0 );
	} else {
		CGContextSetRGBStrokeColor( theContext, 0.8, 0.8, 0.8, 1.0 );
	}
	CGContextStrokePath( theContext );
	
}

- (void)setColorWithRed:(float)_r green:(float)_g blue:(float)_b
{
	r = _r;
	g = _g;
	b = _b;
	[self setNeedsDisplay];
}


- (void) setColorWith:(UIColor *)aColor
{
	const CGFloat *rgba = CGColorGetComponents( aColor.CGColor );
	r = rgba[0];
	g = rgba[1];
	b = rgba[2];
	[self setNeedsDisplay];
}

- (UIColor *) getColor
{
	return [UIColor colorWithRed:r green:g blue:b alpha:1.0];
}


#pragma mark -
#pragma mark list picker support

- (void)actionSheet:(UIActionSheet *)actionSheet didDismissWithButtonIndex:(NSInteger)buttonIndex
{
	if( buttonIndex < 0 || buttonIndex > 7 ) {
		[self setColorWithRed:0.0 green:1.0 blue:1.0];
	} else {
		[self setColorWith:aColors[buttonIndex]];
	}

//	[self.delegate colorChanged:self];
}

- (int)nItems
{
	if( maxSetNames < maxSetColors ) return maxSetNames;
	return maxSetColors;
}

- (void)doListPicker
{
	if( [self nItems] != 0 )
	{
		aNames[ [self nItems]+1 ] = nil;
	}
	UIActionSheet *actionSheet = [[UIActionSheet alloc] initWithTitle:NSLocalizedString( @"Select Color", @"" )
															 delegate:self 
													cancelButtonTitle:nil 
											   destructiveButtonTitle:nil
													otherButtonTitles:
								  aNames[0], aNames[1], aNames[2], aNames[3],
								  aNames[4], aNames[5], aNames[6], aNames[7], nil];
	actionSheet.actionSheetStyle = UIActionSheetStyleDefault;
	[actionSheet showInView:[[UIApplication sharedApplication] keyWindow]];
//	[actionSheet release];	
}

- (void)doBoxPicker:(int)columnCode
{
	cbp = [[ColorBoxPicker alloc] initWithLayoutType:columnCode];
	cbp.delegate = self;
//	[cbp setColorArray:aColors ofSize:((maxSetColors == 0)?7:maxSetColors)+1];
	[cbp addViewAndTransitionIn];
}

- (void)doTouchPicker
{
	ctp = [[ColorTouchPicker alloc] init];
	ctp.delegate = self;
	[ctp setLegend:legend];
	[ctp addViewAndTransitionIn];
}

- (void)pickNewColor
{
	if( pickerType == kPickerType_List )		[self doListPicker];
	if( pickerType == kPickerType_1Column )		[self doBoxPicker:kBP_OneColumn];
	if( pickerType == kPickerType_2Columns )	[self doBoxPicker:kBP_TwoColumns];
	if( pickerType == kPickerType_Touch )		[self doTouchPicker];
}


#pragma mark -
#pragma mark ColorBoxPickerDelegate

- (void) userSelectedInPicker:(id)sender theColor:(UIColor *)color
{
	if( !color ) return;
	[self setColorWith:color];
//	[self.delegate colorChanged:self];
	//	[pool release];
}



#pragma mark -
#pragma mark ColorTouchPickerDelegate

- (void) userTouchedInPicker:(id)sender theColor:(UIColor *)color
{
	if( !color ) return;
	[self setColorWith:color];
//	[self.delegate colorChanged:self];
	//	[pool release];
}

#pragma mark -
#pragma mark SetItem Stuff

- (void) setItem:(int)itemNo Name:(NSString *)nam
{
	if( itemNo < 0 || itemNo > 7 ) return;
//	if( aNames != nil ) [aNames[itemNo] release];
	aNames[itemNo] = nam;
//	[aNames[itemNo] retain];
	if( itemNo > maxSetNames ) maxSetNames = itemNo;
}

- (void) setItem:(int)itemNo Color:(UIColor *)col
{
	if( itemNo < 0 || itemNo > 7 ) return;
//	if( aColors != nil ) [aColors[itemNo] release];
	aColors[itemNo] = col;
//	[aColors[itemNo] retain];
	if( itemNo > maxSetColors ) maxSetColors = itemNo;
}

- (void) setItem:(int)itemNo Name:(NSString *)nam Color:(UIColor *)col
{
	[self setItem:itemNo Name:nam];
	[self setItem:itemNo Color:col];
}

- (void) setItem:(int)itemNo Name:(NSString *)nam ColorStr:(NSString *)col
{
	[self setItem:itemNo Name:nam];
	[self setItem:itemNo Color:[ColorHelper colorFromString:col]];
}
	
@end
