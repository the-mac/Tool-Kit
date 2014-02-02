//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "ProfilerGraph.h"
#include "Profiler.h"

#include "DirectXSample.h"
#include "BasicLoader.h"

using namespace DirectX;
using namespace Microsoft::WRL;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;

struct Vertex
{
    XMFLOAT2 pos;
};

struct VertexPositionBuffer
{
    XMFLOAT4 scale;
    XMFLOAT4 offset;
    XMFLOAT4 frameOffset;
};

////////////////////////////////////////////////////////////////////////////////

ProfilerGraph::ProfilerGraph()
{
#ifdef ENABLE_PROFILER
    m_currentSlot = 0;
    m_frameTimeTotal = 0.0f;
    m_framesCounted = 0;
    m_frameCurrentSlot = 0;

    // Zero out data.
    for (int i = 0; i < HistorySize; ++i) m_history.frames[i] = XMFLOAT4(0.0f, 0.0f, 0.0f, 0.0f);
    for (int i = 0; i < 4; ++i) m_colors.colors[i] = XMFLOAT4(0.0f, 0.0f, 0.0f, 0.0f);
    for (int i = 0; i < HistorySize; ++i) m_frameTimeHistory[i] = 0.0f;

    Profiler::SetListener(this);

#endif // ENABLE_PROFILER
}

ProfilerGraph::~ProfilerGraph()
{
#ifdef ENABLE_PROFILER
    Profiler::SetListener(nullptr);
#endif // ENABLE_PROFILER
}

void ProfilerGraph::Initialize()
{
#ifdef ENABLE_PROFILER
    ID3D11Device* device = UserInterface::GetD3DDevice();

    // Create the blend state.
    D3D11_BLEND_DESC blendDesc = {0};
    blendDesc.RenderTarget[0].BlendEnable = TRUE;
    blendDesc.RenderTarget[0].SrcBlend = D3D11_BLEND_SRC_ALPHA;
    blendDesc.RenderTarget[0].DestBlend = D3D11_BLEND_INV_SRC_ALPHA;
    blendDesc.RenderTarget[0].BlendOp = D3D11_BLEND_OP_ADD;
    blendDesc.RenderTarget[0].SrcBlendAlpha = D3D11_BLEND_ONE;
    blendDesc.RenderTarget[0].DestBlendAlpha = D3D11_BLEND_ZERO;
    blendDesc.RenderTarget[0].BlendOpAlpha = D3D11_BLEND_OP_ADD;
    blendDesc.RenderTarget[0].RenderTargetWriteMask = D3D11_COLOR_WRITE_ENABLE_ALL;

    DX::ThrowIfFailed(
        device->CreateBlendState(
            &blendDesc,
            &m_blendState
            )
        );

    D3D11_DEPTH_STENCIL_DESC DSDesc;
    ZeroMemory(&DSDesc, sizeof(D3D11_DEPTH_STENCIL_DESC));
    DSDesc.DepthEnable = FALSE;
    DSDesc.DepthFunc = D3D11_COMPARISON_ALWAYS;
    DSDesc.DepthWriteMask = D3D11_DEPTH_WRITE_MASK_ZERO;
    DX::ThrowIfFailed(
        device->CreateDepthStencilState(&DSDesc, &m_stencilState)
        );

    BasicLoader^ loader = ref new BasicLoader(device);

    // Define vertex data format
    D3D11_INPUT_ELEMENT_DESC vertexDesc[] = 
    {
        { "POSITION", 0, DXGI_FORMAT_R32G32_FLOAT, 0, 0, D3D11_INPUT_PER_VERTEX_DATA, 0 }
    };

    loader->LoadShader(L"ProfilerGraphVertexShader.cso", vertexDesc, ARRAYSIZE(vertexDesc), &m_vertexShader, &m_vertexLayout);
    loader->LoadShader(L"ProfilerGraphPixelShader.cso", &m_pixelShader);

    // Create vertex buffers.
    Vertex vertices[HistorySize * 2];
    const float k_columnWidth = 1.0f / (HistorySize - 1);
    float vertexX = 0.0f;

    for (int i = 0, len = HistorySize * 2; i < len; i += 2) {
        vertices[i].pos = XMFLOAT2(vertexX, 0.0f);
        vertices[i + 1].pos = XMFLOAT2(vertexX, 1.0f);
        vertexX += k_columnWidth;
    }

    D3D11_SUBRESOURCE_DATA vertexBufferData = {0};
    vertexBufferData.pSysMem = vertices;
    CD3D11_BUFFER_DESC vertexBufferDesc(ARRAYSIZE(vertices) * sizeof(Vertex), D3D11_BIND_VERTEX_BUFFER, D3D11_USAGE_IMMUTABLE);
    DX::ThrowIfFailed(
        device->CreateBuffer(
            &vertexBufferDesc,
            &vertexBufferData,
            &m_vertexBuffer
            )
        );

    // Dynamic shader buffers.
    CD3D11_BUFFER_DESC vertexPositionBufferDesc(sizeof(VertexPositionBuffer), D3D11_BIND_CONSTANT_BUFFER, D3D11_USAGE_DYNAMIC, D3D11_CPU_ACCESS_WRITE);
    DX::ThrowIfFailed(
        device->CreateBuffer(
            &vertexPositionBufferDesc,
            nullptr,
            &m_vertexPositionBuffer
            )
        );

    CD3D11_BUFFER_DESC historyBufferDesc(sizeof(History), D3D11_BIND_CONSTANT_BUFFER, D3D11_USAGE_DYNAMIC, D3D11_CPU_ACCESS_WRITE);
    DX::ThrowIfFailed(
        device->CreateBuffer(
            &historyBufferDesc,
            nullptr,
            &m_historyBuffer
            )
        );

    CD3D11_BUFFER_DESC colorBufferDesc(sizeof(Colors), D3D11_BIND_CONSTANT_BUFFER, D3D11_USAGE_DYNAMIC, D3D11_CPU_ACCESS_WRITE);
    DX::ThrowIfFailed(
        device->CreateBuffer(
            &colorBufferDesc,
            nullptr,
            &m_colorBuffer
            )
        );


    m_fpsText.Initialize();
    m_fpsText.SetFont(L"Media\\Fonts\\DebugFont.spritefont");
    m_fpsText.SetTextColor(DirectX::Colors::Green);
    m_fpsText.SetContainer(Rect(0.0f, 0.0f, 100.0f, 100.0f));
#endif // ENABLE_PROFILER
}

