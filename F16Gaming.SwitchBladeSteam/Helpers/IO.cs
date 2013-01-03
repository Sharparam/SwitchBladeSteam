using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace F16Gaming.SwitchBladeSteam.Helpers
{
	public static class IO
	{
		public static string GetAbsolutePath(string path)
		{
			return Path.Combine(Directory.GetCurrentDirectory(), path);
		}
	}
}
