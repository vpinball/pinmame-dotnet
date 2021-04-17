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

		internal enum PinmameDisplayType : int
		{
			SEG16 = 0,                  // 16 segments
			SEG16R = 1,                 // 16 segments with comma and period reversed
			SEG10 = 2,                  // 9 segments and comma
			SEG9 = 3,                   // 9 segments
			SEG8 = 4,                   // 7 segments and comma
			SEG8D = 5,                  // 7 segments and period
			SEG7 = 6,                   // 7 segments
			SEG87 = 7,                  // 7 segments, comma every three
			SEG87F = 8,                 // 7 segments, forced comma every three
			SEG98 = 9,                  // 9 segments, comma every three
			SEG98F = 10,                // 9 segments, forced comma every three
			SEG7S = 11,                 // 7 segments, small
			SEG7SC = 12,                // 7 segments, small, with comma
			SEG16S = 13,                // 16 segments with split top and bottom line
			DMD = 14,                   // DMD Display
			VIDEO = 15,                 // VIDEO Display
			SEG16N = 16,                // 16 segments without commas
			SEG16D = 17,                // 16 segments with periods only
			SEGALL = 0x1f,              // maximum segment definition number
			IMPORT = 0x20,              // Link to another display layout
			SEGMASK = 0x3f,             // Note that CORE_IMPORT must be part of the segmask as well!
			SEGHIBIT = 0x40,
			SEGREV = 0x80,
			DMDNOAA = 0x100,
			NODISP = 0x200,
			SEG8H = SEG8 | SEGHIBIT,
			SEG7H = SEG7 | SEGHIBIT,
			SEG87H = SEG87 | SEGHIBIT,
			SEG87FH = SEG87F| SEGHIBIT,
			SEG7SH = SEG7S | SEGHIBIT,
			SEG7SCH = SEG7SC| SEGHIBIT
		}

		internal enum PinmameHardwareGen : ulong
		{
			WPCALPHA_1 = 0x0000000000001,  // Alpha-numeric display S11 sound, Dr Dude 10/90
			WPCALPHA_2 = 0x0000000000002,  // Alpha-numeric display,  - The Machine BOP 4/91
			WPCDMD = 0x0000000000004,      // Dot Matrix Display, Terminator 2 7/91 - Party Zone 10/91
			WPCFLIPTRON = 0x0000000000008, // Fliptronic flippers, Addams Family 2/92 - Twilight Zone 5/93
			WPCDCS = 0x0000000000010,      // DCS Sound system, Indiana Jones 10/93 - Popeye 3/94
			WPCSECURITY = 0x0000000000020, // Security chip, World Cup Soccer 3/94 - Jackbot 10/95
			WPC95DCS = 0x0000000000040,    // Hybrid WPC95 driver + DCS sound, Who Dunnit
			WPC95 = 0x0000000000080,       // Integrated boards, Congo 3/96 - Cactus Canyon 2/99
			S11 = 0x0000080000000,         // No external sound board
			S11X = 0x0000000000100,        // S11C sound board
			S11A = S11X,
			S11B = S11X,
			S11B2 = 0x0000000000200,       // Jokerz! sound board
			S11C = 0x0000000000400,        // No CPU board sound
			S9 = 0x0000000000800,          // S9 CPU, 4x7+1x4
			DE = 0x0000000001000,          // DE AlphaSeg
			DEDMD16 = 0x0000000002000,     // DE 128x16
			DEDMD32 = 0x0000000004000,     // DE 128x32
			DEDMD64 = 0x0000000008000,     // DE 192x64
			S7 = 0x0000000010000,          // S7 CPU
			S6 = 0x0000000020000,          // S6 CPU
			S4 = 0x0000000040000,          // S4 CPU
			S3C = 0x0000000080000,         // S3 CPU No Chimes
			S3 = 0x0000000100000,
			BY17 = 0x0000000200000,
			BY35 = 0x0000000400000,
			STMPU100 = 0x0000000800000,    // Stern MPU - 100
			STMPU200 = 0x0000001000000,    // Stern MPU - 200
			ASTRO = 0x0000002000000,       // Unknown Astro game, Stern hardware
			HNK = 0x0000004000000,         // Hankin
			BYPROTO = 0x0000008000000,     // Bally Bow & Arrow prototype
			BY6803 = 0x0000010000000,
			BY6803A = 0x0000020000000,
			BOWLING = 0x0000040000000,     // Big Ball Bowling, Stern hardware
			GTS1 = 0x0000100000000,        // GTS1
			GTS80 = 0x0000200000000,       // GTS80
			GTS80A = GTS80,
			GTS80B = 0x0000400000000,      // GTS80B
			WS = 0x0004000000000,          // Whitestar
			WS_1 = 0x0008000000000,        // Whitestar with extra RAM
			WS_2 = 0x0010000000000,        // Whitestar with extra DMD
			GTS3 = 0x0020000000000,        // GTS3
			ZAC1 = 0x0040000000000,
			ZAC2 = 0x0080000000000,
			SAM = 0x0100000000000,         // Stern SAM
			ALVG = 0x0200000000000,        // Alvin G Hardware
			ALVG_DMD2 = 0x0400000000000,   // Alvin G Hardware, with more shades
			MRGAME = 0x0800000000000,      // Mr.Game Hardware
			SLEIC = 0x1000000000000,       // Sleic Hardware
			WICO = 0x2000000000000,        // Wico Hardware
			SPA = 0x4000000000000,         // Stern PA
			ALLWPC = 0x00000000000ff,      // All WPC
			ALLS11 = 0x000008000ff00,      // All Sys11
			ALLBY35 = 0x0000047e00000,     // All Bally35 and derivatives
			ALLS80 = 0x0000600000000,      // All GTS80
			ALLWS = 0x001c000000000        // All Whitestar
		}

		internal delegate void PinmameGameCallback(IntPtr gamePtr);
		internal delegate void PinmameOnStateUpdatedCallback(int change);
		internal delegate void PinmameOnDisplayUpdatedCallback(int index, IntPtr framePtr, ref PinmameDisplayLayout displayLayout);
		internal delegate void PinmameOnSolenoidUpdatedCallback(int solenoid, int isActive);

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
			internal PinmameOnStateUpdatedCallback onStateUpdated;
			internal PinmameOnDisplayUpdatedCallback onDisplayUpdated;
			internal PinmameOnSolenoidUpdatedCallback onSolenoidUpdated;
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
			internal int depth;
		};

		#region Game library functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void PinmameGetGames(PinmameGameCallback callback);
		#endregion

		#region Setup functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void PinmameSetConfig(ref PinmameConfig config);
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

		#region Hardware related functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern PinmameHardwareGen PinmameGetHardwareGen();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int PinmameGetDisplayCount();
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

		#region GI strings related functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int PinmameGetMaxGIs();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int PinmameGetChangedGIs(int[] changedStates);
		#endregion
	}
}
