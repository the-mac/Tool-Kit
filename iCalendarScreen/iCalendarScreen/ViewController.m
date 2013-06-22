//
//  ViewController.m
//  iCalendarScreen
//
//  Created by Christopher Miller on 6/14/13.
//  Copyright (c) 2013 Christopher Miller. All rights reserved.
//

#import "AppDelegate.h"
#import "ViewController.h"
#import "DetailController.h"
#import "Detail.h"

@interface ViewController ()

@end

@implementation ViewController

-(Detail*) create:(NSString*) s with:(NSString*) w
              and:(NSString*) d and:(NSString*) l and:(NSString*) de
              and:(NSString*) cn with:(NSString*) ce and:(NSString*) cp
{
    Detail * detail = [[Detail alloc]init];
    detail.summary = s;
    detail.who = w;
    detail.date = d;
    detail.location = l;
    detail.description = de;
    detail.contactName = cn;
    detail.contactEmail = ce;
    detail.contactPhone = cp;
    
    return detail;
}

-(void) update:(Detail *) detail
{
    if([AppDelegate debugging]) NSLog(@"%s",__PRETTY_FUNCTION__);

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
        
        if(self.theDetailScreen == nil)
            self.theDetailScreen = [[DetailController alloc] initWithNibName:@"DetailController" bundle:nil];
        
        if([AppDelegate debugging]) NSLog(@"The detail: %@",detail);
        
        self.theDetailScreen.theDetail = detail;

        [self presentViewController:self.theDetailScreen animated:YES completion:nil];

        [self.theDetailScreen update:detail];
    }
}

-(void) setup
{
    if([AppDelegate debugging]) NSLog(@"%s",__PRETTY_FUNCTION__);

    Detail * d = [self.events objectAtIndex:0];

    if(d == nil) return;
    
    [self update:d];
}

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    NSString * longDescription = @"Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor.\n Invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.";
    
    //Detail * d = [self create:@"Summary" with:@"who" and:@"date" and:@"location" and:@"description" and:@"cName" with:@"cEmail" and:@"cPhone"];
    Detail * d1 = [self create:@"First" with:@"Christopher" and:@"6/28/13" and:@"J-251, The Atrium" and:longDescription and:@"Christopher Miller" with:@"" and:@""];
    Detail * d2 = [self create:@"Second" with:@"Jaymes" and:@"6/28/13" and:@"J-251, The Atrium" and:longDescription and:@"Christopher Miller" with:@"" and:@""];
    Detail * d3 = [self create:@"Third" with:@"James" and:@"6/28/13" and:@"J-251, The Atrium" and:longDescription and:@"Christopher Miller" with:@"" and:@""];
    Detail * d4 = [self create:@"Fourth" with:@"Josh" and:@"6/28/13" and:@"J-251, The Atrium" and:longDescription and:@"Christopher Miller" with:@"" and:@""];
    Detail * d5 = [self create:@"Fifth" with:@"Roger" and:@"6/28/13" and:@"J-251, The Atrium" and:longDescription and:@"Christopher Miller" with:@"" and:@""];
    
//    self.events = @[@"Event 1",@"Event 2",@"Event 3",@"Event 4"];
    self.events = @[d1,d2,d3,d4,d5];
    
    [self setup];
}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}


- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView
{
    return 1;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    return [self.events count];
}

// Customize the appearance of table view cells.
- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    static NSString *CellIdentifier = @"Cell";
    
    UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:CellIdentifier];
    if (cell == nil) {
        cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:CellIdentifier];
        cell.accessoryType = UITableViewCellAccessoryDisclosureIndicator;
    }
    
//    NSDictionary * event = [self.events objectAtIndex:indexPath.row];
//    NSString * event = [self.events objectAtIndex:indexPath.row];

    Detail * event = [self.events objectAtIndex:indexPath.row];
    cell.textLabel.text = event.summary;
    cell.detailTextLabel.text = event.who;

//    cell.detailTextLabel.text = [event objectForKey:@"description"];
    return cell;
}

- (BOOL)tableView:(UITableView *)tableView canEditRowAtIndexPath:(NSIndexPath *)indexPath
{
    // Return NO if you do not want the specified item to be editable.
    return NO;
}

//- (void)tableView:(UITableView *)tableView commitEditingStyle:(UITableViewCellEditingStyle)editingStyle forRowAtIndexPath:(NSIndexPath *)indexPath
//{
//    if (editingStyle == UITableViewCellEditingStyleDelete) {
//        [_objects removeObjectAtIndex:indexPath.row];
//        [tableView deleteRowsAtIndexPaths:@[indexPath] withRowAnimation:UITableViewRowAnimationFade];
//    } else if (editingStyle == UITableViewCellEditingStyleInsert) {
//        // Create a new instance of the appropriate class, insert it into the array, and add a new row to the table view.
//    }
//}

/*
 // Override to support rearranging the table view.
 - (void)tableView:(UITableView *)tableView moveRowAtIndexPath:(NSIndexPath *)fromIndexPath toIndexPath:(NSIndexPath *)toIndexPath
 {
 }
 */

/*
 // Override to support conditional rearranging of the table view.
 - (BOOL)tableView:(UITableView *)tableView canMoveRowAtIndexPath:(NSIndexPath *)indexPath
 {
 // Return NO if you do not want the item to be re-orderable.
 return YES;
 }
 */

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    
    //    if (!self.detailViewController) {
    //        self.detailViewController = [[DetailViewController alloc] initWithNibName:@"DetailViewController" bundle:nil];
    //    }
    //    NSDate *object = _objects[indexPath.row];
    //    self.detailViewController.detailItem = object;
    //    [self.navigationController pushViewController:self.detailViewController animated:YES];
    
//    NSDictionary * event = [self.events objectAtIndex:indexPath.row];
//    NSString * event = [self.events objectAtIndex:indexPath.row];
    
    
    Detail * event = [self.events objectAtIndex:indexPath.row];

    [self update:event];
    
//    NSString * summary = [event objectForKey:@"summary"];
//    NSString * desc = [event objectForKey:@"description"];
//    [AlertViewSingleton show:summary andThe:desc];
}
@end
