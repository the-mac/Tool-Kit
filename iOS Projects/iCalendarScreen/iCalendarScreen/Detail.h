//
//  Detail.h
//  iCalendarScreen
//
//  Created by Christopher Miller on 6/21/13.
//  Copyright (c) 2013 Christopher Miller. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface Detail : NSObject
@property (nonatomic) NSInteger index;
@property (strong, nonatomic) NSString * summary;
@property (strong, nonatomic) NSString * who;
@property (strong, nonatomic) NSString * date;
@property (strong, nonatomic) NSString * location;
@property (strong, nonatomic) NSString * desc;
@property (strong, nonatomic) NSString * contactName;
@property (strong, nonatomic) NSString * contactEmail;
@property (strong, nonatomic) NSString * contactPhone;
@end
