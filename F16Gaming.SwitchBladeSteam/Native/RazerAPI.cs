/* RazerAPI.cs
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

// Credits to itsbth for helping with P/Invoke

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace F16Gaming.SwitchBladeSteam.Native
{
	public static class RazerAPI
	{
		// Native functions from SwitchBladeSDK32.dll, all functions are __cdecl calls

		[DllImport("SwitchBladeSDK32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern HRESULT RzSBStart();

		[DllImport("SwitchBladeSDK32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern HRESULT RzSBStop();

		[DllImport("SwitchBladeSDK32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern HRESULT RzSBQueryCapabilities(out SBSDKQUERYCAPABILITIES pSBSDKCap);

		[DllImport("SwitchBladeSDK32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern HRESULT RzSBRenderBuffer([In] RZTARGET_DISPLAY dwTarget, [In] IntPtr bufferParams);

		[DllImport("SwitchBladeSDK32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern HRESULT RzSBSetImageDynamicKey(
			[In] RZDYNAMICKEY dk,
			[In] RZDKSTATE state,
			[In] /*[MarshalAs(UnmanagedType.LPStr)]*/ string lpszImageFilename);

		[DllImport("SwitchBladeSDK32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern HRESULT RzSBSetImageTouchpad([In] [MarshalAs(UnmanagedType.LPStr)] string lpszImageFilename);

		[DllImport("SwitchBladeSDK32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern HRESULT RzSBAppEventSetCallback([In] AppEventCallbackDelegate pFn);

		[DllImport("SwitchBladeSDK32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern HRESULT RzSBDynamicKeySetCallback([In] DynamicKeyCallbackFunctionDelegate pFn);

		[DllImport("SwitchBladeSDK32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern HRESULT RzSBGestureSetCallback([In] TouchpadGestureCallbackFunctionDelegate pFn);

		[DllImport("SwitchBladeSDK32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern HRESULT RzSBGestureEnable([In] RZGESTURE gesture, [In] bool bEnable);

		[DllImport("SwitchBladeSDK32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern HRESULT RzSBGestureSetNotification([In] RZGESTURE gesture, [In] bool bEnable);

		[DllImport("SwitchBladeSDK32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern HRESULT RzSBGestureSetOSNotification([In] RZGESTURE gesture, [In] bool bEnable);

		[DllImport("SwitchBladeSDK32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern HRESULT RzSBWinRenderSetDisabledImage([In] [MarshalAs(UnmanagedType.LPWStr)] string pszImageFilename);

		[DllImport("SwitchBladeSDK32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern HRESULT RzSBWinRenderAddKeyInputCtrls([In] IntPtr pKeyboardEvtCtrls, [In] int nCtrlCount, [In] bool bResetList);

		[DllImport("SwitchBladeSDK32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern HRESULT RzSBWinRenderStart([In] IntPtr hwnd, [In] bool bTranslateGestures, [In] bool bVisibleOnDesktop);

		[DllImport("SwitchBladeSDK32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern HRESULT RzSBWinRenderStop([In] bool bEraseOnStop);

		[DllImport("SwitchBladeSDK32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern HRESULT RzSBWinRenderGetStats(out IntPtr pdwCount, out IntPtr pdwMaxTime, out IntPtr pdwLastTime, out IntPtr pdwAverageTime);

		[DllImport("SwitchBladeSDK32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		public static extern HRESULT RzSBWinRenderResetStats();

		// Delegates

		public delegate HRESULT DynamicKeyCallbackFunctionDelegate(RZDYNAMICKEY dynamicKey, RZDKSTATE dynamicKeyState);
		public delegate HRESULT AppEventCallbackDelegate(RZSDKAPPEVENTTYPE appEventType, int dwAppMode, int dwProcessID);
		public delegate HRESULT TouchpadGestureCallbackFunctionDelegate(RZGESTURE gesture, int dwParameters, ushort wXPos, ushort wYPos, ushort wZPos);
		public delegate HRESULT KeyboardCallbackFunctionDelegate(uint uMsg, ushort wParam, int lParam);

		// Structs

		[StructLayout(LayoutKind.Sequential)]
		public struct Point
		{
			public int x;
			public int y;
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct SBSDKQUERYCAPABILITIES
		{
			[FieldOffset(0)]
			public int qc_version;

			[FieldOffset(4)]
			public int qc_BEVersion;

			[FieldOffset(8)]
			public SWITCHBLADEHARDWARETYPE qc_HardwareType;

			[FieldOffset(12)]
			public int qc_numSurfaces;

			[FieldOffset(16)]
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_SUPPORTED_SURFACES)]
			public Point[] qc_surfacegeometry;

			[FieldOffset(32)]
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_SUPPORTED_SURFACES)]
			public int[] qc_pixelformat;

			[FieldOffset(40)]
			public byte qc_numDynamicKeys;

			[FieldOffset(41)]
			public Point qc_DynamicKeyArrangement;

			[FieldOffset(49)]
			public Point qc_keyDynamicKeySize;
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct RZSB_BUFFERPARAMS
		{
			[FieldOffset(0)]
			public PIXEL_TYPE PixelType;

			[FieldOffset(4)]
			public int DataSize;

			[FieldOffset(8)]
			public IntPtr pData;
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct RZSB_KEYEVTCTRLS
		{
			[FieldOffset(0)]
			public IntPtr hwndTarget;

			[FieldOffset(4)]
			public bool bReleaseOnEnter;
		}

		// Enums

		public enum RZDKSTATE
		{
			INVALID = 0,
			DISABLED,
			UP,
			DOWN,
			UNDEFINED
		}

		public enum RZDYNAMICKEY
		{
			INVALID = 0,
			DK1,
			DK2,
			DK3,
			DK4,
			DK5,
			DK6,
			DK7,
			DK8,
			DK9,
			DK10,
			UNDEFINED
		}

		public enum RZTARGET_DISPLAY
		{
			WIDGET = 65536,
			DK_1 = 65537,
			DK_2 = 65538,
			DK_3 = 65539,
			DK_4 = 65540,
			DK_5 = 65541,
			DK_6 = 65542,
			DK_7 = 65543,
			DK_8 = 65544,
			DK_9 = 65545,
			DK_10 = 65546
		}

		public enum PIXEL_TYPE
		{
			RGB565 = 0
		}

		public enum RZSDKAPPEVENTTYPE
		{
			INVALID = 0,
			APPMODE = INVALID + 1,
			UNDEFINED = INVALID + 2
		}

		public enum RZSDKAPPEVENTMODE
		{
			APPLET = 0x02,
			NORMAL = 0x04
		}

		public enum RZGESTURE : uint
		{
			INVALID = 0x00000000,
			NONE = 0x00000001,
			PRESS = 0x00000002,
			TAP = 0x00000004,
			FLICK = 0x00000008,
			ZOOM = 0x00000010,
			ROTATE = 0x00000020,
			ALL = 0x0000003e,
			UNDEFINED = 0xffffffc0
		}

		public enum SWITCHBLADEHARDWARETYPE
		{
			INVALID = 0,
			SWITCHBLADE,
			UNDEFINED
		}

		// Constants

		/*
		 * definitions for the Dynamic Key display region of the Switchblade
		 */
		public const int SWITCHBLADE_DYNAMIC_KEYS_PER_ROW = 5;
		public const int SWITCHBLADE_DYNAMIC_KEYS_ROWS = 2;
		public const int SWITCHBLADE_DYNAMIC_KEY_X_SIZE = 115;
		public const int SWITCHBLADE_DYNAMIC_KEY_Y_SIZE = 115;
		public const int SWITCHBLADE_DK_SIZE_IMAGEDATA = SWITCHBLADE_DYNAMIC_KEY_X_SIZE * SWITCHBLADE_DYNAMIC_KEY_Y_SIZE * sizeof(ushort);

		/*
		 * definitions for the Touchpad display region of the Switchblade
		 */
		public const int SWITCHBLADE_TOUCHPAD_X_SIZE = 800;
		public const int SWITCHBLADE_TOUCHPAD_Y_SIZE = 480;
		public const int SWITCHBLADE_TOUCHPAD_SIZE_IMAGEDATA = SWITCHBLADE_TOUCHPAD_X_SIZE * SWITCHBLADE_TOUCHPAD_Y_SIZE * sizeof(ushort);

		public const int SWITCHBLADE_DISPLAY_COLOR_DEPTH = 16;

		public const int MAX_STRING_LENGTH = 260;

		public const int MAX_SUPPORTED_SURFACES = 2;

		public const int PIXEL_FORMAT_INVALID = 0;
		public const int PIXEL_FORMAT_RGB_565 = 1;

		// "Macros"

		public static bool ValidDynamicKey(int a) { return (int) RZDYNAMICKEY.INVALID < a && a < (int) RZDYNAMICKEY.UNDEFINED; }
		public static bool ValidDynamicKeyState(int a) { return (int) RZDKSTATE.INVALID < a && a < (int) RZDKSTATE.UNDEFINED; }
		public static bool ValidGesture(uint a) { return (a & (uint) RZGESTURE.ALL) != 0; }
		public static bool SingleGesture(uint a) { return 0 == ((a - a) & a); }
		public static int TARGET_MASK(int x) { return (1 << 16) | x; }

#if DEBUG
		// Is this needed?
		public static void DebugCheckFault()
		{
			if (Debugger.IsAttached)
				Debugger.Break();
		}
#else
		public static void DebugCheckFault() {}
#endif
	}
}
