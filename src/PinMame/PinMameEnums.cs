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

// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
// ReSharper disable CommentTypo

using System;

namespace PinMame
{
	[Flags]
	public enum PinMameAudioFormat
	{
		AudioFormatInt16 = PinMameApi.AudioFormat.INT16,
		AudioFormatFloat = PinMameApi.AudioFormat.FLOAT
	}

	public enum PinMameDmdMode
	{
		Brightness = PinMameApi.DmdMode.BRIGHTNESS,
		Raw = PinMameApi.DmdMode.RAW
	}

	/// <summary>
	/// The kinds of files whose folder can be changed with <see cref="PinMame.SetPath"/>. Without
	/// an override, each one is a folder below the VPM path.
	/// </summary>
	public enum PinMameFileType
	{
		/// <summary>
		/// ROM sets, "roms" below the VPM path by default.
		/// </summary>
		Roms = PinMameApi.FileType.ROMS,

		/// <summary>
		/// Non-volatile RAM, "nvram" below the VPM path by default.
		/// </summary>
		Nvram = PinMameApi.FileType.NVRAM,

		/// <summary>
		/// Sound samples, "samples" below the VPM path by default.
		/// </summary>
		Samples = PinMameApi.FileType.SAMPLES,

		/// <summary>
		/// Per-game configuration, "cfg" below the VPM path by default.
		/// </summary>
		Config = PinMameApi.FileType.CONFIG,

		/// <summary>
		/// High scores, "hi" below the VPM path by default.
		/// </summary>
		HighScore = PinMameApi.FileType.HIGHSCORE
	}

	[Flags]
	public enum PinMameDisplayType
	{
		/// <summary>
		/// 14 segments with period and comma, where both period and
		/// comma are always toggeled at the same time.
		/// </summary>
		Seg16 = PinMameApi.DisplayType.SEG16,

		/// <summary>
		/// 16 segments with comma and period reversed
		/// </summary>

		Seg16R = PinMameApi.DisplayType.SEG16R,
		/// <summary>
		/// 9 segments and comma
		/// </summary>
		Seg10 = PinMameApi.DisplayType.SEG10,

		/// <summary>
		/// 9 segments
		/// </summary>
		Seg9 = PinMameApi.DisplayType.SEG9,

		/// <summary>
		/// 7 segments and comma
		/// </summary>
		Seg8 = PinMameApi.DisplayType.SEG8,

		/// <summary>
		/// 7 segments and period
		/// </summary>
		Seg8D = PinMameApi.DisplayType.SEG8D,

		/// <summary>
		/// 7 segments
		/// </summary>
		Seg7 = PinMameApi.DisplayType.SEG7,

		/// <summary>
		/// 7 segments, comma every three
		/// </summary>
		Seg87 = PinMameApi.DisplayType.SEG87,

		/// <summary>
		/// 7 segments, forced comma every three
		/// </summary>
		Seg87F = PinMameApi.DisplayType.SEG87F,

		/// <summary>
		/// 9 segments, comma every three
		/// </summary>
		Seg98 = PinMameApi.DisplayType.SEG98,

		/// <summary>
		/// 9 segments, forced comma every three
		/// </summary>
		Seg98F = PinMameApi.DisplayType.SEG98F,

		/// <summary>
		/// 7 segments, small
		/// </summary>
		Seg7S = PinMameApi.DisplayType.SEG7S,

		/// <summary>
		/// 7 segments, small, with comma
		/// </summary>
		Seg7SC = PinMameApi.DisplayType.SEG7SC,

		/// <summary>
		/// 16 segments with split top and bottom line
		/// </summary>
		Seg16S = PinMameApi.DisplayType.SEG16S,

		/// <summary>
		/// Dot matrix display
		/// </summary>
		Dmd = PinMameApi.DisplayType.DMD,

		/// <summary>
		/// Video display
		/// </summary>
		Video = PinMameApi.DisplayType.VIDEO,

		/// <summary>
		/// 16 segments without commas
		/// </summary>
		Seg16N = PinMameApi.DisplayType.SEG16N,

		/// <summary>
		/// 16 segments with periods only
		/// </summary>
		Seg16D = PinMameApi.DisplayType.SEG16D,

