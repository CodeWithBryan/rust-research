using System;
using System.Collections;
using ConVar;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020004EC RID: 1260
public class GameSetup : MonoBehaviour
{
	// Token: 0x06002804 RID: 10244 RVA: 0x000F5088 File Offset: 0x000F3288
	protected void Awake()
	{
		if (GameSetup.RunOnce)
		{
			GameManager.Destroy(base.gameObject, 0f);
			return;
		}
		GameManifest.Load();
		GameManifest.LoadAssets();
		GameSetup.RunOnce = true;
		if (Bootstrap.needsSetup)
		{
			Bootstrap.Init_Tier0();
			Bootstrap.Init_Systems();
			Bootstrap.Init_Config();
		}
		if (this.initializationCommands.Length > 0)
		{
			foreach (string text in this.initializationCommands.Split(new char[]
			{
				';'
			}))
			{
				ConsoleSystem.Run(ConsoleSystem.Option.Server, text.Trim(), Array.Empty<object>());
			}
		}
		base.StartCoroutine(this.DoGameSetup());
	}

	// Token: 0x06002805 RID: 10245 RVA: 0x000F512D File Offset: 0x000F332D
	private IEnumerator DoGameSetup()
	{
		Rust.Application.isLoading = true;
		TerrainMeta.InitNoTerrain(false);
		ItemManager.Initialize();
		LevelManager.CurrentLevelName = SceneManager.GetActiveScene().name;
		if (this.loadLevel && !string.IsNullOrEmpty(this.loadLevelScene))
		{
			Network.Net.sv.Reset();
			ConVar.Server.level = this.loadLevelScene;
			LoadingScreen.Update("LOADING SCENE");
			UnityEngine.Application.LoadLevelAdditive(this.loadLevelScene);
			LoadingScreen.Update(this.loadLevelScene.ToUpper() + " LOADED");
		}
		if (this.startServer)
		{
			yield return base.StartCoroutine(this.StartServer());
		}
		yield return null;
		Rust.Application.isLoading = false;
		yield break;
	}

	// Token: 0x06002806 RID: 10246 RVA: 0x000F513C File Offset: 0x000F333C
	private IEnumerator StartServer()
	{
		ConVar.GC.collect();
		ConVar.GC.unload();
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		yield return base.StartCoroutine(Bootstrap.StartServer(this.loadSave, this.loadSaveFile, true));
		yield break;
	}

	// Token: 0x0400202D RID: 8237
	public static bool RunOnce;

	// Token: 0x0400202E RID: 8238
	public bool startServer = true;

	// Token: 0x0400202F RID: 8239
	public string clientConnectCommand = "client.connect 127.0.0.1:28015";

	// Token: 0x04002030 RID: 8240
	public bool loadMenu = true;

	// Token: 0x04002031 RID: 8241
	public bool loadLevel;

	// Token: 0x04002032 RID: 8242
	public string loadLevelScene = "";

	// Token: 0x04002033 RID: 8243
	public bool loadSave;

	// Token: 0x04002034 RID: 8244
	public string loadSaveFile = "";

	// Token: 0x04002035 RID: 8245
	public string initializationCommands = "";
}
