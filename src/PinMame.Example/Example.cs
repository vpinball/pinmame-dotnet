// pinmame-dotnet
// Copyright (C) 1999-2021 PinMAME development team and contributors
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions 
// are met:
//
// 1. Redistributions of source code must retain the above copyright 
// notice, this list of conditions and the following disclaimer.
//
// 2. Redistributions in binary form must reproduce the above copyright
// notice, this list of conditions and the following disclaimer in the 
// documentation and/or other materials provided with the distribution.
//
// 3. Neither the name of the copyright holder nor the names of its
// contributors may be used to endorse or promote products derived
// from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS 
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT 
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS 
// FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE 
// COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, 
// INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
// BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
// LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE.

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
