using System;
using F16Gaming.SwitchBladeSteam.Native;

namespace F16Gaming.SwitchBladeSteam.Razer.Events
{
	public class DynamicKeyEventArgs : EventArgs
	{
		public readonly RazerAPI.RZDYNAMICKEY Key;
		public readonly RazerAPI.RZDKSTATE State;

		internal DynamicKeyEventArgs(RazerAPI.RZDYNAMICKEY key, RazerAPI.RZDKSTATE state)
		{
			Key = key;
			State = state;
		}
	}

	public delegate void DynamicKeyEventHandler(object sender, DynamicKeyEventArgs e);
}
