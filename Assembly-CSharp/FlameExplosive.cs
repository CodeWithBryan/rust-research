using System;
using UnityEngine;

// Token: 0x020003CF RID: 975
public class FlameExplosive : TimedExplosive
{
	// Token: 0x06002149 RID: 8521 RVA: 0x000D6ADC File Offset: 0x000D4CDC
	public override void Explode()
	{
		this.FlameExplode(this.forceUpForExplosion ? Vector3.up : (-base.transform.forward));
	}

	// Token: 0x0600214A RID: 8522 RVA: 0x000D6B04 File Offset: 0x000D4D04
	public void FlameExplode(Vector3 surfaceNormal)
	{
		if (!base.isServer)
		{
			return;
		}
		Vector3 position = base.transform.position;
		if (this.blockCreateUnderwater && WaterLevel.Test(position, true, null))
		{
			base.Explode();
			return;
		}
		Collider component = base.GetComponent<Collider>();
		if (component)
		{
			component.enabled = false;
		}
		int num = 0;
		while ((float)num < this.numToCreate)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.createOnExplode.resourcePath, position, default(Quaternion), true);
			if (baseEntity)
			{
				float num2 = (float)num / this.numToCreate;
				Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(this.spreadAngle * this.spreadCurve.Evaluate(num2), surfaceNormal, true);
				baseEntity.transform.SetPositionAndRotation(position, Quaternion.LookRotation(modifiedAimConeDirection));
				baseEntity.creatorEntity = ((this.creatorEntity == null) ? baseEntity : this.creatorEntity);
				baseEntity.Spawn();
				Vector3 vector = modifiedAimConeDirection.normalized * UnityEngine.Random.Range(this.minVelocity, this.maxVelocity) * this.velocityCurve.Evaluate(num2 * UnityEngine.Random.Range(1f, 1.1f));
				FireBall component2 = baseEntity.GetComponent<FireBall>();
				if (component2 != null)
				{
					component2.SetDelayedVelocity(vector);
				}
				else
				{
					baseEntity.SetVelocity(vector);
				}
			}
			num++;
		}
		base.Explode();
	}

	// Token: 0x0600214B RID: 8523 RVA: 0x000D6C64 File Offset: 0x000D4E64
	public override void ProjectileImpact(RaycastHit info, Vector3 rayOrigin)
	{
		this.FlameExplode(info.normal);
	}

	// Token: 0x040019A3 RID: 6563
	public GameObjectRef createOnExplode;

	// Token: 0x040019A4 RID: 6564
	public bool blockCreateUnderwater;

	// Token: 0x040019A5 RID: 6565
	public float numToCreate = 10f;

	// Token: 0x040019A6 RID: 6566
	public float minVelocity = 2f;

	// Token: 0x040019A7 RID: 6567
	public float maxVelocity = 5f;

	// Token: 0x040019A8 RID: 6568
	public float spreadAngle = 90f;

	// Token: 0x040019A9 RID: 6569
	public bool forceUpForExplosion;

	// Token: 0x040019AA RID: 6570
	public AnimationCurve velocityCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x040019AB RID: 6571
	public AnimationCurve spreadCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 1f)
	});
}
