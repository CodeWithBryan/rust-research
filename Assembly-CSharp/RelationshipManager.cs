using System;
using System.Collections.Generic;
using CompanionServer;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000B6 RID: 182
public class RelationshipManager : global::BaseEntity
{
	// Token: 0x06001031 RID: 4145 RVA: 0x00084D54 File Offset: 0x00082F54
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("RelationshipManager.OnRpcMessage", 0))
		{
			if (rpc == 1684577101U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SERVER_ChangeRelationship ");
				}
				using (TimeWarning.New("SERVER_ChangeRelationship", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(1684577101U, "SERVER_ChangeRelationship", this, player, 2UL))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SERVER_ChangeRelationship(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SERVER_ChangeRelationship");
					}
				}
				return true;
			}
			if (rpc == 1239936737U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SERVER_ReceiveMugshot ");
				}
				using (TimeWarning.New("SERVER_ReceiveMugshot", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(1239936737U, "SERVER_ReceiveMugshot", this, player, 10UL))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SERVER_ReceiveMugshot(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in SERVER_ReceiveMugshot");
					}
				}
				return true;
			}
			if (rpc == 2178173141U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SERVER_SendFreshContacts ");
				}
				using (TimeWarning.New("SERVER_SendFreshContacts", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(2178173141U, "SERVER_SendFreshContacts", this, player, 1UL))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg4 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SERVER_SendFreshContacts(msg4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in SERVER_SendFreshContacts");
					}
				}
				return true;
			}
			if (rpc == 290196604U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SERVER_UpdatePlayerNote ");
				}
				using (TimeWarning.New("SERVER_UpdatePlayerNote", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(290196604U, "SERVER_UpdatePlayerNote", this, player, 10UL))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg5 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SERVER_UpdatePlayerNote(msg5);
						}
					}
					catch (Exception exception4)
					{
						Debug.LogException(exception4);
						player.Kick("RPC Error in SERVER_UpdatePlayerNote");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001032 RID: 4146 RVA: 0x00085300 File Offset: 0x00083500
	public override void ServerInit()
	{
		base.ServerInit();
		if (global::RelationshipManager.contacts)
		{
			base.InvokeRepeating(new Action(this.UpdateContactsTick), 0f, 1f);
			base.InvokeRepeating(new Action(this.UpdateReputations), 0f, 0.05f);
			base.InvokeRepeating(new Action(this.SendRelationships), 0f, 5f);
		}
	}

	// Token: 0x06001033 RID: 4147 RVA: 0x00085370 File Offset: 0x00083570
	public void UpdateReputations()
	{
		if (!global::RelationshipManager.contacts)
		{
			return;
		}
		if (global::BasePlayer.activePlayerList.Count == 0)
		{
			return;
		}
		if (this.lastReputationUpdateIndex >= global::BasePlayer.activePlayerList.Count)
		{
			this.lastReputationUpdateIndex = 0;
		}
		global::BasePlayer basePlayer = global::BasePlayer.activePlayerList[this.lastReputationUpdateIndex];
		int reputation = basePlayer.reputation;
		int reputationFor = this.GetReputationFor(basePlayer.userID);
		basePlayer.reputation = reputationFor;
		if (reputation != reputationFor)
		{
			basePlayer.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
		this.lastReputationUpdateIndex++;
	}

	// Token: 0x06001034 RID: 4148 RVA: 0x000853F0 File Offset: 0x000835F0
	public void UpdateContactsTick()
	{
		if (!global::RelationshipManager.contacts)
		{
			return;
		}
		foreach (global::BasePlayer player in global::BasePlayer.activePlayerList)
		{
			this.UpdateAcquaintancesFor(player, 1f);
		}
	}

	// Token: 0x06001035 RID: 4149 RVA: 0x00085450 File Offset: 0x00083650
	public int GetReputationFor(ulong playerID)
	{
		int num = this.startingReputation;
		foreach (global::RelationshipManager.PlayerRelationships playerRelationships in this.relationships.Values)
		{
			foreach (KeyValuePair<ulong, global::RelationshipManager.PlayerRelationshipInfo> keyValuePair in playerRelationships.relations)
			{
				if (keyValuePair.Key == playerID)
				{
					if (keyValuePair.Value.type == global::RelationshipManager.RelationshipType.Friend)
					{
						num++;
					}
					else if (keyValuePair.Value.type == global::RelationshipManager.RelationshipType.Acquaintance)
					{
						if (keyValuePair.Value.weight > 60)
						{
							num++;
						}
					}
					else if (keyValuePair.Value.type == global::RelationshipManager.RelationshipType.Enemy)
					{
						num--;
					}
				}
			}
		}
		return num;
	}

	// Token: 0x06001036 RID: 4150 RVA: 0x00085540 File Offset: 0x00083740
	[ServerVar]
	public static void wipecontacts(ConsoleSystem.Arg arg)
	{
		global::BasePlayer basePlayer = arg.Player();
		if (basePlayer == null)
		{
			return;
		}
		if (global::RelationshipManager.ServerInstance == null)
		{
			return;
		}
		ulong userID = basePlayer.userID;
		if (global::RelationshipManager.ServerInstance.relationships.ContainsKey(userID))
		{
			Debug.Log("Wiped contacts for :" + userID);
			global::RelationshipManager.ServerInstance.relationships.Remove(userID);
			global::RelationshipManager.ServerInstance.MarkRelationshipsDirtyFor(userID);
			return;
		}
		Debug.Log("No contacts for :" + userID);
	}

	// Token: 0x06001037 RID: 4151 RVA: 0x000855CC File Offset: 0x000837CC
	[ServerVar]
	public static void wipe_all_contacts(ConsoleSystem.Arg arg)
	{
		global::BasePlayer basePlayer = arg.Player();
		if (basePlayer == null)
		{
			return;
		}
		if (global::RelationshipManager.ServerInstance == null)
		{
			return;
		}
		if (!arg.HasArgs(1) || arg.Args[0] != "confirm")
		{
			Debug.Log("Please append the word 'confirm' at the end of the console command to execute");
			return;
		}
		ulong userID = basePlayer.userID;
		global::RelationshipManager.ServerInstance.relationships.Clear();
		foreach (global::BasePlayer player in global::BasePlayer.activePlayerList)
		{
			global::RelationshipManager.ServerInstance.MarkRelationshipsDirtyFor(player);
		}
		Debug.Log("Wiped all contacts.");
	}

	// Token: 0x06001038 RID: 4152 RVA: 0x0008568C File Offset: 0x0008388C
	public float GetAcquaintanceMaxDist()
	{
		return global::RelationshipManager.seendistance;
	}

	// Token: 0x06001039 RID: 4153 RVA: 0x00085694 File Offset: 0x00083894
	public void UpdateAcquaintancesFor(global::BasePlayer player, float deltaSeconds)
	{
		global::RelationshipManager.PlayerRelationships playerRelationships = this.GetRelationships(player.userID);
		List<global::BasePlayer> list = Facepunch.Pool.GetList<global::BasePlayer>();
		global::BaseNetworkable.GetCloseConnections(player.transform.position, this.GetAcquaintanceMaxDist(), list);
		foreach (global::BasePlayer basePlayer in list)
		{
			if (!(basePlayer == player) && !basePlayer.isClient && basePlayer.IsAlive() && !basePlayer.IsSleeping())
			{
				global::RelationshipManager.PlayerRelationshipInfo relations = playerRelationships.GetRelations(basePlayer.userID);
				if (Vector3.Distance(player.transform.position, basePlayer.transform.position) <= this.GetAcquaintanceMaxDist())
				{
					relations.lastSeenTime = UnityEngine.Time.realtimeSinceStartup;
					if ((relations.type == global::RelationshipManager.RelationshipType.NONE || relations.type == global::RelationshipManager.RelationshipType.Acquaintance) && player.IsPlayerVisibleToUs(basePlayer, 1218519041))
					{
						int num = Mathf.CeilToInt(deltaSeconds);
						if (player.InSafeZone() || basePlayer.InSafeZone())
						{
							num = 0;
						}
						if (relations.type != global::RelationshipManager.RelationshipType.Acquaintance || (relations.weight < 60 && num > 0))
						{
							this.SetRelationship(player, basePlayer, global::RelationshipManager.RelationshipType.Acquaintance, num, false);
						}
					}
				}
			}
		}
		Facepunch.Pool.FreeList<global::BasePlayer>(ref list);
	}

	// Token: 0x0600103A RID: 4154 RVA: 0x000857E0 File Offset: 0x000839E0
	public void SetSeen(global::BasePlayer player, global::BasePlayer otherPlayer)
	{
		ulong userID = player.userID;
		ulong userID2 = otherPlayer.userID;
		global::RelationshipManager.PlayerRelationshipInfo relations = this.GetRelationships(userID).GetRelations(userID2);
		if (relations.type != global::RelationshipManager.RelationshipType.NONE)
		{
			relations.lastSeenTime = UnityEngine.Time.realtimeSinceStartup;
		}
	}

	// Token: 0x0600103B RID: 4155 RVA: 0x0008581C File Offset: 0x00083A1C
	public bool CleanupOldContacts(global::RelationshipManager.PlayerRelationships ownerRelationships, ulong playerID, global::RelationshipManager.RelationshipType relationshipType = global::RelationshipManager.RelationshipType.Acquaintance)
	{
		int numberRelationships = this.GetNumberRelationships(playerID);
		if (numberRelationships < global::RelationshipManager.maxplayerrelationships)
		{
			return true;
		}
		List<ulong> list = Facepunch.Pool.GetList<ulong>();
		foreach (KeyValuePair<ulong, global::RelationshipManager.PlayerRelationshipInfo> keyValuePair in ownerRelationships.relations)
		{
			if (keyValuePair.Value.type == relationshipType && UnityEngine.Time.realtimeSinceStartup - keyValuePair.Value.lastSeenTime > (float)global::RelationshipManager.forgetafterminutes * 60f)
			{
				list.Add(keyValuePair.Key);
			}
		}
		int count = list.Count;
		foreach (ulong player in list)
		{
			ownerRelationships.Forget(player);
		}
		Facepunch.Pool.FreeList<ulong>(ref list);
		return numberRelationships - count < global::RelationshipManager.maxplayerrelationships;
	}

	// Token: 0x0600103C RID: 4156 RVA: 0x00085918 File Offset: 0x00083B18
	public void ForceRelationshipByID(global::BasePlayer player, ulong otherPlayerID, global::RelationshipManager.RelationshipType newType, int weight, bool sendImmediate = false)
	{
		if (!global::RelationshipManager.contacts)
		{
			return;
		}
		if (player == null)
		{
			return;
		}
		if (player.userID == otherPlayerID)
		{
			return;
		}
		if (player.IsNpc)
		{
			return;
		}
		ulong userID = player.userID;
		if (!this.HasRelations(userID, otherPlayerID))
		{
			return;
		}
		global::RelationshipManager.PlayerRelationshipInfo relations = this.GetRelationships(userID).GetRelations(otherPlayerID);
		if (relations.type != newType)
		{
			relations.weight = 0;
		}
		relations.type = newType;
		relations.weight += weight;
		if (sendImmediate)
		{
			this.SendRelationshipsFor(player);
			return;
		}
		this.MarkRelationshipsDirtyFor(player);
	}

	// Token: 0x0600103D RID: 4157 RVA: 0x000859A4 File Offset: 0x00083BA4
	public void SetRelationship(global::BasePlayer player, global::BasePlayer otherPlayer, global::RelationshipManager.RelationshipType type, int weight = 1, bool sendImmediate = false)
	{
		if (!global::RelationshipManager.contacts)
		{
			return;
		}
		ulong userID = player.userID;
		ulong userID2 = otherPlayer.userID;
		if (player == null)
		{
			return;
		}
		if (player == otherPlayer)
		{
			return;
		}
		if (player.IsNpc)
		{
			return;
		}
		if (otherPlayer != null && otherPlayer.IsNpc)
		{
			return;
		}
		global::RelationshipManager.PlayerRelationships playerRelationships = this.GetRelationships(userID);
		if (!this.CleanupOldContacts(playerRelationships, userID, global::RelationshipManager.RelationshipType.Acquaintance))
		{
			this.CleanupOldContacts(playerRelationships, userID, global::RelationshipManager.RelationshipType.Enemy);
		}
		global::RelationshipManager.PlayerRelationshipInfo relations = playerRelationships.GetRelations(userID2);
		bool flag = false;
		if (relations.type != type)
		{
			flag = true;
			relations.weight = 0;
		}
		relations.type = type;
		relations.weight += weight;
		float num = UnityEngine.Time.realtimeSinceStartup - relations.lastMugshotTime;
		if (flag || relations.mugshotCrc == 0U || num >= global::RelationshipManager.mugshotUpdateInterval)
		{
			bool flag2 = otherPlayer.IsAlive();
			bool flag3 = player.SecondsSinceAttacked > 10f && !player.IsAiming;
			float num2 = 100f;
			if (flag3)
			{
				Vector3 normalized = (otherPlayer.eyes.position - player.eyes.position).normalized;
				bool flag4 = Vector3.Dot(player.eyes.HeadForward(), normalized) >= 0.6f;
				float num3 = Vector3Ex.Distance2D(player.transform.position, otherPlayer.transform.position);
				if (flag2 && num3 < num2 && flag4)
				{
					base.ClientRPCPlayer<ulong>(null, player, "CLIENT_DoMugshot", userID2);
					relations.lastMugshotTime = UnityEngine.Time.realtimeSinceStartup;
				}
			}
		}
		if (sendImmediate)
		{
			this.SendRelationshipsFor(player);
			return;
		}
		this.MarkRelationshipsDirtyFor(player);
	}

	// Token: 0x0600103E RID: 4158 RVA: 0x00085B44 File Offset: 0x00083D44
	public ProtoBuf.RelationshipManager.PlayerRelationships GetRelationshipSaveByID(ulong playerID)
	{
		ProtoBuf.RelationshipManager.PlayerRelationships playerRelationships = Facepunch.Pool.Get<ProtoBuf.RelationshipManager.PlayerRelationships>();
		global::RelationshipManager.PlayerRelationships playerRelationships2 = this.GetRelationships(playerID);
		if (playerRelationships2 != null)
		{
			playerRelationships.playerID = playerID;
			playerRelationships.relations = Facepunch.Pool.GetList<ProtoBuf.RelationshipManager.PlayerRelationshipInfo>();
			foreach (KeyValuePair<ulong, global::RelationshipManager.PlayerRelationshipInfo> keyValuePair in playerRelationships2.relations)
			{
				ProtoBuf.RelationshipManager.PlayerRelationshipInfo playerRelationshipInfo = Facepunch.Pool.Get<ProtoBuf.RelationshipManager.PlayerRelationshipInfo>();
				playerRelationshipInfo.playerID = keyValuePair.Value.player;
				playerRelationshipInfo.type = (int)keyValuePair.Value.type;
				playerRelationshipInfo.weight = keyValuePair.Value.weight;
				playerRelationshipInfo.mugshotCrc = keyValuePair.Value.mugshotCrc;
				playerRelationshipInfo.displayName = keyValuePair.Value.displayName;
				playerRelationshipInfo.notes = keyValuePair.Value.notes;
				playerRelationshipInfo.timeSinceSeen = UnityEngine.Time.realtimeSinceStartup - keyValuePair.Value.lastSeenTime;
				playerRelationships.relations.Add(playerRelationshipInfo);
			}
			return playerRelationships;
		}
		return null;
	}

	// Token: 0x0600103F RID: 4159 RVA: 0x00085C60 File Offset: 0x00083E60
	public void MarkRelationshipsDirtyFor(ulong playerID)
	{
		global::BasePlayer basePlayer = global::RelationshipManager.FindByID(playerID);
		if (basePlayer)
		{
			this.MarkRelationshipsDirtyFor(basePlayer);
		}
	}

	// Token: 0x06001040 RID: 4160 RVA: 0x00085C83 File Offset: 0x00083E83
	public static void ForceSendRelationships(global::BasePlayer player)
	{
		if (global::RelationshipManager.ServerInstance)
		{
			global::RelationshipManager.ServerInstance.MarkRelationshipsDirtyFor(player);
		}
	}

	// Token: 0x06001041 RID: 4161 RVA: 0x00085C9C File Offset: 0x00083E9C
	public void MarkRelationshipsDirtyFor(global::BasePlayer player)
	{
		if (player == null)
		{
			return;
		}
		if (!global::RelationshipManager._dirtyRelationshipPlayers.Contains(player))
		{
			global::RelationshipManager._dirtyRelationshipPlayers.Add(player);
		}
		ulong userID = player.userID;
	}

	// Token: 0x06001042 RID: 4162 RVA: 0x00085CC8 File Offset: 0x00083EC8
	public void SendRelationshipsFor(global::BasePlayer player)
	{
		if (!global::RelationshipManager.contacts)
		{
			return;
		}
		ulong userID = player.userID;
		ProtoBuf.RelationshipManager.PlayerRelationships relationshipSaveByID = this.GetRelationshipSaveByID(userID);
		base.ClientRPCPlayer<ProtoBuf.RelationshipManager.PlayerRelationships>(null, player, "CLIENT_RecieveLocalRelationships", relationshipSaveByID);
	}

	// Token: 0x06001043 RID: 4163 RVA: 0x00085CFC File Offset: 0x00083EFC
	public void SendRelationships()
	{
		if (!global::RelationshipManager.contacts)
		{
			return;
		}
		foreach (global::BasePlayer basePlayer in global::RelationshipManager._dirtyRelationshipPlayers)
		{
			if (!(basePlayer == null) && basePlayer.IsConnected && !basePlayer.IsSleeping())
			{
				this.SendRelationshipsFor(basePlayer);
			}
		}
		global::RelationshipManager._dirtyRelationshipPlayers.Clear();
	}

	// Token: 0x06001044 RID: 4164 RVA: 0x00085D7C File Offset: 0x00083F7C
	public int GetNumberRelationships(ulong player)
	{
		global::RelationshipManager.PlayerRelationships playerRelationships;
		if (this.relationships.TryGetValue(player, out playerRelationships))
		{
			return playerRelationships.relations.Count;
		}
		return 0;
	}

	// Token: 0x06001045 RID: 4165 RVA: 0x00085DA8 File Offset: 0x00083FA8
	public bool HasRelations(ulong player, ulong otherPlayer)
	{
		global::RelationshipManager.PlayerRelationships playerRelationships;
		return this.relationships.TryGetValue(player, out playerRelationships) && playerRelationships.relations.ContainsKey(otherPlayer);
	}

	// Token: 0x06001046 RID: 4166 RVA: 0x00085DD8 File Offset: 0x00083FD8
	public global::RelationshipManager.PlayerRelationships GetRelationships(ulong player)
	{
		global::RelationshipManager.PlayerRelationships result;
		if (this.relationships.TryGetValue(player, out result))
		{
			return result;
		}
		global::RelationshipManager.PlayerRelationships playerRelationships = Facepunch.Pool.Get<global::RelationshipManager.PlayerRelationships>();
		playerRelationships.ownerPlayer = player;
		this.relationships.Add(player, playerRelationships);
		return playerRelationships;
	}

	// Token: 0x06001047 RID: 4167 RVA: 0x00085E14 File Offset: 0x00084014
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(1UL)]
	public void SERVER_SendFreshContacts(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player)
		{
			this.SendRelationshipsFor(player);
		}
	}

	// Token: 0x06001048 RID: 4168 RVA: 0x00085E38 File Offset: 0x00084038
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(2UL)]
	public void SERVER_ChangeRelationship(global::BaseEntity.RPCMessage msg)
	{
		ulong userID = msg.player.userID;
		ulong num = msg.read.UInt64();
		int num2 = Mathf.Clamp(msg.read.Int32(), 0, 3);
		global::RelationshipManager.PlayerRelationships playerRelationships = this.GetRelationships(userID);
		playerRelationships.GetRelations(num);
		global::BasePlayer player = msg.player;
		global::RelationshipManager.RelationshipType relationshipType = (global::RelationshipManager.RelationshipType)num2;
		if (num2 == 0)
		{
			if (playerRelationships.Forget(num))
			{
				this.SendRelationshipsFor(player);
			}
			return;
		}
		global::BasePlayer basePlayer = global::RelationshipManager.FindByID(num);
		if (basePlayer == null)
		{
			this.ForceRelationshipByID(player, num, relationshipType, 0, true);
			return;
		}
		this.SetRelationship(player, basePlayer, relationshipType, 1, true);
	}

	// Token: 0x06001049 RID: 4169 RVA: 0x00085EC8 File Offset: 0x000840C8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(10UL)]
	public void SERVER_UpdatePlayerNote(global::BaseEntity.RPCMessage msg)
	{
		ulong userID = msg.player.userID;
		ulong player = msg.read.UInt64();
		string notes = msg.read.String(256);
		this.GetRelationships(userID).GetRelations(player).notes = notes;
		this.MarkRelationshipsDirtyFor(userID);
	}

	// Token: 0x0600104A RID: 4170 RVA: 0x00085F18 File Offset: 0x00084118
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(10UL)]
	public void SERVER_ReceiveMugshot(global::BaseEntity.RPCMessage msg)
	{
		ulong userID = msg.player.userID;
		ulong num = msg.read.UInt64();
		uint num2 = msg.read.UInt32();
		byte[] array = msg.read.BytesWithSize(65536U);
		if (array == null || !ImageProcessing.IsValidJPG(array, 256, 512))
		{
			return;
		}
		global::RelationshipManager.PlayerRelationships playerRelationships;
		global::RelationshipManager.PlayerRelationshipInfo playerRelationshipInfo;
		if (!this.relationships.TryGetValue(userID, out playerRelationships) || !playerRelationships.relations.TryGetValue(num, out playerRelationshipInfo))
		{
			return;
		}
		uint steamIdHash = global::RelationshipManager.GetSteamIdHash(userID, num);
		uint num3 = FileStorage.server.Store(array, FileStorage.Type.jpg, this.net.ID, steamIdHash);
		if (num3 != num2)
		{
			Debug.LogWarning("Client/Server FileStorage CRC differs");
		}
		if (num3 != playerRelationshipInfo.mugshotCrc)
		{
			FileStorage.server.RemoveExact(playerRelationshipInfo.mugshotCrc, FileStorage.Type.jpg, this.net.ID, steamIdHash);
		}
		playerRelationshipInfo.mugshotCrc = num3;
		this.MarkRelationshipsDirtyFor(userID);
	}

	// Token: 0x0600104B RID: 4171 RVA: 0x00086000 File Offset: 0x00084200
	private void DeleteMugshot(ulong steamId, ulong targetSteamId, uint crc)
	{
		if (crc == 0U)
		{
			return;
		}
		uint steamIdHash = global::RelationshipManager.GetSteamIdHash(steamId, targetSteamId);
		FileStorage.server.RemoveExact(crc, FileStorage.Type.jpg, this.net.ID, steamIdHash);
	}

	// Token: 0x0600104C RID: 4172 RVA: 0x00086031 File Offset: 0x00084231
	private static uint GetSteamIdHash(ulong requesterSteamId, ulong targetSteamId)
	{
		return (uint)((requesterSteamId & 65535UL) << 16 | (targetSteamId & 65535UL));
	}

	// Token: 0x17000161 RID: 353
	// (get) Token: 0x0600104E RID: 4174 RVA: 0x00086067 File Offset: 0x00084267
	// (set) Token: 0x0600104D RID: 4173 RVA: 0x00086048 File Offset: 0x00084248
	[ServerVar]
	public static int maxTeamSize
	{
		get
		{
			return global::RelationshipManager.maxTeamSize_Internal;
		}
		set
		{
			global::RelationshipManager.maxTeamSize_Internal = value;
			if (global::RelationshipManager.ServerInstance)
			{
				global::RelationshipManager.ServerInstance.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
		}
	}

	// Token: 0x0600104F RID: 4175 RVA: 0x00086070 File Offset: 0x00084270
	public int GetMaxTeamSize()
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		if (activeGameMode)
		{
			return activeGameMode.GetMaxRelationshipTeamSize();
		}
		return global::RelationshipManager.maxTeamSize;
	}

	// Token: 0x17000162 RID: 354
	// (get) Token: 0x06001050 RID: 4176 RVA: 0x00086098 File Offset: 0x00084298
	// (set) Token: 0x06001051 RID: 4177 RVA: 0x0008609F File Offset: 0x0008429F
	public static global::RelationshipManager ServerInstance { get; private set; }

	// Token: 0x06001052 RID: 4178 RVA: 0x000860A7 File Offset: 0x000842A7
	public void OnEnable()
	{
		if (base.isServer)
		{
			if (global::RelationshipManager.ServerInstance != null)
			{
				Debug.LogError("Major fuckup! RelationshipManager spawned twice, Contact Developers!");
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			global::RelationshipManager.ServerInstance = this;
		}
	}

	// Token: 0x06001053 RID: 4179 RVA: 0x000860DA File Offset: 0x000842DA
	public void OnDestroy()
	{
		if (base.isServer)
		{
			global::RelationshipManager.ServerInstance = null;
		}
	}

	// Token: 0x06001054 RID: 4180 RVA: 0x000860EC File Offset: 0x000842EC
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.relationshipManager = Facepunch.Pool.Get<ProtoBuf.RelationshipManager>();
		info.msg.relationshipManager.maxTeamSize = global::RelationshipManager.maxTeamSize;
		if (info.forDisk)
		{
			info.msg.relationshipManager.lastTeamIndex = this.lastTeamIndex;
			info.msg.relationshipManager.teamList = Facepunch.Pool.GetList<ProtoBuf.PlayerTeam>();
			foreach (KeyValuePair<ulong, global::RelationshipManager.PlayerTeam> keyValuePair in this.teams)
			{
				global::RelationshipManager.PlayerTeam value = keyValuePair.Value;
				if (value != null)
				{
					ProtoBuf.PlayerTeam playerTeam = Facepunch.Pool.Get<ProtoBuf.PlayerTeam>();
					playerTeam.teamLeader = value.teamLeader;
					playerTeam.teamID = value.teamID;
					playerTeam.teamName = value.teamName;
					playerTeam.members = Facepunch.Pool.GetList<ProtoBuf.PlayerTeam.TeamMember>();
					foreach (ulong num in value.members)
					{
						ProtoBuf.PlayerTeam.TeamMember teamMember = Facepunch.Pool.Get<ProtoBuf.PlayerTeam.TeamMember>();
						global::BasePlayer basePlayer = global::RelationshipManager.FindByID(num);
						teamMember.displayName = ((basePlayer != null) ? basePlayer.displayName : (SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerName(num) ?? "DEAD"));
						teamMember.userID = num;
						playerTeam.members.Add(teamMember);
					}
					info.msg.relationshipManager.teamList.Add(playerTeam);
				}
			}
			info.msg.relationshipManager.relationships = Facepunch.Pool.GetList<ProtoBuf.RelationshipManager.PlayerRelationships>();
			foreach (ulong num2 in this.relationships.Keys)
			{
				global::RelationshipManager.PlayerRelationships playerRelationships = this.relationships[num2];
				ProtoBuf.RelationshipManager.PlayerRelationships relationshipSaveByID = this.GetRelationshipSaveByID(num2);
				info.msg.relationshipManager.relationships.Add(relationshipSaveByID);
			}
		}
	}

	// Token: 0x06001055 RID: 4181 RVA: 0x00086318 File Offset: 0x00084518
	public void DisbandTeam(global::RelationshipManager.PlayerTeam teamToDisband)
	{
		this.teams.Remove(teamToDisband.teamID);
		Facepunch.Pool.Free<global::RelationshipManager.PlayerTeam>(ref teamToDisband);
	}

	// Token: 0x06001056 RID: 4182 RVA: 0x00086334 File Offset: 0x00084534
	public static global::BasePlayer FindByID(ulong userID)
	{
		global::BasePlayer basePlayer = null;
		if (global::RelationshipManager.ServerInstance.cachedPlayers.TryGetValue(userID, out basePlayer))
		{
			if (basePlayer != null)
			{
				return basePlayer;
			}
			global::RelationshipManager.ServerInstance.cachedPlayers.Remove(userID);
		}
		global::BasePlayer basePlayer2 = global::BasePlayer.FindByID(userID);
		if (!basePlayer2)
		{
			basePlayer2 = global::BasePlayer.FindSleeping(userID);
		}
		if (basePlayer2 != null)
		{
			global::RelationshipManager.ServerInstance.cachedPlayers.Add(userID, basePlayer2);
		}
		return basePlayer2;
	}

	// Token: 0x06001057 RID: 4183 RVA: 0x000863A4 File Offset: 0x000845A4
	public global::RelationshipManager.PlayerTeam FindTeam(ulong TeamID)
	{
		if (this.teams.ContainsKey(TeamID))
		{
			return this.teams[TeamID];
		}
		return null;
	}

	// Token: 0x06001058 RID: 4184 RVA: 0x000863C4 File Offset: 0x000845C4
	public global::RelationshipManager.PlayerTeam FindPlayersTeam(ulong userID)
	{
		global::RelationshipManager.PlayerTeam result;
		if (this.playerToTeam.TryGetValue(userID, out result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x06001059 RID: 4185 RVA: 0x000863E4 File Offset: 0x000845E4
	public global::RelationshipManager.PlayerTeam CreateTeam()
	{
		global::RelationshipManager.PlayerTeam playerTeam = Facepunch.Pool.Get<global::RelationshipManager.PlayerTeam>();
		playerTeam.teamID = this.lastTeamIndex;
		playerTeam.teamStartTime = UnityEngine.Time.realtimeSinceStartup;
		this.teams.Add(this.lastTeamIndex, playerTeam);
		this.lastTeamIndex += 1UL;
		return playerTeam;
	}

	// Token: 0x0600105A RID: 4186 RVA: 0x00086430 File Offset: 0x00084630
	[ServerUserVar]
	public static void trycreateteam(ConsoleSystem.Arg arg)
	{
		if (global::RelationshipManager.maxTeamSize == 0)
		{
			arg.ReplyWith("Teams are disabled on this server");
			return;
		}
		global::BasePlayer basePlayer = arg.Player();
		if (basePlayer.currentTeam != 0UL)
		{
			return;
		}
		global::RelationshipManager.PlayerTeam playerTeam = global::RelationshipManager.ServerInstance.CreateTeam();
		playerTeam.teamLeader = basePlayer.userID;
		playerTeam.AddPlayer(basePlayer);
	}

	// Token: 0x0600105B RID: 4187 RVA: 0x00086480 File Offset: 0x00084680
	[ServerUserVar]
	public static void promote(ConsoleSystem.Arg arg)
	{
		global::BasePlayer basePlayer = arg.Player();
		if (basePlayer.currentTeam == 0UL)
		{
			return;
		}
		global::BasePlayer lookingAtPlayer = global::RelationshipManager.GetLookingAtPlayer(basePlayer);
		if (lookingAtPlayer == null)
		{
			return;
		}
		if (lookingAtPlayer.IsDead())
		{
			return;
		}
		if (lookingAtPlayer == basePlayer)
		{
			return;
		}
		if (lookingAtPlayer.currentTeam == basePlayer.currentTeam)
		{
			global::RelationshipManager.PlayerTeam playerTeam = global::RelationshipManager.ServerInstance.teams[basePlayer.currentTeam];
			if (playerTeam != null && playerTeam.teamLeader == basePlayer.userID)
			{
				playerTeam.SetTeamLeader(lookingAtPlayer.userID);
			}
		}
	}

	// Token: 0x0600105C RID: 4188 RVA: 0x00086504 File Offset: 0x00084704
	[ServerUserVar]
	public static void leaveteam(ConsoleSystem.Arg arg)
	{
		global::BasePlayer basePlayer = arg.Player();
		if (basePlayer == null)
		{
			return;
		}
		if (basePlayer.currentTeam == 0UL)
		{
			return;
		}
		global::RelationshipManager.PlayerTeam playerTeam = global::RelationshipManager.ServerInstance.FindTeam(basePlayer.currentTeam);
		if (playerTeam != null)
		{
			playerTeam.RemovePlayer(basePlayer.userID);
			basePlayer.ClearTeam();
		}
	}

	// Token: 0x0600105D RID: 4189 RVA: 0x00086554 File Offset: 0x00084754
	[ServerUserVar]
	public static void acceptinvite(ConsoleSystem.Arg arg)
	{
		global::BasePlayer basePlayer = arg.Player();
		if (basePlayer == null)
		{
			return;
		}
		if (basePlayer.currentTeam != 0UL)
		{
			return;
		}
		ulong @ulong = arg.GetULong(0, 0UL);
		global::RelationshipManager.PlayerTeam playerTeam = global::RelationshipManager.ServerInstance.FindTeam(@ulong);
		if (playerTeam == null)
		{
			basePlayer.ClearPendingInvite();
			return;
		}
		playerTeam.AcceptInvite(basePlayer);
	}

	// Token: 0x0600105E RID: 4190 RVA: 0x000865A4 File Offset: 0x000847A4
	[ServerUserVar]
	public static void rejectinvite(ConsoleSystem.Arg arg)
	{
		global::BasePlayer basePlayer = arg.Player();
		if (basePlayer == null)
		{
			return;
		}
		if (basePlayer.currentTeam != 0UL)
		{
			return;
		}
		ulong @ulong = arg.GetULong(0, 0UL);
		global::RelationshipManager.PlayerTeam playerTeam = global::RelationshipManager.ServerInstance.FindTeam(@ulong);
		if (playerTeam == null)
		{
			basePlayer.ClearPendingInvite();
			return;
		}
		playerTeam.RejectInvite(basePlayer);
	}

	// Token: 0x0600105F RID: 4191 RVA: 0x000865F4 File Offset: 0x000847F4
	public static global::BasePlayer GetLookingAtPlayer(global::BasePlayer source)
	{
		RaycastHit hit;
		if (UnityEngine.Physics.Raycast(source.eyes.position, source.eyes.HeadForward(), out hit, 5f, 1218652417, QueryTriggerInteraction.Ignore))
		{
			global::BaseEntity entity = hit.GetEntity();
			if (entity)
			{
				return entity.GetComponent<global::BasePlayer>();
			}
		}
		return null;
	}

	// Token: 0x06001060 RID: 4192 RVA: 0x00086644 File Offset: 0x00084844
	[ServerVar]
	public static void sleeptoggle(ConsoleSystem.Arg arg)
	{
		global::BasePlayer basePlayer = arg.Player();
		if (basePlayer == null)
		{
			return;
		}
		RaycastHit hit;
		if (UnityEngine.Physics.Raycast(basePlayer.eyes.position, basePlayer.eyes.HeadForward(), out hit, 5f, 1218652417, QueryTriggerInteraction.Ignore))
		{
			global::BaseEntity entity = hit.GetEntity();
			if (entity)
			{
				global::BasePlayer component = entity.GetComponent<global::BasePlayer>();
				if (component && component != basePlayer && !component.IsNpc)
				{
					if (component.IsSleeping())
					{
						component.EndSleeping();
						return;
					}
					component.StartSleeping();
				}
			}
		}
	}

	// Token: 0x06001061 RID: 4193 RVA: 0x000866D0 File Offset: 0x000848D0
	[ServerUserVar]
	public static void kickmember(ConsoleSystem.Arg arg)
	{
		global::BasePlayer basePlayer = arg.Player();
		if (basePlayer == null)
		{
			return;
		}
		global::RelationshipManager.PlayerTeam playerTeam = global::RelationshipManager.ServerInstance.FindTeam(basePlayer.currentTeam);
		if (playerTeam == null)
		{
			return;
		}
		if (playerTeam.GetLeader() != basePlayer)
		{
			return;
		}
		ulong @ulong = arg.GetULong(0, 0UL);
		if (basePlayer.userID == @ulong)
		{
			return;
		}
		playerTeam.RemovePlayer(@ulong);
	}

	// Token: 0x06001062 RID: 4194 RVA: 0x00086730 File Offset: 0x00084930
	[ServerUserVar]
	public static void sendinvite(ConsoleSystem.Arg arg)
	{
		global::BasePlayer basePlayer = arg.Player();
		global::RelationshipManager.PlayerTeam playerTeam = global::RelationshipManager.ServerInstance.FindTeam(basePlayer.currentTeam);
		if (playerTeam == null)
		{
			return;
		}
		if (playerTeam.GetLeader() == null)
		{
			return;
		}
		if (playerTeam.GetLeader() != basePlayer)
		{
			return;
		}
		RaycastHit hit;
		if (UnityEngine.Physics.Raycast(basePlayer.eyes.position, basePlayer.eyes.HeadForward(), out hit, 5f, 1218652417, QueryTriggerInteraction.Ignore))
		{
			global::BaseEntity entity = hit.GetEntity();
			if (entity)
			{
				global::BasePlayer component = entity.GetComponent<global::BasePlayer>();
				if (component && component != basePlayer && !component.IsNpc && component.currentTeam == 0UL)
				{
					playerTeam.SendInvite(component);
				}
			}
		}
	}

	// Token: 0x06001063 RID: 4195 RVA: 0x000867E8 File Offset: 0x000849E8
	[ServerVar]
	public static void fakeinvite(ConsoleSystem.Arg arg)
	{
		global::BasePlayer basePlayer = arg.Player();
		ulong @ulong = arg.GetULong(0, 0UL);
		global::RelationshipManager.PlayerTeam playerTeam = global::RelationshipManager.ServerInstance.FindTeam(@ulong);
		if (playerTeam == null)
		{
			return;
		}
		if (basePlayer.currentTeam != 0UL)
		{
			Debug.Log("already in team");
		}
		playerTeam.SendInvite(basePlayer);
		Debug.Log("sent bot invite");
	}

	// Token: 0x06001064 RID: 4196 RVA: 0x0008683C File Offset: 0x00084A3C
	[ServerVar]
	public static void addtoteam(ConsoleSystem.Arg arg)
	{
		global::BasePlayer basePlayer = arg.Player();
		global::RelationshipManager.PlayerTeam playerTeam = global::RelationshipManager.ServerInstance.FindTeam(basePlayer.currentTeam);
		if (playerTeam == null)
		{
			return;
		}
		if (playerTeam.GetLeader() == null)
		{
			return;
		}
		if (playerTeam.GetLeader() != basePlayer)
		{
			return;
		}
		RaycastHit hit;
		if (UnityEngine.Physics.Raycast(basePlayer.eyes.position, basePlayer.eyes.HeadForward(), out hit, 5f, 1218652417, QueryTriggerInteraction.Ignore))
		{
			global::BaseEntity entity = hit.GetEntity();
			if (entity)
			{
				global::BasePlayer component = entity.GetComponent<global::BasePlayer>();
				if (component && component != basePlayer && !component.IsNpc)
				{
					playerTeam.AddPlayer(component);
				}
			}
		}
	}

	// Token: 0x06001065 RID: 4197 RVA: 0x000868E9 File Offset: 0x00084AE9
	public static bool TeamsEnabled()
	{
		return global::RelationshipManager.maxTeamSize > 0;
	}

	// Token: 0x06001066 RID: 4198 RVA: 0x000868F4 File Offset: 0x00084AF4
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk && info.msg.relationshipManager != null)
		{
			this.lastTeamIndex = info.msg.relationshipManager.lastTeamIndex;
			foreach (ProtoBuf.PlayerTeam playerTeam in info.msg.relationshipManager.teamList)
			{
				global::RelationshipManager.PlayerTeam playerTeam2 = Facepunch.Pool.Get<global::RelationshipManager.PlayerTeam>();
				playerTeam2.teamLeader = playerTeam.teamLeader;
				playerTeam2.teamID = playerTeam.teamID;
				playerTeam2.teamName = playerTeam.teamName;
				playerTeam2.members = new List<ulong>();
				foreach (ProtoBuf.PlayerTeam.TeamMember teamMember in playerTeam.members)
				{
					playerTeam2.members.Add(teamMember.userID);
				}
				this.teams[playerTeam2.teamID] = playerTeam2;
			}
			foreach (global::RelationshipManager.PlayerTeam playerTeam3 in this.teams.Values)
			{
				foreach (ulong num in playerTeam3.members)
				{
					this.playerToTeam[num] = playerTeam3;
					global::BasePlayer basePlayer = global::RelationshipManager.FindByID(num);
					if (basePlayer != null && basePlayer.currentTeam != playerTeam3.teamID)
					{
						Debug.LogWarning(string.Format("Player {0} has the wrong teamID: got {1}, expected {2}. Fixing automatically.", num, basePlayer.currentTeam, playerTeam3.teamID));
						basePlayer.currentTeam = playerTeam3.teamID;
					}
				}
			}
			foreach (ProtoBuf.RelationshipManager.PlayerRelationships playerRelationships in info.msg.relationshipManager.relationships)
			{
				ulong playerID = playerRelationships.playerID;
				global::RelationshipManager.PlayerRelationships playerRelationships2 = this.GetRelationships(playerID);
				playerRelationships2.relations.Clear();
				foreach (ProtoBuf.RelationshipManager.PlayerRelationshipInfo playerRelationshipInfo in playerRelationships.relations)
				{
					global::RelationshipManager.PlayerRelationshipInfo playerRelationshipInfo2 = new global::RelationshipManager.PlayerRelationshipInfo();
					playerRelationshipInfo2.type = (global::RelationshipManager.RelationshipType)playerRelationshipInfo.type;
					playerRelationshipInfo2.weight = playerRelationshipInfo.weight;
					playerRelationshipInfo2.displayName = playerRelationshipInfo.displayName;
					playerRelationshipInfo2.mugshotCrc = playerRelationshipInfo.mugshotCrc;
					playerRelationshipInfo2.notes = playerRelationshipInfo.notes;
					playerRelationshipInfo2.player = playerRelationshipInfo.playerID;
					playerRelationshipInfo2.lastSeenTime = UnityEngine.Time.realtimeSinceStartup - playerRelationshipInfo.timeSinceSeen;
					playerRelationships2.relations.Add(playerRelationshipInfo.playerID, playerRelationshipInfo2);
				}
			}
		}
	}

	// Token: 0x04000A3F RID: 2623
	[ReplicatedVar]
	public static bool contacts = true;

	// Token: 0x04000A40 RID: 2624
	private const int MugshotResolution = 256;

	// Token: 0x04000A41 RID: 2625
	private const int MugshotMaxFileSize = 65536;

	// Token: 0x04000A42 RID: 2626
	private const float MugshotMaxDistance = 50f;

	// Token: 0x04000A43 RID: 2627
	public Dictionary<ulong, global::RelationshipManager.PlayerRelationships> relationships = new Dictionary<ulong, global::RelationshipManager.PlayerRelationships>();

	// Token: 0x04000A44 RID: 2628
	private int lastReputationUpdateIndex;

	// Token: 0x04000A45 RID: 2629
	private const int seenReputationSeconds = 60;

	// Token: 0x04000A46 RID: 2630
	private int startingReputation;

	// Token: 0x04000A47 RID: 2631
	[ServerVar]
	public static int forgetafterminutes = 960;

	// Token: 0x04000A48 RID: 2632
	[ServerVar]
	public static int maxplayerrelationships = 128;

	// Token: 0x04000A49 RID: 2633
	[ServerVar]
	public static float seendistance = 10f;

	// Token: 0x04000A4A RID: 2634
	[ServerVar]
	public static float mugshotUpdateInterval = 300f;

	// Token: 0x04000A4B RID: 2635
	private static List<global::BasePlayer> _dirtyRelationshipPlayers = new List<global::BasePlayer>();

	// Token: 0x04000A4C RID: 2636
	public static int maxTeamSize_Internal = 8;

	// Token: 0x04000A4E RID: 2638
	public Dictionary<ulong, global::BasePlayer> cachedPlayers = new Dictionary<ulong, global::BasePlayer>();

	// Token: 0x04000A4F RID: 2639
	public Dictionary<ulong, global::RelationshipManager.PlayerTeam> playerToTeam = new Dictionary<ulong, global::RelationshipManager.PlayerTeam>();

	// Token: 0x04000A50 RID: 2640
	public Dictionary<ulong, global::RelationshipManager.PlayerTeam> teams = new Dictionary<ulong, global::RelationshipManager.PlayerTeam>();

	// Token: 0x04000A51 RID: 2641
	private ulong lastTeamIndex = 1UL;

	// Token: 0x02000BA7 RID: 2983
	public enum RelationshipType
	{
		// Token: 0x04003F09 RID: 16137
		NONE,
		// Token: 0x04003F0A RID: 16138
		Acquaintance,
		// Token: 0x04003F0B RID: 16139
		Friend,
		// Token: 0x04003F0C RID: 16140
		Enemy
	}

	// Token: 0x02000BA8 RID: 2984
	public class PlayerRelationshipInfo : Facepunch.Pool.IPooled, IServerFileReceiver
	{
		// Token: 0x06004B0E RID: 19214 RVA: 0x001913EC File Offset: 0x0018F5EC
		public void EnterPool()
		{
			this.Reset();
		}

		// Token: 0x06004B0F RID: 19215 RVA: 0x001913EC File Offset: 0x0018F5EC
		public void LeavePool()
		{
			this.Reset();
		}

		// Token: 0x06004B10 RID: 19216 RVA: 0x001913F4 File Offset: 0x0018F5F4
		private void Reset()
		{
			this.displayName = null;
			this.player = 0UL;
			this.type = global::RelationshipManager.RelationshipType.NONE;
			this.weight = 0;
			this.mugshotCrc = 0U;
			this.notes = "";
			this.lastMugshotTime = 0f;
		}

		// Token: 0x04003F0D RID: 16141
		public string displayName;

		// Token: 0x04003F0E RID: 16142
		public ulong player;

		// Token: 0x04003F0F RID: 16143
		public global::RelationshipManager.RelationshipType type;

		// Token: 0x04003F10 RID: 16144
		public int weight;

		// Token: 0x04003F11 RID: 16145
		public uint mugshotCrc;

		// Token: 0x04003F12 RID: 16146
		public string notes;

		// Token: 0x04003F13 RID: 16147
		public float lastSeenTime;

		// Token: 0x04003F14 RID: 16148
		public float lastMugshotTime;
	}

	// Token: 0x02000BA9 RID: 2985
	public class PlayerRelationships : Facepunch.Pool.IPooled
	{
		// Token: 0x06004B12 RID: 19218 RVA: 0x00191430 File Offset: 0x0018F630
		public bool Forget(ulong player)
		{
			global::RelationshipManager.PlayerRelationshipInfo playerRelationshipInfo;
			if (this.relations.TryGetValue(player, out playerRelationshipInfo))
			{
				this.relations.Remove(player);
				if (playerRelationshipInfo.mugshotCrc != 0U)
				{
					global::RelationshipManager.ServerInstance.DeleteMugshot(this.ownerPlayer, player, playerRelationshipInfo.mugshotCrc);
				}
				return true;
			}
			return false;
		}

		// Token: 0x06004B13 RID: 19219 RVA: 0x0019147C File Offset: 0x0018F67C
		public global::RelationshipManager.PlayerRelationshipInfo GetRelations(ulong player)
		{
			global::BasePlayer basePlayer = global::RelationshipManager.FindByID(player);
			global::RelationshipManager.PlayerRelationshipInfo playerRelationshipInfo;
			if (this.relations.TryGetValue(player, out playerRelationshipInfo))
			{
				if (basePlayer != null)
				{
					playerRelationshipInfo.displayName = basePlayer.displayName;
				}
				return playerRelationshipInfo;
			}
			global::RelationshipManager.PlayerRelationshipInfo playerRelationshipInfo2 = Facepunch.Pool.Get<global::RelationshipManager.PlayerRelationshipInfo>();
			if (basePlayer != null)
			{
				playerRelationshipInfo2.displayName = basePlayer.displayName;
			}
			playerRelationshipInfo2.player = player;
			this.relations.Add(player, playerRelationshipInfo2);
			return playerRelationshipInfo2;
		}

		// Token: 0x06004B14 RID: 19220 RVA: 0x001914E7 File Offset: 0x0018F6E7
		public PlayerRelationships()
		{
			this.LeavePool();
		}

		// Token: 0x06004B15 RID: 19221 RVA: 0x001914F5 File Offset: 0x0018F6F5
		public void EnterPool()
		{
			this.ownerPlayer = 0UL;
			if (this.relations != null)
			{
				this.relations.Clear();
				Facepunch.Pool.Free<Dictionary<ulong, global::RelationshipManager.PlayerRelationshipInfo>>(ref this.relations);
			}
		}

		// Token: 0x06004B16 RID: 19222 RVA: 0x0019151D File Offset: 0x0018F71D
		public void LeavePool()
		{
			this.ownerPlayer = 0UL;
			this.relations = Facepunch.Pool.Get<Dictionary<ulong, global::RelationshipManager.PlayerRelationshipInfo>>();
			this.relations.Clear();
		}

		// Token: 0x04003F15 RID: 16149
		public bool dirty;

		// Token: 0x04003F16 RID: 16150
		public ulong ownerPlayer;

		// Token: 0x04003F17 RID: 16151
		public Dictionary<ulong, global::RelationshipManager.PlayerRelationshipInfo> relations;
	}

	// Token: 0x02000BAA RID: 2986
	public class PlayerTeam
	{
		// Token: 0x170005FD RID: 1533
		// (get) Token: 0x06004B17 RID: 19223 RVA: 0x0019153D File Offset: 0x0018F73D
		public float teamLifetime
		{
			get
			{
				return UnityEngine.Time.realtimeSinceStartup - this.teamStartTime;
			}
		}

		// Token: 0x06004B18 RID: 19224 RVA: 0x0019154B File Offset: 0x0018F74B
		public global::BasePlayer GetLeader()
		{
			return global::RelationshipManager.FindByID(this.teamLeader);
		}

		// Token: 0x06004B19 RID: 19225 RVA: 0x00191558 File Offset: 0x0018F758
		public void SendInvite(global::BasePlayer player)
		{
			if (this.invites.Count > 8)
			{
				this.invites.RemoveRange(0, 1);
			}
			global::BasePlayer basePlayer = global::RelationshipManager.FindByID(this.teamLeader);
			if (basePlayer == null)
			{
				return;
			}
			this.invites.Add(player.userID);
			player.ClientRPCPlayer<string, ulong, ulong>(null, player, "CLIENT_PendingInvite", basePlayer.displayName, this.teamLeader, this.teamID);
		}

		// Token: 0x06004B1A RID: 19226 RVA: 0x001915C6 File Offset: 0x0018F7C6
		public void AcceptInvite(global::BasePlayer player)
		{
			if (!this.invites.Contains(player.userID))
			{
				return;
			}
			this.invites.Remove(player.userID);
			this.AddPlayer(player);
			player.ClearPendingInvite();
		}

		// Token: 0x06004B1B RID: 19227 RVA: 0x001915FC File Offset: 0x0018F7FC
		public void RejectInvite(global::BasePlayer player)
		{
			player.ClearPendingInvite();
			this.invites.Remove(player.userID);
		}

		// Token: 0x06004B1C RID: 19228 RVA: 0x00191618 File Offset: 0x0018F818
		public bool AddPlayer(global::BasePlayer player)
		{
			ulong userID = player.userID;
			if (this.members.Contains(userID))
			{
				return false;
			}
			if (player.currentTeam != 0UL)
			{
				return false;
			}
			if (this.members.Count >= global::RelationshipManager.maxTeamSize)
			{
				return false;
			}
			player.currentTeam = this.teamID;
			this.members.Add(userID);
			global::RelationshipManager.ServerInstance.playerToTeam.Add(userID, this);
			this.MarkDirty();
			player.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			return true;
		}

		// Token: 0x06004B1D RID: 19229 RVA: 0x00191694 File Offset: 0x0018F894
		public bool RemovePlayer(ulong playerID)
		{
			if (this.members.Contains(playerID))
			{
				this.members.Remove(playerID);
				global::RelationshipManager.ServerInstance.playerToTeam.Remove(playerID);
				global::BasePlayer basePlayer = global::RelationshipManager.FindByID(playerID);
				if (basePlayer != null)
				{
					basePlayer.ClearTeam();
					basePlayer.BroadcastAppTeamRemoval();
				}
				if (this.teamLeader == playerID)
				{
					if (this.members.Count > 0)
					{
						this.SetTeamLeader(this.members[0]);
					}
					else
					{
						this.Disband();
					}
				}
				this.MarkDirty();
				return true;
			}
			return false;
		}

		// Token: 0x06004B1E RID: 19230 RVA: 0x00191725 File Offset: 0x0018F925
		public void SetTeamLeader(ulong newTeamLeader)
		{
			this.teamLeader = newTeamLeader;
			this.MarkDirty();
		}

		// Token: 0x06004B1F RID: 19231 RVA: 0x00191734 File Offset: 0x0018F934
		public void Disband()
		{
			global::RelationshipManager.ServerInstance.DisbandTeam(this);
			CompanionServer.Server.TeamChat.Remove(this.teamID);
		}

		// Token: 0x06004B20 RID: 19232 RVA: 0x00191754 File Offset: 0x0018F954
		public void MarkDirty()
		{
			foreach (ulong userID in this.members)
			{
				global::BasePlayer basePlayer = global::RelationshipManager.FindByID(userID);
				if (basePlayer != null)
				{
					basePlayer.UpdateTeam(this.teamID);
				}
			}
			this.BroadcastAppTeamUpdate();
		}

		// Token: 0x06004B21 RID: 19233 RVA: 0x001917C0 File Offset: 0x0018F9C0
		public List<Network.Connection> GetOnlineMemberConnections()
		{
			if (this.members.Count == 0)
			{
				return null;
			}
			this.onlineMemberConnections.Clear();
			foreach (ulong userID in this.members)
			{
				global::BasePlayer basePlayer = global::RelationshipManager.FindByID(userID);
				if (!(basePlayer == null) && basePlayer.Connection != null)
				{
					this.onlineMemberConnections.Add(basePlayer.Connection);
				}
			}
			return this.onlineMemberConnections;
		}

		// Token: 0x04003F18 RID: 16152
		public ulong teamID;

		// Token: 0x04003F19 RID: 16153
		public string teamName;

		// Token: 0x04003F1A RID: 16154
		public ulong teamLeader;

		// Token: 0x04003F1B RID: 16155
		public List<ulong> members = new List<ulong>();

		// Token: 0x04003F1C RID: 16156
		public List<ulong> invites = new List<ulong>();

		// Token: 0x04003F1D RID: 16157
		public float teamStartTime;

		// Token: 0x04003F1E RID: 16158
		private List<Network.Connection> onlineMemberConnections = new List<Network.Connection>();
	}
}
