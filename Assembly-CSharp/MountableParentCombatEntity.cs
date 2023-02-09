using System;

// Token: 0x02000489 RID: 1161
public class MountableParentCombatEntity : BaseCombatEntity
{
	// Token: 0x170002F4 RID: 756
	// (get) Token: 0x060025BE RID: 9662 RVA: 0x000EC031 File Offset: 0x000EA231
	private BaseMountable Mountable
	{
		get
		{
			if (this.mountable == null)
			{
				this.mountable = base.GetComponentInParent<BaseMountable>();
			}
			return this.mountable;
		}
	}

	// Token: 0x04001E81 RID: 7809
	private BaseMountable mountable;
}
