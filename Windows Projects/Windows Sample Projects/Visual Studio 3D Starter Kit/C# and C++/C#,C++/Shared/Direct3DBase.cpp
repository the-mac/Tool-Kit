// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "Direct3DBase.h" 
#include <math.h>

#ifndef _PHONE
#include <windows.ui.xaml.media.dxinterop.h>
#endif

using namespace DirectX;
using namespace Microsoft::WRL;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::Core;

// Constructor.
Direct3DBase::Direct3DBase()
{
}

void Direct3DBase::CreateDepthStencilView()
{
    // Create a depth stencil view for use with 3D rendering if needed.
    CD3D11_TEXTURE2D_DESC depthStencilDesc(
        DXGI_FORMAT_D24_UNORM_S8_UINT, 
        static_cast<UINT>(m_d3dRenderTargetSize.Width),
        static_cast<UINT>(m_d3dRenderTargetSize.Height),
        1,
        1,
        D3D11_BIND_DEPTH_STENCIL
        );

    ComPtr<ID3D11Texture2D> depthStencil;
    DX::ThrowIfFailed(
        m_d3dDevice->CreateTexture2D(
        &depthStencilDesc,
        nullptr,
        &depthStencil
        )
        );

    CD3D11_DEPTH_STENCIL_VIEW_DESC depthStencilViewDesc(D3D11_DSV_DIMENSION_TEXTURE2D);
    DX::ThrowIfFailed(
        m_d3dDevice->CreateDepthStencilView(
        depthStencil.Get(),
        &depthStencilViewDesc,
        &m_d3dDepthStencilView
        )
        );
}

void Direct3DBase::SetViewport()
{
    // Set the 3D rendering viewport to target the entire window.
    CD3D11_VIEWPORT viewport(
        0.0f,
        0.0f,
        m_d3dRenderTargetSize.Width,
        m_d3dRenderTargetSize.Height
        );

    m_d3dContext->RSSetViewports(1, &viewport);
}

// Constants used to calculate screen rotations
namespace ScreenRotation
{
    // 0-degree Z-rotation
    const XMFLOAT4X4 Rotation0 = XMFLOAT4X4( 
        1.0f, 0.0f, 0.0f, 0.0f,
        0.0f, 1.0f, 0.0f, 0.0f,
        0.0f, 0.0f, 1.0f, 0.0f,
        0.0f, 0.0f, 0.0f, 1.0f
        );

    // 90-degree Z-rotation
    const XMFLOAT4X4 Rotation90 = XMFLOAT4X4(
        0.0f, 1.0f, 0.0f, 0.0f,
        -1.0f, 0.0f, 0.0f, 0.0f,
        0.0f, 0.0f, 1.0f, 0.0f,
        0.0f, 0.0f, 0.0f, 1.0f
        );

    // 180-degree Z-rotation
    const XMFLOAT4X4 Rotation180 = XMFLOAT4X4(
        -1.0f, 0.0f, 0.0f, 0.0f,
        0.0f, -1.0f, 0.0f, 0.0f,
        0.0f, 0.0f, 1.0f, 0.0f,
        0.0f, 0.0f, 0.0f, 1.0f
        );

    // 270-degree Z-rotation
    const XMFLOAT4X4 Rotation270 = XMFLOAT4X4( 
        0.0f, -1.0f, 0.0f, 0.0f,
        1.0f, 0.0f, 0.0f, 0.0f,
        0.0f, 0.0f, 1.0f, 0.0f,
        0.0f, 0.0f, 0.0f, 1.0f
        );
};

#ifndef _PHONE
// Extends Direct3DBase with code specific for Windows Store apps
Direct3DWindowsBase::Direct3DWindowsBase() : m_dpi(-1.0f) 
{ 
}

