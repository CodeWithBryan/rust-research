using System;
using Facepunch.Extend;
using UnityEngine;

// Token: 0x0200016A RID: 362
[ConsoleSystem.Factory("note")]
public class note : ConsoleSystem
{
	// Token: 0x06001690 RID: 5776 RVA: 0x000AB544 File Offset: 0x000A9744
	[ServerUserVar]
	public static void update(ConsoleSystem.Arg arg)
	{
		uint @uint = arg.GetUInt(0, 0U);
		string @string = arg.GetString(1, "");
		Item item = arg.Player().inventory.FindItemUID(@uint);
		if (item == null)
		{
			return;
		}
		item.text = @string.Truncate(1024, null);
		item.MarkDirty();
	}
}
