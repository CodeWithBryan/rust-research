using System;
using Rust;
using UnityEngine;

// Token: 0x02000310 RID: 784
public class DoorAnimEvents : MonoBehaviour, IClientComponent
{
	// Token: 0x17000235 RID: 565
	// (get) Token: 0x06001DB7 RID: 7607 RVA: 0x000CAD90 File Offset: 0x000C8F90
	public Animator animator
	{
		get
		{
			return base.GetComponent<Animator>();
		}
	}

	// Token: 0x06001DB8 RID: 7608 RVA: 0x000CAD98 File Offset: 0x000C8F98
	private void DoorOpenStart()
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (!this.openStart.isValid)
		{
			return;
		}
		if (this.animator.IsInTransition(0))
		{
			return;
		}
		if (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f)
		{
			return;
		}
		if (this.checkAnimSpeed && this.animator.GetCurrentAnimatorStateInfo(0).speed < 0f)
		{
			return;
		}
		Effect.client.Run(this.openStart.resourcePath, (this.soundTarget == null) ? base.gameObject : this.soundTarget);
	}

	// Token: 0x06001DB9 RID: 7609 RVA: 0x000CAE38 File Offset: 0x000C9038
	private void DoorOpenEnd()
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (!this.openEnd.isValid)
		{
			return;
		}
		if (this.animator.IsInTransition(0))
		{
			return;
		}
		if (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
		{
			return;
		}
		if (this.checkAnimSpeed && this.animator.GetCurrentAnimatorStateInfo(0).speed < 0f)
		{
			return;
		}
		Effect.client.Run(this.openEnd.resourcePath, (this.soundTarget == null) ? base.gameObject : this.soundTarget);
	}

	// Token: 0x06001DBA RID: 7610 RVA: 0x000CAED8 File Offset: 0x000C90D8
	private void DoorCloseStart()
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (!this.closeStart.isValid)
		{
			return;
		}
		if (this.animator.IsInTransition(0))
		{
			return;
		}
		if (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f)
		{
			return;
		}
		if (this.checkAnimSpeed && this.animator.GetCurrentAnimatorStateInfo(0).speed > 0f)
		{
			return;
		}
		Effect.client.Run(this.closeStart.resourcePath, (this.soundTarget == null) ? base.gameObject : this.soundTarget);
	}

	// Token: 0x06001DBB RID: 7611 RVA: 0x000CAF78 File Offset: 0x000C9178
	private void DoorCloseEnd()
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (!this.closeEnd.isValid)
		{
			return;
		}
		if (this.animator.IsInTransition(0))
		{
			return;
		}
		if (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
		{
			return;
		}
		if (this.checkAnimSpeed && this.animator.GetCurrentAnimatorStateInfo(0).speed > 0f)
		{
			return;
		}
		Effect.client.Run(this.closeEnd.resourcePath, (this.soundTarget == null) ? base.gameObject : this.soundTarget);
	}

	// Token: 0x0400171E RID: 5918
	public GameObjectRef openStart;

	// Token: 0x0400171F RID: 5919
	public GameObjectRef openEnd;

	// Token: 0x04001720 RID: 5920
	public GameObjectRef closeStart;

	// Token: 0x04001721 RID: 5921
	public GameObjectRef closeEnd;

	// Token: 0x04001722 RID: 5922
	public GameObject soundTarget;

	// Token: 0x04001723 RID: 5923
	public bool checkAnimSpeed;
}
