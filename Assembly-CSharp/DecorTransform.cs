using System;
using UnityEngine;

// Token: 0x02000637 RID: 1591
public class DecorTransform : DecorComponent
{
	// Token: 0x06002DE1 RID: 11745 RVA: 0x00113C20 File Offset: 0x00111E20
	public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		pos += rot * Vector3.Scale(scale, this.Position);
		rot = Quaternion.Euler(this.Rotation) * rot;
		scale = Vector3.Scale(scale, this.Scale);
	}

	// Token: 0x04002560 RID: 9568
	public Vector3 Position = new Vector3(0f, 0f, 0f);

	// Token: 0x04002561 RID: 9569
	public Vector3 Rotation = new Vector3(0f, 0f, 0f);

	// Token: 0x04002562 RID: 9570
	public Vector3 Scale = new Vector3(1f, 1f, 1f);
}
