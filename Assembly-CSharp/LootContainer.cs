using System;
using Rust;
using UnityEngine;

// Token: 0x020003F2 RID: 1010
public class LootContainer : StorageContainer
{
	// Token: 0x1700029E RID: 670
	// (get) Token: 0x060021EC RID: 8684 RVA: 0x000D9197 File Offset: 0x000D7397
	public bool shouldRefreshContents
	{
		get
		{
			return this.minSecondsBetweenRefresh > 0f && this.maxSecondsBetweenRefresh > 0f;
		}
	}

	// Token: 0x060021ED RID: 8685 RVA: 0x000D91B5 File Offset: 0x000D73B5
	public override void ResetState()
	{
		this.FirstLooted = false;
		base.ResetState();
	}

	// Token: 0x060021EE RID: 8686 RVA: 0x000D91C4 File Offset: 0x000D73C4
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.initialLootSpawn)
		{
			this.SpawnLoot();
		}
		if (this.BlockPlayerItemInput && !Rust.Application.isLoadingSave && base.inventory != null)
		{
			base.inventory.SetFlag(ItemContainer.Flag.NoItemInput, true);
		}
		base.SetFlag(BaseEntity.Flags.Reserved6, PlayerInventory.IsBirthday(), false, true);
	}

	// Token: 0x060021EF RID: 8687 RVA: 0x000D921F File Offset: 0x000D741F
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (this.BlockPlayerItemInput && base.inventory != null)
		{
			base.inventory.SetFlag(ItemContainer.Flag.NoItemInput, true);
		}
	}

	// Token: 0x060021F0 RID: 8688 RVA: 0x000D9248 File Offset: 0x000D7448
	public virtual void SpawnLoot()
	{
		if (base.inventory == null)
		{
			Debug.Log("CONTACT DEVELOPERS! LootContainer::PopulateLoot has null inventory!!!");
			return;
		}
		base.inventory.Clear();
		ItemManager.DoRemoves();
		this.PopulateLoot();
		if (this.shouldRefreshContents)
		{
			base.Invoke(new Action(this.SpawnLoot), UnityEngine.Random.Range(this.minSecondsBetweenRefresh, this.maxSecondsBetweenRefresh));
		}
	}

	// Token: 0x060021F1 RID: 8689 RVA: 0x000D92AA File Offset: 0x000D74AA
	public int ScoreForRarity(Rarity rarity)
	{
		switch (rarity)
		{
		case Rarity.Common:
			return 1;
		case Rarity.Uncommon:
			return 2;
		case Rarity.Rare:
			return 3;
		case Rarity.VeryRare:
			return 4;
		default:
			return 5000;
		}
	}

	// Token: 0x060021F2 RID: 8690 RVA: 0x000D92D4 File Offset: 0x000D74D4
	public virtual void PopulateLoot()
	{
		if (this.LootSpawnSlots.Length != 0)
		{
			foreach (LootContainer.LootSpawnSlot lootSpawnSlot in this.LootSpawnSlots)
			{
				for (int j = 0; j < lootSpawnSlot.numberToSpawn; j++)
				{
					if (UnityEngine.Random.Range(0f, 1f) <= lootSpawnSlot.probability)
					{
						lootSpawnSlot.definition.SpawnIntoContainer(base.inventory);
					}
				}
			}
		}
		else if (this.lootDefinition != null)
		{
			for (int k = 0; k < this.maxDefinitionsToSpawn; k++)
			{
				this.lootDefinition.SpawnIntoContainer(base.inventory);
			}
		}
		if (this.SpawnType == LootContainer.spawnType.ROADSIDE || this.SpawnType == LootContainer.spawnType.TOWN)
		{
			foreach (Item item in base.inventory.itemList)
			{
				if (item.hasCondition)
				{
					item.condition = UnityEngine.Random.Range(item.info.condition.foundCondition.fractionMin, item.info.condition.foundCondition.fractionMax) * item.info.condition.max;
				}
			}
		}
		this.GenerateScrap();
	}

	// Token: 0x060021F3 RID: 8691 RVA: 0x000D942C File Offset: 0x000D762C
	public void GenerateScrap()
	{
		if (this.scrapAmount <= 0)
		{
			return;
		}
		if (LootContainer.scrapDef == null)
		{
			LootContainer.scrapDef = ItemManager.FindItemDefinition("scrap");
		}
		int num = this.scrapAmount;
		if (num > 0)
		{
			Item item = ItemManager.Create(LootContainer.scrapDef, num, 0UL);
			if (!item.MoveToContainer(base.inventory, -1, true, false, null, true))
			{
				item.Drop(base.transform.position, this.GetInheritedDropVelocity(), default(Quaternion));
			}
		}
	}

	// Token: 0x060021F4 RID: 8692 RVA: 0x000D94AC File Offset: 0x000D76AC
	public override void DropBonusItems(BaseEntity initiator, ItemContainer container)
	{
		base.DropBonusItems(initiator, container);
		if (initiator == null || container == null)
		{
			return;
		}
		BasePlayer basePlayer = initiator as BasePlayer;
		if (basePlayer == null)
		{
			return;
		}
		if (this.scrapAmount > 0 && LootContainer.scrapDef != null)
		{
			float num = (basePlayer.modifiers != null) ? (1f + basePlayer.modifiers.GetValue(Modifier.ModifierType.Scrap_Yield, 0f)) : 0f;
			if (num > 1f)
			{
				float num2 = basePlayer.modifiers.GetVariableValue(Modifier.ModifierType.Scrap_Yield, 0f);
				float num3 = Mathf.Max((float)this.scrapAmount * num - (float)this.scrapAmount, 0f);
				num2 += num3;
				int num4 = 0;
				if (num2 >= 1f)
				{
					num4 = (int)num2;
					num2 -= (float)num4;
				}
				basePlayer.modifiers.SetVariableValue(Modifier.ModifierType.Scrap_Yield, num2);
				if (num4 > 0)
				{
					Item item = ItemManager.Create(LootContainer.scrapDef, num4, 0UL);
					if (item != null)
					{
						item.Drop(this.GetDropPosition() + new Vector3(0f, 0.5f, 0f), this.GetInheritedDropVelocity(), default(Quaternion));
					}
				}
			}
		}
	}

	// Token: 0x060021F5 RID: 8693 RVA: 0x000D95D7 File Offset: 0x000D77D7
	public override bool OnStartBeingLooted(BasePlayer baseEntity)
	{
		if (!this.FirstLooted)
		{
			this.FirstLooted = true;
		}
		return base.OnStartBeingLooted(baseEntity);
	}

	// Token: 0x060021F6 RID: 8694 RVA: 0x000D95EF File Offset: 0x000D77EF
	public override void PlayerStoppedLooting(BasePlayer player)
	{
		base.PlayerStoppedLooting(player);
		if (this.destroyOnEmpty && (base.inventory == null || base.inventory.itemList == null || base.inventory.itemList.Count == 0))
		{
			base.Kill(BaseNetworkable.DestroyMode.Gib);
		}
	}

	// Token: 0x060021F7 RID: 8695 RVA: 0x00026D90 File Offset: 0x00024F90
	public void RemoveMe()
	{
		base.Kill(BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x060021F8 RID: 8696 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool ShouldDropItemsIndividually()
	{
		return true;
	}

	// Token: 0x060021F9 RID: 8697 RVA: 0x000D962E File Offset: 0x000D782E
	public override void OnKilled(HitInfo info)
	{
		base.OnKilled(info);
		if (info != null && info.InitiatorPlayer != null && !string.IsNullOrEmpty(this.deathStat))
		{
			info.InitiatorPlayer.stats.Add(this.deathStat, 1, Stats.Life);
		}
	}

	// Token: 0x060021FA RID: 8698 RVA: 0x000D966D File Offset: 0x000D786D
	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
	}

	// Token: 0x060021FB RID: 8699 RVA: 0x000D9676 File Offset: 0x000D7876
	public override void InitShared()
	{
		base.InitShared();
	}

	// Token: 0x04001A4E RID: 6734
	public bool destroyOnEmpty = true;

	// Token: 0x04001A4F RID: 6735
	public LootSpawn lootDefinition;

	// Token: 0x04001A50 RID: 6736
	public int maxDefinitionsToSpawn;

	// Token: 0x04001A51 RID: 6737
	public float minSecondsBetweenRefresh = 3600f;

	// Token: 0x04001A52 RID: 6738
	public float maxSecondsBetweenRefresh = 7200f;

	// Token: 0x04001A53 RID: 6739
	public bool initialLootSpawn = true;

	// Token: 0x04001A54 RID: 6740
	public float xpLootedScale = 1f;

	// Token: 0x04001A55 RID: 6741
	public float xpDestroyedScale = 1f;

	// Token: 0x04001A56 RID: 6742
	public bool BlockPlayerItemInput;

	// Token: 0x04001A57 RID: 6743
	public int scrapAmount;

	// Token: 0x04001A58 RID: 6744
	public string deathStat = "";

	// Token: 0x04001A59 RID: 6745
	public LootContainer.LootSpawnSlot[] LootSpawnSlots;

	// Token: 0x04001A5A RID: 6746
	public LootContainer.spawnType SpawnType;

	// Token: 0x04001A5B RID: 6747
	public bool FirstLooted;

	// Token: 0x04001A5C RID: 6748
	private static ItemDefinition scrapDef;

	// Token: 0x02000C86 RID: 3206
	public enum spawnType
	{
		// Token: 0x040042BA RID: 17082
		GENERIC,
		// Token: 0x040042BB RID: 17083
		PLAYER,
		// Token: 0x040042BC RID: 17084
		TOWN,
		// Token: 0x040042BD RID: 17085
		AIRDROP,
		// Token: 0x040042BE RID: 17086
		CRASHSITE,
		// Token: 0x040042BF RID: 17087
		ROADSIDE
	}

	// Token: 0x02000C87 RID: 3207
	[Serializable]
	public struct LootSpawnSlot
	{
		// Token: 0x040042C0 RID: 17088
		public LootSpawn definition;

		// Token: 0x040042C1 RID: 17089
		public int numberToSpawn;

		// Token: 0x040042C2 RID: 17090
		public float probability;
	}
}
