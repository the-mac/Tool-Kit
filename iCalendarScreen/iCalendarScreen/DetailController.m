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
#import "StringHelper.h"

//Text View contstants
#define kTextViewFontSize		18.0
#define kDefaultNoteLabel		@"Add a Note"

@interface DetailController ()

@end

@implementation DetailController

-(void) updateDetails:(Detail *) d
{
    if([AppDelegate debugging]) NSLog(@"%s",__PRETTY_FUNCTION__);
    self.detail = d;
    
//    self.summary.text = detail.summary;
//    self.who.text = detail.who;
//    self.date.text = detail.date;
//    self.location.text = detail.location;
//    self.description.text = detail.description;
//    self.contactName.text = detail.contactName;
//    self.contactEmail.text = detail.contactEmail;
//    self.contactPhone.text = detail.contactPhone;
    
//    UITableView *table = (UITableView *)[self.view viewWithTag:2];
//    [table reloadData];
    
    
    //        Detail * event = [DetailsSingleton getDetailAt:section and:row];
    //
    //        self.currentEvent = event;
    //
    //        [self update:event];
    
}

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil
{
    if([AppDelegate debugging]) NSLog(@"%s",__PRETTY_FUNCTION__);
    
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        self.title = NSLocalizedString(@"Event Details", @"Event Details");
        if([AppDelegate debugging]) NSLog(@"Initializing the Xib File");
    }
    return self;
}

- (void)viewDidAppear:(BOOL)animated
{
    if([AppDelegate debugging]) NSLog(@"%s",__PRETTY_FUNCTION__);
    [super viewDidAppear:animated];
    
    UITableView *table = (UITableView *)[self.view viewWithTag:2];
    [table reloadData];
}

- (void)viewDidLoad
{
    if([AppDelegate debugging]) NSLog(@"%s",__PRETTY_FUNCTION__);
    [super viewDidLoad];
    
    UINavigationBar *navBar =  [[UINavigationBar alloc] initWithFrame:CGRectMake(0.0f, 0.0f, 320.0f, 44.0f)];
    UIBarButtonItem *buttonItem = [[UIBarButtonItem alloc] initWithBarButtonSystemItem:UIBarButtonSystemItemDone target:self action:@selector(dismissDetails)];
    UINavigationItem *navigationItem = [[UINavigationItem alloc] initWithTitle:@"Event Details"];
    
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

//-(void) processEventSummary :(Detail*)event with:(UITableViewCell*) cell and:(NSInteger) row
//{
//    if([AppDelegate debugging]) NSLog(@"%s",__PRETTY_FUNCTION__);
//    
//    cell.textLabel.text = event.summary;
//    cell.detailTextLabel.text = event.description;
//}
//
//-(void) processEventLocation :(Detail*)event with:(UITableViewCell*) cell and:(NSInteger) row
//{
//    if([AppDelegate debugging]) NSLog(@"%s",__PRETTY_FUNCTION__);
//    cell.textLabel.numberOfLines = 0;
//    cell.textLabel.text = event.summary;
//    cell.detailTextLabel.text = event.description;
//}
//
//-(void) processEventDescription :(Detail*)event with:(UITableViewCell*) cell and:(NSInteger) row
//{
//    if([AppDelegate debugging]) NSLog(@"%s",__PRETTY_FUNCTION__);
//    cell.textLabel.numberOfLines = 0;
//    cell.textLabel.text = event.description;
//}
//
//-(void) processEventContact :(Detail*)event with:(UITableViewCell*) cell and:(NSInteger) row
//{
//    if([AppDelegate debugging]) NSLog(@"%s",__PRETTY_FUNCTION__);
//    cell.textLabel.numberOfLines = 0;
//    cell.textLabel.text = event.contactName;
//    cell.detailTextLabel.text = event.contactEmail;
//}

//Manages the height of the cell.
- (CGFloat)tableView:(UITableView *)tableView heightForRowAtIndexPath:(NSIndexPath *)indexPath  {
	NSString *label = @"";//[self.aNote length] == 0 ? kDefaultNoteLabel : self.aNote;
	CGFloat height = 0.0;//[label RAD_textHeightForSystemFontOfSize:kTextViewFontSize] + 20.0;
    
    int row = indexPath.row;
    if(row == 0){ return 75; }
    else if(row == 1){ return 75.; }
    else if(row == 2){ return 150; }
    else if(row == 3){ return 150; }
    
//    label = [cell.textLabel.text length] == 0 ? kDefaultNoteLabel : cell.textLabel.text;
//    height = [label RAD_textHeightForSystemFontOfSize:kTextViewFontSize] + 20.0;
    
	return height;
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
    if([AppDelegate debugging]) NSLog(@"%s",__PRETTY_FUNCTION__);
    
    return 4;
}

-(NSString *)tableView:(UITableView *)tableView titleForHeaderInSection:(NSInteger)section
{
    return [self.sections objectAtIndex:section];
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    if([AppDelegate debugging]) NSLog(@"%s",__PRETTY_FUNCTION__);
    return 1;
}

// Customize the appearance of table view cells.
- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    if([AppDelegate debugging]) NSLog(@"%s",__PRETTY_FUNCTION__);
    
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
    
    if(section == 0){ [self processEventSummary:event with:cell and:row]; }
    else if(section == 1){ [self processEventLocation:event with:cell and:row]; }
    else if(section == 2){ [self processEventDescription:event with:cell and:row]; }
    else if(section == 3){ [self processEventContact:event with:cell and:row]; }
    
    return cell;
}

- (BOOL)tableView:(UITableView *)tableView canEditRowAtIndexPath:(NSIndexPath *)indexPath
{
    if([AppDelegate debugging]) NSLog(@"%s",__PRETTY_FUNCTION__);
    
    // Return NO if you do not want the specified item to be editable.
    return NO;
}

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    if([AppDelegate debugging]) NSLog(@"%s",__PRETTY_FUNCTION__);
    
    if(YES/* PHONE OPTION HAS BEEN CLICKED*/){/* CALL THE LISTED NUMBER */}
}
@end
