//
//  Communicator.m
//  AClient
//
//  Created by Christopher Miller on 5/26/15.
//  Copyright (c) 2015 The MAC. All rights reserved.
//  Source: https://gist.github.com/rjungemann/446256

#import <Foundation/Foundation.h>
#import "Communicator.h"
#import "NetworkClient.h"

CFReadStreamRef readStream;
CFWriteStreamRef writeStream;

NSInputStream *inputStream;
NSOutputStream *outputStream;
NetworkClient *networkClient;

@implementation Communicator

- (void)setup :(NetworkClient*) client {
    networkClient = client;
    NSURL *url = [NSURL URLWithString:host];
    
    CFStreamCreatePairWithSocketToHost(kCFAllocatorDefault, (__bridge CFStringRef)[url host], port, &readStream, &writeStream);
    
    if(!CFWriteStreamOpen(writeStream)) {
        
        return;
    }
    
    [self open];
}

- (void)open {
    
    inputStream = (__bridge NSInputStream *)readStream;
    outputStream = (__bridge NSOutputStream *)writeStream;
    
    [outputStream setDelegate:self];
    
    [outputStream scheduleInRunLoop:[NSRunLoop currentRunLoop] forMode:NSDefaultRunLoopMode];
    
    [inputStream open];
    [outputStream open];
}

- (void)close {
    
    [inputStream close];
    [outputStream close];
    
    [outputStream removeFromRunLoop:[NSRunLoop currentRunLoop] forMode:NSDefaultRunLoopMode];
    
    [inputStream setDelegate:nil];
    [outputStream setDelegate:nil];
    
    inputStream = nil;
    outputStream = nil;
}

- (void)stream:(NSStream *)stream handleEvent:(NSStreamEvent)event {
    
    switch(event) {
        case NSStreamEventHasSpaceAvailable: {
            if(stream == outputStream) {
                NSLog(@"outputStream is ready.\n\n");
                [networkClient enableConnection];
            }
            
            break;
        }
        default: {
            NSLog(@"Stream is sending an Event: %i", event);
            if(event == NSStreamEventErrorOccurred) [networkClient disableConnection];
            break;
        }
    }
}

- (void)writeOut:(NSString *)s {
    NSString * output = [NSString stringWithFormat:@"%@\r\n",s];
    uint8_t *buf = (uint8_t *)[output UTF8String];
    
    [outputStream write:buf maxLength:strlen((char *)buf)];
}

- (NSString *) readIn {
    
    int len;
    uint8_t buffer[1024];
    NSMutableString *total = [[NSMutableString alloc] init];
    while (![inputStream hasBytesAvailable]) {}
    
    while ([inputStream hasBytesAvailable]) {
        len = [inputStream read:buffer maxLength:sizeof(buffer)];
        if (len > 0) {
            [total appendString: [[NSString alloc] initWithBytes:buffer length:len encoding:NSASCIIStringEncoding]];
        }
    }
    
    [self close];
    
    return total;
}

@end