using System;

// Token: 0x020003A7 RID: 935
public class EntityFlag_ToggleNotify : EntityFlag_Toggle
{
	// Token: 0x06002054 RID: 8276 RVA: 0x000D3248 File Offset: 0x000D1448
	protected override void OnStateToggled(bool state)
	{
		base.OnStateToggled(state);
		IFlagNotify flagNotify;
		if (!this.UseEntityParent && base.baseEntity != null && (flagNotify = (base.baseEntity as IFlagNotify)) != null)
		{
			flagNotify.OnFlagToggled(state);
		}
		IFlagNotify flagNotify2;
		if (this.UseEntityParent && base.baseEntity != null && base.baseEntity.GetParentEntity() != null && (flagNotify2 = (base.baseEntity.GetParentEntity() as IFlagNotify)) != null)
		{
			flagNotify2.OnFlagToggled(state);
		}
	}

	// Token: 0x04001932 RID: 6450
	public bool UseEntityParent;
}
