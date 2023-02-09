using System;
using ConVar;
using UnityEngine;

// Token: 0x020007B8 RID: 1976
public class FPSGraph : Graph
{
	// Token: 0x060033C6 RID: 13254 RVA: 0x0013BF18 File Offset: 0x0013A118
	public void Refresh()
	{
		base.enabled = (FPS.graph > 0);
		this.Area.width = (float)(this.Resolution = Mathf.Clamp(FPS.graph, 0, Screen.width));
	}

	// Token: 0x060033C7 RID: 13255 RVA: 0x0013BF58 File Offset: 0x0013A158
	protected void OnEnable()
	{
		this.Refresh();
	}

	// Token: 0x060033C8 RID: 13256 RVA: 0x0013BF60 File Offset: 0x0013A160
	protected override float GetValue()
	{
		return 1f / UnityEngine.Time.deltaTime;
	}

	// Token: 0x060033C9 RID: 13257 RVA: 0x0013BF6D File Offset: 0x0013A16D
	protected override Color GetColor(float value)
	{
		if (value < 10f)
		{
			return Color.red;
		}
		if (value >= 30f)
		{
			return Color.green;
		}
		return Color.yellow;
	}
}
