using System;
using Rust;
using UnityEngine;

// Token: 0x02000564 RID: 1380
public class TriggerParent : TriggerBase, IServerComponent
{
	// Token: 0x060029DC RID: 10716 RVA: 0x000FDA20 File Offset: 0x000FBC20
	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x060029DD RID: 10717 RVA: 0x000FDA64 File Offset: 0x000FBC64
	internal override void OnEntityEnter(BaseEntity ent)
	{
		if (ent is NPCPlayer && !this.ParentNPCPlayers)
		{
			return;
		}
		if (this.ShouldParent(ent, false))
		{
			this.Parent(ent);
		}
		base.OnEntityEnter(ent);
		if (this.entityContents != null && this.entityContents.Count == 1)
		{
			base.InvokeRepeating(new Action(this.OnTick), 0f, 0f);
		}
	}

	// Token: 0x060029DE RID: 10718 RVA: 0x000FDACC File Offset: 0x000FBCCC
	internal override void OnEntityLeave(BaseEntity ent)
	{
		base.OnEntityLeave(ent);
		if (this.entityContents == null || this.entityContents.Count == 0)
		{
			base.CancelInvoke(new Action(this.OnTick));
		}
		BasePlayer basePlayer = ent.ToPlayer();
		if (this.parentSleepers && basePlayer != null && basePlayer.IsSleeping())
		{
			return;
		}
		this.Unparent(ent);
	}

	// Token: 0x060029DF RID: 10719 RVA: 0x000FDB30 File Offset: 0x000FBD30
	public virtual bool ShouldParent(BaseEntity ent, bool bypassOtherTriggerCheck = false)
	{
		if (!ent.canTriggerParent)
		{
			return false;
		}
		if (!bypassOtherTriggerCheck)
		{
			BaseEntity parentEntity = ent.GetParentEntity();
			if (!this.overrideOtherTriggers && parentEntity.IsValid() && parentEntity != base.gameObject.ToBaseEntity())
			{
				return false;
			}
		}
		if (ent.FindTrigger<TriggerParentExclusion>() != null)
		{
			return false;
		}
		if (this.doClippingCheck && this.IsClipping(ent))
		{
			return false;
		}
		if (!this.parentMountedPlayers || !this.parentSleepers)
		{
			BasePlayer basePlayer = ent.ToPlayer();
			if (basePlayer != null)
			{
				if (!this.parentMountedPlayers && basePlayer.isMounted)
				{
					return false;
				}
				if (!this.parentSleepers && basePlayer.IsSleeping())
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x060029E0 RID: 10720 RVA: 0x000FDBE0 File Offset: 0x000FBDE0
	protected void Parent(BaseEntity ent)
	{
		BaseEntity baseEntity = base.gameObject.ToBaseEntity();
		if (ent.GetParentEntity() == baseEntity)
		{
			return;
		}
		if (baseEntity.GetParentEntity() == ent)
		{
			return;
		}
		ent.SetParent(base.gameObject.ToBaseEntity(), true, true);
	}

	// Token: 0x060029E1 RID: 10721 RVA: 0x000FDC2C File Offset: 0x000FBE2C
	protected void Unparent(BaseEntity ent)
	{
		if (ent.GetParentEntity() != base.gameObject.ToBaseEntity())
		{
			return;
		}
		if (ent.IsValid() && !ent.IsDestroyed)
		{
			TriggerParent triggerParent = ent.FindSuitableParent();
			if (triggerParent != null && triggerParent.gameObject.ToBaseEntity().IsValid())
			{
				triggerParent.Parent(ent);
				return;
			}
		}
		ent.SetParent(null, true, true);
		BasePlayer basePlayer = ent.ToPlayer();
		if (basePlayer != null)
		{
			basePlayer.PauseFlyHackDetection(5f);
			basePlayer.PauseSpeedHackDetection(5f);
			basePlayer.PauseVehicleNoClipDetection(5f);
			if (this.associatedMountable != null && ((this.doClippingCheck && this.IsClipping(ent)) || basePlayer.IsSleeping()))
			{
				Vector3 vector;
				if (this.associatedMountable.GetDismountPosition(basePlayer, out vector))
				{
					basePlayer.MovePosition(vector);
					basePlayer.SendNetworkUpdateImmediate(false);
					basePlayer.ClientRPCPlayer<Vector3>(null, basePlayer, "ForcePositionTo", vector);
					return;
				}
				this.killPlayerTemp = basePlayer;
				base.Invoke(new Action(this.KillPlayerDelayed), 0f);
			}
		}
	}

	// Token: 0x060029E2 RID: 10722 RVA: 0x000FDD3E File Offset: 0x000FBF3E
	private void KillPlayerDelayed()
	{
		if (this.killPlayerTemp.IsValid() && !this.killPlayerTemp.IsDead())
		{
			this.killPlayerTemp.Hurt(1000f, DamageType.Suicide, this.killPlayerTemp, false);
		}
		this.killPlayerTemp = null;
	}

	// Token: 0x060029E3 RID: 10723 RVA: 0x000FDD7C File Offset: 0x000FBF7C
	private void OnTick()
	{
		if (this.entityContents == null)
		{
			return;
		}
		BaseEntity baseEntity = base.gameObject.ToBaseEntity();
		if (!baseEntity.IsValid())
		{
			return;
		}
		if (baseEntity.IsDestroyed)
		{
			return;
		}
		foreach (BaseEntity baseEntity2 in this.entityContents)
		{
			if (baseEntity2.IsValid() && !baseEntity2.IsDestroyed)
			{
				if (this.ShouldParent(baseEntity2, false))
				{
					this.Parent(baseEntity2);
				}
				else
				{
					this.Unparent(baseEntity2);
				}
			}
		}
	}

	// Token: 0x060029E4 RID: 10724 RVA: 0x000FDE1C File Offset: 0x000FC01C
	protected virtual bool IsClipping(BaseEntity ent)
	{
		return GamePhysics.CheckOBB(ent.WorldSpaceBounds(), 1218511105, QueryTriggerInteraction.Ignore);
	}

	// Token: 0x040021DD RID: 8669
	[Tooltip("Deparent if the parented entity clips into an obstacle")]
	[SerializeField]
	private bool doClippingCheck;

	// Token: 0x040021DE RID: 8670
	[Tooltip("If deparenting via clipping, this will be used (if assigned) to also move the entity to a valid dismount position")]
	public BaseMountable associatedMountable;

	// Token: 0x040021DF RID: 8671
	[Tooltip("Needed if the player might dismount inside the trigger and the trigger might be moving. Being mounting inside the trigger lets them dismount in local trigger-space, which means client and server will sync up.Otherwise the client/server delay can have them dismounting into invalid space.")]
	public bool parentMountedPlayers;

	// Token: 0x040021E0 RID: 8672
	[Tooltip("Sleepers don't have all the checks (e.g. clipping) that awake players get. If that might be a problem,sleeper parenting can be disabled. You'll need an associatedMountable though so that the sleeper can be dismounted.")]
	public bool parentSleepers = true;

	// Token: 0x040021E1 RID: 8673
	public bool ParentNPCPlayers;

	// Token: 0x040021E2 RID: 8674
	[Tooltip("If the player is already parented to something else, they'll switch over to another parent only if this is true")]
	public bool overrideOtherTriggers;

	// Token: 0x040021E3 RID: 8675
	public const int CLIP_CHECK_MASK = 1218511105;

	// Token: 0x040021E4 RID: 8676
	private BasePlayer killPlayerTemp;
}
