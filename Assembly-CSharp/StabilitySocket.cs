using System;
using UnityEngine;

// Token: 0x02000269 RID: 617
public class StabilitySocket : Socket_Base
{
	// Token: 0x06001BBC RID: 7100 RVA: 0x000BF3DB File Offset: 0x000BD5DB
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(this.selectCenter, this.selectSize);
	}

	// Token: 0x06001BBD RID: 7101 RVA: 0x00007074 File Offset: 0x00005274
	public override bool TestTarget(Construction.Target target)
	{
		return false;
	}

	// Token: 0x06001BBE RID: 7102 RVA: 0x000C0EB8 File Offset: 0x000BF0B8
	public override bool CanConnect(Vector3 position, Quaternion rotation, Socket_Base socket, Vector3 socketPosition, Quaternion socketRotation)
	{
		if (!base.CanConnect(position, rotation, socket, socketPosition, socketRotation))
		{
			return false;
		}
		OBB selectBounds = base.GetSelectBounds(position, rotation);
		OBB selectBounds2 = socket.GetSelectBounds(socketPosition, socketRotation);
		return selectBounds.Intersects(selectBounds2);
	}

	// Token: 0x040014C5 RID: 5317
	[Range(0f, 1f)]
	public float support = 1f;
}
