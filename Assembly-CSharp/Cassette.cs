using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000051 RID: 81
public class Cassette : global::BaseEntity, IUGCBrowserEntity
{
	// Token: 0x060008FB RID: 2299 RVA: 0x00054C04 File Offset: 0x00052E04
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Cassette.OnRpcMessage", 0))
		{
			if (rpc == 4031457637U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_MakeNewFile ");
				}
				using (TimeWarning.New("Server_MakeNewFile", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(4031457637U, "Server_MakeNewFile", this, player, 1UL))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_MakeNewFile(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Server_MakeNewFile");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060008FC RID: 2300 RVA: 0x00054D6C File Offset: 0x00052F6C
	[ServerVar]
	public static void ClearCassettes(ConsoleSystem.Arg arg)
	{
		int num = 0;
		using (IEnumerator<global::BaseNetworkable> enumerator = global::BaseNetworkable.serverEntities.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				global::Cassette cassette;
				if ((cassette = (enumerator.Current as global::Cassette)) != null && cassette.ClearSavedAudio())
				{
					num++;
				}
			}
		}
		arg.ReplyWith(string.Format("Deleted the contents of {0} cassettes", num));
	}

	// Token: 0x060008FD RID: 2301 RVA: 0x00054DE0 File Offset: 0x00052FE0
	[ServerVar]
	public static void ClearCassettesByUser(ConsoleSystem.Arg arg)
	{
		ulong @uint = arg.GetUInt64(0, 0UL);
		int num = 0;
		using (IEnumerator<global::BaseNetworkable> enumerator = global::BaseNetworkable.serverEntities.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				global::Cassette cassette;
				if ((cassette = (enumerator.Current as global::Cassette)) != null && cassette.CreatorSteamId == @uint)
				{
					cassette.ClearSavedAudio();
					num++;
				}
			}
		}
		arg.ReplyWith(string.Format("Deleted {0} cassettes recorded by {1}", num, @uint));
	}

	// Token: 0x170000D9 RID: 217
	// (get) Token: 0x060008FE RID: 2302 RVA: 0x00054E6C File Offset: 0x0005306C
	// (set) Token: 0x060008FF RID: 2303 RVA: 0x00054E74 File Offset: 0x00053074
	public uint AudioId { get; private set; }

	// Token: 0x170000DA RID: 218
	// (get) Token: 0x06000900 RID: 2304 RVA: 0x00054E7D File Offset: 0x0005307D
	public SoundDefinition PreloadedAudio
	{
		get
		{
			return this.PreloadContent.GetSoundContent(this.preloadedAudioId, this.PreloadType);
		}
	}

	// Token: 0x06000901 RID: 2305 RVA: 0x00054E98 File Offset: 0x00053098
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.cassette != null)
		{
			uint audioId = this.AudioId;
			this.AudioId = info.msg.cassette.audioId;
			this.CreatorSteamId = info.msg.cassette.creatorSteamId;
			this.preloadedAudioId = info.msg.cassette.preloadAudioId;
			if (base.isServer && info.msg.cassette.holder != 0U)
			{
				global::BaseNetworkable baseNetworkable = global::BaseNetworkable.serverEntities.Find(info.msg.cassette.holder);
				ICassettePlayer cassettePlayer;
				if (baseNetworkable != null && (cassettePlayer = (baseNetworkable as ICassettePlayer)) != null)
				{
					this.currentCassettePlayer = cassettePlayer;
				}
			}
		}
	}

	// Token: 0x06000902 RID: 2306 RVA: 0x00054F54 File Offset: 0x00053154
	public void AssignPreloadContent()
	{
		switch (this.PreloadType)
		{
		case PreloadedCassetteContent.PreloadType.Short:
			this.preloadedAudioId = UnityEngine.Random.Range(0, this.PreloadContent.ShortTapeContent.Length);
			return;
		case PreloadedCassetteContent.PreloadType.Medium:
			this.preloadedAudioId = UnityEngine.Random.Range(0, this.PreloadContent.MediumTapeContent.Length);
			return;
		case PreloadedCassetteContent.PreloadType.Long:
			this.preloadedAudioId = UnityEngine.Random.Range(0, this.PreloadContent.LongTapeContent.Length);
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x06000903 RID: 2307 RVA: 0x00054FD0 File Offset: 0x000531D0
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.cassette = Facepunch.Pool.Get<ProtoBuf.Cassette>();
		info.msg.cassette.audioId = this.AudioId;
		info.msg.cassette.creatorSteamId = this.CreatorSteamId;
		info.msg.cassette.preloadAudioId = this.preloadedAudioId;
		if (!this.currentCassettePlayer.IsUnityNull<ICassettePlayer>() && this.currentCassettePlayer.ToBaseEntity.IsValid())
		{
			info.msg.cassette.holder = this.currentCassettePlayer.ToBaseEntity.net.ID;
		}
	}

	// Token: 0x06000904 RID: 2308 RVA: 0x0005507C File Offset: 0x0005327C
	public override void OnParentChanging(global::BaseEntity oldParent, global::BaseEntity newParent)
	{
		base.OnParentChanging(oldParent, newParent);
		ICassettePlayer cassettePlayer = this.currentCassettePlayer;
		if (cassettePlayer != null)
		{
			cassettePlayer.OnCassetteRemoved(this);
		}
		this.currentCassettePlayer = null;
		ICassettePlayer cassettePlayer2;
		if (newParent != null && (cassettePlayer2 = (newParent as ICassettePlayer)) != null)
		{
			base.Invoke(new Action(this.DelayedCassetteInserted), 0.1f);
			this.currentCassettePlayer = cassettePlayer2;
		}
	}

	// Token: 0x06000905 RID: 2309 RVA: 0x000550DB File Offset: 0x000532DB
	private void DelayedCassetteInserted()
	{
		if (this.currentCassettePlayer != null)
		{
			this.currentCassettePlayer.OnCassetteInserted(this);
		}
	}

	// Token: 0x06000906 RID: 2310 RVA: 0x000550F1 File Offset: 0x000532F1
	public void SetAudioId(uint id, ulong userId)
	{
		this.AudioId = id;
		this.CreatorSteamId = userId;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000907 RID: 2311 RVA: 0x00055108 File Offset: 0x00053308
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	public void Server_MakeNewFile(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player == null)
		{
			return;
		}
		global::HeldEntity heldEntity;
		if (base.GetParentEntity() != null && (heldEntity = (base.GetParentEntity() as global::HeldEntity)) != null && heldEntity.GetOwnerPlayer() != msg.player)
		{
			Debug.Log("Player mismatch!");
			return;
		}
		byte[] data = msg.read.BytesWithSize(10485760U);
		ulong userId = msg.read.UInt64();
		if (!global::Cassette.IsOggValid(data, this))
		{
			return;
		}
		FileStorage.server.RemoveAllByEntity(this.net.ID);
		uint id = FileStorage.server.Store(data, FileStorage.Type.ogg, this.net.ID, 0U);
		this.SetAudioId(id, userId);
	}

	// Token: 0x06000908 RID: 2312 RVA: 0x000551BC File Offset: 0x000533BC
	private bool ClearSavedAudio()
	{
		if (this.AudioId == 0U)
		{
			return false;
		}
		FileStorage.server.RemoveAllByEntity(this.net.ID);
		this.AudioId = 0U;
		this.CreatorSteamId = 0UL;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		return true;
	}

	// Token: 0x06000909 RID: 2313 RVA: 0x000551F4 File Offset: 0x000533F4
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		this.ClearSavedAudio();
	}

	// Token: 0x170000DB RID: 219
	// (get) Token: 0x0600090A RID: 2314 RVA: 0x00055203 File Offset: 0x00053403
	public uint[] GetContentCRCs
	{
		get
		{
			if (this.AudioId <= 0U)
			{
				return Array.Empty<uint>();
			}
			return new uint[]
			{
				this.AudioId
			};
		}
	}

	// Token: 0x0600090B RID: 2315 RVA: 0x00055223 File Offset: 0x00053423
	public void ClearContent()
	{
		this.AudioId = 0U;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x170000DC RID: 220
	// (get) Token: 0x0600090C RID: 2316 RVA: 0x0004AF67 File Offset: 0x00049167
	public UGCType ContentType
	{
		get
		{
			return UGCType.AudioOgg;
		}
	}

	// Token: 0x170000DD RID: 221
	// (get) Token: 0x0600090D RID: 2317 RVA: 0x00055233 File Offset: 0x00053433
	public List<ulong> EditingHistory
	{
		get
		{
			return new List<ulong>
			{
				this.CreatorSteamId
			};
		}
	}

	// Token: 0x170000DE RID: 222
	// (get) Token: 0x0600090E RID: 2318 RVA: 0x00002E37 File Offset: 0x00001037
	public global::BaseNetworkable UgcEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x0600090F RID: 2319 RVA: 0x00055246 File Offset: 0x00053446
	public static bool IsOggValid(byte[] data, global::Cassette c)
	{
		return global::Cassette.IsOggValid(data, c.MaxCassetteLength);
	}

	// Token: 0x06000910 RID: 2320 RVA: 0x00055254 File Offset: 0x00053454
	private static bool IsOggValid(byte[] data, float maxLength)
	{
		if (data == null)
		{
			return false;
		}
		if (global::Cassette.ByteToMegabyte(data.Length) >= global::Cassette.MaxCassetteFileSizeMB)
		{
			Debug.Log("Audio file is too large! Aborting");
			return false;
		}
		double oggLength = global::Cassette.GetOggLength(data);
		if (oggLength > (double)(maxLength * 1.2f))
		{
			Debug.Log(string.Format("Audio duration is longer than cassette limit! {0} > {1}", oggLength, maxLength * 1.2f));
			return false;
		}
		return true;
	}

	// Token: 0x06000911 RID: 2321 RVA: 0x000552B7 File Offset: 0x000534B7
	private static float ByteToMegabyte(int byteSize)
	{
		return (float)byteSize / 1024f / 1024f;
	}

	// Token: 0x06000912 RID: 2322 RVA: 0x000552C8 File Offset: 0x000534C8
	private static double GetOggLength(byte[] t)
	{
		int num = t.Length;
		long num2 = -1L;
		int num3 = -1;
		for (int i = num - 1 - 8 - 2 - 4; i >= 0; i--)
		{
			if (t[i] == 79 && t[i + 1] == 103 && t[i + 2] == 103 && t[i + 3] == 83)
			{
				num2 = BitConverter.ToInt64(new byte[]
				{
					t[i + 6],
					t[i + 7],
					t[i + 8],
					t[i + 9],
					t[i + 10],
					t[i + 11],
					t[i + 12],
					t[i + 13]
				}, 0);
				break;
			}
		}
		for (int j = 0; j < num - 8 - 2 - 4; j++)
		{
			if (t[j] == 118 && t[j + 1] == 111 && t[j + 2] == 114 && t[j + 3] == 98 && t[j + 4] == 105 && t[j + 5] == 115)
			{
				num3 = BitConverter.ToInt32(new byte[]
				{
					t[j + 11],
					t[j + 12],
					t[j + 13],
					t[j + 14]
				}, 0);
				break;
			}
		}
		if (RecorderTool.debugRecording)
		{
			Debug.Log(string.Format("{0} / {1}", num2, num3));
		}
		return (double)num2 / (double)num3;
	}

	// Token: 0x040005EA RID: 1514
	public float MaxCassetteLength = 15f;

	// Token: 0x040005EB RID: 1515
	[ReplicatedVar]
	public static float MaxCassetteFileSizeMB = 5f;

	// Token: 0x040005ED RID: 1517
	public ulong CreatorSteamId;

	// Token: 0x040005EE RID: 1518
	public PreloadedCassetteContent.PreloadType PreloadType;

	// Token: 0x040005EF RID: 1519
	public PreloadedCassetteContent PreloadContent;

	// Token: 0x040005F0 RID: 1520
	public SoundDefinition InsertCassetteSfx;

	// Token: 0x040005F1 RID: 1521
	public int ViewmodelIndex;

	// Token: 0x040005F2 RID: 1522
	public Sprite HudSprite;

	// Token: 0x040005F3 RID: 1523
	public int MaximumVoicemailSlots = 1;

	// Token: 0x040005F4 RID: 1524
	private int preloadedAudioId;

	// Token: 0x040005F5 RID: 1525
	private ICassettePlayer currentCassettePlayer;
}
