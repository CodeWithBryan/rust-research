using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ConVar;
using Network;
using Newtonsoft.Json;
using UnityEngine;

namespace Facepunch.Rust
{
	// Token: 0x02000ABC RID: 2748
	public static class Analytics
	{
		// Token: 0x170005BB RID: 1467
		// (get) Token: 0x06004280 RID: 17024 RVA: 0x00184565 File Offset: 0x00182765
		// (set) Token: 0x06004281 RID: 17025 RVA: 0x0018456C File Offset: 0x0018276C
		[ServerVar(Name = "client_analytics_url")]
		public static string ClientAnalyticsUrl { get; set; } = "https://functions-rust-api.azurewebsites.net/api/public/analytics/rust/client";

		// Token: 0x170005BC RID: 1468
		// (get) Token: 0x06004282 RID: 17026 RVA: 0x00184574 File Offset: 0x00182774
		// (set) Token: 0x06004283 RID: 17027 RVA: 0x0018457B File Offset: 0x0018277B
		[ServerVar(Name = "server_analytics_url")]
		public static string ServerAnalyticsUrl { get; set; } = "https://functions-rust-api.azurewebsites.net/api/public/analytics/rust/server";

		// Token: 0x170005BD RID: 1469
		// (get) Token: 0x06004284 RID: 17028 RVA: 0x00184583 File Offset: 0x00182783
		// (set) Token: 0x06004285 RID: 17029 RVA: 0x0018458A File Offset: 0x0018278A
		[ServerVar(Name = "analytics_header")]
		public static string AnalyticsHeader { get; set; } = "X-API-KEY";

		// Token: 0x170005BE RID: 1470
		// (get) Token: 0x06004286 RID: 17030 RVA: 0x00184592 File Offset: 0x00182792
		// (set) Token: 0x06004287 RID: 17031 RVA: 0x00184599 File Offset: 0x00182799
		[ServerVar(Name = "analytics_secret")]
		public static string AnalyticsSecret { get; set; } = "";

		// Token: 0x02000F1F RID: 3871
		public class AzureWebInterface
		{
			// Token: 0x060051F5 RID: 20981 RVA: 0x001A5ED4 File Offset: 0x001A40D4
			public AzureWebInterface(bool isClient)
			{
				this.IsClient = isClient;
			}

			// Token: 0x060051F6 RID: 20982 RVA: 0x001A5F38 File Offset: 0x001A4138
			public void EnqueueEvent(EventRecord point)
			{
				DateTime utcNow = DateTime.UtcNow;
				this.pending.Add(point);
				if (this.pending.Count > this.FlushSize || utcNow > this.nextFlush)
				{
					Analytics.AzureWebInterface.<>c__DisplayClass11_0 CS$<>8__locals1 = new Analytics.AzureWebInterface.<>c__DisplayClass11_0();
					CS$<>8__locals1.<>4__this = this;
					this.nextFlush = utcNow.Add(this.FlushDelay);
					CS$<>8__locals1.toUpload = this.pending;
					Task.Run(delegate()
					{
						Analytics.AzureWebInterface.<>c__DisplayClass11_0.<<EnqueueEvent>b__0>d <<EnqueueEvent>b__0>d;
						<<EnqueueEvent>b__0>d.<>4__this = CS$<>8__locals1;
						<<EnqueueEvent>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
						<<EnqueueEvent>b__0>d.<>1__state = -1;
						AsyncTaskMethodBuilder <>t__builder = <<EnqueueEvent>b__0>d.<>t__builder;
						<>t__builder.Start<Analytics.AzureWebInterface.<>c__DisplayClass11_0.<<EnqueueEvent>b__0>d>(ref <<EnqueueEvent>b__0>d);
						return <<EnqueueEvent>b__0>d.<>t__builder.Task;
					});
					List<EventRecord> list;
					while (this.listPool.TryDequeue(out list))
					{
						foreach (EventRecord eventRecord in list)
						{
							Pool.Free<EventRecord>(ref eventRecord);
						}
						Pool.FreeList<EventRecord>(ref list);
					}
					this.pending = Pool.GetList<EventRecord>();
				}
			}

