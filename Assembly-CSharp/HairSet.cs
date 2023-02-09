using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000723 RID: 1827
[CreateAssetMenu(menuName = "Rust/Hair Set")]
public class HairSet : ScriptableObject
{
	// Token: 0x060032BE RID: 12990 RVA: 0x00139ACC File Offset: 0x00137CCC
	public void Process(PlayerModelHair playerModelHair, HairDyeCollection dyeCollection, HairDye dye, MaterialPropertyBlock block)
	{
		List<SkinnedMeshRenderer> list = Pool.GetList<SkinnedMeshRenderer>();
		playerModelHair.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true, list);
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in list)
		{
			if (!(skinnedMeshRenderer.sharedMesh == null) && !(skinnedMeshRenderer.sharedMaterial == null))
			{
				string name = skinnedMeshRenderer.sharedMesh.name;
				string name2 = skinnedMeshRenderer.sharedMaterial.name;
				if (!skinnedMeshRenderer.gameObject.activeSelf)
				{
					skinnedMeshRenderer.gameObject.SetActive(true);
				}
				for (int i = 0; i < this.MeshReplacements.Length; i++)
				{
					this.MeshReplacements[i].Test(name);
				}
				if (dye != null && skinnedMeshRenderer.gameObject.activeSelf)
				{
					dye.Apply(dyeCollection, block);
				}
			}
		}
		Pool.FreeList<SkinnedMeshRenderer>(ref list);
	}

	// Token: 0x060032BF RID: 12991 RVA: 0x000059DD File Offset: 0x00003BDD
	public void ProcessMorphs(GameObject obj, int blendShapeIndex = -1)
	{
	}

	// Token: 0x0400290A RID: 10506
	public HairSet.MeshReplace[] MeshReplacements;

	// Token: 0x02000E05 RID: 3589
	[Serializable]
	public class MeshReplace
	{
		// Token: 0x06004FC6 RID: 20422 RVA: 0x001A037D File Offset: 0x0019E57D
		public bool Test(string materialName)
		{
			return this.FindName == materialName;
		}

		// Token: 0x040048C3 RID: 18627
		[HideInInspector]
		public string FindName;

		// Token: 0x040048C4 RID: 18628
		public Mesh Find;

		// Token: 0x040048C5 RID: 18629
		public Mesh[] ReplaceShapes;
	}
}
