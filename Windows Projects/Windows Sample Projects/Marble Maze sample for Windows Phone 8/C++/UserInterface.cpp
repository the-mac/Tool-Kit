//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "UserInterface.h"
#include "DirectXSample.h"
#include "BasicLoader.h"

using namespace DirectX;
using namespace Windows::UI::Core;
using namespace Windows::Foundation;
using namespace Microsoft::WRL;
using namespace Windows::UI::ViewManagement;

struct VertexPositionColor
{
    XMFLOAT3 pos;
    XMFLOAT3 color;
};

static const unsigned int RectangleOutlineVertices = 12;
static const unsigned int RectangleOutlineIndices = 8 * 3;

#pragma region ElementBase

ElementBase::ElementBase():
    m_visible(false)
{
}

void ElementBase::SetAlignment(AlignType horizontal, AlignType vertical)
{
    m_alignment.horizontal = horizontal;
    m_alignment.vertical = vertical;
}

void ElementBase::SetContainer(const Rect& container)
{
    m_container = container;
}

void ElementBase::SetVisible(bool visible)
{
    m_visible = visible;
}

Rect ElementBase::GetBounds()
{
    CalculateSize();

    Rect bounds = Rect();

    switch (m_alignment.horizontal)
    {
    case AlignNear:
        bounds.X = m_container.X;
        bounds.Width = m_size.Width;
        break;

    case AlignCenter:
        bounds.X = m_container.X + (m_container.Width - m_size.Width) / 2.0f;
        bounds.Width = m_size.Width;
        break;

    case AlignFar:
        bounds.X = m_container.Right - m_size.Width;
        bounds.Width = m_size.Width;
        break;
    }

    switch (m_alignment.vertical)
    {
    case AlignNear:
        bounds.Y = m_container.Y;
        bounds.Height = m_size.Height;
        break;

    case AlignCenter:
        bounds.Y = m_container.Y + (m_container.Height - m_size.Height) / 2.0f;
        bounds.Height = m_size.Height;
        break;

    case AlignFar:
        bounds.Y = m_container.Bottom - m_size.Height;
        bounds.Height = m_size.Height;
        break;
    }

    return bounds;
}

#pragma endregion

#pragma region TextElement

TextElement::TextElement():
    m_isFadingOut(false),
    m_textColor(DirectX::g_XMOne),
    m_opacity(1.0f),
    m_textAlignment(AlignNear),
    m_fontScale(1.0f)
{
}

void TextElement::Initialize()
{
}

void TextElement::Update(float timeTotal, float timeDelta)
{
    if (m_isFadingOut)
    {
        m_fadeOutTimeElapsed += timeDelta;

        float delta = min(1.0f, m_fadeOutTimeElapsed / m_fadeOutTime);
        SetTextOpacity((1.0f - delta) * m_fadeStartingOpacity);

        if (m_fadeOutTimeElapsed >= m_fadeOutTime)
        {
            m_isFadingOut = false;
            SetVisible(false);
        }
    }
}

void TextElement::Render()
{
    SpriteBatch* spriteBatch = UserInterface::GetSpriteBatch();
    const float scale = UserInterface::GetDpi() / 96.0f; 

    Rect bounds = GetBounds();
    XMFLOAT2 origin(
        bounds.Left - m_textExtents.Left,
        bounds.Top - m_textExtents.Top
        );

    unsigned int lineStart = 0;
    const wchar_t* text = m_text->Data();

    for (unsigned int i = 0, len = m_text->Length(); i < len; ++i)
    {
        if (text[i] == '\n' || i + 1 == len)
        {
            Platform::String^ line = ref new Platform::String(text + lineStart, i - lineStart + 1);
            XMVECTOR size = m_font->MeasureString(line->Data());

            XMFLOAT2 offset = origin;

            // Align this line.
            switch (m_textAlignment)
            {
            case AlignNear:
                break;

            case AlignCenter:
                offset.x += (m_size.Width - XMVectorGetX(size) * m_fontScale) / 2.0f;
                break;

            case AlignFar:
                offset.x += (m_size.Width - XMVectorGetX(size) * m_fontScale);
                break;
            }

            // Draw line.
            m_font->DrawString(
                spriteBatch,
                line->Data(),
                XMFLOAT2((offset.x + 4.0f) * scale, (offset.y + 4.0f) * scale),
                Colors::Black * m_opacity * 0.5f, // Premultiply opacity.
                0.0f, // Rotation.
                XMFLOAT2(0.0f, 0.0f), // Origin.
                scale * m_fontScale
                );

            m_font->DrawString(
                spriteBatch,
                line->Data(),
                XMFLOAT2(offset.x * scale, offset.y * scale),
                m_textColor * m_opacity, // Premultiply opacity.
                0.0f, // Rotation.
                XMFLOAT2(0.0f, 0.0f), // Origin.
                scale * m_fontScale
                );

            origin.y += m_font->GetLineSpacing() * m_fontScale;
            lineStart = i + 1;
        }
    }

}