			// Token: 0x060051F7 RID: 20983 RVA: 0x001A601C File Offset: 0x001A421C
			private async Task UploadAsync(List<EventRecord> records)
			{
				string payload;
				try
				{
					payload = JsonConvert.SerializeObject(records);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
					this.listPool.Enqueue(records);
					return;
				}
				for (int attempt = 0; attempt < this.MaxRetries; attempt++)
				{
					try
					{
						using (StringContent content = new StringContent(payload, Encoding.UTF8, "application/json"))
						{
							content.Headers.Add(Analytics.AnalyticsHeader, Analytics.AnalyticsSecret);
							if (!this.IsClient)
							{
								content.Headers.Add("X-SERVER-IP", Network.Net.sv.ip);
								content.Headers.Add("X-SERVER-PORT", Network.Net.sv.port.ToString());
							}
							(await this.HttpClient.PostAsync(this.IsClient ? Analytics.ClientAnalyticsUrl : Analytics.ServerAnalyticsUrl, content)).EnsureSuccessStatusCode();
							break;
						}
					}
					catch (Exception ex)
					{
						if (!(ex is HttpRequestException))
						{
							Debug.LogException(ex);
						}
					}
				}
				this.listPool.Enqueue(records);
			}

			// Token: 0x04004D55 RID: 19797
			public static readonly Analytics.AzureWebInterface client = new Analytics.AzureWebInterface(true);

			// Token: 0x04004D56 RID: 19798
			public static readonly Analytics.AzureWebInterface server = new Analytics.AzureWebInterface(false);

			// Token: 0x04004D57 RID: 19799
			public bool IsClient;

			// Token: 0x04004D58 RID: 19800
			public int MaxRetries = 1;

			// Token: 0x04004D59 RID: 19801
			public int FlushSize = 1000;

			// Token: 0x04004D5A RID: 19802
			public TimeSpan FlushDelay = TimeSpan.FromSeconds(30.0);

			// Token: 0x04004D5B RID: 19803
			private DateTime nextFlush;

			// Token: 0x04004D5C RID: 19804
			private ConcurrentQueue<List<EventRecord>> listPool = new ConcurrentQueue<List<EventRecord>>();

			// Token: 0x04004D5D RID: 19805
			private List<EventRecord> pending = new List<EventRecord>();

			// Token: 0x04004D5E RID: 19806
			private HttpClient HttpClient = new HttpClient();
		}

		// Token: 0x02000F20 RID: 3872
		public static class Server
		{
			// Token: 0x170006BE RID: 1726
			// (get) Token: 0x060051F9 RID: 20985 RVA: 0x001A6081 File Offset: 0x001A4281
			private static bool WriteToFile
			{
				get
				{
					return ConVar.Server.statBackup;
				}
			}

			// Token: 0x170006BF RID: 1727
			// (get) Token: 0x060051FA RID: 20986 RVA: 0x001A6088 File Offset: 0x001A4288
			private static bool CanSendAnalytics
			{
				get
				{
					return ConVar.Server.official && ConVar.Server.stats && Analytics.Server.Enabled;
				}
			}

			// Token: 0x060051FB RID: 20987 RVA: 0x001A60A0 File Offset: 0x001A42A0
			internal static void Death(BaseEntity initiator, BaseEntity weaponPrefab, Vector3 worldPosition)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				if (initiator != null)
				{
					if (initiator is BasePlayer)
					{
						if (weaponPrefab != null)
						{
							Analytics.Server.Death(weaponPrefab.ShortPrefabName, worldPosition, initiator.IsNpc ? Analytics.Server.DeathType.NPC : Analytics.Server.DeathType.Player);
							return;
						}
						Analytics.Server.Death("player", worldPosition, Analytics.Server.DeathType.Player);
						return;
					}
					else if (initiator is AutoTurret)
					{
						if (weaponPrefab != null)
						{
							Analytics.Server.Death(weaponPrefab.ShortPrefabName, worldPosition, Analytics.Server.DeathType.AutoTurret);
							return;
						}
					}
					else
					{
						Analytics.Server.Death(initiator.Categorize(), worldPosition, initiator.IsNpc ? Analytics.Server.DeathType.NPC : Analytics.Server.DeathType.Player);
					}
				}
			}

