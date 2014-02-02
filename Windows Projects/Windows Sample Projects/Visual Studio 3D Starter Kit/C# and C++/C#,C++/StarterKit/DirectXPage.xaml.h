// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// BlankPage.xaml.h
// Declaration of the BlankPage.xaml class.
//

#pragma once

#include "DirectXPage.g.h"
#include "..\Shared\Game.h"
#include "..\Shared\BasicTimer.h"

namespace StarterKit
{
    /// <summary>
    /// A DirectX page that can be used on its own.  Note that it may not be used within a Frame.
    /// </summary>
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class DirectXPage sealed
    {
    public:
        DirectXPage();
        
        void OnPreviousColorPressed(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void OnNextColorPressed(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

        void SaveInternalState(Windows::Foundation::Collections::IPropertySet^ state);
        void LoadInternalState(Windows::Foundation::Collections::IPropertySet^ state);

    private:
        void OnPointerMoved(Platform::Object^ sender, Windows::UI::Xaml::Input::PointerRoutedEventArgs^ args);
        void OnPointerReleased(Platform::Object^ sender, Windows::UI::Xaml::Input::PointerRoutedEventArgs^ args);
        void OnWindowSizeChanged(Windows::UI::Core::CoreWindow^ sender, Windows::UI::Core::WindowSizeChangedEventArgs^ args);
        void OnLogicalDpiChanged(Platform::Object^ sender);
        void OnOrientationChanged(Platform::Object^ sender);
        void OnDisplayContentsInvalidated(Platform::Object^ sender);
        void OnRendering(Object^ sender, Object^ args);
        void OnTapped(Platform::Object^ sender, Windows::UI::Xaml::Input::TappedRoutedEventArgs^ e);

        void ChangeObjectColor(Platform::String^ objectName, int colorIndex);
                
        Windows::Foundation::EventRegistrationToken m_eventToken;

        Game^ m_renderer;

        int m_hitCountCube;
        int m_hitCountSphere;
        int m_hitCountCone;
        int m_hitCountCylinder;
        int m_hitCountTeapot;

        int m_colorIndex;
        int m_colorCount;

        std::vector<Windows::UI::Color> m_colors;
                
        BasicTimer^ m_timer;
    };
}
