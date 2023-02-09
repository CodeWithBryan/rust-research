using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using CompanionServer;
using ConVar;
using Facepunch;
using Facepunch.Network;
using Facepunch.Network.Raknet;
using Facepunch.Rust;
using Facepunch.Utility;
using Network;
using Rust;
using Rust.Ai;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020004BF RID: 1215
public class Bootstrap : SingletonComponent<Bootstrap>
{
	// Token: 0x17000318 RID: 792
	// (get) Token: 0x0600270A RID: 9994 RVA: 0x000F101C File Offset: 0x000EF21C
	public static bool needsSetup
	{
		get
		{
			return !Bootstrap.bootstrapInitRun;
		}
	}

	// Token: 0x17000319 RID: 793
	// (get) Token: 0x0600270B RID: 9995 RVA: 0x000F1026 File Offset: 0x000EF226
	public static bool isPresent
	{
		get
		{
			return Bootstrap.bootstrapInitRun || UnityEngine.Object.FindObjectsOfType<GameSetup>().Count<GameSetup>() > 0;
		}
	}

	// Token: 0x0600270C RID: 9996 RVA: 0x000F1041 File Offset: 0x000EF241
	public static void RunDefaults()
	{
		Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
		Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
		UnityEngine.Application.targetFrameRate = 256;
		UnityEngine.Time.fixedDeltaTime = 0.0625f;
		UnityEngine.Time.maximumDeltaTime = 0.125f;
	}

	// Token: 0x0600270D RID: 9997 RVA: 0x000F1080 File Offset: 0x000EF280
	public static void Init_Tier0()
	{
		Bootstrap.RunDefaults();
		GameSetup.RunOnce = true;
		Bootstrap.bootstrapInitRun = true;
		ConsoleSystem.Index.Initialize(ConsoleGen.All);
		UnityButtons.Register();
		Output.Install();
		Facepunch.Pool.ResizeBuffer<Networkable>(65536);
		Facepunch.Pool.ResizeBuffer<EntityLink>(65536);
		Facepunch.Pool.FillBuffer<Networkable>(int.MaxValue);
		Facepunch.Pool.FillBuffer<EntityLink>(int.MaxValue);
		if (Facepunch.CommandLine.HasSwitch("-networkthread"))
		{
			BaseNetwork.Multithreading = true;
		}
		SteamNetworking.SetDebugFunction();
		if (Facepunch.CommandLine.HasSwitch("-swnet"))
		{
			Bootstrap.NetworkInitSteamworks(false);
		}
		else if (Facepunch.CommandLine.HasSwitch("-sdrnet"))
		{
			Bootstrap.NetworkInitSteamworks(true);
		}
		else if (Facepunch.CommandLine.HasSwitch("-raknet"))
		{
			Bootstrap.NetworkInitRaknet();
		}
		else
		{
			Bootstrap.NetworkInitRaknet();
		}
		if (!UnityEngine.Application.isEditor)
		{
			string str = Facepunch.CommandLine.Full.Replace(Facepunch.CommandLine.GetSwitch("-rcon.password", Facepunch.CommandLine.GetSwitch("+rcon.password", "RCONPASSWORD")), "******");
			Bootstrap.WriteToLog("Command Line: " + str);
		}
	}

	// Token: 0x0600270E RID: 9998 RVA: 0x000F1174 File Offset: 0x000EF374
	public static void Init_Systems()
	{
		Rust.Global.Init();
		if (GameInfo.IsOfficialServer && ConVar.Server.stats)
		{
			GA.Logging = false;
			GA.Build = BuildInfo.Current.Scm.ChangeId;
			GA.Initialize("218faecaf1ad400a4e15c53392ebeebc", "0c9803ce52c38671278899538b9c54c8d4e19849");
			Analytics.Server.Enabled = true;
		}
		Facepunch.Application.Initialize(new Integration());
		Facepunch.Performance.GetMemoryUsage = (() => SystemInfoEx.systemMemoryUsed);
	}

	// Token: 0x0600270F RID: 9999 RVA: 0x000F11F4 File Offset: 0x000EF3F4
	public static void Init_Config()
	{
		ConsoleNetwork.Init();
		ConsoleSystem.UpdateValuesFromCommandLine();
		ConsoleSystem.Run(ConsoleSystem.Option.Server, "server.readcfg", Array.Empty<object>());
		ServerUsers.Load();
	}

	// Token: 0x06002710 RID: 10000 RVA: 0x000F121A File Offset: 0x000EF41A
	public static void NetworkInitRaknet()
	{
		Network.Net.sv = new Facepunch.Network.Raknet.Server();
	}

