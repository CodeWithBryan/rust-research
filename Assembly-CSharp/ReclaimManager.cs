using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x020004EB RID: 1259
public class ReclaimManager : global::BaseEntity
{
	// Token: 0x1700032A RID: 810
	// (get) Token: 0x060027F5 RID: 10229 RVA: 0x000F4B96 File Offset: 0x000F2D96
	public static global::ReclaimManager instance
	{
		get
		{
			return global::ReclaimManager._instance;
		}
	}

	// Token: 0x060027F6 RID: 10230 RVA: 0x000F4BA0 File Offset: 0x000F2DA0
	public int AddPlayerReclaim(ulong victimID, List<global::Item> itemList, ulong killerID = 0UL, string killerString = "", int reclaimIDToUse = -1)
	{
		global::ReclaimManager.PlayerReclaimEntry playerReclaimEntry = this.NewEntry();
		for (int i = itemList.Count - 1; i >= 0; i--)
		{
			itemList[i].MoveToContainer(playerReclaimEntry.inventory, -1, true, false, null, true);
		}
		if (reclaimIDToUse == -1)
		{
			this.lastReclaimID++;
			reclaimIDToUse = this.lastReclaimID;
		}
		playerReclaimEntry.victimID = victimID;
		playerReclaimEntry.killerID = killerID;
		playerReclaimEntry.killerString = killerString;
		playerReclaimEntry.id = reclaimIDToUse;
		this.entries.Add(playerReclaimEntry);
		return reclaimIDToUse;
	}

	// Token: 0x060027F7 RID: 10231 RVA: 0x000F4C28 File Offset: 0x000F2E28
	public void DoCleanup()
	{
		for (int i = this.entries.Count - 1; i >= 0; i--)
		{
			global::ReclaimManager.PlayerReclaimEntry playerReclaimEntry = this.entries[i];
			if (playerReclaimEntry.inventory.itemList.Count == 0 || playerReclaimEntry.timeAlive / 60f > global::ReclaimManager.reclaim_expire_minutes)
			{
				this.RemoveEntry(playerReclaimEntry);
			}
		}
	}

	// Token: 0x060027F8 RID: 10232 RVA: 0x000F4C88 File Offset: 0x000F2E88
	public void TickEntries()
	{
		float num = Time.realtimeSinceStartup - this.lastTickTime;
		foreach (global::ReclaimManager.PlayerReclaimEntry playerReclaimEntry in this.entries)
		{
			playerReclaimEntry.timeAlive += num;
		}
		this.lastTickTime = Time.realtimeSinceStartup;
		this.DoCleanup();
	}

