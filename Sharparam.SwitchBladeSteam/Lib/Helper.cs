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

        // By punker76 on StackOverflow <http://stackoverflow.com/a/10293595>
        public static T GetDescendantByType<T>(Visual element) where T : class
        {
            if (element == null)
                return null;

            if (element.GetType() == typeof (T))
                return element as T;

            T foundElement = null;

            var frameworkElement = element as FrameworkElement;
            if (frameworkElement != null)
                frameworkElement.ApplyTemplate();

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var visual = VisualTreeHelper.GetChild(element, i) as Visual;
                foundElement = GetDescendantByType<T>(visual);
                if (foundElement != null)
                    break;
            }

            return foundElement;
        }
    }
}
