//
//  DetailsSingleton.h
//  iCalendarScreen
//
//  Created by Christopher Miller on 6/24/13.
//  Copyright (c) 2013 Christopher Miller. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "Detail.h"

@interface DetailsSingleton : NSObject
+(NSInteger) getSections;
+(NSString*) getHeaderAt:(NSInteger) section;
+(NSInteger) getRowsAt:(NSInteger) section;
+(Detail*) getDetailAt:(NSInteger) section and:(NSInteger) row;
+(Detail*) create:(NSString*) s with:(NSString*) w
and:(NSString*) d and:(NSString*) l and:(NSString*) de
and:(NSString*) cn with:(NSString*) ce and:(NSString*) cp;
@end
