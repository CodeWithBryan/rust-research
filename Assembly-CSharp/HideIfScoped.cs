using System;
using UnityEngine;

// Token: 0x020001B8 RID: 440
public class HideIfScoped : MonoBehaviour
{
	// Token: 0x060017E7 RID: 6119 RVA: 0x000B12BC File Offset: 0x000AF4BC
	public void SetVisible(bool vis)
	{
		Renderer[] array = this.renderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = vis;
		}
	}

	// Token: 0x04001118 RID: 4376
	public Renderer[] renderers;
}
