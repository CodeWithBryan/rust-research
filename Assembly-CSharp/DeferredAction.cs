using System;
using UnityEngine;

// Token: 0x020008D1 RID: 2257
public class DeferredAction
{
	// Token: 0x17000418 RID: 1048
	// (get) Token: 0x06003640 RID: 13888 RVA: 0x00143C4E File Offset: 0x00141E4E
	// (set) Token: 0x06003641 RID: 13889 RVA: 0x00143C56 File Offset: 0x00141E56
	public bool Idle { get; private set; }

	// Token: 0x17000419 RID: 1049
	// (get) Token: 0x06003642 RID: 13890 RVA: 0x00143C5F File Offset: 0x00141E5F
	public int Index
	{
		get
		{
			return (int)this.priority;
		}
	}

	// Token: 0x06003643 RID: 13891 RVA: 0x00143C67 File Offset: 0x00141E67
	public DeferredAction(UnityEngine.Object sender, Action action, ActionPriority priority = ActionPriority.Medium)
	{
		this.sender = sender;
		this.action = action;
		this.priority = priority;
		this.Idle = true;
	}

	// Token: 0x06003644 RID: 13892 RVA: 0x00143C92 File Offset: 0x00141E92
	public void Action()
	{
		if (this.Idle)
		{
			throw new Exception("Double invocation of a deferred action.");
		}
		this.Idle = true;
		if (this.sender)
		{
			this.action();
		}
	}

	// Token: 0x06003645 RID: 13893 RVA: 0x00143CC6 File Offset: 0x00141EC6
	public void Invoke()
	{
		if (!this.Idle)
		{
			throw new Exception("Double invocation of a deferred action.");
		}
		LoadBalancer.Enqueue(this);
		this.Idle = false;
	}

	// Token: 0x06003646 RID: 13894 RVA: 0x00143CE8 File Offset: 0x00141EE8
	public static implicit operator bool(DeferredAction obj)
	{
		return obj != null;
	}

	// Token: 0x06003647 RID: 13895 RVA: 0x00143CEE File Offset: 0x00141EEE
	public static void Invoke(UnityEngine.Object sender, Action action, ActionPriority priority = ActionPriority.Medium)
	{
		new DeferredAction(sender, action, priority).Invoke();
	}

	// Token: 0x04003140 RID: 12608
	private UnityEngine.Object sender;

	// Token: 0x04003141 RID: 12609
	private Action action;

	// Token: 0x04003142 RID: 12610
	private ActionPriority priority = ActionPriority.Medium;
}
