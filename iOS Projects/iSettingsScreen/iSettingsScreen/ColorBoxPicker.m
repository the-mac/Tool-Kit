//
//  Copyright 2009 Scott Lawrence. All rights reserved.
//  This is heavily based on UICatalog from Apple's samples
//
//  This module (and submodules) can be used for any purpose in any app
//  no warranty is expressed or implied.  If it works for you, great.
//  If it breaks, bummer.
//

#import "LSColorDisplay.h"
#import "ColorBoxPicker.h"


@interface ColorBoxPicker ()
- (void) buildUI_OneColumn;
- (void) buildUI_TwoColumns;
@end


@implementation ColorBoxPicker

//@synthesize delegate;

/*
// The designated initializer. Override to perform setup that is required before the view is loaded.
- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil {
    if (self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil]) {
        // Custom initialization
    }
    return self;
}
*/

/*
// Implement loadView to create a view hierarchy programmatically, without using a nib.
- (void)loadView {
}
*/

/*
// Implement viewDidLoad to do additional setup after loading the view, typically from a nib.
- (void)viewDidLoad {
    [super viewDidLoad];
}
*/

/*
// Override to allow orientations other than the default portrait orientation.
- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation {
    // Return YES for supported orientations
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}
*/


#pragma mark -
#pragma mark touches to pick area


- (void)touchesBegan:(NSSet *)touches withEvent:(UIEvent *)event
{
	CGPoint startTouchPosition;
	UITouch *touch = [touches anyObject];
    startTouchPosition = [touch locationInView:self.view];
}

- (void)touchesMoved:(NSSet *)touches withEvent:(UIEvent *)event
{
    UITouch *touch = [touches anyObject];
    CGPoint currentTouchPosition;
	currentTouchPosition = [touch locationInView:self.view];
}

- (void)touchesEnded:(NSSet *)touches withEvent:(UIEvent *)event
{
    UITouch *touch = [touches anyObject];
    CGPoint currentTouchPosition;
	currentTouchPosition = [touch locationInView:self.view];
}


#pragma mark -
#pragma mark class setup

- (id)initWithLayoutType:(int)layoutType
{
	self = [super init];
	if (self)
	{
		// this title will appear in the navigation bar
		self.title = NSLocalizedString(@"Colors", @"");
		if( layoutType == kBP_OneColumn ) [self buildUI_OneColumn];
		else [self buildUI_TwoColumns];
		nColors = 0;
	}
	return self;
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning]; // Releases the view if it doesn't have a superview
    // Release anything that's not essential, such as cached data
}


//- (void)dealloc {
//	for( int x=0 ; x<nColors ; x++ )
//	{
//		[lscds release];
//	}
//    [super dealloc];
//}

#pragma mark -
#pragma mark gui and live setup


#define kPaddingSides		(10)
#define kPaddingTop			(10)
#define kPaddingCenter		(10)
#define kPaddingtonBear		(10)
#define kBoxHeight			(100)
#define kBoxPaddingVertical	(10)

// THIS IS A HACK FOR NOW.  I'LL CLEAN IT UP LATER.
// SORRY ABOUT THIS.

