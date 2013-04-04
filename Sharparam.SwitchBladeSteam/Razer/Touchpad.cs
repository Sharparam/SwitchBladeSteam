/* Touchpad.cs
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
using System.Linq;
using F16Gaming.SwitchBladeSteam.Extensions;
using F16Gaming.SwitchBladeSteam.Native;
using F16Gaming.SwitchBladeSteam.Razer.Events;
using F16Gaming.SwitchBladeSteam.Razer.Structs;
using log4net;

namespace F16Gaming.SwitchBladeSteam.Razer
{
	public class Touchpad : ITouchpad
	{
		public event GestureEventHandler Gesture;

		private readonly ILog _log;

		private RazerAPI.RZGESTURE _activeGestures;
		private RazerAPI.RZGESTURE _activeOSGestures;

		private bool _allGestureEnabled;
		private bool _allOSGestureEnabled;

		private static RazerAPI.TouchpadGestureCallbackFunctionDelegate _gestureCallback;

		/// <summary>
		/// Image that will show if Aero is disabled.
		/// </summary>
		public string DisabledImage { get; private set; }

		/// <summary>
		/// Will be set to IntPtr.Zero if no handle associated.
		/// </summary>
		public IntPtr CurrentHandle { get; private set; }

		/// <summary>
		/// Current image shown on the Touchpad, null if not using static image.
		/// </summary>
		public string CurrentImage { get; private set; }

		internal Touchpad(string disabledImage = Constants.DisabledTouchpadImage)
		{
			_log = Logging.LogManager.GetLogger(this);
			_log.Debug(">> Touchpad()");
			_log.Info("Setting disabled image");
			DisabledImage = Helpers.IO.GetAbsolutePath(disabledImage);
			var hResult = RazerAPI.RzSBWinRenderSetDisabledImage(DisabledImage);
			if (HRESULT.RZSB_FAILED(hResult))
				RazerManager.NativeCallFailure("RzSBWinRenderSetDisabledImage", hResult);
			_log.Debug("Setting gesture callback");
			_gestureCallback = HandleTouchpadGesture;
			hResult = RazerAPI.RzSBGestureSetCallback(_gestureCallback);
			if (HRESULT.RZSB_FAILED(hResult))
				RazerManager.NativeCallFailure("RzSBGestureSetCallback", hResult);
			_log.Debug("<< Touchpad()");
		}
		
		private void OnGesture(RazerAPI.RZGESTURE gesture, uint parameter, ushort x, ushort y, ushort z)
		{
			var func = Gesture;
			if (func != null)
				func(this, new GestureEventArgs(gesture, parameter, x, y, z));
		}

		public void SetHandle(IntPtr handle, bool translateGestures = true)
		{
			_log.Debug(">> SetHandle([handle])");
			StopRender(false);

			var hResult = RazerAPI.RzSBWinRenderStart(handle, translateGestures, Constants.DebugEnabled);
			if (hResult == HRESULT.RZSB_ALREADY_STARTED)
			{
				_log.Warn("WinRender already started, attempting to force render stop and try again...");
				StopRender(false, true);
				_log.Info("Attempting to start WinRender again...");
				hResult = RazerAPI.RzSBWinRenderStart(handle, translateGestures, Constants.DebugEnabled);
			}

			if (HRESULT.RZSB_FAILED(hResult))
				RazerManager.NativeCallFailure("RzSBWinRenderStart", hResult);

			CurrentHandle = handle;

			_log.Debug("<< SetHandle()");
		}

		public void SetKeyboardEnabledControls(IEnumerable<KeyboardControl> keyboardControls, bool resetList = true)
		{
			_log.Debug(">> SetKeyboardEnabledControls([handles])");

			_log.Debug("Getting keyboard controls array");
			var kbControlArray = keyboardControls as KeyboardControl[] ?? keyboardControls.ToArray();

			_log.Debug("Creating RZSB_KEYEVTCTRLS array");
			var controls = new RazerAPI.RZSB_KEYEVTCTRLS[kbControlArray.Length];

			_log.Debug("Populating controls array");
			for (var i = 0; i < kbControlArray.Length; i++)
				controls[i] = new RazerAPI.RZSB_KEYEVTCTRLS {hwndTarget = kbControlArray[i].Handle, bReleaseOnEnter = kbControlArray[i].ReleaseOnEnter};

			_log.DebugFormat("Calling RzSBWinRenderAddKeyInputCtrls with {0} controls", controls.Length);
			var hResult = RazerAPI.RzSBWinRenderAddKeyInputCtrls(controls, controls.Length, resetList);
			if (HRESULT.RZSB_FAILED(hResult))
				RazerManager.NativeCallFailure("RzSBWinRenderAddKeyInputCtrls", hResult);

			_log.Debug("<< SetKeyboardEnabledControls()");
		}

		public void SetImage(string image)
		{
			_log.DebugFormat(">> SetImage({0})", image);
			StopRender();

			image = Helpers.IO.GetAbsolutePath(image);

			var hResult = RazerAPI.RzSBSetImageTouchpad(image);
			if (HRESULT.RZSB_FAILED(hResult))
				RazerManager.NativeCallFailure("RzSBSetImageTouchpad", hResult);

			CurrentImage = image;

			_log.Debug("<< SetImage()");
		}

		public void StopRender(bool erase = true, bool force = false)
		{
			_log.DebugFormat(">> StopRender({0})", erase ? "true" : "false");
			if (CurrentHandle == IntPtr.Zero && !force)
			{
				_log.Debug("CurrentHandle is null and force was not specified, aborting.");
				_log.Debug("<< StopRender()");
				return;
			}
			
			var hResult = RazerAPI.RzSBWinRenderStop(erase);
			if (HRESULT.RZSB_FAILED(hResult))
				RazerManager.NativeCallFailure("RzSBWinRenderStop", hResult);

			CurrentHandle = IntPtr.Zero;

			_log.Debug("<< StopRender()");
		}

		public RenderStats GetRenderStats()
		{
			uint count;
			uint maxTime;
			uint lastTime;
			uint averageTime;
			var hResult = RazerAPI.RzSBWinRenderGetStats(out count, out maxTime, out lastTime, out averageTime);
			if (HRESULT.RZSB_FAILED(hResult))
				RazerManager.NativeCallFailure("RzSBWinRenderGetStats", hResult);
			var stats = new RenderStats(count, maxTime, lastTime, averageTime);
			return stats;
		}

		public void ResetRenderStats()
		{
			var hResult = RazerAPI.RzSBWinRenderResetStats();
			if (HRESULT.RZSB_FAILED(hResult))
				RazerManager.NativeCallFailure("RzSBWinRenderResetStats", hResult);
		}

		public void ClearImage()
		{
			_log.Debug(">> ClearImage()");

			var hResult = RazerAPI.RzSBSetImageTouchpad(Helpers.IO.GetAbsolutePath(Constants.BlankTouchpadImage));
			if (HRESULT.RZSB_FAILED(hResult))
				RazerManager.NativeCallFailure("RzSBSetImageTouchpad", hResult);

			CurrentImage = null;

			_log.Debug("<< ClearImage()");
		}

		public void StopAll()
		{
			StopRender();
			ClearImage();
		}

		public void SetGesture(RazerAPI.RZGESTURE gesture, bool enabled)
		{
			_log.DebugFormat(">> SetGesture({0}, {1})", gesture, enabled ? "true" : "false");
			RazerAPI.RZGESTURE newGestures;
			if (gesture == RazerAPI.RZGESTURE.ALL)
			{
				newGestures = gesture;
				enabled = !enabled;
			}
			else if (gesture == RazerAPI.RZGESTURE.NONE)
			{
				if (_activeGestures == RazerAPI.RZGESTURE.NONE)
				{
					_log.Debug("Active gestures already set to none, aborting.");
					_log.Debug("<< SetGesture()");
					return;
				}
				if (!enabled)
				{
					// Request to "disable no gesture"?
					// Then just enable all, since that's the same
					_log.Debug("Requested to set none disabled, calling set all enabled instead");
					SetGesture(RazerAPI.RZGESTURE.ALL, true);
					_log.Debug("<< SetGesture()");
					return;
				}
				newGestures = gesture;
			}
			else if (enabled)
			{
				if (_activeGestures.Has(gesture) && !(_activeGestures == RazerAPI.RZGESTURE.ALL && !_allGestureEnabled))
				{
					_log.Debug("Active gestures already have requested value");
					_log.DebugFormat("_activeGestures == {0}", _activeGestures);
					_log.DebugFormat("_allGestureEnabled == {0}", _allGestureEnabled);
					return;
				}
				newGestures = _activeOSGestures.Include(gesture);
			}
			else
			{
				if (!_activeGestures.Has(gesture))
				{
					_log.DebugFormat("Request to disable gesture already disabled: {0}", gesture);
					_log.DebugFormat("_activeGestures == {0}", _activeGestures);
					return;
				}
				newGestures = _activeOSGestures.Remove(gesture);
			}

			var hResult = RazerAPI.RzSBGestureEnable(newGestures, enabled);
			if (HRESULT.RZSB_FAILED(hResult))
				RazerManager.NativeCallFailure("RzSBGestureEnable", hResult);

			hResult = RazerAPI.RzSBGestureSetNotification(newGestures, enabled);
			if (HRESULT.RZSB_FAILED(hResult))
				RazerManager.NativeCallFailure("RzSBGestureSetNotification", hResult);

			hResult = RazerAPI.RzSBGestureSetCallback(_gestureCallback);
			if (HRESULT.RZSB_FAILED(hResult))
				RazerManager.NativeCallFailure("RzSBGestureSetCallback", hResult);

			_activeGestures = newGestures;
			_allGestureEnabled = _activeGestures == RazerAPI.RZGESTURE.ALL && !enabled;

			_log.Debug("<< SetGesture()");
		}

		public void EnableGesture(RazerAPI.RZGESTURE gesture)
		{
			_log.DebugFormat(">> EnableGesture({0})", gesture);
			SetGesture(gesture, true);
			_log.Debug("<< EnableGesture()");
		}

		public void DisableGesture(RazerAPI.RZGESTURE gesture)
		{
			_log.DebugFormat(">> DisableGesture({0})", gesture);
			SetGesture(gesture, false);
			_log.Debug("<< DisableGesture()");
		}

		public void SetOSGesture(RazerAPI.RZGESTURE gesture, bool enabled)
		{
			_log.DebugFormat(">> SetOSGesture({0}, {1})", gesture, enabled ? "true" : "false");

			RazerAPI.RZGESTURE newGestures;
			if (gesture == RazerAPI.RZGESTURE.ALL)
			{
				// Invert the enabled value because of how Razer API works
				enabled = !enabled;
				// "ALL" replaces any other gesture, so we don't want to include/remove it
				newGestures = gesture;
			}
			else if (gesture == RazerAPI.RZGESTURE.NONE)
			{
				if (_activeOSGestures == RazerAPI.RZGESTURE.NONE)
					return;
				if (!enabled) 
				{
					// Request to "disable no gesture"?
					// Then just enable all, since that's the same
					SetOSGesture(RazerAPI.RZGESTURE.ALL, true);
					return;
				}
				// "NONE" replaces any other gesture, so we don't want to include/remove it
				newGestures = gesture;
			}
			else if (enabled)
			{
				if (_activeOSGestures.Has(gesture) || !(_activeOSGestures == RazerAPI.RZGESTURE.ALL && !_allOSGestureEnabled))
					return;
				newGestures = _activeOSGestures.Include(gesture);
			}
			else
			{
				if (!_activeOSGestures.Has(gesture))
					return;
				newGestures = _activeOSGestures.Remove(gesture);
			}

			var hResult = RazerAPI.RzSBGestureEnable(newGestures, enabled);
			if (HRESULT.RZSB_FAILED(hResult))
				RazerManager.NativeCallFailure("RzSBGestureEnable", hResult);

			hResult = RazerAPI.RzSBGestureSetOSNotification(newGestures, enabled);
			if (HRESULT.RZSB_FAILED(hResult))
				RazerManager.NativeCallFailure("RzSBGestureSetOSNotification", hResult);

			_activeOSGestures = newGestures;
			_allOSGestureEnabled = _activeGestures == RazerAPI.RZGESTURE.ALL && !enabled;
			_log.Debug("<< SetOSGesture()");
		}

		public void EnableOSGesture(RazerAPI.RZGESTURE gesture)
		{
			_log.DebugFormat(">> EnableOSGesture({0})", gesture);
			SetOSGesture(gesture, true);
			_log.Debug("<< EnableOSGesture()");
		}

		public void DisableOSGesture(RazerAPI.RZGESTURE gesture)
		{
			_log.DebugFormat(">> DisableOSGesture({0})", gesture);
			SetOSGesture(gesture, false);
			_log.Debug("<< DisableOSGesture()");
		}

		private HRESULT HandleTouchpadGesture(RazerAPI.RZGESTURE gesture, uint dwParameters, ushort wXPos, ushort wYPos, ushort wZPos)
		{
			// Do not log gesture events, it gets VERY SPAMMY

			//_log.DebugFormat(">> HandleTouchpadGesture({0}, {1}, {2}, {3}, {4})", gesture, dwParameters, wXPos, wYPos, wZPos);
			var result = HRESULT.RZSB_OK;
			OnGesture(gesture, dwParameters, wXPos, wYPos, wZPos);
			//_log.Debug("<< HandleTouchpadGesture()");
			return result;
		}
	}
}
