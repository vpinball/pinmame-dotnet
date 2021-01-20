using System;
using System.Collections.Generic;
using System.Threading;

namespace PinMame
{
	class Example
	{
		static Dictionary<byte, string> DOTS = new Dictionary<byte, string>() {
			{ 0x00, " " },
			{ 0x14, "░" },
			{ 0x21, "▒" },
			{ 0x43, "▓" },
			{ 0x64, "▓" }
		};

		static void Main(string[] args)
		{
			var _pinMame = PinMame.Instance();

			_pinMame.StartGame("mm_109c", showConsole: true);

			while (true)
			{
				if (_pinMame.IsRunning)
				{
					if (_pinMame.NeedsDmdUpdate())
					{
						var dimensions = _pinMame.GetDmdDimensions();
						var buffer = _pinMame.GetDmdPixels();

						var dmd = "";

						for (var y = 0; y < dimensions.Height; y++)
						{
							for (var x = 0; x < dimensions.Width; x++)
							{
								var pixel = y * dimensions.Width + x;
								var value = buffer[pixel];

								dmd += DOTS[value];
							}

							dmd += "\n";
						}

						Console.SetCursorPosition(0, 0);
						Console.WriteLine(dmd);
					}
				}

				Thread.Sleep(100);
			}
		}
	}
}
