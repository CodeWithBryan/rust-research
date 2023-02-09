using System;

// Token: 0x020003CC RID: 972
public struct EntityRef<T> where T : BaseEntity
{
	// Token: 0x0600212B RID: 8491 RVA: 0x000D62E8 File Offset: 0x000D44E8
	public EntityRef(uint uid)
	{
		this.entityRef = new EntityRef
		{
			uid = uid
		};
	}

	// Token: 0x17000291 RID: 657
	// (get) Token: 0x0600212C RID: 8492 RVA: 0x000D630C File Offset: 0x000D450C
	public bool IsSet
	{
		get
		{
			return this.entityRef.IsSet();
		}
	}

	// Token: 0x0600212D RID: 8493 RVA: 0x000D6319 File Offset: 0x000D4519
	public bool IsValid(bool serverside)
	{
		return this.Get(serverside).IsValid();
	}

	// Token: 0x0600212E RID: 8494 RVA: 0x000D632C File Offset: 0x000D452C
	public void Set(T entity)
	{
		this.entityRef.Set(entity);
	}

	// Token: 0x0600212F RID: 8495 RVA: 0x000D6340 File Offset: 0x000D4540
	public T Get(bool serverside)
	{
		BaseEntity baseEntity = this.entityRef.Get(serverside);
		if (baseEntity == null)
		{
			return default(T);
		}
		T result;
		if ((result = (baseEntity as T)) == null)
		{
			this.Set(default(T));
			return default(T);
		}
		return result;
	}

	// Token: 0x06002130 RID: 8496 RVA: 0x000D6395 File Offset: 0x000D4595
	public bool TryGet(bool serverside, out T entity)
	{
		entity = this.Get(serverside);
		return entity != null;
	}

	// Token: 0x17000292 RID: 658
	// (get) Token: 0x06002131 RID: 8497 RVA: 0x000D63B2 File Offset: 0x000D45B2
	// (set) Token: 0x06002132 RID: 8498 RVA: 0x000D63BF File Offset: 0x000D45BF
	public uint uid
	{
		get
		{
			return this.entityRef.uid;
		}
		set
		{
			this.entityRef.uid = value;
		}
	}

	// Token: 0x0400198F RID: 6543
	private EntityRef entityRef;
}
