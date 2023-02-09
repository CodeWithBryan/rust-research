using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200007A RID: 122
public class HBHFSensor : BaseDetector
{
	// Token: 0x06000B95 RID: 2965 RVA: 0x000650CC File Offset: 0x000632CC
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("HBHFSensor.OnRpcMessage", 0))
		{
			if (rpc == 3206885720U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetIncludeAuth ");
				}
				using (TimeWarning.New("SetIncludeAuth", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(3206885720U, "SetIncludeAuth", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage includeAuth = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetIncludeAuth(includeAuth);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SetIncludeAuth");
					}
				}
				return true;
			}
			if (rpc == 2223203375U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetIncludeOthers ");
				}
				using (TimeWarning.New("SetIncludeOthers", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test(2223203375U, "SetIncludeOthers", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage includeOthers = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetIncludeOthers(includeOthers);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in SetIncludeOthers");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000B96 RID: 2966 RVA: 0x000653CC File Offset: 0x000635CC
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		return Mathf.Min(this.detectedPlayers, this.GetCurrentEnergy());
	}

	// Token: 0x06000B97 RID: 2967 RVA: 0x000653DF File Offset: 0x000635DF
	public override void OnObjects()
	{
		base.OnObjects();
		this.UpdatePassthroughAmount();
		base.InvokeRandomized(new Action(this.UpdatePassthroughAmount), 0f, 1f, 0.1f);
	}

	// Token: 0x06000B98 RID: 2968 RVA: 0x0006540E File Offset: 0x0006360E
	public override void OnEmpty()
	{
		base.OnEmpty();
		this.UpdatePassthroughAmount();
		base.CancelInvoke(new Action(this.UpdatePassthroughAmount));
	}

	// Token: 0x06000B99 RID: 2969 RVA: 0x00065430 File Offset: 0x00063630
	public void UpdatePassthroughAmount()
	{
		if (base.isClient)
		{
			return;
		}
		int num = this.detectedPlayers;
		this.detectedPlayers = 0;
		if (this.myTrigger.entityContents != null)
		{
			foreach (BaseEntity baseEntity in this.myTrigger.entityContents)
			{
				if (!(baseEntity == null) && baseEntity.IsVisible(base.transform.position + base.transform.forward * 0.1f, 10f))
				{
					BasePlayer component = baseEntity.GetComponent<BasePlayer>();
					bool flag = component.CanBuild();
					if ((!flag || this.ShouldIncludeAuthorized()) && (flag || this.ShouldIncludeOthers()) && component != null && component.IsAlive() && !component.IsSleeping() && component.isServer)
					{
						this.detectedPlayers++;
					}
				}
			}
		}
		if (num != this.detectedPlayers && this.IsPowered())
		{
			this.MarkDirty();
			if (this.detectedPlayers > num)
			{
				Effect.server.Run(this.detectUp.resourcePath, base.transform.position, Vector3.up, null, false);
				return;
			}
			if (this.detectedPlayers < num)
			{
				Effect.server.Run(this.detectDown.resourcePath, base.transform.position, Vector3.up, null, false);
			}
		}
	}

	// Token: 0x06000B9A RID: 2970 RVA: 0x000655B0 File Offset: 0x000637B0
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void SetIncludeAuth(BaseEntity.RPCMessage msg)
	{
		bool b = msg.read.Bit();
		if (msg.player.CanBuild() && this.IsPowered())
		{
			base.SetFlag(BaseEntity.Flags.Reserved3, b, false, true);
		}
	}

	// Token: 0x06000B9B RID: 2971 RVA: 0x000655EC File Offset: 0x000637EC
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsVisible(3f)]
	public void SetIncludeOthers(BaseEntity.RPCMessage msg)
	{
		bool b = msg.read.Bit();
		if (msg.player.CanBuild() && this.IsPowered())
		{
			base.SetFlag(BaseEntity.Flags.Reserved2, b, false, true);
		}
	}

	// Token: 0x06000B9C RID: 2972 RVA: 0x0002D546 File Offset: 0x0002B746
	public bool ShouldIncludeAuthorized()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved3);
	}

	// Token: 0x06000B9D RID: 2973 RVA: 0x00004C84 File Offset: 0x00002E84
	public bool ShouldIncludeOthers()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved2);
	}

	// Token: 0x0400076A RID: 1898
	public GameObjectRef detectUp;

	// Token: 0x0400076B RID: 1899
	public GameObjectRef detectDown;

	// Token: 0x0400076C RID: 1900
	public const BaseEntity.Flags Flag_IncludeOthers = BaseEntity.Flags.Reserved2;

	// Token: 0x0400076D RID: 1901
	public const BaseEntity.Flags Flag_IncludeAuthed = BaseEntity.Flags.Reserved3;

	// Token: 0x0400076E RID: 1902
	private int detectedPlayers;
}