void ProfilerGraph::Update(float timeTotal, float timeDelta)
{
#ifdef ENABLE_PROFILER
    m_frameTimeTotal += -m_frameTimeHistory[m_frameCurrentSlot] + timeDelta;
    m_frameTimeHistory[m_frameCurrentSlot] = timeDelta;

    ++m_framesCounted;
    if (m_framesCounted > HistorySize) m_framesCounted = HistorySize;

    ++m_frameCurrentSlot;
    if (m_frameCurrentSlot >= HistorySize) m_frameCurrentSlot = 0;

    WCHAR buffer[64] = {0};
    swprintf_s(
        buffer,
        L"%d FPS, %d kb",
        static_cast<int>(m_framesCounted / m_frameTimeTotal),
        static_cast<int>(Windows::Phone::System::Memory::MemoryManager::ProcessCommittedBytes / 1024ULL));

    m_fpsText.SetText(buffer);
    m_fpsText.Update(timeTotal, timeDelta);
#endif // ENABLE_PROFILER
}

void ProfilerGraph::Render()
{
#ifdef ENABLE_PROFILER
    if (m_state != State::Disabled)
    {
        m_fpsText.Render();
    }

    if (m_state != State::Graph) return;

    ID3D11DeviceContext* context = UserInterface::GetD3DContext();
    const float screenScale = UserInterface::GetDpi() / 96.0f;

    // Save previous state.
    ComPtr<ID3D11DepthStencilState> oldStencilState;
    UINT oldStencilRef = 0;
    context->OMGetDepthStencilState(&oldStencilState, &oldStencilRef);

    // Look up the current viewport.
    D3D11_VIEWPORT viewport;
    UINT viewportCount = 1;

    context->RSGetViewports(&viewportCount, &viewport);

    D3D11_MAPPED_SUBRESOURCE MappedResource;
    context->Map(m_vertexPositionBuffer.Get(), 0, D3D11_MAP_WRITE_DISCARD, 0, &MappedResource);
    {
        VertexPositionBuffer* input = reinterpret_cast<VertexPositionBuffer*>(MappedResource.pData);

        // To scroll the graph to the left, we offset retrieving the data each frame so that new
        // data always appears to be coming in from the right.  This calculation returns a value between 0 and almost 1.
        input->frameOffset.x = static_cast<float>(m_currentSlot) / HistorySize;

        // Position and scale the chart.
        input->scale = XMFLOAT4(m_container.Width * 2.0f / viewport.Width * screenScale, m_container.Height * 2.0f / viewport.Height * screenScale, 1.0f, 1.0f);
        input->offset = XMFLOAT4(m_container.X * 2.0f / viewport.Width * screenScale - 1.0f, m_container.Y * 2.0f / viewport.Height * screenScale - 1.0f, 0.0f, 0.0f);
    }
    context->Unmap(m_vertexPositionBuffer.Get(), 0);

    // Update the graph data.
    context->Map(m_historyBuffer.Get(), 0, D3D11_MAP_WRITE_DISCARD, 0, &MappedResource);
    CopyMemory(MappedResource.pData, &m_history, sizeof(History));
    context->Unmap(m_historyBuffer.Get(), 0);

    context->Map(m_colorBuffer.Get(), 0, D3D11_MAP_WRITE_DISCARD, 0, &MappedResource);
    CopyMemory(MappedResource.pData, &m_colors, sizeof(Colors));
    context->Unmap(m_colorBuffer.Get(), 0);

    UINT stride = sizeof(Vertex);
    UINT offset = 0;
    context->IASetVertexBuffers(
        0,
        1,
        m_vertexBuffer.GetAddressOf(),
        &stride,
        &offset
        );

    context->OMSetDepthStencilState(m_stencilState.Get(), 0);

    context->OMSetBlendState(m_blendState.Get(), nullptr, 0xFFFFFFFF);

    context->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLESTRIP);

    context->IASetInputLayout(m_vertexLayout.Get());

    ID3D11Buffer* buffers[] = { m_historyBuffer.Get(), m_vertexPositionBuffer.Get() }; 
    context->VSSetConstantBuffers(0, ARRAYSIZE(buffers), buffers);

    context->PSSetConstantBuffers(0, 1, m_colorBuffer.GetAddressOf());

    context->VSSetShader(
        m_vertexShader.Get(),
        nullptr,
        0
        );

    context->PSSetShader(
        m_pixelShader.Get(),
        nullptr,
        0
        );

    context->Draw(HistorySize * 2, 0);

    // Restore state.
    context->OMSetDepthStencilState(oldStencilState.Get(), oldStencilRef);
