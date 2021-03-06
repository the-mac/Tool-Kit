//
//  Communicator.h
//  iOS Client
//
//  Created by Christopher Miller on 5/26/15.
//  Copyright (c) 2015 The MAC. All rights reserved.
//  Source: https://gist.github.com/rjungemann/446256

#ifndef AClient_Communicator_h
#define AClient_Communicator_h

#import <Foundation/Foundation.h>
#include <ifaddrs.h>
#include <arpa/inet.h>

@class NetworkClient;
@interface Communicator : NSObject <NSStreamDelegate> {
@public
    
    NSString *host;
    int port;
}
+ (NSString *)getLocalIpAddress;
- (void)setup:(NetworkClient*) client;
- (void)open;
- (void)close;
- (void)stream:(NSStream *)stream handleEvent:(NSStreamEvent)event;
- (NSString *) readIn;
- (void)writeOut:(NSString *)s;

@end

#endif
