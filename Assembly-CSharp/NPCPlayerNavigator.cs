using System;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020001F0 RID: 496
public class NPCPlayerNavigator : BaseNavigator
{
	// Token: 0x170001FD RID: 509
	// (get) Token: 0x060019FF RID: 6655 RVA: 0x000B9474 File Offset: 0x000B7674
	// (set) Token: 0x06001A00 RID: 6656 RVA: 0x000B947C File Offset: 0x000B767C
	public NPCPlayer NPCPlayerEntity { get; private set; }

	// Token: 0x06001A01 RID: 6657 RVA: 0x000B9485 File Offset: 0x000B7685
	public override void Init(BaseCombatEntity entity, NavMeshAgent agent)
	{
		base.Init(entity, agent);
		this.NPCPlayerEntity = (entity as NPCPlayer);
	}

	// Token: 0x06001A02 RID: 6658 RVA: 0x000B949B File Offset: 0x000B769B
	protected override bool CanEnableNavMeshNavigation()
	{
		return base.CanEnableNavMeshNavigation() && (!this.NPCPlayerEntity.isMounted || this.CanNavigateMounted);
	}

	// Token: 0x06001A03 RID: 6659 RVA: 0x000B94C0 File Offset: 0x000B76C0
	protected override bool CanUpdateMovement()
	{
		if (!base.CanUpdateMovement())
		{
			return false;
		}
		if (this.NPCPlayerEntity.IsWounded())
		{
			return false;
		}
		if (base.CurrentNavigationType == BaseNavigator.NavigationType.NavMesh && (this.NPCPlayerEntity.IsDormant || !this.NPCPlayerEntity.syncPosition) && base.Agent.enabled)
		{
			base.SetDestination(this.NPCPlayerEntity.ServerPosition, 1f, 0f, 0f);
			return false;
		}
		return true;
	}

	// Token: 0x06001A04 RID: 6660 RVA: 0x000B953C File Offset: 0x000B773C
	protected override void UpdatePositionAndRotation(Vector3 moveToPosition, float delta)
	{
		base.UpdatePositionAndRotation(moveToPosition, delta);
		if (this.overrideFacingDirectionMode == BaseNavigator.OverrideFacingDirectionMode.None)
		{
			if (base.CurrentNavigationType == BaseNavigator.NavigationType.NavMesh)
			{
				this.NPCPlayerEntity.SetAimDirection(base.Agent.desiredVelocity.normalized);
				return;
			}
			if (base.CurrentNavigationType == BaseNavigator.NavigationType.AStar || base.CurrentNavigationType == BaseNavigator.NavigationType.Base)
			{
				this.NPCPlayerEntity.SetAimDirection(Vector3Ex.Direction2D(moveToPosition, base.transform.position));
			}
		}
	}

	// Token: 0x06001A05 RID: 6661 RVA: 0x000B95B0 File Offset: 0x000B77B0
	public override void ApplyFacingDirectionOverride()
	{
		base.ApplyFacingDirectionOverride();
		if (this.overrideFacingDirectionMode == BaseNavigator.OverrideFacingDirectionMode.None)
		{
			return;
		}
		if (this.overrideFacingDirectionMode == BaseNavigator.OverrideFacingDirectionMode.Direction)
		{
			this.NPCPlayerEntity.SetAimDirection(this.facingDirectionOverride);
			return;
		}
		if (this.facingDirectionEntity != null)
		{
			Vector3 aimDirection = NPCPlayerNavigator.GetAimDirection(this.NPCPlayerEntity, this.facingDirectionEntity);
			this.facingDirectionOverride = aimDirection;
			this.NPCPlayerEntity.SetAimDirection(this.facingDirectionOverride);
		}
	}

	// Token: 0x06001A06 RID: 6662 RVA: 0x000B9620 File Offset: 0x000B7820
	private static Vector3 GetAimDirection(BasePlayer aimingPlayer, BaseEntity target)
	{
		if (target == null)
		{
			return Vector3Ex.Direction2D(aimingPlayer.transform.position + aimingPlayer.eyes.BodyForward() * 1000f, aimingPlayer.transform.position);
		}
		if (Vector3Ex.Distance2D(aimingPlayer.transform.position, target.transform.position) <= 0.75f)
		{
			return Vector3Ex.Direction2D(target.transform.position, aimingPlayer.transform.position);
		}
		return (NPCPlayerNavigator.TargetAimPositionOffset(target) - aimingPlayer.eyes.position).normalized;
	}

	// Token: 0x06001A07 RID: 6663 RVA: 0x000B96C8 File Offset: 0x000B78C8
	private static Vector3 TargetAimPositionOffset(BaseEntity target)
	{
		BasePlayer basePlayer = target as BasePlayer;
		if (!(basePlayer != null))
		{
			return target.CenterPoint();
		}
		if (basePlayer.IsSleeping() || basePlayer.IsWounded())
		{
			return basePlayer.transform.position + Vector3.up * 0.1f;
		}
		return basePlayer.eyes.position - Vector3.up * 0.15f;
	}
}
