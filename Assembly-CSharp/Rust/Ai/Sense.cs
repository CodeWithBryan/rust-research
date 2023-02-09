using System;

namespace Rust.Ai
{
	// Token: 0x02000AFD RID: 2813
	public static class Sense
	{
		// Token: 0x06004399 RID: 17305 RVA: 0x00188284 File Offset: 0x00186484
		public static void Stimulate(Sensation sensation)
		{
			int inSphere = BaseEntity.Query.Server.GetInSphere(sensation.Position, sensation.Radius, Sense.query, new Func<BaseEntity, bool>(Sense.IsAbleToBeStimulated));
			float num = sensation.Radius * sensation.Radius;
			for (int i = 0; i < inSphere; i++)
			{
				if ((Sense.query[i].transform.position - sensation.Position).sqrMagnitude <= num)
				{
					Sense.query[i].OnSensation(sensation);
				}
			}
		}

		// Token: 0x0600439A RID: 17306 RVA: 0x00188307 File Offset: 0x00186507
		private static bool IsAbleToBeStimulated(BaseEntity ent)
		{
			return ent is BasePlayer || ent is BaseNpc;
		}

		// Token: 0x04003C1E RID: 15390
		private static BaseEntity[] query = new BaseEntity[512];
	}
}
