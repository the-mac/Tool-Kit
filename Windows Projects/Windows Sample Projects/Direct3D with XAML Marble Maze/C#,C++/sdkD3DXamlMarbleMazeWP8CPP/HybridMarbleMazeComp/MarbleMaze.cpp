//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"

using namespace DirectX;

#include "MarbleMaze.h"
#include "BasicLoader.h"
#include "BasicTimer.h"
#include "DDSTextureLoader.h"
#include "Profiler.h"
#include <ppltasks.h>

using namespace Microsoft::WRL;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;
using namespace Windows::System;
using namespace Windows::Storage;
using namespace Concurrency;
using namespace Windows::Phone::UI::Input;
using namespace Windows::Graphics::Display;

const float UserInactivityDuration = 60.0f; // in seconds
const float UserInactivityThreshold = 0.01f;
PCWSTR SaveStateFile = L"savedstate.txt";


MarbleMaze::MarbleMaze() :
    m_gameState(GameState::Initial),
    m_windowActive(false),
    m_loadedSave(false),
    m_savingState(false),
    m_deferredResourcesReady(false),
    m_isDSBG(true)
{
    m_lightStrength = 0.0f;
    m_targetLightStrength = 0.0f;
    m_resetCamera = true;
    m_resetMarbleRotation = true;
    m_previousAcceleration = XMFLOAT3(0, 0, 0);
    m_previousAccelerationTime = 0;

    // Checkpoints (from start to goal).
    m_checkpoints.push_back(XMFLOAT3( 45.7f,  -43.6f, -45.0f)); // Start
    m_checkpoints.push_back(XMFLOAT3(120.7f,  -35.0f, -45.0f)); // Checkpoint 1
    m_checkpoints.push_back(XMFLOAT3(297.6f, -194.6f, -45.0f)); // Checkpoint 2
    m_checkpoints.push_back(XMFLOAT3(770.1f, -391.5f, -45.0f)); // Checkpoint 3
    m_checkpoints.push_back(XMFLOAT3(552.0f, -148.6f, -45.0f)); // Checkpoint 4
    m_checkpoints.push_back(XMFLOAT3(846.8f, -377.0f, -45.0f)); // Goal
}

MarbleMaze::~MarbleMaze()
{
    UserInterface::GetInstance().Uninitialize();
}

void MarbleMaze::Initialize()
{
    // Must happen before initializing base.
    m_audio.Initialize();

    // Returns accelerometer ref if there is one; nullptr otherwise.
    m_accelerometer = Windows::Devices::Sensors::Accelerometer::GetDefault();

    DirectXBase::Initialize();

    LoadState();

    m_dpi = DisplayProperties::LogicalDpi;
    m_featureLevel = D3D_FEATURE_LEVEL_9_3;

    m_audio.CreateResources();
}

void MarbleMaze::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();
    m_camera = ref new Camera();
    m_loadScreen = ref new LoadScreen();

    m_loadScreen->Initialize(
        m_d3dDevice.Get(),
        m_d3dContext.Get(),
        m_windowBounds
        );

    UserInterface::GetInstance().Initialize(
        m_d3dDevice.Get(),
        m_d3dContext.Get()
        );

    m_loadScreen->SetDpi(m_dpi);
    UserInterface::GetInstance().SetDpi(m_dpi);

    LoadDeferredResources(false, true);
}

void MarbleMaze::SetDpi(float dpi)
{
    if (dpi != m_dpi)
    {
        m_loadScreen->SetDpi(dpi);
        UserInterface::GetInstance().SetDpi(dpi);
        m_dpi = dpi;
    }
}

inline Rect ConvertRect(Windows::Foundation::Rect source)
{
    // ignore the source.X and source.Y  These are the location on the screen
    // yet we don't want to use them because all coordinates are window relative.
    return Rect(0.0f, 0.0f, source.Width, source.Height);
}

inline void InflateRect(Rect& rect, float x, float y)
{
    rect.X -= x;
    rect.Width += 2.0f * x;
    rect.Y -= y;
    rect.Height += 2.0f * y;
}

