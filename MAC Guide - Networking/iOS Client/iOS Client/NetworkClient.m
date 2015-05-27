//
//  ViewController.m
//  AClient
//
//  Created by Christopher Miller on 5/25/15.
//  Copyright (c) 2015 The MAC. All rights reserved.
//

#import "NetworkClient.h"

@interface NetworkClient ()
@end

@implementation NetworkClient
NSString* ipAddress;

- (void)viewDidAppear:(BOOL)animated {
    [super viewDidAppear:animated];
    [self setUpAllViews];
}

- (void)setUpAllViews {
    ipAddress = [self getLocalIpAddress];
    
    [self.connect setTitle:@"Connect" forState:UIControlStateNormal];
    [self.connect setTitle:@"Disconnect" forState:UIControlStateSelected];
    
    [self setText:ipBoxID with: ipAddress];
    [self setText:textID with: @"To start the client press the connect button"];
    
    [self setValues:ipBoxID with:true];
    [self setValues:msgBoxID with:false];
    [self setValues:sendID with:false];
    
}

/** References all views that have text changes (e.g, text.setText("some text")).
 * @param view the id for the view to be changed
 * @param content the data to be sent to the view
 **/
- (void) setText:(int) view with:(NSString*) content
{
    switch(view)
    {
        case ipBoxID: [self.ipBox setText:content]; break;
        case msgBoxID: [self.msgBox setText:content]; break;
        case textID: [self.text setText:[content stringByAppendingString:@"\n\n"]]; break;
        case appendTextID: [self.text setText:[NSString stringWithFormat:@"%@%@\n\n",self.text.text,content]]; break;
        case msgBoxHintID: [self.msgBox setPlaceholder:content]; break;
    }
}

/**
 * References all views that have boolean changes (e.g, ipBox.setEnabled(true)).
 * @param view the id for the view to be changed
 * @param value the boolean value to be sent to the view
 * */
- (void) setValues:(int) view with:(BOOL) value
{
    switch(view)
    {
        case ipBoxID: [self.ipBox setEnabled:value]; break;
        case msgBoxID: [self.msgBox setEnabled:value]; break;
        case sendID: [self.send setEnabled:value]; break;
        case connectID: self.connect.selected = value; break;
    }
}

- (NSString *)getLocalIpAddress {
    
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

- (void)setUpIOStreams {
    NSLog(@"setUpIOStreams");
    
    self.mCommunicator = [[Communicator alloc] init];
        
    self.mCommunicator->host = [NSString stringWithFormat:@"http://%@",self.ipBox.text];
    self.mCommunicator->port = 8888;
        
    [self.mCommunicator setup];
}

- (void)enableConnection {
    @try {
        if(self.mCommunicator == nil) [self setUpIOStreams];
        
        [self setText:textID with:[NSString stringWithFormat:@"%@%@",@"Device's IP Address: ",ipAddress]];
        [self setText:appendTextID with:[NSString stringWithFormat:@"%@%@",@"Server's IP Address: ",self.ipBox.text]];
        [self setText:msgBoxHintID with: @"Say something..."];
        [self setText:appendTextID with: @"Enter your message then press the send button"];
    
        [self setValues:sendID with:true];
        [self setValues:msgBoxID with:true];
        [self setValues:ipBoxID with:false];
    } @catch (NSException * e) {
        [self setValues:connectID with:false];
    }
}

- (void) disableConnection
{
    if(self.mCommunicator != nil)
    {
        @try {
            [self.mCommunicator close];
            self.mCommunicator = nil;
        } @catch (NSException * e) { }
        
        [self setText:textID with:@"Press the connect button to start the client"];
        [self setText:msgBoxID with: @""];
        [self setText:msgBoxHintID with: @""];
        
        [self setValues:ipBoxID with:true];
        [self setValues:msgBoxID with:false];
        [self setValues:sendID with:false];
    }
    else
    {
        [self setValues:connectID with:false];
    }
}

- (IBAction)sendDatOverConnection
{
    NSString * sentence = [self.msgBox.text stringByAppendingString:@"\r\n"];
    [self setText:msgBoxID with: @""];
    @try {
        
        if(self.mCommunicator == nil) [self setUpIOStreams];
        [self.mCommunicator writeOut:sentence];
        
        NSString *modifiedSentence = [self.mCommunicator readIn];
        self.mCommunicator = nil;
        
        sentence = [NSString stringWithFormat:@"%@%@%@%@",@"OUT TO SERVER: ",sentence, @"\nIN FROM SERVER: ", modifiedSentence];
        
    } @catch (NSException *e) {
        
        [self setValues:ipBoxID with:true];
        [self setValues:connectID with:false];
        [self setValues:sendID with:false];
        [self setValues:msgBoxID with:false];
    }
    
    [self setText:appendTextID with: sentence];
    [self.msgBox endEditing:YES];
}

- (IBAction)onClick:(id)sender {
    
    //  TOGGLE THE STATE/TEXT FOR BUTTON
    self.connect.selected = !self.connect.selected;
    NSLog(@"isConnected: %i", self.connect.selected);
    
    //  TOGGLE THE STATE FOR CONNECTION
    if(self.connect.selected)
        [self enableConnection];
    else
        [self disableConnection];
}
@end
