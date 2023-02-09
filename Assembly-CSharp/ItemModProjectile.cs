using System;
using Rust;
using UnityEngine;

// Token: 0x020005C9 RID: 1481
public class ItemModProjectile : MonoBehaviour
{
	// Token: 0x06002BD7 RID: 11223 RVA: 0x0010757A File Offset: 0x0010577A
	public float GetRandomVelocity()
	{
		return this.projectileVelocity + UnityEngine.Random.Range(-this.projectileVelocitySpread, this.projectileVelocitySpread);
	}

	// Token: 0x06002BD8 RID: 11224 RVA: 0x00107595 File Offset: 0x00105795
	public float GetSpreadScalar()
	{
		if (this.useCurve)
		{
			return this.spreadScalar.Evaluate(UnityEngine.Random.Range(0f, 1f));
		}
		return 1f;
	}

	// Token: 0x06002BD9 RID: 11225 RVA: 0x001075C0 File Offset: 0x001057C0
	public float GetIndexedSpreadScalar(int shotIndex, int maxShots)
	{
		float time;
		if (shotIndex != -1)
		{
			float num = 1f / (float)maxShots;
			time = (float)shotIndex * num;
		}
		else
		{
			time = UnityEngine.Random.Range(0f, 1f);
		}
		return this.spreadScalar.Evaluate(time);
	}

	// Token: 0x06002BDA RID: 11226 RVA: 0x00107603 File Offset: 0x00105803
	public float GetAverageVelocity()
	{
		return this.projectileVelocity;
	}

	// Token: 0x06002BDB RID: 11227 RVA: 0x0010760B File Offset: 0x0010580B
	public float GetMinVelocity()
	{
		return this.projectileVelocity - this.projectileVelocitySpread;
	}

	// Token: 0x06002BDC RID: 11228 RVA: 0x0010761A File Offset: 0x0010581A
	public float GetMaxVelocity()
	{
		return this.projectileVelocity + this.projectileVelocitySpread;
	}

	// Token: 0x06002BDD RID: 11229 RVA: 0x00107629 File Offset: 0x00105829
	public bool IsAmmo(AmmoTypes ammo)
	{
		return (this.ammoType & ammo) > (AmmoTypes)0;
	}

	// Token: 0x06002BDE RID: 11230 RVA: 0x00107638 File Offset: 0x00105838
	public virtual void ServerProjectileHit(HitInfo info)
	{
		if (this.mods == null)
		{
			return;
		}
		foreach (ItemModProjectileMod itemModProjectileMod in this.mods)
		{
			if (!(itemModProjectileMod == null))
			{
				itemModProjectileMod.ServerProjectileHit(info);
			}
		}
	}

	// Token: 0x040023A4 RID: 9124
	public GameObjectRef projectileObject = new GameObjectRef();

	// Token: 0x040023A5 RID: 9125
	public ItemModProjectileMod[] mods;

	// Token: 0x040023A6 RID: 9126
	public AmmoTypes ammoType;

	// Token: 0x040023A7 RID: 9127
	public int numProjectiles = 1;

	// Token: 0x040023A8 RID: 9128
	public float projectileSpread;

	// Token: 0x040023A9 RID: 9129
	public float projectileVelocity = 100f;

	// Token: 0x040023AA RID: 9130
	public float projectileVelocitySpread;

	// Token: 0x040023AB RID: 9131
	public bool useCurve;

	// Token: 0x040023AC RID: 9132
	public AnimationCurve spreadScalar;

	// Token: 0x040023AD RID: 9133
	public GameObjectRef attackEffectOverride;

	// Token: 0x040023AE RID: 9134
	public float barrelConditionLoss;

	// Token: 0x040023AF RID: 9135
	public string category = "bullet";
}
