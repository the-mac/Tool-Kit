//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DirectXSample.h"
#include "Audio.h"
#include "Profiler.h"

#include <mfapi.h>

using namespace Microsoft::WRL;
using namespace Windows::ApplicationModel;

class MediaEngineNotify :
        public RuntimeClass<RuntimeClassFlags<ClassicCom>, IMFMediaEngineNotify>
{
public:
    STDMETHODIMP EventNotify(DWORD meEvent, DWORD_PTR param1, DWORD param2)
    {
        return S_OK;
    }
};

void AudioEngineCallbacks::Initialize(Audio* audio)
{
    m_audio = audio;
}

// Called when a critical system error causes XAudio2
// to be closed and restarted. The error code is given in Error.
void  _stdcall AudioEngineCallbacks::OnCriticalError(HRESULT Error)
{
    m_audio->SetEngineExperiencedCriticalError();
}

Audio::Audio() : 
    m_initialized(false)
{
    DX::ThrowIfFailed(
        MFStartup(MF_VERSION)
        );
}

Audio::~Audio()
{
    SuspendAudio();
    MFShutdown();
}

void Audio::Initialize()
{
    if (m_initialized)
    {
        ReleaseResources();
    }

    m_isAudioStarted = false;
    m_soundEffectEngine = nullptr;
    m_soundEffectMasteringVoice = nullptr;

    m_reverbParametersSmall.ReflectionsDelay = XAUDIO2FX_REVERB_DEFAULT_REFLECTIONS_DELAY;
    m_reverbParametersSmall.ReverbDelay = XAUDIO2FX_REVERB_DEFAULT_REVERB_DELAY;
    m_reverbParametersSmall.RearDelay = XAUDIO2FX_REVERB_DEFAULT_REAR_DELAY;
    m_reverbParametersSmall.PositionLeft = XAUDIO2FX_REVERB_DEFAULT_POSITION;
    m_reverbParametersSmall.PositionRight = XAUDIO2FX_REVERB_DEFAULT_POSITION;
    m_reverbParametersSmall.PositionMatrixLeft = XAUDIO2FX_REVERB_DEFAULT_POSITION_MATRIX;
    m_reverbParametersSmall.PositionMatrixRight = XAUDIO2FX_REVERB_DEFAULT_POSITION_MATRIX;
    m_reverbParametersSmall.EarlyDiffusion = 4;
    m_reverbParametersSmall.LateDiffusion = 15;
    m_reverbParametersSmall.LowEQGain = XAUDIO2FX_REVERB_DEFAULT_LOW_EQ_GAIN;
    m_reverbParametersSmall.LowEQCutoff = XAUDIO2FX_REVERB_DEFAULT_LOW_EQ_CUTOFF;
    m_reverbParametersSmall.HighEQGain = XAUDIO2FX_REVERB_DEFAULT_HIGH_EQ_GAIN;
    m_reverbParametersSmall.HighEQCutoff = XAUDIO2FX_REVERB_DEFAULT_HIGH_EQ_CUTOFF;
    m_reverbParametersSmall.RoomFilterFreq = XAUDIO2FX_REVERB_DEFAULT_ROOM_FILTER_FREQ;
    m_reverbParametersSmall.RoomFilterMain = XAUDIO2FX_REVERB_DEFAULT_ROOM_FILTER_MAIN;
    m_reverbParametersSmall.RoomFilterHF = XAUDIO2FX_REVERB_DEFAULT_ROOM_FILTER_HF;
    m_reverbParametersSmall.ReflectionsGain = XAUDIO2FX_REVERB_DEFAULT_REFLECTIONS_GAIN;
    m_reverbParametersSmall.ReverbGain = XAUDIO2FX_REVERB_DEFAULT_REVERB_GAIN;
    m_reverbParametersSmall.DecayTime = XAUDIO2FX_REVERB_DEFAULT_DECAY_TIME;
    m_reverbParametersSmall.Density = XAUDIO2FX_REVERB_DEFAULT_DENSITY;
    m_reverbParametersSmall.RoomSize = XAUDIO2FX_REVERB_DEFAULT_ROOM_SIZE;
    m_reverbParametersSmall.WetDryMix = XAUDIO2FX_REVERB_DEFAULT_WET_DRY_MIX;
    m_reverbParametersSmall.DisableLateField = TRUE;

    m_reverbParametersLarge.ReflectionsDelay = XAUDIO2FX_REVERB_DEFAULT_REFLECTIONS_DELAY;
    m_reverbParametersLarge.ReverbDelay = XAUDIO2FX_REVERB_DEFAULT_REVERB_DELAY;
    m_reverbParametersLarge.RearDelay = XAUDIO2FX_REVERB_DEFAULT_REAR_DELAY;
    m_reverbParametersLarge.PositionLeft = XAUDIO2FX_REVERB_DEFAULT_POSITION;
    m_reverbParametersLarge.PositionRight = XAUDIO2FX_REVERB_DEFAULT_POSITION;
    m_reverbParametersLarge.PositionMatrixLeft = XAUDIO2FX_REVERB_DEFAULT_POSITION_MATRIX;
    m_reverbParametersLarge.PositionMatrixRight = XAUDIO2FX_REVERB_DEFAULT_POSITION_MATRIX;
    m_reverbParametersLarge.EarlyDiffusion = 4;
    m_reverbParametersLarge.LateDiffusion = 4;
    m_reverbParametersLarge.LowEQGain = XAUDIO2FX_REVERB_DEFAULT_LOW_EQ_GAIN;
    m_reverbParametersLarge.LowEQCutoff = XAUDIO2FX_REVERB_DEFAULT_LOW_EQ_CUTOFF;
    m_reverbParametersLarge.HighEQGain = XAUDIO2FX_REVERB_DEFAULT_HIGH_EQ_GAIN;
    m_reverbParametersLarge.HighEQCutoff = XAUDIO2FX_REVERB_DEFAULT_HIGH_EQ_CUTOFF;
    m_reverbParametersLarge.RoomFilterFreq = XAUDIO2FX_REVERB_DEFAULT_ROOM_FILTER_FREQ;
    m_reverbParametersLarge.RoomFilterMain = XAUDIO2FX_REVERB_DEFAULT_ROOM_FILTER_MAIN;
    m_reverbParametersLarge.RoomFilterHF = XAUDIO2FX_REVERB_DEFAULT_ROOM_FILTER_HF;
    m_reverbParametersLarge.ReflectionsGain = XAUDIO2FX_REVERB_DEFAULT_REFLECTIONS_GAIN;
    m_reverbParametersLarge.ReverbGain = XAUDIO2FX_REVERB_DEFAULT_REVERB_GAIN;
    m_reverbParametersLarge.DecayTime = 1.0f;
    m_reverbParametersLarge.Density = 10;
    m_reverbParametersLarge.RoomSize = 100;
    m_reverbParametersLarge.WetDryMix = XAUDIO2FX_REVERB_DEFAULT_WET_DRY_MIX;
    m_reverbParametersLarge.DisableLateField = FALSE;

    for (int i = 0; i < ARRAYSIZE(m_soundEffects); i++)
    {
        m_soundEffects[i].m_soundEffectBufferData = nullptr;
        m_soundEffects[i].m_soundEffectSourceVoice = nullptr;
        m_soundEffects[i].m_soundEffectStarted = false;
        ZeroMemory(&m_soundEffects[i].m_audioBuffer, sizeof(m_soundEffects[i].m_audioBuffer));
    }

    m_initialized = true;
}