	// Token: 0x06002711 RID: 10001 RVA: 0x000F1226 File Offset: 0x000EF426
	public static void NetworkInitSteamworks(bool enableSteamDatagramRelay)
	{
		Network.Net.sv = new SteamNetworking.Server(enableSteamDatagramRelay);
	}

	// Token: 0x06002712 RID: 10002 RVA: 0x000F1233 File Offset: 0x000EF433
	private IEnumerator Start()
	{
		Bootstrap.WriteToLog("Bootstrap Startup");
		BenchmarkTimer.Enabled = Facepunch.Utility.CommandLine.Full.Contains("+autobench");
		BenchmarkTimer timer = BenchmarkTimer.New("bootstrap");
		if (!UnityEngine.Application.isEditor)
		{
			ExceptionReporter.InitializeFromUrl("https://83df169465e84da091c1a3cd2fbffeee:3671b903f9a840ecb68411cf946ab9b6@sentry.io/51080");
			ExceptionReporter.Disabled = (!Facepunch.Utility.CommandLine.Full.Contains("-official") && !Facepunch.Utility.CommandLine.Full.Contains("-server.official") && !Facepunch.Utility.CommandLine.Full.Contains("+official") && !Facepunch.Utility.CommandLine.Full.Contains("+server.official"));
			BuildInfo buildInfo = BuildInfo.Current;
			if (buildInfo.Scm.Branch != null && buildInfo.Scm.Branch.StartsWith("main"))
			{
				ExceptionReporter.InitializeFromUrl("https://0654eb77d1e04d6babad83201b6b6b95:d2098f1d15834cae90501548bd5dbd0d@sentry.io/1836389");
				ExceptionReporter.Disabled = false;
			}
		}
		BenchmarkTimer benchmarkTimer;
		if (AssetBundleBackend.Enabled)
		{
			AssetBundleBackend newBackend = new AssetBundleBackend();
			using (BenchmarkTimer.New("bootstrap;bundles"))
			{
				yield return base.StartCoroutine(Bootstrap.LoadingUpdate("Opening Bundles"));
				newBackend.Load("Bundles/Bundles");
				FileSystem.Backend = newBackend;
			}
			benchmarkTimer = null;
			if (FileSystem.Backend.isError)
			{
				this.ThrowError(FileSystem.Backend.loadingError);
				yield break;
			}
			using (BenchmarkTimer.New("bootstrap;bundlesindex"))
			{
				newBackend.BuildFileIndex();
			}
			newBackend = null;
		}
		if (FileSystem.Backend.isError)
		{
			this.ThrowError(FileSystem.Backend.loadingError);
			yield break;
		}
		if (!UnityEngine.Application.isEditor)
		{
			Bootstrap.WriteToLog(SystemInfoGeneralText.currentInfo);
		}
		UnityEngine.Texture.SetGlobalAnisotropicFilteringLimits(1, 16);
		if (Bootstrap.isErrored)
		{
			yield break;
		}
		using (BenchmarkTimer.New("bootstrap;gamemanifest"))
		{
			yield return base.StartCoroutine(Bootstrap.LoadingUpdate("Loading Game Manifest"));
			GameManifest.Load();
			yield return base.StartCoroutine(Bootstrap.LoadingUpdate("DONE!"));
		}
		benchmarkTimer = null;
		using (BenchmarkTimer.New("bootstrap;selfcheck"))
		{
			yield return base.StartCoroutine(Bootstrap.LoadingUpdate("Running Self Check"));
			SelfCheck.Run();
		}
		benchmarkTimer = null;
		if (Bootstrap.isErrored)
		{
			yield break;
		}
		yield return base.StartCoroutine(Bootstrap.LoadingUpdate("Bootstrap Tier0"));
		using (BenchmarkTimer.New("bootstrap;tier0"))
		{
			Bootstrap.Init_Tier0();
		}
		using (BenchmarkTimer.New("bootstrap;commandlinevalues"))
		{
			ConsoleSystem.UpdateValuesFromCommandLine();
		}
		yield return base.StartCoroutine(Bootstrap.LoadingUpdate("Bootstrap Systems"));
		using (BenchmarkTimer.New("bootstrap;init_systems"))
		{
			Bootstrap.Init_Systems();
		}
		yield return base.StartCoroutine(Bootstrap.LoadingUpdate("Bootstrap Config"));
		using (BenchmarkTimer.New("bootstrap;init_config"))
		{
			Bootstrap.Init_Config();
		}
		if (Bootstrap.isErrored)
		{
			yield break;
		}
		yield return base.StartCoroutine(Bootstrap.LoadingUpdate("Loading Items"));
		using (BenchmarkTimer.New("bootstrap;itemmanager"))
		{
			ItemManager.Initialize();
		}
		if (Bootstrap.isErrored)
		{
			yield break;
		}
		yield return base.StartCoroutine(this.DedicatedServerStartup());
		BenchmarkTimer benchmarkTimer3 = timer;
		if (benchmarkTimer3 != null)
		{
			benchmarkTimer3.Dispose();
		}
		GameManager.Destroy(base.gameObject, 0f);
		yield break;
		yield break;
	}

