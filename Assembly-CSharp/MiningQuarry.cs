using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x020003E0 RID: 992
public class MiningQuarry : BaseResourceExtractor
{
	// Token: 0x060021A3 RID: 8611 RVA: 0x0002782C File Offset: 0x00025A2C
	public bool IsEngineOn()
	{
		return base.HasFlag(global::BaseEntity.Flags.On);
	}

	// Token: 0x060021A4 RID: 8612 RVA: 0x000D7E28 File Offset: 0x000D6028
	private void SetOn(bool isOn)
	{
		base.SetFlag(global::BaseEntity.Flags.On, isOn, false, true);
		this.engineSwitchPrefab.instance.SetFlag(global::BaseEntity.Flags.On, isOn, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		this.engineSwitchPrefab.instance.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		if (isOn)
		{
			base.InvokeRepeating(new Action(this.ProcessResources), this.processRate, this.processRate);
			return;
		}
		base.CancelInvoke(new Action(this.ProcessResources));
	}

	// Token: 0x060021A5 RID: 8613 RVA: 0x000D7E9F File Offset: 0x000D609F
	public void EngineSwitch(bool isOn)
	{
		if (isOn && this.FuelCheck())
		{
			this.SetOn(true);
			return;
		}
		this.SetOn(false);
	}

	// Token: 0x060021A6 RID: 8614 RVA: 0x000D7EBC File Offset: 0x000D60BC
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.isStatic)
		{
			this.UpdateStaticDeposit();
		}
		else
		{
			ResourceDepositManager.ResourceDeposit orCreate = ResourceDepositManager.GetOrCreate(base.transform.position);
			this._linkedDeposit = orCreate;
		}
		this.SpawnChildEntities();
		this.engineSwitchPrefab.instance.SetFlag(global::BaseEntity.Flags.On, base.HasFlag(global::BaseEntity.Flags.On), false, true);
		if (base.isServer)
		{
			global::ItemContainer inventory = this.fuelStoragePrefab.instance.GetComponent<StorageContainer>().inventory;
			inventory.canAcceptItem = (Func<global::Item, int, bool>)Delegate.Combine(inventory.canAcceptItem, new Func<global::Item, int, bool>(this.CanAcceptItem));
		}
	}

	// Token: 0x060021A7 RID: 8615 RVA: 0x000D7F55 File Offset: 0x000D6155
	public bool CanAcceptItem(global::Item item, int targetSlot)
	{
		return item.info.shortname == "diesel_barrel";
	}

	// Token: 0x060021A8 RID: 8616 RVA: 0x000D7F6C File Offset: 0x000D616C
	public void UpdateStaticDeposit()
	{
		if (!this.isStatic)
		{
			return;
		}
		if (this._linkedDeposit == null)
		{
			this._linkedDeposit = new ResourceDepositManager.ResourceDeposit();
		}
		else
		{
			this._linkedDeposit._resources.Clear();
		}
		if (this.staticType == global::MiningQuarry.QuarryType.None)
		{
			this._linkedDeposit.Add(ItemManager.FindItemDefinition("stones"), 1f, 1000, 0.3f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
			this._linkedDeposit.Add(ItemManager.FindItemDefinition("metal.ore"), 1f, 1000, 5f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
			this._linkedDeposit.Add(ItemManager.FindItemDefinition("sulfur.ore"), 1f, 1000, 7.5f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
			this._linkedDeposit.Add(ItemManager.FindItemDefinition("hq.metal.ore"), 1f, 1000, 75f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
		}
		else if (this.staticType == global::MiningQuarry.QuarryType.Basic)
		{
			this._linkedDeposit.Add(ItemManager.FindItemDefinition("metal.ore"), 1f, 1000, 1f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
			this._linkedDeposit.Add(ItemManager.FindItemDefinition("stones"), 1f, 1000, 0.2f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
		}
		else if (this.staticType == global::MiningQuarry.QuarryType.Sulfur)
		{
			this._linkedDeposit.Add(ItemManager.FindItemDefinition("sulfur.ore"), 1f, 1000, 1f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
		}
		else if (this.staticType == global::MiningQuarry.QuarryType.HQM)
		{
			this._linkedDeposit.Add(ItemManager.FindItemDefinition("hq.metal.ore"), 1f, 1000, 20f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
		}
		this._linkedDeposit.Add(ItemManager.FindItemDefinition("crude.oil"), 1f, 1000, 16.666666f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, true);
		this._linkedDeposit.Add(ItemManager.FindItemDefinition("lowgradefuel"), 1f, 1000, 5.882353f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, true);
	}

	// Token: 0x060021A9 RID: 8617 RVA: 0x000D8152 File Offset: 0x000D6352
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.EngineSwitch(base.HasFlag(global::BaseEntity.Flags.On));
		this.UpdateStaticDeposit();
	}

	// Token: 0x060021AA RID: 8618 RVA: 0x000D816D File Offset: 0x000D636D
	public void SpawnChildEntities()
	{
		this.engineSwitchPrefab.DoSpawn(this);
		this.hopperPrefab.DoSpawn(this);
		this.fuelStoragePrefab.DoSpawn(this);
	}

	// Token: 0x060021AB RID: 8619 RVA: 0x000D8194 File Offset: 0x000D6394
	public void ProcessResources()
	{
		if (this._linkedDeposit == null)
		{
			return;
		}
		if (this.hopperPrefab.instance == null)
		{
			return;
		}
		if (!this.FuelCheck())
		{
			this.SetOn(false);
		}
		float num = Mathf.Min(this.workToAdd, this.pendingWork);
		this.pendingWork -= num;
		foreach (ResourceDepositManager.ResourceDeposit.ResourceDepositEntry resourceDepositEntry in this._linkedDeposit._resources)
		{
			if ((this.canExtractLiquid || !resourceDepositEntry.isLiquid) && (this.canExtractSolid || resourceDepositEntry.isLiquid))
			{
				float workNeeded = resourceDepositEntry.workNeeded;
				int num2 = Mathf.FloorToInt(resourceDepositEntry.workDone / workNeeded);
				resourceDepositEntry.workDone += num;
				int num3 = Mathf.FloorToInt(resourceDepositEntry.workDone / workNeeded);
				if (resourceDepositEntry.workDone > workNeeded)
				{
					resourceDepositEntry.workDone %= workNeeded;
				}
				if (num2 != num3)
				{
					int iAmount = num3 - num2;
					global::Item item = ItemManager.Create(resourceDepositEntry.type, iAmount, 0UL);
					if (!item.MoveToContainer(this.hopperPrefab.instance.GetComponent<StorageContainer>().inventory, -1, true, false, null, true))
					{
						item.Remove(0f);
						this.SetOn(false);
					}
				}
			}
		}
	}

	// Token: 0x060021AC RID: 8620 RVA: 0x000D82FC File Offset: 0x000D64FC
	public bool FuelCheck()
	{
		if (this.pendingWork > 0f)
		{
			return true;
		}
		global::Item item = this.fuelStoragePrefab.instance.GetComponent<StorageContainer>().inventory.FindItemsByItemName("diesel_barrel");
		if (item != null && item.amount >= 1)
		{
			this.pendingWork += this.workPerFuel;
			item.UseItem(1);
			return true;
		}
		return false;
	}

	// Token: 0x060021AD RID: 8621 RVA: 0x000D8364 File Offset: 0x000D6564
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			if (this.fuelStoragePrefab.instance == null || this.hopperPrefab.instance == null)
			{
				Debug.Log("Cannot save mining quary because children were null");
				return;
			}
			info.msg.miningQuarry = Pool.Get<ProtoBuf.MiningQuarry>();
			info.msg.miningQuarry.extractor = Pool.Get<ResourceExtractor>();
			info.msg.miningQuarry.extractor.fuelContents = this.fuelStoragePrefab.instance.GetComponent<StorageContainer>().inventory.Save();
			info.msg.miningQuarry.extractor.outputContents = this.hopperPrefab.instance.GetComponent<StorageContainer>().inventory.Save();
			info.msg.miningQuarry.staticType = (int)this.staticType;
		}
	}

	// Token: 0x060021AE RID: 8622 RVA: 0x000D8450 File Offset: 0x000D6650
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk && info.msg.miningQuarry != null)
		{
			if (this.fuelStoragePrefab.instance == null || this.hopperPrefab.instance == null)
			{
				Debug.Log("Cannot load mining quary because children were null");
				return;
			}
			this.fuelStoragePrefab.instance.GetComponent<StorageContainer>().inventory.Load(info.msg.miningQuarry.extractor.fuelContents);
			this.hopperPrefab.instance.GetComponent<StorageContainer>().inventory.Load(info.msg.miningQuarry.extractor.outputContents);
			this.staticType = (global::MiningQuarry.QuarryType)info.msg.miningQuarry.staticType;
		}
	}

	// Token: 0x060021AF RID: 8623 RVA: 0x000059DD File Offset: 0x00003BDD
	public void Update()
	{
	}

	// Token: 0x04001A05 RID: 6661
	public Animator beltAnimator;

	// Token: 0x04001A06 RID: 6662
	public Renderer beltScrollRenderer;

	// Token: 0x04001A07 RID: 6663
	public int scrollMatIndex = 3;

	// Token: 0x04001A08 RID: 6664
	public SoundPlayer[] onSounds;

	// Token: 0x04001A09 RID: 6665
	public float processRate = 5f;

	// Token: 0x04001A0A RID: 6666
	public float workToAdd = 15f;

	// Token: 0x04001A0B RID: 6667
	public float workPerFuel = 1000f;

	// Token: 0x04001A0C RID: 6668
	public float pendingWork;

	// Token: 0x04001A0D RID: 6669
	public GameObjectRef bucketDropEffect;

	// Token: 0x04001A0E RID: 6670
	public GameObject bucketDropTransform;

	// Token: 0x04001A0F RID: 6671
	public global::MiningQuarry.ChildPrefab engineSwitchPrefab;

	// Token: 0x04001A10 RID: 6672
	public global::MiningQuarry.ChildPrefab hopperPrefab;

	// Token: 0x04001A11 RID: 6673
	public global::MiningQuarry.ChildPrefab fuelStoragePrefab;

	// Token: 0x04001A12 RID: 6674
	public global::MiningQuarry.QuarryType staticType;

	// Token: 0x04001A13 RID: 6675
	public bool isStatic;

	// Token: 0x04001A14 RID: 6676
	private ResourceDepositManager.ResourceDeposit _linkedDeposit;

	// Token: 0x02000C81 RID: 3201
	[Serializable]
	public enum QuarryType
	{
		// Token: 0x04004299 RID: 17049
		None,
		// Token: 0x0400429A RID: 17050
		Basic,
		// Token: 0x0400429B RID: 17051
		Sulfur,
		// Token: 0x0400429C RID: 17052
		HQM
	}

	// Token: 0x02000C82 RID: 3202
	[Serializable]
	public class ChildPrefab
	{
		// Token: 0x06004CFF RID: 19711 RVA: 0x00196CE0 File Offset: 0x00194EE0
		public void DoSpawn(global::MiningQuarry owner)
		{
			if (!this.prefabToSpawn.isValid)
			{
				return;
			}
			this.instance = GameManager.server.CreateEntity(this.prefabToSpawn.resourcePath, this.origin.transform.localPosition, this.origin.transform.localRotation, true);
			this.instance.SetParent(owner, false, false);
			this.instance.Spawn();
		}

		// Token: 0x0400429D RID: 17053
		public GameObjectRef prefabToSpawn;

		// Token: 0x0400429E RID: 17054
		public GameObject origin;

		// Token: 0x0400429F RID: 17055
		public global::BaseEntity instance;
	}
}
