//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "BasicReaderWriter.h"
#include <vector>

class MediaStreamer
{
private:
    WAVEFORMATEX      m_waveFormat;
    uint32            m_maxStreamLengthInBytes;
    std::vector<byte> m_data;
    UINT32            m_offset;

public:
    BasicReaderWriter^ m_reader;

public:
    MediaStreamer();
    ~MediaStreamer();

    WAVEFORMATEX& GetOutputWaveFormatEx()
    {
        return m_waveFormat;
    }

    UINT32 GetMaxStreamLengthInBytes()
    {
        return m_data.size();
    }

    void Initialize(_In_ const WCHAR* url);
    void ReadAll(uint8* buffer, uint32 maxBufferSize, uint32* bufferLength);
    void Restart();
};