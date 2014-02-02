#include "pch.h"

using namespace DirectX;
#include "Direct3DBackgroundContentProvider.h"

using namespace HybridMarbleMazeComp;

Direct3DBackgroundContentProvider::Direct3DBackgroundContentProvider(Direct3DInterop^ controller) :
    m_controller(controller)
{
}

// IDrawingSurfaceContentProviderNative interface
HRESULT Direct3DBackgroundContentProvider::Connect(_In_ IDrawingSurfaceRuntimeHostNative* host, _In_ ID3D11Device1* device)
{
    m_host = host;
    m_controller->Connect();

    return S_OK;
}

void Direct3DBackgroundContentProvider::Disconnect()
{
    m_controller->Disconnect();
    m_host = nullptr;
}

HRESULT Direct3DBackgroundContentProvider::PrepareResources(_In_ const LARGE_INTEGER* presentTargetTime, _Inout_ DrawingSurfaceSizeF* desiredRenderTargetSize)
{
    m_controller->PrepareResources(*presentTargetTime);

    desiredRenderTargetSize->width = m_controller->RenderResolution.Width;
    desiredRenderTargetSize->height = m_controller->RenderResolution.Height;

    return S_OK;
}

HRESULT Direct3DBackgroundContentProvider::Draw(_In_ ID3D11Device1* device, _In_ ID3D11DeviceContext1* context, _In_ ID3D11RenderTargetView* renderTargetView)
{
    if (!device || !context || !renderTargetView) return E_INVALIDARG;

    m_controller->Draw(device, context, renderTargetView);
    return m_host->RequestAdditionalFrame();
}