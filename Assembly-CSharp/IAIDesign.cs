using System;
using ProtoBuf;

// Token: 0x0200036F RID: 879
internal interface IAIDesign
{
	// Token: 0x06001EF9 RID: 7929
	void LoadAIDesign(ProtoBuf.AIDesign design, global::BasePlayer player);

	// Token: 0x06001EFA RID: 7930
	void StopDesigning();

	// Token: 0x06001EFB RID: 7931
	bool CanPlayerDesignAI(global::BasePlayer player);
}
