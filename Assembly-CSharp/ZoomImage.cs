using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020007AF RID: 1967
public class ZoomImage : MonoBehaviour, IScrollHandler, IEventSystemHandler
{
	// Token: 0x060033B2 RID: 13234 RVA: 0x0013B9BF File Offset: 0x00139BBF
	private void Awake()
	{
		this._thisTransform = (base.transform as RectTransform);
		this._scale.Set(this._initialScale, this._initialScale, 1f);
		this._thisTransform.localScale = this._scale;
	}

	// Token: 0x060033B3 RID: 13235 RVA: 0x0013BA00 File Offset: 0x00139C00
	public void OnScroll(PointerEventData eventData)
	{
		Vector2 a;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this._thisTransform, Input.mousePosition, null, out a);
		float y = eventData.scrollDelta.y;
		if (y > 0f && this._scale.x < this._maximumScale)
		{
			this._scale.Set(this._scale.x + this._scaleIncrement, this._scale.y + this._scaleIncrement, 1f);
			this._thisTransform.localScale = this._scale;
			this._thisTransform.anchoredPosition -= a * this._scaleIncrement;
			return;
		}
		if (y < 0f && this._scale.x > this._minimumScale)
		{
			this._scale.Set(this._scale.x - this._scaleIncrement, this._scale.y - this._scaleIncrement, 1f);
			this._thisTransform.localScale = this._scale;
			this._thisTransform.anchoredPosition += a * this._scaleIncrement;
		}
	}

	// Token: 0x04002B9F RID: 11167
	[SerializeField]
	private float _minimumScale = 0.5f;

	// Token: 0x04002BA0 RID: 11168
	[SerializeField]
	private float _initialScale = 1f;

	// Token: 0x04002BA1 RID: 11169
	[SerializeField]
	private float _maximumScale = 3f;

	// Token: 0x04002BA2 RID: 11170
	[SerializeField]
	private float _scaleIncrement = 0.5f;

	// Token: 0x04002BA3 RID: 11171
	[HideInInspector]
	private Vector3 _scale;

	// Token: 0x04002BA4 RID: 11172
	private RectTransform _thisTransform;
}
