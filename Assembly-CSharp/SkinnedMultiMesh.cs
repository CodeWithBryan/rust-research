using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020002CB RID: 715
public class SkinnedMultiMesh : MonoBehaviour
{
	// Token: 0x17000226 RID: 550
	// (get) Token: 0x06001CB9 RID: 7353 RVA: 0x000C4ED9 File Offset: 0x000C30D9
	public List<Renderer> Renderers { get; } = new List<Renderer>(32);

	// Token: 0x0400165F RID: 5727
	public bool shadowOnly;

	// Token: 0x04001660 RID: 5728
	internal bool IsVisible = true;

	// Token: 0x04001661 RID: 5729
	public bool eyesView;

	// Token: 0x04001662 RID: 5730
	public Skeleton skeleton;

	// Token: 0x04001663 RID: 5731
	public SkeletonSkinLod skeletonSkinLod;

	// Token: 0x04001664 RID: 5732
	public List<SkinnedMultiMesh.Part> parts = new List<SkinnedMultiMesh.Part>();

	// Token: 0x04001665 RID: 5733
	[NonSerialized]
	public List<SkinnedMultiMesh.Part> createdParts = new List<SkinnedMultiMesh.Part>();

	// Token: 0x04001666 RID: 5734
	[NonSerialized]
	public long lastBuildHash;

	// Token: 0x04001667 RID: 5735
	[NonSerialized]
	public MaterialPropertyBlock sharedPropertyBlock;

	// Token: 0x04001668 RID: 5736
	[NonSerialized]
	public MaterialPropertyBlock hairPropertyBlock;

	// Token: 0x04001669 RID: 5737
	public float skinNumber;

	// Token: 0x0400166A RID: 5738
	public float meshNumber;

	// Token: 0x0400166B RID: 5739
	public float hairNumber;

	// Token: 0x0400166C RID: 5740
	public int skinType;

	// Token: 0x0400166D RID: 5741
	public SkinSetCollection SkinCollection;

	// Token: 0x02000C48 RID: 3144
	public struct Part
	{
		// Token: 0x040041A1 RID: 16801
		public Wearable wearable;

		// Token: 0x040041A2 RID: 16802
		public GameObject gameObject;

		// Token: 0x040041A3 RID: 16803
		public string name;

		// Token: 0x040041A4 RID: 16804
		public Item item;
	}
}
