using System;
using UnityEngine;

// Token: 0x02000973 RID: 2419
[ExecuteInEditMode]
public class VertexColorStream : MonoBehaviour
{
	// Token: 0x06003936 RID: 14646 RVA: 0x000059DD File Offset: 0x00003BDD
	private void OnDidApplyAnimationProperties()
	{
	}

	// Token: 0x06003937 RID: 14647 RVA: 0x00152408 File Offset: 0x00150608
	public void init(Mesh origMesh, bool destroyOld)
	{
		this.originalMesh = origMesh;
		this.paintedMesh = UnityEngine.Object.Instantiate<Mesh>(origMesh);
		if (destroyOld)
		{
			UnityEngine.Object.DestroyImmediate(origMesh);
		}
		this.paintedMesh.hideFlags = HideFlags.None;
		this.paintedMesh.name = "vpp_" + base.gameObject.name;
		this.meshHold = new MeshHolder();
		this.meshHold._vertices = this.paintedMesh.vertices;
		this.meshHold._normals = this.paintedMesh.normals;
		this.meshHold._triangles = this.paintedMesh.triangles;
		this.meshHold._TrianglesOfSubs = new trisPerSubmesh[this.paintedMesh.subMeshCount];
		for (int i = 0; i < this.paintedMesh.subMeshCount; i++)
		{
			this.meshHold._TrianglesOfSubs[i] = new trisPerSubmesh();
			this.meshHold._TrianglesOfSubs[i].triangles = this.paintedMesh.GetTriangles(i);
		}
		this.meshHold._bindPoses = this.paintedMesh.bindposes;
		this.meshHold._boneWeights = this.paintedMesh.boneWeights;
		this.meshHold._bounds = this.paintedMesh.bounds;
		this.meshHold._subMeshCount = this.paintedMesh.subMeshCount;
		this.meshHold._tangents = this.paintedMesh.tangents;
		this.meshHold._uv = this.paintedMesh.uv;
		this.meshHold._uv2 = this.paintedMesh.uv2;
		this.meshHold._uv3 = this.paintedMesh.uv3;
		this.meshHold._colors = this.paintedMesh.colors;
		this.meshHold._uv4 = this.paintedMesh.uv4;
		base.GetComponent<MeshFilter>().sharedMesh = this.paintedMesh;
		if (base.GetComponent<MeshCollider>())
		{
			base.GetComponent<MeshCollider>().sharedMesh = this.paintedMesh;
		}
	}

	// Token: 0x06003938 RID: 14648 RVA: 0x00152618 File Offset: 0x00150818
	public void setWholeMesh(Mesh tmpMesh)
	{
		this.paintedMesh.vertices = tmpMesh.vertices;
		this.paintedMesh.triangles = tmpMesh.triangles;
		this.paintedMesh.normals = tmpMesh.normals;
		this.paintedMesh.colors = tmpMesh.colors;
		this.paintedMesh.uv = tmpMesh.uv;
		this.paintedMesh.uv2 = tmpMesh.uv2;
		this.paintedMesh.uv3 = tmpMesh.uv3;
		this.meshHold._vertices = tmpMesh.vertices;
		this.meshHold._triangles = tmpMesh.triangles;
		this.meshHold._normals = tmpMesh.normals;
		this.meshHold._colors = tmpMesh.colors;
		this.meshHold._uv = tmpMesh.uv;
		this.meshHold._uv2 = tmpMesh.uv2;
		this.meshHold._uv3 = tmpMesh.uv3;
	}

	// Token: 0x06003939 RID: 14649 RVA: 0x00152714 File Offset: 0x00150914
	public Vector3[] setVertices(Vector3[] _deformedVertices)
	{
		this.paintedMesh.vertices = _deformedVertices;
		this.meshHold._vertices = _deformedVertices;
		this.paintedMesh.RecalculateNormals();
		this.paintedMesh.RecalculateBounds();
		this.meshHold._normals = this.paintedMesh.normals;
		this.meshHold._bounds = this.paintedMesh.bounds;
		base.GetComponent<MeshCollider>().sharedMesh = null;
		if (base.GetComponent<MeshCollider>())
		{
			base.GetComponent<MeshCollider>().sharedMesh = this.paintedMesh;
		}
		return this.meshHold._normals;
	}

	// Token: 0x0600393A RID: 14650 RVA: 0x001527B0 File Offset: 0x001509B0
	public Vector3[] getVertices()
	{
		return this.paintedMesh.vertices;
	}

	// Token: 0x0600393B RID: 14651 RVA: 0x001527BD File Offset: 0x001509BD
	public Vector3[] getNormals()
	{
		return this.paintedMesh.normals;
	}

	// Token: 0x0600393C RID: 14652 RVA: 0x001527CA File Offset: 0x001509CA
	public int[] getTriangles()
	{
		return this.paintedMesh.triangles;
	}

	// Token: 0x0600393D RID: 14653 RVA: 0x001527D7 File Offset: 0x001509D7
	public void setTangents(Vector4[] _meshTangents)
	{
		this.paintedMesh.tangents = _meshTangents;
		this.meshHold._tangents = _meshTangents;
	}

	// Token: 0x0600393E RID: 14654 RVA: 0x001527F1 File Offset: 0x001509F1
	public Vector4[] getTangents()
	{
		return this.paintedMesh.tangents;
	}

	// Token: 0x0600393F RID: 14655 RVA: 0x001527FE File Offset: 0x001509FE
	public void setColors(Color[] _vertexColors)
	{
		this.paintedMesh.colors = _vertexColors;
		this.meshHold._colors = _vertexColors;
	}

	// Token: 0x06003940 RID: 14656 RVA: 0x00152818 File Offset: 0x00150A18
	public Color[] getColors()
	{
		return this.paintedMesh.colors;
	}

