using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Facepunch.Models;
using Newtonsoft.Json.Linq;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000377 RID: 887
public class BoomBox : EntityComponent<global::BaseEntity>, INotifyLOD
{
	// Token: 0x06001F1B RID: 7963 RVA: 0x000CEE7C File Offset: 0x000CD07C
	[ServerVar]
	public static void ClearRadioByUser(ConsoleSystem.Arg arg)
	{
		ulong @uint = arg.GetUInt64(0, 0UL);
		int num = 0;
		foreach (global::BaseNetworkable baseNetworkable in global::BaseNetworkable.serverEntities)
		{
			DeployableBoomBox deployableBoomBox;
			HeldBoomBox heldBoomBox;
			if ((deployableBoomBox = (baseNetworkable as DeployableBoomBox)) != null)
			{
				if (deployableBoomBox.ClearRadioByUserId(@uint))
				{
					num++;
				}
			}
			else if ((heldBoomBox = (baseNetworkable as HeldBoomBox)) != null && heldBoomBox.ClearRadioByUserId(@uint))
			{
				num++;
			}
		}
		arg.ReplyWith(string.Format("Stopped and cleared saved URL of {0} boom boxes", num));
	}

	// Token: 0x06001F1C RID: 7964 RVA: 0x000CEF18 File Offset: 0x000CD118
	public static void LoadStations()
	{
		if (global::BoomBox.ValidStations != null)
		{
			return;
		}
		global::BoomBox.ValidStations = (global::BoomBox.GetStationData() ?? new Dictionary<string, string>());
		global::BoomBox.ParseServerUrlList();
	}

	// Token: 0x06001F1D RID: 7965 RVA: 0x000CEF3C File Offset: 0x000CD13C
	private static Dictionary<string, string> GetStationData()
	{
		Facepunch.Models.Manifest manifest = Facepunch.Application.Manifest;
		JObject jobject = (manifest != null) ? manifest.Metadata : null;
		JArray jarray;
		if ((jarray = (((jobject != null) ? jobject["RadioStations"] : null) as JArray)) != null && jarray.Count > 0)
		{
			string[] array = new string[2];
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (string text in jarray.Values<string>())
			{
				array = text.Split(new char[]
				{
					','
				});
				if (!dictionary.ContainsKey(array[0]))
				{
					dictionary.Add(array[0], array[1]);
				}
			}
			return dictionary;
		}
		return null;
	}

	// Token: 0x06001F1E RID: 7966 RVA: 0x000CEFF0 File Offset: 0x000CD1F0
	private static bool IsStationValid(string url)
	{
		global::BoomBox.ParseServerUrlList();
		return (global::BoomBox.ValidStations != null && global::BoomBox.ValidStations.ContainsValue(url)) || (global::BoomBox.ServerValidStations != null && global::BoomBox.ServerValidStations.ContainsValue(url));
	}

	// Token: 0x06001F1F RID: 7967 RVA: 0x000CF024 File Offset: 0x000CD224
	public static void ParseServerUrlList()
	{
		if (global::BoomBox.ServerValidStations == null)
		{
			global::BoomBox.ServerValidStations = new Dictionary<string, string>();
		}
		if (global::BoomBox.lastParsedServerList == global::BoomBox.ServerUrlList)
		{
			return;
		}
		global::BoomBox.ServerValidStations.Clear();
		if (!string.IsNullOrEmpty(global::BoomBox.ServerUrlList))
		{
			string[] array = global::BoomBox.ServerUrlList.Split(new char[]
			{
				','
			});
			if (array.Length % 2 != 0)
			{
				Debug.Log("Invalid number of stations in BoomBox.ServerUrlList, ensure you always have a name and a url");
				return;
			}
			for (int i = 0; i < array.Length; i += 2)
			{
				if (global::BoomBox.ServerValidStations.ContainsKey(array[i]))
				{
					Debug.Log("Duplicate station name detected in BoomBox.ServerUrlList, all station names must be unique: " + array[i]);
				}
				else
				{
					global::BoomBox.ServerValidStations.Add(array[i], array[i + 1]);
				}
			}
		}
		global::BoomBox.lastParsedServerList = global::BoomBox.ServerUrlList;
	}

