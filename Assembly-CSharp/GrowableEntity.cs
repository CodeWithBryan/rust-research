using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000077 RID: 119
public class GrowableEntity : BaseCombatEntity, IInstanceDataReceiver
{
	// Token: 0x06000B22 RID: 2850 RVA: 0x00062274 File Offset: 0x00060474
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("GrowableEntity.OnRpcMessage", 0))
		{
			if (rpc == 759768385U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_EatFruit ");
				}
				using (TimeWarning.New("RPC_EatFruit", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(759768385U, "RPC_EatFruit", this, player, 3f))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(759768385U, "RPC_EatFruit", this, player, 3f))
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
							this.RPC_EatFruit(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_EatFruit");
					}
				}
				return true;
			}
			if (rpc == 598660365U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_PickFruit ");
				}
				using (TimeWarning.New("RPC_PickFruit", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(598660365U, "RPC_PickFruit", this, player, 3f))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(598660365U, "RPC_PickFruit", this, player, 3f))
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
							this.RPC_PickFruit(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_PickFruit");
					}
				}
				return true;
			}
			if (rpc == 3465633431U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_PickFruitAll ");
				}
				using (TimeWarning.New("RPC_PickFruitAll", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3465633431U, "RPC_PickFruitAll", this, player, 3f))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3465633431U, "RPC_PickFruitAll", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg4 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_PickFruitAll(msg4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in RPC_PickFruitAll");
					}
				}
				return true;
			}
			if (rpc == 1959480148U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_RemoveDying ");
				}
				using (TimeWarning.New("RPC_RemoveDying", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1959480148U, "RPC_RemoveDying", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg5 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_RemoveDying(msg5);
						}
					}
					catch (Exception exception4)
					{
						Debug.LogException(exception4);
						player.Kick("RPC Error in RPC_RemoveDying");
					}
				}
				return true;
			}
			if (rpc == 1771718099U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_RemoveDyingAll ");
				}
				using (TimeWarning.New("RPC_RemoveDyingAll", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1771718099U, "RPC_RemoveDyingAll", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg6 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_RemoveDyingAll(msg6);
						}
					}
					catch (Exception exception5)
					{
						Debug.LogException(exception5);
						player.Kick("RPC Error in RPC_RemoveDyingAll");
					}
				}
				return true;
			}
			if (rpc == 232075937U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_RequestQualityUpdate ");
				}
				using (TimeWarning.New("RPC_RequestQualityUpdate", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(232075937U, "RPC_RequestQualityUpdate", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg7 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_RequestQualityUpdate(msg7);
						}
					}
					catch (Exception exception6)
					{
						Debug.LogException(exception6);
						player.Kick("RPC Error in RPC_RequestQualityUpdate");
					}
				}
				return true;
			}
			if (rpc == 2222960834U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_TakeClone ");
				}
				using (TimeWarning.New("RPC_TakeClone", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2222960834U, "RPC_TakeClone", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg8 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_TakeClone(msg8);
						}
					}
					catch (Exception exception7)
					{
						Debug.LogException(exception7);
						player.Kick("RPC Error in RPC_TakeClone");
					}
				}
				return true;
			}
			if (rpc == 95639240U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_TakeCloneAll ");
				}
				using (TimeWarning.New("RPC_TakeCloneAll", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(95639240U, "RPC_TakeCloneAll", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg9 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_TakeCloneAll(msg9);
						}
					}
					catch (Exception exception8)
					{
						Debug.LogException(exception8);
						player.Kick("RPC Error in RPC_TakeCloneAll");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000B23 RID: 2851 RVA: 0x00062DF4 File Offset: 0x00060FF4
	public void QueueForQualityUpdate()
	{
		global::GrowableEntity.growableEntityUpdateQueue.Add(this);
	}

	// Token: 0x06000B24 RID: 2852 RVA: 0x00062E04 File Offset: 0x00061004
	public void CalculateQualities(bool firstTime, bool forceArtificialLightUpdates = false, bool forceArtificialTemperatureUpdates = false)
	{
		if (this.IsDead())
		{
			return;
		}
		if (this.sunExposure == null)
		{
			this.sunExposure = new TimeCachedValue<float>
			{
				refreshCooldown = 30f,
				refreshRandomRange = 5f,
				updateValue = new Func<float>(this.SunRaycast)
			};
		}
		if (this.artificialLightExposure == null)
		{
			this.artificialLightExposure = new TimeCachedValue<float>
			{
				refreshCooldown = 60f,
				refreshRandomRange = 5f,
				updateValue = new Func<float>(this.CalculateArtificialLightExposure)
			};
		}
		if (this.artificialTemperatureExposure == null)
		{
			this.artificialTemperatureExposure = new TimeCachedValue<float>
			{
				refreshCooldown = 60f,
				refreshRandomRange = 5f,
				updateValue = new Func<float>(this.CalculateArtificialTemperature)
			};
		}
		if (forceArtificialTemperatureUpdates)
		{
			this.artificialTemperatureExposure.ForceNextRun();
		}
		this.CalculateLightQuality(forceArtificialLightUpdates || firstTime);
		this.CalculateWaterQuality();
		this.CalculateWaterConsumption();
		this.CalculateGroundQuality(firstTime);
		this.CalculateTemperatureQuality();
		this.CalculateOverallQuality();
	}

	// Token: 0x06000B25 RID: 2853 RVA: 0x00062F02 File Offset: 0x00061102
	private void CalculateQualities_Water()
	{
		this.CalculateWaterQuality();
		this.CalculateWaterConsumption();
		this.CalculateOverallQuality();
	}

	// Token: 0x06000B26 RID: 2854 RVA: 0x00062F18 File Offset: 0x00061118
	public void CalculateLightQuality(bool forceArtificalUpdate)
	{
		float num = Mathf.Clamp01(this.Properties.timeOfDayHappiness.Evaluate(TOD_Sky.Instance.Cycle.Hour));
		if (!ConVar.Server.plantlightdetection)
		{
			this.LightQuality = num;
			return;
		}
		this.LightQuality = this.CalculateSunExposure(forceArtificalUpdate) * num;
		if (this.LightQuality <= 0f)
		{
			this.LightQuality = this.GetArtificialLightExposure(forceArtificalUpdate);
		}
		this.LightQuality = global::GrowableEntity.RemapValue(this.LightQuality, 0f, this.Properties.OptimalLightQuality, 0f, 1f);
	}

	// Token: 0x06000B27 RID: 2855 RVA: 0x00062FB0 File Offset: 0x000611B0
	private float CalculateSunExposure(bool force)
	{
		if (TOD_Sky.Instance.IsNight)
		{
			return 0f;
		}
		if (this.GetPlanter() != null)
		{
			return this.GetPlanter().GetSunExposure();
		}
		TimeCachedValue<float> timeCachedValue = this.sunExposure;
		if (timeCachedValue == null)
		{
			return 0f;
		}
		return timeCachedValue.Get(force);
	}

	// Token: 0x06000B28 RID: 2856 RVA: 0x00062FFF File Offset: 0x000611FF
	private float SunRaycast()
	{
		return global::GrowableEntity.SunRaycast(base.transform.position + new Vector3(0f, 1f, 0f));
	}

	// Token: 0x06000B29 RID: 2857 RVA: 0x0006302A File Offset: 0x0006122A
	private float GetArtificialLightExposure(bool force)
	{
		if (this.GetPlanter() != null)
		{
			return this.GetPlanter().GetArtificialLightExposure();
		}
		TimeCachedValue<float> timeCachedValue = this.artificialLightExposure;
		if (timeCachedValue == null)
		{
			return 0f;
		}
		return timeCachedValue.Get(force);
	}

	// Token: 0x06000B2A RID: 2858 RVA: 0x0006305C File Offset: 0x0006125C
	private float CalculateArtificialLightExposure()
	{
		return global::GrowableEntity.CalculateArtificialLightExposure(base.transform);
	}

	// Token: 0x06000B2B RID: 2859 RVA: 0x0006306C File Offset: 0x0006126C
	public static float CalculateArtificialLightExposure(Transform forTransform)
	{
		float result = 0f;
		List<CeilingLight> list = Facepunch.Pool.GetList<CeilingLight>();
		global::Vis.Entities<CeilingLight>(forTransform.position + new Vector3(0f, ConVar.Server.ceilingLightHeightOffset, 0f), ConVar.Server.ceilingLightGrowableRange, list, 256, QueryTriggerInteraction.Collide);
		using (List<CeilingLight>.Enumerator enumerator = list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.IsOn())
				{
					result = 1f;
					break;
				}
			}
		}
		Facepunch.Pool.FreeList<CeilingLight>(ref list);
		return result;
	}

	// Token: 0x06000B2C RID: 2860 RVA: 0x00063108 File Offset: 0x00061308
	public static float SunRaycast(Vector3 checkPosition)
	{
		Vector3 normalized = (TOD_Sky.Instance.Components.Sun.transform.position - checkPosition).normalized;
		RaycastHit raycastHit;
		if (!UnityEngine.Physics.Raycast(checkPosition, normalized, out raycastHit, 100f, 10551297))
		{
			return 1f;
		}
		return 0f;
	}

	// Token: 0x06000B2D RID: 2861 RVA: 0x00063160 File Offset: 0x00061360
	public void CalculateWaterQuality()
	{
		if (this.GetPlanter() != null)
		{
			float soilSaturationFraction = this.planter.soilSaturationFraction;
			if (soilSaturationFraction > ConVar.Server.optimalPlanterQualitySaturation)
			{
				this.WaterQuality = global::GrowableEntity.RemapValue(soilSaturationFraction, ConVar.Server.optimalPlanterQualitySaturation, 1f, 1f, 0.6f);
			}
			else
			{
				this.WaterQuality = global::GrowableEntity.RemapValue(soilSaturationFraction, 0f, ConVar.Server.optimalPlanterQualitySaturation, 0f, 1f);
			}
		}
		else
		{
			TerrainBiome.Enum biomeMaxType = (TerrainBiome.Enum)TerrainMeta.BiomeMap.GetBiomeMaxType(base.transform.position, -1);
			if (biomeMaxType - TerrainBiome.Enum.Arid > 1 && biomeMaxType != TerrainBiome.Enum.Tundra)
			{
				if (biomeMaxType == TerrainBiome.Enum.Arctic)
				{
					this.WaterQuality = 0.1f;
				}
				else
				{
					this.WaterQuality = 0f;
				}
			}
			else
			{
				this.WaterQuality = 0.3f;
			}
		}
		this.WaterQuality = Mathf.Clamp01(this.WaterQuality);
		this.WaterQuality = global::GrowableEntity.RemapValue(this.WaterQuality, 0f, this.Properties.OptimalWaterQuality, 0f, 1f);
	}

	// Token: 0x06000B2E RID: 2862 RVA: 0x0006325C File Offset: 0x0006145C
	public void CalculateGroundQuality(bool firstCheck)
	{
		if (this.underWater && !firstCheck)
		{
			this.GroundQuality = 0f;
			return;
		}
		if (firstCheck)
		{
			Vector3 position = base.transform.position;
			if (WaterLevel.Test(position, true, this))
			{
				this.underWater = true;
				this.GroundQuality = 0f;
				return;
			}
			this.underWater = false;
			this.terrainTypeValue = this.GetGroundTypeValue(position);
		}
		if (this.GetPlanter() != null)
		{
			this.GroundQuality = 0.6f;
			this.GroundQuality += (this.Fertilized ? 0.4f : 0f);
		}
		else
		{
			this.GroundQuality = this.terrainTypeValue;
			float num = (float)this.Genes.GetGeneTypeCount(GrowableGenetics.GeneType.Hardiness) * 0.2f;
			float b = this.GroundQuality + num;
			this.GroundQuality = Mathf.Min(0.6f, b);
		}
		this.GroundQuality = global::GrowableEntity.RemapValue(this.GroundQuality, 0f, this.Properties.OptimalGroundQuality, 0f, 1f);
	}

	// Token: 0x06000B2F RID: 2863 RVA: 0x00063364 File Offset: 0x00061564
	private float GetGroundTypeValue(Vector3 pos)
	{
		TerrainSplat.Enum splatMaxType = (TerrainSplat.Enum)TerrainMeta.SplatMap.GetSplatMaxType(pos, -1);
		if (splatMaxType <= TerrainSplat.Enum.Grass)
		{
			switch (splatMaxType)
			{
			case TerrainSplat.Enum.Dirt:
				return 0.3f;
			case TerrainSplat.Enum.Snow:
				return 0f;
			case (TerrainSplat.Enum)3:
				break;
			case TerrainSplat.Enum.Sand:
				return 0f;
			default:
				if (splatMaxType == TerrainSplat.Enum.Rock)
				{
					return 0f;
				}
				if (splatMaxType == TerrainSplat.Enum.Grass)
				{
					return 0.3f;
				}
				break;
			}
		}
		else
		{
			if (splatMaxType == TerrainSplat.Enum.Forest)
			{
				return 0.2f;
			}
			if (splatMaxType == TerrainSplat.Enum.Stones)
			{
				return 0f;
			}
			if (splatMaxType == TerrainSplat.Enum.Gravel)
			{
				return 0f;
			}
		}
		return 0.5f;
	}

	// Token: 0x06000B30 RID: 2864 RVA: 0x000633F0 File Offset: 0x000615F0
	private void CalculateTemperatureQuality()
	{
		this.TemperatureQuality = Mathf.Clamp01(this.Properties.temperatureHappiness.Evaluate(this.CurrentTemperature));
		float num = (float)this.Genes.GetGeneTypeCount(GrowableGenetics.GeneType.Hardiness) * 0.05f;
		this.TemperatureQuality = Mathf.Clamp01(this.TemperatureQuality + num);
		this.TemperatureQuality = global::GrowableEntity.RemapValue(this.TemperatureQuality, 0f, this.Properties.OptimalTemperatureQuality, 0f, 1f);
	}

	// Token: 0x06000B31 RID: 2865 RVA: 0x00063470 File Offset: 0x00061670
	public float CalculateOverallQuality()
	{
		float num = 1f;
		if (ConVar.Server.useMinimumPlantCondition)
		{
			num = Mathf.Min(num, this.LightQuality);
			num = Mathf.Min(num, this.WaterQuality);
			num = Mathf.Min(num, this.GroundQuality);
			num = Mathf.Min(num, this.TemperatureQuality);
		}
		else
		{
			num = this.LightQuality * this.WaterQuality * this.GroundQuality * this.TemperatureQuality;
		}
		this.OverallQuality = num;
		return this.OverallQuality;
	}

	// Token: 0x06000B32 RID: 2866 RVA: 0x000634EC File Offset: 0x000616EC
	public void CalculateWaterConsumption()
	{
		float num = this.Properties.temperatureWaterRequirementMultiplier.Evaluate(this.CurrentTemperature);
		float num2 = 1f + (float)this.Genes.GetGeneTypeCount(GrowableGenetics.GeneType.WaterRequirement) * 0.1f;
		this.WaterConsumption = this.Properties.WaterIntake * num * num2;
	}

	// Token: 0x06000B33 RID: 2867 RVA: 0x0006353F File Offset: 0x0006173F
	private float CalculateArtificialTemperature()
	{
		return global::GrowableEntity.CalculateArtificialTemperature(base.transform);
	}

	// Token: 0x06000B34 RID: 2868 RVA: 0x0006354C File Offset: 0x0006174C
	public static float CalculateArtificialTemperature(Transform forTransform)
	{
		Vector3 position = forTransform.position;
		List<GrowableHeatSource> list = Facepunch.Pool.GetList<GrowableHeatSource>();
		global::Vis.Components<GrowableHeatSource>(position, ConVar.Server.artificialTemperatureGrowableRange, list, 256, QueryTriggerInteraction.Collide);
		float num = 0f;
		foreach (GrowableHeatSource growableHeatSource in list)
		{
			num = Mathf.Max(growableHeatSource.ApplyHeat(position), num);
		}
		Facepunch.Pool.FreeList<GrowableHeatSource>(ref list);
		return num;
	}

	// Token: 0x06000B35 RID: 2869 RVA: 0x000635CC File Offset: 0x000617CC
	public int CalculateMarketValue()
	{
		int num = this.Properties.BaseMarketValue;
		int num2 = this.Genes.GetPositiveGeneCount() * 10;
		int num3 = this.Genes.GetNegativeGeneCount() * -10;
		num += num2;
		num += num3;
		return Mathf.Max(0, num);
	}

	// Token: 0x06000B36 RID: 2870 RVA: 0x00063614 File Offset: 0x00061814
	private static float RemapValue(float inValue, float minA, float maxA, float minB, float maxB)
	{
		if (inValue >= maxA)
		{
			return maxB;
		}
		float t = Mathf.InverseLerp(minA, maxA, inValue);
		return Mathf.Lerp(minB, maxB, t);
	}

	// Token: 0x06000B37 RID: 2871 RVA: 0x0006363A File Offset: 0x0006183A
	public bool IsFood()
	{
		return this.Properties.pickupItem.category == ItemCategory.Food && this.Properties.pickupItem.GetComponent<ItemModConsume>() != null;
	}

	// Token: 0x170000EC RID: 236
	// (get) Token: 0x06000B38 RID: 2872 RVA: 0x00063668 File Offset: 0x00061868
	public float CurrentTemperature
	{
		get
		{
			if (this.GetPlanter() != null)
			{
				return this.GetPlanter().GetPlantTemperature();
			}
			float temperature = Climate.GetTemperature(base.transform.position);
			TimeCachedValue<float> timeCachedValue = this.artificialTemperatureExposure;
			return temperature + ((timeCachedValue != null) ? timeCachedValue.Get(false) : 0f);
		}
	}

	// Token: 0x06000B39 RID: 2873 RVA: 0x000636B8 File Offset: 0x000618B8
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRandomized(new Action(this.RunUpdate), global::GrowableEntity.ThinkDeltaTime, global::GrowableEntity.ThinkDeltaTime, global::GrowableEntity.ThinkDeltaTime * 0.1f);
		base.health = 10f;
		this.ResetSeason();
		this.Genes.GenerateRandom(this);
		if (!Rust.Application.isLoadingSave)
		{
			this.CalculateQualities(true, false, false);
		}
	}

	// Token: 0x06000B3A RID: 2874 RVA: 0x00063720 File Offset: 0x00061920
	public PlanterBox GetPlanter()
	{
		if (this.planter == null)
		{
			global::BaseEntity parentEntity = base.GetParentEntity();
			if (parentEntity != null)
			{
				this.planter = (parentEntity as PlanterBox);
			}
		}
		return this.planter;
	}

	// Token: 0x06000B3B RID: 2875 RVA: 0x0006375D File Offset: 0x0006195D
	public override void OnParentChanging(global::BaseEntity oldParent, global::BaseEntity newParent)
	{
		base.OnParentChanging(oldParent, newParent);
		this.planter = (newParent as PlanterBox);
		if (!Rust.Application.isLoadingSave && this.planter != null)
		{
			this.planter.FertilizeGrowables();
		}
		this.CalculateQualities(true, false, false);
	}

	// Token: 0x06000B3C RID: 2876 RVA: 0x0006379C File Offset: 0x0006199C
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.CalculateQualities(true, false, false);
	}

	// Token: 0x06000B3D RID: 2877 RVA: 0x000637AD File Offset: 0x000619AD
	public void ResetSeason()
	{
		this.Yield = 0f;
		this.yieldPool = 0f;
	}

	// Token: 0x06000B3E RID: 2878 RVA: 0x000637C8 File Offset: 0x000619C8
	private void RunUpdate()
	{
		if (this.IsDead())
		{
			return;
		}
		this.CalculateQualities(false, false, false);
		float overallQuality = this.CalculateOverallQuality();
		float actualStageAgeIncrease = this.UpdateAge(overallQuality);
		this.UpdateHealthAndYield(overallQuality, actualStageAgeIncrease);
		if (base.health <= 0f)
		{
			this.Die(null);
			return;
		}
		this.UpdateState();
		this.ConsumeWater();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000B3F RID: 2879 RVA: 0x00063828 File Offset: 0x00061A28
	private float UpdateAge(float overallQuality)
	{
		this.Age += this.growDeltaTime;
		float num = this.currentStage.IgnoreConditions ? 1f : (Mathf.Max(overallQuality, 0f) * this.GetGrowthBonus(overallQuality));
		float num2 = this.growDeltaTime * num;
		this.stageAge += num2;
		return num2;
	}

	// Token: 0x06000B40 RID: 2880 RVA: 0x00063888 File Offset: 0x00061A88
	private void UpdateHealthAndYield(float overallQuality, float actualStageAgeIncrease)
	{
		if (this.GetPlanter() == null && UnityEngine.Random.Range(0f, 1f) <= ConVar.Server.nonPlanterDeathChancePerTick)
		{
			base.health = 0f;
			return;
		}
		if (overallQuality <= 0f)
		{
			this.ApplyDeathRate();
		}
		base.health += overallQuality * this.currentStage.health * this.growDeltaTime;
		if (this.yieldPool > 0f)
		{
			float num = this.currentStage.yield / (this.currentStage.lifeLengthSeconds / this.growDeltaTime);
			float num2 = Mathf.Min(this.yieldPool, num * (actualStageAgeIncrease / this.growDeltaTime));
			this.yieldPool -= num;
			float num3 = 1f + (float)this.Genes.GetGeneTypeCount(GrowableGenetics.GeneType.Yield) * 0.25f;
			this.Yield += num2 * 1f * num3;
		}
	}

	// Token: 0x06000B41 RID: 2881 RVA: 0x00063978 File Offset: 0x00061B78
	private void ApplyDeathRate()
	{
		float num = 0f;
		if (this.WaterQuality <= 0f)
		{
			num += 0.1f;
		}
		if (this.LightQuality <= 0f)
		{
			num += 0.1f;
		}
		if (this.GroundQuality <= 0f)
		{
			num += 0.1f;
		}
		if (this.TemperatureQuality <= 0f)
		{
			num += 0.1f;
		}
		base.health -= num;
	}

	// Token: 0x06000B42 RID: 2882 RVA: 0x000639F0 File Offset: 0x00061BF0
	private float GetGrowthBonus(float overallQuality)
	{
		float result = 1f + (float)this.Genes.GetGeneTypeCount(GrowableGenetics.GeneType.GrowthSpeed) * 0.25f;
		if (overallQuality <= 0f)
		{
			result = 1f;
		}
		return result;
	}

	// Token: 0x06000B43 RID: 2883 RVA: 0x00063A28 File Offset: 0x00061C28
	private PlantProperties.State UpdateState()
	{
		if (this.stageAge <= this.currentStage.lifeLengthSeconds)
		{
			return this.State;
		}
		if (this.State == PlantProperties.State.Dying)
		{
			this.Die(null);
			return PlantProperties.State.Dying;
		}
		if (this.currentStage.nextState <= this.State)
		{
			this.seasons++;
		}
		if (this.seasons >= this.Properties.MaxSeasons)
		{
			this.ChangeState(PlantProperties.State.Dying, true, false);
		}
		else
		{
			this.ChangeState(this.currentStage.nextState, true, false);
		}
		return this.State;
	}

	// Token: 0x06000B44 RID: 2884 RVA: 0x00063ABC File Offset: 0x00061CBC
	private void ConsumeWater()
	{
		if (this.State == PlantProperties.State.Dying)
		{
			return;
		}
		if (this.GetPlanter() == null)
		{
			return;
		}
		int num = Mathf.CeilToInt(Mathf.Min((float)this.planter.soilSaturation, this.WaterConsumption));
		if ((float)num > 0f)
		{
			this.planter.ConsumeWater(num, this);
		}
	}

	// Token: 0x06000B45 RID: 2885 RVA: 0x00063B16 File Offset: 0x00061D16
	public void Fertilize()
	{
		if (this.Fertilized)
		{
			return;
		}
		this.Fertilized = true;
		this.CalculateQualities(false, false, false);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000B46 RID: 2886 RVA: 0x00063B38 File Offset: 0x00061D38
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_TakeClone(global::BaseEntity.RPCMessage msg)
	{
		this.TakeClones(msg.player);
	}

	// Token: 0x06000B47 RID: 2887 RVA: 0x00063B48 File Offset: 0x00061D48
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_TakeCloneAll(global::BaseEntity.RPCMessage msg)
	{
		if (base.GetParentEntity() != null)
		{
			List<global::GrowableEntity> list = Facepunch.Pool.GetList<global::GrowableEntity>();
			foreach (global::BaseEntity baseEntity in base.GetParentEntity().children)
			{
				global::GrowableEntity item;
				if (baseEntity != this && (item = (baseEntity as global::GrowableEntity)) != null)
				{
					list.Add(item);
				}
			}
			foreach (global::GrowableEntity growableEntity in list)
			{
				growableEntity.TakeClones(msg.player);
			}
			Facepunch.Pool.FreeList<global::GrowableEntity>(ref list);
		}
		this.TakeClones(msg.player);
	}

	// Token: 0x06000B48 RID: 2888 RVA: 0x00063C20 File Offset: 0x00061E20
	private void TakeClones(global::BasePlayer player)
	{
		if (player == null)
		{
			return;
		}
		if (!this.CanClone())
		{
			return;
		}
		int num = this.Properties.BaseCloneCount + this.Genes.GetGeneTypeCount(GrowableGenetics.GeneType.Yield) / 2;
		if (num <= 0)
		{
			return;
		}
		global::Item item = ItemManager.Create(this.Properties.CloneItem, num, 0UL);
		GrowableGeneEncoding.EncodeGenesToItem(this, item);
		player.GiveItem(item, global::BaseEntity.GiveItemReason.ResourceHarvested);
		if (this.Properties.pickEffect.isValid)
		{
			Effect.server.Run(this.Properties.pickEffect.resourcePath, base.transform.position, Vector3.up, null, false);
		}
		this.Die(null);
	}

	// Token: 0x06000B49 RID: 2889 RVA: 0x00063CC4 File Offset: 0x00061EC4
	public void PickFruit(global::BasePlayer player, bool eat = false)
	{
		if (!this.CanPick())
		{
			return;
		}
		this.harvests++;
		this.GiveFruit(player, this.CurrentPickAmount, eat);
		RandomItemDispenser randomItemDispenser = PrefabAttribute.server.Find<RandomItemDispenser>(this.prefabID);
		if (randomItemDispenser != null)
		{
			randomItemDispenser.DistributeItems(player, base.transform.position);
		}
		this.ResetSeason();
		if (this.Properties.pickEffect.isValid)
		{
			Effect.server.Run(this.Properties.pickEffect.resourcePath, base.transform.position, Vector3.up, null, false);
		}
		if (this.harvests < this.Properties.maxHarvests)
		{
			this.ChangeState(PlantProperties.State.Mature, true, false);
			return;
		}
		if (this.Properties.disappearAfterHarvest)
		{
			this.Die(null);
			return;
		}
		this.ChangeState(PlantProperties.State.Dying, true, false);
	}

	// Token: 0x06000B4A RID: 2890 RVA: 0x00063D9C File Offset: 0x00061F9C
	private void GiveFruit(global::BasePlayer player, int amount, bool eat)
	{
		if (amount <= 0)
		{
			return;
		}
		bool enabled = this.Properties.pickupItem.condition.enabled;
		if (enabled)
		{
			for (int i = 0; i < amount; i++)
			{
				this.GiveFruit(player, 1, enabled, eat);
			}
			return;
		}
		this.GiveFruit(player, amount, enabled, eat);
	}

	// Token: 0x06000B4B RID: 2891 RVA: 0x00063DE8 File Offset: 0x00061FE8
	private void GiveFruit(global::BasePlayer player, int amount, bool applyCondition, bool eat)
	{
		global::Item item = ItemManager.Create(this.Properties.pickupItem, amount, 0UL);
		if (applyCondition)
		{
			item.conditionNormalized = this.Properties.fruitVisualScaleCurve.Evaluate(this.StageProgressFraction);
		}
		if (eat && player != null && this.IsFood())
		{
			ItemModConsume component = item.info.GetComponent<ItemModConsume>();
			if (component != null)
			{
				component.DoAction(item, player);
				return;
			}
		}
		if (player != null)
		{
			player.GiveItem(item, global::BaseEntity.GiveItemReason.ResourceHarvested);
			return;
		}
		item.Drop(base.transform.position + Vector3.up * 0.5f, Vector3.up * 1f, default(Quaternion));
	}

	// Token: 0x06000B4C RID: 2892 RVA: 0x00063EAB File Offset: 0x000620AB
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_PickFruit(global::BaseEntity.RPCMessage msg)
	{
		this.PickFruit(msg.player, false);
	}

	// Token: 0x06000B4D RID: 2893 RVA: 0x00063EBA File Offset: 0x000620BA
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_EatFruit(global::BaseEntity.RPCMessage msg)
	{
		this.PickFruit(msg.player, true);
	}

	// Token: 0x06000B4E RID: 2894 RVA: 0x00063ECC File Offset: 0x000620CC
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_PickFruitAll(global::BaseEntity.RPCMessage msg)
	{
		if (base.GetParentEntity() != null)
		{
			List<global::GrowableEntity> list = Facepunch.Pool.GetList<global::GrowableEntity>();
			foreach (global::BaseEntity baseEntity in base.GetParentEntity().children)
			{
				global::GrowableEntity item;
				if (baseEntity != this && (item = (baseEntity as global::GrowableEntity)) != null)
				{
					list.Add(item);
				}
			}
			foreach (global::GrowableEntity growableEntity in list)
			{
				growableEntity.PickFruit(msg.player, false);
			}
			Facepunch.Pool.FreeList<global::GrowableEntity>(ref list);
		}
		this.PickFruit(msg.player, false);
	}

	// Token: 0x06000B4F RID: 2895 RVA: 0x00063FA4 File Offset: 0x000621A4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_RemoveDying(global::BaseEntity.RPCMessage msg)
	{
		this.RemoveDying(msg.player);
	}

	// Token: 0x06000B50 RID: 2896 RVA: 0x00063FB4 File Offset: 0x000621B4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_RemoveDyingAll(global::BaseEntity.RPCMessage msg)
	{
		if (base.GetParentEntity() != null)
		{
			List<global::GrowableEntity> list = Facepunch.Pool.GetList<global::GrowableEntity>();
			foreach (global::BaseEntity baseEntity in base.GetParentEntity().children)
			{
				global::GrowableEntity item;
				if (baseEntity != this && (item = (baseEntity as global::GrowableEntity)) != null)
				{
					list.Add(item);
				}
			}
			foreach (global::GrowableEntity growableEntity in list)
			{
				growableEntity.RemoveDying(msg.player);
			}
			Facepunch.Pool.FreeList<global::GrowableEntity>(ref list);
		}
		this.RemoveDying(msg.player);
	}

	// Token: 0x06000B51 RID: 2897 RVA: 0x0006408C File Offset: 0x0006228C
	public void RemoveDying(global::BasePlayer receiver)
	{
		if (this.State != PlantProperties.State.Dying)
		{
			return;
		}
		if (this.Properties.removeDyingItem == null)
		{
			return;
		}
		if (this.Properties.removeDyingEffect.isValid)
		{
			Effect.server.Run(this.Properties.removeDyingEffect.resourcePath, base.transform.position, Vector3.up, null, false);
		}
		global::Item item = ItemManager.Create(this.Properties.removeDyingItem, 1, 0UL);
		if (receiver != null)
		{
			receiver.GiveItem(item, global::BaseEntity.GiveItemReason.PickedUp);
		}
		else
		{
			item.Drop(base.transform.position + Vector3.up * 0.5f, Vector3.up * 1f, default(Quaternion));
		}
		this.Die(null);
	}

	// Token: 0x06000B52 RID: 2898 RVA: 0x0006415C File Offset: 0x0006235C
	[ServerVar(ServerAdmin = true)]
	public static void GrowAll(ConsoleSystem.Arg arg)
	{
		global::BasePlayer basePlayer = arg.Player();
		if (!basePlayer.IsAdmin)
		{
			return;
		}
		List<global::GrowableEntity> list = Facepunch.Pool.GetList<global::GrowableEntity>();
		global::Vis.Entities<global::GrowableEntity>(basePlayer.ServerPosition, 6f, list, -1, QueryTriggerInteraction.Collide);
		foreach (global::GrowableEntity growableEntity in list)
		{
			if (growableEntity.isServer)
			{
				growableEntity.ChangeState(growableEntity.currentStage.nextState, false, false);
			}
		}
		Facepunch.Pool.FreeList<global::GrowableEntity>(ref list);
	}

	// Token: 0x06000B53 RID: 2899 RVA: 0x000641F0 File Offset: 0x000623F0
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_RequestQualityUpdate(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player != null)
		{
			ProtoBuf.GrowableEntity growableEntity = Facepunch.Pool.Get<ProtoBuf.GrowableEntity>();
			growableEntity.lightModifier = this.LightQuality;
			growableEntity.groundModifier = this.GroundQuality;
			growableEntity.waterModifier = this.WaterQuality;
			growableEntity.happiness = this.OverallQuality;
			growableEntity.temperatureModifier = this.TemperatureQuality;
			growableEntity.waterConsumption = this.WaterConsumption;
			base.ClientRPCPlayer<ProtoBuf.GrowableEntity>(null, msg.player, "RPC_ReceiveQualityUpdate", growableEntity);
		}
	}

	// Token: 0x170000ED RID: 237
	// (get) Token: 0x06000B54 RID: 2900 RVA: 0x0006426C File Offset: 0x0006246C
	// (set) Token: 0x06000B55 RID: 2901 RVA: 0x00064274 File Offset: 0x00062474
	public PlantProperties.State State { get; private set; }

	// Token: 0x170000EE RID: 238
	// (get) Token: 0x06000B56 RID: 2902 RVA: 0x0006427D File Offset: 0x0006247D
	// (set) Token: 0x06000B57 RID: 2903 RVA: 0x00064285 File Offset: 0x00062485
	public float Age { get; private set; }

	// Token: 0x170000EF RID: 239
	// (get) Token: 0x06000B58 RID: 2904 RVA: 0x0006428E File Offset: 0x0006248E
	// (set) Token: 0x06000B59 RID: 2905 RVA: 0x00064296 File Offset: 0x00062496
	public float LightQuality { get; private set; }

	// Token: 0x170000F0 RID: 240
	// (get) Token: 0x06000B5A RID: 2906 RVA: 0x0006429F File Offset: 0x0006249F
	// (set) Token: 0x06000B5B RID: 2907 RVA: 0x000642A7 File Offset: 0x000624A7
	public float GroundQuality { get; private set; } = 1f;

	// Token: 0x170000F1 RID: 241
	// (get) Token: 0x06000B5C RID: 2908 RVA: 0x000642B0 File Offset: 0x000624B0
	// (set) Token: 0x06000B5D RID: 2909 RVA: 0x000642B8 File Offset: 0x000624B8
	public float WaterQuality { get; private set; }

	// Token: 0x170000F2 RID: 242
	// (get) Token: 0x06000B5E RID: 2910 RVA: 0x000642C1 File Offset: 0x000624C1
	// (set) Token: 0x06000B5F RID: 2911 RVA: 0x000642C9 File Offset: 0x000624C9
	public float WaterConsumption { get; private set; }

	// Token: 0x170000F3 RID: 243
	// (get) Token: 0x06000B60 RID: 2912 RVA: 0x000642D2 File Offset: 0x000624D2
	// (set) Token: 0x06000B61 RID: 2913 RVA: 0x000642DA File Offset: 0x000624DA
	public bool Fertilized { get; private set; }

	// Token: 0x170000F4 RID: 244
	// (get) Token: 0x06000B62 RID: 2914 RVA: 0x000642E3 File Offset: 0x000624E3
	// (set) Token: 0x06000B63 RID: 2915 RVA: 0x000642EB File Offset: 0x000624EB
	public float TemperatureQuality { get; private set; }

	// Token: 0x170000F5 RID: 245
	// (get) Token: 0x06000B64 RID: 2916 RVA: 0x000642F4 File Offset: 0x000624F4
	// (set) Token: 0x06000B65 RID: 2917 RVA: 0x000642FC File Offset: 0x000624FC
	public float OverallQuality { get; private set; }

	// Token: 0x170000F6 RID: 246
	// (get) Token: 0x06000B66 RID: 2918 RVA: 0x00064305 File Offset: 0x00062505
	// (set) Token: 0x06000B67 RID: 2919 RVA: 0x0006430D File Offset: 0x0006250D
	public float Yield { get; private set; }

	// Token: 0x170000F7 RID: 247
	// (get) Token: 0x06000B68 RID: 2920 RVA: 0x00064318 File Offset: 0x00062518
	public float StageProgressFraction
	{
		get
		{
			return this.stageAge / this.currentStage.lifeLengthSeconds;
		}
	}

	// Token: 0x170000F8 RID: 248
	// (get) Token: 0x06000B69 RID: 2921 RVA: 0x0006433A File Offset: 0x0006253A
	private PlantProperties.Stage currentStage
	{
		get
		{
			return this.Properties.stages[(int)this.State];
		}
	}

	// Token: 0x170000F9 RID: 249
	// (get) Token: 0x06000B6A RID: 2922 RVA: 0x00064352 File Offset: 0x00062552
	public static float ThinkDeltaTime
	{
		get
		{
			return ConVar.Server.planttick;
		}
	}

	// Token: 0x170000FA RID: 250
	// (get) Token: 0x06000B6B RID: 2923 RVA: 0x00064359 File Offset: 0x00062559
	private float growDeltaTime
	{
		get
		{
			return ConVar.Server.planttick * ConVar.Server.planttickscale;
		}
	}

	// Token: 0x06000B6C RID: 2924 RVA: 0x00064366 File Offset: 0x00062566
	public void ReceiveInstanceData(ProtoBuf.Item.InstanceData data)
	{
		GrowableGeneEncoding.DecodeIntToGenes(data.dataInt, this.Genes);
		GrowableGeneEncoding.DecodeIntToPreviousGenes(data.dataInt, this.Genes);
	}

	// Token: 0x06000B6D RID: 2925 RVA: 0x0006438A File Offset: 0x0006258A
	public override void ResetState()
	{
		base.ResetState();
		this.State = PlantProperties.State.Seed;
	}

	// Token: 0x06000B6E RID: 2926 RVA: 0x00064399 File Offset: 0x00062599
	public bool CanPick()
	{
		return this.currentStage.resources > 0f;
	}

	// Token: 0x170000FB RID: 251
	// (get) Token: 0x06000B6F RID: 2927 RVA: 0x000643AD File Offset: 0x000625AD
	public int CurrentPickAmount
	{
		get
		{
			return Mathf.RoundToInt(this.CurrentPickAmountFloat);
		}
	}

	// Token: 0x170000FC RID: 252
	// (get) Token: 0x06000B70 RID: 2928 RVA: 0x000643BA File Offset: 0x000625BA
	public float CurrentPickAmountFloat
	{
		get
		{
			return (this.currentStage.resources + this.Yield) * (float)this.Properties.pickupMultiplier;
		}
	}

	// Token: 0x06000B71 RID: 2929 RVA: 0x000643DB File Offset: 0x000625DB
	public bool CanTakeSeeds()
	{
		return this.currentStage.resources > 0f && this.Properties.SeedItem != null;
	}

	// Token: 0x06000B72 RID: 2930 RVA: 0x00064402 File Offset: 0x00062602
	public bool CanClone()
	{
		return this.currentStage.resources > 0f && this.Properties.CloneItem != null;
	}

	// Token: 0x06000B73 RID: 2931 RVA: 0x0006442C File Offset: 0x0006262C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.growableEntity = Facepunch.Pool.Get<ProtoBuf.GrowableEntity>();
		info.msg.growableEntity.state = (int)this.State;
		info.msg.growableEntity.totalAge = this.Age;
		info.msg.growableEntity.stageAge = this.stageAge;
		info.msg.growableEntity.yieldFraction = this.Yield;
		info.msg.growableEntity.yieldPool = this.yieldPool;
		info.msg.growableEntity.fertilized = this.Fertilized;
		if (this.Genes != null)
		{
			this.Genes.Save(info);
		}
		if (!info.forDisk)
		{
			info.msg.growableEntity.lightModifier = this.LightQuality;
			info.msg.growableEntity.groundModifier = this.GroundQuality;
			info.msg.growableEntity.waterModifier = this.WaterQuality;
			info.msg.growableEntity.happiness = this.OverallQuality;
			info.msg.growableEntity.temperatureModifier = this.TemperatureQuality;
			info.msg.growableEntity.waterConsumption = this.WaterConsumption;
		}
	}

	// Token: 0x06000B74 RID: 2932 RVA: 0x00064578 File Offset: 0x00062778
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.growableEntity != null)
		{
			this.Age = info.msg.growableEntity.totalAge;
			this.stageAge = info.msg.growableEntity.stageAge;
			this.Yield = info.msg.growableEntity.yieldFraction;
			this.Fertilized = info.msg.growableEntity.fertilized;
			this.yieldPool = info.msg.growableEntity.yieldPool;
			this.Genes.Load(info);
			this.ChangeState((PlantProperties.State)info.msg.growableEntity.state, false, true);
			return;
		}
		this.Genes.GenerateRandom(this);
	}

	// Token: 0x06000B75 RID: 2933 RVA: 0x0006463C File Offset: 0x0006283C
	private void ChangeState(PlantProperties.State state, bool resetAge, bool loading = false)
	{
		if (base.isServer && this.State == state)
		{
			return;
		}
		this.State = state;
		if (base.isServer)
		{
			if (!loading)
			{
				if (this.currentStage.resources > 0f)
				{
					this.yieldPool = this.currentStage.yield;
				}
				if (state == PlantProperties.State.Crossbreed)
				{
					if (this.Properties.CrossBreedEffect.isValid)
					{
						Effect.server.Run(this.Properties.CrossBreedEffect.resourcePath, base.transform.position, Vector3.up, null, false);
					}
					GrowableGenetics.CrossBreed(this);
				}
				base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
			if (resetAge)
			{
				this.stageAge = 0f;
			}
		}
	}

	// Token: 0x06000B76 RID: 2934 RVA: 0x000646E8 File Offset: 0x000628E8
	public override void OnDeployed(global::BaseEntity parent, global::BasePlayer deployedBy, global::Item fromItem)
	{
		base.OnDeployed(parent, deployedBy, fromItem);
		PlanterBox planterBox;
		if (parent != null && (planterBox = (parent as PlanterBox)) != null)
		{
			planterBox.OnPlantInserted(this, deployedBy);
		}
	}

	// Token: 0x0400072D RID: 1837
	private const float artificalLightQuality = 1f;

	// Token: 0x0400072E RID: 1838
	private const float planterGroundModifierBase = 0.6f;

	// Token: 0x0400072F RID: 1839
	private const float fertilizerGroundModifierBonus = 0.4f;

	// Token: 0x04000730 RID: 1840
	private const float growthGeneSpeedMultiplier = 0.25f;

	// Token: 0x04000731 RID: 1841
	private const float waterGeneRequirementMultiplier = 0.1f;

	// Token: 0x04000732 RID: 1842
	private const float hardinessGeneModifierBonus = 0.2f;

	// Token: 0x04000733 RID: 1843
	private const float hardinessGeneTemperatureModifierBonus = 0.05f;

	// Token: 0x04000734 RID: 1844
	private const float baseYieldIncreaseMultiplier = 1f;

	// Token: 0x04000735 RID: 1845
	private const float yieldGeneBonusMultiplier = 0.25f;

	// Token: 0x04000736 RID: 1846
	private const float maxNonPlanterGroundQuality = 0.6f;

	// Token: 0x04000737 RID: 1847
	private const float deathRatePerQuality = 0.1f;

	// Token: 0x04000738 RID: 1848
	private TimeCachedValue<float> sunExposure;

	// Token: 0x04000739 RID: 1849
	private TimeCachedValue<float> artificialLightExposure;

	// Token: 0x0400073A RID: 1850
	private TimeCachedValue<float> artificialTemperatureExposure;

	// Token: 0x0400073B RID: 1851
	[ServerVar]
	[Help("How many miliseconds to budget for processing growable quality updates per frame")]
	public static float framebudgetms = 0.25f;

	// Token: 0x0400073C RID: 1852
	public static global::GrowableEntity.GrowableEntityUpdateQueue growableEntityUpdateQueue = new global::GrowableEntity.GrowableEntityUpdateQueue();

	// Token: 0x0400073D RID: 1853
	private bool underWater;

	// Token: 0x0400073E RID: 1854
	private int seasons;

	// Token: 0x0400073F RID: 1855
	private int harvests;

	// Token: 0x04000740 RID: 1856
	private float terrainTypeValue;

	// Token: 0x04000741 RID: 1857
	private float yieldPool;

	// Token: 0x04000742 RID: 1858
	private PlanterBox planter;

	// Token: 0x04000743 RID: 1859
	public PlantProperties Properties;

	// Token: 0x04000744 RID: 1860
	public ItemDefinition SourceItemDef;

	// Token: 0x0400074F RID: 1871
	private float stageAge;

	// Token: 0x04000750 RID: 1872
	public GrowableGenes Genes = new GrowableGenes();

	// Token: 0x04000751 RID: 1873
	private const float startingHealth = 10f;

	// Token: 0x02000B86 RID: 2950
	public class GrowableEntityUpdateQueue : ObjectWorkQueue<global::GrowableEntity>
	{
		// Token: 0x06004AEA RID: 19178 RVA: 0x001910EB File Offset: 0x0018F2EB
		protected override void RunJob(global::GrowableEntity entity)
		{
			if (!this.ShouldAdd(entity))
			{
				return;
			}
			entity.CalculateQualities_Water();
		}

		// Token: 0x06004AEB RID: 19179 RVA: 0x001910FD File Offset: 0x0018F2FD
		protected override bool ShouldAdd(global::GrowableEntity entity)
		{
			return base.ShouldAdd(entity) && entity.IsValid();
		}
	}
}