			// Token: 0x060051FC RID: 20988 RVA: 0x001A612C File Offset: 0x001A432C
			internal static void Death(string v, Vector3 worldPosition, Analytics.Server.DeathType deathType = Analytics.Server.DeathType.Player)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				string monumentStringFromPosition = Analytics.Server.GetMonumentStringFromPosition(worldPosition);
				if (!string.IsNullOrEmpty(monumentStringFromPosition))
				{
					switch (deathType)
					{
					case Analytics.Server.DeathType.Player:
						Analytics.Server.DesignEvent("player:" + monumentStringFromPosition + "death:" + v, false);
						return;
					case Analytics.Server.DeathType.NPC:
						Analytics.Server.DesignEvent("player:" + monumentStringFromPosition + "death:npc:" + v, false);
						return;
					case Analytics.Server.DeathType.AutoTurret:
						Analytics.Server.DesignEvent("player:" + monumentStringFromPosition + "death:autoturret:" + v, false);
						return;
					default:
						return;
					}
				}
				else
				{
					switch (deathType)
					{
					case Analytics.Server.DeathType.Player:
						Analytics.Server.DesignEvent("player:death:" + v, false);
						return;
					case Analytics.Server.DeathType.NPC:
						Analytics.Server.DesignEvent("player:death:npc:" + v, false);
						return;
					case Analytics.Server.DeathType.AutoTurret:
						Analytics.Server.DesignEvent("player:death:autoturret:" + v, false);
						return;
					default:
						return;
					}
				}
			}

			// Token: 0x060051FD RID: 20989 RVA: 0x001A61F4 File Offset: 0x001A43F4
			private static string GetMonumentStringFromPosition(Vector3 worldPosition)
			{
				MonumentInfo monumentInfo = TerrainMeta.Path.FindMonumentWithBoundsOverlap(worldPosition);
				if (monumentInfo != null && !string.IsNullOrEmpty(monumentInfo.displayPhrase.token))
				{
					return monumentInfo.displayPhrase.token;
				}
				if (SingletonComponent<EnvironmentManager>.Instance != null && (EnvironmentManager.Get(worldPosition) & EnvironmentType.TrainTunnels) == EnvironmentType.TrainTunnels)
				{
					return "train_tunnel_display_name";
				}
				return string.Empty;
			}

