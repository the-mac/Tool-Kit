//
//  LlamaSettings.h
//
//  Created by Scott Lawrence on 3/13/09.
//  Copyright 2009 Scott Lawrence. All rights reserved.
//
//  This module (and submodules) can be used for any purpose in any app
//  no warranty is expressed or implied.  If it works for you, great.
//  If it breaks, bummer.
//
//  This is heavily based on UICatalog from Apple's samples (as can be gleaned from the TableCells
// 
//  For a current list of what works and what doesn't, refer to LlamaSettingsNotes.txt
//

#import <Foundation/Foundation.h>
#import "WebViewController.h"
#import "ColorHelper.h"
#import "LSColorDisplay.h"

@protocol LlamaSettingsDelegate;

@interface LlamaSettings : NSObject <UITableViewDelegate, UITableViewDataSource, UITextFieldDelegate, 
UIActionSheetDelegate, BLColorDisplayDelegate>

{
	id <LlamaSettingsDelegate> delegate;
	NSDictionary * theDictionary;
	NSMutableDictionary * theWidgets;
	NSMutableDictionary * theWebViews;
	UIViewController * viewController;
	
	WebViewController * wvc;
	
	BOOL valid;
	BOOL readyForSaving;
	
//	NSAutoreleasePool *nsap;
}

@property (nonatomic, assign) id delegate;
@property (nonatomic, assign) UIViewController * viewController;

@property (nonatomic, assign) BOOL valid;

+ (LlamaSettings *)sharedSettings;
+ (LlamaSettings *)sharedSettingsFromPlist:(NSString *)plistName;

- (id) init;
- (id) initWithPlist:(NSString *)plistName;
- (void) loadHeirarchyFromDefaultPlist;
- (void) loadHeirarchyFromPlist:(NSString *)plistName;
- (void) loadSettingsFromSystem;
- (void) saveSettingsToSystem;
- (void) saveColorSettingsToSystem;

- (void) enableItem:(NSString *) itemKey;
- (void) disableItem:(NSString *) itemKey;

@end

// and our callback delegate
@protocol LlamaSettingsDelegate<NSObject>
- (void) settingsChanged:(LlamaSettings *)ls;
- (void) buttonPressed:(NSString *)buttonKey inSettings:(LlamaSettings *)ls;
@end
