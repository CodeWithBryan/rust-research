using System;

// Token: 0x0200016F RID: 367
public class ItemModPhoto : ItemModAssociatedEntity<PhotoEntity>
{
	// Token: 0x170001BD RID: 445
	// (get) Token: 0x0600169F RID: 5791 RVA: 0x00003A54 File Offset: 0x00001C54
	protected override bool AllowNullParenting
	{
		get
		{
			return true;
		}
	}
}
