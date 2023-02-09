using System;
using ConVar;
using UnityEngine;

// Token: 0x02000288 RID: 648
public class CameraSettings : MonoBehaviour, IClientComponent
{
	// Token: 0x06001BF7 RID: 7159 RVA: 0x000C1B8C File Offset: 0x000BFD8C
	private void OnEnable()
	{
		this.cam = base.GetComponent<Camera>();
	}

	// Token: 0x06001BF8 RID: 7160 RVA: 0x000C1B9A File Offset: 0x000BFD9A
	private void Update()
	{
		this.cam.farClipPlane = Mathf.Clamp(ConVar.Graphics.drawdistance, 500f, 2500f);
	}

	// Token: 0x04001537 RID: 5431
	private Camera cam;
}
