// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DirectXHelper.h"

// Helper class that initializes DirectX APIs for both 2D and 3D rendering.
// Some of the code in this class may be omitted if only 2D or only 3D rendering is being used.
ref class Direct3DBase abstract
{
internal:
    Direct3DBase();

protected private:
    virtual void CreateDeviceResources() = 0;
    virtual void CreateWindowSizeDependentResources() = 0;
    virtual void Render() = 0;

    void CreateDepthStencilView();
    void SetViewport();

    // DirectX Core Objects.
    Microsoft::WRL::ComPtr<ID3D11Device1> m_d3dDevice;
    Microsoft::WRL::ComPtr<ID3D11DeviceContext1> m_d3dContext;
    Microsoft::WRL::ComPtr<IDXGISwapChain1> m_swapChain;
    Microsoft::WRL::ComPtr<ID3D11Texture2D> m_backBuffer;
    Microsoft::WRL::ComPtr<ID3D11RenderTargetView> m_d3dRenderTargetView;
    Microsoft::WRL::ComPtr<ID3D11DepthStencilView> m_d3dDepthStencilView;

    // Cached renderer properties.
    D3D_FEATURE_LEVEL m_d3dFeatureLevel;
    Windows::Foundation::Size m_d3dRenderTargetSize;
    Windows::Foundation::Rect m_windowBounds;
    Windows::Graphics::Display::DisplayOrientations m_orientation;

    // Transforms used for display orientation.
    DirectX::XMFLOAT4X4 m_orientationTransform3D;
};

#if !defined(_PHONE) && defined(WINAPI_FAMILY) && WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP
#define _PHONE
#endif

#ifndef _PHONE

// Extends Direct3DBase with code specific for Windows Store apps
ref class Direct3DWindowsBase abstract : Direct3DBase
{
internal:
    Direct3DWindowsBase();

    virtual void Initialize(Windows::UI::Core::CoreWindow^ window, Windows::UI::Xaml::Controls::SwapChainBackgroundPanel^ panel, float dpi);
    virtual void SetDpi(float dpi);
    void ValidateDevice();
    virtual void Present();

protected private:
    virtual void CreateDeviceResources() override;
    virtual void CreateWindowSizeDependentResources() override;
    
    virtual void HandleDeviceLost();
    virtual float ConvertDipsToPixels(float dips);
    virtual void UpdateForWindowSizeChange();

    Platform::Agile<Windows::UI::Core::CoreWindow> m_window;
    Windows::UI::Xaml::Controls::SwapChainBackgroundPanel^ m_panel;
    float m_dpi;
};

#else

// Extends Direct3DBase with code specific for Windows Phone apps
ref class Direct3DPhoneBase abstract : Direct3DBase
{
internal:
    Direct3DPhoneBase();

    virtual void UpdateDevice(_In_ ID3D11Device1* device, _In_ ID3D11DeviceContext1* context, _In_ ID3D11RenderTargetView* renderTargetView, Windows::Graphics::Display::DisplayOrientations orientation);

protected private:
    virtual void CreateDeviceResources() override;
    virtual void CreateWindowSizeDependentResources() override;

    virtual void UpdateForWindowSizeChange(float width, float height);
};

#endif
