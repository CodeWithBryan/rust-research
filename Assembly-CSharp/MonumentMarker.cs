using System;
using Facepunch;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007CA RID: 1994
public class MonumentMarker : MonoBehaviour
{
	// Token: 0x060033FA RID: 13306 RVA: 0x0013CDB0 File Offset: 0x0013AFB0
	public void Setup(LandmarkInfo info)
	{
		this.text.text = (info.displayPhrase.IsValid() ? info.displayPhrase.translated : info.transform.root.name);
		if (info.mapIcon != null)
		{
			this.image.sprite = info.mapIcon;
			this.text.SetActive(false);
			this.imageBackground.SetActive(true);
		}
		else
		{
			this.text.SetActive(true);
			this.imageBackground.SetActive(false);
		}
		this.SetNightMode(false);
	}

	// Token: 0x060033FB RID: 13307 RVA: 0x0013CE4C File Offset: 0x0013B04C
	public void SetNightMode(bool nightMode)
	{
		Color color = nightMode ? this.nightColor : this.dayColor;
		Color color2 = nightMode ? this.dayColor : this.nightColor;
		if (this.text != null)
		{
			this.text.color = color;
		}
		if (this.image != null)
		{
			this.image.color = color;
		}
		if (this.imageBackground != null)
		{
			this.imageBackground.color = color2;
		}
	}

	// Token: 0x04002C44 RID: 11332
	public Text text;

	// Token: 0x04002C45 RID: 11333
	public Image imageBackground;

	// Token: 0x04002C46 RID: 11334
	public Image image;

	// Token: 0x04002C47 RID: 11335
	public Color dayColor;

	// Token: 0x04002C48 RID: 11336
	public Color nightColor;
}
