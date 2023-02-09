using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000532 RID: 1330
[CreateAssetMenu(menuName = "Rust/Skeleton Properties")]
public class SkeletonProperties : ScriptableObject
{
	// Token: 0x060028B6 RID: 10422 RVA: 0x000F7BA4 File Offset: 0x000F5DA4
	public void OnValidate()
	{
		if (this.boneReference == null)
		{
			Debug.LogWarning("boneReference is null", this);
			return;
		}
		List<SkeletonProperties.BoneProperty> list = this.bones.ToList<SkeletonProperties.BoneProperty>();
		using (List<Transform>.Enumerator enumerator = this.boneReference.transform.GetAllChildren().GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Transform child = enumerator.Current;
				if (list.All((SkeletonProperties.BoneProperty x) => x.bone != child.gameObject))
				{
					list.Add(new SkeletonProperties.BoneProperty
					{
						bone = child.gameObject,
						name = new Translate.Phrase("", "")
						{
							token = child.name.ToLower(),
							english = child.name.ToLower()
						}
					});
				}
			}
		}
		this.bones = list.ToArray();
	}

	// Token: 0x060028B7 RID: 10423 RVA: 0x000F7CB0 File Offset: 0x000F5EB0
	private void BuildDictionary()
	{
		this.quickLookup = new Dictionary<uint, SkeletonProperties.BoneProperty>();
		foreach (SkeletonProperties.BoneProperty boneProperty in this.bones)
		{
			if (boneProperty == null || boneProperty.bone == null || boneProperty.bone.name == null)
			{
				Debug.LogWarning("Bone error in SkeletonProperties.BuildDictionary for " + ((this.boneReference != null) ? this.boneReference.name : "?"));
			}
			else
			{
				uint num = StringPool.Get(boneProperty.bone.name);
				if (!this.quickLookup.ContainsKey(num))
				{
					this.quickLookup.Add(num, boneProperty);
				}
				else
				{
					string name = boneProperty.bone.name;
					string name2 = this.quickLookup[num].bone.name;
					Debug.LogWarning(string.Concat(new object[]
					{
						"Duplicate bone id ",
						num,
						" for ",
						name,
						" and ",
						name2
					}));
				}
			}
		}
	}

	// Token: 0x060028B8 RID: 10424 RVA: 0x000F7DC8 File Offset: 0x000F5FC8
	public SkeletonProperties.BoneProperty FindBone(uint id)
	{
		if (this.quickLookup == null)
		{
			this.BuildDictionary();
		}
		SkeletonProperties.BoneProperty result = null;
		if (!this.quickLookup.TryGetValue(id, out result))
		{
			return null;
		}
		return result;
	}

	// Token: 0x04002114 RID: 8468
	public GameObject boneReference;

	// Token: 0x04002115 RID: 8469
	[BoneProperty]
	public SkeletonProperties.BoneProperty[] bones;

	// Token: 0x04002116 RID: 8470
	[NonSerialized]
	private Dictionary<uint, SkeletonProperties.BoneProperty> quickLookup;

	// Token: 0x02000CEF RID: 3311
	[Serializable]
	public class BoneProperty
	{
		// Token: 0x04004456 RID: 17494
		public GameObject bone;

		// Token: 0x04004457 RID: 17495
		public Translate.Phrase name;

		// Token: 0x04004458 RID: 17496
		public HitArea area;
	}
}
