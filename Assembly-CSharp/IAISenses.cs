using System;

// Token: 0x02000366 RID: 870
internal interface IAISenses
{
	// Token: 0x06001EDF RID: 7903
	bool IsThreat(BaseEntity entity);

	// Token: 0x06001EE0 RID: 7904
	bool IsTarget(BaseEntity entity);

	// Token: 0x06001EE1 RID: 7905
	bool IsFriendly(BaseEntity entity);
}
