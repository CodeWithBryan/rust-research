using System;
using Rust;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	// Token: 0x020009EA RID: 2538
	[AddComponentMenu("UI/Scroll Rect Ex", 37)]
	[SelectionBase]
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	public class ScrollRectEx : UIBehaviour, IInitializePotentialDragHandler, IEventSystemHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler, ICanvasElement, ILayoutGroup, ILayoutController
	{
		// Token: 0x170004A3 RID: 1187
		// (get) Token: 0x06003BC3 RID: 15299 RVA: 0x0015CB42 File Offset: 0x0015AD42
		// (set) Token: 0x06003BC4 RID: 15300 RVA: 0x0015CB4A File Offset: 0x0015AD4A
		public RectTransform content
		{
			get
			{
				return this.m_Content;
			}
			set
			{
				this.m_Content = value;
			}
		}

		// Token: 0x170004A4 RID: 1188
		// (get) Token: 0x06003BC5 RID: 15301 RVA: 0x0015CB53 File Offset: 0x0015AD53
		// (set) Token: 0x06003BC6 RID: 15302 RVA: 0x0015CB5B File Offset: 0x0015AD5B
		public bool horizontal
		{
			get
			{
				return this.m_Horizontal;
			}
			set
			{
				this.m_Horizontal = value;
			}
		}

		// Token: 0x170004A5 RID: 1189
		// (get) Token: 0x06003BC7 RID: 15303 RVA: 0x0015CB64 File Offset: 0x0015AD64
		// (set) Token: 0x06003BC8 RID: 15304 RVA: 0x0015CB6C File Offset: 0x0015AD6C
		public bool vertical
		{
			get
			{
				return this.m_Vertical;
			}
			set
			{
				this.m_Vertical = value;
			}
		}

		// Token: 0x170004A6 RID: 1190
		// (get) Token: 0x06003BC9 RID: 15305 RVA: 0x0015CB75 File Offset: 0x0015AD75
		// (set) Token: 0x06003BCA RID: 15306 RVA: 0x0015CB7D File Offset: 0x0015AD7D
		public ScrollRectEx.MovementType movementType
		{
			get
			{
				return this.m_MovementType;
			}
			set
			{
				this.m_MovementType = value;
			}
		}

		// Token: 0x170004A7 RID: 1191
		// (get) Token: 0x06003BCB RID: 15307 RVA: 0x0015CB86 File Offset: 0x0015AD86
		// (set) Token: 0x06003BCC RID: 15308 RVA: 0x0015CB8E File Offset: 0x0015AD8E
		public float elasticity
		{
			get
			{
				return this.m_Elasticity;
			}
			set
			{
				this.m_Elasticity = value;
			}
		}

		// Token: 0x170004A8 RID: 1192
		// (get) Token: 0x06003BCD RID: 15309 RVA: 0x0015CB97 File Offset: 0x0015AD97
		// (set) Token: 0x06003BCE RID: 15310 RVA: 0x0015CB9F File Offset: 0x0015AD9F
		public bool inertia
		{
			get
			{
				return this.m_Inertia;
			}
			set
			{
				this.m_Inertia = value;
			}
		}

		// Token: 0x170004A9 RID: 1193
		// (get) Token: 0x06003BCF RID: 15311 RVA: 0x0015CBA8 File Offset: 0x0015ADA8
		// (set) Token: 0x06003BD0 RID: 15312 RVA: 0x0015CBB0 File Offset: 0x0015ADB0
		public float decelerationRate
		{
			get
			{
				return this.m_DecelerationRate;
			}
			set
			{
				this.m_DecelerationRate = value;
			}
		}

		// Token: 0x170004AA RID: 1194
		// (get) Token: 0x06003BD1 RID: 15313 RVA: 0x0015CBB9 File Offset: 0x0015ADB9
		// (set) Token: 0x06003BD2 RID: 15314 RVA: 0x0015CBC1 File Offset: 0x0015ADC1
		public float scrollSensitivity
		{
			get
			{
				return this.m_ScrollSensitivity;
			}
			set
			{
				this.m_ScrollSensitivity = value;
			}
		}

		// Token: 0x170004AB RID: 1195
		// (get) Token: 0x06003BD3 RID: 15315 RVA: 0x0015CBCA File Offset: 0x0015ADCA
		// (set) Token: 0x06003BD4 RID: 15316 RVA: 0x0015CBD2 File Offset: 0x0015ADD2
		public RectTransform viewport
		{
			get
			{
				return this.m_Viewport;
			}
			set
			{
				this.m_Viewport = value;
				this.SetDirtyCaching();
			}
		}

		// Token: 0x170004AC RID: 1196
		// (get) Token: 0x06003BD5 RID: 15317 RVA: 0x0015CBE1 File Offset: 0x0015ADE1
		// (set) Token: 0x06003BD6 RID: 15318 RVA: 0x0015CBEC File Offset: 0x0015ADEC
		public Scrollbar horizontalScrollbar
		{
			get
			{
				return this.m_HorizontalScrollbar;
			}
			set
			{
				if (this.m_HorizontalScrollbar)
				{
					this.m_HorizontalScrollbar.onValueChanged.RemoveListener(new UnityAction<float>(this.SetHorizontalNormalizedPosition));
				}
				this.m_HorizontalScrollbar = value;
				if (this.m_HorizontalScrollbar)
				{
					this.m_HorizontalScrollbar.onValueChanged.AddListener(new UnityAction<float>(this.SetHorizontalNormalizedPosition));
				}
				this.SetDirtyCaching();
			}
		}

		// Token: 0x170004AD RID: 1197
		// (get) Token: 0x06003BD7 RID: 15319 RVA: 0x0015CC58 File Offset: 0x0015AE58
		// (set) Token: 0x06003BD8 RID: 15320 RVA: 0x0015CC60 File Offset: 0x0015AE60
		public Scrollbar verticalScrollbar
		{
			get
			{
				return this.m_VerticalScrollbar;
			}
			set
			{
				if (this.m_VerticalScrollbar)
				{
					this.m_VerticalScrollbar.onValueChanged.RemoveListener(new UnityAction<float>(this.SetVerticalNormalizedPosition));
				}
				this.m_VerticalScrollbar = value;
				if (this.m_VerticalScrollbar)
				{
					this.m_VerticalScrollbar.onValueChanged.AddListener(new UnityAction<float>(this.SetVerticalNormalizedPosition));
				}
				this.SetDirtyCaching();
			}
		}

		// Token: 0x170004AE RID: 1198
		// (get) Token: 0x06003BD9 RID: 15321 RVA: 0x0015CCCC File Offset: 0x0015AECC
		// (set) Token: 0x06003BDA RID: 15322 RVA: 0x0015CCD4 File Offset: 0x0015AED4
		public ScrollRectEx.ScrollbarVisibility horizontalScrollbarVisibility
		{
			get
			{
				return this.m_HorizontalScrollbarVisibility;
			}
			set
			{
				this.m_HorizontalScrollbarVisibility = value;
				this.SetDirtyCaching();
			}
		}

		// Token: 0x170004AF RID: 1199
		// (get) Token: 0x06003BDB RID: 15323 RVA: 0x0015CCE3 File Offset: 0x0015AEE3
		// (set) Token: 0x06003BDC RID: 15324 RVA: 0x0015CCEB File Offset: 0x0015AEEB
		public ScrollRectEx.ScrollbarVisibility verticalScrollbarVisibility
		{
			get
			{
				return this.m_VerticalScrollbarVisibility;
			}
			set
			{
				this.m_VerticalScrollbarVisibility = value;
				this.SetDirtyCaching();
			}
		}

		// Token: 0x170004B0 RID: 1200
		// (get) Token: 0x06003BDD RID: 15325 RVA: 0x0015CCFA File Offset: 0x0015AEFA
		// (set) Token: 0x06003BDE RID: 15326 RVA: 0x0015CD02 File Offset: 0x0015AF02
		public float horizontalScrollbarSpacing
		{
			get
			{
				return this.m_HorizontalScrollbarSpacing;
			}
			set
			{
				this.m_HorizontalScrollbarSpacing = value;
				this.SetDirty();
			}
		}

		// Token: 0x170004B1 RID: 1201
		// (get) Token: 0x06003BDF RID: 15327 RVA: 0x0015CD11 File Offset: 0x0015AF11
		// (set) Token: 0x06003BE0 RID: 15328 RVA: 0x0015CD19 File Offset: 0x0015AF19
		public float verticalScrollbarSpacing
		{
			get
			{
				return this.m_VerticalScrollbarSpacing;
			}
			set
			{
				this.m_VerticalScrollbarSpacing = value;
				this.SetDirty();
			}
		}

		// Token: 0x170004B2 RID: 1202
		// (get) Token: 0x06003BE1 RID: 15329 RVA: 0x0015CD28 File Offset: 0x0015AF28
		// (set) Token: 0x06003BE2 RID: 15330 RVA: 0x0015CD30 File Offset: 0x0015AF30
		public ScrollRectEx.ScrollRectEvent onValueChanged
		{
			get
			{
				return this.m_OnValueChanged;
			}
			set
			{
				this.m_OnValueChanged = value;
			}
		}

		// Token: 0x170004B3 RID: 1203
		// (get) Token: 0x06003BE3 RID: 15331 RVA: 0x0015CD3C File Offset: 0x0015AF3C
		protected RectTransform viewRect
		{
			get
			{
				if (this.m_ViewRect == null)
				{
					this.m_ViewRect = this.m_Viewport;
				}
				if (this.m_ViewRect == null)
				{
					this.m_ViewRect = (RectTransform)base.transform;
				}
				return this.m_ViewRect;
			}
		}

		// Token: 0x170004B4 RID: 1204
		// (get) Token: 0x06003BE4 RID: 15332 RVA: 0x0015CD88 File Offset: 0x0015AF88
		// (set) Token: 0x06003BE5 RID: 15333 RVA: 0x0015CD90 File Offset: 0x0015AF90
		public Vector2 velocity
		{
			get
			{
				return this.m_Velocity;
			}
			set
			{
				this.m_Velocity = value;
			}
		}

		// Token: 0x170004B5 RID: 1205
		// (get) Token: 0x06003BE6 RID: 15334 RVA: 0x0015CD99 File Offset: 0x0015AF99
		private RectTransform rectTransform
		{
			get
			{
				if (this.m_Rect == null)
				{
					this.m_Rect = base.GetComponent<RectTransform>();
				}
				return this.m_Rect;
			}
		}

		// Token: 0x06003BE7 RID: 15335 RVA: 0x0015CDBC File Offset: 0x0015AFBC
		protected ScrollRectEx()
		{
		}

		// Token: 0x06003BE8 RID: 15336 RVA: 0x0015CE44 File Offset: 0x0015B044
		public virtual void Rebuild(CanvasUpdate executing)
		{
			if (executing == CanvasUpdate.Prelayout)
			{
				this.UpdateCachedData();
			}
			if (executing == CanvasUpdate.PostLayout)
			{
				this.UpdateBounds();
				this.UpdateScrollbars(Vector2.zero);
				this.UpdatePrevData();
				this.m_HasRebuiltLayout = true;
			}
		}

		// Token: 0x06003BE9 RID: 15337 RVA: 0x0015CE74 File Offset: 0x0015B074
		private void UpdateCachedData()
		{
			Transform transform = base.transform;
			this.m_HorizontalScrollbarRect = ((this.m_HorizontalScrollbar == null) ? null : (this.m_HorizontalScrollbar.transform as RectTransform));
			this.m_VerticalScrollbarRect = ((this.m_VerticalScrollbar == null) ? null : (this.m_VerticalScrollbar.transform as RectTransform));
			bool flag = this.viewRect.parent == transform;
			bool flag2 = !this.m_HorizontalScrollbarRect || this.m_HorizontalScrollbarRect.parent == transform;
			bool flag3 = !this.m_VerticalScrollbarRect || this.m_VerticalScrollbarRect.parent == transform;
			bool flag4 = flag && flag2 && flag3;
			this.m_HSliderExpand = (flag4 && this.m_HorizontalScrollbarRect && this.horizontalScrollbarVisibility == ScrollRectEx.ScrollbarVisibility.AutoHideAndExpandViewport);
			this.m_VSliderExpand = (flag4 && this.m_VerticalScrollbarRect && this.verticalScrollbarVisibility == ScrollRectEx.ScrollbarVisibility.AutoHideAndExpandViewport);
			this.m_HSliderHeight = ((this.m_HorizontalScrollbarRect == null) ? 0f : this.m_HorizontalScrollbarRect.rect.height);
			this.m_VSliderWidth = ((this.m_VerticalScrollbarRect == null) ? 0f : this.m_VerticalScrollbarRect.rect.width);
		}

		// Token: 0x06003BEA RID: 15338 RVA: 0x0015CFD4 File Offset: 0x0015B1D4
		protected override void OnEnable()
		{
			base.OnEnable();
			if (this.m_HorizontalScrollbar)
			{
				this.m_HorizontalScrollbar.onValueChanged.AddListener(new UnityAction<float>(this.SetHorizontalNormalizedPosition));
			}
			if (this.m_VerticalScrollbar)
			{
				this.m_VerticalScrollbar.onValueChanged.AddListener(new UnityAction<float>(this.SetVerticalNormalizedPosition));
			}
			CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
		}

		// Token: 0x06003BEB RID: 15339 RVA: 0x0015D040 File Offset: 0x0015B240
		protected override void OnDisable()
		{
			if (Application.isQuitting)
			{
				return;
			}
			CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);
			if (this.m_HorizontalScrollbar)
			{
				this.m_HorizontalScrollbar.onValueChanged.RemoveListener(new UnityAction<float>(this.SetHorizontalNormalizedPosition));
			}
			if (this.m_VerticalScrollbar)
			{
				this.m_VerticalScrollbar.onValueChanged.RemoveListener(new UnityAction<float>(this.SetVerticalNormalizedPosition));
			}
			this.m_HasRebuiltLayout = false;
			this.m_Tracker.Clear();
			LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
			base.OnDisable();
		}

		// Token: 0x06003BEC RID: 15340 RVA: 0x0015D0D0 File Offset: 0x0015B2D0
		public override bool IsActive()
		{
			return base.IsActive() && this.m_Content != null;
		}

		// Token: 0x06003BED RID: 15341 RVA: 0x0015D0E8 File Offset: 0x0015B2E8
		private void EnsureLayoutHasRebuilt()
		{
			if (!this.m_HasRebuiltLayout && !CanvasUpdateRegistry.IsRebuildingLayout())
			{
				Canvas.ForceUpdateCanvases();
			}
		}

		// Token: 0x06003BEE RID: 15342 RVA: 0x0015D0FE File Offset: 0x0015B2FE
		public virtual void StopMovement()
		{
			this.m_Velocity = Vector2.zero;
		}

		// Token: 0x06003BEF RID: 15343 RVA: 0x0015D10C File Offset: 0x0015B30C
		public virtual void OnScroll(PointerEventData data)
		{
			if (!this.IsActive())
			{
				return;
			}
			this.EnsureLayoutHasRebuilt();
			this.UpdateBounds();
			Vector2 scrollDelta = data.scrollDelta;
			scrollDelta.y *= -1f;
			if (this.vertical && !this.horizontal)
			{
				if (Mathf.Abs(scrollDelta.x) > Mathf.Abs(scrollDelta.y))
				{
					scrollDelta.y = scrollDelta.x;
				}
				scrollDelta.x = 0f;
			}
			if (this.horizontal && !this.vertical)
			{
				if (Mathf.Abs(scrollDelta.y) > Mathf.Abs(scrollDelta.x))
				{
					scrollDelta.x = scrollDelta.y;
				}
				scrollDelta.y = 0f;
			}
			Vector2 vector = this.m_Content.anchoredPosition;
			vector += scrollDelta * this.m_ScrollSensitivity;
			if (this.m_MovementType == ScrollRectEx.MovementType.Clamped)
			{
				vector += this.CalculateOffset(vector - this.m_Content.anchoredPosition);
			}
			this.SetContentAnchoredPosition(vector);
			this.UpdateBounds();
		}

		// Token: 0x06003BF0 RID: 15344 RVA: 0x0015D21A File Offset: 0x0015B41A
		public virtual void OnInitializePotentialDrag(PointerEventData eventData)
		{
			if (eventData.button != this.scrollButton && eventData.button != this.altScrollButton)
			{
				return;
			}
			this.m_Velocity = Vector2.zero;
		}

		// Token: 0x06003BF1 RID: 15345 RVA: 0x0015D244 File Offset: 0x0015B444
		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			if (eventData.button != this.scrollButton && eventData.button != this.altScrollButton)
			{
				return;
			}
			if (!this.IsActive())
			{
				return;
			}
			this.UpdateBounds();
			this.m_PointerStartLocalCursor = Vector2.zero;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.viewRect, eventData.position, eventData.pressEventCamera, out this.m_PointerStartLocalCursor);
			this.m_ContentStartPosition = this.m_Content.anchoredPosition;
			this.m_Dragging = true;
		}

		// Token: 0x06003BF2 RID: 15346 RVA: 0x0015D2BE File Offset: 0x0015B4BE
		public virtual void OnEndDrag(PointerEventData eventData)
		{
			if (eventData.button != this.scrollButton && eventData.button != this.altScrollButton)
			{
				return;
			}
			this.m_Dragging = false;
		}

		// Token: 0x06003BF3 RID: 15347 RVA: 0x0015D2E4 File Offset: 0x0015B4E4
		public virtual void OnDrag(PointerEventData eventData)
		{
			if (eventData.button != this.scrollButton && eventData.button != this.altScrollButton)
			{
				return;
			}
			if (!this.IsActive())
			{
				return;
			}
			Vector2 a;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(this.viewRect, eventData.position, eventData.pressEventCamera, out a))
			{
				return;
			}
			this.UpdateBounds();
			Vector2 b = a - this.m_PointerStartLocalCursor;
			Vector2 vector = this.m_ContentStartPosition + b;
			Vector2 vector2 = this.CalculateOffset(vector - this.m_Content.anchoredPosition);
			vector += vector2;
			if (this.m_MovementType == ScrollRectEx.MovementType.Elastic)
			{
				if (vector2.x != 0f)
				{
					vector.x -= ScrollRectEx.RubberDelta(vector2.x, this.m_ViewBounds.size.x);
				}
				if (vector2.y != 0f)
				{
					vector.y -= ScrollRectEx.RubberDelta(vector2.y, this.m_ViewBounds.size.y);
				}
			}
			this.SetContentAnchoredPosition(vector);
		}

		// Token: 0x06003BF4 RID: 15348 RVA: 0x0015D3F0 File Offset: 0x0015B5F0
		protected virtual void SetContentAnchoredPosition(Vector2 position)
		{
			if (!this.m_Horizontal)
			{
				position.x = this.m_Content.anchoredPosition.x;
			}
			if (!this.m_Vertical)
			{
				position.y = this.m_Content.anchoredPosition.y;
			}
			if (position != this.m_Content.anchoredPosition)
			{
				this.m_Content.anchoredPosition = position;
				this.UpdateBounds();
			}
		}

		// Token: 0x06003BF5 RID: 15349 RVA: 0x0015D460 File Offset: 0x0015B660
		protected virtual void LateUpdate()
		{
			if (!this.m_Content)
			{
				return;
			}
			this.EnsureLayoutHasRebuilt();
			this.UpdateScrollbarVisibility();
			this.UpdateBounds();
			float unscaledDeltaTime = Time.unscaledDeltaTime;
			Vector2 vector = this.CalculateOffset(Vector2.zero);
			if (!this.m_Dragging && (vector != Vector2.zero || this.m_Velocity != Vector2.zero))
			{
				Vector2 vector2 = this.m_Content.anchoredPosition;
				for (int i = 0; i < 2; i++)
				{
					if (this.m_MovementType == ScrollRectEx.MovementType.Elastic && vector[i] != 0f)
					{
						float value = this.m_Velocity[i];
						vector2[i] = Mathf.SmoothDamp(this.m_Content.anchoredPosition[i], this.m_Content.anchoredPosition[i] + vector[i], ref value, this.m_Elasticity, float.PositiveInfinity, unscaledDeltaTime);
						this.m_Velocity[i] = value;
					}
					else if (this.m_Inertia)
					{
						ref Vector2 ptr = ref this.m_Velocity;
						int index = i;
						ptr[index] *= Mathf.Pow(this.m_DecelerationRate, unscaledDeltaTime);
						if (Mathf.Abs(this.m_Velocity[i]) < 1f)
						{
							this.m_Velocity[i] = 0f;
						}
						ptr = ref vector2;
						index = i;
						ptr[index] += this.m_Velocity[i] * unscaledDeltaTime;
					}
					else
					{
						this.m_Velocity[i] = 0f;
					}
				}
				if (this.m_Velocity != Vector2.zero)
				{
					if (this.m_MovementType == ScrollRectEx.MovementType.Clamped)
					{
						vector = this.CalculateOffset(vector2 - this.m_Content.anchoredPosition);
						vector2 += vector;
					}
					this.SetContentAnchoredPosition(vector2);
				}
			}
			if (this.m_Dragging && this.m_Inertia)
			{
				Vector3 b = (this.m_Content.anchoredPosition - this.m_PrevPosition) / unscaledDeltaTime;
				this.m_Velocity = Vector3.Lerp(this.m_Velocity, b, unscaledDeltaTime * 10f);
			}
			if (this.m_ViewBounds != this.m_PrevViewBounds || this.m_ContentBounds != this.m_PrevContentBounds || this.m_Content.anchoredPosition != this.m_PrevPosition)
			{
				this.UpdateScrollbars(vector);
				this.m_OnValueChanged.Invoke(this.normalizedPosition);
				this.UpdatePrevData();
			}
		}

		// Token: 0x06003BF6 RID: 15350 RVA: 0x0015D6FC File Offset: 0x0015B8FC
		private void UpdatePrevData()
		{
			if (this.m_Content == null)
			{
				this.m_PrevPosition = Vector2.zero;
			}
			else
			{
				this.m_PrevPosition = this.m_Content.anchoredPosition;
			}
			this.m_PrevViewBounds = this.m_ViewBounds;
			this.m_PrevContentBounds = this.m_ContentBounds;
		}

		// Token: 0x06003BF7 RID: 15351 RVA: 0x0015D750 File Offset: 0x0015B950
		private void UpdateScrollbars(Vector2 offset)
		{
			if (this.m_HorizontalScrollbar)
			{
				if (this.m_ContentBounds.size.x > 0f)
				{
					this.m_HorizontalScrollbar.size = Mathf.Clamp01((this.m_ViewBounds.size.x - Mathf.Abs(offset.x)) / this.m_ContentBounds.size.x);
				}
				else
				{
					this.m_HorizontalScrollbar.size = 1f;
				}
				this.m_HorizontalScrollbar.value = this.horizontalNormalizedPosition;
			}
			if (this.m_VerticalScrollbar)
			{
				if (this.m_ContentBounds.size.y > 0f)
				{
					this.m_VerticalScrollbar.size = Mathf.Clamp01((this.m_ViewBounds.size.y - Mathf.Abs(offset.y)) / this.m_ContentBounds.size.y);
				}
				else
				{
					this.m_VerticalScrollbar.size = 1f;
				}
				this.m_VerticalScrollbar.value = this.verticalNormalizedPosition;
			}
		}

		// Token: 0x170004B6 RID: 1206
		// (get) Token: 0x06003BF8 RID: 15352 RVA: 0x0015D865 File Offset: 0x0015BA65
		// (set) Token: 0x06003BF9 RID: 15353 RVA: 0x0015D878 File Offset: 0x0015BA78
		public Vector2 normalizedPosition
		{
			get
			{
				return new Vector2(this.horizontalNormalizedPosition, this.verticalNormalizedPosition);
			}
			set
			{
				this.SetNormalizedPosition(value.x, 0);
				this.SetNormalizedPosition(value.y, 1);
			}
		}

		// Token: 0x170004B7 RID: 1207
		// (get) Token: 0x06003BFA RID: 15354 RVA: 0x0015D894 File Offset: 0x0015BA94
		// (set) Token: 0x06003BFB RID: 15355 RVA: 0x0015D934 File Offset: 0x0015BB34
		public float horizontalNormalizedPosition
		{
			get
			{
				this.UpdateBounds();
				if (this.m_ContentBounds.size.x <= this.m_ViewBounds.size.x)
				{
					return (float)((this.m_ViewBounds.min.x > this.m_ContentBounds.min.x) ? 1 : 0);
				}
				return (this.m_ViewBounds.min.x - this.m_ContentBounds.min.x) / (this.m_ContentBounds.size.x - this.m_ViewBounds.size.x);
			}
			set
			{
				this.SetNormalizedPosition(value, 0);
			}
		}

		// Token: 0x170004B8 RID: 1208
		// (get) Token: 0x06003BFC RID: 15356 RVA: 0x0015D940 File Offset: 0x0015BB40
		// (set) Token: 0x06003BFD RID: 15357 RVA: 0x0015D9E0 File Offset: 0x0015BBE0
		public float verticalNormalizedPosition
		{
			get
			{
				this.UpdateBounds();
				if (this.m_ContentBounds.size.y <= this.m_ViewBounds.size.y)
				{
					return (float)((this.m_ViewBounds.min.y > this.m_ContentBounds.min.y) ? 1 : 0);
				}
				return (this.m_ViewBounds.min.y - this.m_ContentBounds.min.y) / (this.m_ContentBounds.size.y - this.m_ViewBounds.size.y);
			}
			set
			{
				this.SetNormalizedPosition(value, 1);
			}
		}

		// Token: 0x06003BFE RID: 15358 RVA: 0x0015D934 File Offset: 0x0015BB34
		private void SetHorizontalNormalizedPosition(float value)
		{
			this.SetNormalizedPosition(value, 0);
		}

		// Token: 0x06003BFF RID: 15359 RVA: 0x0015D9E0 File Offset: 0x0015BBE0
		private void SetVerticalNormalizedPosition(float value)
		{
			this.SetNormalizedPosition(value, 1);
		}

		// Token: 0x06003C00 RID: 15360 RVA: 0x0015D9EC File Offset: 0x0015BBEC
		private void SetNormalizedPosition(float value, int axis)
		{
			this.EnsureLayoutHasRebuilt();
			this.UpdateBounds();
			float num = this.m_ContentBounds.size[axis] - this.m_ViewBounds.size[axis];
			float num2 = this.m_ViewBounds.min[axis] - value * num;
			float num3 = this.m_Content.localPosition[axis] + num2 - this.m_ContentBounds.min[axis];
			Vector3 localPosition = this.m_Content.localPosition;
			if (Mathf.Abs(localPosition[axis] - num3) > 0.01f)
			{
				localPosition[axis] = num3;
				this.m_Content.localPosition = localPosition;
				this.m_Velocity[axis] = 0f;
				this.UpdateBounds();
			}
		}

		// Token: 0x06003C01 RID: 15361 RVA: 0x0015DAC7 File Offset: 0x0015BCC7
		private static float RubberDelta(float overStretching, float viewSize)
		{
			return (1f - 1f / (Mathf.Abs(overStretching) * 0.55f / viewSize + 1f)) * viewSize * Mathf.Sign(overStretching);
		}

		// Token: 0x06003C02 RID: 15362 RVA: 0x0015DAF2 File Offset: 0x0015BCF2
		protected override void OnRectTransformDimensionsChange()
		{
			this.SetDirty();
		}

		// Token: 0x170004B9 RID: 1209
		// (get) Token: 0x06003C03 RID: 15363 RVA: 0x0015DAFA File Offset: 0x0015BCFA
		private bool hScrollingNeeded
		{
			get
			{
				return !Application.isPlaying || this.m_ContentBounds.size.x > this.m_ViewBounds.size.x + 0.01f;
			}
		}

		// Token: 0x170004BA RID: 1210
		// (get) Token: 0x06003C04 RID: 15364 RVA: 0x0015DB2D File Offset: 0x0015BD2D
		private bool vScrollingNeeded
		{
			get
			{
				return !Application.isPlaying || this.m_ContentBounds.size.y > this.m_ViewBounds.size.y + 0.01f;
			}
		}

		// Token: 0x06003C05 RID: 15365 RVA: 0x0015DB60 File Offset: 0x0015BD60
		public virtual void SetLayoutHorizontal()
		{
			this.m_Tracker.Clear();
			if (this.m_HSliderExpand || this.m_VSliderExpand)
			{
				this.m_Tracker.Add(this, this.viewRect, DrivenTransformProperties.AnchoredPositionX | DrivenTransformProperties.AnchoredPositionY | DrivenTransformProperties.AnchorMinX | DrivenTransformProperties.AnchorMinY | DrivenTransformProperties.AnchorMaxX | DrivenTransformProperties.AnchorMaxY | DrivenTransformProperties.SizeDeltaX | DrivenTransformProperties.SizeDeltaY);
				this.viewRect.anchorMin = Vector2.zero;
				this.viewRect.anchorMax = Vector2.one;
				this.viewRect.sizeDelta = Vector2.zero;
				this.viewRect.anchoredPosition = Vector2.zero;
				LayoutRebuilder.ForceRebuildLayoutImmediate(this.content);
				this.m_ViewBounds = new Bounds(this.viewRect.rect.center, this.viewRect.rect.size);
				this.m_ContentBounds = this.GetBounds();
			}
			if (this.m_VSliderExpand && this.vScrollingNeeded)
			{
				this.viewRect.sizeDelta = new Vector2(-(this.m_VSliderWidth + this.m_VerticalScrollbarSpacing), this.viewRect.sizeDelta.y);
				LayoutRebuilder.ForceRebuildLayoutImmediate(this.content);
				this.m_ViewBounds = new Bounds(this.viewRect.rect.center, this.viewRect.rect.size);
				this.m_ContentBounds = this.GetBounds();
			}
			if (this.m_HSliderExpand && this.hScrollingNeeded)
			{
				this.viewRect.sizeDelta = new Vector2(this.viewRect.sizeDelta.x, -(this.m_HSliderHeight + this.m_HorizontalScrollbarSpacing));
				this.m_ViewBounds = new Bounds(this.viewRect.rect.center, this.viewRect.rect.size);
				this.m_ContentBounds = this.GetBounds();
			}
			if (this.m_VSliderExpand && this.vScrollingNeeded && this.viewRect.sizeDelta.x == 0f && this.viewRect.sizeDelta.y < 0f)
			{
				this.viewRect.sizeDelta = new Vector2(-(this.m_VSliderWidth + this.m_VerticalScrollbarSpacing), this.viewRect.sizeDelta.y);
			}
		}

		// Token: 0x06003C06 RID: 15366 RVA: 0x0015DDBC File Offset: 0x0015BFBC
		public virtual void SetLayoutVertical()
		{
			this.UpdateScrollbarLayout();
			this.m_ViewBounds = new Bounds(this.viewRect.rect.center, this.viewRect.rect.size);
			this.m_ContentBounds = this.GetBounds();
		}

		// Token: 0x06003C07 RID: 15367 RVA: 0x0015DE18 File Offset: 0x0015C018
		private void UpdateScrollbarVisibility()
		{
			if (this.m_VerticalScrollbar && this.m_VerticalScrollbarVisibility != ScrollRectEx.ScrollbarVisibility.Permanent && this.m_VerticalScrollbar.gameObject.activeSelf != this.vScrollingNeeded)
			{
				this.m_VerticalScrollbar.gameObject.SetActive(this.vScrollingNeeded);
			}
			if (this.m_HorizontalScrollbar && this.m_HorizontalScrollbarVisibility != ScrollRectEx.ScrollbarVisibility.Permanent && this.m_HorizontalScrollbar.gameObject.activeSelf != this.hScrollingNeeded)
			{
				this.m_HorizontalScrollbar.gameObject.SetActive(this.hScrollingNeeded);
			}
		}

		// Token: 0x06003C08 RID: 15368 RVA: 0x0015DEAC File Offset: 0x0015C0AC
		private void UpdateScrollbarLayout()
		{
			if (this.m_VSliderExpand && this.m_HorizontalScrollbar)
			{
				this.m_Tracker.Add(this, this.m_HorizontalScrollbarRect, DrivenTransformProperties.AnchoredPositionX | DrivenTransformProperties.AnchorMinX | DrivenTransformProperties.AnchorMaxX | DrivenTransformProperties.SizeDeltaX);
				this.m_HorizontalScrollbarRect.anchorMin = new Vector2(0f, this.m_HorizontalScrollbarRect.anchorMin.y);
				this.m_HorizontalScrollbarRect.anchorMax = new Vector2(1f, this.m_HorizontalScrollbarRect.anchorMax.y);
				this.m_HorizontalScrollbarRect.anchoredPosition = new Vector2(0f, this.m_HorizontalScrollbarRect.anchoredPosition.y);
				if (this.vScrollingNeeded)
				{
					this.m_HorizontalScrollbarRect.sizeDelta = new Vector2(-(this.m_VSliderWidth + this.m_VerticalScrollbarSpacing), this.m_HorizontalScrollbarRect.sizeDelta.y);
				}
				else
				{
					this.m_HorizontalScrollbarRect.sizeDelta = new Vector2(0f, this.m_HorizontalScrollbarRect.sizeDelta.y);
				}
			}
			if (this.m_HSliderExpand && this.m_VerticalScrollbar)
			{
				this.m_Tracker.Add(this, this.m_VerticalScrollbarRect, DrivenTransformProperties.AnchoredPositionY | DrivenTransformProperties.AnchorMinY | DrivenTransformProperties.AnchorMaxY | DrivenTransformProperties.SizeDeltaY);
				this.m_VerticalScrollbarRect.anchorMin = new Vector2(this.m_VerticalScrollbarRect.anchorMin.x, 0f);
				this.m_VerticalScrollbarRect.anchorMax = new Vector2(this.m_VerticalScrollbarRect.anchorMax.x, 1f);
				this.m_VerticalScrollbarRect.anchoredPosition = new Vector2(this.m_VerticalScrollbarRect.anchoredPosition.x, 0f);
				if (this.hScrollingNeeded)
				{
					this.m_VerticalScrollbarRect.sizeDelta = new Vector2(this.m_VerticalScrollbarRect.sizeDelta.x, -(this.m_HSliderHeight + this.m_HorizontalScrollbarSpacing));
					return;
				}
				this.m_VerticalScrollbarRect.sizeDelta = new Vector2(this.m_VerticalScrollbarRect.sizeDelta.x, 0f);
			}
		}

		// Token: 0x06003C09 RID: 15369 RVA: 0x0015E0B4 File Offset: 0x0015C2B4
		private void UpdateBounds()
		{
			this.m_ViewBounds = new Bounds(this.viewRect.rect.center, this.viewRect.rect.size);
			this.m_ContentBounds = this.GetBounds();
			if (this.m_Content == null)
			{
				return;
			}
			Vector3 size = this.m_ContentBounds.size;
			Vector3 center = this.m_ContentBounds.center;
			Vector3 vector = this.m_ViewBounds.size - size;
			if (vector.x > 0f)
			{
				center.x -= vector.x * (this.m_Content.pivot.x - 0.5f);
				size.x = this.m_ViewBounds.size.x;
			}
			if (vector.y > 0f)
			{
				center.y -= vector.y * (this.m_Content.pivot.y - 0.5f);
				size.y = this.m_ViewBounds.size.y;
			}
			this.m_ContentBounds.size = size;
			this.m_ContentBounds.center = center;
		}

		// Token: 0x06003C0A RID: 15370 RVA: 0x0015E1F4 File Offset: 0x0015C3F4
		private Bounds GetBounds()
		{
			if (this.m_Content == null)
			{
				return default(Bounds);
			}
			Vector3 vector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
			Matrix4x4 worldToLocalMatrix = this.viewRect.worldToLocalMatrix;
			this.m_Content.GetWorldCorners(this.m_Corners);
			for (int i = 0; i < 4; i++)
			{
				Vector3 lhs = worldToLocalMatrix.MultiplyPoint3x4(this.m_Corners[i]);
				vector = Vector3.Min(lhs, vector);
				vector2 = Vector3.Max(lhs, vector2);
			}
			Bounds result = new Bounds(vector, Vector3.zero);
			result.Encapsulate(vector2);
			return result;
		}

		// Token: 0x06003C0B RID: 15371 RVA: 0x0015E2AC File Offset: 0x0015C4AC
		private Vector2 CalculateOffset(Vector2 delta)
		{
			Vector2 zero = Vector2.zero;
			if (this.m_MovementType == ScrollRectEx.MovementType.Unrestricted)
			{
				return zero;
			}
			Vector2 vector = this.m_ContentBounds.min;
			Vector2 vector2 = this.m_ContentBounds.max;
			if (this.m_Horizontal)
			{
				vector.x += delta.x;
				vector2.x += delta.x;
				if (vector.x > this.m_ViewBounds.min.x)
				{
					zero.x = this.m_ViewBounds.min.x - vector.x;
				}
				else if (vector2.x < this.m_ViewBounds.max.x)
				{
					zero.x = this.m_ViewBounds.max.x - vector2.x;
				}
			}
			if (this.m_Vertical)
			{
				vector.y += delta.y;
				vector2.y += delta.y;
				if (vector2.y < this.m_ViewBounds.max.y)
				{
					zero.y = this.m_ViewBounds.max.y - vector2.y;
				}
				else if (vector.y > this.m_ViewBounds.min.y)
				{
					zero.y = this.m_ViewBounds.min.y - vector.y;
				}
			}
			return zero;
		}

		// Token: 0x06003C0C RID: 15372 RVA: 0x0015E422 File Offset: 0x0015C622
		protected void SetDirty()
		{
			if (!this.IsActive())
			{
				return;
			}
			LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
		}

		// Token: 0x06003C0D RID: 15373 RVA: 0x0015E438 File Offset: 0x0015C638
		protected void SetDirtyCaching()
		{
			if (!this.IsActive())
			{
				return;
			}
			CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
			LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
		}

		// Token: 0x06003C0E RID: 15374 RVA: 0x0015E454 File Offset: 0x0015C654
		public void CenterOnPosition(Vector2 pos)
		{
			RectTransform rectTransform = base.transform as RectTransform;
			Vector2 vector = new Vector2(this.content.localScale.x, this.content.localScale.y);
			pos.x *= vector.x;
			pos.y *= vector.y;
			Vector2 vector2 = new Vector2(this.content.rect.width * vector.x - rectTransform.rect.width, this.content.rect.height * vector.y - rectTransform.rect.height);
			pos.x = pos.x / vector2.x + this.content.pivot.x;
			pos.y = pos.y / vector2.y + this.content.pivot.y;
			if (this.movementType != ScrollRectEx.MovementType.Unrestricted)
			{
				pos.x = Mathf.Clamp(pos.x, 0f, 1f);
				pos.y = Mathf.Clamp(pos.y, 0f, 1f);
			}
			this.normalizedPosition = pos;
		}

		// Token: 0x06003C0F RID: 15375 RVA: 0x000059DD File Offset: 0x00003BDD
		public void LayoutComplete()
		{
		}

		// Token: 0x06003C10 RID: 15376 RVA: 0x000059DD File Offset: 0x00003BDD
		public void GraphicUpdateComplete()
		{
		}

		// Token: 0x06003C11 RID: 15377 RVA: 0x00059891 File Offset: 0x00057A91
		Transform ICanvasElement.get_transform()
		{
			return base.transform;
		}

		// Token: 0x04003556 RID: 13654
		public PointerEventData.InputButton scrollButton;

		// Token: 0x04003557 RID: 13655
		public PointerEventData.InputButton altScrollButton;

		// Token: 0x04003558 RID: 13656
		[SerializeField]
		private RectTransform m_Content;

		// Token: 0x04003559 RID: 13657
		[SerializeField]
		private bool m_Horizontal = true;

		// Token: 0x0400355A RID: 13658
		[SerializeField]
		private bool m_Vertical = true;

		// Token: 0x0400355B RID: 13659
		[SerializeField]
		private ScrollRectEx.MovementType m_MovementType = ScrollRectEx.MovementType.Elastic;

		// Token: 0x0400355C RID: 13660
		[SerializeField]
		private float m_Elasticity = 0.1f;

		// Token: 0x0400355D RID: 13661
		[SerializeField]
		private bool m_Inertia = true;

		// Token: 0x0400355E RID: 13662
		[SerializeField]
		private float m_DecelerationRate = 0.135f;

		// Token: 0x0400355F RID: 13663
		[SerializeField]
		private float m_ScrollSensitivity = 1f;

		// Token: 0x04003560 RID: 13664
		[SerializeField]
		private RectTransform m_Viewport;

		// Token: 0x04003561 RID: 13665
		[SerializeField]
		private Scrollbar m_HorizontalScrollbar;

		// Token: 0x04003562 RID: 13666
		[SerializeField]
		private Scrollbar m_VerticalScrollbar;

		// Token: 0x04003563 RID: 13667
		[SerializeField]
		private ScrollRectEx.ScrollbarVisibility m_HorizontalScrollbarVisibility;

		// Token: 0x04003564 RID: 13668
		[SerializeField]
		private ScrollRectEx.ScrollbarVisibility m_VerticalScrollbarVisibility;

		// Token: 0x04003565 RID: 13669
		[SerializeField]
		private float m_HorizontalScrollbarSpacing;

		// Token: 0x04003566 RID: 13670
		[SerializeField]
		private float m_VerticalScrollbarSpacing;

		// Token: 0x04003567 RID: 13671
		[SerializeField]
		private ScrollRectEx.ScrollRectEvent m_OnValueChanged = new ScrollRectEx.ScrollRectEvent();

		// Token: 0x04003568 RID: 13672
		private Vector2 m_PointerStartLocalCursor = Vector2.zero;

		// Token: 0x04003569 RID: 13673
		private Vector2 m_ContentStartPosition = Vector2.zero;

		// Token: 0x0400356A RID: 13674
		private RectTransform m_ViewRect;

		// Token: 0x0400356B RID: 13675
		private Bounds m_ContentBounds;

		// Token: 0x0400356C RID: 13676
		private Bounds m_ViewBounds;

		// Token: 0x0400356D RID: 13677
		private Vector2 m_Velocity;

		// Token: 0x0400356E RID: 13678
		private bool m_Dragging;

		// Token: 0x0400356F RID: 13679
		private Vector2 m_PrevPosition = Vector2.zero;

		// Token: 0x04003570 RID: 13680
		private Bounds m_PrevContentBounds;

		// Token: 0x04003571 RID: 13681
		private Bounds m_PrevViewBounds;

		// Token: 0x04003572 RID: 13682
		[NonSerialized]
		private bool m_HasRebuiltLayout;

		// Token: 0x04003573 RID: 13683
		private bool m_HSliderExpand;

		// Token: 0x04003574 RID: 13684
		private bool m_VSliderExpand;

		// Token: 0x04003575 RID: 13685
		private float m_HSliderHeight;

		// Token: 0x04003576 RID: 13686
		private float m_VSliderWidth;

		// Token: 0x04003577 RID: 13687
		[NonSerialized]
		private RectTransform m_Rect;

		// Token: 0x04003578 RID: 13688
		private RectTransform m_HorizontalScrollbarRect;

		// Token: 0x04003579 RID: 13689
		private RectTransform m_VerticalScrollbarRect;

		// Token: 0x0400357A RID: 13690
		private DrivenRectTransformTracker m_Tracker;

		// Token: 0x0400357B RID: 13691
		private readonly Vector3[] m_Corners = new Vector3[4];

		// Token: 0x02000EAA RID: 3754
		public enum MovementType
		{
			// Token: 0x04004B83 RID: 19331
			Unrestricted,
			// Token: 0x04004B84 RID: 19332
			Elastic,
			// Token: 0x04004B85 RID: 19333
			Clamped
		}

		// Token: 0x02000EAB RID: 3755
		public enum ScrollbarVisibility
		{
			// Token: 0x04004B87 RID: 19335
			Permanent,
			// Token: 0x04004B88 RID: 19336
			AutoHide,
			// Token: 0x04004B89 RID: 19337
			AutoHideAndExpandViewport
		}

		// Token: 0x02000EAC RID: 3756
		[Serializable]
		public class ScrollRectEvent : UnityEvent<Vector2>
		{
		}
	}
}
