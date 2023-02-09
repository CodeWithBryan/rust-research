using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer.Handlers
{
	// Token: 0x020009C7 RID: 2503
	public class MapMarkers : BaseHandler<AppEmpty>
	{
		// Token: 0x06003B4A RID: 15178 RVA: 0x0015B05C File Offset: 0x0015925C
		public override void Execute()
		{
			AppMapMarkers appMapMarkers = Pool.Get<AppMapMarkers>();
			appMapMarkers.markers = Pool.GetList<AppMarker>();
			global::RelationshipManager.PlayerTeam playerTeam = global::RelationshipManager.ServerInstance.FindPlayersTeam(base.UserId);
			if (playerTeam != null)
			{
				using (List<ulong>.Enumerator enumerator = playerTeam.members.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ulong userID = enumerator.Current;
						global::BasePlayer basePlayer = global::RelationshipManager.FindByID(userID);
						if (!(basePlayer == null))
						{
							appMapMarkers.markers.Add(MapMarkers.GetPlayerMarker(basePlayer));
						}
					}
					goto IL_9A;
				}
			}
			if (base.Player != null)
			{
				appMapMarkers.markers.Add(MapMarkers.GetPlayerMarker(base.Player));
			}
			IL_9A:
			foreach (MapMarker mapMarker in MapMarker.serverMapMarkers)
			{
				if (mapMarker.appType != AppMarkerType.Undefined)
				{
					appMapMarkers.markers.Add(mapMarker.GetAppMarkerData());
				}
			}
			AppResponse appResponse = Pool.Get<AppResponse>();
			appResponse.mapMarkers = appMapMarkers;
			base.Send(appResponse);
		}

		// Token: 0x06003B4B RID: 15179 RVA: 0x0015B180 File Offset: 0x00159380
		private static AppMarker GetPlayerMarker(global::BasePlayer player)
		{
			AppMarker appMarker = Pool.Get<AppMarker>();
			Vector2 vector = Util.WorldToMap(player.transform.position);
			appMarker.id = player.net.ID;
			appMarker.type = AppMarkerType.Player;
			appMarker.x = vector.x;
			appMarker.y = vector.y;
			appMarker.steamId = player.userID;
			return appMarker;
		}
	}
}
