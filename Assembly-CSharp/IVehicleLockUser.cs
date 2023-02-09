using System;

// Token: 0x02000473 RID: 1139
public interface IVehicleLockUser
{
	// Token: 0x0600252C RID: 9516
	bool PlayerCanDestroyLock(BasePlayer player, BaseVehicleModule viaModule);

	// Token: 0x0600252D RID: 9517
	bool PlayerHasUnlockPermission(BasePlayer player);

	// Token: 0x0600252E RID: 9518
	bool PlayerCanUseThis(BasePlayer player, ModularCarCodeLock.LockType lockType);

	// Token: 0x0600252F RID: 9519
	void RemoveLock();
}
