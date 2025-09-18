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

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

using System.Collections.Generic;

namespace PinMame
{
	using System;
	using System.Runtime.InteropServices;
	using Interop;

	internal static class PinMameApi
	{
		internal const int MaxMechSwitches = 20;

		#region Enums

		internal enum LogLevel
		{
			DEBUG = 0,
			INFO = 1,
			ERROR = 2
		}

		internal enum Status
		{
			OK = 0,
			CONFIG_NOT_SET = 1,
			GAME_NOT_FOUND = 2,
			GAME_ALREADY_RUNNING = 3,
			EMULATOR_NOT_RUNNING = 4,
			MECH_HANDLE_MECHANICS = 5,
			MECH_NO_INVALID = 6
		}

		internal enum FileType
		{
			ROMS = 0,
			NVRAM = 1,
			SAMPLES = 2,
			CONFIG = 3,
			HIGHSCORE = 4
		}

		internal enum DmdMode
		{
			BRIGHTNESS = 0,
			RAW = 1
		}

		internal enum SoundMode
		{
			DEFAULT = 0,
			ALTSOUND = 1
		}

		internal enum AudioFormat : int
		{
			INT16 = 0,
			FLOAT = 1
		}

		internal enum DisplayType : int
		{
			/// <summary>
			/// 16 segments
			/// </summary>
			SEG16 = 0,
			/// <summary>
			/// 16 segments with comma and period reversed
			/// </summary>
			SEG16R = 1,
			/// <summary>
			/// 9 segments and comma
			/// </summary>
			SEG10 = 2,
			/// <summary>
			/// 9 segments
			/// </summary>
			SEG9 = 3,
			/// <summary>
			/// 7 segments and comma
			/// </summary>
			SEG8 = 4,
			/// <summary>
			/// 7 segments and period
			/// </summary>
			SEG8D = 5,
			/// <summary>
			/// 7 segments
			/// </summary>
			SEG7 = 6,
			/// <summary>
			/// 7 segments, comma every three
			/// </summary>
			SEG87 = 7,
			/// <summary>
			/// 7 segments, forced comma every three
			/// </summary>
			SEG87F = 8,
			/// <summary>
			/// 9 segments, comma every three
			/// </summary>
			SEG98 = 9,
			/// <summary>
			/// 9 segments, forced comma every three
			/// </summary>
			SEG98F = 10,
			/// <summary>
			/// 7 segments, small
			/// </summary>
			SEG7S = 11,
			/// <summary>
			/// 7 segments, small, with comma
			/// </summary>
			SEG7SC = 12,
			/// <summary>
			/// 16 segments with split top and bottom line
			/// </summary>
			SEG16S = 13,
			/// <summary>
			/// DMD Display
			/// </summary>
			DMD = 14,
			/// <summary>
			/// VIDEO Display
			/// </summary>
			VIDEO = 15,
			/// <summary>
			/// 16 segments without commas
			/// </summary>
			SEG16N = 16,
			/// <summary>
			/// 16 segments with periods only
			/// </summary>
			SEG16D = 17,
			/// <summary>
			/// maximum segment definition number
			/// </summary>
			SEGALL = 0x1f,
			/// <summary>
			/// Link to another display layout
			/// </summary>
			IMPORT = 0x20,
			/// <summary>
			/// Note that CORE_IMPORT must be part of the segmask as well!
			/// </summary>
			SEGMASK = 0x3f,             
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

		internal enum ModOutputType
		{
			/// <summary>
			/// Solenoid output type
			/// </summary>
			SOLENOID = 0,
			/// <summary>
			/// Lamp output type
			/// </summary>
			LAMP = 1,
			/// <summary>
			/// Global Illumination output type
			/// </summary>
			GI = 2,
			/// <summary>
			/// Alpha Numeric segment output type
			/// </summary>
			ALPHASEG = 3,  
		}

