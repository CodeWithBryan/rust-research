using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x02000189 RID: 393
public class HalloweenDungeon : BasePortal
{
	// Token: 0x0600170E RID: 5902 RVA: 0x000AD3AD File Offset: 0x000AB5AD
	public virtual float GetLifetime()
	{
		return HalloweenDungeon.lifetime;
	}

	// Token: 0x0600170F RID: 5903 RVA: 0x000AD3B4 File Offset: 0x000AB5B4
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk && info.msg.ioEntity != null)
		{
			this.dungeonInstance.uid = info.msg.ioEntity.genericEntRef3;
			this.secondsUsed = info.msg.ioEntity.genericFloat1;
			this.timeAlive = info.msg.ioEntity.genericFloat2;
		}
	}

	// Token: 0x06001710 RID: 5904 RVA: 0x000AD424 File Offset: 0x000AB624
	public float GetLifeFraction()
	{
		return Mathf.Clamp01(this.secondsUsed / this.GetLifetime());
	}

	// Token: 0x06001711 RID: 5905 RVA: 0x000AD438 File Offset: 0x000AB638
	public void Update()
	{
		if (base.isClient)
		{
			return;
		}
		if (this.secondsUsed > 0f)
		{
			this.secondsUsed += Time.deltaTime;
		}
		this.timeAlive += Time.deltaTime;
		float lifeFraction = this.GetLifeFraction();
		if (this.dungeonInstance.IsValid(true))
		{
			ProceduralDynamicDungeon proceduralDynamicDungeon = this.dungeonInstance.Get(true);
			float value = this.radiationCurve.Evaluate(lifeFraction) * 80f;
			proceduralDynamicDungeon.exitRadiation.RadiationAmountOverride = Mathf.Clamp(value, 0f, float.PositiveInfinity);
		}
		if (lifeFraction >= 1f)
		{
			this.KillIfNoPlayers();
			return;
		}
		if (this.timeAlive > 3600f && this.secondsUsed == 0f)
		{
			this.ClearAllEntitiesInRadius(80f);
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x06001712 RID: 5906 RVA: 0x000AD508 File Offset: 0x000AB708
	public void KillIfNoPlayers()
	{
		if (!this.AnyPlayersInside())
		{
			this.ClearAllEntitiesInRadius(80f);
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x06001713 RID: 5907 RVA: 0x000AD524 File Offset: 0x000AB724
	public bool AnyPlayersInside()
	{
		ProceduralDynamicDungeon proceduralDynamicDungeon = this.dungeonInstance.Get(true);
		if (proceduralDynamicDungeon == null)
		{
			this.anyplayers_cached = false;
		}
		else if (Time.time > this.nextPlayerCheckTime)
		{
			this.nextPlayerCheckTime = Time.time + 10f;
			this.anyplayers_cached = global::BaseNetworkable.HasCloseConnections(proceduralDynamicDungeon.transform.position, 80f);
		}
		return this.anyplayers_cached;
	}

	// Token: 0x06001714 RID: 5908 RVA: 0x000AD590 File Offset: 0x000AB790
	private void ClearAllEntitiesInRadius(float radius)
	{
		ProceduralDynamicDungeon proceduralDynamicDungeon = this.dungeonInstance.Get(true);
		if (proceduralDynamicDungeon == null)
		{
			return;
		}
		List<global::BaseEntity> list = Pool.GetList<global::BaseEntity>();
		Vis.Entities<global::BaseEntity>(proceduralDynamicDungeon.transform.position, radius, list, -1, QueryTriggerInteraction.Collide);
		foreach (global::BaseEntity baseEntity in list)
		{
			if (baseEntity.IsValid() && !baseEntity.IsDestroyed)
			{
				global::LootableCorpse lootableCorpse;
				if ((lootableCorpse = (baseEntity as global::LootableCorpse)) != null)
				{
					lootableCorpse.blockBagDrop = true;
				}
				baseEntity.Kill(global::BaseNetworkable.DestroyMode.None);
			}
		}
		Pool.FreeList<global::BaseEntity>(ref list);
	}

	// Token: 0x06001715 RID: 5909 RVA: 0x000AD63C File Offset: 0x000AB83C
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.ioEntity == null)
		{
			info.msg.ioEntity = Pool.Get<ProtoBuf.IOEntity>();
		}
		info.msg.ioEntity.genericEntRef3 = this.dungeonInstance.uid;
		info.msg.ioEntity.genericFloat1 = this.secondsUsed;
		info.msg.ioEntity.genericFloat2 = this.timeAlive;
	}

	// Token: 0x06001716 RID: 5910 RVA: 0x000AD6B4 File Offset: 0x000AB8B4
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.timeAlive += UnityEngine.Random.Range(0f, 60f);
	}

	// Token: 0x06001717 RID: 5911 RVA: 0x000AD6D8 File Offset: 0x000AB8D8
	public override void UsePortal(global::BasePlayer player)
	{
		if (this.GetLifeFraction() > 0.8f)
		{
			player.ShowToast(GameTip.Styles.Blue_Normal, this.collapsePhrase, Array.Empty<string>());
			return;
		}
		if (player.isMounted)
		{
			player.ShowToast(GameTip.Styles.Blue_Normal, this.mountPhrase, Array.Empty<string>());
			return;
		}
		if (this.secondsUsed == 0f)
		{
			this.secondsUsed = 1f;
		}
		base.UsePortal(player);
	}

	// Token: 0x06001718 RID: 5912 RVA: 0x0005F292 File Offset: 0x0005D492
	public override void Spawn()
	{
		base.Spawn();
	}

	// Token: 0x06001719 RID: 5913 RVA: 0x000AD740 File Offset: 0x000AB940
	public override void ServerInit()
	{
		base.ServerInit();
		if (!Rust.Application.isLoadingSave)
		{
			this.timeAlive = UnityEngine.Random.Range(0f, 60f);
			this.SpawnSubEntities();
		}
		this.localEntryExitPos.DropToGround(false, 10f);
		this.localEntryExitPos.transform.position += Vector3.up * 0.05f;
		base.Invoke(new Action(this.CheckBlocked), 0.25f);
	}

	// Token: 0x0600171A RID: 5914 RVA: 0x000AD7C8 File Offset: 0x000AB9C8
	public void CheckBlocked()
	{
		float num = 0.5f;
		float num2 = 1.8f;
		Vector3 position = this.localEntryExitPos.position;
		Vector3 start = position + new Vector3(0f, num, 0f);
		Vector3 end = position + new Vector3(0f, num2 - num, 0f);
		if (Physics.CheckCapsule(start, end, num, 1537286401))
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x0600171B RID: 5915 RVA: 0x000AD834 File Offset: 0x000ABA34
	public static Vector3 GetDungeonSpawnPoint()
	{
		float num = Mathf.Floor(TerrainMeta.Size.x / 200f);
		float num2 = 1000f;
		Vector3 zero = Vector3.zero;
		zero.x = -Mathf.Min(TerrainMeta.Size.x * 0.5f, 4000f) + 200f;
		zero.y = 1025f;
		zero.z = -Mathf.Min(TerrainMeta.Size.z * 0.5f, 4000f) + 200f;
		Vector3 zero2 = Vector3.zero;
		int num3 = 0;
		while ((float)num3 < num2)
		{
			int num4 = 0;
			while ((float)num4 < num)
			{
				Vector3 vector = zero + new Vector3((float)num4 * 200f, (float)num3 * 100f, 0f);
				bool flag = false;
				foreach (ProceduralDynamicDungeon proceduralDynamicDungeon in ProceduralDynamicDungeon.dungeons)
				{
					if (proceduralDynamicDungeon != null && proceduralDynamicDungeon.isServer && Vector3.Distance(proceduralDynamicDungeon.transform.position, vector) < 10f)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return vector;
				}
				num4++;
			}
			num3++;
		}
		return Vector3.zero;
	}

	// Token: 0x0600171C RID: 5916 RVA: 0x000AD994 File Offset: 0x000ABB94
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (this.dungeonInstance.IsValid(true))
		{
			this.dungeonInstance.Get(true).Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x0600171D RID: 5917 RVA: 0x000029D4 File Offset: 0x00000BD4
	public void DelayedDestroy()
	{
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x0600171E RID: 5918 RVA: 0x000AD9BC File Offset: 0x000ABBBC
	public void SpawnSubEntities()
	{
		Vector3 dungeonSpawnPoint = HalloweenDungeon.GetDungeonSpawnPoint();
		if (dungeonSpawnPoint == Vector3.zero)
		{
			Debug.LogError("No dungeon spawn point");
			base.Invoke(new Action(this.DelayedDestroy), 5f);
			return;
		}
		global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.dungeonPrefab.resourcePath, dungeonSpawnPoint, Quaternion.identity, true);
		ProceduralDynamicDungeon component = baseEntity.GetComponent<ProceduralDynamicDungeon>();
		component.mapOffset = base.transform.position - dungeonSpawnPoint;
		baseEntity.Spawn();
		this.dungeonInstance.Set(component);
		BasePortal exitPortal = component.GetExitPortal();
		this.targetPortal = exitPortal;
		exitPortal.targetPortal = this;
		base.LinkPortal();
		exitPortal.LinkPortal();
	}

	// Token: 0x0400102A RID: 4138
	public GameObjectRef dungeonPrefab;

	// Token: 0x0400102B RID: 4139
	public EntityRef<ProceduralDynamicDungeon> dungeonInstance;

	// Token: 0x0400102C RID: 4140
	[ServerVar(Help = "Population active on the server", ShowInAdminUI = true)]
	public static float population = 0f;

	// Token: 0x0400102D RID: 4141
	[ServerVar(Help = "How long each active dungeon should last before dying", ShowInAdminUI = true)]
	public static float lifetime = 600f;

	// Token: 0x0400102E RID: 4142
	private float secondsUsed;

	// Token: 0x0400102F RID: 4143
	private float timeAlive;

	// Token: 0x04001030 RID: 4144
	public AnimationCurve radiationCurve;

	// Token: 0x04001031 RID: 4145
	public Translate.Phrase collapsePhrase;

	// Token: 0x04001032 RID: 4146
	public Translate.Phrase mountPhrase;

	// Token: 0x04001033 RID: 4147
	private bool anyplayers_cached;

	// Token: 0x04001034 RID: 4148
	private float nextPlayerCheckTime = float.NegativeInfinity;
}
