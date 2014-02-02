/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using ShakeGestures;
using System.IO;

namespace ShakeGestureLibrarySample
{
    public partial class MainPage : PhoneApplicationPage
    {

        // Constructor
        public MainPage()
        {
            ShakeGesturesHelper.Instance.ShakeGesture +=
                new EventHandler<ShakeGestureEventArgs>(Instance_ShakeGesture);
            ShakeGesturesHelper.Instance.MinimumRequiredMovesForShake = 4;
            ShakeGesturesHelper.Instance.Active = true;

            InitializeComponent();
        }

        // Set the data context of the TextBlock to the answer.
        void Instance_ShakeGesture(object sender, ShakeGestureEventArgs e)
        {
            // Use BeginInvoke to write to the UI thread.
            textBlock1.Dispatcher.BeginInvoke(() =>
            {
                textBlock1.DataContext = GetAnswer();

            });
        }

        // Generate a random number and retrieve this item from the anser
        // list.
        private string GetAnswer()
        {
            Random random = new Random();
            int randomNumber = random.Next(Answers.Count);
            return Answers[randomNumber];
        }

        // List of answers.
        private List<string> answersValue;
        public List<string> Answers
        {
            get
            {
                if (answersValue == null)
                    LoadAnswers();

                return answersValue;
            }
        }

        // Load the answers from the text file.
        private void LoadAnswers()
        {
            answersValue = new List<string>();

            using (StreamReader reader =
                new StreamReader(Application.GetResourceStream(new Uri(
                    "answers.txt", UriKind.Relative)).Stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                    answersValue.Add(line);
            }
        }
    }
}
