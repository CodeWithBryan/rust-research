using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x020003F8 RID: 1016
public class HelicopterDebris : ServerGib
{
	// Token: 0x06002214 RID: 8724 RVA: 0x000DA4CB File Offset: 0x000D86CB
	public override void ServerInit()
	{
		base.ServerInit();
		this.tooHotUntil = Time.realtimeSinceStartup + 480f;
	}

	// Token: 0x06002215 RID: 8725 RVA: 0x000DA4E4 File Offset: 0x000D86E4
	public override void PhysicsInit(Mesh mesh)
	{
		base.PhysicsInit(mesh);
		if (base.isServer)
		{
			this.resourceDispenser = base.GetComponent<ResourceDispenser>();
			float num = Mathf.Clamp01(base.GetComponent<Rigidbody>().mass / this.massReductionScalar);
			this.resourceDispenser.containedItems = new List<ItemAmount>();
			if (num > 0.75f && this.hqMetal != null)
			{
				this.resourceDispenser.containedItems.Add(new ItemAmount(this.hqMetal, (float)Mathf.CeilToInt(7f * num)));
			}
			if (num > 0f)
			{
				if (this.metalFragments != null)
				{
					this.resourceDispenser.containedItems.Add(new ItemAmount(this.metalFragments, (float)Mathf.CeilToInt(150f * num)));
				}
				if (this.charcoal != null)
				{
					this.resourceDispenser.containedItems.Add(new ItemAmount(this.charcoal, (float)Mathf.CeilToInt(80f * num)));
				}
			}
			this.resourceDispenser.Initialize();
		}
	}

	// Token: 0x06002216 RID: 8726 RVA: 0x000DA5F4 File Offset: 0x000D87F4
	public bool IsTooHot()
	{
		return this.tooHotUntil > Time.realtimeSinceStartup;
	}

	// Token: 0x06002217 RID: 8727 RVA: 0x000DA604 File Offset: 0x000D8804
	public override void OnAttacked(HitInfo info)
	{
		if (this.IsTooHot() && info.WeaponPrefab is BaseMelee)
		{
			if (info.Initiator is BasePlayer)
			{
				HitInfo hitInfo = new HitInfo();
				hitInfo.damageTypes.Add(DamageType.Heat, 5f);
				hitInfo.DoHitEffects = true;
				hitInfo.DidHit = true;
				hitInfo.HitBone = 0U;
				hitInfo.Initiator = this;
				hitInfo.PointStart = base.transform.position;
				Effect.server.Run("assets/bundled/prefabs/fx/impacts/additive/fire.prefab", info.Initiator, 0U, new Vector3(0f, 1f, 0f), Vector3.up, null, false);
				return;
			}
		}
		else
		{
			if (this.resourceDispenser)
			{
				this.resourceDispenser.OnAttacked(info);
			}
			base.OnAttacked(info);
		}
	}

	// Token: 0x04001A9E RID: 6814
	public ItemDefinition metalFragments;

	// Token: 0x04001A9F RID: 6815
	public ItemDefinition hqMetal;

	// Token: 0x04001AA0 RID: 6816
	public ItemDefinition charcoal;

	// Token: 0x04001AA1 RID: 6817
	[Tooltip("Divide mass by this amount to produce a scalar of resources, default = 5")]
	public float massReductionScalar = 5f;

	// Token: 0x04001AA2 RID: 6818
	private ResourceDispenser resourceDispenser;

	// Token: 0x04001AA3 RID: 6819
	private float tooHotUntil;
}
