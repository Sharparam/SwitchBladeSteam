using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace F16Gaming.SwitchBladeSteam.Razer.Exceptions
{
	public class RazerException : Exception
	{
		internal RazerException(string message = null, Exception innerException = null) : base(message, innerException)
		{
			
		}
	}
}
