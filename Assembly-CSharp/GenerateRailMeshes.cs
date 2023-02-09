using System;
using UnityEngine;

// Token: 0x02000694 RID: 1684
public class GenerateRailMeshes : ProceduralComponent
{
	// Token: 0x06002FFE RID: 12286 RVA: 0x00122B4C File Offset: 0x00120D4C
	public override void Process(uint seed)
	{
		if (this.RailMeshes == null || this.RailMeshes.Length == 0)
		{
			this.RailMeshes = new Mesh[]
			{
				this.RailMesh
			};
		}
		foreach (PathList pathList in TerrainMeta.Path.Rails)
		{
			foreach (PathList.MeshObject meshObject in pathList.CreateMesh(this.RailMeshes, 0f, false, !pathList.Path.Circular && !pathList.Start, !pathList.Path.Circular && !pathList.End))
			{
				GameObject gameObject = new GameObject("Rail Mesh");
				gameObject.transform.position = meshObject.Position;
				gameObject.tag = "Railway";
				gameObject.layer = 16;
				gameObject.SetHierarchyGroup(pathList.Name, true, false);
				gameObject.SetActive(false);
				MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
				meshCollider.sharedMaterial = this.RailPhysicMaterial;
				meshCollider.sharedMesh = meshObject.Meshes[0];
				gameObject.AddComponent<AddToHeightMap>();
				gameObject.SetActive(true);
			}
			this.AddTrackSpline(pathList);
		}
	}

	// Token: 0x170003B3 RID: 947
	// (get) Token: 0x06002FFF RID: 12287 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool RunOnCache
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06003000 RID: 12288 RVA: 0x00122CB8 File Offset: 0x00120EB8
	private void AddTrackSpline(PathList rail)
	{
		TrainTrackSpline trainTrackSpline = HierarchyUtil.GetRoot(rail.Name, true, false).AddComponent<TrainTrackSpline>();
		trainTrackSpline.aboveGroundSpawn = (rail.Hierarchy == 2);
		trainTrackSpline.hierarchy = rail.Hierarchy;
		if (trainTrackSpline.aboveGroundSpawn)
		{
			TrainTrackSpline.SidingSplines.Add(trainTrackSpline);
		}
		Vector3[] array = new Vector3[rail.Path.Points.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = rail.Path.Points[i];
			Vector3[] array2 = array;
			int num = i;
			array2[num].y = array2[num].y + 0.41f;
		}
		Vector3[] array3 = new Vector3[rail.Path.Tangents.Length];
		for (int j = 0; j < array.Length; j++)
		{
			array3[j] = rail.Path.Tangents[j];
		}
		trainTrackSpline.SetAll(array, array3, 0.25f);
	}

	// Token: 0x040026BB RID: 9915
	public const float NormalSmoothing = 0f;

	// Token: 0x040026BC RID: 9916
	public const bool SnapToTerrain = false;

	// Token: 0x040026BD RID: 9917
	public Mesh RailMesh;

	// Token: 0x040026BE RID: 9918
	public Mesh[] RailMeshes;

	// Token: 0x040026BF RID: 9919
	public Material RailMaterial;

	// Token: 0x040026C0 RID: 9920
	public PhysicMaterial RailPhysicMaterial;
}
