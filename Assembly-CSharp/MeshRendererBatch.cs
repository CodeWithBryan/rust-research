using System;
using System.Collections.Generic;
using ConVar;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000912 RID: 2322
public class MeshRendererBatch : MeshBatch
{
	// Token: 0x17000426 RID: 1062
	// (get) Token: 0x06003736 RID: 14134 RVA: 0x001468E3 File Offset: 0x00144AE3
	public override int VertexCapacity
	{
		get
		{
			return Batching.renderer_capacity;
		}
	}

	// Token: 0x17000427 RID: 1063
	// (get) Token: 0x06003737 RID: 14135 RVA: 0x001468EA File Offset: 0x00144AEA
	public override int VertexCutoff
	{
		get
		{
			return Batching.renderer_vertices;
		}
	}

	// Token: 0x06003738 RID: 14136 RVA: 0x00147BD4 File Offset: 0x00145DD4
	protected void Awake()
	{
		this.meshFilter = base.GetComponent<MeshFilter>();
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.meshData = new MeshRendererData();
		this.meshGroup = new MeshRendererGroup();
		this.meshLookup = new MeshRendererLookup();
	}

	// Token: 0x06003739 RID: 14137 RVA: 0x00147C10 File Offset: 0x00145E10
	public void Setup(Vector3 position, Material material, ShadowCastingMode shadows, int layer)
	{
		base.transform.position = position;
		this.position = position;
		base.gameObject.layer = layer;
		this.meshRenderer.sharedMaterial = material;
		this.meshRenderer.shadowCastingMode = shadows;
		if (shadows == ShadowCastingMode.ShadowsOnly)
		{
			this.meshRenderer.receiveShadows = false;
			this.meshRenderer.motionVectors = false;
			this.meshRenderer.lightProbeUsage = LightProbeUsage.Off;
			this.meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
			return;
		}
		this.meshRenderer.receiveShadows = true;
		this.meshRenderer.motionVectors = true;
		this.meshRenderer.lightProbeUsage = LightProbeUsage.BlendProbes;
		this.meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.BlendProbes;
	}

	// Token: 0x0600373A RID: 14138 RVA: 0x00147CBC File Offset: 0x00145EBC
	public void Add(MeshRendererInstance instance)
	{
		instance.position -= this.position;
		this.meshGroup.data.Add(instance);
		base.AddVertices(instance.mesh.vertexCount);
	}

	// Token: 0x0600373B RID: 14139 RVA: 0x00147D09 File Offset: 0x00145F09
	protected override void AllocMemory()
	{
		this.meshGroup.Alloc();
		this.meshData.Alloc();
	}

	// Token: 0x0600373C RID: 14140 RVA: 0x00147D21 File Offset: 0x00145F21
	protected override void FreeMemory()
	{
		this.meshGroup.Free();
		this.meshData.Free();
	}

	// Token: 0x0600373D RID: 14141 RVA: 0x00147D39 File Offset: 0x00145F39
	protected override void RefreshMesh()
	{
		this.meshLookup.dst.Clear();
		this.meshData.Clear();
		this.meshData.Combine(this.meshGroup, this.meshLookup);
	}

	// Token: 0x0600373E RID: 14142 RVA: 0x00147D70 File Offset: 0x00145F70
	protected override void ApplyMesh()
	{
		if (!this.meshBatch)
		{
			this.meshBatch = AssetPool.Get<UnityEngine.Mesh>();
		}
		this.meshLookup.Apply();
		this.meshData.Apply(this.meshBatch);
		this.meshBatch.UploadMeshData(false);
	}

	// Token: 0x0600373F RID: 14143 RVA: 0x00147DC0 File Offset: 0x00145FC0
	protected override void ToggleMesh(bool state)
	{
		List<MeshRendererLookup.LookupEntry> data = this.meshLookup.src.data;
		for (int i = 0; i < data.Count; i++)
		{
			Renderer renderer = data[i].renderer;
			if (renderer)
			{
				renderer.enabled = !state;
			}
		}
		if (state)
		{
			if (this.meshFilter)
			{
				this.meshFilter.sharedMesh = this.meshBatch;
			}
			if (this.meshRenderer)
			{
				this.meshRenderer.enabled = true;
				return;
			}
		}
		else
		{
			if (this.meshFilter)
			{
				this.meshFilter.sharedMesh = null;
			}
			if (this.meshRenderer)
			{
				this.meshRenderer.enabled = false;
			}
		}
	}

	// Token: 0x06003740 RID: 14144 RVA: 0x00147E7C File Offset: 0x0014607C
	protected override void OnPooled()
	{
		if (this.meshFilter)
		{
			this.meshFilter.sharedMesh = null;
		}
		if (this.meshBatch)
		{
			AssetPool.Free(ref this.meshBatch);
		}
		this.meshData.Free();
		this.meshGroup.Free();
		this.meshLookup.src.Clear();
		this.meshLookup.dst.Clear();
	}

	// Token: 0x040031B1 RID: 12721
	private Vector3 position;

	// Token: 0x040031B2 RID: 12722
	private UnityEngine.Mesh meshBatch;

	// Token: 0x040031B3 RID: 12723
	private MeshFilter meshFilter;

	// Token: 0x040031B4 RID: 12724
	private MeshRenderer meshRenderer;

	// Token: 0x040031B5 RID: 12725
	private MeshRendererData meshData;

	// Token: 0x040031B6 RID: 12726
	private MeshRendererGroup meshGroup;

	// Token: 0x040031B7 RID: 12727
	private MeshRendererLookup meshLookup;
}
