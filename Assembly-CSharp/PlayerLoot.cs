using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000AA RID: 170
public class PlayerLoot : EntityComponent<global::BasePlayer>
{
	// Token: 0x06000F8D RID: 3981 RVA: 0x00080CB0 File Offset: 0x0007EEB0
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PlayerLoot.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000F8E RID: 3982 RVA: 0x00080CF0 File Offset: 0x0007EEF0
	public bool IsLooting()
	{
		return this.containers.Count > 0;
	}

	// Token: 0x06000F8F RID: 3983 RVA: 0x00080D00 File Offset: 0x0007EF00
	public void Clear()
	{
		if (!this.IsLooting())
		{
			return;
		}
		this.MarkDirty();
		if (this.entitySource)
		{
			this.entitySource.SendMessage("PlayerStoppedLooting", base.baseEntity, SendMessageOptions.DontRequireReceiver);
		}
		foreach (global::ItemContainer itemContainer in this.containers)
		{
			if (itemContainer != null)
			{
				itemContainer.onDirty -= this.MarkDirty;
			}
		}
		this.containers.Clear();
		this.entitySource = null;
		this.itemSource = null;
	}

	// Token: 0x06000F90 RID: 3984 RVA: 0x00080DB0 File Offset: 0x0007EFB0
	public global::ItemContainer FindContainer(uint id)
	{
		this.Check();
		if (!this.IsLooting())
		{
			return null;
		}
		foreach (global::ItemContainer itemContainer in this.containers)
		{
			global::ItemContainer itemContainer2 = itemContainer.FindContainer(id);
			if (itemContainer2 != null)
			{
				return itemContainer2;
			}
		}
		return null;
	}

	// Token: 0x06000F91 RID: 3985 RVA: 0x00080E1C File Offset: 0x0007F01C
	public global::Item FindItem(uint id)
	{
		this.Check();
		if (!this.IsLooting())
		{
			return null;
		}
		foreach (global::ItemContainer itemContainer in this.containers)
		{
			global::Item item = itemContainer.FindItemByUID(id);
			if (item != null && item.IsValid())
			{
				return item;
			}
		}
		return null;
	}

	// Token: 0x06000F92 RID: 3986 RVA: 0x00080E90 File Offset: 0x0007F090
	public void Check()
	{
		if (!this.IsLooting())
		{
			return;
		}
		if (!base.baseEntity.isServer)
		{
			return;
		}
		if (this.entitySource == null)
		{
			base.baseEntity.ChatMessage("Stopping Looting because lootable doesn't exist!");
			this.Clear();
			return;
		}
		if (!this.entitySource.CanBeLooted(base.baseEntity))
		{
			this.Clear();
			return;
		}
		if (this.PositionChecks)
		{
			float num = this.entitySource.Distance(base.baseEntity.eyes.position);
			if (num > 3f)
			{
				LootDistanceOverride component = this.entitySource.GetComponent<LootDistanceOverride>();
				if (component == null || num > component.amount)
				{
					this.Clear();
					return;
				}
			}
		}
	}

	// Token: 0x06000F93 RID: 3987 RVA: 0x00080F44 File Offset: 0x0007F144
	private void MarkDirty()
	{
		if (!this.isInvokingSendUpdate)
		{
			this.isInvokingSendUpdate = true;
			base.Invoke(new Action(this.SendUpdate), 0.1f);
		}
	}

	// Token: 0x06000F94 RID: 3988 RVA: 0x00080F6C File Offset: 0x0007F16C
	public void SendImmediate()
	{
		if (this.isInvokingSendUpdate)
		{
			this.isInvokingSendUpdate = false;
			base.CancelInvoke(new Action(this.SendUpdate));
		}
		this.SendUpdate();
	}

	// Token: 0x06000F95 RID: 3989 RVA: 0x00080F98 File Offset: 0x0007F198
	private void SendUpdate()
	{
		this.isInvokingSendUpdate = false;
		if (!base.baseEntity.IsValid())
		{
			return;
		}
		using (PlayerUpdateLoot playerUpdateLoot = Pool.Get<PlayerUpdateLoot>())
		{
			if (this.entitySource && this.entitySource.net != null)
			{
				playerUpdateLoot.entityID = this.entitySource.net.ID;
			}
			if (this.itemSource != null)
			{
				playerUpdateLoot.itemID = this.itemSource.uid;
			}
			if (this.containers.Count > 0)
			{
				playerUpdateLoot.containers = Pool.Get<List<ProtoBuf.ItemContainer>>();
				foreach (global::ItemContainer itemContainer in this.containers)
				{
					playerUpdateLoot.containers.Add(itemContainer.Save());
				}
			}
			base.baseEntity.ClientRPCPlayer<PlayerUpdateLoot>(null, base.baseEntity, "UpdateLoot", playerUpdateLoot);
		}
	}

	// Token: 0x06000F96 RID: 3990 RVA: 0x000810A4 File Offset: 0x0007F2A4
	public bool StartLootingEntity(global::BaseEntity targetEntity, bool doPositionChecks = true)
	{
		this.Clear();
		if (!targetEntity)
		{
			return false;
		}
		if (!targetEntity.OnStartBeingLooted(base.baseEntity))
		{
			return false;
		}
		Assert.IsTrue(targetEntity.isServer, "Assure is server");
		this.PositionChecks = doPositionChecks;
		this.entitySource = targetEntity;
		this.itemSource = null;
		this.MarkDirty();
		return true;
	}

	// Token: 0x06000F97 RID: 3991 RVA: 0x000810FD File Offset: 0x0007F2FD
	public void AddContainer(global::ItemContainer container)
	{
		if (container == null)
		{
			return;
		}
		this.containers.Add(container);
		container.onDirty += this.MarkDirty;
	}

	// Token: 0x06000F98 RID: 3992 RVA: 0x00081121 File Offset: 0x0007F321
	public void RemoveContainer(global::ItemContainer container)
	{
		if (container == null)
		{
			return;
		}
		container.onDirty -= this.MarkDirty;
		this.containers.Remove(container);
	}

	// Token: 0x06000F99 RID: 3993 RVA: 0x00081148 File Offset: 0x0007F348
	public bool RemoveContainerAt(int index)
	{
		if (index < 0 || index >= this.containers.Count)
		{
			return false;
		}
		if (this.containers[index] != null)
		{
			this.containers[index].onDirty -= this.MarkDirty;
		}
		this.containers.RemoveAt(index);
		return true;
	}

	// Token: 0x06000F9A RID: 3994 RVA: 0x000811A4 File Offset: 0x0007F3A4
	public void StartLootingItem(global::Item item)
	{
		this.Clear();
		if (item == null)
		{
			return;
		}
		if (item.contents == null)
		{
			return;
		}
		this.PositionChecks = true;
		this.containers.Add(item.contents);
		item.contents.onDirty += this.MarkDirty;
		this.itemSource = item;
		this.entitySource = item.GetWorldEntity();
		this.MarkDirty();
	}

	// Token: 0x040009EF RID: 2543
	public global::BaseEntity entitySource;

	// Token: 0x040009F0 RID: 2544
	public global::Item itemSource;

	// Token: 0x040009F1 RID: 2545
	public List<global::ItemContainer> containers = new List<global::ItemContainer>();

	// Token: 0x040009F2 RID: 2546
	internal bool PositionChecks = true;

	// Token: 0x040009F3 RID: 2547
	private bool isInvokingSendUpdate;
}
