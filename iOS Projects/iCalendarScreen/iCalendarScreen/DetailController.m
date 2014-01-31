//
//  DetailController.m
//  iCalendarScreen
//
//  Created by Christopher Miller on 6/21/13.
//  Copyright (c) 2013 Christopher Miller. All rights reserved.
//

#import "AppDelegate.h"
#import "DetailController.h"
#import "DetailsSingleton.h"

@interface DetailController ()

@end

@implementation DetailController

-(void) updateDetails:(Detail *) d
{
    if([AppDelegate debugging]) NSLog(DEUBBING_OUTPUT,__PRETTY_FUNCTION__);
    self.detail = d;
}

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil
{
    if([AppDelegate debugging]) NSLog(DEUBBING_OUTPUT,__PRETTY_FUNCTION__);
    
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        if([AppDelegate debugging]) NSLog(@"Initializing the Xib File");
    }
    return self;
}

-(void) viewWillAppear:(BOOL)animated
{
    [super viewWillAppear:animated];
    UITableView *table = (UITableView *)[self.view viewWithTag:2];
    [table reloadData];
}

- (void)viewDidAppear:(BOOL)animated
{
    if([AppDelegate debugging]) NSLog(DEUBBING_OUTPUT,__PRETTY_FUNCTION__);
    
    [super viewDidAppear:animated];
}

- (void)viewDidLoad
{
    if([AppDelegate debugging]) NSLog(DEUBBING_OUTPUT,__PRETTY_FUNCTION__);
    [super viewDidLoad];
    
    UINavigationBar *navBar =  [[UINavigationBar alloc] initWithFrame:CGRectMake(0.0f, 0.0f, 320.0f, 44.0f)];
    UIBarButtonItem *buttonItem = [[UIBarButtonItem alloc] initWithBarButtonSystemItem:UIBarButtonSystemItemDone target:self action:@selector(dismissDetails)];
    
    self.navigation = [[UINavigationItem alloc] initWithTitle:@"Event Details"];
    self.navigation.leftBarButtonItem = buttonItem;
    
    [navBar pushNavigationItem:self.navigation animated:NO];
    [self.view addSubview:navBar];
}

- (void)didReceiveMemoryWarning
{
    if([AppDelegate debugging]) NSLog(DEUBBING_OUTPUT,__PRETTY_FUNCTION__);

    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

-(void) dismissDetails
{
    if([AppDelegate debugging]) NSLog(DEUBBING_OUTPUT,__PRETTY_FUNCTION__);
    [self dismissViewControllerAnimated:YES completion:NULL];
}

//Manages the height of the cell.
- (CGFloat)tableView:(UITableView *)tableView heightForRowAtIndexPath:(NSIndexPath *)indexPath  {
    
    if (tableView.tag == 1) {
        return 45;
    } else {
        int section = indexPath.section;
        int desc_size = [self.detail.desc length];
        int name_size = [self.detail.contactName length];
        
        if(section == 0){ return 45; }
        else if(section == 1){ return 45; }
        else if(section == 2){ return (desc_size*.86)+65; }
        else if(section == 3){ return (name_size*.86)+65; }
    }
    
	return 45;
}


- (UIView *) tableView:(UITableView *)tableView viewForHeaderInSection:(NSInteger)s
{
    UIView * headerView = [[UIView alloc] initWithFrame:CGRectMake(0, 0, tableView.bounds.size.width, 22)];
    UILabel *label = [[UILabel alloc] initWithFrame:CGRectMake(10, 3, tableView.bounds.size.width - 10, 18)];

    label.text = [self.sections objectAtIndex:s];
    label.textColor = [UIColor colorWithRed:1.0 green:1.0 blue:1.0 alpha:1.0];
    label.backgroundColor = [UIColor clearColor];
    [headerView addSubview:label];
    
    return headerView;
}

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView
{
    if([AppDelegate debugging]) NSLog(DEUBBING_OUTPUT,__PRETTY_FUNCTION__);
    
    return 4;
}

-(NSString *)tableView:(UITableView *)tableView titleForHeaderInSection:(NSInteger)section
{
    return [self.sections objectAtIndex:section];
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    if([AppDelegate debugging]) NSLog(DEUBBING_OUTPUT,__PRETTY_FUNCTION__);
    return 1;
}

// Customize the appearance of table view cells.
- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    if([AppDelegate debugging]) NSLog(DEUBBING_OUTPUT,__PRETTY_FUNCTION__);
    
    static NSString *CellIdentifier = @"Cell";
    
    UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:CellIdentifier];
    if (cell == nil) {
        cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:CellIdentifier];
        cell.accessoryType = UITableViewCellAccessoryNone;
        cell.selectionStyle = UITableViewCellSelectionStyleNone;
    }

    int row = indexPath.row;
    int section = indexPath.section;
    Detail * event = self.detail;
    self.navigation.title = self.detail.date;
    
    if(section == 0){ [self processEventSummary:event with:cell and:row]; }
    else if(section == 1){ [self processEventLocation:event with:cell and:row]; }
    else if(section == 2){ [self processEventDescription:event with:cell and:row]; }
    else if(section == 3){ [self processEventContact:event with:cell and:row]; }
    
    return cell;
}

- (BOOL)tableView:(UITableView *)tableView canEditRowAtIndexPath:(NSIndexPath *)indexPath
{
    if([AppDelegate debugging]) NSLog(DEUBBING_OUTPUT,__PRETTY_FUNCTION__);
    
    // Return NO if you do not want the specified item to be editable.
    return NO;
}

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    if([AppDelegate debugging]) NSLog(DEUBBING_OUTPUT,__PRETTY_FUNCTION__);
    
    if(YES/* PHONE OPTION HAS BEEN CLICKED*/){/* CALL THE LISTED NUMBER */}
}
@end
