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

#define STEAM

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using F16Gaming.SwitchBladeSteam.Native;
using F16Gaming.SwitchBladeSteam.Razer;
using F16Gaming.SwitchBladeSteam.Razer.Events;
using F16Gaming.SwitchBladeSteam.Razer.Exceptions;
using F16Gaming.SwitchBladeSteam.Razer.Structs;
using F16Gaming.SwitchBladeSteam.Steam;
using Steam4NET;
using log4net;
using LogManager = F16Gaming.SwitchBladeSteam.Logging.LogManager;

namespace F16Gaming.SwitchBladeSteam.App
{
	internal class DynamicKeyOptions
	{
		public readonly string Image;
		public readonly DynamicKeyPressedEventHandler Callback;

		internal DynamicKeyOptions(string image, DynamicKeyPressedEventHandler callback)
		{
			Image = image;
			Callback = callback;
		}
	}

	public static class Program
	{
		private static ILog _log;

		private static Form _activeForm;
		private static Form _nextForm;
		private static bool _activeFormClosed;

#if DEBUG
		private static Form _keyDebugWindow;
#endif

#if RAZER_ENABLED
		private static RazerManager _razerManager;

		private static Dictionary<RazerAPI.RZDYNAMICKEY, DynamicKeyOptions> _dynamicKeyHandlers;
#endif

		internal static bool DebugMode { get; private set; }

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
			DebugMode = true;
#else
			if (Debugger.IsAttached)
				DebugMode = true;
			else if (args.Length > 0)
				DebugMode = args[0] == "debug";
#endif

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

#if STEAM
			_log.Info("Initializing steam objects");
			SteamClient = SteamManager.Client;
			SteamFriends = SteamManager.FriendsManager;
#else
			MessageBox.Show("Steam is not enabled in this build, steam features will be disabled.", "Steam Disabled",
			                MessageBoxButtons.OK, MessageBoxIcon.Information);
#endif

#if RAZER_ENABLED
			_dynamicKeyHandlers = new Dictionary<RazerAPI.RZDYNAMICKEY, DynamicKeyOptions>
			{
				{RazerAPI.RZDYNAMICKEY.DK1, new DynamicKeyOptions(@"res\images\dk_home.png", HomeKeyPressed)},
				{RazerAPI.RZDYNAMICKEY.DK2, new DynamicKeyOptions(@"res\images\dk_friends.png", FriendKeyPressed)},
				{RazerAPI.RZDYNAMICKEY.DK6, new DynamicKeyOptions(@"res\images\dk_appear_online.png", OnlineKeyPressed)},
				{RazerAPI.RZDYNAMICKEY.DK7, new DynamicKeyOptions(@"res\images\dk_appear_offline.png", OfflineKeyPressed)}
			};

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

			_log.Info("Registering app event callback");
			_razerManager.AppEvent += OnAppEvent;
			_log.Debug("App event callback registered!");
			
			_log.Info("Enabling dynamic keys");
			
			foreach (KeyValuePair<RazerAPI.RZDYNAMICKEY, DynamicKeyOptions> pair in _dynamicKeyHandlers)
				_razerManager.EnableDynamicKey(pair.Key, pair.Value.Callback, pair.Value.Image);
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

#if RAZER_ENABLED
			_log.Debug("Setting handle on switchblade touchpad");
			var touchpad = _razerManager.GetTouchpad();
			try
			{
				touchpad.SetHandle(form.Handle);
			}
			catch (ObjectDisposedException ex)
			{
				_log.ErrorFormat("ShowForm: touchpad.SetHandle failed [ObjectDisposedException]: {0}", ex.Message);
				_log.Error("Exception detail:", ex);
				_log.Warn("Attempting to recover by showing main window");
				ShowForm(new MainWindow("Failed to complete the requested action."));
				return;
			}

			_log.Debug("Nulling _nextForm");
			_nextForm = null;

			// Add controls, if needed
			var kbForm = form as IKeyboardEnabledForm;
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
					var kbControls = new KeyboardControl[numCtrls];
					for (int i = 0; i < numCtrls; i++)
					{
						kbControls[i] = new KeyboardControl(controls[i].Handle);
					}
					_log.Debug("Calling SetKeyboardEnabledControls on touchpad object");
					touchpad.SetKeyboardEnabledControls(kbControls);
					_log.Debug("Done! Controls have been registered for keyboard interaction!");
				}
			}

			// Forward gestures, if needed
			var gestureForm = form as IGestureEnabledForm;
			if (gestureForm == null)
			{
				_log.Debug("Form is not gesture enabled, resetting gestures");
				touchpad.DisableOSGesture(RazerAPI.RZGESTURE.ALL);
			}
			else
			{
				_log.Debug("Form is gesture enabled, setting gestures to be forwarded");
				touchpad.EnableOSGesture(gestureForm.EnabledGestures);
			}
