/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using Microsoft.Phone.Controls;
using sdkSpeechColorChangeWP8CS.Resources;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Windows.Foundation;
using Windows.Phone.Speech.Recognition;
using Windows.Phone.Speech.Synthesis;


// Before trying your app in the emulator, verify that your microphone and audio are working properly. 
// You can verify they are working with speech by trying a long press of the Start button, 
// accepting the Speech Privacy Policy, and then testing a system Voice Command (e.g., "Start Internet Explorer").

// The following capabilities are required for this app to run, and have been set in the WMAppManifest.xml
// ID_CAP_SPEECH_RECOGNITION
// ID_CAP_MICROPHONE
// ID_CAP_NETWORKING

namespace sdkSpeechColorChangeWP8CS
{
    public partial class MainPage : PhoneApplicationPage
    {
        SpeechSynthesizer _synthesizer;                             // The speech synthesizer (text-to-speech, TTS) object
        SpeechRecognizer _recognizer;                               // The speech recognition object
        IAsyncOperation<SpeechRecognitionResult> _recoOperation;    // Used to canel the current asynchronous speech recognition operation

        bool _recoEnabled = false;                                  // When this is true, we will continue to recognize 
        Dictionary<string, SolidColorBrush> _colorBrushes;          // Dictionary of all colors we will recognize and their equivalent color brushes

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        // User has navigated to this page.  Initialize if necessary.
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {

            //Set up the dictionary of SolidColorBrush objects. 
            if (_colorBrushes == null)
            {
                _colorBrushes = new Dictionary<string, SolidColorBrush>();
                _colorBrushes.Add("red", new SolidColorBrush(Colors.Red));
                _colorBrushes.Add("cyan", new SolidColorBrush(Colors.Cyan));
                _colorBrushes.Add("blue", new SolidColorBrush(Colors.Blue));
                _colorBrushes.Add("yellow", new SolidColorBrush(Colors.Yellow));
                _colorBrushes.Add("orange", new SolidColorBrush(Colors.Orange));
                _colorBrushes.Add("fire color", new SolidColorBrush(Colors.Magenta));
                _colorBrushes.Add("purple", new SolidColorBrush(Colors.Purple));
                _colorBrushes.Add("black", new SolidColorBrush(Colors.Black));
                _colorBrushes.Add("jet black", new SolidColorBrush(Colors.Black));
                _colorBrushes.Add("green", new SolidColorBrush(Colors.Green));
                _colorBrushes.Add("white", new SolidColorBrush(Colors.White));
                _colorBrushes.Add("dark gray", new SolidColorBrush(Colors.DarkGray));
                _colorBrushes.Add("brown", new SolidColorBrush(Colors.Brown));
                _colorBrushes.Add("magenta", new SolidColorBrush(Colors.Magenta));
                _colorBrushes.Add("gray", new SolidColorBrush(Colors.Gray));
            }

            try
            {
                // Create the speech recognizer and speech synthesizer objects. 
                if (_synthesizer == null)
                {
                    _synthesizer = new SpeechSynthesizer();
                }
                if (_recognizer == null)
                {
                    _recognizer = new SpeechRecognizer();

                    // Set up a list of colors to recognize.
                    _recognizer.Grammars.AddGrammarFromList("Colors", _colorBrushes.Keys);
                }
                
            }
            catch (Exception err)
            {
                txtResult.Text = err.ToString();
            }

            base.OnNavigatedTo(e);
        }

        // The btnContinuousRecognition acts like a toggle. The intial state is to start continuous speech recognition.
        // The next time the button is pressed, it should stop speech recognition.
        private async void btnContinuousRecognition_Click(object sender, RoutedEventArgs e)
        {
            // Change the button text. 
            if (this._recoEnabled)
            {
                // Update the UI to the initial state
                _recoEnabled = false;
                btnContinuousRecognition.Content = "Start speech recognition";
                txtResult.Text = String.Empty;
                txtInstructions.Visibility = System.Windows.Visibility.Collapsed;
                
                // Cancel the outstanding recognition operation, if one exists
                if (_recoOperation != null && _recoOperation.Status == AsyncStatus.Started)
                {
                    _recoOperation.Cancel();
                }
                return;
            }
            else
            {
                // Set the flag to say that we are in recognition mode
                _recoEnabled = true;

                // Update the UI
                btnContinuousRecognition.Content = "Listening... tap to cancel";
                txtInstructions.Visibility = System.Windows.Visibility.Visible;
            }

            // Continuously recognize speech until the user has canceled 
            while (this._recoEnabled)
            {
                try
                {
                    // Perform speech recognition.  
                    _recoOperation = _recognizer.RecognizeAsync();
                    var recoResult = await this._recoOperation;

                    // Check the confidence level of the speech recognition attempt.
                    if (recoResult.TextConfidence < SpeechRecognitionConfidence.Medium)
                    {
                        // If the confidence level of the speech recognition attempt is low, 
                        // ask the user to try again.
                        txtResult.Text = "Not sure what you said, please try again.";
                        await _synthesizer.SpeakTextAsync("Not sure what you said, please try again");
                    }
                    else
                    {
                        // Output that the color of the rectangle is changing by updating
                        // the TextBox control and by using text-to-speech (TTS). 
                        txtResult.Text = "Changing color to: " + recoResult.Text;
                        await _synthesizer.SpeakTextAsync("Changing color to " + recoResult.Text);

                        // Set the fill color of the rectangle to the recognized color. 
                        rectangleResult.Background = TryGetBrush(recoResult.Text.ToLower());
                        
                    }
                }
                catch (System.Threading.Tasks.TaskCanceledException)
                { 
                    // Ignore the cancellation exception of the recoOperation.
                    // When recoOperation.Cancel() is called to cancel the asynchronous speech recognition operation
                    // initiated by RecognizeAsync(),  a TaskCanceledException is thrown to signify early exit.
                }
                catch (Exception err)
                {
                    // Handle the speech privacy policy error.
                    const int privacyPolicyHResult = unchecked((int)0x80045509);

                    if (err.HResult == privacyPolicyHResult)
                    {
                        MessageBox.Show("To run this sample, you must first accept the speech privacy policy. To do so, navigate to Settings -> speech on your phone and check 'Enable Speech Recognition Service' ");
                        _recoEnabled = false;
                        btnContinuousRecognition.Content = "Start speech recognition";
                    }
                    else
                    {
                        txtResult.Text = "Error: " + err.Message;
                    }
                }
            }

        }

        /// <summary>
        /// Returns a SolidColorBrush that matches the recognized color string.
        /// </summary>
        /// <param name="reco">The recognized color string.</param>
        /// <returns>The matching colored SolidColorBrush if the color was matched; Otherwise, white.</returns>
        private SolidColorBrush TryGetBrush(string recognizedColor)
        {
            if (_colorBrushes.ContainsKey(recognizedColor))
                return _colorBrushes[recognizedColor];

            return _colorBrushes["white"];
        }
    }
}
