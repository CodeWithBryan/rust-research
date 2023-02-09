using System;
using System.Collections.Generic;
using ProtoBuf;

// Token: 0x02000597 RID: 1431
public class ItemCraftTask
{
	// Token: 0x040022A2 RID: 8866
	public ItemBlueprint blueprint;

	// Token: 0x040022A3 RID: 8867
	public float endTime;

	// Token: 0x040022A4 RID: 8868
	public int taskUID;

	// Token: 0x040022A5 RID: 8869
	public global::BasePlayer owner;

	// Token: 0x040022A6 RID: 8870
	public bool cancelled;

	// Token: 0x040022A7 RID: 8871
	public ProtoBuf.Item.InstanceData instanceData;

	// Token: 0x040022A8 RID: 8872
	public int amount = 1;

	// Token: 0x040022A9 RID: 8873
	public int skinID;

	// Token: 0x040022AA RID: 8874
	public List<ulong> potentialOwners;

	// Token: 0x040022AB RID: 8875
	public List<global::Item> takenItems;

	// Token: 0x040022AC RID: 8876
	public int numCrafted;

	// Token: 0x040022AD RID: 8877
	public float conditionScale = 1f;

	// Token: 0x040022AE RID: 8878
	public float workSecondsComplete;

	// Token: 0x040022AF RID: 8879
	public float worksecondsRequired;
}
