using System;
using Rust.UI;
using UnityEngine.EventSystems;

// Token: 0x02000782 RID: 1922
public class DemoShotButton : RustButton, IPointerClickHandler, IEventSystemHandler
{
	// Token: 0x0600337D RID: 13181 RVA: 0x0013B852 File Offset: 0x00139A52
	public override void OnPointerDown(PointerEventData eventData)
	{
		if (this.FireEventOnClicked)
		{
			return;
		}
		base.OnPointerDown(eventData);
	}

	// Token: 0x0600337E RID: 13182 RVA: 0x0013B864 File Offset: 0x00139A64
	public override void OnPointerUp(PointerEventData eventData)
	{
		if (this.FireEventOnClicked)
		{
			return;
		}
		base.OnPointerUp(eventData);
	}

	// Token: 0x0600337F RID: 13183 RVA: 0x0013B876 File Offset: 0x00139A76
	public void OnPointerClick(PointerEventData eventData)
	{
		if (this.FireEventOnClicked)
		{
			base.Press();
		}
	}

	// Token: 0x04002A0E RID: 10766
	public bool FireEventOnClicked;
}
