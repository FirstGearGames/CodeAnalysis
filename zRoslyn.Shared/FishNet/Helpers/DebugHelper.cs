using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Roslyn.FishNet.Helpers
{
	internal static class Debugg
	{
		private static List<string> _msgs = new();

		/// <summary>
		/// Writes text over a TcpClient.
		/// </summary>
		public static void Log(string txt)
		{
			  _msgs.Add(txt);
		}

		public static string Quoted(this string s) => $"\"{s}\"";

		public static void Send()
		{
			string path = @"D:/Output.txt";
			try
			{
				DateTime startTime = DateTime.Now;
				File.Delete(path);
				while (File.Exists(path))
				{
					Thread.Sleep(100);
					if ((DateTime.Now - startTime).TotalSeconds > 3)
						break;
				}
			}
			catch { }

			File.WriteAllLines(path, _msgs);
			_msgs.Clear();
			//System.Diagnostics.Process.Start(path);
		}
	}
}