void MarbleMaze::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();

    // update the 3D projection matrix
    m_camera->SetProjectionParameters(
        40.0f,                                                  // use a 70-degree vertical field of view
        m_renderTargetSize.Width /  m_renderTargetSize.Height,  // specify the aspect ratio of the window
        50.0f,                                                  // specify the nearest Z-distance at which to draw vertices
        500.0f                                                  // specify the farthest Z-distance at which to draw vertice
        );

    // MarbleMaze specific
    float4x4 projection;
    m_camera->GetProjectionMatrix(&projection);
    m_mazeConstantBufferData.projection = projection;
    m_marbleConstantBufferData.projection = projection;

    // user interface
    PCWSTR largeFont = L"Media\\Fonts\\SegoeBlack40.spritefont";
    PCWSTR smallFont = L"Media\\Fonts\\SegoeBold30.spritefont";

    // Load higher resolution fonts for larger devices.
    float fontScale = 1.0f;
    if (m_dpi > 96.0f)
    {
        largeFont = L"Media\\Fonts\\SegoeBlack60.spritefont";
        smallFont = L"Media\\Fonts\\SegoeBold45.spritefont";
        fontScale = 2.0f / 3.0f; // The higher-res fonts are 50% larger.
    }

    const float padding = 32.0f;
    const float cornerTriggerRegion = 75.0f;
    Rect clientRect = ConvertRect(m_windowBounds);
    Rect originalClientRect = clientRect;
    InflateRect(clientRect, -padding, -padding);

    Rect topHalfRect = clientRect;
    topHalfRect.Height = (clientRect.Height + padding) * 0.5f;
    Rect bottomHalfRect = clientRect;
    bottomHalfRect.Y = topHalfRect.Bottom + padding;
    bottomHalfRect.Height = topHalfRect.Height;

    m_startGameButton.Initialize();
    m_startGameButton.SetAlignment(AlignCenter, AlignFar);
    m_startGameButton.SetContainer(topHalfRect);
    m_startGameButton.SetText(L"Start Game");
    m_startGameButton.SetTextColor(Colors::White);

    m_startGameButton.SetPadding(Size(32.0f, 16.0f));
    m_startGameButton.SetFont(largeFont);
    m_startGameButton.SetFontScale(fontScale);

    UserInterface::GetInstance().RegisterElement(&m_startGameButton);

    m_highScoreButton.Initialize();
    m_highScoreButton.SetAlignment(AlignCenter, AlignNear);
    m_highScoreButton.SetContainer(bottomHalfRect);
    m_highScoreButton.SetText(L"High Scores");
    m_highScoreButton.SetTextColor(Colors::White);

    m_highScoreButton.SetPadding(Size(32.0f, 16.0f));
    m_highScoreButton.SetFont(largeFont);
    m_highScoreButton.SetFontScale(fontScale);

    UserInterface::GetInstance().RegisterElement(&m_highScoreButton);

    m_highScoreTable.Initialize();
    m_highScoreTable.SetAlignment(AlignCenter, AlignCenter);
    m_highScoreTable.SetContainer(clientRect);
    m_highScoreTable.SetTextColor(Colors::White);
    m_highScoreTable.SetTextAlignment(AlignCenter);

    m_highScoreTable.SetFont(smallFont);
    m_highScoreTable.SetFontScale(fontScale);
    UserInterface::GetInstance().RegisterElement(&m_highScoreTable);

    m_preGameCountdownTimer.Initialize();
    m_preGameCountdownTimer.SetAlignment(AlignCenter, AlignCenter);
    m_preGameCountdownTimer.SetContainer(clientRect);
    m_preGameCountdownTimer.SetTextColor(Colors::White);
    
    m_preGameCountdownTimer.SetFont(largeFont);
    m_preGameCountdownTimer.SetFontScale(fontScale);
    UserInterface::GetInstance().RegisterElement(&m_preGameCountdownTimer);

    m_inGameStopwatchTimer.Initialize();
    m_inGameStopwatchTimer.SetAlignment(AlignNear, AlignFar);
    m_inGameStopwatchTimer.SetContainer(clientRect);
    m_inGameStopwatchTimer.SetTextColor(Colors::White);

    m_inGameStopwatchTimer.SetFont(smallFont);
    m_inGameStopwatchTimer.SetFontScale(fontScale);
    UserInterface::GetInstance().RegisterElement(&m_inGameStopwatchTimer);

    m_checkpointText.Initialize();
    m_checkpointText.SetAlignment(AlignCenter, AlignCenter);
    m_checkpointText.SetContainer(clientRect);
    m_checkpointText.SetText(L"Checkpoint!");
    m_checkpointText.SetTextColor(Colors::White);

    m_checkpointText.SetFont(largeFont);
    m_checkpointText.SetFontScale(fontScale);
    UserInterface::GetInstance().RegisterElement(&m_checkpointText);

    m_pausedText.Initialize();
    m_pausedText.SetAlignment(AlignCenter, AlignCenter);
    m_pausedText.SetContainer(clientRect);
    m_pausedText.SetText(L"Paused");
    m_pausedText.SetTextColor(Colors::White);

    m_pausedText.SetFont(largeFont);
    m_pausedText.SetFontScale(fontScale);
    UserInterface::GetInstance().RegisterElement(&m_pausedText);

    m_resultsText.Initialize();
    m_resultsText.SetAlignment(AlignCenter, AlignCenter);
    m_resultsText.SetContainer(clientRect);
    m_resultsText.SetTextColor(Colors::White);
    m_resultsText.SetTextAlignment(AlignCenter);

    m_resultsText.SetFont(smallFont);
    m_resultsText.SetFontScale(fontScale);
    UserInterface::GetInstance().RegisterElement(&m_resultsText);

    m_profilerGraph.Initialize();
    m_profilerGraph.SetContainer(Rect(0, 16.0f, m_windowBounds.Width, 100.0f));
    m_profilerGraph.SetVisible(true);
    UserInterface::GetInstance().RegisterElement(&m_profilerGraph);

    m_resolutionText.Initialize();
    m_resolutionText.SetAlignment(AlignCenter, AlignCenter);
    m_resolutionText.SetContainer(clientRect);
    m_resolutionText.SetTextColor(Colors::White);

    m_resolutionText.SetFont(largeFont);
    m_resolutionText.SetFontScale(fontScale);
    UserInterface::GetInstance().RegisterElement(&m_resolutionText);

    m_drawingSurfaceTypeText.Initialize();
    m_drawingSurfaceTypeText.SetAlignment(AlignCenter, AlignCenter);
    m_drawingSurfaceTypeText.SetContainer(clientRect);
    m_drawingSurfaceTypeText.SetTextColor(Colors::White);

    m_drawingSurfaceTypeText.SetFont(largeFont);
    m_drawingSurfaceTypeText.SetFontScale(fontScale);
    UserInterface::GetInstance().RegisterElement(&m_drawingSurfaceTypeText);

    m_topRightToggleCorner.Initialize();
    m_topRightToggleCorner.SetAlignment(AlignFar, AlignNear);
    m_topRightToggleCorner.SetContainer(originalClientRect);
    m_topRightToggleCorner.SetSize(Size(cornerTriggerRegion, cornerTriggerRegion));
    m_topRightToggleCorner.SetVisible(true);
    UserInterface::GetInstance().RegisterElement(&m_topRightToggleCorner);

    m_topLeftToggleCorner.Initialize();
    m_topLeftToggleCorner.SetAlignment(AlignNear, AlignNear);
    m_topLeftToggleCorner.SetContainer(originalClientRect);
    m_topLeftToggleCorner.SetSize(Size(cornerTriggerRegion, cornerTriggerRegion));
    m_topLeftToggleCorner.SetVisible(true);
    UserInterface::GetInstance().RegisterElement(&m_topLeftToggleCorner);

    if ((!m_deferredResourcesReady) && m_loadScreen)
    {
        m_loadScreen->UpdateForWindowSizeChange(m_windowBounds);
    }
}

