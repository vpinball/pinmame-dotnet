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

// ReSharper disable MemberCanBeMadeStatic.Global

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using NLog;

namespace PinMame
{
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
		#region Delegates Definition

		/// <summary>
		/// A delegate, called when a game starts.
		/// </summary>
		public delegate void OnGameStartedEventHandler();

		/// <summary>
		/// A delegate, called when displays are available.
		/// </summary>
		public delegate void OnDisplayAvailableEventHandler(int index, int displayCount, PinMameDisplayLayout displayLayout);

		/// <summary>
		/// A delegate, called when a display is updated.
		/// </summary>
		public delegate void OnDisplayUpdatedEventHandler(int index, IntPtr framePtr, PinMameDisplayLayout displayLayout);

		/// <summary>
		/// A delegate, called when a solenoid is updated.
		/// </summary>
		public delegate void OnSolenoidUpdatedEventHandler(int solenoid, bool isActive);

		/// <summary>
		/// A delegate, called when a game ends.
		/// </summary>
		public delegate void OnGameEndedEventHandler();

		/// <summary>
		/// A delegate, called when a keycode is requested.
		/// </summary>
		public delegate int IsKeyPressedEventHandler(PinMameKeycode keycode);

		#endregion

		/// <summary>
		/// Game is started and ready to receive switch changes.
		/// </summary>
		public event OnGameStartedEventHandler OnGameStarted;

		/// <summary>
		/// A new display is available. This is called as soon as possible,
		/// and before <see cref="OnDisplayUpdated"/>.
		/// </summary>
		public event OnDisplayAvailableEventHandler OnDisplayAvailable;

		/// <summary>
		/// A display needs updating due to new content.
		/// </summary>
		public event OnDisplayUpdatedEventHandler OnDisplayUpdated;

		/// <summary>
		/// A coil state has changed.
		/// </summary>
		public event OnSolenoidUpdatedEventHandler OnSolenoidUpdated;

		/// <summary>
		/// The game has ended.
		/// </summary>
		public event OnGameEndedEventHandler OnGameEnded;

		/// <summary>
		/// The game has ended.
		/// </summary>
		public event IsKeyPressedEventHandler IsKeyPressed;

		/// <summary>
		/// Retrieves rom path which is vpmPath + roms.
		/// </summary>
		public string RomPath => _config.vpmPath + "roms";

		/// <summary>
		/// The currently running game
		/// </summary>
		public string RunningGame { get; private set; }

		/// <summary>
		/// Returns whether a game is currently running.
		/// </summary>
		public static bool IsRunning => PinMameApi.PinmameIsRunning() == 1;

		/// <summary>
		/// Returns the hardware generation
		/// </summary>
		/// <returns>Value of the hardware generation</returns>
		public static PinMameHardwareGen CurrentHardwareGen => (PinMameHardwareGen)PinMameApi.PinmameGetHardwareGen();

		private readonly PinMameApi.PinmameConfig _config;
		private int[] _changedLamps;
		private int[] _changedGIs;
		private readonly Dictionary<int, PinMameDisplayLayout> _availableDisplays = new Dictionary<int, PinMameDisplayLayout>();

		private static PinMame _instance;
		private CancellationTokenSource _availableDisplaysToken;

		private const int DisplayAvailableTimeoutMs = 1000;
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// Creates or retrieves the PinMame instance.
		/// </summary>
		/// <param name="sampleRate">Audio sample rate</param>
		/// <param name="vpmPath">Fallback path for VPM folder, if VPM is not registered</param>
		/// <exception cref="ArgumentException">If VPM cannot be found</exception>
		public static PinMame Instance(string vpmPath = null, int sampleRate = 48000) =>
			_instance ?? (_instance = new PinMame(vpmPath, sampleRate));

		private PinMame(string vpmPath, int sampleRate)
		{
			Logger.Info($"PinMame - sampleRate={sampleRate}, vpmPath={vpmPath}");

			var path = vpmPath ?? GetVpmPath();
			if (path == null) {
				throw new ArgumentException("Could not determine VPM path. Either install VPinMAME or provide it manually.", nameof(vpmPath));
			}

			if (!Directory.Exists(path)) {
				throw new ArgumentException($"Could not find VPM path - {path} does not exist.", nameof(vpmPath));
			}

			_config = new PinMameApi.PinmameConfig {
				sampleRate = 48000,
				vpmPath = path + Path.DirectorySeparatorChar,
				onStateUpdated = OnStateUpdatedCallback,
				onDisplayAvailable = OnDisplayAvailableCallback,
				onDisplayUpdated = OnDisplayUpdatedCallback,
				onSolenoidUpdated = OnSolenoidUpdatedCallback,
				isKeyPressed = IsKeyPressedFunction,
			};
			PinMameApi.PinmameSetConfig(ref _config);
		}

