using System;
using ConVar;
using Facepunch.Rust;
using Network;
using UnityEngine;

// Token: 0x020000DF RID: 223
public class TreeEntity : ResourceEntity, IPrefabPreProcess
{
	// Token: 0x0600137B RID: 4987 RVA: 0x00099A94 File Offset: 0x00097C94
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("TreeEntity.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600137C RID: 4988 RVA: 0x00099AD4 File Offset: 0x00097CD4
	public override void ResetState()
	{
		base.ResetState();
	}

	// Token: 0x0600137D RID: 4989 RVA: 0x000062DD File Offset: 0x000044DD
	public override float BoundsPadding()
	{
		return 1f;
	}

	// Token: 0x0600137E RID: 4990 RVA: 0x00099ADC File Offset: 0x00097CDC
	public override void ServerInit()
	{
		base.ServerInit();
		this.lastDirection = (float)((UnityEngine.Random.Range(0, 2) == 0) ? -1 : 1);
		TreeManager.OnTreeSpawned(this);
	}

	// Token: 0x0600137F RID: 4991 RVA: 0x00099AFE File Offset: 0x00097CFE
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		this.CleanupMarker();
		TreeManager.OnTreeDestroyed(this);
	}

	// Token: 0x06001380 RID: 4992 RVA: 0x00099B14 File Offset: 0x00097D14
	public bool DidHitMarker(HitInfo info)
	{
		if (this.xMarker == null)
		{
			return false;
		}
		if (PrefabAttribute.server.Find<TreeMarkerData>(this.prefabID) != null)
		{
			Bounds bounds = new Bounds(this.xMarker.transform.position, Vector3.one * 0.2f);
			if (bounds.Contains(info.HitPositionWorld))
			{
				return true;
			}
		}
		else
		{
			Vector3 lhs = Vector3Ex.Direction2D(base.transform.position, this.xMarker.transform.position);
			Vector3 attackNormal = info.attackNormal;
			float num = Vector3.Dot(lhs, attackNormal);
			float num2 = Vector3.Distance(this.xMarker.transform.position, info.HitPositionWorld);
			if (num >= 0.3f && num2 <= 0.2f)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001381 RID: 4993 RVA: 0x00099BDB File Offset: 0x00097DDB
	public void StartBonusGame()
	{
		if (base.IsInvoking(new Action(this.StopBonusGame)))
		{
			base.CancelInvoke(new Action(this.StopBonusGame));
		}
		base.Invoke(new Action(this.StopBonusGame), 60f);
	}

	// Token: 0x06001382 RID: 4994 RVA: 0x00099C1A File Offset: 0x00097E1A
	public void StopBonusGame()
	{
		this.CleanupMarker();
		this.lastHitTime = 0f;
		this.currentBonusLevel = 0;
	}

	// Token: 0x06001383 RID: 4995 RVA: 0x00099C34 File Offset: 0x00097E34
	public bool BonusActive()
	{
		return this.xMarker != null;
	}

	// Token: 0x06001384 RID: 4996 RVA: 0x00099C44 File Offset: 0x00097E44
	private void DoBirds()
	{
		if (base.isClient)
		{
			return;
		}
		if (UnityEngine.Time.realtimeSinceStartup < this.nextBirdTime)
		{
			return;
		}
		if (this.bounds.extents.y < 6f)
		{
			return;
		}
		uint num = this.net.ID + this.birdCycleIndex;
		if (SeedRandom.Range(ref num, 0, 2) == 0)
		{
			Effect.server.Run("assets/prefabs/npc/birds/birdemission.prefab", base.transform.position + Vector3.up * UnityEngine.Random.Range(this.bounds.extents.y * 0.65f, this.bounds.extents.y * 0.9f), Vector3.up, null, false);
		}
		this.birdCycleIndex += 1U;
		this.nextBirdTime = UnityEngine.Time.realtimeSinceStartup + 90f;
	}

	// Token: 0x06001385 RID: 4997 RVA: 0x00099D1C File Offset: 0x00097F1C
	public override void OnAttacked(HitInfo info)
	{
		bool canGather = info.CanGather;
		float num = UnityEngine.Time.time - this.lastHitTime;
		this.lastHitTime = UnityEngine.Time.time;
		this.DoBirds();
		if (!this.hasBonusGame || !canGather || info.Initiator == null || (this.BonusActive() && !this.DidHitMarker(info)))
		{
			base.OnAttacked(info);
			return;
		}
		if (this.xMarker != null && !info.DidGather && info.gatherScale > 0f)
		{
			this.xMarker.ClientRPC<int>(null, "MarkerHit", this.currentBonusLevel);
			this.currentBonusLevel++;
			info.gatherScale = 1f + Mathf.Clamp((float)this.currentBonusLevel * 0.125f, 0f, 1f);
		}
		Vector3 vector = (this.xMarker != null) ? this.xMarker.transform.position : info.HitPositionWorld;
		this.CleanupMarker();
		TreeMarkerData treeMarkerData = PrefabAttribute.server.Find<TreeMarkerData>(this.prefabID);
		if (treeMarkerData != null)
		{
			Vector3 direction;
			Vector3 vector2 = treeMarkerData.GetNearbyPoint(base.transform.InverseTransformPoint(vector), ref this.lastHitMarkerIndex, out direction);
			vector2 = base.transform.TransformPoint(vector2);
			Quaternion rot = QuaternionEx.LookRotationNormal(base.transform.TransformDirection(direction), default(Vector3));
			this.xMarker = GameManager.server.CreateEntity("assets/content/nature/treesprefabs/trees/effects/tree_marking_nospherecast.prefab", vector2, rot, true);
		}
		else
		{
			Vector3 vector3 = Vector3Ex.Direction2D(base.transform.position, vector);
			Vector3 a = Vector3.Cross(vector3, Vector3.up);
			float d = this.lastDirection;
			float t = UnityEngine.Random.Range(0.5f, 0.5f);
			Vector3 vector4 = Vector3.Lerp(-vector3, a * d, t);
			Vector3 vector5 = base.transform.InverseTransformDirection(vector4.normalized) * 2.5f;
			vector5 = base.transform.InverseTransformPoint(this.GetCollider().ClosestPoint(base.transform.TransformPoint(vector5)));
			Vector3 aimFrom = base.transform.TransformPoint(vector5);
			Vector3 vector6 = base.transform.InverseTransformPoint(info.HitPositionWorld);
			vector5.y = vector6.y;
			Vector3 vector7 = base.transform.InverseTransformPoint(info.Initiator.CenterPoint());
			float min = Mathf.Max(0.75f, vector7.y);
			float max = vector7.y + 0.5f;
			vector5.y = Mathf.Clamp(vector5.y + UnityEngine.Random.Range(0.1f, 0.2f) * ((UnityEngine.Random.Range(0, 2) == 0) ? -1f : 1f), min, max);
			Vector3 vector8 = Vector3Ex.Direction2D(base.transform.position, aimFrom);
			Vector3 a2 = vector8;
			vector8 = base.transform.InverseTransformDirection(vector8);
			Quaternion rot2 = QuaternionEx.LookRotationNormal(-vector8, Vector3.zero);
			vector5 = base.transform.TransformPoint(vector5);
			rot2 = QuaternionEx.LookRotationNormal(-a2, Vector3.zero);
			vector5 = this.GetCollider().ClosestPoint(vector5);
			Line line = new Line(this.GetCollider().transform.TransformPoint(new Vector3(0f, 10f, 0f)), this.GetCollider().transform.TransformPoint(new Vector3(0f, -10f, 0f)));
			rot2 = QuaternionEx.LookRotationNormal(-Vector3Ex.Direction(line.ClosestPoint(vector5), vector5), default(Vector3));
			this.xMarker = GameManager.server.CreateEntity("assets/content/nature/treesprefabs/trees/effects/tree_marking.prefab", vector5, rot2, true);
		}
		this.xMarker.Spawn();
		if (num > 5f)
		{
			this.StartBonusGame();
		}
		base.OnAttacked(info);
		if (this.health > 0f)
		{
			this.lastAttackDamage = info.damageTypes.Total();
			int num2 = Mathf.CeilToInt(this.health / this.lastAttackDamage);
			if (num2 < 2)
			{
				base.ClientRPC<int>(null, "CrackSound", 1);
				return;
			}
			if (num2 < 5)
			{
				base.ClientRPC<int>(null, "CrackSound", 0);
			}
		}
	}

	// Token: 0x06001386 RID: 4998 RVA: 0x0009A144 File Offset: 0x00098344
	public void CleanupMarker()
	{
		if (this.xMarker)
		{
			this.xMarker.Kill(BaseNetworkable.DestroyMode.None);
		}
		this.xMarker = null;
	}

	// Token: 0x06001387 RID: 4999 RVA: 0x0009A168 File Offset: 0x00098368
	public Collider GetCollider()
	{
		if (base.isServer)
		{
			if (!(this.serverCollider == null))
			{
				return this.serverCollider;
			}
			return base.GetComponentInChildren<CapsuleCollider>();
		}
		else
		{
			if (!(this.clientCollider == null))
			{
				return this.clientCollider;
			}
			return base.GetComponent<Collider>();
		}
	}

	// Token: 0x06001388 RID: 5000 RVA: 0x0009A1B4 File Offset: 0x000983B4
	public override void OnKilled(HitInfo info)
	{
		if (this.isKilled)
		{
			return;
		}
		this.isKilled = true;
		this.CleanupMarker();
		Analytics.Server.TreeKilled(info.WeaponPrefab);
		if (this.fallOnKilled)
		{
			Collider collider = this.GetCollider();
			if (collider)
			{
				collider.enabled = false;
			}
			base.ClientRPC<Vector3>(null, "TreeFall", info.attackNormal);
			base.Invoke(new Action(this.DelayedKill), this.fallDuration + 1f);
			return;
		}
		this.DelayedKill();
	}

	// Token: 0x06001389 RID: 5001 RVA: 0x000029D4 File Offset: 0x00000BD4
	public void DelayedKill()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x0600138A RID: 5002 RVA: 0x0009A237 File Offset: 0x00098437
	public override void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(preProcess, rootObj, name, serverside, clientside, bundling);
		if (serverside)
		{
			this.globalBroadcast = ConVar.Tree.global_broadcast;
		}
	}

	// Token: 0x04000C3B RID: 3131
	[Header("Falling")]
	public bool fallOnKilled = true;

	// Token: 0x04000C3C RID: 3132
	public float fallDuration = 1.5f;

	// Token: 0x04000C3D RID: 3133
	public GameObjectRef fallStartSound;

	// Token: 0x04000C3E RID: 3134
	public GameObjectRef fallImpactSound;

	// Token: 0x04000C3F RID: 3135
	public GameObjectRef fallImpactParticles;

	// Token: 0x04000C40 RID: 3136
	public SoundDefinition fallLeavesLoopDef;

	// Token: 0x04000C41 RID: 3137
	[NonSerialized]
	public bool[] usedHeights = new bool[20];

	// Token: 0x04000C42 RID: 3138
	public bool impactSoundPlayed;

	// Token: 0x04000C43 RID: 3139
	private float treeDistanceUponFalling;

	// Token: 0x04000C44 RID: 3140
	public GameObjectRef prefab;

	// Token: 0x04000C45 RID: 3141
	public bool hasBonusGame = true;

	// Token: 0x04000C46 RID: 3142
	public GameObjectRef bonusHitEffect;

	// Token: 0x04000C47 RID: 3143
	public GameObjectRef bonusHitSound;

	// Token: 0x04000C48 RID: 3144
	public Collider serverCollider;

	// Token: 0x04000C49 RID: 3145
	public Collider clientCollider;

	// Token: 0x04000C4A RID: 3146
	public SoundDefinition smallCrackSoundDef;

	// Token: 0x04000C4B RID: 3147
	public SoundDefinition medCrackSoundDef;

	// Token: 0x04000C4C RID: 3148
	private float lastAttackDamage;

	// Token: 0x04000C4D RID: 3149
	[NonSerialized]
	protected BaseEntity xMarker;

	// Token: 0x04000C4E RID: 3150
	private int currentBonusLevel;

	// Token: 0x04000C4F RID: 3151
	private float lastDirection = -1f;

	// Token: 0x04000C50 RID: 3152
	private float lastHitTime;

	// Token: 0x04000C51 RID: 3153
	private int lastHitMarkerIndex = -1;

	// Token: 0x04000C52 RID: 3154
	private float nextBirdTime;

	// Token: 0x04000C53 RID: 3155
	private uint birdCycleIndex;
}
