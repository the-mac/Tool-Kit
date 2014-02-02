#include "pch.h"

using namespace DirectX;
#include "Direct3DInterop.h"
#include "Direct3DContentProvider.h"
#include "Direct3DBackgroundContentProvider.h"

using namespace Windows::Foundation;
using namespace Windows::UI::Core;
using namespace Windows::UI::Input;
using namespace Microsoft::WRL;
using namespace Windows::Phone::Graphics::Interop;
using namespace Windows::Phone::Input::Interop;
using namespace Windows::Graphics::Display;

namespace HybridMarbleMazeComp
{

Direct3DInterop::Direct3DInterop() :
    m_timer(ref new BasicTimer())
{
    m_marbleMaze = ref new MarbleMaze();
    m_marbleMaze->Initialize();

    m_marbleMaze->DrawingSurfaceTypeToggled += ref new DrawingSurfaceTypeToggledHandler(this, &Direct3DInterop::OnDrawingSurfaceTypeToggle);
    m_marbleMaze->ResolutionToggled += ref new ResolutionToggledHandler(this, &Direct3DInterop::OnResolutionToggled);

    m_renderResolution = Resolutions::Full;

    Profiler::Initialize();
}

void Direct3DInterop::OnDrawingSurfaceTypeToggle(bool isDSBG) {
    DrawingSurfaceTypeToggled(isDSBG);
}

void Direct3DInterop::OnResolutionToggled()
{
    switch (m_renderResolution)
    {
        case Resolutions::Full:
            m_renderResolution = Resolutions::Half;
            break;
        case Resolutions::Half:
            m_renderResolution = Resolutions::Quarter;
            break;
        case Resolutions::Quarter:
            m_renderResolution = Resolutions::Eighth;
            break;
        case Resolutions::Eighth:
            m_renderResolution = Resolutions::Full;
            break;
    }

    RenderResolution = Windows::Foundation::Size(
    	NativeResolution.Width / static_cast<int>(m_renderResolution),
    	NativeResolution.Height / static_cast<int>(m_renderResolution)
    	);

    m_marbleMaze->ShowResolutionText(RenderResolution.Width.ToString() + "x" + RenderResolution.Height.ToString());
    m_marbleMaze->SetDpi(RenderResolution.Width / 480.0f * 96.0f);
}

IDrawingSurfaceContentProvider^ Direct3DInterop::CreateContentProvider()
{
    ComPtr<Direct3DContentProvider> provider = Make<Direct3DContentProvider>(this);
    return reinterpret_cast<IDrawingSurfaceContentProvider^>(provider.Get());
}

IDrawingSurfaceBackgroundContentProvider^ Direct3DInterop::CreateBackgroundContentProvider()
{
    ComPtr<Direct3DBackgroundContentProvider> provider = Make<Direct3DBackgroundContentProvider>(this);
    return reinterpret_cast<IDrawingSurfaceBackgroundContentProvider^>(provider.Get());
}

// IDrawingSurfaceManipulationHandler
void Direct3DInterop::SetManipulationHost(DrawingSurfaceManipulationHost^ manipulationHost)
{
    manipulationHost->PointerPressed +=
        ref new TypedEventHandler<DrawingSurfaceManipulationHost^, PointerEventArgs^>(this, &Direct3DInterop::OnPointerPressed);

    manipulationHost->PointerMoved +=
        ref new TypedEventHandler<DrawingSurfaceManipulationHost^, PointerEventArgs^>(this, &Direct3DInterop::OnPointerMoved);

    manipulationHost->PointerReleased +=
        ref new TypedEventHandler<DrawingSurfaceManipulationHost^, PointerEventArgs^>(this, &Direct3DInterop::OnPointerReleased);
}

void Direct3DInterop::UpdateForWindowSizeChange(float width, float height)
{
    m_marbleMaze->UpdateForWindowSizeChange(width, height);
}

void Direct3DInterop::OnFocusChange(bool active)
{
    m_marbleMaze->OnFocusChange(active);
}

void Direct3DInterop::OnResuming()
{
    m_marbleMaze->OnResuming();
}

IAsyncAction^ Direct3DInterop::OnSuspending()
{
    return m_marbleMaze->OnSuspending();
}

bool Direct3DInterop::IsBackKeyHandled()
{
    return m_marbleMaze->OnBackPressed();
}

bool Direct3DInterop::IsDSBG()
{
    return m_marbleMaze->IsDSBG();
}

// Event Handlers
void Direct3DInterop::OnPointerPressed(DrawingSurfaceManipulationHost^ sender, PointerEventArgs^ args)
{
    PointerPoint^ point = args->CurrentPoint;

    m_marbleMaze->AddTouch(point->PointerId, ::Windows::Foundation::Point(point->RawPosition.X, point->RawPosition.Y));
}

void Direct3DInterop::OnPointerMoved(DrawingSurfaceManipulationHost^ sender, PointerEventArgs^ args)
{
    PointerPoint^ point = args->CurrentPoint;

    m_marbleMaze->UpdateTouch(point->PointerId, ::Windows::Foundation::Point(point->RawPosition.X, point->RawPosition.Y));
}

void Direct3DInterop::OnPointerReleased(DrawingSurfaceManipulationHost^ sender, PointerEventArgs^ args)
{
    m_marbleMaze->RemoveTouch(args->CurrentPoint->PointerId);
}

// Interface With Direct3DContentProvider
void Direct3DInterop::Connect()
{
    // Restart timer after renderer has finished initializing.
    m_timer->Reset();
}

void Direct3DInterop::Disconnect()
{
}

void Direct3DInterop::PrepareResources(LARGE_INTEGER presentTargetTime)
{
    Profiler::FrameStart();
    m_timer->Update();
    m_marbleMaze->Update(m_timer->Total, m_timer->Delta);
}

void Direct3DInterop::Draw(_In_ ID3D11Device1* device, _In_ ID3D11DeviceContext1* context, _In_ ID3D11RenderTargetView* renderTargetView)
{
    m_marbleMaze->UpdateDevice(device, context, renderTargetView);
    m_marbleMaze->Render();

    Profiler::Render(device, context);
}

}
