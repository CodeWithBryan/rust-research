using System;

// Token: 0x02000931 RID: 2353
public class NetworkedProperty<T> where T : IEquatable<T>
{
	// Token: 0x1700043C RID: 1084
	// (get) Token: 0x0600380B RID: 14347 RVA: 0x0014BD87 File Offset: 0x00149F87
	// (set) Token: 0x0600380C RID: 14348 RVA: 0x0014BD8F File Offset: 0x00149F8F
	public T Value
	{
		get
		{
			return this.val;
		}
		set
		{
			if (!this.val.Equals(value))
			{
				this.val = value;
				if (this.entity.isServer)
				{
					this.entity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
				}
			}
		}
	}

	// Token: 0x0600380D RID: 14349 RVA: 0x0014BDC5 File Offset: 0x00149FC5
	public NetworkedProperty(BaseEntity entity)
	{
		this.entity = entity;
	}

	// Token: 0x0600380E RID: 14350 RVA: 0x0014BDD4 File Offset: 0x00149FD4
	public static implicit operator T(NetworkedProperty<T> value)
	{
		return value.Value;
	}

	// Token: 0x0400321C RID: 12828
	private T val;

	// Token: 0x0400321D RID: 12829
	private BaseEntity entity;
}
