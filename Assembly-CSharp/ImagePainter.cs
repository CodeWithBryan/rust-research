using System;
using Painting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Token: 0x020007C1 RID: 1985
public class ImagePainter : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IInitializePotentialDragHandler
{
	// Token: 0x170003FB RID: 1019
	// (get) Token: 0x060033DC RID: 13276 RVA: 0x000B11AC File Offset: 0x000AF3AC
	public RectTransform rectTransform
	{
		get
		{
			return base.transform as RectTransform;
		}
	}

	// Token: 0x060033DD RID: 13277 RVA: 0x0013C390 File Offset: 0x0013A590
	public virtual void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			return;
		}
		Vector2 position;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rectTransform, eventData.position, eventData.pressEventCamera, out position);
		this.DrawAt(position, eventData.button);
		this.pointerState[(int)eventData.button].isDown = true;
	}

	// Token: 0x060033DE RID: 13278 RVA: 0x0013C3E1 File Offset: 0x0013A5E1
	public virtual void OnPointerUp(PointerEventData eventData)
	{
		this.pointerState[(int)eventData.button].isDown = false;
	}

	// Token: 0x060033DF RID: 13279 RVA: 0x0013C3F8 File Offset: 0x0013A5F8
	public virtual void OnDrag(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			if (this.redirectRightClick)
			{
				this.redirectRightClick.SendMessage("OnDrag", eventData);
			}
			return;
		}
		Vector2 position;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rectTransform, eventData.position, eventData.pressEventCamera, out position);
		this.DrawAt(position, eventData.button);
	}

	// Token: 0x060033E0 RID: 13280 RVA: 0x0013C454 File Offset: 0x0013A654
	public virtual void OnBeginDrag(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			if (this.redirectRightClick)
			{
				this.redirectRightClick.SendMessage("OnBeginDrag", eventData);
			}
			return;
		}
	}

	// Token: 0x060033E1 RID: 13281 RVA: 0x0013C47E File Offset: 0x0013A67E
	public virtual void OnEndDrag(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			if (this.redirectRightClick)
			{
				this.redirectRightClick.SendMessage("OnEndDrag", eventData);
			}
			return;
		}
	}

	// Token: 0x060033E2 RID: 13282 RVA: 0x0013C4A8 File Offset: 0x0013A6A8
	public virtual void OnInitializePotentialDrag(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			if (this.redirectRightClick)
			{
				this.redirectRightClick.SendMessage("OnInitializePotentialDrag", eventData);
			}
			return;
		}
	}

	// Token: 0x060033E3 RID: 13283 RVA: 0x0013C4D4 File Offset: 0x0013A6D4
	private void DrawAt(Vector2 position, PointerEventData.InputButton button)
	{
		if (this.brush == null)
		{
			return;
		}
		ImagePainter.PointerState pointerState = this.pointerState[(int)button];
		Vector2 vector = this.rectTransform.Unpivot(position);
		if (pointerState.isDown)
		{
			Vector2 vector2 = pointerState.lastPos - vector;
			Vector2 normalized = vector2.normalized;
			for (float num = 0f; num < vector2.magnitude; num += Mathf.Max(this.brush.spacing, 1f) * Mathf.Max(this.spacingScale, 0.1f))
			{
				this.onDrawing.Invoke(vector + num * normalized, this.brush);
			}
			pointerState.lastPos = vector;
			return;
		}
		this.onDrawing.Invoke(vector, this.brush);
		pointerState.lastPos = vector;
	}

	// Token: 0x060033E4 RID: 13284 RVA: 0x000059DD File Offset: 0x00003BDD
	private void Start()
	{
	}

	// Token: 0x060033E5 RID: 13285 RVA: 0x0013C59C File Offset: 0x0013A79C
	public void UpdateBrush(Brush brush)
	{
		this.brush = brush;
	}

	// Token: 0x04002BFD RID: 11261
	public ImagePainter.OnDrawingEvent onDrawing = new ImagePainter.OnDrawingEvent();

	// Token: 0x04002BFE RID: 11262
	public MonoBehaviour redirectRightClick;

	// Token: 0x04002BFF RID: 11263
	[Tooltip("Spacing scale will depend on your texel size, tweak to what's right.")]
	public float spacingScale = 1f;

	// Token: 0x04002C00 RID: 11264
	internal Brush brush;

	// Token: 0x04002C01 RID: 11265
	internal ImagePainter.PointerState[] pointerState = new ImagePainter.PointerState[]
	{
		new ImagePainter.PointerState(),
		new ImagePainter.PointerState(),
		new ImagePainter.PointerState()
	};

	// Token: 0x02000E23 RID: 3619
	[Serializable]
	public class OnDrawingEvent : UnityEvent<Vector2, Brush>
	{
	}

	// Token: 0x02000E24 RID: 3620
	internal class PointerState
	{
		// Token: 0x0400495C RID: 18780
		public Vector2 lastPos;

		// Token: 0x0400495D RID: 18781
		public bool isDown;
	}
}
