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
using Sharparam.SwitchBladeSteam.Extensions;
using Sharparam.SwitchBladeSteam.Helpers;
using Sharparam.SwitchBladeSteam.Native;
using Sharparam.SwitchBladeSteam.Razer.Events;
using Sharparam.SwitchBladeSteam.Razer.Structs;
using log4net;
using LogManager = Sharparam.SwitchBladeSteam.Logging.LogManager;

namespace Sharparam.SwitchBladeSteam.Razer
{
    public class Touchpad : ITouchpad
    {
        public event GestureEventHandler Gesture;

        private readonly ILog _log;

        private RazerAPI.GestureType _activeGesturesType;
        private RazerAPI.GestureType _activeOSGesturesType;

        private bool _allGestureEnabled;
        private bool _allOSGestureEnabled;

        private static RazerAPI.TouchpadGestureCallbackFunctionDelegate _gestureCallback;

        /// <summary>
        /// Will be set to IntPtr.Zero if no handle associated.
        /// </summary>
        public IntPtr CurrentHandle { get; private set; }

        /// <summary>
        /// Current image shown on the Touchpad, null if not using static image.
        /// </summary>
        public string CurrentImage { get; private set; }

        internal Touchpad()
        {
            _log = LogManager.GetLogger(this);
            _log.Debug(">> Touchpad()");
            _log.Info("Setting disabled image");
            _log.Debug("Setting gesture callback");
            _gestureCallback = HandleTouchpadGesture;
            var hResult = RazerAPI.RzSBGestureSetCallback(_gestureCallback);
            if (HRESULT.RZSB_FAILED(hResult))
                RazerManager.NativeCallFailure("RzSBGestureSetCallback", hResult);
            _log.Debug("<< Touchpad()");
        }
        
        private void OnGesture(RazerAPI.GestureType gestureType, uint parameter, ushort x, ushort y, ushort z)
        {
            var func = Gesture;
            if (func != null)
                func(this, new GestureEventArgs(gestureType, parameter, x, y, z));
        }

        public void SetHandle(IntPtr handle, bool translateGestures = true)
        {
            _log.Debug(">> SetHandle([handle])");
            StopRender(false);

            //var hResult = RazerAPI.RzSBWinRenderStart(handle, translateGestures, Constants.DebugEnabled);
            //if (hResult == HRESULT.RZSB_ALREADY_STARTED)
            //{
            //    _log.Warn("WinRender already started, attempting to force render stop and try again...");
            //    StopRender(false, true);
            //    _log.Info("Attempting to start WinRender again...");
            //    hResult = RazerAPI.RzSBWinRenderStart(handle, translateGestures, Constants.DebugEnabled);
            //}

            //if (HRESULT.RZSB_FAILED(hResult))
            //    RazerManager.NativeCallFailure("RzSBWinRenderStart", hResult);

            CurrentHandle = handle;

            _log.Debug("<< SetHandle()");
        }

        public void SetKeyboardEnabledControls(IEnumerable<KeyboardControl> keyboardControls, bool resetList = true)
        {
            _log.Debug(">> SetKeyboardEnabledControls([handles])");

            //_log.Debug("Getting keyboard controls array");
            //var kbControlArray = keyboardControls as KeyboardControl[] ?? keyboardControls.ToArray();

            _log.Debug("<< SetKeyboardEnabledControls()");
        }

        public void SetImage(string image)
        {
            _log.DebugFormat(">> SetImage({0})", image);
            StopRender();

            image = IO.GetAbsolutePath(image);

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
            
            //var hResult = RazerAPI.RzSBWinRenderStop(erase);
            //if (HRESULT.RZSB_FAILED(hResult))
            //    RazerManager.NativeCallFailure("RzSBWinRenderStop", hResult);

            CurrentHandle = IntPtr.Zero;

            _log.Debug("<< StopRender()");
        }

