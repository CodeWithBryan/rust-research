using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x0200011C RID: 284
public class InvisibleVendingMachine : NPCVendingMachine
{
	// Token: 0x060015A6 RID: 5542 RVA: 0x000A73A0 File Offset: 0x000A55A0
	public NPCShopKeeper GetNPCShopKeeper()
	{
		List<NPCShopKeeper> list = Pool.GetList<NPCShopKeeper>();
		Vis.Entities<NPCShopKeeper>(base.transform.position, 2f, list, 131072, QueryTriggerInteraction.Collide);
		NPCShopKeeper result = null;
		if (list.Count > 0)
		{
			result = list[0];
		}
		Pool.FreeList<NPCShopKeeper>(ref list);
		return result;
	}

	// Token: 0x060015A7 RID: 5543 RVA: 0x000A73EC File Offset: 0x000A55EC
	public void KeeperLookAt(Vector3 pos)
	{
		NPCShopKeeper npcshopKeeper = this.GetNPCShopKeeper();
		if (npcshopKeeper == null)
		{
			return;
		}
		npcshopKeeper.SetAimDirection(Vector3Ex.Direction2D(pos, npcshopKeeper.transform.position));
	}

	// Token: 0x060015A8 RID: 5544 RVA: 0x00007074 File Offset: 0x00005274
	public override bool HasVendingSounds()
	{
		return false;
	}

	// Token: 0x060015A9 RID: 5545 RVA: 0x00026238 File Offset: 0x00024438
	public override float GetBuyDuration()
	{
		return 0.5f;
	}

	// Token: 0x060015AA RID: 5546 RVA: 0x000A7424 File Offset: 0x000A5624
	public override void CompletePendingOrder()
	{
		Effect.server.Run(this.buyEffect.resourcePath, base.transform.position, Vector3.up, null, false);
		NPCShopKeeper npcshopKeeper = this.GetNPCShopKeeper();
		if (npcshopKeeper)
		{
			npcshopKeeper.SignalBroadcast(BaseEntity.Signal.Gesture, "victory", null);
			if (this.vend_Player != null)
			{
				npcshopKeeper.SetAimDirection(Vector3Ex.Direction2D(this.vend_Player.transform.position, npcshopKeeper.transform.position));
			}
		}
		base.CompletePendingOrder();
	}

	// Token: 0x060015AB RID: 5547 RVA: 0x000A74AA File Offset: 0x000A56AA
	public override bool PlayerOpenLoot(BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		this.KeeperLookAt(player.transform.position);
		return base.PlayerOpenLoot(player, panelToOpen, true);
	}

	// Token: 0x060015AC RID: 5548 RVA: 0x000A74C8 File Offset: 0x000A56C8
	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (this.vmoManifest != null && info.msg.vendingMachine != null)
		{
			info.msg.vendingMachine.vmoIndex = this.vmoManifest.GetIndex(this.vendingOrders);
		}
	}

	// Token: 0x060015AD RID: 5549 RVA: 0x000A7518 File Offset: 0x000A5718
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.vmoManifest.GetIndex(this.vendingOrders) == -1)
		{
			Debug.LogError("VENDING ORDERS NOT FOUND! Did you forget to add these orders to the VMOManifest?");
		}
	}

	// Token: 0x060015AE RID: 5550 RVA: 0x000A7540 File Offset: 0x000A5740
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk && this.vmoManifest != null && info.msg.vendingMachine != null)
		{
			if (info.msg.vendingMachine.vmoIndex == -1 && TerrainMeta.Path.Monuments != null)
			{
				foreach (MonumentInfo monumentInfo in TerrainMeta.Path.Monuments)
				{
					if (monumentInfo.displayPhrase.token.Contains("fish") && Vector3.Distance(monumentInfo.transform.position, base.transform.position) < 100f)
					{
						info.msg.vendingMachine.vmoIndex = 17;
					}
				}
			}
			NPCVendingOrder fromIndex = this.vmoManifest.GetFromIndex(info.msg.vendingMachine.vmoIndex);
			this.vendingOrders = fromIndex;
		}
	}

	// Token: 0x04000E09 RID: 3593
	public GameObjectRef buyEffect;

	// Token: 0x04000E0A RID: 3594
	public NPCVendingOrderManifest vmoManifest;
}
