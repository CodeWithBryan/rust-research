using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200089E RID: 2206
public class UIPixelDownscale : MonoBehaviour
{
	// Token: 0x060035C3 RID: 13763 RVA: 0x001429F4 File Offset: 0x00140BF4
	private void Awake()
	{
		if (this.CanvasScaler == null)
		{
			this.CanvasScaler = base.GetComponent<CanvasScaler>();
			if (this.CanvasScaler == null)
			{
				Debug.LogError(base.GetType().Name + " is attached to a gameobject that is missing a canvas scaler");
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}

	// Token: 0x060035C4 RID: 13764 RVA: 0x00142A50 File Offset: 0x00140C50
	private void Update()
	{
		if ((float)Screen.width < this.CanvasScaler.referenceResolution.x || (float)Screen.height < this.CanvasScaler.referenceResolution.y)
		{
			this.CanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
			return;
		}
		this.CanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
	}

	// Token: 0x040030D1 RID: 12497
	public CanvasScaler CanvasScaler;
}
