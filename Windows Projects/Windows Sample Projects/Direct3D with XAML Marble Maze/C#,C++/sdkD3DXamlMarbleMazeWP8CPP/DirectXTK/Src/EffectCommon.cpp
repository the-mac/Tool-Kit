//--------------------------------------------------------------------------------------
// File: EffectCommon.cpp
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
// http://go.microsoft.com/fwlink/?LinkId=248929
//--------------------------------------------------------------------------------------

#include "pch.h"
#include "EffectCommon.h"
#include "DemandCreate.h"

using namespace DirectX;
using namespace Microsoft::WRL;


// Constructor initializes default matrix values.
EffectMatrices::EffectMatrices()
{
    world = XMMatrixIdentity();
    view = XMMatrixIdentity();
    projection = XMMatrixIdentity();
    worldView = XMMatrixIdentity();
}


// Lazily recomputes the combined world+view+projection matrix.
_Use_decl_annotations_ void EffectMatrices::SetConstants(int& dirtyFlags, XMMATRIX& worldViewProjConstant)
{
    if (dirtyFlags & EffectDirtyFlags::WorldViewProj)
    {
        worldView = XMMatrixMultiply(world, view);

        worldViewProjConstant = XMMatrixTranspose(XMMatrixMultiply(worldView, projection));
                
        dirtyFlags &= ~EffectDirtyFlags::WorldViewProj;
        dirtyFlags |= EffectDirtyFlags::ConstantBuffer;
    }
}


// Constructor initializes default fog settings.
EffectFog::EffectFog()
{
    enabled = false;
    start = 0;
    end = 1;
}


// Lazily recomputes the derived vector used by shader fog calculations.
_Use_decl_annotations_ void EffectFog::SetConstants(int& dirtyFlags, CXMMATRIX worldView, XMVECTOR& fogVectorConstant)
{
    if (enabled)
    {
        if (dirtyFlags & (EffectDirtyFlags::FogVector | EffectDirtyFlags::FogEnable))
        {
            if (start == end)
            {
                // Degenerate case: force everything to 100% fogged if start and end are the same.
                static const XMVECTORF32 fullyFogged = { 0, 0, 0, 1 };

                fogVectorConstant = fullyFogged;
            }
            else
            {
                // We want to transform vertex positions into view space, take the resulting
                // Z value, then scale and offset according to the fog start/end distances.
                // Because we only care about the Z component, the shader can do all this
                // with a single dot product, using only the Z row of the world+view matrix.
        
                // _13, _23, _33, _43
                XMVECTOR worldViewZ = XMVectorMergeXY(XMVectorMergeZW(worldView.r[0], worldView.r[2]),
                                                      XMVectorMergeZW(worldView.r[1], worldView.r[3]));

                // 0, 0, 0, fogStart
                XMVECTOR wOffset = XMVectorSwizzle<1, 2, 3, 0>(XMLoadFloat(&start));

                fogVectorConstant = (worldViewZ + wOffset) / (start - end);
            }

            dirtyFlags &= ~(EffectDirtyFlags::FogVector | EffectDirtyFlags::FogEnable);
            dirtyFlags |= EffectDirtyFlags::ConstantBuffer;
        }
    }
    else
    {
        // When fog is disabled, make sure the fog vector is reset to zero.
        if (dirtyFlags & EffectDirtyFlags::FogEnable)
        {
            fogVectorConstant = g_XMZero;

            dirtyFlags &= ~EffectDirtyFlags::FogEnable;
            dirtyFlags |= EffectDirtyFlags::ConstantBuffer;
        }
    }
}


// Constructor initializes default material color settings.
EffectColor::EffectColor()
{
    diffuseColor = g_XMOne;
    alpha = 1;
}


// Lazily recomputes the material color parameter for shaders that do not support realtime lighting.
void EffectColor::SetConstants(_Inout_ int& dirtyFlags, _Inout_ XMVECTOR& diffuseColorConstant)
{
    if (dirtyFlags & EffectDirtyFlags::MaterialColor)
    {
        XMVECTOR alphaVector = XMVectorReplicate(alpha);

        // xyz = diffuse * alpha, w = alpha.
        diffuseColorConstant = XMVectorSelect(alphaVector, diffuseColor * alphaVector, g_XMSelect1110);

        dirtyFlags &= ~EffectDirtyFlags::MaterialColor;
        dirtyFlags |= EffectDirtyFlags::ConstantBuffer;
    }
}


// Constructor initializes default light settings.
EffectLights::EffectLights()
{
    emissiveColor = g_XMZero;
    ambientLightColor = g_XMZero;

    for (int i = 0; i < MaxDirectionalLights; i++)
    {
        lightEnabled[i] = (i == 0);
        lightDiffuseColor[i] = g_XMOne;
        lightSpecularColor[i] = g_XMZero;
    }
}