void Audio::SetRoomSize(float roomSize, float* wallDistances)
{
    // Maximum "outdoors" is a room size of 1000.0f
    float normalizedRoomSize = roomSize  / 1000.0f;

    float outputMatrix[4] = {0, 0, 0, 0};
    float leftRatio = wallDistances[2] / (wallDistances[2] + wallDistances[3]);
    if ((wallDistances[2] + wallDistances[3]) == 0)
    {
        leftRatio = 0.5f;
    }
    float width = abs((wallDistances[3] + wallDistances[5] + wallDistances[7]) / 3.0f - (wallDistances[2] + wallDistances[4] + wallDistances[6]) / 3.0f);
    float height = abs(wallDistances[1] - wallDistances[0]);
    float widthNormalized = width / max(width, height);
    if (width == 0)
    {
        widthNormalized = 1.0f;
    }

    // As widthNormalized approaches 0, the room is taller than wide.
    // This means that the late-field reverb should be non-localized across both speakers.
    leftRatio -= .5f;
    leftRatio *= widthNormalized;
    leftRatio += .5f;

    // Near-field reverb with the speaker closest to the wall getting more reverb
    outputMatrix[0] = (1.0f - leftRatio)* normalizedRoomSize;
    outputMatrix[1] = 0;
    outputMatrix[2] = 0;
    outputMatrix[3] = leftRatio * normalizedRoomSize;
    DX::ThrowIfFailed(
        m_soundEffectReverbVoiceSmallRoom->SetOutputMatrix(m_soundEffectMasteringVoice, 2, 2, outputMatrix)
        );

    // Late-field reverb: The wall furthest away gets more of the late-field reverb
    outputMatrix[0] = leftRatio * (1.0f - normalizedRoomSize);
    outputMatrix[1] = 0;
    outputMatrix[2] = 0;
    outputMatrix[3]= (1.0f - leftRatio) * (1.0f - normalizedRoomSize);
    DX::ThrowIfFailed(
        m_soundEffectReverbVoiceLargeRoom->SetOutputMatrix(m_soundEffectMasteringVoice, 2, 2, outputMatrix)
        );
}

