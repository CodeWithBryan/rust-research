using System;
using UnityEngine;

// Token: 0x02000636 RID: 1590
public class DecorSwim : DecorComponent
{
	// Token: 0x06002DDF RID: 11743 RVA: 0x00113C00 File Offset: 0x00111E00
	public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		pos.y = TerrainMeta.WaterMap.GetHeight(pos);
	}
}