// Allocate all memory resources that change on a window SizeChanged event.
void Direct3DWindowsBase::CreateWindowSizeDependentResources()
{
    // Store the window bounds so the next time we get a SizeChanged event we can
    // avoid rebuilding everything if the size is identical.
    m_windowBounds = m_window->Bounds;

    // Calculate the necessary swap chain and render target size in pixels.
    float windowWidth = ConvertDipsToPixels(m_windowBounds.Width);
    float windowHeight = ConvertDipsToPixels(m_windowBounds.Height);

    // The width and height of the swap chain must be based on the window's
    // landscape-oriented width and height. If the window is in a portrait
    // orientation, the dimensions must be reversed.
    m_orientation = DisplayProperties::CurrentOrientation;

    bool swapDimensions =  m_orientation == DisplayOrientations::Portrait || m_orientation == DisplayOrientations::PortraitFlipped;
    m_d3dRenderTargetSize.Width = swapDimensions ? windowHeight : windowWidth;
    m_d3dRenderTargetSize.Height = swapDimensions ? windowWidth : windowHeight;

    // The swap chain is not managed by the developer on Windows Phone XAML apps
    if (m_swapChain != nullptr)
    {
        // If the swap chain already exists, resize it.
        HRESULT hr = m_swapChain->ResizeBuffers(
            2, // Double-buffered swap chain.
            static_cast<UINT>(m_d3dRenderTargetSize.Width),
            static_cast<UINT>(m_d3dRenderTargetSize.Height),
            DXGI_FORMAT_B8G8R8A8_UNORM,
            0
            );

        if (hr == DXGI_ERROR_DEVICE_REMOVED)
        {
            HandleDeviceLost();
            return;
        }
        else
        {
            DX::ThrowIfFailed(hr);
        }
    }
    else
    {
        // Otherwise, create a new one using the same adapter as the existing Direct3D device.
        DXGI_SWAP_CHAIN_DESC1 swapChainDesc = {0};
        swapChainDesc.Width = static_cast<UINT>(m_d3dRenderTargetSize.Width); // Match the size of the window.
        swapChainDesc.Height = static_cast<UINT>(m_d3dRenderTargetSize.Height);
        swapChainDesc.Format = DXGI_FORMAT_B8G8R8A8_UNORM; // This is the most common swap chain format.
        swapChainDesc.Stereo = false; 
        swapChainDesc.SampleDesc.Count = 1; // Don't use multi-sampling.
        swapChainDesc.SampleDesc.Quality = 0;
        swapChainDesc.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
        swapChainDesc.BufferCount = 2; // Use double-buffering to minimize latency.
        swapChainDesc.Scaling = DXGI_SCALING_STRETCH;
        swapChainDesc.SwapEffect = DXGI_SWAP_EFFECT_FLIP_SEQUENTIAL; // All Windows Store apps must use this SwapEffect.
        swapChainDesc.Flags = 0;

        ComPtr<IDXGIDevice1> dxgiDevice;
        DX::ThrowIfFailed(
            m_d3dDevice.As(&dxgiDevice)
            );

        ComPtr<IDXGIAdapter> dxgiAdapter;
        DX::ThrowIfFailed(
            dxgiDevice->GetAdapter(&dxgiAdapter)
            );

        ComPtr<IDXGIFactory2> dxgiFactory;
        DX::ThrowIfFailed(
            dxgiAdapter->GetParent(IID_PPV_ARGS(&dxgiFactory))
            );

        DX::ThrowIfFailed(
            dxgiFactory->CreateSwapChainForComposition(
            m_d3dDevice.Get(),
            &swapChainDesc,
            nullptr,
            &m_swapChain
            )
            );

        // Associate the new swap chain with the SwapChainBackgroundPanel element.
        ComPtr<ISwapChainBackgroundPanelNative> panelNative;
        DX::ThrowIfFailed(
            reinterpret_cast<IUnknown*>(m_panel)->QueryInterface(IID_PPV_ARGS(&panelNative))
            );

        DX::ThrowIfFailed(
            panelNative->SetSwapChain(m_swapChain.Get())
            );

        // Ensure that DXGI does not queue more than one frame at a time. This both reduces latency and
        // ensures that the application will only render after each VSync, minimizing power consumption.
        DX::ThrowIfFailed(
            dxgiDevice->SetMaximumFrameLatency(1)
            );
    }

    // Set the proper orientation for the swap chain, and generate 2D and
    // 3D matrix transformations for rendering to the rotated swap chain.
    // Note the rotation angle for the 2D and 3D transforms are different.
    // This is due to the difference in coordinate spaces.  Additionally,
    // the 3D matrix is specified explicitly to avoid rounding errors.

    DXGI_MODE_ROTATION rotation = DXGI_MODE_ROTATION_UNSPECIFIED;
    switch (m_orientation)
    {
    case DisplayOrientations::Landscape:
        rotation = DXGI_MODE_ROTATION_IDENTITY;
        m_orientationTransform3D = ScreenRotation::Rotation0;
        break;
    case DisplayOrientations::Portrait:
        rotation = DXGI_MODE_ROTATION_ROTATE270;
        m_orientationTransform3D = ScreenRotation::Rotation90;
        break;
    case DisplayOrientations::LandscapeFlipped:
        rotation = DXGI_MODE_ROTATION_ROTATE180;
        m_orientationTransform3D = ScreenRotation::Rotation180;
        break;
    case DisplayOrientations::PortraitFlipped:
        rotation = DXGI_MODE_ROTATION_ROTATE90;
        m_orientationTransform3D = ScreenRotation::Rotation270;
        break;
    default:
        throw ref new Platform::FailureException();
    }

    DX::ThrowIfFailed(
        m_swapChain->SetRotation(rotation)
        );

    // Create a Direct3D render target view of the swap chain back buffer.
    ComPtr<ID3D11Texture2D> backBuffer;
    DX::ThrowIfFailed(
        m_swapChain->GetBuffer(0, IID_PPV_ARGS(&backBuffer))
        );

    DX::ThrowIfFailed(
        m_d3dDevice->CreateRenderTargetView(
        backBuffer.Get(),
        nullptr,
        &m_d3dRenderTargetView
        )
        );

    CreateDepthStencilView();
    SetViewport();
}

