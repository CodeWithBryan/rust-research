using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000C4 RID: 196
public class Signage : global::IOEntity, ILOD, ISignage, IUGCBrowserEntity
{
	// Token: 0x06001149 RID: 4425 RVA: 0x0008BFBC File Offset: 0x0008A1BC
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Signage.OnRpcMessage", 0))
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

	// Token: 0x1700016A RID: 362
	// (get) Token: 0x0600114A RID: 4426 RVA: 0x0008C434 File Offset: 0x0008A634
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

	// Token: 0x1700016B RID: 363
	// (get) Token: 0x0600114B RID: 4427 RVA: 0x0008C472 File Offset: 0x0008A672
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

	// Token: 0x0600114C RID: 4428 RVA: 0x0008C484 File Offset: 0x0008A684
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

	// Token: 0x0600114D RID: 4429 RVA: 0x0008C4F0 File Offset: 0x0008A6F0
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

	// Token: 0x0600114E RID: 4430 RVA: 0x0008C638 File Offset: 0x0008A838
	private void EnsureInitialized()
	{
		int num = Mathf.Max(this.paintableSources.Length, 1);
		if (this.textureIDs == null || this.textureIDs.Length != num)
		{
			Array.Resize<uint>(ref this.textureIDs, num);
		}
	}

	// Token: 0x0600114F RID: 4431 RVA: 0x00054657 File Offset: 0x00052857
	[Conditional("SIGN_DEBUG")]
	private static void SignDebugLog(string str)
	{
		UnityEngine.Debug.Log(str);
	}

	// Token: 0x06001150 RID: 4432 RVA: 0x0008C674 File Offset: 0x0008A874
	public virtual bool CanUpdateSign(global::BasePlayer player)
	{
		if (player.IsAdmin || player.IsDeveloper)
		{
			return true;
		}
		if (!player.CanBuild())
		{
			return false;
		}
		if (base.IsLocked())
		{
			return player.userID == base.OwnerID;
		}
		return this.HeldEntityCheck(player);
	}

	// Token: 0x06001151 RID: 4433 RVA: 0x0008C6C0 File Offset: 0x0008A8C0
	public bool CanUnlockSign(global::BasePlayer player)
	{
		return base.IsLocked() && this.HeldEntityCheck(player) && this.CanUpdateSign(player);
	}

	// Token: 0x06001152 RID: 4434 RVA: 0x0008C6DE File Offset: 0x0008A8DE
	public bool CanLockSign(global::BasePlayer player)
	{
		return !base.IsLocked() && this.HeldEntityCheck(player) && this.CanUpdateSign(player);
	}

	// Token: 0x06001153 RID: 4435 RVA: 0x0008C6FC File Offset: 0x0008A8FC
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

	// Token: 0x06001154 RID: 4436 RVA: 0x0008C93C File Offset: 0x0008AB3C
	private bool HeldEntityCheck(global::BasePlayer player)
	{
		return !(this.RequiredHeldEntity != null) || (player.GetHeldEntity() && !(player.GetHeldEntity().GetItem().info != this.RequiredHeldEntity));
	}

	// Token: 0x06001155 RID: 4437 RVA: 0x0008C979 File Offset: 0x0008AB79
	public uint[] GetTextureCRCs()
	{
		return this.textureIDs;
	}

	// Token: 0x1700016C RID: 364
	// (get) Token: 0x06001156 RID: 4438 RVA: 0x0004D6FC File Offset: 0x0004B8FC
	public uint NetworkID
	{
		get
		{
			return this.net.ID;
		}
	}

	// Token: 0x1700016D RID: 365
	// (get) Token: 0x06001157 RID: 4439 RVA: 0x00007074 File Offset: 0x00005274
	public FileStorage.Type FileType
	{
		get
		{
			return FileStorage.Type.png;
		}
	}

	// Token: 0x1700016E RID: 366
	// (get) Token: 0x06001158 RID: 4440 RVA: 0x00003A54 File Offset: 0x00001C54
	public UGCType ContentType
	{
		get
		{
			return UGCType.ImagePng;
		}
	}

