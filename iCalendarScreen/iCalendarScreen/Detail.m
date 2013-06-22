//
//  Detail.m
//  iCalendarScreen
//
//  Created by Christopher Miller on 6/21/13.
//  Copyright (c) 2013 Christopher Miller. All rights reserved.
//

#import "Detail.h"

@implementation Detail

-(Detail*) init
{
    self = [super init];
    self.summary = @"Default Text";
    self.who = @"Default Text";
    self.date = @"Default Text";
    self.location = @"Default Text";
    self.description = @"Default Text";
    self.contactName = @"Default Text";
    self.contactEmail = @"Default Text";
    self.contactPhone = @"Default Text";
    return self;
}

-(NSString*) description
{   
    return [NSString stringWithFormat:@"%@ %@",self.summary,self.date];
}

@end
