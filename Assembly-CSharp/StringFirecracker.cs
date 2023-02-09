using System;
using UnityEngine;

// Token: 0x02000138 RID: 312
public class StringFirecracker : TimedExplosive
{
	// Token: 0x06001600 RID: 5632 RVA: 0x000A8604 File Offset: 0x000A6804
	public override void InitShared()
	{
		base.InitShared();
		if (base.isServer)
		{
			foreach (Rigidbody rigidbody in this.clientParts)
			{
				if (rigidbody != null)
				{
					rigidbody.isKinematic = true;
				}
			}
		}
	}

	// Token: 0x06001601 RID: 5633 RVA: 0x000A8648 File Offset: 0x000A6848
	public void CreatePinJoint()
	{
		if (this.serverClientJoint != null)
		{
			return;
		}
		this.serverClientJoint = base.gameObject.AddComponent<SpringJoint>();
		this.serverClientJoint.connectedBody = this.clientMiddleBody;
		this.serverClientJoint.autoConfigureConnectedAnchor = false;
		this.serverClientJoint.anchor = Vector3.zero;
		this.serverClientJoint.connectedAnchor = Vector3.zero;
		this.serverClientJoint.minDistance = 0f;
		this.serverClientJoint.maxDistance = 1f;
		this.serverClientJoint.damper = 1000f;
		this.serverClientJoint.spring = 5000f;
		this.serverClientJoint.enableCollision = false;
		this.serverClientJoint.enablePreprocessing = false;
	}

	// Token: 0x04000E96 RID: 3734
	public Rigidbody serverRigidBody;

	// Token: 0x04000E97 RID: 3735
	public Rigidbody clientMiddleBody;

	// Token: 0x04000E98 RID: 3736
	public Rigidbody[] clientParts;

	// Token: 0x04000E99 RID: 3737
	public SpringJoint serverClientJoint;

	// Token: 0x04000E9A RID: 3738
	public Transform clientFirecrackerTransform;
}
