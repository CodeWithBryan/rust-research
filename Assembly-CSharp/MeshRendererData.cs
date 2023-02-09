using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;

// Token: 0x02000294 RID: 660
public class MeshRendererData
{
	// Token: 0x06001C1B RID: 7195 RVA: 0x000C2814 File Offset: 0x000C0A14
	public void Alloc()
	{
		if (this.triangles == null)
		{
			this.triangles = Facepunch.Pool.GetList<int>();
		}
		if (this.vertices == null)
		{
			this.vertices = Facepunch.Pool.GetList<Vector3>();
		}
		if (this.normals == null)
		{
			this.normals = Facepunch.Pool.GetList<Vector3>();
		}
		if (this.tangents == null)
		{
			this.tangents = Facepunch.Pool.GetList<Vector4>();
		}
		if (this.colors32 == null)
		{
			this.colors32 = Facepunch.Pool.GetList<Color32>();
		}
		if (this.uv == null)
		{
			this.uv = Facepunch.Pool.GetList<Vector2>();
		}
		if (this.uv2 == null)
		{
			this.uv2 = Facepunch.Pool.GetList<Vector2>();
		}
		if (this.positions == null)
		{
			this.positions = Facepunch.Pool.GetList<Vector4>();
		}
	}

	// Token: 0x06001C1C RID: 7196 RVA: 0x000C28BC File Offset: 0x000C0ABC
	public void Free()
	{
		if (this.triangles != null)
		{
			Facepunch.Pool.FreeList<int>(ref this.triangles);
		}
		if (this.vertices != null)
		{
			Facepunch.Pool.FreeList<Vector3>(ref this.vertices);
		}
		if (this.normals != null)
		{
			Facepunch.Pool.FreeList<Vector3>(ref this.normals);
		}
		if (this.tangents != null)
		{
			Facepunch.Pool.FreeList<Vector4>(ref this.tangents);
		}
		if (this.colors32 != null)
		{
			Facepunch.Pool.FreeList<Color32>(ref this.colors32);
		}
		if (this.uv != null)
		{
			Facepunch.Pool.FreeList<Vector2>(ref this.uv);
		}
		if (this.uv2 != null)
		{
			Facepunch.Pool.FreeList<Vector2>(ref this.uv2);
		}
		if (this.positions != null)
		{
			Facepunch.Pool.FreeList<Vector4>(ref this.positions);
		}
	}

	// Token: 0x06001C1D RID: 7197 RVA: 0x000C2964 File Offset: 0x000C0B64
	public void Clear()
	{
		if (this.triangles != null)
		{
			this.triangles.Clear();
		}
		if (this.vertices != null)
		{
			this.vertices.Clear();
		}
		if (this.normals != null)
		{
			this.normals.Clear();
		}
		if (this.tangents != null)
		{
			this.tangents.Clear();
		}
		if (this.colors32 != null)
		{
			this.colors32.Clear();
		}
		if (this.uv != null)
		{
			this.uv.Clear();
		}
		if (this.uv2 != null)
		{
			this.uv2.Clear();
		}
		if (this.positions != null)
		{
			this.positions.Clear();
		}
	}

	// Token: 0x06001C1E RID: 7198 RVA: 0x000C2A0C File Offset: 0x000C0C0C
	public void Apply(UnityEngine.Mesh mesh)
	{
		mesh.Clear();
		if (this.vertices != null)
		{
			mesh.SetVertices(this.vertices);
		}
		if (this.triangles != null)
		{
			mesh.SetTriangles(this.triangles, 0);
		}
		if (this.normals != null)
		{
			if (this.normals.Count == this.vertices.Count)
			{
				mesh.SetNormals(this.normals);
			}
			else if (this.normals.Count > 0 && Batching.verbose > 0)
			{
				Debug.LogWarning("Skipping renderer normals because some meshes were missing them.");
			}
		}
		if (this.tangents != null)
		{
			if (this.tangents.Count == this.vertices.Count)
			{
				mesh.SetTangents(this.tangents);
			}
			else if (this.tangents.Count > 0 && Batching.verbose > 0)
			{
				Debug.LogWarning("Skipping renderer tangents because some meshes were missing them.");
			}
		}
		if (this.colors32 != null)
		{
			if (this.colors32.Count == this.vertices.Count)
			{
				mesh.SetColors(this.colors32);
			}
			else if (this.colors32.Count > 0 && Batching.verbose > 0)
			{
				Debug.LogWarning("Skipping renderer colors because some meshes were missing them.");
			}
		}
		if (this.uv != null)
		{
			if (this.uv.Count == this.vertices.Count)
			{
				mesh.SetUVs(0, this.uv);
			}
			else if (this.uv.Count > 0 && Batching.verbose > 0)
			{
				Debug.LogWarning("Skipping renderer uvs because some meshes were missing them.");
			}
		}
		if (this.uv2 != null)
		{
			if (this.uv2.Count == this.vertices.Count)
			{
				mesh.SetUVs(1, this.uv2);
			}
			else if (this.uv2.Count > 0 && Batching.verbose > 0)
			{
				Debug.LogWarning("Skipping renderer uv2s because some meshes were missing them.");
			}
		}
		if (this.positions != null)
		{
			mesh.SetUVs(2, this.positions);
		}
	}