		/// <summary>
		/// maximum segment definition number
		/// </summary>
		SegAll = PinMameApi.DisplayType.SEGALL,

		/// <summary>
		/// Link to another display layout
		/// </summary>
		Import = PinMameApi.DisplayType.IMPORT,

		/// <summary>
		/// Note that CORE_IMPORT must be part of the segmask as well!
		/// </summary>
		SegMask = PinMameApi.DisplayType.SEGMASK,
		SegHiBit = PinMameApi.DisplayType.SEGHIBIT,
		SegRev = PinMameApi.DisplayType.SEGREV,
		DmdNoAA = PinMameApi.DisplayType.DMDNOAA,
		NoDisp = PinMameApi.DisplayType.NODISP,
		Seg8H = PinMameApi.DisplayType.SEG8H,
		Seg7H = PinMameApi.DisplayType.SEG7H,
		Seg87H = PinMameApi.DisplayType.SEG87H,
		Seg87FH = PinMameApi.DisplayType.SEG87FH,
		Seg7SH = PinMameApi.DisplayType.SEG7SH,
		Seg7SCH = PinMameApi.DisplayType.SEG7SCH
	}

	public enum PinMameHardwareGen : ulong
	{

		/// <summary>
		/// Alpha-numeric display S11 sound - Dr Dude 10/90
		/// </summary>
		WpcAlpha1 = PinMameApi.HardwareGen.WPCALPHA_1,

		/// <summary>
		/// Alpha-numeric display - The Machine BOP 4/91
		/// </summary>
		WpcAlpha2 = PinMameApi.HardwareGen.WPCALPHA_2,

		/// <summary>
		/// Dot Matrix Display -  Terminator 2 7/91, Party Zone 10/91
		/// </summary>
		WpcDmd = PinMameApi.HardwareGen.WPCDMD,

		/// <summary>
		/// Fliptronic flippers - Addams Family 2/92, Twilight Zone 5/93
		/// </summary>
		WpcFliptronic = PinMameApi.HardwareGen.WPCFLIPTRON,

		/// <summary>
		/// DCS Sound system - Indiana Jones 10/93, Popeye 3/94
		/// </summary>
		WpcDcs = PinMameApi.HardwareGen.WPCDCS,

		/// <summary>
		/// Security chip - World Cup Soccer 3/94, Jackbot 10/95
		/// </summary>
		WpcSecurity = PinMameApi.HardwareGen.WPCSECURITY,

		/// <summary>
		/// Hybrid WPC95 driver + DCS sound - Who Dunnit
		/// </summary>
		Wpc95Dcs = PinMameApi.HardwareGen.WPC95DCS,

		/// <summary>
		/// Integrated boards - Congo 3/96, Cactus Canyon 2/99
		/// </summary>
		Wpc95 = PinMameApi.HardwareGen.WPC95,

		/// <summary>
		/// No external sound board
		/// </summary>
		S11 = PinMameApi.HardwareGen.S11,

		/// <summary>
		/// S11C sound board
		/// </summary>
		S11X = PinMameApi.HardwareGen.S11X,
		S11A = PinMameApi.HardwareGen.S11A,
		S11B = PinMameApi.HardwareGen.S11B,

		/// <summary>
		/// Jokerz! sound board
		/// </summary>
		S11B2 = PinMameApi.HardwareGen.S11B2,

		/// <summary>
		/// No CPU board sound
		/// </summary>
		S11C = PinMameApi.HardwareGen.S11C,

		/// <summary>
		/// S9 CPU, 4x7+1x4
		/// </summary>
		S9 = PinMameApi.HardwareGen.S9,

		/// <summary>
		/// DE AlphaSeg
		/// </summary>
		De = PinMameApi.HardwareGen.DE,

		/// <summary>
		/// DE 128x16
		/// </summary>
		DeDmd16 = PinMameApi.HardwareGen.DEDMD16,

		/// <summary>
		/// DE 128x32
		/// </summary>
		DeDmd32 = PinMameApi.HardwareGen.DEDMD32,

		/// <summary>
		/// DE 192x64
		/// </summary>
		DeDmd64 = PinMameApi.HardwareGen.DEDMD64,

