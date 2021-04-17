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
		static PinMame _pinMame;
		static bool _isRunning = false;

		static void DumpGames()
		{
			foreach (var game in PinMame.GetGames())
			{
				Console.WriteLine($"PARENT: name={game.name}, description={game.description}, year={game.year}, manufacturer={game.manufacturer}");

				foreach (var clone in game.clones)
				{
					Console.WriteLine($"  CLONE: name={clone.name}, description={clone.description}, year={clone.year}, manufacturer={clone.manufacturer}");
				}
			}
		}

		static void DumpDmd(int index, IntPtr framePtr, PinMameDisplayLayout displayLayout)
		{
			Dictionary<byte, string> map;

			if (displayLayout.depth == 2)
			{
				// 20, 33, 67, 100

				map = new Dictionary<byte, string>() {
					{ 0x14, "░" },
					{ 0x21, "▒" },
					{ 0x43, "▓" },
					{ 0x64, "▓" }
				};
			}
			else
			{
				if ((_pinMame.GetHardwareGen() & (PinMameHardwareGen.SAM | PinMameHardwareGen.SPA)) > 0)
				{
					// SAM: 0, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 90, 100

					map = new Dictionary<byte, string>() {
						{ 0x00, "░" },
						{ 0x14, "░" },
						{ 0x19, "░" },
						{ 0x1E, "░" },

						{ 0x23, "▒" },
						{ 0x28, "▒" },
						{ 0x2D, "▒" },
						{ 0x32, "▒" },

						{ 0x37, "▓" },
						{ 0x3C, "▓" },
						{ 0x41, "▓" },
						{ 0x46, "▓" },

						{ 0x4B, "▓" },
						{ 0x50, "▓" },
						{ 0x5A, "▓" },
						{ 0x64, "▓" }
					};
				}
				else
				{
					// GTS3: 0, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100

					map = new Dictionary<byte, string>() {
						{ 0x00, "░" },
						{ 0x1E, "░" },
						{ 0x23, "░" },
						{ 0x28, "░" },

						{ 0x2D, "▒" },
						{ 0x32, "▒" },
						{ 0x37, "▒" },
						{ 0x3C, "▒" },

						{ 0x41, "▓" },
						{ 0x46, "▓" },
						{ 0x4B, "▓" },
						{ 0x50, "▓" },

						{ 0x55, "▓" },
						{ 0x5A, "▓" },
						{ 0x5F, "▓" },
						{ 0x64, "▓" }
					};
				}
			}

			unsafe
			{
				byte* ptr = (byte*)framePtr;

				for (var y = 0; y < displayLayout.height; y++)
				{
					var dmd = "";

					for (var x = 0; x < displayLayout.width; x++)
					{
						dmd += map[ptr[y * displayLayout.width + x]];
					}

					Console.SetCursorPosition(0, y);
					Console.Write(dmd);
				}
			}
		}

		static void DumpAlpha(int index, IntPtr framePtr, PinMameDisplayLayout displayLayout)
		{
			unsafe
			{
				ushort* ptr = (ushort*)framePtr;

				for (var position = 0; position < displayLayout.length; position++)
				{
					var segments = new string[8] {
						" AAAAA   " ,
						"FI J KB  " ,
						"F IJK B  " ,
						" GG LL   " ,
						"E ONM C  " ,
						"EO N MC P" ,
						" DDDDD  H" ,
						"       H " 
					};

					for (var row = 0; row < 8; row++)
					{
						for (var column = 0; column < segments[row].Length; column++)
						{
							for (var bit = 0; bit < 16; bit++)
							{
								segments[row] = segments[row].Replace("" + (char)('A' + bit), (ptr[position] & (1 << bit)) > 0 ? "▓" : " ");
							}
						}

						Console.SetCursorPosition(position * 10, index * 8 + row);
						Console.Write(segments[row] + " ");
					}

				}
			}
		}

		static void OnGameStarted(object sender, EventArgs e)
		{
			Console.WriteLine("OnGameStarted");
		}

		static void OnDisplayUpdated(object sender, EventArgs e, int index, IntPtr framePtr, PinMameDisplayLayout displayLayout)
		{
			Console.WriteLine("OnDisplayUpdated: index={0}, type={1}, top={2}, left={3}, length={4}, height={5}, width={6}, depth={7}",
				index,
				displayLayout.type,
				displayLayout.top,
				displayLayout.left,
				displayLayout.length,
				displayLayout.height,
				displayLayout.width,
				displayLayout.depth);

			if (displayLayout.IsDmd)
			{
				DumpDmd(index, framePtr, displayLayout);
			}
			else
			{
				DumpAlpha(index, framePtr, displayLayout);
			}
		}

		static void OnSolenoidUpdated(object sender, EventArgs e, int solenoid, bool isActive)
		{
			Console.WriteLine("OnSolenoidUpdated: solenoid={0}, isActive={1}",
				solenoid,
				isActive);
		}

		static void OnGameEnded(object sender, EventArgs e)
		{
			Console.WriteLine("OnGameEnded");

			_isRunning = false;
		}

		static void Main(string[] args)
		{
			DumpGames();

			_pinMame = PinMame.Instance();

			_pinMame.OnGameStarted += OnGameStarted;
			_pinMame.OnDisplayUpdated += OnDisplayUpdated;
			_pinMame.OnSolenoidUpdated += OnSolenoidUpdated;
			_pinMame.OnGameEnded += OnGameEnded;

			//_pinMame.StartGame("flashgdn");
			//_pinMame.StartGame("mm_109c");
			_pinMame.StartGame("fh_906h");

			_isRunning = true;

			while (_isRunning)
			{
				Thread.Sleep(100);
			}
		}
	}
}
