/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
using AudioComponent_Phone;
using SdkHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace sdkWindowsRuntimeComponentWP8CS
{
    public class AudioViewModel 
    {
        AudioComponent _audio;

        public AudioViewModel()
        {
            _audio = new AudioComponent();
        }

        public void SuspendAudio()
        {
            _audio.Suspend();
        }

        public void ResumeAudio()
        {
            _audio.Resume();
        }

        bool _playingSound;
        private bool PlayingSound
        {
            get
            {
                return _playingSound;
            }

            set
            {
                if (value != _playingSound)
                {
                    _playingSound = value;
                    PlayCommand.RaiseCanExecuteChanged();
                    StopCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private void Play()
        {
            this.PlayingSound = true;
            _audio.PlaySound();
        }

        private void Stop()
        {
            _audio.StopSound();
            this.PlayingSound = false;
        }

        #region Commands
        RelayCommand _playCommand;

        public RelayCommand PlayCommand
        {
            get
            {
                if (_playCommand == null)
                {
                    _playCommand = new RelayCommand(
                        param => Play(),
                        param => !this.PlayingSound);
                }
                return _playCommand;
            }
        }

        RelayCommand _stopCommand;
        public RelayCommand StopCommand
        {
            get
            {
                if (_stopCommand == null)
                {
                    _stopCommand = new RelayCommand(
                      param => this.Stop(),
                        param => this.PlayingSound);
                }
                return _stopCommand;
            }
        }
        #endregion

    }
}