// These are the resources that depend on the device.
void Direct3DWindowsBase::CreateDeviceResources()
{
    // We only need to create a device on Windows Store apps. 
    // On Windows Phone, the XAML layer manages the device and context.

    // This flag adds support for surfaces with a different color channel ordering
    // than the API default. It is required for compatibility with Direct2D.
    UINT creationFlags = D3D11_CREATE_DEVICE_BGRA_SUPPORT;
    ComPtr<IDXGIDevice> dxgiDevice;

#if defined(_DEBUG)
    // If the project is in a debug build, enable debugging via SDK Layers with this flag.
    creationFlags |= D3D11_CREATE_DEVICE_DEBUG;
#endif

    // This array defines the set of DirectX hardware feature levels this app will support.
    // Note the ordering should be preserved.
    // Don't forget to declare your application's minimum required feature level in its
    // description.  All applications are assumed to support 9.1 unless otherwise stated.
    D3D_FEATURE_LEVEL featureLevels[] = 
    {
        D3D_FEATURE_LEVEL_11_1,
        D3D_FEATURE_LEVEL_11_0,
        D3D_FEATURE_LEVEL_10_1,
        D3D_FEATURE_LEVEL_10_0,
        D3D_FEATURE_LEVEL_9_3,
        D3D_FEATURE_LEVEL_9_2,
        D3D_FEATURE_LEVEL_9_1
    };

    // Create the Direct3D 11 API device object and a corresponding context.
    ComPtr<ID3D11Device> d3dDevice;
    ComPtr<ID3D11DeviceContext> d3dContext;
    if (FAILED(D3D11CreateDevice(
        nullptr, // Specify nullptr to use the default adapter.
        D3D_DRIVER_TYPE_HARDWARE,
        0,
        creationFlags, // Set debug and Direct2D compatibility flags.
        featureLevels, // List of feature levels this app can support.
        ARRAYSIZE(featureLevels),
        D3D11_SDK_VERSION, // Always set this to D3D11_SDK_VERSION for Windows Store apps.
        &d3dDevice, // Returns the Direct3D device created.
        &m_d3dFeatureLevel, // Returns feature level of device created.
        &d3dContext // Returns the device immediate context.
        )))
    {
        DX::ThrowIfFailed(D3D11CreateDevice(
            nullptr, // Specify nullptr to use the default adapter.
            D3D_DRIVER_TYPE_WARP,
            0,
            creationFlags, // Set debug and Direct2D compatibility flags.
            featureLevels, // List of feature levels this app can support.
            ARRAYSIZE(featureLevels),
            D3D11_SDK_VERSION, // Always set this to D3D11_SDK_VERSION for Windows Store apps.
            &d3dDevice, // Returns the Direct3D device created.
            &m_d3dFeatureLevel, // Returns feature level of device created.
            &d3dContext // Returns the device immediate context.
            ));
    }

    // Get the Direct3D 11.1 API device and context interfaces.
    DX::ThrowIfFailed(
        d3dDevice.As(&m_d3dDevice)
        );

    DX::ThrowIfFailed(
        d3dContext.As(&m_d3dContext)
        );
}