void TextElement::SetFont(__nullterminated PCWSTR filename)
{
    m_font = std::unique_ptr<SpriteFont>(new SpriteFont(UserInterface::GetD3DDevice(), filename));
}

void TextElement::SetFontScale(float scale)
{
    m_fontScale = scale;
}


void TextElement::SetTextColor(const XMVECTOR& textColor)
{
    m_textColor = textColor;
}

void TextElement::SetTextOpacity(float textOpacity)
{
    m_opacity = textOpacity;
}

void TextElement::SetTextAlignment(AlignType textAlignment)
{
    m_textAlignment = textAlignment;
}

void TextElement::SetText(__nullterminated WCHAR* text)
{
    SetText(ref new Platform::String(text));
}

void TextElement::SetText(Platform::String^ text)
{
    if (!m_text->Equals(text))
    {
        m_text = text;
    }
}

void TextElement::FadeOut(float fadeOutTime)
{
    m_fadeStartingOpacity = m_opacity;
    m_fadeOutTime = fadeOutTime;
    m_fadeOutTimeElapsed = 0.0f;
    m_isFadingOut = true;
    SetVisible(true);
}

void TextElement::CalculateSize()
{
    XMVECTOR size = m_font->MeasureString(m_text->Data());

    m_textExtents = Rect(
        0.0f,
        0.0f,
        XMVectorGetX(size) * m_fontScale,
        XMVectorGetY(size) * m_fontScale
        );

    m_size = Size(
        m_textExtents.Width,
        m_textExtents.Height
        );
}

#pragma endregion

#pragma region CountdownTimer

CountdownTimer::CountdownTimer():
    m_elapsedTime(0.0f),
    m_secondsRemaining(0)
{
}

void CountdownTimer::Initialize()
{
    TextElement::Initialize();
}

void CountdownTimer::Update(float timeTotal, float timeDelta)
{
    if (m_secondsRemaining > 0)
    {
        m_elapsedTime += timeDelta;
        if (m_elapsedTime >= 1.0f)
        {
            m_elapsedTime -= 1.0f;

            if (--m_secondsRemaining > 0)
            {
                WCHAR buffer[4];
                swprintf_s(buffer, L"%2d", m_secondsRemaining);
                SetText(buffer);
                SetTextOpacity(1.0f);
                FadeOut(1.0f);
            }
            else
            {
                SetText(L"Go!");
                SetTextOpacity(1.0f);
                FadeOut(1.0f);
            }
        }
    }

    TextElement::Update(timeTotal, timeDelta);
}

void CountdownTimer::Render()
{
    TextElement::Render();
}

void CountdownTimer::StartCountdown(int seconds)
{
    m_secondsRemaining = seconds;
    m_elapsedTime = 0.0f;

    WCHAR buffer[4];
    swprintf_s(buffer, L"%2d", m_secondsRemaining);
    SetText(buffer);
    SetTextOpacity(1.0f);
    FadeOut(1.0f);
}

bool CountdownTimer::IsCountdownComplete() const
{
    return (m_secondsRemaining == 0);
}

#pragma endregion

#pragma region StopwatchTimer

StopwatchTimer::StopwatchTimer():
    m_active(false),
    m_elapsedTime(0.0f)
{
}

void StopwatchTimer::Initialize()
{
    TextElement::Initialize();
}

void StopwatchTimer::Update(float timeTotal, float timeDelta)
{
    if (m_active)
    {
        m_elapsedTime += timeDelta;

        WCHAR buffer[16];
        GetFormattedTime(buffer);
        SetText(buffer);
    }

    TextElement::Update(timeTotal, timeDelta);
}

void StopwatchTimer::Render()
{
    TextElement::Render();
}

void StopwatchTimer::Start()
{
    m_active = true;
}

void StopwatchTimer::Stop()
{
    m_active = false;
}

void StopwatchTimer::Reset()
{
    m_elapsedTime = 0.0f;
}

