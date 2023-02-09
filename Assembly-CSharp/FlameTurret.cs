using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using Rust;
using UnityEngine;

// Token: 0x02000071 RID: 113
public class FlameTurret : StorageContainer
{
	// Token: 0x06000ABF RID: 2751 RVA: 0x00060444 File Offset: 0x0005E644
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("FlameTurret.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000AC0 RID: 2752 RVA: 0x00020F08 File Offset: 0x0001F108
	public bool IsTriggered()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved4);
	}

	// Token: 0x06000AC1 RID: 2753 RVA: 0x00060484 File Offset: 0x0005E684
	public Vector3 GetEyePosition()
	{
		return this.eyeTransform.position;
	}

	// Token: 0x06000AC2 RID: 2754 RVA: 0x00060491 File Offset: 0x0005E691
	public override bool CanPickup(BasePlayer player)
	{
		return base.CanPickup(player) && !this.IsTriggered();
	}

	// Token: 0x06000AC3 RID: 2755 RVA: 0x000604A7 File Offset: 0x0005E6A7
	public void SetTriggered(bool triggered)
	{
		if (triggered && this.HasFuel())
		{
			this.triggeredTime = Time.realtimeSinceStartup;
		}
		base.SetFlag(BaseEntity.Flags.Reserved4, triggered && this.HasFuel(), false, true);
	}

	// Token: 0x06000AC4 RID: 2756 RVA: 0x000604D8 File Offset: 0x0005E6D8
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRepeating(new Action(this.SendAimDir), 0f, 0.1f);
	}

	// Token: 0x06000AC5 RID: 2757 RVA: 0x000604FC File Offset: 0x0005E6FC
	public void SendAimDir()
	{
		float delta = Time.realtimeSinceStartup - this.lastMovementUpdate;
		this.lastMovementUpdate = Time.realtimeSinceStartup;
		this.MovementUpdate(delta);
		base.ClientRPC<Vector3>(null, "CLIENT_ReceiveAimDir", this.aimDir);
		FlameTurret.updateFlameTurretQueueServer.Add(this);
	}

	// Token: 0x06000AC6 RID: 2758 RVA: 0x00060545 File Offset: 0x0005E745
	public float GetSpinSpeed()
	{
		return (float)(this.IsTriggered() ? 180 : 45);
	}

	// Token: 0x06000AC7 RID: 2759 RVA: 0x00060559 File Offset: 0x0005E759
	public override void OnAttacked(HitInfo info)
	{
		if (base.isClient)
		{
			return;
		}
		if (info.damageTypes.IsMeleeType())
		{
			this.SetTriggered(true);
		}
		base.OnAttacked(info);
	}

	// Token: 0x06000AC8 RID: 2760 RVA: 0x00060580 File Offset: 0x0005E780
	public void MovementUpdate(float delta)
	{
		this.aimDir += new Vector3(0f, delta * this.GetSpinSpeed(), 0f) * (float)this.turnDir;
		if (this.aimDir.y >= this.arc || this.aimDir.y <= -this.arc)
		{
			this.turnDir *= -1;
			this.aimDir.y = Mathf.Clamp(this.aimDir.y, -this.arc, this.arc);
		}
	}

	// Token: 0x06000AC9 RID: 2761 RVA: 0x00060620 File Offset: 0x0005E820
	public void ServerThink()
	{
		bool flag = this.IsTriggered();
		float delta = Time.realtimeSinceStartup - this.lastServerThink;
		this.lastServerThink = Time.realtimeSinceStartup;
		if (this.IsTriggered() && (Time.realtimeSinceStartup - this.triggeredTime > this.triggeredDuration || !this.HasFuel()))
		{
			this.SetTriggered(false);
		}
		if (!this.IsTriggered() && this.HasFuel() && this.CheckTrigger())
		{
			this.SetTriggered(true);
			Effect.server.Run(this.triggeredEffect.resourcePath, base.transform.position, Vector3.up, null, false);
		}
		if (flag != this.IsTriggered())
		{
			base.SendNetworkUpdateImmediate(false);
		}
		if (this.IsTriggered())
		{
			this.DoFlame(delta);
		}
	}

	// Token: 0x06000ACA RID: 2762 RVA: 0x000606D8 File Offset: 0x0005E8D8
	public bool CheckTrigger()
	{
		if (Time.realtimeSinceStartup < this.nextTriggerCheckTime)
		{
			return false;
		}
		this.nextTriggerCheckTime = Time.realtimeSinceStartup + 1f / this.triggerCheckRate;
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		HashSet<BaseEntity> entityContents = this.trigger.entityContents;
		bool flag = false;
		if (entityContents != null)
		{
			foreach (BaseEntity baseEntity in entityContents)
			{
				BasePlayer component = baseEntity.GetComponent<BasePlayer>();
				if (!component.IsSleeping() && component.IsAlive() && component.transform.position.y <= this.GetEyePosition().y + 0.5f && !component.IsBuildingAuthed())
				{
					list.Clear();
					GamePhysics.TraceAll(new Ray(component.eyes.position, (this.GetEyePosition() - component.eyes.position).normalized), 0f, list, 9f, 1218519297, QueryTriggerInteraction.UseGlobal, null);
					for (int i = 0; i < list.Count; i++)
					{
						BaseEntity entity = list[i].GetEntity();
						if (entity != null && (entity == this || entity.EqualNetID(this)))
						{
							flag = true;
							break;
						}
						if (!(entity != null) || entity.ShouldBlockProjectiles())
						{
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
			}
		}
		Pool.FreeList<RaycastHit>(ref list);
		return flag;
	}

	// Token: 0x06000ACB RID: 2763 RVA: 0x00060878 File Offset: 0x0005EA78
	public override void OnKilled(HitInfo info)
	{
		float num = (float)this.GetFuelAmount() / 500f;
		DamageUtil.RadiusDamage(this, base.LookupPrefab(), this.GetEyePosition(), 2f, 6f, this.damagePerSec, 133120, true);
		Effect.server.Run(this.explosionEffect.resourcePath, base.transform.position, Vector3.up, null, false);
		int num2 = Mathf.CeilToInt(Mathf.Clamp(num * 8f, 1f, 8f));
		for (int i = 0; i < num2; i++)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.fireballPrefab.resourcePath, base.transform.position, base.transform.rotation, true);
			if (baseEntity)
			{
				Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
				baseEntity.transform.position = base.transform.position + new Vector3(0f, 1.5f, 0f) + onUnitSphere * UnityEngine.Random.Range(-1f, 1f);
				baseEntity.Spawn();
				baseEntity.SetVelocity(onUnitSphere * (float)UnityEngine.Random.Range(3, 10));
			}
		}
		base.OnKilled(info);
	}

	// Token: 0x06000ACC RID: 2764 RVA: 0x000609B0 File Offset: 0x0005EBB0
	public int GetFuelAmount()
	{
		Item slot = base.inventory.GetSlot(0);
		if (slot == null || slot.amount < 1)
		{
			return 0;
		}
		return slot.amount;
	}

	// Token: 0x06000ACD RID: 2765 RVA: 0x000609DE File Offset: 0x0005EBDE
	public bool HasFuel()
	{
		return this.GetFuelAmount() > 0;
	}

	// Token: 0x06000ACE RID: 2766 RVA: 0x000609EC File Offset: 0x0005EBEC
	public bool UseFuel(float seconds)
	{
		Item slot = base.inventory.GetSlot(0);
		if (slot == null || slot.amount < 1)
		{
			return false;
		}
		this.pendingFuel += seconds * this.fuelPerSec;
		if (this.pendingFuel >= 1f)
		{
			int num = Mathf.FloorToInt(this.pendingFuel);
			slot.UseItem(num);
			this.pendingFuel -= (float)num;
		}
		return true;
	}

	// Token: 0x06000ACF RID: 2767 RVA: 0x00060A5C File Offset: 0x0005EC5C
	public void DoFlame(float delta)
	{
		if (!this.UseFuel(delta))
		{
			return;
		}
		Ray ray = new Ray(this.GetEyePosition(), base.transform.TransformDirection(Quaternion.Euler(this.aimDir) * Vector3.forward));
		Vector3 origin = ray.origin;
		RaycastHit raycastHit;
		bool flag = Physics.SphereCast(ray, 0.4f, out raycastHit, this.flameRange, 1218652417);
		if (!flag)
		{
			raycastHit.point = origin + ray.direction * this.flameRange;
		}
		float amount = this.damagePerSec[0].amount;
		this.damagePerSec[0].amount = amount * delta;
		DamageUtil.RadiusDamage(this, base.LookupPrefab(), raycastHit.point - ray.direction * 0.1f, this.flameRadius * 0.5f, this.flameRadius, this.damagePerSec, 2230272, true);
		DamageUtil.RadiusDamage(this, base.LookupPrefab(), base.transform.position + new Vector3(0f, 1.25f, 0f), 0.25f, 0.25f, this.damagePerSec, 133120, false);
		this.damagePerSec[0].amount = amount;
		if (Time.realtimeSinceStartup >= this.nextFireballTime)
		{
			this.nextFireballTime = Time.realtimeSinceStartup + UnityEngine.Random.Range(1f, 2f);
			Vector3 a = (UnityEngine.Random.Range(0, 10) <= 7 && flag) ? raycastHit.point : (ray.origin + ray.direction * (flag ? raycastHit.distance : this.flameRange) * UnityEngine.Random.Range(0.4f, 1f));
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.fireballPrefab.resourcePath, a - ray.direction * 0.25f, default(Quaternion), true);
			if (baseEntity)
			{
				baseEntity.creatorEntity = this;
				baseEntity.Spawn();
			}
		}
	}

	// Token: 0x040006FC RID: 1788
	public static FlameTurret.UpdateFlameTurretWorkQueue updateFlameTurretQueueServer = new FlameTurret.UpdateFlameTurretWorkQueue();

	// Token: 0x040006FD RID: 1789
	public Transform upper;

	// Token: 0x040006FE RID: 1790
	public Vector3 aimDir;

	// Token: 0x040006FF RID: 1791
	public float arc = 45f;

	// Token: 0x04000700 RID: 1792
	public float triggeredDuration = 5f;

	// Token: 0x04000701 RID: 1793
	public float flameRange = 7f;

	// Token: 0x04000702 RID: 1794
	public float flameRadius = 4f;

	// Token: 0x04000703 RID: 1795
	public float fuelPerSec = 1f;

	// Token: 0x04000704 RID: 1796
	public Transform eyeTransform;

	// Token: 0x04000705 RID: 1797
	public List<DamageTypeEntry> damagePerSec;

	// Token: 0x04000706 RID: 1798
	public GameObjectRef triggeredEffect;

	// Token: 0x04000707 RID: 1799
	public GameObjectRef fireballPrefab;

	// Token: 0x04000708 RID: 1800
	public GameObjectRef explosionEffect;

	// Token: 0x04000709 RID: 1801
	public TargetTrigger trigger;

	// Token: 0x0400070A RID: 1802
	private float nextFireballTime;

	// Token: 0x0400070B RID: 1803
	private int turnDir = 1;

	// Token: 0x0400070C RID: 1804
	private float lastMovementUpdate;

	// Token: 0x0400070D RID: 1805
	private float triggeredTime;

	// Token: 0x0400070E RID: 1806
	private float lastServerThink;

	// Token: 0x0400070F RID: 1807
	private float triggerCheckRate = 2f;

	// Token: 0x04000710 RID: 1808
	private float nextTriggerCheckTime;

	// Token: 0x04000711 RID: 1809
	private float pendingFuel;

	// Token: 0x02000B82 RID: 2946
	public class UpdateFlameTurretWorkQueue : ObjectWorkQueue<FlameTurret>
	{
		// Token: 0x06004AD5 RID: 19157 RVA: 0x00190DEB File Offset: 0x0018EFEB
		protected override void RunJob(FlameTurret entity)
		{
			if (!this.ShouldAdd(entity))
			{
				return;
			}
			entity.ServerThink();
		}

		// Token: 0x06004AD6 RID: 19158 RVA: 0x00190DFD File Offset: 0x0018EFFD
		protected override bool ShouldAdd(FlameTurret entity)
		{
			return base.ShouldAdd(entity) && entity.IsValid();
		}
	}
}
