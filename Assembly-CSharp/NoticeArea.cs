using System;
using UnityEngine;

// Token: 0x02000811 RID: 2065
public class NoticeArea : SingletonComponent<NoticeArea>
{
	// Token: 0x0600345F RID: 13407 RVA: 0x0013D7E6 File Offset: 0x0013B9E6
	protected override void Awake()
	{
		base.Awake();
		this.notices = base.GetComponentsInChildren<IVitalNotice>(true);
	}

	// Token: 0x04002DA8 RID: 11688
	public GameObjectRef itemPickupPrefab;

	// Token: 0x04002DA9 RID: 11689
	public GameObjectRef itemPickupCondensedText;

	// Token: 0x04002DAA RID: 11690
	public GameObjectRef itemDroppedPrefab;

	// Token: 0x04002DAB RID: 11691
	public AnimationCurve pickupSizeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04002DAC RID: 11692
	public AnimationCurve pickupAlphaCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04002DAD RID: 11693
	public AnimationCurve reuseAlphaCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04002DAE RID: 11694
	public AnimationCurve reuseSizeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04002DAF RID: 11695
	private IVitalNotice[] notices;
}
