//
//  Communicator.h
//  iOS Swift Client
//
//  Created by Christopher Miller on 6/7/15.
//  Copyright (c) 2015 The MAC. All rights reserved.
//

#ifndef iOS_Swift_Client_Communicator_h
#define iOS_Swift_Client_Communicator_h

#import <Foundation/Foundation.h>
#include <ifaddrs.h>
#include <arpa/inet.h>

@class NetworkClient;
@interface Communicator : NSObject <NSStreamDelegate>

@property (nonatomic, retain) NSString* host;
@property (nonatomic) int port;
@property (nonatomic) BOOL connected;

+ (NSString *)getLocalIpAddress;
- (void)setup:(NetworkClient*) client;
- (void)open;
- (void)close;
- (void)stream:(NSStream *)stream handleEvent:(NSStreamEvent)event;
- (NSString *) readIn;
- (void)writeOut:(NSString *)s;

@end

#endif
