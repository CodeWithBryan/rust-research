using System;
using Facepunch;
using ProtoBuf;

namespace CompanionServer.Handlers
{
	// Token: 0x020009C2 RID: 2498
	public class EntityInfo : BaseEntityHandler<AppEmpty>
	{
		// Token: 0x06003B3D RID: 15165 RVA: 0x0015AD4C File Offset: 0x00158F4C
		public override void Execute()
		{
			AppEntityInfo appEntityInfo = Pool.Get<AppEntityInfo>();
			appEntityInfo.type = base.Entity.Type;
			appEntityInfo.payload = Pool.Get<AppEntityPayload>();
			base.Entity.FillEntityPayload(appEntityInfo.payload);
			AppResponse appResponse = Pool.Get<AppResponse>();
			appResponse.entityInfo = appEntityInfo;
			base.Send(appResponse);
		}
	}
}
