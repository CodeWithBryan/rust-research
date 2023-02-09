using System;
using UnityEngine;

// Token: 0x020005C7 RID: 1479
[RequireComponent(typeof(ItemModWearable))]
public class ItemModPaintable : ItemModAssociatedEntity<PaintedItemStorageEntity>
{
	// Token: 0x1700036A RID: 874
	// (get) Token: 0x06002BD2 RID: 11218 RVA: 0x00003A54 File Offset: 0x00001C54
	protected override bool AllowNullParenting
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700036B RID: 875
	// (get) Token: 0x06002BD3 RID: 11219 RVA: 0x00003A54 File Offset: 0x00001C54
	protected override bool OwnedByParentPlayer
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0400239E RID: 9118
	public GameObjectRef ChangeSignTextDialog;

	// Token: 0x0400239F RID: 9119
	public MeshPaintableSource[] PaintableSources;
}
