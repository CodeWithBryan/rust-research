using System;
using UnityEngine;

// Token: 0x02000560 RID: 1376
public class TriggerNotify : TriggerBase, IPrefabPreProcess
{
	// Token: 0x1700033F RID: 831
	// (get) Token: 0x060029CE RID: 10702 RVA: 0x000FD896 File Offset: 0x000FBA96
	public bool HasContents
	{
		get
		{
			return this.contents != null && this.contents.Count > 0;
		}
	}

	// Token: 0x060029CF RID: 10703 RVA: 0x000FD8B0 File Offset: 0x000FBAB0
	internal override void OnObjects()
	{
		base.OnObjects();
		if (this.toNotify != null || (this.notifyTarget != null && this.notifyTarget.TryGetComponent<INotifyTrigger>(out this.toNotify)))
		{
			this.toNotify.OnObjects(this);
		}
	}

	// Token: 0x060029D0 RID: 10704 RVA: 0x000FD8ED File Offset: 0x000FBAED
	internal override void OnEmpty()
	{
		base.OnEmpty();
		if (this.toNotify != null || (this.notifyTarget != null && this.notifyTarget.TryGetComponent<INotifyTrigger>(out this.toNotify)))
		{
			this.toNotify.OnEmpty();
		}
	}

	// Token: 0x060029D1 RID: 10705 RVA: 0x000FD929 File Offset: 0x000FBB29
	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if ((!clientside || !this.runClientside) && (!serverside || !this.runServerside))
		{
			preProcess.RemoveComponent(this);
		}
	}

	// Token: 0x040021D5 RID: 8661
	public GameObject notifyTarget;

	// Token: 0x040021D6 RID: 8662
	private INotifyTrigger toNotify;

	// Token: 0x040021D7 RID: 8663
	public bool runClientside = true;

	// Token: 0x040021D8 RID: 8664
	public bool runServerside = true;
}
