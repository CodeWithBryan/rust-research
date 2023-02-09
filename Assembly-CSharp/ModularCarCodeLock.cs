using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using UnityEngine;

// Token: 0x02000474 RID: 1140
public class ModularCarCodeLock
{
	// Token: 0x170002DB RID: 731
	// (get) Token: 0x06002530 RID: 9520 RVA: 0x000E941F File Offset: 0x000E761F
	public bool HasALock
	{
		get
		{
			return this.isServer && !string.IsNullOrEmpty(this.Code);
		}
	}

	// Token: 0x170002DC RID: 732
	// (get) Token: 0x06002531 RID: 9521 RVA: 0x000E9439 File Offset: 0x000E7639
	public bool CentralLockingIsOn
	{
		get
		{
			return this.owner != null && this.owner.HasFlag(BaseEntity.Flags.Reserved2);
		}
	}

	// Token: 0x170002DD RID: 733
	// (get) Token: 0x06002532 RID: 9522 RVA: 0x000E945B File Offset: 0x000E765B
	// (set) Token: 0x06002533 RID: 9523 RVA: 0x000E9463 File Offset: 0x000E7663
	public List<ulong> WhitelistPlayers { get; private set; } = new List<ulong>();

	// Token: 0x06002534 RID: 9524 RVA: 0x000E946C File Offset: 0x000E766C
	public ModularCarCodeLock(ModularCar owner, bool isServer)
	{
		this.owner = owner;
		this.isServer = isServer;
		if (isServer)
		{
			this.CheckEnableCentralLocking();
		}
	}

	// Token: 0x06002535 RID: 9525 RVA: 0x000E94AC File Offset: 0x000E76AC
	public bool PlayerCanDestroyLock(BaseVehicleModule viaModule)
	{
		return this.HasALock && viaModule.healthFraction <= 0.2f;
	}

	// Token: 0x06002536 RID: 9526 RVA: 0x000E94C8 File Offset: 0x000E76C8
	public bool CodeEntryBlocked(BasePlayer player)
	{
		return !this.HasLockPermission(player) && this.owner != null && this.owner.HasFlag(BaseEntity.Flags.Reserved10);
	}

	// Token: 0x06002537 RID: 9527 RVA: 0x000E94F8 File Offset: 0x000E76F8
	public void Load(BaseNetworkable.LoadInfo info)
	{
		this.Code = info.msg.modularCar.lockCode;
		if (this.Code == null)
		{
			this.Code = "";
		}
		this.WhitelistPlayers.Clear();
		this.WhitelistPlayers.AddRange(info.msg.modularCar.whitelistUsers);
	}

	// Token: 0x06002538 RID: 9528 RVA: 0x000E9554 File Offset: 0x000E7754
	public bool HasLockPermission(BasePlayer player)
	{
		return !this.HasALock || (player.IsValid() && !player.IsDead() && this.WhitelistPlayers.Contains(player.userID));
	}

	// Token: 0x06002539 RID: 9529 RVA: 0x000E9583 File Offset: 0x000E7783
	public bool PlayerCanUseThis(BasePlayer player, ModularCarCodeLock.LockType lockType)
	{
		return (lockType == ModularCarCodeLock.LockType.Door && !this.CentralLockingIsOn) || this.HasLockPermission(player);
	}

	// Token: 0x170002DE RID: 734
	// (get) Token: 0x0600253A RID: 9530 RVA: 0x000E9599 File Offset: 0x000E7799
	// (set) Token: 0x0600253B RID: 9531 RVA: 0x000E95A1 File Offset: 0x000E77A1
	public string Code { get; private set; } = "";

	// Token: 0x0600253C RID: 9532 RVA: 0x000E95AA File Offset: 0x000E77AA
	public void PostServerLoad()
	{
		this.owner.SetFlag(BaseEntity.Flags.Reserved10, false, false, true);
		this.CheckEnableCentralLocking();
	}

	// Token: 0x0600253D RID: 9533 RVA: 0x000E95C5 File Offset: 0x000E77C5
	public bool CanHaveALock()
	{
		return !this.owner.IsDead() && this.owner.HasDriverMountPoints();
	}

	// Token: 0x0600253E RID: 9534 RVA: 0x000E95E1 File Offset: 0x000E77E1
	public bool TryAddALock(string code, ulong userID)
	{
		if (!this.isServer)
		{
			return false;
		}
		if (this.owner.IsDead())
		{
			return false;
		}
		this.TrySetNewCode(code, userID);
		return this.HasALock;
	}

	// Token: 0x0600253F RID: 9535 RVA: 0x000E960B File Offset: 0x000E780B
	public bool IsValidLockCode(string code)
	{
		return code != null && code.Length == 4 && code.IsNumeric();
	}

