using System;
using UnityEngine;

// Token: 0x0200064D RID: 1613
public class RandomDynamicPrefab : MonoBehaviour, IClientComponent, ILOD
{
	// Token: 0x040025B9 RID: 9657
	public uint Seed;

	// Token: 0x040025BA RID: 9658
	public float Distance = 100f;

	// Token: 0x040025BB RID: 9659
	public float Probability = 0.5f;

	// Token: 0x040025BC RID: 9660
	public string ResourceFolder = string.Empty;
}