void Audio::CreateResources()
{
    try
    {
        // Create a separate engine and mastering voice for sound effects in the sample
        // Games will use many voices in a complex graph for audio, mixing all effects down to a
        // single mastering voice.
        // We are creating an entirely new engine instance and mastering voice in order to tag
        // our sound effects with the audio category AudioCategory_GameEffects.
        DX::ThrowIfFailed(
            XAudio2Create(&m_soundEffectEngine)
            );

        m_soundEffectEngineCallback.Initialize(this);
        m_soundEffectEngine->RegisterForCallbacks(&m_soundEffectEngineCallback);

        // We default the mastering voice to 2 channels to simplify the reverb logic.
        DX::ThrowIfFailed(
            m_soundEffectEngine->CreateMasteringVoice(&m_soundEffectMasteringVoice, 2, 48000, 0, nullptr, nullptr, AudioCategory_GameEffects)
            );

        CreateReverb(m_soundEffectEngine.Get(), m_soundEffectMasteringVoice, &m_reverbParametersSmall, &m_soundEffectReverbVoiceSmallRoom, true);
        CreateReverb(m_soundEffectEngine.Get(), m_soundEffectMasteringVoice, &m_reverbParametersLarge, &m_soundEffectReverbVoiceLargeRoom, true);

        CreateSourceVoice(RollingEvent);
        CreateSourceVoice(FallingEvent);
        CreateSourceVoice(CollisionEvent);
        CreateSourceVoice(MenuChangeEvent);
        CreateSourceVoice(MenuSelectedEvent);
        CreateSourceVoice(CheckpointEvent);
    }
    catch (...)
    {
        m_engineExperiencedCriticalError = true;
    }

    try
    {
        m_mediaEngine = nullptr;

        ComPtr<IMFMediaEngineClassFactory> mediaEngineFactory;
        ComPtr<IMFAttributes> mediaEngineAttributes;

        // Create the class factory for the Media Engine.
        DX::ThrowIfFailed(
            CoCreateInstance(CLSID_MFMediaEngineClassFactory, nullptr, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&mediaEngineFactory))
            );

        // Define configuration attributes.
        DX::ThrowIfFailed(
            MFCreateAttributes(&mediaEngineAttributes, 1)
            );

        ComPtr<MediaEngineNotify> notify = Make<MediaEngineNotify>();
        ComPtr<IUnknown> unknownNotify;
        DX::ThrowIfFailed(
            notify.As(&unknownNotify)
            );

        DX::ThrowIfFailed(
            mediaEngineAttributes->SetUnknown(MF_MEDIA_ENGINE_CALLBACK, unknownNotify.Get())
            );

        // Create the Media Engine.
        DX::ThrowIfFailed(
            mediaEngineFactory->CreateInstance(0, mediaEngineAttributes.Get(), &m_mediaEngine)
            );

        Platform::String^ musicPath = Platform::String::Concat(Package::Current->InstalledLocation->Path, L"\\Media\\Audio\\background.wma");

        // Set the music source.
        DX::ThrowIfFailed(
            m_mediaEngine->SetSource(const_cast<wchar_t *>(musicPath->Data()))
            );

        DX::ThrowIfFailed(
            m_mediaEngine->SetLoop(TRUE)
            );
    }
    catch (...)
    {
        m_mediaEngine = nullptr;
    }
}

