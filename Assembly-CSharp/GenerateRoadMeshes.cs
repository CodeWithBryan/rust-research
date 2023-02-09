using System;
using UnityEngine;

// Token: 0x020006A0 RID: 1696
public class GenerateRoadMeshes : ProceduralComponent
{
	// Token: 0x0600301F RID: 12319 RVA: 0x0012547C File Offset: 0x0012367C
	public override void Process(uint seed)
	{
		if (this.RoadMeshes == null || this.RoadMeshes.Length == 0)
		{
			this.RoadMeshes = new Mesh[]
			{
				this.RoadMesh
			};
		}
		foreach (PathList pathList in TerrainMeta.Path.Roads)
		{
			if (pathList.Hierarchy < 2)
			{
				foreach (PathList.MeshObject meshObject in pathList.CreateMesh(this.RoadMeshes, 0f, true, !pathList.Path.Circular, !pathList.Path.Circular))
				{
					GameObject gameObject = new GameObject("Road Mesh");
					gameObject.transform.position = meshObject.Position;
					gameObject.layer = 16;
					gameObject.SetHierarchyGroup(pathList.Name, true, false);
					gameObject.SetActive(false);
					MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
					meshCollider.sharedMaterial = this.RoadPhysicMaterial;
					meshCollider.sharedMesh = meshObject.Meshes[0];
					gameObject.AddComponent<AddToHeightMap>();
					gameObject.SetActive(true);
				}
			}
		}
	}

	// Token: 0x170003B5 RID: 949
	// (get) Token: 0x06003020 RID: 12320 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool RunOnCache
	{
		get
		{
			return true;
		}
	}

	// Token: 0x040026F8 RID: 9976
	public const float NormalSmoothing = 0f;

	// Token: 0x040026F9 RID: 9977
	public const bool SnapToTerrain = true;

	// Token: 0x040026FA RID: 9978
	public Mesh RoadMesh;

	// Token: 0x040026FB RID: 9979
	public Mesh[] RoadMeshes;

	// Token: 0x040026FC RID: 9980
	public Material RoadMaterial;

	// Token: 0x040026FD RID: 9981
	public Material RoadRingMaterial;

	// Token: 0x040026FE RID: 9982
	public PhysicMaterial RoadPhysicMaterial;
}
