using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000C2 RID: 194
public class SearchLight : global::IOEntity
{
	// Token: 0x06001120 RID: 4384 RVA: 0x0008B110 File Offset: 0x00089310
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SearchLight.OnRpcMessage", 0))
		{
			if (rpc == 3611615802U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_UseLight ");
				}
				using (TimeWarning.New("RPC_UseLight", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3611615802U, "RPC_UseLight", this, player, 3f))
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
							this.RPC_UseLight(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_UseLight");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001121 RID: 4385 RVA: 0x0008B278 File Offset: 0x00089478
	public override void ResetState()
	{
		this.aimDir = Vector3.zero;
	}

	// Token: 0x06001122 RID: 4386 RVA: 0x00020C77 File Offset: 0x0001EE77
	public override int ConsumptionAmount()
	{
		return 10;
	}

	// Token: 0x06001123 RID: 4387 RVA: 0x0008B285 File Offset: 0x00089485
	public bool IsMounted()
	{
		return this.mountedPlayer != null;
	}

	// Token: 0x06001124 RID: 4388 RVA: 0x0008B293 File Offset: 0x00089493
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.autoturret = Facepunch.Pool.Get<ProtoBuf.AutoTurret>();
		info.msg.autoturret.aimDir = this.aimDir;
	}

	// Token: 0x06001125 RID: 4389 RVA: 0x0008B2C2 File Offset: 0x000894C2
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.autoturret != null)
		{
			this.aimDir = info.msg.autoturret.aimDir;
		}
	}

	// Token: 0x06001126 RID: 4390 RVA: 0x0008B2EE File Offset: 0x000894EE
	public void PlayerEnter(global::BasePlayer player)
	{
		if (this.IsMounted() && player != this.mountedPlayer)
		{
			return;
		}
		this.PlayerExit();
		if (player != null)
		{
			this.mountedPlayer = player;
			base.SetFlag(global::BaseEntity.Flags.Reserved5, true, false, true);
		}
	}

	// Token: 0x06001127 RID: 4391 RVA: 0x0008B32B File Offset: 0x0008952B
	public void PlayerExit()
	{
		if (this.mountedPlayer)
		{
			this.mountedPlayer = null;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved5, false, false, true);
	}

	// Token: 0x06001128 RID: 4392 RVA: 0x0008B350 File Offset: 0x00089550
	public void MountedUpdate()
	{
		if (this.mountedPlayer == null || this.mountedPlayer.IsSleeping() || !this.mountedPlayer.IsAlive() || this.mountedPlayer.IsWounded() || Vector3.Distance(this.mountedPlayer.transform.position, base.transform.position) > 2f)
		{
			this.PlayerExit();
			return;
		}
		Vector3 targetAimpoint = this.eyePoint.transform.position + this.mountedPlayer.eyes.BodyForward() * 100f;
		this.SetTargetAimpoint(targetAimpoint);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001129 RID: 4393 RVA: 0x0008B400 File Offset: 0x00089600
	public void SetTargetAimpoint(Vector3 worldPos)
	{
		this.aimDir = (worldPos - this.eyePoint.transform.position).normalized;
	}

	// Token: 0x0600112A RID: 4394 RVA: 0x0008B431 File Offset: 0x00089631
	public override int GetCurrentEnergy()
	{
		if (this.currentEnergy >= this.ConsumptionAmount())
		{
			return base.GetCurrentEnergy();
		}
		return Mathf.Clamp(this.currentEnergy - base.ConsumptionAmount(), 0, this.currentEnergy);
	}

	// Token: 0x0600112B RID: 4395 RVA: 0x0008B464 File Offset: 0x00089664
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_UseLight(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		bool flag = msg.read.Bit();
		if (flag && this.IsMounted())
		{
			return;
		}
		if (this.needsBuildingPrivilegeToUse && !msg.player.CanBuild())
		{
			return;
		}
		if (flag)
		{
			this.PlayerEnter(player);
			return;
		}
		this.PlayerExit();
	}

	// Token: 0x0600112C RID: 4396 RVA: 0x0008B4B7 File Offset: 0x000896B7
	public override void OnKilled(HitInfo info)
	{
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
		base.OnKilled(info);
	}

	// Token: 0x0600112D RID: 4397 RVA: 0x0008B4CA File Offset: 0x000896CA
	public void Update()
	{
		if (base.isServer && this.IsMounted())
		{
			this.MountedUpdate();
		}
	}

	// Token: 0x04000AC5 RID: 2757
	public GameObject pitchObject;

	// Token: 0x04000AC6 RID: 2758
	public GameObject yawObject;

	// Token: 0x04000AC7 RID: 2759
	public GameObject eyePoint;

	// Token: 0x04000AC8 RID: 2760
	public SoundPlayer turnLoop;

	// Token: 0x04000AC9 RID: 2761
	public bool needsBuildingPrivilegeToUse = true;

	// Token: 0x04000ACA RID: 2762
	private Vector3 aimDir = Vector3.zero;

	// Token: 0x04000ACB RID: 2763
	private global::BasePlayer mountedPlayer;

	// Token: 0x02000BAF RID: 2991
	public static class SearchLightFlags
	{
		// Token: 0x04003F26 RID: 16166
		public const global::BaseEntity.Flags PlayerUsing = global::BaseEntity.Flags.Reserved5;
	}
}
