using System;
using UnityEngine;

// Token: 0x02000254 RID: 596
public class NeighbourSocket : Socket_Base
{
	// Token: 0x06001B6A RID: 7018 RVA: 0x000BF3DB File Offset: 0x000BD5DB
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(this.selectCenter, this.selectSize);
	}

	// Token: 0x06001B6B RID: 7019 RVA: 0x00007074 File Offset: 0x00005274
	public override bool TestTarget(Construction.Target target)
	{
		return false;
	}

	// Token: 0x06001B6C RID: 7020 RVA: 0x000BF400 File Offset: 0x000BD600
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
}
