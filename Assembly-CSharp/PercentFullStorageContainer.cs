using System;
using Facepunch;
using ProtoBuf;

// Token: 0x020003B7 RID: 951
public class PercentFullStorageContainer : StorageContainer
{
	// Token: 0x0600208A RID: 8330 RVA: 0x000D41F6 File Offset: 0x000D23F6
	public bool IsFull()
	{
		return this.GetPercentFull() == 1f;
	}

	// Token: 0x0600208B RID: 8331 RVA: 0x000D4205 File Offset: 0x000D2405
	public bool IsEmpty()
	{
		return this.GetPercentFull() == 0f;
	}

	// Token: 0x0600208C RID: 8332 RVA: 0x000059DD File Offset: 0x00003BDD
	protected virtual void OnPercentFullChanged(float newPercentFull)
	{
	}

	// Token: 0x0600208D RID: 8333 RVA: 0x000D4214 File Offset: 0x000D2414
	public float GetPercentFull()
	{
		if (base.isServer)
		{
			float num = 0f;
			if (base.inventory != null)
			{
				foreach (global::Item item in base.inventory.itemList)
				{
					num += (float)item.amount / (float)item.MaxStackable();
				}
				num /= (float)base.inventory.capacity;
			}
			return num;
		}
		return 0f;
	}

	// Token: 0x0600208E RID: 8334 RVA: 0x000D42A4 File Offset: 0x000D24A4
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		SimpleFloat simpleFloat = info.msg.simpleFloat;
	}

	// Token: 0x0600208F RID: 8335 RVA: 0x000D42B9 File Offset: 0x000D24B9
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.simpleFloat = Pool.Get<SimpleFloat>();
		info.msg.simpleFloat.value = this.GetPercentFull();
	}

	// Token: 0x06002090 RID: 8336 RVA: 0x000D42E8 File Offset: 0x000D24E8
	protected override void OnInventoryDirty()
	{
		base.OnInventoryDirty();
		float percentFull = this.GetPercentFull();
		if (percentFull != this.prevPercentFull)
		{
			this.OnPercentFullChanged(percentFull);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			this.prevPercentFull = percentFull;
		}
	}

	// Token: 0x04001953 RID: 6483
	private float prevPercentFull = -1f;
}
