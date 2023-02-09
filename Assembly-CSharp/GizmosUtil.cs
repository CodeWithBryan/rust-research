using System;
using UnityEngine;

// Token: 0x02000905 RID: 2309
public static class GizmosUtil
{
	// Token: 0x060036F1 RID: 14065 RVA: 0x00146BA8 File Offset: 0x00144DA8
	public static void DrawWireCircleX(Vector3 pos, float radius)
	{
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix *= Matrix4x4.TRS(pos, Quaternion.identity, new Vector3(0f, 1f, 1f));
		Gizmos.DrawWireSphere(Vector3.zero, radius);
		Gizmos.matrix = matrix;
	}

	// Token: 0x060036F2 RID: 14066 RVA: 0x00146BF8 File Offset: 0x00144DF8
	public static void DrawWireCircleY(Vector3 pos, float radius)
	{
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix *= Matrix4x4.TRS(pos, Quaternion.identity, new Vector3(1f, 0f, 1f));
		Gizmos.DrawWireSphere(Vector3.zero, radius);
		Gizmos.matrix = matrix;
	}

	// Token: 0x060036F3 RID: 14067 RVA: 0x00146C48 File Offset: 0x00144E48
	public static void DrawWireCircleZ(Vector3 pos, float radius)
	{
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix *= Matrix4x4.TRS(pos, Quaternion.identity, new Vector3(1f, 1f, 0f));
		Gizmos.DrawWireSphere(Vector3.zero, radius);
		Gizmos.matrix = matrix;
	}

	// Token: 0x060036F4 RID: 14068 RVA: 0x00146C98 File Offset: 0x00144E98
	public static void DrawCircleX(Vector3 pos, float radius)
	{
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix *= Matrix4x4.TRS(pos, Quaternion.identity, new Vector3(0f, 1f, 1f));
		Gizmos.DrawSphere(Vector3.zero, radius);
		Gizmos.matrix = matrix;
	}

	// Token: 0x060036F5 RID: 14069 RVA: 0x00146CE8 File Offset: 0x00144EE8
	public static void DrawCircleY(Vector3 pos, float radius)
	{
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix *= Matrix4x4.TRS(pos, Quaternion.identity, new Vector3(1f, 0f, 1f));
		Gizmos.DrawSphere(Vector3.zero, radius);
		Gizmos.matrix = matrix;
	}

	// Token: 0x060036F6 RID: 14070 RVA: 0x00146D38 File Offset: 0x00144F38
	public static void DrawCircleZ(Vector3 pos, float radius)
	{
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix *= Matrix4x4.TRS(pos, Quaternion.identity, new Vector3(1f, 1f, 0f));
		Gizmos.DrawSphere(Vector3.zero, radius);
		Gizmos.matrix = matrix;
	}

	// Token: 0x060036F7 RID: 14071 RVA: 0x00146D88 File Offset: 0x00144F88
	public static void DrawWireCylinderX(Vector3 pos, float radius, float height)
	{
		GizmosUtil.DrawWireCircleX(pos - new Vector3(0.5f * height, 0f, 0f), radius);
		GizmosUtil.DrawWireCircleX(pos + new Vector3(0.5f * height, 0f, 0f), radius);
	}

	// Token: 0x060036F8 RID: 14072 RVA: 0x00146DDC File Offset: 0x00144FDC
	public static void DrawWireCylinderY(Vector3 pos, float radius, float height)
	{
		GizmosUtil.DrawWireCircleY(pos - new Vector3(0f, 0.5f * height, 0f), radius);
		GizmosUtil.DrawWireCircleY(pos + new Vector3(0f, 0.5f * height, 0f), radius);
	}

	// Token: 0x060036F9 RID: 14073 RVA: 0x00146E30 File Offset: 0x00145030
	public static void DrawWireCylinderZ(Vector3 pos, float radius, float height)
	{
		GizmosUtil.DrawWireCircleZ(pos - new Vector3(0f, 0f, 0.5f * height), radius);
		GizmosUtil.DrawWireCircleZ(pos + new Vector3(0f, 0f, 0.5f * height), radius);
	}

	// Token: 0x060036FA RID: 14074 RVA: 0x00146E84 File Offset: 0x00145084
	public static void DrawCylinderX(Vector3 pos, float radius, float height)
	{
		GizmosUtil.DrawCircleX(pos - new Vector3(0.5f * height, 0f, 0f), radius);
		GizmosUtil.DrawCircleX(pos + new Vector3(0.5f * height, 0f, 0f), radius);
	}

