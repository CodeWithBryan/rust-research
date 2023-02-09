using System;
using UnityEngine;

// Token: 0x02000028 RID: 40
public class ScrapTransportHelicopterWheelEffects : MonoBehaviour, IServerComponent
{
	// Token: 0x06000107 RID: 263 RVA: 0x0000745C File Offset: 0x0000565C
	public void Update()
	{
		bool isGrounded = this.wheelCollider.isGrounded;
		if (isGrounded && !this.wasGrounded)
		{
			this.DoImpactEffect();
		}
		this.wasGrounded = isGrounded;
	}

	// Token: 0x06000108 RID: 264 RVA: 0x00007490 File Offset: 0x00005690
	private void DoImpactEffect()
	{
		if (!this.impactEffect.isValid)
		{
			return;
		}
		if (Time.time < this.lastEffectPlayed + this.minTimeBetweenEffects)
		{
			return;
		}
		if (Vector3.Distance(base.transform.position, this.lastCollisionPos) < this.minDistBetweenEffects && this.lastEffectPlayed != 0f)
		{
			return;
		}
		Effect.server.Run(this.impactEffect.resourcePath, base.transform.position, base.transform.up, null, false);
		this.lastEffectPlayed = Time.time;
		this.lastCollisionPos = base.transform.position;
	}

	// Token: 0x04000151 RID: 337
	public WheelCollider wheelCollider;

	// Token: 0x04000152 RID: 338
	public GameObjectRef impactEffect;

	// Token: 0x04000153 RID: 339
	public float minTimeBetweenEffects = 0.25f;

	// Token: 0x04000154 RID: 340
	public float minDistBetweenEffects = 0.1f;

	// Token: 0x04000155 RID: 341
	private bool wasGrounded;

	// Token: 0x04000156 RID: 342
	private float lastEffectPlayed;

	// Token: 0x04000157 RID: 343
	private Vector3 lastCollisionPos;
}
