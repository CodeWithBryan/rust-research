using System;
using System.Collections.Generic;
using System.Linq;
using CompanionServer;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x02000037 RID: 55
[Serializable]
public class BaseGameMode : global::BaseEntity
{
	// Token: 0x06000355 RID: 853 RVA: 0x0002C424 File Offset: 0x0002A624
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseGameMode.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000356 RID: 854 RVA: 0x0002C464 File Offset: 0x0002A664
	public GameMode GetGameScores()
	{
		return this.gameModeScores;
	}

	// Token: 0x06000357 RID: 855 RVA: 0x0002C46C File Offset: 0x0002A66C
	public int ScoreColumnIndex(string scoreName)
	{
		for (int i = 0; i < this.scoreColumns.Length; i++)
		{
			if (this.scoreColumns[i] == scoreName)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06000358 RID: 856 RVA: 0x0002C4A0 File Offset: 0x0002A6A0
	public void InitScores()
	{
		this.gameModeScores = new GameMode();
		this.gameModeScores.scoreColumns = new List<GameMode.ScoreColumn>();
		this.gameModeScores.playerScores = new List<GameMode.PlayerScore>();
		this.gameModeScores.teams = new List<GameMode.TeamInfo>();
		foreach (BaseGameMode.GameModeTeam gameModeTeam in this.teams)
		{
			GameMode.TeamInfo teamInfo = new GameMode.TeamInfo();
			teamInfo.score = 0;
			teamInfo.ShouldPool = false;
			this.gameModeScores.teams.Add(teamInfo);
		}
		foreach (string name in this.scoreColumns)
		{
			GameMode.ScoreColumn scoreColumn = new GameMode.ScoreColumn();
			scoreColumn.name = name;
			scoreColumn.ShouldPool = false;
			this.gameModeScores.scoreColumns.Add(scoreColumn);
		}
		this.gameModeScores.ShouldPool = false;
	}

	// Token: 0x06000359 RID: 857 RVA: 0x0002C578 File Offset: 0x0002A778
	public void CopyGameModeScores(GameMode from, GameMode to)
	{
		to.teams.Clear();
		to.scoreColumns.Clear();
		to.playerScores.Clear();
		foreach (GameMode.TeamInfo teamInfo in from.teams)
		{
			GameMode.TeamInfo teamInfo2 = new GameMode.TeamInfo();
			teamInfo2.score = teamInfo.score;
			to.teams.Add(teamInfo2);
		}
		foreach (GameMode.ScoreColumn scoreColumn in from.scoreColumns)
		{
			GameMode.ScoreColumn scoreColumn2 = new GameMode.ScoreColumn();
			scoreColumn2.name = scoreColumn.name;
			to.scoreColumns.Add(scoreColumn2);
		}
		foreach (GameMode.PlayerScore playerScore in from.playerScores)
		{
			GameMode.PlayerScore playerScore2 = new GameMode.PlayerScore();
			playerScore2.playerName = playerScore.playerName;
			playerScore2.userid = playerScore.userid;
			playerScore2.team = playerScore.team;
			playerScore2.scores = new List<int>();
			foreach (int item in playerScore.scores)
			{
				playerScore2.scores.Add(item);
			}
			to.playerScores.Add(playerScore2);
		}
	}

	// Token: 0x0600035A RID: 858 RVA: 0x0002C73C File Offset: 0x0002A93C
	public GameMode.PlayerScore GetPlayerScoreForPlayer(global::BasePlayer player)
	{
		GameMode.PlayerScore playerScore = null;
		foreach (GameMode.PlayerScore playerScore2 in this.gameModeScores.playerScores)
		{
			if (playerScore2.userid == player.userID)
			{
				playerScore = playerScore2;
				break;
			}
		}
		if (playerScore == null)
		{
			playerScore = new GameMode.PlayerScore();
			playerScore.ShouldPool = false;
			playerScore.playerName = player.displayName;
			playerScore.userid = player.userID;
			playerScore.scores = new List<int>();
			foreach (string text in this.scoreColumns)
			{
				playerScore.scores.Add(0);
			}
			this.gameModeScores.playerScores.Add(playerScore);
		}
		return playerScore;
	}

	// Token: 0x0600035B RID: 859 RVA: 0x0002C810 File Offset: 0x0002AA10
	public int GetScoreIndexByName(string name)
	{
		for (int i = 0; i < this.scoreColumns.Length; i++)
		{
			if (this.scoreColumns[i] == name)
			{
				return i;
			}
		}
		Debug.LogWarning("No score colum named : " + name + "returning default");
		return 0;
	}

	// Token: 0x0600035C RID: 860 RVA: 0x0002C858 File Offset: 0x0002AA58
	public virtual bool IsDraw()
	{
		if (this.IsTeamGame())
		{
			int num = -1;
			int num2 = 1000000;
			for (int i = 0; i < this.teams.Length; i++)
			{
				int teamScore = this.GetTeamScore(i);
				if (teamScore < num2)
				{
					num2 = teamScore;
				}
				if (teamScore > num)
				{
					num = teamScore;
				}
			}
			return num == num2;
		}
		int num3 = -1;
		int num4 = 0;
		int num5 = this.ScoreColumnIndex(this.victoryScoreName);
		if (num5 != -1)
		{
			for (int j = 0; j < this.gameModeScores.playerScores.Count; j++)
			{
				GameMode.PlayerScore playerScore = this.gameModeScores.playerScores[j];
				if (playerScore.scores[num5] > num3)
				{
					num3 = playerScore.scores[num5];
					num4 = 1;
				}
				else if (playerScore.scores[num5] == num3)
				{
					num4++;
				}
			}
		}
		return num3 == 0 || num4 > 1;
	}

	// Token: 0x0600035D RID: 861 RVA: 0x0002C93C File Offset: 0x0002AB3C
	public virtual string GetWinnerName()
	{
		int num = -1;
		int num2 = -1;
		if (this.IsTeamGame())
		{
			for (int i = 0; i < this.teams.Length; i++)
			{
				int teamScore = this.GetTeamScore(i);
				if (teamScore > num)
				{
					num = teamScore;
					num2 = i;
				}
			}
			if (num2 == -1)
			{
				return "NO ONE";
			}
			return this.teams[num2].name;
		}
		else
		{
			int num3 = this.ScoreColumnIndex(this.victoryScoreName);
			if (num3 != -1)
			{
				for (int j = 0; j < this.gameModeScores.playerScores.Count; j++)
				{
					GameMode.PlayerScore playerScore = this.gameModeScores.playerScores[j];
					if (playerScore.scores[num3] > num)
					{
						num = playerScore.scores[num3];
						num2 = j;
					}
				}
			}
			if (num2 != -1)
			{
				return this.gameModeScores.playerScores[num2].playerName;
			}
			return "";
		}
	}

	// Token: 0x0600035E RID: 862 RVA: 0x00007074 File Offset: 0x00005274
	public virtual int GetPlayerTeamPosition(global::BasePlayer player)
	{
		return 0;
	}

	// Token: 0x0600035F RID: 863 RVA: 0x0002CA1C File Offset: 0x0002AC1C
	public virtual int GetPlayerRank(global::BasePlayer player)
	{
		int num = this.ScoreColumnIndex(this.victoryScoreName);
		if (num == -1)
		{
			return 10;
		}
		int num2 = this.GetPlayerScoreForPlayer(player).scores[num];
		int num3 = 0;
		foreach (GameMode.PlayerScore playerScore in this.gameModeScores.playerScores)
		{
			if (playerScore.scores[num] > num2 && playerScore.userid != player.userID)
			{
				num3++;
			}
		}
		return num3 + 1;
	}

	// Token: 0x06000360 RID: 864 RVA: 0x0002CAC0 File Offset: 0x0002ACC0
	public int GetWinningTeamIndex()
	{
		int num = -1;
		int num2 = -1;
		if (!this.IsTeamGame())
		{
			return -1;
		}
		for (int i = 0; i < this.teams.Length; i++)
		{
			int teamScore = this.GetTeamScore(i);
			if (teamScore > num)
			{
				num = teamScore;
				num2 = i;
			}
		}
		if (num2 == -1)
		{
			return -1;
		}
		return num2;
	}

	// Token: 0x06000361 RID: 865 RVA: 0x0002CB08 File Offset: 0x0002AD08
	public virtual bool DidPlayerWin(global::BasePlayer player)
	{
		if (player == null)
		{
			return false;
		}
		if (this.IsDraw())
		{
			return false;
		}
		if (this.IsTeamGame())
		{
			GameMode.PlayerScore playerScoreForPlayer = this.GetPlayerScoreForPlayer(player);
			return playerScoreForPlayer.team != -1 && playerScoreForPlayer.team == this.GetWinningTeamIndex();
		}
		return this.GetPlayerRank(player) == 1;
	}

	// Token: 0x06000362 RID: 866 RVA: 0x0002CB5E File Offset: 0x0002AD5E
	public bool IsTeamGame()
	{
		return this.teams.Length > 1;
	}

	// Token: 0x06000363 RID: 867 RVA: 0x0002CB6B File Offset: 0x0002AD6B
	public bool KeepScores()
	{
		return this.scoreColumns.Length != 0;
	}

	// Token: 0x06000364 RID: 868 RVA: 0x0002CB77 File Offset: 0x0002AD77
	public void ModifyTeamScore(int teamIndex, int modifyAmount)
	{
		if (!this.KeepScores())
		{
			return;
		}
		this.gameModeScores.teams[teamIndex].score += modifyAmount;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		this.CheckGameConditions(false);
	}

	// Token: 0x06000365 RID: 869 RVA: 0x0002CBAE File Offset: 0x0002ADAE
	public void SetTeamScore(int teamIndex, int score)
	{
		this.gameModeScores.teams[teamIndex].score = score;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000366 RID: 870 RVA: 0x0002CBD0 File Offset: 0x0002ADD0
	public virtual void ResetPlayerScores(global::BasePlayer player)
	{
		if (base.isClient)
		{
			return;
		}
		for (int i = 0; i < this.scoreColumns.Length; i++)
		{
			this.SetPlayerGameScore(player, i, 0);
		}
	}

	// Token: 0x06000367 RID: 871 RVA: 0x0002CC04 File Offset: 0x0002AE04
	public void ModifyPlayerGameScore(global::BasePlayer player, string scoreName, int modifyAmount)
	{
		if (!this.KeepScores())
		{
			return;
		}
		int scoreIndexByName = this.GetScoreIndexByName(scoreName);
		this.ModifyPlayerGameScore(player, scoreIndexByName, modifyAmount);
	}

	// Token: 0x06000368 RID: 872 RVA: 0x0002CC2C File Offset: 0x0002AE2C
	public void ModifyPlayerGameScore(global::BasePlayer player, int scoreIndex, int modifyAmount)
	{
		if (!this.KeepScores())
		{
			return;
		}
		this.GetPlayerScoreForPlayer(player);
		int playerGameScore = this.GetPlayerGameScore(player, scoreIndex);
		if (this.IsTeamGame() && player.gamemodeteam >= 0 && scoreIndex == this.GetScoreIndexByName(this.teamScoreName))
		{
			this.gameModeScores.teams[player.gamemodeteam].score = this.gameModeScores.teams[player.gamemodeteam].score + modifyAmount;
		}
		this.SetPlayerGameScore(player, scoreIndex, playerGameScore + modifyAmount);
	}

	// Token: 0x06000369 RID: 873 RVA: 0x0002CCB6 File Offset: 0x0002AEB6
	public int GetPlayerGameScore(global::BasePlayer player, int scoreIndex)
	{
		return this.GetPlayerScoreForPlayer(player).scores[scoreIndex];
	}

	// Token: 0x0600036A RID: 874 RVA: 0x0002CCCA File Offset: 0x0002AECA
	public void SetPlayerTeam(global::BasePlayer player, int newTeam)
	{
		player.gamemodeteam = newTeam;
		this.GetPlayerScoreForPlayer(player).team = newTeam;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600036B RID: 875 RVA: 0x0002CCE7 File Offset: 0x0002AEE7
	public void SetPlayerGameScore(global::BasePlayer player, int scoreIndex, int scoreValue)
	{
		if (base.isClient)
		{
			return;
		}
		if (!this.KeepScores())
		{
			return;
		}
		this.GetPlayerScoreForPlayer(player).scores[scoreIndex] = scoreValue;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		this.CheckGameConditions(false);
	}

	// Token: 0x0600036C RID: 876 RVA: 0x0002CD1C File Offset: 0x0002AF1C
	public int GetMaxBeds(global::BasePlayer player)
	{
		return this.maximumSleepingBags;
	}

	// Token: 0x0600036D RID: 877 RVA: 0x0002CD24 File Offset: 0x0002AF24
	protected virtual void SetupTags()
	{
		this.gameModeTags.Add("missions-" + (this.missionSystem ? "enabled" : "disabled"));
		this.gameModeTags.Add("mlrs-" + (this.mlrs ? "enabled" : "disabled"));
		this.gameModeTags.Add("map-" + (this.ingameMap ? "enabled" : "disabled"));
	}

	// Token: 0x0600036E RID: 878 RVA: 0x0002CDAC File Offset: 0x0002AFAC
	public virtual BaseGameMode.ResearchCostResult GetScrapCostForResearch(ItemDefinition item, global::ResearchTable.ResearchType researchType)
	{
		return default(BaseGameMode.ResearchCostResult);
	}

	// Token: 0x0600036F RID: 879 RVA: 0x0002CDC4 File Offset: 0x0002AFC4
	public virtual float? EvaluateSleepingBagReset(global::SleepingBag bag, Vector3 position, global::SleepingBag.SleepingBagResetReason reason)
	{
		return null;
	}

	// Token: 0x06000370 RID: 880 RVA: 0x0002CDDC File Offset: 0x0002AFDC
	public virtual BaseGameMode.CanBuildResult? CanBuildEntity(global::BasePlayer player, Construction construction)
	{
		GameObject gameObject = GameManager.server.FindPrefab(construction.prefabID);
		if (((gameObject != null) ? gameObject.GetComponent<global::BaseEntity>() : null) is global::SleepingBag)
		{
			BaseGameMode.CanAssignBedResult? canAssignBedResult = this.CanAssignBed(player, null, player.userID, 1, 0, null);
			if (canAssignBedResult != null)
			{
				if (canAssignBedResult.Value.Result)
				{
					BaseGameMode.CanBuildResult value = default(BaseGameMode.CanBuildResult);
					value.Result = true;
					value.Phrase = global::SleepingBag.bagLimitPhrase;
					string[] array = new string[2];
					int num = 0;
					BaseGameMode.CanAssignBedResult value2 = canAssignBedResult.Value;
					array[num] = value2.Count.ToString();
					int num2 = 1;
					value2 = canAssignBedResult.Value;
					array[num2] = value2.Max.ToString();
					value.Arguments = array;
					return new BaseGameMode.CanBuildResult?(value);
				}
				return new BaseGameMode.CanBuildResult?(new BaseGameMode.CanBuildResult
				{
					Result = false,
					Phrase = global::SleepingBag.bagLimitReachedPhrase
				});
			}
		}
		return null;
	}

	// Token: 0x06000371 RID: 881 RVA: 0x0002CEC8 File Offset: 0x0002B0C8
	public virtual BaseGameMode.CanAssignBedResult? CanAssignBed(global::BasePlayer player, global::SleepingBag newBag, ulong targetPlayer, int countOffset = 1, int maxOffset = 0, global::SleepingBag ignore = null)
	{
		int num = this.GetMaxBeds(player) + maxOffset;
		if (num < 0)
		{
			return null;
		}
		int num2 = countOffset;
		foreach (global::SleepingBag sleepingBag in global::SleepingBag.sleepingBags)
		{
			if (sleepingBag != ignore && sleepingBag.deployerUserID == targetPlayer)
			{
				num2++;
				if (num2 > num)
				{
					return new BaseGameMode.CanAssignBedResult?(new BaseGameMode.CanAssignBedResult
					{
						Count = num2,
						Max = num,
						Result = false
					});
				}
			}
		}
		return new BaseGameMode.CanAssignBedResult?(new BaseGameMode.CanAssignBedResult
		{
			Count = num2,
			Max = num,
			Result = true
		});
	}

	// Token: 0x06000372 RID: 882 RVA: 0x0002CFA0 File Offset: 0x0002B1A0
	private void DeleteEntities()
	{
		if (!SingletonComponent<ServerMgr>.Instance.runFrameUpdate)
		{
			base.Invoke(new Action(this.DeleteEntities), 5f);
		}
		foreach (MonumentInfo monumentInfo in (from x in TerrainMeta.Path.Monuments
		where x.IsSafeZone
		select x).ToArray<MonumentInfo>())
		{
			List<global::BaseEntity> list = new List<global::BaseEntity>();
			global::Vis.Entities<global::BaseEntity>(new OBB(monumentInfo.transform, monumentInfo.Bounds), list, -1, QueryTriggerInteraction.Collide);
			foreach (global::BaseEntity baseEntity in list)
			{
				if (!this.safeZone && (baseEntity is HumanNPC || baseEntity is NPCAutoTurret || baseEntity is Marketplace))
				{
					baseEntity.Kill(global::BaseNetworkable.DestroyMode.None);
				}
			}
			if (!this.safeZone)
			{
				NPCSpawner[] componentsInChildren = monumentInfo.GetComponentsInChildren<NPCSpawner>();
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					componentsInChildren[j].isSpawnerActive = false;
				}
			}
			if (!this.mlrs)
			{
				foreach (IndividualSpawner individualSpawner in monumentInfo.GetComponentsInChildren<IndividualSpawner>())
				{
					if (individualSpawner.entityPrefab.isValid && individualSpawner.entityPrefab.GetEntity() is global::MLRS)
					{
						individualSpawner.isSpawnerActive = false;
					}
				}
			}
		}
		foreach (global::BaseNetworkable baseNetworkable in global::BaseNetworkable.serverEntities)
		{
			if (!this.mlrs && baseNetworkable is global::MLRS)
			{
				baseNetworkable.Kill(global::BaseNetworkable.DestroyMode.None);
			}
			if (!this.missionSystem && baseNetworkable is NPCMissionProvider)
			{
				baseNetworkable.Kill(global::BaseNetworkable.DestroyMode.None);
			}
		}
	}

	// Token: 0x06000373 RID: 883 RVA: 0x0002D190 File Offset: 0x0002B390
	protected void OnCreated_Vanilla()
	{
		if (this.rustPlus != CompanionServer.Server.IsEnabled)
		{
			if (this.rustPlus)
			{
				CompanionServer.Server.Initialize();
			}
			else
			{
				CompanionServer.Server.Shutdown();
			}
		}
		if (!this.teamSystem)
		{
			global::RelationshipManager.maxTeamSize = 0;
		}
		ConVar.Server.crawlingenabled = this.crawling;
		this.DeleteEntities();
		if (this.wipeBpsOnProtocol)
		{
			SingletonComponent<ServerMgr>.Instance.persistance.Dispose();
			SingletonComponent<ServerMgr>.Instance.persistance = new UserPersistance(ConVar.Server.rootFolder);
			global::BasePlayer[] array = UnityEngine.Object.FindObjectsOfType<global::BasePlayer>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].InvalidateCachedPeristantPlayer();
			}
		}
		global::RelationshipManager.contacts = this.contactSystem;
		Chat.globalchat = this.globalChat;
		Chat.localchat = this.localChat;
	}

	// Token: 0x14000001 RID: 1
	// (add) Token: 0x06000374 RID: 884 RVA: 0x0002D248 File Offset: 0x0002B448
	// (remove) Token: 0x06000375 RID: 885 RVA: 0x0002D27C File Offset: 0x0002B47C
	public static event Action<BaseGameMode> GameModeChanged;

	// Token: 0x06000376 RID: 886 RVA: 0x0002D2B0 File Offset: 0x0002B4B0
	public bool HasAnyGameModeTag(string[] tags)
	{
		for (int i = 0; i < this.gameModeTags.Count; i++)
		{
			for (int j = 0; j < tags.Length; j++)
			{
				if (tags[j] == this.gameModeTags[i])
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000377 RID: 887 RVA: 0x0002D2FC File Offset: 0x0002B4FC
	public bool HasGameModeTag(string tag)
	{
		for (int i = 0; i < this.gameModeTags.Count; i++)
		{
			if (this.gameModeTags[i] == tag)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000378 RID: 888 RVA: 0x0002D336 File Offset: 0x0002B536
	public bool AllowsSleeping()
	{
		return this.allowSleeping;
	}

	// Token: 0x06000379 RID: 889 RVA: 0x0002D33E File Offset: 0x0002B53E
	public bool HasLoadouts()
	{
		return this.loadouts.Length != 0 || (this.IsTeamGame() && this.teams[0].teamloadouts.Length != 0);
	}

	// Token: 0x0600037A RID: 890 RVA: 0x0002D366 File Offset: 0x0002B566
	public int GetNumTeams()
	{
		if (this.teams.Length > 1)
		{
			return this.teams.Length;
		}
		return 1;
	}

	// Token: 0x0600037B RID: 891 RVA: 0x0002D37D File Offset: 0x0002B57D
	public int GetTeamScore(int teamIndex)
	{
		return this.gameModeScores.teams[teamIndex].score;
	}

	// Token: 0x0600037C RID: 892 RVA: 0x0002D398 File Offset: 0x0002B598
	public static void CreateGameMode(string overrideMode = "")
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		if (activeGameMode)
		{
			activeGameMode.ShutdownGame();
			activeGameMode.Kill(global::BaseNetworkable.DestroyMode.None);
			BaseGameMode.SetActiveGameMode(null, true);
		}
		string text = ConVar.Server.gamemode;
		Debug.Log("Gamemode Convar :" + text);
		if (!string.IsNullOrEmpty(overrideMode))
		{
			text = overrideMode;
		}
		if (string.IsNullOrEmpty(text))
		{
			Debug.Log("No Gamemode.");
			if (BaseGameMode.GameModeChanged != null)
			{
				BaseGameMode.GameModeChanged(null);
				return;
			}
		}
		else
		{
			global::BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/gamemodes/" + text + ".prefab", Vector3.zero, Quaternion.identity, true);
			if (baseEntity)
			{
				baseEntity.Spawn();
				return;
			}
			Debug.Log("Failed to create gamemode : " + text);
		}
	}

	// Token: 0x0600037D RID: 893 RVA: 0x0002D452 File Offset: 0x0002B652
	public static void SetActiveGameMode(BaseGameMode newActive, bool serverside)
	{
		if (newActive)
		{
			newActive.InitScores();
		}
		if (BaseGameMode.GameModeChanged != null)
		{
			BaseGameMode.GameModeChanged(newActive);
		}
		if (serverside)
		{
			BaseGameMode.svActiveGameMode = newActive;
			return;
		}
	}

	// Token: 0x0600037E RID: 894 RVA: 0x0002D47E File Offset: 0x0002B67E
	public static BaseGameMode GetActiveGameMode(bool serverside)
	{
		return BaseGameMode.svActiveGameMode;
	}

	// Token: 0x0600037F RID: 895 RVA: 0x0002D485 File Offset: 0x0002B685
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.gameMode != null)
		{
			this.CopyGameModeScores(info.msg.gameMode, this.gameModeScores);
		}
	}

	// Token: 0x06000380 RID: 896 RVA: 0x0002D4B4 File Offset: 0x0002B6B4
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.gameMode = Facepunch.Pool.Get<GameMode>();
		info.msg.gameMode.scoreColumns = Facepunch.Pool.GetList<GameMode.ScoreColumn>();
		info.msg.gameMode.playerScores = Facepunch.Pool.GetList<GameMode.PlayerScore>();
		info.msg.gameMode.teams = Facepunch.Pool.GetList<GameMode.TeamInfo>();
		this.CopyGameModeScores(this.gameModeScores, info.msg.gameMode);
		info.msg.gameMode.ShouldPool = true;
	}

	// Token: 0x06000381 RID: 897 RVA: 0x0002D53F File Offset: 0x0002B73F
	public virtual float CorpseRemovalTime(BaseCorpse corpse)
	{
		return ConVar.Server.corpsedespawn;
	}

	// Token: 0x06000382 RID: 898 RVA: 0x00020A80 File Offset: 0x0001EC80
	public virtual bool InWarmup()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved1);
	}

