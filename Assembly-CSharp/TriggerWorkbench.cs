using System;
using UnityEngine;

// Token: 0x02000572 RID: 1394
public class TriggerWorkbench : TriggerBase
{
	// Token: 0x06002A26 RID: 10790 RVA: 0x000FECC4 File Offset: 0x000FCEC4
	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x06002A27 RID: 10791 RVA: 0x000FED07 File Offset: 0x000FCF07
	public float WorkbenchLevel()
	{
		return (float)this.parentBench.Workbenchlevel;
	}

	// Token: 0x04002209 RID: 8713
	public Workbench parentBench;
}
