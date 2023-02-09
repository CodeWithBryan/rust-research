using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000853 RID: 2131
public class ToggleLayer : MonoBehaviour, IClientComponent
{
	// Token: 0x060034E7 RID: 13543 RVA: 0x0013F540 File Offset: 0x0013D740
	protected void OnEnable()
	{
		if (MainCamera.mainCamera)
		{
			this.toggleControl.isOn = ((MainCamera.mainCamera.cullingMask & this.layer.Mask) != 0);
		}
	}

	// Token: 0x060034E8 RID: 13544 RVA: 0x0013F574 File Offset: 0x0013D774
	public void OnToggleChanged()
	{
		if (MainCamera.mainCamera)
		{
			if (this.toggleControl.isOn)
			{
				MainCamera.mainCamera.cullingMask |= this.layer.Mask;
				return;
			}
			MainCamera.mainCamera.cullingMask &= ~this.layer.Mask;
		}
	}

	// Token: 0x060034E9 RID: 13545 RVA: 0x0013F5D4 File Offset: 0x0013D7D4
	protected void OnValidate()
	{
		if (this.textControl)
		{
			this.textControl.text = this.layer.Name;
		}
	}

	// Token: 0x04002F68 RID: 12136
	public Toggle toggleControl;

	// Token: 0x04002F69 RID: 12137
	public TextMeshProUGUI textControl;

	// Token: 0x04002F6A RID: 12138
	public LayerSelect layer;
}
