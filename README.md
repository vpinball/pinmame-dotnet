# PinMAME for .NET

*Add PinMAME support to any .NET application*

## Setup

Add the `PinMame` package references to your project file:

```xml
  <ItemGroup>
    <PackageReference Include="PinMame" Version="0.1.0-preview.16" />
    <PackageReference Include="PinMame.Native" Version="3.4.0-preview.184" />
  </ItemGroup>
```

## Usage

Create a `PinMame` instance, and then start a game. 

```c#
using PinMame;
.
.
.
	var _pinMame = PinMame.PinMame.Instance();

	_pinMame.StartGame("mm_109c");
```

You can add event handlers for `OnGameStarted`, `OnRegisterDisplay`, `OnSolenoid`, and `OnGameEnded`.

To process display data, in your event loop call `GetDisplays`: 

```
	_pinMame.GetDisplays((index, displayLayout, frame) => {
		if (displayLayout.type == PinMameDisplayType.DMD)
		{
			// Handle DMD displays
		}
		else
		{
			// Handle Alphanumeric displays
		}
	});
```

See the [example project](https://github.com/VisualPinball/pinmame-dotnet/blob/master/src/PinMame.Example/Example.cs) for more information.


## License

[MAME/BSD-3-Clause](LICENSE.txt)
