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

namespace PinMame
{
	using System;
	using System.IO;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Runtime.InteropServices;
	using Registry = Microsoft.Win32.Registry;
	using NLog;
	using Logger = NLog.Logger;
	using Internal;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// PinMAME, a pinball ROM emulator.
	/// </summary>
	///
	/// <remarks>
	/// Note this doesn't use Visual PinMAME but is based on a cross-platform
	/// library.
	/// </remarks>

	public class PinMame
	{
		public bool IsRunning => _isRunning;
		public event EventHandler OnGameStarted;
		public event EventHandler<string> OnGameStartFailed;

		private int _gameIndex = -1;
		private DmdDimensions _dmd;
		private byte[] _frame;
		private int[] _changedLamps;
		private int[] _changedSolenoids;
		private int[] _changedGIs;
		private bool _isRunning;

		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		private static PinMame _instance;

		/// <summary>
		/// Retrieves all supported games. Sorted by parent description and clone descriptions.

		public static PinMameGame[] GetGames() {
			Dictionary<string, PinMameGame> games = new Dictionary<string, PinMameGame>();

			PinMameApi.GetGames((gameInfoStructPtr) =>
			{
				PinMameApi.GameInfoStruct gameInfoStruct = (PinMameApi.GameInfoStruct)Marshal.PtrToStructure(gameInfoStructPtr, typeof(PinMameApi.GameInfoStruct));

				if (string.IsNullOrEmpty(gameInfoStruct.cloneOf))
				{
					games.Add(gameInfoStruct.name, new PinMameGame(gameInfoStruct)
					{
						clones = Array.Empty<PinMameGame>()
					}); 
				}
			});

			PinMameApi.GetGames((gameInfoStructPtr) =>
			{
				PinMameApi.GameInfoStruct gameInfoStruct = (PinMameApi.GameInfoStruct)Marshal.PtrToStructure(gameInfoStructPtr, typeof(PinMameApi.GameInfoStruct));

				if (!string.IsNullOrEmpty(gameInfoStruct.cloneOf))
				{
					if (games.TryGetValue(gameInfoStruct.cloneOf, out PinMameGame game))
					{
						game.addClone(new PinMameGame(gameInfoStruct));
					}
				}
			});

			return games.Values.OrderBy(game => game.description).ToArray();
		}

		/// <summary>
		/// Creates or retrieves the PinMame instance.
		/// </summary>
		/// <param name="sampleRate">Audio sample rate</param>
		/// <param name="vpmPath">Fallback path for VPM folder, if VPM is not registered</param>
		/// <exception cref="InvalidOperationException">If VPM cannot be found</exception>
		public static PinMame Instance(int sampleRate = 48000, string vpmPath = null) =>
			_instance ?? (_instance = new PinMame(sampleRate, vpmPath));

		private PinMame(int sampleRate, string vpmPath)
		{
                       
			var path = vpmPath ?? GetVpmPath();
			if (path == null)
			{
				throw new InvalidOperationException("Could not determine VPM path. Either install VPinMAME or provide it manually.");
			}
			if (!Directory.Exists(path))
			{
				throw new InvalidOperationException($"Could not find VPM path {path} does not exist.");
			}

			PinMameApi.SetVPMPath(path + Path.DirectorySeparatorChar);
			PinMameApi.SetSampleRate(sampleRate);
		}

		/// <summary>
		/// Starts a new game.
		///
		/// When the game has successfully started, the `GameStarted` event is triggered.
		/// </summary>
		/// <param name="gameName">Name of the game, e.g. "tz_94h"</param>
		/// <param name="timeout">Timeout in milliseconds to wait for game to start</param>
		/// <param name="showConsole">If true, open PinMAME console</param>
		/// <exception cref="InvalidOperationException">If there is already a game running.</exception>
		/// <exception cref="ArgumentException">If the game name is invalid.</exception>
		public void StartGame(string gameName, int timeout = 5000, bool showConsole = false)
		{
			if (_isRunning)
			{
				throw new InvalidOperationException("Game is running, must stop first.");
			}

			Logger.Info("StartThreadedGame {0}", showConsole);

			_gameIndex = PinMameApi.StartThreadedGame(gameName, showConsole);

			if (_gameIndex < 0)
			{
				throw new ArgumentException("Unknown game \"" + gameName + "\".");
			}

			// start game async, and notify via event
			Task.Run(() =>
			{
				const int sleep = 10;
				var n = timeout / sleep;
				var i = 0;
				while (i++ < n && !PinMameApi.IsGameReady())
				{
					Thread.Sleep(sleep);
				}

				if (!PinMameApi.IsGameReady())
				{
					Logger.Info("Timed out waiting");

					OnGameStartFailed?.Invoke(this, "Timed out waiting for game to start.");
					return;
				}
				OnGameStarted?.Invoke(this, EventArgs.Empty);

				_isRunning = true;
				_dmd = GetDmdDimensions();

				Logger.Info("DMD {0} {1}", _dmd.Width, _dmd.Height);

				_frame = new byte[_dmd.Width * _dmd.Height];
				_changedLamps = new int[GetMaxLamps() * 2];
				_changedSolenoids = new int[GetMaxSolenoids() * 2];
				_changedGIs = new int[GetMaxGIs() * 2];
			});
		}

		public void StopGame()
		{
			Logger.Info("StopGame");

			_isRunning = false;
			PinMameApi.StopThreadedGame(true);
		}

		public void ResetGame()
		{
			Logger.Info("ResetGame");

			PinMameApi.ResetGame();
		}

