//
//  DetailsSingleton.m
//  iCalendarScreen
//
//  Created by Christopher Miller on 6/24/13.
//  Copyright (c) 2013 Christopher Miller. All rights reserved.
//

#import "AppDelegate.h"
#import "DetailsSingleton.h"

@implementation DetailsSingleton
static NSMutableDictionary * data;

+(NSString*) getHeaderAt:(NSInteger) section
{
    NSArray * sectionKeys = [data allKeys];
    NSString * key = [sectionKeys objectAtIndex:section];
    return key;
}

+(NSInteger) getSections
{
    return [data count];
}

+(NSInteger) getRowsAt:(NSInteger) section
{
    NSArray * sectionKeys = [data allKeys];
    NSString * key = [sectionKeys objectAtIndex:section];
    
    NSInteger number = [[data objectForKey:key] count];
    return number;
}

+(Detail*) getDetailAt:(NSInteger) section and:(NSInteger) row
{
    if([AppDelegate debugging]) NSLog(@"%s",__PRETTY_FUNCTION__);
    
    if(data == nil) data = [[NSMutableDictionary alloc]init];
    
    NSArray * sectionKeys = [data allKeys];
    NSString * key = [sectionKeys objectAtIndex:section];
    
    NSMutableArray * sectionArray = [data objectForKey:key];
    Detail* detail = [sectionArray objectAtIndex:row];
    
    return detail;
}

/*
 [DetailsSingleton
 create:title        // SUMMARY
 with:attendees    // ATTENDEES
 and:date            // DATE
 and:location        // LOCATION
 and:longDescription // DESCRIPTION
 and:contactName     // CONTACT NAME
 with:contactEmail   // CONTACT EMAIL
 and:contactPhone    // CONTACT PHONE
 ];
 */
+(Detail*)
    create:(NSString*) s // SUMMARY
    with:(NSString*) w   // ATTENDEES
    and:(NSString*) d    // DATE
    and:(NSString*) l    // LOCATION
    and:(NSString*) de   // DESCRIPTION
    and:(NSString*) cn   // CONTACT NAME
    with:(NSString*) ce  // CONTACT EMAIL
    and:(NSString*) cp   // CONTACT PHONE
{
    if([AppDelegate debugging]) NSLog(@"%s",__PRETTY_FUNCTION__);
    
    if(data == nil) data = [[NSMutableDictionary alloc]init];
    
    Detail * detail = [[Detail alloc]init];
    detail.summary = s;
    detail.who = w;
    detail.date = d;
    detail.location = l;
    detail.description = de;
    detail.contactName = cn;
    detail.contactEmail = ce;
    detail.contactPhone = cp;
    
    NSMutableArray * arr = [data objectForKey:d];
    if(arr == nil) {
        arr = [[NSMutableArray alloc]init];
        [data setValue:arr forKey:d];
    }
    
    [arr addObject:detail];
    
    return detail;
    
    //    // GRAB TODAY'S DATE
    //    NSDate *now = [NSDate date];
    //    // GRAB DATE FROM DETAIL
    //    NSDate *next = [NSDate date];
    
    //    NSInteger index = [self daysBetween:now and:next];
}

//+ (NSInteger)daysBetween:(NSDate *)dt1 and:(NSDate *)dt2 {
//    NSUInteger unitFlags = NSDayCalendarUnit;
//    NSCalendar* calendar = [NSCalendar currentCalendar];
//    NSDateComponents *components = [calendar components:unitFlags fromDate:dt1 toDate:dt2 options:0];
//    NSInteger daysBetween = abs([components day]);
//    return daysBetween+1;
//}
@end
