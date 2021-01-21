# PinMAME for .NET

*Add PinMAME support to any .NET application*

## Setup

Add the `PinMame` package references to your project file:

```
  <ItemGroup>
    <PackageReference Include="PinMame" Version="0.0.3" />
    <PackageReference Include="PinMame.Native" Version="0.0.3" />
  </ItemGroup>
```

## Usage

Create a `PinMame` instance, and then start a game. 

```
using PinMame;
.
.
.
	var _pinMame = PinMame.PinMame.Instance();

	_pinMame.StartGame("mm_109c", showConsole: false);
```

You will need to build an event loop and process DMD Updates. 

See the [example project](https://github.com/VisualPinball/pinmame-dotnet/blob/master/src/PinMame.Example/Example.cs) for more information.


## License

[BSD-3-Clause](LICENSE.txt)
