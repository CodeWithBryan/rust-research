using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000815 RID: 2069
public class OvenLineRow : MonoBehaviour
{
	// Token: 0x06003469 RID: 13417 RVA: 0x0013DAE0 File Offset: 0x0013BCE0
	private void Update()
	{
		LootGrid above = this.Above;
		int num = (above != null) ? above.transform.childCount : 0;
		LootGrid below = this.Below;
		int num2 = (below != null) ? below.transform.childCount : 0;
		if (num2 == this._bottomCount && num == this._topCount)
		{
			return;
		}
		this._bottomCount = num2;
		this._topCount = num;
		foreach (GameObject obj in this.images)
		{
			UnityEngine.Object.Destroy(obj);
		}
		this.CreateRow(true);
		this.CreateRow(false);
	}

	// Token: 0x0600346A RID: 13418 RVA: 0x0013DB90 File Offset: 0x0013BD90
	private void CreateRow(bool above)
	{
		LootGrid lootGrid = above ? this.Above : this.Below;
		int num = above ? this._topCount : this._bottomCount;
		if (num == 0)
		{
			return;
		}
		int num2 = num;
		GridLayoutGroup component = lootGrid.GetComponent<GridLayoutGroup>();
		float x = component.cellSize.x;
		float x2 = component.spacing.x;
		float num3 = x + x2;
		float num4 = num3 * (float)(num - 1) / 2f;
		if (above)
		{
			for (int i = 0; i < num; i++)
			{
				if (i == 0 || i == num - 1)
				{
					Image image = this.CreateImage();
					image.rectTransform.anchorMin = new Vector2(0.5f, above ? 0.5f : 0f);
					image.rectTransform.anchorMax = new Vector2(0.5f, above ? 1f : 0.5f);
					image.rectTransform.offsetMin = new Vector2(-num4 + (float)i * num3 - (float)(this.LineWidth / 2), (float)(above ? (this.LineWidth / 2) : this.Padding));
					image.rectTransform.offsetMax = new Vector2(-num4 + (float)i * num3 + (float)(this.LineWidth / 2), (float)(above ? (-(float)this.Padding) : (-(float)this.LineWidth / 2)));
				}
			}
		}
		else
		{
			Image image2 = this.CreateImage();
			image2.rectTransform.anchorMin = new Vector2(0.5f, 0f);
			image2.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
			image2.rectTransform.offsetMin = new Vector2((float)(-(float)this.LineWidth / 2), (float)this.Padding);
			image2.rectTransform.offsetMax = new Vector2((float)(this.LineWidth / 2), (float)(-(float)this.LineWidth / 2));
			Image image3 = this.CreateImage();
			image3.sprite = this.TriangleSprite;
			image3.gameObject.name = "triangle";
			image3.useSpriteMesh = true;
			image3.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 180f);
			image3.rectTransform.anchorMin = new Vector2(0.5f, 0f);
			image3.rectTransform.anchorMax = new Vector2(0.5f, 0f);
			image3.rectTransform.pivot = new Vector2(0.5f, 0f);
			image3.rectTransform.offsetMin = new Vector2((float)(-(float)this.ArrowWidth / 2), 0f);
			image3.rectTransform.offsetMax = new Vector2((float)(this.ArrowWidth / 2), (float)this.ArrowHeight);
		}
		if (above && num2 >= 1)
		{
			float num5 = num3 * (float)(num2 - 1) + (float)this.LineWidth;
			Image image4 = this.CreateImage();
			image4.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
			image4.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
			image4.rectTransform.offsetMin = new Vector2(num5 / -2f, (float)(-(float)this.LineWidth / 2));
			image4.rectTransform.offsetMax = new Vector2(num5 / 2f, (float)(this.LineWidth / 2));
		}
	}

	// Token: 0x0600346B RID: 13419 RVA: 0x0013DED4 File Offset: 0x0013C0D4
	private Image CreateImage()
	{
		GameObject gameObject = new GameObject("Line");
		Image image = gameObject.AddComponent<Image>();
		this.images.Add(gameObject);
		image.rectTransform.SetParent(this.Container ?? base.transform);
		image.transform.localScale = Vector3.one;
		image.color = this.Color;
		return image;
	}

	// Token: 0x04002DC2 RID: 11714
	public LootGrid Above;

	// Token: 0x04002DC3 RID: 11715
	public LootGrid Below;

	// Token: 0x04002DC4 RID: 11716
	public Transform Container;

	// Token: 0x04002DC5 RID: 11717
	public Color Color = Color.white;

	// Token: 0x04002DC6 RID: 11718
	public Sprite TriangleSprite;

	// Token: 0x04002DC7 RID: 11719
	public int LineWidth = 2;

	// Token: 0x04002DC8 RID: 11720
	public int ArrowWidth = 6;

	// Token: 0x04002DC9 RID: 11721
	public int ArrowHeight = 4;

	// Token: 0x04002DCA RID: 11722
	public int Padding = 2;

	// Token: 0x04002DCB RID: 11723
	private int _topCount;

	// Token: 0x04002DCC RID: 11724
	private int _bottomCount;

	// Token: 0x04002DCD RID: 11725
	private List<GameObject> images = new List<GameObject>();
}
