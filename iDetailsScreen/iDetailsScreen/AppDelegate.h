//
//  AppDelegate.h
//  iDetailsScreen
//
//  Created by Christopher Miller on 6/14/13.
//  Copyright (c) 2013 Christopher Miller. All rights reserved.
//

#import <UIKit/UIKit.h>

@class RecipeDetailViewController;

@interface AppDelegate : UIResponder <UIApplicationDelegate>

@property (strong, nonatomic) UIWindow *window;

@property (strong, nonatomic) RecipeDetailViewController *viewController;

@end
