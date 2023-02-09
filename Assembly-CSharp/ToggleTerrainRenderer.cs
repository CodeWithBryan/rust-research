using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000855 RID: 2133
public class ToggleTerrainRenderer : MonoBehaviour
{
	// Token: 0x060034EC RID: 13548 RVA: 0x0013F5F9 File Offset: 0x0013D7F9
	protected void OnEnable()
	{
		if (Terrain.activeTerrain)
		{
			this.toggleControl.isOn = Terrain.activeTerrain.drawHeightmap;
		}
	}

	// Token: 0x060034ED RID: 13549 RVA: 0x0013F61C File Offset: 0x0013D81C
	public void OnToggleChanged()
	{
		if (Terrain.activeTerrain)
		{
			Terrain.activeTerrain.drawHeightmap = this.toggleControl.isOn;
		}
	}

	// Token: 0x060034EE RID: 13550 RVA: 0x0013F63F File Offset: 0x0013D83F
	protected void OnValidate()
	{
		if (this.textControl)
		{
			this.textControl.text = "Terrain Renderer";
		}
	}

	// Token: 0x04002F6B RID: 12139
	public Toggle toggleControl;

	// Token: 0x04002F6C RID: 12140
	public Text textControl;
}