void MarbleMaze::LoadDeferredResources(bool delay, bool deviceOnly)
{
    BasicTimer^ loadingTimer = ref new BasicTimer;
    loadingTimer->Reset();

    BasicLoader^ loader = ref new BasicLoader(m_d3dDevice.Get());

    D3D11_INPUT_ELEMENT_DESC layoutDesc[] =
        {
            { "POSITION", 0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 0,  D3D11_INPUT_PER_VERTEX_DATA, 0 },
            { "NORMAL",   0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 12, D3D11_INPUT_PER_VERTEX_DATA, 0 },
            { "TEXCOORD", 0, DXGI_FORMAT_R32G32_FLOAT,    0, 24, D3D11_INPUT_PER_VERTEX_DATA, 0 },
            { "TANGENT",  0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 32, D3D11_INPUT_PER_VERTEX_DATA, 0 },
        };
    m_vertexStride = 44; // must set this to match the size of layoutDesc above

    loader->LoadShader(
        L"BasicVertexShader.cso",
        layoutDesc,
        ARRAYSIZE(layoutDesc),
        &m_vertexShader,
        &m_inputLayout
        );

    // create the constant buffer for updating model and camera data.
    D3D11_BUFFER_DESC constantBufferDesc = {0};
    constantBufferDesc.ByteWidth           = ((sizeof(ConstantBuffer) + 15) / 16) * 16; // multiple of 16 bytes
    constantBufferDesc.Usage               = D3D11_USAGE_DEFAULT;
    constantBufferDesc.BindFlags           = D3D11_BIND_CONSTANT_BUFFER;
    constantBufferDesc.CPUAccessFlags      = 0;
    constantBufferDesc.MiscFlags           = 0;
    // this will not be used as a structured buffer, so this parameter is ignored
    constantBufferDesc.StructureByteStride = 0;

    DX::ThrowIfFailed(
        m_d3dDevice->CreateBuffer(
            &constantBufferDesc,
            nullptr,             // leave the buffer uninitialized
            &m_constantBuffer
            )
        );

    loader->LoadShader(
        L"BasicPixelShader.cso",
        &m_pixelShader
        );

    // create the blend state.
    D3D11_BLEND_DESC blendDesc = {0};
    blendDesc.RenderTarget[0].BlendEnable = TRUE;
    blendDesc.RenderTarget[0].SrcBlend = D3D11_BLEND_SRC_ALPHA;
    blendDesc.RenderTarget[0].DestBlend = D3D11_BLEND_INV_SRC_ALPHA;
    blendDesc.RenderTarget[0].BlendOp = D3D11_BLEND_OP_ADD;
    blendDesc.RenderTarget[0].SrcBlendAlpha = D3D11_BLEND_ONE;
    blendDesc.RenderTarget[0].DestBlendAlpha = D3D11_BLEND_ZERO;
    blendDesc.RenderTarget[0].BlendOpAlpha = D3D11_BLEND_OP_ADD;
    blendDesc.RenderTarget[0].RenderTargetWriteMask = D3D11_COLOR_WRITE_ENABLE_ALL;

    DX::ThrowIfFailed(
        m_d3dDevice->CreateBlendState(
            &blendDesc,
            &m_blendState
            )
        );

    // create the sampler
    D3D11_SAMPLER_DESC samplerDesc;
    samplerDesc.Filter = D3D11_FILTER_ANISOTROPIC;
    samplerDesc.AddressU = D3D11_TEXTURE_ADDRESS_WRAP;
    samplerDesc.AddressV = D3D11_TEXTURE_ADDRESS_WRAP;
    samplerDesc.AddressW = D3D11_TEXTURE_ADDRESS_WRAP;
    samplerDesc.MipLODBias = 0.0f;
    samplerDesc.MaxAnisotropy = m_featureLevel > D3D_FEATURE_LEVEL_9_1 ? 4 : 2;
    samplerDesc.ComparisonFunc = D3D11_COMPARISON_NEVER;
    samplerDesc.BorderColor[0] = 0.0f;
    samplerDesc.BorderColor[1] = 0.0f;
    samplerDesc.BorderColor[2] = 0.0f;
    samplerDesc.BorderColor[3] = 0.0f;
    // allow use of all mip levels
    samplerDesc.MinLOD = 0;
    samplerDesc.MaxLOD = D3D11_FLOAT32_MAX;

    DX::ThrowIfFailed(
        m_d3dDevice->CreateSamplerState(
            &samplerDesc,
            &m_sampler
            )
        );

    // Load the meshes.
    DX::ThrowIfFailed(
        m_mazeMesh.Create(
            m_d3dDevice.Get(),
            L"Media\\Models\\maze1.sdkmesh",
            false
            )
        );

    DX::ThrowIfFailed(
        m_marbleMesh.Create(
            m_d3dDevice.Get(),
            L"Media\\Models\\marble2.sdkmesh",
            false
            )
        );

    // Extract mesh geometry for physics system.
    DX::ThrowIfFailed(
        ExtractTrianglesFromMesh(
            m_mazeMesh,
            "Mesh_walls",
            m_collision.m_wallTriList
            )
        );

    DX::ThrowIfFailed(
        ExtractTrianglesFromMesh(
            m_mazeMesh,
            "Mesh_Floor",
            m_collision.m_groundTriList
            )
        );

    DX::ThrowIfFailed(
        ExtractTrianglesFromMesh(
            m_mazeMesh,
            "Mesh_floorSides",
            m_collision.m_floorTriList
            )
        );

    m_physics.SetCollision(&m_collision);
    float radius = m_marbleMesh.GetMeshBoundingBoxExtents(0).x / 2;
    m_physics.SetRadius(radius);

    if (!deviceOnly)
    {
        // When handling device lost, we only need to recreate the graphics-device related
        // resources. All other delayed resources that only need to be created on app
        // startup go here.
    }

    if (delay)
    {
        while (loadingTimer->Total < 3.5)
        {
            // MarbleMaze doesn't take long to load resources,
            // so we're simulating a longer load time to demonstrate
            // a more real world example
            loadingTimer->Update();
        }
    }

    m_deferredResourcesReady = true;
}

