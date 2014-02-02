//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "MediaStreamer.h"
#include "BasicMath.h"
#include <mfmediaengine.h>

enum SoundEvent
{
    RollingEvent        = 0,
    FallingEvent        = 1,
    CollisionEvent      = 2,
    CheckpointEvent     = 3,
    MenuChangeEvent     = 4,
    MenuSelectedEvent   = 5,
    LastSoundEvent,
};

// Make sure this matches the number of entries in the SoundEvent enum above
static const int SOUND_EVENTS = LastSoundEvent;

struct SoundEffectData
{
    SoundEvent                  m_soundEventType;
    IXAudio2SourceVoice*        m_soundEffectSourceVoice;
    XAUDIO2_BUFFER              m_audioBuffer;
    byte*                       m_soundEffectBufferData;
    uint32                      m_soundEffectBufferLength;
    uint32                      m_soundEffectSampleRate;
    bool                        m_soundEffectStarted;
};

class Audio;
class AudioEngineCallbacks: public IXAudio2EngineCallback
{
private:
    Audio* m_audio;

public :
    AudioEngineCallbacks(){};
    void Initialize(Audio* audio);

    // Called by XAudio2 just before an audio processing pass begins.
    void _stdcall OnProcessingPassStart(){};

    // Called just after an audio processing pass ends.
    void  _stdcall OnProcessingPassEnd(){};

    // Called when a critical system error causes XAudio2
    // to be closed and restarted. The error code is given in Error.
    void  _stdcall OnCriticalError(HRESULT Error);
};

class Audio
{
private:
    Microsoft::WRL::ComPtr<IXAudio2> m_soundEffectEngine;
    IXAudio2MasteringVoice*     m_soundEffectMasteringVoice;
    SoundEffectData             m_soundEffects[SOUND_EVENTS];
    XAUDIO2FX_REVERB_PARAMETERS m_reverbParametersLarge;
    XAUDIO2FX_REVERB_PARAMETERS m_reverbParametersSmall;
    IXAudio2SubmixVoice*        m_soundEffectReverbVoiceSmallRoom;
    IXAudio2SubmixVoice*        m_soundEffectReverbVoiceLargeRoom;
    bool                        m_engineExperiencedCriticalError;
    AudioEngineCallbacks        m_soundEffectEngineCallback;
    bool                        m_initialized;
    Microsoft::WRL::ComPtr<IMFMediaEngine> m_mediaEngine;

    void CreateSourceVoice(SoundEvent);
    void CreateReverb(
        IXAudio2* engine,
        IXAudio2MasteringVoice* masteringVoice,
        XAUDIO2FX_REVERB_PARAMETERS* parameters,
        IXAudio2SubmixVoice** newSubmix,
        bool enableEffect
        );

public:
    bool m_isAudioStarted;

    Audio();
    ~Audio();
    void Initialize();
    void CreateResources();
    void ReleaseResources();
    void Start();
    void Render();
    void SuspendAudio();
    void ResumeAudio();

    // This flag can be used to tell when the audio system
    // is experiencing critial errors.
    // XAudio2 gives a critical error when the user unplugs
    // the headphones and a new speaker configuration is generated.
    void SetEngineExperiencedCriticalError()
    {
        m_engineExperiencedCriticalError = true;
    }

    bool HasEngineExperiencedCriticalError()
    {
        return m_engineExperiencedCriticalError;
    }

    void PlaySoundEffect(SoundEvent sound);
    bool IsSoundEffectStarted(SoundEvent sound);
    void StopSoundEffect(SoundEvent sound);
    void SetSoundEffectVolume(SoundEvent sound, float volume);
    void SetSoundEffectPitch(SoundEvent sound, float pitch);
    void SetSoundEffectFilter(SoundEvent sound, float frequency, float oneOverQ);
    void SetRoomSize(float roomSize, float* wallDistances);
};
