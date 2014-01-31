//
//  ViewController.m
//  iHomeScreen
//
//  Created by Christopher Miller on 6/12/13.
//  Copyright (c) 2013 Christopher Miller. All rights reserved.
//

#import "HomeScreen.h"

@interface HomeScreen ()

@end

@implementation HomeScreen

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil
{
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        self.title = NSLocalizedString(@"Nest", @"Nest");
    }
    return self;
}

- (void)viewDidLoad
{
    [super viewDidLoad];
	// Do any additional setup after loading the view, typically from a nib.
}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

- (IBAction)onClick:(id)sender {
    if(sender == self.shareNestButton)
    {
        if(self.theShareScreen == nil)
        {
            if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad) {
                // The device is an iPad running iPhone 3.2 or later.
                //self.theShareScreen = [[ShareScreen alloc] initWithNibName:@"ShareScreenPad" bundle:nil];
                // set up the iPad-specific view
            }
            else {
                // The device is an iPhone or iPod touch.
                //self.theShareScreen = [[ShareScreen alloc] initWithNibName:@"ShareScreen" bundle:nil];
                // set up the iPhone/iPod Touch view
            }
        }
        
        [self presentViewController:self.theShareScreen animated:YES completion:nil];
    }
    else if(self.settingsButton == sender)
    {
        if(self.theSettingsScreen == nil)
        {
            if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad) {
                // The device is an iPad running iPhone 3.2 or later.
                //self.theSettingsScreen = [[SettingsScreen alloc] initWithNibName:@"SettingsScreenPad" bundle:nil];
                // set up the iPad-specific view
            }
            else {
                // The device is an iPhone or iPod touch.
                //self.theSettingsScreen = [[SettingsScreen alloc] initWithNibName:@"SettingsScreen" bundle:nil];
                // set up the iPhone/iPod Touch view
            }
            //            self.theSettingsScreen = [[FlipsideViewController alloc] initWithNibName:@"FlipsideView" bundle:nil];
            //            self.theSettingsScreen = [[SettingsScreen alloc] initWithNibName:@"SettingsScreen" bundle:nil];
        }
        
        [self presentViewController:self.theSettingsScreen animated:YES completion:nil];
    }
    else
    {
        //[AlertViewSingleton show:@"Hornet Buzz" andThe:@"View your favorite SPSU feeds (Twitter, FaceBook, PolyBlog, etc.)"];
    }
}

@end
