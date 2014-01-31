//
//  Recipe.m
//  iDetailsScreen
//
//  Created by Christopher Miller on 6/14/13.
//  Copyright (c) 2013 Christopher Miller. All rights reserved.
//

#import "Recipe.h"

@implementation Recipe

static NSArray *propertyNames;
static NSArray *recipeDictionaries;
/*
 Initialize a recipe object.
 */
- (id)init {
	if ((self = [super init])) {
        if(recipeDictionaries == nil) recipeDictionaries = [[NSArray alloc] initWithContentsOfFile:[[NSBundle mainBundle] pathForResource:@"Recipes" ofType:@"plist"]];
        if(propertyNames == nil) propertyNames = [[NSArray alloc] initWithObjects:@"name", @"description", @"prepTime", @"instructions", @"ingredients", nil];
		self.ingredients = [[NSMutableArray alloc] init];
        
        NSDictionary * recipeDictionary = [recipeDictionaries objectAtIndex:0];
        for (NSString *property in propertyNames) {
            [self setValue:[recipeDictionary objectForKey:property] forKey:property];
        }
        
        NSString *imageName = [recipeDictionary objectForKey:@"imageName"];
        self.image = [UIImage imageNamed:imageName];
        
        imageName = [[imageName stringByDeletingPathExtension] stringByAppendingString:@"_thumbnail.png"];
        self.thumbnailImage = [UIImage imageNamed:imageName];
        
	}
	return self;
}


// Ingredients array setter
- (void)setIngredients:(NSMutableArray *)newIngredients {
	if (self.ingredients != newIngredients) {
		//[self.ingredients release];
		self.ingredients = [newIngredients mutableCopy];
	}
}

/*
 This method suplies an html representation of the recipe according to
 the way it will be displayed when printed. The iOS Printing architecture
 accepts an html representation via the UIMarkupTextPrintFormatter
 */
- (NSString *)htmlRepresentation {
    NSMutableString *body = [NSMutableString stringWithString:@"<!DOCTYPE html><html><body>"];
    
    if ([[self ingredients] count] > 0) {
        [body appendString:@"<h2>Ingredients</h2>"];
        [body appendString:@"<ul>"];
        for (NSDictionary *ingredient in [self ingredients]) {
            [body appendFormat:@"<li>%@ %@</li>", [ingredient objectForKey:@"amount"], [ingredient objectForKey:@"name"]];
        }
        [body appendString:@"</ul>"];
    }
    
    if ([[self instructions] length] > 0) {
        [body appendString:@"<h2>Instructions</h2>"];
        [body appendFormat:@"<p>%@</p>", [self instructions]];
    }
    
    [body appendString:@"</body></html>"];
    return body;
}

/*
 Aggregate some information in a custom way for printing.
 Show the description, and Prep Time under the Recipe Title.
 */
- (NSString *)aggregatedInfo {
    
    NSMutableArray *infoPieces = [NSMutableArray array];
    if (self.description.length > 0) {
        [infoPieces addObject:self.description];
    }
    if (self.prepTime.length > 0) {
        [infoPieces addObject:[NSString stringWithFormat:@"Preparation Time: %@", self.prepTime]];
    }
    
    return [infoPieces componentsJoinedByString:@"\n"];
}

@end
