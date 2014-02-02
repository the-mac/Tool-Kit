// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

// This header shows you how to render animated meshes.
// If you don't need animations, remove this file and fix any build errors
// by removing all references to the SkinnedMeshedRenderer class.

#pragma once

#include "../VSD3DStarter.h"

#define MAX_BONES 59
struct BoneConstants
{
    DirectX::XMMATRIX Bones[MAX_BONES];
};

struct AnimationState
{
    AnimationState()
    {
        m_animTime = 0;
    }

    std::vector<DirectX::XMFLOAT4X4> m_boneWorldTransforms;
    float m_animTime;
};

class SkinnedMeshRenderer
{   
public:
    SkinnedMeshRenderer()
    {
    }

    ~SkinnedMeshRenderer()
    {
    }

    void Initialize(ID3D11Device* device, ID3D11DeviceContext* deviceContext)
    {
        UNREFERENCED_PARAMETER (deviceContext);

        m_skinningShader = nullptr;
        m_boneConstantBuffer = nullptr;
        m_skinningVertexLayout = nullptr;

        //
        // create constant buffers
        //
        D3D11_BUFFER_DESC bufferDesc;
        bufferDesc.Usage = D3D11_USAGE_DEFAULT;
        bufferDesc.BindFlags = D3D11_BIND_CONSTANT_BUFFER;
        bufferDesc.CPUAccessFlags = 0;
        bufferDesc.MiscFlags = 0;
        bufferDesc.StructureByteStride = 0;

        bufferDesc.ByteWidth = sizeof(BoneConstants);
        device->CreateBuffer(&bufferDesc, nullptr, &m_boneConstantBuffer);

        //
        // skinning assets
        //
        FILE* fp = NULL;

        fopen_s(&fp, "SkinningVertexShader.cso", "rb");
        fseek(fp, 0, SEEK_END);
        long size = ftell(fp);
        fseek(fp, 0, SEEK_SET);

        std::vector<BYTE> buffer(size);
        if (size > 0)
        {
            fread(&buffer[0], 1, size, fp);

            device->CreateVertexShader(&buffer[0], size, NULL, &m_skinningShader);
        }

        fclose(fp);

        //
        // create the vertex layout
        //
        D3D11_INPUT_ELEMENT_DESC layout[] =
        {
            { "POSITION", 0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 0, D3D11_INPUT_PER_VERTEX_DATA, 0 },
            { "NORMAL", 0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 12, D3D11_INPUT_PER_VERTEX_DATA, 0 },
            { "TANGENT", 0, DXGI_FORMAT_R32G32B32A32_FLOAT, 0, 24, D3D11_INPUT_PER_VERTEX_DATA, 0 },
            { "COLOR", 0, DXGI_FORMAT_R8G8B8A8_UNORM, 0, 40, D3D11_INPUT_PER_VERTEX_DATA, 0 },
            { "TEXCOORD", 0, DXGI_FORMAT_R32G32_FLOAT, 0, 44, D3D11_INPUT_PER_VERTEX_DATA, 0 },
            { "BLENDINDICES", 0, DXGI_FORMAT_R8G8B8A8_UINT, 1, 0, D3D11_INPUT_PER_VERTEX_DATA, 0 },
            { "BLENDWEIGHT", 0, DXGI_FORMAT_R32G32B32A32_FLOAT, 1, 4, D3D11_INPUT_PER_VERTEX_DATA, 0 },
        };
        device->CreateInputLayout(layout, ARRAYSIZE(layout), &buffer[0], buffer.size(), &m_skinningVertexLayout);
    }