void StopwatchTimer::GetFormattedTime(WCHAR* buffer, int length) const
{
    GetFormattedTime(buffer, length, m_elapsedTime);
}

void StopwatchTimer::GetFormattedTime(WCHAR* buffer, int length, float time)
{
    int partial = (int)floor(fmodf(time * 10.0f, 10.0f));
    int seconds = (int)floor(fmodf(time, 60.0f));
    int minutes = (int)floor(time / 60.0f);
    swprintf_s(buffer, length, L"%02d:%02d.%01d", minutes, seconds, partial);
}

#pragma endregion

#pragma region TextButton

TextButton::TextButton():
    m_selected(false)
{
}

void TextButton::Initialize()
{
    ID3D11Device* device = UserInterface::GetD3DDevice();

    // Initialize resources.
    D3D11_DEPTH_STENCIL_DESC DSDesc;
    ZeroMemory(&DSDesc, sizeof(D3D11_DEPTH_STENCIL_DESC));
    DSDesc.DepthEnable = FALSE;
    DSDesc.DepthFunc = D3D11_COMPARISON_ALWAYS;
    DSDesc.DepthWriteMask = D3D11_DEPTH_WRITE_MASK_ZERO;
    DX::ThrowIfFailed(
        device->CreateDepthStencilState(&DSDesc, &m_stencilState)
        );

    BasicLoader^ loader = ref new BasicLoader(device);

    // Define vertex data format.
    D3D11_INPUT_ELEMENT_DESC vertexDesc[] = 
    {
        { "POSITION", 0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 0,  D3D11_INPUT_PER_VERTEX_DATA, 0 },
        { "COLOR",    0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 12, D3D11_INPUT_PER_VERTEX_DATA, 0 }
    };

    loader->LoadShader(L"UntexturedVertexShader.cso", vertexDesc, ARRAYSIZE(vertexDesc), &m_vertexShader, &m_vertexLayout);
    loader->LoadShader(L"UntexturedPixelShader.cso", &m_pixelShader);

    CD3D11_BUFFER_DESC vertexBufferDesc(12 * sizeof(VertexPositionColor), D3D11_BIND_VERTEX_BUFFER, D3D11_USAGE_DYNAMIC, D3D11_CPU_ACCESS_WRITE);

    // Create vertex/index buffers.
    DX::ThrowIfFailed(
        device->CreateBuffer(
            &vertexBufferDesc,
            nullptr,
            &m_vertexBuffer
            )
        );
    
    /*
     * Draw the outline of a rectangle with twelve vertices and
     * eight triangles.
     *
     *  0 +-----------------------+ 1
     *    |                  /    |
     *    |           /           |
     *    |    /  8       9       |
     *  7 +-------+-------+-------+ 2
     *    |     / |       |     / |
     *    |   /   |       |   /   |
     *    | /     |       | /     |
     *  6 +-------+-------+-------+ 3
     *    |       11      10 /    |
     *    |           /           |
     *    |    /                  |
     *  5 +-----------------------+ 4
    **/

    const unsigned short indices[RectangleOutlineIndices] = {
        0, 7, 1, // Top.
        1, 7, 2,
        2, 9, 10, // Right.
        2, 10, 3,
        3, 6, 5, // Bottom.
        3, 5, 4,
        6, 8, 7, // Left.
        6, 11, 8
    };

    D3D11_SUBRESOURCE_DATA indexBufferData = {0};
    indexBufferData.pSysMem = indices;
    CD3D11_BUFFER_DESC indexBufferDesc(sizeof(indices), D3D11_BIND_INDEX_BUFFER);
    DX::ThrowIfFailed(
        device->CreateBuffer(
            &indexBufferDesc,
            &indexBufferData,
            &m_indexBuffer
            )
        );

    TextElement::Initialize();
}

void TextButton::Update(float timeTotal, float timeDelta)
{
    TextElement::Update(timeTotal, timeDelta);
}

