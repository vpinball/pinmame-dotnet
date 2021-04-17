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
	using Internal;

	public enum PinMameDisplayType : int
	{
		SEG16 = PinMameApi.PinmameDisplayType.SEG16,       // 16 segments
		SEG16R = PinMameApi.PinmameDisplayType.SEG16R,     // 16 segments with comma and period reversed
		SEG10 = PinMameApi.PinmameDisplayType.SEG10,       // 9 segments and comma
		SEG9 = PinMameApi.PinmameDisplayType.SEG9,         // 9 segments
		SEG8 = PinMameApi.PinmameDisplayType.SEG8,         // 7 segments and comma
		SEG8D = PinMameApi.PinmameDisplayType.SEG8D,       // 7 segments and period
		SEG7 = PinMameApi.PinmameDisplayType.SEG7,         // 7 segments
		SEG87 = PinMameApi.PinmameDisplayType.SEG87,       // 7 segments, comma every three
		SEG87F = PinMameApi.PinmameDisplayType.SEG87F,     // 7 segments, forced comma every three
		SEG98 = PinMameApi.PinmameDisplayType.SEG98,       // 9 segments, comma every three
		SEG98F = PinMameApi.PinmameDisplayType.SEG98F,     // 9 segments, forced comma every three
		SEG7S = PinMameApi.PinmameDisplayType.SEG7S,       // 7 segments, small
		SEG7SC = PinMameApi.PinmameDisplayType.SEG7SC,     // 7 segments, small, with comma
		SEG16S = PinMameApi.PinmameDisplayType.SEG16S,     // 16 segments with split top and bottom line
		DMD = PinMameApi.PinmameDisplayType.DMD,           // DMD Display
		VIDEO = PinMameApi.PinmameDisplayType.VIDEO,       // VIDEO Display
		SEG16N = PinMameApi.PinmameDisplayType.SEG16N,     // 16 segments without commas
		SEG16D = PinMameApi.PinmameDisplayType.SEG16D,     // 16 segments with periods only,
		SEGALL = PinMameApi.PinmameDisplayType.SEGALL,     // maximum segment definition number
		IMPORT = PinMameApi.PinmameDisplayType.IMPORT,     // Link to another display layout
		SEGMASK = PinMameApi.PinmameDisplayType.SEGMASK,   // Note that CORE_IMPORT must be part of the segmask as well!
		SEGHIBIT = PinMameApi.PinmameDisplayType.SEGHIBIT,
		SEGREV = PinMameApi.PinmameDisplayType.SEGREV,
		DMDNOAA = PinMameApi.PinmameDisplayType.DMDNOAA,
		NODISP = PinMameApi.PinmameDisplayType.NODISP,
		SEG8H = PinMameApi.PinmameDisplayType.SEG8H,
		SEG7H = PinMameApi.PinmameDisplayType.SEG7H,
		SEG87H = PinMameApi.PinmameDisplayType.SEG87H,
		SEG87FH = PinMameApi.PinmameDisplayType.SEG87FH,
		SEG7SH = PinMameApi.PinmameDisplayType.SEG7SH,
		SEG7SCH = PinMameApi.PinmameDisplayType.SEG7SCH
	}

	public enum PinMameHardwareGen : ulong
	{
		WPCALPHA_1 = PinMameApi.PinmameHardwareGen.WPCALPHA_1,   // Alpha-numeric display S11 sound, Dr Dude 10/90
		WPCALPHA_2 = PinMameApi.PinmameHardwareGen.WPCALPHA_2,   // Alpha-numeric display,  - The Machine BOP 4/91
		WPCDMD = PinMameApi.PinmameHardwareGen.WPCDMD,           // Dot Matrix Display, Terminator 2 7/91 - Party Zone 10/91
		WPCFLIPTRON = PinMameApi.PinmameHardwareGen.WPCFLIPTRON, // Fliptronic flippers, Addams Family 2/92 - Twilight Zone 5/93
		WPCDCS = PinMameApi.PinmameHardwareGen.WPCDCS,           // DCS Sound system, Indiana Jones 10/93 - Popeye 3/94
		WPCSECURITY = PinMameApi.PinmameHardwareGen.WPCSECURITY, // Security chip, World Cup Soccer 3/94 - Jackbot 10/95
		WPC95DCS = PinMameApi.PinmameHardwareGen.WPC95DCS,       // Hybrid WPC95 driver + DCS sound, Who Dunnit
		WPC95 = PinMameApi.PinmameHardwareGen.WPC95,             // Integrated boards, Congo 3/96 - Cactus Canyon 2/99
		S11 = PinMameApi.PinmameHardwareGen.S11,                 // No external sound board
		S11X = PinMameApi.PinmameHardwareGen.S11X,               // S11C sound board
		S11A = PinMameApi.PinmameHardwareGen.S11A,
		S11B = PinMameApi.PinmameHardwareGen.S11B,
		S11B2 = PinMameApi.PinmameHardwareGen.S11B2,             // Jokerz! sound board
		S11C = PinMameApi.PinmameHardwareGen.S11C,               // No CPU board sound
		S9 = PinMameApi.PinmameHardwareGen.S9,                   // S9 CPU, 4x7+1x4
		DE = PinMameApi.PinmameHardwareGen.DE,                   // DE AlphaSeg
		DEDMD16 = PinMameApi.PinmameHardwareGen.DEDMD16,         // DE 128x16
		DEDMD32 = PinMameApi.PinmameHardwareGen.DEDMD32,         // DE 128x32
		DEDMD64 = PinMameApi.PinmameHardwareGen.DEDMD64,         // DE 192x64
		S7 = PinMameApi.PinmameHardwareGen.S7,                   // S7 CPU
		S6 = PinMameApi.PinmameHardwareGen.S6,                   // S6 CPU
		S4 = PinMameApi.PinmameHardwareGen.S4,                   // S4 CPU
		S3C = PinMameApi.PinmameHardwareGen.S3C,                 // S3 CPU No Chimes
		S3 = PinMameApi.PinmameHardwareGen.S3,
		BY17 = PinMameApi.PinmameHardwareGen.BY17,
		BY35 = PinMameApi.PinmameHardwareGen.BY35,
		STMPU100 = PinMameApi.PinmameHardwareGen.STMPU100,       // Stern MPU - 100
		STMPU200 = PinMameApi.PinmameHardwareGen.STMPU200,       // Stern MPU - 200
		ASTRO = PinMameApi.PinmameHardwareGen.ASTRO,             // Unknown Astro game, Stern hardware
		HNK = PinMameApi.PinmameHardwareGen.HNK,                 // Hankin
		BYPROTO = PinMameApi.PinmameHardwareGen.BYPROTO,         // Bally Bow & Arrow prototype
		BY6803 = PinMameApi.PinmameHardwareGen.BY6803,
		BY6803A = PinMameApi.PinmameHardwareGen.BY6803A,
		BOWLING = PinMameApi.PinmameHardwareGen.BOWLING,         // Big Ball Bowling, Stern hardware
		GTS1 = PinMameApi.PinmameHardwareGen.GTS1,               // GTS1
		GTS80 = PinMameApi.PinmameHardwareGen.GTS80,             // GTS80
		GTS80A = PinMameApi.PinmameHardwareGen.GTS80A,
		GTS80B = PinMameApi.PinmameHardwareGen.GTS80B,           // GTS80B
		WS = PinMameApi.PinmameHardwareGen.WS,                   // Whitestar
		WS_1 = PinMameApi.PinmameHardwareGen.WS_1,               // Whitestar with extra RAM
		WS_2 = PinMameApi.PinmameHardwareGen.WS_2,               // Whitestar with extra DMD
		GTS3 = PinMameApi.PinmameHardwareGen.GTS3,               // GTS3
		ZAC1 = PinMameApi.PinmameHardwareGen.ZAC1,
		ZAC2 = PinMameApi.PinmameHardwareGen.ZAC2,
		SAM = PinMameApi.PinmameHardwareGen.SAM,                 // Stern SAM
		ALVG = PinMameApi.PinmameHardwareGen.ALVG,               // Alvin G Hardware
		ALVG_DMD2 = PinMameApi.PinmameHardwareGen.ALVG_DMD2,     // Alvin G Hardware, with more shades
		MRGAME = PinMameApi.PinmameHardwareGen.MRGAME,           // Mr.Game Hardware
		SLEIC = PinMameApi.PinmameHardwareGen.SLEIC,             // Sleic Hardware
		WICO = PinMameApi.PinmameHardwareGen.WICO,               // Wico Hardware 
		SPA = PinMameApi.PinmameHardwareGen.SPA,                 // Stern PA
		ALLWPC = PinMameApi.PinmameHardwareGen.ALLWPC,           // All WPC
		ALLS11 = PinMameApi.PinmameHardwareGen.ALLS11,           // All Sys11
		ALLBY35 = PinMameApi.PinmameHardwareGen.ALLBY35,         // All Bally35 and derivatives
		ALLS80 = PinMameApi.PinmameHardwareGen.ALLS80,           // All GTS80
		ALLWS = PinMameApi.PinmameHardwareGen.ALLWS,             // All Whitestar
	}
}
