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

	public enum PinMameDisplayType
	{
		SEG16 = PinMameApi.PinmameDisplayType.SEG16,     // 16 segments
		SEG16R = PinMameApi.PinmameDisplayType.SEG16R,   // 16 segments with comma and period reversed
		SEG10 = PinMameApi.PinmameDisplayType.SEG10,     // 9  segments and comma
		SEG9 = PinMameApi.PinmameDisplayType.SEG9,       // 9  segments
		SEG8 = PinMameApi.PinmameDisplayType.SEG8,       // 7  segments and comma
		SEG8D = PinMameApi.PinmameDisplayType.SEG8D,     // 7  segments and period
		SEG7 = PinMameApi.PinmameDisplayType.SEG7,       // 7  segments
		SEG87 = PinMameApi.PinmameDisplayType.SEG87,     // 7  segments, comma every three
		SEG87F = PinMameApi.PinmameDisplayType.SEG87F,   // 7  segments, forced comma every three
		SEG98 = PinMameApi.PinmameDisplayType.SEG98,     // 9  segments, comma every three
		SEG98F = PinMameApi.PinmameDisplayType.SEG98F,   // 9  segments, forced comma every three
		SEG7S = PinMameApi.PinmameDisplayType.SEG7S,     // 7  segments, small
		SEG7SC = PinMameApi.PinmameDisplayType.SEG7SC,   // 7  segments, small, with comma
		SEG16S = PinMameApi.PinmameDisplayType.SEG16S,   // 16 segments with split top and bottom line
		DMD = PinMameApi.PinmameDisplayType.DMD,         // DMD Display
		VIDEO = PinMameApi.PinmameDisplayType.VIDEO,     // VIDEO Display
		SEG16N = PinMameApi.PinmameDisplayType.SEG16N,   // 16 segments without commas
		SEG16D = PinMameApi.PinmameDisplayType.SEG16D    // 16 segments with periods only
	}

	public struct PinMameDisplayLayout
	{
		public PinMameDisplayType type;
		public int top;
		public int left;
		public int length;
		public int height;
		public int width;

		internal PinMameDisplayLayout(PinMameApi.PinmameDisplayLayout displayLayout)
		{
			type = (PinMameDisplayType)displayLayout.type;
			top = displayLayout.top;
			left = displayLayout.left;
			length = displayLayout.length;
			height = displayLayout.height;
			width = displayLayout.width;
		}
	}
}