	// Token: 0x06002540 RID: 9536 RVA: 0x000E9621 File Offset: 0x000E7821
	public bool TrySetNewCode(string newCode, ulong userID)
	{
		if (!this.IsValidLockCode(newCode))
		{
			return false;
		}
		this.Code = newCode;
		this.WhitelistPlayers.Clear();
		this.WhitelistPlayers.Add(userID);
		this.owner.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		return true;
	}

	// Token: 0x06002541 RID: 9537 RVA: 0x000E9659 File Offset: 0x000E7859
	public void RemoveLock()
	{
		if (!this.isServer)
		{
			return;
		}
		if (!this.HasALock)
		{
			return;
		}
		this.Code = "";
		this.owner.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06002542 RID: 9538 RVA: 0x000E9684 File Offset: 0x000E7884
	public bool TryOpenWithCode(BasePlayer player, string codeEntered)
	{
		if (this.CodeEntryBlocked(player))
		{
			return false;
		}
		if (!(codeEntered == this.Code))
		{
			if (Time.realtimeSinceStartup > this.lastWrongTime + 60f)
			{
				this.wrongCodes = 0;
			}
			player.Hurt((float)(this.wrongCodes + 1) * 5f, DamageType.ElectricShock, this.owner, false);
			this.wrongCodes++;
			if (this.wrongCodes > 5)
			{
				player.ShowToast(GameTip.Styles.Red_Normal, CodeLock.blockwarning, Array.Empty<string>());
			}
			if ((float)this.wrongCodes >= CodeLock.maxFailedAttempts)
			{
				this.owner.SetFlag(BaseEntity.Flags.Reserved10, true, false, true);
				this.owner.Invoke(new Action(this.ClearCodeEntryBlocked), CodeLock.lockoutCooldown);
			}
			this.lastWrongTime = Time.realtimeSinceStartup;
			return false;
		}
		if (!this.WhitelistPlayers.Contains(player.userID))
		{
			this.WhitelistPlayers.Add(player.userID);
			this.wrongCodes = 0;
		}
		this.owner.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		return true;
	}

	// Token: 0x06002543 RID: 9539 RVA: 0x000E978F File Offset: 0x000E798F
	private void ClearCodeEntryBlocked()
	{
		this.owner.SetFlag(BaseEntity.Flags.Reserved10, false, false, true);
		this.wrongCodes = 0;
	}

	// Token: 0x06002544 RID: 9540 RVA: 0x000E97AC File Offset: 0x000E79AC
	public void CheckEnableCentralLocking()
	{
		if (this.CentralLockingIsOn)
		{
			return;
		}
		bool flag = false;
		using (List<BaseVehicleModule>.Enumerator enumerator = this.owner.AttachedModuleEntities.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				VehicleModuleSeating vehicleModuleSeating;
				if ((vehicleModuleSeating = (enumerator.Current as VehicleModuleSeating)) != null && vehicleModuleSeating.HasADriverSeat() && vehicleModuleSeating.AnyMounted())
				{
					flag = true;
					break;
				}
			}
		}
		if (!flag)
		{
			this.owner.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
		}
	}

	// Token: 0x06002545 RID: 9541 RVA: 0x000E983C File Offset: 0x000E7A3C
	public void ToggleCentralLocking()
	{
		this.owner.SetFlag(BaseEntity.Flags.Reserved2, !this.CentralLockingIsOn, false, true);
	}

	// Token: 0x06002546 RID: 9542 RVA: 0x000E985C File Offset: 0x000E7A5C
	public void Save(BaseNetworkable.SaveInfo info)
	{
		info.msg.modularCar.hasLock = this.HasALock;
		if (info.forDisk)
		{
			info.msg.modularCar.lockCode = this.Code;
		}
		info.msg.modularCar.whitelistUsers = Pool.Get<List<ulong>>();
		info.msg.modularCar.whitelistUsers.AddRange(this.WhitelistPlayers);
	}

	// Token: 0x04001DD0 RID: 7632
	private readonly bool isServer;

	// Token: 0x04001DD1 RID: 7633
	private readonly ModularCar owner;

	// Token: 0x04001DD2 RID: 7634
	public const BaseEntity.Flags FLAG_CENTRAL_LOCKING = BaseEntity.Flags.Reserved2;

	// Token: 0x04001DD3 RID: 7635
	public const BaseEntity.Flags FLAG_CODE_ENTRY_BLOCKED = BaseEntity.Flags.Reserved10;

	// Token: 0x04001DD4 RID: 7636
	public const float LOCK_DESTROY_HEALTH = 0.2f;

	// Token: 0x04001DD7 RID: 7639
	private int wrongCodes;

	// Token: 0x04001DD8 RID: 7640
	private float lastWrongTime = float.NegativeInfinity;

	// Token: 0x02000CB1 RID: 3249
	public enum LockType
	{
		// Token: 0x0400437B RID: 17275
		Door,
		// Token: 0x0400437C RID: 17276
		General
	}
}
