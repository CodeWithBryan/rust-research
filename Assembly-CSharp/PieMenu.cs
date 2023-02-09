using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000872 RID: 2162
[ExecuteInEditMode]
public class PieMenu : UIBehaviour
{
	// Token: 0x06003549 RID: 13641 RVA: 0x0014063C File Offset: 0x0013E83C
	protected override void Start()
	{
		base.Start();
		PieMenu.Instance = this;
		this.canvasGroup = base.GetComponentInChildren<CanvasGroup>();
		this.canvasGroup.alpha = 0f;
		this.canvasGroup.interactable = false;
		this.canvasGroup.blocksRaycasts = false;
		this.IsOpen = false;
		this.isClosing = true;
		base.gameObject.SetChildComponentsEnabled(false);
	}

	// Token: 0x0600354A RID: 13642 RVA: 0x001406A3 File Offset: 0x0013E8A3
	public void Clear()
	{
		this.options = new PieMenu.MenuOption[0];
	}

	// Token: 0x0600354B RID: 13643 RVA: 0x001406B4 File Offset: 0x0013E8B4
	public void AddOption(PieMenu.MenuOption option)
	{
		List<PieMenu.MenuOption> list = this.options.ToList<PieMenu.MenuOption>();
		list.Add(option);
		this.options = list.ToArray();
	}