void TextButton::Render()
{
    ID3D11DeviceContext* d3dContext = UserInterface::GetD3DContext();

    if (m_selected)
    {
        // Look up the current viewport.
        D3D11_VIEWPORT viewport;
        UINT viewportCount = 1;

        d3dContext->RSGetViewports(&viewportCount, &viewport);

        // Constants.
        const float screenScale = UserInterface::GetDpi() / 96.0f;
        const XMFLOAT2 pixelRatio(2.0f / viewport.Width * screenScale, 2.0f / viewport.Height * screenScale);

        const float outlineHalfWidth = 2.0f * screenScale;
        const XMFLOAT2 outline(outlineHalfWidth * pixelRatio.x, outlineHalfWidth * pixelRatio.y);

        Rect bounds = GetBounds();
        bounds.X = bounds.X * pixelRatio.x;
        bounds.Y = bounds.Y * pixelRatio.y;
        bounds.Width *= pixelRatio.x;
        bounds.Height *= pixelRatio.y;

        const XMFLOAT2 topLeft(bounds.X - 1.0f, 1.0f - bounds.Y - bounds.Height);
        const XMFLOAT2 bottomRight(topLeft.x + bounds.Width, 1.0f - bounds.Y);

        d3dContext->OMSetDepthStencilState(m_stencilState.Get(), 0);

        d3dContext->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);

        d3dContext->IASetInputLayout(m_vertexLayout.Get());

        d3dContext->VSSetShader(
            m_vertexShader.Get(),
            nullptr,
            0
            );

        d3dContext->PSSetShader(
            m_pixelShader.Get(),
            nullptr,
            0
            );

        D3D11_MAPPED_SUBRESOURCE MappedResource;
        d3dContext->Map(m_vertexBuffer.Get(), 0, D3D11_MAP_WRITE_DISCARD, 0, &MappedResource);
        {
            VertexPositionColor* vertices = reinterpret_cast<VertexPositionColor*>(MappedResource.pData);

            // Set color.
            XMFLOAT3 color;
            XMStoreFloat3(&color, m_textColor); // Throws away opacity.
            for (int i = 0; i < RectangleOutlineVertices; ++i)
            {
                vertices[i].color = color;
            }

            const float z = 1.0f;

            // Set positions.
            vertices[0].pos = XMFLOAT3(topLeft.x - outline.x, topLeft.y - outline.y, z); // Outer top left.
            vertices[1].pos = XMFLOAT3(bottomRight.x + outline.x, topLeft.y - outline.y, z); // Outer top right.
            vertices[2].pos = XMFLOAT3(bottomRight.x + outline.x, topLeft.y + outline.y, z);
            vertices[3].pos = XMFLOAT3(bottomRight.x + outline.x, bottomRight.y - outline.y, z);
            vertices[4].pos = XMFLOAT3(bottomRight.x + outline.x, bottomRight.y + outline.y, z); // Outer bottom right.
            vertices[5].pos = XMFLOAT3(topLeft.x - outline.x, bottomRight.y + outline.y, z); // Outer bottom left.
            vertices[6].pos = XMFLOAT3(topLeft.x - outline.x, bottomRight.y - outline.y, z);
            vertices[7].pos = XMFLOAT3(topLeft.x - outline.x, topLeft.y + outline.y, z);
            vertices[8].pos = XMFLOAT3(topLeft.x + outline.x, topLeft.y + outline.y, z); // Inner top left.
            vertices[9].pos = XMFLOAT3(bottomRight.x - outline.x, topLeft.y + outline.y, z); // Inner top right.
            vertices[10].pos = XMFLOAT3(bottomRight.x - outline.x, bottomRight.y - outline.y, z); // Inner bottom right.
            vertices[11].pos = XMFLOAT3(topLeft.x + outline.x, bottomRight.y - outline.y, z); // Inner bottom left.
        }
        d3dContext->Unmap(m_vertexBuffer.Get(), 0);

        UINT stride = sizeof(VertexPositionColor);
        UINT offset = 0;
        d3dContext->IASetVertexBuffers(
            0,
            1,
            m_vertexBuffer.GetAddressOf(),
            &stride,
            &offset
            );
    
        d3dContext->IASetIndexBuffer(
            m_indexBuffer.Get(),
            DXGI_FORMAT_R16_UINT,
            0
            );

        d3dContext->DrawIndexed(
            RectangleOutlineIndices,
            0,
            0
            );
    }

    TextElement::Render();
}

void TextButton::SetPadding(Size padding)
{
    m_padding = padding;
}

void TextButton::SetSelected(bool selected)
{
    m_selected = selected;
}

void TextButton::SetPressed(bool pressed)
{
    m_pressed = pressed;
}

void TextButton::CalculateSize()
{
    TextElement::CalculateSize();
    m_textExtents.X -= m_padding.Width;
    m_textExtents.Y -= m_padding.Height;
    m_size.Width += m_padding.Width * 2;
    m_size.Height += m_padding.Height * 2;
}

#pragma endregion

