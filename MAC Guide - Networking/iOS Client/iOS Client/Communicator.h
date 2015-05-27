//
//  Communicator.h
//  AClient
//
//  Created by Christopher Miller on 5/26/15.
//  Copyright (c) 2015 The MAC. All rights reserved.
//  Source: https://gist.github.com/rjungemann/446256

#ifndef AClient_Communicator_h
#define AClient_Communicator_h

#import <Foundation/Foundation.h>

@interface Communicator : NSObject <NSStreamDelegate> {
@public
    
    NSString *host;
    int port;
}

- (void)setup;
- (void)open;
- (void)close;
- (void)stream:(NSStream *)stream handleEvent:(NSStreamEvent)event;
- (NSString *) readIn;
- (void)writeOut:(NSString *)s;

@end

#endif
