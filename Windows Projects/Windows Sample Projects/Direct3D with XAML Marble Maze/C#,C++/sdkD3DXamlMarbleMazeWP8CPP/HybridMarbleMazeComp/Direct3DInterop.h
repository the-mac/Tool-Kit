#pragma once

#include "pch.h"
#include "BasicTimer.h"
#include "MarbleMaze.h"

namespace HybridMarbleMazeComp
{

enum class Resolutions
{
    Full    = 1,
    Half    = 2,
    Quarter = 4,
    Eighth  = 8
};

[Windows::Foundation::Metadata::WebHostHidden]
public ref class Direct3DInterop sealed : public Windows::Phone::Input::Interop::IDrawingSurfaceManipulationHandler
{
public:
    Direct3DInterop();

    Windows::Phone::Graphics::Interop::IDrawingSurfaceContentProvider^ CreateContentProvider();
    Windows::Phone::Graphics::Interop::IDrawingSurfaceBackgroundContentProvider^ CreateBackgroundContentProvider();

    // IDrawingSurfaceManipulationHandler
    virtual void SetManipulationHost(Windows::Phone::Input::Interop::DrawingSurfaceManipulationHost^ manipulationHost);

    void UpdateForWindowSizeChange(float width, float height);
    void OnFocusChange(bool active);
    void OnResuming();
    Windows::Foundation::IAsyncAction^ OnSuspending();
    bool IsBackKeyHandled();
    bool IsDSBG();

    event DrawingSurfaceTypeToggledHandler^ DrawingSurfaceTypeToggled;
    property Windows::Foundation::Size NativeResolution;
    property Windows::Foundation::Size RenderResolution;

protected:
    // Event Handlers
    void OnPointerPressed(Windows::Phone::Input::Interop::DrawingSurfaceManipulationHost^ sender, Windows::UI::Core::PointerEventArgs^ args);
    void OnPointerMoved(Windows::Phone::Input::Interop::DrawingSurfaceManipulationHost^ sender, Windows::UI::Core::PointerEventArgs^ args);
    void OnPointerReleased(Windows::Phone::Input::Interop::DrawingSurfaceManipulationHost^ sender, Windows::UI::Core::PointerEventArgs^ args);

internal:
    void Connect();
    void Disconnect();
    void PrepareResources(LARGE_INTEGER presentTargetTime);
    void Draw(_In_ ID3D11Device1* device, _In_ ID3D11DeviceContext1* context, _In_ ID3D11RenderTargetView* renderTargetView);
    void OnDrawingSurfaceTypeToggle(bool isDSBG);
    void OnResolutionToggled();

private:
    MarbleMaze^ m_marbleMaze;
    BasicTimer^ m_timer;
    Resolutions m_renderResolution;
};

}
