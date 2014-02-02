/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System.Collections.Generic;
using System.Linq;
using Windows.Phone.Speech.Recognition; 
using Windows.Phone.Speech.Synthesis; 


namespace AlarmClockWithVoice
{
    public static class Speech
    {
        public static SpeechRecognizer recognizer;
        public static SpeechSynthesizer synthesizer;
        public static SpeechRecognizerUI recognizerUI;

        private static bool initialized = false;

        // Must be called before using static methods.
        public static void Initialize()
        {
            if (Speech.initialized)
                return;

            Speech.recognizer = new SpeechRecognizer();
            Speech.synthesizer = new SpeechSynthesizer();
            Speech.recognizerUI = new SpeechRecognizerUI();

            // Sets the en-US male voice.
            IEnumerable<VoiceInformation> enUSMaleVoices = from voice in InstalledVoices.All
                                    where voice.Gender == VoiceGender.Male
                                    && voice.Language == "en-US"
                                    select voice;

            Speech.synthesizer.SetVoice(enUSMaleVoices.ElementAt(0));

            Speech.initialized = true;
        }
    }
}