	// Token: 0x06002713 RID: 10003 RVA: 0x000F1242 File Offset: 0x000EF442
	private IEnumerator DedicatedServerStartup()
	{
		Rust.Application.isLoading = true;
		UnityEngine.Application.backgroundLoadingPriority = UnityEngine.ThreadPriority.High;
		Bootstrap.WriteToLog("Skinnable Warmup");
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		GameManifest.LoadAssets();
		Bootstrap.WriteToLog("Loading Scene");
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		UnityEngine.Physics.solverIterationCount = 3;
		int @int = PlayerPrefs.GetInt("UnityGraphicsQuality");
		QualitySettings.SetQualityLevel(0);
		PlayerPrefs.SetInt("UnityGraphicsQuality", @int);
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		UnityEngine.Object.DontDestroyOnLoad(GameManager.server.CreatePrefab("assets/bundled/prefabs/system/server_console.prefab", true));
		this.StartupShared();
		global::World.InitSize(ConVar.Server.worldsize);
		global::World.InitSeed(ConVar.Server.seed);
		global::World.InitSalt(ConVar.Server.salt);
		global::World.Url = ConVar.Server.levelurl;
		global::World.Transfer = ConVar.Server.leveltransfer;
		LevelManager.LoadLevel(ConVar.Server.level, true);
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		string[] assetList = FileSystem_Warmup.GetAssetList(null);
		yield return base.StartCoroutine(FileSystem_Warmup.Run(assetList, new Action<string>(Bootstrap.WriteToLog), "Asset Warmup ({0}/{1})", 0));
		yield return base.StartCoroutine(Bootstrap.StartServer(!Facepunch.CommandLine.HasSwitch("-skipload"), "", false));
		if (!UnityEngine.Object.FindObjectOfType<global::Performance>())
		{
			UnityEngine.Object.DontDestroyOnLoad(GameManager.server.CreatePrefab("assets/bundled/prefabs/system/performance.prefab", true));
		}
		Facepunch.Pool.Clear();
		Rust.GC.Collect();
		Rust.Application.isLoading = false;
		yield break;
	}

