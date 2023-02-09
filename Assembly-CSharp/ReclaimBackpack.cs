using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200012C RID: 300
public class ReclaimBackpack : StorageContainer
{
	// Token: 0x060015D9 RID: 5593 RVA: 0x000A7CDB File Offset: 0x000A5EDB
	public void InitForPlayer(ulong playerID, int newID)
	{
		this.playerSteamID = playerID;
		this.reclaimID = newID;
	}

	// Token: 0x060015DA RID: 5594 RVA: 0x000A7CEC File Offset: 0x000A5EEC
	public override void ServerInit()
	{
		base.ServerInit();
		base.inventory.SetFlag(global::ItemContainer.Flag.NoItemInput, true);
		base.Invoke(new Action(this.RemoveMe), global::ReclaimManager.reclaim_expire_minutes * 60f);
		base.InvokeRandomized(new Action(this.CheckEmpty), 1f, 30f, 3f);
	}

	// Token: 0x060015DB RID: 5595 RVA: 0x000029D4 File Offset: 0x00000BD4
	public void RemoveMe()
	{
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x060015DC RID: 5596 RVA: 0x000A7D4E File Offset: 0x000A5F4E
	public void CheckEmpty()
	{
		if (global::ReclaimManager.instance.GetReclaimForPlayer(this.playerSteamID, this.reclaimID) == null && !this.isBeingLooted)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x060015DD RID: 5597 RVA: 0x000A7D78 File Offset: 0x000A5F78
	public override bool OnStartBeingLooted(global::BasePlayer baseEntity)
	{
		if (baseEntity.InSafeZone() && baseEntity.userID != this.playerSteamID)
		{
			return false;
		}
		if (this.onlyOwnerLoot && baseEntity.userID != this.playerSteamID)
		{
			return false;
		}
		global::ReclaimManager.PlayerReclaimEntry reclaimForPlayer = global::ReclaimManager.instance.GetReclaimForPlayer(baseEntity.userID, this.reclaimID);
		if (reclaimForPlayer != null)
		{
			for (int i = reclaimForPlayer.inventory.itemList.Count - 1; i >= 0; i--)
			{
				reclaimForPlayer.inventory.itemList[i].MoveToContainer(base.inventory, -1, true, false, null, true);
			}
			global::ReclaimManager.instance.RemoveEntry(reclaimForPlayer);
		}
		this.isBeingLooted = true;
		return base.OnStartBeingLooted(baseEntity);
	}

	// Token: 0x060015DE RID: 5598 RVA: 0x000A7E28 File Offset: 0x000A6028
	public override void PlayerStoppedLooting(global::BasePlayer player)
	{
		base.PlayerStoppedLooting(player);
		this.isBeingLooted = false;
		if (base.inventory.itemList.Count > 0)
		{
			global::ReclaimManager.instance.AddPlayerReclaim(this.playerSteamID, base.inventory.itemList, 0UL, "", this.reclaimID);
		}
	}

	// Token: 0x060015DF RID: 5599 RVA: 0x000A7E80 File Offset: 0x000A6080
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.lootableCorpse = Pool.Get<ProtoBuf.LootableCorpse>();
		info.msg.lootableCorpse.playerID = this.playerSteamID;
		info.msg.lootableCorpse.underwearSkin = (uint)this.reclaimID;
	}

	// Token: 0x060015E0 RID: 5600 RVA: 0x000A7ED0 File Offset: 0x000A60D0
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.lootableCorpse != null)
		{
			this.playerSteamID = info.msg.lootableCorpse.playerID;
			this.reclaimID = (int)info.msg.lootableCorpse.underwearSkin;
		}
	}

	// Token: 0x04000E4F RID: 3663
	public int reclaimID;

	// Token: 0x04000E50 RID: 3664
	public ulong playerSteamID;

	// Token: 0x04000E51 RID: 3665
	public bool onlyOwnerLoot = true;

	// Token: 0x04000E52 RID: 3666
	public Collider myCollider;

	// Token: 0x04000E53 RID: 3667
	public GameObject art;

	// Token: 0x04000E54 RID: 3668
	private bool isBeingLooted;
}
