using System;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x02000AFC RID: 2812
	public struct Sensation
	{
		// Token: 0x04003C17 RID: 15383
		public SensationType Type;

		// Token: 0x04003C18 RID: 15384
		public Vector3 Position;

		// Token: 0x04003C19 RID: 15385
		public float Radius;

		// Token: 0x04003C1A RID: 15386
		public float DamagePotential;

		// Token: 0x04003C1B RID: 15387
		public BaseEntity Initiator;

		// Token: 0x04003C1C RID: 15388
		public BasePlayer InitiatorPlayer;

		// Token: 0x04003C1D RID: 15389
		public BaseEntity UsedEntity;
	}
}
