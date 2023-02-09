using System;
using UnityEngine;

// Token: 0x020004DE RID: 1246
public class EnvironmentVolumeTrigger : MonoBehaviour
{
	// Token: 0x17000329 RID: 809
	// (get) Token: 0x060027B6 RID: 10166 RVA: 0x000F3826 File Offset: 0x000F1A26
	// (set) Token: 0x060027B7 RID: 10167 RVA: 0x000F382E File Offset: 0x000F1A2E
	public EnvironmentVolume volume { get; private set; }

	// Token: 0x060027B8 RID: 10168 RVA: 0x000F3838 File Offset: 0x000F1A38
	protected void Awake()
	{
		this.volume = base.gameObject.GetComponent<EnvironmentVolume>();
		if (this.volume == null)
		{
			this.volume = base.gameObject.AddComponent<EnvironmentVolume>();
			this.volume.Center = this.Center;
			this.volume.Size = this.Size;
			this.volume.UpdateTrigger();
		}
	}

	// Token: 0x04001FE5 RID: 8165
	[HideInInspector]
	public Vector3 Center = Vector3.zero;

	// Token: 0x04001FE6 RID: 8166
	[HideInInspector]
	public Vector3 Size = Vector3.one;
}
