using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x020005AA RID: 1450
public abstract class ItemModAssociatedEntity<T> : ItemMod where T : global::BaseEntity
{
	// Token: 0x17000361 RID: 865
	// (get) Token: 0x06002B6E RID: 11118 RVA: 0x00007074 File Offset: 0x00005274
	protected virtual bool AllowNullParenting
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000362 RID: 866
	// (get) Token: 0x06002B6F RID: 11119 RVA: 0x00007074 File Offset: 0x00005274
	protected virtual bool AllowHeldEntityParenting
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000363 RID: 867
	// (get) Token: 0x06002B70 RID: 11120 RVA: 0x00003A54 File Offset: 0x00001C54
	protected virtual bool ShouldAutoCreateEntity
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000364 RID: 868
	// (get) Token: 0x06002B71 RID: 11121 RVA: 0x00007074 File Offset: 0x00005274
	protected virtual bool OwnedByParentPlayer
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06002B72 RID: 11122 RVA: 0x00105DFE File Offset: 0x00103FFE
	public override void OnItemCreated(global::Item item)
	{
		base.OnItemCreated(item);
		if (this.ShouldAutoCreateEntity)
		{
			this.CreateAssociatedEntity(item);
		}
	}

	// Token: 0x06002B73 RID: 11123 RVA: 0x00105E18 File Offset: 0x00104018
	public T CreateAssociatedEntity(global::Item item)
	{
		if (item.instanceData != null)
		{
			return default(T);
		}
		global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.entityPrefab.resourcePath, Vector3.zero, default(Quaternion), true);
		T component = baseEntity.GetComponent<T>();
		this.OnAssociatedItemCreated(component);
		baseEntity.Spawn();
		item.instanceData = new ProtoBuf.Item.InstanceData();
		item.instanceData.ShouldPool = false;
		item.instanceData.subEntity = baseEntity.net.ID;
		item.MarkDirty();
		return component;
	}

	// Token: 0x06002B74 RID: 11124 RVA: 0x000059DD File Offset: 0x00003BDD
	protected virtual void OnAssociatedItemCreated(T ent)
	{
	}

	// Token: 0x06002B75 RID: 11125 RVA: 0x00105EA4 File Offset: 0x001040A4
	public override void OnRemove(global::Item item)
	{
		base.OnRemove(item);
		T associatedEntity = ItemModAssociatedEntity<T>.GetAssociatedEntity(item, true);
		if (associatedEntity)
		{
			associatedEntity.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x06002B76 RID: 11126 RVA: 0x00105ED9 File Offset: 0x001040D9
	public override void OnMovedToWorld(global::Item item)
	{
		this.UpdateParent(item);
		base.OnMovedToWorld(item);
	}

	// Token: 0x06002B77 RID: 11127 RVA: 0x00105EE9 File Offset: 0x001040E9
	public override void OnRemovedFromWorld(global::Item item)
	{
		this.UpdateParent(item);
		base.OnRemovedFromWorld(item);
	}

	// Token: 0x06002B78 RID: 11128 RVA: 0x00105EFC File Offset: 0x001040FC
	public void UpdateParent(global::Item item)
	{
		T associatedEntity = ItemModAssociatedEntity<T>.GetAssociatedEntity(item, true);
		if (associatedEntity == null)
		{
			return;
		}
		global::BaseEntity entityForParenting = this.GetEntityForParenting(item);
		if (entityForParenting == null)
		{
			if (this.AllowNullParenting)
			{
				associatedEntity.SetParent(null, false, true);
			}
			if (this.OwnedByParentPlayer)
			{
				associatedEntity.OwnerID = 0UL;
			}
			return;
		}
		if (!entityForParenting.isServer)
		{
			return;
		}
		if (!entityForParenting.IsFullySpawned())
		{
			return;
		}
		associatedEntity.SetParent(entityForParenting, false, true);
		global::BasePlayer basePlayer;
		if (this.OwnedByParentPlayer && (basePlayer = (entityForParenting as global::BasePlayer)) != null)
		{
			associatedEntity.OwnerID = basePlayer.userID;
		}
	}

	// Token: 0x06002B79 RID: 11129 RVA: 0x00105FA0 File Offset: 0x001041A0
	public override void OnParentChanged(global::Item item)
	{
		base.OnParentChanged(item);
		this.UpdateParent(item);
	}

	// Token: 0x06002B7A RID: 11130 RVA: 0x00105FB0 File Offset: 0x001041B0
	public global::BaseEntity GetEntityForParenting(global::Item item = null)
	{
		if (item == null)
		{
			return null;
		}
		global::BasePlayer ownerPlayer = item.GetOwnerPlayer();
		if (ownerPlayer)
		{
			return ownerPlayer;
		}
		global::BaseEntity baseEntity = (item.parent == null) ? null : item.parent.entityOwner;
		if (baseEntity != null)
		{
			return baseEntity;
		}
		global::BaseEntity worldEntity = item.GetWorldEntity();
		if (worldEntity)
		{
			return worldEntity;
		}
		if (this.AllowHeldEntityParenting && item.parentItem != null && item.parentItem.GetHeldEntity() != null)
		{
			return item.parentItem.GetHeldEntity();
		}
		return null;
	}

	// Token: 0x06002B7B RID: 11131 RVA: 0x00106038 File Offset: 0x00104238
	public static T GetAssociatedEntity(global::Item item, bool isServer = true)
	{
		if (((item != null) ? item.instanceData : null) == null)
		{
			return default(T);
		}
		global::BaseNetworkable baseNetworkable = null;
		if (isServer)
		{
			baseNetworkable = global::BaseNetworkable.serverEntities.Find(item.instanceData.subEntity);
		}
		if (baseNetworkable)
		{
			return baseNetworkable.GetComponent<T>();
		}
		return default(T);
	}

	// Token: 0x04002348 RID: 9032
	public GameObjectRef entityPrefab;
}
