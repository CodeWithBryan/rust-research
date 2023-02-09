using System;
using ConVar;

// Token: 0x020003C0 RID: 960
public class DebrisEntity : BaseCombatEntity
{
	// Token: 0x060020D0 RID: 8400 RVA: 0x000D5112 File Offset: 0x000D3312
	public override void ServerInit()
	{
		this.ResetRemovalTime();
		base.ServerInit();
	}

	// Token: 0x060020D1 RID: 8401 RVA: 0x000029D4 File Offset: 0x00000BD4
	public void RemoveCorpse()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x060020D2 RID: 8402 RVA: 0x000D5120 File Offset: 0x000D3320
	public void ResetRemovalTime(float dur)
	{
		using (TimeWarning.New("ResetRemovalTime", 0))
		{
			if (base.IsInvoking(new Action(this.RemoveCorpse)))
			{
				base.CancelInvoke(new Action(this.RemoveCorpse));
			}
			base.Invoke(new Action(this.RemoveCorpse), dur);
		}
	}

	// Token: 0x060020D3 RID: 8403 RVA: 0x000D5190 File Offset: 0x000D3390
	public float GetRemovalTime()
	{
		if (this.DebrisDespawnOverride <= 0f)
		{
			return Server.debrisdespawn;
		}
		return this.DebrisDespawnOverride;
	}

	// Token: 0x060020D4 RID: 8404 RVA: 0x000D51AB File Offset: 0x000D33AB
	public void ResetRemovalTime()
	{
		this.ResetRemovalTime(this.GetRemovalTime());
	}

	// Token: 0x060020D5 RID: 8405 RVA: 0x000D51B9 File Offset: 0x000D33B9
	public override string Categorize()
	{
		return "debris";
	}

	// Token: 0x04001972 RID: 6514
	public float DebrisDespawnOverride;
}