		[Flags]
		internal enum HardwareGen : ulong
		{
			/// <summary>
			/// Alpha-numeric display S11 sound, Dr Dude 10/90
			/// </summary>
			WPCALPHA_1 = 0x0000000000001,
			/// <summary>
			/// Alpha-numeric display,  - The Machine BOP 4/91
			/// </summary>
			WPCALPHA_2 = 0x0000000000002,
			/// <summary>
			/// Dot Matrix Display, Terminator 2 7/91 - Party Zone 10/91
			/// </summary>
			WPCDMD = 0x0000000000004,
			/// <summary>
			/// Fliptronic flippers, Addams Family 2/92 - Twilight Zone 5/93
			/// </summary>
			WPCFLIPTRON = 0x0000000000008,
			/// <summary>
			/// DCS Sound system, Indiana Jones 10/93 - Popeye 3/94
			/// </summary>
			WPCDCS = 0x0000000000010,
			/// <summary>
			/// Security chip, World Cup Soccer 3/94 - Jackbot 10/95
			/// </summary>
			WPCSECURITY = 0x0000000000020,
			/// <summary>
			/// Hybrid WPC95 driver + DCS sound, Who Dunnit
			/// </summary>
			WPC95DCS = 0x0000000000040,
			/// <summary>
			/// Integrated boards, Congo 3/96 - Cactus Canyon 2/99
			/// </summary>
			WPC95 = 0x0000000000080,
			/// <summary>
			/// No external sound board
			/// </summary>
			S11 = 0x0000080000000,
			/// <summary>
			/// S11C sound board
			/// </summary>
			S11X = 0x0000000000100,        
			S11A = S11X,
			S11B = S11X,
			/// <summary>
			/// Jokerz! sound board
			/// </summary>
			S11B2 = 0x0000000000200,
			/// <summary>
			/// No CPU board sound
			/// </summary>
			S11C = 0x0000000000400,
			/// <summary>
			/// S9 CPU, 4x7+1x4
			/// </summary>
			S9 = 0x0000000000800,
			/// <summary>
			/// DE AlphaSeg
			/// </summary>
			DE = 0x0000000001000,
			/// <summary>
			/// DE 128x16
			/// </summary>
			DEDMD16 = 0x0000000002000,
			/// <summary>
			/// DE 128x32
			/// </summary>
			DEDMD32 = 0x0000000004000,
			/// <summary>
			/// DE 192x64
			/// </summary>
			DEDMD64 = 0x0000000008000,
			/// <summary>
			/// S7 CPU
			/// </summary>
			S7 = 0x0000000010000,
			/// <summary>
			/// S6 CPU
			/// </summary>
			S6 = 0x0000000020000,
			/// <summary>
			/// S4 CPU
			/// </summary>
			S4 = 0x0000000040000,
			/// <summary>
			/// S3 CPU No Chimes
			/// </summary>
			S3C = 0x0000000080000,         
			S3 = 0x0000000100000,
			BY17 = 0x0000000200000,
			BY35 = 0x0000000400000,
			/// <summary>
			/// Stern MPU - 100
			/// </summary>
			STMPU100 = 0x0000000800000,
			/// <summary>
			/// Stern MPU - 200
			/// </summary>
			STMPU200 = 0x0000001000000,
			/// <summary>
			/// Unknown Astro game, Stern hardware
			/// </summary>
			ASTRO = 0x0000002000000,
			/// <summary>
			/// Hankin
			/// </summary>
			HNK = 0x0000004000000,
			/// <summary>
			/// Bally Bow & Arrow prototype
			/// </summary>
			BYPROTO = 0x0000008000000,     
			BY6803 = 0x0000010000000,
			BY6803A = 0x0000020000000,
			/// <summary>
			/// Big Ball Bowling, Stern hardware
			/// </summary>
			BOWLING = 0x0000040000000,
			/// <summary>
			/// GTS1
			/// </summary>
			GTS1 = 0x0000100000000,
			/// <summary>
			/// GTS80
			/// </summary>
			GTS80 = 0x0000200000000,       
			GTS80A = GTS80,
			/// <summary>
			/// GTS80B
			/// </summary>
			GTS80B = 0x0000400000000,
			/// <summary>
			/// Whitestar
			/// </summary>
			WS = 0x0004000000000,
			/// <summary>
			/// Whitestar with extra RAM
			/// </summary>
			WS_1 = 0x0008000000000,
			/// <summary>
			/// Whitestar with extra DMD
			/// </summary>
			WS_2 = 0x0010000000000,
			/// <summary>
			/// GTS3
			/// </summary>
			GTS3 = 0x0020000000000,        
			ZAC1 = 0x0040000000000,
			ZAC2 = 0x0080000000000,
			/// <summary>
			/// Stern SAM
			/// </summary>
			SAM = 0x0100000000000,
			/// <summary>
			/// Alvin G Hardware
			/// </summary>
			ALVG = 0x0200000000000,
			/// <summary>
			/// Alvin G Hardware, with more shades
			/// </summary>
			ALVG_DMD2 = 0x0400000000000,
			/// <summary>
			/// Mr.Game Hardware
			/// </summary>
			MRGAME = 0x0800000000000,
			/// <summary>
			/// Sleic Hardware
			/// </summary>
			SLEIC = 0x1000000000000,
			/// <summary>
			/// Wico Hardware
			/// </summary>
			WICO = 0x2000000000000,
			/// <summary>
			/// Stern PA
			/// </summary>
			SPA = 0x4000000000000,
			/// <summary>
			/// All WPC
			/// </summary>
			ALLWPC = 0x00000000000ff,
			/// <summary>
			/// All Sys11
			/// </summary>
			ALLS11 = 0x000008000ff00,
			/// <summary>
			/// All Bally35 and derivatives
			/// </summary>
			ALLBY35 = 0x0000047e00000,
			/// <summary>
			/// All GTS80
			/// </summary>
			ALLS80 = 0x0000600000000,
			/// <summary>
			/// All Whitestar
			/// </summary>
			ALLWS = 0x001c000000000        
		}

