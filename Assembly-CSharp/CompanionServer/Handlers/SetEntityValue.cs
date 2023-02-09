using System;
using ProtoBuf;

namespace CompanionServer.Handlers
{
	// Token: 0x020009CA RID: 2506
	public class SetEntityValue : BaseEntityHandler<AppSetEntityValue>
	{
		// Token: 0x06003B52 RID: 15186 RVA: 0x0015B330 File Offset: 0x00159530
		public override void Execute()
		{
			SmartSwitch smartSwitch;
			if ((smartSwitch = (base.Entity as SmartSwitch)) != null)
			{
				smartSwitch.Value = base.Proto.value;
				base.SendSuccess();
				return;
			}
			base.SendError("wrong_type");
		}
	}
}