        public void ClearImage()
        {
            _log.Debug(">> ClearImage()");

            var hResult = RazerAPI.RzSBSetImageTouchpad(IO.GetAbsolutePath(Constants.BlankTouchpadImage));
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

        public void SetGesture(RazerAPI.GestureType gestureType, bool enabled)
        {
            _log.DebugFormat(">> SetGesture({0}, {1})", gestureType, enabled ? "true" : "false");
            RazerAPI.GestureType newGesturesType;
            if (gestureType == RazerAPI.GestureType.All)
            {
                newGesturesType = gestureType;
                enabled = !enabled;
            }
            else if (gestureType == RazerAPI.GestureType.None)
            {
                if (_activeGesturesType == RazerAPI.GestureType.None)
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
                    SetGesture(RazerAPI.GestureType.All, true);
                    _log.Debug("<< SetGesture()");
                    return;
                }
                newGesturesType = gestureType;
            }
            else if (enabled)
            {
                if (_activeGesturesType.Has(gestureType) && !(_activeGesturesType == RazerAPI.GestureType.All && !_allGestureEnabled))
                {
                    _log.Debug("Active gestures already have requested value");
                    _log.DebugFormat("_activeGestures == {0}", _activeGesturesType);
                    _log.DebugFormat("_allGestureEnabled == {0}", _allGestureEnabled);
                    return;
                }
                newGesturesType = _activeOSGesturesType.Include(gestureType);
            }
            else
            {
                if (!_activeGesturesType.Has(gestureType))
                {
                    _log.DebugFormat("Request to disable gesture already disabled: {0}", gestureType);
                    _log.DebugFormat("_activeGestures == {0}", _activeGesturesType);
                    return;
                }
                newGesturesType = _activeOSGesturesType.Remove(gestureType);
            }

            var hResult = RazerAPI.RzSBEnableGesture(newGesturesType, enabled);
            if (HRESULT.RZSB_FAILED(hResult))
                RazerManager.NativeCallFailure("RzSBGestureEnable", hResult);

            hResult = RazerAPI.RzSBGestureSetCallback(_gestureCallback);
            if (HRESULT.RZSB_FAILED(hResult))
                RazerManager.NativeCallFailure("RzSBGestureSetCallback", hResult);

            _activeGesturesType = newGesturesType;
            _allGestureEnabled = _activeGesturesType == RazerAPI.GestureType.All && !enabled;

            _log.Debug("<< SetGesture()");
        }

        public void EnableGesture(RazerAPI.GestureType gestureType)
        {
            _log.DebugFormat(">> EnableGesture({0})", gestureType);
            SetGesture(gestureType, true);
            _log.Debug("<< EnableGesture()");
        }

        public void DisableGesture(RazerAPI.GestureType gestureType)
        {
            _log.DebugFormat(">> DisableGesture({0})", gestureType);
            SetGesture(gestureType, false);
            _log.Debug("<< DisableGesture()");
        }

        public void SetOSGesture(RazerAPI.GestureType gestureType, bool enabled)
        {
            _log.DebugFormat(">> SetOSGesture({0}, {1})", gestureType, enabled ? "true" : "false");

            RazerAPI.GestureType newGesturesType;
            if (gestureType == RazerAPI.GestureType.All)
            {
                // Invert the enabled value because of how Razer API works
                enabled = !enabled;
                // "ALL" replaces any other gesture, so we don't want to include/remove it
                newGesturesType = gestureType;
            }
            else if (gestureType == RazerAPI.GestureType.None)
            {
                if (_activeOSGesturesType == RazerAPI.GestureType.None)
                    return;
                if (!enabled) 
                {
                    // Request to "disable no gesture"?
                    // Then just enable all, since that's the same
                    SetOSGesture(RazerAPI.GestureType.All, true);
                    return;
                }
                // "NONE" replaces any other gesture, so we don't want to include/remove it
                newGesturesType = gestureType;
            }
            else if (enabled)
            {
                if (_activeOSGesturesType.Has(gestureType) || !(_activeOSGesturesType == RazerAPI.GestureType.All && !_allOSGestureEnabled))
                    return;
                newGesturesType = _activeOSGesturesType.Include(gestureType);
            }
            else
            {
                if (!_activeOSGesturesType.Has(gestureType))
                    return;
                newGesturesType = _activeOSGesturesType.Remove(gestureType);
            }

            var hResult = RazerAPI.RzSBEnableGesture(newGesturesType, enabled);
            if (HRESULT.RZSB_FAILED(hResult))
                RazerManager.NativeCallFailure("RzSBGestureEnable", hResult);

            hResult = RazerAPI.RzSBEnableOSGesture(newGesturesType, enabled);
            if (HRESULT.RZSB_FAILED(hResult))
                RazerManager.NativeCallFailure("RzSBGestureSetOSNotification", hResult);

            _activeOSGesturesType = newGesturesType;
            _allOSGestureEnabled = _activeGesturesType == RazerAPI.GestureType.All && !enabled;
            _log.Debug("<< SetOSGesture()");
        }

        public void EnableOSGesture(RazerAPI.GestureType gestureType)
        {
            _log.DebugFormat(">> EnableOSGesture({0})", gestureType);
            SetOSGesture(gestureType, true);
            _log.Debug("<< EnableOSGesture()");
        }

        public void DisableOSGesture(RazerAPI.GestureType gestureType)
        {
            _log.DebugFormat(">> DisableOSGesture({0})", gestureType);
            SetOSGesture(gestureType, false);
            _log.Debug("<< DisableOSGesture()");
        }

        private HRESULT HandleTouchpadGesture(RazerAPI.GestureType gestureType, uint dwParameters, ushort wXPos, ushort wYPos, ushort wZPos)
        {
            // Do not log gesture events, it gets VERY SPAMMY

            //_log.DebugFormat(">> HandleTouchpadGesture({0}, {1}, {2}, {3}, {4})", gesture, dwParameters, wXPos, wYPos, wZPos);
            var result = HRESULT.RZSB_OK;
            OnGesture(gestureType, dwParameters, wXPos, wYPos, wZPos);
            //_log.Debug("<< HandleTouchpadGesture()");
            return result;
        }
    }
}
