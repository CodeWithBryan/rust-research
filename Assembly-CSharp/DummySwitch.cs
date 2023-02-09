using System;

// Token: 0x020004A1 RID: 1185
public class DummySwitch : IOEntity
{
	// Token: 0x06002669 RID: 9833 RVA: 0x00006C79 File Offset: 0x00004E79
	public override bool WantsPower()
	{
		return base.IsOn();
	}

	// Token: 0x0600266A RID: 9834 RVA: 0x0005E44D File Offset: 0x0005C64D
	public override void ResetIOState()
	{
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x0600266B RID: 9835 RVA: 0x0005E459 File Offset: 0x0005C659
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.IsOn())
		{
			return 0;
		}
		return this.GetCurrentEnergy();
	}

	// Token: 0x0600266C RID: 9836 RVA: 0x000EF1CD File Offset: 0x000ED3CD
	public void SetOn(bool wantsOn)
	{
		base.SetFlag(BaseEntity.Flags.On, wantsOn, false, true);
		this.MarkDirty();
		if (base.IsOn() && this.duration != -1f)
		{
			base.Invoke(new Action(this.SetOff), this.duration);
		}
	}

	// Token: 0x0600266D RID: 9837 RVA: 0x000EF20C File Offset: 0x000ED40C
	public void SetOff()
	{
		this.SetOn(false);
	}

	// Token: 0x0600266E RID: 9838 RVA: 0x000EF218 File Offset: 0x000ED418
	public override void OnEntityMessage(BaseEntity from, string msg)
	{
		if (msg == this.listenString)
		{
			if (base.IsOn())
			{
				this.SetOn(false);
			}
			this.SetOn(true);
			return;
		}
		if (msg == this.listenStringOff && this.listenStringOff != "" && base.IsOn())
		{
			this.SetOn(false);
		}
	}

	// Token: 0x04001F1D RID: 7965
	public string listenString = "";

	// Token: 0x04001F1E RID: 7966
	public string listenStringOff = "";

	// Token: 0x04001F1F RID: 7967
	public float duration = -1f;
}