			// Token: 0x060051FE RID: 20990 RVA: 0x001A6259 File Offset: 0x001A4459
			public static void Crafting(string targetItemShortname, int skinId)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				Analytics.Server.DesignEvent("player:craft:" + targetItemShortname, false);
				Analytics.Server.SkinUsed(targetItemShortname, skinId);
			}

			// Token: 0x060051FF RID: 20991 RVA: 0x001A627B File Offset: 0x001A447B
			public static void SkinUsed(string itemShortName, int skinId)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				if (skinId == 0)
				{
					return;
				}
				Analytics.Server.DesignEvent(string.Format("skinUsed:{0}:{1}", itemShortName, skinId), false);
			}

			// Token: 0x06005200 RID: 20992 RVA: 0x001A62A0 File Offset: 0x001A44A0
			public static void ExcavatorStarted()
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				Analytics.Server.DesignEvent("monuments:excavatorstarted", false);
			}

			// Token: 0x06005201 RID: 20993 RVA: 0x001A62B5 File Offset: 0x001A44B5
			public static void ExcavatorStopped(float activeDuration)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				Analytics.Server.DesignEvent("monuments:excavatorstopped", activeDuration, false);
			}

			// Token: 0x06005202 RID: 20994 RVA: 0x001A62CB File Offset: 0x001A44CB
			public static void SlotMachineTransaction(int scrapSpent, int scrapReceived)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				Analytics.Server.DesignEvent("slots:scrapSpent", scrapSpent, false);
				Analytics.Server.DesignEvent("slots:scrapReceived", scrapReceived, false);
			}

			// Token: 0x06005203 RID: 20995 RVA: 0x001A62ED File Offset: 0x001A44ED
			public static void VehiclePurchased(string vehicleType)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				Analytics.Server.DesignEvent("vehiclePurchased:" + vehicleType, false);
			}

			// Token: 0x06005204 RID: 20996 RVA: 0x001A6308 File Offset: 0x001A4508
			public static void FishCaught(ItemDefinition fish)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				if (fish == null)
				{
					return;
				}
				Analytics.Server.DesignEvent("fishCaught:" + fish.shortname, false);
			}

			// Token: 0x06005205 RID: 20997 RVA: 0x001A6334 File Offset: 0x001A4534
			public static void VendingMachineTransaction(NPCVendingOrder npcVendingOrder, ItemDefinition purchased, int amount)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				if (purchased == null)
				{
					return;
				}
				if (npcVendingOrder == null)
				{
					Analytics.Server.DesignEvent("vendingPurchase:player:" + purchased.shortname, amount, false);
					return;
				}
				Analytics.Server.DesignEvent("vendingPurchase:static:" + purchased.shortname, amount, false);
			}

			// Token: 0x06005206 RID: 20998 RVA: 0x001A638B File Offset: 0x001A458B
			public static void Consume(string consumedItem)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				if (string.IsNullOrEmpty(consumedItem))
				{
					return;
				}
				Analytics.Server.DesignEvent("player:consume:" + consumedItem, false);
			}

			// Token: 0x06005207 RID: 20999 RVA: 0x001A63AF File Offset: 0x001A45AF
			public static void TreeKilled(BaseEntity withWeapon)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				if (withWeapon != null)
				{
					Analytics.Server.DesignEvent("treekilled:" + withWeapon.ShortPrefabName, false);
					return;
				}
				Analytics.Server.DesignEvent("treekilled", false);
			}

			// Token: 0x06005208 RID: 21000 RVA: 0x001A63E4 File Offset: 0x001A45E4
			public static void OreKilled(OreResourceEntity entity, HitInfo info)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				ResourceDispenser resourceDispenser;
				if (entity.TryGetComponent<ResourceDispenser>(out resourceDispenser) && resourceDispenser.containedItems.Count > 0 && resourceDispenser.containedItems[0].itemDef != null)
				{
					if (info.WeaponPrefab != null)
					{
						Analytics.Server.DesignEvent("orekilled:" + resourceDispenser.containedItems[0].itemDef.shortname + ":" + info.WeaponPrefab.ShortPrefabName, false);
						return;
					}
					Analytics.Server.DesignEvent(string.Format("orekilled:{0}", resourceDispenser.containedItems[0]), false);
				}
			}

			// Token: 0x06005209 RID: 21001 RVA: 0x001A648E File Offset: 0x001A468E
			public static void MissionComplete(BaseMission mission)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				Analytics.Server.DesignEvent("missionComplete:" + mission.shortname, true);
			}

			// Token: 0x0600520A RID: 21002 RVA: 0x001A64AE File Offset: 0x001A46AE
			public static void MissionFailed(BaseMission mission, BaseMission.MissionFailReason reason)
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				Analytics.Server.DesignEvent(string.Format("missionFailed:{0}:{1}", mission.shortname, reason), true);
			}

			// Token: 0x0600520B RID: 21003 RVA: 0x001A64D4 File Offset: 0x001A46D4
			public static void FreeUnderwaterCrate()
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				Analytics.Server.DesignEvent("loot:freeUnderWaterCrate", false);
			}

			// Token: 0x0600520C RID: 21004 RVA: 0x001A64E9 File Offset: 0x001A46E9
			public static void HeldItemDeployed(ItemDefinition def)
			{
				if (!Analytics.Server.CanSendAnalytics || Analytics.Server.lastHeldItemEvent < 0.1f)
				{
					return;
				}
				Analytics.Server.lastHeldItemEvent = 0f;
				Analytics.Server.DesignEvent("heldItemDeployed:" + def.shortname, false);
			}

			// Token: 0x0600520D RID: 21005 RVA: 0x001A6529 File Offset: 0x001A4729
			public static void UsedZipline()
			{
				if (!Analytics.Server.CanSendAnalytics)
				{
					return;
				}
				Analytics.Server.DesignEvent("usedZipline", false);
			}

			// Token: 0x0600520E RID: 21006 RVA: 0x001A653E File Offset: 0x001A473E
			public static void ReportCandiesCollectedByPlayer(int count)
			{
				if (!Analytics.Server.Enabled)
				{
					return;
				}
				Analytics.Server.DesignEvent("halloween:candiesCollected", count, false);
			}

			// Token: 0x0600520F RID: 21007 RVA: 0x001A6554 File Offset: 0x001A4754
			public static void ReportPlayersParticipatedInHalloweenEvent(int count)
			{
				if (!Analytics.Server.Enabled)
				{
					return;
				}
				Analytics.Server.DesignEvent("halloween:playersParticipated", count, false);
			}

			// Token: 0x06005210 RID: 21008 RVA: 0x001A656A File Offset: 0x001A476A
			public static void Trigger(string message)
			{
				if (!Analytics.Server.CanSendAnalytics || string.IsNullOrEmpty(message))
				{
					return;
				}
				Analytics.Server.DesignEvent(message, false);
			}

			// Token: 0x06005211 RID: 21009 RVA: 0x001A6583 File Offset: 0x001A4783
			private static void DesignEvent(string message, bool canBackup = false)
			{
				if (!Analytics.Server.CanSendAnalytics || string.IsNullOrEmpty(message))
				{
					return;
				}
				GA.DesignEvent(message);
				if (canBackup)
				{
					Analytics.Server.LocalBackup(message, 1f);
				}
			}

			// Token: 0x06005212 RID: 21010 RVA: 0x001A65A9 File Offset: 0x001A47A9
			private static void DesignEvent(string message, float value, bool canBackup = false)
			{
				if (!Analytics.Server.CanSendAnalytics || string.IsNullOrEmpty(message))
				{
					return;
				}
				GA.DesignEvent(message, value);
				if (canBackup)
				{
					Analytics.Server.LocalBackup(message, value);
				}
			}

			// Token: 0x06005213 RID: 21011 RVA: 0x001A65CC File Offset: 0x001A47CC
			private static void DesignEvent(string message, int value, bool canBackup = false)
			{
				if (!Analytics.Server.CanSendAnalytics || string.IsNullOrEmpty(message))
				{
					return;
				}
				GA.DesignEvent(message, (float)value);
				if (canBackup)
				{
					Analytics.Server.LocalBackup(message, (float)value);
				}
			}

			// Token: 0x06005214 RID: 21012 RVA: 0x001A65F4 File Offset: 0x001A47F4
			private static string GetBackupPath(DateTime date)
			{
				return string.Format("{0}/{1}_{2}_{3}_analytics_backup.txt", new object[]
				{
					ConVar.Server.GetServerFolder("analytics"),
					date.Day,
					date.Month,
					date.Year
				});
			}

			// Token: 0x170006C0 RID: 1728
			// (get) Token: 0x06005215 RID: 21013 RVA: 0x001A664B File Offset: 0x001A484B
			private static DateTime currentDate
			{
				get
				{
					return DateTime.Now;
				}
			}

			// Token: 0x06005216 RID: 21014 RVA: 0x001A6654 File Offset: 0x001A4854
			private static void LocalBackup(string message, float value)
			{
				if (!Analytics.Server.WriteToFile)
				{
					return;
				}
				if (Analytics.Server.bufferData != null && Analytics.Server.backupDate.Date != Analytics.Server.currentDate.Date)
				{
					Analytics.Server.<LocalBackup>g__SaveBufferIntoDateFile|38_1(Analytics.Server.backupDate);
					Analytics.Server.bufferData.Clear();
					Analytics.Server.backupDate = Analytics.Server.currentDate;
				}
				if (Analytics.Server.bufferData == null)
				{
					if (Analytics.Server.bufferData == null)
					{
						Analytics.Server.bufferData = new Dictionary<string, float>();
					}
					Analytics.Server.lastAnalyticsSave = 0f;
					Analytics.Server.backupDate = Analytics.Server.currentDate;
				}
				if (Analytics.Server.bufferData.ContainsKey(message))
				{
					Dictionary<string, float> dictionary = Analytics.Server.bufferData;
					dictionary[message] += value;
				}
				else
				{
					Analytics.Server.bufferData.Add(message, value);
				}
				if (Analytics.Server.lastAnalyticsSave > 120f)
				{
					Analytics.Server.lastAnalyticsSave = 0f;
					Analytics.Server.<LocalBackup>g__SaveBufferIntoDateFile|38_1(Analytics.Server.currentDate);
					Analytics.Server.bufferData.Clear();
				}
			}

			// Token: 0x06005218 RID: 21016 RVA: 0x001A6744 File Offset: 0x001A4944
			[CompilerGenerated]
			internal static void <LocalBackup>g__MergeBuffers|38_0(Dictionary<string, float> target, Dictionary<string, float> destination)
			{
				foreach (KeyValuePair<string, float> keyValuePair in target)
				{
					if (destination.ContainsKey(keyValuePair.Key))
					{
						string key = keyValuePair.Key;
						destination[key] += keyValuePair.Value;
					}
					else
					{
						destination.Add(keyValuePair.Key, keyValuePair.Value);
					}
				}
			}

			// Token: 0x06005219 RID: 21017 RVA: 0x001A67D0 File Offset: 0x001A49D0
			[CompilerGenerated]
			internal static void <LocalBackup>g__SaveBufferIntoDateFile|38_1(DateTime date)
			{
				string backupPath = Analytics.Server.GetBackupPath(date);
				if (File.Exists(backupPath))
				{
					Dictionary<string, float> dictionary = (Dictionary<string, float>)JsonConvert.DeserializeObject(File.ReadAllText(backupPath), typeof(Dictionary<string, float>));
					if (dictionary != null)
					{
						Analytics.Server.<LocalBackup>g__MergeBuffers|38_0(dictionary, Analytics.Server.bufferData);
					}
				}
				string contents = JsonConvert.SerializeObject(Analytics.Server.bufferData);
				File.WriteAllText(Analytics.Server.GetBackupPath(date), contents);
			}

			// Token: 0x04004D5F RID: 19807
			public static bool Enabled;

			// Token: 0x04004D60 RID: 19808
			private static Dictionary<string, float> bufferData;

			// Token: 0x04004D61 RID: 19809
			private static TimeSince lastHeldItemEvent;

			// Token: 0x04004D62 RID: 19810
			private static TimeSince lastAnalyticsSave;

			// Token: 0x04004D63 RID: 19811
			private static DateTime backupDate;

			// Token: 0x02000F77 RID: 3959
			public enum DeathType
			{
				// Token: 0x04004E69 RID: 20073
				Player,
				// Token: 0x04004E6A RID: 20074
				NPC,
				// Token: 0x04004E6B RID: 20075
				AutoTurret
			}
		}
	}
}
