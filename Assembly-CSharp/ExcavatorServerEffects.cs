using System;
using UnityEngine;

// Token: 0x02000412 RID: 1042
public class ExcavatorServerEffects : MonoBehaviour
{
	// Token: 0x060022EA RID: 8938 RVA: 0x000DE70B File Offset: 0x000DC90B
	public void Awake()
	{
		ExcavatorServerEffects.instance = this;
		ExcavatorServerEffects.SetMining(false, true);
	}

	// Token: 0x060022EB RID: 8939 RVA: 0x000DE71A File Offset: 0x000DC91A
	public void OnDestroy()
	{
		ExcavatorServerEffects.instance = null;
	}

	// Token: 0x060022EC RID: 8940 RVA: 0x000DE724 File Offset: 0x000DC924
	public static void SetMining(bool isMining, bool force = false)
	{
		if (ExcavatorServerEffects.instance == null)
		{
			return;
		}
		foreach (TriggerBase triggerBase in ExcavatorServerEffects.instance.miningTriggers)
		{
			if (!(triggerBase == null))
			{
				triggerBase.gameObject.SetActive(isMining);
			}
		}
	}

	// Token: 0x04001B4B RID: 6987
	public static ExcavatorServerEffects instance;

	// Token: 0x04001B4C RID: 6988
	public TriggerBase[] miningTriggers;
}
