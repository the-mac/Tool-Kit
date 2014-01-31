//
//  ViewController.h
//  iSettingsScreen
//
//  Created by Christopher Miller on 6/27/13.
//  Copyright (c) 2013 Christopher Miller. All rights reserved.
//


#import <UIKit/UIKit.h>
#import "LlamaSettings.h"

@interface SettingsScreen : UITableViewController<LlamaSettingsDelegate>
@property (strong, nonatomic) LlamaSettings *llamaSettings;
@property (strong, nonatomic) IBOutlet UITableView *tableView;


@end