//
//  Copyright 2009 Scott Lawrence. All rights reserved.
//  This is heavily based on UICatalog from Apple's samples
//
//  This module (and submodules) can be used for any purpose in any app
//  no warranty is expressed or implied.  If it works for you, great.
//  If it breaks, bummer.
//

#import "EditableTableViewCell.h"

@implementation EditableTableViewCell

// Instruct the compiler to create accessor methods for the property.
// It will use the internal variable with the same name for storage.
//@synthesize delegate;
@synthesize isInlineEditing;

// To be implemented by subclasses. 
- (void)stopEditing
{}

@end
