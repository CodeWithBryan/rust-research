using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000E0 RID: 224
public class TreeManager : global::BaseEntity
{
	// Token: 0x0600138C RID: 5004 RVA: 0x0009A298 File Offset: 0x00098498
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("TreeManager.OnRpcMessage", 0))
		{
			if (rpc == 1907121457U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SERVER_RequestTrees ");
				}
				using (TimeWarning.New("SERVER_RequestTrees", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(1907121457U, "SERVER_RequestTrees", this, player, 0UL))
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
							this.SERVER_RequestTrees(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SERVER_RequestTrees");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600138D RID: 5005 RVA: 0x0009A400 File Offset: 0x00098600
	public static Vector3 ProtoHalf3ToVec3(ProtoBuf.Half3 half3)
	{
		return new Vector3
		{
			x = Mathf.HalfToFloat((ushort)half3.x),
			y = Mathf.HalfToFloat((ushort)half3.y),
			z = Mathf.HalfToFloat((ushort)half3.z)
		};
	}

	// Token: 0x0600138E RID: 5006 RVA: 0x0009A450 File Offset: 0x00098650
	public static ProtoBuf.Half3 Vec3ToProtoHalf3(Vector3 vec3)
	{
		return new ProtoBuf.Half3
		{
			x = (uint)Mathf.FloatToHalf(vec3.x),
			y = (uint)Mathf.FloatToHalf(vec3.y),
			z = (uint)Mathf.FloatToHalf(vec3.z)
		};
	}

	// Token: 0x0600138F RID: 5007 RVA: 0x0009A49C File Offset: 0x0009869C
	public override void ServerInit()
	{
		base.ServerInit();
		TreeManager.server = this;
	}

	// Token: 0x06001390 RID: 5008 RVA: 0x0009A4AA File Offset: 0x000986AA
	public static void OnTreeDestroyed(global::BaseEntity billboardEntity)
	{
		TreeManager.entities.Remove(billboardEntity);
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (Rust.Application.isQuitting)
		{
			return;
		}
		TreeManager.server.ClientRPC<uint>(null, "CLIENT_TreeDestroyed", billboardEntity.net.ID);
	}

	// Token: 0x06001391 RID: 5009 RVA: 0x0009A4E4 File Offset: 0x000986E4
	public static void OnTreeSpawned(global::BaseEntity billboardEntity)
	{
		TreeManager.entities.Add(billboardEntity);
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (Rust.Application.isQuitting)
		{
			return;
		}
		using (ProtoBuf.Tree tree = Facepunch.Pool.Get<ProtoBuf.Tree>())
		{
			TreeManager.ExtractTreeNetworkData(billboardEntity, tree);
			TreeManager.server.ClientRPC<ProtoBuf.Tree>(null, "CLIENT_TreeSpawned", tree);
		}
	}

	// Token: 0x06001392 RID: 5010 RVA: 0x0009A548 File Offset: 0x00098748
	private static void ExtractTreeNetworkData(global::BaseEntity billboardEntity, ProtoBuf.Tree tree)
	{
		tree.netId = billboardEntity.net.ID;
		tree.prefabId = billboardEntity.prefabID;
		tree.position = TreeManager.Vec3ToProtoHalf3(billboardEntity.transform.position);
		tree.scale = billboardEntity.transform.lossyScale.y;
	}

	// Token: 0x06001393 RID: 5011 RVA: 0x0009A5A0 File Offset: 0x000987A0
	public static void SendSnapshot(global::BasePlayer player)
	{
		BufferList<global::BaseEntity> values = TreeManager.entities.Values;
		TreeList treeList = null;
		for (int i = 0; i < values.Count; i++)
		{
			global::BaseEntity billboardEntity = values[i];
			ProtoBuf.Tree tree = Facepunch.Pool.Get<ProtoBuf.Tree>();
			TreeManager.ExtractTreeNetworkData(billboardEntity, tree);
			if (treeList == null)
			{
				treeList = Facepunch.Pool.Get<TreeList>();
				treeList.trees = Facepunch.Pool.GetList<ProtoBuf.Tree>();
			}
			treeList.trees.Add(tree);
			if (treeList.trees.Count >= 100)
			{
				TreeManager.server.ClientRPCPlayer<TreeList>(null, player, "CLIENT_ReceiveTrees", treeList);
				treeList.Dispose();
				treeList = null;
			}
		}
		if (treeList != null)
		{
			TreeManager.server.ClientRPCPlayer<TreeList>(null, player, "CLIENT_ReceiveTrees", treeList);
			treeList.Dispose();
		}
	}

	// Token: 0x06001394 RID: 5012 RVA: 0x0009A644 File Offset: 0x00098844
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(0UL)]
	private void SERVER_RequestTrees(global::BaseEntity.RPCMessage msg)
	{
		TreeManager.SendSnapshot(msg.player);
	}

	// Token: 0x04000C54 RID: 3156
	public static ListHashSet<global::BaseEntity> entities = new ListHashSet<global::BaseEntity>(8);

	// Token: 0x04000C55 RID: 3157
	public static TreeManager server;

	// Token: 0x04000C56 RID: 3158
	private const int maxTreesPerPacket = 100;
}
