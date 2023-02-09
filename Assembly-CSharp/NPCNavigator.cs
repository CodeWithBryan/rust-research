using System;
using ConVar;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020001EF RID: 495
public class NPCNavigator : BaseNavigator
{
	// Token: 0x170001FC RID: 508
	// (get) Token: 0x060019F5 RID: 6645 RVA: 0x000B92AA File Offset: 0x000B74AA
	// (set) Token: 0x060019F6 RID: 6646 RVA: 0x000B92B2 File Offset: 0x000B74B2
	public BaseNpc NPC { get; private set; }

	// Token: 0x060019F7 RID: 6647 RVA: 0x000B92BB File Offset: 0x000B74BB
	public override void Init(BaseCombatEntity entity, NavMeshAgent agent)
	{
		base.Init(entity, agent);
		this.NPC = (entity as BaseNpc);
	}

	// Token: 0x060019F8 RID: 6648 RVA: 0x000B92D1 File Offset: 0x000B74D1
	protected override bool CanEnableNavMeshNavigation()
	{
		return base.CanEnableNavMeshNavigation();
	}

	// Token: 0x060019F9 RID: 6649 RVA: 0x000B92E0 File Offset: 0x000B74E0
	protected override bool CanUpdateMovement()
	{
		if (!base.CanUpdateMovement())
		{
			return false;
		}
		if (this.NPC != null && (this.NPC.IsDormant || !this.NPC.syncPosition) && base.Agent.enabled)
		{
			base.SetDestination(this.NPC.ServerPosition, 1f, 0f, 0f);
			return false;
		}
		return true;
	}

	// Token: 0x060019FA RID: 6650 RVA: 0x000B9350 File Offset: 0x000B7550
	protected override void UpdatePositionAndRotation(Vector3 moveToPosition, float delta)
	{
		base.UpdatePositionAndRotation(moveToPosition, delta);
		this.UpdateRotation(moveToPosition, delta);
	}

	// Token: 0x060019FB RID: 6651 RVA: 0x000B9364 File Offset: 0x000B7564
	private void UpdateRotation(Vector3 moveToPosition, float delta)
	{
		if (this.overrideFacingDirectionMode != BaseNavigator.OverrideFacingDirectionMode.None)
		{
			return;
		}
		if (this.traversingNavMeshLink)
		{
			Vector3 vector = base.Agent.destination - base.BaseEntity.ServerPosition;
			if (vector.sqrMagnitude > 1f)
			{
				vector = this.currentNavMeshLinkEndPos - base.BaseEntity.ServerPosition;
			}
			float sqrMagnitude = vector.sqrMagnitude;
			return;
		}
		if ((base.Agent.destination - base.BaseEntity.ServerPosition).sqrMagnitude > 1f)
		{
			Vector3 normalized = base.Agent.desiredVelocity.normalized;
			if (normalized.sqrMagnitude > 0.001f)
			{
				base.BaseEntity.ServerRotation = Quaternion.LookRotation(normalized);
				return;
			}
		}
	}

	// Token: 0x060019FC RID: 6652 RVA: 0x000B9430 File Offset: 0x000B7630
	public override void ApplyFacingDirectionOverride()
	{
		base.ApplyFacingDirectionOverride();
		base.BaseEntity.ServerRotation = Quaternion.LookRotation(base.FacingDirectionOverride);
	}

	// Token: 0x060019FD RID: 6653 RVA: 0x000B944E File Offset: 0x000B764E
	public override bool IsSwimming()
	{
		return AI.npcswimming && this.NPC != null && this.NPC.swimming;
	}
}
