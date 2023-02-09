using System;
using System.Collections.Generic;
using Facepunch;

// Token: 0x020003CA RID: 970
public static class EntityLinkEx
{
	// Token: 0x06002122 RID: 8482 RVA: 0x000D6118 File Offset: 0x000D4318
	public static void FreeLinks(this List<EntityLink> links)
	{
		for (int i = 0; i < links.Count; i++)
		{
			EntityLink entityLink = links[i];
			entityLink.Clear();
			Pool.Free<EntityLink>(ref entityLink);
		}
		links.Clear();
	}

	// Token: 0x06002123 RID: 8483 RVA: 0x000D6154 File Offset: 0x000D4354
	public static void ClearLinks(this List<EntityLink> links)
	{
		for (int i = 0; i < links.Count; i++)
		{
			links[i].Clear();
		}
	}

	// Token: 0x06002124 RID: 8484 RVA: 0x000D6180 File Offset: 0x000D4380
	public static void AddLinks(this List<EntityLink> links, BaseEntity entity, Socket_Base[] sockets)
	{
		foreach (Socket_Base socket in sockets)
		{
			EntityLink entityLink = Pool.Get<EntityLink>();
			entityLink.Setup(entity, socket);
			links.Add(entityLink);
		}
	}
}