	// Token: 0x06001159 RID: 4441 RVA: 0x0008C984 File Offset: 0x0008AB84
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

	// Token: 0x0600115A RID: 4442 RVA: 0x0008C9D1 File Offset: 0x0008ABD1
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

	// Token: 0x0600115B RID: 4443 RVA: 0x0008CA04 File Offset: 0x0008AC04
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
		if (this.editHistory != null && this.editHistory.Count > 0)
		{
			info.msg.sign.editHistory = Facepunch.Pool.GetList<ulong>();
			foreach (ulong item2 in this.editHistory)
			{
				info.msg.sign.editHistory.Add(item2);
			}
		}
	}

	// Token: 0x0600115C RID: 4444 RVA: 0x0008CAFC File Offset: 0x0008ACFC
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

	// Token: 0x0600115D RID: 4445 RVA: 0x0008CB4C File Offset: 0x0008AD4C
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

	// Token: 0x0600115E RID: 4446 RVA: 0x0008CB9C File Offset: 0x0008AD9C
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

	// Token: 0x0600115F RID: 4447 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool ShouldNetworkOwnerInfo()
	{
		return true;
	}

	// Token: 0x06001160 RID: 4448 RVA: 0x0008CBDA File Offset: 0x0008ADDA
	public void SetTextureCRCs(uint[] crcs)
	{
		this.textureIDs = new uint[crcs.Length];
		crcs.CopyTo(this.textureIDs, 0);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x1700016F RID: 367
	// (get) Token: 0x06001161 RID: 4449 RVA: 0x0008CBFE File Offset: 0x0008ADFE
	public List<ulong> EditingHistory
	{
		get
		{
			return this.editHistory;
		}
	}

	// Token: 0x17000170 RID: 368
	// (get) Token: 0x06001162 RID: 4450 RVA: 0x00002E37 File Offset: 0x00001037
	public global::BaseNetworkable UgcEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x06001163 RID: 4451 RVA: 0x0008CC08 File Offset: 0x0008AE08
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

	// Token: 0x17000171 RID: 369
	// (get) Token: 0x06001164 RID: 4452 RVA: 0x0008CC62 File Offset: 0x0008AE62
	public uint[] GetContentCRCs
	{
		get
		{
			return this.GetTextureCRCs();
		}
	}

	// Token: 0x06001165 RID: 4453 RVA: 0x0008CC6A File Offset: 0x0008AE6A
	public void ClearContent()
	{
		this.SetTextureCRCs(Array.Empty<uint>());
	}

	// Token: 0x06001166 RID: 4454 RVA: 0x0008CC78 File Offset: 0x0008AE78
	public override string Admin_Who()
	{
		if (this.editHistory == null || this.editHistory.Count == 0)
		{
			return base.Admin_Who();
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(base.Admin_Who());
		for (int i = 0; i < this.editHistory.Count; i++)
		{
			stringBuilder.AppendLine(string.Format("Edit {0}: {1}", i, this.editHistory[i]));
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06001167 RID: 4455 RVA: 0x00007074 File Offset: 0x00005274
	public override int ConsumptionAmount()
	{
		return 0;
	}

	// Token: 0x06001168 RID: 4456 RVA: 0x00054BE7 File Offset: 0x00052DE7
	public override string Categorize()
	{
		return "sign";
	}

	// Token: 0x04000AD2 RID: 2770
	private const float TextureRequestTimeout = 15f;

	// Token: 0x04000AD3 RID: 2771
	public GameObjectRef changeTextDialog;

	// Token: 0x04000AD4 RID: 2772
	public MeshPaintableSource[] paintableSources;

	// Token: 0x04000AD5 RID: 2773
	[NonSerialized]
	public uint[] textureIDs;

	// Token: 0x04000AD6 RID: 2774
	public ItemDefinition RequiredHeldEntity;

	// Token: 0x04000AD7 RID: 2775
	private List<ulong> editHistory = new List<ulong>();
}
