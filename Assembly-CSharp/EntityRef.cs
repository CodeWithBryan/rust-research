using System;
using UnityEngine;

// Token: 0x020003CB RID: 971
public struct EntityRef
{
	// Token: 0x06002125 RID: 8485 RVA: 0x000D61B4 File Offset: 0x000D43B4
	public bool IsSet()
	{
		return this.id_cached > 0U;
	}

	// Token: 0x06002126 RID: 8486 RVA: 0x000D61BF File Offset: 0x000D43BF
	public bool IsValid(bool serverside)
	{
		return this.Get(serverside).IsValid();
	}

	// Token: 0x06002127 RID: 8487 RVA: 0x000D61CD File Offset: 0x000D43CD
	public void Set(BaseEntity ent)
	{
		this.ent_cached = ent;
		this.id_cached = 0U;
		if (this.ent_cached.IsValid())
		{
			this.id_cached = this.ent_cached.net.ID;
		}
	}

	// Token: 0x06002128 RID: 8488 RVA: 0x000D6200 File Offset: 0x000D4400
	public BaseEntity Get(bool serverside)
	{
		if (this.ent_cached == null && this.id_cached > 0U)
		{
			if (serverside)
			{
				this.ent_cached = (BaseNetworkable.serverEntities.Find(this.id_cached) as BaseEntity);
			}
			else
			{
				Debug.LogWarning("EntityRef: Looking for clientside entities on pure server!");
			}
		}
		if (!this.ent_cached.IsValid())
		{
			this.ent_cached = null;
		}
		return this.ent_cached;
	}

	// Token: 0x17000290 RID: 656
	// (get) Token: 0x06002129 RID: 8489 RVA: 0x000D6268 File Offset: 0x000D4468
	// (set) Token: 0x0600212A RID: 8490 RVA: 0x000D6294 File Offset: 0x000D4494
	public uint uid
	{
		get
		{
			if (this.ent_cached.IsValid())
			{
				this.id_cached = this.ent_cached.net.ID;
			}
			return this.id_cached;
		}
		set
		{
			this.id_cached = value;
			if (this.id_cached == 0U)
			{
				this.ent_cached = null;
				return;
			}
			if (this.ent_cached.IsValid() && this.ent_cached.net.ID == this.id_cached)
			{
				return;
			}
			this.ent_cached = null;
		}
	}

	// Token: 0x0400198D RID: 6541
	internal BaseEntity ent_cached;

	// Token: 0x0400198E RID: 6542
	internal uint id_cached;
}
