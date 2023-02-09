using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020003A6 RID: 934
public class EntityFlag_Toggle : EntityComponent<BaseEntity>, IOnPostNetworkUpdate, IOnSendNetworkUpdate, IPrefabPreProcess
{
	// Token: 0x0600204D RID: 8269 RVA: 0x000D3153 File Offset: 0x000D1353
	protected void OnDisable()
	{
		this.hasRunOnce = false;
		this.lastHasFlag = false;
	}

	// Token: 0x0600204E RID: 8270 RVA: 0x000D3164 File Offset: 0x000D1364
	public void DoUpdate(BaseEntity entity)
	{
		bool flag = entity.HasFlag(this.flag);
		if (this.hasRunOnce && flag == this.lastHasFlag)
		{
			return;
		}
		this.hasRunOnce = true;
		this.lastHasFlag = flag;
		if (flag)
		{
			this.onFlagEnabled.Invoke();
		}
		else
		{
			this.onFlagDisabled.Invoke();
		}
		this.OnStateToggled(flag);
	}

	// Token: 0x0600204F RID: 8271 RVA: 0x000059DD File Offset: 0x00003BDD
	protected virtual void OnStateToggled(bool state)
	{
	}

	// Token: 0x06002050 RID: 8272 RVA: 0x000D31C0 File Offset: 0x000D13C0
	public void OnPostNetworkUpdate(BaseEntity entity)
	{
		if (base.baseEntity != entity)
		{
			return;
		}
		if (!this.runClientside)
		{
			return;
		}
		this.DoUpdate(entity);
	}

	// Token: 0x06002051 RID: 8273 RVA: 0x000D31E1 File Offset: 0x000D13E1
	public void OnSendNetworkUpdate(BaseEntity entity)
	{
		if (!this.runServerside)
		{
			return;
		}
		this.DoUpdate(entity);
	}

	// Token: 0x06002052 RID: 8274 RVA: 0x000D31F3 File Offset: 0x000D13F3
	public void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if ((!clientside || !this.runClientside) && (!serverside || !this.runServerside))
		{
			process.RemoveComponent(this);
		}
	}

	// Token: 0x0400192B RID: 6443
	public bool runClientside = true;

	// Token: 0x0400192C RID: 6444
	public bool runServerside = true;

	// Token: 0x0400192D RID: 6445
	public BaseEntity.Flags flag;

	// Token: 0x0400192E RID: 6446
	[SerializeField]
	private UnityEvent onFlagEnabled = new UnityEvent();

	// Token: 0x0400192F RID: 6447
	[SerializeField]
	private UnityEvent onFlagDisabled = new UnityEvent();

	// Token: 0x04001930 RID: 6448
	internal bool hasRunOnce;

	// Token: 0x04001931 RID: 6449
	internal bool lastHasFlag;
}
