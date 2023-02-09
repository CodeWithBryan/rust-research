using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000297 RID: 663
public class MeshRendererLookup
{
	// Token: 0x06001C25 RID: 7205 RVA: 0x000C30C0 File Offset: 0x000C12C0
	public void Apply()
	{
		MeshRendererLookup.LookupGroup lookupGroup = this.src;
		this.src = this.dst;
		this.dst = lookupGroup;
		this.dst.Clear();
	}

	// Token: 0x06001C26 RID: 7206 RVA: 0x000C30F2 File Offset: 0x000C12F2
	public void Clear()
	{
		this.dst.Clear();
	}

	// Token: 0x06001C27 RID: 7207 RVA: 0x000C30FF File Offset: 0x000C12FF
	public void Add(MeshRendererInstance instance)
	{
		this.dst.Add(instance);
	}

	// Token: 0x06001C28 RID: 7208 RVA: 0x000C310D File Offset: 0x000C130D
	public MeshRendererLookup.LookupEntry Get(int index)
	{
		return this.src.Get(index);
	}

	// Token: 0x0400156B RID: 5483
	public MeshRendererLookup.LookupGroup src = new MeshRendererLookup.LookupGroup();

	// Token: 0x0400156C RID: 5484
	public MeshRendererLookup.LookupGroup dst = new MeshRendererLookup.LookupGroup();

	// Token: 0x02000C41 RID: 3137
	public class LookupGroup
	{
		// Token: 0x06004C5E RID: 19550 RVA: 0x00195855 File Offset: 0x00193A55
		public void Clear()
		{
			this.data.Clear();
		}

		// Token: 0x06004C5F RID: 19551 RVA: 0x00195862 File Offset: 0x00193A62
		public void Add(MeshRendererInstance instance)
		{
			this.data.Add(new MeshRendererLookup.LookupEntry(instance));
		}

		// Token: 0x06004C60 RID: 19552 RVA: 0x00195875 File Offset: 0x00193A75
		public MeshRendererLookup.LookupEntry Get(int index)
		{
			return this.data[index];
		}

		// Token: 0x0400418F RID: 16783
		public List<MeshRendererLookup.LookupEntry> data = new List<MeshRendererLookup.LookupEntry>();
	}

	// Token: 0x02000C42 RID: 3138
	public struct LookupEntry
	{
		// Token: 0x06004C62 RID: 19554 RVA: 0x00195896 File Offset: 0x00193A96
		public LookupEntry(MeshRendererInstance instance)
		{
			this.renderer = instance.renderer;
		}

		// Token: 0x04004190 RID: 16784
		public Renderer renderer;
	}
}