#endif // ENABLE_PROFILER
}

void ProfilerGraph::HitTest(Point point)
{
#ifdef ENABLE_PROFILER
    // Scale to screen coordinates.
    const float scale = UserInterface::GetDpi() / 96.0f;
    point = Point(point.X * scale, point.Y * scale);

    // Look up the current viewport.
    D3D11_VIEWPORT viewport;
    UINT viewportCount = 1;

    UserInterface::GetD3DContext()->RSGetViewports(&viewportCount, &viewport);

    point.X -= viewport.TopLeftX;
    point.Y -= viewport.TopLeftY;

    const float k_triggerAreaSize = 75.0f * scale; // 75 dips.
    if (point.X > viewport.Width - k_triggerAreaSize &&
           point.Y > viewport.Height - k_triggerAreaSize)
    {
        Toggle();
    }
#endif // ENABLE_PROFILER
}

void ProfilerGraph::Toggle()
{
#ifdef ENABLE_PROFILER
    switch (m_state)
    {
    case State::Disabled:
        m_state = State::FPSCounter;
        break;
    case State::FPSCounter:
        m_state = State::ColorBar;
        Profiler::SetVisible(true);
        break;
    case State::ColorBar:
        m_state = State::Graph;
        break;
    case State::Graph:
        m_state = State::Disabled;
        Profiler::SetVisible(false);
        break;
    }
#endif // ENABLE_PROFILER
}

void ProfilerGraph::OnProfileFrameStart()
{
#ifdef ENABLE_PROFILER
    ++m_currentSlot;
    if (m_currentSlot == HistorySize)
    {
        m_currentSlot = 0;
    }
#endif // ENABLE_PROFILER
}

void ProfilerGraph::OnProfileRegion(const Profiler::Identifier& identifier, double duration, double endTime)
{
#ifdef ENABLE_PROFILER
    const float k_secondsToGraphPercentage = 30.0f; // Graph is 1/30th of a second "tall".
    XMFLOAT4 color(identifier.color.x, identifier.color.y, identifier.color.z, 1.0f);

    float height = static_cast<float>(duration * k_secondsToGraphPercentage);

    // In the order that the events occur.
    if (identifier.name == ref new Platform::String(L"Physics"))
    {
        m_colors.colors[0] = color;
        m_history.frames[m_currentSlot].x = height;
    }
    else if (identifier.name == ref new Platform::String(L"Update"))
    {
        // Subtract time spent on physics from update, as it is nested within.
        m_colors.colors[1] = color;
        m_history.frames[m_currentSlot].y = height - m_history.frames[m_currentSlot].x;
    }
    else if (identifier.name == ref new Platform::String(L"Render"))
    {
        m_colors.colors[2] = color;
        m_history.frames[m_currentSlot].z = height;
    }
    else if (identifier.name == ref new Platform::String(L"Present"))
    {
        m_colors.colors[3] = color;
        m_history.frames[m_currentSlot].w = height;
    }
#endif // ENABLE_PROFILER
}
