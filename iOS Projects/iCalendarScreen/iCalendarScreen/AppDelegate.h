//
//  AppDelegate.h
//  iCalendarScreen
//
//  Created by Christopher Miller on 6/14/13.
//  Copyright (c) 2013 Christopher Miller. All rights reserved.
//

#import <UIKit/UIKit.h>
#define DEUBBING_OUTPUT @"\n\n%s\n\n"

@class ViewController;

@interface AppDelegate : UIResponder <UIApplicationDelegate>

@property (strong, nonatomic) UIWindow *window;

@property (strong, nonatomic) ViewController *viewController;

+(BOOL) debugging;

@end