- (void) buildUI_OneColumn
{
	UIColor * bgcolor = [UIColor colorWithRed:0.0 green:0.0 blue:0.0 alpha:1.0];
	[self.view setBackgroundColor:bgcolor];
	int vsize = ((self.view.frame.size.height - (kPaddingTop * 2)) - (kBoxPaddingVertical * 7))/8;
	
	lscds = [NSMutableArray array];	LSColorDisplay * lscd;
	UIColor * col;
		
	CGRect frm = CGRectMake( kPaddingSides, kPaddingTop, 
							self.view.frame.size.width - (2 * kPaddingCenter), vsize );
	
	col = [UIColor redColor];
	
	lscd = [[LSColorDisplay alloc] initWithFrame:frm];
	//lscd.delegate = self;
	[lscd setTouchBehavior:kTouchBehavior_SelectColor];
	[lscds addObject:lscd];
	[self.view addSubview:lscd];
	
	frm.origin.y += (vsize + kBoxPaddingVertical);
	lscd = [[LSColorDisplay alloc] initWithFrame:frm];
	//lscd.delegate = self;
	[lscd setTouchBehavior:kTouchBehavior_SelectColor];
	[lscds addObject:lscd];
	[self.view addSubview:lscd];
	
	frm.origin.y += (vsize + kBoxPaddingVertical);
	lscd = [[LSColorDisplay alloc] initWithFrame:frm];
	//lscd.delegate = self;
	[lscd setTouchBehavior:kTouchBehavior_SelectColor];
	[lscds addObject:lscd];
	[self.view addSubview:lscd];
	
	frm.origin.y += (vsize + kBoxPaddingVertical);
	lscd = [[LSColorDisplay alloc] initWithFrame:frm];
	//lscd.delegate = self;
	[lscd setTouchBehavior:kTouchBehavior_SelectColor];
	[lscds addObject:lscd];
	[self.view addSubview:lscd];
	
	frm.origin.y += (vsize + kBoxPaddingVertical);
	lscd = [[LSColorDisplay alloc] initWithFrame:frm];
	//lscd.delegate = self;
	[lscd setTouchBehavior:kTouchBehavior_SelectColor];
	[lscds addObject:lscd];
	[self.view addSubview:lscd];
	
	frm.origin.y += (vsize + kBoxPaddingVertical);
	lscd = [[LSColorDisplay alloc] initWithFrame:frm];
	//lscd.delegate = self;
	[lscd setTouchBehavior:kTouchBehavior_SelectColor];
	[lscds addObject:lscd];
	[self.view addSubview:lscd];
	
	frm.origin.y += (vsize + kBoxPaddingVertical);
	lscd = [[LSColorDisplay alloc] initWithFrame:frm];
	//lscd.delegate = self;
	[lscd setTouchBehavior:kTouchBehavior_SelectColor];
	[lscds addObject:lscd];
	[self.view addSubview:lscd];
	
	frm.origin.y += (vsize + kBoxPaddingVertical);
	lscd = [[LSColorDisplay alloc] initWithFrame:frm];
	//lscd.delegate = self;
	[lscd setTouchBehavior:kTouchBehavior_SelectColor];
	[lscds addObject:lscd];
	[self.view addSubview:lscd];
}


#define kORIGIN_COLA	kPaddingSides
#define kORIGIN_COLB	(kPaddingSides + kPaddingCenter + frm.size.width)

- (void) buildUI_TwoColumns
{
	UIColor * bgcolor = [UIColor colorWithRed:0.0 green:0.0 blue:0.0 alpha:1.0];
	[self.view setBackgroundColor:bgcolor];
	int vpad = (self.view.frame.size.height - (kBoxHeight * 4) - (3 * kBoxPaddingVertical))/2;
	
	lscds = [NSMutableArray array];	LSColorDisplay * lscd;
	UIColor * col;
	
	
	CGRect frm = CGRectMake( kPaddingSides,
							vpad, 
							(self.view.frame.size.width - (2 * kPaddingCenter) - kPaddingCenter)/2,
							kBoxHeight);
	
	col = [UIColor redColor];
	
	frm.origin.x = kORIGIN_COLA;
	lscd = [[LSColorDisplay alloc] initWithFrame:frm];
	//lscd.delegate = self;
	[lscd setTouchBehavior:kTouchBehavior_SelectColor];
	[lscds addObject:lscd];
	[self.view addSubview:lscd];
	
	frm.origin.x = kORIGIN_COLB;
	lscd = [[LSColorDisplay alloc] initWithFrame:frm];
	//lscd.delegate = self;
	[lscd setTouchBehavior:kTouchBehavior_SelectColor];
	[lscds addObject:lscd];
	[self.view addSubview:lscd];
	
	frm.origin.x = kORIGIN_COLA;
	frm.origin.y += (kBoxHeight + kBoxPaddingVertical);
	lscd = [[LSColorDisplay alloc] initWithFrame:frm];
	//lscd.delegate = self;
	[lscd setTouchBehavior:kTouchBehavior_SelectColor];
	[lscds addObject:lscd];
	[self.view addSubview:lscd];
	
	frm.origin.x = kORIGIN_COLB;
	lscd = [[LSColorDisplay alloc] initWithFrame:frm];
	//lscd.delegate = self;
	[lscd setTouchBehavior:kTouchBehavior_SelectColor];
	[lscds addObject:lscd];
	[self.view addSubview:lscd];
	
	frm.origin.x = kORIGIN_COLA;
	frm.origin.y += (kBoxHeight + kBoxPaddingVertical);
	lscd = [[LSColorDisplay alloc] initWithFrame:frm];
	//lscd.delegate = self;
	[lscd setTouchBehavior:kTouchBehavior_SelectColor];
	[lscds addObject:lscd];
	[self.view addSubview:lscd];
	
	frm.origin.x = kORIGIN_COLB;
	lscd = [[LSColorDisplay alloc] initWithFrame:frm];
	//lscd.delegate = self;
	[lscd setTouchBehavior:kTouchBehavior_SelectColor];
	[lscds addObject:lscd];
	[self.view addSubview:lscd];
	
	frm.origin.x = kORIGIN_COLA;
	frm.origin.y += (kBoxHeight + kBoxPaddingVertical);
	lscd = [[LSColorDisplay alloc] initWithFrame:frm];
	//lscd.delegate = self;
	[lscd setTouchBehavior:kTouchBehavior_SelectColor];
	[lscds addObject:lscd];
	[self.view addSubview:lscd];
	
	frm.origin.x = kORIGIN_COLB;
	lscd = [[LSColorDisplay alloc] initWithFrame:frm];
	//lscd.delegate = self;
	[lscd setTouchBehavior:kTouchBehavior_SelectColor];
	[lscds addObject:lscd];
	[self.view addSubview:lscd];
}


