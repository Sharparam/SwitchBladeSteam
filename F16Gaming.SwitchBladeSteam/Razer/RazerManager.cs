using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

			_log.Debug("Calling RzSBDynamicKeySetCallback");

			hResult = RazerAPI.RzSBDynamicKeySetCallback(HandleDynamicKeyEvent);
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
			_touchpad.StopRender();
			_touchpad.ClearImage();
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

			_log.Debug("Calling callback");
			callback();

			_log.Debug("<< HandleDynamicKeyEvent()");
			return result;
		}
	}
}
