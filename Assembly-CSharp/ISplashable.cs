using System;

// Token: 0x020003F1 RID: 1009
public interface ISplashable
{
	// Token: 0x060021EA RID: 8682
	bool WantsSplash(ItemDefinition splashType, int amount);

	// Token: 0x060021EB RID: 8683
	int DoSplash(ItemDefinition splashType, int amount);
}
