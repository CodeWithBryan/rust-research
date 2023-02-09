using System;
using System.Collections.Generic;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000A1 RID: 161
public class NPCTalking : NPCShopKeeper, IConversationProvider
{
	// Token: 0x06000EC9 RID: 3785 RVA: 0x0007B820 File Offset: 0x00079A20
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("NPCTalking.OnRpcMessage", 0))
		{
			if (rpc == 4224060672U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ConversationAction ");
				}
				using (TimeWarning.New("ConversationAction", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.CallsPerSecond.Test(4224060672U, "ConversationAction", this, player, 5UL))
						{
							return true;
						}
						if (!BaseEntity.RPC_Server.MaxDistance.Test(4224060672U, "ConversationAction", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ConversationAction(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in ConversationAction");
					}
				}
				return true;
			}
			if (rpc == 2112414875U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_BeginTalking ");
				}
				using (TimeWarning.New("Server_BeginTalking", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.CallsPerSecond.Test(2112414875U, "Server_BeginTalking", this, player, 1UL))
						{
							return true;
						}
						if (!BaseEntity.RPC_Server.MaxDistance.Test(2112414875U, "Server_BeginTalking", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg3 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_BeginTalking(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in Server_BeginTalking");
					}
				}
				return true;
			}
			if (rpc == 1597539152U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_EndTalking ");
				}
				using (TimeWarning.New("Server_EndTalking", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.CallsPerSecond.Test(1597539152U, "Server_EndTalking", this, player, 1UL))
						{
							return true;
						}
						if (!BaseEntity.RPC_Server.MaxDistance.Test(1597539152U, "Server_EndTalking", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg4 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_EndTalking(msg4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in Server_EndTalking");
					}
				}
				return true;
			}
			if (rpc == 2713250658U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_ResponsePressed ");
				}
				using (TimeWarning.New("Server_ResponsePressed", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.CallsPerSecond.Test(2713250658U, "Server_ResponsePressed", this, player, 5UL))
						{
							return true;
						}
						if (!BaseEntity.RPC_Server.MaxDistance.Test(2713250658U, "Server_ResponsePressed", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg5 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_ResponsePressed(msg5);
						}
					}
					catch (Exception exception4)
					{
						Debug.LogException(exception4);
						player.Kick("RPC Error in Server_ResponsePressed");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000ECA RID: 3786 RVA: 0x0007BE48 File Offset: 0x0007A048
	public int GetConversationIndex(string conversationName)
	{
		for (int i = 0; i < this.conversations.Length; i++)
		{
			if (this.conversations[i].shortname == conversationName)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06000ECB RID: 3787 RVA: 0x0007BE80 File Offset: 0x0007A080
	public virtual string GetConversationStartSpeech(BasePlayer player)
	{
		return "intro";
	}

	// Token: 0x06000ECC RID: 3788 RVA: 0x0007BE87 File Offset: 0x0007A087
	public ConversationData GetConversation(string conversationName)
	{
		return this.GetConversation(this.GetConversationIndex(conversationName));
	}

	// Token: 0x06000ECD RID: 3789 RVA: 0x0007BE96 File Offset: 0x0007A096
	public ConversationData GetConversation(int index)
	{
		return this.conversations[index];
	}

	// Token: 0x06000ECE RID: 3790 RVA: 0x0007BEA0 File Offset: 0x0007A0A0
	public virtual ConversationData GetConversationFor(BasePlayer player)
	{
		return this.conversations[0];
	}

	// Token: 0x06000ECF RID: 3791 RVA: 0x00020A80 File Offset: 0x0001EC80
	public bool ProviderBusy()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	// Token: 0x06000ED0 RID: 3792 RVA: 0x0007BEAA File Offset: 0x0007A0AA
	public void ForceEndConversation(BasePlayer player)
	{
		base.ClientRPCPlayer(null, player, "Client_EndConversation");
	}

	// Token: 0x06000ED1 RID: 3793 RVA: 0x0007BEB9 File Offset: 0x0007A0B9
	public void ForceSpeechNode(BasePlayer player, int speechNodeIndex)
	{
		if (player == null)
		{
			return;
		}
		base.ClientRPCPlayer<int>(null, player, "Client_ForceSpeechNode", speechNodeIndex);
	}

	// Token: 0x06000ED2 RID: 3794 RVA: 0x0007BED3 File Offset: 0x0007A0D3
	public virtual void OnConversationEnded(BasePlayer player)
	{
		if (this.conversingPlayers.Contains(player))
		{
			this.conversingPlayers.Remove(player);
		}
	}

	// Token: 0x06000ED3 RID: 3795 RVA: 0x0007BEF0 File Offset: 0x0007A0F0
	public void CleanupConversingPlayers()
	{
		for (int i = this.conversingPlayers.Count - 1; i >= 0; i--)
		{
			BasePlayer basePlayer = this.conversingPlayers[i];
			if (basePlayer == null || !basePlayer.IsAlive() || basePlayer.IsSleeping())
			{
				this.conversingPlayers.RemoveAt(i);
			}
		}
	}

	// Token: 0x06000ED4 RID: 3796 RVA: 0x0007BF48 File Offset: 0x0007A148
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	[BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	public void Server_BeginTalking(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		this.CleanupConversingPlayers();
		ConversationData conversationFor = this.GetConversationFor(player);
		if (conversationFor != null)
		{
			if (this.conversingPlayers.Contains(player))
			{
				this.OnConversationEnded(player);
			}
			this.conversingPlayers.Add(player);
			this.UpdateFlags();
			this.OnConversationStarted(player);
			base.ClientRPCPlayer<int, string>(null, player, "Client_StartConversation", this.GetConversationIndex(conversationFor.shortname), this.GetConversationStartSpeech(player));
		}
	}

	// Token: 0x06000ED5 RID: 3797 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnConversationStarted(BasePlayer speakingTo)
	{
	}

	// Token: 0x06000ED6 RID: 3798 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void UpdateFlags()
	{
	}

	// Token: 0x06000ED7 RID: 3799 RVA: 0x0007BFC1 File Offset: 0x0007A1C1
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	[BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	public void Server_EndTalking(BaseEntity.RPCMessage msg)
	{
		this.OnConversationEnded(msg.player);
	}

	// Token: 0x06000ED8 RID: 3800 RVA: 0x0007BFD0 File Offset: 0x0007A1D0
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	[BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void ConversationAction(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		string action = msg.read.String(256);
		this.OnConversationAction(player, action);
	}

	// Token: 0x06000ED9 RID: 3801 RVA: 0x0007BFFD File Offset: 0x0007A1FD
	public bool ValidConversationPlayer(BasePlayer player)
	{
		return Vector3.Distance(player.transform.position, base.transform.position) <= this.maxConversationDistance && !this.conversingPlayers.Contains(player);
	}

	// Token: 0x06000EDA RID: 3802 RVA: 0x0007C038 File Offset: 0x0007A238
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	[BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void Server_ResponsePressed(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		int num = msg.read.Int32();
		int num2 = msg.read.Int32();
		ConversationData conversationFor = this.GetConversationFor(player);
		if (conversationFor == null)
		{
			return;
		}
		ConversationData.ResponseNode responseNode = conversationFor.speeches[num].responses[num2];
		if (responseNode != null)
		{
			if (responseNode.conditions.Length != 0)
			{
				this.UpdateFlags();
			}
			bool flag = responseNode.PassesConditions(player, this);
			if (flag && !string.IsNullOrEmpty(responseNode.actionString))
			{
				this.OnConversationAction(player, responseNode.actionString);
			}
			int speechNodeIndex = conversationFor.GetSpeechNodeIndex(flag ? responseNode.resultingSpeechNode : responseNode.GetFailedSpeechNode(player, this));
			if (speechNodeIndex == -1)
			{
				this.ForceEndConversation(player);
				return;
			}
			this.ForceSpeechNode(player, speechNodeIndex);
		}
	}

	// Token: 0x06000EDB RID: 3803 RVA: 0x0007C0FA File Offset: 0x0007A2FA
	public BasePlayer GetActionPlayer()
	{
		return this.lastActionPlayer;
	}

	// Token: 0x06000EDC RID: 3804 RVA: 0x0007C104 File Offset: 0x0007A304
	public virtual void OnConversationAction(BasePlayer player, string action)
	{
		if (action == "openvending")
		{
			InvisibleVendingMachine vendingMachine = base.GetVendingMachine();
			if (vendingMachine != null && Vector3.Distance(player.transform.position, base.transform.position) < 5f)
			{
				this.ForceEndConversation(player);
				vendingMachine.PlayerOpenLoot(player, "vendingmachine.customer", false);
				return;
			}
		}
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition("scrap");
		NPCTalking.NPCConversationResultAction[] array = this.conversationResultActions;
		int i = 0;
		while (i < array.Length)
		{
			NPCTalking.NPCConversationResultAction npcconversationResultAction = array[i];
			if (npcconversationResultAction.action == action)
			{
				this.CleanupConversingPlayers();
				foreach (BasePlayer basePlayer in this.conversingPlayers)
				{
					if (!(basePlayer == player) && !(basePlayer == null))
					{
						int speechNodeIndex = this.GetConversationFor(player).GetSpeechNodeIndex("startbusy");
						this.ForceSpeechNode(basePlayer, speechNodeIndex);
					}
				}
				int num = npcconversationResultAction.scrapCost;
				List<Item> list = player.inventory.FindItemIDs(itemDefinition.itemid);
				foreach (Item item in list)
				{
					num -= item.amount;
				}
				if (num > 0)
				{
					int speechNodeIndex2 = this.GetConversationFor(player).GetSpeechNodeIndex("toopoor");
					this.ForceSpeechNode(player, speechNodeIndex2);
					return;
				}
				num = npcconversationResultAction.scrapCost;
				foreach (Item item2 in list)
				{
					int num2 = Mathf.Min(num, item2.amount);
					item2.UseItem(num2);
					num -= num2;
					if (num <= 0)
					{
						break;
					}
				}
				this.lastActionPlayer = player;
				base.BroadcastEntityMessage(npcconversationResultAction.broadcastMessage, npcconversationResultAction.broadcastRange, 1218652417);
				this.lastActionPlayer = null;
				return;
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x040009B9 RID: 2489
	public ConversationData[] conversations;

	// Token: 0x040009BA RID: 2490
	public NPCTalking.NPCConversationResultAction[] conversationResultActions;

	// Token: 0x040009BB RID: 2491
	[NonSerialized]
	private float maxConversationDistance = 5f;

	// Token: 0x040009BC RID: 2492
	private List<BasePlayer> conversingPlayers = new List<BasePlayer>();

	// Token: 0x040009BD RID: 2493
	private BasePlayer lastActionPlayer;

	// Token: 0x02000BA2 RID: 2978
	[Serializable]
	public class NPCConversationResultAction
	{
		// Token: 0x04003EFD RID: 16125
		public string action;

		// Token: 0x04003EFE RID: 16126
		public int scrapCost;

		// Token: 0x04003EFF RID: 16127
		public string broadcastMessage;

		// Token: 0x04003F00 RID: 16128
		public float broadcastRange;
	}
}
