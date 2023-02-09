using System;
using UnityEngine;

// Token: 0x0200043F RID: 1087
public class BaseTrapTrigger : TriggerBase
{
	// Token: 0x060023C2 RID: 9154 RVA: 0x000E2370 File Offset: 0x000E0570
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

	// Token: 0x060023C3 RID: 9155 RVA: 0x000E23B3 File Offset: 0x000E05B3
	internal override void OnObjectAdded(GameObject obj, Collider col)
	{
		base.OnObjectAdded(obj, col);
		this._trap.ObjectEntered(obj);
	}

	// Token: 0x060023C4 RID: 9156 RVA: 0x000E23C9 File Offset: 0x000E05C9
	internal override void OnEmpty()
	{
		base.OnEmpty();
		this._trap.OnEmpty();
	}

	// Token: 0x04001C61 RID: 7265
	public BaseTrap _trap;
}
