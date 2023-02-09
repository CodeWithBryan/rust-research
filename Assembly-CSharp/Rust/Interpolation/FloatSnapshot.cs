using System;
using UnityEngine;

namespace Rust.Interpolation
{
	// Token: 0x02000AF2 RID: 2802
	public struct FloatSnapshot : ISnapshot<FloatSnapshot>
	{
		// Token: 0x170005DB RID: 1499
		// (get) Token: 0x0600434F RID: 17231 RVA: 0x00186E26 File Offset: 0x00185026
		// (set) Token: 0x06004350 RID: 17232 RVA: 0x00186E2E File Offset: 0x0018502E
		public float Time { get; set; }

		// Token: 0x06004351 RID: 17233 RVA: 0x00186E37 File Offset: 0x00185037
		public FloatSnapshot(float time, float value)
		{
			this.Time = time;
			this.value = value;
		}

		// Token: 0x06004352 RID: 17234 RVA: 0x00186E47 File Offset: 0x00185047
		public void MatchValuesTo(FloatSnapshot entry)
		{
			this.value = entry.value;
		}

		// Token: 0x06004353 RID: 17235 RVA: 0x00186E55 File Offset: 0x00185055
		public void Lerp(FloatSnapshot prev, FloatSnapshot next, float delta)
		{
			this.value = Mathf.Lerp(prev.value, next.value, delta);
		}

		// Token: 0x06004354 RID: 17236 RVA: 0x00186E70 File Offset: 0x00185070
		public FloatSnapshot GetNew()
		{
			return default(FloatSnapshot);
		}

		// Token: 0x04003BDD RID: 15325
		public float value;
	}
}