	// Token: 0x06002714 RID: 10004 RVA: 0x000F1251 File Offset: 0x000EF451
	public static IEnumerator StartServer(bool doLoad, string saveFileOverride, bool allowOutOfDateSaves)
	{
		float timeScale = UnityEngine.Time.timeScale;
		if (ConVar.Time.pausewhileloading)
		{
			UnityEngine.Time.timeScale = 0f;
		}
		RCon.Initialize();
		BaseEntity.Query.Server = new BaseEntity.Query.EntityTree(8096f);
		if (SingletonComponent<WorldSetup>.Instance)
		{
			yield return SingletonComponent<WorldSetup>.Instance.StartCoroutine(SingletonComponent<WorldSetup>.Instance.InitCoroutine());
		}
		if (SingletonComponent<DynamicNavMesh>.Instance && SingletonComponent<DynamicNavMesh>.Instance.enabled && !AiManager.nav_disable)
		{
			yield return SingletonComponent<DynamicNavMesh>.Instance.StartCoroutine(SingletonComponent<DynamicNavMesh>.Instance.UpdateNavMeshAndWait());
		}
		if (SingletonComponent<AiManager>.Instance && SingletonComponent<AiManager>.Instance.enabled)
		{
			SingletonComponent<AiManager>.Instance.Initialize();
			if (!AiManager.nav_disable && AI.npc_enable && TerrainMeta.Path != null)
			{
				foreach (MonumentInfo monumentInfo in TerrainMeta.Path.Monuments)
				{
					if (monumentInfo.HasNavmesh)
					{
						yield return monumentInfo.StartCoroutine(monumentInfo.GetMonumentNavMesh().UpdateNavMeshAndWait());
					}
				}
				List<MonumentInfo>.Enumerator enumerator = default(List<MonumentInfo>.Enumerator);
				if (TerrainMeta.Path && TerrainMeta.Path.DungeonGridRoot)
				{
					DungeonNavmesh dungeonNavmesh = TerrainMeta.Path.DungeonGridRoot.AddComponent<DungeonNavmesh>();
					dungeonNavmesh.NavMeshCollectGeometry = NavMeshCollectGeometry.PhysicsColliders;
					dungeonNavmesh.LayerMask = 65537;
					yield return dungeonNavmesh.StartCoroutine(dungeonNavmesh.UpdateNavMeshAndWait());
				}
				else
				{
					Debug.LogError("Failed to find DungeonGridRoot, NOT generating Dungeon navmesh");
				}
				if (TerrainMeta.Path && TerrainMeta.Path.DungeonBaseRoot)
				{
					DungeonNavmesh dungeonNavmesh2 = TerrainMeta.Path.DungeonBaseRoot.AddComponent<DungeonNavmesh>();
					dungeonNavmesh2.NavmeshResolutionModifier = 0.3f;
					dungeonNavmesh2.NavMeshCollectGeometry = NavMeshCollectGeometry.PhysicsColliders;
					dungeonNavmesh2.LayerMask = 65537;
					yield return dungeonNavmesh2.StartCoroutine(dungeonNavmesh2.UpdateNavMeshAndWait());
				}
				else
				{
					Debug.LogError("Failed to find DungeonBaseRoot , NOT generating Dungeon navmesh");
				}
				GenerateDungeonBase.SetupAI();
			}
		}
		GameObject gameObject = GameManager.server.CreatePrefab("assets/bundled/prefabs/system/server.prefab", true);
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		ServerMgr serverMgr = gameObject.GetComponent<ServerMgr>();
		serverMgr.Initialize(doLoad, saveFileOverride, allowOutOfDateSaves, false);
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		SaveRestore.InitializeEntityLinks();
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		SaveRestore.InitializeEntitySupports();
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		SaveRestore.InitializeEntityConditionals();
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		SaveRestore.GetSaveCache();
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		BaseGameMode.CreateGameMode("");
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		MissionManifest.Get();
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		serverMgr.OpenConnection();
		CompanionServer.Server.Initialize();
		using (BenchmarkTimer.New("Boombox.LoadStations"))
		{
			BoomBox.LoadStations();
		}
		if (ConVar.Time.pausewhileloading)
		{
			UnityEngine.Time.timeScale = timeScale;
		}
		Bootstrap.WriteToLog("Server startup complete");
		yield break;
		yield break;
	}

	// Token: 0x06002715 RID: 10005 RVA: 0x000F126E File Offset: 0x000EF46E
	private void StartupShared()
	{
		ItemManager.Initialize();
	}

	// Token: 0x06002716 RID: 10006 RVA: 0x000F1275 File Offset: 0x000EF475
	public void ThrowError(string error)
	{
		Debug.Log("ThrowError: " + error);
		this.errorPanel.SetActive(true);
		this.errorText.text = error;
		Bootstrap.isErrored = true;
	}

	// Token: 0x06002717 RID: 10007 RVA: 0x000F12A5 File Offset: 0x000EF4A5
	public void ExitGame()
	{
		Debug.Log("Exiting due to Exit Game button on bootstrap error panel");
		Rust.Application.Quit();
	}

	// Token: 0x06002718 RID: 10008 RVA: 0x000F12B6 File Offset: 0x000EF4B6
	public static IEnumerator LoadingUpdate(string str)
	{
		if (!SingletonComponent<Bootstrap>.Instance)
		{
			yield break;
		}
		SingletonComponent<Bootstrap>.Instance.messageString = str;
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		yield break;
	}

	// Token: 0x06002719 RID: 10009 RVA: 0x000F12C5 File Offset: 0x000EF4C5
	public static void WriteToLog(string str)
	{
		if (Bootstrap.lastWrittenValue == str)
		{
			return;
		}
		DebugEx.Log(str, StackTraceLogType.None);
		Bootstrap.lastWrittenValue = str;
	}

	// Token: 0x04001F7A RID: 8058
	internal static bool bootstrapInitRun;

	// Token: 0x04001F7B RID: 8059
	public static bool isErrored;

	// Token: 0x04001F7C RID: 8060
	public string messageString = "Loading...";

	// Token: 0x04001F7D RID: 8061
	public CanvasGroup BootstrapUiCanvas;

	// Token: 0x04001F7E RID: 8062
	public GameObject errorPanel;

	// Token: 0x04001F7F RID: 8063
	public TextMeshProUGUI errorText;

	// Token: 0x04001F80 RID: 8064
	public TextMeshProUGUI statusText;

	// Token: 0x04001F81 RID: 8065
	private static string lastWrittenValue;
}
