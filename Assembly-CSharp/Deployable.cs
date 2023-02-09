using System;
using UnityEngine;

// Token: 0x020004D2 RID: 1234
public class Deployable : PrefabAttribute
{
	// Token: 0x06002790 RID: 10128 RVA: 0x000F307A File Offset: 0x000F127A
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		this.bounds = rootObj.GetComponent<BaseEntity>().bounds;
	}

	// Token: 0x06002791 RID: 10129 RVA: 0x000F309A File Offset: 0x000F129A
	protected override Type GetIndexedType()
	{
		return typeof(Deployable);
	}

	// Token: 0x04001FC3 RID: 8131
	public Mesh guideMesh;

	// Token: 0x04001FC4 RID: 8132
	public Vector3 guideMeshScale = Vector3.one;

	// Token: 0x04001FC5 RID: 8133
	public bool guideLights = true;

	// Token: 0x04001FC6 RID: 8134
	public bool wantsInstanceData;

	// Token: 0x04001FC7 RID: 8135
	public bool copyInventoryFromItem;

	// Token: 0x04001FC8 RID: 8136
	public bool setSocketParent;

	// Token: 0x04001FC9 RID: 8137
	public bool toSlot;

	// Token: 0x04001FCA RID: 8138
	public BaseEntity.Slot slot;

	// Token: 0x04001FCB RID: 8139
	public GameObjectRef placeEffect;

	// Token: 0x04001FCC RID: 8140
	[NonSerialized]
	public Bounds bounds;
}
