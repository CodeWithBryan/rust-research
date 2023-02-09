using System;
using UnityEngine;

// Token: 0x0200064B RID: 1611
public class RandomDestroy : MonoBehaviour
{
	// Token: 0x06002E0C RID: 11788 RVA: 0x001145EC File Offset: 0x001127EC
	protected void Start()
	{
		uint num = base.transform.position.Seed(World.Seed + this.Seed);
		if (SeedRandom.Value(ref num) > this.Probability)
		{
			GameManager.Destroy(this, 0f);
			return;
		}
		GameManager.Destroy(base.gameObject, 0f);
	}

	// Token: 0x040025B3 RID: 9651
	public uint Seed;

	// Token: 0x040025B4 RID: 9652
	public float Probability = 0.5f;
}
