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

using System;
using System.Collections.Generic;

namespace PinMame
{
	public struct PinMameMechSwitchConfig
	{
		public readonly int SwNo;
		public readonly int StartPos;
		public readonly int EndPos;
		public readonly int Pulse;

		public PinMameMechSwitchConfig(int swNo, int startPos, int endPos, int pulse = 0)
		{
			SwNo = swNo;
			StartPos = startPos;
			EndPos = endPos;
			Pulse = pulse;
		}
	}

	public struct PinMameMechConfig
	{
		public readonly uint Type;
		public readonly int Sol1;
		public readonly int Sol2;
		public readonly int Length;
		public readonly int Steps;
		public readonly int InitialPos;
		public readonly int Acc;
		public readonly int Ret;
		public readonly List<PinMameMechSwitchConfig> SwitchList;

		public PinMameMechConfig(uint type, int sol1, int sol2, int length, int steps, int initialPos, int acc, int ret)
		{
			Type = type;
			Sol1 = sol1;
			Sol2 = sol2;
			Length = length;
			Steps = steps;
			InitialPos = initialPos;
			Acc = acc;
			Ret = ret;
			SwitchList = new List<PinMameMechSwitchConfig>();
		}

		public PinMameMechConfig(uint type, int sol1, int length, int steps, int initialPos, int acc, int ret)
		{
			Type = type;
			Sol1 = sol1;
			Length = length;
			Steps = steps;
			InitialPos = initialPos;
			Acc = acc;
			Ret = ret;
			SwitchList = new List<PinMameMechSwitchConfig>();

			Sol2 = 0;
		}

		public void AddSwitch(PinMameMechSwitchConfig switchConfig)
		{
			if (SwitchList.Count < PinMameApi.MaxMechSwitches)
			{
				SwitchList.Add(switchConfig);
			}
			else
			{
				throw new InvalidOperationException($"Maximum of {PinMameApi.MaxMechSwitches} switches can be added");
			}
		}
	}
}
