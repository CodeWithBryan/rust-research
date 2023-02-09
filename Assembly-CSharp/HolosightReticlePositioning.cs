using System;
using UnityEngine;

// Token: 0x020001B6 RID: 438
public class HolosightReticlePositioning : MonoBehaviour
{
	// Token: 0x170001CD RID: 461
	// (get) Token: 0x060017E2 RID: 6114 RVA: 0x000B11AC File Offset: 0x000AF3AC
	public RectTransform rectTransform
	{
		get
		{
			return base.transform as RectTransform;
		}
	}

	// Token: 0x060017E3 RID: 6115 RVA: 0x000B11B9 File Offset: 0x000AF3B9
	private void Update()
	{
		if (MainCamera.isValid)
		{
			this.UpdatePosition(MainCamera.mainCamera);
		}
	}

	// Token: 0x060017E4 RID: 6116 RVA: 0x000B11D0 File Offset: 0x000AF3D0
	private void UpdatePosition(Camera cam)
	{
		Vector3 position = this.aimPoint.targetPoint.transform.position;
		Vector2 vector = RectTransformUtility.WorldToScreenPoint(cam, position);
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rectTransform.parent as RectTransform, vector, cam, out vector);
		vector.x /= (this.rectTransform.parent as RectTransform).rect.width * 0.5f;
		vector.y /= (this.rectTransform.parent as RectTransform).rect.height * 0.5f;
		this.rectTransform.anchoredPosition = vector;
	}

	// Token: 0x0400110A RID: 4362
	public IronsightAimPoint aimPoint;
}