#pragma region HighScoreTable

HighScoreTable::HighScoreTable()
{
}

void HighScoreTable::Initialize()
{
    TextElement::Initialize();
    UpdateText();
}

void HighScoreTable::Reset()
{
    m_entries.clear();
    UpdateText();
}

void HighScoreTable::Update(float timeTotal, float timeDelta)
{
    TextElement::Update(timeTotal, timeDelta);
}

void HighScoreTable::Render()
{
    TextElement::Render();
}

void HighScoreTable::AddScoreToTable(HighScoreEntry& entry)
{
    for (auto iter = m_entries.begin(); iter != m_entries.end(); ++iter)
    {
        iter->wasJustAdded = false;
    }

    entry.wasJustAdded = false;

    for (auto iter = m_entries.begin(); iter != m_entries.end(); ++iter)
    {
        if (entry.elapsedTime < iter->elapsedTime)
        {
            m_entries.insert(iter, entry);
            while (m_entries.size() > MAX_HIGH_SCORES)
                m_entries.pop_back();

            entry.wasJustAdded = true;
            UpdateText();
            return;
        }
    }

    if (m_entries.size() < MAX_HIGH_SCORES)
    {
        m_entries.push_back(entry);
        UpdateText();
        entry.wasJustAdded = true;
    }
}

void HighScoreTable::UpdateText()
{
    WCHAR formattedTime[32];
    WCHAR lines[1024] = { 0, };
    WCHAR buffer[128];

    swprintf_s(lines, L"High Scores:");
    for (unsigned int i = 0; i < MAX_HIGH_SCORES; ++i)
    {
        if (i < m_entries.size())
        {
            StopwatchTimer::GetFormattedTime(formattedTime, m_entries[i].elapsedTime);
            swprintf_s(
                buffer,
                (m_entries[i].wasJustAdded ? L"\n>> %s    %s <<" : L"\n%s    %s"),
                m_entries[i].tag->Data(),
                formattedTime
                );
            wcscat_s(lines, buffer);
        }
        else
        {
            wcscat_s(lines, L"\n-");
        }
    }

    SetText(lines);
}

#pragma endregion

#pragma region UserInterface

UserInterface UserInterface::m_instance;

void UserInterface::Initialize(
    _In_ ID3D11Device*         d3dDevice,
    _In_ ID3D11DeviceContext*  d3dContext
    )
{
    m_d3dDevice = d3dDevice;
    m_d3dContext = d3dContext;

    m_spriteBatch = std::unique_ptr<SpriteBatch>(new SpriteBatch(d3dContext));
    m_dpi = 96.0f;
}

void UserInterface::SetDpi(float dpi)
{
    m_dpi = dpi;
}

void UserInterface::Update(float timeTotal, float timeDelta)
{
    for (auto iter = m_elements.begin(); iter != m_elements.end(); ++iter)
    {
        (*iter)->Update(timeTotal, timeDelta);
    }
}

void UserInterface::Render()
{
    // Save previous state.
    ComPtr<ID3D11DepthStencilState> oldStencilState;
    UINT oldStencilRef = 0;
    m_d3dContext->OMGetDepthStencilState(&oldStencilState, &oldStencilRef);
    m_spriteBatch->Begin();

    for (auto iter = m_elements.begin(); iter != m_elements.end(); ++iter)
    {
        if ((*iter)->IsVisible())
            (*iter)->Render();
    }

    // Restore state.
    m_spriteBatch->End();
    m_d3dContext->OMSetDepthStencilState(oldStencilState.Get(), oldStencilRef);
}

void UserInterface::RegisterElement(ElementBase* element)
{
    m_elements.insert(element);
}

void UserInterface::UnregisterElement(ElementBase* element)
{
    auto iter = m_elements.find(element);
    if (iter != m_elements.end())
    {
        m_elements.erase(iter);
    }
}

inline bool PointInRect(Point point, Rect rect)
{
    if ((point.X < rect.Left) ||
        (point.X > rect.Right) ||
        (point.Y < rect.Top) ||
        (point.Y > rect.Bottom))
    {
        return false;
    }

    return true;
}

void UserInterface::HitTest(Point point)
{
    for (auto iter = m_elements.begin(); iter != m_elements.end(); ++iter)
    {
        if (!(*iter)->IsVisible())
            continue;

        Rect bounds = (*iter)->GetBounds();
        (*iter)->SetPressed(PointInRect(point, bounds));
    }
}

#pragma endregion