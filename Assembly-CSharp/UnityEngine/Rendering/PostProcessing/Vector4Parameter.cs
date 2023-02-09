using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A41 RID: 2625
	[Serializable]
	public sealed class Vector4Parameter : ParameterOverride<Vector4>
	{
		// Token: 0x06003E14 RID: 15892 RVA: 0x0016D068 File Offset: 0x0016B268
		public override void Interp(Vector4 from, Vector4 to, float t)
		{
			this.value.x = from.x + (to.x - from.x) * t;
			this.value.y = from.y + (to.y - from.y) * t;
			this.value.z = from.z + (to.z - from.z) * t;
			this.value.w = from.w + (to.w - from.w) * t;
		}

		// Token: 0x06003E15 RID: 15893 RVA: 0x0016D0F9 File Offset: 0x0016B2F9
		public static implicit operator Vector2(Vector4Parameter prop)
		{
			return prop.value;
		}

		// Token: 0x06003E16 RID: 15894 RVA: 0x0016D106 File Offset: 0x0016B306
		public static implicit operator Vector3(Vector4Parameter prop)
		{
			return prop.value;
		}
	}
}
