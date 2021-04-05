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
		internal enum PinmameStatus
		{
			OK = 0,
			GAME_NOT_FOUND = 1,
			GAME_ALREADY_RUNNING = 2,
			EMULATOR_NOT_RUNNING = 3
		}

		internal enum PinmameDisplayType
		{
			SEG16 = 0,    // 16 segments
			SEG16R = 1,   // 16 segments with comma and period reversed
			SEG10 = 2,    // 9  segments and comma
			SEG9 = 3,     // 9  segments
			SEG8 = 4,     // 7  segments and comma
			SEG8D = 5,    // 7  segments and period
			SEG7 = 6,     // 7  segments
			SEG87 = 7,    // 7  segments, comma every three
			SEG87F = 8,   // 7  segments, forced comma every three
			SEG98 = 9,    // 9  segments, comma every three
			SEG98F = 10,  // 9  segments, forced comma every three
			SEG7S = 11,   // 7  segments, small
			SEG7SC = 12,  // 7  segments, small, with comma
			SEG16S = 13,  // 16 segments with split top and bottom line
			DMD = 14,     // DMD Display
			VIDEO = 15,   // VIDEO Display
			SEG16N = 16,  // 16 segments without commas
			SEG16D = 17   // 16 segments with periods only
		}

		internal delegate void PinmameGameCallback(IntPtr gamePtr);
		internal delegate void PinmameOnStateChangeCallback(int change);
		internal delegate void PinmameOnSolenoidCallback(int solenoid, int isActive);
		internal delegate void PinmameDisplayLayoutCallback(int index, IntPtr displayLayoutPtr);
		internal delegate void PinmameDisplayCallback(int index, IntPtr displayLayoutPtr);

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		internal struct PinmameGame
		{
			internal string name;
			internal string cloneOf;
			internal string description;
			internal string year;
			internal string manufacturer;
		};

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		internal struct PinmameConfig
		{
			internal int sampleRate;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
			internal string vpmPath;
			internal PinmameOnStateChangeCallback onStateChange;
			internal PinmameOnSolenoidCallback onSolenoid;
		};

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		internal struct PinmameDisplayLayout
		{
			internal PinmameDisplayType type;
			internal int top;
			internal int left;
			internal int length;
			internal int width;
			internal int height;
		};

		#region Game library functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void PinmameGetGames(PinmameGameCallback callback);
		#endregion

		#region Setup functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void PinmameSetConfig(ref PinmameConfig config);
		#endregion

		#region Display functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern PinmameStatus PinmameGetDisplayLayouts(PinmameDisplayLayoutCallback callback);

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern PinmameStatus PinmameGetDisplays(byte[] buffer, PinmameDisplayCallback callback);
		#endregion

		#region Game functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern PinmameStatus PinmameRun(string name);

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int PinmameIsRunning();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern PinmameStatus PinmamePause(int pause);

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern PinmameStatus PinmameReset();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void PinmameStop();
		#endregion

		#region Switch related functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int PinmameGetSwitch(int slot);

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void PinmameSetSwitch(int slot, int state);
		#endregion

		#region Lamp related functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int PinmameGetMaxLamps();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int PinmameGetChangedLamps(int[] changedStates);
		#endregion

		#region Solenoid related functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int PinmameGetMaxSolenoids();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int PinmameGetChangedSolenoids(int[] changedStates);
		#endregion

		#region GI strings related functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int PinmameGetMaxGIs();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int PinmameGetChangedGIs(int[] changedStates);
		#endregion
	}
}
