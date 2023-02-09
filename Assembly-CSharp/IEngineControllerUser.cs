using System;
using Rust;

// Token: 0x02000496 RID: 1174
public interface IEngineControllerUser : IEntity
{
	// Token: 0x0600261C RID: 9756
	bool HasFlag(BaseEntity.Flags f);

	// Token: 0x0600261D RID: 9757
	bool IsDead();

	// Token: 0x0600261E RID: 9758
	void SetFlag(BaseEntity.Flags f, bool b, bool recursive = false, bool networkupdate = true);

	// Token: 0x0600261F RID: 9759
	void Invoke(Action action, float time);

	// Token: 0x06002620 RID: 9760
	void CancelInvoke(Action action);

	// Token: 0x06002621 RID: 9761
	void OnEngineStartFailed();

	// Token: 0x06002622 RID: 9762
	bool MeetsEngineRequirements();
}
