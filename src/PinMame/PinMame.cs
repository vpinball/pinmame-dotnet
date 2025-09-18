// pinmame-dotnet
// Copyright (C) 1999-2022 PinMAME development team and contributors
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
		/// A delegate, called when the audio stream is available.
		/// </summary>
		public delegate int OnAudioAvailableEventHandler(PinMameAudioInfo audioInfo);

		/// <summary>
		/// A delegate, called when the audio stream is updated.
		/// </summary>
		public delegate int OnAudioUpdatedEventHandler(IntPtr bufferPtr, int samples);

		/// <summary>
		/// A delegate, called when a mech is available.
		/// </summary>
		public delegate void OnMechAvailableEventHandler(int mechNo, PinMameMechInfo mechInfo);

		/// <summary>
		/// A delegate, called when a mech is updated.
		/// </summary>
		public delegate void OnMechUpdatedEventHandler(int mechNo, PinMameMechInfo mechInfo);

		/// <summary>
		/// A delegate, called when a solenoid is updated.
		/// </summary>
		public delegate void OnSolenoidUpdatedEventHandler(int solenoid, bool isActive);

		/// <summary>
		/// A delegate, called when console data is updated.
		/// </summary>
		public delegate void OnConsoleDataUpdatedEventHandler(IntPtr dataPtr, int size);

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
		/// The audio stream is available. This is called as soon as possible,
		/// and before <see cref="OnAudioUpdated"/>.
		/// </summary>
		public event OnAudioAvailableEventHandler OnAudioAvailable;

		/// <summary>
		/// The audio stream needs updating due to new content.
		/// </summary>
		public event OnAudioUpdatedEventHandler OnAudioUpdated;

		/// <summary>
		/// A new mech is available.
		/// </summary>
		public event OnMechAvailableEventHandler OnMechAvailable;

		/// <summary>
		/// A mech speed or position has changed.
		/// </summary>
		public event OnMechUpdatedEventHandler OnMechUpdated;

		/// <summary>
		/// A coil state has changed.
		/// </summary>
		public event OnSolenoidUpdatedEventHandler OnSolenoidUpdated;

		/// <summary>
		/// Console data has changed.
		/// </summary>
		public event OnConsoleDataUpdatedEventHandler OnConsoleDataUpdated;

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
		public static bool IsRunning => PinMameApi.IsRunning() == 1;

		/// <summary>
		/// Returns the hardware generation
		/// </summary>
		/// <returns>Value of the hardware generation</returns>
		public static PinMameHardwareGen CurrentHardwareGen => (PinMameHardwareGen)PinMameApi.GetHardwareGen();

		private readonly PinMameApi.Config _config;
		private int[] _changedLamps;
		private int[] _changedGIs;
		private readonly Dictionary<int, PinMameDisplayLayout> _availableDisplays = new Dictionary<int, PinMameDisplayLayout>();

		private static PinMame _instance;
		private CancellationTokenSource _availableDisplaysToken;

		private const int DisplayAvailableTimeoutMs = 1000;
		public const int MaxMechSwitches = 20;

		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// Creates or retrieves the PinMame instance.
		/// </summary>
		/// <param name="audioFormat">Audio format (Int16 / Float)</param>
		/// <param name="sampleRate">Audio sample rate</param>
		/// <param name="vpmPath">Fallback path for VPM folder, if VPM is not registered</param>
		/// <exception cref="ArgumentException">If VPM cannot be found</exception>
		public static PinMame Instance(PinMameAudioFormat audioFormat = PinMameAudioFormat.AudioFormatInt16, int sampleRate = 48000, string vpmPath = null) =>
			_instance ?? (_instance = new PinMame(audioFormat, sampleRate, vpmPath));

		private PinMame(PinMameAudioFormat audioFormat, int sampleRate, string vpmPath)
		{
			Logger.Info($"PinMame - audioFormat={audioFormat}, sampleRate={sampleRate}, vpmPath={vpmPath}");

			var path = vpmPath ?? GetVpmPath();
			if (path == null) {
				throw new ArgumentException("Could not determine VPM path. Either install VPinMAME or provide it manually.", nameof(vpmPath));
			}

			if (!Directory.Exists(path)) {
				throw new ArgumentException($"Could not find VPM path - {path} does not exist.", nameof(vpmPath));
			}

			_config = new PinMameApi.Config {
				audioFormat = (PinMameApi.AudioFormat)audioFormat,
				sampleRate = sampleRate,
				vpmPath = path + Path.DirectorySeparatorChar,
				onStateUpdated = OnStateUpdatedCallbackPInvoke,
				onDisplayAvailable = OnDisplayAvailableCallbackPInvoke,
				onDisplayUpdated = OnDisplayUpdatedCallbackPInvoke,
				onAudioAvailable = OnAudioAvailableCallbackPInvoke,
				onAudioUpdated = OnAudioUpdatedCallbackPInvoke,
				onMechAvailable = OnMechAvailableCallbackPInvoke,
				onMechUpdated = OnMechUpdatedCallbackPInvoke,
				onSolenoidUpdated = OnSolenoidUpdatedCallbackPInvoke,
				onConsoleDataUpdated = OnConsoleDataUpdatedCallbackPInvoke,
				isKeyPressed = IsKeyPressedFunctionPInvoke,
			};
			PinMameApi.SetConfig(ref _config);
		}

		/// <summary>
		/// Returns if the HandleKeyboard option is enabled or disabled.
		/// </summary>
		public bool GetHandleKeyboard() => PinMameApi.GetHandleKeyboard() == 1;

		/// <summary>
		/// Enables or disables the HandleKeyboard option.
		/// </summary>
		/// <param name="handleKeyboard">New value of the HandleKeyboard flag.</param>
		public void SetHandleKeyboard(bool handleKeyboard) => PinMameApi.SetHandleKeyboard(handleKeyboard ? 1 : 0);

		/// <summary>
		/// Returns the HandleMechanics value.
		/// </summary>
		public int GetHandleMechanics() => PinMameApi.GetHandleMechanics();

		/// <summary>
		/// Sets the HandleMechanics option.
		/// </summary>
		/// <param name="handleMechanics">New value of the HandleMechanics option.</param>
		public void SetHandleMechanics(int handleMechanics) => PinMameApi.SetHandleMechanics(handleMechanics);

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
			var status = PinMameApi.Run(gameName);
			if (status != PinMameApi.Status.OK) {
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
			var status = PinMameApi.GetGame(name, gamePtr => {
				game = new PinMameGame(gamePtr);
			});

			if (status != PinMameApi.Status.OK) {
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

			var status = PinMameApi.GetGames(gamePtr => {
				var game = new PinMameGame(gamePtr);

				if (string.IsNullOrEmpty(game.CloneOf)) {
					games[game.Name] = game;

				} else {
					clones.Add((game.CloneOf, game));
				}
			});

			if (status != PinMameApi.Status.OK) {
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
			var status = PinMameApi.GetGames(gamePtr => {
				var game = new PinMameGame(gamePtr);
				if (game.RomFound) {
					games.Add(game);
				}
			});

			if (status != PinMameApi.Status.OK) {
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
			PinMameApi.Reset();
		}

		/// <summary>
		/// Stops a game.
		/// </summary>
		public void StopGame()
		{
			Logger.Info("StopGame");
			PinMameApi.Stop();
		}

		/// <summary>
		/// Pauses a game.
		/// </summary>
		/// <exception cref="InvalidOperationException"></exception>
		public void Pause()
		{
			Logger.Info("Pause");

			var status = PinMameApi.Pause(1);
			if (status != PinMameApi.Status.OK) {
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

			var status = PinMameApi.Pause(0);
			if (status != PinMameApi.Status.OK) {
				throw new InvalidOperationException($"Unable to continue game, status={status}");
			}
		}

		[MonoPInvokeCallback(typeof(PinMameApi.OnStateUpdatedCallback))]
		private static void OnStateUpdatedCallbackPInvoke(int state)
		{
			_instance.OnStateUpdatedCallback(state);
		}

		private void OnStateUpdatedCallback(int state)
		{
			Logger.Debug($"OnStateUpdatedCallback - state={state}");

			if (state == 1) {
				_changedLamps = new int[PinMameApi.GetMaxLamps() * 2];
				_changedGIs = new int[PinMameApi.GetMaxGIs() * 2];

				OnGameStarted?.Invoke();
			}
			else {
				OnGameEnded?.Invoke();
				RunningGame = null;
			}
		}

		[MonoPInvokeCallback(typeof(PinMameApi.OnDisplayAvailableCallback))]
		private static void OnDisplayAvailableCallbackPInvoke(int index, int displayCount, ref PinMameApi.DisplayLayout displayLayoutRef)
		{
			_instance.OnDisplayAvailableCallback(index, displayCount, ref displayLayoutRef);
		}

		private void OnDisplayAvailableCallback(int index, int displayCount, ref PinMameApi.DisplayLayout displayLayoutRef)
		{
			var displayLayout = new PinMameDisplayLayout(displayLayoutRef, PinMameApi.GetHardwareGen());

			Logger.Trace($"OnDisplayAvailableCallback - index={index}, displayCount={displayCount}, displayLayout={displayLayout}");

			OnDisplayAvailable?.Invoke(index, displayCount, displayLayout);
		}

		[MonoPInvokeCallback(typeof(PinMameApi.OnDisplayUpdatedCallback))]
		private static void OnDisplayUpdatedCallbackPInvoke(int index, IntPtr framePtr, ref PinMameApi.DisplayLayout displayLayoutRef)
		{
			_instance.OnDisplayUpdatedCallback(index, framePtr, ref displayLayoutRef);
		}

		private void OnDisplayUpdatedCallback(int index, IntPtr framePtr, ref PinMameApi.DisplayLayout displayLayoutRef)
		{
			var displayLayout = new PinMameDisplayLayout(displayLayoutRef);

			Logger.Trace($"OnDisplayUpdatedCallback - index={index}, displayLayout={displayLayout}");

			OnDisplayUpdated?.Invoke(index, framePtr, displayLayout);
		}

		[MonoPInvokeCallback(typeof(PinMameApi.OnAudioAvailableCallback))]
		private static int OnAudioAvailableCallbackPInvoke(ref PinMameApi.AudioInfo audioInfoRef)
		{
			return _instance.OnAudioAvailableCallback(ref audioInfoRef);
		}

		private int OnAudioAvailableCallback(ref PinMameApi.AudioInfo audioInfoRef)
		{ 
			var audioInfo = new PinMameAudioInfo(audioInfoRef);

			Logger.Trace($"OnAudioAvailableCallback - audioInfo={audioInfo}");

			return OnAudioAvailable?.Invoke(audioInfo) ?? 0;
		}

		[MonoPInvokeCallback(typeof(PinMameApi.OnAudioUpdatedCallback))]
		private static int OnAudioUpdatedCallbackPInvoke(IntPtr bufferPtr, int samples)
		{
			return _instance.OnAudioUpdatedCallback(bufferPtr, samples);
		}

		private int OnAudioUpdatedCallback(IntPtr bufferPtr, int samples)
		{
			Logger.Trace($"OnAudioUpdatedCallback - samples={samples}");

			return OnAudioUpdated?.Invoke(bufferPtr, samples) ?? 0;
		}

		[MonoPInvokeCallback(typeof(PinMameApi.OnMechAvailableCallback))]
		private static void OnMechAvailableCallbackPInvoke(int mechNo, ref PinMameApi.MechInfo mechInfoRef)
		{
			_instance.OnMechAvailableCallback(mechNo, ref mechInfoRef);
		}

		private void OnMechAvailableCallback(int mechNo, ref PinMameApi.MechInfo mechInfoRef)
		{
			var mechInfo = new PinMameMechInfo(mechInfoRef);

			Logger.Trace($"OnMechAvailableCallback - mechNo={mechNo}, mechInfo={mechInfo}");

			OnMechAvailable?.Invoke(mechNo, mechInfo);
		}

		[MonoPInvokeCallback(typeof(PinMameApi.OnMechUpdatedCallback))]
		private static void OnMechUpdatedCallbackPInvoke(int mechNo, ref PinMameApi.MechInfo mechInfoRef)
		{
			_instance.OnMechUpdatedCallback(mechNo, ref mechInfoRef);
		}

		private void OnMechUpdatedCallback(int mechNo, ref PinMameApi.MechInfo mechInfoRef)
		{
			var mechInfo = new PinMameMechInfo(mechInfoRef);

			Logger.Trace($"OnMechUpdatedCallback - mechNo={mechNo}, mechInfo={mechInfo}");

			_instance.OnMechUpdated?.Invoke(mechNo, mechInfo);
		}

		[MonoPInvokeCallback(typeof(PinMameApi.OnSolenoidUpdatedCallback))]
		private static void OnSolenoidUpdatedCallbackPInvoke(int solenoid, int isActive)
		{
			_instance.OnSolenoidUpdatedCallback(solenoid, isActive);
		}

		private void OnSolenoidUpdatedCallback(int solenoid, int isActive)
		{ 
			Logger.Debug($"OnSolenoidUpdatedCallback - solenoid={solenoid}, isActive={isActive}");

			OnSolenoidUpdated?.Invoke(solenoid, isActive == 1);
		}

		[MonoPInvokeCallback(typeof(PinMameApi.OnConsoleDataUpdatedCallback))]
		private static void OnConsoleDataUpdatedCallbackPInvoke(IntPtr dataPtr, int size)
		{
			_instance.OnConsoleDataUpdatedCallback(dataPtr, size);
		}

		private void OnConsoleDataUpdatedCallback(IntPtr dataPtr, int size)
		{
			Logger.Debug($"OnConsoleDataUpdatedCallback - size={size}");

			OnConsoleDataUpdated?.Invoke(dataPtr, size);
		}

		[MonoPInvokeCallback(typeof(PinMameApi.IsKeyPressedFunction))]
		private static int IsKeyPressedFunctionPInvoke(PinMameApi.Keycode keycode)
		{
			return _instance.IsKeyPressedFunction(keycode);
		}

		private int IsKeyPressedFunction(PinMameApi.Keycode keycode)
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
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
					var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\VPinMAME.Controller\CLSID");
					if (reg != null) {
						var clsId = reg.GetValue(null).ToString();
						var x64Suffix = Environment.Is64BitOperatingSystem ? @"WOW6432Node\" : "";
						reg = Registry.ClassesRoot.OpenSubKey(x64Suffix + @"CLSID\" + clsId + @"\InprocServer32");
						if (reg != null) {
							reg = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Freeware\Visual PinMame\globals");

							if (reg != null) {
								var path = reg.GetValue("rompath").ToString();

								if (path.EndsWith(@"\roms")) {
									return path.Substring(0, path.Length - 5);
								}
								else {
									Logger.Warn($"Rom Path {path} last folder is not 'roms'");
								}
							}
							else {
								Logger.Warn($"Could not Rom Path in registry.");
							}
						}
						else {
							Logger.Warn($"Could not find CLSID {clsId} of VPinMAME.dll.");
						}
					}
					else {
						Logger.Warn("Looks like VPinMAME.Controller is not registered on the system!");
					}
				}
				else {
					var profilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
					var path = Path.GetFullPath(Path.Combine(profilePath, ".pinmame"));

					if (Directory.Exists(path)) {
						return path;
					}
					else {
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

		/// <summary>
		/// Returns the state of a given switch.
		/// </summary>
		/// <param name="slot">Slot number of the switch</param>
		/// <returns>Value of the switch</returns>
		public bool GetSwitch(int slot) => PinMameApi.GetSwitch(slot) != 0;

		/// <summary>
		/// Sets the state of a given switch.
		/// </summary>
		/// <param name="slot">Slot number of the switch</param>
		/// <param name="state">New value of the switch</param>
		public void SetSwitch(int slot, bool state) => PinMameApi.SetSwitch(slot, state ? 1 : 0);

		/// <summary>
		/// Returns the maximal supported number of lamps.
		/// </summary>
		/// <returns>Number of lamps</returns>
		public int GetMaxLamps() => PinMameApi.GetMaxLamps();

		/// <summary>
		/// Clears and updates a list with all changed lamps since the last call. <p/>
		/// </summary>
		/// <remarks>
		/// This API allows to perform a GC-free operation. See also <see cref="GetChangedLamps()"/>.
		/// </remarks>
		public void GetChangedLamps(List<PinMameLampInfo> list)
		{
			list.Clear();
			var num = PinMameApi.GetChangedLamps(_changedLamps);

			for (var index = 0; index < num; index++) {
				list.Add(new PinMameLampInfo(_changedLamps[index * 2], _changedLamps[(index * 2) + 1]));
			}
		}

		/// <summary>
		/// Returns an array of all changed lamps since the last call.
		/// </summary>
		/// <remarks>
		/// This call produces a little bit of garbage, since the result array is created on each call.
		/// </remarks>
		/// <returns>Array of Id/Value pairs.</returns>
		public PinMameLampInfo[] GetChangedLamps()
		{
			var num = PinMameApi.GetChangedLamps(_changedLamps);

			PinMameLampInfo[] array = new PinMameLampInfo[num];

			for (int index = 0; index < num; index++) {
				array[index] = new PinMameLampInfo(_changedLamps[index * 2], _changedLamps[(index * 2) + 1]);
			}

			return array;
		}

		/// <summary>
		/// Returns the maximal supported number of GIs.
		/// </summary>
		/// <returns>Number of GIs</returns>
		public int GetMaxGIs() => PinMameApi.GetMaxGIs();

		/// <summary>
		/// Clears and updates a list with all changed GIs since the last call.
		/// </summary>
		/// <remarks>
		/// This API allows to perform a GC-free operation. See also <see cref="GetChangedGIs()"/>.
		/// </remarks>
		/// <param name="list">List to write the result to.</param>
		public void GetChangedGIs(List<PinMameLampInfo> list)
		{
			var num = PinMameApi.GetChangedGIs(_changedGIs);
			for (var index = 0; index < num; index++) {
				list.Add(new PinMameLampInfo(_changedGIs[index * 2], _changedGIs[(index * 2) + 1]));
			}
		}

		/// <summary>
		/// Returns an array of all changed GIs since the last call.
		/// </summary>
		/// <remarks>
		/// This call produces a little bit of garbage, since the result array is created on each call.
		/// </remarks>
		/// <returns>Array of Id/Value pairs.</returns>
		public PinMameLampInfo[] GetChangedGIs()
		{
			var num = PinMameApi.GetChangedGIs(_changedGIs);

			PinMameLampInfo[] array = new PinMameLampInfo[num];

			for (int index = 0; index < num; index++) {
				array[index] = new PinMameLampInfo(_changedGIs[index * 2], _changedGIs[(index * 2) + 1]);
			}

			return array;
		}

		/// <summary>
		/// Returns the maximal supported number of Mechs.
		/// </summary>
		/// <returns>Number of Mechs</returns>
		public int GetMaxMechs() => PinMameApi.GetMaxMechs();

		/// <summary>
		/// Sets the configuration of a given mech.
		/// </summary>
		/// <param name="mechNo">Mech number</param>
		/// <param name="config">Mech configuration. A null value will remove the mech.</param>
		public void SetMech(int mechNo, PinMameMechConfig? config = null)
		{
			PinMameApi.Status status;

			if (config.HasValue)
			{
				var tmpConfig = (PinMameMechConfig)config;

				var mechConfig = new PinMameApi.MechConfig();
				mechConfig.sol1 = tmpConfig.Sol1;
				mechConfig.sol2 = tmpConfig.Sol2;
				mechConfig.type = (int)tmpConfig.Type;
				mechConfig.length = tmpConfig.Length;
				mechConfig.steps = tmpConfig.Steps;
				mechConfig.initialPos = tmpConfig.InitialPos;

				mechConfig.sw = new PinMameApi.MechSwitchConfig[PinMameApi.MaxMechSwitches];

				var index = 0;

				foreach(var switchConfig in tmpConfig.SwitchList)
				{
					mechConfig.sw[index].swNo = switchConfig.SwNo;
					mechConfig.sw[index].startPos = switchConfig.StartPos;
					mechConfig.sw[index].endPos = switchConfig.EndPos;
					mechConfig.sw[index].pulse = switchConfig.Pulse;

					index++;
				}

				status = PinMameApi.SetMech(mechNo, ref mechConfig);
			}
			else
			{
				status = PinMameApi.SetMech(mechNo, IntPtr.Zero);
			}

			if (status != PinMameApi.Status.OK)
			{
				throw new InvalidOperationException($"Unable to set mech, status={status}");
			}
		}
	}
}