// Initializes constant buffer fields to match the current lighting state.
_Use_decl_annotations_ void EffectLights::InitializeConstants(XMVECTOR& specularColorAndPowerConstant, XMVECTOR* lightDirectionConstant, XMVECTOR* lightDiffuseConstant, XMVECTOR* lightSpecularConstant)
{
    static const XMVECTORF32 defaultSpecular = { 1, 1, 1, 16 };
    static const XMVECTORF32 defaultLightDirection = { 0, -1, 0, 0 };
    
    specularColorAndPowerConstant = defaultSpecular;

    for (int i = 0; i < MaxDirectionalLights; i++)
    {
        lightDirectionConstant[i] = defaultLightDirection;

        lightDiffuseConstant[i]  = lightEnabled[i] ? lightDiffuseColor[i]  : g_XMZero;
        lightSpecularConstant[i] = lightEnabled[i] ? lightSpecularColor[i] : g_XMZero;
    }
}


// Lazily recomputes derived parameter values used by shader lighting calculations.
_Use_decl_annotations_ void EffectLights::SetConstants(int& dirtyFlags, EffectMatrices const& matrices, XMMATRIX& worldConstant, XMVECTOR worldInverseTransposeConstant[3], XMVECTOR& eyePositionConstant, XMVECTOR& diffuseColorConstant, XMVECTOR& emissiveColorConstant, bool lightingEnabled)
{
    if (lightingEnabled)
    {
        // World inverse transpose matrix.
        if (dirtyFlags & EffectDirtyFlags::WorldInverseTranspose)
        {
            worldConstant = XMMatrixTranspose(matrices.world);

            XMMATRIX worldInverse = XMMatrixInverse(nullptr, matrices.world);

            worldInverseTransposeConstant[0] = worldInverse.r[0];
            worldInverseTransposeConstant[1] = worldInverse.r[1];
            worldInverseTransposeConstant[2] = worldInverse.r[2];

            dirtyFlags &= ~EffectDirtyFlags::WorldInverseTranspose;
            dirtyFlags |= EffectDirtyFlags::ConstantBuffer;
        }

        // Eye position vector.
        if (dirtyFlags & EffectDirtyFlags::EyePosition)
        {
            XMMATRIX viewInverse = XMMatrixInverse(nullptr, matrices.view);
        
            eyePositionConstant = viewInverse.r[3];

            dirtyFlags &= ~EffectDirtyFlags::EyePosition;
            dirtyFlags |= EffectDirtyFlags::ConstantBuffer;
        }
    }

    // Material color parameters. The desired lighting model is:
    //
    //     ((ambientLightColor + sum(diffuse directional light)) * diffuseColor) + emissiveColor
    //
    // When lighting is disabled, ambient and directional lights are ignored, leaving:
    //
    //     diffuseColor + emissiveColor
    //
    // For the lighting disabled case, we can save one shader instruction by precomputing
    // diffuse+emissive on the CPU, after which the shader can use diffuseColor directly,
    // ignoring its emissive parameter.
    //
    // When lighting is enabled, we can merge the ambient and emissive settings. If we
    // set our emissive parameter to emissive+(ambient*diffuse), the shader no longer
    // needs to bother adding the ambient contribution, simplifying its computation to:
    //
    //     (sum(diffuse directional light) * diffuseColor) + emissiveColor
    //
    // For futher optimization goodness, we merge material alpha with the diffuse
    // color parameter, and premultiply all color values by this alpha.

    if (dirtyFlags & EffectDirtyFlags::MaterialColor)
    {
        XMVECTOR diffuse = diffuseColor;
        XMVECTOR alphaVector = XMVectorReplicate(alpha);

        if (lightingEnabled)
        {
            // Merge emissive and ambient light contributions.
            emissiveColorConstant = (emissiveColor + ambientLightColor * diffuse) * alphaVector;
        }
        else
        {
            // Merge diffuse and emissive light contributions.
            diffuse += emissiveColor;
        }

        // xyz = diffuse * alpha, w = alpha.
        diffuseColorConstant = XMVectorSelect(alphaVector, diffuse * alphaVector, g_XMSelect1110);

        dirtyFlags &= ~EffectDirtyFlags::MaterialColor;
        dirtyFlags |= EffectDirtyFlags::ConstantBuffer;
    }
}


