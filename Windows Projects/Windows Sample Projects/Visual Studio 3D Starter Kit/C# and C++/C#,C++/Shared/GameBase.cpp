// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"

#include "DirectXCollision.h"
#include "GameBase.h"

using namespace DirectX;
using namespace Microsoft::WRL;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::Core;

using namespace VSD3DStarter;

GameBase::GameBase()
{
}

void GameBase::CreateDeviceResources()
{
    __super::CreateDeviceResources();

    m_graphics.Initialize(m_d3dDevice.Get(), m_d3dContext.Get(), m_d3dFeatureLevel);

    // so mesh triangles always show, don't cull
    CD3D11_RASTERIZER_DESC d3dRas(D3D11_DEFAULT);
    d3dRas.CullMode = D3D11_CULL_NONE;
    d3dRas.MultisampleEnable = true;
    d3dRas.AntialiasedLineEnable = true;

    ComPtr<ID3D11RasterizerState> p3d3RasState;
    m_d3dDevice->CreateRasterizerState(&d3dRas, &p3d3RasState);
    m_d3dContext->RSSetState(p3d3RasState.Get());

    Initialize();
}

void GameBase::CreateWindowSizeDependentResources()
{
    __super::CreateWindowSizeDependentResources();

    m_graphics.GetCamera().SetOrientationMatrix(m_orientationTransform3D);

    // only enable MSAA if the device has enough power
    if (m_d3dFeatureLevel >= D3D_FEATURE_LEVEL_10_0)
    {
        // Calculate Multisample and Quality capabilities
        m_msaaSampleCount = 4;
        m_msaaQuality = 0;

        while (m_msaaSampleCount > 1)
        {
            uint32 numQualityLevels;

            m_d3dDevice->CheckMultisampleQualityLevels(
                DXGI_FORMAT_B8G8R8A8_UNORM,
                m_msaaSampleCount,
                &numQualityLevels
                );

            // if this MSAA has valid quality levels, we can break out of loop
            if (numQualityLevels > 0)
            {
                m_msaaQuality = numQualityLevels - 1;
                break;
            }

            // drop down the MSAA count and try again
            m_msaaSampleCount--;
        }

        // Get the swap chain back buffer and store it to use while rendering.
        DX::ThrowIfFailed(
            m_swapChain->GetBuffer(0, IID_PPV_ARGS(&m_backBuffer))
            );

        // Create a multi-sampled back buffer.
        CD3D11_TEXTURE2D_DESC textureDesc(DXGI_FORMAT_B8G8R8A8_UNORM, (UINT)m_d3dRenderTargetSize.Width, (UINT)m_d3dRenderTargetSize.Height, 1, 1);
        textureDesc.BindFlags = D3D11_BIND_RENDER_TARGET;
        textureDesc.SampleDesc.Count = m_msaaSampleCount; // 4x MSAA
        textureDesc.SampleDesc.Quality = m_msaaQuality; // Use best quality

        DX::ThrowIfFailed(m_d3dDevice->CreateTexture2D(&textureDesc, NULL, &m_backBufferMsaa));

        DX::ThrowIfFailed(
            m_d3dDevice->CreateRenderTargetView(
            m_backBufferMsaa.Get(),
            nullptr,
            &m_d3dRenderTargetView
            )
            );

        // Create a depth stencil view for use with 3D rendering if needed.
        CD3D11_TEXTURE2D_DESC depthStencilDesc(
            DXGI_FORMAT_D24_UNORM_S8_UINT, 
            static_cast<UINT>(m_d3dRenderTargetSize.Width),
            static_cast<UINT>(m_d3dRenderTargetSize.Height),
            1,
            1,
            D3D11_BIND_DEPTH_STENCIL,
            D3D11_USAGE_DEFAULT,
            0,
            m_msaaSampleCount,
            m_msaaQuality
            );

        ComPtr<ID3D11Texture2D> depthStencil;
        DX::ThrowIfFailed(
            m_d3dDevice->CreateTexture2D(
            &depthStencilDesc,
            nullptr,
            &depthStencil
            )
            );

        CD3D11_DEPTH_STENCIL_VIEW_DESC depthStencilViewDesc(D3D11_DSV_DIMENSION_TEXTURE2DMS);
        DX::ThrowIfFailed(
            m_d3dDevice->CreateDepthStencilView(
            depthStencil.Get(),
            &depthStencilViewDesc,
            &m_d3dDepthStencilView
            )
            );
    }
    else
    {
        m_msaaSampleCount = 1;
        m_msaaQuality = 0;
    }
}

