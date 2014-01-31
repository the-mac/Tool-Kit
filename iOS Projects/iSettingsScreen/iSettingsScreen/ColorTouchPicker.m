//
//  Copyright 2009 Scott Lawrence. All rights reserved.
//  This is heavily based on UICatalog from Apple's samples
//
//  This module (and submodules) can be used for any purpose in any app
//  no warranty is expressed or implied.  If it works for you, great.
//  If it breaks, bummer.
//

#import "ColorTouchPicker.h"


@interface ColorTouchPicker ()
@end


@implementation ColorTouchPicker

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

+ (UIColor *) colorHueFromX:(float)x fromY:(float)y
{
	float sat = 1.0f;
	float hue = x;
	float brt = 1.0f;
	float alp = 1.0f;
	
	if( y < 0.5f ) { // top half of screen
		sat -= (0.5f-y) * 2.0f;
	}
	
	if( y > 0.5 ) { // bottom half of screen
		brt -= (y-0.5f) * 2.0f;
	}

	return( [UIColor colorWithHue:hue saturation:sat brightness:brt alpha:alp ] );
}


-(float) horizontalPercentageOf:(int)x
{
	return ((float)x / (float) [self.view bounds].size.width);
}

-(float) verticalPercentageOf:(int)y
{
	return ((float)y / (float) [self.view bounds].size.height);
}

- (void)setColorWith:(UITouch *)touch
{
	float x = [self horizontalPercentageOf:[touch locationInView:self.view].x];
	float y = [self verticalPercentageOf:[touch locationInView:self.view].y];
	
//	[theColor release];
	theColor = [ColorTouchPicker colorHueFromX:x fromY:y];
//	[theColor retain];
	[self.view setBackgroundColor:theColor];
}

- (void)touchesBegan:(NSSet *)touches withEvent:(UIEvent *)event
{
	CGPoint startTouchPosition;
	UITouch *touch = [touches anyObject];
    startTouchPosition = [touch locationInView:self.view];
	[self setColorWith:touch];
}

- (void)touchesMoved:(NSSet *)touches withEvent:(UIEvent *)event
{
    UITouch *touch = [touches anyObject];
    CGPoint currentTouchPosition;
	currentTouchPosition = [touch locationInView:self.view];
	[self setColorWith:touch];
}

- (void)foregroundStuff:(id)obj
{
	[delegate userTouchedInPicker:self theColor:theColor];
	[self transitionOutAndRemoveView:self];
}

- (void)touchesEnded:(NSSet *)touches withEvent:(UIEvent *)event
{
    UITouch *touch = [touches anyObject];
    CGPoint currentTouchPosition;
	currentTouchPosition = [touch locationInView:self.view];
	
	[self performSelectorOnMainThread:@selector(foregroundStuff:) withObject:self waitUntilDone:NO];
}


#pragma mark -
#pragma mark class setup and accessors

- (void) setLegend:(NSString *)text
{
	legendLabel.text = text;
}

- (id)init
{
	self = [super init];
	if( self )
	{
		theColor = [UIColor cyanColor];
		[self.view setNeedsDisplay];
		
		[self.view setBackgroundColor:[UIColor blackColor]];
		
		legendLabel = [[UILabel alloc] initWithFrame:self.view.frame];
		
		[self setLegend:@"Touch the screen to pick a color"];
		legendLabel.textColor = [UIColor whiteColor];
		legendLabel.shadowColor = [UIColor blackColor];
		legendLabel.backgroundColor = [UIColor clearColor];
		legendLabel.textAlignment = UITextAlignmentCenter;
		[legendLabel setAlpha:0.75f];
		[self.view addSubview:legendLabel];
	}
	return self;
}


- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning]; // Releases the view if it doesn't have a superview
    // Release anything that's not essential, such as cached data
}


//- (void)dealloc {
//    [super dealloc];
//}

#pragma mark -
#pragma mark draw rect

- (void)drawRect:(CGRect)rect
{
//	[self.view setBackgroundColor:[UIColor blueColor]];
	CGContextRef theContext = UIGraphicsGetCurrentContext();
	CGContextSetRGBFillColor( theContext, 1.0, 0.3, 1.0, 0.5 );
	CGContextFillRect( theContext, rect );
}

#pragma mark -
#pragma mark transitions

- (void)addViewAndTransitionIn
{
	//[self.view setAlpha:0.0];
	CGPoint oldCenter = [self.view center];
	CGPoint newCenter = [self.view center];
	newCenter.x += 320;
	[self.view setCenter:newCenter];
	[[[UIApplication sharedApplication] keyWindow] addSubview:self.view];
	
	[UIView beginAnimations:@"FadeInColorTouchPicker" context:nil];
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
	
	[UIView beginAnimations:@"FadeOutTouchColorPIcker" context:nil];
	[UIView setAnimationBeginsFromCurrentState:YES];
	[UIView setAnimationDelegate:self.view];
	[UIView setAnimationDidStopSelector:@selector( removeFromSuperview: )];
	[UIView setAnimationDuration:0.3];
	[UIView setAnimationCurve:UIViewAnimationCurveEaseIn];
	[self.view setCenter:newCenter];
	[UIView commitAnimations];
}

@end
