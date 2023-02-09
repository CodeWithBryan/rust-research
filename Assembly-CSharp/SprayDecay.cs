using System;
using ConVar;
using UnityEngine;

// Token: 0x020003C6 RID: 966
public class SprayDecay : global::Decay
{
	// Token: 0x06002102 RID: 8450 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool ShouldDecay(BaseEntity entity)
	{
		return true;
	}

	// Token: 0x06002103 RID: 8451 RVA: 0x00026FFC File Offset: 0x000251FC
	public override float GetDecayDelay(BaseEntity entity)
	{
		return 0f;
	}

	// Token: 0x06002104 RID: 8452 RVA: 0x000D5C41 File Offset: 0x000D3E41
	public override float GetDecayDuration(BaseEntity entity)
	{
		return Mathf.Max(Global.SprayDuration, 1f);
	}
}
