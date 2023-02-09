using System;
using ConVar;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000903 RID: 2307
public class FoliageGridBatch : MeshBatch
{
	// Token: 0x1700041B RID: 1051
	// (get) Token: 0x060036E4 RID: 14052 RVA: 0x001468E3 File Offset: 0x00144AE3
	public override int VertexCapacity
	{
		get
		{
			return Batching.renderer_capacity;
		}
	}

	// Token: 0x1700041C RID: 1052
	// (get) Token: 0x060036E5 RID: 14053 RVA: 0x001468EA File Offset: 0x00144AEA
	public override int VertexCutoff
	{
		get
		{
			return Batching.renderer_vertices;
		}
	}

	// Token: 0x060036E6 RID: 14054 RVA: 0x001468F1 File Offset: 0x00144AF1
	protected void Awake()
	{
		this.meshFilter = base.GetComponent<MeshFilter>();
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.meshData = new FoliageGridMeshData();
		this.meshGroup = new MeshGroup();
	}

	// Token: 0x060036E7 RID: 14055 RVA: 0x00146924 File Offset: 0x00144B24
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

	// Token: 0x060036E8 RID: 14056 RVA: 0x001469D0 File Offset: 0x00144BD0
	public void Add(MeshInstance instance)
	{
		instance.position -= this.position;
		this.meshGroup.data.Add(instance);
		base.AddVertices(instance.mesh.vertexCount);
	}

	// Token: 0x060036E9 RID: 14057 RVA: 0x00146A1D File Offset: 0x00144C1D
	protected override void AllocMemory()
	{
		this.meshGroup.Alloc();
		this.meshData.Alloc();
	}

	// Token: 0x060036EA RID: 14058 RVA: 0x00146A35 File Offset: 0x00144C35
	protected override void FreeMemory()
	{
		this.meshGroup.Free();
		this.meshData.Free();
	}

	// Token: 0x060036EB RID: 14059 RVA: 0x00146A4D File Offset: 0x00144C4D
	protected override void RefreshMesh()
	{
		this.meshData.Clear();
		this.meshData.Combine(this.meshGroup);
	}

	// Token: 0x060036EC RID: 14060 RVA: 0x00146A6B File Offset: 0x00144C6B
	protected override void ApplyMesh()
	{
		if (!this.meshBatch)
		{
			this.meshBatch = AssetPool.Get<UnityEngine.Mesh>();
		}
		this.meshData.Apply(this.meshBatch);
		this.meshBatch.UploadMeshData(false);
	}

	// Token: 0x060036ED RID: 14061 RVA: 0x00146AA4 File Offset: 0x00144CA4
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

	// Token: 0x060036EE RID: 14062 RVA: 0x00146B20 File Offset: 0x00144D20
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

	// Token: 0x0400319F RID: 12703
	private Vector3 position;

	// Token: 0x040031A0 RID: 12704
	private UnityEngine.Mesh meshBatch;

	// Token: 0x040031A1 RID: 12705
	private MeshFilter meshFilter;

	// Token: 0x040031A2 RID: 12706
	private MeshRenderer meshRenderer;

	// Token: 0x040031A3 RID: 12707
	private FoliageGridMeshData meshData;

	// Token: 0x040031A4 RID: 12708
	private MeshGroup meshGroup;
}
