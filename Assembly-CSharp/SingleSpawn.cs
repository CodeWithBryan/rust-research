using System;

// Token: 0x0200054A RID: 1354
public class SingleSpawn : SpawnGroup
{
	// Token: 0x06002931 RID: 10545 RVA: 0x00007074 File Offset: 0x00005274
	public override bool WantsInitialSpawn()
	{
		return false;
	}

	// Token: 0x06002932 RID: 10546 RVA: 0x000FA4F3 File Offset: 0x000F86F3
	public void FillDelay(float delay)
	{
		base.Invoke(new Action(this.Fill), delay);
	}
}
