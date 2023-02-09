using System;
using System.Collections.Generic;
using System.Diagnostics;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000A4 RID: 164
public class PaintedItemStorageEntity : global::BaseEntity, IServerFileReceiver, IUGCBrowserEntity
{
	// Token: 0x06000F02 RID: 3842 RVA: 0x0007CD94 File Offset: 0x0007AF94
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PaintedItemStorageEntity.OnRpcMessage", 0))
		{
			if (rpc == 2439017595U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					UnityEngine.Debug.Log("SV_RPCMessage: " + player + " - Server_UpdateImage ");
				}
				using (TimeWarning.New("Server_UpdateImage", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(2439017595U, "Server_UpdateImage", this, player, 3UL))
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
							this.Server_UpdateImage(msg2);
						}
					}
					catch (Exception exception)
					{
						UnityEngine.Debug.LogException(exception);
						player.Kick("RPC Error in Server_UpdateImage");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000F03 RID: 3843 RVA: 0x0007CEFC File Offset: 0x0007B0FC
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.paintedItem != null)
		{
			this._currentImageCrc = info.msg.paintedItem.imageCrc;
			if (base.isServer)
			{
				this.lastEditedBy = info.msg.paintedItem.editedBy;
			}
		}
	}

	// Token: 0x06000F04 RID: 3844 RVA: 0x0007CF54 File Offset: 0x0007B154
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.paintedItem = Facepunch.Pool.Get<PaintedItem>();
		info.msg.paintedItem.imageCrc = this._currentImageCrc;
		info.msg.paintedItem.editedBy = this.lastEditedBy;
	}

	// Token: 0x06000F05 RID: 3845 RVA: 0x0007CFA4 File Offset: 0x0007B1A4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(3UL)]
	private void Server_UpdateImage(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player == null || msg.player.userID != base.OwnerID)
		{
			return;
		}
		foreach (global::Item item in msg.player.inventory.containerWear.itemList)
		{
			if (item.instanceData != null && item.instanceData.subEntity == this.net.ID)
			{
				return;
			}
		}
		global::Item item2 = msg.player.inventory.FindBySubEntityID(this.net.ID);
		if (item2 == null || item2.isBroken)
		{
			return;
		}
		byte[] array = msg.read.BytesWithSize(10485760U);
		if (array == null)
		{
			if (this._currentImageCrc != 0U)
			{
				FileStorage.server.RemoveExact(this._currentImageCrc, FileStorage.Type.png, this.net.ID, 0U);
			}
			this._currentImageCrc = 0U;
		}
		else
		{
			if (!ImageProcessing.IsValidPNG(array, 512, 512))
			{
				return;
			}
			uint currentImageCrc = this._currentImageCrc;
			if (this._currentImageCrc != 0U)
			{
				FileStorage.server.RemoveExact(this._currentImageCrc, FileStorage.Type.png, this.net.ID, 0U);
			}
			this._currentImageCrc = FileStorage.server.Store(array, FileStorage.Type.png, this.net.ID, 0U);
			if (this._currentImageCrc != currentImageCrc)
			{
				item2.LoseCondition(0.25f);
			}
			this.lastEditedBy = msg.player.userID;
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000F06 RID: 3846 RVA: 0x0007D140 File Offset: 0x0007B340
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (!Rust.Application.isQuitting && this.net != null)
		{
			FileStorage.server.RemoveAllByEntity(this.net.ID);
		}
	}

	// Token: 0x17000149 RID: 329
	// (get) Token: 0x06000F07 RID: 3847 RVA: 0x0007D16C File Offset: 0x0007B36C
	public uint[] GetContentCRCs
	{
		get
		{
			if (this._currentImageCrc <= 0U)
			{
				return Array.Empty<uint>();
			}
			return new uint[]
			{
				this._currentImageCrc
			};
		}
	}

	// Token: 0x06000F08 RID: 3848 RVA: 0x0007D18C File Offset: 0x0007B38C
	public void ClearContent()
	{
		this._currentImageCrc = 0U;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x1700014A RID: 330
	// (get) Token: 0x06000F09 RID: 3849 RVA: 0x00007074 File Offset: 0x00005274
	public UGCType ContentType
	{
		get
		{
			return UGCType.ImageJpg;
		}
	}

	// Token: 0x1700014B RID: 331
	// (get) Token: 0x06000F0A RID: 3850 RVA: 0x0007D19C File Offset: 0x0007B39C
	public List<ulong> EditingHistory
	{
		get
		{
			if (this.lastEditedBy <= 0UL)
			{
				return new List<ulong>();
			}
			return new List<ulong>
			{
				this.lastEditedBy
			};
		}
	}

	// Token: 0x1700014C RID: 332
	// (get) Token: 0x06000F0B RID: 3851 RVA: 0x00002E37 File Offset: 0x00001037
	public global::BaseNetworkable UgcEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x06000F0C RID: 3852 RVA: 0x0007D1BF File Offset: 0x0007B3BF
	[Conditional("PAINTED_ITEM_DEBUG")]
	private void DebugOnlyLog(string str)
	{
		UnityEngine.Debug.Log(str, this);
	}

	// Token: 0x040009CC RID: 2508
	private uint _currentImageCrc;

	// Token: 0x040009CD RID: 2509
	private ulong lastEditedBy;
}
