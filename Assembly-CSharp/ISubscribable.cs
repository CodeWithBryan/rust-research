using System;

// Token: 0x0200049E RID: 1182
public interface ISubscribable
{
	// Token: 0x0600264F RID: 9807
	bool AddSubscription(ulong steamId);

	// Token: 0x06002650 RID: 9808
	bool RemoveSubscription(ulong steamId);

	// Token: 0x06002651 RID: 9809
	bool HasSubscription(ulong steamId);
}
