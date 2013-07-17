// This file is automatically generated.
using System;
using System.Text;
using System.Runtime.InteropServices;

namespace Steam4NET
{

	public enum ESteamAPICallFailure : int
	{
		k_ESteamAPICallFailureNone = -1,
		k_ESteamAPICallFailureSteamGone = 0,
		k_ESteamAPICallFailureNetworkFailure = 1,
		k_ESteamAPICallFailureInvalidHandle = 2,
		k_ESteamAPICallFailureMismatchedCallback = 3,
	};
	
	public enum EConfigStore : int
	{
		k_EConfigStoreInvalid = 0,
		k_EConfigStoreInstall = 1,
		k_EConfigStoreUserRoaming = 2,
		k_EConfigStoreUserLocal = 3,
		k_EConfigStoreMax = 4,
	};
	
	public enum ECheckFileSignature : int
	{
		k_ECheckFileSignatureInvalidSignature = 0,
		k_ECheckFileSignatureValidSignature = 1,
		k_ECheckFileSignatureFileNotFound = 2,
		k_ECheckFileSignatureNoSignaturesFoundForThisApp = 3,
		k_ECheckFileSignatureNoSignaturesFoundForThisFile = 4,
	};
	
	public enum ESpewGroup : int
	{
		k_ESpewGroupConsole = 0,
		k_ESpewGroupPublish = 1,
		k_ESpewGroupBootstrap = 2,
		k_ESpewGroupStartup = 3,
		k_ESpewGroupService = 4,
		k_ESpewGroupFileop = 5,
		k_ESpewGroupSystem = 6,
		k_ESpewGroupSmtp = 7,
		k_ESpewGroupAccount = 8,
		k_ESpewGroupJob = 9,
		k_ESpewGroupCrypto = 10,
		k_ESpewGroupNetwork = 11,
		k_ESpewGroupVac = 12,
		k_ESpewGroupClient = 13,
		k_ESpewGroupContent = 14,
		k_ESpewGroupCloud = 15,
		k_ESpewGroupLogon = 16,
		k_ESpewGroupClping = 17,
		k_ESpewGroupThreads = 18,
		k_ESpewGroupBsnova = 19,
		k_ESpewGroupTest = 20,
		k_ESpewGroupFiletx = 21,
		k_ESpewGroupStats = 22,
		k_ESpewGroupSrvping = 23,
		k_ESpewGroupFriends = 24,
		k_ESpewGroupChat = 25,
		k_ESpewGroupGuestpass = 26,
		k_ESpewGroupLicense = 27,
		k_ESpewGroupP2p = 28,
		k_ESpewGroupDatacoll = 29,
		k_ESpewGroupDrm = 30,
		k_ESpewGroupSvcm = 31,
		k_ESpewGroupHttpclient = 32,
		k_ESpewGroupHttpserver = 33,
	};
	
	public enum EUIMode : int
	{
		k_EUIModeNormal = 0,
		k_EUIModeTenFoot = 1,
	};
	
	public enum EGamepadTextInputMode : int
	{
	};
	
	public enum EGamepadTextInputLineMode : int
	{
	};
	
	public enum EWindowType : int
	{
	};
	
	[StructLayout(LayoutKind.Sequential,Pack=8)]
	[InteropHelp.CallbackIdentity(701)]
	public struct IPCountry_t
	{
		public const int k_iCallback = 701;
	};
	
	[StructLayout(LayoutKind.Sequential,Pack=8)]
	[InteropHelp.CallbackIdentity(702)]
	public struct LowBatteryPower_t
	{
		public const int k_iCallback = 702;
		public Byte m_nMinutesBatteryLeft;
	};
	
	[StructLayout(LayoutKind.Sequential,Pack=8)]
	[InteropHelp.CallbackIdentity(703)]
	public struct SteamAPICallCompleted_t
	{
		public const int k_iCallback = 703;
		public UInt64 m_hAsyncCall;
	};
	
	[StructLayout(LayoutKind.Sequential,Pack=8)]
	[InteropHelp.CallbackIdentity(704)]
	public struct SteamShutdown_t
	{
		public const int k_iCallback = 704;
	};
	
	[StructLayout(LayoutKind.Sequential,Pack=8)]
	[InteropHelp.CallbackIdentity(705)]
	public struct CheckFileSignature_t
	{
		public const int k_iCallback = 705;
		public ECheckFileSignature m_eCheckFileSignature;
	};
	
	[StructLayout(LayoutKind.Sequential,Pack=8)]
	[InteropHelp.CallbackIdentity(711)]
	public struct SteamConfigStoreChanged_t
	{
		public const int k_iCallback = 711;
		public EConfigStore m_eConfigStore;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
		public string m_szRootOfChanges;
	};
	
	[StructLayout(LayoutKind.Sequential,Pack=8)]
	[InteropHelp.CallbackIdentity(1603)]
	public struct CellIDChanged_t
	{
		public const int k_iCallback = 1603;
		public UInt32 m_nCellID;
	};
	
}