	// Token: 0x0600354C RID: 13644 RVA: 0x001406E0 File Offset: 0x0013E8E0
	public void FinishAndOpen()
	{
		this.IsOpen = true;
		this.isClosing = false;
		this.SetDefaultOption();
		this.Rebuild();
		this.UpdateInteraction(false);
		this.PlayOpenSound();
		LeanTween.cancel(base.gameObject);
		LeanTween.cancel(this.scaleTarget);
		base.GetComponent<CanvasGroup>().alpha = 0f;
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 1f, 0.1f).setEase(LeanTweenType.easeOutCirc);
		this.scaleTarget.transform.localScale = Vector3.one * 1.5f;
		LeanTween.scale(this.scaleTarget, Vector3.one, 0.1f).setEase(LeanTweenType.easeOutBounce);
		PieMenu.Instance.gameObject.SetChildComponentsEnabled(true);
	}

	// Token: 0x0600354D RID: 13645 RVA: 0x001407A3 File Offset: 0x0013E9A3
	protected override void OnEnable()
	{
		base.OnEnable();
		this.Rebuild();
	}

	// Token: 0x0600354E RID: 13646 RVA: 0x001407B4 File Offset: 0x0013E9B4
	public void SetDefaultOption()
	{
		this.defaultOption = null;
		for (int i = 0; i < this.options.Length; i++)
		{
			if (!this.options[i].disabled)
			{
				if (this.defaultOption == null)
				{
					this.defaultOption = this.options[i];
				}
				if (this.options[i].selected)
				{
					this.defaultOption = this.options[i];
					return;
				}
			}
		}
	}

	// Token: 0x0600354F RID: 13647 RVA: 0x000059DD File Offset: 0x00003BDD
	public void PlayOpenSound()
	{
	}

	// Token: 0x06003550 RID: 13648 RVA: 0x000059DD File Offset: 0x00003BDD
	public void PlayCancelSound()
	{
	}

	// Token: 0x06003551 RID: 13649 RVA: 0x00140820 File Offset: 0x0013EA20
	public void Close(bool success = false)
	{
		if (this.isClosing)
		{
			return;
		}
		this.isClosing = true;
		NeedsCursor component = base.GetComponent<NeedsCursor>();
		if (component != null)
		{
			component.enabled = false;
		}
		LeanTween.cancel(base.gameObject);
		LeanTween.cancel(this.scaleTarget);
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 0f, 0.2f).setEase(LeanTweenType.easeOutCirc);
		LeanTween.scale(this.scaleTarget, Vector3.one * (success ? 1.5f : 0.5f), 0.2f).setEase(LeanTweenType.easeOutCirc);
		PieMenu.Instance.gameObject.SetChildComponentsEnabled(false);
		this.IsOpen = false;
	}

	// Token: 0x06003552 RID: 13650 RVA: 0x001408D0 File Offset: 0x0013EAD0
	private void Update()
	{
		if (this.pieBackground.innerSize != this.innerSize || this.pieBackground.outerSize != this.outerSize || this.pieBackground.startRadius != this.startRadius || this.pieBackground.endRadius != this.startRadius + this.radiusSize)
		{
			this.pieBackground.startRadius = this.startRadius;
			this.pieBackground.endRadius = this.startRadius + this.radiusSize;
			this.pieBackground.innerSize = this.innerSize;
			this.pieBackground.outerSize = this.outerSize;
			this.pieBackground.SetVerticesDirty();
		}
		this.UpdateInteraction(true);
		if (this.IsOpen)
		{
			CursorManager.HoldOpen(false);
			IngameMenuBackground.Enabled = true;
		}
	}

	// Token: 0x06003553 RID: 13651 RVA: 0x001409A4 File Offset: 0x0013EBA4
	public void Rebuild()
	{
		this.options = (from x in this.options
		orderby x.order
		select x).ToArray<PieMenu.MenuOption>();
		while (this.optionsCanvas.transform.childCount > 0)
		{
			if (UnityEngine.Application.isPlaying)
			{
				GameManager.DestroyImmediate(this.optionsCanvas.transform.GetChild(0).gameObject, true);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(this.optionsCanvas.transform.GetChild(0).gameObject);
			}
		}
		if (this.options.Length != 0)
		{
			for (int i = 0; i < this.options.Length; i++)
			{
				bool flag = false;
				if (this.options[i].allowMerge)
				{
					if (i > 0)
					{
						flag |= (this.options[i].order == this.options[i - 1].order);
					}
					if (i < this.options.Length - 1)
					{
						flag |= (this.options[i].order == this.options[i + 1].order);
					}
				}
				this.options[i].wantsMerge = flag;
			}
			int num = this.options.Length;
			int num2 = (from x in this.options
			where x.wantsMerge
			select x).Count<PieMenu.MenuOption>();
			int num3 = num - num2;
			int num4 = num3 + num2 / 2;
			float num5 = this.radiusSize / (float)num * 0.75f;
			float num6 = (this.radiusSize - num5 * (float)num2) / (float)num3;
			float num7 = this.startRadius - this.radiusSize / (float)num4 * 0.25f;
			for (int j = 0; j < this.options.Length; j++)
			{
				float num8 = this.options[j].wantsMerge ? 0.8f : 1f;
				float num9 = this.options[j].wantsMerge ? num5 : num6;
				GameObject gameObject = Facepunch.Instantiate.GameObject(this.pieOptionPrefab, null);
				gameObject.transform.SetParent(this.optionsCanvas.transform, false);
				this.options[j].option = gameObject.GetComponent<PieOption>();
				this.options[j].option.UpdateOption(num7, num9, this.sliceGaps, this.options[j].name, this.outerSize, this.innerSize, num8 * this.iconSize, this.options[j].sprite);
				this.options[j].option.imageIcon.material = ((this.options[j].overrideColorMode != null && this.options[j].overrideColorMode.Value.Mode == PieMenu.MenuOption.ColorMode.PieMenuSpriteColorOption.SpriteColor) ? null : this.IconMaterial);
				num7 += num9;
			}
		}
		this.selectedOption = null;
	}

	// Token: 0x06003554 RID: 13652 RVA: 0x00140C9C File Offset: 0x0013EE9C
	public void UpdateInteraction(bool allowLerp = true)
	{
		if (this.isClosing)
		{
			return;
		}
		Vector3 vector = UnityEngine.Input.mousePosition - new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f);
		float num = Mathf.Atan2(vector.x, vector.y) * 57.29578f;
		if (num < 0f)
		{
			num += 360f;
		}
		for (int i = 0; i < this.options.Length; i++)
		{
			float midRadius = this.options[i].option.midRadius;
			float sliceSize = this.options[i].option.sliceSize;
			if ((vector.magnitude < 32f && this.options[i] == this.defaultOption) || (vector.magnitude >= 32f && Mathf.Abs(Mathf.DeltaAngle(num, midRadius)) < sliceSize * 0.5f))
			{
				if (allowLerp)
				{
					this.pieSelection.startRadius = Mathf.MoveTowardsAngle(this.pieSelection.startRadius, this.options[i].option.background.startRadius, Time.deltaTime * Mathf.Abs(Mathf.DeltaAngle(this.pieSelection.startRadius, this.options[i].option.background.startRadius) * 30f + 10f));
					this.pieSelection.endRadius = Mathf.MoveTowardsAngle(this.pieSelection.endRadius, this.options[i].option.background.endRadius, Time.deltaTime * Mathf.Abs(Mathf.DeltaAngle(this.pieSelection.endRadius, this.options[i].option.background.endRadius) * 30f + 10f));
				}
				else
				{
					this.pieSelection.startRadius = this.options[i].option.background.startRadius;
					this.pieSelection.endRadius = this.options[i].option.background.endRadius;
				}
				this.middleImage.material = this.IconMaterial;
				if (this.options[i].overrideColorMode != null)
				{
					if (this.options[i].overrideColorMode.Value.Mode == PieMenu.MenuOption.ColorMode.PieMenuSpriteColorOption.CustomColor)
					{
						Color customColor = this.options[i].overrideColorMode.Value.CustomColor;
						this.pieSelection.color = customColor;
						customColor.a = PieMenu.middleImageColor.a;
						this.middleImage.color = customColor;
					}
					else if (this.options[i].overrideColorMode.Value.Mode == PieMenu.MenuOption.ColorMode.PieMenuSpriteColorOption.SpriteColor)
					{
						this.pieSelection.color = PieMenu.pieSelectionColor;
						this.middleImage.color = Color.white;
						this.middleImage.material = null;
					}
				}
				else
				{
					this.pieSelection.color = PieMenu.pieSelectionColor;
					this.middleImage.color = PieMenu.middleImageColor;
				}
				this.pieSelection.SetVerticesDirty();
				this.middleImage.sprite = this.options[i].sprite;
				this.middleTitle.text = this.options[i].name;
				this.middleDesc.text = this.options[i].desc;
				this.middleRequired.text = "";
				string text = this.options[i].requirements;
				if (text != null)
				{
					text = text.Replace("[e]", "<color=#CD412B>");
					text = text.Replace("[/e]", "</color>");
					this.middleRequired.text = text;
				}
				this.options[i].option.imageIcon.color = this.colorIconHovered;
				if (this.selectedOption != this.options[i])
				{
					if (this.selectedOption != null && !this.options[i].disabled)
					{
						this.scaleTarget.transform.localScale = Vector3.one;
						LeanTween.scale(this.scaleTarget, Vector3.one * 1.03f, 0.2f).setEase(PieMenu.easePunch);
					}
					if (this.selectedOption != null)
					{
						this.selectedOption.option.imageIcon.RebuildHackUnity2019();
					}
					this.selectedOption = this.options[i];
					if (this.selectedOption != null)
					{
						this.selectedOption.option.imageIcon.RebuildHackUnity2019();
					}
				}
			}
			else
			{
				this.options[i].option.imageIcon.material = this.IconMaterial;
				if (this.options[i].overrideColorMode != null)
				{
					if (this.options[i].overrideColorMode.Value.Mode == PieMenu.MenuOption.ColorMode.PieMenuSpriteColorOption.CustomColor)
					{
						this.options[i].option.imageIcon.color = this.options[i].overrideColorMode.Value.CustomColor;
					}
					else if (this.options[i].overrideColorMode.Value.Mode == PieMenu.MenuOption.ColorMode.PieMenuSpriteColorOption.SpriteColor)
					{
						this.options[i].option.imageIcon.color = Color.white;
						this.options[i].option.imageIcon.material = null;
					}
				}
				else
				{
					this.options[i].option.imageIcon.color = this.colorIconActive;
				}
			}
			if (this.options[i].disabled)
			{
				this.options[i].option.imageIcon.color = this.colorIconDisabled;
				this.options[i].option.background.color = this.colorBackgroundDisabled;
			}
		}
	}

	// Token: 0x06003555 RID: 13653 RVA: 0x00003A54 File Offset: 0x00001C54
	public bool DoSelect()
	{
		return true;
	}

	// Token: 0x04002FE7 RID: 12263
	public static PieMenu Instance;

	// Token: 0x04002FE8 RID: 12264
	public Image middleBox;

	// Token: 0x04002FE9 RID: 12265
	public PieShape pieBackgroundBlur;

	// Token: 0x04002FEA RID: 12266
	public PieShape pieBackground;

	// Token: 0x04002FEB RID: 12267
	public PieShape pieSelection;

	// Token: 0x04002FEC RID: 12268
	public GameObject pieOptionPrefab;

	// Token: 0x04002FED RID: 12269
	public GameObject optionsCanvas;

	// Token: 0x04002FEE RID: 12270
	public PieMenu.MenuOption[] options;

	// Token: 0x04002FEF RID: 12271
	public GameObject scaleTarget;

	// Token: 0x04002FF0 RID: 12272
	public float sliceGaps = 10f;

	// Token: 0x04002FF1 RID: 12273
	[Range(0f, 1f)]
	public float outerSize = 1f;

	// Token: 0x04002FF2 RID: 12274
	[Range(0f, 1f)]
	public float innerSize = 0.5f;

	// Token: 0x04002FF3 RID: 12275
	[Range(0f, 1f)]
	public float iconSize = 0.8f;

	// Token: 0x04002FF4 RID: 12276
	[Range(0f, 360f)]
	public float startRadius;

	// Token: 0x04002FF5 RID: 12277
	[Range(0f, 360f)]
	public float radiusSize = 360f;

	// Token: 0x04002FF6 RID: 12278
	public Image middleImage;

	// Token: 0x04002FF7 RID: 12279
	public TextMeshProUGUI middleTitle;

	// Token: 0x04002FF8 RID: 12280
	public TextMeshProUGUI middleDesc;

	// Token: 0x04002FF9 RID: 12281
	public TextMeshProUGUI middleRequired;

	// Token: 0x04002FFA RID: 12282
	public Color colorIconActive;

	// Token: 0x04002FFB RID: 12283
	public Color colorIconHovered;

	// Token: 0x04002FFC RID: 12284
	public Color colorIconDisabled;

	// Token: 0x04002FFD RID: 12285
	public Color colorBackgroundDisabled;

	// Token: 0x04002FFE RID: 12286
	public SoundDefinition clipOpen;

	// Token: 0x04002FFF RID: 12287
	public SoundDefinition clipCancel;

	// Token: 0x04003000 RID: 12288
	public SoundDefinition clipChanged;

	// Token: 0x04003001 RID: 12289
	public SoundDefinition clipSelected;

	// Token: 0x04003002 RID: 12290
	public PieMenu.MenuOption defaultOption;

	// Token: 0x04003003 RID: 12291
	private bool isClosing;

	// Token: 0x04003004 RID: 12292
	private CanvasGroup canvasGroup;

	// Token: 0x04003005 RID: 12293
	public bool IsOpen;

	// Token: 0x04003006 RID: 12294
	public Material IconMaterial;

	// Token: 0x04003007 RID: 12295
	internal PieMenu.MenuOption selectedOption;

	// Token: 0x04003008 RID: 12296
	private static Color pieSelectionColor = new Color(0.804f, 0.255f, 0.169f, 1f);

	// Token: 0x04003009 RID: 12297
	private static Color middleImageColor = new Color(0.804f, 0.255f, 0.169f, 0.784f);

	// Token: 0x0400300A RID: 12298
	private static AnimationCurve easePunch = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.112586f, 0.9976035f),
		new Keyframe(0.3120486f, 0.01720615f),
		new Keyframe(0.4316337f, 0.17030682f),
		new Keyframe(0.5524869f, 0.03141804f),
		new Keyframe(0.6549395f, 0.002909959f),
		new Keyframe(0.770987f, 0.009817753f),
		new Keyframe(0.8838775f, 0.001939224f),
		new Keyframe(1f, 0f)
	});

	// Token: 0x02000E43 RID: 3651
	[Serializable]
	public class MenuOption
	{
		// Token: 0x040049C7 RID: 18887
		public string name;

		// Token: 0x040049C8 RID: 18888
		public string desc;

		// Token: 0x040049C9 RID: 18889
		public string requirements;

		// Token: 0x040049CA RID: 18890
		public Sprite sprite;

		// Token: 0x040049CB RID: 18891
		public bool disabled;

		// Token: 0x040049CC RID: 18892
		public int order;

		// Token: 0x040049CD RID: 18893
		public PieMenu.MenuOption.ColorMode? overrideColorMode;

		// Token: 0x040049CE RID: 18894
		[NonSerialized]
		public Action<BasePlayer> action;

		// Token: 0x040049CF RID: 18895
		[NonSerialized]
		public PieOption option;

		// Token: 0x040049D0 RID: 18896
		[NonSerialized]
		public bool selected;

		// Token: 0x040049D1 RID: 18897
		[NonSerialized]
		public bool allowMerge;

		// Token: 0x040049D2 RID: 18898
		[NonSerialized]
		public bool wantsMerge;

		// Token: 0x02000F6D RID: 3949
		public struct ColorMode
		{
			// Token: 0x04004E40 RID: 20032
			public PieMenu.MenuOption.ColorMode.PieMenuSpriteColorOption Mode;

			// Token: 0x04004E41 RID: 20033
			public Color CustomColor;

			// Token: 0x02000F79 RID: 3961
			public enum PieMenuSpriteColorOption
			{
				// Token: 0x04004E71 RID: 20081
				CustomColor,
				// Token: 0x04004E72 RID: 20082
				SpriteColor
			}
		}
	}
}
