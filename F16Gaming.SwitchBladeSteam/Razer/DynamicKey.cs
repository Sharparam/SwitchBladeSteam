/* DynamicKey.cs
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

using F16Gaming.SwitchBladeSteam.Native;
using F16Gaming.SwitchBladeSteam.Razer.Exceptions;

namespace F16Gaming.SwitchBladeSteam.Razer
{
	/// <summary>
	/// Represents a dynamic key on the SwitchBlade device
	/// </summary>
	public class DynamicKey
	{
		public RazerAPI.RZDYNAMICKEY Key { get; private set; }
		public string UpImage { get; private set; }
		public string DownImage { get; private set; }
		public bool SingleImage { get { return UpImage == DownImage; } }

		internal DynamicKey(RazerAPI.RZDYNAMICKEY key, string upImage, string downImage = null)
		{
			if (string.IsNullOrEmpty(upImage))
				throw new ArgumentException("Can't be null or empty", "upImage");

			if (string.IsNullOrEmpty(downImage))
				downImage = upImage;

			UpImage = upImage;
			DownImage = downImage;
			Key = key;

			SetImage(UpImage, RazerAPI.RZDKSTATE.UP);
			SetImage(DownImage, RazerAPI.RZDKSTATE.DOWN);
		}

		public void SetImage(string image)
		{
			SetUpImage(image);
			SetDownImage(image);
		}

		public void SetImage(string image, RazerAPI.RZDKSTATE state)
		{
			if (state != RazerAPI.RZDKSTATE.UP && state != RazerAPI.RZDKSTATE.DOWN)
				throw new ArgumentException("State can only be up or down", "state");

			var hResult = RazerAPI.RzSBSetImageDynamicKey(Key, state, image);
			if (!HRESULT.RZSB_SUCCESS(hResult))
				throw new RazerNativeException(hResult);

			if (state == RazerAPI.RZDKSTATE.UP)
				UpImage = image;
			else
				DownImage = image;
		}

		public void SetUpImage(string image)
		{
			SetImage(image, RazerAPI.RZDKSTATE.UP);
		}

		public void SetDownImage(string image)
		{
			SetImage(image, RazerAPI.RZDKSTATE.DOWN);
		}
	}
}
