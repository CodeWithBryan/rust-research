using System;
using UnityEngine;

// Token: 0x02000631 RID: 1585
public class DecorOffset : DecorComponent
{
	// Token: 0x06002DD3 RID: 11731 RVA: 0x001138F0 File Offset: 0x00111AF0
	public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		uint num = pos.Seed(World.Seed) + 1U;
		pos.x += scale.x * SeedRandom.Range(ref num, this.MinOffset.x, this.MaxOffset.x);
		pos.y += scale.y * SeedRandom.Range(ref num, this.MinOffset.y, this.MaxOffset.y);
		pos.z += scale.z * SeedRandom.Range(ref num, this.MinOffset.z, this.MaxOffset.z);
	}

	// Token: 0x0400255A RID: 9562
	public Vector3 MinOffset = new Vector3(0f, 0f, 0f);

	// Token: 0x0400255B RID: 9563
	public Vector3 MaxOffset = new Vector3(0f, 0f, 0f);
}