	// Token: 0x060036FB RID: 14075 RVA: 0x00146ED8 File Offset: 0x001450D8
	public static void DrawCylinderY(Vector3 pos, float radius, float height)
	{
		GizmosUtil.DrawCircleY(pos - new Vector3(0f, 0.5f * height, 0f), radius);
		GizmosUtil.DrawCircleY(pos + new Vector3(0f, 0.5f * height, 0f), radius);
	}

	// Token: 0x060036FC RID: 14076 RVA: 0x00146F2C File Offset: 0x0014512C
	public static void DrawCylinderZ(Vector3 pos, float radius, float height)
	{
		GizmosUtil.DrawCircleZ(pos - new Vector3(0f, 0f, 0.5f * height), radius);
		GizmosUtil.DrawCircleZ(pos + new Vector3(0f, 0f, 0.5f * height), radius);
	}

	// Token: 0x060036FD RID: 14077 RVA: 0x00146F80 File Offset: 0x00145180
	public static void DrawWireCapsuleX(Vector3 pos, float radius, float height)
	{
		Vector3 vector = pos - new Vector3(0.5f * height, 0f, 0f) + Vector3.right * radius;
		Vector3 vector2 = pos + new Vector3(0.5f * height, 0f, 0f) - Vector3.right * radius;
		Gizmos.DrawWireSphere(vector, radius);
		Gizmos.DrawWireSphere(vector2, radius);
		Gizmos.DrawLine(vector + Vector3.forward * radius, vector2 + Vector3.forward * radius);
		Gizmos.DrawLine(vector + Vector3.up * radius, vector2 + Vector3.up * radius);
		Gizmos.DrawLine(vector + Vector3.back * radius, vector2 + Vector3.back * radius);
		Gizmos.DrawLine(vector + Vector3.down * radius, vector2 + Vector3.down * radius);
	}

	// Token: 0x060036FE RID: 14078 RVA: 0x00147090 File Offset: 0x00145290
	public static void DrawWireCapsuleY(Vector3 pos, float radius, float height)
	{
		Vector3 vector = pos - new Vector3(0f, 0.5f * height, 0f) + Vector3.up * radius;
		Vector3 vector2 = pos + new Vector3(0f, 0.5f * height, 0f) - Vector3.up * radius;
		Gizmos.DrawWireSphere(vector, radius);
		Gizmos.DrawWireSphere(vector2, radius);
		Gizmos.DrawLine(vector + Vector3.forward * radius, vector2 + Vector3.forward * radius);
		Gizmos.DrawLine(vector + Vector3.right * radius, vector2 + Vector3.right * radius);
		Gizmos.DrawLine(vector + Vector3.back * radius, vector2 + Vector3.back * radius);
		Gizmos.DrawLine(vector + Vector3.left * radius, vector2 + Vector3.left * radius);
	}

	// Token: 0x060036FF RID: 14079 RVA: 0x001471A0 File Offset: 0x001453A0
	public static void DrawWireCapsuleZ(Vector3 pos, float radius, float height)
	{
		Vector3 vector = pos - new Vector3(0f, 0f, 0.5f * height) + Vector3.forward * radius;
		Vector3 vector2 = pos + new Vector3(0f, 0f, 0.5f * height) - Vector3.forward * radius;
		Gizmos.DrawWireSphere(vector, radius);
		Gizmos.DrawWireSphere(vector2, radius);
		Gizmos.DrawLine(vector + Vector3.up * radius, vector2 + Vector3.up * radius);
		Gizmos.DrawLine(vector + Vector3.right * radius, vector2 + Vector3.right * radius);
		Gizmos.DrawLine(vector + Vector3.down * radius, vector2 + Vector3.down * radius);
		Gizmos.DrawLine(vector + Vector3.left * radius, vector2 + Vector3.left * radius);
	}

	// Token: 0x06003700 RID: 14080 RVA: 0x001472B0 File Offset: 0x001454B0
	public static void DrawCapsuleX(Vector3 pos, float radius, float height)
	{
		Vector3 center = pos - new Vector3(0.5f * height, 0f, 0f);
		Vector3 center2 = pos + new Vector3(0.5f * height, 0f, 0f);
		Gizmos.DrawSphere(center, radius);
		Gizmos.DrawSphere(center2, radius);
	}

	// Token: 0x06003701 RID: 14081 RVA: 0x00147304 File Offset: 0x00145504
	public static void DrawCapsuleY(Vector3 pos, float radius, float height)
	{
		Vector3 center = pos - new Vector3(0f, 0.5f * height, 0f);
		Vector3 center2 = pos + new Vector3(0f, 0.5f * height, 0f);
		Gizmos.DrawSphere(center, radius);
		Gizmos.DrawSphere(center2, radius);
	}