	// Token: 0x06001C1F RID: 7199 RVA: 0x000C2BE8 File Offset: 0x000C0DE8
	public void Combine(MeshRendererGroup meshGroup)
	{
		for (int i = 0; i < meshGroup.data.Count; i++)
		{
			MeshRendererInstance meshRendererInstance = meshGroup.data[i];
			Matrix4x4 matrix4x = Matrix4x4.TRS(meshRendererInstance.position, meshRendererInstance.rotation, meshRendererInstance.scale);
			int count = this.vertices.Count;
			for (int j = 0; j < meshRendererInstance.data.triangles.Length; j++)
			{
				this.triangles.Add(count + meshRendererInstance.data.triangles[j]);
			}
			for (int k = 0; k < meshRendererInstance.data.vertices.Length; k++)
			{
				this.vertices.Add(matrix4x.MultiplyPoint3x4(meshRendererInstance.data.vertices[k]));
				this.positions.Add(meshRendererInstance.position);
			}
			for (int l = 0; l < meshRendererInstance.data.normals.Length; l++)
			{
				this.normals.Add(matrix4x.MultiplyVector(meshRendererInstance.data.normals[l]));
			}
			for (int m = 0; m < meshRendererInstance.data.tangents.Length; m++)
			{
				Vector4 vector = meshRendererInstance.data.tangents[m];
				Vector3 vector2 = new Vector3(vector.x, vector.y, vector.z);
				Vector3 vector3 = matrix4x.MultiplyVector(vector2);
				this.tangents.Add(new Vector4(vector3.x, vector3.y, vector3.z, vector.w));
			}
			for (int n = 0; n < meshRendererInstance.data.colors32.Length; n++)
			{
				this.colors32.Add(meshRendererInstance.data.colors32[n]);
			}
			for (int num = 0; num < meshRendererInstance.data.uv.Length; num++)
			{
				this.uv.Add(meshRendererInstance.data.uv[num]);
			}
			for (int num2 = 0; num2 < meshRendererInstance.data.uv2.Length; num2++)
			{
				this.uv2.Add(meshRendererInstance.data.uv2[num2]);
			}
		}
	}

	// Token: 0x06001C20 RID: 7200 RVA: 0x000C2E40 File Offset: 0x000C1040
	public void Combine(MeshRendererGroup meshGroup, MeshRendererLookup rendererLookup)
	{
		for (int i = 0; i < meshGroup.data.Count; i++)
		{
			MeshRendererInstance meshRendererInstance = meshGroup.data[i];
			Matrix4x4 matrix4x = Matrix4x4.TRS(meshRendererInstance.position, meshRendererInstance.rotation, meshRendererInstance.scale);
			int count = this.vertices.Count;
			for (int j = 0; j < meshRendererInstance.data.triangles.Length; j++)
			{
				this.triangles.Add(count + meshRendererInstance.data.triangles[j]);
			}
			for (int k = 0; k < meshRendererInstance.data.vertices.Length; k++)
			{
				this.vertices.Add(matrix4x.MultiplyPoint3x4(meshRendererInstance.data.vertices[k]));
				this.positions.Add(meshRendererInstance.position);
			}
			for (int l = 0; l < meshRendererInstance.data.normals.Length; l++)
			{
				this.normals.Add(matrix4x.MultiplyVector(meshRendererInstance.data.normals[l]));
			}
			for (int m = 0; m < meshRendererInstance.data.tangents.Length; m++)
			{
				Vector4 vector = meshRendererInstance.data.tangents[m];
				Vector3 vector2 = new Vector3(vector.x, vector.y, vector.z);
				Vector3 vector3 = matrix4x.MultiplyVector(vector2);
				this.tangents.Add(new Vector4(vector3.x, vector3.y, vector3.z, vector.w));
			}
			for (int n = 0; n < meshRendererInstance.data.colors32.Length; n++)
			{
				this.colors32.Add(meshRendererInstance.data.colors32[n]);
			}
			for (int num = 0; num < meshRendererInstance.data.uv.Length; num++)
			{
				this.uv.Add(meshRendererInstance.data.uv[num]);
			}
			for (int num2 = 0; num2 < meshRendererInstance.data.uv2.Length; num2++)
			{
				this.uv2.Add(meshRendererInstance.data.uv2[num2]);
			}
			rendererLookup.Add(meshRendererInstance);
		}
	}

	// Token: 0x0400155D RID: 5469
	public List<int> triangles;

	// Token: 0x0400155E RID: 5470
	public List<Vector3> vertices;

	// Token: 0x0400155F RID: 5471
	public List<Vector3> normals;

	// Token: 0x04001560 RID: 5472
	public List<Vector4> tangents;

	// Token: 0x04001561 RID: 5473
	public List<Color32> colors32;

	// Token: 0x04001562 RID: 5474
	public List<Vector2> uv;

	// Token: 0x04001563 RID: 5475
	public List<Vector2> uv2;

	// Token: 0x04001564 RID: 5476
	public List<Vector4> positions;
}
