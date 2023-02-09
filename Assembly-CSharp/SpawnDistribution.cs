using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200053C RID: 1340
public class SpawnDistribution
{
	// Token: 0x060028DB RID: 10459 RVA: 0x000F9038 File Offset: 0x000F7238
	public SpawnDistribution(SpawnHandler handler, byte[] baseValues, Vector3 origin, Vector3 area)
	{
		this.Handler = handler;
		this.quadtree.UpdateValues(baseValues);
		this.origin = origin;
		float num = 0f;
		for (int i = 0; i < baseValues.Length; i++)
		{
			num += (float)baseValues[i];
		}
		this.Density = num / (float)(255 * baseValues.Length);
		this.Count = 0;
		this.area = new Vector3(area.x / (float)this.quadtree.Size, area.y, area.z / (float)this.quadtree.Size);
		this.grid = new WorldSpaceGrid<int>(area.x, 20f);
	}

	// Token: 0x060028DC RID: 10460 RVA: 0x000F9100 File Offset: 0x000F7300
	public bool Sample(out Vector3 spawnPos, out Quaternion spawnRot, bool alignToNormal = false, float dithering = 0f)
	{
		return this.Sample(out spawnPos, out spawnRot, this.SampleNode(), alignToNormal, dithering);
	}

	// Token: 0x060028DD RID: 10461 RVA: 0x000F9114 File Offset: 0x000F7314
	public bool Sample(out Vector3 spawnPos, out Quaternion spawnRot, ByteQuadtree.Element node, bool alignToNormal = false, float dithering = 0f)
	{
		if (this.Handler == null || TerrainMeta.HeightMap == null)
		{
			spawnPos = Vector3.zero;
			spawnRot = Quaternion.identity;
			return false;
		}
		LayerMask placementMask = this.Handler.PlacementMask;
		LayerMask placementCheckMask = this.Handler.PlacementCheckMask;
		float placementCheckHeight = this.Handler.PlacementCheckHeight;
		LayerMask radiusCheckMask = this.Handler.RadiusCheckMask;
		float radiusCheckDistance = this.Handler.RadiusCheckDistance;
		for (int i = 0; i < 15; i++)
		{
			spawnPos = this.origin;
			spawnPos.x += node.Coords.x * this.area.x;
			spawnPos.z += node.Coords.y * this.area.z;
			spawnPos.x += UnityEngine.Random.value * this.area.x;
			spawnPos.z += UnityEngine.Random.value * this.area.z;
			spawnPos.x += UnityEngine.Random.Range(-dithering, dithering);
			spawnPos.z += UnityEngine.Random.Range(-dithering, dithering);
			Vector3 vector = new Vector3(spawnPos.x, TerrainMeta.HeightMap.GetHeight(spawnPos), spawnPos.z);
			if (vector.y > spawnPos.y)
			{
				RaycastHit raycastHit;
				if (placementCheckMask != 0 && Physics.Raycast(vector + Vector3.up * placementCheckHeight, Vector3.down, out raycastHit, placementCheckHeight, placementCheckMask))
				{
					if ((1 << raycastHit.transform.gameObject.layer & placementMask) == 0)
					{
						goto IL_243;
					}
					vector.y = raycastHit.point.y;
				}
				if (radiusCheckMask == 0 || !Physics.CheckSphere(vector, radiusCheckDistance, radiusCheckMask))
				{
					spawnPos.y = vector.y;
					spawnRot = Quaternion.Euler(new Vector3(0f, UnityEngine.Random.Range(0f, 360f), 0f));
					if (alignToNormal)
					{
						Vector3 normal = TerrainMeta.HeightMap.GetNormal(spawnPos);
						spawnRot = QuaternionEx.LookRotationForcedUp(spawnRot * Vector3.forward, normal);
					}
					return true;
				}
			}
			IL_243:;
		}
		spawnPos = Vector3.zero;
		spawnRot = Quaternion.identity;
		return false;
	}

	// Token: 0x060028DE RID: 10462 RVA: 0x000F938C File Offset: 0x000F758C
	public ByteQuadtree.Element SampleNode()
	{
		ByteQuadtree.Element result = this.quadtree.Root;
		while (!result.IsLeaf)
		{
			result = result.RandChild;
		}
		return result;
	}

	// Token: 0x060028DF RID: 10463 RVA: 0x000F93B9 File Offset: 0x000F75B9
	public void AddInstance(Spawnable spawnable)
	{
		this.UpdateCount(spawnable, 1);
	}

	// Token: 0x060028E0 RID: 10464 RVA: 0x000F93C3 File Offset: 0x000F75C3
	public void RemoveInstance(Spawnable spawnable)
	{
		this.UpdateCount(spawnable, -1);
	}

	// Token: 0x060028E1 RID: 10465 RVA: 0x000F93D0 File Offset: 0x000F75D0
	private void UpdateCount(Spawnable spawnable, int delta)
	{
		this.Count += delta;
		WorldSpaceGrid<int> worldSpaceGrid = this.grid;
		Vector3 spawnPosition = spawnable.SpawnPosition;
		worldSpaceGrid[spawnPosition] += delta;
		BaseEntity component = spawnable.GetComponent<BaseEntity>();
		if (component)
		{
			int num;
			if (this.dict.TryGetValue(component.prefabID, out num))
			{
				this.dict[component.prefabID] = num + delta;
				return;
			}
			num = delta;
			this.dict.Add(component.prefabID, num);
		}
	}

	// Token: 0x060028E2 RID: 10466 RVA: 0x000F9458 File Offset: 0x000F7658
	public int GetCount(uint prefabID)
	{
		int result;
		this.dict.TryGetValue(prefabID, out result);
		return result;
	}

	// Token: 0x060028E3 RID: 10467 RVA: 0x000F9475 File Offset: 0x000F7675
	public int GetCount(Vector3 position)
	{
		return this.grid[position];
	}

	// Token: 0x060028E4 RID: 10468 RVA: 0x000F9483 File Offset: 0x000F7683
	public float GetGridCellArea()
	{
		return this.grid.CellArea;
	}

	// Token: 0x0400212A RID: 8490
	internal SpawnHandler Handler;

	// Token: 0x0400212B RID: 8491
	internal float Density;

	// Token: 0x0400212C RID: 8492
	internal int Count;

	// Token: 0x0400212D RID: 8493
	private WorldSpaceGrid<int> grid;

	// Token: 0x0400212E RID: 8494
	private Dictionary<uint, int> dict = new Dictionary<uint, int>();

	// Token: 0x0400212F RID: 8495
	private ByteQuadtree quadtree = new ByteQuadtree();

	// Token: 0x04002130 RID: 8496
	private Vector3 origin;

	// Token: 0x04002131 RID: 8497
	private Vector3 area;
}
