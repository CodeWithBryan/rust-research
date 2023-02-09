using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000298 RID: 664
public class RendererInfo : ComponentInfo<Renderer>
{
	// Token: 0x06001C2A RID: 7210 RVA: 0x000C313C File Offset: 0x000C133C
	public override void Reset()
	{
		this.component.shadowCastingMode = this.shadows;
		if (this.material)
		{
			this.component.sharedMaterial = this.material;
		}
		SkinnedMeshRenderer skinnedMeshRenderer;
		if ((skinnedMeshRenderer = (this.component as SkinnedMeshRenderer)) != null)
		{
			skinnedMeshRenderer.sharedMesh = this.mesh;
			return;
		}
		if (this.component is MeshRenderer)
		{
			this.meshFilter.sharedMesh = this.mesh;
		}
	}

	// Token: 0x06001C2B RID: 7211 RVA: 0x000C31B4 File Offset: 0x000C13B4
	public override void Setup()
	{
		this.shadows = this.component.shadowCastingMode;
		this.material = this.component.sharedMaterial;
		SkinnedMeshRenderer skinnedMeshRenderer;
		if ((skinnedMeshRenderer = (this.component as SkinnedMeshRenderer)) != null)
		{
			this.mesh = skinnedMeshRenderer.sharedMesh;
			return;
		}
		if (this.component is MeshRenderer)
		{
			this.meshFilter = base.GetComponent<MeshFilter>();
			this.mesh = this.meshFilter.sharedMesh;
		}
	}

	// Token: 0x0400156D RID: 5485
	public ShadowCastingMode shadows;

	// Token: 0x0400156E RID: 5486
	public Material material;

	// Token: 0x0400156F RID: 5487
	public Mesh mesh;

	// Token: 0x04001570 RID: 5488
	public MeshFilter meshFilter;
}