#ifdef _PHONE
void GameBase::UpdateForWindowSizeChange(float width, float height)
{
    if (width  != m_windowBounds.Width ||
        height != m_windowBounds.Height)
    {
        m_backBuffer = nullptr;
        m_backBufferMsaa = nullptr;
    }

    __super::UpdateForWindowSizeChange(width, height);
}
#else
void GameBase::UpdateForWindowSizeChange()
{
    auto resizeManager = CoreWindowResizeManager::GetForCurrentView();

    if (m_window->Bounds.Width  != m_windowBounds.Width ||
        m_window->Bounds.Height != m_windowBounds.Height ||
        m_orientation != DisplayProperties::CurrentOrientation)
    {
        m_backBuffer = nullptr;
        m_backBufferMsaa = nullptr;
    }

    __super::UpdateForWindowSizeChange();
    resizeManager->NotifyLayoutCompleted();
}
#endif

void GameBase::Render() 
{
    //
    // setup misc constants for our scene
    //
    m_miscConstants.ViewportHeight = m_windowBounds.Height;
    m_miscConstants.ViewportWidth = m_windowBounds.Width;
    m_graphics.UpdateMiscConstants(m_miscConstants);
}

#pragma region Hit Testing

bool GameBase::LineSphereHitTest(Mesh* mesh, const XMFLOAT3* p0, const XMFLOAT3* dir, float& outT)
{
    XMFLOAT3 center(mesh->Extents().CenterX, mesh->Extents().CenterY, mesh->Extents().CenterZ);
    BoundingSphere* sphere = new BoundingSphere(center, mesh->Extents().Radius);
    return sphere->Intersects(XMLoadFloat3(p0), XMLoadFloat3(dir), outT);
}

bool GameBase::LineHitTest(Mesh* mesh, const XMFLOAT3* p0, const XMFLOAT3* dir, const XMFLOAT4X4* objectWorldTransform, float* outT)
{
    XMMATRIX objInvMatrix = XMLoadFloat4x4(objectWorldTransform);

    XMVECTOR det;
    objInvMatrix = XMMatrixInverse(&det, objInvMatrix);

    XMVECTOR p0Vec = XMVector3TransformCoord(XMLoadFloat3(p0), objInvMatrix);
    XMVECTOR dirVec = XMVector3Normalize(XMVector3TransformNormal(XMLoadFloat3(dir), objInvMatrix));

    XMFLOAT3 p0InObj;
    XMFLOAT3 dirInObj;

    XMStoreFloat3(&p0InObj, p0Vec);
    XMStoreFloat3(&dirInObj, dirVec);

    bool hit = false;
    float closestT = FLT_MAX;
    float t = 0;
    if (this->LineSphereHitTest(mesh, &p0InObj, &dirInObj, t))
    {
        for (Mesh::Triangle& triangle : mesh->Triangles())
        {
            XMVECTOR triangleV0 = XMLoadFloat3(&triangle.points[0]);
            XMVECTOR triangleV1 = XMLoadFloat3(&triangle.points[1]);
            XMVECTOR triangleV2 = XMLoadFloat3(&triangle.points[2]);

            if (TriangleTests::Intersects(p0Vec, dirVec, triangleV0, triangleV1, triangleV2, t))
            {
                if (t >= 0 && t < closestT)
                {
                    closestT = t;
                    hit = true;
                }
            }
        }
    }

    *outT = closestT;
    return hit;
}

#pragma endregion
