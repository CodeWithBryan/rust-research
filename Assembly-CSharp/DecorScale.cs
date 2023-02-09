using System;
using UnityEngine;

// Token: 0x02000633 RID: 1587
public class DecorScale : DecorComponent
{
	// Token: 0x06002DD7 RID: 11735 RVA: 0x00113AA8 File Offset: 0x00111CA8
	public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		uint num = pos.Seed(World.Seed) + 3U;
		float t = SeedRandom.Value(ref num);
		scale.x *= Mathf.Lerp(this.MinScale.x, this.MaxScale.x, t);
		scale.y *= Mathf.Lerp(this.MinScale.y, this.MaxScale.y, t);
		scale.z *= Mathf.Lerp(this.MinScale.z, this.MaxScale.z, t);
	}

	// Token: 0x0400255E RID: 9566
	public Vector3 MinScale = new Vector3(1f, 1f, 1f);

	// Token: 0x0400255F RID: 9567
	public Vector3 MaxScale = new Vector3(2f, 2f, 2f);
}
