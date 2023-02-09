using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000278 RID: 632
public class BoneDictionary
{
	// Token: 0x17000217 RID: 535
	// (get) Token: 0x06001BDF RID: 7135 RVA: 0x000C188F File Offset: 0x000BFA8F
	public int Count
	{
		get
		{
			return this.transforms.Length;
		}
	}

	// Token: 0x06001BE0 RID: 7136 RVA: 0x000C189C File Offset: 0x000BFA9C
	public BoneDictionary(Transform rootBone)
	{
		this.transform = rootBone;
		this.transforms = rootBone.GetComponentsInChildren<Transform>(true);
		this.names = new string[this.transforms.Length];
		for (int i = 0; i < this.transforms.Length; i++)
		{
			Transform transform = this.transforms[i];
			if (transform != null)
			{
				this.names[i] = transform.name;
			}
		}
		this.BuildBoneDictionary();
	}

	// Token: 0x06001BE1 RID: 7137 RVA: 0x000C1938 File Offset: 0x000BFB38
	public BoneDictionary(Transform rootBone, Transform[] boneTransforms, string[] boneNames)
	{
		this.transform = rootBone;
		this.transforms = boneTransforms;
		this.names = boneNames;
		this.BuildBoneDictionary();
	}

	// Token: 0x06001BE2 RID: 7138 RVA: 0x000C198C File Offset: 0x000BFB8C
	private void BuildBoneDictionary()
	{
		for (int i = 0; i < this.transforms.Length; i++)
		{
			Transform transform = this.transforms[i];
			string text = this.names[i];
			uint num = StringPool.Get(text);
			if (!this.nameDict.ContainsKey(text))
			{
				this.nameDict.Add(text, transform);
			}
			if (!this.hashDict.ContainsKey(num))
			{
				this.hashDict.Add(num, transform);
			}
			if (transform != null && !this.transformDict.ContainsKey(transform))
			{
				this.transformDict.Add(transform, num);
			}
		}
	}

	// Token: 0x06001BE3 RID: 7139 RVA: 0x000C1A24 File Offset: 0x000BFC24
	public Transform FindBone(string name, bool defaultToRoot = true)
	{
		Transform result = null;
		if (this.nameDict.TryGetValue(name, out result))
		{
			return result;
		}
		if (!defaultToRoot)
		{
			return null;
		}
		return this.transform;
	}

	// Token: 0x06001BE4 RID: 7140 RVA: 0x000C1A50 File Offset: 0x000BFC50
	public Transform FindBone(uint hash, bool defaultToRoot = true)
	{
		Transform result = null;
		if (this.hashDict.TryGetValue(hash, out result))
		{
			return result;
		}
		if (!defaultToRoot)
		{
			return null;
		}
		return this.transform;
	}

	// Token: 0x06001BE5 RID: 7141 RVA: 0x000C1A7C File Offset: 0x000BFC7C
	public uint FindBoneID(Transform transform)
	{
		uint result;
		if (!this.transformDict.TryGetValue(transform, out result))
		{
			return StringPool.closest;
		}
		return result;
	}

	// Token: 0x04001505 RID: 5381
	public Transform transform;

	// Token: 0x04001506 RID: 5382
	public Transform[] transforms;

	// Token: 0x04001507 RID: 5383
	public string[] names;

	// Token: 0x04001508 RID: 5384
	private Dictionary<string, Transform> nameDict = new Dictionary<string, Transform>(StringComparer.OrdinalIgnoreCase);

	// Token: 0x04001509 RID: 5385
	private Dictionary<uint, Transform> hashDict = new Dictionary<uint, Transform>();

	// Token: 0x0400150A RID: 5386
	private Dictionary<Transform, uint> transformDict = new Dictionary<Transform, uint>();
}