void MarbleMaze::Render()
{
    // Bind the render targets.
    m_d3dContext->OMSetRenderTargets(
        1,
        m_d3dRenderTargetView.GetAddressOf(),
        m_d3dDepthStencilView.Get()
        );

    // Clear the render target and depth stencil to default values.
    const float clearColor[4] = { 0.0f, 0.0f, 0.0f, 1.0f };

    m_d3dContext->ClearRenderTargetView(
        m_d3dRenderTargetView.Get(),
        clearColor
        );

    m_d3dContext->ClearDepthStencilView(
        m_d3dDepthStencilView.Get(),
        D3D11_CLEAR_DEPTH,
        1.0f,
        0
        );

    if (!m_deferredResourcesReady)
    {
        // Only render the loading screen for now.

        m_loadScreen->Render();
        return;
    }

    m_d3dContext->IASetInputLayout(m_inputLayout.Get());

    FLOAT blendFactors[4] = { 0, };
    m_d3dContext->OMSetBlendState(m_blendState.Get(), blendFactors, 0xffffffff);

    // Set the vertex shader stage state.
    m_d3dContext->VSSetShader(
        m_vertexShader.Get(),   // use this vertex shader
        nullptr,                // don't use shader linkage
        0                       // don't use shader linkage
        );

    // Set the pixel shader stage state.
    m_d3dContext->PSSetShader(
        m_pixelShader.Get(),    // use this pixel shader
        nullptr,                // don't use shader linkage
        0                       // don't use shader linkage
        );

    m_d3dContext->PSSetSamplers(
        0,                       // starting at the first sampler slot
        1,                       // set one sampler binding
        m_sampler.GetAddressOf() // to use this sampler
        );

#pragma region Rendering Maze

    // Update the constant buffer with the new data.
    m_d3dContext->UpdateSubresource(
        m_constantBuffer.Get(),
        0,
        nullptr,
        &m_mazeConstantBufferData,
        0,
        0
        );

    m_d3dContext->VSSetConstantBuffers(
        0,                // starting at the first constant buffer slot
        1,                // set one constant buffer binding
        m_constantBuffer.GetAddressOf() // to use this buffer
        );

    m_d3dContext->PSSetConstantBuffers(
        0,                // starting at the first constant buffer slot
        1,                // set one constant buffer binding
        m_constantBuffer.GetAddressOf() // to use this buffer
        );

    m_mazeMesh.Render(m_d3dContext.Get(), 0, INVALID_SAMPLER_SLOT, INVALID_SAMPLER_SLOT);

#pragma endregion

#pragma region Rendering Marble

    // update the constant buffer with the new data
    m_d3dContext->UpdateSubresource(
        m_constantBuffer.Get(),
        0,
        nullptr,
        &m_marbleConstantBufferData,
        0,
        0
        );

    m_d3dContext->VSSetConstantBuffers(
        0,                // starting at the first constant buffer slot
        1,                // set one constant buffer binding
        m_constantBuffer.GetAddressOf() // to use this buffer
        );

    m_d3dContext->PSSetConstantBuffers(
        0,                // starting at the first constant buffer slot
        1,                // set one constant buffer binding
        m_constantBuffer.GetAddressOf() // to use this buffer
        );

    m_marbleMesh.Render(m_d3dContext.Get(), 0, INVALID_SAMPLER_SLOT, INVALID_SAMPLER_SLOT);

#pragma endregion

    // Process audio.
    m_audio.Render();

    // Draw the user interface and the overlay.
    UserInterface::GetInstance().Render();
}

void MarbleMaze::SetGameState(GameState nextState)
{
    // previous state cleanup
    switch (m_gameState)
    {
    case GameState::MainMenu:
        m_startGameButton.SetVisible(false);
        m_highScoreButton.SetVisible(false);
        break;

    case GameState::HighScoreDisplay:
        m_highScoreTable.SetVisible(false);
        break;

    case GameState::PreGameCountdown:
        m_inGameStopwatchTimer.Reset();
        m_inGameStopwatchTimer.SetVisible(true);
        break;

    case GameState::PostGameResults:
        m_resultsText.SetVisible(false);
        break;

    case GameState::InGameActive:
        break;
    }

    // next state setup
    switch (nextState)
    {
    case GameState::MainMenu:
        m_startGameButton.SetVisible(true);
        m_startGameButton.SetSelected(true);
        m_highScoreButton.SetVisible(true);
        m_highScoreButton.SetSelected(false);
        m_pausedText.SetVisible(false);

        m_resetCamera = true;
        m_resetMarbleRotation = true;
        m_physics.SetPosition(XMFLOAT3(305, -210, -43));
        m_targetLightStrength = 0.6f;
        break;

    case GameState::HighScoreDisplay:
        m_highScoreTable.SetVisible(true);
        break;

    case GameState::PreGameCountdown:
        m_inGameStopwatchTimer.SetVisible(false);
        m_preGameCountdownTimer.SetVisible(true);
        m_preGameCountdownTimer.StartCountdown(3);

        ResetCheckpoints();
        m_resetCamera = true;
        m_resetMarbleRotation = true;
        m_physics.SetPosition(m_checkpoints[0]);
        m_physics.SetVelocity(XMFLOAT3(0, 0, 0));
        m_targetLightStrength = 1.0f;
        break;

    case GameState::InGameActive:
        m_pausedText.SetVisible(false);
        m_inGameStopwatchTimer.Start();
        m_targetLightStrength = 1.0f;
        break;

    case GameState::InGamePaused:
        m_pausedText.SetVisible(true);
        m_inGameStopwatchTimer.Stop();
        m_targetLightStrength = 0.6f;
        break;

    case GameState::PostGameResults:
        m_inGameStopwatchTimer.Stop();
        m_inGameStopwatchTimer.SetVisible(false);
        m_resultsText.SetVisible(true);
        {
            WCHAR formattedTime[32];
            m_inGameStopwatchTimer.GetFormattedTime(formattedTime, m_newHighScore.elapsedTime);
            WCHAR buffer[64];
            swprintf_s(
                buffer,
                L"%s\nYour time: %s",
                (m_newHighScore.wasJustAdded ? L"New High Score!" : L"Finished!"),
                formattedTime);
            m_resultsText.SetText(buffer);
            m_resultsText.SetVisible(true);
        }
        m_targetLightStrength = 0.6f;
        break;
    }

    m_gameState = nextState;
}

void MarbleMaze::ResetCheckpoints()
{
    m_currentCheckpoint = 0;
}

