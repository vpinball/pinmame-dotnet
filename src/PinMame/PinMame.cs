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
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		public delegate void DisplayHandler(int index,
											PinMameDisplayLayout displayLayout,
											byte[] frame);

		public delegate void RegisterDisplayEventHandler(object sender,
														 EventArgs e,
														 int index,
														 PinMameDisplayLayout displayLayout);

		public delegate void SolenoidEventHandler(object sender,
												  EventArgs e,
											  	  int solenoid,
												  bool isActive);

		public event EventHandler OnGameStarted;
		public event RegisterDisplayEventHandler OnRegisterDisplay;
		public event SolenoidEventHandler OnSolenoid;
		public event EventHandler OnGameEnded;

		private PinMameApi.PinmameConfig _config;
		private byte[] _frame;
		private int[] _changedLamps;
		private int[] _changedSolenoids;
		private int[] _changedGIs;

		private static PinMame _instance;

		/// <summary>
		/// Retrieves all supported games. Sorted by parent description and clone descriptions.
		public static PinMameGame[] GetGames() {
			Logger.Info("GetGames");

			Dictionary<string, PinMameGame> games = new Dictionary<string, PinMameGame>();

			PinMameApi.PinmameGetGames((gamePtr) =>
			{
				PinMameApi.PinmameGame pinmameGame = (PinMameApi.PinmameGame)Marshal.PtrToStructure(gamePtr, typeof(PinMameApi.PinmameGame));

				if (string.IsNullOrEmpty(pinmameGame.cloneOf))
				{
					games.Add(pinmameGame.name, new PinMameGame(pinmameGame)
					{
						clones = Array.Empty<PinMameGame>()
					}); 
				}
			});

			PinMameApi.PinmameGetGames((gamePtr) =>
			{
				PinMameApi.PinmameGame pinmameGame = (PinMameApi.PinmameGame)Marshal.PtrToStructure(gamePtr, typeof(PinMameApi.PinmameGame));

				if (!string.IsNullOrEmpty(pinmameGame.cloneOf))
				{
					if (games.TryGetValue(pinmameGame.cloneOf, out PinMameGame game))
					{
						game.addClone(new PinMameGame(pinmameGame));
					}
				}
			});

			PinMameGame[] array = games.Values.OrderBy(game => game.description).ToArray();

			Logger.Info("GetGames - total={0}", array.Length);

			return array;
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
			Logger.Info("PinMame - sampleRate={0}, vpmPath={1}", vpmPath);

			var path = vpmPath ?? GetVpmPath();
			if (path == null)
			{
				throw new InvalidOperationException("Could not determine VPM path. Either install VPinMAME or provide it manually.");
			}
			if (!Directory.Exists(path))
			{
				throw new InvalidOperationException($"Could not find VPM path {path} does not exist.");
			}

			_config = new PinMameApi.PinmameConfig {
				sampleRate = 48000,
				vpmPath = path + Path.DirectorySeparatorChar,
				onStateChange = OnStateChangeCallback,
				onSolenoid = OnSolenoidCallback,
			};

			PinMameApi.PinmameSetConfig(ref _config);
		}

		private void OnStateChangeCallback(int state)
		{
			Logger.Info("OnStateChangeCallback - state={0}, isActive={1}", state);

			if (state == 1)
			{
				_changedLamps = new int[PinMameApi.PinmameGetMaxLamps() * 2];
				_changedSolenoids = new int[PinMameApi.PinmameGetMaxSolenoids() * 2];
				_changedGIs = new int[PinMameApi.PinmameGetMaxGIs() * 2];
				_frame = new byte[128 * 32];

				PinMameApi.PinmameGetDisplayLayouts((index, displayLayoutPtr) =>
				{
					PinMameApi.PinmameDisplayLayout displayLayout = (PinMameApi.PinmameDisplayLayout)Marshal.PtrToStructure(displayLayoutPtr, typeof(PinMameApi.PinmameDisplayLayout));

					OnRegisterDisplay?.Invoke(this, EventArgs.Empty, index, new PinMameDisplayLayout(displayLayout));
				});

				OnGameStarted?.Invoke(this, EventArgs.Empty);
			}
			else
			{
				OnGameEnded?.Invoke(this, EventArgs.Empty);
			}
		}

		private void OnSolenoidCallback(int solenoid, int isActive)
		{
			Logger.Info("OnSolenoidCallback - solenoid={0}, isActive={1}", solenoid, isActive);

			OnSolenoid?.Invoke(this, EventArgs.Empty, solenoid, isActive == 1);
		}

		/// <summary>
		/// Starts a new game.
		///
		/// When the game has successfully started, the `GameStarted` event is triggered.
		/// </summary>
		/// <param name="gameName">Name of the game, e.g. "tz_94h"</param>
		/// <exception cref="InvalidOperationException">If there is already a game running.</exception>
		/// <exception cref="ArgumentException">If the game name is invalid.</exception>
		public void StartGame(string gameName)
		{
			Logger.Info("StartGame");

			PinMameApi.PinmameStatus status = PinMameApi.PinmameRun(gameName);

			if (status != PinMameApi.PinmameStatus.OK)
			{
				throw new InvalidOperationException("Unable to start game, status=" + status);
			}
		}

		/// <summary>
		/// GetDisplays
		/// </summary>
		public void GetDisplays(DisplayHandler callback)
		{
			PinMameApi.PinmameGetDisplays(_frame, (index, displayLayoutPtr) =>
			{
				PinMameApi.PinmameDisplayLayout displayLayout = (PinMameApi.PinmameDisplayLayout)Marshal.PtrToStructure(displayLayoutPtr, typeof(PinMameApi.PinmameDisplayLayout));

				callback(index, new PinMameDisplayLayout(displayLayout), _frame);
			});
		}

		/// <summary>
		/// Resets a game.
		/// </summary>
		public void ResetGame()
		{
			Logger.Info("ResetGame");

			PinMameApi.PinmameReset();
		}

		/// <summary>
		/// Stops a game.
		/// </summary>
		public void StopGame()
		{
			Logger.Info("StopGame");

			PinMameApi.PinmameStop();
		}

		public bool IsRunning()
		{
			return (PinMameApi.PinmameIsRunning() == 1);
		}

		public void Pause()
		{
			Logger.Info("Pause");

			PinMameApi.PinmameStatus status = PinMameApi.PinmamePause(1);

			if (status != PinMameApi.PinmameStatus.OK)
			{
				throw new InvalidOperationException("Unable to pause game, status=" + status);
			}
		}

		public void Continue()
		{
			Logger.Info("Continue");

			PinMameApi.PinmameStatus status = PinMameApi.PinmamePause(0);

			if (status != PinMameApi.PinmameStatus.OK)
			{
				throw new InvalidOperationException("Unable to continue game, status=" + status);
			}
		}

		/// <summary>
		/// Returns the state of a given switch.
		/// </summary>
		/// <param name="slot">Slot number of the switch</param>
		/// <returns>Value of the switch</returns>
		public bool GetSwitch(int slot) => PinMameApi.PinmameGetSwitch(slot) == 1;

		/// <summary>
		/// Sets the state of a given switch.
		/// </summary>
		/// <param name="slot">Slot number of the switch</param>
		/// <param name="state">New value of the switch</param>
		public void SetSwitch(int slot, bool state) => PinMameApi.PinmameSetSwitch(slot, state ? 1 : 0);

		/// <summary>
		/// Returns the maximal supported number of lamps.
		/// </summary>
		/// <returns>Number of lamps</returns>
		public int GetMaxLamps() => PinMameApi.PinmameGetMaxLamps();

		/// <summary>
		/// Returns an array of all changed lamps since the last call. <p/>
		///
		/// The returned array contains pairs, where the first element is the
		/// lamp number, and the second element the value.
		/// </summary>
		public Span<int> GetChangedLamps()
		{
			var num = PinMameApi.PinmameGetChangedLamps(_changedLamps);
			return _changedLamps.AsSpan().Slice(0, num * 2);
		}

		/// <summary>
		/// Returns the maximal supported number of solenoids.
		/// </summary>
		/// <returns>Number of solenoids</returns>
		public int GetMaxSolenoids() => PinMameApi.PinmameGetMaxSolenoids();

		/// <summary>
		/// Returns an array of all changed solenoids since the last call. <p/>
		///
		/// The returned array contains pairs, where the first element is the
		/// solenoid number, and the second element the value.
		/// </summary>
		public Span<int> GetChangedSolenoids()
		{
			var num = PinMameApi.PinmameGetChangedSolenoids(_changedSolenoids);
			return _changedSolenoids.AsSpan().Slice(0, num * 2);
		}

		/// <summary>
		/// Returns the maximal supported number of GIs.
		/// </summary>
		/// <returns>Number of GIs</returns>
		public int GetMaxGIs() => PinMameApi.PinmameGetMaxGIs();

		/// <summary>
		/// Returns an array of all changed GIs since the last call. <p/>
		///
		/// The returned array contains pairs, where the first element is the
		/// GI number, and the second element the value.
		/// </summary>
		public Span<int> GetChangedGIs()
		{
			var num = PinMameApi.PinmameGetChangedGIs(_changedGIs);
			return _changedGIs.AsSpan().Slice(0, num * 2);
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
}
