using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer.Handlers
{
	// Token: 0x020009C6 RID: 2502
	public class Map : BaseHandler<AppEmpty>
	{
		// Token: 0x1700049E RID: 1182
		// (get) Token: 0x06003B45 RID: 15173 RVA: 0x0001F1CE File Offset: 0x0001D3CE
		protected override int TokenCost
		{
			get
			{
				return 5;
			}
		}

		// Token: 0x06003B46 RID: 15174 RVA: 0x0015AE6C File Offset: 0x0015906C
		public override void Execute()
		{
			if (Map._imageData == null)
			{
				base.SendError("no_map");
				return;
			}
			AppMap appMap = Pool.Get<AppMap>();
			appMap.width = (uint)Map._width;
			appMap.height = (uint)Map._height;
			appMap.oceanMargin = 500;
			appMap.jpgImage = Map._imageData;
			appMap.background = Map._background;
			appMap.monuments = Pool.GetList<AppMap.Monument>();
			if (TerrainMeta.Path != null && TerrainMeta.Path.Landmarks != null)
			{
				foreach (LandmarkInfo landmarkInfo in TerrainMeta.Path.Landmarks)
				{
					if (landmarkInfo.shouldDisplayOnMap)
					{
						Vector2 vector = Util.WorldToMap(landmarkInfo.transform.position);
						AppMap.Monument monument = Pool.Get<AppMap.Monument>();
						monument.token = (landmarkInfo.displayPhrase.IsValid() ? landmarkInfo.displayPhrase.token : landmarkInfo.transform.root.name);
						monument.x = vector.x;
						monument.y = vector.y;
						appMap.monuments.Add(monument);
					}
				}
			}
			AppResponse appResponse = Pool.Get<AppResponse>();
			appResponse.map = appMap;
			base.Send(appResponse);
		}

		// Token: 0x06003B47 RID: 15175 RVA: 0x0015AFCC File Offset: 0x001591CC
		public static void PopulateCache()
		{
			Map.RenderToCache();
		}

		// Token: 0x06003B48 RID: 15176 RVA: 0x0015AFD4 File Offset: 0x001591D4
		private static void RenderToCache()
		{
			Map._imageData = null;
			Map._width = 0;
			Map._height = 0;
			try
			{
				Color color;
				Map._imageData = MapImageRenderer.Render(out Map._width, out Map._height, out color, 0.5f, true);
				Map._background = "#" + ColorUtility.ToHtmlStringRGB(color);
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("Exception thrown when rendering map for the app: {0}", arg));
			}
			if (Map._imageData == null)
			{
				Debug.LogError("Map image is null! App users will not be able to see the map.");
			}
		}

		// Token: 0x04003533 RID: 13619
		private static int _width;

		// Token: 0x04003534 RID: 13620
		private static int _height;

		// Token: 0x04003535 RID: 13621
		private static byte[] _imageData;

		// Token: 0x04003536 RID: 13622
		private static string _background;
	}
}