// Helper for turning one of the directional lights on or off.
_Use_decl_annotations_ int EffectLights::SetLightEnabled(int whichLight, bool value, XMVECTOR* lightDiffuseConstant, XMVECTOR* lightSpecularConstant)
{
    ValidateLightIndex(whichLight);

    if (lightEnabled[whichLight] == value)
        return 0;

    lightEnabled[whichLight] = value;

    if (value)
    {
        // If this light is now on, store its color in the constant buffer.
        lightDiffuseConstant[whichLight] = lightDiffuseColor[whichLight];
        lightSpecularConstant[whichLight] = lightSpecularColor[whichLight];
    }
    else
    {
        // If the light is off, reset constant buffer colors to zero.
        lightDiffuseConstant[whichLight] = g_XMZero;
        lightSpecularConstant[whichLight] = g_XMZero;
    }

    return EffectDirtyFlags::ConstantBuffer;
}


// Helper for setting diffuse color of one of the directional lights.
_Use_decl_annotations_ int EffectLights::SetLightDiffuseColor(int whichLight, FXMVECTOR value, XMVECTOR* lightDiffuseConstant)
{
    ValidateLightIndex(whichLight);

    // Locally store the new color.
    lightDiffuseColor[whichLight] = value;

    // If this light is currently on, also update the constant buffer.
    if (lightEnabled[whichLight])
    {
        lightDiffuseConstant[whichLight] = value;
        
        return EffectDirtyFlags::ConstantBuffer;
    }

    return 0;
}


// Helper for setting specular color of one of the directional lights.
_Use_decl_annotations_ int EffectLights::SetLightSpecularColor(int whichLight, FXMVECTOR value, XMVECTOR* lightSpecularConstant)
{
    ValidateLightIndex(whichLight);

    // Locally store the new color.
    lightSpecularColor[whichLight] = value;

    // If this light is currently on, also update the constant buffer.
    if (lightEnabled[whichLight])
    {
        lightSpecularConstant[whichLight] = value;

        return EffectDirtyFlags::ConstantBuffer;
    }
    
    return 0;
}


// Parameter validation helper.
void EffectLights::ValidateLightIndex(int whichLight)
{
    if (whichLight < 0 || whichLight >= MaxDirectionalLights)
    {
        throw std::out_of_range("whichLight parameter out of range");
    }
}


// Activates the default lighting rig (key, fill, and back lights).
void EffectLights::EnableDefaultLighting(_In_ IEffectLights* effect)
{
    static const XMVECTORF32 defaultDirections[MaxDirectionalLights] =
    {
        { -0.5265408f, -0.5735765f, -0.6275069f },
        {  0.7198464f,  0.3420201f,  0.6040227f },
        {  0.4545195f, -0.7660444f,  0.4545195f },
    };

    static const XMVECTORF32 defaultDiffuse[MaxDirectionalLights] =
    {
        { 1.0000000f, 0.9607844f, 0.8078432f },
        { 0.9647059f, 0.7607844f, 0.4078432f },
        { 0.3231373f, 0.3607844f, 0.3937255f },
    };

    static const XMVECTORF32 defaultSpecular[MaxDirectionalLights] =
    {
        { 1.0000000f, 0.9607844f, 0.8078432f },
        { 0.0000000f, 0.0000000f, 0.0000000f },
        { 0.3231373f, 0.3607844f, 0.3937255f },
    };

    static const XMVECTORF32 defaultAmbient = { 0.05333332f, 0.09882354f, 0.1819608f };

    effect->SetLightingEnabled(true);
    effect->SetAmbientLightColor(defaultAmbient);

    for (int i = 0; i < MaxDirectionalLights; i++)
    {
        effect->SetLightEnabled(i, true);
        effect->SetLightDirection(i, defaultDirections[i]);
        effect->SetLightDiffuseColor(i, defaultDiffuse[i]);
        effect->SetLightSpecularColor(i, defaultSpecular[i]);
    }
}


// Gets or lazily creates the specified vertex shader permutation.
ID3D11VertexShader* EffectDeviceResources::DemandCreateVertexShader(_Inout_ ComPtr<ID3D11VertexShader>& vertexShader, ShaderBytecode const& bytecode)
{
    return DemandCreate(vertexShader, mMutex, [&](ID3D11VertexShader** pResult)
    {
        return mDevice->CreateVertexShader(bytecode.code, bytecode.length, nullptr, pResult);
    });
}


// Gets or lazily creates the specified pixel shader permutation.
ID3D11PixelShader* EffectDeviceResources::DemandCreatePixelShader(_Inout_ ComPtr<ID3D11PixelShader>& pixelShader, ShaderBytecode const& bytecode)
{
    return DemandCreate(pixelShader, mMutex, [&](ID3D11PixelShader** pResult)
    {
        return mDevice->CreatePixelShader(bytecode.code, bytecode.length, nullptr, pResult);
    });
}
