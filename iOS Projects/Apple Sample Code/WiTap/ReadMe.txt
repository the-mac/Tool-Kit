WiTap

===========================================================================
DESCRIPTION:

The WiTap sample application demonstrates how to achieve network communication between applications. Using Bonjour, the application both advertises itself on the local network and displays a list of other instances of this application on the network.

Simply build the sample using Xcode and run it in the simulator or on the device. Wait for another player to connect or select a game to connect to. Once connected, tap one or more colored pads on a device to see them highlighted simultaneously on the remote device.

The version of WiTap no longer works over Bluetooth; see QA1753 "Bonjour over Bluetooth on iOS 5.0" for a discussion of why that's the case and what you can do about it.

<https://developer.apple.com/library/ios/#qa/qa1753/_index.html>

===========================================================================
BUILD REQUIREMENTS:

Mac OS X 10.8.2, Xcode 4.6.1, iOS 6.1 SDK

===========================================================================
RUNTIME REQUIREMENTS:

iOS 6.0 or later

===========================================================================
PACKING LIST:

ReadMe.txt
This document.

WiTap.xcodeproj
An Xcode project for the sample.

Info.plist
Icons
Default Images
Various boilerplate resources.

AppController.h
AppController.m
UIApplication's delegate class, the central controller of the application.

TapViewController.h
TapViewController.m
The main view controller for the app.

TapView.h
TapView.m
UIView subclass that can highlight itself when locally or remotely tapped.

PickerViewController.h
PickerViewController.m
A view controller that displays both the currently advertised game name and a list of other games available on the local network.

PickerExtras.xib
PickerHeaderBackground.png
Some user interface bits and bobs for the PickerViewController.

Networking/QServer.m
Networking/QServer.m
A general-purpose TCP server class.

main.m
The main file for the WiTap application.

===========================================================================
CHANGES FROM PREVIOUS VERSIONS:

Version 2.0
- A major rewrite.
- Updated to the latest coding techniques and standards.
- Adopted QServer to replace the ancient TCPServer.
- Changed the on-the-wire protocol to allow easy testing via telnet.
- Implement an 'end game' button to allow for easier testing.
- Support rotation in all view controllers.
- Fully adopted view controllers (the previous code came from a time when dinosaurs ruled the Earth and view controllers weren't de rigueur, and thus parts of the app used view controllers and parts didn't).
- Basic multitasking support.
- Keep the TCP server running to avoid port creep.
- Fixed down numerous sharp edge cases.

Version 1.8
- Upgraded project to build with the iOS 4.1 SDK
- Implemented an autorelease to the NSNetServiceBrowser object to release the browser once the connection is established. An active browser can cause a delay in sending packets - <rdar://problem/7000938>
- Upgraded for IPv6 as default, creates IPv4 socket is unable to create IPv6 socket.
- fixed logic bug in handleEvent to check for read method failure.

Version 1.7
- Fixed table selection and cell update bugs, and added support for handling stream errors.

Version 1.6
- Upgraded for 3.0 SDK due to deprecated APIs; in "cellForRowAtIndexPath" it now uses UITableViewCell's initWithStyle.

Version 1.5
- Updated for and tested with iPhone OS 2.0. First public release.

Version 1.4
- Updated for Beta 7.
- Code clean up.
- Improved Bonjour support.

Version 1.3
- Updated for Beta 4. 
- Added code signing.

Version 1.2
- Added icon.

Copyright (C) 2008-2013 Apple Inc. All rights reserved.