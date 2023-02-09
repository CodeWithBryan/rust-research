using System;
using ConVar;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000911 RID: 2321
public class MeshDataBatch : MeshBatch
{
	// Token: 0x17000424 RID: 1060
	// (get) Token: 0x0600372A RID: 14122 RVA: 0x001468E3 File Offset: 0x00144AE3
	public override int VertexCapacity
	{
		get
		{
			return Batching.renderer_capacity;
		}
	}

	// Token: 0x17000425 RID: 1061
	// (get) Token: 0x0600372B RID: 14123 RVA: 0x001468EA File Offset: 0x00144AEA
	public override int VertexCutoff
	{
		get
		{
			return Batching.renderer_vertices;
		}
	}

	// Token: 0x0600372C RID: 14124 RVA: 0x00147954 File Offset: 0x00145B54
	protected void Awake()
	{
		this.meshFilter = base.GetComponent<MeshFilter>();
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.meshData = new MeshData();
		this.meshGroup = new MeshGroup();
	}

	// Token: 0x0600372D RID: 14125 RVA: 0x00147984 File Offset: 0x00145B84
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

	// Token: 0x0600372E RID: 14126 RVA: 0x00147A30 File Offset: 0x00145C30
	public void Add(MeshInstance instance)
	{
		instance.position -= this.position;
		this.meshGroup.data.Add(instance);
		base.AddVertices(instance.mesh.vertexCount);
	}

	// Token: 0x0600372F RID: 14127 RVA: 0x00147A7D File Offset: 0x00145C7D
	protected override void AllocMemory()
	{
		this.meshGroup.Alloc();
		this.meshData.Alloc();
	}

	// Token: 0x06003730 RID: 14128 RVA: 0x00147A95 File Offset: 0x00145C95
	protected override void FreeMemory()
	{
		this.meshGroup.Free();
		this.meshData.Free();
	}

	// Token: 0x06003731 RID: 14129 RVA: 0x00147AAD File Offset: 0x00145CAD
	protected override void RefreshMesh()
	{
		this.meshData.Clear();
		this.meshData.Combine(this.meshGroup);
	}

	// Token: 0x06003732 RID: 14130 RVA: 0x00147ACB File Offset: 0x00145CCB
	protected override void ApplyMesh()
	{
		if (!this.meshBatch)
		{
			this.meshBatch = AssetPool.Get<UnityEngine.Mesh>();
		}
		this.meshData.Apply(this.meshBatch);
		this.meshBatch.UploadMeshData(false);
	}

	// Token: 0x06003733 RID: 14131 RVA: 0x00147B04 File Offset: 0x00145D04
	protected override void ToggleMesh(bool state)
	{
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

	// Token: 0x06003734 RID: 14132 RVA: 0x00147B80 File Offset: 0x00145D80
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
	}

	// Token: 0x040031AB RID: 12715
	private Vector3 position;

	// Token: 0x040031AC RID: 12716
	private UnityEngine.Mesh meshBatch;

	// Token: 0x040031AD RID: 12717
	private MeshFilter meshFilter;

	// Token: 0x040031AE RID: 12718
	private MeshRenderer meshRenderer;

	// Token: 0x040031AF RID: 12719
	private MeshData meshData;

	// Token: 0x040031B0 RID: 12720
	private MeshGroup meshGroup;
}
