/* RazerManager.cs
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
using F16Gaming.SwitchBladeSteam.Native;
using F16Gaming.SwitchBladeSteam.Razer.Exceptions;
using log4net;

using LogManager = F16Gaming.SwitchBladeSteam.Logging.LogManager;

namespace F16Gaming.SwitchBladeSteam.Razer
{
	public delegate void VoidDelegate();

	public class RazerManager
	{
		private const string RazerControlFile = "DO_NOT_TOUCH__RAZER_CONTROL_FILE";

		private ILog _log;

		private Touchpad _touchpad;
		private readonly DynamicKey[] _dynamicKeys;
		private readonly VoidDelegate[] _dkCallbacks;

		// Callback for handling dynamic keys
		private static RazerAPI.DynamicKeyCallbackFunctionDelegate _dkCallback;

		public RazerManager(string disabledImage = Constants.DisabledTouchpadImage)
		{
			_log = LogManager.GetLogger(this);

			_log.Debug(">> RazerManager()");

			_log.Info("RazerManager is initializing");

			if (File.Exists(RazerControlFile))
			{
				_log.Error("Detected control file presence, throwing exception.");
				_log.Debug("<< RazerManager()");
				throw new RazerUnstableShutdownException();
			}

			File.Create(RazerControlFile);

			_log.Debug("Calling RzSBStart()");

			var hResult = RazerAPI.RzSBStart();
			if (!HRESULT.RZSB_SUCCESS(hResult))
				NativeCallFailure("RzSBStart", hResult);

			_log.Debug("Calling RzSBWinRenderSetDisabledImage");
			
			hResult = RazerAPI.RzSBWinRenderSetDisabledImage(Helpers.IO.GetAbsolutePath(disabledImage));
			if (!HRESULT.RZSB_SUCCESS(hResult))
				NativeCallFailure("RzSBWinRenderSetDisabledImage", hResult);

			_log.Info("Setting up dynamic keys");

			_log.Debug("Creating dynamic key callback");
			_dkCallback = new RazerAPI.DynamicKeyCallbackFunctionDelegate(HandleDynamicKeyEvent);
			_log.Debug("Calling RzSBDynamicKeySetCallback");
			hResult = RazerAPI.RzSBDynamicKeySetCallback(_dkCallback);
			if (!HRESULT.RZSB_SUCCESS(hResult))
				NativeCallFailure("RzSBDynamicKeySetCallback", hResult);

			_log.Debug("Initializing dynamic key arrays");

			_dynamicKeys = new DynamicKey[RazerAPI.DYNAMIC_KEYS_COUNT];
			_dkCallbacks = new VoidDelegate[RazerAPI.DYNAMIC_KEYS_COUNT];

			_log.Debug("<< RazerManager()");
		}

		public static void DeleteControlFile()
		{
			if (File.Exists(RazerControlFile))
				File.Delete(RazerControlFile);
		}

		public static void Stop()
		{
			var hResult = RazerAPI.RzSBStop();
			if (!HRESULT.RZSB_SUCCESS(hResult))
				throw new RazerNativeException(hResult);
		}

		private void NativeCallFailure(string func, HRESULT result)
		{
			_log.FatalFormat("Call to RazerAPI native function {0} FAILED with error: {1}", func, result.ToString());
			_log.Debug("Throwing exception...");
			throw new RazerNativeException(result);
		}

		public Touchpad GetTouchpad()
		{
			return _touchpad;
		}

		public DynamicKey GetDynamicKey(RazerAPI.RZDYNAMICKEY key)
		{
			return _dynamicKeys[(int) key - 1];
		}

		public bool EnableTouchpad(IntPtr windowHandle)
		{
			_log.Debug(">> EnableTouchpad([handle])");

			DisableTouchpad();

			_log.Debug("Initializing new Touchpad object");
			_touchpad = new Touchpad(windowHandle);

			_log.Debug("<< EnableTouchpad()");
			return true;
		}

		public bool EnableTouchpad(string image)
		{
			_log.DebugFormat(">> EnableTouchpad({0})", image);

			DisableTouchpad();

			_log.Debug("Initializing new Touchpad object");
			_touchpad = new Touchpad(image);

			_log.Debug("<< EnableTouchpad()");
			return true;
		}

		public void DisableTouchpad()
		{
			_log.Debug(">> DisableTouchpad()");
			if (_touchpad != null)
			{
				_touchpad.StopRender();
				_touchpad.ClearImage();
			}
			_log.Debug("<< DisableTouchpad()");
		}

		public bool EnableDynamicKey(RazerAPI.RZDYNAMICKEY key, VoidDelegate callback, string upImage, string downImage = null, bool replace = false)
		{
			_log.DebugFormat(">> EnableDynamicKey({0}, [callback], \"{1}\", {2}, {3})", key, upImage,
			                 downImage == null ? "null" : "\"" + downImage + "\"", replace ? "true" : "false");

			bool result = true;

			var index = (int) key - 1;
			if (_dynamicKeys[index] != null && !replace)
			{
				_log.Info("Dynamic key already enabled and replace is false.");
				_log.Debug("<< EnableDynamicKey()");
				return false;
			}

			_log.Debug("Resetting dynamic key (DisableDynamicKey");
			DisableDynamicKey(key);
			try
			{
				_log.Debug("Creating new DynamicKey object");
				var dk = new DynamicKey(key, upImage, downImage);
				_dynamicKeys[index] = dk;
				_log.Debug("Registering dynamic key callback");
				RegisterDynamicKeyCallback(key, callback);
			}
			catch (RazerNativeException ex)
			{
				_log.ErrorFormat("Failed to enable dynamic key {0}: {1}", key, ex.Hresult.ToString());
				_log.Debug("<< EnableDynamicKey()");
				result = false;
			}

			_log.Debug("<< EnableDynamicKey()");
			return result;
		}

		public void DisableDynamicKey(RazerAPI.RZDYNAMICKEY key)
		{
			_log.DebugFormat(">> DisableDynamicKey({0})", key);
			_log.Debug("Unregistering dynamic key callback");
			UnregisterDynamicKeyCallback(key);
			_dynamicKeys[(int) key - 1] = null;
			_log.Debug("<< DisableDynamicKey()");
		}

		private void RegisterDynamicKeyCallback(RazerAPI.RZDYNAMICKEY key, VoidDelegate callback)
		{
			_log.DebugFormat(">> RegisterDynamicKeyCallback({0}, [callback])", key);
			_dkCallbacks[(int) key - 1] = callback;
			_log.Debug("<< RegisterDynamicKeyCallback()");
		}

		private void UnregisterDynamicKeyCallback(RazerAPI.RZDYNAMICKEY key)
		{
			_log.DebugFormat(">> UnregisterDynamicKeyCallback({0})", key);
			_dkCallbacks[(int) key - 1] = null;
			_log.Debug("<< UnregisterDynamicKeyCallback()");
		}

		private HRESULT HandleDynamicKeyEvent(RazerAPI.RZDYNAMICKEY key, RazerAPI.RZDKSTATE state)
		{
			_log.DebugFormat(">> HandleDynamicKeyEvent({0}, {1})", key, state);

			var result = HRESULT.RZSB_OK;

			var index = (int) key - 1;
			var dk = _dynamicKeys[index];
			if (dk == null)
			{
				_log.Debug("No callback registered for key");
				_log.Debug("<< HandleDynamicKeyEvent()");
				return result;
			}

			_log.Debug("Updating previous key state");
			dk.UpdatePreviousState(dk.State);
			_log.Debug("Updating current key state");
			dk.UpdateState(state);

			// Only proceed if this is the first down event on the key
			if (dk.PreviousState == RazerAPI.RZDKSTATE.DOWN || dk.State != RazerAPI.RZDKSTATE.DOWN)
			{
				_log.Debug("Key already pressed");
				_log.Debug("<< HandleDynamicKeyEvent()");
				return result;
			}

			_log.Debug("Getting callback");
			var callback = _dkCallbacks[index];

			if (callback == null)
			{
				_log.Debug("Callback is null");
				_log.Debug("<< HandleDynamicKeyEvent()");
				return result;
			}

			try
			{
				_log.Debug("Calling callback");
				callback();
			}
			catch (ObjectDisposedException ex)
			{
				_log.ErrorFormat("Call to dyamic key callback #{0} failed (ObjectDisposedException): {1}", index + 1, ex.Message);
				_log.Error("Exception details as follows", ex);
			}

			_log.Debug("<< HandleDynamicKeyEvent()");
			return result;
		}
	}
}
