//
//  ViewController.swift
//  iOSPHPClient
//
//  Created by Christopher Miller on 6/15/15.
//  Copyright (c) 2015 The MAC. All rights reserved.
//

import UIKit

class NetworkClient: UIViewController {
    
    let serverURL = "MY_PHP_SCRIPT"
    
    @IBOutlet weak var textField: UITextField!
    @IBOutlet weak var textView: UITextView!
    
    @IBAction func sendJSON() {
        
        if "MY_PHP_SCRIPT" == serverURL  {
            println("Alert: You need to set up your own PHP Script. See MAC Guide (https://github.com/the-mac/Tool-Kit/wiki/PHP-JSON-API-(Eclipse)-HowTo) on what to replace this value with.")
            exit(0)
        }
        
        if let text = textField.text {
            
            // SET UP THE CONSTANTS/VARIABLES
            var response: NSURLResponse? = nil
            var error: NSError? = nil
            let url = NSURL(string:serverURL)!
            let cachePolicy = NSURLRequestCachePolicy.ReloadIgnoringLocalCacheData
            
            // SET UP THE REQUEST
            var request = NSMutableURLRequest(URL: url, cachePolicy: cachePolicy, timeoutInterval: 2.0)
            request.HTTPMethod = "POST"
            request.HTTPBody = ("{\"message\":\"\(textField.text)\"}" as NSString).dataUsingEncoding(NSUTF8StringEncoding)!
            
            // SET UP THE RESPONSE
            NSURLProtocol.setProperty("application/json;", forKey: "Content-Type", inRequest: request)
            NSURLProtocol.setProperty(request.HTTPBody!.length, forKey: "Content-Length", inRequest: request)
            let responseString = NSURLConnection.sendSynchronousRequest(request, returningResponse:&response, error:&error)!
            
            textView.text = NSString(data:responseString, encoding:NSUTF8StringEncoding)! as String
        }
    }
}

