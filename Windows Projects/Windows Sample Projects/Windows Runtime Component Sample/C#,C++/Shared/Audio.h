/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
#pragma once

namespace AudioComponent_Phone
{
	#define SAMPLE_RATE (44100)

    public ref class AudioComponent sealed
    {
	private:
		static const int BASE_FREQ = 20;
		static const int BUFFER_LENGTH = SAMPLE_RATE  / BASE_FREQ;
		interface IXAudio2*  pXAudio2;
		IXAudio2MasteringVoice * pMasteringVoice;
		IXAudio2SourceVoice * pVoice;
		short soundData[BUFFER_LENGTH];
		XAUDIO2_BUFFER buffer;
		bool initialized;

		void Initialize();
		void ThrowIfFailed(HRESULT);
    public:
        AudioComponent();

		
		void Suspend();
		void Resume();

		void PlaySound();
		void StopSound();
    };
}
