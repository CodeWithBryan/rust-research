using System;
using Network;

// Token: 0x020003A2 RID: 930
public class EntityComponentBase : BaseMonoBehaviour
{
	// Token: 0x06002040 RID: 8256 RVA: 0x0002A0CF File Offset: 0x000282CF
	protected virtual BaseEntity GetBaseEntity()
	{
		return null;
	}

	// Token: 0x06002041 RID: 8257 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void SaveComponent(BaseNetworkable.SaveInfo info)
	{
	}

	// Token: 0x06002042 RID: 8258 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void LoadComponent(BaseNetworkable.LoadInfo info)
	{
	}

	// Token: 0x06002043 RID: 8259 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		return false;
	}
}
