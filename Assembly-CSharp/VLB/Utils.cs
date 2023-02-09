using System;
using UnityEngine;

namespace VLB
{
	// Token: 0x02000987 RID: 2439
	public static class Utils
	{
		// Token: 0x060039A2 RID: 14754 RVA: 0x00154C44 File Offset: 0x00152E44
		public static string GetPath(Transform current)
		{
			if (current.parent == null)
			{
				return "/" + current.name;
			}
			return Utils.GetPath(current.parent) + "/" + current.name;
		}

		// Token: 0x060039A3 RID: 14755 RVA: 0x00154C80 File Offset: 0x00152E80
		public static T NewWithComponent<T>(string name) where T : Component
		{
			return new GameObject(name, new Type[]
			{
				typeof(T)
			}).GetComponent<T>();
		}

		// Token: 0x060039A4 RID: 14756 RVA: 0x00154CA0 File Offset: 0x00152EA0
		public static T GetOrAddComponent<T>(this GameObject self) where T : Component
		{
			T t = self.GetComponent<T>();
			if (t == null)
			{
				t = self.AddComponent<T>();
			}
			return t;
		}

		// Token: 0x060039A5 RID: 14757 RVA: 0x00154CCA File Offset: 0x00152ECA
		public static T GetOrAddComponent<T>(this MonoBehaviour self) where T : Component
		{
			return self.gameObject.GetOrAddComponent<T>();
		}

		// Token: 0x060039A6 RID: 14758 RVA: 0x00154CD7 File Offset: 0x00152ED7
		public static bool HasFlag(this Enum mask, Enum flags)
		{
			return ((int)mask & (int)flags) == (int)flags;
		}

		// Token: 0x060039A7 RID: 14759 RVA: 0x00154CEE File Offset: 0x00152EEE
		public static Vector2 xy(this Vector3 aVector)
		{
			return new Vector2(aVector.x, aVector.y);
		}

		// Token: 0x060039A8 RID: 14760 RVA: 0x00154D01 File Offset: 0x00152F01
		public static Vector2 xz(this Vector3 aVector)
		{
			return new Vector2(aVector.x, aVector.z);
		}

		// Token: 0x060039A9 RID: 14761 RVA: 0x00154D14 File Offset: 0x00152F14
		public static Vector2 yz(this Vector3 aVector)
		{
			return new Vector2(aVector.y, aVector.z);
		}

		// Token: 0x060039AA RID: 14762 RVA: 0x00154D27 File Offset: 0x00152F27
		public static Vector2 yx(this Vector3 aVector)
		{
			return new Vector2(aVector.y, aVector.x);
		}

		// Token: 0x060039AB RID: 14763 RVA: 0x00154D3A File Offset: 0x00152F3A
		public static Vector2 zx(this Vector3 aVector)
		{
			return new Vector2(aVector.z, aVector.x);
		}

		// Token: 0x060039AC RID: 14764 RVA: 0x00154D4D File Offset: 0x00152F4D
		public static Vector2 zy(this Vector3 aVector)
		{
			return new Vector2(aVector.z, aVector.y);
		}

		// Token: 0x060039AD RID: 14765 RVA: 0x00154D60 File Offset: 0x00152F60
		public static float GetVolumeCubic(this Bounds self)
		{
			return self.size.x * self.size.y * self.size.z;
		}

		// Token: 0x060039AE RID: 14766 RVA: 0x00154D88 File Offset: 0x00152F88
		public static float GetMaxArea2D(this Bounds self)
		{
			return Mathf.Max(Mathf.Max(self.size.x * self.size.y, self.size.y * self.size.z), self.size.x * self.size.z);
		}

		// Token: 0x060039AF RID: 14767 RVA: 0x00154DEA File Offset: 0x00152FEA
		public static Color Opaque(this Color self)
		{
			return new Color(self.r, self.g, self.b, 1f);
		}