CheckpointState MarbleMaze::UpdateCheckpoints()
{
    if (m_currentCheckpoint >= (m_checkpoints.size() - 1))
        return CheckpointState::None;

    const float checkpointRadius = 20.0f;
    float radius = m_physics.GetRadius();
    float horizDistSq = (radius + checkpointRadius) * (radius + checkpointRadius);
    XMVECTOR horizDistSqLimit = XMVectorSet(horizDistSq, horizDistSq, horizDistSq, horizDistSq);
    float vertDistSq = radius * radius;
    XMVECTOR vertDistSqLimit = XMVectorSet(vertDistSq, vertDistSq, vertDistSq, vertDistSq);

    XMVECTOR position = XMLoadFloat3(&m_physics.GetPosition());
    XMVECTOR up = XMVectorSet(0.0f, 0.0f, -1.0f, 0.0f);

    for (size_t i = m_currentCheckpoint + 1; i < m_checkpoints.size(); ++i)
    {
        XMVECTOR checkpointPos = XMLoadFloat3(&m_checkpoints[i]);
        XMVECTOR posCheckSpace = position - checkpointPos;
        XMVECTOR posVertical = up * XMVector3Dot(up, posCheckSpace);
        XMVECTOR posHorizontal = posCheckSpace - posVertical;
        XMVECTOR vHorizDistSq = XMVector3LengthSq(posHorizontal);
        XMVECTOR vVertDistSq = XMVector3LengthSq(posVertical);

        XMVECTOR check = XMVectorAndInt(
            XMVectorLessOrEqual(vHorizDistSq, horizDistSqLimit),
            XMVectorLessOrEqual(vVertDistSq, vertDistSqLimit)
            );
        if (XMVector3EqualInt(check, XMVectorTrueInt()))
        {
            m_currentCheckpoint = i;

            if (i == (m_checkpoints.size() - 1))
                return CheckpointState::Goal;
            else
                return CheckpointState::Save;
        }
    }

    return CheckpointState::None;
}

