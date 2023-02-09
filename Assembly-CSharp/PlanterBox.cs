using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000A8 RID: 168
public class PlanterBox : StorageContainer, ISplashable
{
	// Token: 0x06000F40 RID: 3904 RVA: 0x0007EA94 File Offset: 0x0007CC94
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PlanterBox.OnRpcMessage", 0))
		{
			if (rpc == 2965786167U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_RequestSaturationUpdate ");
				}
				using (TimeWarning.New("RPC_RequestSaturationUpdate", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2965786167U, "RPC_RequestSaturationUpdate", this, player, 3f))
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
							this.RPC_RequestSaturationUpdate(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_RequestSaturationUpdate");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000F41 RID: 3905 RVA: 0x0007EBFC File Offset: 0x0007CDFC
	public override void ServerInit()
	{
		base.ServerInit();
		base.inventory.onItemAddedRemoved = new Action<global::Item, bool>(this.OnItemAddedOrRemoved);
		base.inventory.SetOnlyAllowedItem(this.allowedItem);
		global::ItemContainer inventory = base.inventory;
		inventory.canAcceptItem = (Func<global::Item, int, bool>)Delegate.Combine(inventory.canAcceptItem, new Func<global::Item, int, bool>(this.InventoryItemFilter));
		this.sunExposure = new TimeCachedValue<float>
		{
			refreshCooldown = 30f,
			refreshRandomRange = 5f,
			updateValue = new Func<float>(this.CalculateSunExposure)
		};
		this.artificialLightExposure = new TimeCachedValue<float>
		{
			refreshCooldown = 60f,
			refreshRandomRange = 5f,
			updateValue = new Func<float>(this.CalculateArtificialLightExposure)
		};
		this.plantTemperature = new TimeCachedValue<float>
		{
			refreshCooldown = 20f,
			refreshRandomRange = 5f,
			updateValue = new Func<float>(this.CalculatePlantTemperature)
		};
		this.plantArtificalTemperature = new TimeCachedValue<float>
		{
			refreshCooldown = 60f,
			refreshRandomRange = 5f,
			updateValue = new Func<float>(this.CalculateArtificialTemperature)
		};
		this.lastRainCheck = 0f;
		base.InvokeRandomized(new Action(this.CalculateRainFactor), 20f, 30f, 15f);
	}

	// Token: 0x06000F42 RID: 3906 RVA: 0x0007ED5C File Offset: 0x0007CF5C
	public override void OnItemAddedOrRemoved(global::Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
		if (added && this.ItemIsFertilizer(item))
		{
			this.FertilizeGrowables();
		}
	}

	// Token: 0x06000F43 RID: 3907 RVA: 0x0007ED78 File Offset: 0x0007CF78
	public bool InventoryItemFilter(global::Item item, int targetSlot)
	{
		return item != null && this.ItemIsFertilizer(item);
	}

	// Token: 0x06000F44 RID: 3908 RVA: 0x0007ED8B File Offset: 0x0007CF8B
	private bool ItemIsFertilizer(global::Item item)
	{
		return item.info.shortname == "fertilizer";
	}

	// Token: 0x06000F45 RID: 3909 RVA: 0x0007EDA2 File Offset: 0x0007CFA2
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.resource = Facepunch.Pool.Get<BaseResource>();
		info.msg.resource.stage = this.soilSaturation;
	}

	// Token: 0x06000F46 RID: 3910 RVA: 0x0007EDD1 File Offset: 0x0007CFD1
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.resource != null)
		{
			this.soilSaturation = info.msg.resource.stage;
		}
	}

	// Token: 0x17000156 RID: 342
	// (get) Token: 0x06000F47 RID: 3911 RVA: 0x0007EDFD File Offset: 0x0007CFFD
	public float soilSaturationFraction
	{
		get
		{
			return (float)this.soilSaturation / (float)this.soilSaturationMax;
		}
	}

	// Token: 0x17000157 RID: 343
	// (get) Token: 0x06000F48 RID: 3912 RVA: 0x0007EE0E File Offset: 0x0007D00E
	public int availableIdealWaterCapacity
	{
		get
		{
			return Mathf.Max(this.availableIdealWaterCapacity, Mathf.Max(this.idealSaturation - this.soilSaturation, 0));
		}
	}

	// Token: 0x17000158 RID: 344
	// (get) Token: 0x06000F49 RID: 3913 RVA: 0x0007EE2E File Offset: 0x0007D02E
	public int availableWaterCapacity
	{
		get
		{
			return this.soilSaturationMax - this.soilSaturation;
		}
	}

	// Token: 0x17000159 RID: 345
	// (get) Token: 0x06000F4A RID: 3914 RVA: 0x0007EE3D File Offset: 0x0007D03D
	public int idealSaturation
	{
		get
		{
			return Mathf.FloorToInt((float)this.soilSaturationMax * ConVar.Server.optimalPlanterQualitySaturation);
		}
	}

	// Token: 0x1700015A RID: 346
	// (get) Token: 0x06000F4B RID: 3915 RVA: 0x0007EE51 File Offset: 0x0007D051
	public bool BelowMinimumSaturationTriggerLevel
	{
		get
		{
			return this.soilSaturationFraction < PlanterBox.MinimumSaturationTriggerLevel;
		}
	}

	// Token: 0x1700015B RID: 347
	// (get) Token: 0x06000F4C RID: 3916 RVA: 0x0007EE60 File Offset: 0x0007D060
	public bool AboveMaximumSaturationTriggerLevel
	{
		get
		{
			return this.soilSaturationFraction > PlanterBox.MaximumSaturationTriggerLevel;
		}
	}

	// Token: 0x06000F4D RID: 3917 RVA: 0x0007EE70 File Offset: 0x0007D070
	public void FertilizeGrowables()
	{
		int num = this.GetFertilizerCount();
		if (num <= 0)
		{
			return;
		}
		foreach (global::BaseEntity baseEntity in this.children)
		{
			if (!(baseEntity == null))
			{
				global::GrowableEntity growableEntity = baseEntity as global::GrowableEntity;
				if (!(growableEntity == null) && !growableEntity.Fertilized && this.ConsumeFertilizer())
				{
					growableEntity.Fertilize();
					num--;
					if (num == 0)
					{
						break;
					}
				}
			}
		}
	}

	// Token: 0x06000F4E RID: 3918 RVA: 0x0007EF00 File Offset: 0x0007D100
	public int GetFertilizerCount()
	{
		int num = 0;
		for (int i = 0; i < base.inventory.capacity; i++)
		{
			global::Item slot = base.inventory.GetSlot(i);
			if (slot != null && this.ItemIsFertilizer(slot))
			{
				num += slot.amount;
			}
		}
		return num;
	}

	// Token: 0x06000F4F RID: 3919 RVA: 0x0007EF48 File Offset: 0x0007D148
	public bool ConsumeFertilizer()
	{
		for (int i = 0; i < base.inventory.capacity; i++)
		{
			global::Item slot = base.inventory.GetSlot(i);
			if (slot != null && this.ItemIsFertilizer(slot))
			{
				int num = Mathf.Min(1, slot.amount);
				if (num > 0)
				{
					slot.UseItem(num);
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000F50 RID: 3920 RVA: 0x0007EFA0 File Offset: 0x0007D1A0
	public int ConsumeWater(int amount, global::GrowableEntity ignoreEntity = null)
	{
		int num = Mathf.Min(amount, this.soilSaturation);
		this.soilSaturation -= num;
		this.RefreshGrowables(ignoreEntity);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		return num;
	}

	// Token: 0x06000F51 RID: 3921 RVA: 0x0007EFD8 File Offset: 0x0007D1D8
	public bool WantsSplash(ItemDefinition splashType, int amount)
	{
		return !base.IsDestroyed && !(splashType == null) && splashType.shortname != null && (splashType.shortname == "water.salt" || this.soilSaturation < this.soilSaturationMax);
	}

	// Token: 0x06000F52 RID: 3922 RVA: 0x0007F024 File Offset: 0x0007D224
	public int DoSplash(ItemDefinition splashType, int amount)
	{
		if (splashType.shortname == "water.salt")
		{
			this.soilSaturation = 0;
			this.RefreshGrowables(null);
			if (this.lastSplashNetworkUpdate > 60f)
			{
				base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
				this.lastSplashNetworkUpdate = 0f;
			}
			return amount;
		}
		int num = Mathf.Min(this.availableWaterCapacity, amount);
		this.soilSaturation += num;
		this.RefreshGrowables(null);
		if (this.lastSplashNetworkUpdate > 60f)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			this.lastSplashNetworkUpdate = 0f;
		}
		return num;
	}

	// Token: 0x06000F53 RID: 3923 RVA: 0x0007F0C8 File Offset: 0x0007D2C8
	private void RefreshGrowables(global::GrowableEntity ignoreEntity = null)
	{
		if (this.children == null)
		{
			return;
		}
		foreach (global::BaseEntity baseEntity in this.children)
		{
			global::GrowableEntity growableEntity;
			if (!(baseEntity == null) && !(baseEntity == ignoreEntity) && (growableEntity = (baseEntity as global::GrowableEntity)) != null)
			{
				growableEntity.QueueForQualityUpdate();
			}
		}
	}

	// Token: 0x06000F54 RID: 3924 RVA: 0x0007F140 File Offset: 0x0007D340
	public void ForceLightUpdate()
	{
		TimeCachedValue<float> timeCachedValue = this.sunExposure;
		if (timeCachedValue != null)
		{
			timeCachedValue.ForceNextRun();
		}
		TimeCachedValue<float> timeCachedValue2 = this.artificialLightExposure;
		if (timeCachedValue2 == null)
		{
			return;
		}
		timeCachedValue2.ForceNextRun();
	}

	// Token: 0x06000F55 RID: 3925 RVA: 0x0007F163 File Offset: 0x0007D363
	public void ForceTemperatureUpdate()
	{
		TimeCachedValue<float> timeCachedValue = this.plantArtificalTemperature;
		if (timeCachedValue == null)
		{
			return;
		}
		timeCachedValue.ForceNextRun();
	}

	// Token: 0x06000F56 RID: 3926 RVA: 0x0007F175 File Offset: 0x0007D375
	public float GetSunExposure()
	{
		TimeCachedValue<float> timeCachedValue = this.sunExposure;
		if (timeCachedValue == null)
		{
			return 0f;
		}
		return timeCachedValue.Get(false);
	}

	// Token: 0x06000F57 RID: 3927 RVA: 0x00062FFF File Offset: 0x000611FF
	private float CalculateSunExposure()
	{
		return global::GrowableEntity.SunRaycast(base.transform.position + new Vector3(0f, 1f, 0f));
	}

	// Token: 0x06000F58 RID: 3928 RVA: 0x0007F18D File Offset: 0x0007D38D
	public float GetArtificialLightExposure()
	{
		TimeCachedValue<float> timeCachedValue = this.artificialLightExposure;
		if (timeCachedValue == null)
		{
			return 0f;
		}
		return timeCachedValue.Get(false);
	}

	// Token: 0x06000F59 RID: 3929 RVA: 0x0006305C File Offset: 0x0006125C
	private float CalculateArtificialLightExposure()
	{
		return global::GrowableEntity.CalculateArtificialLightExposure(base.transform);
	}

	// Token: 0x06000F5A RID: 3930 RVA: 0x0007F1A5 File Offset: 0x0007D3A5
	public float GetPlantTemperature()
	{
		TimeCachedValue<float> timeCachedValue = this.plantTemperature;
		float num = (timeCachedValue != null) ? timeCachedValue.Get(false) : 0f;
		TimeCachedValue<float> timeCachedValue2 = this.plantArtificalTemperature;
		return num + ((timeCachedValue2 != null) ? timeCachedValue2.Get(false) : 0f);
	}

	// Token: 0x06000F5B RID: 3931 RVA: 0x0007F1D6 File Offset: 0x0007D3D6
	private float CalculatePlantTemperature()
	{
		return Mathf.Max(Climate.GetTemperature(base.transform.position), 15f);
	}

	// Token: 0x06000F5C RID: 3932 RVA: 0x0007F1F4 File Offset: 0x0007D3F4
	private void CalculateRainFactor()
	{
		if (this.sunExposure.Get(false) > 0f)
		{
			float rain = Climate.GetRain(base.transform.position);
			if (rain > 0f)
			{
				this.soilSaturation = Mathf.Clamp(this.soilSaturation + Mathf.RoundToInt(4f * rain * this.lastRainCheck), 0, this.soilSaturationMax);
				this.RefreshGrowables(null);
				base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
		}
		this.lastRainCheck = 0f;
	}

	// Token: 0x06000F5D RID: 3933 RVA: 0x0006353F File Offset: 0x0006173F
	private float CalculateArtificialTemperature()
	{
		return global::GrowableEntity.CalculateArtificialTemperature(base.transform);
	}

	// Token: 0x06000F5E RID: 3934 RVA: 0x0007F27C File Offset: 0x0007D47C
	public void OnPlantInserted(global::GrowableEntity entity, global::BasePlayer byPlayer)
	{
		if (!GameInfo.HasAchievements)
		{
			return;
		}
		List<uint> list = Facepunch.Pool.GetList<uint>();
		using (List<global::BaseEntity>.Enumerator enumerator = this.children.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				global::GrowableEntity growableEntity;
				if ((growableEntity = (enumerator.Current as global::GrowableEntity)) != null && !list.Contains(growableEntity.prefabID))
				{
					list.Add(growableEntity.prefabID);
				}
			}
		}
		if (list.Count == 9)
		{
			byPlayer.GiveAchievement("HONEST_WORK");
		}
		Facepunch.Pool.FreeList<uint>(ref list);
	}

	// Token: 0x06000F5F RID: 3935 RVA: 0x0007F314 File Offset: 0x0007D514
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_RequestSaturationUpdate(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player != null)
		{
			base.ClientRPCPlayer<int>(null, msg.player, "RPC_ReceiveSaturationUpdate", this.soilSaturation);
		}
	}

	// Token: 0x06000F60 RID: 3936 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool SupportsChildDeployables()
	{
		return true;
	}

	// Token: 0x040009DC RID: 2524
	public int soilSaturation;

	// Token: 0x040009DD RID: 2525
	public int soilSaturationMax = 8000;

	// Token: 0x040009DE RID: 2526
	public MeshRenderer soilRenderer;

	// Token: 0x040009DF RID: 2527
	private static readonly float MinimumSaturationTriggerLevel = ConVar.Server.optimalPlanterQualitySaturation - 0.2f;

	// Token: 0x040009E0 RID: 2528
	private static readonly float MaximumSaturationTriggerLevel = ConVar.Server.optimalPlanterQualitySaturation + 0.1f;

	// Token: 0x040009E1 RID: 2529
	private TimeCachedValue<float> sunExposure;

	// Token: 0x040009E2 RID: 2530
	private TimeCachedValue<float> artificialLightExposure;

	// Token: 0x040009E3 RID: 2531
	private TimeCachedValue<float> plantTemperature;

	// Token: 0x040009E4 RID: 2532
	private TimeCachedValue<float> plantArtificalTemperature;

	// Token: 0x040009E5 RID: 2533
	private TimeSince lastSplashNetworkUpdate;

	// Token: 0x040009E6 RID: 2534
	private TimeSince lastRainCheck;
}
