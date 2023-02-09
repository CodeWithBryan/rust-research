using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;

// Token: 0x020000BE RID: 190
public class RidableHorse : BaseRidableAnimal
{
	// Token: 0x060010CD RID: 4301 RVA: 0x00088E70 File Offset: 0x00087070
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("RidableHorse.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060010CE RID: 4302 RVA: 0x00088EB0 File Offset: 0x000870B0
	public int GetStorageSlotCount()
	{
		return this.numStorageSlots;
	}

	// Token: 0x17000166 RID: 358
	// (get) Token: 0x060010CF RID: 4303 RVA: 0x00088EB8 File Offset: 0x000870B8
	public override float RealisticMass
	{
		get
		{
			return 550f;
		}
	}

	// Token: 0x060010D0 RID: 4304 RVA: 0x00088EC0 File Offset: 0x000870C0
	public void ApplyBreed(int index)
	{
		if (this.currentBreed == index)
		{
			return;
		}
		if (index >= this.breeds.Length || index < 0)
		{
			Debug.LogError(string.Concat(new object[]
			{
				"ApplyBreed issue! index is ",
				index,
				" breed length is : ",
				this.breeds.Length
			}));
			return;
		}
		this.ApplyBreedInternal(this.breeds[index]);
		this.currentBreed = index;
	}

	// Token: 0x060010D1 RID: 4305 RVA: 0x00088F35 File Offset: 0x00087135
	protected void ApplyBreedInternal(HorseBreed breed)
	{
		if (base.isServer)
		{
			base.SetMaxHealth(this.StartHealth() * breed.maxHealth);
			base.health = this.MaxHealth();
		}
	}

	// Token: 0x060010D2 RID: 4306 RVA: 0x00088F5E File Offset: 0x0008715E
	public HorseBreed GetBreed()
	{
		if (this.currentBreed == -1 || this.currentBreed >= this.breeds.Length)
		{
			return null;
		}
		return this.breeds[this.currentBreed];
	}

	// Token: 0x060010D3 RID: 4307 RVA: 0x00088F88 File Offset: 0x00087188
	public override float GetTrotSpeed()
	{
		float num = this.equipmentSpeedMod / (base.GetRunSpeed() * this.GetBreed().maxSpeed);
		return base.GetTrotSpeed() * this.GetBreed().maxSpeed * (1f + num);
	}

	// Token: 0x060010D4 RID: 4308 RVA: 0x00088FCC File Offset: 0x000871CC
	public override float GetRunSpeed()
	{
		float runSpeed = base.GetRunSpeed();
		HorseBreed breed = this.GetBreed();
		return runSpeed * breed.maxSpeed + this.equipmentSpeedMod;
	}

	// Token: 0x060010D5 RID: 4309 RVA: 0x00088FF4 File Offset: 0x000871F4
	public override void SetupCorpse(BaseCorpse corpse)
	{
		base.SetupCorpse(corpse);
		HorseCorpse component = corpse.GetComponent<HorseCorpse>();
		if (component)
		{
			component.breedIndex = this.currentBreed;
			return;
		}
		Debug.Log("no horse corpse");
	}

	// Token: 0x060010D6 RID: 4310 RVA: 0x0008902E File Offset: 0x0008722E
	public override void ScaleDamageForPlayer(global::BasePlayer player, HitInfo info)
	{
		base.ScaleDamageForPlayer(player, info);
		this.riderProtection.Scale(info.damageTypes, 1f);
	}

	// Token: 0x060010D7 RID: 4311 RVA: 0x0008904E File Offset: 0x0008724E
	public override void OnKilled(HitInfo hitInfo = null)
	{
		this.TryLeaveHitch();
		base.OnKilled(hitInfo);
	}

	// Token: 0x060010D8 RID: 4312 RVA: 0x0008905D File Offset: 0x0008725D
	public void SetBreed(int index)
	{
		this.ApplyBreed(index);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060010D9 RID: 4313 RVA: 0x0008906D File Offset: 0x0008726D
	public override void LeadingChanged()
	{
		if (!base.IsLeading())
		{
			this.TryHitch();
		}
	}

	// Token: 0x060010DA RID: 4314 RVA: 0x00089080 File Offset: 0x00087280
	public override void ServerInit()
	{
		base.ServerInit();
		this.SetBreed(UnityEngine.Random.Range(0, this.breeds.Length));
		this.baseHorseProtection = this.baseProtection;
		this.riderProtection = ScriptableObject.CreateInstance<ProtectionProperties>();
		this.baseProtection = ScriptableObject.CreateInstance<ProtectionProperties>();
		this.baseProtection.Add(this.baseHorseProtection, 1f);
		this.EquipmentUpdate();
	}

	// Token: 0x17000167 RID: 359
	// (get) Token: 0x060010DB RID: 4315 RVA: 0x00031D65 File Offset: 0x0002FF65
	protected override float PositionTickRate
	{
		get
		{
			return 0.05f;
		}
	}

	// Token: 0x060010DC RID: 4316 RVA: 0x000890E5 File Offset: 0x000872E5
	public override void PlayerMounted(global::BasePlayer player, BaseMountable seat)
	{
		base.PlayerMounted(player, seat);
		base.InvokeRepeating(new Action(this.RecordDistance), this.distanceRecordingSpacing, this.distanceRecordingSpacing);
		this.TryLeaveHitch();
	}

	// Token: 0x060010DD RID: 4317 RVA: 0x00089113 File Offset: 0x00087313
	public override void PlayerDismounted(global::BasePlayer player, BaseMountable seat)
	{
		base.PlayerDismounted(player, seat);
		base.CancelInvoke(new Action(this.RecordDistance));
		this.TryHitch();
	}

	// Token: 0x060010DE RID: 4318 RVA: 0x00089135 File Offset: 0x00087335
	public bool IsHitched()
	{
		return this.currentHitch != null;
	}

	// Token: 0x060010DF RID: 4319 RVA: 0x00089143 File Offset: 0x00087343
	public void SetHitch(HitchTrough Hitch)
	{
		this.currentHitch = Hitch;
		base.SetFlag(global::BaseEntity.Flags.Reserved3, this.currentHitch != null, false, true);
	}

	// Token: 0x060010E0 RID: 4320 RVA: 0x000062DD File Offset: 0x000044DD
	public override float ReplenishRatio()
	{
		return 1f;
	}

	// Token: 0x060010E1 RID: 4321 RVA: 0x00089168 File Offset: 0x00087368
	public override void EatNearbyFood()
	{
		if (Time.time < this.nextEatTime)
		{
			return;
		}
		if (base.StaminaCoreFraction() >= 1f && base.healthFraction >= 1f)
		{
			return;
		}
		if (this.IsHitched())
		{
			global::Item foodItem = this.currentHitch.GetFoodItem();
			if (foodItem != null && foodItem.amount > 0)
			{
				ItemModConsumable component = foodItem.info.GetComponent<ItemModConsumable>();
				if (component)
				{
					float amount = component.GetIfType(MetabolismAttribute.Type.Calories) * this.currentHitch.caloriesToDecaySeconds;
					base.AddDecayDelay(amount);
					base.ReplenishFromFood(component);
					foodItem.UseItem(1);
					this.nextEatTime = Time.time + UnityEngine.Random.Range(2f, 3f) + Mathf.InverseLerp(0.5f, 1f, base.StaminaCoreFraction()) * 4f;
					return;
				}
			}
		}
		base.EatNearbyFood();
	}

	// Token: 0x060010E2 RID: 4322 RVA: 0x0008923C File Offset: 0x0008743C
	public void TryLeaveHitch()
	{
		if (this.currentHitch)
		{
			this.currentHitch.Unhitch(this);
		}
	}

	// Token: 0x060010E3 RID: 4323 RVA: 0x00089258 File Offset: 0x00087458
	public void TryHitch()
	{
		List<HitchTrough> list = Pool.GetList<HitchTrough>();
		Vis.Entities<HitchTrough>(base.transform.position, 2.5f, list, 256, QueryTriggerInteraction.Ignore);
		foreach (HitchTrough hitchTrough in list)
		{
			if (Vector3.Dot(Vector3Ex.Direction2D(hitchTrough.transform.position, base.transform.position), base.transform.forward) >= 0.4f && !hitchTrough.isClient && hitchTrough.HasSpace() && hitchTrough.ValidHitchPosition(base.transform.position) && hitchTrough.AttemptToHitch(this, null))
			{
				break;
			}
		}
		Pool.FreeList<HitchTrough>(ref list);
	}

	// Token: 0x060010E4 RID: 4324 RVA: 0x0008932C File Offset: 0x0008752C
	public void RecordDistance()
	{
		global::BasePlayer driver = base.GetDriver();
		if (driver == null)
		{
			this.tempDistanceTravelled = 0f;
			return;
		}
		this.kmDistance += this.tempDistanceTravelled / 1000f;
		if (this.kmDistance >= 1f)
		{
			driver.stats.Add(this.distanceStatName + "_km", 1, (global::Stats)5);
			this.kmDistance -= 1f;
		}
		driver.stats.Add(this.distanceStatName, Mathf.FloorToInt(this.tempDistanceTravelled), global::Stats.Steam);
		driver.stats.Save(false);
		this.totalDistance += this.tempDistanceTravelled;
		this.tempDistanceTravelled = 0f;
	}

	// Token: 0x060010E5 RID: 4325 RVA: 0x000893F1 File Offset: 0x000875F1
	public override void MarkDistanceTravelled(float amount)
	{
		this.tempDistanceTravelled += amount;
	}

	// Token: 0x060010E6 RID: 4326 RVA: 0x00089404 File Offset: 0x00087604
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.horse = Pool.Get<ProtoBuf.Horse>();
		info.msg.horse.staminaSeconds = this.staminaSeconds;
		info.msg.horse.currentMaxStaminaSeconds = this.currentMaxStaminaSeconds;
		info.msg.horse.breedIndex = this.currentBreed;
		info.msg.horse.numStorageSlots = this.numStorageSlots;
		if (!info.forDisk)
		{
			info.msg.horse.runState = (int)this.currentRunState;
			info.msg.horse.maxSpeed = this.GetRunSpeed();
		}
	}

	// Token: 0x060010E7 RID: 4327 RVA: 0x000894B4 File Offset: 0x000876B4
	public override void OnInventoryDirty()
	{
		this.EquipmentUpdate();
	}

	// Token: 0x060010E8 RID: 4328 RVA: 0x000894BC File Offset: 0x000876BC
	public override bool CanAnimalAcceptItem(global::Item item, int targetSlot)
	{
		ItemModAnimalEquipment component = item.info.GetComponent<ItemModAnimalEquipment>();
		if (targetSlot >= 0 && targetSlot < this.numEquipmentSlots && !component)
		{
			return false;
		}
		if (targetSlot < this.numEquipmentSlots)
		{
			if (component.slot == ItemModAnimalEquipment.SlotType.Basic)
			{
				return true;
			}
			for (int i = 0; i < this.numEquipmentSlots; i++)
			{
				global::Item slot = this.inventory.GetSlot(i);
				if (slot != null)
				{
					ItemModAnimalEquipment component2 = slot.info.GetComponent<ItemModAnimalEquipment>();
					if (!(component2 == null) && component2.slot == component.slot)
					{
						Debug.Log(string.Concat(new object[]
						{
							"rejecting because slot same, found : ",
							(int)component2.slot,
							" new : ",
							(int)component.slot
						}));
						return false;
					}
				}
			}
		}
		return true;
	}

	// Token: 0x060010E9 RID: 4329 RVA: 0x00089585 File Offset: 0x00087785
	public int GetStorageStartIndex()
	{
		return this.numEquipmentSlots;
	}

	// Token: 0x060010EA RID: 4330 RVA: 0x00089590 File Offset: 0x00087790
	public void EquipmentUpdate()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved4, false, false, false);
		base.SetFlag(global::BaseEntity.Flags.Reserved5, false, false, false);
		base.SetFlag(global::BaseEntity.Flags.Reserved6, false, false, false);
		this.riderProtection.Clear();
		this.baseProtection.Clear();
		this.equipmentSpeedMod = 0f;
		this.numStorageSlots = 0;
		for (int i = 0; i < this.numEquipmentSlots; i++)
		{
			global::Item slot = this.inventory.GetSlot(i);
			if (slot != null)
			{
				ItemModAnimalEquipment component = slot.info.GetComponent<ItemModAnimalEquipment>();
				if (component)
				{
					base.SetFlag(component.WearableFlag, true, false, false);
					if (component.hideHair)
					{
						base.SetFlag(global::BaseEntity.Flags.Reserved4, true, false, true);
					}
					if (component.riderProtection)
					{
						this.riderProtection.Add(component.riderProtection, 1f);
					}
					if (component.animalProtection)
					{
						this.baseProtection.Add(component.animalProtection, 1f);
					}
					this.equipmentSpeedMod += component.speedModifier;
					this.numStorageSlots += component.additionalInventorySlots;
				}
			}
		}
		for (int j = this.GetStorageStartIndex(); j < this.inventory.capacity; j++)
		{
			if (j >= this.GetStorageStartIndex() + this.numStorageSlots)
			{
				global::Item slot2 = this.inventory.GetSlot(j);
				if (slot2 != null)
				{
					slot2.RemoveFromContainer();
					slot2.Drop(base.transform.position + Vector3.up + UnityEngine.Random.insideUnitSphere * 0.25f, Vector3.zero, default(Quaternion));
				}
			}
		}
		this.inventory.capacity = this.GetStorageStartIndex() + this.numStorageSlots;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060010EB RID: 4331 RVA: 0x00089764 File Offset: 0x00087964
	public override void DoNetworkUpdate()
	{
		bool flag = false || this.prevStamina != this.staminaSeconds || this.prevMaxStamina != this.currentMaxStaminaSeconds || this.prevBreed != this.currentBreed || this.prevSlots != this.numStorageSlots || this.prevRunState != (int)this.currentRunState || this.prevMaxSpeed != this.GetRunSpeed();
		this.prevStamina = this.staminaSeconds;
		this.prevMaxStamina = this.currentMaxStaminaSeconds;
		this.prevRunState = (int)this.currentRunState;
		this.prevMaxSpeed = this.GetRunSpeed();
		this.prevBreed = this.currentBreed;
		this.prevSlots = this.numStorageSlots;
		if (flag)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x060010EC RID: 4332 RVA: 0x00089848 File Offset: 0x00087A48
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.horse != null)
		{
			this.staminaSeconds = info.msg.horse.staminaSeconds;
			this.currentMaxStaminaSeconds = info.msg.horse.currentMaxStaminaSeconds;
			this.numStorageSlots = info.msg.horse.numStorageSlots;
			this.ApplyBreed(info.msg.horse.breedIndex);
		}
	}

	// Token: 0x060010ED RID: 4333 RVA: 0x00007074 File Offset: 0x00005274
	protected override bool CanPushNow(global::BasePlayer pusher)
	{
		return false;
	}

	// Token: 0x060010EE RID: 4334 RVA: 0x000898C4 File Offset: 0x00087AC4
	[ServerVar]
	public static void setHorseBreed(ConsoleSystem.Arg arg)
	{
		global::BasePlayer basePlayer = arg.Player();
		if (basePlayer == null)
		{
			return;
		}
		int @int = arg.GetInt(0, 0);
		List<RidableHorse> list = Pool.GetList<RidableHorse>();
		Vis.Entities<RidableHorse>(basePlayer.eyes.position, basePlayer.eyes.position + basePlayer.eyes.HeadForward() * 5f, 0f, list, -1, QueryTriggerInteraction.Collide);
		foreach (RidableHorse ridableHorse in list)
		{
			ridableHorse.SetBreed(@int);
		}
		Pool.FreeList<RidableHorse>(ref list);
	}

	// Token: 0x04000A72 RID: 2674
	[ServerVar(Help = "Population active on the server, per square km", ShowInAdminUI = true)]
	public static float Population = 2f;

	// Token: 0x04000A73 RID: 2675
	public string distanceStatName = "";

	// Token: 0x04000A74 RID: 2676
	public HorseBreed[] breeds;

	// Token: 0x04000A75 RID: 2677
	public SkinnedMeshRenderer[] bodyRenderers;

	// Token: 0x04000A76 RID: 2678
	public SkinnedMeshRenderer[] hairRenderers;

	// Token: 0x04000A77 RID: 2679
	private int currentBreed = -1;

	// Token: 0x04000A78 RID: 2680
	private ProtectionProperties riderProtection;

	// Token: 0x04000A79 RID: 2681
	private ProtectionProperties baseHorseProtection;

	// Token: 0x04000A7A RID: 2682
	public const global::BaseEntity.Flags Flag_HideHair = global::BaseEntity.Flags.Reserved4;

	// Token: 0x04000A7B RID: 2683
	public const global::BaseEntity.Flags Flag_WoodArmor = global::BaseEntity.Flags.Reserved5;

	// Token: 0x04000A7C RID: 2684
	public const global::BaseEntity.Flags Flag_RoadsignArmor = global::BaseEntity.Flags.Reserved6;

	// Token: 0x04000A7D RID: 2685
	private float equipmentSpeedMod;

	// Token: 0x04000A7E RID: 2686
	private int numStorageSlots;

	// Token: 0x04000A7F RID: 2687
	private int prevBreed;

	// Token: 0x04000A80 RID: 2688
	private int prevSlots;

	// Token: 0x04000A81 RID: 2689
	private static Material[] breedAssignmentArray = new Material[2];

	// Token: 0x04000A82 RID: 2690
	private float distanceRecordingSpacing = 5f;

	// Token: 0x04000A83 RID: 2691
	private HitchTrough currentHitch;

	// Token: 0x04000A84 RID: 2692
	private float totalDistance;

	// Token: 0x04000A85 RID: 2693
	private float kmDistance;

	// Token: 0x04000A86 RID: 2694
	private float tempDistanceTravelled;

	// Token: 0x04000A87 RID: 2695
	private int numEquipmentSlots = 4;
}
