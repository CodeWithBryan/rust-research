using System;
using UnityEngine;

// Token: 0x02000241 RID: 577
public class DecayPoint : PrefabAttribute
{
	// Token: 0x06001B27 RID: 6951 RVA: 0x000BE111 File Offset: 0x000BC311
	public bool IsOccupied(BaseEntity entity)
	{
		return entity.IsOccupied(this.socket);
	}

	// Token: 0x06001B28 RID: 6952 RVA: 0x000BE11F File Offset: 0x000BC31F
	protected override Type GetIndexedType()
	{
		return typeof(DecayPoint);
	}

	// Token: 0x04001436 RID: 5174
	[Tooltip("If this point is occupied this will take this % off the power of the decay")]
	public float protection = 0.25f;

	// Token: 0x04001437 RID: 5175
	public Socket_Base socket;
}