	// Token: 0x1700025D RID: 605
	// (get) Token: 0x06001F20 RID: 7968 RVA: 0x000CF0DF File Offset: 0x000CD2DF
	// (set) Token: 0x06001F21 RID: 7969 RVA: 0x000CF0E7 File Offset: 0x000CD2E7
	public string CurrentRadioIp { get; private set; } = "rustradio.facepunch.com";

	// Token: 0x06001F22 RID: 7970 RVA: 0x000CF0F0 File Offset: 0x000CD2F0
	public void Server_UpdateRadioIP(global::BaseEntity.RPCMessage msg)
	{
		string text = msg.read.String(256);
		if (global::BoomBox.IsStationValid(text))
		{
			if (msg.player != null)
			{
				ulong userID = msg.player.userID;
				this.AssignedRadioBy = userID;
			}
			this.CurrentRadioIp = text;
			base.baseEntity.ClientRPC<string>(null, "OnRadioIPChanged", this.CurrentRadioIp);
			if (this.IsOn())
			{
				this.ServerTogglePlay(false);
			}
		}
	}

	// Token: 0x06001F23 RID: 7971 RVA: 0x000CF164 File Offset: 0x000CD364
	public void Save(global::BaseNetworkable.SaveInfo info)
	{
		if (info.msg.boomBox == null)
		{
			info.msg.boomBox = Facepunch.Pool.Get<ProtoBuf.BoomBox>();
		}
		info.msg.boomBox.radioIp = this.CurrentRadioIp;
		info.msg.boomBox.assignedRadioBy = this.AssignedRadioBy;
	}

	// Token: 0x06001F24 RID: 7972 RVA: 0x000CF1BA File Offset: 0x000CD3BA
	public bool ClearRadioByUserId(ulong id)
	{
		if (this.AssignedRadioBy == id)
		{
			this.CurrentRadioIp = string.Empty;
			this.AssignedRadioBy = 0UL;
			if (this.HasFlag(global::BaseEntity.Flags.On))
			{
				this.ServerTogglePlay(false);
			}
			return true;
		}
		return false;
	}

	// Token: 0x06001F25 RID: 7973 RVA: 0x000CF1EB File Offset: 0x000CD3EB
	public void Load(global::BaseNetworkable.LoadInfo info)
	{
		if (info.msg.boomBox != null)
		{
			this.CurrentRadioIp = info.msg.boomBox.radioIp;
			this.AssignedRadioBy = info.msg.boomBox.assignedRadioBy;
		}
	}

	// Token: 0x1700025E RID: 606
	// (get) Token: 0x06001F26 RID: 7974 RVA: 0x000CF226 File Offset: 0x000CD426
	public global::BaseEntity BaseEntity
	{
		get
		{
			return base.baseEntity;
		}
	}

	// Token: 0x06001F27 RID: 7975 RVA: 0x000CF230 File Offset: 0x000CD430
	public void ServerTogglePlay(global::BaseEntity.RPCMessage msg)
	{
		if (!this.IsPowered())
		{
			return;
		}
		bool play = msg.read.ReadByte() == 1;
		this.ServerTogglePlay(play);
	}

	// Token: 0x06001F28 RID: 7976 RVA: 0x000CF25C File Offset: 0x000CD45C
	private void DeductCondition()
	{
		Action<float> hurtCallback = this.HurtCallback;
		if (hurtCallback == null)
		{
			return;
		}
		hurtCallback(this.ConditionLossRate * ConVar.Decay.scale);
	}

	// Token: 0x06001F29 RID: 7977 RVA: 0x000CF27C File Offset: 0x000CD47C
	public void ServerTogglePlay(bool play)
	{
		if (base.baseEntity == null)
		{
			return;
		}
		this.SetFlag(global::BaseEntity.Flags.On, play);
		global::IOEntity ioentity;
		if ((ioentity = (base.baseEntity as global::IOEntity)) != null)
		{
			ioentity.SendChangedToRoot(true);
			ioentity.MarkDirtyForceUpdateOutputs();
		}
		if (play && !base.IsInvoking(new Action(this.DeductCondition)) && this.ConditionLossRate > 0f)
		{
			base.InvokeRepeating(new Action(this.DeductCondition), 1f, 1f);
			return;
		}
		if (base.IsInvoking(new Action(this.DeductCondition)))
		{
			base.CancelInvoke(new Action(this.DeductCondition));
		}
	}

