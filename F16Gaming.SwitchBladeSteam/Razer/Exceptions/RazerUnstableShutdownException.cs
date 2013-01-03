using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace F16Gaming.SwitchBladeSteam.Razer.Exceptions
{
	public class RazerUnstableShutdownException : RazerException
	{
		internal RazerUnstableShutdownException()
			: base("The application did not properly call RzSBStop() on the last shutdown.")
		{
			
		}
	}
}
