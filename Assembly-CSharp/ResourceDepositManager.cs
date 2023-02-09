using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003E1 RID: 993
public class ResourceDepositManager : BaseEntity
{
	// Token: 0x060021B1 RID: 8625 RVA: 0x000D8554 File Offset: 0x000D6754
	public static Vector2i GetIndexFrom(Vector3 pos)
	{
		return new Vector2i((int)pos.x / 20, (int)pos.z / 20);
	}

	// Token: 0x060021B2 RID: 8626 RVA: 0x000D856F File Offset: 0x000D676F
	public static ResourceDepositManager Get()
	{
		return ResourceDepositManager._manager;
	}

	// Token: 0x060021B3 RID: 8627 RVA: 0x000D8576 File Offset: 0x000D6776
	public ResourceDepositManager()
	{
		ResourceDepositManager._manager = this;
		this._deposits = new Dictionary<Vector2i, ResourceDepositManager.ResourceDeposit>();
	}

	// Token: 0x060021B4 RID: 8628 RVA: 0x000D8590 File Offset: 0x000D6790
	public ResourceDepositManager.ResourceDeposit CreateFromPosition(Vector3 pos)
	{
		Vector2i indexFrom = ResourceDepositManager.GetIndexFrom(pos);
		UnityEngine.Random.State state = UnityEngine.Random.state;
		UnityEngine.Random.InitState((int)new Vector2((float)indexFrom.x, (float)indexFrom.y).Seed(World.Seed + World.Salt));
		ResourceDepositManager.ResourceDeposit resourceDeposit = new ResourceDepositManager.ResourceDeposit();
		resourceDeposit.origin = new Vector3((float)(indexFrom.x * 20), 0f, (float)(indexFrom.y * 20));
		if (UnityEngine.Random.Range(0f, 1f) < 0.5f)
		{
			resourceDeposit.Add(ItemManager.FindItemDefinition("stones"), 1f, 100, 1f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
		}
		else if (!false)
		{
			resourceDeposit.Add(ItemManager.FindItemDefinition("stones"), 1f, UnityEngine.Random.Range(30000, 100000), UnityEngine.Random.Range(0.3f, 0.5f), ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
			float num;
			if (World.Procedural)
			{
				num = ((TerrainMeta.BiomeMap.GetBiome(pos, 2) > 0.5f) ? 1f : 0f) * 0.25f;
			}
			else
			{
				num = 0.1f;
			}
			if (UnityEngine.Random.Range(0f, 1f) >= 1f - num)
			{
				resourceDeposit.Add(ItemManager.FindItemDefinition("metal.ore"), 1f, UnityEngine.Random.Range(10000, 100000), UnityEngine.Random.Range(2f, 4f), ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
			}
			float num2;
			if (World.Procedural)
			{
				num2 = ((TerrainMeta.BiomeMap.GetBiome(pos, 1) > 0.5f) ? 1f : 0f) * (0.25f + 0.25f * (TerrainMeta.TopologyMap.GetTopology(pos, 8) ? 1f : 0f) + 0.25f * (TerrainMeta.TopologyMap.GetTopology(pos, 1) ? 1f : 0f));
			}
			else
			{
				num2 = 0.1f;
			}
			if (UnityEngine.Random.Range(0f, 1f) >= 1f - num2)
			{
				resourceDeposit.Add(ItemManager.FindItemDefinition("sulfur.ore"), 1f, UnityEngine.Random.Range(10000, 100000), UnityEngine.Random.Range(4f, 4f), ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
			}
			float num3 = 0f;
			if (World.Procedural)
			{
				if (TerrainMeta.BiomeMap.GetBiome(pos, 8) > 0.5f || TerrainMeta.BiomeMap.GetBiome(pos, 4) > 0.5f)
				{
					num3 += 0.25f;
				}
			}
			else
			{
				num3 += 0.15f;
			}
			if (UnityEngine.Random.Range(0f, 1f) >= 1f - num3)
			{
				resourceDeposit.Add(ItemManager.FindItemDefinition("hq.metal.ore"), 1f, UnityEngine.Random.Range(5000, 10000), UnityEngine.Random.Range(30f, 50f), ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
			}
		}
		this._deposits.Add(indexFrom, resourceDeposit);
		UnityEngine.Random.state = state;
		return resourceDeposit;
	}

	// Token: 0x060021B5 RID: 8629 RVA: 0x000D886C File Offset: 0x000D6A6C
	public ResourceDepositManager.ResourceDeposit GetFromPosition(Vector3 pos)
	{
		ResourceDepositManager.ResourceDeposit result = null;
		if (this._deposits.TryGetValue(ResourceDepositManager.GetIndexFrom(pos), out result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x060021B6 RID: 8630 RVA: 0x000D8894 File Offset: 0x000D6A94
	public static ResourceDepositManager.ResourceDeposit GetOrCreate(Vector3 pos)
	{
		ResourceDepositManager.ResourceDeposit fromPosition = ResourceDepositManager.Get().GetFromPosition(pos);
		if (fromPosition != null)
		{
			return fromPosition;
		}
		return ResourceDepositManager.Get().CreateFromPosition(pos);
	}

	// Token: 0x04001A15 RID: 6677
	public static ResourceDepositManager _manager;

	// Token: 0x04001A16 RID: 6678
	private const int resolution = 20;

	// Token: 0x04001A17 RID: 6679
	public Dictionary<Vector2i, ResourceDepositManager.ResourceDeposit> _deposits;

	// Token: 0x02000C83 RID: 3203
	[Serializable]
	public class ResourceDeposit
	{
		// Token: 0x06004D01 RID: 19713 RVA: 0x00196D50 File Offset: 0x00194F50
		public ResourceDeposit()
		{
			this._resources = new List<ResourceDepositManager.ResourceDeposit.ResourceDepositEntry>();
		}

		// Token: 0x06004D02 RID: 19714 RVA: 0x00196D70 File Offset: 0x00194F70
		public void Add(ItemDefinition type, float efficiency, int amount, float workNeeded, ResourceDepositManager.ResourceDeposit.surveySpawnType spawnType, bool liquid = false)
		{
			ResourceDepositManager.ResourceDeposit.ResourceDepositEntry resourceDepositEntry = new ResourceDepositManager.ResourceDeposit.ResourceDepositEntry();
			resourceDepositEntry.type = type;
			resourceDepositEntry.efficiency = efficiency;
			ResourceDepositManager.ResourceDeposit.ResourceDepositEntry resourceDepositEntry2 = resourceDepositEntry;
			resourceDepositEntry.amount = amount;
			resourceDepositEntry2.startAmount = amount;
			resourceDepositEntry.spawnType = spawnType;
			resourceDepositEntry.workNeeded = workNeeded;
			resourceDepositEntry.isLiquid = liquid;
			this._resources.Add(resourceDepositEntry);
		}

		// Token: 0x040042A0 RID: 17056
		public float lastSurveyTime = float.NegativeInfinity;

		// Token: 0x040042A1 RID: 17057
		public Vector3 origin;

		// Token: 0x040042A2 RID: 17058
		public List<ResourceDepositManager.ResourceDeposit.ResourceDepositEntry> _resources;

		// Token: 0x02000F63 RID: 3939
		[Serializable]
		public enum surveySpawnType
		{
			// Token: 0x04004E19 RID: 19993
			ITEM,
			// Token: 0x04004E1A RID: 19994
			OIL,
			// Token: 0x04004E1B RID: 19995
			WATER
		}

		// Token: 0x02000F64 RID: 3940
		[Serializable]
		public class ResourceDepositEntry
		{
			// Token: 0x06005267 RID: 21095 RVA: 0x001A714C File Offset: 0x001A534C
			public void Subtract(int subamount)
			{
				if (subamount <= 0)
				{
					return;
				}
				this.amount -= subamount;
				if (this.amount < 0)
				{
					this.amount = 0;
				}
			}

			// Token: 0x04004E1C RID: 19996
			public ItemDefinition type;

			// Token: 0x04004E1D RID: 19997
			public float efficiency = 1f;

			// Token: 0x04004E1E RID: 19998
			public int amount;

			// Token: 0x04004E1F RID: 19999
			public int startAmount;

			// Token: 0x04004E20 RID: 20000
			public float workNeeded = 1f;

			// Token: 0x04004E21 RID: 20001
			public float workDone;

			// Token: 0x04004E22 RID: 20002
			public ResourceDepositManager.ResourceDeposit.surveySpawnType spawnType;

			// Token: 0x04004E23 RID: 20003
			public bool isLiquid;
		}
	}
}
