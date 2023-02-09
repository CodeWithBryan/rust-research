using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000874 RID: 2164
public class PieOption : MonoBehaviour
{
	// Token: 0x17000402 RID: 1026
	// (get) Token: 0x0600355A RID: 13658 RVA: 0x00141441 File Offset: 0x0013F641
	internal float midRadius
	{
		get
		{
			return (this.background.startRadius + this.background.endRadius) * 0.5f;
		}
	}

	// Token: 0x17000403 RID: 1027
	// (get) Token: 0x0600355B RID: 13659 RVA: 0x00141460 File Offset: 0x0013F660
	internal float sliceSize
	{
		get
		{
			return this.background.endRadius - this.background.startRadius;
		}
	}

	// Token: 0x0600355C RID: 13660 RVA: 0x0014147C File Offset: 0x0013F67C
	public void UpdateOption(float startSlice, float sliceSize, float border, string optionTitle, float outerSize, float innerSize, float imageSize, Sprite sprite)
	{
		if (this.background == null)
		{
			return;
		}
		float num = this.background.rectTransform.rect.height * 0.5f;
		float num2 = num * (innerSize + (outerSize - innerSize) * 0.5f);
		float num3 = num * (outerSize - innerSize);
		this.background.startRadius = startSlice;
		this.background.endRadius = startSlice + sliceSize;
		this.background.border = border;
		this.background.outerSize = outerSize;
		this.background.innerSize = innerSize;
		this.background.color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 0f);
		float num4 = startSlice + sliceSize * 0.5f;
		float x = Mathf.Sin(num4 * 0.017453292f) * num2;
		float y = Mathf.Cos(num4 * 0.017453292f) * num2;
		this.imageIcon.rectTransform.localPosition = new Vector3(x, y);
		this.imageIcon.rectTransform.sizeDelta = new Vector2(num3 * imageSize, num3 * imageSize);
		this.imageIcon.sprite = sprite;
	}

	// Token: 0x0400300C RID: 12300
	public PieShape background;

	// Token: 0x0400300D RID: 12301
	public Image imageIcon;
}
