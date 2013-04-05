/* LogManager.cs
 *
 * Copyright © 2013 by Adam Hellberg
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of
 * this software and associated documentation files (the "Software"), to deal in
 * the Software without restriction, including without limitation the rights to
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
 * of the Software, and to permit persons to whom the Software is furnished to do
 * so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 * 
 * Disclaimer: SwitchBladeSteam is in no way affiliated
 * with Razer and/or any of its employees and/or licensors.
 * Adam Hellberg does not take responsibility for any harm caused, direct
 * or indirect, to any Razer peripherals via the use of SwitchBladeSteam.
 * 
 * "Razer" is a trademark of Razer USA Ltd.
 */

using System;
using System.IO;
using System.Linq;
using Sharparam.SwitchBladeSteam.Native;
using log4net;
using log4net.Config;

#if DEBUG
using System.Text;
using Microsoft.Win32.SafeHandles;
#endif

namespace Sharparam.SwitchBladeSteam.Logging
{
    public static class LogManager
    {
        private static bool _loaded;
        private static bool _consoleLoaded;

        public static void LoadConfig(string file = null)
        {
            if (file == null)
            {
                if (File.Exists(AppDomain.CurrentDomain.FriendlyName + ".config"))
                    XmlConfigurator.Configure();
                else
                    BasicConfigurator.Configure();
            }
            else
            {
                if (File.Exists(file))
                    XmlConfigurator.Configure(new FileInfo(file));
                else
                {
                    LoadConfig();
                    return;
                }
            }

            _loaded = true;
        }

        public static ILog GetLogger(object sender)
        {
            if (!_loaded)
                LoadConfig();

            return log4net.LogManager.GetLogger(sender.GetType().ToString() == "System.RuntimeType" ? (Type) sender : sender.GetType());
        }

        public static void SetupConsole()
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
                return;

            WinAPI.AllocConsole();
            var stdHandle = WinAPI.GetStdHandle(WinAPI.STD_OUTPUT_HANDLE);
            var safeFileHandle = new SafeFileHandle(stdHandle, true);
            var fileStream = new FileStream(safeFileHandle, FileAccess.Write);
            var encoding = Encoding.GetEncoding(WinAPI.CODE_PAGE);
            var stdOut = new StreamWriter(fileStream, encoding) {AutoFlush = true};
            Console.SetOut(stdOut);
            _consoleLoaded = true;
#endif
        }

        public static void DestroyConsole()
        {
#if DEBUG
            if (_consoleLoaded)
                WinAPI.FreeConsole();
#endif
        }

        public static void ClearOldLogs(int daysOld = 7, string logsDir = "logs")
        {
            var log = GetLogger(typeof (LogManager));

            log.InfoFormat(">> ClearOldLogs({0}, \"{1}\")", daysOld, logsDir);

            if (!Directory.Exists(logsDir))
            {
                log.InfoFormat("Directory {0} not found, no logs to clear", logsDir);
                log.Info("<< ClearOldLogs()");
                return;
            }

            var now = DateTime.Now;
            var max = new TimeSpan(daysOld, 0, 0, 0);
            var count = 0;
            foreach (var file in from file in Directory.GetFiles(logsDir)
                let modTime = File.GetLastAccessTime(file)
                let age = now.Subtract(modTime)
                where age > max
                select file)
            {
                try
                {
                    File.Delete(file);
                    log.InfoFormat("Deleted old log file: {0}", file);
                    count++;
                }
                catch (IOException ex)
                {
                    log.WarnFormat("Failed to delete log file: {0} ({1})", file, ex.Message);
                }
            }

            log.InfoFormat("Done! Cleared {0} log files.", count);
            log.Info("<< ClearOldLogs()");
        }
    }
}
