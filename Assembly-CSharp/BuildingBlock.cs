using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200004B RID: 75
public class BuildingBlock : global::StabilityEntity
{
	// Token: 0x06000860 RID: 2144 RVA: 0x00050CE0 File Offset: 0x0004EEE0
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BuildingBlock.OnRpcMessage", 0))
		{
			if (rpc == 2858062413U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DoDemolish ");
				}
				using (TimeWarning.New("DoDemolish", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2858062413U, "DoDemolish", this, player, 3f))
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
							this.DoDemolish(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in DoDemolish");
					}
				}
				return true;
			}
			if (rpc == 216608990U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DoImmediateDemolish ");
				}
				using (TimeWarning.New("DoImmediateDemolish", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(216608990U, "DoImmediateDemolish", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.DoImmediateDemolish(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in DoImmediateDemolish");
					}
				}
				return true;
			}
			if (rpc == 1956645865U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DoRotation ");
				}
				using (TimeWarning.New("DoRotation", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1956645865U, "DoRotation", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg4 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.DoRotation(msg4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in DoRotation");
					}
				}
				return true;
			}
			if (rpc == 3746288057U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DoUpgradeToGrade ");
				}
				using (TimeWarning.New("DoUpgradeToGrade", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3746288057U, "DoUpgradeToGrade", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg5 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.DoUpgradeToGrade(msg5);
						}
					}
					catch (Exception exception4)
					{
						Debug.LogException(exception4);
						player.Kick("RPC Error in DoUpgradeToGrade");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000861 RID: 2145 RVA: 0x00051298 File Offset: 0x0004F498
	private bool CanDemolish(global::BasePlayer player)
	{
		return this.IsDemolishable() && this.HasDemolishPrivilege(player);
	}

	// Token: 0x06000862 RID: 2146 RVA: 0x000512AB File Offset: 0x0004F4AB
	private bool IsDemolishable()
	{
		return ConVar.Server.pve || base.HasFlag(global::BaseEntity.Flags.Reserved2);
	}

	// Token: 0x06000863 RID: 2147 RVA: 0x000512C4 File Offset: 0x0004F4C4
	private bool HasDemolishPrivilege(global::BasePlayer player)
	{
		return player.IsBuildingAuthed(base.transform.position, base.transform.rotation, this.bounds);
	}

	// Token: 0x06000864 RID: 2148 RVA: 0x000512E8 File Offset: 0x0004F4E8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void DoDemolish(global::BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		if (!this.CanDemolish(msg.player))
		{
			return;
		}
		base.Kill(global::BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x06000865 RID: 2149 RVA: 0x0005130E File Offset: 0x0004F50E
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void DoImmediateDemolish(global::BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		if (!msg.player.IsAdmin)
		{
			return;
		}
		base.Kill(global::BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x06000866 RID: 2150 RVA: 0x00051333 File Offset: 0x0004F533
	private void StopBeingDemolishable()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000867 RID: 2151 RVA: 0x0005134A File Offset: 0x0004F54A
	private void StartBeingDemolishable()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved2, true, false, true);
		base.Invoke(new Action(this.StopBeingDemolishable), 600f);
	}

	// Token: 0x06000868 RID: 2152 RVA: 0x00051371 File Offset: 0x0004F571
	public void SetConditionalModel(int state)
	{
		this.modelState = state;
	}

	// Token: 0x06000869 RID: 2153 RVA: 0x0005137A File Offset: 0x0004F57A
	public bool GetConditionalModel(int index)
	{
		return (this.modelState & 1 << index) != 0;
	}

	// Token: 0x170000D0 RID: 208
	// (get) Token: 0x0600086A RID: 2154 RVA: 0x0005138C File Offset: 0x0004F58C
	public ConstructionGrade currentGrade
	{
		get
		{
			ConstructionGrade constructionGrade = this.GetGrade(this.grade);
			if (constructionGrade != null)
			{
				return constructionGrade;
			}
			for (int i = 0; i < this.blockDefinition.grades.Length; i++)
			{
				if (this.blockDefinition.grades[i] != null)
				{
					return this.blockDefinition.grades[i];
				}
			}
			Debug.LogWarning("Building block grade not found: " + this.grade);
			return null;
		}
	}

	// Token: 0x0600086B RID: 2155 RVA: 0x00051408 File Offset: 0x0004F608
	private ConstructionGrade GetGrade(BuildingGrade.Enum iGrade)
	{
		if (this.grade >= (BuildingGrade.Enum)this.blockDefinition.grades.Length)
		{
			Debug.LogWarning(string.Concat(new object[]
			{
				"Grade out of range ",
				base.gameObject,
				" ",
				this.grade,
				" / ",
				this.blockDefinition.grades.Length
			}));
			return this.blockDefinition.defaultGrade;
		}
		return this.blockDefinition.grades[(int)iGrade];
	}

	// Token: 0x0600086C RID: 2156 RVA: 0x00051497 File Offset: 0x0004F697
	private bool CanChangeToGrade(BuildingGrade.Enum iGrade, global::BasePlayer player)
	{
		return this.HasUpgradePrivilege(iGrade, player) && !this.IsUpgradeBlocked();
	}

	// Token: 0x0600086D RID: 2157 RVA: 0x000514B0 File Offset: 0x0004F6B0
	private bool HasUpgradePrivilege(BuildingGrade.Enum iGrade, global::BasePlayer player)
	{
		return iGrade != this.grade && iGrade < (BuildingGrade.Enum)this.blockDefinition.grades.Length && iGrade >= BuildingGrade.Enum.Twigs && iGrade >= this.grade && !player.IsBuildingBlocked(base.transform.position, base.transform.rotation, this.bounds);
	}

	// Token: 0x0600086E RID: 2158 RVA: 0x00051510 File Offset: 0x0004F710
	private bool IsUpgradeBlocked()
	{
		if (!this.blockDefinition.checkVolumeOnUpgrade)
		{
			return false;
		}
		DeployVolume[] volumes = PrefabAttribute.server.FindAll<DeployVolume>(this.prefabID);
		return DeployVolume.Check(base.transform.position, base.transform.rotation, volumes, ~(1 << base.gameObject.layer));
	}

	// Token: 0x0600086F RID: 2159 RVA: 0x0005156C File Offset: 0x0004F76C
	private bool CanAffordUpgrade(BuildingGrade.Enum iGrade, global::BasePlayer player)
	{
		foreach (ItemAmount itemAmount in this.GetGrade(iGrade).costToBuild)
		{
			if ((float)player.inventory.GetAmount(itemAmount.itemid) < itemAmount.amount)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000870 RID: 2160 RVA: 0x000515E0 File Offset: 0x0004F7E0
	public void SetGrade(BuildingGrade.Enum iGradeID)
	{
		if (this.blockDefinition.grades == null || iGradeID >= (BuildingGrade.Enum)this.blockDefinition.grades.Length)
		{
			Debug.LogError("Tried to set to undefined grade! " + this.blockDefinition.fullName, base.gameObject);
			return;
		}
		this.grade = iGradeID;
		this.grade = this.currentGrade.gradeBase.type;
		this.UpdateGrade();
	}

	// Token: 0x06000871 RID: 2161 RVA: 0x0005164E File Offset: 0x0004F84E
	private void UpdateGrade()
	{
		this.baseProtection = this.currentGrade.gradeBase.damageProtecton;
	}

	// Token: 0x06000872 RID: 2162 RVA: 0x00051666 File Offset: 0x0004F866
	public void SetHealthToMax()
	{
		base.health = this.MaxHealth();
	}

	// Token: 0x06000873 RID: 2163 RVA: 0x00051674 File Offset: 0x0004F874
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void DoUpgradeToGrade(global::BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		BuildingGrade.Enum @enum = (BuildingGrade.Enum)msg.read.Int32();
		ConstructionGrade constructionGrade = this.GetGrade(@enum);
		if (constructionGrade == null)
		{
			return;
		}
		if (!this.CanChangeToGrade(@enum, msg.player))
		{
			return;
		}
		if (!this.CanAffordUpgrade(@enum, msg.player))
		{
			return;
		}
		if (base.SecondsSinceAttacked < 30f)
		{
			return;
		}
		this.PayForUpgrade(constructionGrade, msg.player);
		this.ChangeGrade(@enum, true);
	}

	// Token: 0x06000874 RID: 2164 RVA: 0x000516F0 File Offset: 0x0004F8F0
	public void ChangeGrade(BuildingGrade.Enum targetGrade, bool playEffect = false)
	{
		if (this.grade == targetGrade)
		{
			return;
		}
		this.SetGrade(targetGrade);
		this.SetHealthToMax();
		this.StartBeingRotatable();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		this.UpdateSkin(false);
		base.ResetUpkeepTime();
		base.UpdateSurroundingEntities();
		BuildingManager.Building building = BuildingManager.server.GetBuilding(this.buildingID);
		if (building != null)
		{
			building.Dirty();
		}
		if (playEffect)
		{
			Effect.server.Run("assets/bundled/prefabs/fx/build/promote_" + targetGrade.ToString().ToLower() + ".prefab", this, 0U, Vector3.zero, Vector3.zero, null, false);
		}
	}

	// Token: 0x06000875 RID: 2165 RVA: 0x00051788 File Offset: 0x0004F988
	private void PayForUpgrade(ConstructionGrade g, global::BasePlayer player)
	{
		List<global::Item> list = new List<global::Item>();
		foreach (ItemAmount itemAmount in g.costToBuild)
		{
			player.inventory.Take(list, itemAmount.itemid, (int)itemAmount.amount);
			player.Command(string.Concat(new object[]
			{
				"note.inv ",
				itemAmount.itemid,
				" ",
				itemAmount.amount * -1f
			}), Array.Empty<object>());
		}
		foreach (global::Item item in list)
		{
			item.Remove(0f);
		}
	}

	// Token: 0x06000876 RID: 2166 RVA: 0x0005187C File Offset: 0x0004FA7C
	private bool NeedsSkinChange()
	{
		return this.currentSkin == null || this.forceSkinRefresh || this.lastGrade != this.grade || this.lastModelState != this.modelState;
	}

	// Token: 0x06000877 RID: 2167 RVA: 0x000518B8 File Offset: 0x0004FAB8
	public void UpdateSkin(bool force = false)
	{
		if (force)
		{
			this.forceSkinRefresh = true;
		}
		if (!this.NeedsSkinChange())
		{
			return;
		}
		if (this.cachedStability <= 0f || base.isServer)
		{
			this.ChangeSkin();
			return;
		}
		if (!this.skinChange)
		{
			this.skinChange = new DeferredAction(this, new Action(this.ChangeSkin), ActionPriority.Medium);
		}
		if (!this.skinChange.Idle)
		{
			return;
		}
		this.skinChange.Invoke();
	}

	// Token: 0x06000878 RID: 2168 RVA: 0x00051933 File Offset: 0x0004FB33
	private void DestroySkin()
	{
		if (this.currentSkin != null)
		{
			this.currentSkin.Destroy(this);
			this.currentSkin = null;
		}
	}

	// Token: 0x06000879 RID: 2169 RVA: 0x00051958 File Offset: 0x0004FB58
	private void RefreshNeighbours(bool linkToNeighbours)
	{
		List<EntityLink> entityLinks = base.GetEntityLinks(linkToNeighbours);
		for (int i = 0; i < entityLinks.Count; i++)
		{
			EntityLink entityLink = entityLinks[i];
			for (int j = 0; j < entityLink.connections.Count; j++)
			{
				global::BuildingBlock buildingBlock = entityLink.connections[j].owner as global::BuildingBlock;
				if (!(buildingBlock == null))
				{
					if (Rust.Application.isLoading)
					{
						buildingBlock.UpdateSkin(true);
					}
					else
					{
						global::BuildingBlock.updateSkinQueueServer.Add(buildingBlock);
					}
				}
			}
		}
	}

	// Token: 0x0600087A RID: 2170 RVA: 0x000519DB File Offset: 0x0004FBDB
	private void UpdatePlaceholder(bool state)
	{
		if (this.placeholderRenderer)
		{
			this.placeholderRenderer.enabled = state;
		}
		if (this.placeholderCollider)
		{
			this.placeholderCollider.enabled = state;
		}
	}

	// Token: 0x0600087B RID: 2171 RVA: 0x00051A10 File Offset: 0x0004FC10
	private void ChangeSkin()
	{
		if (base.IsDestroyed)
		{
			return;
		}
		ConstructionGrade currentGrade = this.currentGrade;
		if (currentGrade.skinObject.isValid)
		{
			this.ChangeSkin(currentGrade.skinObject);
			return;
		}
		foreach (ConstructionGrade constructionGrade in this.blockDefinition.grades)
		{
			if (constructionGrade.skinObject.isValid)
			{
				this.ChangeSkin(constructionGrade.skinObject);
				return;
			}
		}
		Debug.LogWarning("No skins found for " + base.gameObject);
	}

	// Token: 0x0600087C RID: 2172 RVA: 0x00051A94 File Offset: 0x0004FC94
	private void ChangeSkin(GameObjectRef prefab)
	{
		bool flag = this.lastGrade != this.grade;
		this.lastGrade = this.grade;
		if (flag)
		{
			if (this.currentSkin == null)
			{
				this.UpdatePlaceholder(false);
			}
			else
			{
				this.DestroySkin();
			}
			GameObject gameObject = base.gameManager.CreatePrefab(prefab.resourcePath, base.transform, true);
			this.currentSkin = gameObject.GetComponent<ConstructionSkin>();
			Model component = this.currentSkin.GetComponent<Model>();
			base.SetModel(component);
			Assert.IsTrue(this.model == component, "Didn't manage to set model successfully!");
		}
		if (base.isServer)
		{
			this.modelState = this.currentSkin.DetermineConditionalModelState(this);
		}
		bool flag2 = this.lastModelState != this.modelState;
		this.lastModelState = this.modelState;
		if (flag || flag2 || this.forceSkinRefresh)
		{
			this.currentSkin.Refresh(this);
			if (base.isServer && flag2)
			{
				this.CheckForPipes();
			}
			this.forceSkinRefresh = false;
		}
		if (base.isServer)
		{
			if (flag)
			{
				this.RefreshNeighbours(true);
			}
			if (flag2)
			{
				base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
		}
	}

	// Token: 0x0600087D RID: 2173 RVA: 0x00051BB1 File Offset: 0x0004FDB1
	public override bool ShouldBlockProjectiles()
	{
		return this.grade > BuildingGrade.Enum.Twigs;
	}

	// Token: 0x0600087E RID: 2174 RVA: 0x00051BBC File Offset: 0x0004FDBC
	public void CheckForPipes()
	{
		if (!this.CheckForPipesOnModelChange || !ConVar.Server.enforcePipeChecksOnBuildingBlockChanges || Rust.Application.isLoading)
		{
			return;
		}
		List<ColliderInfo_Pipe> list = Facepunch.Pool.GetList<ColliderInfo_Pipe>();
		global::Vis.Components<ColliderInfo_Pipe>(new OBB(base.transform, this.bounds), list, 536870912, QueryTriggerInteraction.Collide);
		foreach (ColliderInfo_Pipe colliderInfo_Pipe in list)
		{
			if (!(colliderInfo_Pipe == null) && colliderInfo_Pipe.gameObject.activeInHierarchy && colliderInfo_Pipe.HasFlag(ColliderInfo.Flags.OnlyBlockBuildingBlock) && colliderInfo_Pipe.ParentEntity != null && colliderInfo_Pipe.ParentEntity.isServer)
			{
				WireTool.AttemptClearSlot(colliderInfo_Pipe.ParentEntity, null, colliderInfo_Pipe.OutputSlotIndex, false);
			}
		}
		Facepunch.Pool.FreeList<ColliderInfo_Pipe>(ref list);
	}

	// Token: 0x0600087F RID: 2175 RVA: 0x000059DD File Offset: 0x00003BDD
	private void OnHammered()
	{
	}

	// Token: 0x06000880 RID: 2176 RVA: 0x00051C94 File Offset: 0x0004FE94
	public override float MaxHealth()
	{
		return this.currentGrade.maxHealth;
	}

	// Token: 0x06000881 RID: 2177 RVA: 0x00051CA1 File Offset: 0x0004FEA1
	public override List<ItemAmount> BuildCost()
	{
		return this.currentGrade.costToBuild;
	}

	// Token: 0x06000882 RID: 2178 RVA: 0x00051CAE File Offset: 0x0004FEAE
	public override void OnHealthChanged(float oldvalue, float newvalue)
	{
		base.OnHealthChanged(oldvalue, newvalue);
		if (!base.isServer)
		{
			return;
		}
		if (Mathf.RoundToInt(oldvalue) == Mathf.RoundToInt(newvalue))
		{
			return;
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.UpdateDistance);
	}

	// Token: 0x06000883 RID: 2179 RVA: 0x000062DD File Offset: 0x000044DD
	public override float RepairCostFraction()
	{
		return 1f;
	}

	// Token: 0x06000884 RID: 2180 RVA: 0x00051CD7 File Offset: 0x0004FED7
	private bool CanRotate(global::BasePlayer player)
	{
		return this.IsRotatable() && this.HasRotationPrivilege(player) && !this.IsRotationBlocked();
	}

	// Token: 0x06000885 RID: 2181 RVA: 0x00051CF5 File Offset: 0x0004FEF5
	private bool IsRotatable()
	{
		return this.blockDefinition.grades != null && this.blockDefinition.canRotateAfterPlacement && base.HasFlag(global::BaseEntity.Flags.Reserved1);
	}

	// Token: 0x06000886 RID: 2182 RVA: 0x00051D28 File Offset: 0x0004FF28
	private bool IsRotationBlocked()
	{
		if (!this.blockDefinition.checkVolumeOnRotate)
		{
			return false;
		}
		DeployVolume[] volumes = PrefabAttribute.server.FindAll<DeployVolume>(this.prefabID);
		return DeployVolume.Check(base.transform.position, base.transform.rotation, volumes, ~(1 << base.gameObject.layer));
	}

	// Token: 0x06000887 RID: 2183 RVA: 0x00051D82 File Offset: 0x0004FF82
	private bool HasRotationPrivilege(global::BasePlayer player)
	{
		return !player.IsBuildingBlocked(base.transform.position, base.transform.rotation, this.bounds);
	}

	// Token: 0x06000888 RID: 2184 RVA: 0x00051DAC File Offset: 0x0004FFAC
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void DoRotation(global::BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		if (!this.CanRotate(msg.player))
		{
			return;
		}
		if (!this.blockDefinition.canRotateAfterPlacement)
		{
			return;
		}
		base.transform.localRotation *= Quaternion.Euler(this.blockDefinition.rotationAmount);
		base.RefreshEntityLinks();
		base.UpdateSurroundingEntities();
		this.UpdateSkin(true);
		this.RefreshNeighbours(false);
		base.SendNetworkUpdateImmediate(false);
		base.ClientRPC(null, "RefreshSkin");
	}

	// Token: 0x06000889 RID: 2185 RVA: 0x00051E37 File Offset: 0x00050037
	private void StopBeingRotatable()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved1, false, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600088A RID: 2186 RVA: 0x00051E50 File Offset: 0x00050050
	private void StartBeingRotatable()
	{
		if (this.blockDefinition.grades == null)
		{
			return;
		}
		if (!this.blockDefinition.canRotateAfterPlacement)
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved1, true, false, true);
		base.Invoke(new Action(this.StopBeingRotatable), 600f);
	}

	// Token: 0x0600088B RID: 2187 RVA: 0x00051EA0 File Offset: 0x000500A0
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.buildingBlock = Facepunch.Pool.Get<ProtoBuf.BuildingBlock>();
		info.msg.buildingBlock.model = this.modelState;
		info.msg.buildingBlock.grade = (int)this.grade;
	}

	// Token: 0x0600088C RID: 2188 RVA: 0x00051EF0 File Offset: 0x000500F0
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.buildingBlock != null)
		{
			this.SetConditionalModel(info.msg.buildingBlock.model);
			this.SetGrade((BuildingGrade.Enum)info.msg.buildingBlock.grade);
		}
		if (info.fromDisk)
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
			base.SetFlag(global::BaseEntity.Flags.Reserved1, false, false, true);
			this.UpdateSkin(false);
		}
	}

	// Token: 0x0600088D RID: 2189 RVA: 0x00051F68 File Offset: 0x00050168
	public override void AttachToBuilding(global::DecayEntity other)
	{
		if (other != null && other is global::BuildingBlock)
		{
			base.AttachToBuilding(other.buildingID);
			BuildingManager.server.CheckMerge(this);
			return;
		}
		base.AttachToBuilding(BuildingManager.server.NewBuildingID());
	}

	// Token: 0x0600088E RID: 2190 RVA: 0x00051FA4 File Offset: 0x000501A4
	public override void ServerInit()
	{
		this.blockDefinition = PrefabAttribute.server.Find<Construction>(this.prefabID);
		if (this.blockDefinition == null)
		{
			Debug.LogError("Couldn't find Construction for prefab " + this.prefabID);
		}
		base.ServerInit();
		this.UpdateSkin(false);
		if (base.HasFlag(global::BaseEntity.Flags.Reserved1) || !Rust.Application.isLoadingSave)
		{
			this.StartBeingRotatable();
		}
		if (base.HasFlag(global::BaseEntity.Flags.Reserved2) || !Rust.Application.isLoadingSave)
		{
			this.StartBeingDemolishable();
		}
		if (this.CullBushes && !Rust.Application.isLoadingSave)
		{
			List<BushEntity> list = Facepunch.Pool.GetList<BushEntity>();
			global::Vis.Entities<BushEntity>(this.WorldSpaceBounds(), list, 67108864, QueryTriggerInteraction.Collide);
			foreach (BushEntity bushEntity in list)
			{
				if (bushEntity.isServer)
				{
					bushEntity.Kill(global::BaseNetworkable.DestroyMode.None);
				}
			}
			Facepunch.Pool.FreeList<BushEntity>(ref list);
		}
	}

	// Token: 0x0600088F RID: 2191 RVA: 0x000520A8 File Offset: 0x000502A8
	public override void Hurt(HitInfo info)
	{
		if (ConVar.Server.pve && info.Initiator && info.Initiator is global::BasePlayer)
		{
			(info.Initiator as global::BasePlayer).Hurt(info.damageTypes.Total(), DamageType.Generic, null, true);
			return;
		}
		base.Hurt(info);
	}

	// Token: 0x06000890 RID: 2192 RVA: 0x000520FC File Offset: 0x000502FC
	public override void ResetState()
	{
		base.ResetState();
		this.blockDefinition = null;
		this.forceSkinRefresh = false;
		this.modelState = 0;
		this.lastModelState = 0;
		this.grade = BuildingGrade.Enum.Twigs;
		this.lastGrade = BuildingGrade.Enum.None;
		this.DestroySkin();
		this.UpdatePlaceholder(true);
	}

	// Token: 0x06000891 RID: 2193 RVA: 0x0005213B File Offset: 0x0005033B
	public override void InitShared()
	{
		base.InitShared();
		this.placeholderRenderer = base.GetComponent<MeshRenderer>();
		this.placeholderCollider = base.GetComponent<MeshCollider>();
	}

	// Token: 0x06000892 RID: 2194 RVA: 0x0005215B File Offset: 0x0005035B
	public override void PostInitShared()
	{
		this.baseProtection = this.currentGrade.gradeBase.damageProtecton;
		this.grade = this.currentGrade.gradeBase.type;
		base.PostInitShared();
	}

	// Token: 0x06000893 RID: 2195 RVA: 0x0005218F File Offset: 0x0005038F
	public override void DestroyShared()
	{
		if (base.isServer)
		{
			this.RefreshNeighbours(false);
		}
		base.DestroyShared();
	}

	// Token: 0x06000894 RID: 2196 RVA: 0x000521A6 File Offset: 0x000503A6
	public override string Categorize()
	{
		return "building";
	}

	// Token: 0x06000895 RID: 2197 RVA: 0x000062DD File Offset: 0x000044DD
	public override float BoundsPadding()
	{
		return 1f;
	}

	// Token: 0x06000896 RID: 2198 RVA: 0x000521B0 File Offset: 0x000503B0
	public override bool IsOutside()
	{
		float outside_test_range = ConVar.Decay.outside_test_range;
		Vector3 a = base.PivotPoint();
		for (int i = 0; i < global::BuildingBlock.outsideLookupOffsets.Length; i++)
		{
			Vector3 a2 = global::BuildingBlock.outsideLookupOffsets[i];
			Vector3 origin = a + a2 * outside_test_range;
			if (!UnityEngine.Physics.Raycast(new Ray(origin, -a2), outside_test_range - 0.5f, 2097152))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000897 RID: 2199 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool SupportsChildDeployables()
	{
		return true;
	}

	// Token: 0x040005C4 RID: 1476
	private bool forceSkinRefresh;

	// Token: 0x040005C5 RID: 1477
	private int modelState;

	// Token: 0x040005C6 RID: 1478
	private int lastModelState;

	// Token: 0x040005C7 RID: 1479
	public BuildingGrade.Enum grade;

	// Token: 0x040005C8 RID: 1480
	private BuildingGrade.Enum lastGrade = BuildingGrade.Enum.None;

	// Token: 0x040005C9 RID: 1481
	private ConstructionSkin currentSkin;

	// Token: 0x040005CA RID: 1482
	private DeferredAction skinChange;

	// Token: 0x040005CB RID: 1483
	private MeshRenderer placeholderRenderer;

	// Token: 0x040005CC RID: 1484
	private MeshCollider placeholderCollider;

	// Token: 0x040005CD RID: 1485
	public static global::BuildingBlock.UpdateSkinWorkQueue updateSkinQueueServer = new global::BuildingBlock.UpdateSkinWorkQueue();

	// Token: 0x040005CE RID: 1486
	public bool CullBushes;

	// Token: 0x040005CF RID: 1487
	public bool CheckForPipesOnModelChange;

	// Token: 0x040005D0 RID: 1488
	[NonSerialized]
	public Construction blockDefinition;

	// Token: 0x040005D1 RID: 1489
	private static Vector3[] outsideLookupOffsets = new Vector3[]
	{
		new Vector3(0f, 1f, 0f).normalized,
		new Vector3(1f, 1f, 0f).normalized,
		new Vector3(-1f, 1f, 0f).normalized,
		new Vector3(0f, 1f, 1f).normalized,
		new Vector3(0f, 1f, -1f).normalized
	};

	// Token: 0x02000B7A RID: 2938
	public static class BlockFlags
	{
		// Token: 0x04003E88 RID: 16008
		public const global::BaseEntity.Flags CanRotate = global::BaseEntity.Flags.Reserved1;

		// Token: 0x04003E89 RID: 16009
		public const global::BaseEntity.Flags CanDemolish = global::BaseEntity.Flags.Reserved2;
	}

	// Token: 0x02000B7B RID: 2939
	public class UpdateSkinWorkQueue : ObjectWorkQueue<global::BuildingBlock>
	{
		// Token: 0x06004AC6 RID: 19142 RVA: 0x00190D3F File Offset: 0x0018EF3F
		protected override void RunJob(global::BuildingBlock entity)
		{
			if (!this.ShouldAdd(entity))
			{
				return;
			}
			entity.UpdateSkin(true);
		}

		// Token: 0x06004AC7 RID: 19143 RVA: 0x00190D52 File Offset: 0x0018EF52
		protected override bool ShouldAdd(global::BuildingBlock entity)
		{
			return entity.IsValid();
		}
	}
}
