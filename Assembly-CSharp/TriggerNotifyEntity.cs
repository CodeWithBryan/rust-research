using System;
using UnityEngine;

// Token: 0x02000562 RID: 1378
public class TriggerNotifyEntity : TriggerBase, IPrefabPreProcess
{
	// Token: 0x17000340 RID: 832
	// (get) Token: 0x060029D5 RID: 10709 RVA: 0x000FD896 File Offset: 0x000FBA96
	public bool HasContents
	{
		get
		{
			return this.contents != null && this.contents.Count > 0;
		}
	}

	// Token: 0x060029D6 RID: 10710 RVA: 0x000FD966 File Offset: 0x000FBB66
	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		if (this.toNotify != null || (this.notifyTarget != null && this.notifyTarget.TryGetComponent<INotifyEntityTrigger>(out this.toNotify)))
		{
			this.toNotify.OnEntityEnter(ent);
		}
	}

	// Token: 0x060029D7 RID: 10711 RVA: 0x000FD9A4 File Offset: 0x000FBBA4
	internal override void OnEntityLeave(BaseEntity ent)
	{
		base.OnEntityLeave(ent);
		if (this.toNotify != null || (this.notifyTarget != null && this.notifyTarget.TryGetComponent<INotifyEntityTrigger>(out this.toNotify)))
		{
			this.toNotify.OnEntityLeave(ent);
		}
	}

	// Token: 0x060029D8 RID: 10712 RVA: 0x000FD9E2 File Offset: 0x000FBBE2
	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if ((!clientside || !this.runClientside) && (!serverside || !this.runServerside))
		{
			preProcess.RemoveComponent(this);
		}
	}

	// Token: 0x040021D9 RID: 8665
	public GameObject notifyTarget;

	// Token: 0x040021DA RID: 8666
	private INotifyEntityTrigger toNotify;

	// Token: 0x040021DB RID: 8667
	public bool runClientside = true;

	// Token: 0x040021DC RID: 8668
	public bool runServerside = true;
}
