using System;
using ConVar;
using Facepunch;
using Facepunch.Extend;
using Network;
using UnityEngine;
using Windows;

// Token: 0x020002F4 RID: 756
public class ServerConsole : SingletonComponent<ServerConsole>
{
	// Token: 0x06001D73 RID: 7539 RVA: 0x000C9528 File Offset: 0x000C7728
	public void OnEnable()
	{
		this.console.Initialize();
		this.input.OnInputText += this.OnInputText;
		Output.OnMessage += this.HandleLog;
		this.input.ClearLine(System.Console.WindowHeight);
		for (int i = 0; i < System.Console.WindowHeight; i++)
		{
			System.Console.WriteLine("");
		}
	}

	// Token: 0x06001D74 RID: 7540 RVA: 0x000C9592 File Offset: 0x000C7792
	private void OnDisable()
	{
		Output.OnMessage -= this.HandleLog;
		this.input.OnInputText -= this.OnInputText;
		this.console.Shutdown();
	}

	// Token: 0x06001D75 RID: 7541 RVA: 0x000C95C7 File Offset: 0x000C77C7
	private void OnInputText(string obj)
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Server, obj, Array.Empty<object>());
	}

	// Token: 0x06001D76 RID: 7542 RVA: 0x000C95DC File Offset: 0x000C77DC
	public static void PrintColoured(params object[] objects)
	{
		if (SingletonComponent<ServerConsole>.Instance == null)
		{
			return;
		}
		SingletonComponent<ServerConsole>.Instance.input.ClearLine(SingletonComponent<ServerConsole>.Instance.input.statusText.Length);
		for (int i = 0; i < objects.Length; i++)
		{
			if (i % 2 == 0)
			{
				System.Console.ForegroundColor = (ConsoleColor)objects[i];
			}
			else
			{
				System.Console.Write((string)objects[i]);
			}
		}
		if (System.Console.CursorLeft != 0)
		{
			System.Console.CursorTop++;
		}
		SingletonComponent<ServerConsole>.Instance.input.RedrawInputLine();
	}

	// Token: 0x06001D77 RID: 7543 RVA: 0x000C9668 File Offset: 0x000C7868
	private void HandleLog(string message, string stackTrace, LogType type)
	{
		if (message.StartsWith("[CHAT]") || message.StartsWith("[TEAM CHAT]") || message.StartsWith("[CARDS CHAT]"))
		{
			return;
		}
		if (type == LogType.Warning)
		{
			if (message.StartsWith("HDR RenderTexture format is not") || message.StartsWith("The image effect") || message.StartsWith("Image Effects are not supported on this platform") || message.StartsWith("[AmplifyColor]") || message.StartsWith("Skipping profile frame.") || message.StartsWith("Kinematic body only supports Speculative Continuous collision detection"))
			{
				return;
			}
			System.Console.ForegroundColor = ConsoleColor.Yellow;
		}
		else if (type == LogType.Error)
		{
			System.Console.ForegroundColor = ConsoleColor.Red;
		}
		else if (type == LogType.Exception)
		{
			System.Console.ForegroundColor = ConsoleColor.Red;
		}
		else if (type == LogType.Assert)
		{
			System.Console.ForegroundColor = ConsoleColor.Red;
		}
		else
		{
			System.Console.ForegroundColor = ConsoleColor.Gray;
		}
		this.input.ClearLine(this.input.statusText.Length);
		System.Console.WriteLine(message);
		this.input.RedrawInputLine();
	}

	// Token: 0x06001D78 RID: 7544 RVA: 0x000C974E File Offset: 0x000C794E
	private void Update()
	{
		this.UpdateStatus();
		this.input.Update();
	}

	// Token: 0x06001D79 RID: 7545 RVA: 0x000C9764 File Offset: 0x000C7964
	private void UpdateStatus()
	{
		if (this.nextUpdate > UnityEngine.Time.realtimeSinceStartup)
		{
			return;
		}
		if (Network.Net.sv == null || !Network.Net.sv.IsConnected())
		{
			return;
		}
		this.nextUpdate = UnityEngine.Time.realtimeSinceStartup + 0.33f;
		if (!this.input.valid)
		{
			return;
		}
		string text = ((long)UnityEngine.Time.realtimeSinceStartup).FormatSeconds();
		string text2 = this.currentGameTime.ToString("[H:mm]");
		string text3 = string.Concat(new object[]
		{
			" ",
			text2,
			" [",
			this.currentPlayerCount,
			"/",
			this.maxPlayerCount,
			"] ",
			ConVar.Server.hostname,
			" [",
			ConVar.Server.level,
			"]"
		});
		string text4 = string.Concat(new object[]
		{
			global::Performance.current.frameRate,
			"fps ",
			global::Performance.current.memoryCollections,
			"gc ",
			text
		}) ?? "";
		string text5 = Network.Net.sv.GetStat(null, BaseNetwork.StatTypeLong.BytesReceived_LastSecond).FormatBytes(true) + "/s in, " + Network.Net.sv.GetStat(null, BaseNetwork.StatTypeLong.BytesSent_LastSecond).FormatBytes(true) + "/s out";
		string text6 = text4.PadLeft(this.input.lineWidth - 1);
		text6 = text3 + ((text3.Length < text6.Length) ? text6.Substring(text3.Length) : "");
		string text7 = string.Concat(new string[]
		{
			" ",
			this.currentEntityCount.ToString("n0"),
			" ents, ",
			this.currentSleeperCount.ToString("n0"),
			" slprs"
		});
		string text8 = text5.PadLeft(this.input.lineWidth - 1);
		text8 = text7 + ((text7.Length < text8.Length) ? text8.Substring(text7.Length) : "");
		this.input.statusText[0] = "";
		this.input.statusText[1] = text6;
		this.input.statusText[2] = text8;
	}

	// Token: 0x1700022E RID: 558
	// (get) Token: 0x06001D7A RID: 7546 RVA: 0x000C99C7 File Offset: 0x000C7BC7
	private DateTime currentGameTime
	{
		get
		{
			if (!TOD_Sky.Instance)
			{
				return DateTime.Now;
			}
			return TOD_Sky.Instance.Cycle.DateTime;
		}
	}

	// Token: 0x1700022F RID: 559
	// (get) Token: 0x06001D7B RID: 7547 RVA: 0x000C99EA File Offset: 0x000C7BEA
	private int currentPlayerCount
	{
		get
		{
			return BasePlayer.activePlayerList.Count;
		}
	}

	// Token: 0x17000230 RID: 560
	// (get) Token: 0x06001D7C RID: 7548 RVA: 0x000C99F6 File Offset: 0x000C7BF6
	private int maxPlayerCount
	{
		get
		{
			return ConVar.Server.maxplayers;
		}
	}

	// Token: 0x17000231 RID: 561
	// (get) Token: 0x06001D7D RID: 7549 RVA: 0x000C99FD File Offset: 0x000C7BFD
	private int currentEntityCount
	{
		get
		{
			return BaseNetworkable.serverEntities.Count;
		}
	}

	// Token: 0x17000232 RID: 562
	// (get) Token: 0x06001D7E RID: 7550 RVA: 0x000C9A09 File Offset: 0x000C7C09
	private int currentSleeperCount
	{
		get
		{
			return BasePlayer.sleepingPlayerList.Count;
		}
	}

	// Token: 0x040016DB RID: 5851
	private ConsoleWindow console = new ConsoleWindow();

	// Token: 0x040016DC RID: 5852
	private ConsoleInput input = new ConsoleInput();

	// Token: 0x040016DD RID: 5853
	private float nextUpdate;
}
