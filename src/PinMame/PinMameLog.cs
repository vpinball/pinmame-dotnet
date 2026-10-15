// pinmame-dotnet
// Copyright (C) 1999-2026 PinMAME development team and contributors
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions
// are met:
//
// 1. Redistributions of source code must retain the above copyright
//    notice, this list of conditions and the following disclaimer.
//
// 2. Redistributions in binary form must reproduce the above copyright
//    notice, this list of conditions and the following disclaimer in the
//    documentation and/or other materials provided with the distribution.
//
// 3. Neither the name of the copyright holder nor the names of its
//    contributors may be used to endorse or promote products derived
//    from this software without specific prior written permission.
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
// LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY
// WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Runtime.InteropServices;

namespace PinMame
{
	/// <summary>
	/// How serious a log message from libpinmame is.
	/// </summary>
	public enum PinMameLogLevel
	{
		Debug = 0,
		Info = 1,
		Error = 2,
	}

	/// <summary>
	/// Turns libpinmame's printf-style log calls into strings.
	/// </summary>
	///
	/// <remarks>
	/// libpinmame passes the format and a <c>va_list</c>. The <c>va_list</c> is only valid during
	/// the callback and can only be read once, so it's formatted right there with the C runtime's
	/// <c>vsnprintf</c> (on Windows, the UCRT's <c>__stdio_common_vsprintf</c> behind it, since
	/// ucrtbase.dll doesn't export <c>vsnprintf</c> itself). On every supported ABI a <c>va_list</c>
	/// parameter arrives as a pointer (on arm64 Linux, as the pointer to the caller's copy), which
	/// is what the formatting function takes again.
	/// </remarks>
	internal static class PinMameLogFormatter
	{
		private const int BufferSize = 1024;

		/// <summary>
		/// <c>_CRT_INTERNAL_PRINTF_STANDARD_SNPRINTF_BEHAVIOR</c>: C99 <c>vsnprintf</c> semantics,
		/// always terminated, returns the full length.
		/// </summary>
		private const ulong StandardSnprintfBehavior = 0x2;

		private static readonly char[] LineEnd = { '\r', '\n' };

		[ThreadStatic]
		private static byte[] _buffer;

		private static bool _unavailable;

		/// <summary>
		/// The formatted message, or the bare format if formatting isn't available.
		/// </summary>
		internal static string Format(IntPtr format, IntPtr args)
		{
			if (format == IntPtr.Zero) {
				return string.Empty;
			}
			if (_unavailable || args == IntPtr.Zero) {
				return Bare(format);
			}
			var buffer = _buffer ?? (_buffer = new byte[BufferSize]);
			try {
				var written = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
					? WindowsVsprintf(StandardSnprintfBehavior, buffer, (UIntPtr)buffer.Length, format, IntPtr.Zero, args)
					: UnixVsnprintf(buffer, (UIntPtr)buffer.Length, format, args);
				if (written < 0) {
					return Bare(format);
				}
				// decoded like every other string crossing this API: as ANSI, which is the system
				// code page on Windows and UTF-8 elsewhere
				var length = Math.Min(written, buffer.Length - 1);
				var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
				try {
					return (Marshal.PtrToStringAnsi(handle.AddrOfPinnedObject(), length) ?? string.Empty).TrimEnd(LineEnd);
				} finally {
					handle.Free();
				}

			} catch (Exception e) when (e is DllNotFoundException || e is EntryPointNotFoundException) {
				_unavailable = true;
				return Bare(format);
			}
		}

		private static string Bare(IntPtr format) => (Marshal.PtrToStringAnsi(format) ?? string.Empty).TrimEnd(LineEnd);

		[DllImport("ucrtbase.dll", EntryPoint = "__stdio_common_vsprintf", CallingConvention = CallingConvention.Cdecl)]
		private static extern int WindowsVsprintf(ulong options, byte[] buffer, UIntPtr size, IntPtr format, IntPtr locale, IntPtr args);

		[DllImport("libc", EntryPoint = "vsnprintf", CallingConvention = CallingConvention.Cdecl)]
		private static extern int UnixVsnprintf(byte[] buffer, UIntPtr size, IntPtr format, IntPtr args);
	}
}
