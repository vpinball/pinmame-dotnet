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

// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
// ReSharper disable CommentTypo

using System;

namespace PinMame
{
	[Flags]
	public enum PinMameAudioFormat
	{
		AudioFormatInt16 = PinMameApi.PinmameAudioFormat.AUDIO_FORMAT_INT16,
		AudioFormatFloat = PinMameApi.PinmameAudioFormat.AUDIO_FORMAT_FLOAT
	}

	[Flags]
	public enum PinMameDisplayType
	{
		/// <summary>
		/// 14 segments with period and comma, where both period and
		/// comma are always toggeled at the same time.
		/// </summary>
		Seg16 = PinMameApi.PinmameDisplayType.SEG16,

		/// <summary>
		/// 16 segments with comma and period reversed
		/// </summary>

		Seg16R = PinMameApi.PinmameDisplayType.SEG16R,
		/// <summary>
		/// 9 segments and comma
		/// </summary>
		Seg10 = PinMameApi.PinmameDisplayType.SEG10,

		/// <summary>
		/// 9 segments
		/// </summary>
		Seg9 = PinMameApi.PinmameDisplayType.SEG9,

		/// <summary>
		/// 7 segments and comma
		/// </summary>
		Seg8 = PinMameApi.PinmameDisplayType.SEG8,

		/// <summary>
		/// 7 segments and period
		/// </summary>
		Seg8D = PinMameApi.PinmameDisplayType.SEG8D,

		/// <summary>
		/// 7 segments
		/// </summary>
		Seg7 = PinMameApi.PinmameDisplayType.SEG7,

		/// <summary>
		/// 7 segments, comma every three
		/// </summary>
		Seg87 = PinMameApi.PinmameDisplayType.SEG87,

		/// <summary>
		/// 7 segments, forced comma every three
		/// </summary>
		Seg87F = PinMameApi.PinmameDisplayType.SEG87F,

		/// <summary>
		/// 9 segments, comma every three
		/// </summary>
		Seg98 = PinMameApi.PinmameDisplayType.SEG98,

		/// <summary>
		/// 9 segments, forced comma every three
		/// </summary>
		Seg98F = PinMameApi.PinmameDisplayType.SEG98F,

		/// <summary>
		/// 7 segments, small
		/// </summary>
		Seg7S = PinMameApi.PinmameDisplayType.SEG7S,

		/// <summary>
		/// 7 segments, small, with comma
		/// </summary>
		Seg7SC = PinMameApi.PinmameDisplayType.SEG7SC,

		/// <summary>
		/// 16 segments with split top and bottom line
		/// </summary>
		Seg16S = PinMameApi.PinmameDisplayType.SEG16S,

		/// <summary>
		/// Dot matrix display
		/// </summary>
		Dmd = PinMameApi.PinmameDisplayType.DMD,

		/// <summary>
		/// Video display
		/// </summary>
		Video = PinMameApi.PinmameDisplayType.VIDEO,

		/// <summary>
		/// 16 segments without commas
		/// </summary>
		Seg16N = PinMameApi.PinmameDisplayType.SEG16N,

		/// <summary>
		/// 16 segments with periods only
		/// </summary>
		Seg16D = PinMameApi.PinmameDisplayType.SEG16D,

		/// <summary>
		/// maximum segment definition number
		/// </summary>
		SegAll = PinMameApi.PinmameDisplayType.SEGALL,

		/// <summary>
		/// Link to another display layout
		/// </summary>
		Import = PinMameApi.PinmameDisplayType.IMPORT,

