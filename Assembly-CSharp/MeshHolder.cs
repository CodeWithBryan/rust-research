using System;
using UnityEngine;

// Token: 0x02000970 RID: 2416
[Serializable]
public class MeshHolder
{
	// Token: 0x0600392B RID: 14635 RVA: 0x0015207A File Offset: 0x0015027A
	public void setAnimationData(Mesh mesh)
	{
		this._colors = mesh.colors;
	}

	// Token: 0x04003393 RID: 13203
	[HideInInspector]
	public Vector3[] _vertices;

	// Token: 0x04003394 RID: 13204
	[HideInInspector]
	public Vector3[] _normals;

	// Token: 0x04003395 RID: 13205
	[HideInInspector]
	public int[] _triangles;

	// Token: 0x04003396 RID: 13206
	[HideInInspector]
	public trisPerSubmesh[] _TrianglesOfSubs;

	// Token: 0x04003397 RID: 13207
	[HideInInspector]
	public Matrix4x4[] _bindPoses;

	// Token: 0x04003398 RID: 13208
	[HideInInspector]
	public BoneWeight[] _boneWeights;

	// Token: 0x04003399 RID: 13209
	[HideInInspector]
	public Bounds _bounds;

	// Token: 0x0400339A RID: 13210
	[HideInInspector]
	public int _subMeshCount;

	// Token: 0x0400339B RID: 13211
	[HideInInspector]
	public Vector4[] _tangents;

	// Token: 0x0400339C RID: 13212
	[HideInInspector]
	public Vector2[] _uv;

	// Token: 0x0400339D RID: 13213
	[HideInInspector]
	public Vector2[] _uv2;

	// Token: 0x0400339E RID: 13214
	[HideInInspector]
	public Vector2[] _uv3;

	// Token: 0x0400339F RID: 13215
	[HideInInspector]
	public Color[] _colors;

	// Token: 0x040033A0 RID: 13216
	[HideInInspector]
	public Vector2[] _uv4;
}
