using System;

// Token: 0x020003A5 RID: 933
public class EntityFlag_TOD : EntityComponent<BaseEntity>
{
	// Token: 0x06002048 RID: 8264 RVA: 0x000D307B File Offset: 0x000D127B
	public void Start()
	{
		base.Invoke(new Action(this.Initialize), 1f);
	}

	// Token: 0x06002049 RID: 8265 RVA: 0x000D3094 File Offset: 0x000D1294
	public void Initialize()
	{
		if (base.baseEntity == null || base.baseEntity.isClient)
		{
			return;
		}
		base.InvokeRandomized(new Action(this.DoTimeCheck), 0f, 5f, 1f);
	}

	// Token: 0x0600204A RID: 8266 RVA: 0x000D30D4 File Offset: 0x000D12D4
	public bool WantsOn()
	{
		if (TOD_Sky.Instance == null)
		{
			return false;
		}
		bool isNight = TOD_Sky.Instance.IsNight;
		return this.onAtNight == isNight;
	}

	// Token: 0x0600204B RID: 8267 RVA: 0x000D3108 File Offset: 0x000D1308
	private void DoTimeCheck()
	{
		bool flag = base.baseEntity.HasFlag(this.desiredFlag);
		bool flag2 = this.WantsOn();
		if (flag != flag2)
		{
			base.baseEntity.SetFlag(this.desiredFlag, flag2, false, true);
		}
	}

	// Token: 0x04001929 RID: 6441
	public BaseEntity.Flags desiredFlag;

	// Token: 0x0400192A RID: 6442
	public bool onAtNight = true;
}
