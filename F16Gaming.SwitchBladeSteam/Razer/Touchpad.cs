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
using System.Text;
using F16Gaming.SwitchBladeSteam.Native;
using F16Gaming.SwitchBladeSteam.Razer.Exceptions;

namespace F16Gaming.SwitchBladeSteam.Razer
{
	public class Touchpad
	{
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

		internal Touchpad(IntPtr handle)
		{
			
		}

		internal Touchpad(string image)
		{
			
		}

		public void SetHandle(IntPtr handle)
		{
			HRESULT hResult;

			// Check if we are currently rendering a window
			if (CurrentHandle != IntPtr.Zero)
			{
				// Stop current render before starting new one
				hResult = RazerAPI.RzSBWinRenderStop(false);
				if (!HRESULT.RZSB_SUCCESS(hResult))
					throw new RazerNativeException(hResult);
			}

			hResult = RazerAPI.RzSBWinRenderStart(handle, true, Constants.DebugEnabled);
			if (!HRESULT.RZSB_SUCCESS(hResult))
				throw new RazerNativeException(hResult);
		}

		public void SetImage(string image)
		{
			
		}
	}
}
