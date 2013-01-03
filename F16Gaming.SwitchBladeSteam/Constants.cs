using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace F16Gaming.SwitchBladeSteam
{
	public static class Constants
	{
#if DEBUG
		public const bool DebugEnabled = true;
#else
		public const bool DebugEnabled = false;
#endif
	}
}
