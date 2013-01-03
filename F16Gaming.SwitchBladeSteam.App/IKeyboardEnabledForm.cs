using System.Collections.Generic;
using System.Windows.Forms;

namespace F16Gaming.SwitchBladeSteam.App
{
	internal interface IKeyboardEnabledForm
	{
		IEnumerable<Control> GetKeyboardEnabledControls();
	}
}