void Audio::CreateReverb(IXAudio2* engine, IXAudio2MasteringVoice* masteringVoice, XAUDIO2FX_REVERB_PARAMETERS* parameters, IXAudio2SubmixVoice** newSubmix, bool enableEffect)
{
    XAUDIO2_EFFECT_DESCRIPTOR soundEffectdescriptor;
    XAUDIO2_EFFECT_CHAIN soundEffectChain;
    Microsoft::WRL::ComPtr<IUnknown> soundEffectXAPO;

    DX::ThrowIfFailed(
        XAudio2CreateReverb(&soundEffectXAPO)
        );

    soundEffectdescriptor.InitialState = false;
    soundEffectdescriptor.OutputChannels = 2;
    soundEffectdescriptor.pEffect = soundEffectXAPO.Get();
    soundEffectChain.EffectCount = 1;
    soundEffectChain.pEffectDescriptors = &soundEffectdescriptor;

    DX::ThrowIfFailed(
        engine->CreateSubmixVoice(newSubmix, 2, 48000, 0, 0, nullptr, &soundEffectChain)
        );

    DX::ThrowIfFailed(
        (*newSubmix)->SetEffectParameters(0, parameters, sizeof(m_reverbParametersSmall))
        );

    if (enableEffect)
    {
        DX::ThrowIfFailed(
            (*newSubmix)->EnableEffect(0)
            );
    }

    DX::ThrowIfFailed(
        (*newSubmix)->SetVolume (1.0f)
        );

    float outputMatrix[4] = {0, 0, 0, 0};
    DX::ThrowIfFailed(
        (*newSubmix)->SetOutputMatrix(masteringVoice, 2, 2, outputMatrix)
        );
}

void Audio::CreateSourceVoice(SoundEvent sound)
{
    // Load all data for each sound effect into a single in-memory buffer
    MediaStreamer soundEffectStream;
    switch (sound)
    {
        case RollingEvent: soundEffectStream.Initialize(L"Media\\Audio\\MarbleRoll.wav"); break;
        case FallingEvent: soundEffectStream.Initialize(L"Media\\Audio\\MarbleFall.wav"); break;
        case CollisionEvent: soundEffectStream.Initialize(L"Media\\Audio\\MarbleHit.wav"); break;
        case MenuChangeEvent: soundEffectStream.Initialize(L"Media\\Audio\\MenuChange.wav"); break;
        case MenuSelectedEvent: soundEffectStream.Initialize(L"Media\\Audio\\MenuSelect.wav"); break;
        case CheckpointEvent: soundEffectStream.Initialize(L"Media\\Audio\\Checkpoint.wav"); break;
    }
    m_soundEffects[sound].m_soundEventType = sound;

    uint32 bufferLength = soundEffectStream.GetMaxStreamLengthInBytes();
    m_soundEffects[sound].m_soundEffectBufferData = new byte[bufferLength];
    soundEffectStream.ReadAll(m_soundEffects[sound].m_soundEffectBufferData, bufferLength, &m_soundEffects[sound].m_soundEffectBufferLength);

    XAUDIO2_SEND_DESCRIPTOR descriptors[3];
    descriptors[0].pOutputVoice = m_soundEffectMasteringVoice;
    descriptors[0].Flags = 0;
    descriptors[1].pOutputVoice = m_soundEffectReverbVoiceSmallRoom;
    descriptors[1].Flags = 0;
    descriptors[2].pOutputVoice = m_soundEffectReverbVoiceLargeRoom;
    descriptors[2].Flags = 0;
    XAUDIO2_VOICE_SENDS sends = {0};
    sends.SendCount = 3;
    sends.pSends = descriptors;

    // The rolling sound can have pitch shifting and a low-pass filter.
    if (sound == RollingEvent)
    {
        DX::ThrowIfFailed(
            m_soundEffectEngine->CreateSourceVoice(
                &m_soundEffects[sound].m_soundEffectSourceVoice,
                &(soundEffectStream.GetOutputWaveFormatEx()),
                XAUDIO2_VOICE_USEFILTER,
                2.0f,
                nullptr,
                &sends)
            );
    }
    else
    {
        DX::ThrowIfFailed(
            m_soundEffectEngine->CreateSourceVoice(
                &m_soundEffects[sound].m_soundEffectSourceVoice,
                &(soundEffectStream.GetOutputWaveFormatEx()),
                0,
                1.0f,
                nullptr,
                &sends)
            );
    }
    m_soundEffects[sound].m_soundEffectSampleRate = soundEffectStream.GetOutputWaveFormatEx().nSamplesPerSec;

    // Queue in-memory buffer for playback
    ZeroMemory(&m_soundEffects[sound].m_audioBuffer, sizeof(m_soundEffects[sound].m_audioBuffer));

    m_soundEffects[sound].m_audioBuffer.AudioBytes = m_soundEffects[sound].m_soundEffectBufferLength;
    m_soundEffects[sound].m_audioBuffer.pAudioData = m_soundEffects[sound].m_soundEffectBufferData;
    m_soundEffects[sound].m_audioBuffer.pContext = &m_soundEffects[sound];
    m_soundEffects[sound].m_audioBuffer.Flags = XAUDIO2_END_OF_STREAM;
    if (sound == RollingEvent)
    {
        m_soundEffects[sound].m_audioBuffer.LoopCount = XAUDIO2_LOOP_INFINITE;
    }
    else
    {
        m_soundEffects[sound].m_audioBuffer.LoopCount = 0;
    }

    if (sound == FallingEvent || sound == CheckpointEvent)
    {
        m_soundEffects[sound].m_soundEffectSourceVoice->SetVolume(0.4f);
    }

    DX::ThrowIfFailed(
        m_soundEffects[sound].m_soundEffectSourceVoice->SubmitSourceBuffer(&m_soundEffects[sound].m_audioBuffer)
        );
}

