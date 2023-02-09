using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Facepunch;
using Newtonsoft.Json;
using Steamworks;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000A84 RID: 2692
	[ConsoleSystem.Factory("inventory")]
	public class Inventory : ConsoleSystem
	{
		// Token: 0x0600403F RID: 16447 RVA: 0x0017A3BC File Offset: 0x001785BC
		[ServerUserVar]
		public static void lighttoggle(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			if (basePlayer.IsDead())
			{
				return;
			}
			if (basePlayer.IsSleeping())
			{
				return;
			}
			if (basePlayer.InGesture)
			{
				return;
			}
			basePlayer.LightToggle(true);
		}

		// Token: 0x06004040 RID: 16448 RVA: 0x0017A3FC File Offset: 0x001785FC
		[ServerUserVar]
		public static void endloot(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			if (basePlayer.IsDead())
			{
				return;
			}
			if (basePlayer.IsSleeping())
			{
				return;
			}
			basePlayer.inventory.loot.Clear();
		}

		// Token: 0x06004041 RID: 16449 RVA: 0x0017A43C File Offset: 0x0017863C
		[ServerVar]
		public static void give(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			Item item = ItemManager.CreateByPartialName(arg.GetString(0, ""), 1, arg.GetULong(3, 0UL));
			if (item == null)
			{
				arg.ReplyWith("Invalid Item!");
				return;
			}
			int @int = arg.GetInt(1, 1);
			item.amount = @int;
			float @float = arg.GetFloat(2, 1f);
			item.conditionNormalized = @float;
			item.OnVirginSpawn();
			if (!basePlayer.inventory.GiveItem(item, null))
			{
				item.Remove(0f);
				arg.ReplyWith("Couldn't give item (inventory full?)");
				return;
			}
			basePlayer.Command("note.inv", new object[]
			{
				item.info.itemid,
				@int
			});
			Debug.Log(string.Concat(new object[]
			{
				"giving ",
				basePlayer.displayName,
				" ",
				@int,
				" x ",
				item.info.displayName.english
			}));
			if (basePlayer.IsDeveloper)
			{
				basePlayer.ChatMessage(string.Concat(new object[]
				{
					"you silently gave yourself ",
					@int,
					" x ",
					item.info.displayName.english
				}));
				return;
			}
			Chat.Broadcast(string.Concat(new object[]
			{
				basePlayer.displayName,
				" gave themselves ",
				@int,
				" x ",
				item.info.displayName.english
			}), "SERVER", "#eee", 0UL);
		}

		// Token: 0x06004042 RID: 16450 RVA: 0x0017A5E8 File Offset: 0x001787E8
		[ServerVar]
		public static void resetbp(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.GetPlayer(0);
			if (basePlayer == null)
			{
				if (arg.HasArgs(1))
				{
					arg.ReplyWith("Can't find player");
					return;
				}
				basePlayer = arg.Player();
			}
			basePlayer.blueprints.Reset();
		}

		// Token: 0x06004043 RID: 16451 RVA: 0x0017A630 File Offset: 0x00178830
		[ServerVar]
		public static void unlockall(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.GetPlayer(0);
			if (basePlayer == null)
			{
				if (arg.HasArgs(1))
				{
					arg.ReplyWith("Can't find player");
					return;
				}
				basePlayer = arg.Player();
			}
			basePlayer.blueprints.UnlockAll();
		}

		// Token: 0x06004044 RID: 16452 RVA: 0x0017A678 File Offset: 0x00178878
		[ServerVar]
		public static void giveall(ConsoleSystem.Arg arg)
		{
			Item item = null;
			string text = "SERVER";
			if (arg.Player() != null)
			{
				text = arg.Player().displayName;
			}
			foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
			{
				item = ItemManager.CreateByPartialName(arg.GetString(0, ""), 1, 0UL);
				if (item == null)
				{
					arg.ReplyWith("Invalid Item!");
					return;
				}
				int @int = arg.GetInt(1, 1);
				item.amount = @int;
				item.OnVirginSpawn();
				if (!basePlayer.inventory.GiveItem(item, null))
				{
					item.Remove(0f);
					arg.ReplyWith("Couldn't give item (inventory full?)");
				}
				else
				{
					basePlayer.Command("note.inv", new object[]
					{
						item.info.itemid,
						@int
					});
					Debug.Log(string.Concat(new object[]
					{
						" [ServerVar] giving ",
						basePlayer.displayName,
						" ",
						item.amount,
						" x ",
						item.info.displayName.english
					}));
				}
			}
			if (item != null)
			{
				Chat.Broadcast(string.Concat(new object[]
				{
					text,
					" gave everyone ",
					item.amount,
					" x ",
					item.info.displayName.english
				}), "SERVER", "#eee", 0UL);
			}
		}

		// Token: 0x06004045 RID: 16453 RVA: 0x0017A838 File Offset: 0x00178A38
		[ServerVar]
		public static void giveto(ConsoleSystem.Arg arg)
		{
			string text = "SERVER";
			if (arg.Player() != null)
			{
				text = arg.Player().displayName;
			}
			BasePlayer basePlayer = BasePlayer.Find(arg.GetString(0, ""));
			if (basePlayer == null)
			{
				arg.ReplyWith("Couldn't find player!");
				return;
			}
			Item item = ItemManager.CreateByPartialName(arg.GetString(1, ""), 1, arg.GetULong(3, 0UL));
			if (item == null)
			{
				arg.ReplyWith("Invalid Item!");
				return;
			}
			int @int = arg.GetInt(2, 1);
			item.amount = @int;
			item.OnVirginSpawn();
			if (!basePlayer.inventory.GiveItem(item, null))
			{
				item.Remove(0f);
				arg.ReplyWith("Couldn't give item (inventory full?)");
				return;
			}
			basePlayer.Command("note.inv", new object[]
			{
				item.info.itemid,
				@int
			});
			Debug.Log(string.Concat(new object[]
			{
				" [ServerVar] giving ",
				basePlayer.displayName,
				" ",
				@int,
				" x ",
				item.info.displayName.english
			}));
			Chat.Broadcast(string.Concat(new object[]
			{
				text,
				" gave ",
				basePlayer.displayName,
				" ",
				@int,
				" x ",
				item.info.displayName.english
			}), "SERVER", "#eee", 0UL);
		}

		// Token: 0x06004046 RID: 16454 RVA: 0x0017A9CC File Offset: 0x00178BCC
		[ServerVar]
		public static void giveid(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			Item item = ItemManager.CreateByItemID(arg.GetInt(0, 0), 1, 0UL);
			if (item == null)
			{
				arg.ReplyWith("Invalid Item!");
				return;
			}
			int @int = arg.GetInt(1, 1);
			item.amount = @int;
			item.OnVirginSpawn();
			if (!basePlayer.inventory.GiveItem(item, null))
			{
				item.Remove(0f);
				arg.ReplyWith("Couldn't give item (inventory full?)");
				return;
			}
			basePlayer.Command("note.inv", new object[]
			{
				item.info.itemid,
				@int
			});
			Debug.Log(string.Concat(new object[]
			{
				" [ServerVar] giving ",
				basePlayer.displayName,
				" ",
				@int,
				" x ",
				item.info.displayName.english
			}));
			if (basePlayer.IsDeveloper)
			{
				basePlayer.ChatMessage(string.Concat(new object[]
				{
					"you silently gave yourself ",
					@int,
					" x ",
					item.info.displayName.english
				}));
				return;
			}
			Chat.Broadcast(string.Concat(new object[]
			{
				basePlayer.displayName,
				" gave themselves ",
				@int,
				" x ",
				item.info.displayName.english
			}), "SERVER", "#eee", 0UL);
		}

		// Token: 0x06004047 RID: 16455 RVA: 0x0017AB58 File Offset: 0x00178D58
		[ServerVar]
		public static void givearm(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			Item item = ItemManager.CreateByItemID(arg.GetInt(0, 0), 1, 0UL);
			if (item == null)
			{
				arg.ReplyWith("Invalid Item!");
				return;
			}
			int @int = arg.GetInt(1, 1);
			item.amount = @int;
			item.OnVirginSpawn();
			if (!basePlayer.inventory.GiveItem(item, basePlayer.inventory.containerBelt))
			{
				item.Remove(0f);
				arg.ReplyWith("Couldn't give item (inventory full?)");
				return;
			}
			basePlayer.Command("note.inv", new object[]
			{
				item.info.itemid,
				@int
			});
			Debug.Log(string.Concat(new object[]
			{
				" [ServerVar] giving ",
				basePlayer.displayName,
				" ",
				item.amount,
				" x ",
				item.info.displayName.english
			}));
			if (basePlayer.IsDeveloper)
			{
				basePlayer.ChatMessage(string.Concat(new object[]
				{
					"you silently gave yourself ",
					item.amount,
					" x ",
					item.info.displayName.english
				}));
				return;
			}
			Chat.Broadcast(string.Concat(new object[]
			{
				basePlayer.displayName,
				" gave themselves ",
				item.amount,
				" x ",
				item.info.displayName.english
			}), "SERVER", "#eee", 0UL);
		}

		// Token: 0x06004048 RID: 16456 RVA: 0x0017ACFC File Offset: 0x00178EFC
		[ServerVar(Help = "Copies the players inventory to the player in front of them")]
		public static void copyTo(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			if (basePlayer == null)
			{
				return;
			}
			BasePlayer basePlayer2 = null;
			if (arg.HasArgs(1) && arg.GetString(0, "").ToLower() != "true")
			{
				basePlayer2 = arg.GetPlayer(0);
				if (basePlayer2 == null)
				{
					uint @uint = arg.GetUInt(0, 0U);
					basePlayer2 = BasePlayer.FindByID((ulong)@uint);
					if (basePlayer2 == null)
					{
						basePlayer2 = BasePlayer.FindBot((ulong)@uint);
					}
				}
			}
			else
			{
				basePlayer2 = RelationshipManager.GetLookingAtPlayer(basePlayer);
			}
			if (basePlayer2 == null)
			{
				return;
			}
			basePlayer2.inventory.containerBelt.Clear();
			basePlayer2.inventory.containerWear.Clear();
			int num = 0;
			foreach (Item item in basePlayer.inventory.containerBelt.itemList)
			{
				basePlayer2.inventory.containerBelt.AddItem(item.info, item.amount, item.skin, ItemContainer.LimitStack.Existing);
				if (item.contents != null)
				{
					Item item2 = basePlayer2.inventory.containerBelt.itemList[num];
					foreach (Item item3 in item.contents.itemList)
					{
						item2.contents.AddItem(item3.info, item3.amount, item3.skin, ItemContainer.LimitStack.Existing);
					}
				}
				num++;
			}
			foreach (Item item4 in basePlayer.inventory.containerWear.itemList)
			{
				basePlayer2.inventory.containerWear.AddItem(item4.info, item4.amount, item4.skin, ItemContainer.LimitStack.Existing);
			}
			if (basePlayer.IsDeveloper)
			{
				basePlayer.ChatMessage("you silently copied items to " + basePlayer2.displayName);
				return;
			}
			Chat.Broadcast(basePlayer.displayName + " copied their inventory to " + basePlayer2.displayName, "SERVER", "#eee", 0UL);
		}

		// Token: 0x06004049 RID: 16457 RVA: 0x0017AF78 File Offset: 0x00179178
		[ServerVar(Help = "Deploys a loadout to players in a radius eg. inventory.deployLoadoutInRange testloadout 30")]
		public static void deployLoadoutInRange(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			if (basePlayer == null)
			{
				return;
			}
			string @string = arg.GetString(0, "");
			Inventory.SavedLoadout savedLoadout;
			if (!Inventory.LoadLoadout(@string, out savedLoadout))
			{
				arg.ReplyWith("Can't find loadout: " + @string);
				return;
			}
			float @float = arg.GetFloat(1, 0f);
			List<BasePlayer> list = Pool.GetList<BasePlayer>();
			Vis.Entities<BasePlayer>(basePlayer.transform.position, @float, list, 131072, QueryTriggerInteraction.Collide);
			int num = 0;
			foreach (BasePlayer basePlayer2 in list)
			{
				if (!(basePlayer2 == basePlayer) && !basePlayer2.isClient)
				{
					savedLoadout.LoadItemsOnTo(basePlayer2);
					num++;
				}
			}
			arg.ReplyWith(string.Format("Applied loadout {0} to {1} players", @string, num));
			Pool.FreeList<BasePlayer>(ref list);
		}

		// Token: 0x0600404A RID: 16458 RVA: 0x0017B084 File Offset: 0x00179284
		[ServerVar(Help = "Deploys the given loadout to a target player. eg. inventory.deployLoadout testloadout jim")]
		public static void deployLoadout(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				return;
			}
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			string @string = arg.GetString(0, "");
			BasePlayer basePlayer2 = arg.GetPlayerOrSleeperOrBot(1);
			if (basePlayer2 == null)
			{
				basePlayer2 = basePlayer;
			}
			if (basePlayer2 == null)
			{
				arg.ReplyWith("Could not find player " + arg.GetString(1, "") + " and no local player available");
				return;
			}
			Inventory.SavedLoadout savedLoadout;
			if (Inventory.LoadLoadout(@string, out savedLoadout))
			{
				savedLoadout.LoadItemsOnTo(basePlayer2);
				arg.ReplyWith("Deployed loadout " + @string + " to " + basePlayer2.displayName);
				return;
			}
			arg.ReplyWith("Could not find loadout " + @string);
		}

		// Token: 0x0600404B RID: 16459 RVA: 0x0017B144 File Offset: 0x00179344
		[ServerVar(Help = "Clears the inventory of a target player. eg. inventory.clearInventory jim")]
		public static void clearInventory(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				return;
			}
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			BasePlayer basePlayer2 = arg.GetPlayerOrSleeperOrBot(0);
			if (basePlayer2 == null)
			{
				basePlayer2 = basePlayer;
			}
			if (basePlayer2 == null)
			{
				arg.ReplyWith("Could not find player " + arg.GetString(1, "") + " and no local player available");
				return;
			}
			basePlayer2.inventory.containerBelt.Clear();
			basePlayer2.inventory.containerWear.Clear();
			basePlayer2.inventory.containerMain.Clear();
		}

		// Token: 0x0600404C RID: 16460 RVA: 0x0017B1E8 File Offset: 0x001793E8
		private static string GetLoadoutPath(string loadoutName)
		{
			return Server.GetServerFolder("loadouts") + "/" + loadoutName + ".ldt";
		}

		// Token: 0x0600404D RID: 16461 RVA: 0x0017B204 File Offset: 0x00179404
		[ServerVar(Help = "Saves the current equipped loadout of the calling player. eg. inventory.saveLoadout loaduoutname")]
		public static void saveloadout(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				return;
			}
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			string @string = arg.GetString(0, "");
			string contents = JsonConvert.SerializeObject(new Inventory.SavedLoadout(basePlayer), Formatting.Indented);
			string loadoutPath = Inventory.GetLoadoutPath(@string);
			File.WriteAllText(loadoutPath, contents);
			arg.ReplyWith("Saved loadout to " + loadoutPath);
		}

		// Token: 0x0600404E RID: 16462 RVA: 0x0017B274 File Offset: 0x00179474
		public static bool LoadLoadout(string name, out Inventory.SavedLoadout so)
		{
			so = new Inventory.SavedLoadout();
			string loadoutPath = Inventory.GetLoadoutPath(name);
			if (!File.Exists(loadoutPath))
			{
				return false;
			}
			so = JsonConvert.DeserializeObject<Inventory.SavedLoadout>(File.ReadAllText(loadoutPath));
			return so != null;
		}

		// Token: 0x0600404F RID: 16463 RVA: 0x0017B2B0 File Offset: 0x001794B0
		[ServerVar(Help = "Prints all saved inventory loadouts")]
		public static void listloadouts(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				return;
			}
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			string serverFolder = Server.GetServerFolder("loadouts");
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string value in Directory.EnumerateFiles(serverFolder))
			{
				stringBuilder.AppendLine(value);
			}
			arg.ReplyWith(stringBuilder.ToString());
		}

		// Token: 0x06004050 RID: 16464 RVA: 0x0017B344 File Offset: 0x00179544
		[ClientVar]
		[ServerVar]
		public static void defs(ConsoleSystem.Arg arg)
		{
			if (Steamworks.SteamInventory.Definitions == null)
			{
				arg.ReplyWith("no definitions");
				return;
			}
			if (Steamworks.SteamInventory.Definitions.Length == 0)
			{
				arg.ReplyWith("0 definitions");
				return;
			}
			string[] obj = (from x in Steamworks.SteamInventory.Definitions
			select x.Name).ToArray<string>();
			arg.ReplyWith(obj);
		}

		// Token: 0x06004051 RID: 16465 RVA: 0x0017B3AE File Offset: 0x001795AE
		[ClientVar]
		[ServerVar]
		public static void reloaddefs(ConsoleSystem.Arg arg)
		{
			Steamworks.SteamInventory.LoadItemDefinitions();
		}

		// Token: 0x06004052 RID: 16466 RVA: 0x0017B3B8 File Offset: 0x001795B8
		[ServerVar]
		public static void equipslottarget(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			if (basePlayer == null)
			{
				return;
			}
			BasePlayer lookingAtPlayer = RelationshipManager.GetLookingAtPlayer(basePlayer);
			if (lookingAtPlayer == null)
			{
				return;
			}
			int @int = arg.GetInt(0, 0);
			Inventory.EquipItemInSlot(lookingAtPlayer, @int);
			arg.ReplyWith(string.Format("Equipped slot {0} on player {1}", @int, lookingAtPlayer.displayName));
		}

		// Token: 0x06004053 RID: 16467 RVA: 0x0017B42C File Offset: 0x0017962C
		[ServerVar]
		public static void equipslot(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			if (basePlayer == null)
			{
				return;
			}
			BasePlayer basePlayer2 = null;
			if (arg.HasArgs(2))
			{
				basePlayer2 = arg.GetPlayer(1);
				if (basePlayer2 == null)
				{
					uint @uint = arg.GetUInt(1, 0U);
					basePlayer2 = BasePlayer.FindByID((ulong)@uint);
					if (basePlayer2 == null)
					{
						basePlayer2 = BasePlayer.FindBot((ulong)@uint);
					}
				}
			}
			if (basePlayer2 == null)
			{
				return;
			}
			int @int = arg.GetInt(0, 0);
			Inventory.EquipItemInSlot(basePlayer2, @int);
			Debug.Log(string.Format("Equipped slot {0} on player {1}", @int, basePlayer2.displayName));
		}

		// Token: 0x06004054 RID: 16468 RVA: 0x0017B4D8 File Offset: 0x001796D8
		private static void EquipItemInSlot(BasePlayer player, int slot)
		{
			uint itemID = 0U;
			for (int i = 0; i < player.inventory.containerBelt.itemList.Count; i++)
			{
				if (player.inventory.containerBelt.itemList[i] != null && i == slot)
				{
					itemID = player.inventory.containerBelt.itemList[i].uid;
					break;
				}
			}
			player.UpdateActiveItem(itemID);
		}

		// Token: 0x06004055 RID: 16469 RVA: 0x0017B54C File Offset: 0x0017974C
		private static int GetSlotIndex(BasePlayer player)
		{
			if (player.GetActiveItem() == null)
			{
				return -1;
			}
			uint uid = player.GetActiveItem().uid;
			for (int i = 0; i < player.inventory.containerBelt.itemList.Count; i++)
			{
				if (player.inventory.containerBelt.itemList[i] != null && player.inventory.containerBelt.itemList[i].uid == uid)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06004056 RID: 16470 RVA: 0x0017B5C8 File Offset: 0x001797C8
		[ServerVar]
		public static void giveBp(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer)
			{
				return;
			}
			ItemDefinition itemDefinition = ItemManager.FindDefinitionByPartialName(arg.GetString(0, ""), 1, 0UL);
			if (itemDefinition == null)
			{
				arg.ReplyWith("Could not find item: " + arg.GetString(0, ""));
				return;
			}
			if (itemDefinition.Blueprint == null)
			{
				arg.ReplyWith(itemDefinition.shortname + " has no blueprint!");
				return;
			}
			Item item = ItemManager.Create(ItemManager.blueprintBaseDef, 1, 0UL);
			item.blueprintTarget = itemDefinition.itemid;
			item.OnVirginSpawn();
			if (!basePlayer.inventory.GiveItem(item, null))
			{
				item.Remove(0f);
				arg.ReplyWith("Couldn't give item (inventory full?)");
				return;
			}
			basePlayer.Command("note.inv", new object[]
			{
				item.info.itemid,
				1
			});
			Debug.Log(string.Concat(new string[]
			{
				"giving ",
				basePlayer.displayName,
				" 1 x ",
				item.blueprintTargetDef.shortname,
				" blueprint"
			}));
			if (basePlayer.IsDeveloper)
			{
				basePlayer.ChatMessage("you silently gave yourself 1 x " + item.blueprintTargetDef.shortname + " blueprint");
				return;
			}
			Chat.Broadcast(basePlayer.displayName + " gave themselves 1 x " + item.blueprintTargetDef.shortname + " blueprint", "SERVER", "#eee", 0UL);
		}

		// Token: 0x04003970 RID: 14704
		private const string LoadoutDirectory = "loadouts";

		// Token: 0x04003971 RID: 14705
		[ServerVar(Help = "Disables all attire limitations, so NPC clothing and invalid overlaps can be equipped")]
		public static bool disableAttireLimitations;

		// Token: 0x02000EFE RID: 3838
		public class SavedLoadout
		{
			// Token: 0x060051B8 RID: 20920 RVA: 0x00007D7F File Offset: 0x00005F7F
			public SavedLoadout()
			{
			}

			// Token: 0x060051B9 RID: 20921 RVA: 0x001A511A File Offset: 0x001A331A
			public SavedLoadout(BasePlayer player)
			{
				this.belt = Inventory.SavedLoadout.SaveItems(player.inventory.containerBelt);
				this.wear = Inventory.SavedLoadout.SaveItems(player.inventory.containerWear);
				this.heldItemIndex = Inventory.GetSlotIndex(player);
			}

			// Token: 0x060051BA RID: 20922 RVA: 0x001A515C File Offset: 0x001A335C
			private static Inventory.SavedLoadout.SavedItem[] SaveItems(ItemContainer itemContainer)
			{
				List<Inventory.SavedLoadout.SavedItem> list = new List<Inventory.SavedLoadout.SavedItem>();
				for (int i = 0; i < itemContainer.capacity; i++)
				{
					Item slot = itemContainer.GetSlot(i);
					if (slot != null)
					{
						Inventory.SavedLoadout.SavedItem item = new Inventory.SavedLoadout.SavedItem
						{
							id = slot.info.itemid,
							amount = slot.amount,
							skin = slot.skin
						};
						if (slot.contents != null && slot.contents.itemList != null)
						{
							List<int> list2 = new List<int>();
							foreach (Item item2 in slot.contents.itemList)
							{
								list2.Add(item2.info.itemid);
							}
							item.containedItems = list2.ToArray();
						}
						list.Add(item);
					}
				}
				return list.ToArray();
			}

			// Token: 0x060051BB RID: 20923 RVA: 0x001A525C File Offset: 0x001A345C
			public void LoadItemsOnTo(BasePlayer player)
			{
				player.inventory.containerBelt.Clear();
				player.inventory.containerWear.Clear();
				foreach (Inventory.SavedLoadout.SavedItem item in this.belt)
				{
					player.inventory.GiveItem(this.LoadItem(item), player.inventory.containerBelt);
				}
				foreach (Inventory.SavedLoadout.SavedItem item2 in this.wear)
				{
					player.inventory.GiveItem(this.LoadItem(item2), player.inventory.containerWear);
				}
				Inventory.EquipItemInSlot(player, this.heldItemIndex);
			}

			// Token: 0x060051BC RID: 20924 RVA: 0x001A530C File Offset: 0x001A350C
			private Item LoadItem(Inventory.SavedLoadout.SavedItem item)
			{
				Item item2 = ItemManager.CreateByItemID(item.id, item.amount, item.skin);
				if (item.containedItems != null && item.containedItems.Length != 0)
				{
					foreach (int itemID in item.containedItems)
					{
						item2.contents.AddItem(ItemManager.FindItemDefinition(itemID), 1, 0UL, ItemContainer.LimitStack.Existing);
					}
				}
				return item2;
			}

			// Token: 0x04004CCD RID: 19661
			public Inventory.SavedLoadout.SavedItem[] belt;

			// Token: 0x04004CCE RID: 19662
			public Inventory.SavedLoadout.SavedItem[] wear;

			// Token: 0x04004CCF RID: 19663
			public int heldItemIndex;

			// Token: 0x02000F73 RID: 3955
			public struct SavedItem
			{
				// Token: 0x04004E58 RID: 20056
				public int id;

				// Token: 0x04004E59 RID: 20057
				public int amount;

				// Token: 0x04004E5A RID: 20058
				public ulong skin;

				// Token: 0x04004E5B RID: 20059
				public int[] containedItems;
			}
		}
	}
}
