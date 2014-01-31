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
#import "DetailsSingleton.h"
#import "Detail.h"

@interface ViewController ()

@end

@implementation ViewController

-(void) update:(Detail *) detail
{
    if([AppDelegate debugging]) NSLog(DEUBBING_OUTPUT,__PRETTY_FUNCTION__);
    
    self.currentEvent = detail;
    
    if([AppDelegate debugging]) NSLog(@"%@",[detail date]);

    if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPhone) {
        if([AppDelegate debugging]) NSLog(@"Opening the detail screen for the iPhone");
        
        // The device is an iPhone or iPod touch.
        // set up the iPhone/iPod Touch view
        
        if(self.theDetailScreen == nil)
            self.theDetailScreen = [[DetailController alloc] initWithNibName:@"DetailController" bundle:nil];
                
        [self presentViewController:self.theDetailScreen animated:YES completion:nil];

        [self.theDetailScreen updateDetails:detail];
    }
}

-(void) setup
{
    if([AppDelegate debugging]) NSLog(DEUBBING_OUTPUT,__PRETTY_FUNCTION__);
    self.sections = @[@"Summary",@"Location",@"Description",@"Contact"];

    NSString *path = [[NSBundle mainBundle] bundlePath];
    NSString *finalPath = [path stringByAppendingPathComponent:@"newest-no--object.json"];
    
    NSData* data = [NSData dataWithContentsOfFile:finalPath];
    [self fetchedData:data];
    
    Detail * d = [DetailsSingleton getDetailAt:0 and:0];
    
    if(d == nil) return;
    
    [self update:d];
}

- (void)fileWillLoad
{
    dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_DEFAULT, 0), ^{
    	NSString *path = [[NSBundle mainBundle] bundlePath];
    	NSString *finalPath = [path stringByAppendingPathComponent:@"new-no--object.json"];
        NSData* data = [NSData dataWithContentsOfFile:finalPath];
        [self performSelectorOnMainThread:@selector(fetchedData:) withObject:data waitUntilDone:YES];
    });
}

- (void)fetchedData:(NSData *)responseData {

    //parse out the json data
    NSError* error;
    NSDictionary* json = [NSJSONSerialization JSONObjectWithData:responseData //1
                                                         options:kNilOptions
                                                           error:&error];
    NSDictionary* eventsDictionary = [json objectForKey:@"bwEventList"]; //2
    NSArray * events = [eventsDictionary objectForKey:@"events"]; //2
    events = [events sortedArrayUsingComparator:^NSComparisonResult(id a, id b) {
        
        NSDictionary * dict_0 = (NSDictionary *)a;
        NSDictionary * dict_1 = (NSDictionary *)b;
        
        NSDictionary * start_0 = [dict_0 objectForKey:@"start"];
        NSDictionary * start_1 = [dict_1 objectForKey:@"start"];
        
        //Convert NSString to NSDate:
        NSDateFormatter *dateFormatter = [[NSDateFormatter alloc] init];
        
        //Specify only 1 M for month, 1 d for day and 1 h for hour
        [dateFormatter setDateFormat:@"EEEE M/d/yy"];
        
        
        NSString * date_0 = [NSString stringWithFormat:@"%@ %@",[start_0 objectForKey:@"dayname"],[start_0 objectForKey:@"shortdate"]];//@"6/27/13";
        NSString * date_1 = [NSString stringWithFormat:@"%@ %@",[start_1 objectForKey:@"dayname"],[start_1 objectForKey:@"shortdate"]];//@"6/27/13";

        NSDate *first = [dateFormatter dateFromString:date_0];
        NSDate *second = [dateFormatter dateFromString:date_1];
        return [first compare:second];
    }];
    
//    NSDictionary * indices = [[NSDictionary alloc]init];
//    int count = [events count];
//    int position = 0;
//    
//    for (int index = 0; index < count - 1; index++) {
//        NSDictionary * nsd1 = [events objectAtIndex:index];
//        NSDictionary * nsd2 = [events objectAtIndex:index + 1];
//        
//        [indices setValue:@"" forKey:@""];
//    }
    int index = 0;
    for (NSDictionary *event = [events objectAtIndex:index++]; index < [events count]; event = [events objectAtIndex:index++]) {
//        statements
//    }
//    for (NSDictionary *event in events) {
        
        // GATHER SUMMARY INFORMATION
        NSString * summary = [event objectForKey:@"summary"];//
        NSString * desc = [event objectForKey:@"description"];//
        NSString * cost = [event objectForKey:@"cost"];//[event objectForKey:@""];//
        
        // GATHER DATE INFORMATION
        NSDictionary * d0 = [event objectForKey:@"start"];
        NSString * date = [NSString stringWithFormat:@"%@ %@",[d0 objectForKey:@"dayname"],[d0 objectForKey:@"shortdate"]];//@"6/27/13";
        
        
        // GATHER LOCATION INFORMATION
        NSDictionary * l0 = [event objectForKey:@"location"];
        NSString * location = [l0 objectForKey:@"address"];//@"J-251, The Atrium";

        // GATHER CONTACT INFORMATION
        NSDictionary * c0 = [event objectForKey:@"contact"];
        NSString * contactName = [c0 objectForKey:@"name"];//@"Christopher Miller";
        NSString * contactEmail = [c0 objectForKey:@"link"];//@"cmiller3@spsu.edu";
        NSString * contactPhone = [c0 objectForKey:@"phone"];//@"000-000-0000";
        
        [DetailsSingleton
            create:summary      // SUMMARY
            with:cost           // ATTENDEES
            and:date            // DATE
            and:location        // LOCATION
            and:desc            // DESCRIPTION
            and:contactName     // CONTACT NAME
            with:contactEmail   // CONTACT EMAIL
            and:contactPhone    // CONTACT PHONE
        ];
    }
}