		/// <summary>
		/// Note that CORE_IMPORT must be part of the segmask as well!
		/// </summary>
		SegMask = PinMameApi.PinmameDisplayType.SEGMASK,
		SegHiBit = PinMameApi.PinmameDisplayType.SEGHIBIT,
		SegRev = PinMameApi.PinmameDisplayType.SEGREV,
		DmdNoAA = PinMameApi.PinmameDisplayType.DMDNOAA,
		NoDisp = PinMameApi.PinmameDisplayType.NODISP,
		Seg8H = PinMameApi.PinmameDisplayType.SEG8H,
		Seg7H = PinMameApi.PinmameDisplayType.SEG7H,
		Seg87H = PinMameApi.PinmameDisplayType.SEG87H,
		Seg87FH = PinMameApi.PinmameDisplayType.SEG87FH,
		Seg7SH = PinMameApi.PinmameDisplayType.SEG7SH,
		Seg7SCH = PinMameApi.PinmameDisplayType.SEG7SCH
	}

	public enum PinMameHardwareGen : ulong
	{

		/// <summary>
		/// Alpha-numeric display S11 sound - Dr Dude 10/90
		/// </summary>
		WpcAlpha1 = PinMameApi.PinmameHardwareGen.WPCALPHA_1,

		/// <summary>
		/// Alpha-numeric display - The Machine BOP 4/91
		/// </summary>
		WpcAlpha2 = PinMameApi.PinmameHardwareGen.WPCALPHA_2,

		/// <summary>
		/// Dot Matrix Display -  Terminator 2 7/91, Party Zone 10/91
		/// </summary>
		WpcDmd = PinMameApi.PinmameHardwareGen.WPCDMD,

		/// <summary>
		/// Fliptronic flippers - Addams Family 2/92, Twilight Zone 5/93
		/// </summary>
		WpcFliptronic = PinMameApi.PinmameHardwareGen.WPCFLIPTRON,

		/// <summary>
		/// DCS Sound system - Indiana Jones 10/93, Popeye 3/94
		/// </summary>
		WpcDcs = PinMameApi.PinmameHardwareGen.WPCDCS,

		/// <summary>
		/// Security chip - World Cup Soccer 3/94, Jackbot 10/95
		/// </summary>
		WpcSecurity = PinMameApi.PinmameHardwareGen.WPCSECURITY,

		/// <summary>
		/// Hybrid WPC95 driver + DCS sound - Who Dunnit
		/// </summary>
		Wpc95Dcs = PinMameApi.PinmameHardwareGen.WPC95DCS,

		/// <summary>
		/// Integrated boards - Congo 3/96, Cactus Canyon 2/99
		/// </summary>
		Wpc95 = PinMameApi.PinmameHardwareGen.WPC95,

		/// <summary>
		/// No external sound board
		/// </summary>
		S11 = PinMameApi.PinmameHardwareGen.S11,

		/// <summary>
		/// S11C sound board
		/// </summary>
		S11X = PinMameApi.PinmameHardwareGen.S11X,
		S11A = PinMameApi.PinmameHardwareGen.S11A,
		S11B = PinMameApi.PinmameHardwareGen.S11B,

		/// <summary>
		/// Jokerz! sound board
		/// </summary>
		S11B2 = PinMameApi.PinmameHardwareGen.S11B2,

		/// <summary>
		/// No CPU board sound
		/// </summary>
		S11C = PinMameApi.PinmameHardwareGen.S11C,

		/// <summary>
		/// S9 CPU, 4x7+1x4
		/// </summary>
		S9 = PinMameApi.PinmameHardwareGen.S9,

		/// <summary>
		/// DE AlphaSeg
		/// </summary>
		De = PinMameApi.PinmameHardwareGen.DE,

		/// <summary>
		/// DE 128x16
		/// </summary>
		DeDmd16 = PinMameApi.PinmameHardwareGen.DEDMD16,

		/// <summary>
		/// DE 128x32
		/// </summary>
		DeDmd32 = PinMameApi.PinmameHardwareGen.DEDMD32,

		/// <summary>
		/// DE 192x64
		/// </summary>
		DeDmd64 = PinMameApi.PinmameHardwareGen.DEDMD64,

		/// <summary>
		/// S7 CPU
		/// </summary>
		S7 = PinMameApi.PinmameHardwareGen.S7,

		/// <summary>
		/// S6 CPU
		/// </summary>
		S6 = PinMameApi.PinmameHardwareGen.S6,

