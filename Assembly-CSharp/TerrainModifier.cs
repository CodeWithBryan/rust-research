using System;
using UnityEngine;

// Token: 0x020006C4 RID: 1732
public abstract class TerrainModifier : PrefabAttribute
{
	// Token: 0x06003085 RID: 12421 RVA: 0x0012AD4C File Offset: 0x00128F4C
	public void Apply(Vector3 pos, float scale)
	{
		float opacity = this.Opacity;
		float radius = scale * this.Radius;
		float fade = scale * this.Fade;
		this.Apply(pos, opacity, radius, fade);
	}

	// Token: 0x06003086 RID: 12422
	protected abstract void Apply(Vector3 position, float opacity, float radius, float fade);

	// Token: 0x06003087 RID: 12423 RVA: 0x0012AD7C File Offset: 0x00128F7C
	protected override Type GetIndexedType()
	{
		return typeof(TerrainModifier);
	}

	// Token: 0x0400277A RID: 10106
	public float Opacity = 1f;

	// Token: 0x0400277B RID: 10107
	public float Radius;

	// Token: 0x0400277C RID: 10108
	public float Fade;
}
