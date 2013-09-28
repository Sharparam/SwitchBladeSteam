using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
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
        [Flags]
        public enum ExtendedWindowStyles
        {
            WS_EX_TOOLWINDOW = 0x00000080
        }

        public enum GetWindowLongFields
        {
            GWL_EXSTYLE = -20
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr IntSetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        private static extern Int32 IntSetWindowLong(IntPtr hWnd, int nIndex, Int32 dwNewLong);

        [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
        public static extern void SetLastError(int dwErrorCode);

        private static int IntPtrToInt32(IntPtr intPtr)
        {
            return unchecked((int)intPtr.ToInt64());
        }

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

        // By Franci Penov & Peter Mortensen on StackOverflow <http://stackoverflow.com/a/551847>
        public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            int error = 0;
            IntPtr result = IntPtr.Zero;
            // Win32 SetWindowLong doesn't clear error on success
            SetLastError(0);

            if (IntPtr.Size == 4)
            {
                // use SetWindowLong
                Int32 tempResult = IntSetWindowLong(hWnd, nIndex, IntPtrToInt32(dwNewLong));
                error = Marshal.GetLastWin32Error();
                result = new IntPtr(tempResult);
            }
            else
            {
                // use SetWindowLongPtr
                result = IntSetWindowLongPtr(hWnd, nIndex, dwNewLong);
                error = Marshal.GetLastWin32Error();
            }

            if ((result == IntPtr.Zero) && (error != 0))
            {
                throw new System.ComponentModel.Win32Exception(error);
            }

            return result;
        }

        public static void ExtendWindowStyleWithTool(Window window)
        {
            var wndHelper = new WindowInteropHelper(window);
            var exStyle = (int) GetWindowLong(wndHelper.Handle, (int) GetWindowLongFields.GWL_EXSTYLE);
            exStyle |= (int) ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            SetWindowLong(wndHelper.Handle, (int) GetWindowLongFields.GWL_EXSTYLE, (IntPtr) exStyle);
        }
    }
}
