using System;

// Token: 0x020003B6 RID: 950
public class ItemModSign : ItemModAssociatedEntity<SignContent>
{
	// Token: 0x1700027C RID: 636
	// (get) Token: 0x06002086 RID: 8326 RVA: 0x00003A54 File Offset: 0x00001C54
	protected override bool AllowNullParenting
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700027D RID: 637
	// (get) Token: 0x06002087 RID: 8327 RVA: 0x00007074 File Offset: 0x00005274
	protected override bool ShouldAutoCreateEntity
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06002088 RID: 8328 RVA: 0x000D41C8 File Offset: 0x000D23C8
	public void OnSignPickedUp(ISignage s, IUGCBrowserEntity ugc, Item toItem)
	{
		SignContent signContent = base.CreateAssociatedEntity(toItem);
		if (signContent != null)
		{
			signContent.CopyInfoFromSign(s, ugc);
		}
	}
}
