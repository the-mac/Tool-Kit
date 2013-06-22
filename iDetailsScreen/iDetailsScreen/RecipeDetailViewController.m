//
//  ViewController.m
//  iDetailsScreen
//
//  Created by Christopher Miller on 6/14/13.
//  Copyright (c) 2013 Christopher Miller. All rights reserved.
//

#import "RecipeDetailViewController.h"
#import "Recipe.h"

@interface RecipeDetailViewController ()
- (IBAction)showPhoto;
@end

@implementation RecipeDetailViewController

//@synthesize recipe;
//@synthesize tableHeaderView;
//@synthesize photoButton;
//@synthesize nameLabel;

// Define constants for the table view sections
enum{
    INGREDIENTS_SECTION, // 0
    INSTRUCTIONS_SECTION, // 1
    TotalNumberOfSections // 2
};

// Custom initialize that makes the recipe available to this table view controller
-(RecipeDetailViewController*)initWithRecipe:(Recipe*)aRecipe;
{
    self = [super init];//]WithStyle:UITableViewStyleGrouped];
    if(self){
        self.recipe = aRecipe;
        self.navigationItem.title = @"Recipe";
    }
    return self;
}

#pragma mark -
#pragma mark UIViewController

// Release ownership.
- (void) viewDidUnload;
{
//    self.tableHeaderView = nil;
//    self.photoButton = nil;
    self.nameLabel = nil;
}


- (void)viewDidLoad {
//	if (self.tableHeaderView == nil) {
//        
//        // Load the header view that shows the recipe name and thumbnail
//		[[NSBundle mainBundle] loadNibNamed:@"DetailHeaderView" owner:self options:nil];
//		self.tableHeaderView.backgroundColor = [UIColor groupTableViewBackgroundColor];
//        
//        // Set it onto the tables header view property
//		self.tableView.tableHeaderView = self.tableHeaderView;
//        self.tableView.allowsSelectionDuringEditing = YES;
//	}
}

- (void)viewWillAppear:(BOOL)animated {
	[super viewWillAppear:animated];
	
    // to update recipe type and ingredients on return
	[self.tableView reloadData];
    
    // Skin the photo button with the recipe thumbnail - displayed up in the header view
	//[photoButton setImage:self.recipe.thumbnailImage forState:UIControlStateNormal];
    // Set the header view text
	self.nameLabel.text = self.recipe.name;
    
    // Set the recipe name as the nav bar title
	self.title = self.recipe.name;
    
    // Provide a back button to go back to the recipes
    UIBarButtonItem *backButton = [[UIBarButtonItem alloc] initWithTitle:@"Recipe" style:UIBarButtonItemStylePlain target:nil action:nil];
    self.navigationItem.backBarButtonItem = backButton;
}

// Support all orientations except portrait-upside down
- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation {
    return (interfaceOrientation != UIInterfaceOrientationPortraitUpsideDown);
}

#pragma mark -
#pragma mark UITableView Delegate/Datasource

// Provide the architecture with the number of sections in this table view. 2 for this sample.
- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView {
    return TotalNumberOfSections;
}

// Define a title for the argument section
- (NSString *)tableView:(UITableView *)tableView titleForHeaderInSection:(NSInteger)section {
    switch (section) {
            // Only define a title for the ingredients section
        case INGREDIENTS_SECTION:
            return @"Ingredients";
		default:
			return nil;
    }
}

// Provide the architecture with the number of rows for the argument section
- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
	switch (section) {
		case INSTRUCTIONS_SECTION:
			return 1;
		case INGREDIENTS_SECTION:
			return [self.recipe.ingredients count];
		default:
			return 0;
	}
}

