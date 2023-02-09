using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using Rust;
using UnityEngine;

// Token: 0x02000053 RID: 83
public class CeilingLight : IOEntity
{
	// Token: 0x06000924 RID: 2340 RVA: 0x00055A7C File Offset: 0x00053C7C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CeilingLight.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000925 RID: 2341 RVA: 0x00055ABC File Offset: 0x00053CBC
	public override int ConsumptionAmount()
	{
		if (base.IsOn())
		{
			return 2;
		}
		return base.ConsumptionAmount();
	}

	// Token: 0x06000926 RID: 2342 RVA: 0x00055AD0 File Offset: 0x00053CD0
	public override void Hurt(HitInfo info)
	{
		if (base.isServer)
		{
			if (info.damageTypes.Has(DamageType.Explosion))
			{
				base.ClientRPC<int, Vector3, Vector3>(null, "ClientPhysPush", 0, info.attackNormal * 3f * (info.damageTypes.Total() / 50f), info.HitPositionWorld);
			}
			base.Hurt(info);
		}
	}

	// Token: 0x06000927 RID: 2343 RVA: 0x00055B34 File Offset: 0x00053D34
	public void RefreshGrowables()
	{
		List<GrowableEntity> list = Facepunch.Pool.GetList<GrowableEntity>();
		global::Vis.Entities<GrowableEntity>(base.transform.position + new Vector3(0f, -ConVar.Server.ceilingLightHeightOffset, 0f), ConVar.Server.ceilingLightGrowableRange, list, 512, QueryTriggerInteraction.Collide);
		List<PlanterBox> list2 = Facepunch.Pool.GetList<PlanterBox>();
		foreach (GrowableEntity growableEntity in list)
		{
			if (growableEntity.isServer)
			{
				PlanterBox planter = growableEntity.GetPlanter();
				if (planter != null && !list2.Contains(planter))
				{
					list2.Add(planter);
					planter.ForceLightUpdate();
				}
				growableEntity.CalculateQualities(false, true, false);
				growableEntity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			}
		}
		Facepunch.Pool.FreeList<PlanterBox>(ref list2);
		Facepunch.Pool.FreeList<GrowableEntity>(ref list);
	}

	// Token: 0x06000928 RID: 2344 RVA: 0x00055C10 File Offset: 0x00053E10
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		bool flag = base.IsOn();
		base.SetFlag(BaseEntity.Flags.On, this.IsPowered(), false, true);
		if (flag != base.IsOn())
		{
			if (base.IsOn())
			{
				this.LightsOn();
				return;
			}
			this.LightsOff();
		}
	}

	// Token: 0x06000929 RID: 2345 RVA: 0x00055C5C File Offset: 0x00053E5C
	public void LightsOn()
	{
		this.RefreshGrowables();
	}

	// Token: 0x0600092A RID: 2346 RVA: 0x00055C5C File Offset: 0x00053E5C
	public void LightsOff()
	{
		this.RefreshGrowables();
	}

	// Token: 0x0600092B RID: 2347 RVA: 0x00055C64 File Offset: 0x00053E64
	public override void OnKilled(HitInfo info)
	{
		base.OnKilled(info);
		this.RefreshGrowables();
	}

	// Token: 0x0600092C RID: 2348 RVA: 0x00055C74 File Offset: 0x00053E74
	public override void OnAttacked(HitInfo info)
	{
		float d = 3f * (info.damageTypes.Total() / 50f);
		base.ClientRPC<uint, Vector3, Vector3>(null, "ClientPhysPush", (info.Initiator != null && info.Initiator is BasePlayer && !info.IsPredicting) ? info.Initiator.net.ID : 0U, info.attackNormal * d, info.HitPositionWorld);
		base.OnAttacked(info);
	}

	// Token: 0x04000604 RID: 1540
	public float pushScale = 2f;
}
