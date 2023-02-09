using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000902 RID: 2306
public class FoliageGridMeshData
{
	// Token: 0x060036DE RID: 14046 RVA: 0x001465EF File Offset: 0x001447EF
	public void Alloc()
	{
		if (this.triangles == null)
		{
			this.triangles = Pool.GetList<int>();
		}
		if (this.vertices == null)
		{
			this.vertices = Pool.GetList<FoliageGridMeshData.FoliageVertex>();
		}
	}

	// Token: 0x060036DF RID: 14047 RVA: 0x00146617 File Offset: 0x00144817
	public void Free()
	{
		if (this.triangles != null)
		{
			Pool.FreeList<int>(ref this.triangles);
		}
		if (this.vertices != null)
		{
			Pool.FreeList<FoliageGridMeshData.FoliageVertex>(ref this.vertices);
		}
	}

	// Token: 0x060036E0 RID: 14048 RVA: 0x0014663F File Offset: 0x0014483F
	public void Clear()
	{
		List<int> list = this.triangles;
		if (list != null)
		{
			list.Clear();
		}
		List<FoliageGridMeshData.FoliageVertex> list2 = this.vertices;
		if (list2 == null)
		{
			return;
		}
		list2.Clear();
	}

	// Token: 0x060036E1 RID: 14049 RVA: 0x00146664 File Offset: 0x00144864
	public void Combine(MeshGroup meshGroup)
	{
		if (meshGroup.data.Count == 0)
		{
			return;
		}
		this.bounds = new Bounds(meshGroup.data[0].position, Vector3.one);
		for (int i = 0; i < meshGroup.data.Count; i++)
		{
			MeshInstance meshInstance = meshGroup.data[i];
			Matrix4x4 matrix4x = Matrix4x4.TRS(meshInstance.position, meshInstance.rotation, meshInstance.scale);
			int count = this.vertices.Count;
			for (int j = 0; j < meshInstance.data.triangles.Length; j++)
			{
				this.triangles.Add(count + meshInstance.data.triangles[j]);
			}
			for (int k = 0; k < meshInstance.data.vertices.Length; k++)
			{
				Vector4 vector = meshInstance.data.tangents[k];
				Vector3 vector2 = new Vector3(vector.x, vector.y, vector.z);
				Vector3 vector3 = matrix4x.MultiplyVector(vector2);
				FoliageGridMeshData.FoliageVertex item = default(FoliageGridMeshData.FoliageVertex);
				item.position = matrix4x.MultiplyPoint3x4(meshInstance.data.vertices[k]);
				item.normal = matrix4x.MultiplyVector(meshInstance.data.normals[k]);
				item.uv = meshInstance.data.uv[k];
				item.uv2 = meshInstance.position;
				item.tangent = new Vector4(vector3.x, vector3.y, vector3.z, vector.w);
				if (meshInstance.data.colors32.Length != 0)
				{
					item.color = meshInstance.data.colors32[k];
				}
				this.vertices.Add(item);
			}
			this.bounds.Encapsulate(meshInstance.position);
		}
		this.bounds.size = this.bounds.size + Vector3.one * 2f;
	}

	// Token: 0x060036E2 RID: 14050 RVA: 0x00146888 File Offset: 0x00144A88
	public void Apply(Mesh mesh)
	{
		mesh.SetVertexBufferParams(this.vertices.Count, FoliageGridMeshData.FoliageVertex.VertexLayout);
		mesh.SetVertexBufferData<FoliageGridMeshData.FoliageVertex>(this.vertices, 0, 0, this.vertices.Count, 0, MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontRecalculateBounds);
		mesh.SetIndices(this.triangles, MeshTopology.Triangles, 0, false, 0);
		mesh.bounds = this.bounds;
	}

	// Token: 0x0400319C RID: 12700
	public List<FoliageGridMeshData.FoliageVertex> vertices;

	// Token: 0x0400319D RID: 12701
	public List<int> triangles;

	// Token: 0x0400319E RID: 12702
	public Bounds bounds;

	// Token: 0x02000E5F RID: 3679
	public struct FoliageVertex
	{
		// Token: 0x04004A30 RID: 18992
		public Vector3 position;

		// Token: 0x04004A31 RID: 18993
		public Vector3 normal;

		// Token: 0x04004A32 RID: 18994
		public Vector4 tangent;

		// Token: 0x04004A33 RID: 18995
		public Color32 color;

		// Token: 0x04004A34 RID: 18996
		public Vector2 uv;

		// Token: 0x04004A35 RID: 18997
		public Vector4 uv2;

		// Token: 0x04004A36 RID: 18998
		public static readonly VertexAttributeDescriptor[] VertexLayout = new VertexAttributeDescriptor[]
		{
			new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3, 0),
			new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3, 0),
			new VertexAttributeDescriptor(VertexAttribute.Tangent, VertexAttributeFormat.Float32, 4, 0),
			new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.UNorm8, 4, 0),
			new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2, 0),
			new VertexAttributeDescriptor(VertexAttribute.TexCoord2, VertexAttributeFormat.Float32, 4, 0)
		};
	}
}