		internal enum GameDriverFlag : uint
		{
			ORIENTATION_MASK = 0x0007,
			/// <summary>
			/// mirror everything in the X direction
			/// </summary>
			ORIENTATION_FLIP_X = 0x0001,
			/// <summary>
			/// mirror everything in the Y direction
			/// </summary>
			ORIENTATION_FLIP_Y = 0x0002,
			/// <summary>
			/// mirror along the top-left/bottom-right diagonal
			/// </summary>
			ORIENTATION_SWAP_XY = 0x0004,         
			GAME_NOT_WORKING = 0x0008,
			/// <summary>
			/// game's protection not fully emulated
			/// </summary>
			GAME_UNEMULATED_PROTECTION = 0x0010,
			/// <summary>
			/// colors are totally wrong
			/// </summary>
			GAME_WRONG_COLORS = 0x0020,
			/// <summary>
			/// colors are not 100% accurate, but close
			/// </summary>
			GAME_IMPERFECT_COLORS = 0x0040,
			/// <summary>
			/// graphics are wrong/incomplete
			/// </summary>
			GAME_IMPERFECT_GRAPHICS = 0x0080,
			/// <summary>
			/// screen flip support is missing
			/// </summary>
			GAME_NO_COCKTAIL = 0x0100,
			/// <summary>
			/// sound is missing
			/// </summary>
			GAME_NO_SOUND = 0x0200,
			/// <summary>
			/// sound is known to be wrong
			/// </summary>
			GAME_IMPERFECT_SOUND = 0x0400,
			/// <summary>
			/// set by the fake "root" driver_0 and by "containers"
			/// </summary>
			NOT_A_DRIVER = 0x4000,                
		}

		internal enum MechFlag : uint
		{
			LINEAR = 0x00,
			NONLINEAR = 0x01,
			CIRCLE = 0x00,
			STOPEND = 0x02,
			REVERSE = 0x04,
			ONESOL = 0x00,
			ONEDIRSOL = 0x10,
			TWODIRSOL = 0x20,
			TWOSTEPSOL = 0x40,
			FOURSTEPSOL = (TWODIRSOL | TWOSTEPSOL),
			SLOW = 0x00,
			FAST = 0x80,
			STEPSW = 0x00,
			LENGTHSW = 0x100
		}

