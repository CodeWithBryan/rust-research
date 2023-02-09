using System;
using System.Collections.Generic;
using ConVar;
using Rust.AI;
using UnityEngine;

// Token: 0x0200033D RID: 829
public class AIBrainSenses
{
	// Token: 0x17000236 RID: 566
	// (get) Token: 0x06001E1F RID: 7711 RVA: 0x000CC6D5 File Offset: 0x000CA8D5
	public float TimeSinceThreat
	{
		get
		{
			return UnityEngine.Time.realtimeSinceStartup - this.LastThreatTimestamp;
		}
	}

	// Token: 0x17000237 RID: 567
	// (get) Token: 0x06001E20 RID: 7712 RVA: 0x000CC6E3 File Offset: 0x000CA8E3
	// (set) Token: 0x06001E21 RID: 7713 RVA: 0x000CC6EB File Offset: 0x000CA8EB
	public SimpleAIMemory Memory { get; private set; } = new SimpleAIMemory();

	// Token: 0x17000238 RID: 568
	// (get) Token: 0x06001E22 RID: 7714 RVA: 0x000CC6F4 File Offset: 0x000CA8F4
	public float TargetLostRange
	{
		get
		{
			return this.targetLostRange;
		}
	}

	// Token: 0x17000239 RID: 569
	// (get) Token: 0x06001E23 RID: 7715 RVA: 0x000CC6FC File Offset: 0x000CA8FC
	// (set) Token: 0x06001E24 RID: 7716 RVA: 0x000CC704 File Offset: 0x000CA904
	public bool ignoreSafeZonePlayers { get; private set; }

	// Token: 0x06001E25 RID: 7717 RVA: 0x000CC710 File Offset: 0x000CA910
	public void Init(BaseEntity owner, BaseAIBrain brain, float memoryDuration, float range, float targetLostRange, float visionCone, bool checkVision, bool checkLOS, bool ignoreNonVisionSneakers, float listenRange, bool hostileTargetsOnly, bool senseFriendlies, bool ignoreSafeZonePlayers, EntityType senseTypes, bool refreshKnownLOS)
	{
		this.owner = owner;
		this.brain = brain;
		this.MemoryDuration = memoryDuration;
		this.ownerAttack = (owner as IAIAttack);
		this.playerOwner = (owner as BasePlayer);
		this.maxRange = range;
		this.targetLostRange = targetLostRange;
		this.visionCone = visionCone;
		this.checkVision = checkVision;
		this.checkLOS = checkLOS;
		this.ignoreNonVisionSneakers = ignoreNonVisionSneakers;
		this.listenRange = listenRange;
		this.hostileTargetsOnly = hostileTargetsOnly;
		this.senseFriendlies = senseFriendlies;
		this.ignoreSafeZonePlayers = ignoreSafeZonePlayers;
		this.senseTypes = senseTypes;
		this.LastThreatTimestamp = UnityEngine.Time.realtimeSinceStartup;
		this.refreshKnownLOS = refreshKnownLOS;
		this.ownerSenses = (owner as IAISenses);
		this.knownPlayersLOSUpdateInterval = ((owner is HumanNPC) ? AIBrainSenses.HumanKnownPlayersLOSUpdateInterval : AIBrainSenses.KnownPlayersLOSUpdateInterval);
	}

	// Token: 0x06001E26 RID: 7718 RVA: 0x000CC7DB File Offset: 0x000CA9DB
	public void Update()
	{
		if (this.owner == null)
		{
			return;
		}
		this.UpdateSenses();
		this.UpdateKnownPlayersLOS();
	}

	// Token: 0x06001E27 RID: 7719 RVA: 0x000CC7F8 File Offset: 0x000CA9F8
	private void UpdateSenses()
	{
		if (UnityEngine.Time.time < this.nextUpdateTime)
		{
			return;
		}
		this.nextUpdateTime = UnityEngine.Time.time + AIBrainSenses.UpdateInterval;
		if (this.senseTypes != (EntityType)0)
		{
			if (this.senseTypes == EntityType.Player)
			{
				this.SensePlayers();
			}
			else
			{
				this.SenseBrains();
				if (this.senseTypes.HasFlag(EntityType.Player))
				{
					this.SensePlayers();
				}
			}
		}
		this.Memory.Forget(this.MemoryDuration);
	}

	// Token: 0x06001E28 RID: 7720 RVA: 0x000CC874 File Offset: 0x000CAA74
	public void UpdateKnownPlayersLOS()
	{
		if (UnityEngine.Time.time < this.nextKnownPlayersLOSUpdateTime)
		{
			return;
		}
		this.nextKnownPlayersLOSUpdateTime = UnityEngine.Time.time + this.knownPlayersLOSUpdateInterval;
		foreach (BaseEntity baseEntity in this.Memory.Players)
		{
			if (!(baseEntity == null) && !baseEntity.IsNpc)
			{
				bool flag = this.ownerAttack.CanSeeTarget(baseEntity);
				this.Memory.SetLOS(baseEntity, flag);
				if (this.refreshKnownLOS && this.owner != null && flag && Vector3.Distance(baseEntity.transform.position, this.owner.transform.position) <= this.TargetLostRange)
				{
					this.Memory.SetKnown(baseEntity, this.owner, this);
				}
			}
		}
	}

