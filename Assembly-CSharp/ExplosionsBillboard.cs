using System;
using UnityEngine;

// Token: 0x0200095F RID: 2399
public class ExplosionsBillboard : MonoBehaviour
{
	// Token: 0x0600389D RID: 14493 RVA: 0x0014E14C File Offset: 0x0014C34C
	private void Awake()
	{
		if (this.AutoInitCamera)
		{
			this.Camera = Camera.main;
			this.Active = true;
		}
		this.t = base.transform;
		Vector3 localScale = this.t.parent.transform.localScale;
		localScale.z = localScale.x;
		this.t.parent.transform.localScale = localScale;
		this.camT = this.Camera.transform;
		Transform parent = this.t.parent;
		this.myContainer = new GameObject
		{
			name = "Billboard_" + this.t.gameObject.name
		};
		this.contT = this.myContainer.transform;
		this.contT.position = this.t.position;
		this.t.parent = this.myContainer.transform;
		this.contT.parent = parent;
	}

	// Token: 0x0600389E RID: 14494 RVA: 0x0014E24C File Offset: 0x0014C44C
	private void Update()
	{
		if (this.Active)
		{
			this.contT.LookAt(this.contT.position + this.camT.rotation * Vector3.back, this.camT.rotation * Vector3.up);
		}
	}

	// Token: 0x040032E9 RID: 13033
	public Camera Camera;

	// Token: 0x040032EA RID: 13034
	public bool Active = true;

	// Token: 0x040032EB RID: 13035
	public bool AutoInitCamera = true;

	// Token: 0x040032EC RID: 13036
	private GameObject myContainer;

	// Token: 0x040032ED RID: 13037
	private Transform t;

	// Token: 0x040032EE RID: 13038
	private Transform camT;

	// Token: 0x040032EF RID: 13039
	private Transform contT;
}
