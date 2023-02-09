using System;
using UnityEngine;

// Token: 0x020005C0 RID: 1472
public class ItemModEntity : ItemMod
{
	// Token: 0x06002BBB RID: 11195 RVA: 0x00106FB0 File Offset: 0x001051B0
	public override void OnChanged(Item item)
	{
		HeldEntity heldEntity = item.GetHeldEntity() as HeldEntity;
		if (heldEntity != null)
		{
			heldEntity.OnItemChanged(item);
		}
		base.OnChanged(item);
	}

	// Token: 0x06002BBC RID: 11196 RVA: 0x00106FE0 File Offset: 0x001051E0
	public override void OnItemCreated(Item item)
	{
		if (item.GetHeldEntity() == null)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.entityPrefab.resourcePath, default(Vector3), default(Quaternion), true);
			if (baseEntity == null)
			{
				Debug.LogWarning(string.Concat(new string[]
				{
					"Couldn't create item entity ",
					item.info.displayName.english,
					" (",
					this.entityPrefab.resourcePath,
					")"
				}));
				return;
			}
			baseEntity.skinID = item.skin;
			baseEntity.Spawn();
			item.SetHeldEntity(baseEntity);
		}
	}

	// Token: 0x06002BBD RID: 11197 RVA: 0x00107094 File Offset: 0x00105294
	public override void OnRemove(Item item)
	{
		BaseEntity heldEntity = item.GetHeldEntity();
		if (heldEntity == null)
		{
			return;
		}
		heldEntity.Kill(BaseNetworkable.DestroyMode.None);
		item.SetHeldEntity(null);
	}

	// Token: 0x06002BBE RID: 11198 RVA: 0x001070C0 File Offset: 0x001052C0
	private bool ParentToParent(Item item, BaseEntity ourEntity)
	{
		if (item.parentItem == null)
		{
			return false;
		}
		BaseEntity baseEntity = item.parentItem.GetWorldEntity();
		if (baseEntity == null)
		{
			baseEntity = item.parentItem.GetHeldEntity();
		}
		ourEntity.SetFlag(BaseEntity.Flags.Disabled, false, false, true);
		ourEntity.limitNetworking = false;
		ourEntity.SetParent(baseEntity, this.defaultBone, false, false);
		return true;
	}

	// Token: 0x06002BBF RID: 11199 RVA: 0x0010711C File Offset: 0x0010531C
	private bool ParentToPlayer(Item item, BaseEntity ourEntity)
	{
		HeldEntity heldEntity = ourEntity as HeldEntity;
		if (heldEntity == null)
		{
			return false;
		}
		BasePlayer ownerPlayer = item.GetOwnerPlayer();
		if (ownerPlayer)
		{
			heldEntity.SetOwnerPlayer(ownerPlayer);
			return true;
		}
		heldEntity.ClearOwnerPlayer();
		return true;
	}

	// Token: 0x06002BC0 RID: 11200 RVA: 0x0010715C File Offset: 0x0010535C
	public override void OnParentChanged(Item item)
	{
		BaseEntity heldEntity = item.GetHeldEntity();
		if (heldEntity == null)
		{
			return;
		}
		if (this.ParentToParent(item, heldEntity))
		{
			return;
		}
		if (this.ParentToPlayer(item, heldEntity))
		{
			return;
		}
		heldEntity.SetParent(null, false, false);
		heldEntity.limitNetworking = true;
		heldEntity.SetFlag(BaseEntity.Flags.Disabled, true, false, true);
	}

	// Token: 0x06002BC1 RID: 11201 RVA: 0x001071AC File Offset: 0x001053AC
	public override void CollectedForCrafting(Item item, BasePlayer crafter)
	{
		BaseEntity heldEntity = item.GetHeldEntity();
		if (heldEntity == null)
		{
			return;
		}
		HeldEntity heldEntity2 = heldEntity as HeldEntity;
		if (heldEntity2 == null)
		{
			return;
		}
		heldEntity2.CollectedForCrafting(item, crafter);
	}

	// Token: 0x06002BC2 RID: 11202 RVA: 0x001071E4 File Offset: 0x001053E4
	public override void ReturnedFromCancelledCraft(Item item, BasePlayer crafter)
	{
		BaseEntity heldEntity = item.GetHeldEntity();
		if (heldEntity == null)
		{
			return;
		}
		HeldEntity heldEntity2 = heldEntity as HeldEntity;
		if (heldEntity2 == null)
		{
			return;
		}
		heldEntity2.ReturnedFromCancelledCraft(item, crafter);
	}

	// Token: 0x04002382 RID: 9090
	public GameObjectRef entityPrefab = new GameObjectRef();

	// Token: 0x04002383 RID: 9091
	public string defaultBone;
}
