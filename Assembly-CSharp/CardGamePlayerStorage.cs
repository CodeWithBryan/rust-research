using System;
using Facepunch;
using ProtoBuf;

// Token: 0x020003E8 RID: 1000
public class CardGamePlayerStorage : StorageContainer
{
	// Token: 0x060021D2 RID: 8658 RVA: 0x000D8CF8 File Offset: 0x000D6EF8
	public BaseCardGameEntity GetCardGameEntity()
	{
		global::BaseEntity baseEntity = this.cardTableRef.Get(base.isServer);
		if (baseEntity != null && baseEntity.IsValid())
		{
			return baseEntity as BaseCardGameEntity;
		}
		return null;
	}

	// Token: 0x060021D3 RID: 8659 RVA: 0x000D8D30 File Offset: 0x000D6F30
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.simpleUID != null)
		{
			this.cardTableRef.uid = info.msg.simpleUID.uid;
		}
	}

	// Token: 0x060021D4 RID: 8660 RVA: 0x000D8D64 File Offset: 0x000D6F64
	protected override void OnInventoryDirty()
	{
		base.OnInventoryDirty();
		BaseCardGameEntity cardGameEntity = this.GetCardGameEntity();
		if (cardGameEntity != null)
		{
			cardGameEntity.PlayerStorageChanged();
		}
	}

	// Token: 0x060021D5 RID: 8661 RVA: 0x000D8D8D File Offset: 0x000D6F8D
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.simpleUID = Pool.Get<SimpleUID>();
		info.msg.simpleUID.uid = this.cardTableRef.uid;
	}

	// Token: 0x060021D6 RID: 8662 RVA: 0x000D8DC1 File Offset: 0x000D6FC1
	public void SetCardTable(BaseCardGameEntity cardGameEntity)
	{
		this.cardTableRef.Set(cardGameEntity);
	}

	// Token: 0x04001A2A RID: 6698
	private EntityRef cardTableRef;
}
