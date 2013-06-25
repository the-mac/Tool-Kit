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
                
        [self presentViewController:self.theDetailScreen animated:YES completion:nil];

        [self.theDetailScreen update:detail];
    }
}

-(void) setup
{
    if([AppDelegate debugging]) NSLog(@"%s",__PRETTY_FUNCTION__);
    
    NSString *path = [[NSBundle mainBundle] bundlePath];
    NSString *finalPath = [path stringByAppendingPathComponent:@"new-no--object.json"];
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
    NSString * longDescription = @"Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor.\n Invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.";

    //parse out the json data
    NSError* error;
    NSDictionary* json = [NSJSONSerialization JSONObjectWithData:responseData //1
                                                         options:kNilOptions
                                                           error:&error];
    NSDictionary* eventsDictionary = [json objectForKey:@"bwEventList"]; //2
    NSArray * events = [eventsDictionary objectForKey:@"events"]; //2
    
    for (NSDictionary *event in events) {
        
        // GATHER SUMMARY INFORMATION
        NSString * title = [event objectForKey:@"summary"];//
        NSString * attendees = @"Christopher";//[event objectForKey:@""];//
        
        // GATHER DATE INFORMATION
        NSDictionary * d0 = [event objectForKey:@"start"];
        NSString * date = [NSString stringWithFormat:@"%@ %@",[d0 objectForKey:@"dayname"],[d0 objectForKey:@"shortdate"]];//@"6/27/13";
        
        
        // GATHER LOCATION INFORMATION
        NSDictionary * l0 = [event objectForKey:@"location"];
        NSString * location = [l0 objectForKey:@"address"];//@"J-251, The Atrium";

        // GATHER CONTACT INFORMATION
        NSDictionary * c0 = [event objectForKey:@"contact"];
        NSString * contactName = [c0 objectForKey:@"name"];//@"Christopher Miller";
        NSString * contactEmail = [c0 objectForKey:@"phone"];//@"cmiller3@spsu.edu";
        NSString * contactPhone = [c0 objectForKey:@"link"];//@"000-000-0000";
        
        [DetailsSingleton
            create:title        // SUMMARY
            with:attendees    // ATTENDEES
            and:date            // DATE
            and:location        // LOCATION
            and:longDescription // DESCRIPTION
            and:contactName     // CONTACT NAME
            with:contactEmail   // CONTACT EMAIL
            and:contactPhone    // CONTACT PHONE
        ];
    }
    
}

- (void)viewDidLoad
{
    if([AppDelegate debugging]) NSLog(@"%s",__PRETTY_FUNCTION__);
    
    [super viewDidLoad];
//    [self fileWillLoad];
    
//    Detail * d = [self create:@"Summary" with:@"who" and:@"date" and:@"location" and:@"description" and:@"cName" with:@"cEmail" and:@"cPhone"];
//    Detail * d1 = [DetailsSingleton create:@"First" with:@"Christopher" and:@"6/27/13" and:@"J-251, The Atrium" and:longDescription and:@"Christopher Miller" with:@"" and:@""];
//    Detail * d2 = [DetailsSingleton create:@"Second" with:@"Jaymes" and:@"6/26/13" and:@"J-251, The Atrium" and:longDescription and:@"Christopher Miller" with:@"" and:@""];
//    Detail * d3 = [DetailsSingleton create:@"Third" with:@"James" and:@"6/28/13" and:@"J-251, The Atrium" and:longDescription and:@"Christopher Miller" with:@"" and:@""];
//    Detail * d4 = [DetailsSingleton create:@"Fourth" with:@"Josh" and:@"6/25/13" and:@"J-251, The Atrium" and:longDescription and:@"Christopher Miller" with:@"" and:@""];
//    Detail * d5 = [DetailsSingleton create:@"Fifth" with:@"Roger" and:@"6/28/13" and:@"J-251, The Atrium" and:longDescription and:@"Christopher Miller" with:@"" and:@""];
    
    [self setup];
}

- (void)didReceiveMemoryWarning
{
    if([AppDelegate debugging]) NSLog(@"%s",__PRETTY_FUNCTION__);
    
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}


- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView
{
    if([AppDelegate debugging]) NSLog(@"%s",__PRETTY_FUNCTION__);
    
    return [DetailsSingleton getSections];
}

-(NSString *)tableView:(UITableView *)tableView titleForHeaderInSection:(NSInteger)section
{
    return [DetailsSingleton getHeaderAt:section];
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    if([AppDelegate debugging]) NSLog(@"%s",__PRETTY_FUNCTION__);
    
    return [DetailsSingleton getRowsAt:section];
}

// Customize the appearance of table view cells.
- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    if([AppDelegate debugging]) NSLog(@"%s",__PRETTY_FUNCTION__);
    
    static NSString *CellIdentifier = @"Cell";
    
    UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:CellIdentifier];
    if (cell == nil) {
        cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:CellIdentifier];
        cell.accessoryType = UITableViewCellAccessoryDisclosureIndicator;
    }
    
    int row = indexPath.row;
    int section = indexPath.section;
    Detail * event = [DetailsSingleton getDetailAt:section and:row];
    
    cell.textLabel.text = event.summary;
    cell.detailTextLabel.text = event.description;
    
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
     
    int row = indexPath.row;
    int section = indexPath.section;
    Detail * event = [DetailsSingleton getDetailAt:section and:row];

    [self update:event];
}
@end