		/// <summary>
		/// S7 CPU
		/// </summary>
		S7 = PinMameApi.HardwareGen.S7,

		/// <summary>
		/// S6 CPU
		/// </summary>
		S6 = PinMameApi.HardwareGen.S6,

		/// <summary>
		/// S4 CPU
		/// </summary>
		S4 = PinMameApi.HardwareGen.S4,

		/// <summary>
		/// S3 CPU No Chimes
		/// </summary>
		S3C = PinMameApi.HardwareGen.S3C,
		S3 = PinMameApi.HardwareGen.S3,
		By17 = PinMameApi.HardwareGen.BY17,
		By35 = PinMameApi.HardwareGen.BY35,

		/// <summary>
		/// Stern MPU - 100
		/// </summary>
		Stmpu100 = PinMameApi.HardwareGen.STMPU100,

		/// <summary>
		/// Stern MPU - 200
		/// </summary>
		Stmpu200 = PinMameApi.HardwareGen.STMPU200,

		/// <summary>
		/// Unknown Astro game, Stern hardware
		/// </summary>
		Astro = PinMameApi.HardwareGen.ASTRO,

		/// <summary>
		/// Hankin
		/// </summary>
		Hnk = PinMameApi.HardwareGen.HNK,

		/// <summary>
		/// Bally Bow & Arrow prototype
		/// </summary>
		ByProto = PinMameApi.HardwareGen.BYPROTO,
		By6803 = PinMameApi.HardwareGen.BY6803,
		By6803A = PinMameApi.HardwareGen.BY6803A,

		/// <summary>
		/// Big Ball Bowling, Stern hardware
		/// </summary>
		Bowling = PinMameApi.HardwareGen.BOWLING,

		/// <summary>
		/// GTS1
		/// </summary>
		Gts1 = PinMameApi.HardwareGen.GTS1,

		/// <summary>
		/// GTS80
		/// </summary>
		Gts80 = PinMameApi.HardwareGen.GTS80,
		Gts80A = PinMameApi.HardwareGen.GTS80A,

		/// <summary>
		/// GTS80B
		/// </summary>
		Gts80B = PinMameApi.HardwareGen.GTS80B,

		/// <summary>
		/// Whitestar
		/// </summary>
		Whitestar = PinMameApi.HardwareGen.WS,

		/// <summary>
		/// Whitestar with extra RAM
		/// </summary>
		Whitestar1 = PinMameApi.HardwareGen.WS_1,

		/// <summary>
		/// Whitestar with extra DMD
		/// </summary>
		Whitestar2 = PinMameApi.HardwareGen.WS_2,

		/// <summary>
		/// GTS3
		/// </summary>
		Gts3 = PinMameApi.HardwareGen.GTS3,
		Zac1 = PinMameApi.HardwareGen.ZAC1,
		Zac2 = PinMameApi.HardwareGen.ZAC2,

		/// <summary>
		/// Stern S.A.M.
		/// </summary>
		Sam = PinMameApi.HardwareGen.SAM,

		/// <summary>
		/// Alvin G Hardware
		/// </summary>
		Alvg = PinMameApi.HardwareGen.ALVG,

		/// <summary>
		/// Alvin G Hardware, with more shades
		/// </summary>
		AlvgDmd2 = PinMameApi.HardwareGen.ALVG_DMD2,

		/// <summary>
		/// Mr.Game Hardware
		/// </summary>
		MrGame = PinMameApi.HardwareGen.MRGAME,

		/// <summary>
		/// Sleic Hardware
		/// </summary>
		Sleic = PinMameApi.HardwareGen.SLEIC,

		/// <summary>
		/// Wico Hardware
		/// </summary>
		Wico = PinMameApi.HardwareGen.WICO,

		/// <summary>
		/// Stern Pinball Arcade
		/// </summary>
		Spa = PinMameApi.HardwareGen.SPA,

		/// <summary>
		/// All WPC
		/// </summary>
		AllWpc = PinMameApi.HardwareGen.ALLWPC,

		/// <summary>
		/// All Sys11
		/// </summary>
		AllS11 = PinMameApi.HardwareGen.ALLS11,

