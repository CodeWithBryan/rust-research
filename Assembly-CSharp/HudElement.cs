using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007BB RID: 1979
public class HudElement : MonoBehaviour
{
	// Token: 0x060033D3 RID: 13267 RVA: 0x0013C298 File Offset: 0x0013A498
	public void SetValue(float value, float max = 1f)
	{
		using (TimeWarning.New("HudElement.SetValue", 0))
		{
			value = (float)Mathf.CeilToInt(value);
			if (value != this.lastValue || max != this.lastMax)
			{
				this.lastValue = value;
				this.lastMax = max;
				float image = value / max;
				this.SetText(value.ToString("0"));
				this.SetImage(image);
			}
		}
	}

	// Token: 0x060033D4 RID: 13268 RVA: 0x0013C318 File Offset: 0x0013A518
	private void SetText(string v)
	{
		for (int i = 0; i < this.ValueText.Length; i++)
		{
			this.ValueText[i].text = v;
		}
	}

	// Token: 0x060033D5 RID: 13269 RVA: 0x0013C348 File Offset: 0x0013A548
	private void SetImage(float f)
	{
		for (int i = 0; i < this.FilledImage.Length; i++)
		{
			this.FilledImage[i].fillAmount = f;
		}
	}

	// Token: 0x04002BE0 RID: 11232
	public Text[] ValueText;

	// Token: 0x04002BE1 RID: 11233
	public Image[] FilledImage;

	// Token: 0x04002BE2 RID: 11234
	private float lastValue;

	// Token: 0x04002BE3 RID: 11235
	private float lastMax;
}
