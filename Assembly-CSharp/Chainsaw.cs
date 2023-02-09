using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000054 RID: 84
public class Chainsaw : BaseMelee
{
	// Token: 0x0600092E RID: 2350 RVA: 0x00055D08 File Offset: 0x00053F08
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("Chainsaw.OnRpcMessage", 0))
		{
			if (rpc == 3381353917U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DoReload ");
				}
				using (TimeWarning.New("DoReload", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(3381353917U, "DoReload", this, player))
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
							this.DoReload(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in DoReload");
					}
				}
				return true;
			}
			if (rpc == 706698034U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_SetAttacking ");
				}
				using (TimeWarning.New("Server_SetAttacking", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(706698034U, "Server_SetAttacking", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_SetAttacking(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in Server_SetAttacking");
					}
				}
				return true;
			}
			if (rpc == 3881794867U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_StartEngine ");
				}
				using (TimeWarning.New("Server_StartEngine", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(3881794867U, "Server_StartEngine", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg4 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_StartEngine(msg4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in Server_StartEngine");
					}
				}
				return true;
			}
			if (rpc == 841093980U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_StopEngine ");
				}
				using (TimeWarning.New("Server_StopEngine", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(841093980U, "Server_StopEngine", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg5 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_StopEngine(msg5);
						}
					}
					catch (Exception exception4)
					{
						Debug.LogException(exception4);
						player.Kick("RPC Error in Server_StopEngine");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600092F RID: 2351 RVA: 0x0002782C File Offset: 0x00025A2C
	public bool EngineOn()
	{
		return base.HasFlag(global::BaseEntity.Flags.On);
	}

	// Token: 0x06000930 RID: 2352 RVA: 0x00027875 File Offset: 0x00025A75
	public bool IsAttacking()
	{
		return base.HasFlag(global::BaseEntity.Flags.Busy);
	}

	// Token: 0x06000931 RID: 2353 RVA: 0x000562AC File Offset: 0x000544AC
	public void ServerNPCStart()
	{
		if (base.HasFlag(global::BaseEntity.Flags.On))
		{
			return;
		}
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer != null && ownerPlayer.IsNpc)
		{
			this.DoReload(default(global::BaseEntity.RPCMessage));
			this.SetEngineStatus(true);
			base.SendNetworkUpdateImmediate(false);
		}
	}

	// Token: 0x06000932 RID: 2354 RVA: 0x000562F8 File Offset: 0x000544F8
	public override void ServerUse()
	{
		base.ServerUse();
		this.SetAttackStatus(true);
		base.Invoke(new Action(this.DelayedStopAttack), this.attackSpacing + 0.5f);
	}

	// Token: 0x06000933 RID: 2355 RVA: 0x00056325 File Offset: 0x00054525
	public override void ServerUse_OnHit(HitInfo info)
	{
		this.EnableHitEffect(info.HitMaterial);
	}

	// Token: 0x06000934 RID: 2356 RVA: 0x00056333 File Offset: 0x00054533
	private void DelayedStopAttack()
	{
		this.SetAttackStatus(false);
	}

	// Token: 0x06000935 RID: 2357 RVA: 0x0005633C File Offset: 0x0005453C
	protected override bool VerifyClientAttack(global::BasePlayer player)
	{
		return this.EngineOn() && this.IsAttacking() && base.VerifyClientAttack(player);
	}

	// Token: 0x06000936 RID: 2358 RVA: 0x00056357 File Offset: 0x00054557
	public override void CollectedForCrafting(global::Item item, global::BasePlayer crafter)
	{
		this.ServerCommand(item, "unload_ammo", crafter);
	}

	// Token: 0x06000937 RID: 2359 RVA: 0x00056366 File Offset: 0x00054566
	public override void SetHeld(bool bHeld)
	{
		if (!bHeld)
		{
			this.SetEngineStatus(false);
		}
		base.SetHeld(bHeld);
	}

	// Token: 0x06000938 RID: 2360 RVA: 0x0005637C File Offset: 0x0005457C
	public void ReduceAmmo(float firingTime)
	{
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer != null && ownerPlayer.IsNpc)
		{
			return;
		}
		this.ammoRemainder += firingTime;
		if (this.ammoRemainder >= 1f)
		{
			int num = Mathf.FloorToInt(this.ammoRemainder);
			this.ammoRemainder -= (float)num;
			if (this.ammoRemainder >= 1f)
			{
				num++;
				this.ammoRemainder -= 1f;
			}
			this.ammo -= num;
			if (this.ammo <= 0)
			{
				this.ammo = 0;
			}
		}
		if ((float)this.ammo <= 0f)
		{
			this.SetEngineStatus(false);
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000939 RID: 2361 RVA: 0x00056438 File Offset: 0x00054638
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	public void DoReload(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return;
		}
		if (this.IsAttacking())
		{
			return;
		}
		global::Item item;
		while (this.ammo < this.maxAmmo && (item = this.GetAmmo()) != null && item.amount > 0)
		{
			int num = Mathf.Min(this.maxAmmo - this.ammo, item.amount);
			this.ammo += num;
			item.UseItem(num);
		}
		base.SendNetworkUpdateImmediate(false);
		ItemManager.DoRemoves();
		ownerPlayer.inventory.ServerUpdate(0f);
	}

	// Token: 0x0600093A RID: 2362 RVA: 0x000564CC File Offset: 0x000546CC
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.baseProjectile = Facepunch.Pool.Get<ProtoBuf.BaseProjectile>();
		info.msg.baseProjectile.primaryMagazine = Facepunch.Pool.Get<Magazine>();
		info.msg.baseProjectile.primaryMagazine.contents = this.ammo;
	}

	// Token: 0x0600093B RID: 2363 RVA: 0x00056520 File Offset: 0x00054720
	public void SetEngineStatus(bool status)
	{
		base.SetFlag(global::BaseEntity.Flags.On, status, false, true);
		if (!status)
		{
			this.SetAttackStatus(false);
		}
		base.CancelInvoke(new Action(this.EngineTick));
		if (status)
		{
			base.InvokeRepeating(new Action(this.EngineTick), 0f, 1f);
		}
	}

	// Token: 0x0600093C RID: 2364 RVA: 0x00056574 File Offset: 0x00054774
	public void SetAttackStatus(bool status)
	{
		if (!this.EngineOn())
		{
			status = false;
		}
		base.SetFlag(global::BaseEntity.Flags.Busy, status, false, true);
		base.CancelInvoke(new Action(this.AttackTick));
		if (status)
		{
			base.InvokeRepeating(new Action(this.AttackTick), 0f, 1f);
		}
	}

	// Token: 0x0600093D RID: 2365 RVA: 0x000565CB File Offset: 0x000547CB
	public void EngineTick()
	{
		this.ReduceAmmo(0.05f);
	}

	// Token: 0x0600093E RID: 2366 RVA: 0x000565D8 File Offset: 0x000547D8
	public void AttackTick()
	{
		this.ReduceAmmo(this.fuelPerSec);
	}

	// Token: 0x0600093F RID: 2367 RVA: 0x000565E8 File Offset: 0x000547E8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	public void Server_StartEngine(global::BaseEntity.RPCMessage msg)
	{
		if (this.ammo <= 0 || this.EngineOn())
		{
			return;
		}
		this.ReduceAmmo(0.25f);
		bool flag = UnityEngine.Random.Range(0f, 1f) <= this.engineStartChance;
		if (!flag)
		{
			this.failedAttempts++;
		}
		if (flag || this.failedAttempts >= 3)
		{
			this.failedAttempts = 0;
			this.SetEngineStatus(true);
			base.SendNetworkUpdateImmediate(false);
		}
	}

	// Token: 0x06000940 RID: 2368 RVA: 0x0005665D File Offset: 0x0005485D
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	public void Server_StopEngine(global::BaseEntity.RPCMessage msg)
	{
		this.SetEngineStatus(false);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000941 RID: 2369 RVA: 0x00056670 File Offset: 0x00054870
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	public void Server_SetAttacking(global::BaseEntity.RPCMessage msg)
	{
		bool flag = msg.read.Bit();
		if (this.IsAttacking() == flag)
		{
			return;
		}
		if (!this.EngineOn())
		{
			return;
		}
		this.SetAttackStatus(flag);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000942 RID: 2370 RVA: 0x000566AC File Offset: 0x000548AC
	public override void ServerCommand(global::Item item, string command, global::BasePlayer player)
	{
		if (item == null)
		{
			return;
		}
		if (command == "unload_ammo")
		{
			int num = this.ammo;
			if (num > 0)
			{
				this.ammo = 0;
				base.SendNetworkUpdateImmediate(false);
				global::Item item2 = ItemManager.Create(this.fuelType, num, 0UL);
				if (!item2.MoveToContainer(player.inventory.containerMain, -1, true, false, null, true))
				{
					item2.Drop(player.GetDropPosition(), player.GetDropVelocity(), default(Quaternion));
				}
			}
		}
	}

	// Token: 0x06000943 RID: 2371 RVA: 0x00056726 File Offset: 0x00054926
	public void DisableHitEffects()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved6, false, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved7, false, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved8, false, false, true);
		base.SendNetworkUpdateImmediate(false);
	}

	// Token: 0x06000944 RID: 2372 RVA: 0x0005675C File Offset: 0x0005495C
	public void EnableHitEffect(uint hitMaterial)
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved6, false, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved7, false, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved8, false, false, true);
		if (hitMaterial == StringPool.Get("Flesh"))
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved8, true, false, true);
		}
		else if (hitMaterial == StringPool.Get("Wood"))
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved7, true, false, true);
		}
		else
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved6, true, false, true);
		}
		base.SendNetworkUpdateImmediate(false);
		base.CancelInvoke(new Action(this.DisableHitEffects));
		base.Invoke(new Action(this.DisableHitEffects), 0.5f);
	}

	// Token: 0x06000945 RID: 2373 RVA: 0x0005680B File Offset: 0x00054A0B
	public override void DoAttackShared(HitInfo info)
	{
		base.DoAttackShared(info);
		if (base.isServer)
		{
			this.EnableHitEffect(info.HitMaterial);
		}
	}

	// Token: 0x06000946 RID: 2374 RVA: 0x00056828 File Offset: 0x00054A28
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseProjectile != null && info.msg.baseProjectile.primaryMagazine != null)
		{
			this.ammo = info.msg.baseProjectile.primaryMagazine.contents;
		}
	}

	// Token: 0x06000947 RID: 2375 RVA: 0x00056876 File Offset: 0x00054A76
	public bool HasAmmo()
	{
		return this.GetAmmo() != null;
	}

	// Token: 0x06000948 RID: 2376 RVA: 0x00056884 File Offset: 0x00054A84
	public global::Item GetAmmo()
	{
		global::BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return null;
		}
		global::Item item = ownerPlayer.inventory.containerMain.FindItemsByItemName(this.fuelType.shortname);
		if (item == null)
		{
			item = ownerPlayer.inventory.containerBelt.FindItemsByItemName(this.fuelType.shortname);
		}
		return item;
	}

	// Token: 0x04000605 RID: 1541
	public float attackFadeInTime = 0.1f;

	// Token: 0x04000606 RID: 1542
	public float attackFadeInDelay = 0.1f;

	// Token: 0x04000607 RID: 1543
	public float attackFadeOutTime = 0.1f;

	// Token: 0x04000608 RID: 1544
	public float idleFadeInTimeFromOff = 0.1f;

	// Token: 0x04000609 RID: 1545
	public float idleFadeInTimeFromAttack = 0.3f;

	// Token: 0x0400060A RID: 1546
	public float idleFadeInDelay = 0.1f;

	// Token: 0x0400060B RID: 1547
	public float idleFadeOutTime = 0.1f;

	// Token: 0x0400060C RID: 1548
	public Renderer chainRenderer;

	// Token: 0x0400060D RID: 1549
	private MaterialPropertyBlock block;

	// Token: 0x0400060E RID: 1550
	private Vector2 saveST;

	// Token: 0x0400060F RID: 1551
	[Header("Chainsaw")]
	public float fuelPerSec = 1f;

	// Token: 0x04000610 RID: 1552
	public int maxAmmo = 100;

	// Token: 0x04000611 RID: 1553
	public int ammo = 100;

	// Token: 0x04000612 RID: 1554
	public ItemDefinition fuelType;

	// Token: 0x04000613 RID: 1555
	public float reloadDuration = 2.5f;

	// Token: 0x04000614 RID: 1556
	[Header("Sounds")]
	public SoundPlayer idleLoop;

	// Token: 0x04000615 RID: 1557
	public SoundPlayer attackLoopAir;

	// Token: 0x04000616 RID: 1558
	public SoundPlayer revUp;

	// Token: 0x04000617 RID: 1559
	public SoundPlayer revDown;

	// Token: 0x04000618 RID: 1560
	public SoundPlayer offSound;

	// Token: 0x04000619 RID: 1561
	private int failedAttempts;

	// Token: 0x0400061A RID: 1562
	public float engineStartChance = 0.33f;

	// Token: 0x0400061B RID: 1563
	private float ammoRemainder;
}
