//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DirectXSample.h"
#include "MediaStreamer.h"

#ifndef MAKEFOURCC
    #define MAKEFOURCC(ch0, ch1, ch2, ch3)                              \
                ((uint32)(byte)(ch0) | ((uint32)(byte)(ch1) << 8) |       \
                ((uint32)(byte)(ch2) << 16) | ((uint32)(byte)(ch3) << 24 ))
#endif /* defined(MAKEFOURCC) */

MediaStreamer::MediaStreamer() :
    m_offset(0)
{
    ZeroMemory(&m_waveFormat, sizeof(m_waveFormat));
}

MediaStreamer::~MediaStreamer()
{
}

void MediaStreamer::Initialize(_In_ const WCHAR* url)
{
    BasicReaderWriter^ reader = ref new BasicReaderWriter();
    Platform::Array<byte>^ data = reader->ReadData(ref new Platform::String(url));
    UINT32 length = data->Length;
    const byte* dataPtr = data->Data;
    UINT32 offset = 0;

    DWORD riffDataSize = 0;

    auto ReadChunk = [&](DWORD fourcc, DWORD& outChunkSize, DWORD& outChunkPos) -> HRESULT
    {
        while (true)
        {
            if (offset + sizeof(DWORD) * 2 >= length)
            {
                return E_FAIL;
            }

            // Read two DWORDs.
            DWORD chunkType = *reinterpret_cast<const DWORD*>(&dataPtr[offset]);
            DWORD chunkSize = *reinterpret_cast<const DWORD*>(&dataPtr[offset + sizeof(DWORD)]);
            offset += sizeof(DWORD) * 2;

            if (chunkType == MAKEFOURCC('R', 'I', 'F', 'F'))
            {
                riffDataSize = chunkSize;
                chunkSize = sizeof(DWORD);
                outChunkSize = sizeof(DWORD);
                outChunkPos = offset;
            }
            else
            {
                outChunkSize = chunkSize;
                outChunkPos = offset;
            }

            offset += chunkSize;

            if (chunkType == fourcc)
            {
                return S_OK;
            }
        }
    };

    // Locate riff chunk, check the file type.
    DWORD chunkSize = 0;
    DWORD chunkPos = 0;

    DX::ThrowIfFailed(ReadChunk(MAKEFOURCC('R', 'I', 'F', 'F'), chunkSize, chunkPos));
    if (*reinterpret_cast<const DWORD *>(&dataPtr[chunkPos]) != MAKEFOURCC('W', 'A', 'V', 'E')) DX::ThrowIfFailed(E_FAIL);

    // Locate 'fmt ' chunk, copy to WAVEFORMATEXTENSIBLE.
    DX::ThrowIfFailed(ReadChunk(MAKEFOURCC('f', 'm', 't', ' '), chunkSize, chunkPos));
    DX::ThrowIfFailed((chunkSize <= sizeof(m_waveFormat)) ? S_OK : E_FAIL);
    CopyMemory(&m_waveFormat, &dataPtr[chunkPos], chunkSize);

    // Locate the 'data' chunk and copy its contents to a buffer.
    DX::ThrowIfFailed(ReadChunk(MAKEFOURCC('d', 'a', 't', 'a'), chunkSize, chunkPos));
    m_data.resize(chunkSize);
    CopyMemory(m_data.data(), &dataPtr[chunkPos], chunkSize);

    m_offset = 0;
}

void MediaStreamer::ReadAll(uint8* buffer, uint32 maxBufferSize, uint32* bufferLength)
{
    UINT32 toCopy = m_data.size() - m_offset;
    if (toCopy > maxBufferSize) toCopy = maxBufferSize;

    CopyMemory(buffer, m_data.data(), toCopy);
    *bufferLength = toCopy;

    m_offset += toCopy;
    if (m_offset > m_data.size()) m_offset = m_data.size();
}

void MediaStreamer::Restart()
{
    m_offset = 0;
}