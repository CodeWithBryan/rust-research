using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000800 RID: 2048
public class ItemTextValue : MonoBehaviour
{
	// Token: 0x0600344B RID: 13387 RVA: 0x0013D67C File Offset: 0x0013B87C
	public void SetValue(float val, int numDecimals = 0, string overrideText = "")
	{
		val *= this.multiplier;
		this.text.text = ((overrideText == "") ? string.Format("{0}{1:n" + numDecimals + "}", (val > 0f && this.signed) ? "+" : "", val) : overrideText);
		if (this.asPercentage)
		{
			Text text = this.text;
			text.text += " %";
		}
		if (this.suffix != "" && !float.IsPositiveInfinity(val))
		{
			Text text2 = this.text;
			text2.text += this.suffix;
		}
		bool flag = val > 0f;
		if (this.negativestat)
		{
			flag = !flag;
		}
		if (this.useColors)
		{
			this.text.color = (flag ? this.good : this.bad);
		}
	}

	// Token: 0x04002D64 RID: 11620
	public Text text;

	// Token: 0x04002D65 RID: 11621
	public Color bad;

	// Token: 0x04002D66 RID: 11622
	public Color good;

	// Token: 0x04002D67 RID: 11623
	public bool negativestat;

	// Token: 0x04002D68 RID: 11624
	public bool asPercentage;

	// Token: 0x04002D69 RID: 11625
	public bool useColors = true;

	// Token: 0x04002D6A RID: 11626
	public bool signed = true;

	// Token: 0x04002D6B RID: 11627
	public string suffix;

	// Token: 0x04002D6C RID: 11628
	public float multiplier = 1f;
}
