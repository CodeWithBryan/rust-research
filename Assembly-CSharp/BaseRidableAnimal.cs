using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000043 RID: 67
public class BaseRidableAnimal : global::BaseVehicle
{
	// Token: 0x060006EA RID: 1770 RVA: 0x00047560 File Offset: 0x00045760
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseRidableAnimal.OnRpcMessage", 0))
		{
			if (rpc == 2333451803U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Claim ");
				}
				using (TimeWarning.New("RPC_Claim", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2333451803U, "RPC_Claim", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Claim(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_Claim");
					}
				}
				return true;
			}
			if (rpc == 3653170552U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Lead ");
				}
				using (TimeWarning.New("RPC_Lead", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3653170552U, "RPC_Lead", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Lead(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_Lead");
					}
				}
				return true;
			}
			if (rpc == 331989034U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_OpenLoot ");
				}
				using (TimeWarning.New("RPC_OpenLoot", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(331989034U, "RPC_OpenLoot", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_OpenLoot(rpc2);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in RPC_OpenLoot");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060006EB RID: 1771 RVA: 0x00004C84 File Offset: 0x00002E84
	public bool IsForSale()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved2);
	}

	// Token: 0x060006EC RID: 1772 RVA: 0x000479BC File Offset: 0x00045BBC
	public void ContainerServerInit()
	{
		if (this.inventory == null)
		{
			this.CreateInventory(true);
			this.OnInventoryFirstCreated(this.inventory);
		}
	}

	// Token: 0x060006ED RID: 1773 RVA: 0x000479DC File Offset: 0x00045BDC
	public void CreateInventory(bool giveUID)
	{
		this.inventory = new global::ItemContainer();
		this.inventory.entityOwner = this;
		this.inventory.allowedContents = ((this.allowedContents == (global::ItemContainer.ContentsType)0) ? global::ItemContainer.ContentsType.Generic : this.allowedContents);
		this.inventory.SetOnlyAllowedItem(this.onlyAllowedItem);
		this.inventory.maxStackSize = this.maxStackSize;
		this.inventory.ServerInitialize(null, this.numSlots);
		this.inventory.canAcceptItem = new Func<global::Item, int, bool>(this.ItemFilter);
		if (giveUID)
		{
			this.inventory.GiveUID();
		}
		this.inventory.onItemAddedRemoved = new Action<global::Item, bool>(this.OnItemAddedOrRemoved);
		this.inventory.onDirty += this.OnInventoryDirty;
	}

	// Token: 0x060006EE RID: 1774 RVA: 0x00047AA8 File Offset: 0x00045CA8
	public void SaveContainer(global::BaseNetworkable.SaveInfo info)
	{
		if (info.forDisk)
		{
			if (this.inventory != null)
			{
				info.msg.storageBox = Facepunch.Pool.Get<StorageBox>();
				info.msg.storageBox.contents = this.inventory.Save();
				return;
			}
			Debug.LogWarning("Storage container without inventory: " + this.ToString());
		}
	}

	// Token: 0x060006EF RID: 1775 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnInventoryFirstCreated(global::ItemContainer container)
	{
	}

	// Token: 0x060006F0 RID: 1776 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnInventoryDirty()
	{
	}

	// Token: 0x060006F1 RID: 1777 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnItemAddedOrRemoved(global::Item item, bool added)
	{
	}

	// Token: 0x060006F2 RID: 1778 RVA: 0x00047B06 File Offset: 0x00045D06
	public bool ItemFilter(global::Item item, int targetSlot)
	{
		return this.CanAnimalAcceptItem(item, targetSlot);
	}

	// Token: 0x060006F3 RID: 1779 RVA: 0x00003A54 File Offset: 0x00001C54
	public virtual bool CanAnimalAcceptItem(global::Item item, int targetSlot)
	{
		return true;
	}

	// Token: 0x060006F4 RID: 1780 RVA: 0x00047B10 File Offset: 0x00045D10
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void RPC_OpenLoot(global::BaseEntity.RPCMessage rpc)
	{
		if (this.inventory == null)
		{
			return;
		}
		global::BasePlayer player = rpc.player;
		if (!player || !player.CanInteract())
		{
			return;
		}
		if (!this.CanOpenStorage(player))
		{
			return;
		}
		if (this.needsBuildingPrivilegeToUse && !player.CanBuild())
		{
			return;
		}
		if (player.inventory.loot.StartLootingEntity(this, true))
		{
			player.inventory.loot.AddContainer(this.inventory);
			player.inventory.loot.SendImmediate();
			player.ClientRPCPlayer<string>(null, player, "RPC_OpenLootPanel", this.lootPanelName);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x060006F5 RID: 1781 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void PlayerStoppedLooting(global::BasePlayer player)
	{
	}

	// Token: 0x060006F6 RID: 1782 RVA: 0x00047BAD File Offset: 0x00045DAD
	public virtual bool CanOpenStorage(global::BasePlayer player)
	{
		return !base.HasFlag(global::BaseEntity.Flags.On) || this.PlayerIsMounted(player);
	}

	// Token: 0x060006F7 RID: 1783 RVA: 0x00047BC8 File Offset: 0x00045DC8
	public void LoadContainer(global::BaseNetworkable.LoadInfo info)
	{
		if (info.fromDisk && info.msg.storageBox != null)
		{
			if (this.inventory != null)
			{
				this.inventory.Load(info.msg.storageBox.contents);
				this.inventory.capacity = this.numSlots;
				return;
			}
			Debug.LogWarning("Storage container without inventory: " + this.ToString());
		}
	}

	// Token: 0x060006F8 RID: 1784 RVA: 0x00047C34 File Offset: 0x00045E34
	public float GetBreathingDelay()
	{
		switch (this.currentRunState)
		{
		default:
			return -1f;
		case BaseRidableAnimal.RunState.walk:
			return 8f;
		case BaseRidableAnimal.RunState.run:
			return 5f;
		case BaseRidableAnimal.RunState.sprint:
			return 2.5f;
		}
	}

	// Token: 0x060006F9 RID: 1785 RVA: 0x00047C77 File Offset: 0x00045E77
	public bool IsLeading()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved7);
	}

	// Token: 0x060006FA RID: 1786 RVA: 0x00047C84 File Offset: 0x00045E84
	public static float UnitsToKPH(float unitsPerSecond)
	{
		return unitsPerSecond * 60f * 60f / 1000f;
	}

	// Token: 0x170000B1 RID: 177
	// (get) Token: 0x060006FB RID: 1787 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool IsNpc
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060006FC RID: 1788 RVA: 0x00047C9C File Offset: 0x00045E9C
	public static void ProcessQueue()
	{
		float realtimeSinceStartup = UnityEngine.Time.realtimeSinceStartup;
		float num = BaseRidableAnimal.framebudgetms / 1000f;
		while (BaseRidableAnimal._processQueue.Count > 0 && UnityEngine.Time.realtimeSinceStartup < realtimeSinceStartup + num)
		{
			BaseRidableAnimal baseRidableAnimal = BaseRidableAnimal._processQueue.Dequeue();
			if (baseRidableAnimal != null)
			{
				baseRidableAnimal.BudgetedUpdate();
				baseRidableAnimal.inQueue = false;
			}
		}
	}

	// Token: 0x060006FD RID: 1789 RVA: 0x00047CF6 File Offset: 0x00045EF6
	public void SetLeading(global::BaseEntity newLeadTarget)
	{
		this.leadTarget = newLeadTarget;
		base.SetFlag(global::BaseEntity.Flags.Reserved7, this.leadTarget != null, false, true);
	}

	// Token: 0x060006FE RID: 1790 RVA: 0x00047D18 File Offset: 0x00045F18
	public override float GetNetworkTime()
	{
		return this.lastMovementUpdateTime;
	}

	// Token: 0x060006FF RID: 1791 RVA: 0x00047D20 File Offset: 0x00045F20
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		this.SaveContainer(info);
	}

	// Token: 0x06000700 RID: 1792 RVA: 0x00047D30 File Offset: 0x00045F30
	private void OnPhysicsNeighbourChanged()
	{
		base.Invoke(new Action(this.DelayedDropToGround), UnityEngine.Time.fixedDeltaTime);
	}

	// Token: 0x06000701 RID: 1793 RVA: 0x00047D49 File Offset: 0x00045F49
	public void DelayedDropToGround()
	{
		this.DropToGround(base.transform.position, true);
		this.UpdateGroundNormal(true);
	}

	// Token: 0x06000702 RID: 1794 RVA: 0x00047D65 File Offset: 0x00045F65
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		this.LoadContainer(info);
	}

