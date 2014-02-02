// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "VSD3DStarter.h"
#include "Direct3DBase.h"

#ifdef _PHONE
ref class GameBase abstract : public Direct3DPhoneBase
#else
ref class GameBase abstract : public Direct3DWindowsBase
#endif
{
internal:
    GameBase();

public:
    virtual void CreateDeviceResources() override;
    virtual void CreateWindowSizeDependentResources() override;
#ifdef _PHONE
    virtual void UpdateForWindowSizeChange(float width, float height) override;
#else
    virtual void UpdateForWindowSizeChange() override;
#endif
    virtual void Initialize() = 0;
    virtual void Update(float timeTotal, float timeDelta) = 0;
    virtual void Render() override;

protected private:
    bool LineSphereHitTest(VSD3DStarter::Mesh* mesh, const DirectX::XMFLOAT3* p0, const DirectX::XMFLOAT3* dir, float& outT);
    bool LineHitTest(VSD3DStarter::Mesh* mesh, const DirectX::XMFLOAT3* p0, const DirectX::XMFLOAT3* dir, const DirectX::XMFLOAT4X4* objectWorldTransform, float* outT);

    VSD3DStarter::Graphics m_graphics;
    VSD3DStarter::LightConstants m_lightConstants;
    VSD3DStarter::MiscConstants m_miscConstants;

    // Resources for Multi-sample Antialiasing (MSAA)
    Microsoft::WRL::ComPtr<ID3D11Texture2D> m_backBuffer;
    Microsoft::WRL::ComPtr<ID3D11Texture2D> m_backBufferMsaa;
    int m_msaaSampleCount;
    int m_msaaQuality;
};

