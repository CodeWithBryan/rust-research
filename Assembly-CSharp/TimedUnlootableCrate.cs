using System;

// Token: 0x020003FD RID: 1021
public class TimedUnlootableCrate : LootContainer
{
	// Token: 0x06002273 RID: 8819 RVA: 0x000DCD5D File Offset: 0x000DAF5D
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.unlootableOnSpawn)
		{
			this.SetUnlootableFor(this.unlootableDuration);
		}
	}

	// Token: 0x06002274 RID: 8820 RVA: 0x000DCD79 File Offset: 0x000DAF79
	public void SetUnlootableFor(float duration)
	{
		base.SetFlag(BaseEntity.Flags.OnFire, true, false, true);
		base.SetFlag(BaseEntity.Flags.Locked, true, false, true);
		this.unlootableDuration = duration;
		base.Invoke(new Action(this.MakeLootable), duration);
	}

	// Token: 0x06002275 RID: 8821 RVA: 0x000DCDAA File Offset: 0x000DAFAA
	public void MakeLootable()
	{
		base.SetFlag(BaseEntity.Flags.OnFire, false, false, true);
		base.SetFlag(BaseEntity.Flags.Locked, false, false, true);
	}

	// Token: 0x04001AF5 RID: 6901
	public bool unlootableOnSpawn = true;

	// Token: 0x04001AF6 RID: 6902
	public float unlootableDuration = 300f;
}
