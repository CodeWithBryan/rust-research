using System;
using UnityEngine;

// Token: 0x0200064F RID: 1615
public class RandomStaticPrefab : MonoBehaviour
{
	// Token: 0x06002E12 RID: 11794 RVA: 0x00114770 File Offset: 0x00112970
	protected void Start()
	{
		uint num = base.transform.position.Seed(World.Seed + this.Seed);
		if (SeedRandom.Value(ref num) > this.Probability)
		{
			GameManager.Destroy(this, 0f);
			return;
		}
		Prefab.LoadRandom("assets/bundled/prefabs/autospawn/" + this.ResourceFolder, ref num, null, null, true).Spawn(base.transform, true);
		GameManager.Destroy(this, 0f);
	}

	// Token: 0x040025C0 RID: 9664
	public uint Seed;

	// Token: 0x040025C1 RID: 9665
	public float Probability = 0.5f;

	// Token: 0x040025C2 RID: 9666
	public string ResourceFolder = string.Empty;
}
