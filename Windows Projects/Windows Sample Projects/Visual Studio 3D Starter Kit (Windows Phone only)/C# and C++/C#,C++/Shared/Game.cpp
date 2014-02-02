// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved


#include "pch.h"
#include "Game.h"
#include <DirectXMath.h>
#include <DirectXColors.h>
#include <algorithm>

using namespace DirectX;
using namespace Microsoft::WRL;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::Core;
using namespace VSD3DStarter;

Game::Game()
{
}

Game::~Game()
{
    for (Mesh* m : m_meshModels)
    {
        if (m != nullptr)
        {
            AnimationState* animState = (AnimationState*)m->Tag;
            if (animState != nullptr)
            {
                m->Tag = nullptr;
                delete animState;
            }
        }
        delete m;
    }
    m_meshModels.clear();
}

void Game::CreateWindowSizeDependentResources()
{
    GameBase::CreateWindowSizeDependentResources();

    float aspectRatio = m_windowBounds.Width / m_windowBounds.Height;

    //
    // setup camera for our scene
    //
    m_graphics.GetCamera().SetViewport((UINT)m_windowBounds.Width, (UINT)m_windowBounds.Height);
    m_graphics.GetCamera().SetPosition(XMFLOAT3(0.0f, 6.0f, -18.0f));
    m_graphics.GetCamera().SetLookAt(XMFLOAT3(0.0f, 0.0f, 0.0f));

    float fovAngleY = 70.0f * XM_PI / 180.0f;

    if (aspectRatio < 1.0f)
    {
        ///
        /// portrait or snap view
        ///
        m_graphics.GetCamera().SetUpVector(XMFLOAT3(1.0f, 0.0f, 0.0f));
        fovAngleY = 120.0f * XM_PI / 180.0f;

    }
    else
    {
        ///
        /// landscape view
        ///
        m_graphics.GetCamera().SetUpVector(XMFLOAT3(0.0f, 1.0f, 0.0f));
    }

    m_graphics.GetCamera().SetProjection(fovAngleY, aspectRatio, 1.0f, 1000.0f);

    //
    // setup lighting for our scene
    //
    XMFLOAT3 pos = XMFLOAT3(5.0f, 5.0f, -2.5f);
    XMVECTOR vPos = XMLoadFloat3(&pos);

    XMFLOAT3 dir;
    XMStoreFloat3(&dir, XMVector3Normalize(vPos));

    m_lightConstants.ActiveLights = 1;
    m_lightConstants.Ambient =  XMFLOAT4(0.3f, 0.3f, 0.3f, 1.0f);
    m_lightConstants.IsPointLight[0] = false;
    m_lightConstants.LightColor[0] = XMFLOAT4(0.8f, 0.8f, 0.8f, 1.0f); 
    m_lightConstants.LightDirection[0].x = dir.x;
    m_lightConstants.LightDirection[0].y = dir.y;
    m_lightConstants.LightDirection[0].z = dir.z;
    m_lightConstants.LightDirection[0].w = 0;
    m_lightConstants.LightSpecularIntensity[0].x = 2;

    m_graphics.UpdateLightConstants(m_lightConstants);
}

void Game::Initialize()
{
    Mesh::LoadFromFile(
        m_graphics, 
        L"gamelevel.cmo", 
        L"", 
        L"Folder", 
        m_meshModels
        );

    //
    // load the teapot from a separate file 
    // and add it to the vector of meshes
    //
    std::vector<Mesh*> tempModels;

    Mesh::LoadFromFile(
        m_graphics, 
        L"teapot.cmo", 
        L"", 
        L"Folder", 
        tempModels
        );

    m_meshModels.push_back(tempModels[0]);

    //
    // create teapot transform
    //
    m_teapotTransform = XMMatrixScaling(0.044f, 0.044f, 0.044f) * XMMatrixTranslation(0.0f, -1.6f, 0.0f);

    for (Mesh* m : m_meshModels)
    {
        if (m->BoneInfoCollection().empty() == false)
        {
            AnimationState* animState = new AnimationState();
            animState->m_boneWorldTransforms.resize(m->BoneInfoCollection().size());

            m->Tag = animState;
        }
    }

    m_skinnedMeshRenderer.Initialize(m_d3dDevice.Get(), m_d3dContext.Get());

    // each mesh object has it's own "time" used to control glow effect
    m_time.clear();
    for (size_t i = 0; i < m_meshModels.size(); i++)
    {
        m_time.push_back(0.0f);
    }
}

