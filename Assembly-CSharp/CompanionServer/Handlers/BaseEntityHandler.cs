using System;

namespace CompanionServer.Handlers
{
	// Token: 0x020009BF RID: 2495
	public abstract class BaseEntityHandler<T> : BaseHandler<T> where T : class
	{
		// Token: 0x17000496 RID: 1174
		// (get) Token: 0x06003B21 RID: 15137 RVA: 0x0015A9F5 File Offset: 0x00158BF5
		// (set) Token: 0x06003B22 RID: 15138 RVA: 0x0015A9FD File Offset: 0x00158BFD
		private protected AppIOEntity Entity { protected get; private set; }

		// Token: 0x06003B23 RID: 15139 RVA: 0x0015AA06 File Offset: 0x00158C06
		public override void EnterPool()
		{
			base.EnterPool();
			this.Entity = null;
		}

		// Token: 0x06003B24 RID: 15140 RVA: 0x0015AA18 File Offset: 0x00158C18
		public override ValidationResult Validate()
		{
			ValidationResult validationResult = base.Validate();
			if (validationResult != ValidationResult.Success)
			{
				return validationResult;
			}
			AppIOEntity appIOEntity = BaseNetworkable.serverEntities.Find(base.Request.entityId) as AppIOEntity;
			if (appIOEntity == null)
			{
				return ValidationResult.NotFound;
			}
			BuildingPrivlidge buildingPrivilege = appIOEntity.GetBuildingPrivilege();
			if (buildingPrivilege != null && !buildingPrivilege.IsAuthed(base.UserId))
			{
				return ValidationResult.NotFound;
			}
			this.Entity = appIOEntity;
			base.Client.Subscribe(new EntityTarget(base.Request.entityId));
			return ValidationResult.Success;
		}
	}
}
