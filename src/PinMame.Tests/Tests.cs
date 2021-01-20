using System;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PinMame
{
	[TestClass]
	public class Tests
	{
		[TestMethod]
		public void Start()
		{
			var profilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
			var path = Path.GetFullPath(Path.Combine(profilePath, ".pinmame"));

			var _pinMame = PinMame.Instance(48000, path);

			_pinMame.StartGame("mm_109c", showConsole: true);

			var i = 0;

			while (i++ < 10)
			{
				Thread.Sleep(1000);
			}
		}
	}
}
