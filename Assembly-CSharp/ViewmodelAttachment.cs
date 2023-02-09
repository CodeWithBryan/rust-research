using System;

// Token: 0x02000947 RID: 2375
public class ViewmodelAttachment : EntityComponent<BaseEntity>, IClientComponent, IViewModeChanged, IViewModelUpdated
{
	// Token: 0x0400326D RID: 12909
	public GameObjectRef modelObject;

	// Token: 0x0400326E RID: 12910
	public string targetBone;

	// Token: 0x0400326F RID: 12911
	public bool hideViewModelIronSights;
}