		/// <summary>
		/// Returns the dimensions of the DMD in pixels.
		/// </summary>
		/// <exception cref="InvalidOperationException">If there is no game running.</exception>
		/// <returns>Dimensions</returns>
		public DmdDimensions GetDmdDimensions()
		{
			AssertRunningGame();
			return new DmdDimensions(PinMameApi.GetRawDMDWidth(), PinMameApi.GetRawDMDHeight());
		}

		/// <summary>
		/// Returns whether the DMD changed since the pixels were last
		/// retrieved.
		/// </summary>
		/// <exception cref="InvalidOperationException">If there is no game running.</exception>
		/// <returns>True if DMD changed, false otherwise.</returns>
		public bool NeedsDmdUpdate()
		{
			AssertRunningGame();
			return PinMameApi.NeedsDMDUpdate();
		}

		/// <summary>
		/// Returns the pixels of the DMD.
		/// </summary>
		/// <returns>Current DMD frame</returns>
		/// <exception cref="InvalidOperationException">If there is no game running.</exception>
		/// <exception cref="InvalidOperationException">If retrieving the pixels failed otherwise.</exception>
		public byte[] GetDmdPixels()
		{
			var res = PinMameApi.GetRawDMDPixels(_frame);
			if (res < 0)
			{
				throw new InvalidOperationException($"Got {res} from GetRawDMDPixels().");
			}
			return _frame;
		}

		/// <summary>
		/// Returns the state of a given switch.
		/// </summary>
		/// <param name="slot">Slot number of the switch</param>
		/// <returns>Value of the switch</returns>
		public bool GetSwitch(int slot) => PinMameApi.GetSwitch(slot);

		/// <summary>
		/// Sets the state of a given switch.
		/// </summary>
		/// <param name="slot">Slot number of the switch</param>
		/// <param name="state">New value of the switch</param>
		public void SetSwitch(int slot, bool state) => PinMameApi.SetSwitch(slot, state);

		/// <summary>
		/// Returns the maximal supported number of lamps.
		/// </summary>
		/// <returns>Number of lamps</returns>
		public int GetMaxLamps() => PinMameApi.GetMaxLamps();

		/// <summary>
		/// Returns an array of all changed lamps since the last call. <p/>
		///
		/// The returned array contains pairs, where the first element is the
		/// lamp number, and the second element the value.
		/// </summary>
		public Span<int> GetChangedLamps()
		{
			var num = PinMameApi.GetChangedLamps(_changedLamps);
			return _changedLamps.AsSpan().Slice(0, num * 2);
		}

		/// <summary>
		/// Returns the maximal supported number of solenoids.
		/// </summary>
		/// <returns>Number of solenoids</returns>
		public int GetMaxSolenoids() => PinMameApi.GetMaxSolenoids();

		/// <summary>
		/// Returns an array of all changed solenoids since the last call. <p/>
		///
		/// The returned array contains pairs, where the first element is the
		/// solenoid number, and the second element the value.
		/// </summary>
		public Span<int> GetChangedSolenoids()
		{
			var num = PinMameApi.GetChangedSolenoids(_changedSolenoids);
			return _changedSolenoids.AsSpan().Slice(0, num * 2);
		}

		/// <summary>
		/// Returns the maximal supported number of GIs.
		/// </summary>
		/// <returns>Number of GIs</returns>
		public int GetMaxGIs() => PinMameApi.GetMaxGIStrings();

		/// <summary>
		/// Returns an array of all changed GIs since the last call. <p/>
		///
		/// The returned array contains pairs, where the first element is the
		/// GI number, and the second element the value.
		/// </summary>
		public Span<int> GetChangedGIs()
		{
			var num = PinMameApi.GetChangedGIs(_changedGIs);
			return _changedGIs.AsSpan().Slice(0, num * 2);
		}

		private void AssertRunningGame()
		{
			if (!_isRunning)
			{
				throw new InvalidOperationException("Game must be running.");
			}
		}

		private static string GetVpmPath()
		{
			try
			{
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				{
					var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\VPinMAME.Controller\CLSID");
					if (reg != null)
					{
						var clsId = reg.GetValue(null).ToString();
						var x64Suffix = Environment.Is64BitOperatingSystem ? @"WOW6432Node\" : "";
						reg = Registry.ClassesRoot.OpenSubKey(x64Suffix + @"CLSID\" + clsId + @"\InprocServer32");
						if (reg != null)
						{
							return Path.GetDirectoryName(reg.GetValue(null).ToString());
						}
						Logger.Warn($"Could not find CLSID {clsId} of VPinMAME.dll.");
					}
					else
					{
						Logger.Warn("Looks like VPinMAME.Controller is not registered on the system!");
					}
				}
				else
				{
					var profilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
					var path = Path.GetFullPath(Path.Combine(profilePath, ".pinmame"));

					if (Directory.Exists(path))
					{
						return path;
					}
					else
					{
						Logger.Warn($"Could not find .pinmame folder in {profilePath}");
					}
				}
			}

			catch (Exception e)
			{
				Logger.Error("ERROR: " + e);
			}

			return null;
		}
	}

	public struct DmdDimensions
	{
		/// <summary>
		/// Width in pixels
		/// </summary>
		public readonly int Width;

		/// <summary>
		/// Height in pixels
		/// </summary>
		public readonly int Height;

		public DmdDimensions(int width, int height)
		{
			Width = width;
			Height = height;
		}
	}
}
