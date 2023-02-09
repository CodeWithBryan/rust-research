using System;
using UnityEngine;

// Token: 0x0200015B RID: 347
public class SpiderWeb : BaseCombatEntity
{
	// Token: 0x0600165C RID: 5724 RVA: 0x000AA174 File Offset: 0x000A8374
	public bool Fresh()
	{
		return !base.HasFlag(BaseEntity.Flags.Reserved1) && !base.HasFlag(BaseEntity.Flags.Reserved2) && !base.HasFlag(BaseEntity.Flags.Reserved3) && !base.HasFlag(BaseEntity.Flags.Reserved4);
	}

	// Token: 0x0600165D RID: 5725 RVA: 0x000AA1B0 File Offset: 0x000A83B0
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.Fresh())
		{
			int num = UnityEngine.Random.Range(0, 4);
			BaseEntity.Flags f = BaseEntity.Flags.Reserved1;
			if (num == 0)
			{
				f = BaseEntity.Flags.Reserved1;
			}
			else if (num == 1)
			{
				f = BaseEntity.Flags.Reserved2;
			}
			else if (num == 2)
			{
				f = BaseEntity.Flags.Reserved3;
			}
			else if (num == 3)
			{
				f = BaseEntity.Flags.Reserved4;
			}
			base.SetFlag(f, true, false, true);
		}
	}
}
