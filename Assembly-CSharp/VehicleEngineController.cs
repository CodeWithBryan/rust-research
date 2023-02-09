using System;
using UnityEngine;

// Token: 0x02000497 RID: 1175
public class VehicleEngineController<TOwner> where TOwner : BaseVehicle, IEngineControllerUser
{
	// Token: 0x17000306 RID: 774
	// (get) Token: 0x06002623 RID: 9763 RVA: 0x000EDF03 File Offset: 0x000EC103
	public VehicleEngineController<TOwner>.EngineState CurEngineState
	{
		get
		{
			if (this.owner.HasFlag(this.engineStartingFlag))
			{
				return VehicleEngineController<TOwner>.EngineState.Starting;
			}
			if (this.owner.HasFlag(BaseEntity.Flags.On))
			{
				return VehicleEngineController<TOwner>.EngineState.On;
			}
			return VehicleEngineController<TOwner>.EngineState.Off;
		}
	}

	// Token: 0x17000307 RID: 775
	// (get) Token: 0x06002624 RID: 9764 RVA: 0x000EDF35 File Offset: 0x000EC135
	public bool IsOn
	{
		get
		{
			return this.CurEngineState == VehicleEngineController<TOwner>.EngineState.On;
		}
	}

	// Token: 0x17000308 RID: 776
	// (get) Token: 0x06002625 RID: 9765 RVA: 0x000EDF40 File Offset: 0x000EC140
	public bool IsOff
	{
		get
		{
			return this.CurEngineState == VehicleEngineController<TOwner>.EngineState.Off;
		}
	}

	// Token: 0x17000309 RID: 777
	// (get) Token: 0x06002626 RID: 9766 RVA: 0x000EDF4B File Offset: 0x000EC14B
	public bool IsStarting
	{
		get
		{
			return this.CurEngineState == VehicleEngineController<TOwner>.EngineState.Starting;
		}
	}

	// Token: 0x1700030A RID: 778
	// (get) Token: 0x06002627 RID: 9767 RVA: 0x000EDF56 File Offset: 0x000EC156
	public bool IsStartingOrOn
	{
		get
		{
			return this.CurEngineState > VehicleEngineController<TOwner>.EngineState.Off;
		}
	}

	// Token: 0x1700030B RID: 779
	// (get) Token: 0x06002628 RID: 9768 RVA: 0x000EDF61 File Offset: 0x000EC161
	// (set) Token: 0x06002629 RID: 9769 RVA: 0x000EDF69 File Offset: 0x000EC169
	public EntityFuelSystem FuelSystem { get; private set; }

	// Token: 0x0600262A RID: 9770 RVA: 0x000EDF74 File Offset: 0x000EC174
	public VehicleEngineController(TOwner owner, bool isServer, float engineStartupTime, GameObjectRef fuelStoragePrefab, Transform waterloggedPoint = null, BaseEntity.Flags engineStartingFlag = BaseEntity.Flags.Reserved1)
	{
		this.FuelSystem = new EntityFuelSystem(isServer, fuelStoragePrefab, owner.children, true);
		this.owner = owner;
		this.isServer = isServer;
		this.engineStartupTime = engineStartupTime;
		this.waterloggedPoint = waterloggedPoint;
		this.engineStartingFlag = engineStartingFlag;
	}

	// Token: 0x0600262B RID: 9771 RVA: 0x000EDFC6 File Offset: 0x000EC1C6
	public VehicleEngineController<TOwner>.EngineState EngineStateFrom(BaseEntity.Flags flags)
	{
		if (flags.HasFlag(this.engineStartingFlag))
		{
			return VehicleEngineController<TOwner>.EngineState.Starting;
		}
		if (flags.HasFlag(BaseEntity.Flags.On))
		{
			return VehicleEngineController<TOwner>.EngineState.On;
		}
		return VehicleEngineController<TOwner>.EngineState.Off;
	}

