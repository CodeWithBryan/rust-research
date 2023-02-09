using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000885 RID: 2181
public class ScrollRectZoom : MonoBehaviour, IScrollHandler, IEventSystemHandler
{
	// Token: 0x17000405 RID: 1029
	// (get) Token: 0x06003571 RID: 13681 RVA: 0x0014190B File Offset: 0x0013FB0B
	public RectTransform rectTransform
	{
		get
		{
			return this.scrollRect.transform as RectTransform;
		}
	}

	// Token: 0x06003572 RID: 13682 RVA: 0x0014191D File Offset: 0x0013FB1D
	private void OnEnable()
	{
		this.SetZoom(this.zoom, true);
	}

	// Token: 0x06003573 RID: 13683 RVA: 0x0014192C File Offset: 0x0013FB2C
	public void OnScroll(PointerEventData data)
	{
		if (this.mouseWheelZoom)
		{
			this.SetZoom(this.zoom + this.scrollAmount * data.scrollDelta.y, true);
		}
	}

	// Token: 0x06003574 RID: 13684 RVA: 0x00141958 File Offset: 0x0013FB58
	public void SetZoom(float z, bool expZoom = true)
	{
		z = Mathf.Clamp(z, this.min, this.max);
		this.zoom = z;
		Vector2 normalizedPosition = this.scrollRect.normalizedPosition;
		if (expZoom)
		{
			this.scrollRect.content.localScale = Vector3.one * Mathf.Exp(this.zoom);
		}
		else
		{
			this.scrollRect.content.localScale = Vector3.one * this.zoom;
		}
		this.scrollRect.normalizedPosition = normalizedPosition;
	}

	// Token: 0x04003055 RID: 12373
	public ScrollRectEx scrollRect;

	// Token: 0x04003056 RID: 12374
	public float zoom = 1f;

	// Token: 0x04003057 RID: 12375
	public float max = 1.5f;

	// Token: 0x04003058 RID: 12376
	public float min = 0.5f;

	// Token: 0x04003059 RID: 12377
	public bool mouseWheelZoom = true;

	// Token: 0x0400305A RID: 12378
	public float scrollAmount = 0.2f;
}
