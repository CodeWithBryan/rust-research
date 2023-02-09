using System;
using UnityEngine;

// Token: 0x0200058C RID: 1420
[CreateAssetMenu(menuName = "Rust/Gestures/Gesture Collection")]
public class GestureCollection : ScriptableObject
{
	// Token: 0x06002A83 RID: 10883 RVA: 0x0010154C File Offset: 0x000FF74C
	public GestureConfig IdToGesture(uint id)
	{
		foreach (GestureConfig gestureConfig in this.AllGestures)
		{
			if (gestureConfig.gestureId == id)
			{
				return gestureConfig;
			}
		}
		return null;
	}

	// Token: 0x06002A84 RID: 10884 RVA: 0x00101580 File Offset: 0x000FF780
	public GestureConfig StringToGesture(string gestureName)
	{
		foreach (GestureConfig gestureConfig in this.AllGestures)
		{
			if (gestureConfig.convarName == gestureName)
			{
				return gestureConfig;
			}
		}
		return null;
	}

	// Token: 0x0400225D RID: 8797
	public GestureConfig[] AllGestures;

	// Token: 0x0400225E RID: 8798
	public float GestureVmInDuration = 0.25f;

	// Token: 0x0400225F RID: 8799
	public AnimationCurve GestureInCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04002260 RID: 8800
	public float GestureVmOutDuration = 0.25f;

	// Token: 0x04002261 RID: 8801
	public AnimationCurve GestureOutCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04002262 RID: 8802
	public float GestureViewmodelDeployDelay = 0.25f;
}
