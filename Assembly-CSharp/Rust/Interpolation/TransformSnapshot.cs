using System;
using UnityEngine;

namespace Rust.Interpolation
{
	// Token: 0x02000AF1 RID: 2801
	public struct TransformSnapshot : ISnapshot<TransformSnapshot>
	{
		// Token: 0x170005DA RID: 1498
		// (get) Token: 0x06004349 RID: 17225 RVA: 0x00186D9C File Offset: 0x00184F9C
		// (set) Token: 0x0600434A RID: 17226 RVA: 0x00186DA4 File Offset: 0x00184FA4
		public float Time { get; set; }

		// Token: 0x0600434B RID: 17227 RVA: 0x00186DAD File Offset: 0x00184FAD
		public TransformSnapshot(float time, Vector3 pos, Quaternion rot)
		{
			this.Time = time;
			this.pos = pos;
			this.rot = rot;
		}

		// Token: 0x0600434C RID: 17228 RVA: 0x00186DC4 File Offset: 0x00184FC4
		public void MatchValuesTo(TransformSnapshot entry)
		{
			this.pos = entry.pos;
			this.rot = entry.rot;
		}

		// Token: 0x0600434D RID: 17229 RVA: 0x00186DDE File Offset: 0x00184FDE
		public void Lerp(TransformSnapshot prev, TransformSnapshot next, float delta)
		{
			this.pos = Vector3.LerpUnclamped(prev.pos, next.pos, delta);
			this.rot = Quaternion.SlerpUnclamped(prev.rot, next.rot, delta);
		}

		// Token: 0x0600434E RID: 17230 RVA: 0x00186E10 File Offset: 0x00185010
		public TransformSnapshot GetNew()
		{
			return default(TransformSnapshot);
		}

		// Token: 0x04003BDA RID: 15322
		public Vector3 pos;

		// Token: 0x04003BDB RID: 15323
		public Quaternion rot;
	}
}
