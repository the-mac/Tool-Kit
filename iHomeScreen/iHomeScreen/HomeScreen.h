//
//  ViewController.h
//  iHomeScreen
//
//  Created by Christopher Miller on 6/12/13.
//  Copyright (c) 2013 Christopher Miller. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface HomeScreen : UIViewController

@property (strong, nonatomic) NSObject *theShareScreen;
@property (strong, nonatomic) NSObject *theSettingsScreen;
@property (strong, nonatomic) NSObject *locations;
@property (strong, nonatomic) NSObject *calendar;

@property (weak, nonatomic) IBOutlet UIButton *hornetBuzzButton;
@property (weak, nonatomic) IBOutlet UIButton *shareNestButton;
@property (weak, nonatomic) IBOutlet UIButton *settingsButton;

@property (weak, nonatomic) IBOutlet UIImageView *logoImageView;

- (IBAction)onClick:(id)sender;
@end