		/// <summary>
		/// Starts a new game. <p/>
		///
		/// When the game has successfully started, the <see cref="OnGameStarted"/> event is triggered.
		/// </summary>
		/// <param name="gameName">Name of the game, e.g. "tz_94h"</param>
		/// <exception cref="InvalidOperationException">If there is already a game running.</exception>
		/// <exception cref="ArgumentException">If the game name is invalid.</exception>
		public void StartGame(string gameName)
		{
			Logger.Info("StartGame");
			RunningGame = gameName;
			var status = PinMameApi.PinmameRun(gameName);
			if (status != PinMameApi.PinmameStatus.OK) {
				throw new InvalidOperationException($"Unable to start game, status={status}");
			}
		}

		/// <summary>
		/// Retrieves all available displays from a given game by running it.
		/// </summary>
		///
		/// <remarks>
		/// This is a blocking operation. No game must be currently running.
		/// </remarks>
		/// <param name="romId">ROM ID of the game to retrieve available displays for.</param>
		/// <returns>A dictionary where keys are the displays IDs and values are the layouts. Result is always returned on the calling thread.</returns>
		/// <exception cref="InvalidOperationException">Thrown when a game is already running.</exception>
		public Dictionary<int, PinMameDisplayLayout> GetAvailableDisplays(string romId)
		{
			if (IsRunning) {
				throw new InvalidOperationException("Cannot retrieve available displays while another game is already running.");
			}

			_availableDisplays.Clear();
			OnDisplayAvailable += ProbeOnDisplayAvailable;
			try {
				StartGame(romId);
				WaitForAvailableDisplays();
				return _availableDisplays;

			} finally {
				StopGame();
				OnDisplayAvailable -= ProbeOnDisplayAvailable;
			}
		}

		/// <summary>
		/// Retrieves a game by game name.
		/// </summary>
		/// <exception cref="InvalidOperationException">If the configuration has not been set or the game name is not found.</exception>
		public PinMameGame GetGame(string name)
		{
			Logger.Info($"GetGame: name={name}");

			PinMameGame game = null;
			var status = PinMameApi.PinmameGetGame(name, gamePtr => {
				game = new PinMameGame(gamePtr);
			});

			if (status != PinMameApi.PinmameStatus.OK) {
				throw new InvalidOperationException($"Unable to get game, name={name}, status={status}");
			}

			return game;
		}

		/// <summary>
		/// Retrieves all supported games.
		/// </summary>
		/// <exception cref="InvalidOperationException">If the configuration has not been set.</exception>
		public IEnumerable<PinMameGame> GetGames()
		{
			var games = new Dictionary<string, PinMameGame>(); // game name -> game
			var clones = new List<(string, PinMameGame)>();    // parent game name -> game

			var status = PinMameApi.PinmameGetGames(gamePtr => {
				var game = new PinMameGame(gamePtr);

				if (string.IsNullOrEmpty(game.CloneOf)) {
					games[game.Name] = game;

				} else {
					clones.Add((game.CloneOf, game));
				}
			});

			if (status != PinMameApi.PinmameStatus.OK) {
				throw new InvalidOperationException($"Unable to get games, status={status}");
			}

			foreach (var (cloneName, clone) in clones) {
				if (games.ContainsKey(cloneName)) {
					games[cloneName].AddClone(clone);

				} else {
					games[cloneName] = clone;
				}
			}

			return games.Values;
		}

