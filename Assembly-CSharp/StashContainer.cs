using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000D2 RID: 210
public class StashContainer : StorageContainer
{
	// Token: 0x06001251 RID: 4689 RVA: 0x00093120 File Offset: 0x00091320
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("StashContainer.OnRpcMessage", 0))
		{
			if (rpc == 4130263076U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_HideStash ");
				}
				using (TimeWarning.New("RPC_HideStash", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(4130263076U, "RPC_HideStash", this, player, 3f))
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
							this.RPC_HideStash(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_HideStash");
					}
				}
				return true;
			}
			if (rpc == 298671803U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_WantsUnhide ");
				}
				using (TimeWarning.New("RPC_WantsUnhide", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(298671803U, "RPC_WantsUnhide", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage rpc3 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_WantsUnhide(rpc3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_WantsUnhide");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001252 RID: 4690 RVA: 0x000035F8 File Offset: 0x000017F8
	public bool IsHidden()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved5);
	}

	// Token: 0x06001253 RID: 4691 RVA: 0x00093420 File Offset: 0x00091620
	public bool PlayerInRange(BasePlayer ply)
	{
		if (Vector3.Distance(base.transform.position, ply.transform.position) <= this.uncoverRange)
		{
			Vector3 normalized = (base.transform.position - ply.eyes.position).normalized;
			if (Vector3.Dot(ply.eyes.BodyForward(), normalized) > 0.95f)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001254 RID: 4692 RVA: 0x00093490 File Offset: 0x00091690
	public void DoOccludedCheck()
	{
		if (UnityEngine.Physics.SphereCast(new Ray(base.transform.position + Vector3.up * 5f, Vector3.down), 0.25f, 5f, 2097152))
		{
			base.DropItems(null);
			base.Kill(BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x06001255 RID: 4693 RVA: 0x000934EA File Offset: 0x000916EA
	public void OnPhysicsNeighbourChanged()
	{
		if (!base.IsInvoking(new Action(this.DoOccludedCheck)))
		{
			base.Invoke(new Action(this.DoOccludedCheck), UnityEngine.Random.Range(5f, 10f));
		}
	}

	// Token: 0x06001256 RID: 4694 RVA: 0x00093524 File Offset: 0x00091724
	public void SetHidden(bool isHidden)
	{
		if (UnityEngine.Time.realtimeSinceStartup - this.lastToggleTime < 3f)
		{
			return;
		}
		if (isHidden == base.HasFlag(BaseEntity.Flags.Reserved5))
		{
			return;
		}
		this.lastToggleTime = UnityEngine.Time.realtimeSinceStartup;
		base.Invoke(new Action(this.Decay), 259200f);
		if (base.isServer)
		{
			base.SetFlag(BaseEntity.Flags.Reserved5, isHidden, false, true);
		}
	}

	// Token: 0x06001257 RID: 4695 RVA: 0x0009358C File Offset: 0x0009178C
	public void DisableNetworking()
	{
		base.limitNetworking = true;
		base.SetFlag(BaseEntity.Flags.Disabled, true, false, true);
	}

	// Token: 0x06001258 RID: 4696 RVA: 0x000029D4 File Offset: 0x00000BD4
	public void Decay()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06001259 RID: 4697 RVA: 0x000935A0 File Offset: 0x000917A0
	public override void ServerInit()
	{
		base.ServerInit();
		this.SetHidden(false);
	}

	// Token: 0x0600125A RID: 4698 RVA: 0x000935AF File Offset: 0x000917AF
	public void ToggleHidden()
	{
		this.SetHidden(!this.IsHidden());
	}

	// Token: 0x0600125B RID: 4699 RVA: 0x000935C0 File Offset: 0x000917C0
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_HideStash(BaseEntity.RPCMessage rpc)
	{
		this.SetHidden(true);
	}

	// Token: 0x0600125C RID: 4700 RVA: 0x000935CC File Offset: 0x000917CC
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_WantsUnhide(BaseEntity.RPCMessage rpc)
	{
		if (!this.IsHidden())
		{
			return;
		}
		BasePlayer player = rpc.player;
		if (this.PlayerInRange(player))
		{
			this.SetHidden(false);
		}
	}

	// Token: 0x04000B7C RID: 2940
	public Transform visuals;

	// Token: 0x04000B7D RID: 2941
	public float burriedOffset;

	// Token: 0x04000B7E RID: 2942
	public float raisedOffset;

	// Token: 0x04000B7F RID: 2943
	public GameObjectRef buryEffect;

	// Token: 0x04000B80 RID: 2944
	public float uncoverRange = 3f;

	// Token: 0x04000B81 RID: 2945
	private float lastToggleTime;

	// Token: 0x02000BBC RID: 3004
	public static class StashContainerFlags
	{
		// Token: 0x04003F58 RID: 16216
		public const BaseEntity.Flags Hidden = BaseEntity.Flags.Reserved5;
	}
}
