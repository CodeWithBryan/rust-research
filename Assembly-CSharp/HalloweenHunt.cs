using System;
using System.Collections.Generic;
using ConVar;
using Facepunch.Rust;
using UnityEngine;

// Token: 0x02000159 RID: 345
public class HalloweenHunt : EggHuntEvent
{
	// Token: 0x06001659 RID: 5721 RVA: 0x000A9F3C File Offset: 0x000A813C
	public override void PrintWinnersAndAward()
	{
		List<EggHuntEvent.EggHunter> topHunters = base.GetTopHunters();
		if (topHunters.Count > 0)
		{
			EggHuntEvent.EggHunter eggHunter = topHunters[0];
			Chat.Broadcast(string.Concat(new object[]
			{
				eggHunter.displayName,
				" is the top creep with ",
				eggHunter.numEggs,
				" candies collected."
			}), "", "#eee", 0UL);
			for (int i = 0; i < topHunters.Count; i++)
			{
				EggHuntEvent.EggHunter eggHunter2 = topHunters[i];
				BasePlayer basePlayer = BasePlayer.FindByID(eggHunter2.userid);
				if (basePlayer)
				{
					basePlayer.ChatMessage(string.Concat(new object[]
					{
						"You placed ",
						i + 1,
						" of ",
						topHunters.Count,
						" with ",
						topHunters[i].numEggs,
						" candies collected."
					}));
					Analytics.Server.ReportCandiesCollectedByPlayer(topHunters[i].numEggs);
				}
				else
				{
					Debug.LogWarning("EggHuntEvent Printwinners could not find player with id :" + eggHunter2.userid);
				}
			}
			Analytics.Server.ReportPlayersParticipatedInHalloweenEvent(topHunters.Count);
			int num = 0;
			while (num < this.placementAwards.Length && num < topHunters.Count)
			{
				BasePlayer basePlayer2 = BasePlayer.FindByID(topHunters[num].userid);
				if (basePlayer2)
				{
					basePlayer2.inventory.GiveItem(ItemManager.Create(this.placementAwards[num].itemDef, (int)this.placementAwards[num].amount, 0UL), basePlayer2.inventory.containerMain);
					basePlayer2.ChatMessage(string.Concat(new object[]
					{
						"You received ",
						(int)this.placementAwards[num].amount,
						"x ",
						this.placementAwards[num].itemDef.displayName.english,
						" as an award!"
					}));
				}
				num++;
			}
			return;
		}
		Chat.Broadcast("Wow, no one played so no one won.", "", "#eee", 0UL);
	}
}