void MarbleMaze::Update(float timeTotal, float timeDelta)
{
    ProfileScope profiler(L"Update", Colors::Lime); // Green

    // When the game is first loaded, we display a load screen
    // and load any deferred resources that might be too expensive
    // to load during initialization.
    if (!m_deferredResourcesReady || !m_loadedSave)
    {
        // At this point we can draw a progress bar, or if we had
        // loaded audio, we could play audio during the loading process.

        return;
    }

    if (!m_audio.m_isAudioStarted && m_windowActive)
    {
        m_audio.Start();
    }

    UserInterface::GetInstance().Update(timeTotal, timeDelta);

    if (m_gameState == GameState::Initial)
        SetGameState(GameState::MainMenu);

    switch (m_gameState)
    {
        case GameState::PreGameCountdown:
            if (m_preGameCountdownTimer.IsCountdownComplete())
                SetGameState(GameState::InGameActive);
            break;
    }

#pragma region Process Input

    float combinedTiltX = 0.0f;
    float combinedTiltY = 0.0f;

    // Check whether the game should return to the main menu.
    if (m_backKeyPressed)
    {
        m_backKeyPressed = false;
        if (m_gameState == GameState::InGameActive ||
            m_gameState == GameState::InGamePaused ||
            m_gameState == GameState::PreGameCountdown ||
            m_gameState == GameState::HighScoreDisplay)
        {
            SetGameState(GameState::MainMenu);
            m_inGameStopwatchTimer.SetVisible(false);
            m_preGameCountdownTimer.SetVisible(false);
        }
    }

    // Check whether the user chose a button from the UI.
    bool anyPoints = !m_pointQueue.empty();
    while (!m_pointQueue.empty())
    {
        UserInterface::GetInstance().HitTest(m_pointQueue.front());
        m_profilerGraph.HitTest(m_pointQueue.front());
        m_pointQueue.pop();
    }

    if (m_topRightToggleCorner.IsPressed())
    {
        m_topRightToggleCorner.SetPressed(false);
        m_touches.clear();

        m_isDSBG = !m_isDSBG;
        DrawingSurfaceTypeToggled(m_isDSBG);
        
        m_drawingSurfaceTypeText.SetText(m_isDSBG ? L"Background Grid" : L"Drawing Surface");        
        m_drawingSurfaceTypeText.SetVisible(true);
        m_drawingSurfaceTypeText.SetTextOpacity(1.0f);
        m_drawingSurfaceTypeText.FadeOut(2.0f);
    }

    if (m_topLeftToggleCorner.IsPressed())
    {
        m_topLeftToggleCorner.SetPressed(false);
        m_touches.clear();

        ResolutionToggled();
    }

    switch (m_gameState)
    {
    case GameState::HighScoreDisplay:
        if (anyPoints)
            SetGameState(GameState::MainMenu);
        break;

    case GameState::PostGameResults:
        if (anyPoints)
            SetGameState(GameState::HighScoreDisplay);
        break;

    case GameState::InGamePaused:
        if (m_pausedText.IsPressed())
        {
            m_pausedText.SetPressed(false);
            SetGameState(GameState::InGameActive);
        }
        break;
    }

    // Update the game state if the user chose a menu option.
    if (m_startGameButton.IsPressed())
    {
        SetGameState(GameState::PreGameCountdown);
        m_startGameButton.SetPressed(false);
    }
    if (m_highScoreButton.IsPressed())
    {
        SetGameState(GameState::HighScoreDisplay);
        m_highScoreButton.SetPressed(false);
    }

    // Account for touch input.
    const float touchScalingFactor = 2.0f;
    for (TouchMap::const_iterator iter = m_touches.cbegin(); iter != m_touches.cend(); ++iter)
    {
        combinedTiltX += iter->second.x * touchScalingFactor;
        combinedTiltY += iter->second.y * touchScalingFactor;
    }

    // Account for sensors.
    const float acceleromterScalingFactor = 3.5f;
    if (m_accelerometer != nullptr)
    {
        Windows::Devices::Sensors::AccelerometerReading^ reading =
            m_accelerometer->GetCurrentReading();

        if (reading != nullptr)
        {
            combinedTiltX += static_cast<float>(reading->AccelerationX) * acceleromterScalingFactor;
            combinedTiltY += static_cast<float>(reading->AccelerationY) * acceleromterScalingFactor;
            // Check whether the phone is moving or stationary. If it's been stationary for too long, release the
            // request that keeps the screen from timing out.
            XMFLOAT3 currentAccReading(
                static_cast<float>(reading->AccelerationX),
                static_cast<float>(reading->AccelerationY),
                static_cast<float>(reading->AccelerationZ)
                );
            XMVECTOR difference = XMVector3LengthSq(XMLoadFloat3(&currentAccReading) - XMLoadFloat3(&m_previousAcceleration));
            if (XMVectorGetX(difference) < UserInactivityThreshold) 
            {
            }
            else
            {
                // Store the value of the accelerometer and the time of the reading to compare to future readings
                // so that it's possible to determine if the phone has been stationary for too long.
                m_previousAcceleration = currentAccReading;
                m_previousAccelerationTime = timeTotal;
            }
        }
    }

    // Clamp input.
    combinedTiltX = max(-1, min(1, combinedTiltX));
    combinedTiltY = max(-1, min(1, combinedTiltY));

    if (m_gameState != GameState::PreGameCountdown &&
        m_gameState != GameState::InGameActive &&
        m_gameState != GameState::InGamePaused)
    {
        // Ignore tilt when the menu is active.
        combinedTiltX = 0.0f;
        combinedTiltY = 0.0f;
    }

#pragma endregion

#pragma region Physics

    const float maxTilt = 1.0f / 8.0f;
    XMVECTOR gravity = XMVectorSet(combinedTiltX * maxTilt, combinedTiltY * maxTilt, 1.0f, 0.0f);
    gravity = XMVector3Normalize(gravity);

    XMFLOAT3 g;
    XMStoreFloat3(&g, gravity);
    m_physics.SetGravity(g);

    if (m_gameState == GameState::InGameActive)
    {
        // Only update physics when gameplay is active.
        m_physics.UpdatePhysicsSimulation(timeDelta);

        // Handle checkpoints.
        switch (UpdateCheckpoints())
        {
        case CheckpointState::Save:
            // Display checkpoint notice.
            m_checkpointText.SetVisible(true);
            m_checkpointText.SetTextOpacity(1.0f);
            m_checkpointText.FadeOut(2.0f);
            m_audio.PlaySoundEffect(CheckpointEvent);
            SaveState();
            break;

        case CheckpointState::Goal:
            m_inGameStopwatchTimer.Stop();
            m_newHighScore.elapsedTime = m_inGameStopwatchTimer.GetElapsedTime();
            SYSTEMTIME systemTime;
            GetLocalTime(&systemTime);
            WCHAR buffer[64];
            swprintf_s(buffer, L"%d/%d/%d", systemTime.wYear, systemTime.wMonth, systemTime.wDay);
            m_newHighScore.tag = ref new Platform::String(buffer);
            m_highScoreTable.AddScoreToTable(m_newHighScore);

            m_audio.PlaySoundEffect(CheckpointEvent);
            m_audio.StopSoundEffect(RollingEvent);

            // Display game results.
            SetGameState(GameState::PostGameResults);
            SaveState();
            break;
        }
    }

    float3 marblePosition;
    memcpy(&marblePosition, &m_physics.GetPosition(), sizeof(float3));
    static float3 oldMarblePosition = marblePosition;

    const float4x4 initialMarbleRotationMatrix = mul(rotationY(90.0f), rotationX(90.0f));
    static float4x4 marbleRotationMatrix = initialMarbleRotationMatrix;

    // Check whether the marble fell off of the maze.
    const float fadeOutDepth = 0.0f;
    const float resetDepth = 80.0f;
    if (marblePosition.z >= fadeOutDepth)
    {
        m_targetLightStrength = 0.0f;
    }
    if (marblePosition.z >= resetDepth)
    {
        // Reset marble.
        memcpy(&marblePosition, &m_checkpoints[m_currentCheckpoint], sizeof(XMFLOAT3));
        oldMarblePosition = marblePosition;
        m_physics.SetPosition((const XMFLOAT3&)marblePosition);
        m_physics.SetVelocity(XMFLOAT3(0, 0, 0));
        m_lightStrength = 0.0f;
        m_targetLightStrength = 1.0f;

        m_resetCamera = true;
        m_resetMarbleRotation = true;
        m_audio.PlaySoundEffect(FallingEvent);
    }

    float3 marbleRotation = (oldMarblePosition - marblePosition) / m_physics.GetRadius();
    oldMarblePosition = marblePosition;

    if (m_resetMarbleRotation)
    {
        marbleRotationMatrix = initialMarbleRotationMatrix;
        m_resetMarbleRotation = false;
    }
    else
    {
        marbleRotationMatrix = mul(rotationY(marbleRotation.x * 180.0f / 3.1415926535f), marbleRotationMatrix);
        marbleRotationMatrix = mul(rotationX(marbleRotation.y * -180.0f / 3.1415926535f), marbleRotationMatrix);
    }

#pragma endregion

#pragma region Update Camera

    static float eyeDistance = 200.0f;
    static float3 eyePosition = float3(0, 0, 0);

    // Gradually move the camera above the marble.
    float3 targetEyePosition = marblePosition - (eyeDistance * float3(g.x, g.y, g.z));
    if (m_resetCamera)
    {
        eyePosition = targetEyePosition;
        m_resetCamera = false;
    }
    else
    {
        eyePosition = eyePosition + ((targetEyePosition - eyePosition) * min(1, timeDelta * 8));
    }

    // Look at the marble.
    if ((m_gameState == GameState::MainMenu) || (m_gameState == GameState::HighScoreDisplay))
    {
        // Override camera position for menus.
        eyePosition = marblePosition + float3(75.0f, -150.0f, -75.0f);
        m_camera->SetViewParameters(eyePosition, marblePosition, float3(0.0f, 0.0f, -1.0f));
    }
    else
    {
        m_camera->SetViewParameters(eyePosition, marblePosition, float3(0.0f, 1.0f, 0.0f));
    }

#pragma endregion

#pragma region Update Constant Buffers

    // Update the model matrices based on the simulation.
    m_mazeConstantBufferData.model = identity();
    m_marbleConstantBufferData.model = mul(
        translation(marblePosition.x, marblePosition.y, marblePosition.z),
        marbleRotationMatrix
        );

    // Update the view matrix based on the camera.
    float4x4 view;
    m_camera->GetViewMatrix(&view);
    m_mazeConstantBufferData.view = view;
    m_marbleConstantBufferData.view = view;

    // Update lighting constants.
    m_lightStrength += (m_targetLightStrength - m_lightStrength) * min(1, timeDelta * 4);

    m_mazeConstantBufferData.marblePosition = marblePosition;
    m_mazeConstantBufferData.marbleRadius = m_physics.GetRadius();
    m_mazeConstantBufferData.lightStrength = m_lightStrength;
    m_marbleConstantBufferData.marblePosition = marblePosition;
    m_marbleConstantBufferData.marbleRadius = m_physics.GetRadius();
    m_marbleConstantBufferData.lightStrength = m_lightStrength;

#pragma endregion

#pragma region Update Audio

    if (! m_audio.HasEngineExperiencedCriticalError())
    {
        if (m_gameState == GameState::InGameActive)
        {
            float wallDistances[8];
            int returnedCount = m_physics.GetRoomDimensions(wallDistances, ARRAYSIZE(wallDistances));
            assert(returnedCount == ARRAYSIZE(wallDistances));
            m_audio.SetRoomSize(m_physics.GetRoomSize(), wallDistances);
            CollisionInfo ci = m_physics.GetCollisionInfo();

            // Calculate roll sound, and pitch according to velocity.
            XMFLOAT3 velocity = m_physics.GetVelocity();
            XMFLOAT3 position = m_physics.GetPosition();
            float volumeX = abs(velocity.x) / 200;
            if (volumeX > 1.0) volumeX = 1.0;
            if (volumeX < 0.0) volumeX = 0.0;
            float volumeY = abs(velocity.y) / 200;
            if (volumeY > 1.0) volumeY = 1.0;
            if (volumeY < 0.0) volumeY = 0.0;
            float volume = max(volumeX, volumeY);

            // Pitch of the rolling sound ranges from .85 to 1.05f,
            // increasing logarithmically.
            float pitch = .85f + (volume * volume / 5.0f);

            // Play the roll sound only if the marble is actually rolling.
            if (ci.isRollingOnFloor && volume > 0)
            {
                if (!m_audio.IsSoundEffectStarted(RollingEvent))
                {
                    m_audio.PlaySoundEffect(RollingEvent);
                }

                // Update the volume and pitch by the velocity.
                m_audio.SetSoundEffectVolume(RollingEvent, volume);
                m_audio.SetSoundEffectPitch(RollingEvent, pitch);

                // The rolling sound has at most 8000Hz sounds, so we linearly
                // ramp up the low-pass filter the faster we go.
                // We also reduce the Q-value of the filter, starting with a
                // relatively broad cutoff and get progressively tighter.
                m_audio.SetSoundEffectFilter(
                    RollingEvent,
                    600.0f + 8000.0f * volume,
                    XAUDIO2_MAX_FILTER_ONEOVERQ - volume*volume
                    );
            }
            else
            {
                m_audio.SetSoundEffectVolume(RollingEvent, 0);
            }

            if (ci.elasticCollision && ci.maxCollisionSpeed > 10)
            {
                m_audio.PlaySoundEffect(CollisionEvent);

                float collisionVolume = ci.maxCollisionSpeed/150.0f;
                collisionVolume = min(collisionVolume * collisionVolume, 1.0f);
                m_audio.SetSoundEffectVolume(CollisionEvent, collisionVolume);
            }
        }
        else
        {
            m_audio.SetSoundEffectVolume(RollingEvent, 0);
        }
    }
#pragma endregion

}

