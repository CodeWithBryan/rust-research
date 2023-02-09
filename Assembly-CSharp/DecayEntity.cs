using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x020003C4 RID: 964
public class DecayEntity : BaseCombatEntity
{
	// Token: 0x060020EA RID: 8426 RVA: 0x000D5510 File Offset: 0x000D3710
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.decayEntity = Facepunch.Pool.Get<ProtoBuf.DecayEntity>();
		info.msg.decayEntity.buildingID = this.buildingID;
		if (info.forDisk)
		{
			info.msg.decayEntity.decayTimer = this.decayTimer;
			info.msg.decayEntity.upkeepTimer = this.upkeepTimer;
		}
	}

	// Token: 0x060020EB RID: 8427 RVA: 0x000D5580 File Offset: 0x000D3780
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.decayEntity != null)
		{
			this.decayTimer = info.msg.decayEntity.decayTimer;
			this.upkeepTimer = info.msg.decayEntity.upkeepTimer;
			if (this.buildingID != info.msg.decayEntity.buildingID)
			{
				this.AttachToBuilding(info.msg.decayEntity.buildingID);
				if (info.fromDisk)
				{
					BuildingManager.server.LoadBuildingID(this.buildingID);
				}
			}
		}
	}

	// Token: 0x060020EC RID: 8428 RVA: 0x000D5613 File Offset: 0x000D3813
	public override void ResetState()
	{
		base.ResetState();
		this.buildingID = 0U;
		if (base.isServer)
		{
			this.decayTimer = 0f;
		}
	}

	// Token: 0x060020ED RID: 8429 RVA: 0x000D5635 File Offset: 0x000D3835
	public void AttachToBuilding(uint id)
	{
		if (base.isServer)
		{
			BuildingManager.server.Remove(this);
			this.buildingID = id;
			BuildingManager.server.Add(this);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x060020EE RID: 8430 RVA: 0x000D5663 File Offset: 0x000D3863
	public BuildingManager.Building GetBuilding()
	{
		if (base.isServer)
		{
			return BuildingManager.server.GetBuilding(this.buildingID);
		}
		return null;
	}

	// Token: 0x060020EF RID: 8431 RVA: 0x000D5680 File Offset: 0x000D3880
	public override BuildingPrivlidge GetBuildingPrivilege()
	{
		BuildingManager.Building building = this.GetBuilding();
		if (building != null)
		{
			return building.GetDominatingBuildingPrivilege();
		}
		return base.GetBuildingPrivilege();
	}

	// Token: 0x060020F0 RID: 8432 RVA: 0x000D56A4 File Offset: 0x000D38A4
	public void CalculateUpkeepCostAmounts(List<ItemAmount> itemAmounts, float multiplier)
	{
		if (this.upkeep == null)
		{
			return;
		}
		float num = this.upkeep.upkeepMultiplier * multiplier;
		if (num == 0f)
		{
			return;
		}
		List<ItemAmount> list = this.BuildCost();
		if (list == null)
		{
			return;
		}
		foreach (ItemAmount itemAmount in list)
		{
			if (itemAmount.itemDef.category == ItemCategory.Resources)
			{
				float num2 = itemAmount.amount * num;
				bool flag = false;
				foreach (ItemAmount itemAmount2 in itemAmounts)
				{
					if (itemAmount2.itemDef == itemAmount.itemDef)
					{
						itemAmount2.amount += num2;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					itemAmounts.Add(new ItemAmount(itemAmount.itemDef, num2));
				}
			}
		}
	}

	// Token: 0x060020F1 RID: 8433 RVA: 0x000D57B8 File Offset: 0x000D39B8
	public override void ServerInit()
	{
		base.ServerInit();
		this.decayVariance = UnityEngine.Random.Range(0.95f, 1f);
		this.decay = PrefabAttribute.server.Find<global::Decay>(this.prefabID);
		this.decayPoints = PrefabAttribute.server.FindAll<DecayPoint>(this.prefabID);
		this.upkeep = PrefabAttribute.server.Find<Upkeep>(this.prefabID);
		BuildingManager.server.Add(this);
		if (!Rust.Application.isLoadingSave)
		{
			BuildingManager.server.CheckMerge(this);
		}
		this.lastDecayTick = UnityEngine.Time.time;
	}

	// Token: 0x060020F2 RID: 8434 RVA: 0x000D584A File Offset: 0x000D3A4A
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		BuildingManager.server.Remove(this);
		BuildingManager.server.CheckSplit(this);
	}

	// Token: 0x060020F3 RID: 8435 RVA: 0x000D5868 File Offset: 0x000D3A68
	public virtual void AttachToBuilding(global::DecayEntity other)
	{
		if (other != null)
		{
			this.AttachToBuilding(other.buildingID);
			BuildingManager.server.CheckMerge(this);
			return;
		}
		global::BuildingBlock nearbyBuildingBlock = this.GetNearbyBuildingBlock();
		if (nearbyBuildingBlock)
		{
			this.AttachToBuilding(nearbyBuildingBlock.buildingID);
		}
	}

	// Token: 0x060020F4 RID: 8436 RVA: 0x000D58B4 File Offset: 0x000D3AB4
	public global::BuildingBlock GetNearbyBuildingBlock()
	{
		float num = float.MaxValue;
		global::BuildingBlock result = null;
		Vector3 position = base.PivotPoint();
		List<global::BuildingBlock> list = Facepunch.Pool.GetList<global::BuildingBlock>();
		global::Vis.Entities<global::BuildingBlock>(position, 1.5f, list, 2097152, QueryTriggerInteraction.Collide);
		for (int i = 0; i < list.Count; i++)
		{
			global::BuildingBlock buildingBlock = list[i];
			if (buildingBlock.isServer == base.isServer)
			{
				float num2 = buildingBlock.SqrDistance(position);
				if (!buildingBlock.grounded)
				{
					num2 += 1f;
				}
				if (num2 < num)
				{
					num = num2;
					result = buildingBlock;
				}
			}
		}
		Facepunch.Pool.FreeList<global::BuildingBlock>(ref list);
		return result;
	}

	// Token: 0x060020F5 RID: 8437 RVA: 0x000D5946 File Offset: 0x000D3B46
	public void ResetUpkeepTime()
	{
		this.upkeepTimer = 0f;
	}

	// Token: 0x060020F6 RID: 8438 RVA: 0x000D5953 File Offset: 0x000D3B53
	public void DecayTouch()
	{
		this.decayTimer = 0f;
	}

	// Token: 0x060020F7 RID: 8439 RVA: 0x000D5960 File Offset: 0x000D3B60
	public void AddUpkeepTime(float time)
	{
		this.upkeepTimer -= time;
	}

	// Token: 0x060020F8 RID: 8440 RVA: 0x000D5970 File Offset: 0x000D3B70
	public float GetProtectedSeconds()
	{
		return Mathf.Max(0f, -this.upkeepTimer);
	}

	// Token: 0x060020F9 RID: 8441 RVA: 0x000D5984 File Offset: 0x000D3B84
	public virtual void DecayTick()
	{
		if (this.decay == null)
		{
			return;
		}
		float num = UnityEngine.Time.time - this.lastDecayTick;
		if (num < ConVar.Decay.tick)
		{
			return;
		}
		this.lastDecayTick = UnityEngine.Time.time;
		if (!this.decay.ShouldDecay(this))
		{
			return;
		}
		float num2 = num * ConVar.Decay.scale;
		if (ConVar.Decay.upkeep)
		{
			this.upkeepTimer += num2;
			if (this.upkeepTimer > 0f)
			{
				BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege();
				if (buildingPrivilege != null)
				{
					this.upkeepTimer -= buildingPrivilege.PurchaseUpkeepTime(this, Mathf.Max(this.upkeepTimer, 600f));
				}
			}
			if (this.upkeepTimer < 1f)
			{
				if (base.healthFraction < 1f && ConVar.Decay.upkeep_heal_scale > 0f && base.SecondsSinceAttacked > 600f)
				{
					float num3 = num / this.decay.GetDecayDuration(this) * ConVar.Decay.upkeep_heal_scale;
					this.Heal(this.MaxHealth() * num3);
				}
				return;
			}
			this.upkeepTimer = 1f;
		}
		this.decayTimer += num2;
		if (this.decayTimer < this.decay.GetDecayDelay(this))
		{
			return;
		}
		using (TimeWarning.New("DecayTick", 0))
		{
			float num4 = 1f;
			if (ConVar.Decay.upkeep)
			{
				if (!this.BypassInsideDecayMultiplier && !this.IsOutside())
				{
					num4 *= ConVar.Decay.upkeep_inside_decay_scale;
				}
			}
			else
			{
				for (int i = 0; i < this.decayPoints.Length; i++)
				{
					DecayPoint decayPoint = this.decayPoints[i];
					if (decayPoint.IsOccupied(this))
					{
						num4 -= decayPoint.protection;
					}
				}
			}
			if (num4 > 0f)
			{
				float num5 = num2 / this.decay.GetDecayDuration(this) * this.MaxHealth();
				base.Hurt(num5 * num4 * this.decayVariance, DamageType.Decay, null, true);
			}
		}
	}

	// Token: 0x060020FA RID: 8442 RVA: 0x000D5B7C File Offset: 0x000D3D7C
	public override void OnRepairFinished()
	{
		base.OnRepairFinished();
		this.DecayTouch();
	}

	// Token: 0x060020FB RID: 8443 RVA: 0x000D5B8C File Offset: 0x000D3D8C
	public override void OnKilled(HitInfo info)
	{
		if (this.debrisPrefab.isValid)
		{
			global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.debrisPrefab.resourcePath, base.transform.position, base.transform.rotation, true);
			if (baseEntity)
			{
				baseEntity.Spawn();
			}
		}
		base.OnKilled(info);
	}

	// Token: 0x1700028E RID: 654
	// (get) Token: 0x060020FC RID: 8444 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool BypassInsideDecayMultiplier
	{
		get
		{
			return false;
		}
	}

	// Token: 0x04001976 RID: 6518
	public GameObjectRef debrisPrefab;

	// Token: 0x04001977 RID: 6519
	[NonSerialized]
	public uint buildingID;

	// Token: 0x04001978 RID: 6520
	private float decayTimer;

	// Token: 0x04001979 RID: 6521
	private float upkeepTimer;

	// Token: 0x0400197A RID: 6522
	private Upkeep upkeep;

	// Token: 0x0400197B RID: 6523
	private global::Decay decay;

	// Token: 0x0400197C RID: 6524
	private DecayPoint[] decayPoints;

	// Token: 0x0400197D RID: 6525
	private float lastDecayTick;

	// Token: 0x0400197E RID: 6526
	private float decayVariance = 1f;
}
