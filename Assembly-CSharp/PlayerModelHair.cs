using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020002BA RID: 698
public class PlayerModelHair : MonoBehaviour
{
	// Token: 0x17000223 RID: 547
	// (get) Token: 0x06001C78 RID: 7288 RVA: 0x000C3D9C File Offset: 0x000C1F9C
	public Dictionary<Renderer, PlayerModelHair.RendererMaterials> Materials
	{
		get
		{
			return this.materials;
		}
	}

	// Token: 0x06001C79 RID: 7289 RVA: 0x000C3DA4 File Offset: 0x000C1FA4
	private void CacheOriginalMaterials()
	{
		if (this.materials != null)
		{
			return;
		}
		List<SkinnedMeshRenderer> list = Pool.GetList<SkinnedMeshRenderer>();
		base.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true, list);
		this.materials = new Dictionary<Renderer, PlayerModelHair.RendererMaterials>();
		this.materials.Clear();
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in list)
		{
			this.materials.Add(skinnedMeshRenderer, new PlayerModelHair.RendererMaterials(skinnedMeshRenderer));
		}
		Pool.FreeList<SkinnedMeshRenderer>(ref list);
	}

	// Token: 0x06001C7A RID: 7290 RVA: 0x000C3E38 File Offset: 0x000C2038
	private void Setup(HairType type, HairSetCollection hair, int meshIndex, float typeNum, float dyeNum, MaterialPropertyBlock block)
	{
		this.CacheOriginalMaterials();
		HairSetCollection.HairSetEntry hairSetEntry = hair.Get(type, typeNum);
		if (hairSetEntry.HairSet == null)
		{
			Debug.LogWarning("Hair.Get returned a NULL hair");
			return;
		}
		int blendShapeIndex = -1;
		if (type == HairType.Facial || type == HairType.Eyebrow)
		{
			blendShapeIndex = meshIndex;
		}
		HairDye dye = null;
		HairDyeCollection hairDyeCollection = hairSetEntry.HairDyeCollection;
		if (hairDyeCollection != null)
		{
			dye = hairDyeCollection.Get(dyeNum);
		}
		hairSetEntry.HairSet.Process(this, hairDyeCollection, dye, block);
		hairSetEntry.HairSet.ProcessMorphs(base.gameObject, blendShapeIndex);
	}

	// Token: 0x06001C7B RID: 7291 RVA: 0x000C3EB8 File Offset: 0x000C20B8
	public void Setup(SkinSetCollection skin, float hairNum, float meshNum, MaterialPropertyBlock block)
	{
		int index = skin.GetIndex(meshNum);
		SkinSet skinSet = skin.Skins[index];
		if (skinSet == null)
		{
			Debug.LogError("Skin.Get returned a NULL skin");
			return;
		}
		int typeIndex = (int)this.type;
		float typeNum;
		float dyeNum;
		PlayerModelHair.GetRandomVariation(hairNum, typeIndex, index, out typeNum, out dyeNum);
		this.Setup(this.type, skinSet.HairCollection, index, typeNum, dyeNum, block);
	}

	// Token: 0x06001C7C RID: 7292 RVA: 0x000C3F15 File Offset: 0x000C2115
	public static void GetRandomVariation(float hairNum, int typeIndex, int meshIndex, out float typeNum, out float dyeNum)
	{
		int num = Mathf.FloorToInt(hairNum * 100000f);
		typeNum = PlayerModelHair.GetRandomHairType(hairNum, typeIndex);
		UnityEngine.Random.InitState(num + meshIndex);
		dyeNum = UnityEngine.Random.Range(0f, 1f);
	}

	// Token: 0x06001C7D RID: 7293 RVA: 0x000C3F45 File Offset: 0x000C2145
	public static float GetRandomHairType(float hairNum, int typeIndex)
	{
		UnityEngine.Random.InitState(Mathf.FloorToInt(hairNum * 100000f) + typeIndex);
		return UnityEngine.Random.Range(0f, 1f);
	}

	// Token: 0x040015EC RID: 5612
	public HairType type;

	// Token: 0x040015ED RID: 5613
	private Dictionary<Renderer, PlayerModelHair.RendererMaterials> materials;

	// Token: 0x02000C45 RID: 3141
	public struct RendererMaterials
	{
		// Token: 0x06004C66 RID: 19558 RVA: 0x001958C0 File Offset: 0x00193AC0
		public RendererMaterials(Renderer r)
		{
			this.original = r.sharedMaterials;
			this.replacement = (this.original.Clone() as Material[]);
			this.names = new string[this.original.Length];
			for (int i = 0; i < this.original.Length; i++)
			{
				this.names[i] = this.original[i].name;
			}
		}

		// Token: 0x04004195 RID: 16789
		public string[] names;

		// Token: 0x04004196 RID: 16790
		public Material[] original;

		// Token: 0x04004197 RID: 16791
		public Material[] replacement;
	}
}
