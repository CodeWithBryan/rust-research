using System;
using UnityEngine;

// Token: 0x0200064C RID: 1612
public class RandomDynamicObject : MonoBehaviour, IClientComponent, ILOD
{
	// Token: 0x040025B5 RID: 9653
	public uint Seed;

	// Token: 0x040025B6 RID: 9654
	public float Distance = 100f;

	// Token: 0x040025B7 RID: 9655
	public float Probability = 0.5f;

	// Token: 0x040025B8 RID: 9656
	public GameObject[] Candidates;
}
