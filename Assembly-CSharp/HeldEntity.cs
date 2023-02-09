using System;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200007C RID: 124
public class HeldEntity : global::BaseEntity
{
	// Token: 0x06000BAC RID: 2988 RVA: 0x00065A2C File Offset: 0x00063C2C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("HeldEntity.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000BAD RID: 2989 RVA: 0x00065A6C File Offset: 0x00063C6C
	public void SendPunch(Vector3 amount, float duration)
	{
		base.ClientRPCPlayer<Vector3, float>(null, this.GetOwnerPlayer(), "CL_Punch", amount, duration);
	}

	// Token: 0x170000FE RID: 254
	// (get) Token: 0x06000BAE RID: 2990 RVA: 0x00065A82 File Offset: 0x00063C82
	public bool hostile
	{
		get
		{
			return this.hostileScore > 0f;
		}
	}

	// Token: 0x06000BAF RID: 2991 RVA: 0x000035F8 File Offset: 0x000017F8
	public bool LightsOn()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved5);
	}

	// Token: 0x06000BB0 RID: 2992 RVA: 0x00020F08 File Offset: 0x0001F108
	public bool IsDeployed()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved4);
	}

	// Token: 0x06000BB1 RID: 2993 RVA: 0x00065A94 File Offset: 0x00063C94
	public global::BasePlayer GetOwnerPlayer()
	{
		global::BaseEntity parentEntity = base.GetParentEntity();
		if (!parentEntity.IsValid())
		{
			return null;
		}
		global::BasePlayer basePlayer = parentEntity.ToPlayer();
		if (basePlayer == null)
		{
			return null;
		}
		if (basePlayer.IsDead())
		{
			return null;
		}
		return basePlayer;
	}

	// Token: 0x06000BB2 RID: 2994 RVA: 0x00065AD0 File Offset: 0x00063CD0
	public Connection GetOwnerConnection()
	{
		global::BasePlayer ownerPlayer = this.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return null;
		}
		if (ownerPlayer.net == null)
		{
			return null;
		}
		return ownerPlayer.net.connection;
	}

	// Token: 0x06000BB3 RID: 2995 RVA: 0x00065B04 File Offset: 0x00063D04
	public virtual void SetOwnerPlayer(global::BasePlayer player)
	{
		Assert.IsTrue(base.isServer, "Should be server!");
		Assert.IsTrue(player.isServer, "Player should be serverside!");
		base.gameObject.Identity();
		base.SetParent(player, this.handBone, false, false);
		this.SetHeld(false);
	}

	// Token: 0x06000BB4 RID: 2996 RVA: 0x00065B52 File Offset: 0x00063D52
	public virtual void ClearOwnerPlayer()
	{
		Assert.IsTrue(base.isServer, "Should be server!");
		base.SetParent(null, false, false);
		this.SetHeld(false);
	}

	// Token: 0x06000BB5 RID: 2997 RVA: 0x00065B74 File Offset: 0x00063D74
	public virtual void SetVisibleWhileHolstered(bool visible)
	{
		if (!this.holsterInfo.displayWhenHolstered)
		{
			return;
		}
		this.holsterVisible = visible;
		this.UpdateHeldItemVisibility();
	}

	// Token: 0x06000BB6 RID: 2998 RVA: 0x00065B91 File Offset: 0x00063D91
	public virtual void SetGenericVisible(bool wantsVis)
	{
		this.genericVisible = wantsVis;
		base.SetFlag(global::BaseEntity.Flags.Reserved8, wantsVis, false, true);
		this.UpdateHeldItemVisibility();
	}

	// Token: 0x06000BB7 RID: 2999 RVA: 0x00065BAE File Offset: 0x00063DAE
	public uint GetBone(string bone)
	{
		return StringPool.Get(bone);
	}

	// Token: 0x06000BB8 RID: 3000 RVA: 0x00065BB6 File Offset: 0x00063DB6
	public virtual void SetLightsOn(bool isOn)
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved5, isOn, false, true);
	}

	// Token: 0x06000BB9 RID: 3001 RVA: 0x00065BC8 File Offset: 0x00063DC8
	public void UpdateHeldItemVisibility()
	{
		bool flag = false;
		if (this.GetOwnerPlayer())
		{
			bool flag2 = this.GetOwnerPlayer().GetHeldEntity() == this;
			if (!ConVar.Server.showHolsteredItems && !flag2)
			{
				flag = this.UpdateVisiblity_Invis();
			}
			else if (flag2)
			{
				flag = this.UpdateVisibility_Hand();
			}
			else if (this.holsterVisible)
			{
				flag = this.UpdateVisiblity_Holster();
			}
			else
			{
				flag = this.UpdateVisiblity_Invis();
			}
		}
		else if (this.genericVisible)
		{
			flag = this.UpdateVisibility_GenericVis();
		}
		else if (!this.genericVisible)
		{
			flag = this.UpdateVisiblity_Invis();
		}
		if (flag)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000BBA RID: 3002 RVA: 0x00065C5C File Offset: 0x00063E5C
	public bool UpdateVisibility_Hand()
	{
		if (this.currentVisState == global::HeldEntity.heldEntityVisState.Hand)
		{
			return false;
		}
		this.currentVisState = global::HeldEntity.heldEntityVisState.Hand;
		base.limitNetworking = false;
		base.SetFlag(global::BaseEntity.Flags.Disabled, false, false, true);
		base.SetParent(this.GetOwnerPlayer(), this.GetBone(this.handBone), false, false);
		return true;
	}

	// Token: 0x06000BBB RID: 3003 RVA: 0x00065CA8 File Offset: 0x00063EA8
	public bool UpdateVisibility_GenericVis()
	{
		if (this.currentVisState == global::HeldEntity.heldEntityVisState.GenericVis)
		{
			return false;
		}
		this.currentVisState = global::HeldEntity.heldEntityVisState.GenericVis;
		base.limitNetworking = false;
		base.SetFlag(global::BaseEntity.Flags.Disabled, false, false, true);
		return true;
	}

	// Token: 0x06000BBC RID: 3004 RVA: 0x00065CD0 File Offset: 0x00063ED0
	public bool UpdateVisiblity_Holster()
	{
		if (this.currentVisState == global::HeldEntity.heldEntityVisState.Holster)
		{
			return false;
		}
		this.currentVisState = global::HeldEntity.heldEntityVisState.Holster;
		base.limitNetworking = false;
		base.SetFlag(global::BaseEntity.Flags.Disabled, false, false, true);
		base.SetParent(this.GetOwnerPlayer(), this.GetBone(this.holsterInfo.holsterBone), false, false);
		return true;
	}

	// Token: 0x06000BBD RID: 3005 RVA: 0x00065D24 File Offset: 0x00063F24
	public bool UpdateVisiblity_Invis()
	{
		if (this.currentVisState == global::HeldEntity.heldEntityVisState.Invis)
		{
			return false;
		}
		this.currentVisState = global::HeldEntity.heldEntityVisState.Invis;
		base.SetParent(this.GetOwnerPlayer(), this.GetBone(this.handBone), false, false);
		base.limitNetworking = true;
		base.SetFlag(global::BaseEntity.Flags.Disabled, true, false, true);
		return true;
	}

	// Token: 0x06000BBE RID: 3006 RVA: 0x00065D70 File Offset: 0x00063F70
	public virtual void SetHeld(bool bHeld)
	{
		Assert.IsTrue(base.isServer, "Should be server!");
		base.SetFlag(global::BaseEntity.Flags.Reserved4, bHeld, false, true);
		if (!bHeld)
		{
			this.UpdateVisiblity_Invis();
		}
		base.limitNetworking = !bHeld;
		base.SetFlag(global::BaseEntity.Flags.Disabled, !bHeld, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		if (bHeld && this.lastHeldEvent > 1f && Analytics.Server.Enabled && !this.GetOwnerPlayer().IsNpc)
		{
			Analytics.Server.HeldItemDeployed(this.GetItem().info);
			this.lastHeldEvent = 0f;
		}
		this.OnHeldChanged();
	}

	// Token: 0x06000BBF RID: 3007 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnHeldChanged()
	{
	}

	// Token: 0x170000FF RID: 255
	// (get) Token: 0x06000BC0 RID: 3008 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool IsUsableByTurret
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000100 RID: 256
	// (get) Token: 0x06000BC1 RID: 3009 RVA: 0x0002A0CF File Offset: 0x000282CF
	public virtual Transform MuzzleTransform
	{
		get
		{
			return null;
		}
	}

	// Token: 0x06000BC2 RID: 3010 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool CanBeUsedInWater()
	{
		return false;
	}

	// Token: 0x06000BC3 RID: 3011 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool BlocksGestures()
	{
		return false;
	}

	// Token: 0x06000BC4 RID: 3012 RVA: 0x00065E14 File Offset: 0x00064014
	protected global::Item GetOwnerItem()
	{
		global::BasePlayer ownerPlayer = this.GetOwnerPlayer();
		if (ownerPlayer == null || ownerPlayer.inventory == null)
		{
			return null;
		}
		return ownerPlayer.inventory.FindItemUID(this.ownerItemUID);
	}

	// Token: 0x06000BC5 RID: 3013 RVA: 0x00065E52 File Offset: 0x00064052
	public override global::Item GetItem()
	{
		return this.GetOwnerItem();
	}

	// Token: 0x06000BC6 RID: 3014 RVA: 0x00065E5C File Offset: 0x0006405C
	public ItemDefinition GetOwnerItemDefinition()
	{
		global::Item ownerItem = this.GetOwnerItem();
		if (ownerItem == null)
		{
			Debug.LogWarning("GetOwnerItem - null!", this);
			return null;
		}
		return ownerItem.info;
	}

	// Token: 0x06000BC7 RID: 3015 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void CollectedForCrafting(global::Item item, global::BasePlayer crafter)
	{
	}

	// Token: 0x06000BC8 RID: 3016 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void ReturnedFromCancelledCraft(global::Item item, global::BasePlayer crafter)
	{
	}

	// Token: 0x06000BC9 RID: 3017 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void ServerCommand(global::Item item, string command, global::BasePlayer player)
	{
	}

	// Token: 0x06000BCA RID: 3018 RVA: 0x00065E86 File Offset: 0x00064086
	public virtual void SetupHeldEntity(global::Item item)
	{
		this.ownerItemUID = item.uid;
		this.InitOwnerPlayer();
	}

	// Token: 0x06000BCB RID: 3019 RVA: 0x00065E9A File Offset: 0x0006409A
	public global::Item GetCachedItem()
	{
		return this.cachedItem;
	}

	// Token: 0x06000BCC RID: 3020 RVA: 0x00065EA2 File Offset: 0x000640A2
	public void OnItemChanged(global::Item item)
	{
		this.cachedItem = item;
	}

	// Token: 0x06000BCD RID: 3021 RVA: 0x00065EAB File Offset: 0x000640AB
	public override void PostServerLoad()
	{
		this.InitOwnerPlayer();
	}

	// Token: 0x06000BCE RID: 3022 RVA: 0x00065EB4 File Offset: 0x000640B4
	private void InitOwnerPlayer()
	{
		global::BasePlayer ownerPlayer = this.GetOwnerPlayer();
		if (ownerPlayer != null)
		{
			this.SetOwnerPlayer(ownerPlayer);
			return;
		}
		this.ClearOwnerPlayer();
	}

	// Token: 0x06000BCF RID: 3023 RVA: 0x00065EDF File Offset: 0x000640DF
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.heldEntity = Facepunch.Pool.Get<ProtoBuf.HeldEntity>();
		info.msg.heldEntity.itemUID = this.ownerItemUID;
	}

	// Token: 0x06000BD0 RID: 3024 RVA: 0x00065F10 File Offset: 0x00064110
	public void DestroyThis()
	{
		global::Item ownerItem = this.GetOwnerItem();
		if (ownerItem != null)
		{
			ownerItem.Remove(0f);
		}
	}

	// Token: 0x06000BD1 RID: 3025 RVA: 0x00065F34 File Offset: 0x00064134
	protected bool HasItemAmount()
	{
		global::Item ownerItem = this.GetOwnerItem();
		return ownerItem != null && ownerItem.amount > 0;
	}

	// Token: 0x06000BD2 RID: 3026 RVA: 0x00065F58 File Offset: 0x00064158
	protected bool UseItemAmount(int iAmount)
	{
		if (iAmount <= 0)
		{
			return true;
		}
		global::Item ownerItem = this.GetOwnerItem();
		if (ownerItem == null)
		{
			this.DestroyThis();
			return true;
		}
		ownerItem.amount -= iAmount;
		ownerItem.MarkDirty();
		if (ownerItem.amount <= 0)
		{
			this.DestroyThis();
			return true;
		}
		return false;
	}

	// Token: 0x06000BD3 RID: 3027 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void ServerUse()
	{
	}

	// Token: 0x06000BD4 RID: 3028 RVA: 0x00065FA3 File Offset: 0x000641A3
	public virtual void ServerUse(float damageModifier, Transform originOverride = null)
	{
		this.ServerUse();
	}

	// Token: 0x06000BD5 RID: 3029 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool IsInstrument()
	{
		return false;
	}

	// Token: 0x06000BD6 RID: 3030 RVA: 0x00065FAB File Offset: 0x000641AB
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.heldEntity != null)
		{
			this.ownerItemUID = info.msg.heldEntity.itemUID;
		}
	}

	// Token: 0x04000771 RID: 1905
	public Animator worldModelAnimator;

	// Token: 0x04000772 RID: 1906
	public SoundDefinition thirdPersonDeploySound;

	// Token: 0x04000773 RID: 1907
	public SoundDefinition thirdPersonAimSound;

	// Token: 0x04000774 RID: 1908
	public SoundDefinition thirdPersonAimEndSound;

	// Token: 0x04000775 RID: 1909
	public const global::BaseEntity.Flags Flag_ForceVisible = global::BaseEntity.Flags.Reserved8;

	// Token: 0x04000776 RID: 1910
	[Header("Held Entity")]
	public string handBone = "r_prop";

	// Token: 0x04000777 RID: 1911
	public AnimatorOverrideController HoldAnimationOverride;

	// Token: 0x04000778 RID: 1912
	public bool isBuildingTool;

	// Token: 0x04000779 RID: 1913
	[Header("Hostility")]
	public float hostileScore;

	// Token: 0x0400077A RID: 1914
	public global::HeldEntity.HolsterInfo holsterInfo;

	// Token: 0x0400077B RID: 1915
	[Header("Camera")]
	public global::BasePlayer.CameraMode HeldCameraMode;

	// Token: 0x0400077C RID: 1916
	public Vector3 FirstPersonArmOffset;

	// Token: 0x0400077D RID: 1917
	public Vector3 FirstPersonArmRotation;

	// Token: 0x0400077E RID: 1918
	[Range(0f, 1f)]
	public float FirstPersonRotationStrength = 1f;

	// Token: 0x0400077F RID: 1919
	private bool holsterVisible;

	// Token: 0x04000780 RID: 1920
	private bool genericVisible;

	// Token: 0x04000781 RID: 1921
	private global::HeldEntity.heldEntityVisState currentVisState;

	// Token: 0x04000782 RID: 1922
	private TimeSince lastHeldEvent;

	// Token: 0x04000783 RID: 1923
	internal uint ownerItemUID;

	// Token: 0x04000784 RID: 1924
	private global::Item cachedItem;

	// Token: 0x02000B88 RID: 2952
	[Serializable]
	public class HolsterInfo
	{
		// Token: 0x04003EA1 RID: 16033
		public global::HeldEntity.HolsterInfo.HolsterSlot slot;

		// Token: 0x04003EA2 RID: 16034
		public bool displayWhenHolstered;

		// Token: 0x04003EA3 RID: 16035
		public string holsterBone = "spine3";

		// Token: 0x04003EA4 RID: 16036
		public Vector3 holsterOffset;

		// Token: 0x04003EA5 RID: 16037
		public Vector3 holsterRotationOffset;

		// Token: 0x02000F60 RID: 3936
		public enum HolsterSlot
		{
			// Token: 0x04004E08 RID: 19976
			BACK,
			// Token: 0x04004E09 RID: 19977
			RIGHT_THIGH,
			// Token: 0x04004E0A RID: 19978
			LEFT_THIGH
		}
	}

	// Token: 0x02000B89 RID: 2953
	public static class HeldEntityFlags
	{
		// Token: 0x04003EA6 RID: 16038
		public const global::BaseEntity.Flags Deployed = global::BaseEntity.Flags.Reserved4;

		// Token: 0x04003EA7 RID: 16039
		public const global::BaseEntity.Flags LightsOn = global::BaseEntity.Flags.Reserved5;
	}

	// Token: 0x02000B8A RID: 2954
	public enum heldEntityVisState
	{
		// Token: 0x04003EA9 RID: 16041
		UNSET,
		// Token: 0x04003EAA RID: 16042
		Invis,
		// Token: 0x04003EAB RID: 16043
		Hand,
		// Token: 0x04003EAC RID: 16044
		Holster,
		// Token: 0x04003EAD RID: 16045
		GenericVis
	}
}