IAsyncAction^ MarbleMaze::SaveState()
{
    if (m_savingState) return create_async([]() { });

    m_savingState = true;

    auto folder = ApplicationData::Current->LocalFolder;
    task<StorageFile^> getFileTask(folder->CreateFileAsync(
        ref new Platform::String(SaveStateFile), 
        CreationCollisionOption::ReplaceExisting));

    // Create a local to allow the DataReader to be passed between lambdas.
    auto writer = std::make_shared<Streams::DataWriter^>(nullptr);
    
    return create_async([=] () {
        return getFileTask.then([](StorageFile^ file)
        {
            return file->OpenAsync(FileAccessMode::ReadWrite);
        }).then([this, writer](Streams::IRandomAccessStream^ stream)
        {
            Streams::DataWriter^ state = ref new Streams::DataWriter(stream);
            *writer = state;

            XMFLOAT3 postion = m_physics.GetPosition();
            state->WriteSingle(postion.x);
            state->WriteSingle(postion.y);
            state->WriteSingle(postion.z);

            state->WriteSingle(m_inGameStopwatchTimer.GetElapsedTime());

            XMFLOAT3 velocity = m_physics.GetVelocity();
            state->WriteSingle(velocity.x);
            state->WriteSingle(velocity.y);
            state->WriteSingle(velocity.z);

            state->WriteInt32(static_cast<int>(m_gameState));
            state->WriteInt32(static_cast<int>(m_currentCheckpoint));

            // Scores.
            int i = 0;
            HighScoreEntries entries = m_highScoreTable.GetEntries();

            state->WriteInt32(static_cast<int>(entries.size()));
            for (auto iter = entries.begin(); iter != entries.end(); ++iter)
            {
                state->WriteSingle(iter->elapsedTime);

                unsigned int codeUnits = state->MeasureString(iter->tag);
                state->WriteUInt32(codeUnits);
                state->WriteString(iter->tag);
            }

            return state->StoreAsync();
        }).then([writer](uint32 count)
        {
            return (*writer)->FlushAsync();
        }).then([this, writer](bool flushed)
        {
            delete (*writer);
            m_savingState = false;
        });
    });
}

