using System;
using UnityEngine;

// Token: 0x020003A0 RID: 928
public class EntityCollisionMessage : EntityComponent<BaseEntity>
{
	// Token: 0x0600203A RID: 8250 RVA: 0x000D2ED8 File Offset: 0x000D10D8
	private void OnCollisionEnter(Collision collision)
	{
		if (base.baseEntity == null)
		{
			return;
		}
		if (base.baseEntity.IsDestroyed)
		{
			return;
		}
		BaseEntity baseEntity = collision.GetEntity();
		if (baseEntity == base.baseEntity)
		{
			return;
		}
		if (baseEntity != null)
		{
			if (baseEntity.IsDestroyed)
			{
				return;
			}
			if (base.baseEntity.isServer)
			{
				baseEntity = baseEntity.ToServer<BaseEntity>();
			}
		}
		base.baseEntity.OnCollision(collision, baseEntity);
	}
}
