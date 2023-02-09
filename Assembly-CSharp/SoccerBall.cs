using System;
using UnityEngine;

// Token: 0x02000482 RID: 1154
public class SoccerBall : BaseCombatEntity
{
	// Token: 0x06002580 RID: 9600 RVA: 0x000EA09C File Offset: 0x000E829C
	protected void OnCollisionEnter(Collision collision)
	{
		if (base.isClient)
		{
			return;
		}
		if (collision.impulse.magnitude > 0f && collision.collider.attachedRigidbody != null && !collision.collider.attachedRigidbody.HasComponent<SoccerBall>())
		{
			Vector3 a = this.rigidBody.position - collision.collider.attachedRigidbody.position;
			float magnitude = collision.impulse.magnitude;
			this.rigidBody.AddForce(a * magnitude * this.additionalForceMultiplier + Vector3.up * magnitude * this.upForceMultiplier, ForceMode.Impulse);
		}
	}

	// Token: 0x06002581 RID: 9601 RVA: 0x000EA158 File Offset: 0x000E8358
	public override void Hurt(HitInfo info)
	{
		if (base.isClient)
		{
			return;
		}
		float num = 0f;
		foreach (float num2 in info.damageTypes.types)
		{
			if ((int)num2 == 16 || (int)num2 == 22)
			{
				num += num2 * this.explosionForceMultiplier;
			}
			else
			{
				num += num2 * this.otherForceMultiplier;
			}
		}
		if (num > 3f)
		{
			this.rigidBody.AddExplosionForce(num, info.HitPositionWorld, 0.25f, 0.5f);
		}
		base.Hurt(info);
	}

	// Token: 0x04001E26 RID: 7718
	[Header("Soccer Ball")]
	[SerializeField]
	private Rigidbody rigidBody;

	// Token: 0x04001E27 RID: 7719
	[SerializeField]
	private float additionalForceMultiplier = 0.2f;

	// Token: 0x04001E28 RID: 7720
	[SerializeField]
	private float upForceMultiplier = 0.15f;

	// Token: 0x04001E29 RID: 7721
	[SerializeField]
	private DamageRenderer damageRenderer;

	// Token: 0x04001E2A RID: 7722
	[SerializeField]
	private float explosionForceMultiplier = 40f;

	// Token: 0x04001E2B RID: 7723
	[SerializeField]
	private float otherForceMultiplier = 10f;
}
