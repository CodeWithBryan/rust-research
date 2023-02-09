using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000041 RID: 65
public class BasePortal : BaseCombatEntity
{
	// Token: 0x06000694 RID: 1684 RVA: 0x00044458 File Offset: 0x00042658
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BasePortal.OnRpcMessage", 0))
		{
			if (rpc == 561762999U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_UsePortal ");
				}
				using (TimeWarning.New("RPC_UsePortal", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(561762999U, "RPC_UsePortal", this, player, 1UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(561762999U, "RPC_UsePortal", this, player, 3f))
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
							this.RPC_UsePortal(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_UsePortal");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000695 RID: 1685 RVA: 0x00044618 File Offset: 0x00042818
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity = Facepunch.Pool.Get<ProtoBuf.IOEntity>();
		info.msg.ioEntity.genericEntRef1 = this.targetID;
	}

	// Token: 0x06000696 RID: 1686 RVA: 0x00044647 File Offset: 0x00042847
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.targetID = info.msg.ioEntity.genericEntRef1;
		}
	}

	// Token: 0x06000697 RID: 1687 RVA: 0x00044673 File Offset: 0x00042873
	public override void ServerInit()
	{
		base.ServerInit();
	}

	// Token: 0x06000698 RID: 1688 RVA: 0x0004467C File Offset: 0x0004287C
	public void LinkPortal()
	{
		if (this.targetPortal != null)
		{
			this.targetID = this.targetPortal.net.ID;
		}
		if (this.targetPortal == null && this.targetID != 0U)
		{
			global::BaseNetworkable baseNetworkable = global::BaseNetworkable.serverEntities.Find(this.targetID);
			if (baseNetworkable != null)
			{
				this.targetPortal = baseNetworkable.GetComponent<BasePortal>();
			}
		}
	}

	// Token: 0x06000699 RID: 1689 RVA: 0x000446E9 File Offset: 0x000428E9
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		Debug.Log("Post server load");
		this.LinkPortal();
	}

	// Token: 0x0600069A RID: 1690 RVA: 0x00044701 File Offset: 0x00042901
	public void SetDestination(Vector3 destPos, Quaternion destRot)
	{
		this.destination_pos = destPos;
		this.destination_rot = destRot;
	}

	// Token: 0x0600069B RID: 1691 RVA: 0x00044711 File Offset: 0x00042911
	public Vector3 GetLocalEntryExitPosition()
	{
		return this.localEntryExitPos.transform.position;
	}

	// Token: 0x0600069C RID: 1692 RVA: 0x00044723 File Offset: 0x00042923
	public Quaternion GetLocalEntryExitRotation()
	{
		return this.localEntryExitPos.transform.rotation;
	}

	// Token: 0x0600069D RID: 1693 RVA: 0x00044735 File Offset: 0x00042935
	public BasePortal GetPortal()
	{
		this.LinkPortal();
		return this.targetPortal;
	}

	// Token: 0x0600069E RID: 1694 RVA: 0x00044744 File Offset: 0x00042944
	public virtual void UsePortal(global::BasePlayer player)
	{
		this.LinkPortal();
		if (this.targetPortal != null)
		{
			player.PauseFlyHackDetection(1f);
			player.PauseSpeedHackDetection(1f);
			Vector3 position = player.transform.position;
			Vector3 vector = this.targetPortal.GetLocalEntryExitPosition();
			Vector3 direction = base.transform.InverseTransformDirection(player.eyes.BodyForward());
			Vector3 arg;
			if (this.isMirrored)
			{
				Vector3 position2 = base.transform.InverseTransformPoint(player.transform.position);
				vector = this.targetPortal.relativeAnchor.transform.TransformPoint(position2);
				arg = this.targetPortal.relativeAnchor.transform.TransformDirection(direction);
			}
			else
			{
				arg = this.targetPortal.GetLocalEntryExitRotation() * Vector3.forward;
			}
			if (this.disappearEffect.isValid)
			{
				Effect.server.Run(this.disappearEffect.resourcePath, position, Vector3.up, null, false);
			}
			if (this.appearEffect.isValid)
			{
				Effect.server.Run(this.appearEffect.resourcePath, vector, Vector3.up, null, false);
			}
			player.SetParent(null, true, false);
			player.Teleport(vector);
			player.ForceUpdateTriggers(true, true, true);
			player.ClientRPCPlayer<Vector3>(null, player, "ForceViewAnglesTo", arg);
			if (this.transitionSoundEffect.isValid)
			{
				Effect.server.Run(this.transitionSoundEffect.resourcePath, this.targetPortal.relativeAnchor.transform.position, Vector3.up, null, false);
			}
			player.UpdateNetworkGroup();
			player.SetPlayerFlag(global::BasePlayer.PlayerFlags.ReceivingSnapshot, true);
			base.SendNetworkUpdateImmediate(false);
			player.ClientRPCPlayer<bool>(null, player, "StartLoading_Quick", true);
			return;
		}
		Debug.Log("No portal...");
	}

	// Token: 0x0600069F RID: 1695 RVA: 0x000448F0 File Offset: 0x00042AF0
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	public void RPC_UsePortal(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (!this.IsActive())
		{
			return;
		}
		this.UsePortal(player);
	}

	// Token: 0x060006A0 RID: 1696 RVA: 0x00003A54 File Offset: 0x00001C54
	public bool IsActive()
	{
		return true;
	}

	// Token: 0x04000436 RID: 1078
	public bool isUsablePortal = true;

	// Token: 0x04000437 RID: 1079
	private Vector3 destination_pos;

	// Token: 0x04000438 RID: 1080
	private Quaternion destination_rot;

	// Token: 0x04000439 RID: 1081
	public BasePortal targetPortal;

	// Token: 0x0400043A RID: 1082
	public uint targetID;

	// Token: 0x0400043B RID: 1083
	public Transform localEntryExitPos;

	// Token: 0x0400043C RID: 1084
	public Transform relativeAnchor;

	// Token: 0x0400043D RID: 1085
	public bool isMirrored = true;

	// Token: 0x0400043E RID: 1086
	public GameObjectRef appearEffect;

	// Token: 0x0400043F RID: 1087
	public GameObjectRef disappearEffect;

	// Token: 0x04000440 RID: 1088
	public GameObjectRef transitionSoundEffect;

	// Token: 0x04000441 RID: 1089
	public string useTagString = "";
}
