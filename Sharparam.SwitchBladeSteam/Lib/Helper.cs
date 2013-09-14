using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32.SafeHandles;

namespace Sharparam.SwitchBladeSteam.Lib
{
    public class SafeHBitmapHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [SecurityCritical]
        public SafeHBitmapHandle(IntPtr preexistingHandle, bool ownsHandle)
            : base(ownsHandle)
        {
            SetHandle(preexistingHandle);
        }

        protected override bool ReleaseHandle()
        {
            return DeleteObject(handle);
        }
    }

    public static class Helper
    {
        public static BitmapSource BitmapToSource(Bitmap bmp)
        {
            IntPtr hbitmap = bmp.GetHbitmap();
            BitmapSource result;
            using (var handle = new SafeHBitmapHandle(hbitmap, true))
            {
                result = Imaging.CreateBitmapSourceFromHBitmap(handle.DangerousGetHandle(), IntPtr.Zero, Int32Rect.Empty,
                                                               BitmapSizeOptions.FromEmptyOptions());
            }
            return result;
        }
    }
}
