using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200006C RID: 108
public class ElevatorLift : BaseCombatEntity
{
	// Token: 0x06000A75 RID: 2677 RVA: 0x0005E520 File Offset: 0x0005C720
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ElevatorLift.OnRpcMessage", 0))
		{
			if (rpc == 4061236510U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_RaiseLowerFloor ");
				}
				using (TimeWarning.New("Server_RaiseLowerFloor", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(4061236510U, "Server_RaiseLowerFloor", this, player, 3f))
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
							this.Server_RaiseLowerFloor(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Server_RaiseLowerFloor");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170000EA RID: 234
	// (get) Token: 0x06000A76 RID: 2678 RVA: 0x0005E688 File Offset: 0x0005C888
	private Elevator owner
	{
		get
		{
			return base.GetParentEntity() as Elevator;
		}
	}

	// Token: 0x06000A77 RID: 2679 RVA: 0x0005E695 File Offset: 0x0005C895
	public override void ServerInit()
	{
		base.ServerInit();
		this.ToggleHurtTrigger(false);
	}

	// Token: 0x06000A78 RID: 2680 RVA: 0x0005E6A4 File Offset: 0x0005C8A4
	public void ToggleHurtTrigger(bool state)
	{
		if (this.DescendingHurtTrigger != null)
		{
			this.DescendingHurtTrigger.SetActive(state);
		}
	}

	// Token: 0x06000A79 RID: 2681 RVA: 0x0005E6C0 File Offset: 0x0005C8C0
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void Server_RaiseLowerFloor(BaseEntity.RPCMessage msg)
	{
		if (!this.CanMove())
		{
			return;
		}
		Elevator.Direction direction = (Elevator.Direction)msg.read.Int32();
		bool goTopBottom = msg.read.Bit();
		base.SetFlag((direction == Elevator.Direction.Up) ? BaseEntity.Flags.Reserved1 : BaseEntity.Flags.Reserved2, true, false, true);
		this.owner.Server_RaiseLowerElevator(direction, goTopBottom);
		base.Invoke(new Action(this.ClearDirection), 0.7f);
		if (this.liftButtonPressedEffect.isValid)
		{
			Effect.server.Run(this.liftButtonPressedEffect.resourcePath, base.transform.position, Vector3.up, null, false);
		}
	}

	// Token: 0x06000A7A RID: 2682 RVA: 0x0005E759 File Offset: 0x0005C959
	private void ClearDirection()
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved2, false, false, true);
	}

	// Token: 0x06000A7B RID: 2683 RVA: 0x0005E778 File Offset: 0x0005C978
	public override void Hurt(HitInfo info)
	{
		BaseCombatEntity baseCombatEntity;
		if (base.HasParent() && (baseCombatEntity = (base.GetParentEntity() as BaseCombatEntity)) != null)
		{
			baseCombatEntity.Hurt(info);
		}
	}

	// Token: 0x06000A7C RID: 2684 RVA: 0x0005E7A3 File Offset: 0x0005C9A3
	public override void AdminKill()
	{
		if (base.HasParent())
		{
			base.GetParentEntity().AdminKill();
			return;
		}
		base.AdminKill();
	}

	// Token: 0x06000A7D RID: 2685 RVA: 0x0005E7BF File Offset: 0x0005C9BF
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.ClearDirection();
	}

	// Token: 0x06000A7E RID: 2686 RVA: 0x0005E7CD File Offset: 0x0005C9CD
	public bool CanMove()
	{
		return !this.VehicleTrigger.HasContents;
	}

	// Token: 0x06000A7F RID: 2687 RVA: 0x0005E7DD File Offset: 0x0005C9DD
	public void ToggleMovementCollider(bool state)
	{
		if (this.MovementCollider != null)
		{
			this.MovementCollider.SetActive(state);
		}
	}

	// Token: 0x040006B2 RID: 1714
	public GameObject DescendingHurtTrigger;

	// Token: 0x040006B3 RID: 1715
	public GameObject MovementCollider;

	// Token: 0x040006B4 RID: 1716
	public Transform UpButtonPoint;

	// Token: 0x040006B5 RID: 1717
	public Transform DownButtonPoint;

	// Token: 0x040006B6 RID: 1718
	public TriggerNotify VehicleTrigger;

	// Token: 0x040006B7 RID: 1719
	public GameObjectRef LiftArrivalScreenBounce;

	// Token: 0x040006B8 RID: 1720
	public SoundDefinition liftMovementLoopDef;

	// Token: 0x040006B9 RID: 1721
	public SoundDefinition liftMovementStartDef;

	// Token: 0x040006BA RID: 1722
	public SoundDefinition liftMovementStopDef;

	// Token: 0x040006BB RID: 1723
	public SoundDefinition liftMovementAccentSoundDef;

	// Token: 0x040006BC RID: 1724
	public GameObjectRef liftButtonPressedEffect;

	// Token: 0x040006BD RID: 1725
	public float movementAccentMinInterval = 0.75f;

	// Token: 0x040006BE RID: 1726
	public float movementAccentMaxInterval = 3f;

	// Token: 0x040006BF RID: 1727
	private Sound liftMovementLoopSound;

	// Token: 0x040006C0 RID: 1728
	private float nextMovementAccent;

	// Token: 0x040006C1 RID: 1729
	private const BaseEntity.Flags PressedUp = BaseEntity.Flags.Reserved1;

	// Token: 0x040006C2 RID: 1730
	private const BaseEntity.Flags PressedDown = BaseEntity.Flags.Reserved2;
}