		internal enum Keycode
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
			NUMBER_0 = 26,
			NUMBER_1 = 27,
			NUMBER_2 = 28,
			NUMBER_3 = 29,
			NUMBER_4 = 30,
			NUMBER_5 = 31,
			NUMBER_6 = 32,
			NUMBER_7 = 33,
			NUMBER_8 = 34,
			NUMBER_9 = 35,
			KEYPAD_0 = 36,
			KEYPAD_1 = 37,
			KEYPAD_2 = 38,
			KEYPAD_3 = 39,
			KEYPAD_4 = 40,
			KEYPAD_5 = 41,
			KEYPAD_6 = 42,
			KEYPAD_7 = 43,
			KEYPAD_8 = 44,
			KEYPAD_9 = 45,
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
			ESCAPE = 58,
			GRAVE_ACCENT = 59,
			MINUS = 60,
			EQUALS = 61,
			BACKSPACE = 62,
			TAB = 63,
			LEFT_BRACKET = 64,
			RIGHT_BRACKET = 65,
			ENTER = 66,
			SEMICOLON = 67,
			QUOTE = 68,
			BACKSLASH = 69,
			COMMA = 71,
			PERIOD = 72,
			SLASH = 73,
			SPACE = 74,
			INSERT = 75,
			DELETE = 76,
			HOME = 77,
			END = 78,
			PAGE_UP = 79,
			PAGE_DOWN = 80,
			LEFT = 81,
			RIGHT = 82,
			UP = 83,
			DOWN = 84,
			KEYPAD_DIVIDE = 85,
			KEYPAD_MULTIPLY = 86,
			KEYPAD_SUBTRACT = 87,
			KEYPAD_ADD = 88,
			KEYPAD_ENTER = 90,
			PRINT_SCREEN = 91,
			PAUSE = 92,
			LEFT_SHIFT = 93,
			RIGHT_SHIFT = 94,
			LEFT_CONTROL = 95,
			RIGHT_CONTROL = 96,
			LEFT_ALT = 97,
			RIGHT_ALT = 98,
			SCROLL_LOCK = 99,
			NUM_LOCK = 100,
			CAPS_LOCK = 101,
			LEFT_SUPER = 102,
			RIGHT_SUPER = 103,
			MENU = 104,
		}

		#endregion

