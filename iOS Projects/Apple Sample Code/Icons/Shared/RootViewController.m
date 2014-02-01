/*
     File: RootViewController.m
 Abstract: The view controller displays what each icon does on iOS.
  Version: 1.1
 
 Disclaimer: IMPORTANT:  This Apple software is supplied to you by Apple
 Inc. ("Apple") in consideration of your agreement to the following
 terms, and your use, installation, modification or redistribution of
 this Apple software constitutes acceptance of these terms.  If you do
 not agree with these terms, please do not use, install, modify or
 redistribute this Apple software.
 
 In consideration of your agreement to abide by the following terms, and
 subject to these terms, Apple grants you a personal, non-exclusive
 license, under Apple's copyrights in this original Apple software (the
 "Apple Software"), to use, reproduce, modify and redistribute the Apple
 Software, with or without modifications, in source and/or binary forms;
 provided that if you redistribute the Apple Software in its entirety and
 without modifications, you must retain this notice and the following
 text and disclaimers in all such redistributions of the Apple Software.
 Neither the name, trademarks, service marks or logos of Apple Inc. may
 be used to endorse or promote products derived from the Apple Software
 without specific prior written permission from Apple.  Except as
 expressly stated in this notice, no other rights or licenses, express or
 implied, are granted by Apple herein, including but not limited to any
 patent rights that may be infringed by your derivative works or by other
 works in which the Apple Software may be incorporated.
 
 The Apple Software is provided by Apple on an "AS IS" basis.  APPLE
 MAKES NO WARRANTIES, EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION
 THE IMPLIED WARRANTIES OF NON-INFRINGEMENT, MERCHANTABILITY AND FITNESS
 FOR A PARTICULAR PURPOSE, REGARDING THE APPLE SOFTWARE OR ITS USE AND
 OPERATION ALONE OR IN COMBINATION WITH YOUR PRODUCTS.
 
 IN NO EVENT SHALL APPLE BE LIABLE FOR ANY SPECIAL, INDIRECT, INCIDENTAL
 OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 INTERRUPTION) ARISING IN ANY WAY OUT OF THE USE, REPRODUCTION,
 MODIFICATION AND/OR DISTRIBUTION OF THE APPLE SOFTWARE, HOWEVER CAUSED
 AND WHETHER UNDER THEORY OF CONTRACT, TORT (INCLUDING NEGLIGENCE),
 STRICT LIABILITY OR OTHERWISE, EVEN IF APPLE HAS BEEN ADVISED OF THE
 POSSIBILITY OF SUCH DAMAGE.
 
 Copyright (C) 2013 Apple Inc. All Rights Reserved.
 
 */

#import "RootViewController.h"

#define kTopBottomMargins 20

@implementation RootViewController

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
	return 7;
}

- (CGFloat)tableView:(UITableView *)tableView heightForRowAtIndexPath:(NSIndexPath *)indexPath
{
    CGFloat rowHeight = 0;
    
    switch (indexPath.row)
    {
        case 0:
        {
            rowHeight = 57.0;
            break;
        }
        case 1:
        {
            rowHeight = 72.0;
            break;
        }
        case 2:
        {
            rowHeight = 114.0;
            break;
        }
        case 3:
        {
            rowHeight = 65.0;
            break;
        }
        case 4:
        {
            rowHeight = 50.0;
            break;
        }
        case 5:
        {
            rowHeight = 58.0;
            break;
        }
        case 6:
        {
            rowHeight = 114.0;
            break;
        }
    }
    
    return rowHeight + kTopBottomMargins;
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
	UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:@"cellID" forIndexPath:indexPath];
	
    NSString *titleStr;
    NSString *detailStr;
    UIImage *image;
    switch (indexPath.row)
    {
        case 0:
        {
            titleStr = @"Icon.png";
            detailStr = @"Homescreen icon on iPhone/iPod touch";
            cell.detailTextLabel.numberOfLines = 2;
            image = [UIImage imageNamed:@"Icon"];
            break;
        }
        case 1:
        {
            titleStr = @"Icon-72.png";
            detailStr = @"Homescreen icon on iPad";
            image = [UIImage imageNamed:@"Icon-72"];
            break;
        }
        case 2:
        {
            titleStr = @"Icon@2x.png";
            detailStr = @"Homescreen icon on iPhone Retina";
            cell.detailTextLabel.numberOfLines = 2;
            image = [UIImage imageNamed:@"Icon@2x"];
            break;
        }
        case 3:
        {
            titleStr = @"Icon-Small.png";
            detailStr = @"Icon in Spotlight and Settings app on iPhone/iPod touch and icon in Settings app on iPad";
            cell.detailTextLabel.numberOfLines = 3;
            image = [UIImage imageNamed:@"Icon-Small"];
            break;
        }
        case 4:
        {
            titleStr = @"Icon-Small-50.png";
            detailStr = @"Icon in Spotlight on iPad";
            image = [UIImage imageNamed:@"Icon-Small-50"];
            break;
        }
        case 5:
        {
            titleStr = @"Icon-Small@2x.png";
            detailStr = @"Icon in Spotlight and Settings app on iPhone Retina";
            cell.detailTextLabel.numberOfLines = 2;
            image = [UIImage imageNamed:@"Icon-Small@2x"];
            break;
        }
        case 6:
        {
            titleStr = @"iTunesArtwork";
            detailStr = @"Icon in iTunes for Ad Hoc distribution builds";
            cell.detailTextLabel.numberOfLines = 3;
            image = [UIImage imageNamed:@"iTunesArtwork"];
            break;
        }
    }
    
    cell.imageView.image = image;
    cell.textLabel.text = titleStr;
	cell.detailTextLabel.text = detailStr;
    
	return cell;
}

@end
