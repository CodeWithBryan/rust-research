using System;
using UnityEngine;

// Token: 0x02000255 RID: 597
public class SocketHandle : PrefabAttribute
{
	// Token: 0x06001B6E RID: 7022 RVA: 0x000BF442 File Offset: 0x000BD642
	protected override Type GetIndexedType()
	{
		return typeof(SocketHandle);
	}

	// Token: 0x06001B6F RID: 7023 RVA: 0x000BF450 File Offset: 0x000BD650
	internal void AdjustTarget(ref Construction.Target target, float maxplaceDistance)
	{
		Vector3 worldPosition = this.worldPosition;
		Vector3 a = target.ray.origin + target.ray.direction * maxplaceDistance - worldPosition;
		target.ray.direction = (a - target.ray.origin).normalized;
	}
}