// Initialize the DirectX resources required to run.
void Direct3DWindowsBase::Initialize(CoreWindow^ window, Windows::UI::Xaml::Controls::SwapChainBackgroundPanel^ panel, float dpi)
{
    m_window = window;
    m_panel = panel;

    CreateDeviceResources();
    SetDpi(dpi);
}

// This method is called in the event handler for the SizeChanged event.
void Direct3DWindowsBase::UpdateForWindowSizeChange()
{
    // Only handle window size changed if there is no pending DPI change.
    if (m_dpi != DisplayProperties::LogicalDpi)
    {
        return;
    }

    if (m_window->Bounds.Width  != m_windowBounds.Width ||
        m_window->Bounds.Height != m_windowBounds.Height ||
        m_orientation != DisplayProperties::CurrentOrientation)
    {
        ID3D11RenderTargetView* nullViews[] = {nullptr};
        m_d3dContext->OMSetRenderTargets(ARRAYSIZE(nullViews), nullViews, nullptr);
        m_d3dRenderTargetView = nullptr;
        m_d3dDepthStencilView = nullptr;
        m_d3dContext->Flush();
        CreateWindowSizeDependentResources();
    }
}

// Recreate all device resources and set them back to the current state.
void Direct3DWindowsBase::HandleDeviceLost()
{
    // Reset these member variables to ensure that SetDpi recreates all resources.
    float dpi = m_dpi;
    m_dpi = -1.0f;
    m_windowBounds.Width = 0;
    m_windowBounds.Height = 0;
    m_swapChain = nullptr;

    CreateDeviceResources();
    SetDpi(dpi);
}

// This method is called in the event handler for the LogicalDpiChanged event.
void Direct3DWindowsBase::SetDpi(float dpi)
{
    if (dpi != m_dpi)
    {
        // Save the updated DPI value.
        m_dpi = dpi;

        // Often a DPI change implies a window size change. In some cases Windows will issue
        // both a size changed event and a DPI changed event. In this case, the resulting bounds 
        // will not change, and the window resize code will only be executed once.
        UpdateForWindowSizeChange();
    }
}

// Method to convert a length in device-independent pixels (DIPs) to a length in physical pixels.
float Direct3DWindowsBase::ConvertDipsToPixels(float dips)
{
    static const float dipsPerInch = 96.0f;
    return floor(dips * m_dpi / dipsPerInch + 0.5f); // Round to nearest integer.
}

// Method to deliver the final image to the display.
void Direct3DWindowsBase::Present()
{
    // The application may optionally specify "dirty" or "scroll"
    // rects to improve efficiency in certain scenarios.
    DXGI_PRESENT_PARAMETERS parameters = {0};
    parameters.DirtyRectsCount = 0;
    parameters.pDirtyRects = nullptr;
    parameters.pScrollRect = nullptr;
    parameters.pScrollOffset = nullptr;

    // The first argument instructs DXGI to block until VSync, putting the application
    // to sleep until the next VSync. This ensures we don't waste any cycles rendering
    // frames that will never be displayed to the screen.
    HRESULT hr = m_swapChain->Present1(1, 0, &parameters);

    // Discard the contents of the render target.
    // This is a valid operation only when the existing contents will be entirely
    // overwritten. If dirty or scroll rects are used, this call should be removed.
    m_d3dContext->DiscardView(m_d3dRenderTargetView.Get());

    // Discard the contents of the depth stencil.
    m_d3dContext->DiscardView(m_d3dDepthStencilView.Get());

    // If the device was removed either by a disconnect or a driver upgrade, we 
    // must recreate all device resources.
    if (hr == DXGI_ERROR_DEVICE_REMOVED)
    {
        HandleDeviceLost();
    }
    else
    {
        DX::ThrowIfFailed(hr);
    }
}

