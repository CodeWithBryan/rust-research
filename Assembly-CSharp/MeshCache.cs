using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200028C RID: 652
public static class MeshCache
{
	// Token: 0x06001C01 RID: 7169 RVA: 0x000C1CB0 File Offset: 0x000BFEB0
	public static MeshCache.Data Get(Mesh mesh)
	{
		MeshCache.Data data;
		if (!MeshCache.dictionary.TryGetValue(mesh, out data))
		{
			data = new MeshCache.Data();
			data.mesh = mesh;
			data.vertices = mesh.vertices;
			data.normals = mesh.normals;
			data.tangents = mesh.tangents;
			data.colors32 = mesh.colors32;
			data.triangles = mesh.triangles;
			data.uv = mesh.uv;
			data.uv2 = mesh.uv2;
			data.uv3 = mesh.uv3;
			data.uv4 = mesh.uv4;
			MeshCache.dictionary.Add(mesh, data);
		}
		return data;
	}

	// Token: 0x04001543 RID: 5443
	public static Dictionary<Mesh, MeshCache.Data> dictionary = new Dictionary<Mesh, MeshCache.Data>();

	// Token: 0x02000C3E RID: 3134
	[Serializable]
	public class Data
	{
		// Token: 0x0400417F RID: 16767
		public Mesh mesh;

		// Token: 0x04004180 RID: 16768
		public Vector3[] vertices;

		// Token: 0x04004181 RID: 16769
		public Vector3[] normals;

		// Token: 0x04004182 RID: 16770
		public Vector4[] tangents;

		// Token: 0x04004183 RID: 16771
		public Color32[] colors32;

		// Token: 0x04004184 RID: 16772
		public int[] triangles;

		// Token: 0x04004185 RID: 16773
		public Vector2[] uv;

		// Token: 0x04004186 RID: 16774
		public Vector2[] uv2;

		// Token: 0x04004187 RID: 16775
		public Vector2[] uv3;

		// Token: 0x04004188 RID: 16776
		public Vector2[] uv4;
	}
}
