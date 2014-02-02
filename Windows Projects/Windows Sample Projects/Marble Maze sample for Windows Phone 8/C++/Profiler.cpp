//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "Profiler.h"
#include "DirectXSample.h"
#include "BasicLoader.h"

using namespace DirectX;
using namespace Microsoft::WRL;

/**
 * When ENABLE_PROFILER is not defined all Profiler methods become a no-op.
**/
#ifdef ENABLE_PROFILER

////////////////////////////////////////////////////////////////////////////////

static const int RectangleCount = 32;
static const int VertexCount = RectangleCount * 4;

////////////////////////////////////////////////////////////////////////////////

std::stack<Profiler::SampleStart> Profiler::s_activeSamples;
std::priority_queue<Profiler::Sample> Profiler::s_samples;
Profiler::Listener* Profiler::s_listener = nullptr;
bool Profiler::s_visible = false;

static ULONGLONG s_frameStart;
static ULONGLONG s_frequency;

static ComPtr<ID3D11VertexShader> s_vertexShader;
static ComPtr<ID3D11PixelShader> s_pixelShader;
static ComPtr<ID3D11InputLayout> s_vertexLayout;
static ComPtr<ID3D11Buffer> s_vertexBuffer;
static ComPtr<ID3D11Buffer> s_indexBuffer;
static ComPtr<ID3D11DepthStencilState> s_stencilState;

static ID3D11Device* s_device = nullptr;

////////////////////////////////////////////////////////////////////////////////

struct VertexPositionColor
{
    XMFLOAT3 pos;
    XMFLOAT3 color;
};

static double TimeToSeconds(ULONGLONG time)
{
    return static_cast<double>(time) / static_cast<double>(s_frequency);
}

static ULONGLONG CurrentTime()
{
    LARGE_INTEGER currentTime;

    if (!QueryPerformanceCounter(&currentTime))
    {
        throw ref new Platform::FailureException();
    }

    return currentTime.QuadPart;
}

////////////////////////////////////////////////////////////////////////////////

#endif // ENABLE_PROFILER

void Profiler::Initialize()
{
#ifdef ENABLE_PROFILER
    LARGE_INTEGER frequency;

    if (!QueryPerformanceFrequency(&frequency))
    {
        throw ref new Platform::FailureException();
    }

    s_frequency = frequency.QuadPart;
#endif // ENABLE_PROFILER
}

void Profiler::Uninitialize()
{
#ifdef ENABLE_PROFILER
    s_vertexShader = nullptr;
    s_pixelShader = nullptr;
    s_vertexLayout = nullptr;
    s_vertexBuffer = nullptr;
    s_indexBuffer = nullptr;
    s_stencilState = nullptr;

    s_device = nullptr;
#endif // ENABLE_PROFILER
}

void Profiler::Begin(PCWSTR name, const XMVECTORF32& color)
{
#ifdef ENABLE_PROFILER
    s_activeSamples.push(SampleStart(CurrentTime(), Identifier(name, color)));
#endif // ENABLE_PROFILER
}

void Profiler::End()
{
#ifdef ENABLE_PROFILER
    assert(!s_activeSamples.empty());

    Sample sample(s_activeSamples.top().startTime, CurrentTime(), s_activeSamples.size() - 1, s_activeSamples.top().ident);
    
    if (s_listener)
    {
        s_listener->OnProfileRegion(sample.ident, TimeToSeconds(sample.endTime - sample.startTime), TimeToSeconds(sample.endTime - s_frameStart));
    }

    s_samples.push(sample);
    s_activeSamples.pop();
#endif // ENABLE_PROFILER
}

void Profiler::SetVisible(bool visible)
{
#ifdef ENABLE_PROFILER
    s_visible = visible;
#endif // ENABLE_PROFILE
}

void Profiler::SetListener(Listener* listener)
{
#ifdef ENABLE_PROFILER
    s_listener = listener;
#endif // ENABLE_PROFILE
}

void Profiler::FrameStart()
{
#ifdef ENABLE_PROFILER
    // Empty frame state.
    if (!s_samples.empty()) std::priority_queue<Profiler::Sample>().swap(s_samples);
    if (!s_activeSamples.empty()) std::stack<Profiler::SampleStart>().swap(s_activeSamples);
    
    s_frameStart = CurrentTime();

    if (s_listener)
    {
        s_listener->OnProfileFrameStart();
    }
#endif // ENABLE_PROFILER
}

