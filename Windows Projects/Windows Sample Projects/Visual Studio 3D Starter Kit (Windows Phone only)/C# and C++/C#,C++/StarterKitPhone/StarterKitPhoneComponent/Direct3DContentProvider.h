// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "pch.h"
#include <wrl/module.h>
#include <Windows.Phone.Graphics.Interop.h>
#include <DrawingSurfaceNative.h>

#include "Direct3DBackground.h"

class Direct3DContentProvider : public Microsoft::WRL::RuntimeClass<
    Microsoft::WRL::RuntimeClassFlags<Microsoft::WRL::WinRtClassicComMix>,
    ABI::Windows::Phone::Graphics::Interop::IDrawingSurfaceBackgroundContentProvider,
    IDrawingSurfaceBackgroundContentProviderNative>
{
public:
    Direct3DContentProvider(StarterKitPhoneComponent::Direct3DBackground^ controller);

    // IDrawingSurfaceContentProviderNative
    HRESULT STDMETHODCALLTYPE Connect(_In_ IDrawingSurfaceRuntimeHostNative* host, _In_ ID3D11Device1* device);
    void STDMETHODCALLTYPE Disconnect();

    HRESULT STDMETHODCALLTYPE PrepareResources(_In_ const LARGE_INTEGER* presentTargetTime, _Inout_ DrawingSurfaceSizeF* desiredRenderTargetSize);
    HRESULT STDMETHODCALLTYPE Draw(_In_ ID3D11Device1* device, _In_ ID3D11DeviceContext1* context, _In_ ID3D11RenderTargetView* renderTargetView);

private:
    StarterKitPhoneComponent::Direct3DBackground^ m_controller;
    Microsoft::WRL::ComPtr<IDrawingSurfaceRuntimeHostNative> m_host;
};