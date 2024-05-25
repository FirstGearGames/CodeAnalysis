using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace RoslynLearning.Helpers
{

    internal static class Debugg
    {
        private const int PORT_NO = 5005;
        private const string SERVER_IP = "127.0.0.1";


        private static List<string> _msgs = new();

        private static StreamWriter? _writer;
        /// <summary>
        /// Writes text over a TcpClient.
        /// </summary>
        public static void Log(string txt)
        {
            //if (_writer == null)
            //_writer = new StreamWriter(@"D:/Output.txt", false);

            //_writer.WriteLine(txt);
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
            System.Diagnostics.Process.Start(path);

       
            //if (_writer == null)
            //    return;
            //_writer.Flush();
            //_writer.Close();
            //_writer = null;       
        }

    }
}