void Direct3DWindowsBase::ValidateDevice()
{
    ComPtr<IDXGIDevice1> dxgiDevice;
    ComPtr<IDXGIAdapter> deviceAdapter;
    DXGI_ADAPTER_DESC deviceDesc;
    DX::ThrowIfFailed(m_d3dDevice.As(&dxgiDevice));
    DX::ThrowIfFailed(dxgiDevice->GetAdapter(&deviceAdapter));
    DX::ThrowIfFailed(deviceAdapter->GetDesc(&deviceDesc));

    ComPtr<IDXGIFactory2> dxgiFactory;
    ComPtr<IDXGIAdapter1> currentAdapter;
    DXGI_ADAPTER_DESC currentDesc;
    DX::ThrowIfFailed(CreateDXGIFactory1(IID_PPV_ARGS(&dxgiFactory)));
    DX::ThrowIfFailed(dxgiFactory->EnumAdapters1(0, &currentAdapter));
    DX::ThrowIfFailed(currentAdapter->GetDesc(&currentDesc));

    if (deviceDesc.AdapterLuid.LowPart != currentDesc.AdapterLuid.LowPart ||
        deviceDesc.AdapterLuid.HighPart != currentDesc.AdapterLuid.HighPart ||
        FAILED(m_d3dDevice->GetDeviceRemovedReason()))
    {
        dxgiDevice = nullptr;
        deviceAdapter = nullptr;

        HandleDeviceLost();
    }
}

#else
// Extends Direct3DBase with code specific for Windows Phone apps
Direct3DPhoneBase::Direct3DPhoneBase() 
{
}

void Direct3DPhoneBase::CreateDeviceResources()
{
}

// Allocate all memory resources that change on a window SizeChanged event.
void Direct3DPhoneBase::CreateWindowSizeDependentResources()
{
    // On Windows Phone, the default orientation is Portrait, while on
    // Windows Store, the default orientation is Landscape
    bool swapDimensions = m_orientation == DisplayOrientations::Landscape || m_orientation == DisplayOrientations::LandscapeFlipped;
    m_windowBounds.Width = swapDimensions ? m_d3dRenderTargetSize.Height : m_d3dRenderTargetSize.Width;
    m_windowBounds.Height = swapDimensions ? m_d3dRenderTargetSize.Width : m_d3dRenderTargetSize.Height;

    // Set the proper orientation for the swap chain, and generate 2D and
    // 3D matrix transformations for rendering to the rotated swap chain.
    // The 3D matrix is specified explicitly to avoid rounding errors.
    switch (m_orientation)
    {
    case DisplayOrientations::Portrait:
        m_orientationTransform3D = ScreenRotation::Rotation0;
        break;
    case DisplayOrientations::Landscape:
        m_orientationTransform3D = ScreenRotation::Rotation90;
        break;
    case DisplayOrientations::LandscapeFlipped:
        m_orientationTransform3D = ScreenRotation::Rotation270;
        break;
    default:
        throw ref new Platform::FailureException();
    }

    CreateDepthStencilView();
    SetViewport();
}

// This method is called in the event handler for the SizeChanged event.
void Direct3DPhoneBase::UpdateForWindowSizeChange(float width, float height)
{
    m_windowBounds.Width = width;
    m_windowBounds.Height = height;
}

// Gets the device, context and render target view from the XAML layer, as well as the orientation of the device
void Direct3DPhoneBase::UpdateDevice(_In_ ID3D11Device1* device, _In_ ID3D11DeviceContext1* context, _In_ ID3D11RenderTargetView* renderTargetView, DisplayOrientations orientation)
{
    m_d3dContext = context;
    m_d3dRenderTargetView = renderTargetView;

    if (m_d3dDevice.Get() != device)
    {
        m_d3dDevice = device;
        CreateDeviceResources();

        // Force call to CreateWindowSizeDependentResources.
        m_d3dRenderTargetSize.Width  = -1;
        m_d3dRenderTargetSize.Height = -1;
    }

    ComPtr<ID3D11Resource> renderTargetViewResource;
    m_d3dRenderTargetView->GetResource(&renderTargetViewResource);

    DX::ThrowIfFailed(
        renderTargetViewResource.As(&m_backBuffer)
        );

    // Cache the rendertarget dimensions in our helper class for convenient use.
    D3D11_TEXTURE2D_DESC backBufferDesc;
    m_backBuffer->GetDesc(&backBufferDesc);

    if (m_d3dRenderTargetSize.Width  != static_cast<float>(backBufferDesc.Width) ||
        m_d3dRenderTargetSize.Height != static_cast<float>(backBufferDesc.Height) || 
        m_orientation != orientation)
    {
        m_d3dRenderTargetSize.Width  = static_cast<float>(backBufferDesc.Width);
        m_d3dRenderTargetSize.Height = static_cast<float>(backBufferDesc.Height);
        m_orientation = orientation;

        CreateWindowSizeDependentResources();
    }

    SetViewport();
}
#endif