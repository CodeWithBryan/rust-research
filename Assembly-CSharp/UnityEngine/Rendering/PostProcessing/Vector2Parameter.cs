using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A3F RID: 2623
	[Serializable]
	public sealed class Vector2Parameter : ParameterOverride<Vector2>
	{
		// Token: 0x06003E0C RID: 15884 RVA: 0x0016CF60 File Offset: 0x0016B160
		public override void Interp(Vector2 from, Vector2 to, float t)
		{
			this.value.x = from.x + (to.x - from.x) * t;
			this.value.y = from.y + (to.y - from.y) * t;
		}

		// Token: 0x06003E0D RID: 15885 RVA: 0x0016CFAF File Offset: 0x0016B1AF
		public static implicit operator Vector3(Vector2Parameter prop)
		{
			return prop.value;
		}

		// Token: 0x06003E0E RID: 15886 RVA: 0x0016CFBC File Offset: 0x0016B1BC
		public static implicit operator Vector4(Vector2Parameter prop)
		{
			return prop.value;
		}
	}
}
