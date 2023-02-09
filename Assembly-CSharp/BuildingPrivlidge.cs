using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200004C RID: 76
public class BuildingPrivlidge : StorageContainer
{
	// Token: 0x0600089A RID: 2202 RVA: 0x00052300 File Offset: 0x00050500
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BuildingPrivlidge.OnRpcMessage", 0))
		{
			if (rpc == 1092560690U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - AddSelfAuthorize ");
				}
				using (TimeWarning.New("AddSelfAuthorize", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1092560690U, "AddSelfAuthorize", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.AddSelfAuthorize(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in AddSelfAuthorize");
					}
				}
				return true;
			}
			if (rpc == 253307592U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ClearList ");
				}
				using (TimeWarning.New("ClearList", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(253307592U, "ClearList", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ClearList(rpc3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in ClearList");
					}
				}
				return true;
			}
			if (rpc == 3617985969U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RemoveSelfAuthorize ");
				}
				using (TimeWarning.New("RemoveSelfAuthorize", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(3617985969U, "RemoveSelfAuthorize", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc4 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RemoveSelfAuthorize(rpc4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in RemoveSelfAuthorize");
					}
				}
				return true;
			}
			if (rpc == 2051750736U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Rotate ");
				}
				using (TimeWarning.New("RPC_Rotate", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2051750736U, "RPC_Rotate", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Rotate(msg2);
						}
					}
					catch (Exception exception4)
					{
						Debug.LogException(exception4);
						player.Kick("RPC Error in RPC_Rotate");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600089B RID: 2203 RVA: 0x000528B8 File Offset: 0x00050AB8
	public float CalculateUpkeepPeriodMinutes()
	{
		if (base.isServer)
		{
			return ConVar.Decay.upkeep_period_minutes;
		}
		return 0f;
	}

	// Token: 0x0600089C RID: 2204 RVA: 0x000528CD File Offset: 0x00050ACD
	public float CalculateUpkeepCostFraction()
	{
		if (base.isServer)
		{
			return this.CalculateBuildingTaxRate();
		}
		return 0f;
	}

	// Token: 0x0600089D RID: 2205 RVA: 0x000528E4 File Offset: 0x00050AE4
	public void CalculateUpkeepCostAmounts(List<ItemAmount> itemAmounts)
	{
		BuildingManager.Building building = base.GetBuilding();
		if (building == null)
		{
			return;
		}
		if (!building.HasDecayEntities())
		{
			return;
		}
		float multiplier = this.CalculateUpkeepCostFraction();
		foreach (global::DecayEntity decayEntity in building.decayEntities)
		{
			decayEntity.CalculateUpkeepCostAmounts(itemAmounts, multiplier);
		}
	}

	// Token: 0x0600089E RID: 2206 RVA: 0x00052954 File Offset: 0x00050B54
	public float GetProtectedMinutes(bool force = false)
	{
		if (!base.isServer)
		{
			return 0f;
		}
		if (!force && UnityEngine.Time.realtimeSinceStartup < this.nextProtectedCalcTime)
		{
			return this.cachedProtectedMinutes;
		}
		this.nextProtectedCalcTime = UnityEngine.Time.realtimeSinceStartup + 60f;
		List<ItemAmount> list = Facepunch.Pool.GetList<ItemAmount>();
		this.CalculateUpkeepCostAmounts(list);
		float num = this.CalculateUpkeepPeriodMinutes();
		float num2 = -1f;
		if (base.inventory != null)
		{
			foreach (ItemAmount itemAmount in list)
			{
				int num3 = base.inventory.FindItemsByItemID(itemAmount.itemid).Sum((global::Item x) => x.amount);
				if (num3 > 0 && itemAmount.amount > 0f)
				{
					float num4 = (float)num3 / itemAmount.amount * num;
					if (num2 == -1f || num4 < num2)
					{
						num2 = num4;
					}
				}
				else
				{
					num2 = 0f;
				}
			}
			if (num2 == -1f)
			{
				num2 = 0f;
			}
		}
		Facepunch.Pool.FreeList<ItemAmount>(ref list);
		this.cachedProtectedMinutes = num2;
		return this.cachedProtectedMinutes;
	}

	// Token: 0x0600089F RID: 2207 RVA: 0x00052A94 File Offset: 0x00050C94
	public override void OnKilled(HitInfo info)
	{
		if (ConVar.Decay.upkeep_grief_protection > 0f)
		{
			this.PurchaseUpkeepTime(ConVar.Decay.upkeep_grief_protection * 60f);
		}
		base.OnKilled(info);
	}

	// Token: 0x060008A0 RID: 2208 RVA: 0x00052ABA File Offset: 0x00050CBA
	public override void DecayTick()
	{
		if (this.EnsurePrimary())
		{
			base.DecayTick();
		}
	}

	// Token: 0x060008A1 RID: 2209 RVA: 0x00052ACC File Offset: 0x00050CCC
	private bool EnsurePrimary()
	{
		BuildingManager.Building building = base.GetBuilding();
		if (building != null)
		{
			BuildingPrivlidge dominatingBuildingPrivilege = building.GetDominatingBuildingPrivilege();
			if (dominatingBuildingPrivilege != null && dominatingBuildingPrivilege != this)
			{
				base.Kill(global::BaseNetworkable.DestroyMode.Gib);
				return false;
			}
		}
		return true;
	}

	// Token: 0x060008A2 RID: 2210 RVA: 0x00052B06 File Offset: 0x00050D06
	public void MarkProtectedMinutesDirty(float delay = 0f)
	{
		this.nextProtectedCalcTime = UnityEngine.Time.realtimeSinceStartup + delay;
	}

	// Token: 0x060008A3 RID: 2211 RVA: 0x00052B18 File Offset: 0x00050D18
	private float CalculateBuildingTaxRate()
	{
		BuildingManager.Building building = base.GetBuilding();
		if (building == null)
		{
			return ConVar.Decay.bracket_0_costfraction;
		}
		if (!building.HasBuildingBlocks())
		{
			return ConVar.Decay.bracket_0_costfraction;
		}
		int count = building.buildingBlocks.Count;
		int num = count;
		for (int i = 0; i < BuildingPrivlidge.upkeepBrackets.Length; i++)
		{
			BuildingPrivlidge.UpkeepBracket upkeepBracket = BuildingPrivlidge.upkeepBrackets[i];
			upkeepBracket.blocksTaxPaid = 0f;
			if (num > 0)
			{
				int num2;
				if (i == BuildingPrivlidge.upkeepBrackets.Length - 1)
				{
					num2 = num;
				}
				else
				{
					num2 = Mathf.Min(num, BuildingPrivlidge.upkeepBrackets[i].objectsUpTo);
				}
				num -= num2;
				upkeepBracket.blocksTaxPaid = (float)num2 * upkeepBracket.fraction;
			}
		}
		float num3 = 0f;
		for (int j = 0; j < BuildingPrivlidge.upkeepBrackets.Length; j++)
		{
			BuildingPrivlidge.UpkeepBracket upkeepBracket2 = BuildingPrivlidge.upkeepBrackets[j];
			if (upkeepBracket2.blocksTaxPaid <= 0f)
			{
				break;
			}
			num3 += upkeepBracket2.blocksTaxPaid;
		}
		return num3 / (float)count;
	}

	// Token: 0x060008A4 RID: 2212 RVA: 0x00052C08 File Offset: 0x00050E08
	private void ApplyUpkeepPayment()
	{
		List<global::Item> list = Facepunch.Pool.GetList<global::Item>();
		for (int i = 0; i < this.upkeepBuffer.Count; i++)
		{
			ItemAmount itemAmount = this.upkeepBuffer[i];
			int num = (int)itemAmount.amount;
			if (num >= 1)
			{
				base.inventory.Take(list, itemAmount.itemid, num);
				foreach (global::Item item in list)
				{
					if (this.IsDebugging())
					{
						Debug.Log(string.Concat(new object[]
						{
							this.ToString(),
							": Using ",
							item.amount,
							" of ",
							item.info.shortname
						}));
					}
					item.UseItem(item.amount);
				}
				list.Clear();
				itemAmount.amount -= (float)num;
				this.upkeepBuffer[i] = itemAmount;
			}
		}
		Facepunch.Pool.FreeList<global::Item>(ref list);
	}

	// Token: 0x060008A5 RID: 2213 RVA: 0x00052D2C File Offset: 0x00050F2C
	private void QueueUpkeepPayment(List<ItemAmount> itemAmounts)
	{
		for (int i = 0; i < itemAmounts.Count; i++)
		{
			ItemAmount itemAmount = itemAmounts[i];
			bool flag = false;
			foreach (ItemAmount itemAmount2 in this.upkeepBuffer)
			{
				if (itemAmount2.itemDef == itemAmount.itemDef)
				{
					itemAmount2.amount += itemAmount.amount;
					if (this.IsDebugging())
					{
						Debug.Log(string.Concat(new object[]
						{
							this.ToString(),
							": Adding ",
							itemAmount.amount,
							" of ",
							itemAmount.itemDef.shortname,
							" to ",
							itemAmount2.amount
						}));
					}
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				if (this.IsDebugging())
				{
					Debug.Log(string.Concat(new object[]
					{
						this.ToString(),
						": Adding ",
						itemAmount.amount,
						" of ",
						itemAmount.itemDef.shortname
					}));
				}
				this.upkeepBuffer.Add(new ItemAmount(itemAmount.itemDef, itemAmount.amount));
			}
		}
	}

	// Token: 0x060008A6 RID: 2214 RVA: 0x00052E9C File Offset: 0x0005109C
	private bool CanAffordUpkeepPayment(List<ItemAmount> itemAmounts)
	{
		for (int i = 0; i < itemAmounts.Count; i++)
		{
			ItemAmount itemAmount = itemAmounts[i];
			if ((float)base.inventory.GetAmount(itemAmount.itemid, true) < itemAmount.amount)
			{
				if (this.IsDebugging())
				{
					Debug.Log(string.Concat(new object[]
					{
						this.ToString(),
						": Can't afford ",
						itemAmount.amount,
						" of ",
						itemAmount.itemDef.shortname
					}));
				}
				return false;
			}
		}
		return true;
	}

	// Token: 0x060008A7 RID: 2215 RVA: 0x00052F30 File Offset: 0x00051130
	public float PurchaseUpkeepTime(global::DecayEntity entity, float deltaTime)
	{
		float num = this.CalculateUpkeepCostFraction();
		float num2 = this.CalculateUpkeepPeriodMinutes() * 60f;
		float multiplier = num * deltaTime / num2;
		List<ItemAmount> list = Facepunch.Pool.GetList<ItemAmount>();
		entity.CalculateUpkeepCostAmounts(list, multiplier);
		bool flag = this.CanAffordUpkeepPayment(list);
		this.QueueUpkeepPayment(list);
		Facepunch.Pool.FreeList<ItemAmount>(ref list);
		this.ApplyUpkeepPayment();
		if (!flag)
		{
			return 0f;
		}
		return deltaTime;
	}

	// Token: 0x060008A8 RID: 2216 RVA: 0x00052F88 File Offset: 0x00051188
	public void PurchaseUpkeepTime(float deltaTime)
	{
		BuildingManager.Building building = base.GetBuilding();
		if (building != null && building.HasDecayEntities())
		{
			float num = Mathf.Min(this.GetProtectedMinutes(true) * 60f, deltaTime);
			if (num > 0f)
			{
				foreach (global::DecayEntity decayEntity in building.decayEntities)
				{
					float protectedSeconds = decayEntity.GetProtectedSeconds();
					if (num > protectedSeconds)
					{
						float num2 = this.PurchaseUpkeepTime(decayEntity, num - protectedSeconds);
						decayEntity.AddUpkeepTime(num2);
						if (this.IsDebugging())
						{
							Debug.Log(string.Concat(new object[]
							{
								this.ToString(),
								" purchased upkeep time for ",
								decayEntity.ToString(),
								": ",
								protectedSeconds,
								" + ",
								num2,
								" = ",
								decayEntity.GetProtectedSeconds()
							}));
						}
					}
				}
			}
		}
	}

	// Token: 0x060008A9 RID: 2217 RVA: 0x000530A4 File Offset: 0x000512A4
	public override void ResetState()
	{
		base.ResetState();
		this.authorizedPlayers.Clear();
	}

	// Token: 0x060008AA RID: 2218 RVA: 0x000530B8 File Offset: 0x000512B8
	public bool IsAuthed(global::BasePlayer player)
	{
		return this.authorizedPlayers.Any((PlayerNameID x) => x.userid == player.userID);
	}

	// Token: 0x060008AB RID: 2219 RVA: 0x000530EC File Offset: 0x000512EC
	public bool IsAuthed(ulong userID)
	{
		return this.authorizedPlayers.Any((PlayerNameID x) => x.userid == userID);
	}

	// Token: 0x060008AC RID: 2220 RVA: 0x0005311D File Offset: 0x0005131D
	public bool AnyAuthed()
	{
		return this.authorizedPlayers.Count > 0;
	}

	// Token: 0x060008AD RID: 2221 RVA: 0x00053130 File Offset: 0x00051330
	public override bool ItemFilter(global::Item item, int targetSlot)
	{
		bool flag = this.allowedConstructionItems.Contains(item.info);
		if (!flag && targetSlot == -1)
		{
			int num = 0;
			foreach (global::Item item2 in base.inventory.itemList)
			{
				if (!this.allowedConstructionItems.Contains(item2.info) && (item2.info != item.info || item2.amount == item2.MaxStackable()))
				{
					num++;
				}
			}
			if (num >= 24)
			{
				return false;
			}
		}
		if (targetSlot >= 24 && targetSlot <= 27)
		{
			return flag;
		}
		return base.ItemFilter(item, targetSlot);
	}

	// Token: 0x060008AE RID: 2222 RVA: 0x000531F0 File Offset: 0x000513F0
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.buildingPrivilege = Facepunch.Pool.Get<BuildingPrivilege>();
		info.msg.buildingPrivilege.users = this.authorizedPlayers;
		if (!info.forDisk)
		{
			info.msg.buildingPrivilege.upkeepPeriodMinutes = this.CalculateUpkeepPeriodMinutes();
			info.msg.buildingPrivilege.costFraction = this.CalculateUpkeepCostFraction();
			info.msg.buildingPrivilege.protectedMinutes = this.GetProtectedMinutes(false);
		}
	}

	// Token: 0x060008AF RID: 2223 RVA: 0x00053275 File Offset: 0x00051475
	public override void PostSave(global::BaseNetworkable.SaveInfo info)
	{
		info.msg.buildingPrivilege.users = null;
	}

	// Token: 0x060008B0 RID: 2224 RVA: 0x00053288 File Offset: 0x00051488
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		this.authorizedPlayers.Clear();
		if (info.msg.buildingPrivilege != null && info.msg.buildingPrivilege.users != null)
		{
			this.authorizedPlayers = info.msg.buildingPrivilege.users;
			if (!info.fromDisk)
			{
				this.cachedProtectedMinutes = info.msg.buildingPrivilege.protectedMinutes;
			}
			info.msg.buildingPrivilege.users = null;
		}
	}

	// Token: 0x060008B1 RID: 2225 RVA: 0x0005330B File Offset: 0x0005150B
	public void BuildingDirty()
	{
		if (base.isServer)
		{
			this.AddDelayedUpdate();
		}
	}

	// Token: 0x060008B2 RID: 2226 RVA: 0x000035F8 File Offset: 0x000017F8
	public bool AtMaxAuthCapacity()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved5);
	}

	// Token: 0x060008B3 RID: 2227 RVA: 0x0005331C File Offset: 0x0005151C
	public void UpdateMaxAuthCapacity()
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		if (activeGameMode && activeGameMode.limitTeamAuths)
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved5, this.authorizedPlayers.Count >= activeGameMode.GetMaxRelationshipTeamSize(), false, true);
		}
	}

	// Token: 0x060008B4 RID: 2228 RVA: 0x00053363 File Offset: 0x00051563
	protected override void OnInventoryDirty()
	{
		base.OnInventoryDirty();
		this.AddDelayedUpdate();
	}

	// Token: 0x060008B5 RID: 2229 RVA: 0x00053371 File Offset: 0x00051571
	public override void OnItemAddedOrRemoved(global::Item item, bool bAdded)
	{
		base.OnItemAddedOrRemoved(item, bAdded);
		this.AddDelayedUpdate();
	}

	// Token: 0x060008B6 RID: 2230 RVA: 0x00053381 File Offset: 0x00051581
	public void AddDelayedUpdate()
	{
		if (base.IsInvoking(new Action(this.DelayedUpdate)))
		{
			base.CancelInvoke(new Action(this.DelayedUpdate));
		}
		base.Invoke(new Action(this.DelayedUpdate), 1f);
	}

	// Token: 0x060008B7 RID: 2231 RVA: 0x000533C0 File Offset: 0x000515C0
	public void DelayedUpdate()
	{
		this.MarkProtectedMinutesDirty(0f);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060008B8 RID: 2232 RVA: 0x000533D4 File Offset: 0x000515D4
	public bool CanAdministrate(global::BasePlayer player)
	{
		BaseLock baseLock = base.GetSlot(global::BaseEntity.Slot.Lock) as BaseLock;
		return baseLock == null || baseLock.OnTryToOpen(player);
	}

	// Token: 0x060008B9 RID: 2233 RVA: 0x00053400 File Offset: 0x00051600
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void AddSelfAuthorize(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!this.CanAdministrate(rpc.player))
		{
			return;
		}
		this.AddPlayer(rpc.player);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060008BA RID: 2234 RVA: 0x00053434 File Offset: 0x00051634
	public void AddPlayer(global::BasePlayer player)
	{
		if (this.AtMaxAuthCapacity())
		{
			return;
		}
		this.authorizedPlayers.RemoveAll((PlayerNameID x) => x.userid == player.userID);
		PlayerNameID playerNameID = new PlayerNameID();
		playerNameID.userid = player.userID;
		playerNameID.username = player.displayName;
		this.authorizedPlayers.Add(playerNameID);
		this.UpdateMaxAuthCapacity();
	}

	// Token: 0x060008BB RID: 2235 RVA: 0x000534AC File Offset: 0x000516AC
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void RemoveSelfAuthorize(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!this.CanAdministrate(rpc.player))
		{
			return;
		}
		this.authorizedPlayers.RemoveAll((PlayerNameID x) => x.userid == rpc.player.userID);
		this.UpdateMaxAuthCapacity();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060008BC RID: 2236 RVA: 0x00053512 File Offset: 0x00051712
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	private void ClearList(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!this.CanAdministrate(rpc.player))
		{
			return;
		}
		this.authorizedPlayers.Clear();
		this.UpdateMaxAuthCapacity();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060008BD RID: 2237 RVA: 0x0005354C File Offset: 0x0005174C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void RPC_Rotate(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player.CanBuild() && player.GetHeldEntity() && player.GetHeldEntity().GetComponent<Hammer>() != null && (base.GetSlot(global::BaseEntity.Slot.Lock) == null || !base.GetSlot(global::BaseEntity.Slot.Lock).IsLocked()) && !base.HasAttachedStorageAdaptor())
		{
			base.transform.rotation = Quaternion.LookRotation(-base.transform.forward, base.transform.up);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			Deployable component = base.GetComponent<Deployable>();
			if (component != null && component.placeEffect.isValid)
			{
				Effect.server.Run(component.placeEffect.resourcePath, base.transform.position, Vector3.up, null, false);
			}
		}
		global::BaseEntity slot = base.GetSlot(global::BaseEntity.Slot.Lock);
		if (slot != null)
		{
			slot.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x060008BE RID: 2238 RVA: 0x00053640 File Offset: 0x00051840
	public override int GetIdealSlot(global::BasePlayer player, global::Item item)
	{
		if (item != null && item.info != null && this.allowedConstructionItems.Contains(item.info))
		{
			for (int i = 24; i <= 27; i++)
			{
				if (base.inventory.GetSlot(i) == null)
				{
					return i;
				}
			}
		}
		return base.GetIdealSlot(player, item);
	}

	// Token: 0x060008BF RID: 2239 RVA: 0x00053697 File Offset: 0x00051897
	public override bool HasSlot(global::BaseEntity.Slot slot)
	{
		return slot == global::BaseEntity.Slot.Lock || base.HasSlot(slot);
	}

	// Token: 0x060008C0 RID: 2240 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool SupportsChildDeployables()
	{
		return true;
	}

	// Token: 0x040005D2 RID: 1490
	private float cachedProtectedMinutes;

	// Token: 0x040005D3 RID: 1491
	private float nextProtectedCalcTime;

	// Token: 0x040005D4 RID: 1492
	private static BuildingPrivlidge.UpkeepBracket[] upkeepBrackets = new BuildingPrivlidge.UpkeepBracket[]
	{
		new BuildingPrivlidge.UpkeepBracket(ConVar.Decay.bracket_0_blockcount, ConVar.Decay.bracket_0_costfraction),
		new BuildingPrivlidge.UpkeepBracket(ConVar.Decay.bracket_1_blockcount, ConVar.Decay.bracket_1_costfraction),
		new BuildingPrivlidge.UpkeepBracket(ConVar.Decay.bracket_2_blockcount, ConVar.Decay.bracket_2_costfraction),
		new BuildingPrivlidge.UpkeepBracket(ConVar.Decay.bracket_3_blockcount, ConVar.Decay.bracket_3_costfraction)
	};

	// Token: 0x040005D5 RID: 1493
	private List<ItemAmount> upkeepBuffer = new List<ItemAmount>();

	// Token: 0x040005D6 RID: 1494
	public List<PlayerNameID> authorizedPlayers = new List<PlayerNameID>();

	// Token: 0x040005D7 RID: 1495
	public const global::BaseEntity.Flags Flag_MaxAuths = global::BaseEntity.Flags.Reserved5;

	// Token: 0x040005D8 RID: 1496
	public List<ItemDefinition> allowedConstructionItems = new List<ItemDefinition>();

	// Token: 0x02000B7C RID: 2940
	public class UpkeepBracket
	{
		// Token: 0x06004AC9 RID: 19145 RVA: 0x00190D62 File Offset: 0x0018EF62
		public UpkeepBracket(int numObjs, float frac)
		{
			this.objectsUpTo = numObjs;
			this.fraction = frac;
			this.blocksTaxPaid = 0f;
		}

		// Token: 0x04003E8A RID: 16010
		public int objectsUpTo;

		// Token: 0x04003E8B RID: 16011
		public float fraction;

		// Token: 0x04003E8C RID: 16012
		public float blocksTaxPaid;
	}
}
