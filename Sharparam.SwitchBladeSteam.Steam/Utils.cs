/* Utils.cs
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

using System.Collections.Generic;
using System.Drawing;
using Steam4NET;

namespace F16Gaming.SwitchBladeSteam.Steam
{
	public static class Utils
	{
		private static readonly Dictionary<EPersonaState, string> StateMapping = new Dictionary<EPersonaState, string>
		{
			{EPersonaState.k_EPersonaStateAway, "Away"},
			{EPersonaState.k_EPersonaStateBusy, "Busy"},
			{EPersonaState.k_EPersonaStateLookingToPlay, "Looking To Play"},
			{EPersonaState.k_EPersonaStateLookingToTrade, "Looking To Trade"},
			{EPersonaState.k_EPersonaStateOffline, "Offline"},
			{EPersonaState.k_EPersonaStateOnline, "Online"},
			{EPersonaState.k_EPersonaStateSnooze, "Snooze"}
		};

		public static string StateToString(EPersonaState state)
		{
			return StateMapping.ContainsKey(state) ? StateMapping[state] : "Unknown";
		}

		// All credit to kimoto on GitHub
		// https://gist.github.com/986866/b2dc849940d4bdba858f3b305bf68afda641e21e
		public static Bitmap GetAvatarFromHandle(int handle, ISteamUtils005 utils)
		{
			uint width = 0;
			uint height = 0;
			if (!utils.GetImageSize(handle, ref width, ref height))
				return null;

			var size = 4 * width * height;
			var buffer = new byte[size];
			if (!utils.GetImageRGBA(handle, buffer, (int) size))
				return null;

			var bitmap = new Bitmap((int) width, (int) height);
			for (var x = 0; x < width; x++)
				for (var y = 0; y < height; y++)
				{
					var index = (y * (int) width + x) * 4;
					int r = buffer[index + 0];
					int g = buffer[index + 1];
					int b = buffer[index + 2];
					int a = buffer[index + 3];
					var color = Color.FromArgb(a, r, g, b);
					bitmap.SetPixel(x, y, color);
				}

			return bitmap;
		}
	}
}
