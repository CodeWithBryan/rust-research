using System;
using System.Collections.Generic;
using Facepunch;
using Facepunch.Rust;
using Rust;
using UnityEngine;

// Token: 0x020005E2 RID: 1506
[CreateAssetMenu(menuName = "Rust/Missions/BaseMission")]
public class BaseMission : BaseScriptableObject
{
	// Token: 0x1700036E RID: 878
	// (get) Token: 0x06002C34 RID: 11316 RVA: 0x001093AE File Offset: 0x001075AE
	public uint id
	{
		get
		{
			return this.shortname.ManifestHash();
		}
	}

	// Token: 0x06002C35 RID: 11317 RVA: 0x001093BC File Offset: 0x001075BC
	public static void PlayerDisconnected(BasePlayer player)
	{
		if (player.IsNpc)
		{
			return;
		}
		int activeMission = player.GetActiveMission();
		if (activeMission != -1 && activeMission < player.missions.Count)
		{
			BaseMission.MissionInstance missionInstance = player.missions[activeMission];
			BaseMission mission = missionInstance.GetMission();
			if (mission.missionEntities.Length != 0)
			{
				mission.MissionFailed(missionInstance, player, BaseMission.MissionFailReason.Disconnect);
			}
		}
	}

	// Token: 0x06002C36 RID: 11318 RVA: 0x000059DD File Offset: 0x00003BDD
	public static void PlayerKilled(BasePlayer player)
	{
	}

	// Token: 0x1700036F RID: 879
	// (get) Token: 0x06002C37 RID: 11319 RVA: 0x00109411 File Offset: 0x00107611
	public bool isRepeatable
	{
		get
		{
			return this.repeatDelaySecondsSuccess != -1 || this.repeatDelaySecondsFailed != -1;
		}
	}

	// Token: 0x06002C38 RID: 11320 RVA: 0x0010942A File Offset: 0x0010762A
	public virtual Sprite GetIcon(BaseMission.MissionInstance instance)
	{
		return this.icon;
	}

	// Token: 0x06002C39 RID: 11321 RVA: 0x00109434 File Offset: 0x00107634
	public virtual void SetupPositions(BaseMission.MissionInstance instance, BasePlayer assignee)
	{
		foreach (BaseMission.PositionGenerator positionGenerator in this.positionGenerators)
		{
			if (!positionGenerator.IsDependant())
			{
				instance.missionPoints.Add(positionGenerator.GetIdentifier(), positionGenerator.GetPosition(assignee));
			}
		}
		foreach (BaseMission.PositionGenerator positionGenerator2 in this.positionGenerators)
		{
			if (positionGenerator2.IsDependant())
			{
				instance.missionPoints.Add(positionGenerator2.GetIdentifier(), positionGenerator2.GetPosition(assignee));
			}
		}
	}

	// Token: 0x06002C3A RID: 11322 RVA: 0x001094B4 File Offset: 0x001076B4
	public void AddBlockers(BaseMission.MissionInstance instance)
	{
		foreach (KeyValuePair<string, Vector3> keyValuePair in instance.missionPoints)
		{
			if (!BaseMission.blockedPoints.Contains(keyValuePair.Value))
			{
				BaseMission.blockedPoints.Add(keyValuePair.Value);
			}
		}
	}

	// Token: 0x06002C3B RID: 11323 RVA: 0x00109524 File Offset: 0x00107724
	public void RemoveBlockers(BaseMission.MissionInstance instance)
	{
		foreach (KeyValuePair<string, Vector3> keyValuePair in instance.missionPoints)
		{
			if (BaseMission.blockedPoints.Contains(keyValuePair.Value))
			{
				BaseMission.blockedPoints.Remove(keyValuePair.Value);
			}
		}
	}

	// Token: 0x06002C3C RID: 11324 RVA: 0x00109598 File Offset: 0x00107798
	public virtual void SetupRewards(BaseMission.MissionInstance instance, BasePlayer assignee)
	{
		if (this.baseRewards.Length == 0)
		{
			return;
		}
		instance.rewards = new ItemAmount[this.baseRewards.Length];
		for (int i = 0; i < this.baseRewards.Length; i++)
		{
			instance.rewards[i] = new ItemAmount(this.baseRewards[i].itemDef, this.baseRewards[i].amount);
		}
	}

