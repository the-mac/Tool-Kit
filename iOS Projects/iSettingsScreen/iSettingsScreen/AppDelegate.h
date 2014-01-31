//
//  AppDelegate.h
//  iSettingsScreen
//
//  Created by Christopher Miller on 6/27/13.
//  Copyright (c) 2013 Christopher Miller. All rights reserved.
//

#import <UIKit/UIKit.h>

@class SettingsScreen;

@interface AppDelegate : UIResponder <UIApplicationDelegate>

@property (strong, nonatomic) UIWindow *window;

@property (strong, nonatomic) SettingsScreen *viewController;

@end
