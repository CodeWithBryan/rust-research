using System;
using UnityEngine;

// Token: 0x02000191 RID: 401
public class NPCMissionProvider : NPCTalking, IMissionProvider
{
	// Token: 0x06001754 RID: 5972 RVA: 0x0004D6FC File Offset: 0x0004B8FC
	public uint ProviderID()
	{
		return this.net.ID;
	}

	// Token: 0x06001755 RID: 5973 RVA: 0x000299AB File Offset: 0x00027BAB
	public Vector3 ProviderPosition()
	{
		return base.transform.position;
	}

	// Token: 0x06001756 RID: 5974 RVA: 0x00002E37 File Offset: 0x00001037
	public BaseEntity Entity()
	{
		return this;
	}

	// Token: 0x06001757 RID: 5975 RVA: 0x000AE8E8 File Offset: 0x000ACAE8
	public override void OnConversationEnded(BasePlayer player)
	{
		player.ProcessMissionEvent(BaseMission.MissionEventType.CONVERSATION, this.ProviderID().ToString(), 0f);
		base.OnConversationEnded(player);
	}

	// Token: 0x06001758 RID: 5976 RVA: 0x000AE918 File Offset: 0x000ACB18
	public override void OnConversationStarted(BasePlayer speakingTo)
	{
		speakingTo.ProcessMissionEvent(BaseMission.MissionEventType.CONVERSATION, this.ProviderID().ToString(), 1f);
		base.OnConversationStarted(speakingTo);
	}

	// Token: 0x06001759 RID: 5977 RVA: 0x000AE948 File Offset: 0x000ACB48
	public bool ContainsSpeech(string speech)
	{
		ConversationData[] conversations = this.conversations;
		for (int i = 0; i < conversations.Length; i++)
		{
			ConversationData.SpeechNode[] speeches = conversations[i].speeches;
			for (int j = 0; j < speeches.Length; j++)
			{
				if (speeches[j].shortname == speech)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x0600175A RID: 5978 RVA: 0x000AE994 File Offset: 0x000ACB94
	public string IntroOverride(string overrideSpeech)
	{
		if (!this.ContainsSpeech(overrideSpeech))
		{
			return "intro";
		}
		return overrideSpeech;
	}

	// Token: 0x0600175B RID: 5979 RVA: 0x000AE9A8 File Offset: 0x000ACBA8
	public override string GetConversationStartSpeech(BasePlayer player)
	{
		string text = "";
		foreach (BaseMission.MissionInstance missionInstance in player.missions)
		{
			if (missionInstance.status == BaseMission.MissionStatus.Active)
			{
				text = this.IntroOverride("missionactive");
			}
			if (missionInstance.status == BaseMission.MissionStatus.Completed && missionInstance.providerID == this.ProviderID() && Time.time - missionInstance.endTime < 5f)
			{
				text = this.IntroOverride("missionreturn");
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			text = base.GetConversationStartSpeech(player);
		}
		return text;
	}

	// Token: 0x0600175C RID: 5980 RVA: 0x000AEA58 File Offset: 0x000ACC58
	public override void OnConversationAction(BasePlayer player, string action)
	{
		if (action.Contains("assignmission"))
		{
			int num = action.IndexOf(" ");
			BaseMission fromShortName = MissionManifest.GetFromShortName(action.Substring(num + 1));
			if (fromShortName)
			{
				BaseMission.AssignMission(player, this, fromShortName);
			}
		}
		base.OnConversationAction(player, action);
	}

	// Token: 0x04001058 RID: 4184
	public MissionManifest manifest;
}
