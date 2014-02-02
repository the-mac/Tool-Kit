//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include <SpriteBatch.h>
#include <SpriteFont.h>

enum AlignType
{
    AlignNear,
    AlignCenter,
    AlignFar,
};

struct Alignment
{
    AlignType horizontal;
    AlignType vertical;
};

class ElementBase
{
public:
    virtual void Initialize() { }
    virtual void Update(float timeTotal, float timeDelta) { }
    virtual void Render() { }

    void SetAlignment(AlignType horizontal, AlignType vertical);
    virtual void SetContainer(const Windows::Foundation::Rect& container);
    void SetVisible(bool visible);
    virtual void SetPressed(bool pressed) { }

    Windows::Foundation::Rect GetBounds();

    bool IsVisible() const { return m_visible; }

    void SetSize(Windows::Foundation::Size size);

protected:
    ElementBase();

    virtual void CalculateSize() { }

    Alignment m_alignment;
    Windows::Foundation::Rect m_container;
    Windows::Foundation::Size m_size;
    bool m_visible;
};

typedef std::set<ElementBase*> ElementSet;

class TextElement : public ElementBase
{
public:
    TextElement();

    virtual void Initialize();
    virtual void Update(float timeTotal, float timeDelta);
    virtual void Render();

    void SetFont(__nullterminated PCWSTR filename);
    void SetFontScale(float scale);

    void SetTextColor(const DirectX::XMVECTOR& textColor);
    void SetTextOpacity(float textOpacity);
    void SetTextAlignment(AlignType textAlignment);

    void SetText(__nullterminated WCHAR* text);
    void SetText(Platform::String^ text);

    void FadeOut(float fadeOutTime);

protected:
    virtual void CalculateSize();

    Platform::String^ m_text;
    Windows::Foundation::Rect m_textExtents;

    bool m_isFadingOut;
    bool m_setVisibleAfterFadeOut;
    float m_fadeStartingOpacity;
    float m_fadeOutTime;
    float m_fadeOutTimeElapsed;

    DirectX::XMVECTOR m_textColor;
    float m_opacity;
    AlignType m_textAlignment;
    std::unique_ptr<DirectX::SpriteFont> m_font;
    float m_fontScale;
};

class ToggleCorner : public ElementBase
{
public:
    virtual void SetPressed(bool pressed) override;
    bool IsPressed() const { return m_pressed; }

protected:
    bool m_pressed;
};

class CountdownTimer : public TextElement
{
public:
    CountdownTimer();

    virtual void Initialize();
    virtual void Update(float timeTotal, float timeDelta);
    virtual void Render();

    void StartCountdown(int seconds);

    bool IsCountdownComplete() const;

protected:
    float   m_elapsedTime;
    int     m_secondsRemaining;
};

class StopwatchTimer : public TextElement
{
public:
    StopwatchTimer();

    virtual void Initialize();
    virtual void Update(float timeTotal, float timeDelta);
    virtual void Render();

    void Start();
    void Stop();
    void Reset();

    float GetElapsedTime() const { return m_elapsedTime; };
    void SetElapsedTime(float time) { m_elapsedTime = time; };

    void GetFormattedTime(WCHAR* buffer, int length) const;
    template <size_t _Len>
    void GetFormattedTime(WCHAR (&buffer)[_Len]) const { GetFormattedTime(buffer, _Len); }

    static void GetFormattedTime(WCHAR* buffer, int length, float time);
    template <size_t _Len>
    static void GetFormattedTime(WCHAR (&buffer)[_Len], float time) { GetFormattedTime(buffer, _Len, time); }

protected:
    bool    m_active;
    float   m_elapsedTime;
};

class TextButton : public TextElement
{
public:
    TextButton();

    virtual void Initialize();
    virtual void Update(float timeTotal, float timeDelta);
    virtual void Render();

    void SetPadding(Windows::Foundation::Size padding);
    void SetSelected(bool selected);

    bool GetSelected() const { return m_selected; }

    void SetPressed(bool pressed);
    bool IsPressed() const { return m_pressed; }

protected:
    virtual void CalculateSize();

    Windows::Foundation::Size m_padding;
    bool m_selected;
    bool m_pressed;
    
    Microsoft::WRL::ComPtr<ID3D11VertexShader> m_vertexShader;
    Microsoft::WRL::ComPtr<ID3D11PixelShader> m_pixelShader;
    Microsoft::WRL::ComPtr<ID3D11InputLayout> m_vertexLayout;
    Microsoft::WRL::ComPtr<ID3D11Buffer> m_vertexBuffer;
    Microsoft::WRL::ComPtr<ID3D11Buffer> m_indexBuffer;
    Microsoft::WRL::ComPtr<ID3D11DepthStencilState> m_stencilState;
};

struct HighScoreEntry
{
    Platform::String^ tag;
    float elapsedTime;
    bool wasJustAdded;
};

#define MAX_HIGH_SCORES 5
typedef std::vector<HighScoreEntry> HighScoreEntries;

class HighScoreTable : public TextElement
{
public:
    HighScoreTable();

    virtual void Initialize();
    virtual void Update(float timeTotal, float timeDelta);
    virtual void Render();

    void AddScoreToTable(HighScoreEntry& entry);
    HighScoreEntries GetEntries() { return m_entries; };
    void Reset();

protected:
    HighScoreEntries    m_entries;

    void UpdateText();
};

class UserInterface
{
public:
    static UserInterface& GetInstance() { return m_instance; }

    static ID3D11Device* GetD3DDevice() { return m_instance.m_d3dDevice.Get(); }
    static ID3D11DeviceContext* GetD3DContext() { return m_instance.m_d3dContext.Get(); }
    static DirectX::SpriteBatch* GetSpriteBatch() { return m_instance.m_spriteBatch.get(); }
    static float GetDpi() { return m_instance.m_dpi; }

    void Initialize(
        _In_ ID3D11Device* d3dDevice,
        _In_ ID3D11DeviceContext* d3dContext
        );

    void Uninitialize();

    void SetDpi(float dpi);

    void Update(float timeTotal, float timeDelta);
    void Render();

    void RegisterElement(ElementBase* element);
    void UnregisterElement(ElementBase* element);

    void HitTest(Windows::Foundation::Point point);

private:
    UserInterface() { }
    ~UserInterface() { }

    static UserInterface m_instance;

    Microsoft::WRL::ComPtr<ID3D11Device>            m_d3dDevice;
    Microsoft::WRL::ComPtr<ID3D11DeviceContext>     m_d3dContext;
    std::unique_ptr<DirectX::SpriteBatch>           m_spriteBatch;

    float m_dpi;

    ElementSet m_elements;
};