		/// <summary>
		/// S4 CPU
		/// </summary>
		S4 = PinMameApi.PinmameHardwareGen.S4,

		/// <summary>
		/// S3 CPU No Chimes
		/// </summary>
		S3C = PinMameApi.PinmameHardwareGen.S3C,
		S3 = PinMameApi.PinmameHardwareGen.S3,
		By17 = PinMameApi.PinmameHardwareGen.BY17,
		By35 = PinMameApi.PinmameHardwareGen.BY35,

		/// <summary>
		/// Stern MPU - 100
		/// </summary>
		Stmpu100 = PinMameApi.PinmameHardwareGen.STMPU100,

		/// <summary>
		/// Stern MPU - 200
		/// </summary>
		Stmpu200 = PinMameApi.PinmameHardwareGen.STMPU200,

		/// <summary>
		/// Unknown Astro game, Stern hardware
		/// </summary>
		Astro = PinMameApi.PinmameHardwareGen.ASTRO,

		/// <summary>
		/// Hankin
		/// </summary>
		Hnk = PinMameApi.PinmameHardwareGen.HNK,

		/// <summary>
		/// Bally Bow & Arrow prototype
		/// </summary>
		ByProto = PinMameApi.PinmameHardwareGen.BYPROTO,
		By6803 = PinMameApi.PinmameHardwareGen.BY6803,
		By6803A = PinMameApi.PinmameHardwareGen.BY6803A,

		/// <summary>
		/// Big Ball Bowling, Stern hardware
		/// </summary>
		Bowling = PinMameApi.PinmameHardwareGen.BOWLING,

		/// <summary>
		/// GTS1
		/// </summary>
		Gts1 = PinMameApi.PinmameHardwareGen.GTS1,

		/// <summary>
		/// GTS80
		/// </summary>
		Gts80 = PinMameApi.PinmameHardwareGen.GTS80,
		Gts80A = PinMameApi.PinmameHardwareGen.GTS80A,

		/// <summary>
		/// GTS80B
		/// </summary>
		Gts80B = PinMameApi.PinmameHardwareGen.GTS80B,

		/// <summary>
		/// Whitestar
		/// </summary>
		Whitestar = PinMameApi.PinmameHardwareGen.WS,

		/// <summary>
		/// Whitestar with extra RAM
		/// </summary>
		Whitestar1 = PinMameApi.PinmameHardwareGen.WS_1,

		/// <summary>
		/// Whitestar with extra DMD
		/// </summary>
		Whitestar2 = PinMameApi.PinmameHardwareGen.WS_2,

		/// <summary>
		/// GTS3
		/// </summary>
		Gts3 = PinMameApi.PinmameHardwareGen.GTS3,
		Zac1 = PinMameApi.PinmameHardwareGen.ZAC1,
		Zac2 = PinMameApi.PinmameHardwareGen.ZAC2,

		/// <summary>
		/// Stern S.A.M.
		/// </summary>
		Sam = PinMameApi.PinmameHardwareGen.SAM,

		/// <summary>
		/// Alvin G Hardware
		/// </summary>
		Alvg = PinMameApi.PinmameHardwareGen.ALVG,

		/// <summary>
		/// Alvin G Hardware, with more shades
		/// </summary>
		AlvgDmd2 = PinMameApi.PinmameHardwareGen.ALVG_DMD2,

		/// <summary>
		/// Mr.Game Hardware
		/// </summary>
		MrGame = PinMameApi.PinmameHardwareGen.MRGAME,

		/// <summary>
		/// Sleic Hardware
		/// </summary>
		Sleic = PinMameApi.PinmameHardwareGen.SLEIC,

		/// <summary>
		/// Wico Hardware
		/// </summary>
		Wico = PinMameApi.PinmameHardwareGen.WICO,

		/// <summary>
		/// Stern Pinball Arcade
		/// </summary>
		Spa = PinMameApi.PinmameHardwareGen.SPA,

