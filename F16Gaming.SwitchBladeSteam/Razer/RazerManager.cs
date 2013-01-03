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
	public class RazerManager
	{
		private const string RazerControlFile = "DO_NOT_TOUCH__RAZER_CONTROL_FILE";

		private ILog _log;

		private Touchpad _touchpad;
		private DynamicKey[] _dynamicKeys;

		public RazerManager()
		{
			_log = LogManager.GetLogger(this);

			_log.Info(">> RazerManager()");

			if (File.Exists(RazerControlFile))
			{
				_log.Error("Detected control file presence, throwing exception.");
				_log.Info("<< RazerManager()");
				throw new RazerUnstableShutdownException();
			}

			File.Create(RazerControlFile);

			var hResult = RazerAPI.RzSBStart();
			if (!HRESULT.RZSB_SUCCESS(hResult))
				throw new RazerNativeException(hResult);

			_dynamicKeys = new DynamicKey[RazerAPI.DYNAMIC_KEYS_COUNT];
		}
	}
}
