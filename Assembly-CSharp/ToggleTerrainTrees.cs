using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000856 RID: 2134
public class ToggleTerrainTrees : MonoBehaviour
{
	// Token: 0x060034F0 RID: 13552 RVA: 0x0013F65E File Offset: 0x0013D85E
	protected void OnEnable()
	{
		if (Terrain.activeTerrain)
		{
			this.toggleControl.isOn = Terrain.activeTerrain.drawTreesAndFoliage;
		}
	}

	// Token: 0x060034F1 RID: 13553 RVA: 0x0013F681 File Offset: 0x0013D881
	public void OnToggleChanged()
	{
		if (Terrain.activeTerrain)
		{
			Terrain.activeTerrain.drawTreesAndFoliage = this.toggleControl.isOn;
		}
	}

	// Token: 0x060034F2 RID: 13554 RVA: 0x0013F6A4 File Offset: 0x0013D8A4
	protected void OnValidate()
	{
		if (this.textControl)
		{
			this.textControl.text = "Terrain Trees";
		}
	}

	// Token: 0x04002F6D RID: 12141
	public Toggle toggleControl;

	// Token: 0x04002F6E RID: 12142
	public Text textControl;
}
