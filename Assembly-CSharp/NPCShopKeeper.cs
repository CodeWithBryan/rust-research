using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000192 RID: 402
public class NPCShopKeeper : NPCPlayer
{
	// Token: 0x0600175E RID: 5982 RVA: 0x000AEAAE File Offset: 0x000ACCAE
	public InvisibleVendingMachine GetVendingMachine()
	{
		if (!this.invisibleVendingMachineRef.IsValid(base.isServer))
		{
			return null;
		}
		return this.invisibleVendingMachineRef.Get(base.isServer).GetComponent<InvisibleVendingMachine>();
	}

	// Token: 0x0600175F RID: 5983 RVA: 0x000AEADC File Offset: 0x000ACCDC
	public void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawCube(base.transform.position + Vector3.up * 1f, new Vector3(0.5f, 1f, 0.5f));
	}

	// Token: 0x06001760 RID: 5984 RVA: 0x000059DD File Offset: 0x00003BDD
	public override void UpdateProtectionFromClothing()
	{
	}

	// Token: 0x06001761 RID: 5985 RVA: 0x000059DD File Offset: 0x00003BDD
	public override void Hurt(HitInfo info)
	{
	}

	// Token: 0x06001762 RID: 5986 RVA: 0x000AEB2C File Offset: 0x000ACD2C
	public override void ServerInit()
	{
		base.ServerInit();
		this.initialFacingDir = base.transform.rotation * Vector3.forward;
		base.Invoke(new Action(this.DelayedSleepEnd), 3f);
		this.SetAimDirection(base.transform.rotation * Vector3.forward);
		base.InvokeRandomized(new Action(this.Greeting), UnityEngine.Random.Range(5f, 10f), 5f, UnityEngine.Random.Range(0f, 2f));
		if (this.invisibleVendingMachineRef.IsValid(true) && this.machine == null)
		{
			this.machine = this.GetVendingMachine();
			return;
		}
		if (this.machine != null && !this.invisibleVendingMachineRef.IsValid(true))
		{
			this.invisibleVendingMachineRef.Set(this.machine);
		}
	}

	// Token: 0x06001763 RID: 5987 RVA: 0x000AEC17 File Offset: 0x000ACE17
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.shopKeeper = Pool.Get<ShopKeeper>();
		info.msg.shopKeeper.vendingRef = this.invisibleVendingMachineRef.uid;
	}

	// Token: 0x06001764 RID: 5988 RVA: 0x000AEC4B File Offset: 0x000ACE4B
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.shopKeeper != null)
		{
			this.invisibleVendingMachineRef.uid = info.msg.shopKeeper.vendingRef;
		}
	}

	// Token: 0x06001765 RID: 5989 RVA: 0x000AEC7C File Offset: 0x000ACE7C
	public override void PostServerLoad()
	{
		base.PostServerLoad();
	}

	// Token: 0x06001766 RID: 5990 RVA: 0x000AEC84 File Offset: 0x000ACE84
	public void DelayedSleepEnd()
	{
		this.EndSleeping();
	}

	// Token: 0x06001767 RID: 5991 RVA: 0x000AEC8C File Offset: 0x000ACE8C
	public void GreetPlayer(global::BasePlayer player)
	{
		if (player != null)
		{
			base.SignalBroadcast(global::BaseEntity.Signal.Gesture, "wave", null);
			this.SetAimDirection(Vector3Ex.Direction2D(player.eyes.position, this.eyes.position));
			this.lastWavedAtPlayer = player;
			return;
		}
		this.SetAimDirection(this.initialFacingDir);
	}

	// Token: 0x06001768 RID: 5992 RVA: 0x000AECE8 File Offset: 0x000ACEE8
	public void Greeting()
	{
		List<global::BasePlayer> list = Pool.GetList<global::BasePlayer>();
		Vis.Entities<global::BasePlayer>(base.transform.position, 10f, list, 131072, QueryTriggerInteraction.Collide);
		Vector3 position = base.transform.position;
		global::BasePlayer basePlayer = null;
		foreach (global::BasePlayer basePlayer2 in list)
		{
			if (!basePlayer2.isClient && !basePlayer2.IsNpc && !(basePlayer2 == this) && basePlayer2.IsVisible(this.eyes.position, float.PositiveInfinity) && !(basePlayer2 == this.lastWavedAtPlayer) && Vector3.Dot(Vector3Ex.Direction2D(basePlayer2.eyes.position, this.eyes.position), this.initialFacingDir) >= 0.2f)
			{
				basePlayer = basePlayer2;
				break;
			}
		}
		if (basePlayer == null && !list.Contains(this.lastWavedAtPlayer))
		{
			this.lastWavedAtPlayer = null;
		}
		if (basePlayer != null)
		{
			base.SignalBroadcast(global::BaseEntity.Signal.Gesture, "wave", null);
			this.SetAimDirection(Vector3Ex.Direction2D(basePlayer.eyes.position, this.eyes.position));
			this.lastWavedAtPlayer = basePlayer;
		}
		else
		{
			this.SetAimDirection(this.initialFacingDir);
		}
		Pool.FreeList<global::BasePlayer>(ref list);
	}

	// Token: 0x04001059 RID: 4185
	public EntityRef invisibleVendingMachineRef;

	// Token: 0x0400105A RID: 4186
	public InvisibleVendingMachine machine;

	// Token: 0x0400105B RID: 4187
	private float greetDir;

	// Token: 0x0400105C RID: 4188
	private Vector3 initialFacingDir;

	// Token: 0x0400105D RID: 4189
	private global::BasePlayer lastWavedAtPlayer;
}
