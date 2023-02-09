using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200005D RID: 93
public class CustomDoorManipulator : DoorManipulator
{
	// Token: 0x060009B3 RID: 2483 RVA: 0x00059C64 File Offset: 0x00057E64
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CustomDoorManipulator.OnRpcMessage", 0))
		{
			if (rpc == 1224330484U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DoPair ");
				}
				using (TimeWarning.New("DoPair", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1224330484U, "DoPair", this, player, 3f))
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
							this.DoPair(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in DoPair");
					}
				}
				return true;
			}
			if (rpc == 3800726972U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ServerActionChange ");
				}
				using (TimeWarning.New("ServerActionChange", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(3800726972U, "ServerActionChange", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg3 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ServerActionChange(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in ServerActionChange");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060009B4 RID: 2484 RVA: 0x00007074 File Offset: 0x00005274
	public override bool PairWithLockedDoors()
	{
		return false;
	}

	// Token: 0x060009B5 RID: 2485 RVA: 0x00059F64 File Offset: 0x00058164
	public bool CanPlayerAdmin(BasePlayer player)
	{
		return player != null && player.CanBuild() && !base.IsOn();
	}

	// Token: 0x060009B6 RID: 2486 RVA: 0x00059F82 File Offset: 0x00058182
	public bool IsPaired()
	{
		return this.targetDoor != null;
	}

	// Token: 0x060009B7 RID: 2487 RVA: 0x00059F90 File Offset: 0x00058190
	public void RefreshDoor()
	{
		this.SetTargetDoor(this.targetDoor);
	}

	// Token: 0x060009B8 RID: 2488 RVA: 0x00059F9E File Offset: 0x0005819E
	private void OnPhysicsNeighbourChanged()
	{
		this.SetTargetDoor(this.targetDoor);
		base.Invoke(new Action(this.RefreshDoor), 0.1f);
	}

	// Token: 0x060009B9 RID: 2489 RVA: 0x00059FC3 File Offset: 0x000581C3
	public override void SetupInitialDoorConnection()
	{
		if (this.entityRef.IsValid(true) && this.targetDoor == null)
		{
			this.SetTargetDoor(this.entityRef.Get(true).GetComponent<Door>());
		}
	}

	// Token: 0x060009BA RID: 2490 RVA: 0x00059FF8 File Offset: 0x000581F8
	public override void DoActionDoorMissing()
	{
		this.SetTargetDoor(null);
	}

	// Token: 0x060009BB RID: 2491 RVA: 0x0005A004 File Offset: 0x00058204
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void DoPair(BaseEntity.RPCMessage msg)
	{
		Door targetDoor = this.targetDoor;
		Door door = base.FindDoor(this.PairWithLockedDoors());
		if (door != targetDoor)
		{
			this.SetTargetDoor(door);
		}
	}

	// Token: 0x060009BC RID: 2492 RVA: 0x000059DD File Offset: 0x00003BDD
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void ServerActionChange(BaseEntity.RPCMessage msg)
	{
	}
}
