//
//  DetailsSingleton.m
//  iCalendarScreen
//
//  Created by Christopher Miller on 6/24/13.
//  Copyright (c) 2013 Christopher Miller. All rights reserved.
//

#import "Detail.h"
#import "AppDelegate.h"
#import "DetailsSingleton.h"

@implementation DetailsSingleton
static NSMutableDictionary * data;
static NSMutableDictionary * indexPair;

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
    if([AppDelegate debugging]) NSLog(DEUBBING_OUTPUT,__PRETTY_FUNCTION__);
    
    if(indexPair == nil) indexPair = [[NSMutableDictionary alloc]init];
    if(data == nil) data = [[NSMutableDictionary alloc]init];
    
    Detail * detail = [[Detail alloc]init];
    detail.summary = s;
    detail.who = w;
    detail.date = d;
    detail.location = l;
    detail.desc = de;
    detail.contactName = cn;
    detail.contactEmail = ce;
    detail.contactPhone = cp;
    
    
    NSMutableArray * arr = [data objectForKey:d];
    
    if(arr == nil) {
        
        int index = [data count];
        detail.index = index;
        
        arr = [[NSMutableArray alloc]init];
        NSString * key = [NSString stringWithFormat:@"%d,%d",[data count],0];
        
        [indexPair setValue:d forKey:key];
        [data setValue:arr forKey:d];
    }
    else
    {
        Detail * detail = [arr objectAtIndex:0];
        int index = detail.index;
        detail.index = index;
//        
//        NSString * existingKey = [NSString stringWithFormat:@"%d,%d",[data count],0];
//        NSArray * localArray = [indexPair allKeys];
//        int index = [localArray indexOfObject:existingKey];
        
        NSString * key = [NSString stringWithFormat:@"%d,%d",index,[arr count]];
        
        [indexPair setValue:d forKey:key];
        [data setValue:arr forKey:d];
    }
    
    [arr addObject:detail];
    
    return detail;
}

+(Detail*) getDetailAt:(NSInteger) section and:(NSInteger) row
{
    if([AppDelegate debugging]) NSLog(DEUBBING_OUTPUT,__PRETTY_FUNCTION__);
    
    if(indexPair == nil) indexPair = [[NSMutableDictionary alloc]init];
    if(data == nil) data = [[NSMutableDictionary alloc]init];
    NSString * key = [NSString stringWithFormat:@"%d,%d",section,row];
    
    NSString * keyFromSectionAndRow = [indexPair objectForKey:key];
    NSMutableArray * sectionArray = [data objectForKey:keyFromSectionAndRow];
    Detail* detail = [sectionArray objectAtIndex:row];
    
    if(detail != nil && row > 0) NSLog(@"Detail: %@",detail);
    return detail;
}

+(NSString*) getHeaderAt:(NSInteger) section
{
//    NSArray * sectionKeys = [data allKeys];
    //    NSString * key = [sectionKeys objectAtIndex:section];
    //    NSMutableArray * sectionArray = [data objectForKey:keyFromSectionAndRow];
    
    NSString * key = [NSString stringWithFormat:@"%d,%d",section,0];
    NSString * keyFromSectionAndRow = [indexPair objectForKey:key];

    return keyFromSectionAndRow;
}

+(NSInteger) getSections
{
    return [data count];
}

+(NSInteger) getRowsAt:(NSInteger) section
{
//    NSArray * sectionKeys = [data allKeys];
//    NSString * key = [sectionKeys objectAtIndex:section];
//    
    //    NSInteger number = [[data objectForKey:key] count];
    
    NSString * key = [NSString stringWithFormat:@"%d,%d",section,0];
    NSString * keyFromSectionAndRow = [indexPair objectForKey:key];
    
    NSMutableArray * sectionArray = [data objectForKey:keyFromSectionAndRow];
    NSInteger number = [sectionArray count];
    
    return number;
}



//    NSArray * sectionKeys = [data allKeys];
//    NSString * key = [sectionKeys objectAtIndex:section];

//    // GRAB TODAY'S DATE
//    NSDate *now = [NSDate date];
//    // GRAB DATE FROM DETAIL
//    NSDate *next = [NSDate date];

//    NSInteger index = [self daysBetween:now and:next];

//+(void) sort
//{
//    NSArray *sortedKeys = [data keysSortedByValueUsingComparator: ^(id obj1, id obj2) {
//        //get the key value.
//        Detail *s1 = (Detail*)obj1[0];
//        Detail *s2 = (Detail*)obj2[0];
//
//        //Convert NSString to NSDate:
//        NSDateFormatter *dateFormatter = [[NSDateFormatter alloc] init];
//
//        //Specify only 1 M for month, 1 d for day and 1 h for hour
//        [dateFormatter setDateFormat:@"EEEE M/d/yy"];
//        NSDate *d1 = [dateFormatter dateFromString:s1.date];
//        NSDate *d2 = [dateFormatter dateFromString:s2.date];
//
//        if ([d1 compare:d2] == NSOrderedAscending)
//            return (NSComparisonResult)NSOrderedAscending;
//        if ([d1 compare:d2] == NSOrderedDescending)
//            return (NSComparisonResult)NSOrderedDescending;
//        return (NSComparisonResult)NSOrderedSame;
//    }];
////    NSArray *sortedValues = [[data allValues] sortedArrayUsingSelector:@selector(compare:)];
//}

//+ (NSInteger)daysBetween:(NSDate *)dt1 and:(NSDate *)dt2 {
//    NSUInteger unitFlags = NSDayCalendarUnit;
//    NSCalendar* calendar = [NSCalendar currentCalendar];
//    NSDateComponents *components = [calendar components:unitFlags fromDate:dt1 toDate:dt2 options:0];
//    NSInteger daysBetween = abs([components day]);
//    return daysBetween+1;
//}
@end
