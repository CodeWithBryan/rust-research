using System;

// Token: 0x02000448 RID: 1096
public class BaseVehicleSeat : BaseVehicleMountPoint
{
	// Token: 0x060023E5 RID: 9189 RVA: 0x000E29B4 File Offset: 0x000E0BB4
	public override void ScaleDamageForPlayer(BasePlayer player, HitInfo info)
	{
		BaseVehicle baseVehicle = this.VehicleParent();
		if (baseVehicle == null)
		{
			return;
		}
		baseVehicle.ScaleDamageForPlayer(player, info);
	}

	// Token: 0x060023E6 RID: 9190 RVA: 0x000E29DC File Offset: 0x000E0BDC
	public override void MounteeTookDamage(BasePlayer mountee, HitInfo info)
	{
		BaseVehicle baseVehicle = this.VehicleParent();
		if (baseVehicle == null)
		{
			return;
		}
		baseVehicle.MounteeTookDamage(mountee, info);
	}

	// Token: 0x060023E7 RID: 9191 RVA: 0x000E2A04 File Offset: 0x000E0C04
	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		BaseVehicle baseVehicle = this.VehicleParent();
		if (baseVehicle != null)
		{
			baseVehicle.PlayerServerInput(inputState, player);
		}
		base.PlayerServerInput(inputState, player);
	}

	// Token: 0x060023E8 RID: 9192 RVA: 0x000E2A34 File Offset: 0x000E0C34
	public override void LightToggle(BasePlayer player)
	{
		BaseVehicle baseVehicle = this.VehicleParent();
		if (baseVehicle == null)
		{
			return;
		}
		baseVehicle.LightToggle(player);
	}

	// Token: 0x060023E9 RID: 9193 RVA: 0x000059DD File Offset: 0x00003BDD
	public override void SwitchParent(BaseEntity ent)
	{
	}

	// Token: 0x04001C72 RID: 7282
	public float mountedAnimationSpeed;

	// Token: 0x04001C73 RID: 7283
	public bool sendClientInputToVehicleParent;

	// Token: 0x04001C74 RID: 7284
	public bool forcePlayerModelUpdate;
}
