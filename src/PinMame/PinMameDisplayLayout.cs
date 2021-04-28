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

	public struct PinMameDisplayLayout
	{
		public PinMameDisplayType type;
		public int top;
		public int left;
		public int length;
		public int width;
		public int height;
		public int depth;
		public byte[] levels;

		internal PinMameDisplayLayout(PinMameApi.PinmameDisplayLayout displayLayout)
		{
			type = (PinMameDisplayType)displayLayout.type;
			top = displayLayout.top;
			left = displayLayout.left;
			length = displayLayout.length;
			width = displayLayout.width;
			height = displayLayout.height;
			depth = displayLayout.depth;
			levels = null;
		}

		internal PinMameDisplayLayout(PinMameApi.PinmameDisplayLayout displayLayout, PinMameApi.PinmameHardwareGen hardwareGen)
		{
			type = (PinMameDisplayType)displayLayout.type;
			top = displayLayout.top;
			left = displayLayout.left;
			length = displayLayout.length;
			width = displayLayout.width;
			height = displayLayout.height;
			depth = displayLayout.depth;
			levels = null;

			if (IsDmd)
			{
				if (depth == 2)
				{
					levels = PinMameApi.PinmameDmdLevels.Wpc;
				}
				else
				{
					levels = (hardwareGen & (PinMameApi.PinmameHardwareGen.SAM | PinMameApi.PinmameHardwareGen.SPA)) != 0 ?
						PinMameApi.PinmameDmdLevels.Sam : PinMameApi.PinmameDmdLevels.Gts3;
				}
			}
		}

		public bool IsDmd =>
			type == PinMameDisplayType.DMD
			|| type == (PinMameDisplayType.DMD | PinMameDisplayType.DMDNOAA)
			|| type == (PinMameDisplayType.DMD | PinMameDisplayType.DMDNOAA | PinMameDisplayType.NODISP);

		public override string ToString() =>
			$"type={type}, top={top}, left={left}, length={length}, width={width}, height={height}, depth={depth}";
	}
}
