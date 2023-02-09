using System;

// Token: 0x020001CA RID: 458
public class AIPoint : BaseMonoBehaviour
{
	// Token: 0x06001855 RID: 6229 RVA: 0x000B3CB8 File Offset: 0x000B1EB8
	public bool InUse()
	{
		return this.currentUser != null;
	}

	// Token: 0x06001856 RID: 6230 RVA: 0x000B3CC6 File Offset: 0x000B1EC6
	public bool IsUsedBy(BaseEntity user)
	{
		return this.InUse() && !(user == null) && user == this.currentUser;
	}

	// Token: 0x06001857 RID: 6231 RVA: 0x000B3CE9 File Offset: 0x000B1EE9
	public bool CanBeUsedBy(BaseEntity user)
	{
		return (user != null && this.currentUser == user) || !this.InUse();
	}

	// Token: 0x06001858 RID: 6232 RVA: 0x000B3D0D File Offset: 0x000B1F0D
	public void SetUsedBy(BaseEntity user, float duration = 5f)
	{
		this.currentUser = user;
		base.CancelInvoke(new Action(this.ClearUsed));
		base.Invoke(new Action(this.ClearUsed), duration);
	}

	// Token: 0x06001859 RID: 6233 RVA: 0x000B3D3B File Offset: 0x000B1F3B
	public void SetUsedBy(BaseEntity user)
	{
		this.currentUser = user;
	}

	// Token: 0x0600185A RID: 6234 RVA: 0x000B3D44 File Offset: 0x000B1F44
	public void ClearUsed()
	{
		this.currentUser = null;
	}

	// Token: 0x0600185B RID: 6235 RVA: 0x000B3D4D File Offset: 0x000B1F4D
	public void ClearIfUsedBy(BaseEntity user)
	{
		if (this.currentUser == user)
		{
			this.ClearUsed();
		}
	}

	// Token: 0x0400117E RID: 4478
	private BaseEntity currentUser;
}