void MarbleMaze::LoadState()
{
    auto folder = ApplicationData::Current->LocalFolder;
    task<StorageFile^> getFileTask(folder->GetFileAsync(
        ref new Platform::String(SaveStateFile)));

    // Create a local to allow the DataReader to be passed between lambdas.
    auto reader = std::make_shared<Streams::DataReader^>(nullptr);
    getFileTask.then([this, reader](task<StorageFile^> fileTask)
    {
        try
        {
            StorageFile^ file = fileTask.get();

            task<Streams::IRandomAccessStreamWithContentType^> (file->OpenReadAsync()).then([reader](Streams::IRandomAccessStreamWithContentType^ stream)
            {
                *reader = ref new Streams::DataReader(stream);
                return (*reader)->LoadAsync(static_cast<uint32>(stream->Size));
            }).then([this, reader](uint32 bytesRead)
            {
                XMFLOAT3 position;
                XMFLOAT3 velocity;
                int gameState;
                int currentCheckpoint;
                const int minimumSize = 40; // Ten 4-byte reads.

                Streams::DataReader^ state = (*reader);

                if (bytesRead < minimumSize)
                {
                    // Data is too small, assume it is corrupt.
                    m_loadedSave = true;
                    return;
                }

                try
                {
                    position.x = state->ReadSingle();
                    position.y = state->ReadSingle();
                    position.z = state->ReadSingle();

                    float elapsedTime = state->ReadSingle();

                    velocity.x = state->ReadSingle();
                    velocity.y = state->ReadSingle();
                    velocity.z = state->ReadSingle();

                    gameState = state->ReadInt32();
                    currentCheckpoint = state->ReadInt32();

                    switch (static_cast<GameState>(gameState))
                    {
                    case GameState::Initial:
                        break;

                    case GameState::MainMenu:
                    case GameState::HighScoreDisplay:
                    case GameState::PreGameCountdown:
                    case GameState::PostGameResults:
                        SetGameState(GameState::MainMenu);
                        break;

                    case GameState::InGameActive:
                    case GameState::InGamePaused:
                        m_inGameStopwatchTimer.SetVisible(true);
                        m_inGameStopwatchTimer.SetElapsedTime(elapsedTime);
                        m_physics.SetPosition(position);
                        m_physics.SetVelocity(velocity);
                        m_currentCheckpoint = currentCheckpoint;
                        SetGameState(GameState::InGameActive);
                        break;
                    }

                    int count = state->ReadInt32();

                    const int bufferLength = 16;
                    char16 str[bufferLength];

                    for (int i = 0; i < count; i++)
                    {
                        HighScoreEntry entry;
                        int len = swprintf_s(str, bufferLength, L"%d", i);
                        Platform::String^ string = ref new Platform::String(str, len);

                        entry.elapsedTime = state->ReadSingle();
                        unsigned int codeUnits = state->ReadUInt32();
                        entry.tag = state->ReadString(codeUnits);
                        m_highScoreTable.AddScoreToTable(entry);
                    }
                }
                catch (Platform::Exception^ e)
                {
                    // Do nothing.
                }
                
                m_loadedSave = true;
            });;
        }
        catch (Platform::Exception^ e)
        {
            m_loadedSave = true;
        }
    });
}

inline XMFLOAT2 PointToTouch(Windows::Foundation::Point point, Windows::Foundation::Rect bounds)
{
    float touchRadius = min(bounds.Width, bounds.Height);
    float dx = (point.X - (bounds.Width / 2.0f)) / touchRadius;
    float dy = ((bounds.Height / 2.0f) - point.Y) / touchRadius;

    return XMFLOAT2(dx, dy);
}

void MarbleMaze::AddTouch(int id, Windows::Foundation::Point point)
{
    m_touches[id] = PointToTouch(point, m_windowBounds);

    m_pointQueue.push(point);
}

void MarbleMaze::UpdateTouch(int id, Windows::Foundation::Point point)
{
    if (m_touches.find(id) != m_touches.end())
        m_touches[id] = PointToTouch(point, m_windowBounds);
}

void MarbleMaze::RemoveTouch(int id)
{
    m_touches.erase(id);
}

IAsyncAction^ MarbleMaze::OnSuspending()
{
    m_audio.SuspendAudio();
    return SaveState();
}

void MarbleMaze::OnResuming()
{
    m_audio.ResumeAudio();
}

void MarbleMaze::OnFocusChange(bool active)
{
    static bool lostFocusPause = false;

    if (m_deferredResourcesReady)
    {
        if (m_windowActive != active)
        {
            if (active)
            {
                m_audio.ResumeAudio();
                if ((m_gameState == GameState::InGamePaused) && lostFocusPause)
                {
                    SetGameState(GameState::InGameActive);
                }
            }
            else
            {
                m_audio.SuspendAudio();
                if (m_gameState == GameState::InGameActive)
                {
                    SetGameState(GameState::InGamePaused);
                    lostFocusPause = true;
                    SaveState();
                }
                else if (m_gameState == GameState::PreGameCountdown)
                {
                    SetGameState(GameState::MainMenu);
                    m_inGameStopwatchTimer.SetVisible(false);
                    m_preGameCountdownTimer.SetVisible(false);
                }
            }
        }
    }

    m_windowActive = active;
}

bool MarbleMaze::OnBackPressed()
{
    bool handleBackKey = false;

    if (m_gameState != GameState::MainMenu)
    {
        m_backKeyPressed = true;
        handleBackKey = true;
    }

    return handleBackKey;
}

void MarbleMaze::ShowResolutionText(Platform::String^ resolutionText)
{
    m_resolutionText.SetText(resolutionText);
    m_resolutionText.SetVisible(true);
    m_resolutionText.SetTextOpacity(1.0f);
    m_resolutionText.FadeOut(2.0f);
}

FORCEINLINE int FindMeshIndexByName(SDKMesh &mesh, const char *meshName)
{
    UINT meshCount = mesh.GetNumMeshes();
    for (UINT i = 0; i < meshCount; ++i)
    {
        if (0 == _stricmp(mesh.GetMesh(i)->Name, meshName))
            return i;
    }

    return -1; // Not found.
}

HRESULT MarbleMaze::ExtractTrianglesFromMesh(
    SDKMesh &mesh,
    const char *meshName,
    std::vector<Triangle> &triangles
    )
{
    triangles.clear();

    int meshIndex = FindMeshIndexByName(mesh, meshName);
    if (meshIndex < 0)
    {
        return E_FAIL;
    }
    SDKMESH_MESH *currentmesh = mesh.GetMesh(meshIndex);

    for (UINT i = 0; i < currentmesh->NumSubsets; ++i)
    {
        SDKMESH_SUBSET *subsetmesh = mesh.GetSubset(meshIndex, i);

        USHORT *indices = (USHORT*)mesh.GetRawIndicesAt(currentmesh->IndexBuffer) + subsetmesh->IndexStart;
        BYTE *vertices = mesh.GetRawVerticesAt(currentmesh->VertexBuffers[0]) + (subsetmesh->VertexStart * m_vertexStride);
        for (UINT j = 0; j < subsetmesh->IndexCount; j += 3)
        {
            XMFLOAT3 a, b, c;
            memcpy(&a, vertices + (*(indices++) * m_vertexStride), sizeof(XMFLOAT3));
            memcpy(&b, vertices + (*(indices++) * m_vertexStride), sizeof(XMFLOAT3));
            memcpy(&c, vertices + (*(indices++) * m_vertexStride), sizeof(XMFLOAT3));
            triangles.push_back(Triangle(a, b, c));
        }
    }

    return S_OK;
}