	// Token: 0x06003702 RID: 14082 RVA: 0x00147358 File Offset: 0x00145558
	public static void DrawCapsuleZ(Vector3 pos, float radius, float height)
	{
		Vector3 center = pos - new Vector3(0f, 0f, 0.5f * height);
		Vector3 center2 = pos + new Vector3(0f, 0f, 0.5f * height);
		Gizmos.DrawSphere(center, radius);
		Gizmos.DrawSphere(center2, radius);
	}

	// Token: 0x06003703 RID: 14083 RVA: 0x001473AB File Offset: 0x001455AB
	public static void DrawWireCube(Vector3 pos, Vector3 size, Quaternion rot)
	{
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix = Matrix4x4.TRS(pos, rot, size);
		Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
		Gizmos.matrix = matrix;
	}

	// Token: 0x06003704 RID: 14084 RVA: 0x001473D3 File Offset: 0x001455D3
	public static void DrawCube(Vector3 pos, Vector3 size, Quaternion rot)
	{
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix = Matrix4x4.TRS(pos, rot, size);
		Gizmos.DrawCube(Vector3.zero, Vector3.one);
		Gizmos.matrix = matrix;
	}

	// Token: 0x06003705 RID: 14085 RVA: 0x001473FC File Offset: 0x001455FC
	public static void DrawWirePath(Vector3 a, Vector3 b, float thickness)
	{
		GizmosUtil.DrawWireCircleY(a, thickness);
		GizmosUtil.DrawWireCircleY(b, thickness);
		Vector3 normalized = (b - a).normalized;
		Vector3 a2 = Quaternion.Euler(0f, 90f, 0f) * normalized;
		Gizmos.DrawLine(b + a2 * thickness, a + a2 * thickness);
		Gizmos.DrawLine(b - a2 * thickness, a - a2 * thickness);
	}

	// Token: 0x06003706 RID: 14086 RVA: 0x00147480 File Offset: 0x00145680
	public static void DrawSemiCircle(float radius)
	{
		float num = radius * 0.017453292f * 0.5f;
		Vector3 vector = Mathf.Cos(num) * Vector3.forward + Mathf.Sin(num) * Vector3.right;
		Gizmos.DrawLine(Vector3.zero, vector);
		Vector3 vector2 = Mathf.Cos(-num) * Vector3.forward + Mathf.Sin(-num) * Vector3.right;
		Gizmos.DrawLine(Vector3.zero, vector2);
		float num2 = Mathf.Clamp(radius / 16f, 4f, 64f);
		float num3 = num / num2;
		for (float num4 = num; num4 > 0f; num4 -= num3)
		{
			Vector3 vector3 = Mathf.Cos(num4) * Vector3.forward + Mathf.Sin(num4) * Vector3.right;
			Gizmos.DrawLine(Vector3.zero, vector3);
			if (vector != Vector3.zero)
			{
				Gizmos.DrawLine(vector3, vector);
			}
			vector = vector3;
			Vector3 vector4 = Mathf.Cos(-num4) * Vector3.forward + Mathf.Sin(-num4) * Vector3.right;
			Gizmos.DrawLine(Vector3.zero, vector4);
			if (vector2 != Vector3.zero)
			{
				Gizmos.DrawLine(vector4, vector2);
			}
			vector2 = vector4;
		}
		Gizmos.DrawLine(vector, vector2);
	}

	// Token: 0x06003707 RID: 14087 RVA: 0x001475DC File Offset: 0x001457DC
	public static void DrawMeshes(Transform transform)
	{
		foreach (MeshRenderer meshRenderer in transform.GetComponentsInChildren<MeshRenderer>())
		{
			if (meshRenderer.enabled)
			{
				MeshFilter component = meshRenderer.GetComponent<MeshFilter>();
				if (component)
				{
					Transform transform2 = meshRenderer.transform;
					if (transform2 != null && component != null && component.sharedMesh != null && component.sharedMesh.normals != null && component.sharedMesh.normals.Length != 0)
					{
						Gizmos.DrawMesh(component.sharedMesh, transform2.position, transform2.rotation, transform2.lossyScale);
					}
				}
			}
		}
	}

	// Token: 0x06003708 RID: 14088 RVA: 0x00147684 File Offset: 0x00145884
	public static void DrawBounds(Transform transform)
	{
		Bounds bounds = transform.GetBounds(true, false, true);
		Vector3 lossyScale = transform.lossyScale;
		Quaternion rotation = transform.rotation;
		Vector3 pos = transform.position + rotation * Vector3.Scale(lossyScale, bounds.center);
		Vector3 size = Vector3.Scale(lossyScale, bounds.size);
		GizmosUtil.DrawCube(pos, size, rotation);
		GizmosUtil.DrawWireCube(pos, size, rotation);
	}
}
