using System;

// Token: 0x02000393 RID: 915
public class Keycard : AttackEntity
{
	// Token: 0x1700026F RID: 623
	// (get) Token: 0x06001FD2 RID: 8146 RVA: 0x000D18B4 File Offset: 0x000CFAB4
	public int accessLevel
	{
		get
		{
			Item item = this.GetItem();
			if (item == null)
			{
				return 0;
			}
			ItemModKeycard component = item.info.GetComponent<ItemModKeycard>();
			if (component == null)
			{
				return 0;
			}
			return component.accessLevel;
		}
	}
}