// Build the cell for the argument cell index.
// See the TableViewSuite sample for more information about creating table view cells.
- (UITableViewCell *)ingredientsCellAtIndex:(NSUInteger)ingredientIdx forTableView:(UITableView *)tableView {
    static NSString *IngredientsCellIdentifier = @"IngredientsCell";
    const int AMOUNT_TAG =  1;
    UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:IngredientsCellIdentifier];
    if (cell == nil) {
        cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:IngredientsCellIdentifier];
        
        UILabel *amountLabel = [[UILabel alloc] initWithFrame:CGRectZero];
        [amountLabel setAutoresizingMask:UIViewAutoresizingFlexibleLeftMargin];
        amountLabel.tag = AMOUNT_TAG;
//        amountLabel.textAlignment = UITextAlignmentRight;
        amountLabel.textColor = [UIColor colorWithRed:50.0/255.0 green:79.0/255.0 blue:133.0/255.0 alpha:1.0];
        amountLabel.highlightedTextColor = [UIColor whiteColor];
        [cell.contentView insertSubview:amountLabel aboveSubview:cell.textLabel];
        amountLabel.backgroundColor = [UIColor clearColor];
    }
    
    UILabel *amountLabel = (UILabel *)[cell viewWithTag:AMOUNT_TAG];
    
    NSDictionary *ingredients = [self.recipe.ingredients objectAtIndex:ingredientIdx];
    cell.textLabel.text = [ingredients objectForKey:@"name"];
    amountLabel.text = [ingredients objectForKey:@"amount"];
    
    CGSize desiredSize = [amountLabel sizeThatFits:CGSizeMake(160.0,32.0)];
    CGRect computedFrame = {CGPointMake(CGRectGetMaxX(cell.contentView.bounds) - desiredSize.width - 10.0, floorf(CGRectGetMidY(cell.contentView.bounds) - desiredSize.height / 2.0)), desiredSize};
    amountLabel.frame = computedFrame;
    
    return cell;
}

// if the argument cell index path (section, row) is within the ingredient section, return that cell
- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath {
	if (indexPath.section == INGREDIENTS_SECTION) {
		return [self ingredientsCellAtIndex:indexPath.row forTableView:tableView];
    }
    
	static NSString *MyIdentifier = @"MyIdentifier";
	
	UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:MyIdentifier];
	if (cell == nil) {
		cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:MyIdentifier];
	}
	
	NSString *text = nil;
    switch (indexPath.section) {
		case INSTRUCTIONS_SECTION: // instructions
            cell.accessoryType = UITableViewCellAccessoryDisclosureIndicator;
			text = @"Instructions";
			break;
		default:
            cell.accessoryType = UITableViewCellAccessoryNone;
			break;
	}
    
	if (text) {
        cell.textLabel.text = text;
    }
	return cell;
}

- (NSIndexPath *)tableView:(UITableView *)tableView willSelectRowAtIndexPath:(NSIndexPath *)indexPath {
    // Don't allow ingredients to be selected
    NSInteger section = indexPath.section;
    if (section == INGREDIENTS_SECTION) {
        [tableView deselectRowAtIndexPath:indexPath animated:YES];
        return nil;
    }
    return indexPath;
}

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath {
    // If the intructions section is tapped, navigate to the instructions view controller
	if (indexPath.section == INSTRUCTIONS_SECTION) {
//		InstructionsViewController *nextViewController = [[InstructionsViewController alloc] initWithNibName:@"RecipeInstructionsView" bundle:nil];
        // pass the recipe to the instructions view controller
//		((InstructionsViewController *)nextViewController).recipe = recipe;
//        [self.navigationController pushViewController:nextViewController animated:YES];
    }
}

// Touch handler for tapping the photo in the header view.
- (IBAction)showPhoto {
    // Navigate to the recipe photo view controller to show a large photo for the recipe.
//	RecipePhotoViewController *recipePhotoViewController = [[RecipePhotoViewController alloc] init];
//	recipePhotoViewController.recipe = recipe;
//	[self.navigationController pushViewController:recipePhotoViewController animated:YES];
//	[recipePhotoViewController release];
}

@end
