using System;
using ConVar;
using UnityEngine;

// Token: 0x0200032D RID: 813
public class PhysicsEffects : MonoBehaviour
{
	// Token: 0x06001DE0 RID: 7648 RVA: 0x000CB716 File Offset: 0x000C9916
	public void OnEnable()
	{
		this.enabledAt = UnityEngine.Time.time;
	}

	// Token: 0x06001DE1 RID: 7649 RVA: 0x000CB724 File Offset: 0x000C9924
	public void OnCollisionEnter(Collision collision)
	{
		if (!ConVar.Physics.sendeffects)
		{
			return;
		}
		if (UnityEngine.Time.time < this.enabledAt + this.enableDelay)
		{
			return;
		}
		if (UnityEngine.Time.time < this.lastEffectPlayed + this.minTimeBetweenEffects)
		{
			return;
		}
		if ((1 << collision.gameObject.layer & this.ignoreLayers) != 0)
		{
			return;
		}
		float num = collision.relativeVelocity.magnitude;
		num = num * 0.055f * this.hardnessScale;
		if (num <= this.ignoreImpactThreshold)
		{
			return;
		}
		if (Vector3.Distance(base.transform.position, this.lastCollisionPos) < this.minDistBetweenEffects && this.lastEffectPlayed != 0f)
		{
			return;
		}
		if (this.entity != null)
		{
			this.entity.SignalBroadcast(BaseEntity.Signal.PhysImpact, num.ToString(), null);
		}
		this.lastEffectPlayed = UnityEngine.Time.time;
		this.lastCollisionPos = base.transform.position;
	}

	// Token: 0x0400178E RID: 6030
	public BaseEntity entity;

	// Token: 0x0400178F RID: 6031
	public SoundDefinition physImpactSoundDef;

	// Token: 0x04001790 RID: 6032
	public float minTimeBetweenEffects = 0.25f;

	// Token: 0x04001791 RID: 6033
	public float minDistBetweenEffects = 0.1f;

	// Token: 0x04001792 RID: 6034
	public float hardnessScale = 1f;

	// Token: 0x04001793 RID: 6035
	public float lowMedThreshold = 0.4f;

	// Token: 0x04001794 RID: 6036
	public float medHardThreshold = 0.7f;

	// Token: 0x04001795 RID: 6037
	public float enableDelay = 0.1f;

	// Token: 0x04001796 RID: 6038
	public LayerMask ignoreLayers;

	// Token: 0x04001797 RID: 6039
	private float lastEffectPlayed;

	// Token: 0x04001798 RID: 6040
	private float enabledAt = float.PositiveInfinity;

	// Token: 0x04001799 RID: 6041
	private float ignoreImpactThreshold = 0.02f;

	// Token: 0x0400179A RID: 6042
	private Vector3 lastCollisionPos;
}
