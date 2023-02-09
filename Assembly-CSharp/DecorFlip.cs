using System;
using UnityEngine;

// Token: 0x02000630 RID: 1584
public class DecorFlip : DecorComponent
{
	// Token: 0x06002DD1 RID: 11729 RVA: 0x00113840 File Offset: 0x00111A40
	public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		uint num = pos.Seed(World.Seed) + 4U;
		if (SeedRandom.Value(ref num) > 0.5f)
		{
			return;
		}
		switch (this.FlipAxis)
		{
		case DecorFlip.AxisType.X:
		case DecorFlip.AxisType.Z:
			rot = Quaternion.AngleAxis(180f, rot * Vector3.up) * rot;
			return;
		case DecorFlip.AxisType.Y:
			rot = Quaternion.AngleAxis(180f, rot * Vector3.forward) * rot;
			return;
		default:
			return;
		}
	}

	// Token: 0x04002559 RID: 9561
	public DecorFlip.AxisType FlipAxis = DecorFlip.AxisType.Y;

	// Token: 0x02000D5C RID: 3420
	public enum AxisType
	{
		// Token: 0x0400463F RID: 17983
		X,
		// Token: 0x04004640 RID: 17984
		Y,
		// Token: 0x04004641 RID: 17985
		Z
	}
}
