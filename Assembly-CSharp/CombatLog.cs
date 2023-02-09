using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using UnityEngine;

// Token: 0x0200042D RID: 1069
public class CombatLog
{
	// Token: 0x170002B9 RID: 697
	// (get) Token: 0x0600234F RID: 9039 RVA: 0x000E0072 File Offset: 0x000DE272
	// (set) Token: 0x06002350 RID: 9040 RVA: 0x000E007A File Offset: 0x000DE27A
	public float LastActive { get; private set; }

	// Token: 0x06002351 RID: 9041 RVA: 0x000E0083 File Offset: 0x000DE283
	public CombatLog(BasePlayer player)
	{
		this.player = player;
	}

	// Token: 0x06002352 RID: 9042 RVA: 0x000E0092 File Offset: 0x000DE292
	public void Init()
	{
		this.storage = CombatLog.Get(this.player.userID);
		this.LastActive = this.storage.LastOrDefault<CombatLog.Event>().time;
	}

	// Token: 0x06002353 RID: 9043 RVA: 0x000059DD File Offset: 0x00003BDD
	public void Save()
	{
	}

	// Token: 0x06002354 RID: 9044 RVA: 0x000E00C0 File Offset: 0x000DE2C0
	public void LogInvalid(BasePlayer player, AttackEntity weapon, string description)
	{
		this.Log(player, weapon, null, description, null, -1, -1f, null);
	}

	// Token: 0x06002355 RID: 9045 RVA: 0x000E00E0 File Offset: 0x000DE2E0
	public void LogInvalid(HitInfo info, string description)
	{
		this.Log(info.Initiator, info.Weapon, info.HitEntity as BaseCombatEntity, description, info.ProjectilePrefab, info.ProjectileID, -1f, info);
	}

	// Token: 0x06002356 RID: 9046 RVA: 0x000E0120 File Offset: 0x000DE320
	public void LogAttack(HitInfo info, string description, float oldHealth = -1f)
	{
		this.Log(info.Initiator, info.Weapon, info.HitEntity as BaseCombatEntity, description, info.ProjectilePrefab, info.ProjectileID, oldHealth, info);
	}

	// Token: 0x06002357 RID: 9047 RVA: 0x000E015C File Offset: 0x000DE35C
	public void Log(BaseEntity attacker, AttackEntity weapon, BaseCombatEntity hitEntity, string description, Projectile projectilePrefab = null, int projectileId = -1, float healthOld = -1f, HitInfo hitInfo = null)
	{
		CombatLog.Event val = default(CombatLog.Event);
		float distance = 0f;
		if (hitInfo != null)
		{
			distance = (hitInfo.IsProjectile() ? hitInfo.ProjectileDistance : Vector3.Distance(hitInfo.PointStart, hitInfo.HitPositionWorld));
			BasePlayer basePlayer;
			if ((basePlayer = (hitInfo.Initiator as BasePlayer)) != null && hitInfo.HitEntity != hitInfo.Initiator)
			{
				val.attacker_dead = (basePlayer.IsDead() || basePlayer.IsWounded());
			}
		}
		float health_new = (hitEntity != null) ? hitEntity.Health() : 0f;
		val.time = UnityEngine.Time.realtimeSinceStartup;
		val.attacker_id = ((attacker != null && attacker.net != null) ? attacker.net.ID : 0U);
		val.target_id = ((hitEntity != null && hitEntity.net != null) ? hitEntity.net.ID : 0U);
		val.attacker = ((this.player == attacker) ? "you" : (((attacker != null) ? attacker.ShortPrefabName : null) ?? "N/A"));
		val.target = ((this.player == hitEntity) ? "you" : (((hitEntity != null) ? hitEntity.ShortPrefabName : null) ?? "N/A"));
		val.weapon = ((weapon != null) ? weapon.name : "N/A");
		val.ammo = ((projectilePrefab != null) ? ((projectilePrefab != null) ? projectilePrefab.name : null) : "N/A");
		val.bone = (((hitInfo != null) ? hitInfo.boneName : null) ?? "N/A");
		val.area = ((hitInfo != null) ? hitInfo.boneArea : ((HitArea)0));
		val.distance = distance;
		val.health_old = ((healthOld == -1f) ? 0f : healthOld);
		val.health_new = health_new;
		val.info = (description ?? string.Empty);
		val.proj_hits = ((hitInfo != null) ? hitInfo.ProjectileHits : 0);
		val.proj_integrity = ((hitInfo != null) ? hitInfo.ProjectileIntegrity : 0f);
		val.proj_travel = ((hitInfo != null) ? hitInfo.ProjectileTravelTime : 0f);
		val.proj_mismatch = ((hitInfo != null) ? hitInfo.ProjectileTrajectoryMismatch : 0f);
		BasePlayer basePlayer2 = attacker as BasePlayer;
		BasePlayer.FiredProjectile firedProjectile;
		if (basePlayer2 != null && projectilePrefab != null && basePlayer2.firedProjectiles.TryGetValue(projectileId, out firedProjectile))
		{
			val.desync = (int)(firedProjectile.desyncLifeTime * 1000f);
		}
		this.Log(val);
	}

