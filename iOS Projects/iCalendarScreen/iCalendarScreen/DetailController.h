//
//  DetailController.h
//  iCalendarScreen
//
//  Created by Christopher Miller on 6/21/13.
//  Copyright (c) 2013 Christopher Miller. All rights reserved.
//

#import "Detail.h"
#import "ViewController.h"
#import <UIKit/UIKit.h>

@interface DetailController : ViewController<UITableViewDataSource,UITableViewDelegate>
@property (strong, nonatomic) Detail *detail;
@property (strong, nonatomic) UINavigationItem *navigation;
-(void) updateDetails:(Detail *) detail;
@end
