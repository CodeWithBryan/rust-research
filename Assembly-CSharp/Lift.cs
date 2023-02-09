using System;
using ConVar;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200008A RID: 138
public class Lift : AnimatedBuildingBlock
{
	// Token: 0x06000CD9 RID: 3289 RVA: 0x0006CEDC File Offset: 0x0006B0DC
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Lift.OnRpcMessage", 0))
		{
			if (rpc == 2657791441U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_UseLift ");
				}
				using (TimeWarning.New("RPC_UseLift", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(2657791441U, "RPC_UseLift", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpc2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_UseLift(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_UseLift");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000CDA RID: 3290 RVA: 0x0006D044 File Offset: 0x0006B244
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_UseLift(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		this.MoveUp();
	}

	// Token: 0x06000CDB RID: 3291 RVA: 0x0006D05A File Offset: 0x0006B25A
	private void MoveUp()
	{
		if (base.IsOpen())
		{
			return;
		}
		if (base.IsBusy())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.Open, true, false, true);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000CDC RID: 3292 RVA: 0x0006D07F File Offset: 0x0006B27F
	private void MoveDown()
	{
		if (!base.IsOpen())
		{
			return;
		}
		if (base.IsBusy())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.Open, false, false, true);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000CDD RID: 3293 RVA: 0x0006D0A4 File Offset: 0x0006B2A4
	protected override void OnAnimatorDisabled()
	{
		if (base.isServer && base.IsOpen())
		{
			base.Invoke(new Action(this.MoveDown), this.resetDelay);
		}
	}

	// Token: 0x06000CDE RID: 3294 RVA: 0x0006D0D0 File Offset: 0x0006B2D0
	public override void Spawn()
	{
		base.Spawn();
		if (!Rust.Application.isLoadingSave)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.triggerPrefab.resourcePath, Vector3.zero, Quaternion.identity, true);
			baseEntity.Spawn();
			baseEntity.SetParent(this, this.triggerBone, false, false);
		}
	}

	// Token: 0x04000826 RID: 2086
	public GameObjectRef triggerPrefab;

	// Token: 0x04000827 RID: 2087
	public string triggerBone;

	// Token: 0x04000828 RID: 2088
	public float resetDelay = 5f;
}