		// Token: 0x060039B0 RID: 14768 RVA: 0x00154E08 File Offset: 0x00153008
		public static void GizmosDrawPlane(Vector3 normal, Vector3 position, Color color, float size = 1f)
		{
			Vector3 vector = Vector3.Cross(normal, (Mathf.Abs(Vector3.Dot(normal, Vector3.forward)) < 0.999f) ? Vector3.forward : Vector3.up).normalized * size;
			Vector3 vector2 = position + vector;
			Vector3 vector3 = position - vector;
			vector = Quaternion.AngleAxis(90f, normal) * vector;
			Vector3 vector4 = position + vector;
			Vector3 vector5 = position - vector;
			Gizmos.matrix = Matrix4x4.identity;
			Gizmos.color = color;
			Gizmos.DrawLine(vector2, vector3);
			Gizmos.DrawLine(vector4, vector5);
			Gizmos.DrawLine(vector2, vector4);
			Gizmos.DrawLine(vector4, vector3);
			Gizmos.DrawLine(vector3, vector5);
			Gizmos.DrawLine(vector5, vector2);
		}

		// Token: 0x060039B1 RID: 14769 RVA: 0x00154EBE File Offset: 0x001530BE
		public static Plane TranslateCustom(this Plane plane, Vector3 translation)
		{
			plane.distance += Vector3.Dot(translation.normalized, plane.normal) * translation.magnitude;
			return plane;
		}

		// Token: 0x060039B2 RID: 14770 RVA: 0x00154EEC File Offset: 0x001530EC
		public static bool IsValid(this Plane plane)
		{
			return plane.normal.sqrMagnitude > 0.5f;
		}

		// Token: 0x060039B3 RID: 14771 RVA: 0x00154F10 File Offset: 0x00153110
		public static Matrix4x4 SampleInMatrix(this Gradient self, int floatPackingPrecision)
		{
			Matrix4x4 result = default(Matrix4x4);
			for (int i = 0; i < 16; i++)
			{
				Color color = self.Evaluate(Mathf.Clamp01((float)i / 15f));
				result[i] = color.PackToFloat(floatPackingPrecision);
			}
			return result;
		}

		// Token: 0x060039B4 RID: 14772 RVA: 0x00154F58 File Offset: 0x00153158
		public static Color[] SampleInArray(this Gradient self, int samplesCount)
		{
			Color[] array = new Color[samplesCount];
			for (int i = 0; i < samplesCount; i++)
			{
				array[i] = self.Evaluate(Mathf.Clamp01((float)i / (float)(samplesCount - 1)));
			}
			return array;
		}

		// Token: 0x060039B5 RID: 14773 RVA: 0x00154F92 File Offset: 0x00153192
		private static Vector4 Vector4_Floor(Vector4 vec)
		{
			return new Vector4(Mathf.Floor(vec.x), Mathf.Floor(vec.y), Mathf.Floor(vec.z), Mathf.Floor(vec.w));
		}

		// Token: 0x060039B6 RID: 14774 RVA: 0x00154FC8 File Offset: 0x001531C8
		public static float PackToFloat(this Color color, int floatPackingPrecision)
		{
			Vector4 vector = Utils.Vector4_Floor(color * (float)(floatPackingPrecision - 1));
			return 0f + vector.x * (float)floatPackingPrecision * (float)floatPackingPrecision * (float)floatPackingPrecision + vector.y * (float)floatPackingPrecision * (float)floatPackingPrecision + vector.z * (float)floatPackingPrecision + vector.w;
		}

		// Token: 0x060039B7 RID: 14775 RVA: 0x0015501D File Offset: 0x0015321D
		public static Utils.FloatPackingPrecision GetFloatPackingPrecision()
		{
			if (Utils.ms_FloatPackingPrecision == Utils.FloatPackingPrecision.Undef)
			{
				Utils.ms_FloatPackingPrecision = ((SystemInfo.graphicsShaderLevel >= 35) ? Utils.FloatPackingPrecision.High : Utils.FloatPackingPrecision.Low);
			}
			return Utils.ms_FloatPackingPrecision;
		}

		// Token: 0x060039B8 RID: 14776 RVA: 0x000059DD File Offset: 0x00003BDD
		public static void MarkCurrentSceneDirty()
		{
		}

		// Token: 0x04003446 RID: 13382
		private static Utils.FloatPackingPrecision ms_FloatPackingPrecision;

		// Token: 0x04003447 RID: 13383
		private const int kFloatPackingHighMinShaderLevel = 35;

		// Token: 0x02000E84 RID: 3716
		public enum FloatPackingPrecision
		{
			// Token: 0x04004AE1 RID: 19169
			High = 64,
			// Token: 0x04004AE2 RID: 19170
			Low = 8,
			// Token: 0x04004AE3 RID: 19171
			Undef = 0
		}
	}
}
