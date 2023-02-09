using System;
using UnityEngine;

// Token: 0x02000343 RID: 835
public class AIMemory
{
	// Token: 0x04001823 RID: 6179
	public AIMemoryBank<BaseEntity> Entity = new AIMemoryBank<BaseEntity>(MemoryBankType.Entity, 8);

	// Token: 0x04001824 RID: 6180
	public AIMemoryBank<Vector3> Position = new AIMemoryBank<Vector3>(MemoryBankType.Position, 8);

	// Token: 0x04001825 RID: 6181
	public AIMemoryBank<AIPoint> AIPoint = new AIMemoryBank<AIPoint>(MemoryBankType.AIPoint, 8);
}
