using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008A0 RID: 2208
public abstract class UIRoot : MonoBehaviour
{
	// Token: 0x060035C9 RID: 13769 RVA: 0x00142B4C File Offset: 0x00140D4C
	private void ToggleRaycasters(bool state)
	{
		for (int i = 0; i < this.graphicRaycasters.Length; i++)
		{
			GraphicRaycaster graphicRaycaster = this.graphicRaycasters[i];
			if (graphicRaycaster.enabled != state)
			{
				graphicRaycaster.enabled = state;
			}
		}
	}

	// Token: 0x060035CA RID: 13770 RVA: 0x000059DD File Offset: 0x00003BDD
	protected virtual void Awake()
	{
	}

	// Token: 0x060035CB RID: 13771 RVA: 0x00142B85 File Offset: 0x00140D85
	protected virtual void Start()
	{
		this.graphicRaycasters = base.GetComponentsInChildren<GraphicRaycaster>(true);
	}

	// Token: 0x060035CC RID: 13772 RVA: 0x00142B94 File Offset: 0x00140D94
	protected void Update()
	{
		this.Refresh();
	}

	// Token: 0x060035CD RID: 13773
	protected abstract void Refresh();

	// Token: 0x040030D4 RID: 12500
	private GraphicRaycaster[] graphicRaycasters;

	// Token: 0x040030D5 RID: 12501
	public Canvas overlayCanvas;
}