		/// <summary>
		/// Retrieves all games found in roms folder. Clones array will not be populated.
		/// </summary>
		/// <exception cref="InvalidOperationException">If the configuration has not been set.</exception>
		public IEnumerable<PinMameGame> GetFoundGames()
		{
			var games = new List<PinMameGame>();
			var status = PinMameApi.PinmameGetGames(gamePtr => {
				var game = new PinMameGame(gamePtr);
				if (game.RomFound) {
					games.Add(game);
				}
			});

			if (status != PinMameApi.PinmameStatus.OK) {
				throw new InvalidOperationException($"Unable to get games, status={status}");
			}

			return games;
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

		/// <summary>
		/// Pauses a game.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public void Pause()
		{
			Logger.Info("Pause");

			var status = PinMameApi.PinmamePause(1);
			if (status != PinMameApi.PinmameStatus.OK) {
				throw new InvalidOperationException($"Unable to pause game, status={status}");
			}
		}

		/// <summary>
		/// Continues a paused game.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public void Continue()
		{
			Logger.Info("Continue");

			var status = PinMameApi.PinmamePause(0);
			if (status != PinMameApi.PinmameStatus.OK) {
				throw new InvalidOperationException($"Unable to continue game, status={status}");
			}
		}

		private void OnStateUpdatedCallback(int state)
		{
			Logger.Debug($"OnStateUpdatedCallback - state={state}");

			if (state == 1)
			{
				_changedLamps = new int[PinMameApi.PinmameGetMaxLamps() * 2];
				_changedGIs = new int[PinMameApi.PinmameGetMaxGIs() * 2];

				OnGameStarted?.Invoke();
			}
			else
			{
				OnGameEnded?.Invoke();
				RunningGame = null;
			}
		}

		private void OnDisplayAvailableCallback(int index, int displayCount, ref PinMameApi.PinmameDisplayLayout displayLayoutRef)
		{
			var displayLayout = new PinMameDisplayLayout(displayLayoutRef, PinMameApi.PinmameGetHardwareGen());

			Logger.Trace($"OnDisplayAvailableCallback - index={index}, displayCount={displayCount}, displayLayout={displayLayout}");

			OnDisplayAvailable?.Invoke(index, displayCount, displayLayout);
		}

		private void OnDisplayUpdatedCallback(int index, IntPtr framePtr, ref PinMameApi.PinmameDisplayLayout displayLayoutRef)
		{
			var displayLayout = new PinMameDisplayLayout(displayLayoutRef);

			Logger.Trace($"OnDisplayUpdatedCallback - index={index}, displayLayout={displayLayout}");

			OnDisplayUpdated?.Invoke(index, framePtr, displayLayout);
		}

		private void OnSolenoidUpdatedCallback(int solenoid, int isActive)
		{
			Logger.Debug($"OnSolenoidUpdatedCallback - solenoid={solenoid}, isActive={isActive}");

			OnSolenoidUpdated?.Invoke(solenoid, isActive == 1);
		}

		private int IsKeyPressedFunction(PinMameApi.PinmameKeycode keycode)
		{
			Logger.Trace($"IsKeyPressedFunction - keycode={keycode}");

			return IsKeyPressed?.Invoke((PinMameKeycode)keycode) ?? 0;
		}

		/// <summary>
		/// Blocks until either all available displays have been announced, or <see cref="DisplayAvailableTimeoutMs"/> milliseconds pass.
		/// </summary>
		private void WaitForAvailableDisplays()
		{
			_availableDisplaysToken = new CancellationTokenSource();
			var t = Task.Run(async () => {
				await Task.Delay(DisplayAvailableTimeoutMs, _availableDisplaysToken.Token);
			});
			try {
				t.Wait();
			} catch (AggregateException) {
				// task was cancelled, all good.
			}
		}

		private void ProbeOnDisplayAvailable(int index, int displayCount, PinMameDisplayLayout displayLayout)
		{
			_availableDisplays[index] = displayLayout;
			if (displayCount == _availableDisplays.Count) {
				_availableDisplaysToken.Cancel();
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
							reg = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Freeware\Visual PinMame\globals");

							if (reg != null)
							{
								var path = reg.GetValue("rompath").ToString();

								if (path.EndsWith(@"\roms"))
								{
									return path.Substring(0, path.Length - 5);
								}
								else
								{
									Logger.Warn($"Rom Path {path} last folder is not 'roms'");
								}
							}
							else
							{
								Logger.Warn($"Could not Rom Path in registry.");
							}
						}
						else
						{
							Logger.Warn($"Could not find CLSID {clsId} of VPinMAME.dll.");
						}
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
				Logger.Error($"ERROR: {e}");
			}

			return null;
		}

		#region Deprecated API?

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

		#endregion
	}
}
