using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;

// Token: 0x0200028D RID: 653
public class MeshColliderData
{
	// Token: 0x06001C03 RID: 7171 RVA: 0x000C1D61 File Offset: 0x000BFF61
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
	}

	// Token: 0x06001C04 RID: 7172 RVA: 0x000C1D9C File Offset: 0x000BFF9C
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
	}

	// Token: 0x06001C05 RID: 7173 RVA: 0x000C1DD7 File Offset: 0x000BFFD7
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
	}

	// Token: 0x06001C06 RID: 7174 RVA: 0x000C1E14 File Offset: 0x000C0014
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
				return;
			}
			if (this.normals.Count > 0 && Batching.verbose > 0)
			{
				Debug.LogWarning("Skipping collider normals because some meshes were missing them.");
			}
		}
	}

	// Token: 0x06001C07 RID: 7175 RVA: 0x000C1EA0 File Offset: 0x000C00A0
	public void Combine(MeshColliderGroup meshGroup)
	{
		for (int i = 0; i < meshGroup.data.Count; i++)
		{
			MeshColliderInstance meshColliderInstance = meshGroup.data[i];
			Matrix4x4 matrix4x = Matrix4x4.TRS(meshColliderInstance.position, meshColliderInstance.rotation, meshColliderInstance.scale);
			int count = this.vertices.Count;
			for (int j = 0; j < meshColliderInstance.data.triangles.Length; j++)
			{
				this.triangles.Add(count + meshColliderInstance.data.triangles[j]);
			}
			for (int k = 0; k < meshColliderInstance.data.vertices.Length; k++)
			{
				this.vertices.Add(matrix4x.MultiplyPoint3x4(meshColliderInstance.data.vertices[k]));
			}
			for (int l = 0; l < meshColliderInstance.data.normals.Length; l++)
			{
				this.normals.Add(matrix4x.MultiplyVector(meshColliderInstance.data.normals[l]));
			}
		}
	}

	// Token: 0x06001C08 RID: 7176 RVA: 0x000C1FB4 File Offset: 0x000C01B4
	public void Combine(MeshColliderGroup meshGroup, MeshColliderLookup colliderLookup)
	{
		for (int i = 0; i < meshGroup.data.Count; i++)
		{
			MeshColliderInstance meshColliderInstance = meshGroup.data[i];
			Matrix4x4 matrix4x = Matrix4x4.TRS(meshColliderInstance.position, meshColliderInstance.rotation, meshColliderInstance.scale);
			int count = this.vertices.Count;
			for (int j = 0; j < meshColliderInstance.data.triangles.Length; j++)
			{
				this.triangles.Add(count + meshColliderInstance.data.triangles[j]);
			}
			for (int k = 0; k < meshColliderInstance.data.vertices.Length; k++)
			{
				this.vertices.Add(matrix4x.MultiplyPoint3x4(meshColliderInstance.data.vertices[k]));
			}
			for (int l = 0; l < meshColliderInstance.data.normals.Length; l++)
			{
				this.normals.Add(matrix4x.MultiplyVector(meshColliderInstance.data.normals[l]));
			}
			colliderLookup.Add(meshColliderInstance);
		}
	}

	// Token: 0x04001544 RID: 5444
	public List<int> triangles;

	// Token: 0x04001545 RID: 5445
	public List<Vector3> vertices;

	// Token: 0x04001546 RID: 5446
	public List<Vector3> normals;
}
