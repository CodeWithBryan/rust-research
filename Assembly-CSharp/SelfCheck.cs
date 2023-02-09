using System;
using System.Runtime.InteropServices;
using Facepunch;
using UnityEngine;

// Token: 0x020004C0 RID: 1216
public static class SelfCheck
{
	// Token: 0x0600271C RID: 10012 RVA: 0x000F12F8 File Offset: 0x000EF4F8
	public static bool Run()
	{
		if (FileSystem.Backend.isError)
		{
			return SelfCheck.Failed("Asset Bundle Error: " + FileSystem.Backend.loadingError);
		}
		if (FileSystem.Load<GameManifest>("Assets/manifest.asset", true) == null)
		{
			return SelfCheck.Failed("Couldn't load game manifest - verify your game content!");
		}
		if (!SelfCheck.TestRustNative())
		{
			return false;
		}
		if (CommandLine.HasSwitch("-force-feature-level-9-3"))
		{
			return SelfCheck.Failed("Invalid command line argument: -force-feature-level-9-3");
		}
		if (CommandLine.HasSwitch("-force-feature-level-10-0"))
		{
			return SelfCheck.Failed("Invalid command line argument: -force-feature-level-10-0");
		}
		return !CommandLine.HasSwitch("-force-feature-level-10-1") || SelfCheck.Failed("Invalid command line argument: -force-feature-level-10-1");
	}

	// Token: 0x0600271D RID: 10013 RVA: 0x000F1398 File Offset: 0x000EF598
	private static bool Failed(string Message)
	{
		if (SingletonComponent<Bootstrap>.Instance)
		{
			SingletonComponent<Bootstrap>.Instance.messageString = "";
			SingletonComponent<Bootstrap>.Instance.ThrowError(Message);
		}
		Debug.LogError("SelfCheck Failed: " + Message);
		return false;
	}

	// Token: 0x0600271E RID: 10014 RVA: 0x000F13D4 File Offset: 0x000EF5D4
	private static bool TestRustNative()
	{
		try
		{
			if (!SelfCheck.RustNative_VersionCheck(5))
			{
				return SelfCheck.Failed("RustNative is wrong version!");
			}
		}
		catch (DllNotFoundException ex)
		{
			return SelfCheck.Failed("RustNative library couldn't load! " + ex.Message);
		}
		return true;
	}

	// Token: 0x0600271F RID: 10015
	[DllImport("RustNative")]
	private static extern bool RustNative_VersionCheck(int version);
}
