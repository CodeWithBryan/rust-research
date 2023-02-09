using System;
using Rust;
using UnityEngine;

// Token: 0x02000910 RID: 2320
public abstract class MeshBatch : MonoBehaviour
{
	// Token: 0x1700041D RID: 1053
	// (get) Token: 0x0600370F RID: 14095 RVA: 0x00147829 File Offset: 0x00145A29
	// (set) Token: 0x06003710 RID: 14096 RVA: 0x00147831 File Offset: 0x00145A31
	public bool NeedsRefresh { get; private set; }

	// Token: 0x1700041E RID: 1054
	// (get) Token: 0x06003711 RID: 14097 RVA: 0x0014783A File Offset: 0x00145A3A
	// (set) Token: 0x06003712 RID: 14098 RVA: 0x00147842 File Offset: 0x00145A42
	public int Count { get; private set; }

	// Token: 0x1700041F RID: 1055
	// (get) Token: 0x06003713 RID: 14099 RVA: 0x0014784B File Offset: 0x00145A4B
	// (set) Token: 0x06003714 RID: 14100 RVA: 0x00147853 File Offset: 0x00145A53
	public int BatchedCount { get; private set; }

	// Token: 0x17000420 RID: 1056
	// (get) Token: 0x06003715 RID: 14101 RVA: 0x0014785C File Offset: 0x00145A5C
	// (set) Token: 0x06003716 RID: 14102 RVA: 0x00147864 File Offset: 0x00145A64
	public int VertexCount { get; private set; }

	// Token: 0x06003717 RID: 14103
	protected abstract void AllocMemory();

	// Token: 0x06003718 RID: 14104
	protected abstract void FreeMemory();

	// Token: 0x06003719 RID: 14105
	protected abstract void RefreshMesh();

	// Token: 0x0600371A RID: 14106
	protected abstract void ApplyMesh();

	// Token: 0x0600371B RID: 14107
	protected abstract void ToggleMesh(bool state);

	// Token: 0x0600371C RID: 14108
	protected abstract void OnPooled();

	// Token: 0x17000421 RID: 1057
	// (get) Token: 0x0600371D RID: 14109
	public abstract int VertexCapacity { get; }

	// Token: 0x17000422 RID: 1058
	// (get) Token: 0x0600371E RID: 14110
	public abstract int VertexCutoff { get; }

	// Token: 0x17000423 RID: 1059
	// (get) Token: 0x0600371F RID: 14111 RVA: 0x0014786D File Offset: 0x00145A6D
	public int AvailableVertices
	{
		get
		{
			return Mathf.Clamp(this.VertexCapacity, this.VertexCutoff, 65534) - this.VertexCount;
		}
	}

	// Token: 0x06003720 RID: 14112 RVA: 0x0014788C File Offset: 0x00145A8C
	public void Alloc()
	{
		this.AllocMemory();
	}

	// Token: 0x06003721 RID: 14113 RVA: 0x00147894 File Offset: 0x00145A94
	public void Free()
	{
		this.FreeMemory();
	}

	// Token: 0x06003722 RID: 14114 RVA: 0x0014789C File Offset: 0x00145A9C
	public void Refresh()
	{
		this.RefreshMesh();
	}

	// Token: 0x06003723 RID: 14115 RVA: 0x001478A4 File Offset: 0x00145AA4
	public void Apply()
	{
		this.NeedsRefresh = false;
		this.ApplyMesh();
	}

	// Token: 0x06003724 RID: 14116 RVA: 0x001478B3 File Offset: 0x00145AB3
	public void Display()
	{
		this.ToggleMesh(true);
		this.BatchedCount = this.Count;
	}

	// Token: 0x06003725 RID: 14117 RVA: 0x001478C8 File Offset: 0x00145AC8
	public void Invalidate()
	{
		this.ToggleMesh(false);
		this.BatchedCount = 0;
	}

	// Token: 0x06003726 RID: 14118 RVA: 0x001478D8 File Offset: 0x00145AD8
	protected void AddVertices(int vertices)
	{
		this.NeedsRefresh = true;
		int count = this.Count;
		this.Count = count + 1;
		this.VertexCount += vertices;
	}

	// Token: 0x06003727 RID: 14119 RVA: 0x0014790A File Offset: 0x00145B0A
	protected void OnEnable()
	{
		this.NeedsRefresh = false;
		this.Count = 0;
		this.BatchedCount = 0;
		this.VertexCount = 0;
	}

	// Token: 0x06003728 RID: 14120 RVA: 0x00147928 File Offset: 0x00145B28
	protected void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		this.NeedsRefresh = false;
		this.Count = 0;
		this.BatchedCount = 0;
		this.VertexCount = 0;
		this.OnPooled();
	}
}
