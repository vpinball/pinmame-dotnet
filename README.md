# PinMAME for .NET

[![CI status (x64 Linux, iOS, macOS and Windows)](https://github.com/VisualPinball/pinmame-dotnet/actions/workflows/build.yml/badge.svg?branch=master)](https://github.com/VisualPinball/pinmame-dotnet/actions) 
[![NuGet](https://img.shields.io/nuget/vpre/PinMame.svg)](https://www.nuget.org/packages/PinMame)

*Add PinMAME support to any .NET application*

This NuGet package provides a .NET binding for [PinMAME](https://github.com/vpinball/pinmame),
an emulator for solid state pinball machines. It uses the cross-platform [LibPinMAME](https://github.com/vpinball/pinmame/tree/master/src/libpinmame).

This package is automatically built and published when the main project, PinMAME, is updated.

## Supported Platforms

- .NET Core (.NETStandard 2.1 and higher on Windows, Linux and macOS)
- Mono

## Setup

The native wrapper is a different package and contains pre-compiled binaries of LibPinMAME.

|                       | NuGet Package                                                       |
|-----------------------|:-------------------------------------------------------------------:|
| **Windows 64-bit**    | [![PinMame.Native.win-x64-badge]][PinMame.Native.win-x64-nuget]     |
| **Windows 32-bit**    | [![PinMame.Native.win-x86-badge]][PinMame.Native.win-x86-nuget]     |
| **macOS x64**         | [![PinMame.Native.osx-x64-badge]][PinMame.Native.osx-x64-nuget]     |
| **macOS arm64**       | [![PinMame.Native.osx-arm64-badge]][PinMame.Native.osx-arm64-nuget] |
| **iOS arm64**         | [![PinMame.Native.ios-arm64-badge]][PinMame.Native.ios-arm64-nuget] |
| **Linux x64**         | [![PinMame.Native.linux-x64-badge]][PinMame.Native.linux-x64-nuget] |

[PinMame.Native.win-x64-badge]: https://img.shields.io/nuget/vpre/PinMame.Native.win-x64.svg
[PinMame.Native.win-x64-nuget]: https://www.nuget.org/packages/PinMame.Native.win-x64
[PinMame.Native.win-x86-badge]: https://img.shields.io/nuget/vpre/PinMame.Native.win-x86.svg
[PinMame.Native.win-x86-nuget]: https://www.nuget.org/packages/PinMame.Native.win-x86
[PinMame.Native.osx-x64-badge]: https://img.shields.io/nuget/vpre/PinMame.Native.osx-x64.svg
[PinMame.Native.osx-x64-nuget]: https://www.nuget.org/packages/PinMame.Native.osx-x64
[PinMame.Native.osx-arm64-badge]: https://img.shields.io/nuget/vpre/PinMame.Native.osx-arm64.svg
[PinMame.Native.osx-arm64-nuget]: https://www.nuget.org/packages/PinMame.Native.osx-arm64
[PinMame.Native.ios-arm64-badge]: https://img.shields.io/nuget/vpre/PinMame.Native.ios-arm64.svg
[PinMame.Native.ios-arm64-nuget]: https://www.nuget.org/packages/PinMame.Native.ios-arm64
[PinMame.Native.linux-x64-badge]: https://img.shields.io/nuget/vpre/PinMame.Native.linux-x64.svg
[PinMame.Native.linux-x64-nuget]: https://www.nuget.org/packages/PinMame.Native.linux-x64

To install this package with the native dependency of your current platform, run:

```
Install-Package PinMame
Install-Package PinMame-Native
```

## Usage

Create a `PinMame` instance, and then start a game. 

```cs
using PinMame;
var _pinMame = PinMame.PinMame.Instance();

_pinMame.StartGame("t2_l8");
```

You can add event handlers for:

- `OnGameStarted`
- `OnDisplayAvailable`
- `OnDisplayUpdated`
- `OnAudioAvailable`
- `OnAudioUpdated`
- `OnMechAvailable`
- `OnMechUpdated`
- `OnSolenoidUpdated`
- `OnConsoleDataUpdated`
- `OnGameEnded`
- `IsKeyPressed`

To process display data, in your `OnDisplayUpdated` callback:

```cs
void OnDisplayUpdated(int index, IntPtr framePtr, PinMameDisplayLayout displayLayout) 
{
    if (displayLayout.IsDmd)
    {
        // Handle DMD displays (framePtr is byte*)
    }
    else
    {
        // Handle Alphanumeric displays (framePtr is ushort*)
    }
};
```

To add or update a mech:

```cs
_pinMame.SetHandleMechanics(0);

PinMameMechConfig mechConfig = new PinMameMechConfig(
   (uint)(PinMameMechFlag.NonLinear | PinMameMechFlag.Reverse | PinMameMechFlag.OneSol),
   11,
   240,
   240,
   0,
   0,
   0);
mechConfig.AddSwitch(new PinMameMechSwitchConfig(33, 0, 5));
mechConfig.AddSwitch(new PinMameMechSwitchConfig(32, 98, 105));

_pinMame.SetMech(0, mechConfig);
```

To remove a mech: 

```cs
_pinMame.SetMech(0, null);
```

See the [example project](https://github.com/VisualPinball/pinmame-dotnet/blob/master/src/PinMame.Example/Example.cs) for more information.

## License

[MAME/BSD-3-Clause](LICENSE.txt)