	// Token: 0x06002358 RID: 9048 RVA: 0x000E040C File Offset: 0x000DE60C
	private void Log(CombatLog.Event val)
	{
		this.LastActive = UnityEngine.Time.realtimeSinceStartup;
		if (this.storage == null)
		{
			return;
		}
		this.storage.Enqueue(val);
		int num = Mathf.Max(0, Server.combatlogsize);
		while (this.storage.Count > num)
		{
			this.storage.Dequeue();
		}
	}

	// Token: 0x06002359 RID: 9049 RVA: 0x000E0464 File Offset: 0x000DE664
	public string Get(int count, uint filterByAttacker = 0U, bool json = false, bool isAdmin = false, ulong requestingUser = 0UL)
	{
		if (this.storage == null)
		{
			return string.Empty;
		}
		if (this.storage.Count == 0 && !json)
		{
			return "Combat log empty.";
		}
		TextTable textTable = new TextTable();
		textTable.AddColumn("time");
		textTable.AddColumn("attacker");
		textTable.AddColumn("id");
		textTable.AddColumn("target");
		textTable.AddColumn("id");
		textTable.AddColumn("weapon");
		textTable.AddColumn("ammo");
		textTable.AddColumn("area");
		textTable.AddColumn("distance");
		textTable.AddColumn("old_hp");
		textTable.AddColumn("new_hp");
		textTable.AddColumn("info");
		textTable.AddColumn("hits");
		textTable.AddColumn("integrity");
		textTable.AddColumn("travel");
		textTable.AddColumn("mismatch");
		textTable.AddColumn("desync");
		int num = this.storage.Count - count;
		int combatlogdelay = Server.combatlogdelay;
		int num2 = 0;
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		foreach (CombatLog.Event @event in this.storage)
		{
			if (num > 0)
			{
				num--;
			}
			else if ((filterByAttacker == 0U || @event.attacker_id == filterByAttacker) && (!(activeGameMode != null) || activeGameMode.returnValidCombatlog || isAdmin || @event.proj_hits <= 0))
			{
				float num3 = UnityEngine.Time.realtimeSinceStartup - @event.time;
				if (num3 >= (float)combatlogdelay)
				{
					string text = num3.ToString("0.00s");
					string attacker = @event.attacker;
					uint num4 = @event.attacker_id;
					string text2 = num4.ToString();
					string target = @event.target;
					num4 = @event.target_id;
					string text3 = num4.ToString();
					string weapon = @event.weapon;
					string ammo = @event.ammo;
					string text4 = HitAreaUtil.Format(@event.area).ToLower();
					float num5 = @event.distance;
					string text5 = num5.ToString("0.0m");
					num5 = @event.health_old;
					string text6 = num5.ToString("0.0");
					num5 = @event.health_new;
					string text7 = num5.ToString("0.0");
					string text8 = @event.info;
					if (!this.player.IsDestroyed && this.player.userID == requestingUser && @event.attacker_dead)
					{
						text8 = "you died first (" + text8 + ")";
					}
					int num6 = @event.proj_hits;
					string text9 = num6.ToString();
					num5 = @event.proj_integrity;
					string text10 = num5.ToString("0.00");
					num5 = @event.proj_travel;
					string text11 = num5.ToString("0.00s");
					num5 = @event.proj_mismatch;
					string text12 = num5.ToString("0.00m");
					num6 = @event.desync;
					string text13 = num6.ToString();
					textTable.AddRow(new string[]
					{
						text,
						attacker,
						text2,
						target,
						text3,
						weapon,
						ammo,
						text4,
						text5,
						text6,
						text7,
						text8,
						text9,
						text10,
						text11,
						text12,
						text13
					});
				}
				else
				{
					num2++;
				}
			}
		}
		string text14;
		if (json)
		{
			text14 = textTable.ToJson();
		}
		else
		{
			text14 = textTable.ToString();
			if (num2 > 0)
			{
				text14 = string.Concat(new object[]
				{
					text14,
					"+ ",
					num2,
					" ",
					(num2 > 1) ? "events" : "event"
				});
				text14 = string.Concat(new object[]
				{
					text14,
					" in the last ",
					combatlogdelay,
					" ",
					(combatlogdelay > 1) ? "seconds" : "second"
				});
			}
		}
		return text14;
	}

