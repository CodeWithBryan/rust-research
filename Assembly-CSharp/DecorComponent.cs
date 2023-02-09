using System;
using UnityEngine;

// Token: 0x0200062D RID: 1581
public abstract class DecorComponent : PrefabAttribute
{
	// Token: 0x06002DC8 RID: 11720
	public abstract void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale);

	// Token: 0x06002DC9 RID: 11721 RVA: 0x00113562 File Offset: 0x00111762
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		this.isRoot = (rootObj == base.gameObject);
	}

	// Token: 0x06002DCA RID: 11722 RVA: 0x00113583 File Offset: 0x00111783
	protected override Type GetIndexedType()
	{
		return typeof(DecorComponent);
	}

	// Token: 0x04002554 RID: 9556
	internal bool isRoot;
}
