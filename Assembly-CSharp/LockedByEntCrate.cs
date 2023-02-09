using System;
using UnityEngine;

// Token: 0x020003FA RID: 1018
public class LockedByEntCrate : LootContainer
{
	// Token: 0x06002227 RID: 8743 RVA: 0x000DAAB8 File Offset: 0x000D8CB8
	public void SetLockingEnt(GameObject ent)
	{
		base.CancelInvoke(new Action(this.Think));
		this.SetLocked(false);
		this.lockingEnt = ent;
		if (this.lockingEnt != null)
		{
			base.InvokeRepeating(new Action(this.Think), UnityEngine.Random.Range(0f, 1f), 1f);
			this.SetLocked(true);
		}
	}

	// Token: 0x06002228 RID: 8744 RVA: 0x000DAB20 File Offset: 0x000D8D20
	public void SetLocked(bool isLocked)
	{
		base.SetFlag(BaseEntity.Flags.OnFire, isLocked, false, true);
		base.SetFlag(BaseEntity.Flags.Locked, isLocked, false, true);
	}

	// Token: 0x06002229 RID: 8745 RVA: 0x000DAB37 File Offset: 0x000D8D37
	public void Think()
	{
		if (this.lockingEnt == null && base.IsLocked())
		{
			this.SetLockingEnt(null);
		}
	}

	// Token: 0x04001AB3 RID: 6835
	public GameObject lockingEnt;
}
