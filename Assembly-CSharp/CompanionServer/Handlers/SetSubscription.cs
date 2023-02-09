using System;
using ProtoBuf;

namespace CompanionServer.Handlers
{
	// Token: 0x020009CB RID: 2507
	public class SetSubscription : BaseEntityHandler<AppFlag>
	{
		// Token: 0x06003B54 RID: 15188 RVA: 0x0015B378 File Offset: 0x00159578
		public override void Execute()
		{
			ISubscribable subscribable;
			if ((subscribable = (base.Entity as ISubscribable)) != null)
			{
				if (base.Proto.value)
				{
					if (subscribable.AddSubscription(base.UserId))
					{
						base.SendSuccess();
					}
					else
					{
						base.SendError("too_many_subscribers");
					}
				}
				else
				{
					subscribable.RemoveSubscription(base.UserId);
				}
				base.SendSuccess();
				return;
			}
			base.SendError("wrong_type");
		}
	}
}
