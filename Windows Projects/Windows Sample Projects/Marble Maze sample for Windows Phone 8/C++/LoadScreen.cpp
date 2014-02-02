//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "LoadScreen.h"
#include "BasicLoader.h"

using namespace DirectX;
using namespace Windows::UI::Core;
using namespace Windows::Foundation;
using namespace Microsoft::WRL;
using namespace Windows::Graphics::Display;

void LoadScreen::Initialize(
    _In_ ID3D11Device*         d3dDevice,
    _In_ ID3D11DeviceContext*  d3dContext,
    Windows::Foundation::Rect  windowBounds
    )
{
    m_d3dDevice = d3dDevice;
    m_d3dContext = d3dContext;
    m_imageSize = Size(0.0f, 0.0f);
    m_offset = Size(0.0f, 0.0f);
    m_totalSize = Size(0.0f, 0.0f);
    m_windowBounds = windowBounds;
    m_dpi = 96.0f;
    
    m_spriteBatch = std::unique_ptr<SpriteBatch>(new SpriteBatch(d3dContext));

    ResetDirectXResources();
}

void LoadScreen::ResetDirectXResources()
{
    BasicLoader^ loader = ref new BasicLoader(m_d3dDevice.Get());

    loader->LoadTexture(
           L"Media\\Textures\\loadscreen.dds",
           &m_texture,
           &m_textureView
           );

    D3D11_TEXTURE2D_DESC textureDesc = {0};
    m_texture->GetDesc(&textureDesc);
    m_imageSize.Width  = static_cast<float>(textureDesc.Width);
    m_imageSize.Height = static_cast<float>(textureDesc.Height);

    UpdateForWindowSizeChange(m_windowBounds);
}

void LoadScreen::UpdateForWindowSizeChange(Windows::Foundation::Rect windowBounds)
{
    m_windowBounds = windowBounds;

    m_offset.Width = (windowBounds.Width - m_imageSize.Width) / 2.0f;
    m_offset.Height = (windowBounds.Height - m_imageSize.Height) / 2.0f;

    m_totalSize.Width = m_offset.Width + m_imageSize.Width;
    m_totalSize.Height = m_offset.Height + m_imageSize.Height;
}

void LoadScreen::SetDpi(float dpi)
{
    m_dpi = dpi;
}

void LoadScreen::Render()
{
    // Save previous state.
    ComPtr<ID3D11DepthStencilState> oldStencilState;
    UINT oldStencilRef = 0;
    m_d3dContext->OMGetDepthStencilState(&oldStencilState, &oldStencilRef);
    m_spriteBatch->Begin();

    const float scale = m_dpi / 96.0f;

    RECT destinationRectangle = {
        static_cast<LONG>(m_offset.Width * scale),
        static_cast<LONG>(m_offset.Height * scale),
        static_cast<LONG>((m_imageSize.Width + m_offset.Width) * scale),
        static_cast<LONG>((m_imageSize.Height + m_offset.Height) * scale)
    };

    m_spriteBatch->Draw(m_textureView.Get(), destinationRectangle);

    // Restore state.
    m_spriteBatch->End();
    m_d3dContext->OMSetDepthStencilState(oldStencilState.Get(), oldStencilRef);
}