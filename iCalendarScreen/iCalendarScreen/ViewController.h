//
//  ViewController.h
//  iCalendarScreen
//
//  Created by Christopher Miller on 6/14/13.
//  Copyright (c) 2013 Christopher Miller. All rights reserved.
//

#import <UIKit/UIKit.h>

@class DetailController;
@interface ViewController : UIViewController<UITableViewDelegate,UITableViewDataSource>
@property (weak, nonatomic) IBOutlet UILabel *summary;
@property (weak, nonatomic) IBOutlet UILabel *who;
@property (weak, nonatomic) IBOutlet UILabel *date;
@property (weak, nonatomic) IBOutlet UILabel *location;
@property (weak, nonatomic) IBOutlet UILabel *description;
@property (weak, nonatomic) IBOutlet UILabel *contactName;
@property (weak, nonatomic) IBOutlet UILabel *contactEmail;
@property (weak, nonatomic) IBOutlet UILabel *contactPhone;
@property (strong, nonatomic) DetailController *theDetailScreen;
//@property (strong, nonatomic) NSDictionary* eventsDictionary;
//@property (strong, nonatomic) NSArray* events;
@end
