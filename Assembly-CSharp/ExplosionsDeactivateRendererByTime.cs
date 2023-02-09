using System;
using UnityEngine;

// Token: 0x02000960 RID: 2400
public class ExplosionsDeactivateRendererByTime : MonoBehaviour
{
	// Token: 0x060038A0 RID: 14496 RVA: 0x0014E2BC File Offset: 0x0014C4BC
	private void Awake()
	{
		this.rend = base.GetComponent<Renderer>();
	}

	// Token: 0x060038A1 RID: 14497 RVA: 0x0014E2CA File Offset: 0x0014C4CA
	private void DeactivateRenderer()
	{
		this.rend.enabled = false;
	}

	// Token: 0x060038A2 RID: 14498 RVA: 0x0014E2D8 File Offset: 0x0014C4D8
	private void OnEnable()
	{
		this.rend.enabled = true;
		base.Invoke("DeactivateRenderer", this.TimeDelay);
	}

	// Token: 0x040032F0 RID: 13040
	public float TimeDelay = 1f;

	// Token: 0x040032F1 RID: 13041
	private Renderer rend;
}
