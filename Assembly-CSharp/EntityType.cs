using System;

// Token: 0x0200036D RID: 877
[Flags]
public enum EntityType
{
	// Token: 0x04001868 RID: 6248
	Player = 1,
	// Token: 0x04001869 RID: 6249
	NPC = 2,
	// Token: 0x0400186A RID: 6250
	WorldItem = 4,
	// Token: 0x0400186B RID: 6251
	Corpse = 8,
	// Token: 0x0400186C RID: 6252
	TimedExplosive = 16,
	// Token: 0x0400186D RID: 6253
	Chair = 32,
	// Token: 0x0400186E RID: 6254
	BasePlayerNPC = 64
}
