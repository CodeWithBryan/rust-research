using System;
using UnityEngine;

// Token: 0x02000016 RID: 22
public class PatternFireworkShell : FireworkShell
{
	// Token: 0x0400004F RID: 79
	public GameObjectRef StarPrefab;

	// Token: 0x04000050 RID: 80
	public AnimationCurve StarCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04000051 RID: 81
	public float Duration = 3f;

	// Token: 0x04000052 RID: 82
	public float Scale = 5f;

	// Token: 0x04000053 RID: 83
	[Header("Random Design")]
	[MinMax(0f, 1f)]
	public MinMax RandomSaturation = new MinMax(0f, 0.5f);

	// Token: 0x04000054 RID: 84
	[MinMax(0f, 1f)]
	public MinMax RandomValue = new MinMax(0.5f, 0.75f);
}
