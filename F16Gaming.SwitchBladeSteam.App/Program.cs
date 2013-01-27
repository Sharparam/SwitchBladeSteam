/* Program.cs
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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using F16Gaming.SwitchBladeSteam.Native;
using F16Gaming.SwitchBladeSteam.Razer;
using F16Gaming.SwitchBladeSteam.Razer.Exceptions;
using F16Gaming.SwitchBladeSteam.Steam;
using log4net;
using LogManager = F16Gaming.SwitchBladeSteam.Logging.LogManager;

namespace F16Gaming.SwitchBladeSteam.App
{
	internal class DynamicKeyOptions
	{
		public readonly string Image;
		public readonly VoidDelegate Callback;

		internal DynamicKeyOptions(string image, VoidDelegate callback)
		{
			Image = image;
			Callback = callback;
		}
	}

	public static class Program
	{
		private static ILog _log;

		private static bool _running;
		private static Form _activeForm;
		private static Form _nextForm;
		private static bool _activeFormClosed;

#if DEBUG
		private static Form _keyDebugWindow;
#endif

		private static bool _debugMode;

		private static RazerManager _razerManager;

		private static Dictionary<RazerAPI.RZDYNAMICKEY, DynamicKeyOptions> _dynamicKeyHandlers;

		internal static Client SteamClient;
		internal static FriendsManager SteamFriends;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[MTAThread]
		static void Main(string[] args)
		{
			Helpers.Threading.SetCurrentThreadName("Main");

			LogManager.SetupConsole();

			_log = LogManager.GetLogger(typeof (Program));

			_log.Info("### APPLICATION START ###");

			LogManager.ClearOldLogs();
#if DEBUG
			_debugMode = true;
#else
			if (Debugger.IsAttached)
				_debugMode = true;
			else if (args.Length > 0)
				_debugMode = args[0] == "debug";
#endif

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			_log.Info("Initializing steam objects");
			SteamClient = SteamManager.Client;
			SteamFriends = SteamManager.FriendsManager;

			_dynamicKeyHandlers = new Dictionary<RazerAPI.RZDYNAMICKEY, DynamicKeyOptions>
			{
				{RazerAPI.RZDYNAMICKEY.DK1, new DynamicKeyOptions("res\\images\\dk_home.png", HomeKeyPressed)},
				{RazerAPI.RZDYNAMICKEY.DK2, new DynamicKeyOptions("res\\images\\dk_friends.png", FriendKeyPressed)}
			};
			
#if RAZER_ENABLED
			try
			{
				_log.Info("Initializing razer manager");
				_razerManager = new RazerManager();
			}
			catch (RazerUnstableShutdownException)
			{
				var result = MessageBox.Show("Application was not shut down properly on the last run."
					+ "\n\nIn order for the application to be able to start, it needs to call RzSBStop."
					+ "\n\nIf you have restarted your computer since you last launched this application, you can disregard this message and choose \"No\"."
					+ "\n\nDo you want to call RzSBStop? You will need to restart this application regardless of choice.",
					"Unsafe Application Shutdown", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
				if (result == DialogResult.Yes)
					RazerManager.Stop();

				RazerManager.DeleteControlFile();

				Environment.Exit(0);
			}

			_log.Debug("Razer manager initialized!");

			_log.Info("Enabling dynamic keys");

			foreach (KeyValuePair<RazerAPI.RZDYNAMICKEY, DynamicKeyOptions> pair in _dynamicKeyHandlers)
			{
				_razerManager.EnableDynamicKey(pair.Key, pair.Value.Callback, pair.Value.Image);
			}
#else
			MessageBox.Show(
				"RAZER_ENABLED is not defined, application will not interface with any SwitchBlade capable devices. Running on desktop only.",
				"RAZER_ENABLED undefined", MessageBoxButtons.OK, MessageBoxIcon.Information);
#endif
			ShowForm(new MainWindow());

			_log.Error("Reached end of Main(), unexpected behaviour. Please report to developer.");
		}

		private static void ShowForm(Form form)
		{
			_log.Debug(">> ShowForm([form])");

			_log.Debug("Setting active form");
			_activeForm = form;

#if RAZER_ENABLED
			_log.Debug("Setting handle on switchblade touchpad");
			var touchpad = _razerManager.GetTouchpad();
			if (touchpad == null)
			{
				_razerManager.EnableTouchpad(_activeForm.Handle);
				touchpad = _razerManager.GetTouchpad();
			}
			else
				touchpad.SetHandle(_activeForm.Handle);

			// Add controls, if needed
			var kbForm = _activeForm as IKeyboardEnabledForm;
			if (kbForm != null)
			{
				_log.Debug("Form is keyboard enabled, obtaining control handles to set active keyboard controls");
				var controls = kbForm.GetKeyboardEnabledControls() as List<Control>;
				if (controls == null) // GetKeyboardEnabledControls should never return null, but just in case
				{
					// According to docs, Razer's API will fall back to registering _everything_ as keyboard interactive
					_log.Error("GetKeyboardEnabledControls returned null! Keyboard support will NOT work as expected!");
				}
				else
				{
					var numCtrls = controls.Count;
					_log.DebugFormat("{0} controls will be registered", numCtrls);
					var handles = new IntPtr[numCtrls];
					for (int i = 0; i < numCtrls; i++)
					{
						handles[i] = controls[i].Handle;
					}
					_log.Debug("Calling SetKeyboardEnabledControls on touchpad object");
					touchpad.SetKeyboardEnabledControls(handles);
					_log.Debug("Done! Controls have been registered for keyboard interaction!");
				}
			}
#endif

			_log.Debug("Registering closing event");
			_activeForm.Closing += ActiveFormClosing;
			_log.Debug("Registering closed event");
			_activeForm.Closed += ActiveFormClosed;

			_running = true;

#if DEBUG
			_log.Debug("Creating debug window");
			_keyDebugWindow = new KeyDebugWindow
			{
				StartPosition = FormStartPosition.Manual,
				Location = new Point(Screen.PrimaryScreen.WorkingArea.Width / 2 + _activeForm.Width / 2, Screen.PrimaryScreen.WorkingArea.Height / 2 - _activeForm.Height / 2)
			};
			_keyDebugWindow.Show();
#endif

			_log.Info("Running application with new form");
			_activeFormClosed = false;
			Application.Run(form);

			if (_nextForm != null)
			{
				_log.Info("Loading next form");
				ClearCurrentForm();
				_log.Debug("Setting active form to next form");
				_activeForm = _nextForm;
				_log.Debug("nulling _nextForm var");
				_nextForm = null;
				_log.Debug("Showing next form");
				ShowForm(_activeForm);
				return;
			}

			_log.Info("No next form was set, application will exit.");

			Exit();

			_log.Debug("<< ShowForm() [UNEXPECTED BEHAVIOUR]");
		}

		public static void QueueForm(Form form)
		{
			_log.Debug(">> QueueForm([form])");
			_nextForm = form;
			_log.Debug("<< QueueForm()");
		}

		private static void CloseCurrentForm()
		{
			if (_activeForm.InvokeRequired)
				_activeForm.Invoke((VoidDelegate)(() => _activeForm.Close()));
			else
				_activeForm.Close();
		}

		private static void ClearCurrentForm()
		{
			_log.Debug(">> ClearCurrentForm()");

			if (_activeForm == null || _activeForm.IsDisposed || _activeFormClosed)
				return;

#if RAZER_ENABLED
			_log.Info("Stopping render on touchpad");
			_razerManager.GetTouchpad().StopRender(false);
#endif
			if (_activeForm.Visible)
			{
				_log.Debug("Active form is still visible, closing...");
				_activeForm.Closing -= ActiveFormClosing;
				_activeForm.Closed -= ActiveFormClosed;
				_activeFormClosed = true;
				CloseCurrentForm();
			}

			_log.Debug("<< ClearCurrentForm()");
		}

		private static void ActiveFormClosing(object sender, EventArgs e)
		{
			_log.Debug(">> ActiveFormClosing([sender], [e])");
			_activeForm.Closing -= ActiveFormClosing;
#if RAZER_ENABLED
			_razerManager.GetTouchpad().StopRender(false);
#endif
			_log.Debug("<< ActiveFormClosing()");
		}

		private static void ActiveFormClosed(object sender, EventArgs e)
		{
			_log.Debug(">> ActiveFormClosed([sender], [args])");
			_activeForm.Closed -= ActiveFormClosed;
			ClearCurrentForm();
			_log.Debug("<< ActiveFormClosed()");
		}

		public static void Exit()
		{
			_log.Debug(">> Exit()");

#if RAZER_ENABLED
			if (_running && _activeForm != null)
			{
				_log.Info("Active form is not null, stopping render");
				_razerManager.GetTouchpad().StopRender();
			}

			_log.Info("Stopping Razer interface");
			RazerManager.Stop();

			_log.Info("Deleting control file");
			RazerManager.DeleteControlFile();
#endif

			_running = false;

			_log.Info("### APPLICATION EXIT ###");

			Environment.Exit(0);
		}

		private static void HomeKeyPressed()
		{
			_log.Debug(">> HomeKeyPressed()");
			if (_activeForm is MainWindow)
				return;

			QueueForm(new MainWindow());
			CloseCurrentForm();
			_log.Debug("<< HomeKeyPressed()");
		}

		private static void FriendKeyPressed()
		{
			_log.Debug(">> FriendKeyPressed()");
			if (_activeForm is FriendsWindow)
				return;

			QueueForm(new FriendsWindow());
			CloseCurrentForm();
			_log.Debug("<< FriendKeyPressed()");
		}

#if DEBUG
		public static void DebugHomeButton()
		{
			_log.Debug(">> DebugHomeButton()");
			HomeKeyPressed();
			_log.Debug("<< DebugHomeButton()");
		}

		public static void DebugFriendsButton()
		{
			_log.Debug(">> DebugFriendsButton()");
			FriendKeyPressed();
			_log.Debug("<< DebugFriendsButton()");
		}
#endif
	}
}
