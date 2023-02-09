using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020006FD RID: 1789
public class ImpostorBatch
{
	// Token: 0x170003E0 RID: 992
	// (get) Token: 0x060031A7 RID: 12711 RVA: 0x00130BF7 File Offset: 0x0012EDF7
	// (set) Token: 0x060031A6 RID: 12710 RVA: 0x00130BEE File Offset: 0x0012EDEE
	public Mesh Mesh { get; private set; }

	// Token: 0x170003E1 RID: 993
	// (get) Token: 0x060031A9 RID: 12713 RVA: 0x00130C08 File Offset: 0x0012EE08
	// (set) Token: 0x060031A8 RID: 12712 RVA: 0x00130BFF File Offset: 0x0012EDFF
	public Material Material { get; private set; }

	// Token: 0x170003E2 RID: 994
	// (get) Token: 0x060031AB RID: 12715 RVA: 0x00130C19 File Offset: 0x0012EE19
	// (set) Token: 0x060031AA RID: 12714 RVA: 0x00130C10 File Offset: 0x0012EE10
	public ComputeBuffer PositionBuffer { get; private set; }

	// Token: 0x170003E3 RID: 995
	// (get) Token: 0x060031AD RID: 12717 RVA: 0x00130C2A File Offset: 0x0012EE2A
	// (set) Token: 0x060031AC RID: 12716 RVA: 0x00130C21 File Offset: 0x0012EE21
	public ComputeBuffer ArgsBuffer { get; private set; }

	// Token: 0x170003E4 RID: 996
	// (get) Token: 0x060031AE RID: 12718 RVA: 0x00130C32 File Offset: 0x0012EE32
	// (set) Token: 0x060031AF RID: 12719 RVA: 0x00130C3A File Offset: 0x0012EE3A
	public bool IsDirty { get; set; }

	// Token: 0x170003E5 RID: 997
	// (get) Token: 0x060031B0 RID: 12720 RVA: 0x00130C43 File Offset: 0x0012EE43
	public int Count
	{
		get
		{
			return this.Positions.Count;
		}
	}

	// Token: 0x170003E6 RID: 998
	// (get) Token: 0x060031B1 RID: 12721 RVA: 0x00130C50 File Offset: 0x0012EE50
	public bool Visible
	{
		get
		{
			return this.Positions.Count - this.recycle.Count > 0;
		}
	}

	// Token: 0x060031B2 RID: 12722 RVA: 0x00130C6C File Offset: 0x0012EE6C
	private ComputeBuffer SafeRelease(ComputeBuffer buffer)
	{
		if (buffer != null)
		{
			buffer.Release();
		}
		return null;
	}

	// Token: 0x060031B3 RID: 12723 RVA: 0x00130C78 File Offset: 0x0012EE78
	public void Initialize(Mesh mesh, Material material)
	{
		this.Mesh = mesh;
		this.Material = material;
		this.Positions = Pool.Get<FPNativeList<Vector4>>();
		this.args = Pool.Get<FPNativeList<uint>>();
		this.args.Resize(5);
		this.ArgsBuffer = this.SafeRelease(this.ArgsBuffer);
		this.ArgsBuffer = new ComputeBuffer(1, this.args.Count * 4, ComputeBufferType.DrawIndirect);
		this.args[0] = this.Mesh.GetIndexCount(0);
		this.args[2] = this.Mesh.GetIndexStart(0);
		this.args[3] = this.Mesh.GetBaseVertex(0);
	}

	// Token: 0x060031B4 RID: 12724 RVA: 0x00130D30 File Offset: 0x0012EF30
	public void Release()
	{
		this.recycle.Clear();
		Pool.Free<FPNativeList<Vector4>>(ref this.Positions);
		Pool.Free<FPNativeList<uint>>(ref this.args);
		this.PositionBuffer = this.SafeRelease(this.PositionBuffer);
		this.ArgsBuffer = this.SafeRelease(this.ArgsBuffer);
	}

	// Token: 0x060031B5 RID: 12725 RVA: 0x00130D84 File Offset: 0x0012EF84
	public void AddInstance(ImpostorInstanceData data)
	{
		data.Batch = this;
		if (this.recycle.Count > 0)
		{
			data.BatchIndex = this.recycle.Dequeue();
			this.Positions[data.BatchIndex] = data.PositionAndScale();
		}
		else
		{
			data.BatchIndex = this.Positions.Count;
			this.Positions.Add(data.PositionAndScale());
		}
		this.IsDirty = true;
	}

	// Token: 0x060031B6 RID: 12726 RVA: 0x00130DFC File Offset: 0x0012EFFC
	public void RemoveInstance(ImpostorInstanceData data)
	{
		this.Positions[data.BatchIndex] = new Vector4(0f, 0f, 0f, -1f);
		this.recycle.Enqueue(data.BatchIndex);
		data.BatchIndex = 0;
		data.Batch = null;
		this.IsDirty = true;
	}

	// Token: 0x060031B7 RID: 12727 RVA: 0x00130E5C File Offset: 0x0012F05C
	public void UpdateBuffers()
	{
		if (!this.IsDirty)
		{
			return;
		}
		bool flag = false;
		if (this.PositionBuffer == null || this.PositionBuffer.count != this.Positions.Count)
		{
			this.PositionBuffer = this.SafeRelease(this.PositionBuffer);
			this.PositionBuffer = new ComputeBuffer(this.Positions.Count, 16);
			flag = true;
		}
		if (this.PositionBuffer != null)
		{
			this.PositionBuffer.SetData<Vector4>(this.Positions.Array, 0, 0, this.Positions.Count);
		}
		if (this.ArgsBuffer != null && flag)
		{
			this.args[1] = (uint)this.Positions.Count;
			this.ArgsBuffer.SetData<uint>(this.args.Array, 0, 0, this.args.Count);
		}
		this.IsDirty = false;
	}

	// Token: 0x0400284F RID: 10319
	public FPNativeList<Vector4> Positions;

	// Token: 0x04002851 RID: 10321
	private FPNativeList<uint> args;

	// Token: 0x04002853 RID: 10323
	private Queue<int> recycle = new Queue<int>(32);
}
