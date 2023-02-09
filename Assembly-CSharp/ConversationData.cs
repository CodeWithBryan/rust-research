using System;
using UnityEngine;

// Token: 0x0200018D RID: 397
[CreateAssetMenu(fileName = "NewConversation", menuName = "Rust/ConversationData", order = 1)]
public class ConversationData : ScriptableObject
{
	// Token: 0x170001C8 RID: 456
	// (get) Token: 0x0600174A RID: 5962 RVA: 0x000AE7F0 File Offset: 0x000AC9F0
	public string providerName
	{
		get
		{
			return this.providerNameTranslated.translated;
		}
	}

	// Token: 0x0600174B RID: 5963 RVA: 0x000AE800 File Offset: 0x000ACA00
	public int GetSpeechNodeIndex(string speechShortName)
	{
		for (int i = 0; i < this.speeches.Length; i++)
		{
			if (this.speeches[i].shortname == speechShortName)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x04001052 RID: 4178
	public string shortname;

	// Token: 0x04001053 RID: 4179
	public Translate.Phrase providerNameTranslated;

	// Token: 0x04001054 RID: 4180
	public ConversationData.SpeechNode[] speeches;

	// Token: 0x02000BE5 RID: 3045
	[Serializable]
	public class ConversationCondition
	{
		// Token: 0x06004B67 RID: 19303 RVA: 0x00191D60 File Offset: 0x0018FF60
		public bool Passes(BasePlayer player, IConversationProvider provider)
		{
			bool flag = false;
			if (this.conditionType == ConversationData.ConversationCondition.ConditionType.HASSCRAP)
			{
				flag = ((long)player.inventory.GetAmount(ItemManager.FindItemDefinition("scrap").itemid) >= (long)((ulong)this.conditionAmount));
			}
			else if (this.conditionType == ConversationData.ConversationCondition.ConditionType.HASHEALTH)
			{
				flag = (player.health >= this.conditionAmount);
			}
			else if (this.conditionType == ConversationData.ConversationCondition.ConditionType.PROVIDERBUSY)
			{
				flag = provider.ProviderBusy();
			}
			else if (this.conditionType == ConversationData.ConversationCondition.ConditionType.MISSIONCOMPLETE)
			{
				flag = player.HasCompletedMission(this.conditionAmount);
			}
			else if (this.conditionType == ConversationData.ConversationCondition.ConditionType.MISSIONATTEMPTED)
			{
				flag = player.HasAttemptedMission(this.conditionAmount);
			}
			else if (this.conditionType == ConversationData.ConversationCondition.ConditionType.CANACCEPT)
			{
				flag = player.CanAcceptMission(this.conditionAmount);
			}
			if (!this.inverse)
			{
				return flag;
			}
			return !flag;
		}

		// Token: 0x0400401B RID: 16411
		public ConversationData.ConversationCondition.ConditionType conditionType;

		// Token: 0x0400401C RID: 16412
		public uint conditionAmount;

		// Token: 0x0400401D RID: 16413
		public bool inverse;

		// Token: 0x0400401E RID: 16414
		public string failedSpeechNode;

		// Token: 0x02000F62 RID: 3938
		public enum ConditionType
		{
			// Token: 0x04004E11 RID: 19985
			NONE,
			// Token: 0x04004E12 RID: 19986
			HASHEALTH,
			// Token: 0x04004E13 RID: 19987
			HASSCRAP,
			// Token: 0x04004E14 RID: 19988
			PROVIDERBUSY,
			// Token: 0x04004E15 RID: 19989
			MISSIONCOMPLETE,
			// Token: 0x04004E16 RID: 19990
			MISSIONATTEMPTED,
			// Token: 0x04004E17 RID: 19991
			CANACCEPT
		}
	}

	// Token: 0x02000BE6 RID: 3046
	[Serializable]
	public class ResponseNode
	{
		// Token: 0x17000604 RID: 1540
		// (get) Token: 0x06004B69 RID: 19305 RVA: 0x00191E27 File Offset: 0x00190027
		public string responseText
		{
			get
			{
				return this.responseTextLocalized.translated;
			}
		}

		// Token: 0x06004B6A RID: 19306 RVA: 0x00191E34 File Offset: 0x00190034
		public bool PassesConditions(BasePlayer player, IConversationProvider provider)
		{
			ConversationData.ConversationCondition[] array = this.conditions;
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].Passes(player, provider))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06004B6B RID: 19307 RVA: 0x00191E68 File Offset: 0x00190068
		public string GetFailedSpeechNode(BasePlayer player, IConversationProvider provider)
		{
			foreach (ConversationData.ConversationCondition conversationCondition in this.conditions)
			{
				if (!conversationCondition.Passes(player, provider))
				{
					return conversationCondition.failedSpeechNode;
				}
			}
			return "";
		}

		// Token: 0x0400401F RID: 16415
		public Translate.Phrase responseTextLocalized;

		// Token: 0x04004020 RID: 16416
		public ConversationData.ConversationCondition[] conditions;

		// Token: 0x04004021 RID: 16417
		public string actionString;

		// Token: 0x04004022 RID: 16418
		public string resultingSpeechNode;
	}

	// Token: 0x02000BE7 RID: 3047
	[Serializable]
	public class SpeechNode
	{
		// Token: 0x17000605 RID: 1541
		// (get) Token: 0x06004B6D RID: 19309 RVA: 0x00191EA4 File Offset: 0x001900A4
		public string statement
		{
			get
			{
				return this.statementLocalized.translated;
			}
		}

		// Token: 0x04004023 RID: 16419
		public string shortname;

		// Token: 0x04004024 RID: 16420
		public Translate.Phrase statementLocalized;

		// Token: 0x04004025 RID: 16421
		public ConversationData.ResponseNode[] responses;

		// Token: 0x04004026 RID: 16422
		public Vector2 nodePosition;
	}
}
