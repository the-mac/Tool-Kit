//
//  ViewController.swift
//  iOS Interfaces
//
//  Created by Christopher Miller on 6/10/15.
//  Copyright (c) 2015 The MAC. All rights reserved.
//

import UIKit

class ViewController: UIViewController {

    @IBOutlet weak var droppedText: UITextField!
    @IBOutlet weak var dropZone: UILabel!
    
    @IBAction func dropText() {
        if let text = droppedText.text {
            dropZone.text = text
        }
    }
    
    override func viewDidLoad() {
        super.viewDidLoad()
        // Do any additional setup after loading the view, typically from a nib.
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }


}

