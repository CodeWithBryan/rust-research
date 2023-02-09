using System;
using System.Collections.Generic;
using System.Diagnostics;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000050 RID: 80
public class CarvablePumpkin : global::BaseOven, ILOD, ISignage, IUGCBrowserEntity
{
	// Token: 0x060008DD RID: 2269 RVA: 0x00053FA0 File Offset: 0x000521A0
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CarvablePumpkin.OnRpcMessage", 0))
		{
			if (rpc == 1455609404U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					UnityEngine.Debug.Log("SV_RPCMessage: " + player + " - LockSign ");
				}
				using (TimeWarning.New("LockSign", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1455609404U, "LockSign", this, player, 3f))
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
							this.LockSign(msg2);
						}
					}
					catch (Exception exception)
					{
						UnityEngine.Debug.LogException(exception);
						player.Kick("RPC Error in LockSign");
					}
				}
				return true;
			}
			if (rpc == 4149904254U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					UnityEngine.Debug.Log("SV_RPCMessage: " + player + " - UnLockSign ");
				}
				using (TimeWarning.New("UnLockSign", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(4149904254U, "UnLockSign", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.UnLockSign(msg3);
						}
					}
					catch (Exception exception2)
					{
						UnityEngine.Debug.LogException(exception2);
						player.Kick("RPC Error in UnLockSign");
					}
				}
				return true;
			}
			if (rpc == 1255380462U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					UnityEngine.Debug.Log("SV_RPCMessage: " + player + " - UpdateSign ");
				}
				using (TimeWarning.New("UpdateSign", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(1255380462U, "UpdateSign", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1255380462U, "UpdateSign", this, player, 5f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg4 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.UpdateSign(msg4);
						}
					}
					catch (Exception exception3)
					{
						UnityEngine.Debug.LogException(exception3);
						player.Kick("RPC Error in UpdateSign");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170000D1 RID: 209
	// (get) Token: 0x060008DE RID: 2270 RVA: 0x00054418 File Offset: 0x00052618
	public Vector2i TextureSize
	{
		get
		{
			if (this.paintableSources == null || this.paintableSources.Length == 0)
			{
				return Vector2i.zero;
			}
			MeshPaintableSource meshPaintableSource = this.paintableSources[0];
			return new Vector2i(meshPaintableSource.texWidth, meshPaintableSource.texHeight);
		}
	}

	// Token: 0x170000D2 RID: 210
	// (get) Token: 0x060008DF RID: 2271 RVA: 0x00054456 File Offset: 0x00052656
	public int TextureCount
	{
		get
		{
			MeshPaintableSource[] array = this.paintableSources;
			if (array == null)
			{
				return 0;
			}
			return array.Length;
		}
	}

	// Token: 0x060008E0 RID: 2272 RVA: 0x00054468 File Offset: 0x00052668
	public override void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(preProcess, rootObj, name, serverside, clientside, bundling);
		if (this.paintableSources != null && this.paintableSources.Length > 1)
		{
			MeshPaintableSource meshPaintableSource = this.paintableSources[0];
			for (int i = 1; i < this.paintableSources.Length; i++)
			{
				MeshPaintableSource meshPaintableSource2 = this.paintableSources[i];
				meshPaintableSource2.texWidth = meshPaintableSource.texWidth;
				meshPaintableSource2.texHeight = meshPaintableSource.texHeight;
			}
		}
	}

	// Token: 0x060008E1 RID: 2273 RVA: 0x000544D4 File Offset: 0x000526D4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	[global::BaseEntity.RPC_Server.MaxDistance(5f)]
	public void UpdateSign(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player == null)
		{
			return;
		}
		if (!this.CanUpdateSign(msg.player))
		{
			return;
		}
		int num = msg.read.Int32();
		if (num < 0 || num >= this.paintableSources.Length)
		{
			return;
		}
		byte[] array = msg.read.BytesWithSize(10485760U);
		if (msg.read.Unread > 0 && msg.read.Bit() && !msg.player.IsAdmin)
		{
			UnityEngine.Debug.LogWarning(string.Format("{0} tried to upload a sign from a file but they aren't admin, ignoring", msg.player));
			return;
		}
		this.EnsureInitialized();
		if (array == null)
		{
			if (this.textureIDs[num] != 0U)
			{
				FileStorage.server.RemoveExact(this.textureIDs[num], FileStorage.Type.png, this.net.ID, (uint)num);
			}
			this.textureIDs[num] = 0U;
		}
		else
		{
			if (!ImageProcessing.IsValidPNG(array, 1024, 1024))
			{
				return;
			}
			if (this.textureIDs[num] != 0U)
			{
				FileStorage.server.RemoveExact(this.textureIDs[num], FileStorage.Type.png, this.net.ID, (uint)num);
			}
			this.textureIDs[num] = FileStorage.server.Store(array, FileStorage.Type.png, this.net.ID, (uint)num);
		}
		this.LogEdit(msg.player);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060008E2 RID: 2274 RVA: 0x0005461C File Offset: 0x0005281C
	private void EnsureInitialized()
	{
		int num = Mathf.Max(this.paintableSources.Length, 1);
		if (this.textureIDs == null || this.textureIDs.Length != num)
		{
			Array.Resize<uint>(ref this.textureIDs, num);
		}
	}

	// Token: 0x060008E3 RID: 2275 RVA: 0x00054657 File Offset: 0x00052857
	[Conditional("SIGN_DEBUG")]
	private static void SignDebugLog(string str)
	{
		UnityEngine.Debug.Log(str);
	}

	// Token: 0x170000D3 RID: 211
	// (get) Token: 0x060008E4 RID: 2276 RVA: 0x00007074 File Offset: 0x00005274
	public FileStorage.Type FileType
	{
		get
		{
			return FileStorage.Type.png;
		}
	}

	// Token: 0x060008E5 RID: 2277 RVA: 0x0005465F File Offset: 0x0005285F
	public uint[] GetTextureCRCs()
	{
		return this.textureIDs;
	}

	// Token: 0x170000D4 RID: 212
	// (get) Token: 0x060008E6 RID: 2278 RVA: 0x0004D6FC File Offset: 0x0004B8FC
	public uint NetworkID
	{
		get
		{
			return this.net.ID;
		}
	}

	// Token: 0x060008E7 RID: 2279 RVA: 0x00054667 File Offset: 0x00052867
	public virtual bool CanUpdateSign(global::BasePlayer player)
	{
		return player.IsAdmin || player.IsDeveloper || (player.CanBuild() && (!base.IsLocked() || player.userID == base.OwnerID));
	}

	// Token: 0x060008E8 RID: 2280 RVA: 0x0005469D File Offset: 0x0005289D
	public bool CanUnlockSign(global::BasePlayer player)
	{
		return base.IsLocked() && this.CanUpdateSign(player);
	}

	// Token: 0x060008E9 RID: 2281 RVA: 0x000546B0 File Offset: 0x000528B0
	public bool CanLockSign(global::BasePlayer player)
	{
		return !base.IsLocked() && this.CanUpdateSign(player);
	}

	// Token: 0x060008EA RID: 2282 RVA: 0x000546C4 File Offset: 0x000528C4
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		this.EnsureInitialized();
		bool flag = false;
		if (info.msg.sign != null)
		{
			uint num = this.textureIDs[0];
			if (info.msg.sign.imageIds != null && info.msg.sign.imageIds.Count > 0)
			{
				int num2 = Mathf.Min(info.msg.sign.imageIds.Count, this.textureIDs.Length);
				for (int i = 0; i < num2; i++)
				{
					uint num3 = info.msg.sign.imageIds[i];
					bool flag2 = num3 != this.textureIDs[i];
					flag = (flag || flag2);
					this.textureIDs[i] = num3;
				}
			}
			else
			{
				flag = (num != info.msg.sign.imageid);
				this.textureIDs[0] = info.msg.sign.imageid;
			}
		}
		if (base.isServer)
		{
			bool flag3 = false;
			for (int j = 0; j < this.paintableSources.Length; j++)
			{
				uint num4 = this.textureIDs[j];
				if (num4 != 0U)
				{
					byte[] array = FileStorage.server.Get(num4, FileStorage.Type.png, this.net.ID, (uint)j);
					if (array == null)
					{
						base.Log(string.Format("Frame {0} (id={1}) doesn't exist, clearing", j, num4));
						this.textureIDs[j] = 0U;
					}
					flag3 = (flag3 || array != null);
				}
			}
			if (!flag3)
			{
				base.SetFlag(global::BaseEntity.Flags.Locked, false, false, true);
			}
			if (info.msg.sign != null)
			{
				if (info.msg.sign.editHistory != null)
				{
					if (this.editHistory == null)
					{
						this.editHistory = Facepunch.Pool.GetList<ulong>();
					}
					this.editHistory.Clear();
					using (List<ulong>.Enumerator enumerator = info.msg.sign.editHistory.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ulong item = enumerator.Current;
							this.editHistory.Add(item);
						}
						return;
					}
				}
				if (this.editHistory != null)
				{
					Facepunch.Pool.FreeList<ulong>(ref this.editHistory);
				}
			}
		}
	}

	// Token: 0x170000D5 RID: 213
	// (get) Token: 0x060008EB RID: 2283 RVA: 0x00003A54 File Offset: 0x00001C54
	public UGCType ContentType
	{
		get
		{
			return UGCType.ImagePng;
		}
	}

	// Token: 0x060008EC RID: 2284 RVA: 0x00054904 File Offset: 0x00052B04
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void LockSign(global::BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		if (!this.CanUpdateSign(msg.player))
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Locked, true, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		base.OwnerID = msg.player.userID;
	}

	// Token: 0x060008ED RID: 2285 RVA: 0x00054951 File Offset: 0x00052B51
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void UnLockSign(global::BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		if (!this.CanUnlockSign(msg.player))
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Locked, false, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060008EE RID: 2286 RVA: 0x00054984 File Offset: 0x00052B84
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		this.EnsureInitialized();
		List<uint> list = Facepunch.Pool.GetList<uint>();
		foreach (uint item in this.textureIDs)
		{
			list.Add(item);
		}
		info.msg.sign = Facepunch.Pool.Get<Sign>();
		info.msg.sign.imageid = 0U;
		info.msg.sign.imageIds = list;
		if (this.editHistory.Count > 0)
		{
			info.msg.sign.editHistory = Facepunch.Pool.GetList<ulong>();
			foreach (ulong item2 in this.editHistory)
			{
				info.msg.sign.editHistory.Add(item2);
			}
		}
	}

	// Token: 0x060008EF RID: 2287 RVA: 0x00054A74 File Offset: 0x00052C74
	public override void OnKilled(HitInfo info)
	{
		if (this.net != null)
		{
			FileStorage.server.RemoveAllByEntity(this.net.ID);
		}
		if (this.textureIDs != null)
		{
			Array.Clear(this.textureIDs, 0, this.textureIDs.Length);
		}
		base.OnKilled(info);
	}

	// Token: 0x060008F0 RID: 2288 RVA: 0x00054AC4 File Offset: 0x00052CC4
	public override void OnPickedUpPreItemMove(global::Item createdItem, global::BasePlayer player)
	{
		base.OnPickedUpPreItemMove(createdItem, player);
		bool flag = false;
		uint[] array = this.textureIDs;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != 0U)
			{
				flag = true;
				break;
			}
		}
		ItemModSign itemModSign;
		if (flag && createdItem.info.TryGetComponent<ItemModSign>(out itemModSign))
		{
			itemModSign.OnSignPickedUp(this, this, createdItem);
		}
	}

	// Token: 0x060008F1 RID: 2289 RVA: 0x00054B14 File Offset: 0x00052D14
	public override void OnDeployed(global::BaseEntity parent, global::BasePlayer deployedBy, global::Item fromItem)
	{
		base.OnDeployed(parent, deployedBy, fromItem);
		ItemModSign itemModSign;
		if (fromItem.info.TryGetComponent<ItemModSign>(out itemModSign))
		{
			SignContent associatedEntity = ItemModAssociatedEntity<SignContent>.GetAssociatedEntity(fromItem, true);
			if (associatedEntity != null)
			{
				associatedEntity.CopyInfoToSign(this, this);
			}
		}
	}

	// Token: 0x060008F2 RID: 2290 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool ShouldNetworkOwnerInfo()
	{
		return true;
	}

	// Token: 0x060008F3 RID: 2291 RVA: 0x00054B52 File Offset: 0x00052D52
	public void SetTextureCRCs(uint[] crcs)
	{
		this.textureIDs = new uint[crcs.Length];
		crcs.CopyTo(this.textureIDs, 0);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x170000D6 RID: 214
	// (get) Token: 0x060008F4 RID: 2292 RVA: 0x00054B76 File Offset: 0x00052D76
	public List<ulong> EditingHistory
	{
		get
		{
			return this.editHistory;
		}
	}

	// Token: 0x060008F5 RID: 2293 RVA: 0x00054B80 File Offset: 0x00052D80
	private void LogEdit(global::BasePlayer byPlayer)
	{
		if (this.editHistory.Contains(byPlayer.userID))
		{
			return;
		}
		this.editHistory.Insert(0, byPlayer.userID);
		int num = 0;
		while (this.editHistory.Count > 5 && num < 10)
		{
			this.editHistory.RemoveAt(5);
			num++;
		}
	}

	// Token: 0x170000D7 RID: 215
	// (get) Token: 0x060008F6 RID: 2294 RVA: 0x0005465F File Offset: 0x0005285F
	public uint[] GetContentCRCs
	{
		get
		{
			return this.textureIDs;
		}
	}

	// Token: 0x060008F7 RID: 2295 RVA: 0x00054BDA File Offset: 0x00052DDA
	public void ClearContent()
	{
		this.SetTextureCRCs(Array.Empty<uint>());
	}

	// Token: 0x170000D8 RID: 216
	// (get) Token: 0x060008F8 RID: 2296 RVA: 0x00002E37 File Offset: 0x00001037
	public global::BaseNetworkable UgcEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x060008F9 RID: 2297 RVA: 0x00054BE7 File Offset: 0x00052DE7
	public override string Categorize()
	{
		return "sign";
	}

	// Token: 0x040005E5 RID: 1509
	private const float TextureRequestTimeout = 15f;

	// Token: 0x040005E6 RID: 1510
	public GameObjectRef changeTextDialog;

	// Token: 0x040005E7 RID: 1511
	public MeshPaintableSource[] paintableSources;

	// Token: 0x040005E8 RID: 1512
	[NonSerialized]
	public uint[] textureIDs;

	// Token: 0x040005E9 RID: 1513
	private List<ulong> editHistory = new List<ulong>();
}
