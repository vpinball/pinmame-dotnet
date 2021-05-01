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

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace PinMame
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

		[Flags]
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

		internal enum PinmameGameDriverFlag : uint
		{
			ORIENTATION_MASK = 0x0007,
			ORIENTATION_FLIP_X = 0x0001,          // mirror everything in the X direction
			ORIENTATION_FLIP_Y = 0x0002,          // mirror everything in the Y direction
			ORIENTATION_SWAP_XY = 0x0004,         // mirror along the top-left/bottom-right diagonal
			GAME_NOT_WORKING = 0x0008,
			GAME_UNEMULATED_PROTECTION = 0x0010,  // game's protection not fully emulated
			GAME_WRONG_COLORS = 0x0020,           // colors are totally wrong
			GAME_IMPERFECT_COLORS = 0x0040,       // colors are not 100% accurate, but close
			GAME_IMPERFECT_GRAPHICS = 0x0080,     // graphics are wrong/incomplete
			GAME_NO_COCKTAIL = 0x0100,            // screen flip support is missing
			GAME_NO_SOUND = 0x0200,               // sound is missing
			GAME_IMPERFECT_SOUND = 0x0400,        // sound is known to be wrong
			NOT_A_DRIVER = 0x4000,                // set by the fake "root" driver_0 and by "containers"
		}

		internal enum PinmameKeycode
		{
			A = 0,
			B = 1,
			C = 2,
			D = 3,
			E = 4,
			F = 5,
			G = 6,
			H = 7,
			I = 8,
			J = 9,
			K = 10,
			L = 11,
			M = 12,
			N = 13,
			O = 14,
			P = 15,
			Q = 16,
			R = 17,
			S = 18,
			T = 19,
			U = 20,
			V = 21,
			W = 22,
			X = 23,
			Y = 24,
			Z = 25,
			NUM0 = 26,
			NUM1 = 27,
			NUM2 = 28,
			NUM3 = 29,
			NUM4 = 30,
			NUM5 = 31,
			NUM6 = 32,
			NUM7 = 33,
			NUM8 = 34,
			NUM9 = 35,
			NUM0_PAD = 36,
			NUM1_PAD = 37,
			NUM2_PAD = 38,
			NUM3_PAD = 39,
			NUM4_PAD = 40,
			NUM5_PAD = 41,
			NUM6_PAD = 42,
			NUM7_PAD = 43,
			NUM8_PAD = 44,
			NUM9_PAD = 45,
			F1 = 46,
			F2 = 47,
			F3 = 48,
			F4 = 49,
			F5 = 50,
			F6 = 51,
			F7 = 52,
			F8 = 53,
			F9 = 54,
			F10 = 55,
			F11 = 56,
			F12 = 57,
			ESC = 58,
			TILDE = 59,
			MINUS = 60,
			EQUALS = 61,
			BACKSPACE = 62,
			TAB = 63,
			OPENBRACE = 64,
			CLOSEBRACE = 65,
			ENTER = 66,
			COLON = 67,
			QUOTE = 68,
			BACKSLASH = 69,
			BACKSLASH2 = 70,
			COMMA = 71,
			STOP = 72,
			SLASH = 73,
			SPACE = 74,
			INSERT = 75,
			DEL = 76,
			HOME = 77,
			END = 78,
			PGUP = 79,
			PGDN = 80,
			LEFT = 81,
			RIGHT = 82,
			UP = 83,
			DOWN = 84,
			SLASH_PAD = 85,
			ASTERISK = 86,
			MINUS_PAD = 87,
			PLUS_PAD = 88,
			DEL_PAD = 89,
			ENTER_PAD = 90,
			PRTSCR = 91,
			PAUSE = 92,
			LSHIFT = 93,
			RSHIFT = 94,
			LCONTROL = 95,
			RCONTROL = 96,
			LALT = 97,
			RALT = 98,
			SCRLOCK = 99,
			NUMLOCK = 100,
			CAPSLOCK = 101,
			LWIN = 102,
			RWIN = 103,
			MENU = 104,
		}

		internal struct PinmameDmdLevels
		{
			internal static readonly byte[] Wpc = {
				20, 33, 67, 100
			};

			internal static readonly byte[] Sam =
			{
				0, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 90, 100
			};

			internal static readonly byte[] Gts3 =
			{
				0, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100
			};
		}

		internal delegate void PinmameGameCallback(IntPtr gamePtr);
		internal delegate void PinmameOnStateUpdatedCallback(int change);
		internal delegate void PinmameOnDisplayAvailableCallback(int index, int displayCount, ref PinmameDisplayLayout displayLayout);
		internal delegate void PinmameOnDisplayUpdatedCallback(int index, IntPtr framePtr, ref PinmameDisplayLayout displayLayout);
		internal delegate void PinmameOnSolenoidUpdatedCallback(int solenoid, int isActive);
		internal delegate int PinmameIsKeyPressedFunction(PinmameKeycode keycode);

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		internal readonly struct PinmameGame
		{
			internal readonly string name;
			internal readonly string cloneOf;
			internal readonly string description;
			internal readonly string year;
			internal readonly string manufacturer;
			internal readonly uint flags;
			internal readonly int found;
		};

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		internal struct PinmameConfig
		{
			internal int sampleRate;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
			internal string vpmPath;
			internal PinmameOnStateUpdatedCallback onStateUpdated;
			internal PinmameOnDisplayAvailableCallback onDisplayAvailable;
			internal PinmameOnDisplayUpdatedCallback onDisplayUpdated;
			internal PinmameOnSolenoidUpdatedCallback onSolenoidUpdated;
			internal PinmameIsKeyPressedFunction isKeyPressed;
		};

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		internal readonly struct PinmameDisplayLayout
		{
			internal readonly PinmameDisplayType type;
			internal readonly int top;
			internal readonly int left;
			internal readonly int length;
			internal readonly int width;
			internal readonly int height;
			internal readonly int depth;
		};

		#region Setup functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void PinmameSetConfig(ref PinmameConfig config);
		#endregion

		#region Game library functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern PinmameStatus PinmameGetGame(string name, PinmameGameCallback callback);

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		internal static extern PinmameStatus PinmameGetGames(PinmameGameCallback callback);
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
