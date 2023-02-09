using System;
using Rust;
using UnityEngine;

// Token: 0x0200039B RID: 923
public class AnimatedBuildingBlock : StabilityEntity
{
	// Token: 0x0600202E RID: 8238 RVA: 0x000D2CA0 File Offset: 0x000D0EA0
	public override void ServerInit()
	{
		base.ServerInit();
		if (!Rust.Application.isLoadingSave)
		{
			this.UpdateAnimationParameters(true);
		}
	}

	// Token: 0x0600202F RID: 8239 RVA: 0x000D2CB6 File Offset: 0x000D0EB6
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.UpdateAnimationParameters(true);
	}

	// Token: 0x06002030 RID: 8240 RVA: 0x000D2CC5 File Offset: 0x000D0EC5
	public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		this.UpdateAnimationParameters(false);
	}

	// Token: 0x06002031 RID: 8241 RVA: 0x000D2CD8 File Offset: 0x000D0ED8
	protected void UpdateAnimationParameters(bool init)
	{
		if (!this.model)
		{
			return;
		}
		if (!this.model.animator)
		{
			return;
		}
		if (!this.model.animator.isInitialized)
		{
			return;
		}
		bool flag = this.animatorNeedsInitializing || this.animatorIsOpen != base.IsOpen() || (init && this.isAnimating);
		bool flag2 = this.animatorNeedsInitializing || init;
		if (flag)
		{
			this.isAnimating = true;
			this.model.animator.enabled = true;
			this.model.animator.SetBool("open", this.animatorIsOpen = base.IsOpen());
			if (flag2)
			{
				this.model.animator.fireEvents = false;
				if (this.model.animator.isActiveAndEnabled)
				{
					this.model.animator.Update(0f);
					this.model.animator.Update(20f);
				}
				this.PutAnimatorToSleep();
			}
			else
			{
				this.model.animator.fireEvents = base.isClient;
				if (base.isServer)
				{
					base.SetFlag(BaseEntity.Flags.Busy, true, false, true);
				}
			}
		}
		else if (flag2)
		{
			this.PutAnimatorToSleep();
		}
		this.animatorNeedsInitializing = false;
	}

	// Token: 0x06002032 RID: 8242 RVA: 0x000D2E1E File Offset: 0x000D101E
	protected void OnAnimatorFinished()
	{
		if (!this.isAnimating)
		{
			this.PutAnimatorToSleep();
		}
		this.isAnimating = false;
	}

	// Token: 0x06002033 RID: 8243 RVA: 0x000D2E38 File Offset: 0x000D1038
	private void PutAnimatorToSleep()
	{
		if (!this.model || !this.model.animator)
		{
			Debug.LogWarning(base.transform.GetRecursiveName("") + " has missing model/animator", base.gameObject);
			return;
		}
		this.model.animator.enabled = false;
		if (base.isServer)
		{
			base.SetFlag(BaseEntity.Flags.Busy, false, false, true);
		}
		this.OnAnimatorDisabled();
	}

	// Token: 0x06002034 RID: 8244 RVA: 0x000059DD File Offset: 0x00003BDD
	protected virtual void OnAnimatorDisabled()
	{
	}

	// Token: 0x0400191C RID: 6428
	private bool animatorNeedsInitializing = true;

	// Token: 0x0400191D RID: 6429
	private bool animatorIsOpen = true;

	// Token: 0x0400191E RID: 6430
	private bool isAnimating;
}