	// Token: 0x06001F2A RID: 7978 RVA: 0x000CF324 File Offset: 0x000CD524
	public void OnCassetteInserted(global::Cassette c)
	{
		if (base.baseEntity == null)
		{
			return;
		}
		base.baseEntity.ClientRPC<uint>(null, "Client_OnCassetteInserted", c.net.ID);
		this.ServerTogglePlay(false);
		this.SetFlag(global::BaseEntity.Flags.Reserved1, true);
		base.baseEntity.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001F2B RID: 7979 RVA: 0x000CF37B File Offset: 0x000CD57B
	public void OnCassetteRemoved(global::Cassette c)
	{
		if (base.baseEntity == null)
		{
			return;
		}
		base.baseEntity.ClientRPC(null, "Client_OnCassetteRemoved");
		this.ServerTogglePlay(false);
		this.SetFlag(global::BaseEntity.Flags.Reserved1, false);
	}

	// Token: 0x06001F2C RID: 7980 RVA: 0x000CF3B0 File Offset: 0x000CD5B0
	private bool IsPowered()
	{
		return !(base.baseEntity == null) && (base.baseEntity.HasFlag(global::BaseEntity.Flags.Reserved8) || base.baseEntity is HeldBoomBox);
	}

	// Token: 0x06001F2D RID: 7981 RVA: 0x000CF3E4 File Offset: 0x000CD5E4
	private bool IsOn()
	{
		return !(base.baseEntity == null) && base.baseEntity.IsOn();
	}

	// Token: 0x06001F2E RID: 7982 RVA: 0x000CF401 File Offset: 0x000CD601
	private bool HasFlag(global::BaseEntity.Flags f)
	{
		return !(base.baseEntity == null) && base.baseEntity.HasFlag(f);
	}

	// Token: 0x06001F2F RID: 7983 RVA: 0x000CF41F File Offset: 0x000CD61F
	private void SetFlag(global::BaseEntity.Flags f, bool state)
	{
		if (base.baseEntity != null)
		{
			base.baseEntity.SetFlag(f, state, false, true);
		}
	}

	// Token: 0x1700025F RID: 607
	// (get) Token: 0x06001F30 RID: 7984 RVA: 0x000CF43E File Offset: 0x000CD63E
	private bool isClient
	{
		get
		{
			return base.baseEntity != null && base.baseEntity.isClient;
		}
	}

	// Token: 0x0400187E RID: 6270
	public static Dictionary<string, string> ValidStations;

	// Token: 0x0400187F RID: 6271
	public static Dictionary<string, string> ServerValidStations;

	// Token: 0x04001880 RID: 6272
	[ReplicatedVar(Saved = true, Help = "A list of radio stations that are valid on this server. Format: NAME,URL,NAME,URL,etc", ShowInAdminUI = true)]
	public static string ServerUrlList = string.Empty;

	// Token: 0x04001881 RID: 6273
	private static string lastParsedServerList;

	// Token: 0x04001882 RID: 6274
	public ShoutcastStreamer ShoutcastStreamer;

	// Token: 0x04001883 RID: 6275
	public GameObjectRef RadioIpDialog;

	// Token: 0x04001885 RID: 6277
	public ulong AssignedRadioBy;

	// Token: 0x04001886 RID: 6278
	public AudioSource SoundSource;

	// Token: 0x04001887 RID: 6279
	public float ConditionLossRate = 0.25f;

	// Token: 0x04001888 RID: 6280
	public ItemDefinition[] ValidCassettes;

	// Token: 0x04001889 RID: 6281
	public SoundDefinition PlaySfx;

	// Token: 0x0400188A RID: 6282
	public SoundDefinition StopSfx;

	// Token: 0x0400188B RID: 6283
	public const global::BaseEntity.Flags HasCassette = global::BaseEntity.Flags.Reserved1;

	// Token: 0x0400188C RID: 6284
	[ServerVar(Saved = true)]
	public static int BacktrackLength = 30;

	// Token: 0x0400188D RID: 6285
	public Action<float> HurtCallback;
}
