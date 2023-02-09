using System;
using ProtoBuf;

namespace CompanionServer.Handlers
{
	// Token: 0x020009C1 RID: 2497
	public class CheckSubscription : BaseEntityHandler<AppEmpty>
	{
		// Token: 0x06003B3B RID: 15163 RVA: 0x0015AD08 File Offset: 0x00158F08
		public override void Execute()
		{
			ISubscribable subscribable;
			if ((subscribable = (base.Entity as ISubscribable)) != null)
			{
				bool value = subscribable.HasSubscription(base.UserId);
				base.SendFlag(value);
				return;
			}
			base.SendError("wrong_type");
		}
	}
}
