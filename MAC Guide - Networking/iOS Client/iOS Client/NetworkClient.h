//
//  ViewController.h
//  AClient
//
//  Created by Christopher Miller on 5/25/15.
//  Copyright (c) 2015 The MAC. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#include <ifaddrs.h>
#include <arpa/inet.h>
#import "Communicator.h"

#define connectID 1232
#define sendID 1233
#define ipBoxID 1234
#define msgBoxID 1235
#define textID 1236
#define appendTextID 1237
#define msgBoxHintID 1238

@interface NetworkClient : UIViewController<NSStreamDelegate>

@property (weak, nonatomic) IBOutlet UITextField *ipBox;
@property (weak, nonatomic) IBOutlet UITextField *msgBox;
@property (weak, nonatomic) IBOutlet UITextView *text;
@property (weak, nonatomic) IBOutlet UIButton *connect;
@property (weak, nonatomic) IBOutlet UIButton *send;
@property (strong, retain) Communicator *mCommunicator;

- (IBAction)onClick:(id)sender;

- (IBAction)sendDatOverConnection;

@end