void Audio::ReleaseResources()
{
    if (m_soundEffectReverbVoiceSmallRoom != nullptr)
    {
        m_soundEffectReverbVoiceSmallRoom->DestroyVoice();
    }
    if (m_soundEffectReverbVoiceLargeRoom != nullptr)
    {
        m_soundEffectReverbVoiceLargeRoom->DestroyVoice();
    }
    for (int i = 0; i < ARRAYSIZE(m_soundEffects); i++)
    {
        if (m_soundEffects[i].m_soundEffectSourceVoice != nullptr)
        {
            delete[] m_soundEffects[i].m_soundEffectBufferData;
            m_soundEffects[i].m_soundEffectSourceVoice->DestroyVoice();
        }
        m_soundEffects[i].m_soundEffectSourceVoice = nullptr;
    }
    if (m_soundEffectMasteringVoice != nullptr)
    {
        m_soundEffectMasteringVoice->DestroyVoice();
    }

    m_soundEffectMasteringVoice = nullptr;
    m_soundEffectReverbVoiceSmallRoom = nullptr;
    m_soundEffectReverbVoiceLargeRoom = nullptr;
    m_soundEffectEngine = nullptr;
    m_mediaEngine = nullptr;
}

void Audio::Start()
{
    if (m_engineExperiencedCriticalError)
    {
        return;
    }

    if (m_mediaEngine)
    {
        m_mediaEngine->Play(); // Ignore return result, the emulator may error out.
    }
    
    m_isAudioStarted = true;
}

// This sample processes audio buffers during the render cycle of the application.
// As long as the sample maintains a high-enough frame rate, this approach should
// not glitch audio. In game code, it is best for audio buffers to be processed
// on a separate thread that is not synced to the main render loop of the game.
void Audio::Render()
{
    ProfileScope profiler(L"Audio", DirectX::Colors::Cyan);

    if (m_engineExperiencedCriticalError && m_isAudioStarted)
    {
        m_engineExperiencedCriticalError = false;
        ReleaseResources();
        Initialize();
        CreateResources();
        Start();
    }
}

