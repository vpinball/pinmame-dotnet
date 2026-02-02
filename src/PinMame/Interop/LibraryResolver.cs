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
using System.Reflection;
using System.Runtime.InteropServices;

namespace PinMame.Interop
{
	/// <summary>
	/// Resolves native library names at runtime based on platform and architecture.
	/// This is necessary for Windows to select between pinmame.dll (x86) and pinmame64.dll (x64).
	/// </summary>
	internal static class LibraryResolver
	{
		private static bool _isInitialized;

		/// <summary>
		/// Initialize the DllImport resolver. Called automatically via static constructor.
		/// </summary>
		internal static void Initialize()
		{
			if (_isInitialized)
				return;

			_isInitialized = true;

			// Use reflection to call SetDllImportResolver if available at runtime
			// This works when running on .NET Core 3.0+ or .NET 5+, even though we compile against netstandard2.1
			var success = TrySetDllImportResolverViaReflection();
		}

		private static IntPtr ResolveDllImport(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
		{
			// Only resolve our pinmame library
			if (libraryName != "pinmame")
				return IntPtr.Zero;

			string resolvedName = libraryName;

			// Windows: select architecture-specific DLL
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				resolvedName = RuntimeInformation.ProcessArchitecture == Architecture.X64
					? "pinmame64.dll"
					: "pinmame.dll";
			}
			// Other platforms use the base name (extensions and symlinks are handled by the OS)
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				resolvedName = "libpinmame.dylib";
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				resolvedName = "libpinmame.so";
			}

			// Try to load the resolved library using reflection (supports .NET Core 3.0+/.NET 5+ at runtime)
			return TryLoadLibrary(resolvedName, assembly, searchPath);
		}

		private static IntPtr TryLoadLibrary(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
		{
			try
			{
				// Try to get NativeLibrary.TryLoad method
				var nativeLibraryType = Type.GetType("System.Runtime.InteropServices.NativeLibrary, System.Runtime.InteropServices");
				if (nativeLibraryType == null)
					return IntPtr.Zero;

				var tryLoadMethod = nativeLibraryType.GetMethod(
					"TryLoad",
					BindingFlags.Public | BindingFlags.Static,
					null,
					new[] { typeof(string), typeof(Assembly), typeof(DllImportSearchPath?), typeof(IntPtr).MakeByRefType() },
					null);

				if (tryLoadMethod == null)
					return IntPtr.Zero;

				// Call NativeLibrary.TryLoad
				var parameters = new object[] { libraryName, assembly, searchPath, IntPtr.Zero };
				var success = (bool)tryLoadMethod.Invoke(null, parameters);

				return success ? (IntPtr)parameters[3] : IntPtr.Zero;
			}
			catch
			{
				// If reflection fails, return IntPtr.Zero to use default behavior
				return IntPtr.Zero;
			}
		}

		private static bool TrySetDllImportResolverViaReflection()
		{
			try
			{
				// Try to get NativeLibrary type from System.Runtime.InteropServices
				var nativeLibraryType = Type.GetType("System.Runtime.InteropServices.NativeLibrary, System.Runtime.InteropServices");
				if (nativeLibraryType == null)
					return false;

				// Get DllImportResolver delegate type
				var dllImportResolverType = Type.GetType("System.Runtime.InteropServices.DllImportResolver, System.Runtime.InteropServices");
				if (dllImportResolverType == null)
					return false;

				// Get SetDllImportResolver method - takes (Assembly, DllImportResolver)
				var setResolverMethod = nativeLibraryType.GetMethod(
					"SetDllImportResolver",
					BindingFlags.Public | BindingFlags.Static,
					null,
					new[] { typeof(Assembly), dllImportResolverType },
					null);

				if (setResolverMethod == null)
					return false;

				// Create delegate for our resolver using the DllImportResolver type
				var resolver = Delegate.CreateDelegate(dllImportResolverType, typeof(LibraryResolver).GetMethod(nameof(ResolveDllImport), BindingFlags.Static | BindingFlags.NonPublic));

				// Call SetDllImportResolver
				setResolverMethod.Invoke(null, new object[] { typeof(LibraryResolver).Assembly, resolver });
				return true;
			}
			catch (Exception ex)
			{
				// If reflection fails, fall back to default behavior
				return false;
			}
		}
	}

}