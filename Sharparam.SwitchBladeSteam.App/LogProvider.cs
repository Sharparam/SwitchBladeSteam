/* LogProvider.cs
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
using log4net.Config;
using Sharparam.SharpBlade.Logging;

using ILog = Sharparam.SharpBlade.Logging.ILog;

namespace Sharparam.SwitchBladeSteam.App
{
    public class LogProvider : ILogProvider
    {
        private static bool _loaded;

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

        public ILog GetLogger(Type type)
        {
            if (!_loaded)
                LoadConfig();

            return new Log(log4net.LogManager.GetLogger(type));
        }

        public ILog GetLogger(string name)
        {
            if (!_loaded)
                LoadConfig();

            return new Log(log4net.LogManager.GetLogger(name));
        }
    }

    public class Log : ILog
    {
        private log4net.ILog _log;

        internal Log(log4net.ILog log)
        {
            _log = log;
        }

        public void Debug(object message)
        {
            _log.Debug(message);
        }

        public void Info(object message)
        {
            _log.Info(message);
        }

        public void Warn(object message)
        {
            _log.Warn(message);
        }

        public void Error(object message)
        {
            _log.Error(message);
        }

        public void Fatal(object message)
        {
            _log.Fatal(message);
        }

        public void Debug(object message, Exception exception)
        {
            _log.Debug(message, exception);
        }

        public void Info(object message, Exception exception)
        {
            _log.Info(message, exception);
        }

        public void Warn(object message, Exception exception)
        {
            _log.Warn(message, exception);
        }

        public void Error(object message, Exception exception)
        {
            _log.Error(message, exception);
        }

        public void Fatal(object message, Exception exception)
        {
            _log.Fatal(message, exception);
        }

        public void DebugFormat(string format, params object[] args)
        {
            _log.DebugFormat(format, args);
        }

        public void InfoFormat(string format, params object[] args)
        {
            _log.InfoFormat(format, args);
        }

        public void WarnFormat(string format, params object[] args)
        {
            _log.WarnFormat(format, args);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            _log.ErrorFormat(format, args);
        }

        public void FatalFormat(string format, params object[] args)
        {
            _log.FatalFormat(format, args);
        }

        public void Exception(Exception exception)
        {
            _log.Fatal(
                String.Format("Exception {0}: {1}; Stack trace as follows...", exception.GetType(), exception.Message),
                exception);
        }
    }
}
