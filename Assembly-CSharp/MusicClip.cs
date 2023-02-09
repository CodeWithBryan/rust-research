using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000219 RID: 537
public class MusicClip : ScriptableObject
{
	// Token: 0x06001AAC RID: 6828 RVA: 0x000BBE60 File Offset: 0x000BA060
	public float GetNextFadeInPoint(float currentClipTimeBars)
	{
		if (this.fadeInPoints.Count == 0)
		{
			return currentClipTimeBars + 0.125f;
		}
		float result = -1f;
		float num = float.PositiveInfinity;
		for (int i = 0; i < this.fadeInPoints.Count; i++)
		{
			float num2 = this.fadeInPoints[i];
			float num3 = num2 - currentClipTimeBars;
			if (num2 > 0.01f && num3 > 0f && num3 < num)
			{
				num = num3;
				result = num2;
			}
		}
		return result;
	}

	// Token: 0x04001351 RID: 4945
	public AudioClip audioClip;

	// Token: 0x04001352 RID: 4946
	public int lengthInBars = 1;

	// Token: 0x04001353 RID: 4947
	public int lengthInBarsWithTail;

	// Token: 0x04001354 RID: 4948
	public List<float> fadeInPoints = new List<float>();
}