	// Token: 0x06003941 RID: 14657 RVA: 0x00152825 File Offset: 0x00150A25
	public Vector2[] getUVs()
	{
		return this.paintedMesh.uv;
	}

	// Token: 0x06003942 RID: 14658 RVA: 0x00152832 File Offset: 0x00150A32
	public void setUV4s(Vector2[] _uv4s)
	{
		this.paintedMesh.uv4 = _uv4s;
		this.meshHold._uv4 = _uv4s;
	}

	// Token: 0x06003943 RID: 14659 RVA: 0x0015284C File Offset: 0x00150A4C
	public Vector2[] getUV4s()
	{
		return this.paintedMesh.uv4;
	}

	// Token: 0x06003944 RID: 14660 RVA: 0x00152859 File Offset: 0x00150A59
	public void unlink()
	{
		this.init(this.paintedMesh, false);
	}

	// Token: 0x06003945 RID: 14661 RVA: 0x00152868 File Offset: 0x00150A68
	public void rebuild()
	{
		if (!base.GetComponent<MeshFilter>())
		{
			return;
		}
		this.paintedMesh = new Mesh();
		this.paintedMesh.hideFlags = HideFlags.HideAndDontSave;
		this.paintedMesh.name = "vpp_" + base.gameObject.name;
		if (this.meshHold == null || this.meshHold._vertices.Length == 0 || this.meshHold._TrianglesOfSubs.Length == 0)
		{
			this.paintedMesh.subMeshCount = this._subMeshCount;
			this.paintedMesh.vertices = this._vertices;
			this.paintedMesh.normals = this._normals;
			this.paintedMesh.triangles = this._triangles;
			this.meshHold._TrianglesOfSubs = new trisPerSubmesh[this.paintedMesh.subMeshCount];
			for (int i = 0; i < this.paintedMesh.subMeshCount; i++)
			{
				this.meshHold._TrianglesOfSubs[i] = new trisPerSubmesh();
				this.meshHold._TrianglesOfSubs[i].triangles = this.paintedMesh.GetTriangles(i);
			}
			this.paintedMesh.bindposes = this._bindPoses;
			this.paintedMesh.boneWeights = this._boneWeights;
			this.paintedMesh.bounds = this._bounds;
			this.paintedMesh.tangents = this._tangents;
			this.paintedMesh.uv = this._uv;
			this.paintedMesh.uv2 = this._uv2;
			this.paintedMesh.uv3 = this._uv3;
			this.paintedMesh.colors = this._colors;
			this.paintedMesh.uv4 = this._uv4;
			this.init(this.paintedMesh, true);
			return;
		}
		this.paintedMesh.subMeshCount = this.meshHold._subMeshCount;
		this.paintedMesh.vertices = this.meshHold._vertices;
		this.paintedMesh.normals = this.meshHold._normals;
		for (int j = 0; j < this.meshHold._subMeshCount; j++)
		{
			this.paintedMesh.SetTriangles(this.meshHold._TrianglesOfSubs[j].triangles, j);
		}
		this.paintedMesh.bindposes = this.meshHold._bindPoses;
		this.paintedMesh.boneWeights = this.meshHold._boneWeights;
		this.paintedMesh.bounds = this.meshHold._bounds;
		this.paintedMesh.tangents = this.meshHold._tangents;
		this.paintedMesh.uv = this.meshHold._uv;
		this.paintedMesh.uv2 = this.meshHold._uv2;
		this.paintedMesh.uv3 = this.meshHold._uv3;
		this.paintedMesh.colors = this.meshHold._colors;
		this.paintedMesh.uv4 = this.meshHold._uv4;
		this.init(this.paintedMesh, true);
	}

	// Token: 0x06003946 RID: 14662 RVA: 0x00152B77 File Offset: 0x00150D77
	private void Start()
	{
		if (!this.paintedMesh || this.meshHold == null)
		{
			this.rebuild();
		}
	}

	// Token: 0x040033A7 RID: 13223
	[HideInInspector]
	public Mesh originalMesh;

	// Token: 0x040033A8 RID: 13224
	[HideInInspector]
	public Mesh paintedMesh;

	// Token: 0x040033A9 RID: 13225
	[HideInInspector]
	public MeshHolder meshHold;

	// Token: 0x040033AA RID: 13226
	[HideInInspector]
	public Vector3[] _vertices;

	// Token: 0x040033AB RID: 13227
	[HideInInspector]
	public Vector3[] _normals;

	// Token: 0x040033AC RID: 13228
	[HideInInspector]
	public int[] _triangles;

	// Token: 0x040033AD RID: 13229
	[HideInInspector]
	public int[][] _Subtriangles;

	// Token: 0x040033AE RID: 13230
	[HideInInspector]
	public Matrix4x4[] _bindPoses;

	// Token: 0x040033AF RID: 13231
	[HideInInspector]
	public BoneWeight[] _boneWeights;

	// Token: 0x040033B0 RID: 13232
	[HideInInspector]
	public Bounds _bounds;

	// Token: 0x040033B1 RID: 13233
	[HideInInspector]
	public int _subMeshCount;

	// Token: 0x040033B2 RID: 13234
	[HideInInspector]
	public Vector4[] _tangents;

	// Token: 0x040033B3 RID: 13235
	[HideInInspector]
	public Vector2[] _uv;

	// Token: 0x040033B4 RID: 13236
	[HideInInspector]
	public Vector2[] _uv2;

	// Token: 0x040033B5 RID: 13237
	[HideInInspector]
	public Vector2[] _uv3;

	// Token: 0x040033B6 RID: 13238
	[HideInInspector]
	public Color[] _colors;

	// Token: 0x040033B7 RID: 13239
	[HideInInspector]
	public Vector2[] _uv4;
}
