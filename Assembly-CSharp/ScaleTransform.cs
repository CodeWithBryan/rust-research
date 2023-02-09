using System;
using UnityEngine;

// Token: 0x02000333 RID: 819
public class ScaleTransform : ScaleRenderer
{
	// Token: 0x06001DF4 RID: 7668 RVA: 0x000CBC42 File Offset: 0x000C9E42
	public override void SetScale_Internal(float scale)
	{
		base.SetScale_Internal(scale);
		this.myRenderer.transform.localScale = this.initialScale * scale;
	}

	// Token: 0x06001DF5 RID: 7669 RVA: 0x000CBC67 File Offset: 0x000C9E67
	public override void GatherInitialValues()
	{
		this.initialScale = this.myRenderer.transform.localScale;
		base.GatherInitialValues();
	}

	// Token: 0x040017B3 RID: 6067
	private Vector3 initialScale;
}
