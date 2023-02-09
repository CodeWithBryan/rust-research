using System;
using UnityEngine;

// Token: 0x02000470 RID: 1136
public class MagnetSnap
{
	// Token: 0x06002526 RID: 9510 RVA: 0x000E92A5 File Offset: 0x000E74A5
	public MagnetSnap(Transform snapLocation)
	{
		this.snapLocation = snapLocation;
		this.prevSnapLocation = snapLocation.position;
	}

	// Token: 0x06002527 RID: 9511 RVA: 0x000E92C0 File Offset: 0x000E74C0
	public void FixedUpdate(Transform target)
	{
		this.PositionTarget(target);
		if (this.snapLocation.hasChanged)
		{
			this.prevSnapLocation = this.snapLocation.position;
			this.snapLocation.hasChanged = false;
		}
	}

	// Token: 0x06002528 RID: 9512 RVA: 0x000E92F4 File Offset: 0x000E74F4
	public void PositionTarget(Transform target)
	{
		if (target == null)
		{
			return;
		}
		Transform transform = target.transform;
		Quaternion quaternion = this.snapLocation.rotation;
		if (Vector3.Angle(transform.forward, this.snapLocation.forward) > 90f)
		{
			quaternion *= Quaternion.Euler(0f, 180f, 0f);
		}
		if (transform.position != this.snapLocation.position)
		{
			transform.position += this.snapLocation.position - this.prevSnapLocation;
			transform.position = Vector3.MoveTowards(transform.position, this.snapLocation.position, 1f * Time.fixedDeltaTime);
		}
		if (transform.rotation != quaternion)
		{
			transform.rotation = Quaternion.RotateTowards(transform.rotation, quaternion, 40f * Time.fixedDeltaTime);
		}
	}

	// Token: 0x04001DC7 RID: 7623
	private Transform snapLocation;

	// Token: 0x04001DC8 RID: 7624
	private Vector3 prevSnapLocation;
}
