# PinMAME for .NET

*Add PinMAME support to any .NET application*

## Setup

Add the `PinMame` package references to your project file:

```xml
  <ItemGroup>
    <PackageReference Include="PinMame" Version="0.1.0-preview.26" />
    <PackageReference Include="PinMame.Native" Version="3.4.0-preview.200" />
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

You can add event handlers for `OnGameStarted`, `OnDisplayAvailable`, `OnDisplayUpdated`, `OnSolenoidUpdated`, and `OnGameEnded`.

To process display data, in your `OnDisplayUpdated` callback:

```
	void OnDisplayUpdated(object sender, EventArgs e, int index, IntPtr framePtr, PinMameDisplayLayout displayLayout) 
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

See the [example project](https://github.com/VisualPinball/pinmame-dotnet/blob/master/src/PinMame.Example/Example.cs) for more information.


## License

[MAME/BSD-3-Clause](LICENSE.txt)
