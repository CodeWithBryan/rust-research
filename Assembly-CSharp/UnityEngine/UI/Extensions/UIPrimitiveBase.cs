using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	// Token: 0x020009F7 RID: 2551
	public class UIPrimitiveBase : MaskableGraphic, ILayoutElement, ICanvasRaycastFilter
	{
		// Token: 0x170004D9 RID: 1241
		// (get) Token: 0x06003C92 RID: 15506 RVA: 0x001624BD File Offset: 0x001606BD
		// (set) Token: 0x06003C93 RID: 15507 RVA: 0x001624C5 File Offset: 0x001606C5
		public Sprite sprite
		{
			get
			{
				return this.m_Sprite;
			}
			set
			{
				if (SetPropertyUtility.SetClass<Sprite>(ref this.m_Sprite, value))
				{
					this.GeneratedUVs();
				}
				this.SetAllDirty();
			}
		}

		// Token: 0x170004DA RID: 1242
		// (get) Token: 0x06003C94 RID: 15508 RVA: 0x001624E1 File Offset: 0x001606E1
		// (set) Token: 0x06003C95 RID: 15509 RVA: 0x001624E9 File Offset: 0x001606E9
		public Sprite overrideSprite
		{
			get
			{
				return this.activeSprite;
			}
			set
			{
				if (SetPropertyUtility.SetClass<Sprite>(ref this.m_OverrideSprite, value))
				{
					this.GeneratedUVs();
				}
				this.SetAllDirty();
			}
		}

		// Token: 0x170004DB RID: 1243
		// (get) Token: 0x06003C96 RID: 15510 RVA: 0x00162505 File Offset: 0x00160705
		protected Sprite activeSprite
		{
			get
			{
				if (!(this.m_OverrideSprite != null))
				{
					return this.sprite;
				}
				return this.m_OverrideSprite;
			}
		}

		// Token: 0x170004DC RID: 1244
		// (get) Token: 0x06003C97 RID: 15511 RVA: 0x00162522 File Offset: 0x00160722
		// (set) Token: 0x06003C98 RID: 15512 RVA: 0x0016252A File Offset: 0x0016072A
		public float eventAlphaThreshold
		{
			get
			{
				return this.m_EventAlphaThreshold;
			}
			set
			{
				this.m_EventAlphaThreshold = value;
			}
		}

		// Token: 0x170004DD RID: 1245
		// (get) Token: 0x06003C99 RID: 15513 RVA: 0x00162533 File Offset: 0x00160733
		// (set) Token: 0x06003C9A RID: 15514 RVA: 0x0016253B File Offset: 0x0016073B
		public ResolutionMode ImproveResolution
		{
			get
			{
				return this.m_improveResolution;
			}
			set
			{
				this.m_improveResolution = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x170004DE RID: 1246
		// (get) Token: 0x06003C9B RID: 15515 RVA: 0x0016254A File Offset: 0x0016074A
		// (set) Token: 0x06003C9C RID: 15516 RVA: 0x00162552 File Offset: 0x00160752
		public float Resoloution
		{
			get
			{
				return this.m_Resolution;
			}
			set
			{
				this.m_Resolution = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x170004DF RID: 1247
		// (get) Token: 0x06003C9D RID: 15517 RVA: 0x00162561 File Offset: 0x00160761
		// (set) Token: 0x06003C9E RID: 15518 RVA: 0x00162569 File Offset: 0x00160769
		public bool UseNativeSize
		{
			get
			{
				return this.m_useNativeSize;
			}
			set
			{
				this.m_useNativeSize = value;
				this.SetAllDirty();
			}
		}

		// Token: 0x06003C9F RID: 15519 RVA: 0x00162578 File Offset: 0x00160778
		protected UIPrimitiveBase()
		{
			base.useLegacyMeshGeneration = false;
		}

		// Token: 0x170004E0 RID: 1248
		// (get) Token: 0x06003CA0 RID: 15520 RVA: 0x0016259D File Offset: 0x0016079D
		public static Material defaultETC1GraphicMaterial
		{
			get
			{
				if (UIPrimitiveBase.s_ETC1DefaultUI == null)
				{
					UIPrimitiveBase.s_ETC1DefaultUI = Canvas.GetETC1SupportedCanvasMaterial();
				}
				return UIPrimitiveBase.s_ETC1DefaultUI;
			}
		}

		// Token: 0x170004E1 RID: 1249
		// (get) Token: 0x06003CA1 RID: 15521 RVA: 0x001625BC File Offset: 0x001607BC
		public override Texture mainTexture
		{
			get
			{
				if (!(this.activeSprite == null))
				{
					return this.activeSprite.texture;
				}
				if (this.material != null && this.material.mainTexture != null)
				{
					return this.material.mainTexture;
				}
				return Graphic.s_WhiteTexture;
			}
		}

		// Token: 0x170004E2 RID: 1250
		// (get) Token: 0x06003CA2 RID: 15522 RVA: 0x00162618 File Offset: 0x00160818
		public bool hasBorder
		{
			get
			{
				return this.activeSprite != null && this.activeSprite.border.sqrMagnitude > 0f;
			}
		}

		// Token: 0x170004E3 RID: 1251
		// (get) Token: 0x06003CA3 RID: 15523 RVA: 0x00162650 File Offset: 0x00160850
		public float pixelsPerUnit
		{
			get
			{
				float num = 100f;
				if (this.activeSprite)
				{
					num = this.activeSprite.pixelsPerUnit;
				}
				float num2 = 100f;
				if (base.canvas)
				{
					num2 = base.canvas.referencePixelsPerUnit;
				}
				return num / num2;
			}
		}

		// Token: 0x170004E4 RID: 1252
		// (get) Token: 0x06003CA4 RID: 15524 RVA: 0x001626A0 File Offset: 0x001608A0
		// (set) Token: 0x06003CA5 RID: 15525 RVA: 0x001626EE File Offset: 0x001608EE
		public override Material material
		{
			get
			{
				if (this.m_Material != null)
				{
					return this.m_Material;
				}
				if (this.activeSprite && this.activeSprite.associatedAlphaSplitTexture != null)
				{
					return UIPrimitiveBase.defaultETC1GraphicMaterial;
				}
				return this.defaultMaterial;
			}
			set
			{
				base.material = value;
			}
		}

		// Token: 0x06003CA6 RID: 15526 RVA: 0x001626F8 File Offset: 0x001608F8
		protected UIVertex[] SetVbo(Vector2[] vertices, Vector2[] uvs)
		{
			UIVertex[] array = new UIVertex[4];
			for (int i = 0; i < vertices.Length; i++)
			{
				UIVertex simpleVert = UIVertex.simpleVert;
				simpleVert.color = this.color;
				simpleVert.position = vertices[i];
				simpleVert.uv0 = uvs[i];
				array[i] = simpleVert;
			}
			return array;
		}

		// Token: 0x06003CA7 RID: 15527 RVA: 0x0016275C File Offset: 0x0016095C
		protected Vector2[] IncreaseResolution(Vector2[] input)
		{
			return this.IncreaseResolution(new List<Vector2>(input)).ToArray();
		}

		// Token: 0x06003CA8 RID: 15528 RVA: 0x00162770 File Offset: 0x00160970
		protected List<Vector2> IncreaseResolution(List<Vector2> input)
		{
			this.outputList.Clear();
			ResolutionMode improveResolution = this.ImproveResolution;
			if (improveResolution != ResolutionMode.PerSegment)
			{
				if (improveResolution == ResolutionMode.PerLine)
				{
					float num = 0f;
					for (int i = 0; i < input.Count - 1; i++)
					{
						num += Vector2.Distance(input[i], input[i + 1]);
					}
					this.ResolutionToNativeSize(num);
					float num2 = num / this.m_Resolution;
					int num3 = 0;
					for (int j = 0; j < input.Count - 1; j++)
					{
						Vector2 vector = input[j];
						this.outputList.Add(vector);
						Vector2 vector2 = input[j + 1];
						float num4 = Vector2.Distance(vector, vector2) / num2;
						float num5 = 1f / num4;
						int num6 = 0;
						while ((float)num6 < num4)
						{
							this.outputList.Add(Vector2.Lerp(vector, vector2, (float)num6 * num5));
							num3++;
							num6++;
						}
						this.outputList.Add(vector2);
					}
				}
			}
			else
			{
				for (int k = 0; k < input.Count - 1; k++)
				{
					Vector2 vector3 = input[k];
					this.outputList.Add(vector3);
					Vector2 vector4 = input[k + 1];
					this.ResolutionToNativeSize(Vector2.Distance(vector3, vector4));
					float num2 = 1f / this.m_Resolution;
					for (float num7 = 1f; num7 < this.m_Resolution; num7 += 1f)
					{
						this.outputList.Add(Vector2.Lerp(vector3, vector4, num2 * num7));
					}
					this.outputList.Add(vector4);
				}
			}
			return this.outputList;
		}

		// Token: 0x06003CA9 RID: 15529 RVA: 0x000059DD File Offset: 0x00003BDD
		protected virtual void GeneratedUVs()
		{
		}

		// Token: 0x06003CAA RID: 15530 RVA: 0x000059DD File Offset: 0x00003BDD
		protected virtual void ResolutionToNativeSize(float distance)
		{
		}

		// Token: 0x06003CAB RID: 15531 RVA: 0x000059DD File Offset: 0x00003BDD
		public virtual void CalculateLayoutInputHorizontal()
		{
		}

		// Token: 0x06003CAC RID: 15532 RVA: 0x000059DD File Offset: 0x00003BDD
		public virtual void CalculateLayoutInputVertical()
		{
		}

		// Token: 0x170004E5 RID: 1253
		// (get) Token: 0x06003CAD RID: 15533 RVA: 0x00026FFC File Offset: 0x000251FC
		public virtual float minWidth
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x170004E6 RID: 1254
		// (get) Token: 0x06003CAE RID: 15534 RVA: 0x00162928 File Offset: 0x00160B28
		public virtual float preferredWidth
		{
			get
			{
				if (this.overrideSprite == null)
				{
					return 0f;
				}
				return this.overrideSprite.rect.size.x / this.pixelsPerUnit;
			}
		}

		// Token: 0x170004E7 RID: 1255
		// (get) Token: 0x06003CAF RID: 15535 RVA: 0x0003D349 File Offset: 0x0003B549
		public virtual float flexibleWidth
		{
			get
			{
				return -1f;
			}
		}

		// Token: 0x170004E8 RID: 1256
		// (get) Token: 0x06003CB0 RID: 15536 RVA: 0x00026FFC File Offset: 0x000251FC
		public virtual float minHeight
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x170004E9 RID: 1257
		// (get) Token: 0x06003CB1 RID: 15537 RVA: 0x00162968 File Offset: 0x00160B68
		public virtual float preferredHeight
		{
			get
			{
				if (this.overrideSprite == null)
				{
					return 0f;
				}
				return this.overrideSprite.rect.size.y / this.pixelsPerUnit;
			}
		}

		// Token: 0x170004EA RID: 1258
		// (get) Token: 0x06003CB2 RID: 15538 RVA: 0x0003D349 File Offset: 0x0003B549
		public virtual float flexibleHeight
		{
			get
			{
				return -1f;
			}
		}

		// Token: 0x170004EB RID: 1259
		// (get) Token: 0x06003CB3 RID: 15539 RVA: 0x00007074 File Offset: 0x00005274
		public virtual int layoutPriority
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06003CB4 RID: 15540 RVA: 0x001629A8 File Offset: 0x00160BA8
		public virtual bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
		{
			if (this.m_EventAlphaThreshold >= 1f)
			{
				return true;
			}
			Sprite overrideSprite = this.overrideSprite;
			if (overrideSprite == null)
			{
				return true;
			}
			Vector2 vector;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(base.rectTransform, screenPoint, eventCamera, out vector);
			Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
			vector.x += base.rectTransform.pivot.x * pixelAdjustedRect.width;
			vector.y += base.rectTransform.pivot.y * pixelAdjustedRect.height;
			vector = this.MapCoordinate(vector, pixelAdjustedRect);
			Rect textureRect = overrideSprite.textureRect;
			Vector2 vector2 = new Vector2(vector.x / textureRect.width, vector.y / textureRect.height);
			float u = Mathf.Lerp(textureRect.x, textureRect.xMax, vector2.x) / (float)overrideSprite.texture.width;
			float v = Mathf.Lerp(textureRect.y, textureRect.yMax, vector2.y) / (float)overrideSprite.texture.height;
			bool result;
			try
			{
				result = (overrideSprite.texture.GetPixelBilinear(u, v).a >= this.m_EventAlphaThreshold);
			}
			catch (UnityException ex)
			{
				Debug.LogError("Using clickAlphaThreshold lower than 1 on Image whose sprite texture cannot be read. " + ex.Message + " Also make sure to disable sprite packing for this sprite.", this);
				result = true;
			}
			return result;
		}

		// Token: 0x06003CB5 RID: 15541 RVA: 0x00162B10 File Offset: 0x00160D10
		private Vector2 MapCoordinate(Vector2 local, Rect rect)
		{
			Rect rect2 = this.sprite.rect;
			return new Vector2(local.x * rect.width, local.y * rect.height);
		}

		// Token: 0x06003CB6 RID: 15542 RVA: 0x00162B40 File Offset: 0x00160D40
		private Vector4 GetAdjustedBorders(Vector4 border, Rect rect)
		{
			for (int i = 0; i <= 1; i++)
			{
				float num = border[i] + border[i + 2];
				if (rect.size[i] < num && num != 0f)
				{
					float num2 = rect.size[i] / num;
					ref Vector4 ptr = ref border;
					int index = i;
					ptr[index] *= num2;
					ptr = ref border;
					index = i + 2;
					ptr[index] *= num2;
				}
			}
			return border;
		}

		// Token: 0x06003CB7 RID: 15543 RVA: 0x00162BD7 File Offset: 0x00160DD7
		protected override void OnEnable()
		{
			base.OnEnable();
			this.SetAllDirty();
		}

		// Token: 0x040035E4 RID: 13796
		protected static Material s_ETC1DefaultUI;

		// Token: 0x040035E5 RID: 13797
		private List<Vector2> outputList = new List<Vector2>();

		// Token: 0x040035E6 RID: 13798
		[SerializeField]
		private Sprite m_Sprite;

		// Token: 0x040035E7 RID: 13799
		[NonSerialized]
		private Sprite m_OverrideSprite;

		// Token: 0x040035E8 RID: 13800
		internal float m_EventAlphaThreshold = 1f;

		// Token: 0x040035E9 RID: 13801
		[SerializeField]
		private ResolutionMode m_improveResolution;

		// Token: 0x040035EA RID: 13802
		[SerializeField]
		protected float m_Resolution;

		// Token: 0x040035EB RID: 13803
		[SerializeField]
		private bool m_useNativeSize;
	}
}
