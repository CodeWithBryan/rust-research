using System;

// Token: 0x02000381 RID: 897
public class MobileInventoryEntity : BaseEntity
{
	// Token: 0x06001F4B RID: 8011 RVA: 0x000CF60D File Offset: 0x000CD80D
	public void ToggleRinging(bool state)
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, state, false, true);
	}

	// Token: 0x06001F4C RID: 8012 RVA: 0x000CF61D File Offset: 0x000CD81D
	public void SetSilentMode(bool wantsSilent)
	{
		base.SetFlag(MobileInventoryEntity.Flag_Silent, wantsSilent, false, true);
	}

	// Token: 0x040018B2 RID: 6322
	public SoundDefinition ringingLoop;

	// Token: 0x040018B3 RID: 6323
	public SoundDefinition silentLoop;

	// Token: 0x040018B4 RID: 6324
	public const BaseEntity.Flags Ringing = BaseEntity.Flags.Reserved1;

	// Token: 0x040018B5 RID: 6325
	public static BaseEntity.Flags Flag_Silent = BaseEntity.Flags.Reserved2;
}
