using System;
using UnityEngine;

// Token: 0x0200069B RID: 1691
public class GenerateRiverMeshes : ProceduralComponent
{
	// Token: 0x06003012 RID: 12306 RVA: 0x00124708 File Offset: 0x00122908
	public override void Process(uint seed)
	{
		this.RiverMeshes = new Mesh[]
		{
			this.RiverMesh
		};
		foreach (PathList pathList in TerrainMeta.Path.Rivers)
		{
			foreach (PathList.MeshObject meshObject in pathList.CreateMesh(this.RiverMeshes, 0.1f, true, !pathList.Path.Circular, !pathList.Path.Circular))
			{
				GameObject gameObject = new GameObject("River Mesh");
				gameObject.transform.position = meshObject.Position;
				gameObject.tag = "River";
				gameObject.layer = 4;
				gameObject.SetHierarchyGroup(pathList.Name, true, false);
				gameObject.SetActive(false);
				MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
				meshCollider.sharedMaterial = this.RiverPhysicMaterial;
				meshCollider.sharedMesh = meshObject.Meshes[0];
				gameObject.AddComponent<RiverInfo>();
				gameObject.AddComponent<WaterBody>().FishingType = WaterBody.FishingTag.River;
				gameObject.AddComponent<AddToWaterMap>();
				gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x170003B4 RID: 948
	// (get) Token: 0x06003013 RID: 12307 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool RunOnCache
	{
		get
		{
			return true;
		}
	}

	// Token: 0x040026E3 RID: 9955
	public const float NormalSmoothing = 0.1f;

	// Token: 0x040026E4 RID: 9956
	public const bool SnapToTerrain = true;

	// Token: 0x040026E5 RID: 9957
	public Mesh RiverMesh;

	// Token: 0x040026E6 RID: 9958
	public Mesh[] RiverMeshes;

	// Token: 0x040026E7 RID: 9959
	public Material RiverMaterial;

	// Token: 0x040026E8 RID: 9960
	public PhysicMaterial RiverPhysicMaterial;
}
