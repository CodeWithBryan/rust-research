using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002EC RID: 748
public class ItemButtonTools : MonoBehaviour
{
	// Token: 0x06001D59 RID: 7513 RVA: 0x000C8DD8 File Offset: 0x000C6FD8
	public void GiveSelf(int amount)
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "inventory.giveid", new object[]
		{
			this.itemDef.itemid,
			amount
		});
	}

	// Token: 0x06001D5A RID: 7514 RVA: 0x000C8E0C File Offset: 0x000C700C
	public void GiveArmed()
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "inventory.givearm", new object[]
		{
			this.itemDef.itemid
		});
	}

	// Token: 0x06001D5B RID: 7515 RVA: 0x000059DD File Offset: 0x00003BDD
	public void GiveBlueprint()
	{
	}

	// Token: 0x040016BE RID: 5822
	public Image image;

	// Token: 0x040016BF RID: 5823
	public ItemDefinition itemDef;
}
