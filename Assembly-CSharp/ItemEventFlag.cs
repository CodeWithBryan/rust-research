using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000599 RID: 1433
public class ItemEventFlag : MonoBehaviour, IItemUpdate
{
	// Token: 0x06002AAD RID: 10925 RVA: 0x001026D0 File Offset: 0x001008D0
	public virtual void OnItemUpdate(Item item)
	{
		bool flag = item.HasFlag(this.flag);
		if (!this.firstRun && flag == this.lastState)
		{
			return;
		}
		if (flag)
		{
			this.onEnabled.Invoke();
		}
		else
		{
			this.onDisable.Invoke();
		}
		this.lastState = flag;
		this.firstRun = false;
	}

	// Token: 0x040022B3 RID: 8883
	public Item.Flag flag;

	// Token: 0x040022B4 RID: 8884
	public UnityEvent onEnabled = new UnityEvent();

	// Token: 0x040022B5 RID: 8885
	public UnityEvent onDisable = new UnityEvent();

	// Token: 0x040022B6 RID: 8886
	internal bool firstRun = true;

	// Token: 0x040022B7 RID: 8887
	internal bool lastState;
}