- (void)setColorArray:(UIColor **)carr ofSize:(int)sz
{
	nColors = sz;
	for( int x=0 ; x<nColors ; x++ )
	{
		[ (LSColorDisplay *)[lscds objectAtIndex:x] setColorWith:carr[x]];
	}
}

#pragma mark -
#pragma mark draw rect

- (void)drawRect:(CGRect)rect
{
	/*
	CGContextRef theContext = UIGraphicsGetCurrentContext();
	CGContextSetRGBFillColor( theContext, 1.0, 1.0, 0.0, 0.5 );
	CGContextFillRect( theContext, rect );
	 */
}

#pragma mark -
#pragma mark transitions

- (void)addViewAndTransitionIn
{
	if( nColors == 0 ) {
		[delegate userSelectedInPicker:self theColor:[UIColor cyanColor]];
	}
//	[delegate userSelectedInPicker:self theColor:[UIColor purpleColor]];
	
	//[self.view setAlpha:0.0];
	CGPoint oldCenter = [self.view center];
	CGPoint newCenter = [self.view center];
	newCenter.x += 320;
	[self.view setCenter:newCenter];
	[[[UIApplication sharedApplication] keyWindow] addSubview:self.view];
	
	[UIView beginAnimations:@"FadeInColorBoxPicker" context:nil];
	[UIView setAnimationBeginsFromCurrentState:YES];
	[UIView setAnimationDuration:0.3];
	[UIView setAnimationCurve:UIViewAnimationCurveEaseIn];
	[self.view setCenter:oldCenter];
	[UIView commitAnimations];
	
}

- (void)removeView:(id)sender
{
	[self.view removeFromSuperview];
}

- (void)transitionOutAndRemoveView:(id)sender
{
	CGPoint newCenter = [self.view center];
	newCenter.x += 320;
	
	[UIView beginAnimations:@"FadeOutBoxColorPIcker" context:nil];
	[UIView setAnimationBeginsFromCurrentState:YES];
	[UIView setAnimationDelegate:self.view];
	[UIView setAnimationDidStopSelector:@selector( removeFromSuperview: )];
	[UIView setAnimationDuration:0.3];
	[UIView setAnimationCurve:UIViewAnimationCurveEaseIn];
	[self.view setCenter:newCenter];
	[UIView commitAnimations];
}

#pragma mark -
#pragma mark BLColorDisplayDelegate implementation

- (void) colorChanged:(id)sender
{
	[delegate userSelectedInPicker:self theColor:[sender getColor]];
	//[self transitionOutAndRemoveView:nil];
//	[self performSelectorInBackground:@selector(transitionOutAndRemoveView:) withObject:self];
	[self performSelectorOnMainThread:@selector(transitionOutAndRemoveView:) withObject:self waitUntilDone:NO];
}

@end
