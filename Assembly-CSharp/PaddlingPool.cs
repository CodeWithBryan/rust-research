using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000173 RID: 371
public class PaddlingPool : LiquidContainer, ISplashable
{
	// Token: 0x060016B2 RID: 5810 RVA: 0x000ABA24 File Offset: 0x000A9C24
	public override void OnItemAddedOrRemoved(global::Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
		float normalisedFillLevel = this.GetNormalisedFillLevel();
		base.SetFlag(global::BaseEntity.Flags.Reserved4, normalisedFillLevel >= 1f, false, true);
		this.UpdatePoolFillAmount(normalisedFillLevel);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060016B3 RID: 5811 RVA: 0x000ABA68 File Offset: 0x000A9C68
	protected override void OnInventoryDirty()
	{
		base.OnInventoryDirty();
		float normalisedFillLevel = this.GetNormalisedFillLevel();
		this.UpdatePoolFillAmount(normalisedFillLevel);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060016B4 RID: 5812 RVA: 0x000ABA90 File Offset: 0x000A9C90
	public bool WantsSplash(ItemDefinition splashType, int amount)
	{
		if (base.IsDestroyed)
		{
			return false;
		}
		if (!base.HasFlag(global::BaseEntity.Flags.Reserved4) && splashType != null)
		{
			for (int i = 0; i < this.ValidItems.Length; i++)
			{
				if (this.ValidItems[i] != null && this.ValidItems[i].itemid == splashType.itemid)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060016B5 RID: 5813 RVA: 0x000ABAF8 File Offset: 0x000A9CF8
	public int DoSplash(ItemDefinition splashType, int amount)
	{
		base.inventory.AddItem(splashType, amount, 0UL, global::ItemContainer.LimitStack.Existing);
		return amount;
	}

	// Token: 0x060016B6 RID: 5814 RVA: 0x000ABB0B File Offset: 0x000A9D0B
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.WaterPool = Pool.Get<WaterPool>();
		info.msg.WaterPool.fillAmount = this.GetNormalisedFillLevel();
	}

	// Token: 0x060016B7 RID: 5815 RVA: 0x000ABB3C File Offset: 0x000A9D3C
	private float GetNormalisedFillLevel()
	{
		if (base.inventory.itemList.Count <= 0 || base.inventory.itemList[0] == null)
		{
			return 0f;
		}
		return (float)base.inventory.itemList[0].amount / (float)this.maxStackSize;
	}

	// Token: 0x060016B8 RID: 5816 RVA: 0x000ABB94 File Offset: 0x000A9D94
	private void UpdatePoolFillAmount(float normalisedAmount)
	{
		this.poolWaterVisual.gameObject.SetActive(normalisedAmount > 0f);
		this.waterVolume.waterEnabled = (normalisedAmount > 0f);
		float y = Mathf.Lerp(this.minimumWaterHeight, this.maximumWaterHeight, normalisedAmount);
		Vector3 localPosition = this.poolWaterVolume.localPosition;
		localPosition.y = y;
		this.poolWaterVolume.localPosition = localPosition;
		if (this.alignWaterUp)
		{
			this.poolWaterVolume.up = Vector3.up;
		}
		if (normalisedAmount > 0f && this.lastFillAmount < normalisedAmount && this.waterVolume.entityContents != null)
		{
			using (HashSet<global::BaseEntity>.Enumerator enumerator = this.waterVolume.entityContents.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IPoolVehicle poolVehicle;
					if ((poolVehicle = (enumerator.Current as IPoolVehicle)) != null)
					{
						poolVehicle.WakeUp();
					}
				}
			}
		}
		this.lastFillAmount = normalisedAmount;
	}

	// Token: 0x060016B9 RID: 5817 RVA: 0x00007074 File Offset: 0x00005274
	public override int ConsumptionAmount()
	{
		return 0;
	}

	// Token: 0x060016BA RID: 5818 RVA: 0x000ABC90 File Offset: 0x000A9E90
	public override void DestroyShared()
	{
		base.DestroyShared();
		if (base.isServer)
		{
			List<IPoolVehicle> list = Pool.GetList<IPoolVehicle>();
			if (this.waterVolume.entityContents != null)
			{
				using (HashSet<global::BaseEntity>.Enumerator enumerator = this.waterVolume.entityContents.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						IPoolVehicle item;
						if ((item = (enumerator.Current as IPoolVehicle)) != null)
						{
							list.Add(item);
						}
					}
				}
			}
			foreach (IPoolVehicle poolVehicle in list)
			{
				poolVehicle.OnPoolDestroyed();
			}
			Pool.FreeList<IPoolVehicle>(ref list);
		}
	}

	// Token: 0x04000FBC RID: 4028
	public const global::BaseEntity.Flags FilledUp = global::BaseEntity.Flags.Reserved4;

	// Token: 0x04000FBD RID: 4029
	public Transform poolWaterVolume;

	// Token: 0x04000FBE RID: 4030
	public GameObject poolWaterVisual;

	// Token: 0x04000FBF RID: 4031
	public float minimumWaterHeight;

	// Token: 0x04000FC0 RID: 4032
	public float maximumWaterHeight = 1f;

	// Token: 0x04000FC1 RID: 4033
	public WaterVolume waterVolume;

	// Token: 0x04000FC2 RID: 4034
	public bool alignWaterUp = true;

	// Token: 0x04000FC3 RID: 4035
	public GameObjectRef destroyedWithWaterEffect;

	// Token: 0x04000FC4 RID: 4036
	public Transform destroyedWithWaterEffectPos;

	// Token: 0x04000FC5 RID: 4037
	public Collider requireLookAt;

	// Token: 0x04000FC6 RID: 4038
	private float lastFillAmount = -1f;
}
