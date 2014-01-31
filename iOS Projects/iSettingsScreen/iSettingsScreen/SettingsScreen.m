//
//  ViewController.m
//  iSettingsScreen
//
//  Created by Christopher Miller on 6/27/13.
//  Copyright (c) 2013 Christopher Miller. All rights reserved.
//

#import "AppDelegate.h"
#import "SettingsScreen.h"

@interface SettingsScreen ()

@end

@implementation SettingsScreen

- (void)viewDidLoad
{
    NSLog(@"%s",__PRETTY_FUNCTION__);
    [super viewDidLoad];
        
    self.llamaSettings = [[LlamaSettings alloc] initWithPlist:@"Settings.plist"];
	self.llamaSettings.delegate = self;
    
	[self.tableView setDataSource:self.llamaSettings];
	[self.tableView setDelegate:self.llamaSettings];
    
    UINavigationBar *navBar =  [[UINavigationBar alloc] initWithFrame:CGRectMake(0.0f, 0.0f, 320.0f, 44.0f)];
    UIBarButtonItem *buttonItem = [[UIBarButtonItem alloc] initWithBarButtonSystemItem:UIBarButtonSystemItemDone target:self action:@selector(dismissSettings)];
    UINavigationItem *navigationItem = [[UINavigationItem alloc] initWithTitle:@"Settings"];
    
    navigationItem.leftBarButtonItem = buttonItem;
    [navBar pushNavigationItem:navigationItem animated:NO];
    [self.tableView addSubview:navBar];
}

- (void)didReceiveMemoryWarning { [super didReceiveMemoryWarning]; }

- (void) settingsChanged:(LlamaSettings *)theSettings
{
    NSLog(@"%s",__PRETTY_FUNCTION__);
	NSLog( @"Delegate received 'settingsChanged' message %@", theSettings);
}

- (void) buttonPressed:(NSString *)buttonKey inSettings:(LlamaSettings *)theSettings
{
    NSLog(@"%s",__PRETTY_FUNCTION__);
	NSLog( @"Button Pressed: %@  %@", buttonKey, theSettings);
}


- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    NSLog(@"%s",__PRETTY_FUNCTION__);
    static NSString *CellIdentifier = @"Cell";
    
    UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:CellIdentifier];
    if (cell == nil) cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:CellIdentifier];
    
    return cell;
}

@end
