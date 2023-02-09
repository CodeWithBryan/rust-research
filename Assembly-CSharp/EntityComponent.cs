using System;
using UnityEngine;

// Token: 0x020003A1 RID: 929
public class EntityComponent<T> : EntityComponentBase where T : BaseEntity
{
	// Token: 0x1700027A RID: 634
	// (get) Token: 0x0600203C RID: 8252 RVA: 0x000D2F4B File Offset: 0x000D114B
	protected T baseEntity
	{
		get
		{
			if (this._baseEntity == null)
			{
				this.UpdateBaseEntity();
			}
			return this._baseEntity;
		}
	}

	// Token: 0x0600203D RID: 8253 RVA: 0x000D2F6C File Offset: 0x000D116C
	protected void UpdateBaseEntity()
	{
		if (!this)
		{
			return;
		}
		if (!base.gameObject)
		{
			return;
		}
		this._baseEntity = (base.gameObject.ToBaseEntity() as T);
	}

	// Token: 0x0600203E RID: 8254 RVA: 0x000D2FA0 File Offset: 0x000D11A0
	protected override BaseEntity GetBaseEntity()
	{
		return this.baseEntity;
	}

	// Token: 0x04001920 RID: 6432
	[NonSerialized]
	private T _baseEntity;
}
