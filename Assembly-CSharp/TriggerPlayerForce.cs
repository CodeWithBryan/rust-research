using System;
using UnityEngine;

// Token: 0x02000568 RID: 1384
public class TriggerPlayerForce : TriggerBase, IServerComponent
{
	// Token: 0x060029EF RID: 10735 RVA: 0x000FDF8C File Offset: 0x000FC18C
	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity != null)
		{
			return baseEntity.gameObject;
		}
		return null;
	}

	// Token: 0x060029F0 RID: 10736 RVA: 0x000FDFC5 File Offset: 0x000FC1C5
	internal override void OnObjects()
	{
		base.InvokeRepeating(new Action(this.HackDisableTick), 0f, 3.75f);
	}

	// Token: 0x060029F1 RID: 10737 RVA: 0x000FDFE3 File Offset: 0x000FC1E3
	internal override void OnEmpty()
	{
		base.OnEmpty();
		base.CancelInvoke(new Action(this.HackDisableTick));
	}

	// Token: 0x060029F2 RID: 10738 RVA: 0x000FDFFD File Offset: 0x000FC1FD
	protected override void OnDisable()
	{
		base.CancelInvoke(new Action(this.HackDisableTick));
		base.OnDisable();
	}

	// Token: 0x060029F3 RID: 10739 RVA: 0x000FCF25 File Offset: 0x000FB125
	internal override void OnEntityLeave(BaseEntity ent)
	{
		base.OnEntityLeave(ent);
		ent.ApplyInheritedVelocity(Vector3.zero);
	}

	// Token: 0x060029F4 RID: 10740 RVA: 0x000FE018 File Offset: 0x000FC218
	private void HackDisableTick()
	{
		if (this.entityContents == null || !base.enabled)
		{
			return;
		}
		foreach (BaseEntity baseEntity in this.entityContents)
		{
			if (this.IsInterested(baseEntity))
			{
				BasePlayer basePlayer = baseEntity.ToPlayer();
				if (basePlayer != null && !basePlayer.IsNpc)
				{
					basePlayer.PauseVehicleNoClipDetection(4f);
					basePlayer.PauseSpeedHackDetection(4f);
				}
			}
		}
	}

	// Token: 0x060029F5 RID: 10741 RVA: 0x000FE0AC File Offset: 0x000FC2AC
	protected void FixedUpdate()
	{
		if (this.entityContents != null)
		{
			foreach (BaseEntity baseEntity in this.entityContents)
			{
				if ((!this.requireUpAxis || Vector3.Dot(baseEntity.transform.up, base.transform.up) >= 0f) && this.IsInterested(baseEntity))
				{
					Vector3 velocity = this.GetPushVelocity(baseEntity.gameObject);
					baseEntity.ApplyInheritedVelocity(velocity);
				}
			}
		}
	}

	// Token: 0x060029F6 RID: 10742 RVA: 0x000FE148 File Offset: 0x000FC348
	private Vector3 GetPushVelocity(GameObject obj)
	{
		Vector3 a = -(this.triggerCollider.bounds.center - obj.transform.position);
		a.Normalize();
		a.y = 0.2f;
		a.Normalize();
		return a * this.pushVelocity;
	}

	// Token: 0x060029F7 RID: 10743 RVA: 0x000FE1A4 File Offset: 0x000FC3A4
	private bool IsInterested(BaseEntity entity)
	{
		if (entity == null || entity.isClient)
		{
			return false;
		}
		BasePlayer basePlayer = entity.ToPlayer();
		return !(basePlayer != null) || (((!basePlayer.IsAdmin && !basePlayer.IsDeveloper) || !basePlayer.IsFlying) && (basePlayer != null && basePlayer.IsAlive()) && !basePlayer.isMounted);
	}

	// Token: 0x040021EA RID: 8682
	public BoxCollider triggerCollider;

	// Token: 0x040021EB RID: 8683
	public float pushVelocity = 5f;

	// Token: 0x040021EC RID: 8684
	public bool requireUpAxis;

	// Token: 0x040021ED RID: 8685
	private const float HACK_DISABLE_TIME = 4f;
}
