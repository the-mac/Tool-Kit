//
//  AppDelegate.h
//  iHomeScreen
//
//  Created by Christopher Miller on 6/12/13.
//  Copyright (c) 2013 Christopher Miller. All rights reserved.
//

#import <UIKit/UIKit.h>

@class HomeScreen;

@interface AppDelegate : UIResponder <UIApplicationDelegate>

@property (strong, nonatomic) UIWindow *window;

@property (strong, nonatomic) HomeScreen *viewController;

@end
