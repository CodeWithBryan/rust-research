using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200062A RID: 1578
public class AsyncTerrainNavMeshBake : CustomYieldInstruction
{
	// Token: 0x17000376 RID: 886
	// (get) Token: 0x06002D6E RID: 11630 RVA: 0x00112168 File Offset: 0x00110368
	public override bool keepWaiting
	{
		get
		{
			return this.worker != null;
		}
	}

	// Token: 0x17000377 RID: 887
	// (get) Token: 0x06002D6F RID: 11631 RVA: 0x00112173 File Offset: 0x00110373
	public bool isDone
	{
		get
		{
			return this.worker == null;
		}
	}

	// Token: 0x06002D70 RID: 11632 RVA: 0x00112180 File Offset: 0x00110380
	public NavMeshBuildSource CreateNavMeshBuildSource()
	{
		return new NavMeshBuildSource
		{
			transform = Matrix4x4.TRS(this.pivot, Quaternion.identity, Vector3.one),
			shape = NavMeshBuildSourceShape.Mesh,
			sourceObject = this.mesh
		};
	}

	// Token: 0x06002D71 RID: 11633 RVA: 0x001121C8 File Offset: 0x001103C8
	public NavMeshBuildSource CreateNavMeshBuildSource(int area)
	{
		NavMeshBuildSource result = this.CreateNavMeshBuildSource();
		result.area = area;
		return result;
	}

	// Token: 0x17000378 RID: 888
	// (get) Token: 0x06002D72 RID: 11634 RVA: 0x001121E8 File Offset: 0x001103E8
	public Mesh mesh
	{
		get
		{
			Mesh mesh = new Mesh();
			if (this.vertices != null)
			{
				mesh.SetVertices(this.vertices);
				Pool.FreeList<Vector3>(ref this.vertices);
			}
			if (this.normals != null)
			{
				mesh.SetNormals(this.normals);
				Pool.FreeList<Vector3>(ref this.normals);
			}
			if (this.triangles != null)
			{
				mesh.SetTriangles(this.triangles, 0);
				Pool.FreeList<int>(ref this.triangles);
			}
			if (this.indices != null)
			{
				Pool.FreeList<int>(ref this.indices);
			}
			return mesh;
		}
	}

	// Token: 0x06002D73 RID: 11635 RVA: 0x00112270 File Offset: 0x00110470
	public AsyncTerrainNavMeshBake(Vector3 pivot, int width, int height, bool normal, bool alpha)
	{
		this.pivot = pivot;
		this.width = width;
		this.height = height;
		this.normal = normal;
		this.alpha = alpha;
		this.indices = Pool.GetList<int>();
		this.vertices = Pool.GetList<Vector3>();
		this.normals = (normal ? Pool.GetList<Vector3>() : null);
		this.triangles = Pool.GetList<int>();
		this.Invoke();
	}

	// Token: 0x06002D74 RID: 11636 RVA: 0x001122E4 File Offset: 0x001104E4
	private void DoWork()
	{
		Vector3 vector = new Vector3((float)(this.width / 2), 0f, (float)(this.height / 2));
		Vector3 b = new Vector3(this.pivot.x - vector.x, 0f, this.pivot.z - vector.z);
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainAlphaMap alphaMap = TerrainMeta.AlphaMap;
		int num = 0;
		for (int i = 0; i <= this.height; i++)
		{
			int j = 0;
			while (j <= this.width)
			{
				Vector3 worldPos = new Vector3((float)j, 0f, (float)i) + b;
				Vector3 item = new Vector3((float)j, 0f, (float)i) - vector;
				float num2 = heightMap.GetHeight(worldPos);
				if (num2 < -1f)
				{
					this.indices.Add(-1);
				}
				else if (this.alpha && alphaMap.GetAlpha(worldPos) < 0.1f)
				{
					this.indices.Add(-1);
				}
				else
				{
					if (this.normal)
					{
						Vector3 item2 = heightMap.GetNormal(worldPos);
						this.normals.Add(item2);
					}
					worldPos.y = (item.y = num2 - this.pivot.y);
					this.indices.Add(this.vertices.Count);
					this.vertices.Add(item);
				}
				j++;
				num++;
			}
		}
		int num3 = 0;
		int k = 0;
		while (k < this.height)
		{
			int l = 0;
			while (l < this.width)
			{
				int num4 = this.indices[num3];
				int num5 = this.indices[num3 + this.width + 1];
				int num6 = this.indices[num3 + 1];
				int num7 = this.indices[num3 + 1];
				int num8 = this.indices[num3 + this.width + 1];
				int num9 = this.indices[num3 + this.width + 2];
				if (num4 != -1 && num5 != -1 && num6 != -1)
				{
					this.triangles.Add(num4);
					this.triangles.Add(num5);
					this.triangles.Add(num6);
				}
				if (num7 != -1 && num8 != -1 && num9 != -1)
				{
					this.triangles.Add(num7);
					this.triangles.Add(num8);
					this.triangles.Add(num9);
				}
				l++;
				num3++;
			}
			k++;
			num3++;
		}
	}

	// Token: 0x06002D75 RID: 11637 RVA: 0x00112590 File Offset: 0x00110790
	private void Invoke()
	{
		this.worker = new Action(this.DoWork);
		this.worker.BeginInvoke(new AsyncCallback(this.Callback), null);
	}

	// Token: 0x06002D76 RID: 11638 RVA: 0x001125BD File Offset: 0x001107BD
	private void Callback(IAsyncResult result)
	{
		this.worker.EndInvoke(result);
		this.worker = null;
	}

	// Token: 0x0400253A RID: 9530
	private List<int> indices;

	// Token: 0x0400253B RID: 9531
	private List<Vector3> vertices;

	// Token: 0x0400253C RID: 9532
	private List<Vector3> normals;

	// Token: 0x0400253D RID: 9533
	private List<int> triangles;

	// Token: 0x0400253E RID: 9534
	private Vector3 pivot;

	// Token: 0x0400253F RID: 9535
	private int width;

	// Token: 0x04002540 RID: 9536
	private int height;

	// Token: 0x04002541 RID: 9537
	private bool normal;

	// Token: 0x04002542 RID: 9538
	private bool alpha;

	// Token: 0x04002543 RID: 9539
	private Action worker;
}