		#region Structs

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		internal readonly struct Game
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
		internal readonly struct DisplayLayout
		{
			internal readonly DisplayType type;
			internal readonly int top;
			internal readonly int left;
			internal readonly int length;
			internal readonly int width;
			internal readonly int height;
			internal readonly int depth;
		};

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		internal readonly struct AudioInfo
		{
			internal readonly AudioFormat format;
			internal readonly int channels;
			internal readonly double sampleRate;
			internal readonly double framesPerSecond;
			internal readonly int samplesPerFrame;
			internal readonly int bufferSize;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		internal readonly struct SwitchState
		{
			internal readonly int swNo;
			internal readonly int state;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		internal readonly struct SolenoidState
		{
			internal readonly int solNo;
			internal readonly int state;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		internal readonly struct LampState
		{
			internal readonly int lampNo;
			internal readonly int state;
		}
		
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		internal readonly struct GIState
		{
			internal readonly int giNo;
			internal readonly int state;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		internal readonly struct LEDState
		{
			internal readonly int swNo;
			internal readonly int startPos;
			internal readonly int endPos;
			internal readonly int pulse;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		internal struct MechSwitchConfig
		{
			internal int swNo;
			internal int startPos;
			internal int endPos;
			internal int pulse;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		internal struct MechConfig
		{
			internal int type;
			internal int sol1;
			internal int sol2;
			internal int length;
			internal int steps;
			internal int initialPos;
			internal int acc;
			internal int ret;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxMechSwitches)]
			internal MechSwitchConfig[] sw;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		internal readonly struct MechInfo
		{
			internal readonly int type;
			internal readonly int length;
			internal readonly int steps;
			internal readonly int pos;
			internal readonly int speed;
		};

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		internal readonly struct SoundCommand
		{
			internal readonly int sndNo;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		internal readonly struct NVRAMState
		{
			internal readonly int nvramNo;
			internal readonly byte oldStat;
			internal readonly byte currStat;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		internal readonly struct KeyboardInfo
		{
			internal readonly string name;
			internal readonly Keycode code;
			internal readonly uint standardcode;
		}

		#endregion

		#region Delegates

		internal delegate void GameCallback(IntPtr gamePtr);
		internal delegate void OnStateUpdatedCallback(int change);
		internal delegate void OnDisplayAvailableCallback(int index, int displayCount, ref DisplayLayout displayLayout);
		internal delegate void OnDisplayUpdatedCallback(int index, IntPtr framePtr, ref DisplayLayout displayLayout);
		internal delegate int OnAudioAvailableCallback(ref AudioInfo audioInfo);
		internal delegate int OnAudioUpdatedCallback(IntPtr bufferPtr, int samples);
		internal delegate void OnMechAvailableCallback(int mechNo, ref MechInfo mechInfo);
		internal delegate void OnMechUpdatedCallback(int mechNo, ref MechInfo mechInfo);
		internal delegate void OnSolenoidUpdatedCallback(int solenoid, int isActive);
		internal delegate void OnConsoleDataUpdatedCallback(IntPtr dataPtr, int size);
		internal delegate int IsKeyPressedFunction(Keycode keycode);
		internal delegate void OnLogMessageCallback(LogLevel logLevel, string format, string args);
		internal delegate void OnSoundCommandCallback(int boardNo, int cmd);

		#endregion

		#region Config Struct

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		internal struct Config
		{
			internal AudioFormat audioFormat;
			internal int sampleRate;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
			internal string vpmPath;
			internal OnStateUpdatedCallback onStateUpdated;
			internal OnDisplayAvailableCallback onDisplayAvailable;
			internal OnDisplayUpdatedCallback onDisplayUpdated;
			internal OnAudioAvailableCallback onAudioAvailable;
			internal OnAudioUpdatedCallback onAudioUpdated;
			internal OnMechAvailableCallback onMechAvailable;
			internal OnMechUpdatedCallback onMechUpdated;
			internal OnSolenoidUpdatedCallback onSolenoidUpdated;
			internal OnConsoleDataUpdatedCallback onConsoleDataUpdated;
			internal IsKeyPressedFunction isKeyPressed;
			internal OnLogMessageCallback onLogMessage;
			internal OnSoundCommandCallback onSoundCommand;
		};

		#endregion

		#region Setup functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameSetConfig")]
		internal static extern void SetConfig(ref Config config);

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameSetPath")]
		internal static extern void SetPath(ref FileType fileType, string path);
		#endregion

		#region Game library functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameGetGame")]
		internal static extern Status GetGame(string name, GameCallback callback);

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameGetGames")]
		internal static extern Status GetGames(GameCallback callback);
		#endregion

		#region Options related functions

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameGetCheat")]
		internal static extern int GetCheat();
		
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameSetCheat")]
		internal static extern int SetCheat(int cheat);

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameGetHandleKeyboard")]
		internal static extern int GetHandleKeyboard();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameSetHandleKeyboard")]
		internal static extern void SetHandleKeyboard(int handleKeyboard);

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameGetHandleMechanics")]
		internal static extern int GetHandleMechanics();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameSetHandleMechanics")]
		internal static extern void SetHandleMechanics(int handleMechanics);
		#endregion

		#region Game functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameRun")]
		internal static extern Status Run(string name);

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameIsRunning")]
		internal static extern int IsRunning();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmamePause")]
		internal static extern Status Pause(int pause);

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameIsPaused")]
		internal static extern int IsPaused();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameReset")]
		internal static extern Status Reset();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameStop")]
		internal static extern void Stop();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameSetTimeFence")]
		internal static extern void SetTimeFence(double timeInS);
		#endregion

		#region Hardware related functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameGetHardwareGen")]
		internal static extern HardwareGen GetHardwareGen();
		#endregion

		#region Display and sound modes

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameGetDmdMode")]
		internal static extern DmdMode GetDmdMode();
		
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameSetDmdMode")]
		internal static extern void SetDmdMode(DmdMode dmdMode);
		
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameGetSoundMode")]
		internal static extern SoundMode GetSoundMode();
		
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameSetSoundMode")]
		internal static extern void SetSoundMode(SoundMode dmdMode);

		#endregion

		#region Switch related functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameGetSwitch")]
		internal static extern int GetSwitch(int slot);

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameSetSwitch")]
		internal static extern void SetSwitch(int slot, int state);

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameSetSwitches")]
		internal static extern void SetSwitches(SwitchState[] states, int state, int numSwitches);
		#endregion

		#region Lamp related functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameGetMaxLamps")]
		internal static extern int GetMaxLamps();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameGetChangedLamps")]
		internal static extern int GetChangedLamps(int[] changedStates);

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameGetMaxLEDs")]
		internal static extern int GetMaxLEDs();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameGetChangedLEDs")]
		internal static extern int GetChangedLEDs(LEDState state, ulong mask, ulong mask2);
		#endregion

		#region Solenoid related functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameGetSolenoidMask")]
		internal static extern uint GetSolenoidMask(int low, uint mask);
		
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameGetMaxSolenoids")]
		internal static extern int GetMaxSolenoids();
		
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameGetModOutputType")]
		internal static extern ModOutputType GetModOutputType(int output, int no);
		
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameSetModOutputType")]
		internal static extern void SetModOutputType(int output, int no, ModOutputType type);
		#endregion

		#region GI strings related functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameGetMaxGIs")]
		internal static extern int GetMaxGIs();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameGetChangedGIs")]
		internal static extern int GetChangedGIs(int[] changedStates);

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameGetGI")]
		internal static extern int GetGI(int giNo);
		#endregion

		#region Mech related functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameGetMaxMechs")]
		internal static extern int GetMaxMechs();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameSetMech")]
		internal static extern Status SetMech(int mechNo, ref MechConfig mechConfig);

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameSetMech")]
		internal static extern Status SetMech(int mechNo, IntPtr mechConfig);

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameGetMech")]
		internal static extern int GetMech(int mechNo);

		#endregion

		#region Sound related functions

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameGetMaxSoundCommands")]
		internal static extern int GetMaxSoundCommands();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameGetNewSoundCommands")]
		internal static extern int GetNewSoundCommands(ref SoundCommand newCommand);

		#endregion

		#region DIP related functions

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameGetDIP")]
		internal static extern int GetDIP(int dipBank);

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameSetDIP")]
		internal static extern void SetDIP(int dipBank, int value);

		#endregion

		#region NVRAM related function

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameGetNVRAM")]
		internal static extern int GetNVRAM(NVRAMState[] states);

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "PinmameGetChangedNVRAM")]
		internal static extern void GetChangedNVRAM(NVRAMState[] states);

		#endregion

		[Obsolete("Not in libpinmame.h anymore")]
		internal struct PinmameDmdLevels
		{
			internal static readonly Dictionary<byte, byte> Wpc = new Dictionary<byte, byte> {
				{ 0x00, 0 },
				{ 0x14, 0 },
				{ 0x21, 1 },
				{ 0x43, 2 },
				{ 0x64, 3 },
			};

			internal static readonly Dictionary<byte, byte> Sam = new Dictionary<byte, byte> {
				{ 0x00, 0 }, { 0x14, 1 }, { 0x19, 2 }, { 0x1E, 3 },
				{ 0x23, 4 }, { 0x28, 5 }, { 0x2D, 6 }, { 0x32, 7 },
				{ 0x37, 8 }, { 0x3C, 9 }, { 0x41, 10 }, { 0x46, 11 },
				{ 0x4B, 12 }, { 0x50, 13 }, { 0x5A, 14 }, { 0x64, 15 }
			};

			internal static readonly Dictionary<byte, byte> Gts3 = new Dictionary<byte, byte> {
				{ 0x00, 0 }, { 0x1E, 1 }, { 0x23, 2 }, { 0x28, 3 },
				{ 0x2D, 4 }, { 0x32, 5 }, { 0x37, 6 }, { 0x3C, 7 },
				{ 0x41, 8 }, { 0x46, 9 }, { 0x4B, 10 }, { 0x50, 11 },
				{ 0x55, 12 }, { 0x5A, 13 }, { 0x5F, 14 }, { 0x64, 15 }
			};
		}
	}
}