		/// <summary>
		/// All Bally35 and derivatives
		/// </summary>
		AllBy35 = PinMameApi.HardwareGen.ALLBY35,

		/// <summary>
		/// All GTS80
		/// </summary>
		AllS80 = PinMameApi.HardwareGen.ALLS80,

		/// <summary>
		/// All Whitestar
		/// </summary>
		AllWhitestar = PinMameApi.HardwareGen.ALLWS,
	}

	public enum PinMameGameDriverFlag : uint
	{
		OrientationMask = PinMameApi.GameDriverFlag.ORIENTATION_MASK,

		/// <summary>
		/// Mirror everything in the X direction
		/// </summary>
		OrientationFlipX = PinMameApi.GameDriverFlag.ORIENTATION_FLIP_X,

		/// <summary>
		/// Mirror everything in the Y direction
		/// </summary>
		OrientationFlipY = PinMameApi.GameDriverFlag.ORIENTATION_FLIP_Y,

		/// <summary>
		/// Mirror along the top-left/bottom-right diagonal
		/// </summary>
		OrientationSwapXY = PinMameApi.GameDriverFlag.ORIENTATION_SWAP_XY,

		/// <summary>
		/// Indicates this game is not working.
		/// </summary>
		GameNotWorking = PinMameApi.GameDriverFlag.GAME_NOT_WORKING,

		/// <summary>
		/// Game's protection not fully emulated
		/// </summary>
		GameUnemulatedProtection = PinMameApi.GameDriverFlag.GAME_UNEMULATED_PROTECTION,

		/// <summary>
		/// Colors are totally wrong
		/// </summary>
		GameWrongColors = PinMameApi.GameDriverFlag.GAME_WRONG_COLORS,

		/// <summary>
		/// Colors are not 100% accurate, but close
		/// </summary>
		GameImperfectColors = PinMameApi.GameDriverFlag.GAME_IMPERFECT_COLORS,

		/// <summary>
		/// Graphics are wrong/incomplete
		/// </summary>
		GameImperfectGraphics = PinMameApi.GameDriverFlag.GAME_IMPERFECT_GRAPHICS,

		/// <summary>
		/// Screen flip support is missing
		/// </summary>
		GameNoCocktail = PinMameApi.GameDriverFlag.GAME_NO_COCKTAIL,

		/// <summary>
		/// Sound is missing
		/// </summary>
		GameNoSound = PinMameApi.GameDriverFlag.GAME_NO_SOUND,

		/// <summary>
		/// Sound is known to be wrong
		/// </summary>
		GameImperfectSound = PinMameApi.GameDriverFlag.GAME_IMPERFECT_SOUND,

		/// <summary>
		/// Set by the fake "root" driver_0 and by "containers"
		/// </summary>
		NotADriver = PinMameApi.GameDriverFlag.NOT_A_DRIVER,
	}

	/// <summary>
	/// Which outputs PinMAME reports as physical, PWM-integrated levels instead of binary states. This is
	/// the bit mask of <c>PinmameSetSolenoidMask(2, mask)</c>, which VPX tables set with
	/// <c>UseVPMModSol = 2</c> (<c>Controller.SolMask(2)</c>).
	/// </summary>
	/// <remarks>
	/// In physical mode PinMAME runs each output's PWM signal through a model of the connected device
	/// (an incandescent bulb's filament, a LED, a solenoid) and reports 0-255, where 255 is the device's
	/// rated brightness or full power. The device type of each output comes from the game driver and can
	/// be changed with <see cref="PinMame.SetModOutputType"/>.
	/// </remarks>
	[Flags]
	public enum PinMamePhysicalOutputs : uint
	{
		/// <summary>
		/// Binary outputs (lamps 0/1, GI 0-8, solenoids 0/1). PinMAME's default.
		/// </summary>
		None = 0,

		/// <summary>
		/// The legacy modulated solenoids of WPC and SAM (0-255, plain duty cycle).
		/// </summary>
		LegacyModulatedSolenoids = 0x01,

		/// <summary>
		/// Lamp matrix and other lamp outputs.
		/// </summary>
		Lamps = 0x04,

