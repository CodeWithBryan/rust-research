using System;

// Token: 0x0200037E RID: 894
public class ItemModCassette : ItemModAssociatedEntity<Cassette>
{
	// Token: 0x17000262 RID: 610
	// (get) Token: 0x06001F3E RID: 7998 RVA: 0x00003A54 File Offset: 0x00001C54
	protected override bool AllowNullParenting
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000263 RID: 611
	// (get) Token: 0x06001F3F RID: 7999 RVA: 0x00003A54 File Offset: 0x00001C54
	protected override bool AllowHeldEntityParenting
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001F40 RID: 8000 RVA: 0x000CF57F File Offset: 0x000CD77F
	protected override void OnAssociatedItemCreated(Cassette ent)
	{
		base.OnAssociatedItemCreated(ent);
		ent.AssignPreloadContent();
	}

	// Token: 0x040018A3 RID: 6307
	public int noteSpriteIndex;

	// Token: 0x040018A4 RID: 6308
	public PreloadedCassetteContent PreloadedContent;
}
