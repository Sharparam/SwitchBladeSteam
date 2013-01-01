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
using System.Windows.Forms;
using F16Gaming.SwitchBladeSteam.Native;

namespace F16Gaming.SwitchBladeSteam.App
{
	public delegate void VoidDelegate();

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
		private static bool _running;
		private static Form _activeForm;
		private static Form _nextForm;

#if DEBUG
		private static Form _keyDebugWindow;
#endif

		private static bool _debugMode;

		private static Dictionary<RazerAPI.RZDYNAMICKEY, DynamicKeyOptions> _dynamicKeyHandlers;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[MTAThread]
		static void Main(string[] args)
		{
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

			_dynamicKeyHandlers = new Dictionary<RazerAPI.RZDYNAMICKEY, DynamicKeyOptions>
			{
				{RazerAPI.RZDYNAMICKEY.DK1, new DynamicKeyOptions("res\\images\\dk_home.png", HomeKeyPressed)},
				{RazerAPI.RZDYNAMICKEY.DK2, new DynamicKeyOptions("res\\images\\dk_friends.png", FriendKeyPressed)}
			};

#if RAZER_ENABLED
			var hResult = RazerAPI.RzSBStart();
			if (hResult != HRESULT.RZSB_OK)
			{
				MessageBox.Show("RzSBStart failed with error code: " + hResult.ToString(), "RzSBStart fail", MessageBoxButtons.OK,
								MessageBoxIcon.Error);
				Exit();
			}

			hResult = RazerAPI.RzSBWinRenderSetDisabledImage(@"res\images\tp_aero.png");
			if (hResult != HRESULT.RZSB_OK)
			{
				MessageBox.Show("RzSBWinRenderSetDisbledImage failed with error code: " + hResult.ToString(),
								"RzSBWinRenderSetDisabledImage fail", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Exit();
			}

			hResult = RazerAPI.RzSBDynamicKeySetCallback(DynamicKeyCallback);
			if (hResult != HRESULT.RZSB_OK)
			{
				MessageBox.Show("RzSBDynamicKeySetCallback failed with error code: " + hResult.ToString(),
				                "RzSBDynamicKeySetCallback fail", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Exit();
			}

			/* TODO: Fix problem with RzSBSetImageDynamicKey (error on Razer's side?)
			 * 
			 * If marshalled to UnmanagedType.LPStr:
			 *     Error: RZSB_IMAGE_INVALID_DATA
			 * If marshalled to UnmanagedType.LPWStr:
			 *     Error: E_FAIL (No specifics)
			 */
			foreach (KeyValuePair<RazerAPI.RZDYNAMICKEY, DynamicKeyOptions> pair in _dynamicKeyHandlers)
			{
				hResult = RazerAPI.RzSBSetImageDynamicKey(pair.Key, RazerAPI.RZDKSTATE.UP, pair.Value.Image);
				if (hResult != HRESULT.RZSB_OK)
				{
					MessageBox.Show(
						"RzSBSetImageDynamicKey(" + pair.Key + ", " + RazerAPI.RZDKSTATE.UP + ", " + pair.Value.Image +
						") failed with error code: " + hResult.ToString() + "\n\nDetail:\n" + Helpers.GetErrorMessage(hResult), "RzSBSetImageDynamicKey fail", MessageBoxButtons.OK,
						MessageBoxIcon.Error);
					Exit();
				}

				hResult = RazerAPI.RzSBSetImageDynamicKey(pair.Key, RazerAPI.RZDKSTATE.DOWN, pair.Value.Image);
				if (hResult != HRESULT.RZSB_OK)
				{
					MessageBox.Show(
						"RzSBSetImageDynamicKey(" + pair.Key + ", " + RazerAPI.RZDKSTATE.DOWN + ", " + pair.Value.Image +
						") failed with error code: " + hResult.ToString(), "RzSBSetImageDynamicKey fail", MessageBoxButtons.OK,
						MessageBoxIcon.Error);
					Exit();
				}
			}

			if (RazerAPI.RzSBGestureEnable(RazerAPI.RZGESTURE.PRESS, true) != HRESULT.RZSB_OK)
			{
				MessageBox.Show("RzSBGestureEnable failed", "RzSBGestureEnabled fail", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Exit();
			}
			
			if (RazerAPI.RzSBGestureEnable(RazerAPI.RZGESTURE.TAP, true) != HRESULT.RZSB_OK)
			{
				MessageBox.Show("RzSBGestureEnable failed", "RzSBGestureEnabled fail", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Exit();
			}

			if (RazerAPI.RzSBGestureSetNotification(RazerAPI.RZGESTURE.PRESS, true) != HRESULT.RZSB_OK)
			{
				MessageBox.Show("RzSBGestureSetNotification failed", "RzSBGestureSetNotification fail", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Exit();
			}
			
			if (RazerAPI.RzSBGestureSetNotification(RazerAPI.RZGESTURE.TAP, true) != HRESULT.RZSB_OK)
			{
				MessageBox.Show("RzSBGestureSetNotification failed", "RzSBGestureSetNotification fail", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Exit();
			}

			if (RazerAPI.RzSBGestureSetOSNotification(RazerAPI.RZGESTURE.PRESS, true) != HRESULT.RZSB_OK)
			{
				MessageBox.Show("RzSBGestureSetOSNotification failed", "RzSBGestureSetOSNotification fail", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Exit();
			}
			
			if (RazerAPI.RzSBGestureSetOSNotification(RazerAPI.RZGESTURE.TAP, true) != HRESULT.RZSB_OK)
			{
				MessageBox.Show("RzSBGestureSetOSNotification failed", "RzSBGestureSetOSNotification fail", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Exit();
			}
#else
			MessageBox.Show(
				"RAZER_ENABLED is not defined, application will not interface with any SwitchBlade capable devices. Running on desktop only.",
				"RAZER_ENABLED undefined", MessageBoxButtons.OK, MessageBoxIcon.Information);
#endif
			ShowForm(new MainWindow());
		}

		private static void ShowForm(Form form)
		{
			_activeForm = form;

#if RAZER_ENABLED
			var hResult = RazerAPI.RzSBWinRenderStart(_activeForm.Handle, true, _debugMode);
			if (hResult != HRESULT.RZSB_OK)
			{
				MessageBox.Show("RzSBWinRenderStart failed with error code: " + hResult.ToString(), "RzSBWinRenderStart fail",
								MessageBoxButtons.OK, MessageBoxIcon.Error);
				Exit();
			}
#endif

			_activeForm.Closed += (o, e) => ActiveFormClosed();

			_running = true;

#if DEBUG
			_keyDebugWindow = new KeyDebugWindow();
			_keyDebugWindow.Show();
#endif

			Application.Run(form);

			if (_nextForm != null)
			{
				ClearCurrentForm();
				_activeForm = _nextForm;
				_nextForm = null;
				ShowForm(_activeForm);
				return;
			}

			Exit();
		}

		public static void QueueForm(Form form)
		{
			_nextForm = form;
		}

		private static void ClearCurrentForm()
		{
			if (_activeForm == null || _activeForm.IsDisposed)
				return;

#if RAZER_ENABLED
			var hResult = RazerAPI.RzSBWinRenderStop(true);
			if (hResult != HRESULT.RZSB_OK)
				MessageBox.Show("RzSBWinRenderStop failed with error code: " + hResult.ToString(), "RzSBWinRenderStop fail",
								MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
			_activeForm.Dispose();
			_activeForm = null;
		}

		private static void ActiveFormClosed()
		{
			ClearCurrentForm();
		}

		public static void Exit()
		{
#if RAZER_ENABLED
			if (_running && _activeForm != null)
			{
				var hResult = RazerAPI.RzSBWinRenderStop(true);
				if (hResult != HRESULT.RZSB_OK)
					MessageBox.Show("RzSBWinRenderStop failed with error code: " + hResult.ToString(), "RzSBWinRenderStop fail",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			RazerAPI.RzSBStop();
#endif

			_running = false;

			Environment.Exit(0);
		}

		private static HRESULT DynamicKeyCallback(RazerAPI.RZDYNAMICKEY dynamicKey, RazerAPI.RZDKSTATE keyState)
		{
			var result = HRESULT.RZSB_OK;

			if (keyState != RazerAPI.RZDKSTATE.DOWN || !_dynamicKeyHandlers.ContainsKey(dynamicKey))
				return result;

			_dynamicKeyHandlers[dynamicKey].Callback();

			return result;
		}

		private static void HomeKeyPressed()
		{
			if (_activeForm is MainWindow)
				return;
			
			QueueForm(new MainWindow());
			ClearCurrentForm();
		}

		private static void FriendKeyPressed()
		{
			if (_activeForm is FriendsWindow)
				return;

			QueueForm(new FriendsWindow());
			ClearCurrentForm();
		}

#if DEBUG
		public static void DebugHomeButton()
		{
			HomeKeyPressed();
		}

		public static void DebugFriendsButton()
		{
			FriendKeyPressed();
		}
#endif
	}
}