		/// <summary>
		/// All WPC
		/// </summary>
		AllWpc = PinMameApi.PinmameHardwareGen.ALLWPC,

		/// <summary>
		/// All Sys11
		/// </summary>
		AllS11 = PinMameApi.PinmameHardwareGen.ALLS11,

		/// <summary>
		/// All Bally35 and derivatives
		/// </summary>
		AllBy35 = PinMameApi.PinmameHardwareGen.ALLBY35,

		/// <summary>
		/// All GTS80
		/// </summary>
		AllS80 = PinMameApi.PinmameHardwareGen.ALLS80,

		/// <summary>
		/// All Whitestar
		/// </summary>
		AllWhitestar = PinMameApi.PinmameHardwareGen.ALLWS,
	}

	public enum PinMameGameDriverFlag : uint
	{
		OrientationMask = PinMameApi.PinmameGameDriverFlag.ORIENTATION_MASK,

		/// <summary>
		/// Mirror everything in the X direction
		/// </summary>
		OrientationFlipX = PinMameApi.PinmameGameDriverFlag.ORIENTATION_FLIP_X,

		/// <summary>
		/// Mirror everything in the Y direction
		/// </summary>
		OrientationFlipY = PinMameApi.PinmameGameDriverFlag.ORIENTATION_FLIP_Y,

		/// <summary>
		/// Mirror along the top-left/bottom-right diagonal
		/// </summary>
		OrientationSwapXY = PinMameApi.PinmameGameDriverFlag.ORIENTATION_SWAP_XY,

		/// <summary>
		/// Indicates this game is not working.
		/// </summary>
		GameNotWorking = PinMameApi.PinmameGameDriverFlag.GAME_NOT_WORKING,

		/// <summary>
		/// Game's protection not fully emulated
		/// </summary>
		GameUnemulatedProtection = PinMameApi.PinmameGameDriverFlag.GAME_UNEMULATED_PROTECTION,

		/// <summary>
		/// Colors are totally wrong
		/// </summary>
		GameWrongColors = PinMameApi.PinmameGameDriverFlag.GAME_WRONG_COLORS,

		/// <summary>
		/// Colors are not 100% accurate, but close
		/// </summary>
		GameImperfectColors = PinMameApi.PinmameGameDriverFlag.GAME_IMPERFECT_COLORS,

		/// <summary>
		/// Graphics are wrong/incomplete
		/// </summary>
		GameImperfectGraphics = PinMameApi.PinmameGameDriverFlag.GAME_IMPERFECT_GRAPHICS,

		/// <summary>
		/// Screen flip support is missing
		/// </summary>
		GameNoCocktail = PinMameApi.PinmameGameDriverFlag.GAME_NO_COCKTAIL,

		/// <summary>
		/// Sound is missing
		/// </summary>
		GameNoSound = PinMameApi.PinmameGameDriverFlag.GAME_NO_SOUND,

		/// <summary>
		/// Sound is known to be wrong
		/// </summary>
		GameImperfectSound = PinMameApi.PinmameGameDriverFlag.GAME_IMPERFECT_SOUND,

		/// <summary>
		/// Set by the fake "root" driver_0 and by "containers"
		/// </summary>
		NotADriver = PinMameApi.PinmameGameDriverFlag.NOT_A_DRIVER,
	}

	public enum PinMameMechFlag : uint
	{
		Linear = PinMameApi.PinmameMechFlag.LINEAR,
		NonLinear = PinMameApi.PinmameMechFlag.NONLINEAR,
		Circle = PinMameApi.PinmameMechFlag.CIRCLE,
		StopEnd = PinMameApi.PinmameMechFlag.STOPEND,
		Reverse = PinMameApi.PinmameMechFlag.REVERSE,
		OneSol = PinMameApi.PinmameMechFlag.ONESOL,
		OneDirSol = PinMameApi.PinmameMechFlag.ONEDIRSOL,
		TwoDirSol = PinMameApi.PinmameMechFlag.TWODIRSOL,
		TwoStepSol = PinMameApi.PinmameMechFlag.TWOSTEPSOL,
		FourStepSol = PinMameApi.PinmameMechFlag.FOURSTEPSOL,
		Slow = PinMameApi.PinmameMechFlag.SLOW,
		Fast = PinMameApi.PinmameMechFlag.FAST,
		StepSw = PinMameApi.PinmameMechFlag.STEPSW,
		LengthSw = PinMameApi.PinmameMechFlag.LENGTHSW
	}

