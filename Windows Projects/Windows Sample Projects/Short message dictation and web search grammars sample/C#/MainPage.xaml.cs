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
using System;
using System.Windows;
using Windows.Phone.Speech.Recognition;

namespace sdkSpeechPredefinedGrammarsWP8CS
{
    public partial class MainPage : PhoneApplicationPage
    {
        SpeechRecognizerUI speechRecognizer;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            this.speechRecognizer = new SpeechRecognizerUI();
        }

        private async void btnShortMessageDictation_Click(object sender, RoutedEventArgs e)
        {
            this.speechRecognizer.Recognizer.Grammars.Clear();

            // Use the short message dictation grammar with the speech recognizer.
            this.speechRecognizer.Recognizer.Grammars.AddGrammarFromPredefinedType("message", SpeechPredefinedGrammar.Dictation);

            await this.speechRecognizer.Recognizer.PreloadGrammarsAsync();

            try
            {
                // Use the built-in UI to prompt the user and get the result.
                SpeechRecognitionUIResult recognitionResult = await this.speechRecognizer.RecognizeWithUIAsync();

                if (recognitionResult.ResultStatus == SpeechRecognitionUIStatus.Succeeded)
                {
                    // Output the speech recognition result.
                    txtDictationResult.Text = "You said: " + recognitionResult.RecognitionResult.Text;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void btnWebSearch_Click(object sender, RoutedEventArgs e)
        {
            this.speechRecognizer.Recognizer.Grammars.Clear();

            this.speechRecognizer.Recognizer.Grammars.AddGrammarFromPredefinedType("search", SpeechPredefinedGrammar.WebSearch);

            await this.speechRecognizer.Recognizer.PreloadGrammarsAsync();

            try
            {
                // Use the built-in UI to prompt the user and get the result.
                SpeechRecognitionUIResult recognitionResult = await this.speechRecognizer.RecognizeWithUIAsync();

                if (recognitionResult.ResultStatus == SpeechRecognitionUIStatus.Succeeded)
                {
                    // Output the speech recognition result.
                    this.txtWebSearchResult.Text = "You said: " + recognitionResult.RecognitionResult.Text;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