	// Token: 0x06002C3D RID: 11325 RVA: 0x001095FC File Offset: 0x001077FC
	public static void DoMissionEffect(string effectString, BasePlayer assignee)
	{
		Effect effect = new Effect();
		effect.Init(Effect.Type.Generic, assignee, StringPool.Get("head"), Vector3.zero, Vector3.forward, null);
		effect.pooledString = effectString;
		EffectNetwork.Send(effect, assignee.net.connection);
	}

	// Token: 0x06002C3E RID: 11326 RVA: 0x00109638 File Offset: 0x00107838
	public virtual void MissionStart(BaseMission.MissionInstance instance, BasePlayer assignee)
	{
		this.SetupRewards(instance, assignee);
		this.SetupPositions(instance, assignee);
		this.AddBlockers(instance);
		for (int i = 0; i < this.objectives.Length; i++)
		{
			this.objectives[i].Get().MissionStarted(i, instance);
		}
		if (this.acceptEffect.isValid)
		{
			BaseMission.DoMissionEffect(this.acceptEffect.resourcePath, assignee);
		}
		foreach (BaseMission.MissionEntityEntry missionEntityEntry in this.missionEntities)
		{
			if (missionEntityEntry.entityRef.isValid)
			{
				Vector3 missionPoint = instance.GetMissionPoint(missionEntityEntry.spawnPositionToUse, assignee);
				BaseEntity baseEntity = GameManager.server.CreateEntity(missionEntityEntry.entityRef.resourcePath, missionPoint, Quaternion.identity, true);
				MissionEntity missionEntity = baseEntity.gameObject.AddComponent<MissionEntity>();
				missionEntity.Setup(assignee, instance, missionEntityEntry.cleanupOnMissionSuccess, missionEntityEntry.cleanupOnMissionFailed);
				instance.createdEntities.Add(missionEntity);
				baseEntity.Spawn();
			}
		}
		foreach (MissionEntity missionEntity2 in instance.createdEntities)
		{
			missionEntity2.MissionStarted(assignee, instance);
		}
	}

	// Token: 0x06002C3F RID: 11327 RVA: 0x00109770 File Offset: 0x00107970
	public void CheckObjectives(BaseMission.MissionInstance instance, BasePlayer assignee)
	{
		bool flag = true;
		for (int i = 0; i < this.objectives.Length; i++)
		{
			if (!instance.objectiveStatuses[i].completed || instance.objectiveStatuses[i].failed)
			{
				flag = false;
			}
		}
		if (flag && instance.status == BaseMission.MissionStatus.Active)
		{
			this.MissionSuccess(instance, assignee);
		}
	}

	// Token: 0x06002C40 RID: 11328 RVA: 0x001097C8 File Offset: 0x001079C8
	public virtual void Think(BaseMission.MissionInstance instance, BasePlayer assignee, float delta)
	{
		for (int i = 0; i < this.objectives.Length; i++)
		{
			this.objectives[i].Get().Think(i, instance, assignee, delta);
		}
		this.CheckObjectives(instance, assignee);
	}

	// Token: 0x06002C41 RID: 11329 RVA: 0x00109808 File Offset: 0x00107A08
	public virtual void MissionComplete(BaseMission.MissionInstance instance, BasePlayer assignee)
	{
		BaseMission.DoMissionEffect(this.victoryEffect.resourcePath, assignee);
		assignee.ChatMessage("You have completed the mission : " + this.missionName.english);
		if (instance.rewards != null && instance.rewards.Length != 0)
		{
			foreach (ItemAmount itemAmount in instance.rewards)
			{
				if (itemAmount.itemDef == null || itemAmount.amount == 0f)
				{
					Debug.LogError("BIG REWARD SCREWUP, NULL ITEM DEF");
				}
				Item item = ItemManager.Create(itemAmount.itemDef, Mathf.CeilToInt(itemAmount.amount), 0UL);
				if (item != null)
				{
					assignee.GiveItem(item, BaseEntity.GiveItemReason.PickedUp);
				}
			}
		}
		Analytics.Server.MissionComplete(this);
		instance.status = BaseMission.MissionStatus.Completed;
		assignee.SetActiveMission(-1);
		assignee.MissionDirty(true);
		if (GameInfo.HasAchievements)
		{
			assignee.stats.Add("missions_completed", 1, Stats.All);
			assignee.stats.Save(true);
		}
	}

