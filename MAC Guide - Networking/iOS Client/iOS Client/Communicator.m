//
//  Communicator.m
//  AClient
//
//  Created by Christopher Miller on 5/26/15.
//  Copyright (c) 2015 The MAC. All rights reserved.
//  Source: https://gist.github.com/rjungemann/446256

#import <Foundation/Foundation.h>
#import "Communicator.h"

CFReadStreamRef readStream;
CFWriteStreamRef writeStream;

NSInputStream *inputStream;
NSOutputStream *outputStream;

@implementation Communicator

- (void)setup {
    NSURL *url = [NSURL URLWithString:host];
    
    NSLog(@"Setting up connection to %@ : %i", [url absoluteString], port);
    
    CFStreamCreatePairWithSocketToHost(kCFAllocatorDefault, (__bridge CFStringRef)[url host], port, &readStream, &writeStream);
    
    if(!CFWriteStreamOpen(writeStream)) {
        NSLog(@"Error, writeStream not open");
        
        return;
    }
    [self open];
    
    NSLog(@"Status of outputStream: %lu", [outputStream streamStatus]);
    
    return;
}

- (void)open {
    NSLog(@"Opening streams.");
    
    inputStream = (__bridge NSInputStream *)readStream;
    outputStream = (__bridge NSOutputStream *)writeStream;
    
    [outputStream setDelegate:self];
    
    [outputStream scheduleInRunLoop:[NSRunLoop currentRunLoop] forMode:NSDefaultRunLoopMode];
    
    [inputStream open];
    [outputStream open];
}

- (void)close {
    NSLog(@"Closing streams.");
    
    [inputStream close];
    [outputStream close];
    
    [outputStream removeFromRunLoop:[NSRunLoop currentRunLoop] forMode:NSDefaultRunLoopMode];
    
    [inputStream setDelegate:nil];
    [outputStream setDelegate:nil];
    
    inputStream = nil;
    outputStream = nil;
}

- (void)stream:(NSStream *)stream handleEvent:(NSStreamEvent)event {
    NSLog(@"Stream triggered.");
    
    switch(event) {
        case NSStreamEventHasSpaceAvailable: {
            if(stream == outputStream) {
                NSLog(@"outputStream is ready.");
            }
            break;
        }
        default: {
            NSLog(@"Stream is sending an Event: %i", event);
            
            break;
        }
    }
}

- (void)readIn:(NSString *)s {
    NSLog(@"Reading in the following:");
    NSLog(@"%@", s);
}

- (void)writeOut:(NSString *)s {
    NSString * output = [NSString stringWithFormat:@"%@\r\n",s];
    uint8_t *buf = (uint8_t *)[output UTF8String];
    
    [outputStream write:buf maxLength:strlen((char *)buf)];
    
    NSLog(@"Writing out the following:");
    NSLog(@"%@", s);
    
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
    
    NSLog(@"Reading in the following:");
    NSLog(@"%@", total);
    [self close];
    
    return total;
}

@end