void Game::Update(float timeTotal, float timeDelta)
{
    // update animated models
    m_skinnedMeshRenderer.UpdateAnimation(timeDelta, m_meshModels);

    // rotate object
    m_rotation = timeTotal * 0.5f;

    // update time
    for (float &time : m_time)
    {
        time = std::max<float>(0.0f, time - timeDelta);
    }
}

void Game::Render()
{
    GameBase::Render();

    // clear
    m_d3dContext->OMSetRenderTargets(
        1,
        m_d3dRenderTargetView.GetAddressOf(),
        m_d3dDepthStencilView.Get()
        );

    m_d3dContext->ClearRenderTargetView(
        m_d3dRenderTargetView.Get(),
        DirectX::Colors::DarkSlateGray
        );

    m_d3dContext->ClearDepthStencilView(
        m_d3dDepthStencilView.Get(),
        D3D11_CLEAR_DEPTH,
        1.0f,
        0
        );

    //
    // draw our scene models
    //
    XMMATRIX rotation = XMMatrixRotationY(m_rotation);
    for (UINT i = 0; i < m_meshModels.size(); i++)
    {
        XMMATRIX modelTransform = rotation;

        String^ meshName = ref new String(m_meshModels[i]->Name());
        if (String::CompareOrdinal(meshName, L"Teapot_Node") == 0)
            modelTransform = m_teapotTransform * modelTransform;

        //
        // setup misc constants for our scene
        //
        m_miscConstants.Time = m_time[i];
        m_graphics.UpdateMiscConstants(m_miscConstants);

        //
        // draw
        //
        if (m_meshModels[i]->Tag != nullptr)
        {
            //
            // mesh has animation
            //
            m_skinnedMeshRenderer.RenderSkinnedMesh(m_meshModels[i], m_graphics, modelTransform);
        }
        else
        {
            //
            // render as usual
            //
            m_meshModels[i]->Render(m_graphics, modelTransform);
        }
    }

    //
    // only enable MSAA if the device has enough power
    //
    if (m_d3dFeatureLevel >= D3D_FEATURE_LEVEL_10_0)
    {
        //
        // resolve multi-sample textures into single-sample textures
        //
        UINT resourceIndex = D3D11CalcSubresource(0, 0, 1);
        m_d3dContext->ResolveSubresource(m_backBuffer.Get(), resourceIndex, m_backBufferMsaa.Get(), resourceIndex, DXGI_FORMAT_B8G8R8A8_UNORM);
    }
}

void Game::ToggleHitEffect(String^ object)
{
    for (UINT i = 0; i < m_meshModels.size(); i++)
    {
        Mesh* m = m_meshModels[i];
        String^ meshName = ref new String(m->Name());
        if (String::CompareOrdinal(object, meshName) == 0)
        {
            m_time[i] = 1.0f;
            break;
        }
    }
}

void Game::ChangeMaterialColor(String^ object, float r, float g, float b)
{
    for (Mesh* m : m_meshModels)
    {
        String^ meshName = ref new String(m->Name());
        if (String::CompareOrdinal(object, meshName) == 0)
        {
            m->Materials()[0].Diffuse[0] = r;
            m->Materials()[0].Diffuse[1] = g;
            m->Materials()[0].Diffuse[2] = b;
            break;
        }
    }
}

String^ Game::OnHitObject(int x, int y)
{
    String^ result = nullptr;

    XMFLOAT3 point;
    XMFLOAT3 dir;
    m_graphics.GetCamera().GetWorldLine(x, y, &point, &dir);

    XMFLOAT4X4 world;
    XMMATRIX worldMat = XMMatrixRotationY(m_rotation);
    XMStoreFloat4x4(&world, worldMat);

    XMFLOAT4X4 teapotWorld;
    XMStoreFloat4x4(&teapotWorld, m_teapotTransform * worldMat);

    float closestT = FLT_MAX;
    for (Mesh* m : m_meshModels)
    {
        XMFLOAT4X4 meshTransform = world;

        auto name = ref new String(m->Name());

        if(String::CompareOrdinal(name, L"Teapot_Node") == 0) 
            meshTransform = teapotWorld;

        float t = 0;
        bool hit = this->LineHitTest(m, &point, &dir, &meshTransform, &t);
        if (hit && t < closestT)
        {
            result = name;
        }
    }

    return result;
}