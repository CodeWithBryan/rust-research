using System;
using UnityEngine;

// Token: 0x020004C4 RID: 1220
public class ColliderInfo : MonoBehaviour
{
	// Token: 0x0600275F RID: 10079 RVA: 0x000F2842 File Offset: 0x000F0A42
	public bool HasFlag(ColliderInfo.Flags f)
	{
		return (this.flags & f) == f;
	}

	// Token: 0x06002760 RID: 10080 RVA: 0x000F284F File Offset: 0x000F0A4F
	public void SetFlag(ColliderInfo.Flags f, bool b)
	{
		if (b)
		{
			this.flags |= f;
			return;
		}
		this.flags &= ~f;
	}

	// Token: 0x06002761 RID: 10081 RVA: 0x000F2874 File Offset: 0x000F0A74
	public bool Filter(HitTest info)
	{
		switch (info.type)
		{
		case HitTest.Type.ProjectileEffect:
		case HitTest.Type.Projectile:
			if ((this.flags & ColliderInfo.Flags.Shootable) == (ColliderInfo.Flags)0)
			{
				return false;
			}
			break;
		case HitTest.Type.MeleeAttack:
			if ((this.flags & ColliderInfo.Flags.Melee) == (ColliderInfo.Flags)0)
			{
				return false;
			}
			break;
		case HitTest.Type.Use:
			if ((this.flags & ColliderInfo.Flags.Usable) == (ColliderInfo.Flags)0)
			{
				return false;
			}
			break;
		}
		return true;
	}

	// Token: 0x04001FA6 RID: 8102
	public const ColliderInfo.Flags FlagsNone = (ColliderInfo.Flags)0;

	// Token: 0x04001FA7 RID: 8103
	public const ColliderInfo.Flags FlagsEverything = (ColliderInfo.Flags)(-1);

	// Token: 0x04001FA8 RID: 8104
	public const ColliderInfo.Flags FlagsDefault = ColliderInfo.Flags.Usable | ColliderInfo.Flags.Shootable | ColliderInfo.Flags.Melee | ColliderInfo.Flags.Opaque;

	// Token: 0x04001FA9 RID: 8105
	[InspectorFlags]
	public ColliderInfo.Flags flags = ColliderInfo.Flags.Usable | ColliderInfo.Flags.Shootable | ColliderInfo.Flags.Melee | ColliderInfo.Flags.Opaque;

	// Token: 0x02000CD7 RID: 3287
	[Flags]
	public enum Flags
	{
		// Token: 0x040043F8 RID: 17400
		Usable = 1,
		// Token: 0x040043F9 RID: 17401
		Shootable = 2,
		// Token: 0x040043FA RID: 17402
		Melee = 4,
		// Token: 0x040043FB RID: 17403
		Opaque = 8,
		// Token: 0x040043FC RID: 17404
		Airflow = 16,
		// Token: 0x040043FD RID: 17405
		OnlyBlockBuildingBlock = 32
	}
}
