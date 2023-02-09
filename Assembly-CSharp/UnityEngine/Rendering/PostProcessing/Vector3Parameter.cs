using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A40 RID: 2624
	[Serializable]
	public sealed class Vector3Parameter : ParameterOverride<Vector3>
	{
		// Token: 0x06003E10 RID: 15888 RVA: 0x0016CFD4 File Offset: 0x0016B1D4
		public override void Interp(Vector3 from, Vector3 to, float t)
		{
			this.value.x = from.x + (to.x - from.x) * t;
			this.value.y = from.y + (to.y - from.y) * t;
			this.value.z = from.z + (to.z - from.z) * t;
		}

		// Token: 0x06003E11 RID: 15889 RVA: 0x0016D044 File Offset: 0x0016B244
		public static implicit operator Vector2(Vector3Parameter prop)
		{
			return prop.value;
		}

		// Token: 0x06003E12 RID: 15890 RVA: 0x0016D051 File Offset: 0x0016B251
		public static implicit operator Vector4(Vector3Parameter prop)
		{
			return prop.value;
		}
	}
}
