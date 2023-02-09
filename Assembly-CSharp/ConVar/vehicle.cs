using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AA1 RID: 2721
	[ConsoleSystem.Factory("vehicle")]
	public class vehicle : ConsoleSystem
	{
		// Token: 0x060040FB RID: 16635 RVA: 0x0017E5E4 File Offset: 0x0017C7E4
		[ServerUserVar]
		public static void swapseats(ConsoleSystem.Arg arg)
		{
			int targetSeat = 0;
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				return;
			}
			if (basePlayer.SwapSeatCooldown())
			{
				return;
			}
			BaseMountable mounted = basePlayer.GetMounted();
			if (mounted == null)
			{
				return;
			}
			BaseVehicle baseVehicle = mounted.GetComponent<BaseVehicle>();
			if (baseVehicle == null)
			{
				baseVehicle = mounted.VehicleParent();
			}
			if (baseVehicle == null)
			{
				return;
			}
			baseVehicle.SwapSeats(basePlayer, targetSeat);
		}

		// Token: 0x060040FC RID: 16636 RVA: 0x0017E648 File Offset: 0x0017C848
		[ServerVar]
		public static void fixcars(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				arg.ReplyWith("Null player.");
				return;
			}
			if (!basePlayer.IsAdmin)
			{
				arg.ReplyWith("Must be an admin to use fixcars.");
				return;
			}
			int num = arg.GetInt(0, 2);
			num = Mathf.Clamp(num, 1, 3);
			BaseVehicle[] array = UnityEngine.Object.FindObjectsOfType<BaseVehicle>();
			int num2 = 0;
			foreach (BaseVehicle baseVehicle in array)
			{
				if (baseVehicle.isServer && Vector3.Distance(baseVehicle.transform.position, basePlayer.transform.position) <= 10f && baseVehicle.AdminFixUp(num))
				{
					num2++;
				}
			}
			foreach (MLRS mlrs in UnityEngine.Object.FindObjectsOfType<MLRS>())
			{
				if (mlrs.isServer && Vector3.Distance(mlrs.transform.position, basePlayer.transform.position) <= 10f && mlrs.AdminFixUp())
				{
					num2++;
				}
			}
			arg.ReplyWith(string.Format("Fixed up {0} vehicles.", num2));
		}

		// Token: 0x060040FD RID: 16637 RVA: 0x0017E764 File Offset: 0x0017C964
		[ServerVar]
		public static void stop_all_trains(ConsoleSystem.Arg arg)
		{
			TrainEngine[] array = UnityEngine.Object.FindObjectsOfType<TrainEngine>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].StopEngine();
			}
			arg.ReplyWith("All trains stopped.");
		}

		// Token: 0x060040FE RID: 16638 RVA: 0x0017E798 File Offset: 0x0017C998
		[ServerVar]
		public static void killcars(ConsoleSystem.Arg args)
		{
			ModularCar[] array = BaseEntity.Util.FindAll<ModularCar>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Kill(BaseNetworkable.DestroyMode.None);
			}
		}

		// Token: 0x060040FF RID: 16639 RVA: 0x0017E7C4 File Offset: 0x0017C9C4
		[ServerVar]
		public static void killminis(ConsoleSystem.Arg args)
		{
			foreach (MiniCopter miniCopter in BaseEntity.Util.FindAll<MiniCopter>())
			{
				if (miniCopter.name.ToLower().Contains("minicopter"))
				{
					miniCopter.Kill(BaseNetworkable.DestroyMode.None);
				}
			}
		}

		// Token: 0x06004100 RID: 16640 RVA: 0x0017E808 File Offset: 0x0017CA08
		[ServerVar]
		public static void killscraphelis(ConsoleSystem.Arg args)
		{
			ScrapTransportHelicopter[] array = BaseEntity.Util.FindAll<ScrapTransportHelicopter>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Kill(BaseNetworkable.DestroyMode.None);
			}
		}

		// Token: 0x06004101 RID: 16641 RVA: 0x0017E834 File Offset: 0x0017CA34
		[ServerVar]
		public static void killtrains(ConsoleSystem.Arg args)
		{
			TrainCar[] array = BaseEntity.Util.FindAll<TrainCar>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Kill(BaseNetworkable.DestroyMode.None);
			}
		}

		// Token: 0x06004102 RID: 16642 RVA: 0x0017E860 File Offset: 0x0017CA60
		[ServerVar]
		public static void killboats(ConsoleSystem.Arg args)
		{
			BaseBoat[] array = BaseEntity.Util.FindAll<BaseBoat>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Kill(BaseNetworkable.DestroyMode.None);
			}
		}

		// Token: 0x04003A1C RID: 14876
		[ServerVar]
		[Help("how long until boat corpses despawn")]
		public static float boat_corpse_seconds = 300f;

		// Token: 0x04003A1D RID: 14877
		[ServerVar(Help = "If true, trains always explode when destroyed, and hitting a barrier always destroys the train immediately. Default: false")]
		public static bool cinematictrains = false;

		// Token: 0x04003A1E RID: 14878
		[ServerVar(Help = "Determines whether trains stop automatically when there's no-one on them. Default: false")]
		public static bool trainskeeprunning = false;

		// Token: 0x04003A1F RID: 14879
		[ServerVar(Help = "Determines whether modular cars turn into wrecks when destroyed, or just immediately gib. Default: true")]
		public static bool carwrecks = true;

		// Token: 0x04003A20 RID: 14880
		[ServerVar(Help = "Determines whether vehicles drop storage items when destroyed. Default: true")]
		public static bool vehiclesdroploot = true;
	}
}
