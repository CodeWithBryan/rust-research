using System;
using System.Collections.Generic;
using Facepunch;

// Token: 0x020003C9 RID: 969
public class EntityLink : Pool.IPooled
{
	// Token: 0x1700028F RID: 655
	// (get) Token: 0x06002114 RID: 8468 RVA: 0x000D5F89 File Offset: 0x000D4189
	public string name
	{
		get
		{
			return this.socket.socketName;
		}
	}

	// Token: 0x06002115 RID: 8469 RVA: 0x000D5F96 File Offset: 0x000D4196
	public void Setup(BaseEntity owner, Socket_Base socket)
	{
		this.owner = owner;
		this.socket = socket;
		if (socket.monogamous)
		{
			this.capacity = 1;
		}
	}

	// Token: 0x06002116 RID: 8470 RVA: 0x000D5FB5 File Offset: 0x000D41B5
	public void EnterPool()
	{
		this.owner = null;
		this.socket = null;
		this.capacity = int.MaxValue;
	}

	// Token: 0x06002117 RID: 8471 RVA: 0x000059DD File Offset: 0x00003BDD
	public void LeavePool()
	{
	}

	// Token: 0x06002118 RID: 8472 RVA: 0x000D5FD0 File Offset: 0x000D41D0
	public bool Contains(EntityLink entity)
	{
		return this.connections.Contains(entity);
	}

	// Token: 0x06002119 RID: 8473 RVA: 0x000D5FDE File Offset: 0x000D41DE
	public void Add(EntityLink entity)
	{
		this.connections.Add(entity);
	}

	// Token: 0x0600211A RID: 8474 RVA: 0x000D5FEC File Offset: 0x000D41EC
	public void Remove(EntityLink entity)
	{
		this.connections.Remove(entity);
	}

	// Token: 0x0600211B RID: 8475 RVA: 0x000D5FFC File Offset: 0x000D41FC
	public void Clear()
	{
		for (int i = 0; i < this.connections.Count; i++)
		{
			this.connections[i].Remove(this);
		}
		this.connections.Clear();
	}

	// Token: 0x0600211C RID: 8476 RVA: 0x000D603C File Offset: 0x000D423C
	public bool IsEmpty()
	{
		return this.connections.Count == 0;
	}

	// Token: 0x0600211D RID: 8477 RVA: 0x000D604C File Offset: 0x000D424C
	public bool IsOccupied()
	{
		return this.connections.Count >= this.capacity;
	}

	// Token: 0x0600211E RID: 8478 RVA: 0x000D6064 File Offset: 0x000D4264
	public bool IsMale()
	{
		return this.socket.male;
	}

	// Token: 0x0600211F RID: 8479 RVA: 0x000D6071 File Offset: 0x000D4271
	public bool IsFemale()
	{
		return this.socket.female;
	}

	// Token: 0x06002120 RID: 8480 RVA: 0x000D6080 File Offset: 0x000D4280
	public bool CanConnect(EntityLink link)
	{
		return !this.IsOccupied() && link != null && !link.IsOccupied() && this.socket.CanConnect(this.owner.transform.position, this.owner.transform.rotation, link.socket, link.owner.transform.position, link.owner.transform.rotation);
	}

	// Token: 0x04001989 RID: 6537
	public BaseEntity owner;

	// Token: 0x0400198A RID: 6538
	public Socket_Base socket;

	// Token: 0x0400198B RID: 6539
	public List<EntityLink> connections = new List<EntityLink>(8);

	// Token: 0x0400198C RID: 6540
	public int capacity = int.MaxValue;
}
