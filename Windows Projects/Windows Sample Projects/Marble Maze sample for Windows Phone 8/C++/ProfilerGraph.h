//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "UserInterface.h"
#include "Profiler.h"

class ProfilerGraph : public ElementBase, public Profiler::Listener
{
public:
    ProfilerGraph();
    virtual ~ProfilerGraph();

    virtual void Initialize();
    virtual void Update(float timeTotal, float timeDelta);
    virtual void Render();

    void HitTest(Windows::Foundation::Point point);
    void Toggle();

    // Profiler::Listener
    virtual void OnProfileFrameStart();
    virtual void OnProfileRegion(const Profiler::Identifier& identifier, double duration, double endTime);

protected:
#ifdef ENABLE_PROFILER
    static const unsigned int HistorySize = 100;

    enum class State {
        Disabled,
        FPSCounter,
        ColorBar,
        Graph
    };

    struct History
    {
        DirectX::XMFLOAT4 frames[HistorySize];
    };

    struct Colors
    {
        DirectX::XMFLOAT4 colors[4];
    };

    // FPS.
    TextElement m_fpsText;
    float m_frameTimeHistory[HistorySize];
    float m_frameTimeTotal;
    unsigned int m_frameCurrentSlot;
    unsigned int m_framesCounted;

    State m_state;

    History m_history;
    unsigned int m_currentSlot;
    Colors m_colors;

    Microsoft::WRL::ComPtr<ID3D11VertexShader> m_vertexShader;
    Microsoft::WRL::ComPtr<ID3D11PixelShader> m_pixelShader;
    Microsoft::WRL::ComPtr<ID3D11InputLayout> m_vertexLayout;
    Microsoft::WRL::ComPtr<ID3D11Buffer> m_vertexBuffer;
    Microsoft::WRL::ComPtr<ID3D11BlendState> m_blendState;
    Microsoft::WRL::ComPtr<ID3D11DepthStencilState> m_stencilState;

    Microsoft::WRL::ComPtr<ID3D11Buffer> m_vertexPositionBuffer;
    Microsoft::WRL::ComPtr<ID3D11Buffer> m_historyBuffer;
    Microsoft::WRL::ComPtr<ID3D11Buffer> m_colorBuffer;
#endif // ENABLE_PROFILER
};
