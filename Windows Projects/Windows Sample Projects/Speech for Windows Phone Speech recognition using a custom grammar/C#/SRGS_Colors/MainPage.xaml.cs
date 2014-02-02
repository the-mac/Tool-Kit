/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Windows.Phone.Speech.Recognition;

// The following capabilities required by this sample have been enabled in the WMAppManifest.xml file of this project:
// ID_CAP_SPEECH_RECOGNITION
// ID_CAP_MICROPHONE

namespace sdkSpeechRecoGrammarFromURIWP8CS
{
  public partial class MainPage : PhoneApplicationPage
  {
    SpeechRecognizer _myRecognizer;
    Dictionary<string, Color> _colorLookup = new Dictionary<string, Color> 
            { 
                {"red", Colors.Red }, {"blue", Colors.Blue }, {"black",Colors.Black}, 
                {"brown",Colors.Brown}, {"purple",Colors.Purple}, {"green",Colors.Green}, 
                {"yellow",Colors.Yellow}, {"cyan",Colors.Cyan}, {"magenta",Colors.Magenta}, 
                {"orange",Colors.Orange}, {"gray",Colors.Gray}, {"white",Colors.White}
            };

    // Constructor
    public MainPage()
    {
        InitializeComponent();
        InitializeRecognizer();
    }

    // Creates a SpeechRecognizer instance and initializes the grammar
    private void InitializeRecognizer()
    {
        // Initialize a URI with the path to the SRGS-compliant XML file.
        // For more information about grammars for Windows Phone 8 and how to 
        // define and use SRGS-compliant grammars in your app, see 
        // http://msdn.microsoft.com/en-us/library/windowsphone/develop/jj206991(v=vs.105).aspx

        Uri colorList = new Uri("ms-appx:///Colors.grxml", UriKind.Absolute);

        // Initialize the SpeechRecognizer and add the grammar.
        _myRecognizer = new SpeechRecognizer();
        _myRecognizer.Grammars.AddGrammarFromUri("srgsColors", colorList);

        // Set EndSilenceTimeout to give users more time to complete speaking a phrase.
        _myRecognizer.Settings.EndSilenceTimeout = TimeSpan.FromSeconds(1.2);
    }

    // Create colors from recognized text.
    private Color getColor(string colorString)
    {
        Color newColor = Colors.Transparent;

        if (_colorLookup.ContainsKey(colorString.ToLower()))
        {
            newColor = _colorLookup[colorString];
        }

        return newColor;
    }

    // Handle the click event for the Mic button.
    private async void MicButton_Click(object sender, EventArgs e)
    {
        try
        {
            // Reset the text to prompt the user.
            if (ColorTextBox.Text != "Press the mic button and speak to choose colors for the background, border, and text.")
            {
                ColorTextBox.Text = "Speak to choose colors for the background, border, and text.";
            }

            // Start speech recognition.
            var recoResult = await _myRecognizer.RecognizeAsync();

            // Check the confidence level of the recognition result.
            if (recoResult.TextConfidence == SpeechRecognitionConfidence.High ||
                recoResult.TextConfidence == SpeechRecognitionConfidence.Medium)
            {

                // Declare a string that will contain messages when the color rule matches GARBAGE.
                string garbagePrompt = "";

                // BACKGROUND: Check to see if the recognition result contains the semantic key for the background color, 
                // and not a match for the GARBAGE rule, and change the color.
                if (recoResult.Semantics.ContainsKey("background") && recoResult.Semantics["background"].Value.ToString() != "...")
                {
                    string backgroundColor = recoResult.Semantics["background"].Value.ToString();
                    this.ColorTextBox.Background = new SolidColorBrush(getColor(backgroundColor.ToLower()));
                }

                // If "background" was matched, but the color rule matched GARBAGE, prompt the user.
                else if (recoResult.Semantics.ContainsKey("background") && recoResult.Semantics["background"].Value.ToString() == "...")
                {
                    garbagePrompt += "Didn't get the background color \n\nTry saying blue background\n";
                    ColorTextBox.Text = garbagePrompt;
                }

                // BORDER: Check to see if the recognition result contains the semantic key for the border color,
                // and not a match for the GARBAGE rule, and change the color.
                if (recoResult.Semantics.ContainsKey("border") && recoResult.Semantics["border"].Value.ToString() != "...")
                {
                    string borderColor = recoResult.Semantics["border"].Value.ToString();
                    this.ColorTextBox.BorderBrush = new SolidColorBrush(getColor(borderColor.ToLower()));
                }

                // If "border" was matched, but the color rule matched GARBAGE, prompt the user.
                else if (recoResult.Semantics.ContainsKey("border") && recoResult.Semantics["border"].Value.ToString() == "...")
                {
                    garbagePrompt += "Didn't get the border color\n\n Try saying red border\n";
                    ColorTextBox.Text = garbagePrompt;
                }

                // TEXT: Check to see if the recognition result contains the semantic key for the text color,
                // and not a match for the GARBAGE rule, and change the color.
                if (recoResult.Semantics.ContainsKey("text") && recoResult.Semantics["text"].Value.ToString() != "...")
                {
                    string textColor = recoResult.Semantics["text"].Value.ToString();
                    this.ColorTextBox.Foreground = new SolidColorBrush(getColor(textColor.ToLower()));
                }

                // If "text" was matched, but the color rule matched GARBAGE, prompt the user.
                else if (recoResult.Semantics.ContainsKey("text") && recoResult.Semantics["text"].Value.ToString() == "...")
                {
                    garbagePrompt += "Didn't get the text color\n\n Try saying white text\n";
                    ColorTextBox.Text = garbagePrompt;
                }

                // Initialize a string that will describe the user's color choices.
                string textBoxColors = "You selected -> \n\n";

                // Write the color choices contained in the semantics of the recognition result to the text box.
                foreach (KeyValuePair<String, SemanticProperty> child in recoResult.Semantics)
                {

                    // Check to see if any of the semantic values in recognition result contains a match for the GARBAGE rule.
                    if (!child.Value.Value.Equals("..."))
                    {

                        // Cycle through the semantic keys and values and write them to the text box.
                        textBoxColors += (string.Format(" {0} {1}\n",
                            child.Value.Value, child.Key ?? "null"));

                        ColorTextBox.Text = textBoxColors;
                    }

                    // If there was no match to the colors rule or if it matched GARBAGE, prompt the user.
                    else
                    {
                        ColorTextBox.Text = garbagePrompt;
                    }
                }
            }

          // Prompt the user if recognition failed or recognition confidence is low.
            else if (recoResult.TextConfidence == SpeechRecognitionConfidence.Rejected ||
              recoResult.TextConfidence == SpeechRecognitionConfidence.Low)
            {
                ColorTextBox.Text = "Sorry, didn't get that \n\nTry saying ->\nblue background\nred border\nwhite text";
            }
        }
        catch (System.Threading.Tasks.TaskCanceledException)
        {
            // Ignore the cancellation exception of the recoOperation.
        }
        catch (Exception err)
        {
            // Handle the speech privacy policy error.
            const int privacyPolicyHResult = unchecked((int)0x80045509);

            if (err.HResult == privacyPolicyHResult)
            {
                MessageBox.Show("To run this sample, you must first accept the speech privacy policy. To do so, navigate to Settings -> speech on your phone and check 'Enable Speech Recognition Service' ");
            }
            else
            {
                MessageBox.Show(String.Format("An error occurred: {0}", err.Message));
            }
        }
    }
  }
}