	// Token: 0x06000383 RID: 899 RVA: 0x0002D546 File Offset: 0x0002B746
	public virtual bool IsWaitingForPlayers()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved3);
	}

	// Token: 0x06000384 RID: 900 RVA: 0x00004C84 File Offset: 0x00002E84
	public virtual bool IsMatchOver()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved2);
	}

	// Token: 0x06000385 RID: 901 RVA: 0x0002D553 File Offset: 0x0002B753
	public virtual bool IsMatchActive()
	{
		return !this.InWarmup() && !this.IsWaitingForPlayers() && !this.IsMatchOver() && this.matchStartTime != -1f;
	}

	// Token: 0x06000386 RID: 902 RVA: 0x0002D580 File Offset: 0x0002B780
	public override void InitShared()
	{
		base.InitShared();
		if (BaseGameMode.GetActiveGameMode(base.isServer) != null && BaseGameMode.GetActiveGameMode(base.isServer) != this)
		{
			Debug.LogError("Already an active game mode! was : " + BaseGameMode.GetActiveGameMode(base.isServer).name);
			UnityEngine.Object.Destroy(BaseGameMode.GetActiveGameMode(base.isServer).gameObject);
		}
		this.SetupTags();
		BaseGameMode.SetActiveGameMode(this, base.isServer);
		this.OnCreated();
	}

	// Token: 0x06000387 RID: 903 RVA: 0x0002D605 File Offset: 0x0002B805
	public override void DestroyShared()
	{
		if (BaseGameMode.GetActiveGameMode(base.isServer) == this)
		{
			BaseGameMode.SetActiveGameMode(null, base.isServer);
		}
		base.DestroyShared();
	}

	// Token: 0x06000388 RID: 904 RVA: 0x0002D62C File Offset: 0x0002B82C
	protected virtual void OnCreated()
	{
		this.OnCreated_Vanilla();
		if (base.isServer)
		{
			foreach (string strCommand in this.convars)
			{
				ConsoleSystem.Run(ConsoleSystem.Option.Server, strCommand, Array.Empty<object>());
			}
			this.gameModeSpawnGroups = UnityEngine.Object.FindObjectsOfType<GameModeSpawnGroup>();
			this.UnassignAllPlayers();
			foreach (global::BasePlayer player in global::BasePlayer.activePlayerList)
			{
				this.AutoAssignTeam(player);
			}
			this.InstallSpawnpoints();
			this.ResetMatch();
		}
		Debug.Log("Game created! type was : " + base.name);
	}

	// Token: 0x06000389 RID: 905 RVA: 0x0002D710 File Offset: 0x0002B910
	protected virtual void OnMatchBegin()
	{
		this.matchStartTime = UnityEngine.Time.realtimeSinceStartup;
		base.SetFlag(global::BaseEntity.Flags.Reserved3, false, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved1, false, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
	}

	// Token: 0x0600038A RID: 906 RVA: 0x0002D748 File Offset: 0x0002B948
	public virtual void ResetMatch()
	{
		if (this.IsWaitingForPlayers())
		{
			return;
		}
		BaseGameMode.isResetting = true;
		base.SetFlag(global::BaseEntity.Flags.Reserved1, true, false, false);
		base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
		this.ResetTeamScores();
		foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
		{
			this.ResetPlayerScores(basePlayer);
			basePlayer.Hurt(100000f, DamageType.Suicide, null, false);
			basePlayer.Respawn();
		}
		GameModeSpawnGroup[] array = this.gameModeSpawnGroups;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ResetSpawnGroup();
		}
		this.matchStartTime = -1f;
		base.Invoke(new Action(this.OnMatchBegin), this.warmupDuration);
		BaseGameMode.isResetting = false;
	}

	// Token: 0x0600038B RID: 907 RVA: 0x0002D828 File Offset: 0x0002BA28
	public virtual void ResetTeamScores()
	{
		for (int i = 0; i < this.teams.Length; i++)
		{
			this.SetTeamScore(i, 0);
		}
	}

	// Token: 0x0600038C RID: 908 RVA: 0x0002D850 File Offset: 0x0002BA50
	public virtual void ShutdownGame()
	{
		this.ResetTeamScores();
		foreach (global::BasePlayer player in global::BasePlayer.activePlayerList)
		{
			this.SetPlayerTeam(player, -1);
		}
	}

	// Token: 0x0600038D RID: 909 RVA: 0x0002D8AC File Offset: 0x0002BAAC
	private void Update()
	{
		if (base.isClient)
		{
			return;
		}
		this.OnThink(UnityEngine.Time.deltaTime);
	}

	// Token: 0x0600038E RID: 910 RVA: 0x0002D8C4 File Offset: 0x0002BAC4
	protected virtual void OnThink(float delta)
	{
		if (this.matchStartTime != -1f)
		{
			float num = UnityEngine.Time.realtimeSinceStartup - this.matchStartTime;
			if (this.IsMatchActive() && this.matchDuration > 0f && num >= this.matchDuration)
			{
				this.OnMatchEnd();
			}
		}
		int num2 = 0;
		foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
		{
			if (this.autoHealDelay > 0f && basePlayer.healthFraction < 1f && basePlayer.IsAlive() && !basePlayer.IsWounded() && basePlayer.SecondsSinceAttacked >= this.autoHealDelay)
			{
				basePlayer.Heal(basePlayer.MaxHealth() * delta / this.autoHealDuration);
			}
			if (basePlayer.IsConnected)
			{
				num2++;
			}
		}
		if (num2 >= this.minPlayersToStart || this.IsWaitingForPlayers())
		{
			if (this.IsWaitingForPlayers() && num2 >= this.minPlayersToStart)
			{
				base.SetFlag(global::BaseEntity.Flags.Reserved3, false, false, true);
				base.CancelInvoke(new Action(this.ResetMatch));
				this.ResetMatch();
			}
			return;
		}
		if (this.IsMatchActive())
		{
			this.OnMatchEnd();
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved3, true, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved1, false, false, true);
	}

	// Token: 0x0600038F RID: 911 RVA: 0x0002DA2C File Offset: 0x0002BC2C
	public virtual void OnMatchEnd()
	{
		this.matchEndTime = UnityEngine.Time.time;
		Debug.Log("Match over!");
		base.SetFlag(global::BaseEntity.Flags.Reserved2, true, false, true);
		base.Invoke(new Action(this.ResetMatch), this.timeBetweenMatches);
	}

	// Token: 0x06000390 RID: 912 RVA: 0x0002DA6A File Offset: 0x0002BC6A
	public virtual void OnNewPlayer(global::BasePlayer player)
	{
		player.Respawn();
		if (!this.AllowsSleeping())
		{
			player.EndSleeping();
			player.SendNetworkUpdateImmediate(false);
		}
		this.PostPlayerRespawn(player);
	}

	// Token: 0x06000391 RID: 913 RVA: 0x0002DA8E File Offset: 0x0002BC8E
	public void PostPlayerRespawn(global::BasePlayer player)
	{
		if (this.startHealthOverride > 0f)
		{
			player.SetMaxHealth(this.startHealthOverride);
			player.health = this.startHealthOverride;
		}
	}

	// Token: 0x06000392 RID: 914 RVA: 0x0002DAB5 File Offset: 0x0002BCB5
	public virtual void OnPlayerConnected(global::BasePlayer player)
	{
		this.AutoAssignTeam(player);
		this.ResetPlayerScores(player);
	}

	// Token: 0x06000393 RID: 915 RVA: 0x0002DAC8 File Offset: 0x0002BCC8
	public virtual void UnassignAllPlayers()
	{
		foreach (global::BasePlayer player in global::BasePlayer.activePlayerList)
		{
			this.SetPlayerTeam(player, -1);
		}
	}

	// Token: 0x06000394 RID: 916 RVA: 0x0002DB1C File Offset: 0x0002BD1C
	public void AutoAssignTeam(global::BasePlayer player)
	{
		int newTeam = 0;
		int[] array = new int[this.teams.Length];
		int num = UnityEngine.Random.Range(0, this.teams.Length);
		int num2 = 0;
		if (this.teams.Length > 1)
		{
			foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
			{
				if (basePlayer.gamemodeteam >= 0 && basePlayer.gamemodeteam < this.teams.Length)
				{
					array[basePlayer.gamemodeteam]++;
				}
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] < num2)
				{
					num = i;
				}
			}
			newTeam = num;
		}
		this.SetPlayerTeam(player, newTeam);
	}

	// Token: 0x06000395 RID: 917 RVA: 0x0002DBE8 File Offset: 0x0002BDE8
	public virtual void OnPlayerDisconnected(global::BasePlayer player)
	{
		if (this.gameModeScores == null)
		{
			return;
		}
		if (base.isClient)
		{
			return;
		}
		GameMode.PlayerScore playerScore = null;
		foreach (GameMode.PlayerScore playerScore2 in this.gameModeScores.playerScores)
		{
			if (playerScore2.userid == player.userID)
			{
				playerScore = playerScore2;
				break;
			}
		}
		if (playerScore != null)
		{
			this.gameModeScores.playerScores.Remove(playerScore);
		}
	}

	// Token: 0x06000396 RID: 918 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnPlayerWounded(global::BasePlayer instigator, global::BasePlayer victim, HitInfo info)
	{
	}

	// Token: 0x06000397 RID: 919 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnPlayerRevived(global::BasePlayer instigator, global::BasePlayer victim)
	{
	}

	// Token: 0x06000398 RID: 920 RVA: 0x0002DC74 File Offset: 0x0002BE74
	public virtual void OnPlayerHurt(global::BasePlayer instigator, global::BasePlayer victim, HitInfo deathInfo = null)
	{
		if (!this.allowBleeding && victim.metabolism.bleeding.value != 0f)
		{
			victim.metabolism.bleeding.value = 0f;
			victim.metabolism.SendChangesToClient();
		}
	}

	// Token: 0x06000399 RID: 921 RVA: 0x0002DCC0 File Offset: 0x0002BEC0
	public virtual void OnPlayerDeath(global::BasePlayer instigator, global::BasePlayer victim, HitInfo deathInfo = null)
	{
		if (!this.IsMatchActive())
		{
			return;
		}
		if (victim != null && victim.IsConnected && !victim.IsNpc)
		{
			this.ModifyPlayerGameScore(victim, "deaths", 1);
		}
		bool flag = this.IsTeamGame() && instigator != null && victim != null && instigator.gamemodeteam == victim.gamemodeteam;
		if (instigator != null && victim != instigator && !flag && !instigator.IsNpc)
		{
			this.ModifyPlayerGameScore(instigator, "kills", 1);
		}
		if (instigator != null && instigator.IsConnected && !instigator.IsNpc && instigator != victim)
		{
			base.ClientRPCPlayer<string, int, bool>(null, instigator, "RPC_ScoreSplash", victim.displayName, 100, true);
		}
		if (this.hasKillFeed && instigator != null && victim != null && deathInfo.Weapon != null && deathInfo.Weapon.GetItem() != null)
		{
			string str = Vector3.Distance(instigator.transform.position, victim.transform.position).ToString("N0") + "m";
			string text = " with a " + deathInfo.Weapon.GetItem().info.displayName.translated + " from " + str;
			string msg = "You Killed " + victim.displayName + text;
			string msg2 = instigator.displayName + " Killed You" + text;
			string msg3 = instigator.displayName + " Killed" + victim.displayName + text;
			foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
			{
				if (basePlayer == instigator)
				{
					basePlayer.ChatMessage(msg);
				}
				else if (basePlayer == victim)
				{
					basePlayer.ChatMessage(msg2);
				}
				else if (global::BasePlayer.activePlayerList.Count <= 5)
				{
					basePlayer.ChatMessage(msg3);
				}
			}
		}
		this.CheckGameConditions(true);
	}

	// Token: 0x0600039A RID: 922 RVA: 0x0002DEF4 File Offset: 0x0002C0F4
	public virtual bool CanPlayerRespawn(global::BasePlayer player)
	{
		return !this.IsMatchOver() || this.IsWaitingForPlayers() || BaseGameMode.isResetting;
	}

	// Token: 0x0600039B RID: 923 RVA: 0x0002DF0D File Offset: 0x0002C10D
	public virtual void OnPlayerRespawn(global::BasePlayer player)
	{
		if (!this.AllowsSleeping())
		{
			player.EndSleeping();
			player.MarkRespawn(this.respawnDelayOverride);
			base.SendNetworkUpdateImmediate(false);
		}
		this.PostPlayerRespawn(player);
	}

	// Token: 0x0600039C RID: 924 RVA: 0x0002DF38 File Offset: 0x0002C138
	public virtual void CheckGameConditions(bool force = false)
	{
		if (!this.IsMatchActive())
		{
			return;
		}
		if (this.IsTeamGame())
		{
			for (int i = 0; i < this.teams.Length; i++)
			{
				if (this.GetTeamScore(i) >= this.numScoreForVictory)
				{
					this.OnMatchEnd();
				}
			}
			return;
		}
		int num = this.ScoreColumnIndex(this.victoryScoreName);
		if (num != -1)
		{
			using (List<GameMode.PlayerScore>.Enumerator enumerator = this.gameModeScores.playerScores.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.scores[num] >= this.numScoreForVictory)
					{
						this.OnMatchEnd();
					}
				}
			}
		}
	}

	// Token: 0x0600039D RID: 925 RVA: 0x0002DFEC File Offset: 0x0002C1EC
	public virtual void LoadoutPlayer(global::BasePlayer player)
	{
		PlayerInventoryProperties playerInventoryProperties;
		if (this.IsTeamGame())
		{
			if (player.gamemodeteam == -1)
			{
				Debug.LogWarning("Player loading out without team assigned, auto assigning!");
				this.AutoAssignTeam(player);
			}
			playerInventoryProperties = this.teams[player.gamemodeteam].teamloadouts[SeedRandom.Range((uint)player.userID, 0, this.teams[player.gamemodeteam].teamloadouts.Length)];
		}
		else if (this.useStaticLoadoutPerPlayer)
		{
			playerInventoryProperties = this.loadouts[SeedRandom.Range((uint)player.userID, 0, this.loadouts.Length)];
		}
		else
		{
			playerInventoryProperties = this.loadouts[UnityEngine.Random.Range(0, this.loadouts.Length)];
		}
		if (playerInventoryProperties)
		{
			playerInventoryProperties.GiveToPlayer(player);
		}
		else
		{
			player.inventory.GiveItem(ItemManager.CreateByName("hazmatsuit", 1, 0UL), player.inventory.containerWear);
		}
		if (this.topUpMagazines)
		{
			foreach (global::Item item in player.inventory.containerBelt.itemList)
			{
				global::BaseEntity heldEntity = item.GetHeldEntity();
				if (heldEntity != null)
				{
					global::BaseProjectile component = heldEntity.GetComponent<global::BaseProjectile>();
					if (component != null)
					{
						component.TopUpAmmo();
					}
				}
			}
		}
	}

	// Token: 0x0600039E RID: 926 RVA: 0x0002E13C File Offset: 0x0002C33C
	public virtual void InstallSpawnpoints()
	{
		this.allspawns = GameObject.FindGameObjectsWithTag("spawnpoint");
		if (this.allspawns != null)
		{
			Debug.Log("Installed : " + this.allspawns.Length + "spawn points.");
		}
	}

	// Token: 0x0600039F RID: 927 RVA: 0x0002E178 File Offset: 0x0002C378
	public virtual global::BasePlayer.SpawnPoint GetPlayerSpawn(global::BasePlayer forPlayer)
	{
		if (this.allspawns == null)
		{
			this.InstallSpawnpoints();
		}
		float num = 0f;
		int num2 = UnityEngine.Random.Range(0, this.allspawns.Length);
		if (this.allspawns.Length != 0 && forPlayer != null)
		{
			for (int i = 0; i < this.allspawns.Length; i++)
			{
				GameObject gameObject = this.allspawns[i];
				float num3 = 0f;
				for (int j = 0; j < global::BasePlayer.activePlayerList.Count; j++)
				{
					global::BasePlayer basePlayer = global::BasePlayer.activePlayerList[j];
					if (!(basePlayer == null) && basePlayer.IsAlive() && !(basePlayer == forPlayer))
					{
						float value = Vector3.Distance(basePlayer.transform.position, gameObject.transform.position);
						num3 -= 100f * (1f - Mathf.InverseLerp(8f, 16f, value));
						if (!this.IsTeamGame() || basePlayer.gamemodeteam != forPlayer.gamemodeteam)
						{
							num3 += 100f * Mathf.InverseLerp(16f, 32f, value);
						}
					}
				}
				float value2 = Vector3.Distance((forPlayer.ServerCurrentDeathNote == null) ? this.allspawns[UnityEngine.Random.Range(0, this.allspawns.Length)].transform.position : forPlayer.ServerCurrentDeathNote.worldPosition, gameObject.transform.position);
				float num4 = Mathf.InverseLerp(8f, 25f, value2);
				num3 *= num4;
				if (num3 > num)
				{
					num2 = i;
					num = num3;
				}
			}
		}
		GameObject gameObject2 = this.allspawns[num2];
		return new global::BasePlayer.SpawnPoint
		{
			pos = gameObject2.transform.position,
			rot = gameObject2.transform.rotation
		};
	}

	// Token: 0x060003A0 RID: 928 RVA: 0x0002E346 File Offset: 0x0002C546
	public virtual int GetMaxRelationshipTeamSize()
	{
		return global::RelationshipManager.maxTeamSize;
	}

	// Token: 0x060003A1 RID: 929 RVA: 0x0002E34D File Offset: 0x0002C54D
	public virtual global::SleepingBag[] FindSleepingBagsForPlayer(ulong playerID, bool ignoreTimers)
	{
		return global::SleepingBag.FindForPlayer(playerID, ignoreTimers);
	}

	// Token: 0x060003A2 RID: 930 RVA: 0x00003A54 File Offset: 0x00001C54
	public virtual bool CanMoveItemsFrom(global::PlayerInventory inv, global::BaseEntity source, global::Item item)
	{
		return true;
	}

	// Token: 0x04000279 RID: 633
	private GameMode gameModeScores;

	// Token: 0x0400027A RID: 634
	public string[] scoreColumns;

	// Token: 0x0400027B RID: 635
	[Header("Vanilla")]
	public bool globalChat = true;

	// Token: 0x0400027C RID: 636
	public bool localChat;

	// Token: 0x0400027D RID: 637
	public bool teamSystem = true;

	// Token: 0x0400027E RID: 638
	public bool safeZone = true;

	// Token: 0x0400027F RID: 639
	public bool ingameMap = true;

	// Token: 0x04000280 RID: 640
	public bool compass = true;

	// Token: 0x04000281 RID: 641
	public bool contactSystem = true;

	// Token: 0x04000282 RID: 642
	public bool crawling = true;

	// Token: 0x04000283 RID: 643
	public bool rustPlus = true;

	// Token: 0x04000284 RID: 644
	public bool wipeBpsOnProtocol;

	// Token: 0x04000285 RID: 645
	public int maximumSleepingBags = -1;

	// Token: 0x04000286 RID: 646
	public bool returnValidCombatlog = true;

	// Token: 0x04000287 RID: 647
	public bool missionSystem = true;

	// Token: 0x04000288 RID: 648
	public bool mlrs = true;

	// Token: 0x04000289 RID: 649
	public const global::BaseEntity.Flags Flag_Warmup = global::BaseEntity.Flags.Reserved1;

	// Token: 0x0400028A RID: 650
	public const global::BaseEntity.Flags Flag_GameOver = global::BaseEntity.Flags.Reserved2;

	// Token: 0x0400028B RID: 651
	public const global::BaseEntity.Flags Flag_WaitingForPlayers = global::BaseEntity.Flags.Reserved3;

	// Token: 0x0400028C RID: 652
	[Header("Changelog")]
	public Translate.Phrase[] addedFeatures;

	// Token: 0x0400028D RID: 653
	public Translate.Phrase[] removedFeatures;

	// Token: 0x0400028E RID: 654
	public Translate.Phrase[] changedFeatures;

	// Token: 0x0400028F RID: 655
	public List<string> convars = new List<string>();

	// Token: 0x04000291 RID: 657
	public string shortname = "vanilla";

	// Token: 0x04000292 RID: 658
	public float matchDuration = -1f;

	// Token: 0x04000293 RID: 659
	public float warmupDuration = 10f;

	// Token: 0x04000294 RID: 660
	public float timeBetweenMatches = 10f;

	// Token: 0x04000295 RID: 661
	public int minPlayersToStart = 1;

	// Token: 0x04000296 RID: 662
	public bool useCustomSpawns = true;

	// Token: 0x04000297 RID: 663
	public string victoryScoreName = "kills";

	// Token: 0x04000298 RID: 664
	public string teamScoreName = "kills";

	// Token: 0x04000299 RID: 665
	public int numScoreForVictory = 10;

	// Token: 0x0400029A RID: 666
	public string gamemodeTitle;

	// Token: 0x0400029B RID: 667
	public SoundDefinition[] warmupMusics;

	// Token: 0x0400029C RID: 668
	public SoundDefinition[] lossMusics;

	// Token: 0x0400029D RID: 669
	public SoundDefinition[] winMusics;

	// Token: 0x0400029E RID: 670
	[NonSerialized]
	private float warmupStartTime;

	// Token: 0x0400029F RID: 671
	[NonSerialized]
	private float matchStartTime = -1f;

	// Token: 0x040002A0 RID: 672
	[NonSerialized]
	private float matchEndTime;

	// Token: 0x040002A1 RID: 673
	public List<string> gameModeTags;

	// Token: 0x040002A2 RID: 674
	public global::BasePlayer.CameraMode deathCameraMode = global::BasePlayer.CameraMode.Eyes;

	// Token: 0x040002A3 RID: 675
	public bool permanent = true;

	// Token: 0x040002A4 RID: 676
	public bool limitTeamAuths;

	// Token: 0x040002A5 RID: 677
	public bool allowSleeping = true;

	// Token: 0x040002A6 RID: 678
	public bool allowWounding = true;

	// Token: 0x040002A7 RID: 679
	public bool allowBleeding = true;

	// Token: 0x040002A8 RID: 680
	public bool allowTemperature = true;

	// Token: 0x040002A9 RID: 681
	public bool quickRespawn;

	// Token: 0x040002AA RID: 682
	public bool quickDeploy;

	// Token: 0x040002AB RID: 683
	public float respawnDelayOverride = 5f;

	// Token: 0x040002AC RID: 684
	public float startHealthOverride;

	// Token: 0x040002AD RID: 685
	public float autoHealDelay;

	// Token: 0x040002AE RID: 686
	public float autoHealDuration = 1f;

	// Token: 0x040002AF RID: 687
	public bool hasKillFeed;

	// Token: 0x040002B0 RID: 688
	public static BaseGameMode svActiveGameMode = null;

	// Token: 0x040002B1 RID: 689
	public static List<BaseGameMode> svGameModeManifest = new List<BaseGameMode>();

	// Token: 0x040002B2 RID: 690
	[NonSerialized]
	private GameObject[] allspawns;

	// Token: 0x040002B3 RID: 691
	[NonSerialized]
	private GameModeSpawnGroup[] gameModeSpawnGroups;

	// Token: 0x040002B4 RID: 692
	public PlayerInventoryProperties[] loadouts;

	// Token: 0x040002B5 RID: 693
	[Tooltip("Use steamID to always pick the same loadout per player")]
	public bool useStaticLoadoutPerPlayer;

	// Token: 0x040002B6 RID: 694
	public bool topUpMagazines;

	// Token: 0x040002B7 RID: 695
	public bool sendKillNotifications;

	// Token: 0x040002B8 RID: 696
	public BaseGameMode.GameModeTeam[] teams;

	// Token: 0x040002B9 RID: 697
	public float corpseRemovalTimeOverride;

	// Token: 0x040002BA RID: 698
	private static bool isResetting = false;

	// Token: 0x02000B44 RID: 2884
	public struct ResearchCostResult
	{
		// Token: 0x04003D4A RID: 15690
		public float? Scale;

		// Token: 0x04003D4B RID: 15691
		public int? Amount;
	}

	// Token: 0x02000B45 RID: 2885
	public struct CanAssignBedResult
	{
		// Token: 0x04003D4C RID: 15692
		public bool Result;

		// Token: 0x04003D4D RID: 15693
		public int Count;

		// Token: 0x04003D4E RID: 15694
		public int Max;
	}

	// Token: 0x02000B46 RID: 2886
	public struct CanBuildResult
	{
		// Token: 0x04003D4F RID: 15695
		public bool Result;

		// Token: 0x04003D50 RID: 15696
		public Translate.Phrase Phrase;

		// Token: 0x04003D51 RID: 15697
		public string[] Arguments;
	}

	// Token: 0x02000B47 RID: 2887
	[Serializable]
	public class GameModeTeam
	{
		// Token: 0x04003D52 RID: 15698
		public string name;

		// Token: 0x04003D53 RID: 15699
		public PlayerInventoryProperties[] teamloadouts;
	}
}
