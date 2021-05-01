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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace PinMame
{
	/// <summary>
	/// A playable PinMAME game. It's version-specific, so there might be multiple
	/// versions ("clones") for the same logical game.
	/// </summary>
	public class PinMameGame
	{
		/// <summary>
		/// Name, or ROM ID of the game. It's the name of th e zipped ROM file.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Full title of the game, inclusively variant
		/// </summary>
		public string Description { get; }

		/// <summary>
		/// Release year. Not parsable int, might contain values like <c>198?</c>.
		/// </summary>
		public string Year { get; }

		/// <summary>
		/// Game manufacturer
		/// </summary>
		public string Manufacturer { get; }

		/// <summary>
		/// True if the ROM file was found on disk, false otherwise.
		/// </summary>
		public bool RomFound { get; }

		/// <summary>
		/// Clones are ROMs of the same game but with a different version.
		/// </summary>
		public ICollection<PinMameGame> Clones => _clones.OrderBy(cloneGameInfo => cloneGameInfo.Description).ToList();

		internal string CloneOf { get; }

		private readonly uint _flags;
		private readonly List<PinMameGame> _clones;

		/// <summary>
		/// Checks whether this game has a given flag.
		/// </summary>
		/// <param name="flag">Flag to check</param>
		/// <returns>True if flag enabled, false otherwise.</returns>
		public bool HasFlag(PinMameGameDriverFlag flag)
		{
			return (_flags & (uint)flag) != 0;
		}

		internal PinMameGame(IntPtr gamePtr)
		{
			var game = (PinMameApi.PinmameGame)Marshal.PtrToStructure(gamePtr, typeof(PinMameApi.PinmameGame));

			Name = game.name;
			Description = game.description;
			Year = game.year;
			Manufacturer = game.manufacturer;
			RomFound = game.found == 1;
			CloneOf = game.cloneOf;

			_flags = game.flags;
			_clones = new List<PinMameGame>();
		}

		internal void AddClone(PinMameGame game)
		{
			_clones.Add(game);
		}

		public override string ToString() =>
			$"name={Name}, description={Description}, year={Year}, manufacturer={Manufacturer}, flags={_flags}, found={RomFound}";
	}
}
