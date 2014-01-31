//
//  Copyright 2009 Scott Lawrence. All rights reserved.
//  This is heavily based on UICatalog from Apple's samples
//
//  This module (and submodules) can be used for any purpose in any app
//  no warranty is expressed or implied.  If it works for you, great.
//  If it breaks, bummer.
//

#import <UIKit/UIKit.h>
#import "EditableTableViewCell.h"

// cell identifier for this custom cell
extern NSString *kCellTextField_ID;

@interface CellTextField : EditableTableViewCell <UITextFieldDelegate>
{
    UITextField *view;
}

@property (nonatomic, retain) UITextField *view;

@end
