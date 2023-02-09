using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer
{
	// Token: 0x020009BA RID: 2490
	public static class AppPlayerExtensions
	{
		// Token: 0x06003B03 RID: 15107 RVA: 0x00159D24 File Offset: 0x00157F24
		public static AppTeamInfo GetAppTeamInfo(this global::BasePlayer player, ulong steamId)
		{
			AppTeamInfo appTeamInfo = Pool.Get<AppTeamInfo>();
			appTeamInfo.members = Pool.GetList<AppTeamInfo.Member>();
			AppTeamInfo.Member member = Pool.Get<AppTeamInfo.Member>();
			if (player != null)
			{
				Vector2 vector = Util.WorldToMap(player.transform.position);
				member.steamId = player.userID;
				member.name = (player.displayName ?? "");
				member.x = vector.x;
				member.y = vector.y;
				member.isOnline = player.IsConnected;
				AppTeamInfo.Member member2 = member;
				PlayerLifeStory lifeStory = player.lifeStory;
				member2.spawnTime = ((lifeStory != null) ? lifeStory.timeBorn : 0U);
				member.isAlive = player.IsAlive();
				AppTeamInfo.Member member3 = member;
				PlayerLifeStory previousLifeStory = player.previousLifeStory;
				member3.deathTime = ((previousLifeStory != null) ? previousLifeStory.timeDied : 0U);
			}
			else
			{
				member.steamId = steamId;
				member.name = (SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerName(steamId) ?? "");
				member.x = 0f;
				member.y = 0f;
				member.isOnline = false;
				member.spawnTime = 0U;
				member.isAlive = false;
				member.deathTime = 0U;
			}
			appTeamInfo.members.Add(member);
			appTeamInfo.leaderSteamId = 0UL;
			appTeamInfo.mapNotes = AppPlayerExtensions.GetMapNotes(player, true);
			appTeamInfo.leaderMapNotes = AppPlayerExtensions.GetMapNotes(null, false);
			return appTeamInfo;
		}

		// Token: 0x06003B04 RID: 15108 RVA: 0x00159E70 File Offset: 0x00158070
		public static AppTeamInfo GetAppTeamInfo(this global::RelationshipManager.PlayerTeam team, ulong requesterSteamId)
		{
			AppTeamInfo appTeamInfo = Pool.Get<AppTeamInfo>();
			appTeamInfo.members = Pool.GetList<AppTeamInfo.Member>();
			global::BasePlayer player = null;
			global::BasePlayer basePlayer = null;
			for (int i = 0; i < team.members.Count; i++)
			{
				ulong num = team.members[i];
				global::BasePlayer basePlayer2 = global::RelationshipManager.FindByID(num);
				if (!basePlayer2)
				{
					basePlayer2 = null;
				}
				if (num == requesterSteamId)
				{
					player = basePlayer2;
				}
				if (num == team.teamLeader)
				{
					basePlayer = basePlayer2;
				}
				Vector2 vector = Util.WorldToMap((basePlayer2 != null) ? basePlayer2.transform.position : Vector3.zero);
				AppTeamInfo.Member member = Pool.Get<AppTeamInfo.Member>();
				member.steamId = num;
				AppTeamInfo.Member member2 = member;
				string name;
				if ((name = ((basePlayer2 != null) ? basePlayer2.displayName : null)) == null)
				{
					name = (SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerName(num) ?? "");
				}
				member2.name = name;
				member.x = vector.x;
				member.y = vector.y;
				member.isOnline = (basePlayer2 != null && basePlayer2.IsConnected);
				AppTeamInfo.Member member3 = member;
				uint? num2;
				if (basePlayer2 == null)
				{
					num2 = null;
				}
				else
				{
					PlayerLifeStory lifeStory = basePlayer2.lifeStory;
					num2 = ((lifeStory != null) ? new uint?(lifeStory.timeBorn) : null);
				}
				member3.spawnTime = (num2 ?? 0U);
				member.isAlive = (basePlayer2 != null && basePlayer2.IsAlive());
				AppTeamInfo.Member member4 = member;
				uint? num3;
				if (basePlayer2 == null)
				{
					num3 = null;
				}
				else
				{
					PlayerLifeStory previousLifeStory = basePlayer2.previousLifeStory;
					num3 = ((previousLifeStory != null) ? new uint?(previousLifeStory.timeDied) : null);
				}
				member4.deathTime = (num3 ?? 0U);
				appTeamInfo.members.Add(member);
			}
			appTeamInfo.leaderSteamId = team.teamLeader;
			appTeamInfo.mapNotes = AppPlayerExtensions.GetMapNotes(player, true);
			appTeamInfo.leaderMapNotes = AppPlayerExtensions.GetMapNotes((requesterSteamId != team.teamLeader) ? basePlayer : null, false);
			return appTeamInfo;
		}

		// Token: 0x06003B05 RID: 15109 RVA: 0x0015A068 File Offset: 0x00158268
		private static List<AppTeamInfo.Note> GetMapNotes(global::BasePlayer player, bool personalNotes)
		{
			List<AppTeamInfo.Note> list = Pool.GetList<AppTeamInfo.Note>();
			if (player != null)
			{
				if (personalNotes && player.ServerCurrentDeathNote != null)
				{
					AppPlayerExtensions.AddMapNote(list, player.ServerCurrentDeathNote, global::BasePlayer.MapNoteType.Death);
				}
				if (player.ServerCurrentMapNote != null)
				{
					AppPlayerExtensions.AddMapNote(list, player.ServerCurrentMapNote, global::BasePlayer.MapNoteType.PointOfInterest);
				}
			}
			return list;
		}

		// Token: 0x06003B06 RID: 15110 RVA: 0x0015A0B4 File Offset: 0x001582B4
		private static void AddMapNote(List<AppTeamInfo.Note> result, MapNote note, global::BasePlayer.MapNoteType type)
		{
			Vector2 vector = Util.WorldToMap(note.worldPosition);
			AppTeamInfo.Note note2 = Pool.Get<AppTeamInfo.Note>();
			note2.type = (int)type;
			note2.x = vector.x;
			note2.y = vector.y;
			result.Add(note2);
		}
	}
}
