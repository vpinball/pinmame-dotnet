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
using NLog;
using NLog.Config;
using NLog.Targets;

namespace PinMame
{
	class Example
	{
		static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		static PinMame _pinMame;
		static Dictionary<byte, string> _dmdMap;
		static bool _isRunning = false;

		static void DumpGames()
		{
			Logger.Info($"DumpGames");

			foreach (var game in _pinMame.GetGames())
			{
				Logger.Info($"PARENT: {game}");

				foreach (var clone in game.clones)
				{
					Logger.Info($"  CLONE: {clone}");
				}
			}
		}

		static void DumpFoundGames()
		{
			Logger.Info($"DumpFoundGames");

			foreach (var game in _pinMame.GetFoundGames())
			{
				Logger.Info($"FOUND: {game}");
			}
		}

		static unsafe void DumpDmd(int index, IntPtr framePtr, PinMameDisplayLayout displayLayout)
		{
			byte* ptr = (byte*)framePtr;

			for (var y = 0; y < displayLayout.height; y++)
			{
				var dmd = "";

				for (var x = 0; x < displayLayout.width; x++)
				{
					dmd += _dmdMap[ptr[y * displayLayout.width + x]];
				}

				Console.SetCursorPosition(0, y);
				Console.Write(dmd);
			}
		}

		static unsafe void DumpAlpha(int index, IntPtr framePtr, PinMameDisplayLayout displayLayout)
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

		static void OnGameStarted(object sender, EventArgs e)
		{
			Logger.Info($"OnGameStarted");
		}

		static void OnDisplayAvailable(object sender, EventArgs e, int index, int displayCount, PinMameDisplayLayout displayLayout)
		{
			Logger.Info($"OnDisplayAvailable: index={index}, displayCount={displayCount}, displayLayout={displayLayout}");

			if (displayLayout.IsDmd)
			{
				if (displayLayout.depth == 2)
				{
					_dmdMap = new Dictionary<byte, string>() {
						{ displayLayout.levels[0], "░" },
						{ displayLayout.levels[1], "▒" },
						{ displayLayout.levels[2], "▓" },
						{ displayLayout.levels[3], "▓" }
					};
				}
				else
				{
					_dmdMap = new Dictionary<byte, string>()
					{
						{ displayLayout.levels[0], "░" },
						{ displayLayout.levels[1], "░" },
						{ displayLayout.levels[2], "░" },
						{ displayLayout.levels[3], "░" },

						{ displayLayout.levels[4], "▒" },
						{ displayLayout.levels[5], "▒" },
						{ displayLayout.levels[6], "▒" },
						{ displayLayout.levels[7], "▒" },

						{ displayLayout.levels[8], "▓" },
						{ displayLayout.levels[9], "▓" },
						{ displayLayout.levels[10], "▓" },
						{ displayLayout.levels[11], "▓" },

						{ displayLayout.levels[12], "▓" },
						{ displayLayout.levels[13], "▓" },
						{ displayLayout.levels[14], "▓" },
						{ displayLayout.levels[15], "▓" }
					};
				}
			}
		}

		static void OnDisplayUpdated(object sender, EventArgs e, int index, IntPtr framePtr, PinMameDisplayLayout displayLayout)
		{
			Logger.Info($"OnDisplayUpdated: index={index}, displayLayout={displayLayout}");

			if (displayLayout.IsDmd && _dmdMap != null)
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
			Logger.Info($"OnSolenoidUpdated: solenoid={solenoid}, isActive={isActive}");
		}

		static void OnGameEnded(object sender, EventArgs e)
		{
			Logger.Info("OnGameEnded");

			_isRunning = false;
		}

		static void Main(string[] args)
		{
			LogManager.Configuration = new LoggingConfiguration();

			var target = new ConsoleTarget("PinMame");

			LogManager.Configuration.AddTarget(target);
			LogManager.Configuration.AddRule(LogLevel.Info, LogLevel.Fatal, target);

			LogManager.ReconfigExistingLoggers();

			_pinMame = PinMame.Instance();

			DumpGames();
			DumpFoundGames();

			Logger.Info(_pinMame.GetGame("fh_906h"));

			_pinMame.OnGameStarted += OnGameStarted;
			_pinMame.OnDisplayAvailable += OnDisplayAvailable;
			_pinMame.OnDisplayUpdated += OnDisplayUpdated;
			_pinMame.OnSolenoidUpdated += OnSolenoidUpdated;
			_pinMame.OnGameEnded += OnGameEnded;

			_pinMame.StartGame("tf_180h");
			//_pinMame.StartGame("mm_109c");
			//_pinMame.StartGame("fh_906h");
			//_pinMame.StartGame("flashgdn");

			_isRunning = true;

			while (_isRunning)
			{
				Thread.Sleep(100);
			}
		}
	}
}
