using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000BD RID: 189
public class RHIB : MotorRowboat
{
	// Token: 0x060010C5 RID: 4293 RVA: 0x00088BE0 File Offset: 0x00086DE0
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("RHIB.OnRpcMessage", 0))
		{
			if (rpc == 1382282393U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_Release ");
				}
				using (TimeWarning.New("Server_Release", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(1382282393U, "Server_Release", this, player, 6f))
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
							this.Server_Release(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Server_Release");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060010C6 RID: 4294 RVA: 0x00088D48 File Offset: 0x00086F48
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(6f)]
	public void Server_Release(BaseEntity.RPCMessage msg)
	{
		if (base.GetParentEntity() == null)
		{
			return;
		}
		base.SetParent(null, true, true);
		base.SetToNonKinematic();
	}

	// Token: 0x060010C7 RID: 4295 RVA: 0x00088D68 File Offset: 0x00086F68
	public override void VehicleFixedUpdate()
	{
		this.gasPedal = Mathf.MoveTowards(this.gasPedal, this.targetGasPedal, UnityEngine.Time.fixedDeltaTime * 1f);
		base.VehicleFixedUpdate();
	}

	// Token: 0x060010C8 RID: 4296 RVA: 0x00088D92 File Offset: 0x00086F92
	public override bool EngineOn()
	{
		return base.EngineOn();
	}

	// Token: 0x060010C9 RID: 4297 RVA: 0x00088D9C File Offset: 0x00086F9C
	public override void DriverInput(InputState inputState, BasePlayer player)
	{
		base.DriverInput(inputState, player);
		if (inputState.IsDown(BUTTON.FORWARD))
		{
			this.targetGasPedal = 1f;
		}
		else if (inputState.IsDown(BUTTON.BACKWARD))
		{
			this.targetGasPedal = -0.5f;
		}
		else
		{
			this.targetGasPedal = 0f;
		}
		if (inputState.IsDown(BUTTON.LEFT))
		{
			this.steering = 1f;
			return;
		}
		if (inputState.IsDown(BUTTON.RIGHT))
		{
			this.steering = -1f;
			return;
		}
		this.steering = 0f;
	}

	// Token: 0x060010CA RID: 4298 RVA: 0x00088E20 File Offset: 0x00087020
	public void AddFuel(int amount)
	{
		StorageContainer storageContainer = this.fuelSystem.fuelStorageInstance.Get(true);
		if (storageContainer)
		{
			storageContainer.GetComponent<StorageContainer>().inventory.AddItem(ItemManager.FindItemDefinition("lowgradefuel"), amount, 0UL, ItemContainer.LimitStack.Existing);
		}
	}

	// Token: 0x04000A6F RID: 2671
	public GameObject steeringWheel;

	// Token: 0x04000A70 RID: 2672
	[ServerVar(Help = "Population active on the server", ShowInAdminUI = true)]
	public static float rhibpopulation;

	// Token: 0x04000A71 RID: 2673
	private float targetGasPedal;
}
