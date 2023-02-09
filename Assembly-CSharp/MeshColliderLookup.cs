using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000290 RID: 656
public class MeshColliderLookup
{
	// Token: 0x06001C0D RID: 7181 RVA: 0x000C20F0 File Offset: 0x000C02F0
	public void Apply()
	{
		MeshColliderLookup.LookupGroup lookupGroup = this.src;
		this.src = this.dst;
		this.dst = lookupGroup;
		this.dst.Clear();
	}

	// Token: 0x06001C0E RID: 7182 RVA: 0x000C2122 File Offset: 0x000C0322
	public void Add(MeshColliderInstance instance)
	{
		this.dst.Add(instance);
	}

	// Token: 0x06001C0F RID: 7183 RVA: 0x000C2130 File Offset: 0x000C0330
	public MeshColliderLookup.LookupEntry Get(int index)
	{
		return this.src.Get(index);
	}

	// Token: 0x0400154F RID: 5455
	public MeshColliderLookup.LookupGroup src = new MeshColliderLookup.LookupGroup();

	// Token: 0x04001550 RID: 5456
	public MeshColliderLookup.LookupGroup dst = new MeshColliderLookup.LookupGroup();

	// Token: 0x02000C3F RID: 3135
	public class LookupGroup
	{
		// Token: 0x06004C59 RID: 19545 RVA: 0x0019577D File Offset: 0x0019397D
		public void Clear()
		{
			this.data.Clear();
			this.indices.Clear();
		}

		// Token: 0x06004C5A RID: 19546 RVA: 0x00195798 File Offset: 0x00193998
		public void Add(MeshColliderInstance instance)
		{
			this.data.Add(new MeshColliderLookup.LookupEntry(instance));
			int item = this.data.Count - 1;
			int num = instance.data.triangles.Length / 3;
			for (int i = 0; i < num; i++)
			{
				this.indices.Add(item);
			}
		}

		// Token: 0x06004C5B RID: 19547 RVA: 0x001957EC File Offset: 0x001939EC
		public MeshColliderLookup.LookupEntry Get(int index)
		{
			return this.data[this.indices[index]];
		}

		// Token: 0x04004189 RID: 16777
		public List<MeshColliderLookup.LookupEntry> data = new List<MeshColliderLookup.LookupEntry>();

		// Token: 0x0400418A RID: 16778
		public List<int> indices = new List<int>();
	}

	// Token: 0x02000C40 RID: 3136
	public struct LookupEntry
	{
		// Token: 0x06004C5D RID: 19549 RVA: 0x00195823 File Offset: 0x00193A23
		public LookupEntry(MeshColliderInstance instance)
		{
			this.transform = instance.transform;
			this.rigidbody = instance.rigidbody;
			this.collider = instance.collider;
			this.bounds = instance.bounds;
		}

		// Token: 0x0400418B RID: 16779
		public Transform transform;

		// Token: 0x0400418C RID: 16780
		public Rigidbody rigidbody;

		// Token: 0x0400418D RID: 16781
		public Collider collider;

		// Token: 0x0400418E RID: 16782
		public OBB bounds;
	}
}
