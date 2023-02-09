﻿using System;
using System.Collections;
using System.Collections.Generic;
using ConVar;
using Rust.Ai;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020001F7 RID: 503
public static class NavMeshTools
{
	// Token: 0x06001A34 RID: 6708 RVA: 0x000BA7E4 File Offset: 0x000B89E4
	public static IEnumerator CollectSourcesAsync(Bounds bounds, int mask, NavMeshCollectGeometry geometry, int area, bool useBakedTerrainMesh, int cellSize, List<NavMeshBuildSource> sources, Action<List<NavMeshBuildSource>> append, Action callback, Transform customNavMeshDataRoot)
	{
		while (!AI.move && !AiManager.nav_wait)
		{
			yield return CoroutineEx.waitForSeconds(1f);
		}
		if (customNavMeshDataRoot != null)
		{
			customNavMeshDataRoot.gameObject.SetActive(true);
			yield return new WaitForEndOfFrame();
		}
		float time = UnityEngine.Time.realtimeSinceStartup;
		Debug.Log("Starting Navmesh Source Collecting");
		if (useBakedTerrainMesh)
		{
			mask &= -8388609;
		}
		else
		{
			mask |= 8388608;
		}
		List<NavMeshBuildMarkup> markups = new List<NavMeshBuildMarkup>();
		NavMeshBuilder.CollectSources(bounds, mask, geometry, area, markups, sources);
		if (useBakedTerrainMesh && TerrainMeta.HeightMap != null)
		{
			for (float x = -bounds.extents.x; x < bounds.extents.x - (float)(cellSize / 2); x += (float)cellSize)
			{
				for (float z = -bounds.extents.z; z < bounds.extents.z - (float)(cellSize / 2); z += (float)cellSize)
				{
					AsyncTerrainNavMeshBake terrainSource = new AsyncTerrainNavMeshBake(new Vector3(x, 0f, z), cellSize, cellSize, false, true);
					yield return terrainSource;
					sources.Add(terrainSource.CreateNavMeshBuildSource(area));
					terrainSource = null;
				}
			}
		}
		if (append != null)
		{
			append(sources);
		}
		Debug.Log(string.Format("Navmesh Source Collecting took {0:0.00} seconds", UnityEngine.Time.realtimeSinceStartup - time));
		if (customNavMeshDataRoot != null)
		{
			customNavMeshDataRoot.gameObject.SetActive(false);
		}
		if (callback != null)
		{
			callback();
		}
		yield break;
	}

	// Token: 0x06001A35 RID: 6709 RVA: 0x000BA843 File Offset: 0x000B8A43
	public static IEnumerator CollectSourcesAsync(Transform root, int mask, NavMeshCollectGeometry geometry, int area, List<NavMeshBuildSource> sources, Action<List<NavMeshBuildSource>> append, Action callback)
	{
		while (!AI.move && !AiManager.nav_wait)
		{
			yield return CoroutineEx.waitForSeconds(1f);
		}
		float realtimeSinceStartup = UnityEngine.Time.realtimeSinceStartup;
		Debug.Log("Starting Navmesh Source Collecting");
		List<NavMeshBuildMarkup> markups = new List<NavMeshBuildMarkup>();
		NavMeshBuilder.CollectSources(root, mask, geometry, area, markups, sources);
		if (append != null)
		{
			append(sources);
		}
		Debug.Log(string.Format("Navmesh Source Collecting took {0:0.00} seconds", UnityEngine.Time.realtimeSinceStartup - realtimeSinceStartup));
		if (callback != null)
		{
			callback();
		}
		yield break;
	}
}