#endif

			_log.Debug("Registering closing event");
			form.Closing += ActiveFormClosing;
			_log.Debug("Registering closed event");
			form.Closed += ActiveFormClosed;

#if DEBUG
			_log.Debug("Creating debug window");
			_keyDebugWindow = new KeyDebugWindow
			{
				StartPosition = FormStartPosition.Manual,
				Location = new Point(Screen.PrimaryScreen.WorkingArea.Width / 2 + form.Width / 2, Screen.PrimaryScreen.WorkingArea.Height / 2 - form.Height / 2)
			};
			_keyDebugWindow.Show();
#endif

			_log.Debug("Setting active form");
			_activeForm = form;
			_log.Info("Running application with new form");
			_activeFormClosed = false;
			Application.Run(_activeForm);

			if (_nextForm != null)
			{
				_log.Info("Loading next form");
				ClearCurrentForm();
				_log.Debug("Showing next form");
				ShowForm(_nextForm);
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
			if (_activeForm != null)
			{
				_log.Info("Active form is not null, stopping render");
				var touchpad = _razerManager.GetTouchpad();
				if (touchpad != null)
					touchpad.StopRender();
			}

			_log.Info("Stopping Razer interface");
			RazerManager.Stop();
#endif

			if (SteamClient != null)
			{
				_log.Info("Disposing steam client");
				SteamClient.Dispose();
			}

			_log.Info("### APPLICATION EXIT ###");

			Environment.Exit(0);
		}

		private static void OnAppEvent(object sender, AppEventEventArgs e)
		{
			if (e.Type != RazerAPI.RZSDKAPPEVENTTYPE.APPMODE || e.Mode != RazerAPI.RZSDKAPPEVENTMODE.APPLET ||
			    e.ProcessID == Process.GetCurrentProcess().Id)
				return;

			_log.Info("Process ownership changed, shutting down.");
			Exit();
		}

		private static void ShowHome()
		{
			if (_activeForm is MainWindow)
				return;

			QueueForm(new MainWindow());
			CloseCurrentForm();
		}

		private static void ShowFriends()
		{
			if (_activeForm is FriendsWindow)
				return;

			QueueForm(new FriendsWindow());
			CloseCurrentForm();
		}

		private static void GoOnline()
		{
			SteamClient.SetMyState(EPersonaState.k_EPersonaStateOnline);
		}

		private static void GoOffline()
		{
			SteamClient.SetMyState(EPersonaState.k_EPersonaStateOffline);
		}

		private static void HomeKeyPressed(object sender, EventArgs e)
		{
			_log.Debug(">> HomeKeyPressed()");
			ShowHome();
			_log.Debug("<< HomeKeyPressed()");
		}

		private static void FriendKeyPressed(object sender, EventArgs e)
		{
			_log.Debug(">> FriendKeyPressed()");
			ShowFriends();
			_log.Debug("<< FriendKeyPressed()");
		}

		private static void OnlineKeyPressed(object sender, EventArgs e)
		{
			_log.Debug(">> OnlineKeyPressed()");
			GoOnline();
			_log.Debug("<< OnlineKeyPressed()");
		}

		private static void OfflineKeyPressed(object sender, EventArgs e)
		{
			_log.Debug(">> OfflineKeyPressed()");
			GoOffline();
			_log.Debug("<< OfflineKeyPressed()");
		}

#if DEBUG
		public static void DebugHomeButton()
		{
			_log.Debug(">> DebugHomeButton()");
			ShowHome();
			_log.Debug("<< DebugHomeButton()");
		}

		public static void DebugFriendsButton()
		{
			_log.Debug(">> DebugFriendsButton()");
			ShowFriends();
			_log.Debug("<< DebugFriendsButton()");
		}

		public static void DebugOnlineButton()
		{
			_log.Debug(">> DebugOnlineButton()");
			GoOnline();
			_log.Debug("<< DebugOnlineButton()");
		}

		public static void DebugOfflineButton()
		{
			_log.Debug(">> DebugOfflineButton()");
			GoOffline();
			_log.Debug("<< DebugOfflineButton()");
		}

		public static void ShowRenderStats()
		{
#if RAZER_ENABLED
			var stats = _razerManager.GetTouchpad().GetRenderStats();
			var statsText = string.Format("Count: {0}\nMax Time: {1}\nLast Time: {2}\nAverage Time: {3}",
										  stats.Count, stats.MaxTime, stats.LastTime, stats.AverageTime);
			MessageBox.Show(statsText, "Render Stats", MessageBoxButtons.OK, MessageBoxIcon.Information);
#endif
		}
#endif
	}
}
