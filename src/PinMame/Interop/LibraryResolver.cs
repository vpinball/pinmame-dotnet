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
using NLog;

namespace PinMame.Interop
{
	/// <summary>
	/// Resolves native library names at runtime based on platform and architecture.
	/// This is necessary for Windows to select between pinmame.dll (x86) and pinmame64.dll (x64).
	/// </summary>
	internal static class LibraryResolver
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		private const string LogPrefix = "[PinMAME]";
		private static int _loggedResolved;

		private static bool _isInitialized;

		// These types/methods only exist on some runtimes (.NET Core 3+/ .NET 5+).
		// We compile for netstandard2.1, so we use reflection and fall back to default resolution.
		private static readonly Type NativeLibraryType = Type.GetType("System.Runtime.InteropServices.NativeLibrary, System.Runtime.InteropServices");
		private static readonly Type DllImportResolverType = Type.GetType("System.Runtime.InteropServices.DllImportResolver, System.Runtime.InteropServices");
		private static readonly MethodInfo ResolveMethod = typeof(LibraryResolver).GetMethod(nameof(ResolveDllImport), BindingFlags.Static | BindingFlags.NonPublic);
		private static readonly MethodInfo TryLoadMethod = GetTryLoadMethod();
		private static readonly MethodInfo SetDllImportResolverMethod = GetSetDllImportResolverMethod();

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
			TrySetDllImportResolverViaReflection();
		}

		private static IntPtr ResolveDllImport(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
		{
			// This project only needs runtime resolution for Windows, where the DLL name differs by architecture.
			// Non-Windows platforms use explicit names in Interop/Libraries.$(TargetOS).cs.
			if (libraryName != "pinmame") {
				return IntPtr.Zero;
			}
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
				return IntPtr.Zero;
			}

			// Default behavior: pinmame64.dll on x64, pinmame.dll otherwise.
			var resolvedName = RuntimeInformation.ProcessArchitecture == Architecture.X64
				? "pinmame64.dll"
				: "pinmame.dll";

			var handle = TryLoadLibrary(resolvedName, assembly, searchPath);
			if (handle != IntPtr.Zero) {
				LogResolvedOnce(resolvedName, handle);
			}
			return handle;
		}

		private static void LogResolvedOnce(string libraryName, IntPtr handle)
		{
			if (System.Threading.Interlocked.Exchange(ref _loggedResolved, 1) != 0) {
				return;
			}
			try {
				var path = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
					? GetWindowsModulePath(handle)
					: null;
				Logger.Info($"{LogPrefix} [pinmame-dotnet] Resolved native '{libraryName}' => '{path ?? "<unknown>"}' (0x{handle.ToInt64():X})");
			} catch {
				// ignore
			}
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern uint GetModuleFileNameW(IntPtr hModule, char[] lpFilename, uint nSize);

		private static string GetWindowsModulePath(IntPtr hModule)
		{
			var buffer = new char[2048];
			var len = GetModuleFileNameW(hModule, buffer, (uint)buffer.Length);
			return len == 0 ? null : new string(buffer, 0, (int)len);
		}

		private static IntPtr TryLoadLibrary(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
		{
			try {
				if (TryLoadMethod == null) {
					return IntPtr.Zero;
				}

				var parameters = new object[] { libraryName, assembly, searchPath, IntPtr.Zero };
				var success = (bool)TryLoadMethod.Invoke(null, parameters);
				return success ? (IntPtr)parameters[3] : IntPtr.Zero;
			}
			catch {
				// If reflection fails, return IntPtr.Zero to use default behavior.
				return IntPtr.Zero;
			}
		}

		private static void TrySetDllImportResolverViaReflection()
		{
			try {
				if (SetDllImportResolverMethod == null || DllImportResolverType == null || ResolveMethod == null) {
					return;
				}

				var resolver = Delegate.CreateDelegate(DllImportResolverType, ResolveMethod);
				SetDllImportResolverMethod.Invoke(null, new object[] { typeof(LibraryResolver).Assembly, resolver });
			}
			catch {
				// If reflection fails, fall back to default behavior.
			}
		}

		private static MethodInfo GetTryLoadMethod()
		{
			try {
				return NativeLibraryType?.GetMethod(
					"TryLoad",
					BindingFlags.Public | BindingFlags.Static,
					null,
					new[] { typeof(string), typeof(Assembly), typeof(DllImportSearchPath?), typeof(IntPtr).MakeByRefType() },
					null);
			}
			catch {
				return null;
			}
		}

		private static MethodInfo GetSetDllImportResolverMethod()
		{
			try {
				return (NativeLibraryType == null || DllImportResolverType == null)
					? null
					: NativeLibraryType.GetMethod(
						"SetDllImportResolver",
						BindingFlags.Public | BindingFlags.Static,
						null,
						new[] { typeof(Assembly), DllImportResolverType },
						null);
			}
			catch {
				return null;
			}
		}
	}
}
