using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000060 RID: 96
public class DeployedRecorder : StorageContainer, ICassettePlayer
{
	// Token: 0x060009D6 RID: 2518 RVA: 0x0005A6E0 File Offset: 0x000588E0
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("DeployedRecorder.OnRpcMessage", 0))
		{
			if (rpc == 1785864031U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerTogglePlay ");
				}
				using (TimeWarning.New("ServerTogglePlay", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1785864031U, "ServerTogglePlay", this, player, 3f))
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
							this.ServerTogglePlay(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in ServerTogglePlay");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170000E5 RID: 229
	// (get) Token: 0x060009D7 RID: 2519 RVA: 0x00002E37 File Offset: 0x00001037
	public BaseEntity ToBaseEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x060009D8 RID: 2520 RVA: 0x0005A848 File Offset: 0x00058A48
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void ServerTogglePlay(BaseEntity.RPCMessage msg)
	{
		bool play = msg.read.ReadByte() == 1;
		this.ServerTogglePlay(play);
	}

	// Token: 0x060009D9 RID: 2521 RVA: 0x0005A86B File Offset: 0x00058A6B
	private void ServerTogglePlay(bool play)
	{
		base.SetFlag(BaseEntity.Flags.On, play, false, true);
	}

	// Token: 0x060009DA RID: 2522 RVA: 0x0005A877 File Offset: 0x00058A77
	public void OnCassetteInserted(Cassette c)
	{
		base.ClientRPC<uint>(null, "Client_OnCassetteInserted", c.net.ID);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060009DB RID: 2523 RVA: 0x0005A897 File Offset: 0x00058A97
	public void OnCassetteRemoved(Cassette c)
	{
		base.ClientRPC(null, "Client_OnCassetteRemoved");
		this.ServerTogglePlay(false);
	}

	// Token: 0x060009DC RID: 2524 RVA: 0x0005A8AC File Offset: 0x00058AAC
	public override bool ItemFilter(Item item, int targetSlot)
	{
		ItemDefinition[] validCassettes = this.ValidCassettes;
		for (int i = 0; i < validCassettes.Length; i++)
		{
			if (validCassettes[i] == item.info)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060009DD RID: 2525 RVA: 0x0005A8E1 File Offset: 0x00058AE1
	public override void OnCollision(Collision collision, BaseEntity hitEntity)
	{
		if (base.isServer)
		{
			this.DoCollisionStick(collision, hitEntity);
		}
	}

	// Token: 0x060009DE RID: 2526 RVA: 0x0005A8F4 File Offset: 0x00058AF4
	private void DoCollisionStick(Collision collision, BaseEntity ent)
	{
		ContactPoint contact = collision.GetContact(0);
		this.DoStick(contact.point, contact.normal, ent, collision.collider);
	}

	// Token: 0x060009DF RID: 2527 RVA: 0x0005A924 File Offset: 0x00058B24
	public virtual void SetMotionEnabled(bool wantsMotion)
	{
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component)
		{
			if (this.initialCollisionDetectionMode == null)
			{
				this.initialCollisionDetectionMode = new CollisionDetectionMode?(component.collisionDetectionMode);
			}
			component.useGravity = wantsMotion;
			if (!wantsMotion)
			{
				component.collisionDetectionMode = CollisionDetectionMode.Discrete;
			}
			component.isKinematic = !wantsMotion;
			if (wantsMotion)
			{
				component.collisionDetectionMode = this.initialCollisionDetectionMode.Value;
			}
		}
	}

	// Token: 0x060009E0 RID: 2528 RVA: 0x0005A990 File Offset: 0x00058B90
	public void DoStick(Vector3 position, Vector3 normal, BaseEntity ent, Collider hitCollider)
	{
		if (ent != null && ent is TimedExplosive)
		{
			if (!ent.HasParent())
			{
				return;
			}
			position = ent.transform.position;
			ent = ent.parentEntity.Get(true);
		}
		this.SetMotionEnabled(false);
		this.SetCollisionEnabled(false);
		if (ent != null && base.HasChild(ent))
		{
			return;
		}
		base.transform.position = position;
		base.transform.rotation = Quaternion.LookRotation(normal, base.transform.up);
		if (hitCollider != null && ent != null)
		{
			base.SetParent(ent, ent.FindBoneID(hitCollider.transform), true, false);
		}
		else
		{
			base.SetParent(ent, StringPool.closest, true, false);
		}
		base.ReceiveCollisionMessages(false);
	}

	// Token: 0x060009E1 RID: 2529 RVA: 0x0005AA5D File Offset: 0x00058C5D
	private void UnStick()
	{
		if (!base.GetParentEntity())
		{
			return;
		}
		base.SetParent(null, true, true);
		this.SetMotionEnabled(true);
		this.SetCollisionEnabled(true);
		base.ReceiveCollisionMessages(true);
	}

	// Token: 0x060009E2 RID: 2530 RVA: 0x0005AA8B File Offset: 0x00058C8B
	internal override void OnParentRemoved()
	{
		this.UnStick();
	}

	// Token: 0x060009E3 RID: 2531 RVA: 0x0005AA94 File Offset: 0x00058C94
	public virtual void SetCollisionEnabled(bool wantsCollision)
	{
		Collider component = base.GetComponent<Collider>();
		if (component && component.enabled != wantsCollision)
		{
			component.enabled = wantsCollision;
		}
	}

	// Token: 0x060009E4 RID: 2532 RVA: 0x0005AAC0 File Offset: 0x00058CC0
	public override void ResetState()
	{
		base.ResetState();
		if (base.isServer)
		{
			this.initialCollisionDetectionMode = null;
		}
	}

	// Token: 0x0400066C RID: 1644
	public AudioSource SoundSource;

	// Token: 0x0400066D RID: 1645
	public ItemDefinition[] ValidCassettes;

	// Token: 0x0400066E RID: 1646
	public SoundDefinition PlaySfx;

	// Token: 0x0400066F RID: 1647
	public SoundDefinition StopSfx;

	// Token: 0x04000670 RID: 1648
	public SwapKeycard TapeSwapper;

	// Token: 0x04000671 RID: 1649
	private CollisionDetectionMode? initialCollisionDetectionMode;
}
