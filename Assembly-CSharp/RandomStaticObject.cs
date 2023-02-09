using System;
using UnityEngine;

// Token: 0x0200064E RID: 1614
public class RandomStaticObject : MonoBehaviour
{
	// Token: 0x06002E10 RID: 11792 RVA: 0x0011469C File Offset: 0x0011289C
	protected void Start()
	{
		uint seed = base.transform.position.Seed(World.Seed + this.Seed);
		if (SeedRandom.Value(ref seed) > this.Probability)
		{
			for (int i = 0; i < this.Candidates.Length; i++)
			{
				GameManager.Destroy(this.Candidates[i], 0f);
			}
			GameManager.Destroy(this, 0f);
			return;
		}
		int num = SeedRandom.Range(seed, 0, base.transform.childCount);
		for (int j = 0; j < this.Candidates.Length; j++)
		{
			GameObject gameObject = this.Candidates[j];
			if (j == num)
			{
				gameObject.SetActive(true);
			}
			else
			{
				GameManager.Destroy(gameObject, 0f);
			}
		}
		GameManager.Destroy(this, 0f);
	}

	// Token: 0x040025BD RID: 9661
	public uint Seed;

	// Token: 0x040025BE RID: 9662
	public float Probability = 0.5f;

	// Token: 0x040025BF RID: 9663
	public GameObject[] Candidates;
}
