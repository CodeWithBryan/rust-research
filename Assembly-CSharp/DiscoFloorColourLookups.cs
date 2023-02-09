using System;
using UnityEngine;

// Token: 0x0200037A RID: 890
public class DiscoFloorColourLookups : PrefabAttribute, IClientComponent
{
	// Token: 0x06001F37 RID: 7991 RVA: 0x000CF4AA File Offset: 0x000CD6AA
	protected override Type GetIndexedType()
	{
		return typeof(DiscoFloorColourLookups);
	}

	// Token: 0x04001893 RID: 6291
	public float[] InOutLookup;

	// Token: 0x04001894 RID: 6292
	public float[] RadialLookup;

	// Token: 0x04001895 RID: 6293
	public float[] RippleLookup;

	// Token: 0x04001896 RID: 6294
	public float[] CheckerLookup;

	// Token: 0x04001897 RID: 6295
	public float[] BlockLookup;

	// Token: 0x04001898 RID: 6296
	public Gradient[] ColourGradients;
}
