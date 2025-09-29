#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace CodeAnalysis.Common.Helpers
{
    public static class Debugg
    {
        private static string AssemblyName;
        private static List<string> _msgs = new();

        public static void SetAssemblyName(string value)
        {
            AssemblyName = value;
        }

        /// <summary>
        /// Writes text over a TcpClient.
        /// </summary>
        public static void Log(string txt)
        {
            if (txt.Length == 0)
                txt = " ";
            _msgs.Add(txt);
        }

        public static string Quoted(this string s) => $"\"{s}\"";

        public static void Send()
        {
            // bool fileExist = File.Exists(@"D:\Development\Personal\FishNets\SourceGenFix.txt");
            // if (!fileExist)
            //     fileExist = File.Exists(@"D:\Development\FishNet\SourceGenFix.txt");
            // if (!fileExist)
            // {
            //     _msgs.Clear();
            //     return;
            // }

            string path = @"E:/Output_" + AssemblyName + ".txt";
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