	// Token: 0x06002C42 RID: 11330 RVA: 0x001098F4 File Offset: 0x00107AF4
	public virtual void MissionSuccess(BaseMission.MissionInstance instance, BasePlayer assignee)
	{
		instance.status = BaseMission.MissionStatus.Accomplished;
		this.MissionEnded(instance, assignee);
		this.MissionComplete(instance, assignee);
	}

	// Token: 0x06002C43 RID: 11331 RVA: 0x00109910 File Offset: 0x00107B10
	public virtual void MissionFailed(BaseMission.MissionInstance instance, BasePlayer assignee, BaseMission.MissionFailReason failReason)
	{
		assignee.ChatMessage("You have failed the mission : " + this.missionName.english);
		BaseMission.DoMissionEffect(this.failedEffect.resourcePath, assignee);
		Analytics.Server.MissionFailed(this, failReason);
		instance.status = BaseMission.MissionStatus.Failed;
		this.MissionEnded(instance, assignee);
	}

	// Token: 0x06002C44 RID: 11332 RVA: 0x00109960 File Offset: 0x00107B60
	public virtual void MissionEnded(BaseMission.MissionInstance instance, BasePlayer assignee)
	{
		if (instance.createdEntities != null)
		{
			for (int i = instance.createdEntities.Count - 1; i >= 0; i--)
			{
				MissionEntity missionEntity = instance.createdEntities[i];
				if (!(missionEntity == null))
				{
					missionEntity.MissionEnded(assignee, instance);
				}
			}
		}
		this.RemoveBlockers(instance);
		instance.endTime = Time.time;
		assignee.SetActiveMission(-1);
		assignee.MissionDirty(true);
	}

	// Token: 0x06002C45 RID: 11333 RVA: 0x001099CC File Offset: 0x00107BCC
	public void OnObjectiveCompleted(int objectiveIndex, BaseMission.MissionInstance instance, BasePlayer playerFor)
	{
		BaseMission.MissionObjectiveEntry missionObjectiveEntry = this.objectives[objectiveIndex];
		if (missionObjectiveEntry.autoCompleteOtherObjectives.Length != 0)
		{
			foreach (int num in missionObjectiveEntry.autoCompleteOtherObjectives)
			{
				BaseMission.MissionObjectiveEntry missionObjectiveEntry2 = this.objectives[num];
				if (!instance.objectiveStatuses[num].completed)
				{
					missionObjectiveEntry2.objective.CompleteObjective(num, instance, playerFor);
				}
			}
		}
		this.CheckObjectives(instance, playerFor);
	}

	// Token: 0x06002C46 RID: 11334 RVA: 0x00109A34 File Offset: 0x00107C34
	public static bool AssignMission(BasePlayer assignee, IMissionProvider provider, BaseMission mission)
	{
		if (!BaseMission.missionsenabled)
		{
			return false;
		}
		if (!mission.IsEligableForMission(assignee, provider))
		{
			return false;
		}
		BaseMission.MissionInstance missionInstance = Pool.Get<BaseMission.MissionInstance>();
		missionInstance.missionID = mission.id;
		missionInstance.startTime = Time.time;
		missionInstance.providerID = provider.ProviderID();
		missionInstance.status = BaseMission.MissionStatus.Active;
		missionInstance.createdEntities = Pool.GetList<MissionEntity>();
		missionInstance.objectiveStatuses = new BaseMission.MissionInstance.ObjectiveStatus[mission.objectives.Length];
		for (int i = 0; i < mission.objectives.Length; i++)
		{
			missionInstance.objectiveStatuses[i] = new BaseMission.MissionInstance.ObjectiveStatus();
		}
		assignee.AddMission(missionInstance);
		mission.MissionStart(missionInstance, assignee);
		assignee.SetActiveMission(assignee.missions.Count - 1);
		assignee.MissionDirty(true);
		return true;
	}