	// Token: 0x0600235A RID: 9050 RVA: 0x000E0884 File Offset: 0x000DEA84
	public static Queue<CombatLog.Event> Get(ulong id)
	{
		Queue<CombatLog.Event> queue;
		if (CombatLog.players.TryGetValue(id, out queue))
		{
			return queue;
		}
		queue = new Queue<CombatLog.Event>();
		CombatLog.players.Add(id, queue);
		return queue;
	}

	// Token: 0x04001C10 RID: 7184
	private const string selfname = "you";

	// Token: 0x04001C11 RID: 7185
	private const string noname = "N/A";

	// Token: 0x04001C12 RID: 7186
	private BasePlayer player;

	// Token: 0x04001C13 RID: 7187
	private Queue<CombatLog.Event> storage;

	// Token: 0x04001C15 RID: 7189
	private static Dictionary<ulong, Queue<CombatLog.Event>> players = new Dictionary<ulong, Queue<CombatLog.Event>>();

	// Token: 0x02000C94 RID: 3220
	public struct Event
	{
		// Token: 0x04004319 RID: 17177
		public float time;

		// Token: 0x0400431A RID: 17178
		public uint attacker_id;

		// Token: 0x0400431B RID: 17179
		public uint target_id;

		// Token: 0x0400431C RID: 17180
		public string attacker;

		// Token: 0x0400431D RID: 17181
		public string target;

		// Token: 0x0400431E RID: 17182
		public string weapon;

		// Token: 0x0400431F RID: 17183
		public string ammo;

		// Token: 0x04004320 RID: 17184
		public string bone;

		// Token: 0x04004321 RID: 17185
		public HitArea area;

		// Token: 0x04004322 RID: 17186
		public float distance;

		// Token: 0x04004323 RID: 17187
		public float health_old;

		// Token: 0x04004324 RID: 17188
		public float health_new;

		// Token: 0x04004325 RID: 17189
		public string info;

		// Token: 0x04004326 RID: 17190
		public int proj_hits;

		// Token: 0x04004327 RID: 17191
		public float proj_integrity;

		// Token: 0x04004328 RID: 17192
		public float proj_travel;

		// Token: 0x04004329 RID: 17193
		public float proj_mismatch;

		// Token: 0x0400432A RID: 17194
		public int desync;

		// Token: 0x0400432B RID: 17195
		public bool attacker_dead;
	}
}
