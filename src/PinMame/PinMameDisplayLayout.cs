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

using System.Collections.Generic;

namespace PinMame
{
	public struct PinMameDisplayLayout
	{
		public readonly PinMameDisplayType Type;
		public readonly int Top;
		public readonly int Left;
		public readonly int Length;
		public readonly int Width;
		public readonly int Height;
		public readonly int Depth;
		public readonly Dictionary<byte, byte> Levels;

		internal PinMameDisplayLayout(PinMameApi.DisplayLayout displayLayout)
		{
			Type = (PinMameDisplayType)displayLayout.type;
			Top = displayLayout.top;
			Left = displayLayout.left;
			Length = displayLayout.length;
			Width = displayLayout.width;
			Height = displayLayout.height;
			Depth = displayLayout.depth;
			Levels = null;
		}

		internal PinMameDisplayLayout(PinMameApi.DisplayLayout displayLayout, PinMameApi.HardwareGen hardwareGen)
		{
			Type = (PinMameDisplayType)displayLayout.type;
			Top = displayLayout.top;
			Left = displayLayout.left;
			Length = displayLayout.length;
			Width = displayLayout.width;
			Height = displayLayout.height;
			Depth = displayLayout.depth;
			Levels = null;

			if (!IsDmd) {
				return;
			}

			if (Depth == 2) {
				Levels = PinMameApi.PinmameDmdLevels.Wpc;

			} else {
				Levels = (hardwareGen & (PinMameApi.HardwareGen.SAM | PinMameApi.HardwareGen.SPA)) != 0
					? PinMameApi.PinmameDmdLevels.Sam
					: PinMameApi.PinmameDmdLevels.Gts3;
			}
		}

		public bool IsDmd => (Type & PinMameDisplayType.Dmd) == PinMameDisplayType.Dmd;

		public override string ToString() =>
			$"type={Type}, top={Top}, left={Left}, length={Length}, width={Width}, height={Height}, depth={Depth}";
	}
}
