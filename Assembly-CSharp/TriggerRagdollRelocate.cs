using System;
using UnityEngine;

// Token: 0x0200056B RID: 1387
public class TriggerRagdollRelocate : TriggerBase
{
	// Token: 0x06002A04 RID: 10756 RVA: 0x000FE524 File Offset: 0x000FC724
	internal override void OnObjectAdded(GameObject obj, Collider col)
	{
		base.OnObjectAdded(obj, col);
		BaseEntity baseEntity = obj.transform.ToBaseEntity();
		if (baseEntity != null && baseEntity.isServer)
		{
			this.RepositionTransform(baseEntity.transform);
		}
		Ragdoll componentInParent = obj.GetComponentInParent<Ragdoll>();
		if (componentInParent != null)
		{
			this.RepositionTransform(componentInParent.transform);
			foreach (Rigidbody rigidbody in componentInParent.rigidbodies)
			{
				if (rigidbody.transform.position.y < base.transform.position.y)
				{
					this.RepositionTransform(rigidbody.transform);
				}
			}
		}
	}

	// Token: 0x06002A05 RID: 10757 RVA: 0x000FE5EC File Offset: 0x000FC7EC
	private void RepositionTransform(Transform t)
	{
		Vector3 position = this.targetLocation.InverseTransformPoint(t.position);
		position.y = 0f;
		position = this.targetLocation.TransformPoint(position);
		t.position = position;
	}

	// Token: 0x040021F5 RID: 8693
	public Transform targetLocation;
}
