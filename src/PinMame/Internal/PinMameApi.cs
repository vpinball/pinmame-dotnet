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

namespace PinMame.Internal
{
	using System;
	using System.Runtime.InteropServices;
	using Interop;

	internal static class PinMameApi
	{
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		internal struct GameInfoStruct
		{
			internal string name;
			internal string cloneOf;
			internal string description;
			internal string year;
			internal string manufacturer;
		};

		#region Callbacks
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void GameInfoCallback(IntPtr gameInfoStructPtr);
		#endregion

		#region Setup functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void SetVPMPath(string path);

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void SetSampleRate(int sampleRate);
		#endregion

		#region Game functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void GetGames(GameInfoCallback callback);

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int StartThreadedGame(string gameName, bool showConsole);

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void StopThreadedGame(bool locking);

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool IsGameReady();
		#endregion

		#region Pause related functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void ResetGame();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void Pause();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void Continue();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool IsPaused();
		#endregion

		#region DMD related functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool NeedsDMDUpdate();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int GetRawDMDWidth();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int GetRawDMDHeight();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int GetRawDMDPixels(byte[] buffer);
		#endregion

		#region Audio related functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int GetAudioChannels();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int GetPendingAudioSamples(float[] buffer, int outChannels, int maxNumber);
		#endregion

		#region Switch related functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool GetSwitch(int slot);

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void SetSwitch(int slot, bool state);
		#endregion

		#region Lamps related functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int GetMaxLamps();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int GetChangedLamps(int[] changedStates);
		#endregion

		#region Solenoids related functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int GetMaxSolenoids();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int GetChangedSolenoids(int[] changedStates);
		#endregion

		#region GI strings related functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int GetMaxGIStrings();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int GetChangedGIs(int[] changedStates);
		#endregion
	}
}
