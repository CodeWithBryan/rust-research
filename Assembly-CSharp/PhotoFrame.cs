using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000A6 RID: 166
public class PhotoFrame : StorageContainer, ILOD, IImageReceiver, ISignage, IUGCBrowserEntity
{
	// Token: 0x06000F17 RID: 3863 RVA: 0x0007D414 File Offset: 0x0007B614
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PhotoFrame.OnRpcMessage", 0))
		{
			if (rpc == 1455609404U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - LockSign ");
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
						Debug.LogException(exception);
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
					Debug.Log("SV_RPCMessage: " + player + " - UnLockSign ");
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
						Debug.LogException(exception2);
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
					Debug.Log("SV_RPCMessage: " + player + " - UpdateSign ");
				}
				using (TimeWarning.New("UpdateSign", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(1255380462U, "UpdateSign", this, player, 3UL))
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
						Debug.LogException(exception3);
						player.Kick("RPC Error in UpdateSign");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x1700014D RID: 333
	// (get) Token: 0x06000F18 RID: 3864 RVA: 0x0007D88C File Offset: 0x0007BA8C
	public Vector2i TextureSize
	{
		get
		{
			return new Vector2i(this.PaintableSource.texWidth, this.PaintableSource.texHeight);
		}
	}

	// Token: 0x1700014E RID: 334
	// (get) Token: 0x06000F19 RID: 3865 RVA: 0x00003A54 File Offset: 0x00001C54
	public int TextureCount
	{
		get
		{
			return 1;
		}
	}

	// Token: 0x06000F1A RID: 3866 RVA: 0x00054667 File Offset: 0x00052867
	public bool CanUpdateSign(global::BasePlayer player)
	{
		return player.IsAdmin || player.IsDeveloper || (player.CanBuild() && (!base.IsLocked() || player.userID == base.OwnerID));
	}

	// Token: 0x06000F1B RID: 3867 RVA: 0x0007D8A9 File Offset: 0x0007BAA9
	public bool CanUnlockSign(global::BasePlayer player)
	{
		return base.IsLocked() && this.CanUpdateSign(player);
	}

	// Token: 0x06000F1C RID: 3868 RVA: 0x0007D8BC File Offset: 0x0007BABC
	public bool CanLockSign(global::BasePlayer player)
	{
		return !base.IsLocked() && this.CanUpdateSign(player);
	}

	// Token: 0x06000F1D RID: 3869 RVA: 0x0007D8D0 File Offset: 0x0007BAD0
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(5f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(3UL)]
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
		byte[] array = msg.read.BytesWithSize(10485760U);
		if (array == null)
		{
			return;
		}
		if (!ImageProcessing.IsValidPNG(array, 1024, 1024))
		{
			return;
		}
		FileStorage.server.RemoveAllByEntity(this.net.ID);
		this._overlayTextureCrc = FileStorage.server.Store(array, FileStorage.Type.png, this.net.ID, 0U);
		this.LogEdit(msg.player);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000F1E RID: 3870 RVA: 0x0007D96C File Offset: 0x0007BB6C
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

	// Token: 0x06000F1F RID: 3871 RVA: 0x0007D9B9 File Offset: 0x0007BBB9
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

	// Token: 0x06000F20 RID: 3872 RVA: 0x0007D9EA File Offset: 0x0007BBEA
	public override void OnKilled(HitInfo info)
	{
		if (this.net != null)
		{
			FileStorage.server.RemoveAllByEntity(this.net.ID);
		}
		this._overlayTextureCrc = 0U;
		base.OnKilled(info);
	}

	// Token: 0x06000F21 RID: 3873 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool ShouldNetworkOwnerInfo()
	{
		return true;
	}

	// Token: 0x06000F22 RID: 3874 RVA: 0x00054BE7 File Offset: 0x00052DE7
	public override string Categorize()
	{
		return "sign";
	}

	// Token: 0x06000F23 RID: 3875 RVA: 0x0007DA18 File Offset: 0x0007BC18
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.photoFrame != null)
		{
			this._photoEntity.uid = info.msg.photoFrame.photoEntityId;
			this._overlayTextureCrc = info.msg.photoFrame.overlayImageCrc;
		}
		if (base.isServer && info.msg.photoFrame != null)
		{
			if (info.msg.photoFrame.editHistory != null)
			{
				if (this.editHistory == null)
				{
					this.editHistory = Facepunch.Pool.GetList<ulong>();
				}
				this.editHistory.Clear();
				using (List<ulong>.Enumerator enumerator = info.msg.photoFrame.editHistory.GetEnumerator())
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

	// Token: 0x06000F24 RID: 3876 RVA: 0x0007DB20 File Offset: 0x0007BD20
	public uint[] GetTextureCRCs()
	{
		return new uint[]
		{
			this._overlayTextureCrc
		};
	}

	// Token: 0x1700014F RID: 335
	// (get) Token: 0x06000F25 RID: 3877 RVA: 0x0004D6FC File Offset: 0x0004B8FC
	public uint NetworkID
	{
		get
		{
			return this.net.ID;
		}
	}

	// Token: 0x17000150 RID: 336
	// (get) Token: 0x06000F26 RID: 3878 RVA: 0x00007074 File Offset: 0x00005274
	public FileStorage.Type FileType
	{
		get
		{
			return FileStorage.Type.png;
		}
	}

	// Token: 0x17000151 RID: 337
	// (get) Token: 0x06000F27 RID: 3879 RVA: 0x00003A54 File Offset: 0x00001C54
	public UGCType ContentType
	{
		get
		{
			return UGCType.ImagePng;
		}
	}

	// Token: 0x06000F28 RID: 3880 RVA: 0x0007DB34 File Offset: 0x0007BD34
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.photoFrame = Facepunch.Pool.Get<ProtoBuf.PhotoFrame>();
		info.msg.photoFrame.photoEntityId = this._photoEntity.uid;
		info.msg.photoFrame.overlayImageCrc = this._overlayTextureCrc;
		if (this.editHistory.Count > 0)
		{
			info.msg.photoFrame.editHistory = Facepunch.Pool.GetList<ulong>();
			foreach (ulong item in this.editHistory)
			{
				info.msg.photoFrame.editHistory.Add(item);
			}
		}
	}

	// Token: 0x06000F29 RID: 3881 RVA: 0x0007DC04 File Offset: 0x0007BE04
	public override void OnItemAddedOrRemoved(global::Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
		global::Item item2 = (base.inventory.itemList.Count > 0) ? base.inventory.itemList[0] : null;
		uint num = (item2 != null && item2.IsValid()) ? item2.instanceData.subEntity : 0U;
		if (num != this._photoEntity.uid)
		{
			this._photoEntity.uid = num;
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000F2A RID: 3882 RVA: 0x0007DC7C File Offset: 0x0007BE7C
	public override void OnPickedUpPreItemMove(global::Item createdItem, global::BasePlayer player)
	{
		base.OnPickedUpPreItemMove(createdItem, player);
		ItemModSign itemModSign;
		if (this._overlayTextureCrc > 0U && createdItem.info.TryGetComponent<ItemModSign>(out itemModSign))
		{
			itemModSign.OnSignPickedUp(this, this, createdItem);
		}
	}

	// Token: 0x06000F2B RID: 3883 RVA: 0x0007DCB4 File Offset: 0x0007BEB4
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

	// Token: 0x06000F2C RID: 3884 RVA: 0x0007DCF2 File Offset: 0x0007BEF2
	public void SetTextureCRCs(uint[] crcs)
	{
		if (crcs.Length != 0)
		{
			this._overlayTextureCrc = crcs[0];
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x17000152 RID: 338
	// (get) Token: 0x06000F2D RID: 3885 RVA: 0x0007DD08 File Offset: 0x0007BF08
	public List<ulong> EditingHistory
	{
		get
		{
			return this.editHistory;
		}
	}

	// Token: 0x06000F2E RID: 3886 RVA: 0x0007DD10 File Offset: 0x0007BF10
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

	// Token: 0x17000153 RID: 339
	// (get) Token: 0x06000F2F RID: 3887 RVA: 0x0007DB20 File Offset: 0x0007BD20
	public uint[] GetContentCRCs
	{
		get
		{
			return new uint[]
			{
				this._overlayTextureCrc
			};
		}
	}

	// Token: 0x06000F30 RID: 3888 RVA: 0x0007DD6A File Offset: 0x0007BF6A
	public void ClearContent()
	{
		this._overlayTextureCrc = 0U;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x17000154 RID: 340
	// (get) Token: 0x06000F31 RID: 3889 RVA: 0x00002E37 File Offset: 0x00001037
	public global::BaseNetworkable UgcEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x06000F32 RID: 3890 RVA: 0x0007DD7A File Offset: 0x0007BF7A
	public override bool CanPickup(global::BasePlayer player)
	{
		return base.CanPickup(player) && this._photoEntity.uid == 0U;
	}

	// Token: 0x040009D5 RID: 2517
	public GameObjectRef SignEditorDialog;

	// Token: 0x040009D6 RID: 2518
	public OverlayMeshPaintableSource PaintableSource;

	// Token: 0x040009D7 RID: 2519
	private const float TextureRequestDistance = 100f;

	// Token: 0x040009D8 RID: 2520
	private EntityRef _photoEntity;

	// Token: 0x040009D9 RID: 2521
	private uint _overlayTextureCrc;

	// Token: 0x040009DA RID: 2522
	private List<ulong> editHistory = new List<ulong>();
}
