using System;

// Token: 0x02000447 RID: 1095
public class BaseVehicleMountPoint : BaseMountable
{
	// Token: 0x060023DF RID: 9183 RVA: 0x00007074 File Offset: 0x00005274
	public override bool DirectlyMountable()
	{
		return false;
	}

	// Token: 0x060023E0 RID: 9184 RVA: 0x000E28EC File Offset: 0x000E0AEC
	public override BaseVehicle VehicleParent()
	{
		BaseVehicle baseVehicle = base.GetParentEntity() as BaseVehicle;
		while (baseVehicle != null && !baseVehicle.IsVehicleRoot())
		{
			BaseVehicle baseVehicle2 = baseVehicle.GetParentEntity() as BaseVehicle;
			if (baseVehicle2 == null)
			{
				return baseVehicle;
			}
			baseVehicle = baseVehicle2;
		}
		return baseVehicle;
	}

	// Token: 0x060023E1 RID: 9185 RVA: 0x000E2934 File Offset: 0x000E0B34
	public override bool BlocksWaterFor(BasePlayer player)
	{
		BaseVehicle baseVehicle = this.VehicleParent();
		return !(baseVehicle == null) && baseVehicle.BlocksWaterFor(player);
	}

	// Token: 0x060023E2 RID: 9186 RVA: 0x000E295C File Offset: 0x000E0B5C
	public override float WaterFactorForPlayer(BasePlayer player)
	{
		BaseVehicle baseVehicle = this.VehicleParent();
		if (baseVehicle == null)
		{
			return 0f;
		}
		return baseVehicle.WaterFactorForPlayer(player);
	}

	// Token: 0x060023E3 RID: 9187 RVA: 0x000E2988 File Offset: 0x000E0B88
	public override float AirFactor()
	{
		BaseVehicle baseVehicle = this.VehicleParent();
		if (baseVehicle == null)
		{
			return 0f;
		}
		return baseVehicle.AirFactor();
	}
}