#ifdef ENABLE_PROFILER
static void InitializeResources(ID3D11Device* device)
{
    // Initialize resources.
    s_device = device;

    D3D11_DEPTH_STENCIL_DESC DSDesc;
    ZeroMemory(&DSDesc, sizeof(D3D11_DEPTH_STENCIL_DESC));
    DSDesc.DepthEnable = FALSE;
    DSDesc.DepthFunc = D3D11_COMPARISON_ALWAYS;
    DSDesc.DepthWriteMask = D3D11_DEPTH_WRITE_MASK_ZERO;
    DX::ThrowIfFailed(
        device->CreateDepthStencilState(&DSDesc, &s_stencilState)
        );

    BasicLoader^ loader = ref new BasicLoader(device);

    // Define vertex data format.
    D3D11_INPUT_ELEMENT_DESC vertexDesc[] = 
    {
        { "POSITION", 0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 0,  D3D11_INPUT_PER_VERTEX_DATA, 0 },
        { "COLOR",    0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 12, D3D11_INPUT_PER_VERTEX_DATA, 0 }
    };

    loader->LoadShader(L"UntexturedVertexShader.cso", vertexDesc, ARRAYSIZE(vertexDesc), &s_vertexShader, &s_vertexLayout);
    loader->LoadShader(L"UntexturedPixelShader.cso", &s_pixelShader);

    // Create vertex/index buffers.
    CD3D11_BUFFER_DESC vertexBufferDesc(VertexCount * sizeof(VertexPositionColor), D3D11_BIND_VERTEX_BUFFER, D3D11_USAGE_DYNAMIC, D3D11_CPU_ACCESS_WRITE);
    DX::ThrowIfFailed(
        device->CreateBuffer(
            &vertexBufferDesc,
            nullptr,
            &s_vertexBuffer
            )
        );

    unsigned short indices[RectangleCount * 6];

    int currentIndex = 0;
    for (int i = 0; i < RectangleCount; ++i)
    {
        int offset = i * 4;
        indices[currentIndex++] = offset + 1;
        indices[currentIndex++] = offset + 3;
        indices[currentIndex++] = offset + 2;

        indices[currentIndex++] = offset + 2;
        indices[currentIndex++] = offset + 0;
        indices[currentIndex++] = offset + 1;
    }

    D3D11_SUBRESOURCE_DATA indexBufferData = {0};
    indexBufferData.pSysMem = indices;
    CD3D11_BUFFER_DESC indexBufferDesc(sizeof(indices), D3D11_BIND_INDEX_BUFFER);
    DX::ThrowIfFailed(
        device->CreateBuffer(
            &indexBufferDesc,
            &indexBufferData,
            &s_indexBuffer
            )
        );
}

static void DrawRectangles(ID3D11DeviceContext* context, int indexCount)
{
    UINT stride = sizeof(VertexPositionColor);
    UINT offset = 0;
    context->IASetVertexBuffers(
        0,
        1,
        s_vertexBuffer.GetAddressOf(),
        &stride,
        &offset
        );
    
    context->DrawIndexed(
        indexCount * 6,
        0,
        0
        );
}
#endif // ENABLE_PROFILER

