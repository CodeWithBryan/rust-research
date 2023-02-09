using System;
using Network;
using Rust;
using UnityEngine;

// Token: 0x020000A0 RID: 160
public class NewYearGong : BaseCombatEntity
{
	// Token: 0x06000EC6 RID: 3782 RVA: 0x0007B6F0 File Offset: 0x000798F0
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("NewYearGong.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000EC7 RID: 3783 RVA: 0x0007B730 File Offset: 0x00079930
	public override void Hurt(HitInfo info)
	{
		if (!info.damageTypes.IsMeleeType() && !info.damageTypes.Has(DamageType.Bullet) && !info.damageTypes.Has(DamageType.Arrow))
		{
			base.Hurt(info);
			return;
		}
		Vector3 a = this.gongCentre.InverseTransformPoint(info.HitPositionWorld);
		a.z = 0f;
		float num = Vector3.Distance(a, Vector3.zero);
		if (num < this.gongRadius)
		{
			if (Time.time - this.lastSound > this.minTimeBetweenSounds)
			{
				this.lastSound = Time.time;
				base.ClientRPC<float>(null, "PlaySound", Mathf.Clamp01(num / this.gongRadius));
				return;
			}
		}
		else
		{
			base.Hurt(info);
		}
	}

	// Token: 0x040009B1 RID: 2481
	public SoundDefinition gongSound;

	// Token: 0x040009B2 RID: 2482
	public float minTimeBetweenSounds = 0.25f;

	// Token: 0x040009B3 RID: 2483
	public GameObject soundRoot;

	// Token: 0x040009B4 RID: 2484
	public Transform gongCentre;

	// Token: 0x040009B5 RID: 2485
	public float gongRadius = 1f;

	// Token: 0x040009B6 RID: 2486
	public AnimationCurve pitchCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x040009B7 RID: 2487
	public Animator gongAnimator;

	// Token: 0x040009B8 RID: 2488
	private float lastSound;
}
