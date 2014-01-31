//
//  ViewController.h
//  iDetailsScreen
//
//  Created by Christopher Miller on 6/14/13.
//  Copyright (c) 2013 Christopher Miller. All rights reserved.
//


#import <UIKit/UIKit.h>

#import <AddressBookUI/AddressBookUI.h>

@class Recipe;

@interface RecipeDetailViewController : UIViewController<UITableViewDataSource,UITableViewDelegate>{
//	UIView *tableHeaderView;
//	UIButton *photoButton;
//	UILabel *nameLabel;
}

-(RecipeDetailViewController*)initWithRecipe:(Recipe*)aRecipe;

@property (nonatomic, strong) Recipe *recipe;
@property (nonatomic, strong) IBOutlet UITableView *tableView;
//@property (nonatomic, strong) IBOutlet UIView *tableHeaderView;
//@property (nonatomic, strong) IBOutlet UIButton *photoButton;
@property (nonatomic, strong) IBOutlet UILabel *nameLabel;

@end