void Profiler::Render(ID3D11Device* device, ID3D11DeviceContext* context) {
#ifdef ENABLE_PROFILER
    if (!s_visible) return;
    if (device != s_device) InitializeResources(device);

    // Look up the current viewport.
    D3D11_VIEWPORT viewport;
    UINT viewportCount = 1;

    context->RSGetViewports(&viewportCount, &viewport);

    // Constants.
    const float k_frameDuration = 1.0f / 60.0f; // Length of frame in seconds.
    const float k_frameWidthPercent = 0.6f; // Expected width, in percent, of one frame.
    const float k_secondsToOffset = k_frameWidthPercent * 2.0f / k_frameDuration;
    const XMFLOAT2 k_pixelRatio(2.0f / viewport.Width, 2.0f / viewport.Height);
    const int k_barHeight = 16; // Pixels.
    const int k_nestedBarOffset = 5; // Pixels.

    const XMFLOAT2 k_topLeft(-1.0f, -1.0f + k_barHeight);
    const XMFLOAT2 k_bottomRight(1.0f, -1.0f);

    // Prepare for drawing.
    // Save previous state.
    ComPtr<ID3D11DepthStencilState> oldStencilState;
    UINT oldStencilRef = 0;
    context->OMGetDepthStencilState(&oldStencilState, &oldStencilRef);


    context->IASetIndexBuffer(
        s_indexBuffer.Get(),
        DXGI_FORMAT_R16_UINT,
        0
        );
    
    context->OMSetDepthStencilState(s_stencilState.Get(), 0);

    context->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);

    context->IASetInputLayout(s_vertexLayout.Get());

    context->VSSetShader(
        s_vertexShader.Get(),
        nullptr,
        0
        );

    context->PSSetShader(
        s_pixelShader.Get(),
        nullptr,
        0
        );

    int rectangleIndex = 0;
    int index = 0;
    D3D11_MAPPED_SUBRESOURCE MappedResource;

    context->Map(s_vertexBuffer.Get(), 0, D3D11_MAP_WRITE_DISCARD, 0, &MappedResource);
    {
        VertexPositionColor* vertices = reinterpret_cast<VertexPositionColor*>(MappedResource.pData);

        auto DrawBar = [&](float start, float end, int bottom, int height, XMFLOAT3 color)
        {
            float topPos = k_bottomRight.y + (bottom + height) * k_pixelRatio.y;
            float bottomPos = k_bottomRight.y + bottom * k_pixelRatio.y;

            vertices[index].pos = XMFLOAT3(k_topLeft.x + start, topPos, 1.0f);
            vertices[index + 1].pos = XMFLOAT3(k_topLeft.x + end, topPos, 1.0f);
            vertices[index + 2].pos = XMFLOAT3(k_topLeft.x + start, bottomPos, 1.0f);
            vertices[index + 3].pos = XMFLOAT3(k_topLeft.x + end, bottomPos, 1.0f);

            vertices[index].color = color;
            vertices[index + 1].color = color;
            vertices[index + 2].color = color;
            vertices[index + 3].color = color;

            index += 4;
            ++rectangleIndex;

            if (rectangleIndex == RectangleCount)
            {
                context->Unmap(s_vertexBuffer.Get(), 0);
                DrawRectangles(context, rectangleIndex);
                rectangleIndex = 0;
                index = 0;
                context->Map(s_vertexBuffer.Get(), 0, D3D11_MAP_WRITE_DISCARD, 0, &MappedResource);
            }
        };

        // Draw black background bar.
        DrawBar(0.0f, 2.0f, 0, k_barHeight, XMFLOAT3(0, 0, 0));
        
        int indicatorHeight = k_barHeight + k_nestedBarOffset;
        if (!s_samples.empty())
        {
            indicatorHeight = max(s_samples.top().depth * 2 + k_barHeight / 2, k_barHeight) + k_nestedBarOffset;
        }

        while (!s_samples.empty())
        {
            const Sample& samp = s_samples.top();

            float start = static_cast<float>(TimeToSeconds(samp.startTime - s_frameStart) * k_secondsToOffset);
            float end = static_cast<float>(TimeToSeconds(samp.endTime - s_frameStart) * k_secondsToOffset);

            DrawBar(start, end, k_nestedBarOffset * samp.depth, k_barHeight - k_nestedBarOffset * samp.depth, samp.ident.color);

            s_samples.pop();
        }

        // Final draw: "frame duration" indicator.
        DrawBar(k_frameWidthPercent * 2.0f, k_frameWidthPercent * 2.0f + 1.0f * k_pixelRatio.x, 0, indicatorHeight, XMFLOAT3(1.0f, 1.0f, 1.0f));
    }

    context->Unmap(s_vertexBuffer.Get(), 0);

    if (rectangleIndex) DrawRectangles(context, rectangleIndex);

    // Restore state.
    context->OMSetDepthStencilState(oldStencilState.Get(), oldStencilRef);
#endif // ENABLE_PROFILER
}
