using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000DD RID: 221
public class TrainCarUnloadable : TrainCar
{
	// Token: 0x0600132D RID: 4909 RVA: 0x000985BC File Offset: 0x000967BC
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("TrainCarUnloadable.OnRpcMessage", 0))
		{
			if (rpc == 4254195175U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Open ");
				}
				using (TimeWarning.New("RPC_Open", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(4254195175U, "RPC_Open", this, player, 3f))
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
							this.RPC_Open(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_Open");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600132E RID: 4910 RVA: 0x00098724 File Offset: 0x00096924
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (old.HasFlag(global::BaseEntity.Flags.Reserved4) != next.HasFlag(global::BaseEntity.Flags.Reserved4) && this.fuelHatches != null)
		{
			this.fuelHatches.LinedUpStateChanged(base.LinedUpToUnload);
		}
	}

	// Token: 0x0600132F RID: 4911 RVA: 0x00098784 File Offset: 0x00096984
	protected override void OnChildAdded(global::BaseEntity child)
	{
		base.OnChildAdded(child);
		if (this.IsDead() || base.IsDestroyed)
		{
			return;
		}
		LootContainer lootContainer;
		if (child.TryGetComponent<LootContainer>(out lootContainer))
		{
			if (base.isServer)
			{
				lootContainer.inventory.SetLocked(!this.IsEmpty());
			}
			this.lootContainers.Add(new EntityRef<LootContainer>(lootContainer.net.ID));
		}
		if (base.isServer && child.prefabID == this.storagePrefab.GetEntity().prefabID)
		{
			StorageContainer storageContainer = (StorageContainer)child;
			this.storageInstance.Set(storageContainer);
			if (!Rust.Application.isLoadingSave)
			{
				this.FillWithLoot(storageContainer);
			}
		}
	}

	// Token: 0x06001330 RID: 4912 RVA: 0x0009882C File Offset: 0x00096A2C
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseTrain != null)
		{
			this.lootTypeIndex = info.msg.baseTrain.lootTypeIndex;
			if (base.isServer)
			{
				this.SetVisualOreLevel(info.msg.baseTrain.lootPercent);
			}
		}
	}

	// Token: 0x06001331 RID: 4913 RVA: 0x00098881 File Offset: 0x00096A81
	public bool IsEmpty()
	{
		return this.GetOrePercent() == 0f;
	}

	// Token: 0x06001332 RID: 4914 RVA: 0x00098890 File Offset: 0x00096A90
	public bool TryGetLootType(out TrainWagonLootData.LootOption lootOption)
	{
		return TrainWagonLootData.instance.TryGetLootFromIndex(this.lootTypeIndex, out lootOption);
	}

	// Token: 0x06001333 RID: 4915 RVA: 0x000988A3 File Offset: 0x00096AA3
	public override bool CanBeLooted(global::BasePlayer player)
	{
		return base.CanBeLooted(player) && !this.IsEmpty();
	}

	// Token: 0x06001334 RID: 4916 RVA: 0x000988BC File Offset: 0x00096ABC
	public int GetFilledLootAmount()
	{
		TrainWagonLootData.LootOption lootOption;
		if (this.TryGetLootType(out lootOption))
		{
			return lootOption.maxLootAmount;
		}
		Debug.LogWarning(base.GetType().Name + ": Called GetFilledLootAmount without a lootTypeIndex set.");
		return 0;
	}

	// Token: 0x06001335 RID: 4917 RVA: 0x000988F8 File Offset: 0x00096AF8
	public void SetVisualOreLevel(float percent)
	{
		if (this.orePlaneColliderDetailed == null)
		{
			return;
		}
		this._oreScale.y = Mathf.Clamp01(percent);
		this.orePlaneColliderDetailed.localScale = this._oreScale;
		if (base.isClient)
		{
			this.orePlaneVisuals.localScale = this._oreScale;
			this.orePlaneVisuals.gameObject.SetActive(percent > 0f);
		}
		if (base.isServer)
		{
			this.orePlaneColliderWorld.localScale = this._oreScale;
		}
	}

	// Token: 0x06001336 RID: 4918 RVA: 0x00098980 File Offset: 0x00096B80
	private void AnimateUnload(float startPercent)
	{
		this.prevAnimTime = UnityEngine.Time.time;
		this.animPercent = startPercent;
		if (base.isClient && this.unloadingFXContainer != null)
		{
			this.unloadingFXContainer.Play();
		}
		base.InvokeRepeating(new Action(this.UnloadAnimTick), 0f, 0f);
	}

	// Token: 0x06001337 RID: 4919 RVA: 0x000989DC File Offset: 0x00096BDC
	private void UnloadAnimTick()
	{
		this.animPercent -= (UnityEngine.Time.time - this.prevAnimTime) / 40f;
		this.SetVisualOreLevel(this.animPercent);
		this.prevAnimTime = UnityEngine.Time.time;
		if (this.animPercent <= 0f)
		{
			this.EndUnloadAnim();
		}
	}

	// Token: 0x06001338 RID: 4920 RVA: 0x00098A32 File Offset: 0x00096C32
	private void EndUnloadAnim()
	{
		if (base.isClient && this.unloadingFXContainer != null)
		{
			this.unloadingFXContainer.Stop();
		}
		base.CancelInvoke(new Action(this.UnloadAnimTick));
	}

	// Token: 0x06001339 RID: 4921 RVA: 0x00098A67 File Offset: 0x00096C67
	public float GetOrePercent()
	{
		if (base.isServer)
		{
			return TrainWagonLootData.GetOrePercent(this.lootTypeIndex, this.GetStorageContainer());
		}
		return 0f;
	}

	// Token: 0x0600133A RID: 4922 RVA: 0x00098A88 File Offset: 0x00096C88
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.baseTrain = Facepunch.Pool.Get<BaseTrain>();
		info.msg.baseTrain.lootTypeIndex = this.lootTypeIndex;
		info.msg.baseTrain.lootPercent = this.GetOrePercent();
	}

	// Token: 0x0600133B RID: 4923 RVA: 0x00098AD8 File Offset: 0x00096CD8
	internal override void DoServerDestroy()
	{
		if (vehicle.vehiclesdroploot)
		{
			foreach (EntityRef<LootContainer> entityRef in this.lootContainers)
			{
				LootContainer lootContainer = entityRef.Get(base.isServer);
				if (lootContainer != null && lootContainer.inventory != null && !lootContainer.inventory.IsLocked())
				{
					lootContainer.DropItems(null);
				}
			}
		}
		base.DoServerDestroy();
	}

	// Token: 0x0600133C RID: 4924 RVA: 0x00098B64 File Offset: 0x00096D64
	public bool IsLinedUpToUnload(BoxCollider unloaderBounds)
	{
		foreach (BoxCollider boxCollider in this.unloadingAreas)
		{
			if (unloaderBounds.bounds.Intersects(boxCollider.bounds))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600133D RID: 4925 RVA: 0x00098BA4 File Offset: 0x00096DA4
	public void FillWithLoot(StorageContainer sc)
	{
		sc.inventory.Clear();
		ItemManager.DoRemoves();
		TrainWagonLootData.LootOption lootOption = TrainWagonLootData.instance.GetLootOption(this.wagonType, out this.lootTypeIndex);
		int amount = UnityEngine.Random.Range(lootOption.minLootAmount, lootOption.maxLootAmount);
		ItemDefinition itemToCreate = ItemManager.FindItemDefinition(lootOption.lootItem.itemid);
		sc.inventory.AddItem(itemToCreate, amount, 0UL, global::ItemContainer.LimitStack.All);
		sc.inventory.SetLocked(true);
		this.SetVisualOreLevel(this.GetOrePercent());
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600133E RID: 4926 RVA: 0x00098C2A File Offset: 0x00096E2A
	public void EmptyOutLoot(StorageContainer sc)
	{
		sc.inventory.Clear();
		ItemManager.DoRemoves();
		this.SetVisualOreLevel(this.GetOrePercent());
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600133F RID: 4927 RVA: 0x00098C50 File Offset: 0x00096E50
	public void BeginUnloadAnimation()
	{
		float orePercent = this.GetOrePercent();
		this.AnimateUnload(orePercent);
		base.ClientRPC<float>(null, "RPC_AnimateUnload", orePercent);
	}

	// Token: 0x06001340 RID: 4928 RVA: 0x00098C78 File Offset: 0x00096E78
	public void EndEmptyProcess()
	{
		float orePercent = this.GetOrePercent();
		if (orePercent <= 0f)
		{
			this.lootTypeIndex = -1;
			foreach (EntityRef<LootContainer> entityRef in this.lootContainers)
			{
				LootContainer lootContainer = entityRef.Get(base.isServer);
				if (lootContainer != null && lootContainer.inventory != null)
				{
					lootContainer.inventory.SetLocked(false);
				}
			}
		}
		this.SetVisualOreLevel(orePercent);
		base.ClientRPC<float>(null, "RPC_StopAnimateUnload", orePercent);
		this.decayingFor = 0f;
	}

	// Token: 0x06001341 RID: 4929 RVA: 0x00098D28 File Offset: 0x00096F28
	public StorageContainer GetStorageContainer()
	{
		StorageContainer storageContainer = this.storageInstance.Get(base.isServer);
		if (storageContainer.IsValid())
		{
			return storageContainer;
		}
		return null;
	}

	// Token: 0x06001342 RID: 4930 RVA: 0x00098D52 File Offset: 0x00096F52
	protected override float GetDecayMinutes(bool hasPassengers)
	{
		if ((this.wagonType == TrainCarUnloadable.WagonType.Ore || this.wagonType == TrainCarUnloadable.WagonType.Fuel) && !hasPassengers && this.IsEmpty())
		{
			return TrainCarUnloadable.decayminutesafterunload;
		}
		return base.GetDecayMinutes(hasPassengers);
	}

	// Token: 0x06001343 RID: 4931 RVA: 0x00098D7D File Offset: 0x00096F7D
	protected override bool CanDieFromDecayNow()
	{
		return this.IsEmpty() || base.CanDieFromDecayNow();
	}

	// Token: 0x06001344 RID: 4932 RVA: 0x00098D90 File Offset: 0x00096F90
	public override bool AdminFixUp(int tier)
	{
		if (!base.AdminFixUp(tier))
		{
			return false;
		}
		StorageContainer storageContainer = this.GetStorageContainer();
		if (storageContainer.IsValid())
		{
			if (tier > 1)
			{
				this.FillWithLoot(storageContainer);
			}
			else
			{
				this.EmptyOutLoot(storageContainer);
			}
		}
		return true;
	}

	// Token: 0x06001345 RID: 4933 RVA: 0x00098DCC File Offset: 0x00096FCC
	public float MinDistToUnloadingArea(Vector3 point)
	{
		float num = float.MaxValue;
		point.y = 0f;
		foreach (BoxCollider boxCollider in this.unloadingAreas)
		{
			Vector3 b = boxCollider.transform.position + boxCollider.transform.rotation * boxCollider.center;
			b.y = 0f;
			float num2 = Vector3.Distance(point, b);
			if (num2 < num)
			{
				num = num2;
			}
		}
		return num;
	}

	// Token: 0x06001346 RID: 4934 RVA: 0x00098E4C File Offset: 0x0009704C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_Open(global::BaseEntity.RPCMessage msg)
	{
		global::BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		if (!this.CanBeLooted(player))
		{
			return;
		}
		StorageContainer storageContainer = this.GetStorageContainer();
		if (storageContainer.IsValid())
		{
			storageContainer.PlayerOpenLoot(player, "", true);
			return;
		}
		Debug.LogError(base.GetType().Name + ": No container component found.");
	}

	// Token: 0x04000BF7 RID: 3063
	[Header("Train Car Unloadable")]
	[SerializeField]
	private GameObjectRef storagePrefab;

	// Token: 0x04000BF8 RID: 3064
	[SerializeField]
	private BoxCollider[] unloadingAreas;

	// Token: 0x04000BF9 RID: 3065
	[SerializeField]
	private TrainCarFuelHatches fuelHatches;

	// Token: 0x04000BFA RID: 3066
	[SerializeField]
	private Transform orePlaneVisuals;

	// Token: 0x04000BFB RID: 3067
	[SerializeField]
	private Transform orePlaneColliderDetailed;

	// Token: 0x04000BFC RID: 3068
	[SerializeField]
	private Transform orePlaneColliderWorld;

	// Token: 0x04000BFD RID: 3069
	[SerializeField]
	[Range(0f, 1f)]
	public float vacuumStretchPercent = 0.5f;

	// Token: 0x04000BFE RID: 3070
	[SerializeField]
	private ParticleSystemContainer unloadingFXContainer;

	// Token: 0x04000BFF RID: 3071
	[SerializeField]
	private ParticleSystem unloadingFX;

	// Token: 0x04000C00 RID: 3072
	public TrainCarUnloadable.WagonType wagonType;

	// Token: 0x04000C01 RID: 3073
	private int lootTypeIndex = -1;

	// Token: 0x04000C02 RID: 3074
	private List<EntityRef<LootContainer>> lootContainers = new List<EntityRef<LootContainer>>();

	// Token: 0x04000C03 RID: 3075
	private Vector3 _oreScale = Vector3.one;

	// Token: 0x04000C04 RID: 3076
	private float animPercent;

	// Token: 0x04000C05 RID: 3077
	private float prevAnimTime;

	// Token: 0x04000C06 RID: 3078
	[ServerVar(Help = "How long before an unloadable train car despawns afer being unloaded")]
	public static float decayminutesafterunload = 10f;

	// Token: 0x04000C07 RID: 3079
	private EntityRef<StorageContainer> storageInstance;

	// Token: 0x02000BC1 RID: 3009
	public enum WagonType
	{
		// Token: 0x04003F71 RID: 16241
		Ore,
		// Token: 0x04003F72 RID: 16242
		Lootboxes,
		// Token: 0x04003F73 RID: 16243
		Fuel
	}
}
