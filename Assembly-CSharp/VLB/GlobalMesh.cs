using System;
using UnityEngine;

namespace VLB
{
	// Token: 0x02000982 RID: 2434
	public static class GlobalMesh
	{
		// Token: 0x17000457 RID: 1111
		// (get) Token: 0x0600398E RID: 14734 RVA: 0x00154380 File Offset: 0x00152580
		public static Mesh mesh
		{
			get
			{
				if (GlobalMesh.ms_Mesh == null)
				{
					GlobalMesh.ms_Mesh = MeshGenerator.GenerateConeZ_Radius(1f, 1f, 1f, Config.Instance.sharedMeshSides, Config.Instance.sharedMeshSegments, true);
					GlobalMesh.ms_Mesh.hideFlags = Consts.ProceduralObjectsHideFlags;
				}
				return GlobalMesh.ms_Mesh;
			}
		}

		// Token: 0x0400343B RID: 13371
		private static Mesh ms_Mesh;
	}
}