	// Token: 0x060027F9 RID: 10233 RVA: 0x000F4D00 File Offset: 0x000F2F00
	public bool HasReclaims(ulong playerID)
	{
		foreach (global::ReclaimManager.PlayerReclaimEntry playerReclaimEntry in this.entries)
		{
			if (playerReclaimEntry.victimID == playerID && playerReclaimEntry.inventory.itemList.Count > 0)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060027FA RID: 10234 RVA: 0x000F4D70 File Offset: 0x000F2F70
	public global::ReclaimManager.PlayerReclaimEntry GetReclaimForPlayer(ulong playerID, int reclaimID)
	{
		foreach (global::ReclaimManager.PlayerReclaimEntry playerReclaimEntry in this.entries)
		{
			if (playerReclaimEntry.victimID == playerID && playerReclaimEntry.id == reclaimID)
			{
				return playerReclaimEntry;
			}
		}
		return null;
	}

	// Token: 0x060027FB RID: 10235 RVA: 0x000F4DD8 File Offset: 0x000F2FD8
	public bool GetReclaimsForPlayer(ulong playerID, ref List<global::ReclaimManager.PlayerReclaimEntry> list)
	{
		foreach (global::ReclaimManager.PlayerReclaimEntry playerReclaimEntry in this.entries)
		{
			if (playerReclaimEntry.victimID == playerID)
			{
				list.Add(playerReclaimEntry);
			}
		}
		return list.Count > 0;
	}

	// Token: 0x060027FC RID: 10236 RVA: 0x000F4E40 File Offset: 0x000F3040
	public global::ReclaimManager.PlayerReclaimEntry NewEntry()
	{
		global::ReclaimManager.PlayerReclaimEntry playerReclaimEntry = Pool.Get<global::ReclaimManager.PlayerReclaimEntry>();
		playerReclaimEntry.Init();
		return playerReclaimEntry;
	}

	// Token: 0x060027FD RID: 10237 RVA: 0x000F4E4D File Offset: 0x000F304D
	public void RemoveEntry(global::ReclaimManager.PlayerReclaimEntry entry)
	{
		entry.Cleanup();
		this.entries.Remove(entry);
		Pool.Free<global::ReclaimManager.PlayerReclaimEntry>(ref entry);
		entry = null;
	}

	// Token: 0x060027FE RID: 10238 RVA: 0x000F4E6C File Offset: 0x000F306C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk && info.msg.reclaimManager != null)
		{
			this.lastReclaimID = info.msg.reclaimManager.lastReclaimID;
			foreach (ProtoBuf.ReclaimManager.ReclaimInfo reclaimInfo in info.msg.reclaimManager.reclaimEntries)
			{
				global::ReclaimManager.PlayerReclaimEntry playerReclaimEntry = this.NewEntry();
				playerReclaimEntry.killerID = reclaimInfo.killerID;
				playerReclaimEntry.victimID = reclaimInfo.victimID;
				playerReclaimEntry.killerString = reclaimInfo.killerString;
				playerReclaimEntry.inventory.Load(reclaimInfo.inventory);
				this.entries.Add(playerReclaimEntry);
			}
		}
	}

	// Token: 0x060027FF RID: 10239 RVA: 0x000F4F44 File Offset: 0x000F3144
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			info.msg.reclaimManager = Pool.Get<ProtoBuf.ReclaimManager>();
			info.msg.reclaimManager.reclaimEntries = Pool.GetList<ProtoBuf.ReclaimManager.ReclaimInfo>();
			info.msg.reclaimManager.lastReclaimID = this.lastReclaimID;
			foreach (global::ReclaimManager.PlayerReclaimEntry playerReclaimEntry in this.entries)
			{
				ProtoBuf.ReclaimManager.ReclaimInfo reclaimInfo = Pool.Get<ProtoBuf.ReclaimManager.ReclaimInfo>();
				reclaimInfo.killerID = playerReclaimEntry.killerID;
				reclaimInfo.victimID = playerReclaimEntry.victimID;
				reclaimInfo.killerString = playerReclaimEntry.killerString;
				reclaimInfo.inventory = playerReclaimEntry.inventory.Save();
				info.msg.reclaimManager.reclaimEntries.Add(reclaimInfo);
			}
		}
	}

	// Token: 0x06002800 RID: 10240 RVA: 0x000F5030 File Offset: 0x000F3230
	public override void ServerInit()
	{
		base.InvokeRepeating(new Action(this.TickEntries), 1f, 60f);
		global::ReclaimManager._instance = this;
		base.ServerInit();
	}

	// Token: 0x06002801 RID: 10241 RVA: 0x000F505A File Offset: 0x000F325A
	internal override void DoServerDestroy()
	{
		global::ReclaimManager._instance = null;
		base.DoServerDestroy();
	}

	// Token: 0x04002026 RID: 8230
	private const int defaultReclaims = 128;

	// Token: 0x04002027 RID: 8231
	private const int reclaimSlotCount = 40;

	// Token: 0x04002028 RID: 8232
	private int lastReclaimID;

	// Token: 0x04002029 RID: 8233
	[ServerVar]
	public static float reclaim_expire_minutes = 120f;

	// Token: 0x0400202A RID: 8234
	private static global::ReclaimManager _instance;

	// Token: 0x0400202B RID: 8235
	public List<global::ReclaimManager.PlayerReclaimEntry> entries = new List<global::ReclaimManager.PlayerReclaimEntry>();

	// Token: 0x0400202C RID: 8236
	private float lastTickTime;

	// Token: 0x02000CDD RID: 3293
	public class PlayerReclaimEntry
	{
		// Token: 0x06004DA8 RID: 19880 RVA: 0x0019902C File Offset: 0x0019722C
		public void Init()
		{
			this.inventory = Pool.Get<global::ItemContainer>();
			this.inventory.entityOwner = global::ReclaimManager.instance;
			this.inventory.allowedContents = global::ItemContainer.ContentsType.Generic;
			this.inventory.SetOnlyAllowedItem(null);
			this.inventory.maxStackSize = 0;
			this.inventory.ServerInitialize(null, 40);
			this.inventory.canAcceptItem = null;
			this.inventory.GiveUID();
		}

		// Token: 0x06004DA9 RID: 19881 RVA: 0x001990A0 File Offset: 0x001972A0
		public void Cleanup()
		{
			this.timeAlive = 0f;
			this.killerID = 0UL;
			this.killerString = "";
			this.victimID = 0UL;
			this.id = -2;
			if (this.inventory != null)
			{
				this.inventory.Clear();
				Pool.Free<global::ItemContainer>(ref this.inventory);
			}
		}

		// Token: 0x0400440C RID: 17420
		public ulong killerID;

		// Token: 0x0400440D RID: 17421
		public string killerString;

		// Token: 0x0400440E RID: 17422
		public ulong victimID;

		// Token: 0x0400440F RID: 17423
		public float timeAlive;

		// Token: 0x04004410 RID: 17424
		public int id;

		// Token: 0x04004411 RID: 17425
		public global::ItemContainer inventory;
	}
}