void Audio::PlaySoundEffect(SoundEvent sound)
{
    XAUDIO2_BUFFER buf = {0};
    XAUDIO2_VOICE_STATE state = {0};

    if (m_engineExperiencedCriticalError) {
        // If there's an error, then we'll recreate the engine on the next
        // render pass.
        return;
    }

    SoundEffectData* soundEffect = &m_soundEffects[sound];
    HRESULT hr = soundEffect->m_soundEffectSourceVoice->Start();
    if FAILED(hr)
    {
        m_engineExperiencedCriticalError = true;
        return;
    }

    // For one-off voices, submit a new buffer if there's none queued up,
    // and allow up to two collisions to be queued up.
    if (sound != RollingEvent)
    {
        XAUDIO2_VOICE_STATE state = {0};
        soundEffect->m_soundEffectSourceVoice->GetState(&state, XAUDIO2_VOICE_NOSAMPLESPLAYED);
        if (state.BuffersQueued == 0)
        {
            soundEffect->m_soundEffectSourceVoice->SubmitSourceBuffer(&soundEffect->m_audioBuffer);
        }
        else if (state.BuffersQueued < 2 && sound == CollisionEvent)
        {
            soundEffect->m_soundEffectSourceVoice->SubmitSourceBuffer(&soundEffect->m_audioBuffer);
        }

        // For the menu clicks, we want to stop the voice and replay the click right away.
        // Note that stopping and then flushing could cause a glitch due to the
        // waveform not being at a zero-crossing, but due to the nature of the sound
        // (fast and 'clicky'), we don't mind.
        if (state.BuffersQueued > 0 && sound == MenuChangeEvent)
        {
            soundEffect->m_soundEffectSourceVoice->Stop();
            soundEffect->m_soundEffectSourceVoice->FlushSourceBuffers();
            soundEffect->m_soundEffectSourceVoice->SubmitSourceBuffer(&soundEffect->m_audioBuffer);
            soundEffect->m_soundEffectSourceVoice->Start();
        }
    }

    m_soundEffects[sound].m_soundEffectStarted = true;
}

void Audio::StopSoundEffect(SoundEvent sound)
{
    if (m_engineExperiencedCriticalError) {
        return;
    }

    HRESULT hr = m_soundEffects[sound].m_soundEffectSourceVoice->Stop();
    if FAILED(hr)
    {
        // If there's an error, then we'll recreate the engine on the next render pass
        m_engineExperiencedCriticalError = true;
        return;
    }

    m_soundEffects[sound].m_soundEffectStarted = false;
}

bool Audio::IsSoundEffectStarted(SoundEvent sound)
{
    return m_soundEffects[sound].m_soundEffectStarted;
}

void Audio::SetSoundEffectVolume(SoundEvent sound, float volume)
{
    if (m_soundEffects[sound].m_soundEffectSourceVoice != nullptr) {
        m_soundEffects[sound].m_soundEffectSourceVoice->SetVolume(volume);
    }
}

void Audio::SetSoundEffectPitch(SoundEvent sound, float pitch)
{
    if (m_soundEffects[sound].m_soundEffectSourceVoice != nullptr) {
        m_soundEffects[sound].m_soundEffectSourceVoice->SetFrequencyRatio(pitch);
    }
}

void Audio::SetSoundEffectFilter(SoundEvent sound, float frequency, float oneOverQ)
{
    if (m_soundEffects[sound].m_soundEffectSourceVoice != nullptr) {
        if (oneOverQ <= 0.1f)
        {
            oneOverQ = 0.1f;
        }
        if (oneOverQ > XAUDIO2_MAX_FILTER_ONEOVERQ)
        {
            oneOverQ = XAUDIO2_MAX_FILTER_ONEOVERQ;
        }
        XAUDIO2_FILTER_PARAMETERS params = {LowPassFilter, XAudio2CutoffFrequencyToRadians(frequency, m_soundEffects[sound].m_soundEffectSampleRate), oneOverQ};

        m_soundEffects[sound].m_soundEffectSourceVoice->SetFilterParameters(&params);
    }
}

// Uses the IXAudio2::StopEngine method to stop all audio immediately.
// It leaves the audio graph untouched, which preserves all effect parameters
// and effect histories (like reverb effects) voice states, pending buffers,
// cursor positions and so on.
// When the engines are restarted, the resulting audio will sound as if it had
// never been stopped except for the period of silence.
void Audio::SuspendAudio()
{
    if (m_engineExperiencedCriticalError)
    {
        return;
    }

    if (m_isAudioStarted)
    {
        if (m_mediaEngine)
        {
            m_mediaEngine->Pause();
        }

        m_soundEffectEngine->StopEngine();
    }
    m_isAudioStarted = false;
}

// Restarts the audio streams. A call to this method must match a previous call
// to SuspendAudio. This method causes audio to continue where it left off.
// If there is a problem with the restart, the m_engineExperiencedCriticalError
// flag is set. The next call to Render will recreate all the resources and
// reset the audio pipeline.
void Audio::ResumeAudio()
{
    if (m_engineExperiencedCriticalError)
    {
        return;
    }

    HRESULT hr = m_soundEffectEngine->StartEngine();

    if (FAILED(hr))
    {
        m_engineExperiencedCriticalError = true;
    }
}