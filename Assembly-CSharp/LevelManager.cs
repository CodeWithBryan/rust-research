using System;
using System.Collections;
using Network;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000522 RID: 1314
public static class LevelManager
{
	// Token: 0x17000333 RID: 819
	// (get) Token: 0x0600285C RID: 10332 RVA: 0x000F6254 File Offset: 0x000F4454
	public static bool isLoaded
	{
		get
		{
			return LevelManager.CurrentLevelName != null && !(LevelManager.CurrentLevelName == "") && !(LevelManager.CurrentLevelName == "Empty") && !(LevelManager.CurrentLevelName == "MenuBackground");
		}
	}

	// Token: 0x0600285D RID: 10333 RVA: 0x000F62A4 File Offset: 0x000F44A4
	public static bool IsValid(string strName)
	{
		return Application.CanStreamedLevelBeLoaded(strName);
	}

	// Token: 0x0600285E RID: 10334 RVA: 0x000F62AC File Offset: 0x000F44AC
	public static void LoadLevel(string strName, bool keepLoadingScreenOpen = true)
	{
		if (strName == "proceduralmap")
		{
			strName = "Procedural Map";
		}
		LevelManager.CurrentLevelName = strName;
		Net.sv.Reset();
		SceneManager.LoadScene(strName, LoadSceneMode.Single);
	}

	// Token: 0x0600285F RID: 10335 RVA: 0x000F62D9 File Offset: 0x000F44D9
	public static IEnumerator LoadLevelAsync(string strName, bool keepLoadingScreenOpen = true)
	{
		if (strName == "proceduralmap")
		{
			strName = "Procedural Map";
		}
		LevelManager.CurrentLevelName = strName;
		Net.sv.Reset();
		yield return null;
		yield return SceneManager.LoadSceneAsync(strName, LoadSceneMode.Single);
		yield return null;
		yield return null;
		yield break;
	}

	// Token: 0x06002860 RID: 10336 RVA: 0x000F62E8 File Offset: 0x000F44E8
	public static void UnloadLevel(bool loadingScreen = true)
	{
		LevelManager.CurrentLevelName = null;
		SceneManager.LoadScene("Empty");
	}

	// Token: 0x040020D4 RID: 8404
	public static string CurrentLevelName;
}
