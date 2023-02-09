using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000767 RID: 1895
public class AspectRatioGuide : MonoBehaviour
{
	// Token: 0x0600334B RID: 13131 RVA: 0x0013B500 File Offset: 0x00139700
	private void Populate()
	{
		this.aspect = CameraMan.GuideAspect;
		this.ratio = Mathf.Max(CameraMan.GuideRatio, 1f);
		this.aspectRatioFitter.aspectRatio = this.aspect / this.ratio;
		this.label.text = string.Format("{0}:{1}", this.aspect, this.ratio);
	}

	// Token: 0x0600334C RID: 13132 RVA: 0x0013B570 File Offset: 0x00139770
	public void Awake()
	{
		this.Populate();
	}

	// Token: 0x0600334D RID: 13133 RVA: 0x0013B570 File Offset: 0x00139770
	public void Update()
	{
		this.Populate();
	}

	// Token: 0x040029BD RID: 10685
	public AspectRatioFitter aspectRatioFitter;

	// Token: 0x040029BE RID: 10686
	public RustText label;

	// Token: 0x040029BF RID: 10687
	public float aspect;

	// Token: 0x040029C0 RID: 10688
	public float ratio;
}
