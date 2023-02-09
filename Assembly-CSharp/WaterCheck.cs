using System;
using UnityEngine;

// Token: 0x020006DF RID: 1759
public class WaterCheck : PrefabAttribute
{
	// Token: 0x06003159 RID: 12633 RVA: 0x0012FA61 File Offset: 0x0012DC61
	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0f, 0f, 0.5f, 1f);
		Gizmos.DrawSphere(base.transform.position, 1f);
	}

	// Token: 0x0600315A RID: 12634 RVA: 0x0012FA96 File Offset: 0x0012DC96
	public bool Check(Vector3 pos)
	{
		return pos.y <= TerrainMeta.WaterMap.GetHeight(pos);
	}

	// Token: 0x0600315B RID: 12635 RVA: 0x0012FAAE File Offset: 0x0012DCAE
	protected override Type GetIndexedType()
	{
		return typeof(WaterCheck);
	}

	// Token: 0x040027F8 RID: 10232
	public bool Rotate = true;
}