	// Token: 0x06002C47 RID: 11335 RVA: 0x00109AF0 File Offset: 0x00107CF0
	public bool IsEligableForMission(BasePlayer player, IMissionProvider provider)
	{
		if (!BaseMission.missionsenabled)
		{
			return false;
		}
		foreach (BaseMission.MissionInstance missionInstance in player.missions)
		{
			if (missionInstance.status == BaseMission.MissionStatus.Accomplished || missionInstance.status == BaseMission.MissionStatus.Active)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0400240D RID: 9229
	[ServerVar]
	public static bool missionsenabled = true;

	// Token: 0x0400240E RID: 9230
	public string shortname;

	// Token: 0x0400240F RID: 9231
	public Translate.Phrase missionName;

	// Token: 0x04002410 RID: 9232
	public Translate.Phrase missionDesc;

	// Token: 0x04002411 RID: 9233
	public BaseMission.MissionObjectiveEntry[] objectives;

	// Token: 0x04002412 RID: 9234
	public static List<Vector3> blockedPoints = new List<Vector3>();

	// Token: 0x04002413 RID: 9235
	public const string MISSION_COMPLETE_STAT = "missions_completed";

	// Token: 0x04002414 RID: 9236
	public GameObjectRef acceptEffect;

	// Token: 0x04002415 RID: 9237
	public GameObjectRef failedEffect;

	// Token: 0x04002416 RID: 9238
	public GameObjectRef victoryEffect;

	// Token: 0x04002417 RID: 9239
	public int repeatDelaySecondsSuccess = -1;

	// Token: 0x04002418 RID: 9240
	public int repeatDelaySecondsFailed = -1;

	// Token: 0x04002419 RID: 9241
	public float timeLimitSeconds;

	// Token: 0x0400241A RID: 9242
	public Sprite icon;

	// Token: 0x0400241B RID: 9243
	public Sprite providerIcon;

	// Token: 0x0400241C RID: 9244
	public BaseMission.MissionDependancy[] acceptDependancies;

	// Token: 0x0400241D RID: 9245
	public BaseMission.MissionDependancy[] completionDependancies;

	// Token: 0x0400241E RID: 9246
	public BaseMission.MissionEntityEntry[] missionEntities;

	// Token: 0x0400241F RID: 9247
	public BaseMission.PositionGenerator[] positionGenerators;

	// Token: 0x04002420 RID: 9248
	public ItemAmount[] baseRewards;

	// Token: 0x02000D2F RID: 3375
	[Serializable]
	public class MissionDependancy
	{
		// Token: 0x1700064D RID: 1613
		// (get) Token: 0x06004E35 RID: 20021 RVA: 0x0019A4EA File Offset: 0x001986EA
		public uint targetMissionID
		{
			get
			{
				return this.targetMissionShortname.ManifestHash();
			}
		}

		// Token: 0x04004550 RID: 17744
		public string targetMissionShortname;

		// Token: 0x04004551 RID: 17745
		public BaseMission.MissionStatus targetMissionDesiredStatus;

		// Token: 0x04004552 RID: 17746
		public bool everAttempted;
	}

	// Token: 0x02000D30 RID: 3376
	public enum MissionStatus
	{
		// Token: 0x04004554 RID: 17748
		Default,
		// Token: 0x04004555 RID: 17749
		Active,
		// Token: 0x04004556 RID: 17750
		Accomplished,
		// Token: 0x04004557 RID: 17751
		Failed,
		// Token: 0x04004558 RID: 17752
		Completed
	}

	// Token: 0x02000D31 RID: 3377
	public enum MissionEventType
	{
		// Token: 0x0400455A RID: 17754
		CUSTOM,
		// Token: 0x0400455B RID: 17755
		HARVEST,
		// Token: 0x0400455C RID: 17756
		CONVERSATION,
		// Token: 0x0400455D RID: 17757
		KILL_ENTITY,
		// Token: 0x0400455E RID: 17758
		ACQUIRE_ITEM,
		// Token: 0x0400455F RID: 17759
		FREE_CRATE
	}

	// Token: 0x02000D32 RID: 3378
	[Serializable]
	public class MissionObjectiveEntry
	{
		// Token: 0x06004E37 RID: 20023 RVA: 0x0019A4F7 File Offset: 0x001986F7
		public MissionObjective Get()
		{
			return this.objective;
		}

		// Token: 0x04004560 RID: 17760
		public Translate.Phrase description;

		// Token: 0x04004561 RID: 17761
		public int[] startAfterCompletedObjectives;

		// Token: 0x04004562 RID: 17762
		public int[] autoCompleteOtherObjectives;

		// Token: 0x04004563 RID: 17763
		public bool onlyProgressIfStarted = true;

		// Token: 0x04004564 RID: 17764
		public MissionObjective objective;
	}

	// Token: 0x02000D33 RID: 3379
	public class MissionInstance : Pool.IPooled
	{
		// Token: 0x06004E39 RID: 20025 RVA: 0x0019A50E File Offset: 0x0019870E
		public BaseEntity ProviderEntity()
		{
			if (this._cachedProviderEntity == null)
			{
				this._cachedProviderEntity = (BaseNetworkable.serverEntities.Find(this.providerID) as BaseEntity);
			}
			return this._cachedProviderEntity;
		}

		// Token: 0x06004E3A RID: 20026 RVA: 0x0019A53F File Offset: 0x0019873F
		public BaseMission GetMission()
		{
			if (this._cachedMission == null)
			{
				this._cachedMission = MissionManifest.GetFromID(this.missionID);
			}
			return this._cachedMission;
		}

		// Token: 0x06004E3B RID: 20027 RVA: 0x0019A566 File Offset: 0x00198766
		public bool ShouldShowOnMap()
		{
			return (this.status == BaseMission.MissionStatus.Active || this.status == BaseMission.MissionStatus.Accomplished) && this.missionLocation != Vector3.zero;
		}

		// Token: 0x06004E3C RID: 20028 RVA: 0x0019A58C File Offset: 0x0019878C
		public bool ShouldShowOnCompass()
		{
			return this.ShouldShowOnMap();
		}

		// Token: 0x06004E3D RID: 20029 RVA: 0x0019A594 File Offset: 0x00198794
		public virtual void ProcessMissionEvent(BasePlayer playerFor, BaseMission.MissionEventType type, string identifier, float amount)
		{
			if (this.status != BaseMission.MissionStatus.Active)
			{
				return;
			}
			BaseMission mission = this.GetMission();
			for (int i = 0; i < mission.objectives.Length; i++)
			{
				mission.objectives[i].objective.ProcessMissionEvent(playerFor, this, i, type, identifier, amount);
			}
		}

		// Token: 0x06004E3E RID: 20030 RVA: 0x0019A5E0 File Offset: 0x001987E0
		public void Think(BasePlayer assignee, float delta)
		{
			if (this.status == BaseMission.MissionStatus.Failed || this.status == BaseMission.MissionStatus.Completed)
			{
				return;
			}
			BaseMission mission = this.GetMission();
			this.timePassed += delta;
			mission.Think(this, assignee, delta);
			if (mission.timeLimitSeconds > 0f && this.timePassed >= mission.timeLimitSeconds)
			{
				mission.MissionFailed(this, assignee, BaseMission.MissionFailReason.TimeOut);
			}
		}

		// Token: 0x06004E3F RID: 20031 RVA: 0x0019A644 File Offset: 0x00198844
		public Vector3 GetMissionPoint(string identifier, BasePlayer playerFor)
		{
			if (this.missionPoints.ContainsKey(identifier))
			{
				return this.missionPoints[identifier];
			}
			if (!playerFor)
			{
				Debug.Log("Massive mission failure to get point, correct mission definition of : " + this.GetMission().shortname);
				return Vector3.zero;
			}
			this.GetMission().SetupPositions(this, playerFor);
			Debug.Log("Mission point not found, regenerating");
			if (this.missionPoints.ContainsKey(identifier))
			{
				return this.missionPoints[identifier];
			}
			return Vector3.zero;
		}

		// Token: 0x06004E40 RID: 20032 RVA: 0x0019A6CC File Offset: 0x001988CC
		public void EnterPool()
		{
			this.providerID = 0U;
			this.missionID = 0U;
			this.status = BaseMission.MissionStatus.Default;
			this.completionScale = 0f;
			this.startTime = -1f;
			this.endTime = -1f;
			this.missionLocation = Vector3.zero;
			this._cachedMission = null;
			this.timePassed = 0f;
			this.rewards = null;
			this.missionPoints.Clear();
			if (this.createdEntities != null)
			{
				Pool.FreeList<MissionEntity>(ref this.createdEntities);
			}
		}

		// Token: 0x06004E41 RID: 20033 RVA: 0x0019A751 File Offset: 0x00198951
		public void LeavePool()
		{
			this.createdEntities = Pool.GetList<MissionEntity>();
		}

		// Token: 0x04004565 RID: 17765
		private BaseEntity _cachedProviderEntity;

		// Token: 0x04004566 RID: 17766
		private BaseMission _cachedMission;

		// Token: 0x04004567 RID: 17767
		public uint providerID;

		// Token: 0x04004568 RID: 17768
		public uint missionID;

		// Token: 0x04004569 RID: 17769
		public BaseMission.MissionStatus status;

		// Token: 0x0400456A RID: 17770
		public float completionScale;

		// Token: 0x0400456B RID: 17771
		public float startTime;

		// Token: 0x0400456C RID: 17772
		public float endTime;

		// Token: 0x0400456D RID: 17773
		public Vector3 missionLocation;

		// Token: 0x0400456E RID: 17774
		public float timePassed;

		// Token: 0x0400456F RID: 17775
		public Dictionary<string, Vector3> missionPoints = new Dictionary<string, Vector3>();

		// Token: 0x04004570 RID: 17776
		public BaseMission.MissionInstance.ObjectiveStatus[] objectiveStatuses;

		// Token: 0x04004571 RID: 17777
		public List<MissionEntity> createdEntities;

		// Token: 0x04004572 RID: 17778
		public ItemAmount[] rewards;

		// Token: 0x02000F66 RID: 3942
		[Serializable]
		public class ObjectiveStatus
		{
			// Token: 0x04004E26 RID: 20006
			public bool started;

			// Token: 0x04004E27 RID: 20007
			public bool completed;

			// Token: 0x04004E28 RID: 20008
			public bool failed;

			// Token: 0x04004E29 RID: 20009
			public int genericInt1;

			// Token: 0x04004E2A RID: 20010
			public float genericFloat1;
		}

		// Token: 0x02000F67 RID: 3943
		public enum ObjectiveType
		{
			// Token: 0x04004E2C RID: 20012
			MOVE,
			// Token: 0x04004E2D RID: 20013
			KILL
		}
	}

	// Token: 0x02000D34 RID: 3380
	[Serializable]
	public class PositionGenerator
	{
		// Token: 0x06004E43 RID: 20035 RVA: 0x0019A771 File Offset: 0x00198971
		public bool IsDependant()
		{
			return !string.IsNullOrEmpty(this.centerOnPositionIdentifier);
		}

		// Token: 0x06004E44 RID: 20036 RVA: 0x0019A781 File Offset: 0x00198981
		public string GetIdentifier()
		{
			return this.identifier;
		}

		// Token: 0x06004E45 RID: 20037 RVA: 0x0019A78C File Offset: 0x0019898C
		public bool Validate(BasePlayer assignee, BaseMission missionDef)
		{
			Vector3 vector;
			if (this.positionType == BaseMission.PositionGenerator.PositionType.MissionPoint)
			{
				List<MissionPoint> list = Pool.GetList<MissionPoint>();
				bool missionPoints = MissionPoint.GetMissionPoints(ref list, assignee.transform.position, this.minDistForMovePoint, this.maxDistForMovePoint, (int)this.Flags, (int)this.ExclusionFlags);
				Pool.FreeList<MissionPoint>(ref list);
				if (!missionPoints)
				{
					Debug.Log("FAILED TO FIND MISSION POINTS");
					return false;
				}
			}
			else if (this.positionType == BaseMission.PositionGenerator.PositionType.WorldPositionGenerator && this.worldPositionGenerator != null && !this.worldPositionGenerator.TrySample(assignee.transform.position, this.minDistForMovePoint, this.maxDistForMovePoint, out vector, BaseMission.blockedPoints))
			{
				Debug.Log("FAILED TO GENERATE WORLD POSITION!!!!!");
				return false;
			}
			return true;
		}

		// Token: 0x06004E46 RID: 20038 RVA: 0x0019A838 File Offset: 0x00198A38
		public Vector3 GetPosition(BasePlayer assignee)
		{
			Vector3 vector;
			if (this.positionType == BaseMission.PositionGenerator.PositionType.MissionPoint)
			{
				List<MissionPoint> list = Pool.GetList<MissionPoint>();
				if (MissionPoint.GetMissionPoints(ref list, assignee.transform.position, this.minDistForMovePoint, this.maxDistForMovePoint, (int)this.Flags, (int)this.ExclusionFlags))
				{
					vector = list[UnityEngine.Random.Range(0, list.Count)].GetPosition();
				}
				else
				{
					Debug.LogError("UNABLE TO FIND MISSIONPOINT FOR MISSION!");
					vector = assignee.transform.position;
				}
				Pool.FreeList<MissionPoint>(ref list);
			}
			else if (this.positionType == BaseMission.PositionGenerator.PositionType.WorldPositionGenerator && this.worldPositionGenerator != null)
			{
				if (!this.worldPositionGenerator.TrySample(assignee.transform.position, this.minDistForMovePoint, this.maxDistForMovePoint, out vector, BaseMission.blockedPoints))
				{
					Debug.LogError("UNABLE TO FIND WORLD POINT FOR MISSION!");
					vector = assignee.transform.position;
				}
			}
			else if (this.positionType == BaseMission.PositionGenerator.PositionType.DungeonPoint)
			{
				vector = DynamicDungeon.GetNextDungeonPoint();
			}
			else
			{
				Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
				onUnitSphere.y = 0f;
				onUnitSphere.Normalize();
				vector = (this.centerOnPlayer ? assignee.transform.position : assignee.transform.position) + onUnitSphere * UnityEngine.Random.Range(this.minDistForMovePoint, this.maxDistForMovePoint);
				float b = vector.y;
				float a = vector.y;
				if (TerrainMeta.WaterMap != null)
				{
					a = TerrainMeta.WaterMap.GetHeight(vector);
				}
				if (TerrainMeta.HeightMap != null)
				{
					b = TerrainMeta.HeightMap.GetHeight(vector);
				}
				vector.y = Mathf.Max(a, b);
			}
			return vector;
		}

		// Token: 0x04004573 RID: 17779
		public string identifier;

		// Token: 0x04004574 RID: 17780
		public float minDistForMovePoint;

		// Token: 0x04004575 RID: 17781
		public float maxDistForMovePoint = 25f;

		// Token: 0x04004576 RID: 17782
		public bool centerOnProvider;

		// Token: 0x04004577 RID: 17783
		public bool centerOnPlayer;

		// Token: 0x04004578 RID: 17784
		public string centerOnPositionIdentifier = "";

		// Token: 0x04004579 RID: 17785
		public BaseMission.PositionGenerator.PositionType positionType;

		// Token: 0x0400457A RID: 17786
		[Header("MissionPoint")]
		[global::InspectorFlags]
		public MissionPoint.MissionPointEnum Flags = (MissionPoint.MissionPointEnum)(-1);

		// Token: 0x0400457B RID: 17787
		[global::InspectorFlags]
		public MissionPoint.MissionPointEnum ExclusionFlags;

		// Token: 0x0400457C RID: 17788
		[Header("WorldPositionGenerator")]
		public WorldPositionGenerator worldPositionGenerator;

		// Token: 0x02000F68 RID: 3944
		public enum PositionType
		{
			// Token: 0x04004E2F RID: 20015
			MissionPoint,
			// Token: 0x04004E30 RID: 20016
			WorldPositionGenerator,
			// Token: 0x04004E31 RID: 20017
			DungeonPoint
		}
	}

	// Token: 0x02000D35 RID: 3381
	[Serializable]
	public class MissionEntityEntry
	{
		// Token: 0x0400457D RID: 17789
		public GameObjectRef entityRef;

		// Token: 0x0400457E RID: 17790
		public string spawnPositionToUse;

		// Token: 0x0400457F RID: 17791
		public bool cleanupOnMissionFailed;

		// Token: 0x04004580 RID: 17792
		public bool cleanupOnMissionSuccess;

		// Token: 0x04004581 RID: 17793
		public string entityIdentifier;
	}

	// Token: 0x02000D36 RID: 3382
	public enum MissionFailReason
	{
		// Token: 0x04004583 RID: 17795
		TimeOut,
		// Token: 0x04004584 RID: 17796
		Disconnect,
		// Token: 0x04004585 RID: 17797
		ResetPlayerState,
		// Token: 0x04004586 RID: 17798
		Abandon
	}
}