    void UpdateAnimation(float timeDelta, std::vector<VSD3DStarter::Mesh*>& meshes)
    {
        for (VSD3DStarter::Mesh* mesh : meshes)
        {
            AnimationState* animState = (AnimationState*)mesh->Tag;
            if (animState == NULL)
            {
                continue;
            }

            animState->m_animTime  += timeDelta;

            //
            // Update bones
            //
            const std::vector<VSD3DStarter::Mesh::BoneInfo>& skinningInfo = mesh->BoneInfoCollection();
            for (UINT b = 0; b < skinningInfo.size(); b++)
            {
                animState->m_boneWorldTransforms[b] = skinningInfo[b].BoneLocalTransform;
            }

            // get keyframes
            auto& animClips = mesh->AnimationClips();
            auto found = animClips.find(L"Take 001");
            if (found != animClips.end())
            {
                const auto& kf = found->second.Keyframes;
                for (const auto& frame : kf)
                {
                    if (frame.Time > animState->m_animTime)
                    {
                        break;
                    }

                    animState->m_boneWorldTransforms[frame.BoneIndex] = frame.Transform;
                }
                // transform to world
                for (UINT b = 1; b < skinningInfo.size(); b++)
                {
                    const VSD3DStarter::Mesh::BoneInfo& skinning = skinningInfo[b];

                    if (skinning.ParentIndex < 0)
                        continue;

                    DirectX::XMMATRIX leftMat = XMLoadFloat4x4(&animState->m_boneWorldTransforms[b]);
                    DirectX::XMMATRIX rightMat = XMLoadFloat4x4(&animState->m_boneWorldTransforms[skinning.ParentIndex]);

                    DirectX::XMMATRIX ret = leftMat * rightMat;

                    XMStoreFloat4x4(&animState->m_boneWorldTransforms[b], ret);
                }

                for (UINT b = 0; b < skinningInfo.size(); b++)
                {
                    DirectX::XMMATRIX leftMat = XMLoadFloat4x4(&skinningInfo[b].InvBindPos);
                    DirectX::XMMATRIX rightMat = XMLoadFloat4x4(&animState->m_boneWorldTransforms[b]);

                    DirectX::XMMATRIX ret = leftMat * rightMat;

                    XMStoreFloat4x4(&animState->m_boneWorldTransforms[b], ret);
                }

                if (animState->m_animTime > found->second.EndTime)
                {
                    animState->m_animTime = found->second.StartTime + (animState->m_animTime - found->second.EndTime);
                }
            }
        }
    }

    void RenderSkinnedMesh(VSD3DStarter::Mesh* mesh, const VSD3DStarter::Graphics& graphics, const DirectX::XMMATRIX& world)
    {
        ID3D11DeviceContext* deviceContext = graphics.GetDeviceContext();

        BOOL supportsShaderResources = graphics.GetDeviceFeatureLevel() >= D3D_FEATURE_LEVEL_10_0;

        const DirectX::XMMATRIX& view = graphics.GetCamera().GetView();
        const DirectX::XMMATRIX& projection = graphics.GetCamera().GetProjection() * graphics.GetCamera().GetOrientationMatrix();

        //
        // compute the object matrices
        //
        DirectX::XMMATRIX localToView = world * view;
        DirectX::XMMATRIX localToProj = world * view * projection;

        //
        // initialize object constants and update the constant buffer
        //
        VSD3DStarter::ObjectConstants objConstants;
        objConstants.LocalToWorld4x4 = DirectX::XMMatrixTranspose(world);
        objConstants.LocalToProjected4x4 = DirectX::XMMatrixTranspose(localToProj);
        objConstants.WorldToLocal4x4 = DirectX::XMMatrixTranspose(DirectX::XMMatrixInverse(nullptr, world));
        objConstants.WorldToView4x4 = DirectX::XMMatrixTranspose(view);
        objConstants.UvTransform4x4 = DirectX::XMMatrixIdentity();
        objConstants.EyePosition = graphics.GetCamera().GetPosition();
        graphics.UpdateObjectConstants(objConstants);

        //
        // assign constant buffers to correct slots
        //
        ID3D11Buffer* constantBuffer = graphics.GetLightConstants();
        deviceContext->VSSetConstantBuffers(1, 1, &constantBuffer);
        deviceContext->PSSetConstantBuffers(1, 1, &constantBuffer);

        constantBuffer = graphics.GetMiscConstants();
        deviceContext->VSSetConstantBuffers(3, 1, &constantBuffer);
        deviceContext->PSSetConstantBuffers(3, 1, &constantBuffer);

        ID3D11Buffer* boneConsts = m_boneConstantBuffer.Get();
        deviceContext->VSSetConstantBuffers(4, 1, &boneConsts);

        //
        // prepare to draw
        //

        //
        // NOTE: set the skinning vertex layout
        //
        deviceContext->IASetInputLayout(m_skinningVertexLayout.Get());
        deviceContext->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);

        BoneConstants boneConstants;

        // update bones
        AnimationState* animState = (AnimationState*)mesh->Tag;
        if (animState != NULL)
        {
            // copy to constants
            for (UINT b = 0; b < animState->m_boneWorldTransforms.size(); b++)
            {
                boneConstants.Bones[b] =  DirectX::XMMatrixTranspose((XMLoadFloat4x4(&animState->m_boneWorldTransforms[b])));
            }
        }

        //
        // update constants
        //
        deviceContext->UpdateSubresource(m_boneConstantBuffer.Get(), 0, nullptr, &boneConstants, 0, 0);

