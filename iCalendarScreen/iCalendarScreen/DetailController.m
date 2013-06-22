//
//  DetailController.m
//  iCalendarScreen
//
//  Created by Christopher Miller on 6/21/13.
//  Copyright (c) 2013 Christopher Miller. All rights reserved.
//

#import "AppDelegate.h"
#import "DetailController.h"

@interface DetailController ()

@end

@implementation DetailController

-(void) update:(Detail *) detail
{
    if([AppDelegate debugging]) NSLog(@"%s",__PRETTY_FUNCTION__);
    if([AppDelegate debugging]) NSLog(@"The detail: %@",detail);
    
    if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad) {
        // The device is an iPad running iPhone 3.2 or later.
        // set up the iPad-specific view
        
        self.summary.text = detail.summary;
        self.who.text = detail.who;
        self.date.text = detail.date;
        self.location.text = detail.location;
        self.description.text = detail.description;
        self.contactName.text = detail.contactName;
        self.contactEmail.text = detail.contactEmail;
        self.contactPhone.text = detail.contactPhone;
    }
    else {
        if([AppDelegate debugging]) NSLog(@"Opening the detail screen for the iPhone");
        
        // The device is an iPhone or iPod touch.
        // set up the iPhone/iPod Touch view
        
//        if(self.theDetailScreen == nil)
//            self.theDetailScreen = [[DetailController alloc] initWithNibName:@"DetailController" bundle:nil];
//        
//        self.theDetailScreen.theDetail = detail;
//        [self presentViewController:self.theDetailScreen animated:YES completion:nil];
    }
    [self reloadInputViews];
}

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil
{
    if([AppDelegate debugging]) NSLog(@"%s",__PRETTY_FUNCTION__);
    
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        if([AppDelegate debugging]) NSLog(@"Initializing the Xib File");
    }
    return self;
}

- (void)viewDidAppear:(BOOL)animated
{
    if([AppDelegate debugging]) NSLog(@"%s",__PRETTY_FUNCTION__);
    [super viewDidAppear:animated];
}

- (void)viewDidLoad
{
    if([AppDelegate debugging]) NSLog(@"%s",__PRETTY_FUNCTION__);
    [super viewDidLoad];
}

- (void)didReceiveMemoryWarning
{
    if([AppDelegate debugging]) NSLog(@"%s",__PRETTY_FUNCTION__);

    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

@end
