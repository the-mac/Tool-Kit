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
    
    self.summary.text = detail.summary;
    self.who.text = detail.who;
    self.date.text = detail.date;
    self.location.text = detail.location;
    self.description.text = detail.description;
    self.contactName.text = detail.contactName;
    self.contactEmail.text = detail.contactEmail;
    self.contactPhone.text = detail.contactPhone;
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
    
    UINavigationBar *navBar =  [[UINavigationBar alloc] initWithFrame:CGRectMake(0.0f, 0.0f, 320.0f, 44.0f)];
    UIBarButtonItem *buttonItem = [[UIBarButtonItem alloc] initWithBarButtonSystemItem:UIBarButtonSystemItemDone target:self action:@selector(dismissDetails)];
    UINavigationItem *navigationItem = [[UINavigationItem alloc] initWithTitle:@"Settings"];
    
    navigationItem.leftBarButtonItem = buttonItem;
    [navBar pushNavigationItem:navigationItem animated:NO];
    [self.view addSubview:navBar];
}

- (void)didReceiveMemoryWarning
{
    if([AppDelegate debugging]) NSLog(@"%s",__PRETTY_FUNCTION__);

    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

-(void) dismissDetails
{
    if([AppDelegate debugging]) NSLog(@"%s",__PRETTY_FUNCTION__);
    [self dismissViewControllerAnimated:YES completion:NULL];
}

@end
