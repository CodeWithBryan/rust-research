using System;
using UnityEngine;

// Token: 0x020002A6 RID: 678
public class FollowCamera : MonoBehaviour, IClientComponent
{
	// Token: 0x06001C38 RID: 7224 RVA: 0x000C32B8 File Offset: 0x000C14B8
	private void LateUpdate()
	{
		if (MainCamera.mainCamera == null)
		{
			return;
		}
		base.transform.position = MainCamera.position;
	}
}