	// Token: 0x0600262C RID: 9772 RVA: 0x000EDFF8 File Offset: 0x000EC1F8
	public void TryStartEngine(BasePlayer player)
	{
		if (!this.isServer)
		{
			return;
		}
		if (this.owner.IsDead())
		{
			return;
		}
		if (this.IsStartingOrOn)
		{
			return;
		}
		if (player.net == null)
		{
			return;
		}
		if (!this.CanRunEngine())
		{
			this.owner.OnEngineStartFailed();
			return;
		}
		this.owner.SetFlag(this.engineStartingFlag, true, false, true);
		this.owner.SetFlag(BaseEntity.Flags.On, false, false, true);
		this.owner.Invoke(new Action(this.FinishStartingEngine), this.engineStartupTime);
	}

	// Token: 0x0600262D RID: 9773 RVA: 0x000EE09C File Offset: 0x000EC29C
	public void FinishStartingEngine()
	{
		if (!this.isServer)
		{
			return;
		}
		if (this.owner.IsDead())
		{
			return;
		}
		if (this.IsOn)
		{
			return;
		}
		this.owner.SetFlag(BaseEntity.Flags.On, true, false, true);
		this.owner.SetFlag(this.engineStartingFlag, false, false, true);
	}

	// Token: 0x0600262E RID: 9774 RVA: 0x000EE0FC File Offset: 0x000EC2FC
	public void StopEngine()
	{
		if (!this.isServer)
		{
			return;
		}
		if (this.IsOff)
		{
			return;
		}
		this.CancelEngineStart();
		this.owner.SetFlag(BaseEntity.Flags.On, false, false, true);
		this.owner.SetFlag(this.engineStartingFlag, false, false, true);
	}

	// Token: 0x0600262F RID: 9775 RVA: 0x000EE14E File Offset: 0x000EC34E
	public void CheckEngineState()
	{
		if (this.IsStartingOrOn && !this.CanRunEngine())
		{
			this.StopEngine();
		}
	}

	// Token: 0x06002630 RID: 9776 RVA: 0x000EE166 File Offset: 0x000EC366
	public bool CanRunEngine()
	{
		return this.owner.MeetsEngineRequirements() && this.FuelSystem.HasFuel(false) && !this.IsWaterlogged() && !this.owner.IsDead();
	}

	// Token: 0x06002631 RID: 9777 RVA: 0x000EE1A5 File Offset: 0x000EC3A5
	public bool IsWaterlogged()
	{
		return this.waterloggedPoint != null && WaterLevel.Test(this.waterloggedPoint.position, true, this.owner);
	}

	// Token: 0x06002632 RID: 9778 RVA: 0x000EE1D3 File Offset: 0x000EC3D3
	public int TickFuel(float fuelPerSecond)
	{
		if (this.IsOn)
		{
			return this.FuelSystem.TryUseFuel(Time.fixedDeltaTime, fuelPerSecond);
		}
		return 0;
	}

	// Token: 0x06002633 RID: 9779 RVA: 0x000EE1F0 File Offset: 0x000EC3F0
	private void CancelEngineStart()
	{
		if (this.CurEngineState != VehicleEngineController<TOwner>.EngineState.Starting)
		{
			return;
		}
		this.owner.CancelInvoke(new Action(this.FinishStartingEngine));
	}

	// Token: 0x04001F01 RID: 7937
	private readonly TOwner owner;

	// Token: 0x04001F02 RID: 7938
	private readonly bool isServer;

	// Token: 0x04001F03 RID: 7939
	private readonly float engineStartupTime;

	// Token: 0x04001F04 RID: 7940
	private readonly Transform waterloggedPoint;

	// Token: 0x04001F05 RID: 7941
	private readonly BaseEntity.Flags engineStartingFlag;

	// Token: 0x02000CC4 RID: 3268
	public enum EngineState
	{
		// Token: 0x040043B9 RID: 17337
		Off,
		// Token: 0x040043BA RID: 17338
		Starting,
		// Token: 0x040043BB RID: 17339
		On
	}
}
