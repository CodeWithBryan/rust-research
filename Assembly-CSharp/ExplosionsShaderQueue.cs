using System;
using UnityEngine;

// Token: 0x02000966 RID: 2406
public class ExplosionsShaderQueue : MonoBehaviour
{
	// Token: 0x060038B7 RID: 14519 RVA: 0x0014E814 File Offset: 0x0014CA14
	private void Start()
	{
		this.rend = base.GetComponent<Renderer>();
		if (this.rend != null)
		{
			this.rend.sharedMaterial.renderQueue += this.AddQueue;
			return;
		}
		base.Invoke("SetProjectorQueue", 0.1f);
	}

	// Token: 0x060038B8 RID: 14520 RVA: 0x0014E869 File Offset: 0x0014CA69
	private void SetProjectorQueue()
	{
		base.GetComponent<Projector>().material.renderQueue += this.AddQueue;
	}

	// Token: 0x060038B9 RID: 14521 RVA: 0x0014E888 File Offset: 0x0014CA88
	private void OnDisable()
	{
		if (this.rend != null)
		{
			this.rend.sharedMaterial.renderQueue = -1;
		}
	}

	// Token: 0x04003315 RID: 13077
	public int AddQueue = 1;

	// Token: 0x04003316 RID: 13078
	private Renderer rend;
}
