//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include <DirectXColors.h>
#include <queue>
#include <stack>

class Profiler
{
public:
    struct Identifier
    {
        Platform::String^ name;
        DirectX::XMFLOAT3 color;

        Identifier(PCWSTR _name, const DirectX::XMVECTORF32& _color)
                : name(ref new Platform::String(_name)),
                  color(_color) {
            DirectX::XMStoreFloat3(&color, _color); // Throws away alpha.
        }

        bool operator==(const Identifier& other) const
        {
            return (name == other.name &&
                color.x == other.color.x &&
                color.y == other.color.y &&
                color.z == other.color.z);
        }
    };
    
    class Listener
    {
    public:
        virtual void OnProfileFrameStart() = 0;
        virtual void OnProfileRegion(const Identifier& identifier, double duration, double endTime) = 0;
    };

#ifdef ENABLE_PROFILER
    struct SampleStart
    {
        ULONGLONG startTime;
        Identifier ident;

        SampleStart(ULONGLONG _startTime, const Identifier& _ident)
                : startTime(_startTime),
                  ident(_ident)
        {
        }
    };
    
    // Completed samples.
    struct Sample
    {
        ULONGLONG startTime;
        ULONGLONG endTime;
        int depth;
        Identifier ident;

        Sample(ULONGLONG _startTime, ULONGLONG _endTime, int _depth, const Identifier& _ident)
                : startTime(_startTime),
                  endTime(_endTime),
                  depth(_depth),
                  ident(_ident)
        {
        }

        bool operator<(const Sample& other) const
        {
            return depth > other.depth; // Priority is reverse of depth.
        }
    };

private:
    static std::stack<SampleStart> s_activeSamples;
    static std::priority_queue<Sample> s_samples;
    static Listener* s_listener;
    static bool s_visible;

#endif // ENABLE_PROFILER

public:
    static void Begin(PCWSTR name, const DirectX::XMVECTORF32& color);
    static void End();

    static void SetVisible(bool visible);
    static void SetListener(Listener* listener);
    
    static void Initialize();
    static void Uninitialize();

    static void FrameStart();
    static void Render(ID3D11Device* device, ID3D11DeviceContext* context);
};

/**
 * Reports profiler information for the current scope.
**/
class ProfileScope
{
public:
    ProfileScope(PCWSTR name, const DirectX::XMVECTORF32& color)
    {
        Profiler::Begin(name, color);
    }
    
    ~ProfileScope()
    {
        Profiler::End();
    }
};