	public enum PinMameKeycode
	{
		A = PinMameApi.PinmameKeycode.A,
		B = PinMameApi.PinmameKeycode.B,
		C = PinMameApi.PinmameKeycode.C,
		D = PinMameApi.PinmameKeycode.D,
		E = PinMameApi.PinmameKeycode.E,
		F = PinMameApi.PinmameKeycode.F,
		G = PinMameApi.PinmameKeycode.G,
		H = PinMameApi.PinmameKeycode.H,
		I = PinMameApi.PinmameKeycode.I,
		J = PinMameApi.PinmameKeycode.J,
		K = PinMameApi.PinmameKeycode.K,
		L = PinMameApi.PinmameKeycode.L,
		M = PinMameApi.PinmameKeycode.M,
		N = PinMameApi.PinmameKeycode.N,
		O = PinMameApi.PinmameKeycode.O,
		P = PinMameApi.PinmameKeycode.P,
		Q = PinMameApi.PinmameKeycode.Q,
		R = PinMameApi.PinmameKeycode.R,
		S = PinMameApi.PinmameKeycode.S,
		T = PinMameApi.PinmameKeycode.T,
		U = PinMameApi.PinmameKeycode.U,
		V = PinMameApi.PinmameKeycode.V,
		W = PinMameApi.PinmameKeycode.W,
		X = PinMameApi.PinmameKeycode.X,
		Y = PinMameApi.PinmameKeycode.Y,
		Z = PinMameApi.PinmameKeycode.Z,
		Number0 = PinMameApi.PinmameKeycode.NUMBER_0,
		Number1 = PinMameApi.PinmameKeycode.NUMBER_1,
		Number2 = PinMameApi.PinmameKeycode.NUMBER_2,
		Number3 = PinMameApi.PinmameKeycode.NUMBER_3,
		Number4 = PinMameApi.PinmameKeycode.NUMBER_4,
		Number5 = PinMameApi.PinmameKeycode.NUMBER_5,
		Number6 = PinMameApi.PinmameKeycode.NUMBER_6,
		Number7 = PinMameApi.PinmameKeycode.NUMBER_7,
		Number8 = PinMameApi.PinmameKeycode.NUMBER_8,
		Number9 = PinMameApi.PinmameKeycode.NUMBER_9,
		Keypad0 = PinMameApi.PinmameKeycode.KEYPAD_0,
		Keypad1 = PinMameApi.PinmameKeycode.KEYPAD_1,
		Keypad2 = PinMameApi.PinmameKeycode.KEYPAD_2,
		Keypad3 = PinMameApi.PinmameKeycode.KEYPAD_3,
		Keypad4 = PinMameApi.PinmameKeycode.KEYPAD_4,
		Keypad5 = PinMameApi.PinmameKeycode.KEYPAD_5,
		Keypad6 = PinMameApi.PinmameKeycode.KEYPAD_6,
		Keypad7 = PinMameApi.PinmameKeycode.KEYPAD_7,
		Keypad8 = PinMameApi.PinmameKeycode.KEYPAD_8,
		Keypad9 = PinMameApi.PinmameKeycode.KEYPAD_9,
		F1 = PinMameApi.PinmameKeycode.F1,
		F2 = PinMameApi.PinmameKeycode.F2,
		F3 = PinMameApi.PinmameKeycode.F3,
		F4 = PinMameApi.PinmameKeycode.F4,
		F5 = PinMameApi.PinmameKeycode.F5,
		F6 = PinMameApi.PinmameKeycode.F6,
		F7 = PinMameApi.PinmameKeycode.F7,
		F8 = PinMameApi.PinmameKeycode.F8,
		F9 = PinMameApi.PinmameKeycode.F9,
		F10 = PinMameApi.PinmameKeycode.F10,
		F11 = PinMameApi.PinmameKeycode.F11,
		F12 = PinMameApi.PinmameKeycode.F12,
		Escape = PinMameApi.PinmameKeycode.ESCAPE,
		GraveAccent = PinMameApi.PinmameKeycode.GRAVE_ACCENT,
		Minus = PinMameApi.PinmameKeycode.MINUS,
		Equals = PinMameApi.PinmameKeycode.EQUALS,
		Backspace = PinMameApi.PinmameKeycode.BACKSPACE,
		Tab = PinMameApi.PinmameKeycode.TAB,
		LeftBracket = PinMameApi.PinmameKeycode.LEFT_BRACKET,
		RightBracket = PinMameApi.PinmameKeycode.RIGHT_BRACKET,
		Enter = PinMameApi.PinmameKeycode.ENTER,
		Semicolon = PinMameApi.PinmameKeycode.SEMICOLON,
		Quote = PinMameApi.PinmameKeycode.QUOTE,
		Backslash = PinMameApi.PinmameKeycode.BACKSLASH,
		Comma = PinMameApi.PinmameKeycode.COMMA,
		Period = PinMameApi.PinmameKeycode.PERIOD,
		Slash = PinMameApi.PinmameKeycode.SLASH,
		Space = PinMameApi.PinmameKeycode.SPACE,
		Insert = PinMameApi.PinmameKeycode.INSERT,
		Delete = PinMameApi.PinmameKeycode.DELETE,
		Home = PinMameApi.PinmameKeycode.HOME,
		End = PinMameApi.PinmameKeycode.END,
		PageUp = PinMameApi.PinmameKeycode.PAGE_UP,
		PageDown = PinMameApi.PinmameKeycode.PAGE_DOWN,
		Left = PinMameApi.PinmameKeycode.LEFT,
		Right = PinMameApi.PinmameKeycode.RIGHT,
		Up = PinMameApi.PinmameKeycode.UP,
		Down = PinMameApi.PinmameKeycode.DOWN,
		KeypadDivide = PinMameApi.PinmameKeycode.KEYPAD_DIVIDE,
		KeypadMultiply = PinMameApi.PinmameKeycode.KEYPAD_MULTIPLY,
		KeypadSubtract = PinMameApi.PinmameKeycode.KEYPAD_SUBTRACT,
		KeypadAdd = PinMameApi.PinmameKeycode.KEYPAD_ADD,
		KeypadEnter = PinMameApi.PinmameKeycode.KEYPAD_ENTER,
		PrintScreen = PinMameApi.PinmameKeycode.PRINT_SCREEN,
		Pause = PinMameApi.PinmameKeycode.PAUSE,
		LeftShift = PinMameApi.PinmameKeycode.LEFT_SHIFT,
		RightShift = PinMameApi.PinmameKeycode.RIGHT_SHIFT,
		LeftControl = PinMameApi.PinmameKeycode.LEFT_CONTROL,
		RightControl = PinMameApi.PinmameKeycode.RIGHT_CONTROL,
		LeftAlt = PinMameApi.PinmameKeycode.LEFT_ALT,
		RightAlt = PinMameApi.PinmameKeycode.RIGHT_ALT,
		ScrollLock = PinMameApi.PinmameKeycode.SCROLL_LOCK,
		NumLock = PinMameApi.PinmameKeycode.NUM_LOCK,
		CapsLock = PinMameApi.PinmameKeycode.CAPS_LOCK,
		LeftSuper = PinMameApi.PinmameKeycode.LEFT_SUPER,
		RightSuper = PinMameApi.PinmameKeycode.RIGHT_SUPER,
		Menu = PinMameApi.PinmameKeycode.MENU,
	};
}
