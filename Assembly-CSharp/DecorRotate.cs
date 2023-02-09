using System;
using UnityEngine;

// Token: 0x02000632 RID: 1586
public class DecorRotate : DecorComponent
{
	// Token: 0x06002DD5 RID: 11733 RVA: 0x001139D8 File Offset: 0x00111BD8
	public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		uint num = pos.Seed(World.Seed) + 2U;
		float x = SeedRandom.Range(ref num, this.MinRotation.x, this.MaxRotation.x);
		float y = SeedRandom.Range(ref num, this.MinRotation.y, this.MaxRotation.y);
		float z = SeedRandom.Range(ref num, this.MinRotation.z, this.MaxRotation.z);
		rot = Quaternion.Euler(x, y, z) * rot;
	}

	// Token: 0x0400255C RID: 9564
	public Vector3 MinRotation = new Vector3(0f, -180f, 0f);

	// Token: 0x0400255D RID: 9565
	public Vector3 MaxRotation = new Vector3(0f, 180f, 0f);
}
