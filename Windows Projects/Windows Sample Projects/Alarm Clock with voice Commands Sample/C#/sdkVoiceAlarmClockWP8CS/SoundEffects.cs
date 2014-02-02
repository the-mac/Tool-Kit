/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Windows.Media;
using System.Windows.Resources;
using Microsoft.Xna.Framework.Audio; 

namespace AlarmClockWithVoice
{
    public static class SoundEffects
    {
        private static bool initialized = false;
        private static SoundEffect alarm;

        // Must be called before using static methods.
        public static void Initialize()
        {
            if (SoundEffects.initialized)
                return;

            StreamResourceInfo info = App.GetResourceStream(new Uri("Audio/alarm.wav", UriKind.Relative));
            alarm = SoundEffect.FromStream(info.Stream);

            // Adds an Update delegate, to simulate the XNA update method.
            CompositionTarget.Rendering += delegate(object sender, EventArgs e)
            {
                Microsoft.Xna.Framework.FrameworkDispatcher.Update();
            };

            Microsoft.Xna.Framework.FrameworkDispatcher.Update();
            initialized = true;
        }

        public static SoundEffect Alarm
        {
            get
            {
                // If not initialized, returns null.
                if (!SoundEffects.initialized)
                    return null;

                return alarm;
            }
        }
    }
}
