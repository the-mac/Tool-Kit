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
//@property (strong, nonatomic) NSArray *sections;
@property (weak, nonatomic) IBOutlet UILabel *summary;
@property (weak, nonatomic) IBOutlet UILabel *who;
@property (weak, nonatomic) IBOutlet UILabel *date;
@property (weak, nonatomic) IBOutlet UILabel *location;
@property (weak, nonatomic) IBOutlet UILabel *description;
@property (weak, nonatomic) IBOutlet UILabel *contactName;
@property (weak, nonatomic) IBOutlet UILabel *contactEmail;
@property (weak, nonatomic) IBOutlet UILabel *contactPhone;
-(void) updateDetails:(Detail *) detail;
@end