        //
        // loop over each submesh
        //
        for (const VSD3DStarter::Mesh::SubMesh& submesh : mesh->SubMeshes())
        {
            //
            // only draw submeshes that have valid materials
            //
            VSD3DStarter::MaterialConstants materialConstants;

            if (submesh.IndexBufferIndex < mesh->IndexBuffers().size() &&
                submesh.VertexBufferIndex < mesh->VertexBuffers().size())
            {
                ID3D11Buffer* vbs[2] = 
                {
                    mesh->VertexBuffers()[submesh.VertexBufferIndex],
                    mesh->SkinningVertexBuffers()[submesh.VertexBufferIndex]
                };

                UINT stride[2] = {sizeof(VSD3DStarter::Vertex), sizeof(VSD3DStarter::SkinningVertexInput)};
                UINT offset[2] = {0, 0};
                deviceContext->IASetVertexBuffers(0, 2, vbs, stride, offset);
                deviceContext->IASetIndexBuffer(mesh->IndexBuffers()[submesh.IndexBufferIndex], DXGI_FORMAT_R16_UINT, 0);
            }

            if (submesh.MaterialIndex < mesh->Materials().size())
            {
                const VSD3DStarter::Mesh::Material& material = mesh->Materials()[submesh.MaterialIndex];

                //
                // update material constant buffer
                //
                memcpy(&materialConstants.Ambient, material.Ambient, sizeof(material.Ambient));
                memcpy(&materialConstants.Diffuse, material.Diffuse, sizeof(material.Diffuse));
                memcpy(&materialConstants.Specular, material.Specular, sizeof(material.Specular));
                memcpy(&materialConstants.Emissive, material.Emissive, sizeof(material.Emissive));
                materialConstants.SpecularPower = material.SpecularPower;

                graphics.UpdateMaterialConstants(materialConstants);

                //
                // assign material buffer to correct slots
                //
                constantBuffer = graphics.GetMaterialConstants();
                deviceContext->VSSetConstantBuffers(0, 1, &constantBuffer);
                deviceContext->PSSetConstantBuffers(0, 1, &constantBuffer);

                // update UV transform
                memcpy(&objConstants.UvTransform4x4, &material.UVTransform, sizeof(objConstants.UvTransform4x4));
                graphics.UpdateObjectConstants(objConstants);

                constantBuffer = graphics.GetObjectConstants();
                deviceContext->VSSetConstantBuffers(2, 1, &constantBuffer);
                deviceContext->PSSetConstantBuffers(2, 1, &constantBuffer);

                //
                // assign shaders, samplers and texture resources
                //


                //
                // NOTE: set the skinning shader here
                //

                deviceContext->VSSetShader(m_skinningShader.Get(), nullptr, 0);

                ID3D11SamplerState* samplerState = material.SamplerState.Get();
                if (supportsShaderResources)
                {
                    deviceContext->VSSetSamplers(0, 1, &samplerState);
                }

                deviceContext->PSSetShader(material.PixelShader.Get(), nullptr, 0);
                deviceContext->PSSetSamplers(0, 1, &samplerState);

                for (UINT tex = 0; tex < VSD3DStarter::Mesh::MaxTextures; tex++)
                {
                    ID3D11ShaderResourceView* shaderResourceView = material.Textures[tex].Get();
                    if (supportsShaderResources)
                    {
                        deviceContext->VSSetShaderResources(0+tex, 1, &shaderResourceView);
                        deviceContext->VSSetShaderResources(VSD3DStarter::Mesh::MaxTextures+tex, 1, &shaderResourceView);
                    }

                    deviceContext->PSSetShaderResources(0+tex, 1, &shaderResourceView);
                    deviceContext->PSSetShaderResources(VSD3DStarter::Mesh::MaxTextures+tex, 1, &shaderResourceView);
                }

                //
                // draw the submesh
                //
                deviceContext->DrawIndexed(submesh.PrimCount * 3, submesh.StartIndex, 0);
            }
        }

        ///
        /// clear the extra vertex buffer
        ///
        ID3D11Buffer* vbs[1] = {NULL};
        UINT stride = 0;
        UINT offset = 0;
        deviceContext->IASetVertexBuffers(1, 1, vbs, &stride, &offset);
    }

private:
    Microsoft::WRL::ComPtr<ID3D11VertexShader> m_skinningShader;
    Microsoft::WRL::ComPtr<ID3D11InputLayout> m_skinningVertexLayout;
    Microsoft::WRL::ComPtr<ID3D11Buffer> m_boneConstantBuffer;

};
