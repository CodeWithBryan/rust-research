using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000409 RID: 1033
public class WhitelistLootContainer : LootContainer
{
	// Token: 0x060022AD RID: 8877 RVA: 0x000DDD58 File Offset: 0x000DBF58
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			info.msg.whitelist = Pool.Get<Whitelist>();
			info.msg.whitelist.users = Pool.GetList<ulong>();
			foreach (ulong num in this.whitelist)
			{
				info.msg.whitelist.users.Add(num);
				Debug.Log("Whitelistcontainer saving user " + num);
			}
		}
	}

	// Token: 0x060022AE RID: 8878 RVA: 0x000DDE04 File Offset: 0x000DC004
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		if (info.fromDisk && info.msg.whitelist != null)
		{
			foreach (ulong item in info.msg.whitelist.users)
			{
				this.whitelist.Add(item);
			}
		}
		base.Load(info);
	}

	// Token: 0x060022AF RID: 8879 RVA: 0x000DDE84 File Offset: 0x000DC084
	public void MissionSetupPlayer(global::BasePlayer player)
	{
		this.AddToWhitelist(player.userID);
	}

	// Token: 0x060022B0 RID: 8880 RVA: 0x000DDE92 File Offset: 0x000DC092
	public void AddToWhitelist(ulong userid)
	{
		if (!this.whitelist.Contains(userid))
		{
			this.whitelist.Add(userid);
		}
	}

	// Token: 0x060022B1 RID: 8881 RVA: 0x000DDEAE File Offset: 0x000DC0AE
	public void RemoveFromWhitelist(ulong userid)
	{
		if (this.whitelist.Contains(userid))
		{
			this.whitelist.Remove(userid);
		}
	}

	// Token: 0x060022B2 RID: 8882 RVA: 0x000DDECC File Offset: 0x000DC0CC
	public override bool PlayerOpenLoot(global::BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		ulong userID = player.userID;
		if (!this.whitelist.Contains(userID))
		{
			player.ShowToast(GameTip.Styles.Red_Normal, WhitelistLootContainer.CantLootToast, Array.Empty<string>());
			return false;
		}
		return base.PlayerOpenLoot(player, panelToOpen, doPositionChecks);
	}

	// Token: 0x04001B1C RID: 6940
	public static readonly Translate.Phrase CantLootToast = new Translate.Phrase("whitelistcontainer.noloot", "You are not authorized to access this box");

	// Token: 0x04001B1D RID: 6941
	public List<ulong> whitelist = new List<ulong>();
}