	// Token: 0x06000703 RID: 1795 RVA: 0x00047D75 File Offset: 0x00045F75
	public override void AttemptMount(global::BasePlayer player, bool doMountChecks = true)
	{
		if (this.IsForSale())
		{
			return;
		}
		base.AttemptMount(player, doMountChecks);
	}

	// Token: 0x06000704 RID: 1796 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void LeadingChanged()
	{
	}

	// Token: 0x06000705 RID: 1797 RVA: 0x00047D88 File Offset: 0x00045F88
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_Claim(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		if (!this.IsForSale())
		{
			return;
		}
		global::Item item = this.GetPurchaseToken(player);
		if (item == null)
		{
			return;
		}
		item.UseItem(1);
		base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
		Analytics.Server.VehiclePurchased(base.ShortPrefabName);
		this.AttemptMount(player, false);
	}

	// Token: 0x06000706 RID: 1798 RVA: 0x00047DE4 File Offset: 0x00045FE4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_Lead(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		if (base.HasDriver())
		{
			return;
		}
		if (this.IsForSale())
		{
			return;
		}
		bool flag = this.IsLeading();
		bool flag2 = msg.read.Bit();
		if (flag == flag2)
		{
			return;
		}
		this.SetLeading(flag2 ? player : null);
		this.LeadingChanged();
	}

	// Token: 0x06000707 RID: 1799 RVA: 0x00047E3D File Offset: 0x0004603D
	public override void PlayerMounted(global::BasePlayer player, BaseMountable seat)
	{
		base.PlayerMounted(player, seat);
		base.SetFlag(global::BaseEntity.Flags.On, true, true, true);
	}

	// Token: 0x06000708 RID: 1800 RVA: 0x00047E51 File Offset: 0x00046051
	public override void PlayerDismounted(global::BasePlayer player, BaseMountable seat)
	{
		base.PlayerDismounted(player, seat);
		base.SetFlag(global::BaseEntity.Flags.On, false, true, true);
	}

	// Token: 0x06000709 RID: 1801 RVA: 0x00047E68 File Offset: 0x00046068
	public void SetDecayActive(bool isActive)
	{
		if (isActive)
		{
			base.InvokeRandomized(new Action(this.AnimalDecay), UnityEngine.Random.Range(30f, 60f), 60f, 6f);
			return;
		}
		base.CancelInvoke(new Action(this.AnimalDecay));
	}

	// Token: 0x0600070A RID: 1802 RVA: 0x00047EB6 File Offset: 0x000460B6
	public float TimeUntilNextDecay()
	{
		return this.nextDecayTime - UnityEngine.Time.time;
	}

	// Token: 0x0600070B RID: 1803 RVA: 0x00047EC4 File Offset: 0x000460C4
	public void AddDecayDelay(float amount)
	{
		if (this.nextDecayTime < UnityEngine.Time.time)
		{
			this.nextDecayTime = UnityEngine.Time.time + 5f;
		}
		this.nextDecayTime += amount;
		if (ConVar.Global.developer > 0)
		{
			Debug.Log(string.Concat(new object[]
			{
				"Add Decay Delay ! amount is ",
				amount,
				"time until next decay : ",
				this.nextDecayTime - UnityEngine.Time.time
			}));
		}
	}

	// Token: 0x0600070C RID: 1804 RVA: 0x00047F42 File Offset: 0x00046142
	public override void Hurt(HitInfo info)
	{
		if (this.IsForSale())
		{
			return;
		}
		base.Hurt(info);
	}

	// Token: 0x0600070D RID: 1805 RVA: 0x00047F54 File Offset: 0x00046154
	public void AnimalDecay()
	{
		if (base.healthFraction == 0f || base.IsDestroyed)
		{
			return;
		}
		if (UnityEngine.Time.time < this.lastInputTime + 600f)
		{
			return;
		}
		if (UnityEngine.Time.time < this.lastEatTime + 600f)
		{
			return;
		}
		if (this.IsForSale())
		{
			return;
		}
		if (UnityEngine.Time.time < this.nextDecayTime)
		{
			if (ConVar.Global.developer > 0)
			{
				Debug.Log("Skipping animal decay due to hitching");
			}
			return;
		}
		float num = 1f / BaseRidableAnimal.decayminutes;
		float num2 = (!this.IsOutside()) ? 1f : 0.5f;
		base.Hurt(this.MaxHealth() * num * num2, DamageType.Decay, this, false);
	}

	// Token: 0x0600070E RID: 1806 RVA: 0x00047FFD File Offset: 0x000461FD
	public void UseStamina(float amount)
	{
		if (this.onIdealTerrain)
		{
			amount *= 0.5f;
		}
		this.staminaSeconds -= amount;
		if (this.staminaSeconds <= 0f)
		{
			this.staminaSeconds = 0f;
		}
	}

	// Token: 0x0600070F RID: 1807 RVA: 0x00048036 File Offset: 0x00046236
	public bool CanInitiateSprint()
	{
		return this.staminaSeconds > 4f;
	}

	// Token: 0x06000710 RID: 1808 RVA: 0x00048045 File Offset: 0x00046245
	public bool CanSprint()
	{
		return this.staminaSeconds > 0f;
	}

	// Token: 0x06000711 RID: 1809 RVA: 0x00048054 File Offset: 0x00046254
	public void ReplenishStamina(float amount)
	{
		float num = 1f + Mathf.InverseLerp(this.maxStaminaSeconds * 0.5f, this.maxStaminaSeconds, this.currentMaxStaminaSeconds);
		amount *= num;
		amount = Mathf.Min(this.currentMaxStaminaSeconds - this.staminaSeconds, amount);
		float num2 = Mathf.Min(this.currentMaxStaminaSeconds - this.staminaCoreLossRatio * amount, amount * this.staminaCoreLossRatio);
		this.currentMaxStaminaSeconds = Mathf.Clamp(this.currentMaxStaminaSeconds - num2, 0f, this.maxStaminaSeconds);
		this.staminaSeconds = Mathf.Clamp(this.staminaSeconds + num2 / this.staminaCoreLossRatio, 0f, this.currentMaxStaminaSeconds);
	}

	// Token: 0x06000712 RID: 1810 RVA: 0x000062DD File Offset: 0x000044DD
	public virtual float ReplenishRatio()
	{
		return 1f;
	}

	// Token: 0x06000713 RID: 1811 RVA: 0x00048100 File Offset: 0x00046300
	public void ReplenishStaminaCore(float calories, float hydration)
	{
		float num = calories * this.calorieToStaminaRatio;
		float num2 = hydration * this.hydrationToStaminaRatio;
		float num3 = this.ReplenishRatio();
		num2 = Mathf.Min(this.maxStaminaCoreFromWater - this.currentMaxStaminaSeconds, num2);
		if (num2 < 0f)
		{
			num2 = 0f;
		}
		float num4 = num + num2 * num3;
		this.currentMaxStaminaSeconds = Mathf.Clamp(this.currentMaxStaminaSeconds + num4, 0f, this.maxStaminaSeconds);
		this.staminaSeconds = Mathf.Clamp(this.staminaSeconds + num4, 0f, this.currentMaxStaminaSeconds);
	}

	// Token: 0x06000714 RID: 1812 RVA: 0x00048188 File Offset: 0x00046388
	public void UpdateStamina(float delta)
	{
		if (this.currentRunState == BaseRidableAnimal.RunState.sprint)
		{
			this.UseStamina(delta);
			return;
		}
		if (this.currentRunState == BaseRidableAnimal.RunState.run)
		{
			this.ReplenishStamina(this.staminaReplenishRatioMoving * delta);
			return;
		}
		this.ReplenishStamina(this.staminaReplenishRatioStanding * delta);
	}

	// Token: 0x06000715 RID: 1813 RVA: 0x000481C1 File Offset: 0x000463C1
	public override void PlayerServerInput(InputState inputState, global::BasePlayer player)
	{
		this.RiderInput(inputState, player);
	}

	// Token: 0x06000716 RID: 1814 RVA: 0x000481CC File Offset: 0x000463CC
	public void DismountHeavyPlayers()
	{
		if (base.HasFlag(global::BaseEntity.Flags.On))
		{
			global::BasePlayer driver = base.GetDriver();
			if (driver && this.IsPlayerTooHeavy(driver))
			{
				this.DismountAllPlayers();
			}
		}
	}

	// Token: 0x06000717 RID: 1815 RVA: 0x00048200 File Offset: 0x00046400
	public BaseMountable GetSaddle()
	{
		if (!this.saddleRef.IsValid(base.isServer))
		{
			return null;
		}
		return this.saddleRef.Get(base.isServer).GetComponent<BaseMountable>();
	}

	// Token: 0x06000718 RID: 1816 RVA: 0x00048230 File Offset: 0x00046430
	public void BudgetedUpdate()
	{
		this.DismountHeavyPlayers();
		this.UpdateOnIdealTerrain();
		this.UpdateStamina(UnityEngine.Time.fixedDeltaTime);
		if (this.currentRunState == BaseRidableAnimal.RunState.stopped)
		{
			this.EatNearbyFood();
		}
		if (this.lastMovementUpdateTime == -1f)
		{
			this.lastMovementUpdateTime = UnityEngine.Time.realtimeSinceStartup;
		}
		float delta = UnityEngine.Time.realtimeSinceStartup - this.lastMovementUpdateTime;
		this.UpdateMovement(delta);
		this.lastMovementUpdateTime = UnityEngine.Time.realtimeSinceStartup;
		this.UpdateDung(delta);
	}

	// Token: 0x06000719 RID: 1817 RVA: 0x000482A1 File Offset: 0x000464A1
	public void ApplyDungCalories(float calories)
	{
		this.pendingDungCalories += calories;
	}

	// Token: 0x0600071A RID: 1818 RVA: 0x000482B4 File Offset: 0x000464B4
	private void UpdateDung(float delta)
	{
		if (this.Dung == null)
		{
			return;
		}
		if (Mathf.Approximately(BaseRidableAnimal.dungTimeScale, 0f))
		{
			return;
		}
		float num = Mathf.Min(this.pendingDungCalories * delta, this.CaloriesToDigestPerHour / 3600f * delta) * this.DungProducedPerCalorie;
		this.dungProduction += num;
		this.pendingDungCalories -= num;
		if (this.dungProduction >= 1f)
		{
			this.DoDung();
		}
	}

	// Token: 0x0600071B RID: 1819 RVA: 0x00048334 File Offset: 0x00046534
	private void DoDung()
	{
		this.dungProduction -= 1f;
		ItemManager.Create(this.Dung, 1, 0UL).Drop(base.transform.position + -base.transform.forward + Vector3.up * 1.1f + UnityEngine.Random.insideUnitSphere * 0.1f, -base.transform.forward, default(Quaternion));
	}

	// Token: 0x0600071C RID: 1820 RVA: 0x000483C8 File Offset: 0x000465C8
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		this.timeAlive += UnityEngine.Time.fixedDeltaTime;
		if (!this.inQueue)
		{
			BaseRidableAnimal._processQueue.Enqueue(this);
			this.inQueue = true;
		}
	}

	// Token: 0x0600071D RID: 1821 RVA: 0x000483FC File Offset: 0x000465FC
	public float StaminaCoreFraction()
	{
		return Mathf.InverseLerp(0f, this.maxStaminaSeconds, this.currentMaxStaminaSeconds);
	}

	// Token: 0x0600071E RID: 1822 RVA: 0x00048414 File Offset: 0x00046614
	public void DoEatEvent()
	{
		base.ClientRPC(null, "Eat");
	}

	// Token: 0x0600071F RID: 1823 RVA: 0x00048424 File Offset: 0x00046624
	public void ReplenishFromFood(ItemModConsumable consumable)
	{
		if (consumable)
		{
			base.ClientRPC(null, "Eat");
			this.lastEatTime = UnityEngine.Time.time;
			float ifType = consumable.GetIfType(MetabolismAttribute.Type.Calories);
			float ifType2 = consumable.GetIfType(MetabolismAttribute.Type.Hydration);
			float num = consumable.GetIfType(MetabolismAttribute.Type.Health) + consumable.GetIfType(MetabolismAttribute.Type.HealthOverTime);
			this.ApplyDungCalories(ifType);
			this.ReplenishStaminaCore(ifType, ifType2);
			this.Heal(num * 4f);
		}
	}

	// Token: 0x06000720 RID: 1824 RVA: 0x0004848C File Offset: 0x0004668C
	public virtual void EatNearbyFood()
	{
		if (UnityEngine.Time.time < this.nextEatTime)
		{
			return;
		}
		float num = this.StaminaCoreFraction();
		this.nextEatTime = UnityEngine.Time.time + UnityEngine.Random.Range(2f, 3f) + Mathf.InverseLerp(0.5f, 1f, num) * 4f;
		if (num >= 1f)
		{
			return;
		}
		List<global::BaseEntity> list = Facepunch.Pool.GetList<global::BaseEntity>();
		global::Vis.Entities<global::BaseEntity>(base.transform.position + base.transform.forward * 1.5f, 2f, list, 67109377, QueryTriggerInteraction.Collide);
		list.Sort((global::BaseEntity a, global::BaseEntity b) => (b is DroppedItem).CompareTo(a is DroppedItem));
		foreach (global::BaseEntity baseEntity in list)
		{
			if (!baseEntity.isClient)
			{
				DroppedItem droppedItem = baseEntity as DroppedItem;
				if (droppedItem && droppedItem.item != null && droppedItem.item.info.category == ItemCategory.Food)
				{
					ItemModConsumable component = droppedItem.item.info.GetComponent<ItemModConsumable>();
					if (component)
					{
						this.ReplenishFromFood(component);
						droppedItem.item.UseItem(1);
						if (droppedItem.item.amount <= 0)
						{
							droppedItem.Kill(global::BaseNetworkable.DestroyMode.None);
							break;
						}
						break;
					}
				}
				CollectibleEntity collectibleEntity = baseEntity as CollectibleEntity;
				if (collectibleEntity && collectibleEntity.IsFood(false))
				{
					collectibleEntity.DoPickup(null, false);
					break;
				}
				global::GrowableEntity growableEntity = baseEntity as global::GrowableEntity;
				if (growableEntity && growableEntity.CanPick())
				{
					growableEntity.PickFruit(null, false);
					break;
				}
			}
		}
		Facepunch.Pool.FreeList<global::BaseEntity>(ref list);
	}

	// Token: 0x06000721 RID: 1825 RVA: 0x00048660 File Offset: 0x00046860
	public void SwitchMoveState(BaseRidableAnimal.RunState newState)
	{
		if (newState == this.currentRunState)
		{
			return;
		}
		this.currentRunState = newState;
		this.timeInMoveState = 0f;
		base.SetFlag(global::BaseEntity.Flags.Reserved8, this.currentRunState == BaseRidableAnimal.RunState.sprint, false, false);
		this.MarkObstacleDistanceDirty();
	}

	// Token: 0x06000722 RID: 1826 RVA: 0x0004869C File Offset: 0x0004689C
	public void UpdateOnIdealTerrain()
	{
		if (UnityEngine.Time.time < this.nextIdealTerrainCheckTime)
		{
			return;
		}
		this.nextIdealTerrainCheckTime = UnityEngine.Time.time + UnityEngine.Random.Range(1f, 2f);
		this.onIdealTerrain = false;
		if (TerrainMeta.TopologyMap != null && (TerrainMeta.TopologyMap.GetTopology(base.transform.position) & 526336) != 0)
		{
			this.onIdealTerrain = true;
		}
	}

	// Token: 0x06000723 RID: 1827 RVA: 0x0004870C File Offset: 0x0004690C
	public float MoveStateToVelocity(BaseRidableAnimal.RunState stateToCheck)
	{
		float result;
		switch (stateToCheck)
		{
		default:
			result = 0f;
			break;
		case BaseRidableAnimal.RunState.walk:
			result = this.GetWalkSpeed();
			break;
		case BaseRidableAnimal.RunState.run:
			result = this.GetTrotSpeed();
			break;
		case BaseRidableAnimal.RunState.sprint:
			result = this.GetRunSpeed();
			break;
		}
		return result;
	}

	// Token: 0x06000724 RID: 1828 RVA: 0x00048759 File Offset: 0x00046959
	public float GetDesiredVelocity()
	{
		return this.MoveStateToVelocity(this.currentRunState);
	}

	// Token: 0x06000725 RID: 1829 RVA: 0x00048767 File Offset: 0x00046967
	public BaseRidableAnimal.RunState StateFromSpeed(float speedToUse)
	{
		if (speedToUse <= this.MoveStateToVelocity(BaseRidableAnimal.RunState.stopped))
		{
			return BaseRidableAnimal.RunState.stopped;
		}
		if (speedToUse <= this.MoveStateToVelocity(BaseRidableAnimal.RunState.walk))
		{
			return BaseRidableAnimal.RunState.walk;
		}
		if (speedToUse <= this.MoveStateToVelocity(BaseRidableAnimal.RunState.run))
		{
			return BaseRidableAnimal.RunState.run;
		}
		return BaseRidableAnimal.RunState.sprint;
	}

	// Token: 0x06000726 RID: 1830 RVA: 0x00048790 File Offset: 0x00046990
	public void ModifyRunState(int dir)
	{
		if ((this.currentRunState == BaseRidableAnimal.RunState.stopped && dir < 0) || (this.currentRunState == BaseRidableAnimal.RunState.sprint && dir > 0))
		{
			return;
		}
		BaseRidableAnimal.RunState newState = this.currentRunState + dir;
		this.SwitchMoveState(newState);
	}

	// Token: 0x06000727 RID: 1831 RVA: 0x000487C8 File Offset: 0x000469C8
	public bool CanStand()
	{
		if (this.nextStandTime > UnityEngine.Time.time)
		{
			return false;
		}
		if (this.mountPoints[0].mountable == null)
		{
			return false;
		}
		bool flag = false;
		List<Collider> list = Facepunch.Pool.GetList<Collider>();
		global::Vis.Colliders<Collider>(this.mountPoints[0].mountable.eyePositionOverride.transform.position - base.transform.forward * 1f, 2f, list, 2162689, QueryTriggerInteraction.Collide);
		if (list.Count > 0)
		{
			flag = true;
		}
		Facepunch.Pool.FreeList<Collider>(ref list);
		return !flag;
	}

	// Token: 0x06000728 RID: 1832 RVA: 0x00048868 File Offset: 0x00046A68
	public void DoDebugMovement()
	{
		if (this.aiInputState == null)
		{
			this.aiInputState = new InputState();
		}
		if (!this.debugMovement)
		{
			this.aiInputState.current.buttons &= -3;
			this.aiInputState.current.buttons &= -9;
			this.aiInputState.current.buttons &= -129;
			return;
		}
		this.aiInputState.current.buttons |= 2;
		this.aiInputState.current.buttons |= 8;
		this.aiInputState.current.buttons |= 128;
		this.RiderInput(this.aiInputState, null);
	}

	// Token: 0x06000729 RID: 1833 RVA: 0x00048938 File Offset: 0x00046B38
	public virtual void RiderInput(InputState inputState, global::BasePlayer player)
	{
		float num = UnityEngine.Time.time - this.lastInputTime;
		this.lastInputTime = UnityEngine.Time.time;
		num = Mathf.Clamp(num, 0f, 1f);
		Vector3 zero = Vector3.zero;
		this.timeInMoveState += num;
		if (inputState != null)
		{
			if (inputState.IsDown(BUTTON.FORWARD))
			{
				this.lastForwardPressedTime = UnityEngine.Time.time;
				this.forwardHeldSeconds += num;
			}
			else
			{
				this.forwardHeldSeconds = 0f;
			}
			if (inputState.IsDown(BUTTON.BACKWARD))
			{
				this.lastBackwardPressedTime = UnityEngine.Time.time;
				this.backwardHeldSeconds += num;
			}
			else
			{
				this.backwardHeldSeconds = 0f;
			}
			if (inputState.IsDown(BUTTON.SPRINT))
			{
				this.lastSprintPressedTime = UnityEngine.Time.time;
				this.sprintHeldSeconds += num;
			}
			else
			{
				this.sprintHeldSeconds = 0f;
			}
			if (inputState.IsDown(BUTTON.DUCK) && this.CanStand() && (this.currentRunState == BaseRidableAnimal.RunState.stopped || (this.currentRunState == BaseRidableAnimal.RunState.walk && this.currentSpeed < 1f)))
			{
				base.ClientRPC(null, "Stand");
				this.nextStandTime = UnityEngine.Time.time + 3f;
				this.currentSpeed = 0f;
			}
			if (UnityEngine.Time.time < this.nextStandTime)
			{
				this.forwardHeldSeconds = 0f;
				this.backwardHeldSeconds = 0f;
			}
			if (this.forwardHeldSeconds > 0f)
			{
				if (this.currentRunState == BaseRidableAnimal.RunState.stopped)
				{
					this.SwitchMoveState(BaseRidableAnimal.RunState.walk);
				}
				else if (this.currentRunState == BaseRidableAnimal.RunState.walk)
				{
					if (this.sprintHeldSeconds > 0f)
					{
						this.SwitchMoveState(BaseRidableAnimal.RunState.run);
					}
				}
				else if (this.currentRunState == BaseRidableAnimal.RunState.run && this.sprintHeldSeconds > 1f && this.CanInitiateSprint())
				{
					this.SwitchMoveState(BaseRidableAnimal.RunState.sprint);
				}
			}
			else if (this.backwardHeldSeconds > 1f)
			{
				this.ModifyRunState(-1);
				this.backwardHeldSeconds = 0.1f;
			}
			else if (this.backwardHeldSeconds == 0f && this.forwardHeldSeconds == 0f && this.timeInMoveState > 1f && this.currentRunState != BaseRidableAnimal.RunState.stopped)
			{
				this.ModifyRunState(-1);
			}
			if (this.currentRunState == BaseRidableAnimal.RunState.sprint && (!this.CanSprint() || UnityEngine.Time.time - this.lastSprintPressedTime > 5f))
			{
				this.ModifyRunState(-1);
			}
			if (inputState.IsDown(BUTTON.RIGHT))
			{
				if (this.currentRunState == BaseRidableAnimal.RunState.stopped)
				{
					this.ModifyRunState(1);
				}
				this.desiredRotation = 1f;
				return;
			}
			if (inputState.IsDown(BUTTON.LEFT))
			{
				if (this.currentRunState == BaseRidableAnimal.RunState.stopped)
				{
					this.ModifyRunState(1);
				}
				this.desiredRotation = -1f;
				return;
			}
			this.desiredRotation = 0f;
		}
	}

	// Token: 0x0600072A RID: 1834 RVA: 0x00048BD9 File Offset: 0x00046DD9
	public override float MaxVelocity()
	{
		return this.maxSpeed * 1.5f;
	}

	// Token: 0x0600072B RID: 1835 RVA: 0x00048BE7 File Offset: 0x00046DE7
	private float NormalizeAngle(float angle)
	{
		if (angle > 180f)
		{
			angle -= 360f;
		}
		return angle;
	}

	// Token: 0x0600072C RID: 1836 RVA: 0x00048BFC File Offset: 0x00046DFC
	public void UpdateGroundNormal(bool force = false)
	{
		if (UnityEngine.Time.time >= this.nextGroundNormalUpdateTime || force)
		{
			this.nextGroundNormalUpdateTime = UnityEngine.Time.time + UnityEngine.Random.Range(0.2f, 0.3f);
			this.targetUp = this.averagedUp;
			Transform[] array = this.groundSampleOffsets;
			for (int i = 0; i < array.Length; i++)
			{
				Vector3 vector;
				Vector3 b;
				if (TransformUtil.GetGroundInfo(array[i].position + Vector3.up * 2f, out vector, out b, 4f, 295763969, null))
				{
					this.targetUp += b;
				}
				else
				{
					this.targetUp += Vector3.up;
				}
			}
			this.targetUp /= (float)(this.groundSampleOffsets.Length + 1);
		}
		this.averagedUp = Vector3.Lerp(this.averagedUp, this.targetUp, UnityEngine.Time.deltaTime * 2f);
	}

	// Token: 0x0600072D RID: 1837 RVA: 0x00048CFB File Offset: 0x00046EFB
	public void MarkObstacleDistanceDirty()
	{
		this.nextObstacleCheckTime = 0f;
	}

	// Token: 0x0600072E RID: 1838 RVA: 0x00048D08 File Offset: 0x00046F08
	public float GetObstacleDistance()
	{
		if (UnityEngine.Time.time >= this.nextObstacleCheckTime)
		{
			float desiredVelocity = this.GetDesiredVelocity();
			if (this.currentSpeed > 0f || desiredVelocity > 0f)
			{
				this.cachedObstacleDistance = this.ObstacleDistanceCheck(Mathf.Max(desiredVelocity, 2f));
			}
			this.nextObstacleCheckTime = UnityEngine.Time.time + UnityEngine.Random.Range(0.25f, 0.35f);
		}
		return this.cachedObstacleDistance;
	}

	// Token: 0x0600072F RID: 1839 RVA: 0x00048D78 File Offset: 0x00046F78
	public float ObstacleDistanceCheck(float speed = 10f)
	{
		Vector3 position = base.transform.position;
		float num = (float)Mathf.Max(2, Mathf.Min((int)speed, 10));
		float num2 = 0.5f;
		int num3 = Mathf.CeilToInt(num / num2);
		float num4 = 0f;
		Vector3 vector = QuaternionEx.LookRotationForcedUp(base.transform.forward, Vector3.up) * Vector3.forward;
		Vector3 vector2 = this.movementLOSOrigin.transform.position;
		vector2.y = base.transform.position.y;
		Vector3 up = base.transform.up;
		for (int i = 0; i < num3; i++)
		{
			float num5 = num2;
			bool flag = false;
			float num6 = 0f;
			Vector3 vector3 = Vector3.zero;
			Vector3 vector4 = Vector3.up;
			Vector3 a = vector2;
			Vector3 origin = a + Vector3.up * (this.maxStepHeight + this.obstacleDetectionRadius);
			Vector3 vector5 = a + vector * num5;
			float num7 = this.maxStepDownHeight + this.obstacleDetectionRadius;
			RaycastHit raycastHit;
			if (UnityEngine.Physics.SphereCast(origin, this.obstacleDetectionRadius, vector, out raycastHit, num5, 1486954753))
			{
				num6 = raycastHit.distance;
				vector3 = raycastHit.point;
				vector4 = raycastHit.normal;
				flag = true;
			}
			if (!flag)
			{
				if (!TransformUtil.GetGroundInfo(vector5 + Vector3.up * 2f, out vector3, out vector4, 2f + num7, 295763969, null))
				{
					return num4;
				}
				num6 = Vector3.Distance(a, vector3);
				if (WaterLevel.Test(vector3 + Vector3.one * this.maxWaterDepth, true, this))
				{
					vector4 = -base.transform.forward;
					return num4;
				}
				flag = true;
			}
			if (flag)
			{
				float num8 = Vector3.Angle(up, vector4);
				float num9 = Vector3.Angle(vector4, Vector3.up);
				if (num8 > this.maxWallClimbSlope || num9 > this.maxWallClimbSlope)
				{
					Vector3 vector6 = vector4;
					float num10 = vector3.y;
					int num11 = 1;
					for (int j = 0; j < this.normalOffsets.Length; j++)
					{
						Vector3 a2 = vector5 + this.normalOffsets[j].x * base.transform.right;
						float num12 = this.maxStepHeight * 2.5f;
						Vector3 vector7;
						Vector3 b;
						if (TransformUtil.GetGroundInfo(a2 + Vector3.up * num12 + this.normalOffsets[j].z * base.transform.forward, out vector7, out b, num7 + num12, 295763969, null))
						{
							num11++;
							vector6 += b;
							num10 += vector7.y;
						}
					}
					num10 /= (float)num11;
					vector6.Normalize();
					float num13 = Vector3.Angle(up, vector6);
					num9 = Vector3.Angle(vector6, Vector3.up);
					if (num13 > this.maxWallClimbSlope || num9 > this.maxWallClimbSlope || Mathf.Abs(num10 - vector5.y) > this.maxStepHeight)
					{
						return num4;
					}
				}
			}
			num4 += num6;
			vector = QuaternionEx.LookRotationForcedUp(base.transform.forward, vector4) * Vector3.forward;
			vector2 = vector3;
		}
		return num4;
	}

	// Token: 0x06000730 RID: 1840 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void MarkDistanceTravelled(float amount)
	{
	}

	// Token: 0x06000731 RID: 1841 RVA: 0x000490B4 File Offset: 0x000472B4
	public void UpdateMovement(float delta)
	{
		float num = this.WaterFactor();
		if (num > 1f && !base.IsDestroyed)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
			return;
		}
		if (this.desiredRotation != 0f)
		{
			this.MarkObstacleDistanceDirty();
		}
		if (num >= 0.3f && this.currentRunState > BaseRidableAnimal.RunState.run)
		{
			this.currentRunState = BaseRidableAnimal.RunState.run;
		}
		else if (num >= 0.45f && this.currentRunState > BaseRidableAnimal.RunState.walk)
		{
			this.currentRunState = BaseRidableAnimal.RunState.walk;
		}
		if (UnityEngine.Time.time - this.lastInputTime > 3f && !this.IsLeading())
		{
			this.currentRunState = BaseRidableAnimal.RunState.stopped;
			this.desiredRotation = 0f;
		}
		if ((base.HasDriver() && this.IsLeading()) || this.leadTarget == null)
		{
			this.SetLeading(null);
		}
		if (this.IsLeading())
		{
			Vector3 position = this.leadTarget.transform.position;
			Vector3 lhs = Vector3Ex.Direction2D(base.transform.position + base.transform.right * 1f, base.transform.position);
			Vector3 lhs2 = Vector3Ex.Direction2D(base.transform.position + base.transform.forward * 0.01f, base.transform.position);
			Vector3 rhs = Vector3Ex.Direction2D(position, base.transform.position);
			float value = Vector3.Dot(lhs, rhs);
			float num2 = Vector3.Dot(lhs2, rhs);
			bool flag = Vector3Ex.Distance2D(position, base.transform.position) > 2.5f;
			bool flag2 = Vector3Ex.Distance2D(position, base.transform.position) > 10f;
			if (flag || num2 < 0.95f)
			{
				float num3 = Mathf.InverseLerp(0f, 1f, value);
				float num4 = 1f - Mathf.InverseLerp(-1f, 0f, value);
				this.desiredRotation = 0f;
				this.desiredRotation += num3 * 1f;
				this.desiredRotation += num4 * -1f;
				if (Mathf.Abs(this.desiredRotation) < 0.001f)
				{
					this.desiredRotation = 0f;
				}
				if (flag)
				{
					this.SwitchMoveState(BaseRidableAnimal.RunState.walk);
				}
				else
				{
					this.SwitchMoveState(BaseRidableAnimal.RunState.stopped);
				}
			}
			else
			{
				this.desiredRotation = 0f;
				this.SwitchMoveState(BaseRidableAnimal.RunState.stopped);
			}
			if (flag2)
			{
				this.SetLeading(null);
				this.SwitchMoveState(BaseRidableAnimal.RunState.stopped);
			}
		}
		float obstacleDistance = this.GetObstacleDistance();
		BaseRidableAnimal.RunState runState = this.StateFromSpeed(obstacleDistance * this.GetRunSpeed());
		if (runState < this.currentRunState)
		{
			this.SwitchMoveState(runState);
		}
		float desiredVelocity = this.GetDesiredVelocity();
		Vector3 direction = Vector3.forward * Mathf.Sign(desiredVelocity);
		float num5 = Mathf.InverseLerp(0.85f, 1f, obstacleDistance);
		float num6 = Mathf.InverseLerp(1.25f, 10f, obstacleDistance);
		float num7 = 1f - Mathf.InverseLerp(20f, 45f, Vector3.Angle(Vector3.up, this.averagedUp));
		num6 = num5 * 0.1f + num6 * 0.9f;
		float num8 = Mathf.Min(Mathf.Clamp01(Mathf.Min(num7 + 0.2f, num6)) * this.GetRunSpeed(), desiredVelocity);
		float num9 = (num8 < this.currentSpeed) ? 3f : 1f;
		if (Mathf.Abs(this.currentSpeed) < 2f && desiredVelocity == 0f)
		{
			this.currentSpeed = Mathf.MoveTowards(this.currentSpeed, 0f, delta * 3f);
		}
		else
		{
			this.currentSpeed = Mathf.Lerp(this.currentSpeed, num8, delta * num9);
		}
		if (num6 == 0f)
		{
			this.currentSpeed = 0f;
		}
		float num10 = 1f - Mathf.InverseLerp(2f, 7f, this.currentSpeed);
		num10 = (num10 + 1f) / 2f;
		if (this.desiredRotation != 0f)
		{
			Vector3 position2 = this.animalFront.transform.position;
			Quaternion rotation = base.transform.rotation;
			base.transform.Rotate(Vector3.up, this.desiredRotation * delta * this.turnSpeed * num10);
			if (!this.IsLeading() && global::Vis.AnyColliders(this.animalFront.transform.position, this.obstacleDetectionRadius * 0.25f, 1503731969, QueryTriggerInteraction.Ignore))
			{
				base.transform.rotation = rotation;
			}
		}
		Vector3 a = base.transform.TransformDirection(direction);
		Vector3 normalized = a.normalized;
		float num11 = this.currentSpeed * delta;
		Vector3 a2 = base.transform.position + normalized * num11 * Mathf.Sign(this.currentSpeed);
		this.currentVelocity = a * this.currentSpeed;
		this.UpdateGroundNormal(false);
		if (this.currentSpeed > 0f || this.timeAlive < 2f || this.dropUntilTime > 0f)
		{
			base.transform.position + base.transform.InverseTransformPoint(this.animalFront.transform.position).y * base.transform.up;
			RaycastHit raycastHit;
			bool flag3 = UnityEngine.Physics.SphereCast(this.animalFront.transform.position, this.obstacleDetectionRadius, normalized, out raycastHit, num11, 1503731969);
			bool flag4 = UnityEngine.Physics.SphereCast(base.transform.position + base.transform.InverseTransformPoint(this.animalFront.transform.position).y * base.transform.up, this.obstacleDetectionRadius, normalized, out raycastHit, num11, 1503731969);
			if (!global::Vis.AnyColliders(this.animalFront.transform.position + normalized * num11, this.obstacleDetectionRadius, 1503731969, QueryTriggerInteraction.Ignore) && !flag3 && !flag4)
			{
				if (this.DropToGround(a2 + Vector3.up * this.maxStepHeight, false))
				{
					this.MarkDistanceTravelled(num11);
					return;
				}
				this.currentSpeed = 0f;
				return;
			}
			else
			{
				this.currentSpeed = 0f;
			}
		}
	}

	// Token: 0x06000732 RID: 1842 RVA: 0x000496E4 File Offset: 0x000478E4
	public bool DropToGround(Vector3 targetPos, bool force = false)
	{
		float range = force ? 10000f : (this.maxStepHeight + this.maxStepDownHeight);
		Vector3 vector;
		Vector3 vector2;
		if (!TransformUtil.GetGroundInfo(targetPos, out vector, out vector2, range, 295763969, null))
		{
			return false;
		}
		if (UnityEngine.Physics.CheckSphere(vector + Vector3.up * 1f, 0.2f, 295763969))
		{
			return false;
		}
		base.transform.position = vector;
		Vector3 eulerAngles = QuaternionEx.LookRotationForcedUp(base.transform.forward, this.averagedUp).eulerAngles;
		if (eulerAngles.z > 180f)
		{
			eulerAngles.z -= 360f;
		}
		else if (eulerAngles.z < -180f)
		{
			eulerAngles.z += 360f;
		}
		eulerAngles.z = Mathf.Clamp(eulerAngles.z, -10f, 10f);
		base.transform.rotation = Quaternion.Euler(eulerAngles);
		return true;
	}

	// Token: 0x06000733 RID: 1843 RVA: 0x000497E4 File Offset: 0x000479E4
	public virtual void DoNetworkUpdate()
	{
		bool flag = false || this.prevStamina != this.staminaSeconds || this.prevMaxStamina != this.currentMaxStaminaSeconds || this.prevRunState != (int)this.currentRunState || this.prevMaxSpeed != this.GetRunSpeed();
		this.prevStamina = this.staminaSeconds;
		this.prevMaxStamina = this.currentMaxStaminaSeconds;
		this.prevRunState = (int)this.currentRunState;
		this.prevMaxSpeed = this.GetRunSpeed();
		if (flag)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000734 RID: 1844 RVA: 0x00049883 File Offset: 0x00047A83
	public override void PreServerLoad()
	{
		base.PreServerLoad();
		this.CreateInventory(false);
	}

	// Token: 0x06000735 RID: 1845 RVA: 0x00049894 File Offset: 0x00047A94
	public override void ServerInit()
	{
		this.ContainerServerInit();
		base.ServerInit();
		base.InvokeRepeating(new Action(this.DoNetworkUpdate), UnityEngine.Random.Range(0f, 0.2f), 0.333f);
		this.SetDecayActive(true);
		if (this.debugMovement)
		{
			base.InvokeRandomized(new Action(this.DoDebugMovement), 0f, 0.1f, 0.1f);
		}
	}

	// Token: 0x06000736 RID: 1846 RVA: 0x00049904 File Offset: 0x00047B04
	public override void OnKilled(HitInfo hitInfo = null)
	{
		Assert.IsTrue(base.isServer, "OnKilled called on client!");
		BaseCorpse baseCorpse = base.DropCorpse(this.CorpsePrefab.resourcePath);
		if (baseCorpse)
		{
			this.SetupCorpse(baseCorpse);
			baseCorpse.Spawn();
			baseCorpse.TakeChildren(this);
		}
		base.Invoke(new Action(base.KillMessage), 0.5f);
		base.OnKilled(hitInfo);
	}

	// Token: 0x06000737 RID: 1847 RVA: 0x00049970 File Offset: 0x00047B70
	public virtual void SetupCorpse(BaseCorpse corpse)
	{
		corpse.flags = this.flags;
		global::LootableCorpse component = corpse.GetComponent<global::LootableCorpse>();
		if (component)
		{
			component.TakeFrom(new global::ItemContainer[]
			{
				this.inventory
			});
		}
	}

	// Token: 0x06000738 RID: 1848 RVA: 0x000499AD File Offset: 0x00047BAD
	public override Vector3 GetLocalVelocityServer()
	{
		return this.currentVelocity;
	}

	// Token: 0x06000739 RID: 1849 RVA: 0x000499B5 File Offset: 0x00047BB5
	public void UpdateDropToGroundForDuration(float duration)
	{
		this.dropUntilTime = duration;
	}

	// Token: 0x0600073A RID: 1850 RVA: 0x000499C3 File Offset: 0x00047BC3
	public bool PlayerHasToken(global::BasePlayer player)
	{
		return this.GetPurchaseToken(player) != null;
	}

	// Token: 0x0600073B RID: 1851 RVA: 0x000499D0 File Offset: 0x00047BD0
	public global::Item GetPurchaseToken(global::BasePlayer player)
	{
		int itemid = this.purchaseToken.itemid;
		return player.inventory.FindItemID(itemid);
	}

	// Token: 0x0600073C RID: 1852 RVA: 0x000499F5 File Offset: 0x00047BF5
	public virtual float GetWalkSpeed()
	{
		return this.walkSpeed;
	}

	// Token: 0x0600073D RID: 1853 RVA: 0x000499FD File Offset: 0x00047BFD
	public virtual float GetTrotSpeed()
	{
		return this.trotSpeed;
	}

	// Token: 0x0600073E RID: 1854 RVA: 0x00049A08 File Offset: 0x00047C08
	public virtual float GetRunSpeed()
	{
		if (base.isServer)
		{
			float num = this.runSpeed;
			float num2 = Mathf.InverseLerp(this.maxStaminaSeconds * 0.5f, this.maxStaminaSeconds, this.currentMaxStaminaSeconds) * this.staminaCoreSpeedBonus;
			float num3 = this.onIdealTerrain ? this.roadSpeedBonus : 0f;
			return this.runSpeed + num2 + num3;
		}
		return this.runSpeed;
	}

	// Token: 0x0600073F RID: 1855 RVA: 0x00049A70 File Offset: 0x00047C70
	public bool IsPlayerTooHeavy(global::BasePlayer player)
	{
		return player.Weight >= 10f;
	}

	// Token: 0x0400047F RID: 1151
	public ItemDefinition onlyAllowedItem;

	// Token: 0x04000480 RID: 1152
	public global::ItemContainer.ContentsType allowedContents = global::ItemContainer.ContentsType.Generic;

	// Token: 0x04000481 RID: 1153
	public int maxStackSize = 1;

	// Token: 0x04000482 RID: 1154
	public int numSlots;

	// Token: 0x04000483 RID: 1155
	public string lootPanelName = "generic";

	// Token: 0x04000484 RID: 1156
	public bool needsBuildingPrivilegeToUse;

	// Token: 0x04000485 RID: 1157
	public bool isLootable = true;

	// Token: 0x04000486 RID: 1158
	public global::ItemContainer inventory;

	// Token: 0x04000487 RID: 1159
	public const global::BaseEntity.Flags Flag_ForSale = global::BaseEntity.Flags.Reserved2;

	// Token: 0x04000488 RID: 1160
	private Vector3 lastMoveDirection;

	// Token: 0x04000489 RID: 1161
	public GameObjectRef saddlePrefab;

	// Token: 0x0400048A RID: 1162
	public EntityRef saddleRef;

	// Token: 0x0400048B RID: 1163
	public Transform movementLOSOrigin;

	// Token: 0x0400048C RID: 1164
	public SoundPlayer sprintSounds;

	// Token: 0x0400048D RID: 1165
	public SoundPlayer largeWhinny;

	// Token: 0x0400048E RID: 1166
	public const global::BaseEntity.Flags Flag_Lead = global::BaseEntity.Flags.Reserved7;

	// Token: 0x0400048F RID: 1167
	public const global::BaseEntity.Flags Flag_HasRider = global::BaseEntity.Flags.On;

	// Token: 0x04000490 RID: 1168
	[Header("Purchase")]
	public ItemDefinition purchaseToken;

	// Token: 0x04000491 RID: 1169
	public GameObjectRef eatEffect;

	// Token: 0x04000492 RID: 1170
	public GameObjectRef CorpsePrefab;

	// Token: 0x04000493 RID: 1171
	[Header("Obstacles")]
	public Transform animalFront;

	// Token: 0x04000494 RID: 1172
	public float obstacleDetectionRadius = 0.25f;

	// Token: 0x04000495 RID: 1173
	public float maxWaterDepth = 1.5f;

	// Token: 0x04000496 RID: 1174
	public float roadSpeedBonus = 2f;

	// Token: 0x04000497 RID: 1175
	public float maxWallClimbSlope = 53f;

	// Token: 0x04000498 RID: 1176
	public float maxStepHeight = 1f;

	// Token: 0x04000499 RID: 1177
	public float maxStepDownHeight = 1.35f;

	// Token: 0x0400049A RID: 1178
	[Header("Movement")]
	public BaseRidableAnimal.RunState currentRunState = BaseRidableAnimal.RunState.stopped;

	// Token: 0x0400049B RID: 1179
	public float walkSpeed = 2f;

	// Token: 0x0400049C RID: 1180
	public float trotSpeed = 7f;

	// Token: 0x0400049D RID: 1181
	public float runSpeed = 14f;

	// Token: 0x0400049E RID: 1182
	public float turnSpeed = 30f;

	// Token: 0x0400049F RID: 1183
	public float maxSpeed = 5f;

	// Token: 0x040004A0 RID: 1184
	public Transform[] groundSampleOffsets;

	// Token: 0x040004A1 RID: 1185
	[Header("Dung")]
	public ItemDefinition Dung;

	// Token: 0x040004A2 RID: 1186
	public float CaloriesToDigestPerHour = 100f;

	// Token: 0x040004A3 RID: 1187
	public float DungProducedPerCalorie = 0.001f;

	// Token: 0x040004A4 RID: 1188
	private float pendingDungCalories;

	// Token: 0x040004A5 RID: 1189
	private float dungProduction;

	// Token: 0x040004A6 RID: 1190
	protected float prevStamina;

	// Token: 0x040004A7 RID: 1191
	protected float prevMaxStamina;

	// Token: 0x040004A8 RID: 1192
	protected int prevRunState;

	// Token: 0x040004A9 RID: 1193
	protected float prevMaxSpeed;

	// Token: 0x040004AA RID: 1194
	[Header("Stamina")]
	public float staminaSeconds = 10f;

	// Token: 0x040004AB RID: 1195
	public float currentMaxStaminaSeconds = 10f;

	// Token: 0x040004AC RID: 1196
	public float maxStaminaSeconds = 20f;

	// Token: 0x040004AD RID: 1197
	public float staminaCoreLossRatio = 0.1f;

	// Token: 0x040004AE RID: 1198
	public float staminaCoreSpeedBonus = 3f;

	// Token: 0x040004AF RID: 1199
	public float staminaReplenishRatioMoving = 0.5f;

	// Token: 0x040004B0 RID: 1200
	public float staminaReplenishRatioStanding = 1f;

	// Token: 0x040004B1 RID: 1201
	public float calorieToStaminaRatio = 0.1f;

	// Token: 0x040004B2 RID: 1202
	public float hydrationToStaminaRatio = 0.5f;

	// Token: 0x040004B3 RID: 1203
	public float maxStaminaCoreFromWater = 0.5f;

	// Token: 0x040004B4 RID: 1204
	public bool debugMovement = true;

	// Token: 0x040004B5 RID: 1205
	private const float normalOffsetDist = 0.15f;

	// Token: 0x040004B6 RID: 1206
	private Vector3[] normalOffsets = new Vector3[]
	{
		new Vector3(0.15f, 0f, 0f),
		new Vector3(-0.15f, 0f, 0f),
		new Vector3(0f, 0f, 0.15f),
		new Vector3(0f, 0f, 0.3f),
		new Vector3(0f, 0f, 0.6f),
		new Vector3(0.15f, 0f, 0.3f),
		new Vector3(-0.15f, 0f, 0.3f)
	};

	// Token: 0x040004B7 RID: 1207
	[ServerVar(Help = "How long before a horse dies unattended")]
	public static float decayminutes = 180f;

	// Token: 0x040004B8 RID: 1208
	public float currentSpeed;

	// Token: 0x040004B9 RID: 1209
	public float desiredRotation;

	// Token: 0x040004BA RID: 1210
	public float animalPitchClamp = 90f;

	// Token: 0x040004BB RID: 1211
	public float animalRollClamp;

	// Token: 0x040004BC RID: 1212
	public static Queue<BaseRidableAnimal> _processQueue = new Queue<BaseRidableAnimal>();

	// Token: 0x040004BD RID: 1213
	[ServerVar]
	[Help("How many miliseconds to budget for processing ridable animals per frame")]
	public static float framebudgetms = 1f;

	// Token: 0x040004BE RID: 1214
	[ServerVar]
	[Help("Scale all ridable animal dung production rates by this value. 0 will disable dung production.")]
	public static float dungTimeScale = 1f;

	// Token: 0x040004BF RID: 1215
	private global::BaseEntity leadTarget;

	// Token: 0x040004C0 RID: 1216
	private float nextDecayTime;

	// Token: 0x040004C1 RID: 1217
	private float lastMovementUpdateTime = -1f;

	// Token: 0x040004C2 RID: 1218
	private bool inQueue;

	// Token: 0x040004C3 RID: 1219
	protected float nextEatTime;

	// Token: 0x040004C4 RID: 1220
	private float lastEatTime = float.NegativeInfinity;

	// Token: 0x040004C5 RID: 1221
	private float lastInputTime;

	// Token: 0x040004C6 RID: 1222
	private float forwardHeldSeconds;

	// Token: 0x040004C7 RID: 1223
	private float backwardHeldSeconds;

	// Token: 0x040004C8 RID: 1224
	private float sprintHeldSeconds;

	// Token: 0x040004C9 RID: 1225
	private float lastSprintPressedTime;

	// Token: 0x040004CA RID: 1226
	private float lastForwardPressedTime;

	// Token: 0x040004CB RID: 1227
	private float lastBackwardPressedTime;

	// Token: 0x040004CC RID: 1228
	private float timeInMoveState;

	// Token: 0x040004CD RID: 1229
	protected bool onIdealTerrain;

	// Token: 0x040004CE RID: 1230
	private float nextIdealTerrainCheckTime;

	// Token: 0x040004CF RID: 1231
	private float nextStandTime;

	// Token: 0x040004D0 RID: 1232
	private InputState aiInputState;

	// Token: 0x040004D1 RID: 1233
	private Vector3 currentVelocity;

	// Token: 0x040004D2 RID: 1234
	private Vector3 averagedUp = Vector3.up;

	// Token: 0x040004D3 RID: 1235
	private float nextGroundNormalUpdateTime;

	// Token: 0x040004D4 RID: 1236
	private Vector3 targetUp = Vector3.up;

	// Token: 0x040004D5 RID: 1237
	private float nextObstacleCheckTime;

	// Token: 0x040004D6 RID: 1238
	private float cachedObstacleDistance = float.PositiveInfinity;

	// Token: 0x040004D7 RID: 1239
	private const int maxObstacleCheckSpeed = 10;

	// Token: 0x040004D8 RID: 1240
	private float timeAlive;

	// Token: 0x040004D9 RID: 1241
	private TimeUntil dropUntilTime;

	// Token: 0x02000B6E RID: 2926
	public enum RunState
	{
		// Token: 0x04003E60 RID: 15968
		stopped = 1,
		// Token: 0x04003E61 RID: 15969
		walk,
		// Token: 0x04003E62 RID: 15970
		run,
		// Token: 0x04003E63 RID: 15971
		sprint,
		// Token: 0x04003E64 RID: 15972
		LAST
	}
}