		/// <summary>
		/// General illumination strings.
		/// </summary>
		GI = 0x08,

		/// <summary>
		/// Solenoid outputs, including flashers driven by solenoid drivers.
		/// </summary>
		Solenoids = 0x10,

		/// <summary>
		/// Alphanumeric display segments.
		/// </summary>
		AlphaSegments = 0x20,

		/// <summary>
		/// Lamps, GI, solenoids and alphanumeric segments, like <c>UseVPMModSol = 2</c> in VPX.
		/// </summary>
		All = Lamps | GI | Solenoids | AlphaSegments,
	}

	/// <summary>
	/// The output class of an output whose device type is read or changed with
	/// <see cref="PinMame.GetModOutputType"/> and <see cref="PinMame.SetModOutputType"/>.
	/// </summary>
	public enum PinMameModOutput
	{
		Solenoid = PinMameApi.ModOutputType.SOLENOID,
		Lamp = PinMameApi.ModOutputType.LAMP,
		GI = PinMameApi.ModOutputType.GI,
		AlphaSegment = PinMameApi.ModOutputType.ALPHASEG,
	}

	/// <summary>
	/// The device PinMAME models on an output in physical mode (<c>CORE_MODOUT_*</c> in PinMAME's core.h,
	/// <c>VPM_MODOUT_*</c> in VPX's core.vbs). Game drivers set these per output; values not listed here can
	/// still be cast from an int.
	/// </summary>
	public enum PinMameModOutputType
	{
		/// <summary>No integration, the driver's value is kept unchanged.</summary>
		None = 0,
		/// <summary>No integration, the raw pulse state.</summary>
		Pulse = 1,
		/// <summary>Solenoid: on as soon as it is pulsed, off when no pulse was seen for 60 ms.</summary>
		Solenoid2State = 2,
		/// <summary>The driver's custom solenoid implementation.</summary>
		SolenoidCustom = 3,
		/// <summary>The driver's custom integrator.</summary>
		CustomIntegrator = 4,
		/// <summary>Like <see cref="Solenoid2State"/>, but reported after a few VBlanks.</summary>
		LegacySolenoid2State = 50,
		/// <summary>#44/#555 bulb on 6.3 V AC, common for GI.</summary>
		Bulb44_6_3V_AC = 100,
		/// <summary>#47 bulb on 6.3 V AC.</summary>
		Bulb47_6_3V_AC = 101,
		/// <summary>#86 bulb on 6.3 V AC.</summary>
		Bulb86_6_3V_AC = 102,
		/// <summary>#44/#555 bulb on 5.7 V AC (Whitestar and SAM GI).</summary>
		Bulb44_5_7V_AC = 103,
		/// <summary>#44/#555 bulb on 6.3 V AC through a relay inverter (1 is off, 0 is on).</summary>
		Bulb44_6_3V_AC_Rev = 104,
		/// <summary>#44/#555 bulb on 18 V DC, WPC lamp matrix.</summary>
		Bulb44_18V_DC_WPC = 201,
		/// <summary>#44/#555 bulb on 20 V DC, GTS3 lamp matrix.</summary>
		Bulb44_20V_DC_GTS3 = 202,
		/// <summary>#44/#555 bulb on 18 V DC, System 11 lamp matrix.</summary>
		Bulb44_18V_DC_S11 = 203,
		/// <summary>#44/#555 bulb on 18 V DC, Sega/Data East lamp matrix.</summary>
		Bulb44_18V_DC_SE = 204,
		/// <summary>#44/#555 bulb on 20 V DC, Capcom lamp matrix.</summary>
		Bulb44_20V_DC_CC = 205,
		/// <summary>#89 bulb on 20 V DC, WPC flashers.</summary>
		Bulb89_20V_DC_WPC = 301,
		/// <summary>#89 bulb on 20 V DC, GTS3 flashers.</summary>
		Bulb89_20V_DC_GTS3 = 302,
		/// <summary>#89 bulb on 32 V DC, System 11 flashers.</summary>
		Bulb89_32V_DC_S11 = 303,
		/// <summary>#89 bulb on 25 V DC, System 11 flashers.</summary>
		Bulb89_25V_DC_S11 = 304,
		/// <summary>#906 bulb on 20 V DC, WPC flashers.</summary>
		Bulb906_20V_DC_WPC = 311,
		/// <summary>#906 bulb on 20 V DC, GTS3 flashers.</summary>
		Bulb906_20V_DC_GTS3 = 312,
		/// <summary>#906 bulb on 32 V DC, System 11 flashers.</summary>
		Bulb906_32V_DC_S11 = 313,
		/// <summary>#906 bulb on 25 V DC, System 11 flashers.</summary>
		Bulb906_25V_DC_S11 = 314,
		/// <summary>LED (mostly the eye's response, a LED itself is near instant).</summary>
		Led = 400,
		/// <summary>LED strobed 1 ms over 10 ms for full power.</summary>
		LedStrobe1_10ms = 401,
	}

