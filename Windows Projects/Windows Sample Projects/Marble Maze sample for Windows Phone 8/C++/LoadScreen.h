//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DirectXSample.h"
#include <SpriteBatch.h>

ref class LoadScreen
{
internal:
    void Initialize(
        _In_ ID3D11Device*         d3dDevice,
        _In_ ID3D11DeviceContext*  d3dContext,
        Windows::Foundation::Rect  m_windowBounds
        );

    void ResetDirectXResources();

    void UpdateForWindowSizeChange(Windows::Foundation::Rect windowBounds);
    void SetDpi(float dpi);

    void Render();

private:
    Microsoft::WRL::ComPtr<ID3D11Device>             m_d3dDevice;
    Microsoft::WRL::ComPtr<ID3D11DeviceContext>      m_d3dContext;

    std::unique_ptr<DirectX::SpriteBatch>            m_spriteBatch;
    Microsoft::WRL::ComPtr<ID3D11Texture2D>          m_texture;
    Microsoft::WRL::ComPtr<ID3D11ShaderResourceView> m_textureView;

    Windows::Foundation::Size m_imageSize;
    Windows::Foundation::Size m_offset;
    Windows::Foundation::Size m_totalSize;
    Windows::Foundation::Rect m_windowBounds;
    float m_dpi;
};