	// Token: 0x06001E29 RID: 7721 RVA: 0x000CC96C File Offset: 0x000CAB6C
	private void SensePlayers()
	{
		int playersInSphere = BaseEntity.Query.Server.GetPlayersInSphere(this.owner.transform.position, this.maxRange, AIBrainSenses.playerQueryResults, new Func<BasePlayer, bool>(this.AiCaresAbout));
		for (int i = 0; i < playersInSphere; i++)
		{
			BasePlayer ent = AIBrainSenses.playerQueryResults[i];
			this.Memory.SetKnown(ent, this.owner, this);
		}
	}

	// Token: 0x06001E2A RID: 7722 RVA: 0x000CC9D4 File Offset: 0x000CABD4
	private void SenseBrains()
	{
		int brainsInSphere = BaseEntity.Query.Server.GetBrainsInSphere(this.owner.transform.position, this.maxRange, AIBrainSenses.queryResults, new Func<BaseEntity, bool>(this.AiCaresAbout));
		for (int i = 0; i < brainsInSphere; i++)
		{
			BaseEntity ent = AIBrainSenses.queryResults[i];
			this.Memory.SetKnown(ent, this.owner, this);
		}
	}

	// Token: 0x06001E2B RID: 7723 RVA: 0x000CCA3C File Offset: 0x000CAC3C
	private bool AiCaresAbout(BaseEntity entity)
	{
		if (entity == null)
		{
			return false;
		}
		if (!entity.isServer)
		{
			return false;
		}
		if (entity.EqualNetID(this.owner))
		{
			return false;
		}
		if (entity.Health() <= 0f)
		{
			return false;
		}
		if (!this.IsValidSenseType(entity))
		{
			return false;
		}
		BaseCombatEntity baseCombatEntity = entity as BaseCombatEntity;
		BasePlayer basePlayer = entity as BasePlayer;
		if (basePlayer != null && basePlayer.IsDead())
		{
			return false;
		}
		if (this.ignoreSafeZonePlayers && basePlayer != null && basePlayer.InSafeZone())
		{
			return false;
		}
		if (this.listenRange > 0f && baseCombatEntity != null && baseCombatEntity.TimeSinceLastNoise <= 1f && baseCombatEntity.CanLastNoiseBeHeard(this.owner.transform.position, this.listenRange))
		{
			return true;
		}
		if (this.senseFriendlies && this.ownerSenses != null && this.ownerSenses.IsFriendly(entity))
		{
			return true;
		}
		float num = float.PositiveInfinity;
		if (baseCombatEntity != null && AI.accuratevisiondistance)
		{
			num = Vector3.Distance(this.owner.transform.position, baseCombatEntity.transform.position);
			if (num > this.maxRange)
			{
				return false;
			}
		}
		if (this.checkVision && !this.IsTargetInVision(entity))
		{
			if (!this.ignoreNonVisionSneakers)
			{
				return false;
			}
			if (basePlayer != null && !basePlayer.IsNpc)
			{
				if (!AI.accuratevisiondistance)
				{
					num = Vector3.Distance(this.owner.transform.position, basePlayer.transform.position);
				}
				if ((basePlayer.IsDucked() && num >= this.brain.IgnoreSneakersMaxDistance) || num >= this.brain.IgnoreNonVisionMaxDistance)
				{
					return false;
				}
			}
		}
		if (this.hostileTargetsOnly && baseCombatEntity != null && !baseCombatEntity.IsHostile() && !(baseCombatEntity is ScarecrowNPC))
		{
			return false;
		}
		if (this.checkLOS && this.ownerAttack != null)
		{
			bool flag = this.ownerAttack.CanSeeTarget(entity);
			this.Memory.SetLOS(entity, flag);
			if (!flag)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001E2C RID: 7724 RVA: 0x000CCC38 File Offset: 0x000CAE38
	private bool IsValidSenseType(BaseEntity ent)
	{
		BasePlayer basePlayer = ent as BasePlayer;
		if (basePlayer != null)
		{
			if (basePlayer.IsNpc)
			{
				if (ent is BasePet)
				{
					return true;
				}
				if (ent is ScarecrowNPC)
				{
					return true;
				}
				if (this.senseTypes.HasFlag(EntityType.BasePlayerNPC))
				{
					return true;
				}
			}
			else if (this.senseTypes.HasFlag(EntityType.Player))
			{
				return true;
			}
		}
		return (this.senseTypes.HasFlag(EntityType.NPC) && ent is BaseNpc) || (this.senseTypes.HasFlag(EntityType.WorldItem) && ent is WorldItem) || (this.senseTypes.HasFlag(EntityType.Corpse) && ent is BaseCorpse) || (this.senseTypes.HasFlag(EntityType.TimedExplosive) && ent is TimedExplosive) || (this.senseTypes.HasFlag(EntityType.Chair) && ent is BaseChair);
	}

	// Token: 0x06001E2D RID: 7725 RVA: 0x000CCD54 File Offset: 0x000CAF54
	private bool IsTargetInVision(BaseEntity target)
	{
		Vector3 rhs = Vector3Ex.Direction(target.transform.position, this.owner.transform.position);
		return Vector3.Dot((this.playerOwner != null) ? this.playerOwner.eyes.BodyForward() : this.owner.transform.forward, rhs) >= this.visionCone;
	}

	// Token: 0x06001E2E RID: 7726 RVA: 0x000CCDC3 File Offset: 0x000CAFC3
	public BaseEntity GetNearestPlayer(float rangeFraction)
	{
		return this.GetNearest(this.Memory.Players, rangeFraction);
	}

	// Token: 0x1700023A RID: 570
	// (get) Token: 0x06001E2F RID: 7727 RVA: 0x000CCDD7 File Offset: 0x000CAFD7
	public List<BaseEntity> Players
	{
		get
		{
			return this.Memory.Players;
		}
	}

	// Token: 0x06001E30 RID: 7728 RVA: 0x000CCDE4 File Offset: 0x000CAFE4
	public BaseEntity GetNearestThreat(float rangeFraction)
	{
		return this.GetNearest(this.Memory.Threats, rangeFraction);
	}

	// Token: 0x06001E31 RID: 7729 RVA: 0x000CCDF8 File Offset: 0x000CAFF8
	public BaseEntity GetNearestTarget(float rangeFraction)
	{
		return this.GetNearest(this.Memory.Targets, rangeFraction);
	}

	// Token: 0x06001E32 RID: 7730 RVA: 0x000CCE0C File Offset: 0x000CB00C
	private BaseEntity GetNearest(List<BaseEntity> entities, float rangeFraction)
	{
		if (entities == null || entities.Count == 0)
		{
			return null;
		}
		float positiveInfinity = float.PositiveInfinity;
		BaseEntity result = null;
		foreach (BaseEntity baseEntity in entities)
		{
			if (!(baseEntity == null) && baseEntity.Health() > 0f)
			{
				float num = Vector3.Distance(baseEntity.transform.position, this.owner.transform.position);
				if (num <= rangeFraction * this.maxRange && num < positiveInfinity)
				{
					result = baseEntity;
				}
			}
		}
		return result;
	}

	// Token: 0x040017DC RID: 6108
	[ServerVar]
	public static float UpdateInterval = 0.5f;

	// Token: 0x040017DD RID: 6109
	[ServerVar]
	public static float HumanKnownPlayersLOSUpdateInterval = 0.2f;

	// Token: 0x040017DE RID: 6110
	[ServerVar]
	public static float KnownPlayersLOSUpdateInterval = 0.5f;

	// Token: 0x040017DF RID: 6111
	private float knownPlayersLOSUpdateInterval = 0.2f;

	// Token: 0x040017E0 RID: 6112
	public float MemoryDuration = 10f;

	// Token: 0x040017E1 RID: 6113
	public float LastThreatTimestamp;

	// Token: 0x040017E2 RID: 6114
	public float TimeInAgressiveState;

	// Token: 0x040017E4 RID: 6116
	private static BaseEntity[] queryResults = new BaseEntity[64];

	// Token: 0x040017E5 RID: 6117
	private static BasePlayer[] playerQueryResults = new BasePlayer[64];

	// Token: 0x040017E6 RID: 6118
	private float nextUpdateTime;

	// Token: 0x040017E7 RID: 6119
	private float nextKnownPlayersLOSUpdateTime;

	// Token: 0x040017E8 RID: 6120
	private BaseEntity owner;

	// Token: 0x040017E9 RID: 6121
	private BasePlayer playerOwner;

	// Token: 0x040017EA RID: 6122
	private IAISenses ownerSenses;

	// Token: 0x040017EB RID: 6123
	private float maxRange;

	// Token: 0x040017EC RID: 6124
	private float targetLostRange;

	// Token: 0x040017ED RID: 6125
	private float visionCone;

	// Token: 0x040017EE RID: 6126
	private bool checkVision;

	// Token: 0x040017EF RID: 6127
	private bool checkLOS;

	// Token: 0x040017F0 RID: 6128
	private bool ignoreNonVisionSneakers;

	// Token: 0x040017F1 RID: 6129
	private float listenRange;

	// Token: 0x040017F2 RID: 6130
	private bool hostileTargetsOnly;

	// Token: 0x040017F3 RID: 6131
	private bool senseFriendlies;

	// Token: 0x040017F4 RID: 6132
	private bool refreshKnownLOS;

	// Token: 0x040017F6 RID: 6134
	private EntityType senseTypes;

	// Token: 0x040017F7 RID: 6135
	private IAIAttack ownerAttack;

	// Token: 0x040017F8 RID: 6136
	private BaseAIBrain brain;
}