- (void)viewDidLoad
{
    if([AppDelegate debugging]) NSLog(DEUBBING_OUTPUT,__PRETTY_FUNCTION__);
    
    [super viewDidLoad]; 
    [self setup];
}

- (void)didReceiveMemoryWarning
{
    if([AppDelegate debugging]) NSLog(DEUBBING_OUTPUT,__PRETTY_FUNCTION__);
    
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}


-(void) processEventSummary :(Detail*)event with:(UITableViewCell*) cell and:(NSInteger) row
{
    if([AppDelegate debugging]) NSLog(DEUBBING_OUTPUT,__PRETTY_FUNCTION__);
    cell.textLabel.numberOfLines = 0;
    cell.textLabel.text = [event.summary length] == 0 ? @"No Summary Posted" : event.summary;
}

-(void) processEventLocation :(Detail*)event with:(UITableViewCell*) cell and:(NSInteger) row
{
    if([AppDelegate debugging]) NSLog(DEUBBING_OUTPUT,__PRETTY_FUNCTION__);
    cell.textLabel.numberOfLines = 0;
    cell.textLabel.text = [event.location length] == 0 ? @"No Location Posted" : event.location;
}

-(void) processEventDescription :(Detail*)event with:(UITableViewCell*) cell and:(NSInteger) row
{
    if([AppDelegate debugging]) NSLog(DEUBBING_OUTPUT,__PRETTY_FUNCTION__);
    cell.textLabel.numberOfLines = 0;
    cell.textLabel.text = [event.desc length] == 0 ? @"No Description Posted" : event.desc;
}

-(void) processEventContact :(Detail*)event with:(UITableViewCell*) cell and:(NSInteger) row
{
    if([AppDelegate debugging]) NSLog(DEUBBING_OUTPUT,__PRETTY_FUNCTION__);
    cell.textLabel.numberOfLines = 0;
    cell.textLabel.text = [event.contactName length] == 0 ? @"No Contact Posted" : event.contactName;
}

//Manages the height of the cell.
- (CGFloat)tableView:(UITableView *)tableView heightForRowAtIndexPath:(NSIndexPath *)indexPath  {
    
    if (tableView.tag == 1) {
        return 45;
    } else {
        int section = indexPath.section;
        int desc_size = [self.currentEvent.desc length];
        int name_size = [self.currentEvent.contactName length];
        
        if(section == 0){ return 45; }
        else if(section == 1){ return 45; }
        else if(section == 2){ return (desc_size*.45)+45; }
        else if(section == 3){ return (name_size*.45)+45; }
    }
    
	return 45;
}

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView
{
    if([AppDelegate debugging]) NSLog(DEUBBING_OUTPUT,__PRETTY_FUNCTION__);
    
    if (tableView.tag == 1) {
        return [DetailsSingleton getSections];
    } else {
        return 4;
    }
}

