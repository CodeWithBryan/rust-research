using System;
using Facepunch;
using UnityEngine;

// Token: 0x020002B6 RID: 694
public class Model : MonoBehaviour, IPrefabPreProcess
{
	// Token: 0x06001C66 RID: 7270 RVA: 0x000C3B17 File Offset: 0x000C1D17
	protected void OnEnable()
	{
		this.skin = -1;
	}

	// Token: 0x06001C67 RID: 7271 RVA: 0x000C3B20 File Offset: 0x000C1D20
	public void BuildBoneDictionary()
	{
		if (this.boneDict != null)
		{
			return;
		}
		this.boneDict = new BoneDictionary(base.transform, this.boneTransforms, this.boneNames);
	}

	// Token: 0x06001C68 RID: 7272 RVA: 0x000C3B48 File Offset: 0x000C1D48
	public int GetSkin()
	{
		return this.skin;
	}

	// Token: 0x06001C69 RID: 7273 RVA: 0x000C3B50 File Offset: 0x000C1D50
	private Transform FindBoneInternal(string name)
	{
		this.BuildBoneDictionary();
		return this.boneDict.FindBone(name, false);
	}

	// Token: 0x06001C6A RID: 7274 RVA: 0x000C3B68 File Offset: 0x000C1D68
	public Transform FindBone(string name)
	{
		this.BuildBoneDictionary();
		Transform result = this.rootBone;
		if (string.IsNullOrEmpty(name))
		{
			return result;
		}
		return this.boneDict.FindBone(name, true);
	}

	// Token: 0x06001C6B RID: 7275 RVA: 0x000C3B9C File Offset: 0x000C1D9C
	public Transform FindBone(uint hash)
	{
		this.BuildBoneDictionary();
		Transform result = this.rootBone;
		if (hash == 0U)
		{
			return result;
		}
		return this.boneDict.FindBone(hash, true);
	}

	// Token: 0x06001C6C RID: 7276 RVA: 0x000C3BCA File Offset: 0x000C1DCA
	public uint FindBoneID(Transform transform)
	{
		this.BuildBoneDictionary();
		return this.boneDict.FindBoneID(transform);
	}

	// Token: 0x06001C6D RID: 7277 RVA: 0x000C3BDE File Offset: 0x000C1DDE
	public Transform[] GetBones()
	{
		this.BuildBoneDictionary();
		return this.boneDict.transforms;
	}

	// Token: 0x06001C6E RID: 7278 RVA: 0x000C3BF4 File Offset: 0x000C1DF4
	public Transform FindClosestBone(Vector3 worldPos)
	{
		Transform result = this.rootBone;
		float num = float.MaxValue;
		for (int i = 0; i < this.boneTransforms.Length; i++)
		{
			Transform transform = this.boneTransforms[i];
			if (!(transform == null))
			{
				float num2 = Vector3.Distance(transform.position, worldPos);
				if (num2 < num)
				{
					result = transform;
					num = num2;
				}
			}
		}
		return result;
	}

	// Token: 0x06001C6F RID: 7279 RVA: 0x000C3C4C File Offset: 0x000C1E4C
	public void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (this == null)
		{
			return;
		}
		if (this.animator == null)
		{
			this.animator = base.GetComponent<Animator>();
		}
		if (this.rootBone == null)
		{
			this.rootBone = base.transform;
		}
		this.boneTransforms = this.rootBone.GetComponentsInChildren<Transform>(true);
		this.boneNames = new string[this.boneTransforms.Length];
		for (int i = 0; i < this.boneTransforms.Length; i++)
		{
			this.boneNames[i] = this.boneTransforms[i].name;
		}
	}

	// Token: 0x040015DD RID: 5597
	public SphereCollider collision;

	// Token: 0x040015DE RID: 5598
	public Transform rootBone;

	// Token: 0x040015DF RID: 5599
	public Transform headBone;

	// Token: 0x040015E0 RID: 5600
	public Transform eyeBone;

	// Token: 0x040015E1 RID: 5601
	public Animator animator;

	// Token: 0x040015E2 RID: 5602
	public Skeleton skeleton;

	// Token: 0x040015E3 RID: 5603
	[HideInInspector]
	public Transform[] boneTransforms;

	// Token: 0x040015E4 RID: 5604
	[HideInInspector]
	public string[] boneNames;

	// Token: 0x040015E5 RID: 5605
	internal BoneDictionary boneDict;

	// Token: 0x040015E6 RID: 5606
	internal int skin;
}
