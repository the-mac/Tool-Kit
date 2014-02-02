// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "Direct3DBackground.h"
#include "Direct3DContentProvider.h"

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;
using namespace Microsoft::WRL;
using namespace Windows::Phone::Graphics::Interop;
using namespace Windows::Phone::Input::Interop;

namespace StarterKitPhoneComponent
{

    Direct3DBackground::Direct3DBackground() :
        m_timer(ref new BasicTimer()),
        m_deviceReady(false)
    {
    }

    IDrawingSurfaceBackgroundContentProvider^ Direct3DBackground::CreateContentProvider()
    {
        ComPtr<Direct3DContentProvider> provider = Make<Direct3DContentProvider>(this);
        return reinterpret_cast<IDrawingSurfaceBackgroundContentProvider^>(provider.Detach());
    }

    // IDrawingSurfaceManipulationHandler
    void Direct3DBackground::SetManipulationHost(DrawingSurfaceManipulationHost^ manipulationHost)
    {
        manipulationHost->PointerPressed +=
            ref new TypedEventHandler<DrawingSurfaceManipulationHost^, PointerEventArgs^>(this, &Direct3DBackground::OnPointerPressed);

        manipulationHost->PointerMoved +=
            ref new TypedEventHandler<DrawingSurfaceManipulationHost^, PointerEventArgs^>(this, &Direct3DBackground::OnPointerMoved);

        manipulationHost->PointerReleased +=
            ref new TypedEventHandler<DrawingSurfaceManipulationHost^, PointerEventArgs^>(this, &Direct3DBackground::OnPointerReleased);
    }

    ///
    /// Proxy for renderer methods, simply redirects the calls
    ///
    String^ Direct3DBackground::OnHitObject(int x, int y)
    {
        return m_renderer->OnHitObject(x, y);
    }

    void Direct3DBackground::ToggleHitEffect(String^ object)
    {
        m_renderer->ToggleHitEffect(object);
    }

    void Direct3DBackground::ChangeMaterialColor(String^ object, float r, float g, float b)
    {
        m_renderer->ChangeMaterialColor(object, r, g, b);
    }

    // Event Handlers
    void Direct3DBackground::OnPointerPressed(DrawingSurfaceManipulationHost^ sender, PointerEventArgs^ args)
    {
        // Insert your code here.
    }

    void Direct3DBackground::OnPointerMoved(DrawingSurfaceManipulationHost^ sender, PointerEventArgs^ args)
    {
        // Insert your code here.
    }

    void Direct3DBackground::OnPointerReleased(DrawingSurfaceManipulationHost^ sender, PointerEventArgs^ args)
    {
        // Insert your code here.
    }

    // Interface With Direct3DContentProvider
    HRESULT Direct3DBackground::Connect(_In_ IDrawingSurfaceRuntimeHostNative* host, _In_ ID3D11Device1* device)
    {
        UNREFERENCED_PARAMETER (host);
        UNREFERENCED_PARAMETER (device);

        m_renderer = ref new Game();
        m_renderer->UpdateForWindowSizeChange(WindowBounds.Width, WindowBounds.Height);

        // Restart timer after renderer has finished initializing.
        m_timer->Reset();

        return S_OK;
    }

    void Direct3DBackground::Disconnect()
    {
        m_renderer = nullptr;
    }

    HRESULT Direct3DBackground::PrepareResources(_In_ const LARGE_INTEGER* presentTargetTime, _Inout_ DrawingSurfaceSizeF* desiredRenderTargetSize)
    {
        UNREFERENCED_PARAMETER (presentTargetTime);

        m_timer->Update();
        m_renderer->Update(m_timer->Total, m_timer->Delta);

        desiredRenderTargetSize->width = RenderResolution.Width;
        desiredRenderTargetSize->height = RenderResolution.Height;

        return S_OK;
    }

    HRESULT Direct3DBackground::Draw(_In_ ID3D11Device1* device, _In_ ID3D11DeviceContext1* context, _In_ ID3D11RenderTargetView* renderTargetView)
    {
        m_renderer->UpdateDevice(device, context, renderTargetView, Orientation);

        // On the first call we don't render because the device is being initialized, and
        // therefore we haven't gone through a full update cycle

        if (m_deviceReady)
        {
            m_renderer->Render();
        }
        else
        {
            m_deviceReady = true;
        }

        RequestAdditionalFrame();

        return S_OK;
    }

}