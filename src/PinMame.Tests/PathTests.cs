// pinmame-dotnet
// Copyright (C) 1999-2026 PinMAME development team and contributors
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
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PinMame
{
	/// <summary>
	/// Folder overrides with <see cref="PinMame.SetPath"/>. Needs no ROMs, except for
	/// <see cref="GameFoundThroughRomPath"/>, which runs when PINMAME_TEST_ROM_PATH points to a
	/// folder containing the ROM named by PINMAME_TEST_ROM.
	/// </summary>
	[TestClass]
	public class PathTests
	{
		private string _vpmPath;
		private PinMame _pinMame;

		[TestInitialize]
		public void Initialize()
		{
			_vpmPath = Path.Combine(Path.GetTempPath(), "pinmame-dotnet-tests", Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(Path.Combine(_vpmPath, "roms"));

			PinMame.ResetInstance();
			_pinMame = PinMame.Instance(PinMameAudioFormat.AudioFormatInt16, 44100, _vpmPath);
		}

		[TestCleanup]
		public void Cleanup()
		{
			// overrides live in the native library and outlive the managed instance
			foreach (PinMameFileType fileType in Enum.GetValues(typeof(PinMameFileType))) {
				_pinMame.SetPath(fileType, null);
			}
			PinMame.ResetInstance();
			Directory.Delete(_vpmPath, true);
		}

		[TestMethod]
		public void DefaultsAreBelowVpmPath()
		{
			Assert.AreEqual(Path.Combine(_vpmPath, "roms"), _pinMame.RomPath);
			Assert.AreEqual(Path.Combine(_vpmPath, "nvram"), _pinMame.GetPath(PinMameFileType.Nvram));
			Assert.AreEqual(Path.Combine(_vpmPath, "samples"), _pinMame.GetPath(PinMameFileType.Samples));
			Assert.AreEqual(Path.Combine(_vpmPath, "cfg"), _pinMame.GetPath(PinMameFileType.Config));
			Assert.AreEqual(Path.Combine(_vpmPath, "hi"), _pinMame.GetPath(PinMameFileType.HighScore));
		}

		[TestMethod]
		public void SetPathOverridesEveryFileType()
		{
			foreach (PinMameFileType fileType in Enum.GetValues(typeof(PinMameFileType))) {
				var path = Path.Combine(_vpmPath, "custom", fileType.ToString());
				_pinMame.SetPath(fileType, path);
				Assert.AreEqual(path, _pinMame.GetPath(fileType), fileType.ToString());
			}
		}

		[TestMethod]
		public void RomPathSurvivesApplyConfig()
		{
			var romPath = Path.Combine(_vpmPath, "my roms") + ";" + Path.Combine(_vpmPath, "more roms");
			_pinMame.RomPath = romPath;

			_pinMame.ApplyConfig();

			Assert.AreEqual(romPath, _pinMame.RomPath);
			Assert.AreEqual(Path.Combine(_vpmPath, "nvram"), _pinMame.GetPath(PinMameFileType.Nvram));
		}

		[TestMethod]
		public void ClearingRomPathRestoresDefault()
		{
			_pinMame.RomPath = Path.Combine(_vpmPath, "my roms");
			_pinMame.RomPath = null;
			Assert.AreEqual(Path.Combine(_vpmPath, "roms"), _pinMame.RomPath);

			_pinMame.RomPath = Path.Combine(_vpmPath, "my roms");
			_pinMame.RomPath = string.Empty;
			Assert.AreEqual(Path.Combine(_vpmPath, "roms"), _pinMame.RomPath);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void UnknownFileTypeThrows()
		{
			_pinMame.SetPath((PinMameFileType)99, _vpmPath);
		}

		[TestMethod]
		public void GameFoundThroughRomPath()
		{
			var romPath = Environment.GetEnvironmentVariable("PINMAME_TEST_ROM_PATH");
			var rom = Environment.GetEnvironmentVariable("PINMAME_TEST_ROM");
			if (string.IsNullOrEmpty(romPath) || string.IsNullOrEmpty(rom)) {
				Assert.Inconclusive("Set PINMAME_TEST_ROM_PATH and PINMAME_TEST_ROM to run this test.");
			}

			Assert.IsFalse(_pinMame.GetGame(rom).RomFound, "ROM must not be in the empty VPM path.");

			_pinMame.RomPath = romPath;
			Assert.IsTrue(_pinMame.GetGame(rom).RomFound, "ROM not found through the ROM path.");

			_pinMame.ApplyConfig();
			Assert.IsTrue(_pinMame.GetGame(rom).RomFound, "ROM path lost when re-applying the config.");

			_pinMame.RomPath = null;
			Assert.IsFalse(_pinMame.GetGame(rom).RomFound, "ROM still found after clearing the ROM path.");
		}

		[TestMethod]
		public void FindVpmRomPathOnlyReturnsExistingFolders()
		{
			var romPath = PinMame.FindVpmRomPath();
			if (romPath == null) {
				return;
			}
			foreach (var folder in romPath.Split(';')) {
				Assert.IsTrue(Directory.Exists(folder), folder);
			}
		}
	}
}
