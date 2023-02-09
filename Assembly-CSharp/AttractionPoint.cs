using System;

// Token: 0x02000237 RID: 567
public class AttractionPoint : PrefabAttribute
{
	// Token: 0x06001AF6 RID: 6902 RVA: 0x000BCAF7 File Offset: 0x000BACF7
	protected override Type GetIndexedType()
	{
		return typeof(AttractionPoint);
	}

	// Token: 0x04001401 RID: 5121
	public string groupName;
}
