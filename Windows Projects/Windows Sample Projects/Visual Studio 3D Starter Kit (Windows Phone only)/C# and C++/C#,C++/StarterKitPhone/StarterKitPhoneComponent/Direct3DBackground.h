// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "pch.h"
#include "../../Shared/BasicTimer.h"
#include "../../Shared/Game.h"
#include <DrawingSurfaceNative.h>

namespace StarterKitPhoneComponent
{

    public delegate void RequestAdditionalFrameHandler();

    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class Direct3DBackground sealed : public Windows::Phone::Input::Interop::IDrawingSurfaceManipulationHandler
    {
    public:
        Direct3DBackground();

        Windows::Phone::Graphics::Interop::IDrawingSurfaceBackgroundContentProvider^ CreateContentProvider();

        // IDrawingSurfaceManipulationHandler
        virtual void SetManipulationHost(Windows::Phone::Input::Interop::DrawingSurfaceManipulationHost^ manipulationHost);

        event RequestAdditionalFrameHandler^ RequestAdditionalFrame;

        property Windows::Foundation::Size WindowBounds;
        property Windows::Foundation::Size NativeResolution;
        property Windows::Foundation::Size RenderResolution;
        property Windows::Graphics::Display::DisplayOrientations Orientation;

        Platform::String^ OnHitObject(int x, int y);
        void ToggleHitEffect(Platform::String^ object);
        void ChangeMaterialColor(Platform::String^ object, float r, float g, float b);

    protected:
        // Event Handlers
        void OnPointerPressed(Windows::Phone::Input::Interop::DrawingSurfaceManipulationHost^ sender, Windows::UI::Core::PointerEventArgs^ args);
        void OnPointerReleased(Windows::Phone::Input::Interop::DrawingSurfaceManipulationHost^ sender, Windows::UI::Core::PointerEventArgs^ args);
        void OnPointerMoved(Windows::Phone::Input::Interop::DrawingSurfaceManipulationHost^ sender, Windows::UI::Core::PointerEventArgs^ args);

    internal:
        HRESULT Connect(_In_ IDrawingSurfaceRuntimeHostNative* host, _In_ ID3D11Device1* device);
        void Disconnect();

        HRESULT PrepareResources(_In_ const LARGE_INTEGER* presentTargetTime, _Inout_ DrawingSurfaceSizeF* desiredRenderTargetSize);
        HRESULT Draw(_In_ ID3D11Device1* device, _In_ ID3D11DeviceContext1* context, _In_ ID3D11RenderTargetView* renderTargetView);

    private:
        Game^ m_renderer;
        BasicTimer^ m_timer;
        BOOL m_deviceReady;
    };

}