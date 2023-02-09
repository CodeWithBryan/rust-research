using System;
using UnityEngine;

// Token: 0x020008E2 RID: 2274
public class RigidbodyInfo : PrefabAttribute, IClientComponent
{
	// Token: 0x0600367B RID: 13947 RVA: 0x001443E9 File Offset: 0x001425E9
	protected override Type GetIndexedType()
	{
		return typeof(RigidbodyInfo);
	}

	// Token: 0x0600367C RID: 13948 RVA: 0x001443F8 File Offset: 0x001425F8
	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		Rigidbody component = rootObj.GetComponent<Rigidbody>();
		if (component == null)
		{
			Debug.LogError(base.GetType().Name + ": RigidbodyInfo couldn't find a rigidbody on " + name + "! If a RealmedRemove is removing it, make sure this script is above the RealmedRemove script so that this gets processed first.");
			return;
		}
		this.mass = component.mass;
		this.drag = component.drag;
		this.angularDrag = component.angularDrag;
	}

	// Token: 0x04003185 RID: 12677
	[NonSerialized]
	public float mass;

	// Token: 0x04003186 RID: 12678
	[NonSerialized]
	public float drag;

	// Token: 0x04003187 RID: 12679
	[NonSerialized]
	public float angularDrag;
}
