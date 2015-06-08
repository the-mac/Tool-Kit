//
//  NetworkClient.swift
//  iOS Swift Client
//
//  Created by Christopher Miller on 6/7/15.
//  Copyright (c) 2015 The MAC. All rights reserved.
//

import UIKit
import Foundation

class NetworkClient: UIViewController {
    var ipAddress: String?
    var mCommunicator: Communicator?
    
    let connectID = 1232
    let sendID = 1233
    let ipBoxID = 1234
    let ipHintID = 1134
    let msgBoxID = 1235
    let textID = 1236
    let appendTextID = 1237
    let msgBoxHintID = 1238
    
    @IBOutlet weak var ipBox: UITextField!
    @IBOutlet weak var msgBox: UITextField!
    @IBOutlet weak var connect: UIButton!
    @IBOutlet weak var text: UITextView!
    @IBOutlet weak var send: UIButton!
    
    override func viewDidAppear(animated: Bool) {
        super.viewDidAppear(animated)
        setUpAllViews()
    }
    
    func setUpAllViews() {
        ipAddress = Communicator.getLocalIpAddress()
        
        connect.setTitle("Connect", forState: UIControlState.Normal)
        connect.setTitle("Disconnect", forState: UIControlState.Selected)
        
        if(ipAddress == "error") {
            setText(ipHintID,content:"Check Wifi")
            setValues(ipBoxID,value:false)
        }
        else {
             setText(ipBoxID,content:ipAddress)
             setValues(ipBoxID,value:true)
        }
        
         setText(textID,content:"To start the client press the connect button")
         setValues(msgBoxID,value:false)
         setValues(sendID,value:false)
    }
    
    /** References all views that have text changes (e.g, text.text = "some text").
    * @param view the id for the view to be changed
    * @param content the data to be sent to the view
    **/
    func setText(view: Int , content: String?)
    {
        let uwContent = content!
	    switch(view) {
	    case ipBoxID: ipBox.text = uwContent
	    case ipHintID: ipBox.placeholder = uwContent
	    case msgBoxID: msgBox.text = uwContent
	    case textID: text.text = "\(uwContent)\n\n"
	    case appendTextID: text.text = "\(text.text)\(uwContent)\n\n"
	    default: msgBox.placeholder = uwContent
	    }
    }
    
    /**
    * References all views that have boolean changes (e.g, ipBox.enabled = true).
    * @param view the id for the view to be changed
    * @param value the boolean value to be sent to the view
    * */
    func setValues(view: Int, value: Bool) {
	    switch(view) {
	    case ipBoxID: ipBox.enabled = value
	    case msgBoxID: msgBox.enabled = value
	    case sendID: send.enabled = value
	    default: connect.selected = value
	    }
    }
    
    func setUpIOStreams() {
	    
	    mCommunicator = Communicator();
        
        let text = ipBox.text!
	    mCommunicator!.host = "http://\(text)"
	    mCommunicator!.port = 8888;
	    
	    mCommunicator!.setup(self)
    }
    
    func enableConnection() {
        let text = ipBox.text!
        let address = ipAddress!
        setText(textID,content:"Device's IP Address: \(address)")
        setText(appendTextID,content:"Server's IP Address: \(text)")
        setText(msgBoxHintID, content:"Say something...")
        setText(appendTextID, content:"Enter your message then press the send button")
        
        setValues(sendID,value:true)
        setValues(msgBoxID,value:true)
        setValues(ipBoxID,value:false)
        if(mCommunicator == nil) { setUpIOStreams() }
       
    }
    
    func disableConnection() {
        
        setText(textID, content:"Press the connect button to start the client")
        setText(msgBoxID, content: "")
        setText(msgBoxHintID, content: "")
        
        setValues(ipBoxID,value:true)
        setValues(msgBoxID, value:false)
        setValues(sendID, value:false)
        setValues(connectID, value:false)
        
        if(mCommunicator != nil)
        {
            mCommunicator!.close()
            mCommunicator = nil;
        }
    }
    
    @IBAction func sendDatOverConnection(button: UIButton) {
        
        let text = msgBox.text!
        var sentence = "\(text)\r\n";
        setText(msgBoxID, content: "");
        
        if mCommunicator == nil { setUpIOStreams(); }
        mCommunicator!.writeOut(sentence);
        
        if mCommunicator!.connected {
            
            var modifiedSentence = mCommunicator!.readIn();
            self.mCommunicator = nil;
            
            sentence = "OUT TO SERVER: \(sentence)\nIN FROM SERVER: \(modifiedSentence)"
            
        } else {
            
            setValues(ipBoxID, value:true)
            setValues(connectID, value:false)
            setValues(sendID, value:false)
            setValues(msgBoxID, value:false)
        }
        
        setText(appendTextID, content: sentence);
        msgBox.endEditing(true);
    }
    
    @IBAction func onClick(button: UIButton) {
        
        //  TOGGLE THE STATE/TEXT FOR BUTTON
        connect.selected = !connect.selected;
        let selected = connect.selected
        println("isConnected: \(selected)");
        
        //  TOGGLE THE STATE FOR CONNECTION
        if selected { enableConnection() }
        else { disableConnection() }
    }
}