	public enum PinMameMechFlag : uint
	{
		Linear = PinMameApi.MechFlag.LINEAR,
		NonLinear = PinMameApi.MechFlag.NONLINEAR,
		Circle = PinMameApi.MechFlag.CIRCLE,
		StopEnd = PinMameApi.MechFlag.STOPEND,
		Reverse = PinMameApi.MechFlag.REVERSE,
		OneSol = PinMameApi.MechFlag.ONESOL,
		OneDirSol = PinMameApi.MechFlag.ONEDIRSOL,
		TwoDirSol = PinMameApi.MechFlag.TWODIRSOL,
		TwoStepSol = PinMameApi.MechFlag.TWOSTEPSOL,
		FourStepSol = PinMameApi.MechFlag.FOURSTEPSOL,
		Slow = PinMameApi.MechFlag.SLOW,
		Fast = PinMameApi.MechFlag.FAST,
		StepSw = PinMameApi.MechFlag.STEPSW,
		LengthSw = PinMameApi.MechFlag.LENGTHSW
	}

	public enum PinMameKeycode
	{
		A = PinMameApi.Keycode.A,
		B = PinMameApi.Keycode.B,
		C = PinMameApi.Keycode.C,
		D = PinMameApi.Keycode.D,
		E = PinMameApi.Keycode.E,
		F = PinMameApi.Keycode.F,
		G = PinMameApi.Keycode.G,
		H = PinMameApi.Keycode.H,
		I = PinMameApi.Keycode.I,
		J = PinMameApi.Keycode.J,
		K = PinMameApi.Keycode.K,
		L = PinMameApi.Keycode.L,
		M = PinMameApi.Keycode.M,
		N = PinMameApi.Keycode.N,
		O = PinMameApi.Keycode.O,
		P = PinMameApi.Keycode.P,
		Q = PinMameApi.Keycode.Q,
		R = PinMameApi.Keycode.R,
		S = PinMameApi.Keycode.S,
		T = PinMameApi.Keycode.T,
		U = PinMameApi.Keycode.U,
		V = PinMameApi.Keycode.V,
		W = PinMameApi.Keycode.W,
		X = PinMameApi.Keycode.X,
		Y = PinMameApi.Keycode.Y,
		Z = PinMameApi.Keycode.Z,
		Number0 = PinMameApi.Keycode.NUMBER_0,
		Number1 = PinMameApi.Keycode.NUMBER_1,
		Number2 = PinMameApi.Keycode.NUMBER_2,
		Number3 = PinMameApi.Keycode.NUMBER_3,
		Number4 = PinMameApi.Keycode.NUMBER_4,
		Number5 = PinMameApi.Keycode.NUMBER_5,
		Number6 = PinMameApi.Keycode.NUMBER_6,
		Number7 = PinMameApi.Keycode.NUMBER_7,
		Number8 = PinMameApi.Keycode.NUMBER_8,
		Number9 = PinMameApi.Keycode.NUMBER_9,
		Keypad0 = PinMameApi.Keycode.KEYPAD_0,
		Keypad1 = PinMameApi.Keycode.KEYPAD_1,
		Keypad2 = PinMameApi.Keycode.KEYPAD_2,
		Keypad3 = PinMameApi.Keycode.KEYPAD_3,
		Keypad4 = PinMameApi.Keycode.KEYPAD_4,
		Keypad5 = PinMameApi.Keycode.KEYPAD_5,
		Keypad6 = PinMameApi.Keycode.KEYPAD_6,
		Keypad7 = PinMameApi.Keycode.KEYPAD_7,
		Keypad8 = PinMameApi.Keycode.KEYPAD_8,
		Keypad9 = PinMameApi.Keycode.KEYPAD_9,
		F1 = PinMameApi.Keycode.F1,
		F2 = PinMameApi.Keycode.F2,
		F3 = PinMameApi.Keycode.F3,
		F4 = PinMameApi.Keycode.F4,
		F5 = PinMameApi.Keycode.F5,
		F6 = PinMameApi.Keycode.F6,
		F7 = PinMameApi.Keycode.F7,
		F8 = PinMameApi.Keycode.F8,
		F9 = PinMameApi.Keycode.F9,
		F10 = PinMameApi.Keycode.F10,
		F11 = PinMameApi.Keycode.F11,
		F12 = PinMameApi.Keycode.F12,
		Escape = PinMameApi.Keycode.ESCAPE,
		GraveAccent = PinMameApi.Keycode.GRAVE_ACCENT,
		Minus = PinMameApi.Keycode.MINUS,
		Equals = PinMameApi.Keycode.EQUALS,
		Backspace = PinMameApi.Keycode.BACKSPACE,
		Tab = PinMameApi.Keycode.TAB,
		LeftBracket = PinMameApi.Keycode.LEFT_BRACKET,
		RightBracket = PinMameApi.Keycode.RIGHT_BRACKET,
		Enter = PinMameApi.Keycode.ENTER,
		Semicolon = PinMameApi.Keycode.SEMICOLON,
		Quote = PinMameApi.Keycode.QUOTE,
		Backslash = PinMameApi.Keycode.BACKSLASH,
		Comma = PinMameApi.Keycode.COMMA,
		Period = PinMameApi.Keycode.PERIOD,
		Slash = PinMameApi.Keycode.SLASH,
		Space = PinMameApi.Keycode.SPACE,
		Insert = PinMameApi.Keycode.INSERT,
		Delete = PinMameApi.Keycode.DELETE,
		Home = PinMameApi.Keycode.HOME,
		End = PinMameApi.Keycode.END,
		PageUp = PinMameApi.Keycode.PAGE_UP,
		PageDown = PinMameApi.Keycode.PAGE_DOWN,
		Left = PinMameApi.Keycode.LEFT,
		Right = PinMameApi.Keycode.RIGHT,
		Up = PinMameApi.Keycode.UP,
		Down = PinMameApi.Keycode.DOWN,
		KeypadDivide = PinMameApi.Keycode.KEYPAD_DIVIDE,
		KeypadMultiply = PinMameApi.Keycode.KEYPAD_MULTIPLY,
		KeypadSubtract = PinMameApi.Keycode.KEYPAD_SUBTRACT,
		KeypadAdd = PinMameApi.Keycode.KEYPAD_ADD,
		KeypadEnter = PinMameApi.Keycode.KEYPAD_ENTER,
		PrintScreen = PinMameApi.Keycode.PRINT_SCREEN,
		Pause = PinMameApi.Keycode.PAUSE,
		LeftShift = PinMameApi.Keycode.LEFT_SHIFT,
		RightShift = PinMameApi.Keycode.RIGHT_SHIFT,
		LeftControl = PinMameApi.Keycode.LEFT_CONTROL,
		RightControl = PinMameApi.Keycode.RIGHT_CONTROL,
		LeftAlt = PinMameApi.Keycode.LEFT_ALT,
		RightAlt = PinMameApi.Keycode.RIGHT_ALT,
		ScrollLock = PinMameApi.Keycode.SCROLL_LOCK,
		NumLock = PinMameApi.Keycode.NUM_LOCK,
		CapsLock = PinMameApi.Keycode.CAPS_LOCK,
		LeftSuper = PinMameApi.Keycode.LEFT_SUPER,
		RightSuper = PinMameApi.Keycode.RIGHT_SUPER,
		Menu = PinMameApi.Keycode.MENU,
	};
}
