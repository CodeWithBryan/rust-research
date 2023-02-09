using System;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020001EE RID: 494
public class FishNavigator : BaseNavigator
{
	// Token: 0x170001FB RID: 507
	// (get) Token: 0x060019EE RID: 6638 RVA: 0x000B91DF File Offset: 0x000B73DF
	// (set) Token: 0x060019EF RID: 6639 RVA: 0x000B91E7 File Offset: 0x000B73E7
	public BaseNpc NPC { get; private set; }

	// Token: 0x060019F0 RID: 6640 RVA: 0x000B91F0 File Offset: 0x000B73F0
	public override void Init(BaseCombatEntity entity, NavMeshAgent agent)
	{
		base.Init(entity, agent);
		this.NPC = (entity as BaseNpc);
	}

	// Token: 0x060019F1 RID: 6641 RVA: 0x000B9206 File Offset: 0x000B7406
	protected override bool SetCustomDestination(Vector3 pos, float speedFraction = 1f, float updateInterval = 0f)
	{
		if (!base.SetCustomDestination(pos, speedFraction, updateInterval))
		{
			return false;
		}
		base.Destination = pos;
		return true;
	}

	// Token: 0x060019F2 RID: 6642 RVA: 0x000B9220 File Offset: 0x000B7420
	protected override void UpdatePositionAndRotation(Vector3 moveToPosition, float delta)
	{
		base.transform.position = Vector3.MoveTowards(base.transform.position, moveToPosition, this.GetTargetSpeed() * delta);
		base.BaseEntity.ServerPosition = base.transform.localPosition;
		if (base.ReachedPosition(moveToPosition))
		{
			base.Stop();
			return;
		}
		this.UpdateRotation(moveToPosition, delta);
	}

	// Token: 0x060019F3 RID: 6643 RVA: 0x000B927F File Offset: 0x000B747F
	private void UpdateRotation(Vector3 moveToPosition, float delta)
	{
		base.BaseEntity.ServerRotation = Quaternion.LookRotation(Vector3Ex.Direction(moveToPosition, base.transform.position));
	}
}