-(NSString *)tableView:(UITableView *)tableView titleForHeaderInSection:(NSInteger)section
{
    if (tableView.tag == 1) {
        return [DetailsSingleton getHeaderAt:section];
    } else {
        return [self.sections objectAtIndex:section];
    }
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    if([AppDelegate debugging]) NSLog(DEUBBING_OUTPUT,__PRETTY_FUNCTION__);
    
    int row = -1;
    if (tableView.tag == 1) {
        row = [DetailsSingleton getRowsAt:section];
    } else {
        row = 1;
    }
    NSLog(@"Row is %d in numberOfRowsInSection",row);
    return row;
}

- (UIView *) tableView:(UITableView *)tableView viewForHeaderInSection:(NSInteger)s
{
    UIView * headerView = [[UIView alloc] initWithFrame:CGRectMake(0, 0, tableView.bounds.size.width, 22)];
    UILabel *label = [[UILabel alloc] initWithFrame:CGRectMake(10, 3, tableView.bounds.size.width - 10, 18)];
    
    if (tableView.tag == 1) {
        label.text = [DetailsSingleton getHeaderAt:s];
    } else {
        label.text = [self.sections objectAtIndex:s];
    }
    label.textColor = [UIColor colorWithRed:1.0 green:1.0 blue:1.0 alpha:1.0];
    label.backgroundColor = [UIColor clearColor];
    [headerView addSubview:label];
    return headerView;
    
}

// Customize the appearance of table view cells.
- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    if([AppDelegate debugging]) NSLog(DEUBBING_OUTPUT,__PRETTY_FUNCTION__);
    
    static NSString *CellIdentifier = @"Cell";
    
    UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:CellIdentifier];
    
    int row = indexPath.row;
    int section = indexPath.section;
    
//    NSLog(@"Row is %d outside table 1",row);
    if (tableView.tag == 1) {
        if (cell == nil) {
            cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:CellIdentifier];
            cell.accessoryType = UITableViewCellAccessoryNone;
        }
        
//        NSLog(@"Row is %d inside table 1",row);
        Detail * event = [DetailsSingleton getDetailAt:section and:row];
        cell.textLabel.text = event.summary;
//        cell.detailTextLabel.text = event.desc;

    } else {
        if (cell == nil) {
            cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:CellIdentifier];
            cell.accessoryType = UITableViewCellAccessoryNone;
            cell.selectionStyle = UITableViewCellSelectionStyleNone;
        }
        
        if(section == 0){ [self processEventSummary:self.currentEvent with:cell and:row]; }
        else if(section == 1){ [self processEventLocation:self.currentEvent with:cell and:row]; }
        else if(section == 2){ [self processEventDescription:self.currentEvent with:cell and:row]; }
        else if(section == 3){ [self processEventContact:self.currentEvent with:cell and:row]; }
    }
    
    return cell;
}

- (BOOL)tableView:(UITableView *)tableView canEditRowAtIndexPath:(NSIndexPath *)indexPath
{
    //if([AppDelegate debugging]) NSLog(DEUBBING_OUTPUT,__PRETTY_FUNCTION__);
    
    // Return NO if you do not want the specified item to be editable.
    return NO;
}

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    if([AppDelegate debugging]) NSLog(DEUBBING_OUTPUT,__PRETTY_FUNCTION__);
     
    int row = indexPath.row;
    int section = indexPath.section;
    
    if (tableView.tag == 1) {
        if([AppDelegate debugging]) NSLog(@"Selected an event");
        
        Detail * d = [DetailsSingleton getDetailAt:section and:row];
        
        [self update:d];
        
        UITableView *table = (UITableView *)[self.view viewWithTag:2];
        [table reloadData];
    } else {
        if([AppDelegate debugging]) NSLog(@"Selected the details content");

        if(YES/* PHONE OPTION HAS BEEN CLICKED*/){/* CALL THE LISTED NUMBER */}
    }
}
@end
