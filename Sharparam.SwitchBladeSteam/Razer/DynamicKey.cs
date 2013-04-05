﻿/* DynamicKey.cs
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
using Sharparam.SwitchBladeSteam.Helpers;
using Sharparam.SwitchBladeSteam.Native;
using Sharparam.SwitchBladeSteam.Razer.Events;
using Sharparam.SwitchBladeSteam.Razer.Exceptions;
using log4net;
using LogManager = Sharparam.SwitchBladeSteam.Logging.LogManager;

namespace Sharparam.SwitchBladeSteam.Razer
{
    /// <summary>
    /// Represents a dynamic key on the SwitchBlade device
    /// </summary>
    public class DynamicKey
    {
        public event DynamicKeyPressedEventHandler KeyPressed;

        private readonly ILog _log;

        public RazerAPI.DynamicKeyType KeyType { get; private set; }
        public RazerAPI.DynamicKeyState State { get; private set; }
        public RazerAPI.DynamicKeyState PreviousState { get; private set; }
        public string UpImage { get; private set; }
        public string DownImage { get; private set; }
        public bool SingleImage { get { return UpImage == DownImage; } }

        internal DynamicKey(RazerAPI.DynamicKeyType keyType, string upImage, string downImage = null, DynamicKeyPressedEventHandler callback = null)
        {
            _log = LogManager.GetLogger(this);

            _log.DebugFormat(">> DynamicKey({0}, \"{1}\", {2})", keyType, upImage, downImage == null ? "null" : "\"" + downImage + "\"");

            if (string.IsNullOrEmpty(upImage))
                throw new ArgumentException("Can't be null or empty", "upImage");

            if (string.IsNullOrEmpty(downImage))
            {
                _log.Debug("downImage is null, setting to value of upImage");
                downImage = upImage;
            }

            _log.Debug("Setting default states");
            State = RazerAPI.DynamicKeyState.None;
            PreviousState = RazerAPI.DynamicKeyState.None;
            UpImage = upImage;
            DownImage = downImage;
            KeyType = keyType;

            _log.Debug("Setting images");
            SetUpImage(UpImage);
            SetDownImage(DownImage);

            if (callback != null)
            {
                _log.Debug("Setting callback");
                KeyPressed += callback;
            }

            _log.Debug("<< DynamicKey()");
        }

        private void OnKeyPressed()
        {
            var func = KeyPressed;
            if (func == null)
                return;

            try
            {
                func(this, null);
            }
            catch (ObjectDisposedException ex)
            {
                _log.ErrorFormat("OnKeyPressed: ObjectDisposedException: {0}", ex.Message);
            }
        }

        internal void UpdateState(RazerAPI.DynamicKeyState state)
        {
            _log.DebugFormat(">> UpdateState({0})", state);
            PreviousState = State;
            State = state;
            if (State == RazerAPI.DynamicKeyState.Down && PreviousState == RazerAPI.DynamicKeyState.Up)
                OnKeyPressed();
            _log.Debug("<< UpdateState()");
        }

        [Obsolete("UpdateState now handles setting the previous state")]
        internal void UpdatePreviousState(RazerAPI.DynamicKeyState state)
        {
            _log.DebugFormat(">> UpdatePreviousState({0})", state);
            PreviousState = state;
            _log.Debug("<< UpdatePreviousState()");
        }

        public void SetImage(string image)
        {
            _log.DebugFormat(">> SetImage({0})", image);
            SetUpImage(image);
            SetDownImage(image);
            _log.Debug("<< SetImage()");
        }

        public void SetImage(string image, RazerAPI.DynamicKeyState state)
        {
            _log.DebugFormat(">> SetImage({0}, {1})", image, state);

            if (state != RazerAPI.DynamicKeyState.Up && state != RazerAPI.DynamicKeyState.Down)
                throw new ArgumentException("State can only be up or down", "state");

            var hResult = RazerAPI.RzSBSetImageDynamicKey(KeyType, state, IO.GetAbsolutePath(image));
            if (!HRESULT.RZSB_SUCCESS(hResult))
                throw new RazerNativeException(hResult);

            if (state == RazerAPI.DynamicKeyState.Up)
                UpImage = image;
            else
                DownImage = image;

            _log.Debug("<< SetImage()");
        }

        public void SetUpImage(string image)
        {
            _log.DebugFormat(">> SetUpImage({0})", image);
            SetImage(image, RazerAPI.DynamicKeyState.Up);
            _log.Debug("<< SetUpImage()");
        }

        public void SetDownImage(string image)
        {
            _log.DebugFormat(">> SetDownImage({0})", image);
            SetImage(image, RazerAPI.DynamicKeyState.Down);
            _log.Debug("<< SetDownImage()");
        }

        public void Disable()
        {
            SetImage(Constants.DisabledDynamicKeyImage);
        }
    }
}
