using System;
using UnityEngine;

// Token: 0x02000634 RID: 1588
public class DecorSocketFemale : PrefabAttribute
{
	// Token: 0x06002DD9 RID: 11737 RVA: 0x00113B7E File Offset: 0x00111D7E
	protected override Type GetIndexedType()
	{
		return typeof(DecorSocketFemale);
	}

	// Token: 0x06002DDA RID: 11738 RVA: 0x00113B8A File Offset: 0x00111D8A
	protected void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 0.5f, 0.5f, 1f);
		Gizmos.DrawSphere(base.transform.position, 1f);
	}
}
