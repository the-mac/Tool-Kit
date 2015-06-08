//
//  Communicator.m
//  iOS Swift Client
//
//  Created by Christopher Miller on 6/7/15.
//  Copyright (c) 2015 The MAC. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "Communicator.h"
#import "iOS_Swift_Client-Swift.h"

CFReadStreamRef readStream;
CFWriteStreamRef writeStream;

NSInputStream *inputStream;
NSOutputStream *outputStream;
NetworkClient *networkClient;

@implementation Communicator


+ (NSString *)getLocalIpAddress {
    
    NSString *address = @"error";
    struct ifaddrs *interfaces = NULL;
    struct ifaddrs *temp_addr = NULL;
    int success = 0;
    // retrieve the current interfaces - returns 0 on success
    success = getifaddrs(&interfaces);
    if (success == 0) {
        // Loop through linked list of interfaces
        temp_addr = interfaces;
        while(temp_addr != NULL) {
            if(temp_addr->ifa_addr->sa_family == AF_INET) {
                // Check if interface is en0 which is the wifi connection on the iPhone
                if([[NSString stringWithUTF8String:temp_addr->ifa_name] isEqualToString:@"en0"]) {
                    // Get NSString from C String
                    address = [NSString stringWithUTF8String:inet_ntoa(((struct sockaddr_in *)temp_addr->ifa_addr)->sin_addr)];
                    
                }
                
            }
            
            temp_addr = temp_addr->ifa_next;
        }
    }
    // Free memory
    freeifaddrs(interfaces);
    return address;
    
}

- (void)setup :(NetworkClient*) client {
    self.connected = NO;
    networkClient = client;
    NSURL *url = [NSURL URLWithString:self.host];
    
    CFStreamCreatePairWithSocketToHost(kCFAllocatorDefault, (__bridge CFStringRef)[url host], self.port, &readStream, &writeStream);
    
    if(!CFWriteStreamOpen(writeStream)) {
        
        return;
    }
    
    if(!CFReadStreamOpen(readStream)) {
        
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
    self.connected = YES;
}

- (void)close {
    self.connected = NO;
    
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
            }
            
            break;
        }
        default: {
            NSLog(@"Stream is sending an Event: %i", event);
            if(event == NSStreamEventErrorOccurred) {
                self.connected = NO;
                [networkClient disableConnection];
            }
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
    [NSThread sleepForTimeInterval:0.1f];
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