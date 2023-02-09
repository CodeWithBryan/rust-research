using System;
using UnityEngine;

// Token: 0x02000267 RID: 615
public class Socket_Specific_Female : Socket_Base
{
	// Token: 0x06001BB4 RID: 7092 RVA: 0x000C0C00 File Offset: 0x000BEE00
	private void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.red;
		Gizmos.DrawLine(Vector3.zero, Vector3.forward * 0.2f);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(Vector3.zero, Vector3.right * 0.1f);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(Vector3.zero, Vector3.up * 0.1f);
		Gizmos.DrawIcon(base.transform.position, "light_circle_green.png", false);
	}

	// Token: 0x06001BB5 RID: 7093 RVA: 0x000BF3DB File Offset: 0x000BD5DB
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(this.selectCenter, this.selectSize);
	}

	// Token: 0x06001BB6 RID: 7094 RVA: 0x000C0C9C File Offset: 0x000BEE9C
	public bool CanAccept(Socket_Specific socket)
	{
		foreach (string b in this.allowedMaleSockets)
		{
			if (socket.targetSocketName == b)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x040014C0 RID: 5312
	public int rotationDegrees;

	// Token: 0x040014C1 RID: 5313
	public int rotationOffset;

	// Token: 0x040014C2 RID: 5314
	public string[] allowedMaleSockets;
}
