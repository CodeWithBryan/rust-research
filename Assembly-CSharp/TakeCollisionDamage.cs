using System;
using Rust;
using UnityEngine;

// Token: 0x0200047D RID: 1149
[RequireComponent(typeof(Collider))]
public class TakeCollisionDamage : FacepunchBehaviour
{
	// Token: 0x170002EB RID: 747
	// (get) Token: 0x06002576 RID: 9590 RVA: 0x000E9E7B File Offset: 0x000E807B
	private bool IsServer
	{
		get
		{
			return this.entity.isServer;
		}
	}

	// Token: 0x170002EC RID: 748
	// (get) Token: 0x06002577 RID: 9591 RVA: 0x000E9E88 File Offset: 0x000E8088
	private bool IsClient
	{
		get
		{
			return this.entity.isClient;
		}
	}

	// Token: 0x06002578 RID: 9592 RVA: 0x000E9E98 File Offset: 0x000E8098
	protected void OnCollisionEnter(Collision collision)
	{
		if (this.IsClient || collision == null || collision.gameObject == null || collision.gameObject == null)
		{
			return;
		}
		Rigidbody rigidbody = collision.rigidbody;
		float num = (rigidbody == null) ? 100f : rigidbody.mass;
		float value = collision.relativeVelocity.magnitude * (this.entity.RealisticMass + num) / Time.fixedDeltaTime;
		float num2 = Mathf.InverseLerp(this.forceForAnyDamage, this.forceForMaxDamage, value);
		if (num2 > 0f)
		{
			this.pendingDamage = Mathf.Max(this.pendingDamage, Mathf.Lerp(this.minDamage, this.maxDamage, num2));
			if (this.pendingDamage > this.entity.Health())
			{
				TakeCollisionDamage.ICanRestoreVelocity canRestoreVelocity = collision.gameObject.ToBaseEntity() as TakeCollisionDamage.ICanRestoreVelocity;
				if (canRestoreVelocity != null)
				{
					canRestoreVelocity.RestoreVelocity(collision.relativeVelocity * this.velocityRestorePercent);
				}
			}
			base.Invoke(new Action(this.DoDamage), 0f);
		}
	}

	// Token: 0x06002579 RID: 9593 RVA: 0x000E9FA5 File Offset: 0x000E81A5
	protected void OnDestroy()
	{
		base.CancelInvoke(new Action(this.DoDamage));
	}

	// Token: 0x0600257A RID: 9594 RVA: 0x000E9FBC File Offset: 0x000E81BC
	private void DoDamage()
	{
		if (this.entity == null || this.entity.IsDead() || this.entity.IsDestroyed)
		{
			return;
		}
		if (this.pendingDamage > 0f)
		{
			this.entity.Hurt(this.pendingDamage, DamageType.Collision, null, false);
			this.pendingDamage = 0f;
		}
	}

	// Token: 0x04001DFC RID: 7676
	[SerializeField]
	private BaseCombatEntity entity;

	// Token: 0x04001DFD RID: 7677
	[SerializeField]
	private float minDamage = 1f;

	// Token: 0x04001DFE RID: 7678
	[SerializeField]
	private float maxDamage = 250f;

	// Token: 0x04001DFF RID: 7679
	[SerializeField]
	private float forceForAnyDamage = 20000f;

	// Token: 0x04001E00 RID: 7680
	[SerializeField]
	private float forceForMaxDamage = 1000000f;

	// Token: 0x04001E01 RID: 7681
	[SerializeField]
	private float velocityRestorePercent = 0.75f;

	// Token: 0x04001E02 RID: 7682
	private float pendingDamage;

	// Token: 0x02000CB3 RID: 3251
	public interface ICanRestoreVelocity
	{
		// Token: 0x06004D56 RID: 19798
		void RestoreVelocity(Vector3 amount);
	}
}
