using System;
using UnityEngine;

// Token: 0x02000635 RID: 1589
public class DecorSocketMale : PrefabAttribute
{
	// Token: 0x06002DDC RID: 11740 RVA: 0x00113BBF File Offset: 0x00111DBF
	protected override Type GetIndexedType()
	{
		return typeof(DecorSocketMale);
	}

	// Token: 0x06002DDD RID: 11741 RVA: 0x00113BCB File Offset: 0x00111DCB
	protected void OnDrawGizmos()
	{
		Gizmos.color = new Color(0.5f, 0.5f, 1f, 1f);
		Gizmos.DrawSphere(base.transform.position, 1f);
	}
}
