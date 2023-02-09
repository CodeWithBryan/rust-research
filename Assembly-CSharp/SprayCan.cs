using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000CE RID: 206
public class SprayCan : HeldEntity
{
	// Token: 0x06001213 RID: 4627 RVA: 0x00090EBC File Offset: 0x0008F0BC
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SprayCan.OnRpcMessage", 0))
		{
			if (rpc == 3490735573U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - BeginFreehandSpray ");
				}
				using (TimeWarning.New("BeginFreehandSpray", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(3490735573U, "BeginFreehandSpray", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.BeginFreehandSpray(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in BeginFreehandSpray");
					}
				}
				return true;
			}
			if (rpc == 151738090U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ChangeItemSkin ");
				}
				using (TimeWarning.New("ChangeItemSkin", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(151738090U, "ChangeItemSkin", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg3 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ChangeItemSkin(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in ChangeItemSkin");
					}
				}
				return true;
			}
			if (rpc == 396000799U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - CreateSpray ");
				}
				using (TimeWarning.New("CreateSpray", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(396000799U, "CreateSpray", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg4 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.CreateSpray(msg4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in CreateSpray");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001214 RID: 4628 RVA: 0x00091308 File Offset: 0x0008F508
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	private void BeginFreehandSpray(BaseEntity.RPCMessage msg)
	{
		if (base.IsBusy())
		{
			return;
		}
		if (!this.CanSprayFreehand(msg.player))
		{
			return;
		}
		Vector3 vector = msg.read.Vector3();
		Vector3 atNormal = msg.read.Vector3();
		int num = msg.read.Int32();
		int num2 = msg.read.Int32();
		if (num < 0 || num >= this.SprayColours.Length || num2 < 0 || num2 >= this.SprayWidths.Length)
		{
			return;
		}
		if (Vector3.Distance(vector, base.GetOwnerPlayer().transform.position) > 3f)
		{
			return;
		}
		SprayCanSpray_Freehand sprayCanSpray_Freehand = GameManager.server.CreateEntity(this.LinePrefab.resourcePath, vector, Quaternion.identity, true) as SprayCanSpray_Freehand;
		sprayCanSpray_Freehand.AddInitialPoint(atNormal);
		sprayCanSpray_Freehand.SetColour(this.SprayColours[num]);
		sprayCanSpray_Freehand.SetWidth(this.SprayWidths[num2]);
		sprayCanSpray_Freehand.EnableChanges(msg.player);
		sprayCanSpray_Freehand.Spawn();
		this.paintingLine = sprayCanSpray_Freehand;
		base.ClientRPC<int>(null, "Client_ChangeSprayColour", num);
		base.SetFlag(BaseEntity.Flags.Busy, true, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
		this.CheckAchievementPosition(vector);
	}

	// Token: 0x06001215 RID: 4629 RVA: 0x00091433 File Offset: 0x0008F633
	public void ClearPaintingLine(bool allowNewSprayImmediately)
	{
		this.paintingLine = null;
		this.LoseCondition(this.ConditionLossPerSpray);
		if (allowNewSprayImmediately)
		{
			this.ClearBusy();
			return;
		}
		base.Invoke(new Action(this.ClearBusy), 0.1f);
	}

	// Token: 0x06001216 RID: 4630 RVA: 0x0009146C File Offset: 0x0008F66C
	public bool CanSprayFreehand(BasePlayer player)
	{
		return this.FreeSprayUnlockItem != null && (player.blueprints.steamInventory.HasItem(this.FreeSprayUnlockItem.id) || this.FreeSprayUnlockItem.HasUnlocked(player.userID));
	}

	// Token: 0x06001217 RID: 4631 RVA: 0x000914BC File Offset: 0x0008F6BC
	private bool IsSprayBlockedByTrigger(Vector3 pos)
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return true;
		}
		TriggerNoSpray triggerNoSpray = ownerPlayer.FindTrigger<TriggerNoSpray>();
		return !(triggerNoSpray == null) && !triggerNoSpray.IsPositionValid(pos);
	}

	// Token: 0x06001218 RID: 4632 RVA: 0x000914F8 File Offset: 0x0008F6F8
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	private void ChangeItemSkin(BaseEntity.RPCMessage msg)
	{
		SprayCan.<>c__DisplayClass29_0 CS$<>8__locals1 = new SprayCan.<>c__DisplayClass29_0();
		CS$<>8__locals1.<>4__this = this;
		if (base.IsBusy())
		{
			return;
		}
		uint uid = msg.read.UInt32();
		BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(uid);
		CS$<>8__locals1.targetSkin = msg.read.Int32();
		if (msg.player == null || !msg.player.CanBuild())
		{
			return;
		}
		bool flag = false;
		if (CS$<>8__locals1.targetSkin != 0 && !flag && !msg.player.blueprints.CheckSkinOwnership(CS$<>8__locals1.targetSkin, msg.player.userID))
		{
			CS$<>8__locals1.<ChangeItemSkin>g__SprayFailResponse|2(SprayCan.SprayFailReason.SkinNotOwned);
			return;
		}
		BaseEntity baseEntity;
		if (baseNetworkable != null && (baseEntity = (baseNetworkable as BaseEntity)) != null)
		{
			Vector3 position = baseEntity.WorldSpaceBounds().ClosestPoint(msg.player.eyes.position);
			if (!msg.player.IsVisible(position, 3f))
			{
				CS$<>8__locals1.<ChangeItemSkin>g__SprayFailResponse|2(SprayCan.SprayFailReason.LineOfSight);
				return;
			}
			Door door;
			if ((door = (baseNetworkable as Door)) != null)
			{
				if (!door.GetPlayerLockPermission(msg.player))
				{
					msg.player.ChatMessage("Door must be openable");
					return;
				}
				if (door.IsOpen())
				{
					msg.player.ChatMessage("Door must be closed");
					return;
				}
			}
			ItemDefinition itemDefinition;
			if (!SprayCan.GetItemDefinitionForEntity(baseEntity, out itemDefinition, true))
			{
				CS$<>8__locals1.<ChangeItemSkin>g__SprayFailResponse|2(SprayCan.SprayFailReason.InvalidItem);
				return;
			}
			ItemDefinition itemDefinition2 = null;
			ulong skinID = ItemDefinition.FindSkin(itemDefinition.itemid, CS$<>8__locals1.targetSkin);
			ItemSkinDirectory.Skin skin = itemDefinition.skins.FirstOrDefault((ItemSkinDirectory.Skin x) => x.id == CS$<>8__locals1.targetSkin);
			ItemSkin itemSkin;
			if (skin.invItem != null && (itemSkin = (skin.invItem as ItemSkin)) != null)
			{
				if (itemSkin.Redirect != null)
				{
					itemDefinition2 = itemSkin.Redirect;
				}
				else if (SprayCan.GetItemDefinitionForEntity(baseEntity, out itemDefinition, false) && itemDefinition.isRedirectOf != null)
				{
					itemDefinition2 = itemDefinition.isRedirectOf;
				}
			}
			else if (itemDefinition.isRedirectOf != null || (SprayCan.GetItemDefinitionForEntity(baseEntity, out itemDefinition, false) && itemDefinition.isRedirectOf != null))
			{
				itemDefinition2 = itemDefinition.isRedirectOf;
			}
			if (itemDefinition2 == null)
			{
				baseEntity.skinID = skinID;
				baseEntity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
				Analytics.Server.SkinUsed(itemDefinition.shortname, CS$<>8__locals1.targetSkin);
			}
			else
			{
				SprayCan.SprayFailReason reason;
				if (!this.CanEntityBeRespawned(baseEntity, out reason))
				{
					CS$<>8__locals1.<ChangeItemSkin>g__SprayFailResponse|2(reason);
					return;
				}
				string strPrefab;
				if (!this.GetEntityPrefabPath(itemDefinition2, out strPrefab))
				{
					Debug.LogWarning("Cannot find resource path of redirect entity to spawn! " + itemDefinition2.gameObject.name);
					CS$<>8__locals1.<ChangeItemSkin>g__SprayFailResponse|2(SprayCan.SprayFailReason.InvalidItem);
					return;
				}
				Vector3 position2 = baseEntity.transform.position;
				Quaternion rotation = baseEntity.transform.rotation;
				BaseEntity parentEntity = baseEntity.GetParentEntity();
				float health = baseEntity.Health();
				EntityRef[] slots = baseEntity.GetSlots();
				BaseCombatEntity baseCombatEntity;
				float lastAttackedTime = ((baseCombatEntity = (baseEntity as BaseCombatEntity)) != null) ? baseCombatEntity.lastAttackedTime : 0f;
				bool flag2 = baseEntity is Door;
				Dictionary<SprayCan.ContainerSet, List<Item>> dictionary = new Dictionary<SprayCan.ContainerSet, List<Item>>();
				SprayCan.<ChangeItemSkin>g__SaveEntityStorage|29_0(baseEntity, dictionary, 0);
				List<SprayCan.ChildPreserveInfo> list = Facepunch.Pool.GetList<SprayCan.ChildPreserveInfo>();
				if (flag2)
				{
					foreach (BaseEntity baseEntity2 in baseEntity.children)
					{
						list.Add(new SprayCan.ChildPreserveInfo
						{
							TargetEntity = baseEntity2,
							TargetBone = baseEntity2.parentBone,
							LocalPosition = baseEntity2.transform.localPosition,
							LocalRotation = baseEntity2.transform.localRotation
						});
					}
					using (List<SprayCan.ChildPreserveInfo>.Enumerator enumerator2 = list.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							SprayCan.ChildPreserveInfo childPreserveInfo = enumerator2.Current;
							childPreserveInfo.TargetEntity.SetParent(null, true, false);
						}
						goto IL_3E5;
					}
				}
				for (int i = 0; i < baseEntity.children.Count; i++)
				{
					SprayCan.<ChangeItemSkin>g__SaveEntityStorage|29_0(baseEntity.children[i], dictionary, -1);
				}
				IL_3E5:
				baseEntity.Kill(BaseNetworkable.DestroyMode.None);
				baseEntity = GameManager.server.CreateEntity(strPrefab, position2, rotation, true);
				baseEntity.SetParent(parentEntity, false, false);
				ItemDefinition itemDefinition3;
				if (SprayCan.GetItemDefinitionForEntity(baseEntity, out itemDefinition3, false) && itemDefinition3.isRedirectOf != null)
				{
					baseEntity.skinID = 0UL;
				}
				else
				{
					baseEntity.skinID = skinID;
				}
				DecayEntity decayEntity;
				if ((decayEntity = (baseEntity as DecayEntity)) != null)
				{
					decayEntity.AttachToBuilding(null);
				}
				baseEntity.Spawn();
				BaseCombatEntity baseCombatEntity2;
				if ((baseCombatEntity2 = (baseEntity as BaseCombatEntity)) != null)
				{
					baseCombatEntity2.SetHealth(health);
					baseCombatEntity2.lastAttackedTime = lastAttackedTime;
				}
				if (dictionary.Count > 0)
				{
					SprayCan.<ChangeItemSkin>g__RestoreEntityStorage|29_1(baseEntity, 0, dictionary);
					if (!flag2)
					{
						for (int j = 0; j < baseEntity.children.Count; j++)
						{
							SprayCan.<ChangeItemSkin>g__RestoreEntityStorage|29_1(baseEntity.children[j], -1, dictionary);
						}
					}
					foreach (KeyValuePair<SprayCan.ContainerSet, List<Item>> keyValuePair in dictionary)
					{
						foreach (Item item in keyValuePair.Value)
						{
							Debug.Log(string.Format("Deleting {0} as it has no new container", item));
							item.Remove(0f);
						}
					}
					Analytics.Server.SkinUsed(itemDefinition.shortname, CS$<>8__locals1.targetSkin);
				}
				if (flag2)
				{
					foreach (SprayCan.ChildPreserveInfo childPreserveInfo2 in list)
					{
						childPreserveInfo2.TargetEntity.SetParent(baseEntity, childPreserveInfo2.TargetBone, true, false);
						childPreserveInfo2.TargetEntity.transform.localPosition = childPreserveInfo2.LocalPosition;
						childPreserveInfo2.TargetEntity.transform.localRotation = childPreserveInfo2.LocalRotation;
						childPreserveInfo2.TargetEntity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
					}
					baseEntity.SetSlots(slots);
				}
				Facepunch.Pool.FreeList<SprayCan.ChildPreserveInfo>(ref list);
			}
			base.ClientRPC<int, uint>(null, "Client_ReskinResult", 1, baseEntity.net.ID);
		}
		this.LoseCondition(this.ConditionLossPerReskin);
		base.ClientRPC<int>(null, "Client_ChangeSprayColour", -1);
		base.SetFlag(BaseEntity.Flags.Busy, true, false, true);
		base.Invoke(new Action(this.ClearBusy), this.SprayCooldown);
	}

	// Token: 0x06001219 RID: 4633 RVA: 0x00091B84 File Offset: 0x0008FD84
	private bool GetEntityPrefabPath(ItemDefinition def, out string resourcePath)
	{
		resourcePath = string.Empty;
		ItemModDeployable itemModDeployable;
		if (def.TryGetComponent<ItemModDeployable>(out itemModDeployable))
		{
			resourcePath = itemModDeployable.entityPrefab.resourcePath;
			return true;
		}
		ItemModEntity itemModEntity;
		if (def.TryGetComponent<ItemModEntity>(out itemModEntity))
		{
			resourcePath = itemModEntity.entityPrefab.resourcePath;
			return true;
		}
		ItemModEntityReference itemModEntityReference;
		if (def.TryGetComponent<ItemModEntityReference>(out itemModEntityReference))
		{
			resourcePath = itemModEntityReference.entityPrefab.resourcePath;
			return true;
		}
		return false;
	}

	// Token: 0x0600121A RID: 4634 RVA: 0x00091BE4 File Offset: 0x0008FDE4
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	private void CreateSpray(BaseEntity.RPCMessage msg)
	{
		if (base.IsBusy())
		{
			return;
		}
		base.ClientRPC<int>(null, "Client_ChangeSprayColour", -1);
		base.SetFlag(BaseEntity.Flags.Busy, true, false, true);
		base.Invoke(new Action(this.ClearBusy), this.SprayCooldown);
		Vector3 vector = msg.read.Vector3();
		Vector3 vector2 = msg.read.Vector3();
		Vector3 point = msg.read.Vector3();
		int num = msg.read.Int32();
		if (Vector3.Distance(vector, base.transform.position) > 4.5f)
		{
			return;
		}
		Plane plane = new Plane(vector2, vector);
		Quaternion quaternion = Quaternion.LookRotation((plane.ClosestPointOnPlane(point) - vector).normalized, vector2);
		quaternion *= Quaternion.Euler(0f, 0f, 90f);
		bool flag = false;
		if (num != 0 && !flag && !msg.player.blueprints.CheckSkinOwnership(num, msg.player.userID))
		{
			Debug.Log(string.Format("SprayCan.ChangeItemSkin player does not have item :{0}:", num));
			return;
		}
		ulong skinID = ItemDefinition.FindSkin(this.SprayDecalItem.itemid, num);
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.SprayDecalEntityRef.resourcePath, vector, quaternion, true);
		baseEntity.skinID = skinID;
		baseEntity.OnDeployed(null, base.GetOwnerPlayer(), this.GetItem());
		baseEntity.Spawn();
		this.CheckAchievementPosition(vector);
		this.LoseCondition(this.ConditionLossPerSpray);
	}

	// Token: 0x0600121B RID: 4635 RVA: 0x000059DD File Offset: 0x00003BDD
	private void CheckAchievementPosition(Vector3 pos)
	{
	}

	// Token: 0x0600121C RID: 4636 RVA: 0x00091D58 File Offset: 0x0008FF58
	private void LoseCondition(float amount)
	{
		Item ownerItem = base.GetOwnerItem();
		if (ownerItem == null)
		{
			return;
		}
		ownerItem.LoseCondition(amount);
	}

	// Token: 0x0600121D RID: 4637 RVA: 0x00091D77 File Offset: 0x0008FF77
	public void ClearBusy()
	{
		base.SetFlag(BaseEntity.Flags.Busy, false, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
	}

	// Token: 0x0600121E RID: 4638 RVA: 0x00091D95 File Offset: 0x0008FF95
	public override void OnHeldChanged()
	{
		if (base.IsDisabled())
		{
			this.ClearBusy();
			if (this.paintingLine != null)
			{
				this.paintingLine.Kill(BaseNetworkable.DestroyMode.None);
			}
			this.paintingLine = null;
		}
	}

	// Token: 0x0600121F RID: 4639 RVA: 0x00091DC8 File Offset: 0x0008FFC8
	private bool CanEntityBeRespawned(BaseEntity targetEntity, out SprayCan.SprayFailReason reason)
	{
		BaseMountable baseMountable;
		if ((baseMountable = (targetEntity as BaseMountable)) != null && baseMountable.AnyMounted())
		{
			reason = SprayCan.SprayFailReason.MountedBlocked;
			return false;
		}
		BaseVehicle baseVehicle;
		if (targetEntity.isServer && (baseVehicle = (targetEntity as BaseVehicle)) != null && (baseVehicle.HasDriver() || baseVehicle.AnyMounted()))
		{
			reason = SprayCan.SprayFailReason.MountedBlocked;
			return false;
		}
		IOEntity ioentity;
		if ((ioentity = (targetEntity as IOEntity)) != null && (ioentity.GetConnectedInputCount() != 0 || ioentity.GetConnectedOutputCount() != 0))
		{
			reason = SprayCan.SprayFailReason.IOConnection;
			return false;
		}
		reason = SprayCan.SprayFailReason.None;
		return true;
	}

	// Token: 0x06001220 RID: 4640 RVA: 0x00091E38 File Offset: 0x00090038
	public static bool GetItemDefinitionForEntity(BaseEntity be, out ItemDefinition def, bool useRedirect = true)
	{
		def = null;
		BaseCombatEntity baseCombatEntity;
		if ((baseCombatEntity = (be as BaseCombatEntity)) != null)
		{
			if (baseCombatEntity.pickup.enabled && baseCombatEntity.pickup.itemTarget != null)
			{
				def = baseCombatEntity.pickup.itemTarget;
			}
			else if (baseCombatEntity.repair.enabled && baseCombatEntity.repair.itemTarget != null)
			{
				def = baseCombatEntity.repair.itemTarget;
			}
		}
		if (useRedirect && def != null && def.isRedirectOf != null)
		{
			def = def.isRedirectOf;
		}
		return def != null;
	}

	// Token: 0x06001223 RID: 4643 RVA: 0x00091F60 File Offset: 0x00090160
	[CompilerGenerated]
	internal static void <ChangeItemSkin>g__SaveEntityStorage|29_0(BaseEntity baseEntity, Dictionary<SprayCan.ContainerSet, List<Item>> dictionary, int index)
	{
		IItemContainerEntity itemContainerEntity;
		if ((itemContainerEntity = (baseEntity as IItemContainerEntity)) != null)
		{
			SprayCan.ContainerSet key = new SprayCan.ContainerSet
			{
				ContainerIndex = index,
				PrefabId = ((index == 0) ? 0U : baseEntity.prefabID)
			};
			if (dictionary.ContainsKey(key))
			{
				Debug.Log("Multiple containers with the same prefab id being added during vehicle reskin");
				return;
			}
			dictionary.Add(key, new List<Item>());
			foreach (Item item in itemContainerEntity.inventory.itemList)
			{
				dictionary[key].Add(item);
			}
			foreach (Item item2 in dictionary[key])
			{
				item2.RemoveFromContainer();
			}
		}
	}

	// Token: 0x06001224 RID: 4644 RVA: 0x00092054 File Offset: 0x00090254
	[CompilerGenerated]
	internal static void <ChangeItemSkin>g__RestoreEntityStorage|29_1(BaseEntity baseEntity, int index, Dictionary<SprayCan.ContainerSet, List<Item>> copy)
	{
		IItemContainerEntity itemContainerEntity;
		if ((itemContainerEntity = (baseEntity as IItemContainerEntity)) != null)
		{
			SprayCan.ContainerSet key = new SprayCan.ContainerSet
			{
				ContainerIndex = index,
				PrefabId = ((index == 0) ? 0U : baseEntity.prefabID)
			};
			if (copy.ContainsKey(key))
			{
				foreach (Item item in copy[key])
				{
					item.MoveToContainer(itemContainerEntity.inventory, -1, true, false, null, true);
				}
				copy.Remove(key);
			}
		}
	}

	// Token: 0x04000B56 RID: 2902
	public const float MaxFreeSprayDistanceFromStart = 10f;

	// Token: 0x04000B57 RID: 2903
	public const float MaxFreeSprayStartingDistance = 3f;

	// Token: 0x04000B58 RID: 2904
	private SprayCanSpray_Freehand paintingLine;

	// Token: 0x04000B59 RID: 2905
	public const BaseEntity.Flags IsFreeSpraying = BaseEntity.Flags.Reserved1;

	// Token: 0x04000B5A RID: 2906
	public SoundDefinition SpraySound;

	// Token: 0x04000B5B RID: 2907
	public GameObjectRef SkinSelectPanel;

	// Token: 0x04000B5C RID: 2908
	public float SprayCooldown = 2f;

	// Token: 0x04000B5D RID: 2909
	public float ConditionLossPerSpray = 10f;

	// Token: 0x04000B5E RID: 2910
	public float ConditionLossPerReskin = 10f;

	// Token: 0x04000B5F RID: 2911
	public GameObjectRef LinePrefab;

	// Token: 0x04000B60 RID: 2912
	public Color[] SprayColours = new Color[0];

	// Token: 0x04000B61 RID: 2913
	public float[] SprayWidths = new float[]
	{
		0.1f,
		0.2f,
		0.3f
	};

	// Token: 0x04000B62 RID: 2914
	public ParticleSystem worldSpaceSprayFx;

	// Token: 0x04000B63 RID: 2915
	public GameObjectRef ReskinEffect;

	// Token: 0x04000B64 RID: 2916
	public ItemDefinition SprayDecalItem;

	// Token: 0x04000B65 RID: 2917
	public GameObjectRef SprayDecalEntityRef;

	// Token: 0x04000B66 RID: 2918
	public SteamInventoryItem FreeSprayUnlockItem;

	// Token: 0x04000B67 RID: 2919
	public ParticleSystem.MinMaxGradient DecalSprayGradient;

	// Token: 0x04000B68 RID: 2920
	public SoundDefinition SprayLoopDef;

	// Token: 0x04000B69 RID: 2921
	public static Translate.Phrase FreeSprayNamePhrase = new Translate.Phrase("freespray_radial", "Free Spray");

	// Token: 0x04000B6A RID: 2922
	public static Translate.Phrase FreeSprayDescPhrase = new Translate.Phrase("freespray_radial_desc", "Spray shapes freely with various colors");

	// Token: 0x04000B6B RID: 2923
	public const string ENEMY_BASE_STAT = "sprayed_enemy_base";

	// Token: 0x02000BB7 RID: 2999
	private enum SprayFailReason
	{
		// Token: 0x04003F48 RID: 16200
		None,
		// Token: 0x04003F49 RID: 16201
		MountedBlocked,
		// Token: 0x04003F4A RID: 16202
		IOConnection,
		// Token: 0x04003F4B RID: 16203
		LineOfSight,
		// Token: 0x04003F4C RID: 16204
		SkinNotOwned,
		// Token: 0x04003F4D RID: 16205
		InvalidItem
	}

	// Token: 0x02000BB8 RID: 3000
	private struct ContainerSet
	{
		// Token: 0x04003F4E RID: 16206
		public int ContainerIndex;

		// Token: 0x04003F4F RID: 16207
		public uint PrefabId;
	}

	// Token: 0x02000BB9 RID: 3001
	private struct ChildPreserveInfo
	{
		// Token: 0x04003F50 RID: 16208
		public BaseEntity TargetEntity;

		// Token: 0x04003F51 RID: 16209
		public uint TargetBone;

		// Token: 0x04003F52 RID: 16210
		public Vector3 LocalPosition;

		// Token: 0x04003F53 RID: 16211
		public Quaternion LocalRotation;
	}
}
