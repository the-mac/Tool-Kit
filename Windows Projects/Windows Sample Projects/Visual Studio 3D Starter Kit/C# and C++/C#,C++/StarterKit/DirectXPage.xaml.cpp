// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// DirectXPage.xaml.cpp
// Implementation of the DirectXPage.xaml class.
//

#include "pch.h"
#include "DirectXPage.xaml.h"

using namespace StarterKit;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::UI;
using namespace Windows::UI::Input;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Globalization::NumberFormatting;

DirectXPage::DirectXPage() : 
    m_hitCountCube(0), 
    m_hitCountCylinder(0), 
    m_hitCountCone(0), 
    m_hitCountSphere(0),
    m_hitCountTeapot(0),
    m_colorIndex(0),
    m_colorCount(0)
{
    InitializeComponent();
    
    m_renderer = ref new Game();

    m_renderer->Initialize(
        Window::Current->CoreWindow,
        SwapChainPanel,
        DisplayProperties::LogicalDpi
        );

    Window::Current->CoreWindow->SizeChanged += 
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &DirectXPage::OnWindowSizeChanged);

    DisplayProperties::LogicalDpiChanged +=
        ref new DisplayPropertiesEventHandler(this, &DirectXPage::OnLogicalDpiChanged);

    DisplayProperties::OrientationChanged +=
        ref new DisplayPropertiesEventHandler(this, &DirectXPage::OnOrientationChanged);

    DisplayProperties::DisplayContentsInvalidated +=
        ref new DisplayPropertiesEventHandler(this, &DirectXPage::OnDisplayContentsInvalidated);

    m_eventToken = CompositionTarget::Rendering::add(ref new EventHandler<Object^>(this, &DirectXPage::OnRendering));

    m_timer = ref new BasicTimer();

    m_colors.push_back(Colors::Red);
    m_colors.push_back(Colors::Violet);
    m_colors.push_back(Colors::Indigo);
    m_colors.push_back(Colors::Blue);
    m_colors.push_back(Colors::Green);
    m_colors.push_back(Colors::Yellow);
    m_colors.push_back(Colors::Orange);

    m_colorCount = (int) m_colors.size();
}

void DirectXPage::OnWindowSizeChanged(CoreWindow^ sender, WindowSizeChangedEventArgs^ args)
{
    m_renderer->UpdateForWindowSizeChange();
}

void DirectXPage::OnLogicalDpiChanged(Object^ sender)
{
    m_renderer->SetDpi(DisplayProperties::LogicalDpi);
}

void DirectXPage::OnOrientationChanged(Object^ sender)
{
    m_renderer->UpdateForWindowSizeChange();
}

void DirectXPage::OnDisplayContentsInvalidated(Object^ sender)
{
    m_renderer->ValidateDevice();
}

void DirectXPage::OnRendering(Object^ sender, Object^ args)
{
    m_timer->Update();
    m_renderer->Update(m_timer->Total, m_timer->Delta);
    m_renderer->Render();
    m_renderer->Present();
}

void DirectXPage::OnPreviousColorPressed(Object^ sender, RoutedEventArgs^ args)
{
    m_colorIndex--;
    if (m_colorIndex <= 0)
        m_colorIndex = m_colorCount - 1;

    ChangeObjectColor(L"Teapot_Node", m_colorIndex);
}

void DirectXPage::OnNextColorPressed(Object^ sender, RoutedEventArgs^ args)
{
    m_colorIndex++;
    if (m_colorIndex >= m_colorCount)
        m_colorIndex = 0;

    ChangeObjectColor(L"Teapot_Node", m_colorIndex);
}

void DirectXPage::SaveInternalState(IPropertySet^ state)
{
}

void DirectXPage::LoadInternalState(IPropertySet^ state)
{
}

void DirectXPage::OnTapped(Platform::Object^ sender, Windows::UI::Xaml::Input::TappedRoutedEventArgs^ e)
{
    auto currentPoint = e->GetPosition(nullptr);
    String^ objName = m_renderer->OnHitObject((int)currentPoint.X, (int)currentPoint.Y);
    if (objName != nullptr)
    {
        m_renderer->ToggleHitEffect(objName);

        if ( objName->Equals(L"Cylinder_Node"))
        {
            this->HitCountCylinder->Text = (++m_hitCountCylinder).ToString();
        }
        else if ( objName->Equals(L"Cube_Node"))
        {
            this->HitCountCube->Text = (++m_hitCountCube).ToString();
        }
        else if ( objName->Equals(L"Sphere_Node"))
        {
            this->HitCountSphere->Text = (++m_hitCountSphere).ToString();
        }
        else if ( objName->Equals(L"Cone_Node"))
        {
            this->HitCountCone->Text = (++m_hitCountCone).ToString();
        }
        else if ( objName->Equals(L"Teapot_Node"))
        {
            this->HitCountTeapot->Text = (++m_hitCountTeapot).ToString();
        }
    }
}

void DirectXPage::ChangeObjectColor(String^ objectName, int colorIndex)
{
    auto color = m_colors[colorIndex];

    m_renderer->ChangeMaterialColor(objectName, color.R / 255.0f, color.G / 255.0f, color.B / 255.0f);
}
