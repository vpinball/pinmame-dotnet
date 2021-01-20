namespace PinMame.Internal
{
	using System.Runtime.InteropServices;
	using Interop;

	internal static class PinMameApi
	{
		#region Setup functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		internal static extern void SetVPMPath(string path);

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		internal static extern void SetSampleRate(int sampleRate);
		#endregion

		#region Game related functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		internal static extern int StartThreadedGame(string gameName, bool showConsole);

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		internal static extern void StopThreadedGame(bool locking);

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		internal static extern bool IsGameReady();
		#endregion

		#region Pause related functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		internal static extern void ResetGame();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		internal static extern void Pause();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		internal static extern void Continue();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		internal static extern bool IsPaused();
		#endregion

		#region DMD related functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		internal static extern bool NeedsDMDUpdate();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		internal static extern int GetRawDMDWidth();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		internal static extern int GetRawDMDHeight();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		internal static extern int GetRawDMDPixels(byte[] buffer);
		#endregion

		#region Audio related functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		internal static extern int GetAudioChannels();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		internal static extern int GetPendingAudioSamples(float[] buffer, int outChannels, int maxNumber);
		#endregion

		#region Switch related functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		internal static extern bool GetSwitch(int slot);

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		internal static extern void SetSwitch(int slot, bool state);
		#endregion

		#region Lamps related functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		internal static extern int GetMaxLamps();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		internal static extern int GetChangedLamps(int[] changedStates);
		#endregion

		#region Solenoids related functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		internal static extern int GetMaxSolenoids();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		internal static extern int GetChangedSolenoids(int[] changedStates);
		#endregion

		#region GI strings related functions
		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		internal static extern int GetMaxGIStrings();

		[DllImport(Libraries.PinMame, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		internal static extern int GetChangedGIs(int[] changedStates);
		#endregion
	}
}
