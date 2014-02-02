/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, http://code.msdn.microsoft.com/wpapps
  
*/
#include "pch.h"
#include "audio.h"

using namespace AudioComponent_Phone;
using namespace Platform;

AudioComponent::AudioComponent()
{
	initialized = false;
}

void AudioComponent::Initialize()
{
	if (!initialized)
	{
		// Create an IXAudio2 object
		ThrowIfFailed(XAudio2Create(&pXAudio2));

		// Create a mastering voice
		ThrowIfFailed(pXAudio2->CreateMasteringVoice(&pMasteringVoice));

		// Create a source voice 
		WAVEFORMATEX waveformat;
		waveformat.wFormatTag = WAVE_FORMAT_PCM;
		waveformat.nChannels = 1;
		waveformat.nSamplesPerSec = SAMPLE_RATE;
		waveformat.nAvgBytesPerSec = SAMPLE_RATE * 2;
		waveformat.nBlockAlign = 2;
		waveformat.wBitsPerSample = 16;
		waveformat.cbSize = 0;

		ThrowIfFailed(pXAudio2->CreateSourceVoice(&pVoice, &waveformat, 0, XAUDIO2_MAX_FREQ_RATIO));

		// Create the sound wave
		for (int sample = 0; sample < BUFFER_LENGTH; sample++)
		{
			short value = (short)(65535 * sample / BUFFER_LENGTH - 32768);
			soundData[sample] = value/20;
			soundData[sample++] = value/20;
		}


		// Submit the array
		buffer.AudioBytes = 2 * BUFFER_LENGTH;
		buffer.pAudioData = (byte *)soundData;
		buffer.Flags = XAUDIO2_END_OF_STREAM;
		buffer.PlayBegin = 0;
		buffer.PlayLength = BUFFER_LENGTH;
		buffer.LoopBegin = 0;
		buffer.LoopLength = BUFFER_LENGTH;
		buffer.LoopCount = XAUDIO2_LOOP_INFINITE;

		pVoice->SetFrequencyRatio(BASE_FREQ);
		pVoice->SetVolume(0);
		ThrowIfFailed(pVoice->SubmitSourceBuffer(&buffer));

		// The sound will play in a continuous loop, but the volume is currently set at 0. 
		// Playing the sound in this samples refers to increasing the volume so that the sound is audible.
		ThrowIfFailed(pVoice->Start());

		initialized = true;
	}
}


void AudioComponent::Suspend()
{
	if (!initialized)
		return;

	// Prevent battery drain by stopping the audio engine
	pXAudio2->StopEngine();
}


void AudioComponent::Resume()
{
	if (!initialized)
		return;

	ThrowIfFailed(
		pXAudio2->StartEngine()
		);
}


void AudioComponent::PlaySound()
{
	Initialize();
	ThrowIfFailed(pVoice->SetVolume(1));
}

void AudioComponent::StopSound()
{
	if (!initialized)
		return;

	ThrowIfFailed(pVoice->SetVolume(0));
}

inline void AudioComponent::ThrowIfFailed(HRESULT hr)
{
	if (FAILED(hr))
	{
		// Set a breakpoint on this line to catch DX API errors.
		throw Platform::Exception::CreateException(hr); 
	}
}
