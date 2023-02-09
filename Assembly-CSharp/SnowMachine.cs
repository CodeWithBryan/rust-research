using System;

// Token: 0x02000185 RID: 389
public class SnowMachine : FogMachine
{
	// Token: 0x060016F5 RID: 5877 RVA: 0x00007074 File Offset: 0x00005274
	public override bool MotionModeEnabled()
	{
		return false;
	}

	// Token: 0x060016F6 RID: 5878 RVA: 0x000ACE31 File Offset: 0x000AB031
	public override void EnableFogField()
	{
		base.EnableFogField();
		this.tempTrigger.gameObject.SetActive(true);
	}

	// Token: 0x060016F7 RID: 5879 RVA: 0x000ACE4A File Offset: 0x000AB04A
	public override void FinishFogging()
	{
		base.FinishFogging();
		this.tempTrigger.gameObject.SetActive(false);
	}

	// Token: 0x04001011 RID: 4113
	public AdaptMeshToTerrain snowMesh;

	// Token: 0x04001012 RID: 4114
	public TriggerTemperature tempTrigger;
}
