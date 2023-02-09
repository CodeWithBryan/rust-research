using System;

namespace Rust.Modular
{
	// Token: 0x02000AE2 RID: 2786
	public static class EngineItemTypeEx
	{
		// Token: 0x060042EA RID: 17130 RVA: 0x00185707 File Offset: 0x00183907
		public static bool BoostsAcceleration(this EngineStorage.EngineItemTypes engineItemType)
		{
			return engineItemType == EngineStorage.EngineItemTypes.SparkPlug || engineItemType == EngineStorage.EngineItemTypes.Piston;
		}

		// Token: 0x060042EB RID: 17131 RVA: 0x00185713 File Offset: 0x00183913
		public static bool BoostsTopSpeed(this EngineStorage.EngineItemTypes engineItemType)
		{
			return engineItemType == EngineStorage.EngineItemTypes.Carburetor || engineItemType == EngineStorage.EngineItemTypes.Crankshaft || engineItemType == EngineStorage.EngineItemTypes.Piston;
		}

		// Token: 0x060042EC RID: 17132 RVA: 0x0014F13A File Offset: 0x0014D33A
		public static bool BoostsFuelEconomy(this EngineStorage.EngineItemTypes engineItemType)
		{
			return engineItemType == EngineStorage.EngineItemTypes.Carburetor || engineItemType == EngineStorage.EngineItemTypes.Valve;
		}
	}
}
