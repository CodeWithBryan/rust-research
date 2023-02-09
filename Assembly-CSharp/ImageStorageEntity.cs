using System;
using System.Collections.Generic;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200007E RID: 126
public class ImageStorageEntity : BaseEntity
{
	// Token: 0x06000BF2 RID: 3058 RVA: 0x00066DD4 File Offset: 0x00064FD4
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ImageStorageEntity.OnRpcMessage", 0))
		{
			if (rpc == 652912521U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ImageRequested ");
				}
				using (TimeWarning.New("ImageRequested", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.CallsPerSecond.Test(652912521U, "ImageRequested", this, player, 3UL))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ImageRequested(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in ImageRequested");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000103 RID: 259
	// (get) Token: 0x06000BF3 RID: 3059 RVA: 0x00003A54 File Offset: 0x00001C54
	protected virtual FileStorage.Type StorageType
	{
		get
		{
			return FileStorage.Type.jpg;
		}
	}

	// Token: 0x17000104 RID: 260
	// (get) Token: 0x06000BF4 RID: 3060 RVA: 0x00007074 File Offset: 0x00005274
	protected virtual uint CrcToLoad
	{
		get
		{
			return 0U;
		}
	}

	// Token: 0x06000BF5 RID: 3061 RVA: 0x00066F3C File Offset: 0x0006513C
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.CallsPerSecond(3UL)]
	private void ImageRequested(BaseEntity.RPCMessage msg)
	{
		if (msg.player == null)
		{
			return;
		}
		byte[] array = FileStorage.server.Get(this.CrcToLoad, this.StorageType, this.net.ID, 0U);
		if (array == null)
		{
			Debug.LogWarning("Image entity has no image!");
			return;
		}
		SendInfo sendInfo = new SendInfo(msg.connection)
		{
			method = SendMethod.Reliable,
			channel = 2
		};
		base.ClientRPCEx<uint, byte[]>(sendInfo, null, "ReceiveImage", (uint)array.Length, array);
	}

	// Token: 0x040007A8 RID: 1960
	private List<ImageStorageEntity.ImageRequest> _requests;

	// Token: 0x02000B8B RID: 2955
	private struct ImageRequest
	{
		// Token: 0x04003EAE RID: 16046
		public IImageReceiver Receiver;

		// Token: 0x04003EAF RID: 16047
		public float Time;
	}
}
