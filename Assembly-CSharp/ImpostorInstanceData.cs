using System;
using UnityEngine;

// Token: 0x020006FC RID: 1788
public class ImpostorInstanceData
{
	// Token: 0x170003DD RID: 989
	// (get) Token: 0x0600319A RID: 12698 RVA: 0x00130A19 File Offset: 0x0012EC19
	// (set) Token: 0x06003199 RID: 12697 RVA: 0x00130A10 File Offset: 0x0012EC10
	public Renderer Renderer { get; private set; }

	// Token: 0x170003DE RID: 990
	// (get) Token: 0x0600319C RID: 12700 RVA: 0x00130A2A File Offset: 0x0012EC2A
	// (set) Token: 0x0600319B RID: 12699 RVA: 0x00130A21 File Offset: 0x0012EC21
	public Mesh Mesh { get; private set; }

	// Token: 0x170003DF RID: 991
	// (get) Token: 0x0600319E RID: 12702 RVA: 0x00130A3B File Offset: 0x0012EC3B
	// (set) Token: 0x0600319D RID: 12701 RVA: 0x00130A32 File Offset: 0x0012EC32
	public Material Material { get; private set; }

	// Token: 0x0600319F RID: 12703 RVA: 0x00130A43 File Offset: 0x0012EC43
	public ImpostorInstanceData(Renderer renderer, Mesh mesh, Material material)
	{
		this.Renderer = renderer;
		this.Mesh = mesh;
		this.Material = material;
		this.hash = this.GenerateHashCode();
		this.Update();
	}

	// Token: 0x060031A0 RID: 12704 RVA: 0x00130A80 File Offset: 0x0012EC80
	public ImpostorInstanceData(Vector3 position, Vector3 scale, Mesh mesh, Material material)
	{
		this.positionAndScale = new Vector4(position.x, position.y, position.z, scale.x);
		this.Mesh = mesh;
		this.Material = material;
		this.hash = this.GenerateHashCode();
		this.Update();
	}

	// Token: 0x060031A1 RID: 12705 RVA: 0x00130AE2 File Offset: 0x0012ECE2
	private int GenerateHashCode()
	{
		return (17 * 31 + this.Material.GetHashCode()) * 31 + this.Mesh.GetHashCode();
	}

	// Token: 0x060031A2 RID: 12706 RVA: 0x00130B04 File Offset: 0x0012ED04
	public override bool Equals(object obj)
	{
		ImpostorInstanceData impostorInstanceData = obj as ImpostorInstanceData;
		return impostorInstanceData.Material == this.Material && impostorInstanceData.Mesh == this.Mesh;
	}

	// Token: 0x060031A3 RID: 12707 RVA: 0x00130B3E File Offset: 0x0012ED3E
	public override int GetHashCode()
	{
		return this.hash;
	}

	// Token: 0x060031A4 RID: 12708 RVA: 0x00130B48 File Offset: 0x0012ED48
	public Vector4 PositionAndScale()
	{
		if (this.Renderer != null)
		{
			Transform transform = this.Renderer.transform;
			Vector3 position = transform.position;
			Vector3 lossyScale = transform.lossyScale;
			float w = this.Renderer.enabled ? lossyScale.x : (-lossyScale.x);
			this.positionAndScale = new Vector4(position.x, position.y, position.z, w);
		}
		return this.positionAndScale;
	}

	// Token: 0x060031A5 RID: 12709 RVA: 0x00130BBC File Offset: 0x0012EDBC
	public void Update()
	{
		if (this.Batch != null)
		{
			this.Batch.Positions[this.BatchIndex] = this.PositionAndScale();
			this.Batch.IsDirty = true;
		}
	}

	// Token: 0x04002848 RID: 10312
	public ImpostorBatch Batch;

	// Token: 0x04002849 RID: 10313
	public int BatchIndex;

	// Token: 0x0400284A RID: 10314
	private int hash;

	// Token: 0x0400284B RID: 10315
	private Vector4 positionAndScale = Vector4.zero;
}
