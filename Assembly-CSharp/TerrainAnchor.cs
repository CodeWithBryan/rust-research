using System;
using UnityEngine;

// Token: 0x0200065E RID: 1630
public class TerrainAnchor : PrefabAttribute
{
	// Token: 0x06002E34 RID: 11828 RVA: 0x001152F4 File Offset: 0x001134F4
	public void Apply(out float height, out float min, out float max, Vector3 pos, Vector3 scale)
	{
		float num = this.Extents * scale.y;
		float num2 = this.Offset * scale.y;
		height = TerrainMeta.HeightMap.GetHeight(pos);
		min = height - num2 - num;
		max = height - num2 + num;
		if (this.Radius > 0f)
		{
			int num3 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeX(pos.x - this.Radius));
			int num4 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeX(pos.x + this.Radius));
			int num5 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeZ(pos.z - this.Radius));
			int num6 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeZ(pos.z + this.Radius));
			int num7 = num5;
			while (num7 <= num6 && max >= min)
			{
				int num8 = num3;
				while (num8 <= num4 && max >= min)
				{
					float height2 = TerrainMeta.HeightMap.GetHeight(num8, num7);
					min = Mathf.Max(min, height2 - num2 - num);
					max = Mathf.Min(max, height2 - num2 + num);
					num8++;
				}
				num7++;
			}
		}
	}

	// Token: 0x06002E35 RID: 11829 RVA: 0x0011541F File Offset: 0x0011361F
	protected override Type GetIndexedType()
	{
		return typeof(TerrainAnchor);
	}

	// Token: 0x040025E9 RID: 9705
	public float Extents = 1f;

	// Token: 0x040025EA RID: 9706
	public float Offset;

	// Token: 0x040025EB RID: 9707
	public float Radius;
}
