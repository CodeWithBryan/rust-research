using System;
using Rust;
using Rust.Modular;
using UnityEngine;

// Token: 0x0200047C RID: 1148
public class VehicleModuleEngine : VehicleModuleStorage
{
	// Token: 0x170002E0 RID: 736
	// (get) Token: 0x06002559 RID: 9561 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool HasAnEngine
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170002E1 RID: 737
	// (get) Token: 0x0600255A RID: 9562 RVA: 0x000E9AC6 File Offset: 0x000E7CC6
	// (set) Token: 0x0600255B RID: 9563 RVA: 0x000E9ACE File Offset: 0x000E7CCE
	public bool IsUsable { get; private set; }

	// Token: 0x170002E2 RID: 738
	// (get) Token: 0x0600255C RID: 9564 RVA: 0x000E9AD7 File Offset: 0x000E7CD7
	// (set) Token: 0x0600255D RID: 9565 RVA: 0x000E9ADF File Offset: 0x000E7CDF
	public float PerformanceFractionAcceleration { get; private set; }

	// Token: 0x170002E3 RID: 739
	// (get) Token: 0x0600255E RID: 9566 RVA: 0x000E9AE8 File Offset: 0x000E7CE8
	// (set) Token: 0x0600255F RID: 9567 RVA: 0x000E9AF0 File Offset: 0x000E7CF0
	public float PerformanceFractionTopSpeed { get; private set; }

	// Token: 0x170002E4 RID: 740
	// (get) Token: 0x06002560 RID: 9568 RVA: 0x000E9AF9 File Offset: 0x000E7CF9
	// (set) Token: 0x06002561 RID: 9569 RVA: 0x000E9B01 File Offset: 0x000E7D01
	public float PerformanceFractionFuelEconomy { get; private set; }

	// Token: 0x170002E5 RID: 741
	// (get) Token: 0x06002562 RID: 9570 RVA: 0x000E9B0A File Offset: 0x000E7D0A
	// (set) Token: 0x06002563 RID: 9571 RVA: 0x000E9B12 File Offset: 0x000E7D12
	public float OverallPerformanceFraction { get; private set; }

	// Token: 0x170002E6 RID: 742
	// (get) Token: 0x06002564 RID: 9572 RVA: 0x000E9B1B File Offset: 0x000E7D1B
	public bool AtLowPerformance
	{
		get
		{
			return this.OverallPerformanceFraction <= 0.5f;
		}
	}

	// Token: 0x170002E7 RID: 743
	// (get) Token: 0x06002565 RID: 9573 RVA: 0x000E9B2D File Offset: 0x000E7D2D
	public bool AtPeakPerformance
	{
		get
		{
			return Mathf.Approximately(this.OverallPerformanceFraction, 1f);
		}
	}

	// Token: 0x170002E8 RID: 744
	// (get) Token: 0x06002566 RID: 9574 RVA: 0x000E9B3F File Offset: 0x000E7D3F
	public int KW
	{
		get
		{
			return this.engine.engineKW;
		}
	}

	// Token: 0x170002E9 RID: 745
	// (get) Token: 0x06002567 RID: 9575 RVA: 0x000E9B4C File Offset: 0x000E7D4C
	public EngineAudioSet AudioSet
	{
		get
		{
			return this.engine.audioSet;
		}
	}

	// Token: 0x170002EA RID: 746
	// (get) Token: 0x06002568 RID: 9576 RVA: 0x000E9B59 File Offset: 0x000E7D59
	private bool EngineIsOn
	{
		get
		{
			return base.Car != null && base.Car.CurEngineState == VehicleEngineController<GroundVehicle>.EngineState.On;
		}
	}

	// Token: 0x06002569 RID: 9577 RVA: 0x000E9B79 File Offset: 0x000E7D79
	public override void InitShared()
	{
		base.InitShared();
		this.RefreshPerformanceStats(base.GetContainer() as EngineStorage);
	}

	// Token: 0x0600256A RID: 9578 RVA: 0x000E9B92 File Offset: 0x000E7D92
	public override void OnEngineStateChanged(VehicleEngineController<GroundVehicle>.EngineState oldState, VehicleEngineController<GroundVehicle>.EngineState newState)
	{
		base.OnEngineStateChanged(oldState, newState);
		this.RefreshPerformanceStats(base.GetContainer() as EngineStorage);
	}

	// Token: 0x0600256B RID: 9579 RVA: 0x000E9BAD File Offset: 0x000E7DAD
	public override float GetMaxDriveForce()
	{
		if (!this.IsUsable)
		{
			return 0f;
		}
		return (float)this.engine.engineKW * 12.75f * this.PerformanceFractionTopSpeed;
	}

	// Token: 0x0600256C RID: 9580 RVA: 0x000E9BD8 File Offset: 0x000E7DD8
	public void RefreshPerformanceStats(EngineStorage engineStorage)
	{
		if (engineStorage == null)
		{
			this.IsUsable = false;
			this.PerformanceFractionAcceleration = 0f;
			this.PerformanceFractionTopSpeed = 0f;
			this.PerformanceFractionFuelEconomy = 0f;
		}
		else
		{
			this.IsUsable = engineStorage.isUsable;
			this.PerformanceFractionAcceleration = this.GetPerformanceFraction(engineStorage.accelerationBoostPercent);
			this.PerformanceFractionTopSpeed = this.GetPerformanceFraction(engineStorage.topSpeedBoostPercent);
			this.PerformanceFractionFuelEconomy = this.GetPerformanceFraction(engineStorage.fuelEconomyBoostPercent);
		}
		this.OverallPerformanceFraction = (this.PerformanceFractionAcceleration + this.PerformanceFractionTopSpeed + this.PerformanceFractionFuelEconomy) / 3f;
	}

	// Token: 0x0600256D RID: 9581 RVA: 0x000E9C7C File Offset: 0x000E7E7C
	private float GetPerformanceFraction(float statBoostPercent)
	{
		if (!this.IsUsable)
		{
			return 0f;
		}
		float num = Mathf.Lerp(0f, 0.25f, base.healthFraction);
		float num2;
		if (base.healthFraction == 0f)
		{
			num2 = 0f;
		}
		else
		{
			num2 = statBoostPercent * 0.75f;
		}
		return num + num2;
	}

	// Token: 0x0600256E RID: 9582 RVA: 0x000E9CCB File Offset: 0x000E7ECB
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		this.RefreshPerformanceStats(base.GetContainer() as EngineStorage);
	}

	// Token: 0x0600256F RID: 9583 RVA: 0x000E9CE5 File Offset: 0x000E7EE5
	public override bool CanBeLooted(BasePlayer player)
	{
		return base.CanBeLooted(player);
	}

	// Token: 0x06002570 RID: 9584 RVA: 0x000E9CF4 File Offset: 0x000E7EF4
	public override void VehicleFixedUpdate()
	{
		if (!this.isSpawned || !base.IsOnAVehicle)
		{
			return;
		}
		base.VehicleFixedUpdate();
		if (!base.Vehicle.IsMovingOrOn || base.Car == null)
		{
			return;
		}
		if (base.Car.CurEngineState == VehicleEngineController<GroundVehicle>.EngineState.On && this.IsUsable)
		{
			float num = Mathf.Lerp(this.engine.idleFuelPerSec, this.engine.maxFuelPerSec, Mathf.Abs(base.Car.GetThrottleInput()));
			num /= this.PerformanceFractionFuelEconomy;
			base.Car.TickFuel(num);
		}
	}

	// Token: 0x06002571 RID: 9585 RVA: 0x000E9D8C File Offset: 0x000E7F8C
	public override float GetAdjustedDriveForce(float absSpeed, float topSpeed)
	{
		float maxDriveForce = this.GetMaxDriveForce();
		float bias = Mathf.Lerp(0.0002f, 0.7f, this.PerformanceFractionAcceleration);
		float num = MathEx.BiasedLerp(1f - absSpeed / topSpeed, bias);
		return maxDriveForce * num;
	}

	// Token: 0x06002572 RID: 9586 RVA: 0x000E9DC8 File Offset: 0x000E7FC8
	public override void Hurt(HitInfo info)
	{
		base.Hurt(info);
		if (info.damageTypes.GetMajorityDamageType() == DamageType.Decay)
		{
			return;
		}
		float num = info.damageTypes.Total();
		EngineStorage engineStorage = base.GetContainer() as EngineStorage;
		if (engineStorage != null && num > 0f)
		{
			engineStorage.OnModuleDamaged(num);
		}
	}

	// Token: 0x06002573 RID: 9587 RVA: 0x000E9E1C File Offset: 0x000E801C
	public override void OnHealthChanged(float oldValue, float newValue)
	{
		base.OnHealthChanged(oldValue, newValue);
		if (!base.isServer)
		{
			return;
		}
		this.RefreshPerformanceStats(base.GetContainer() as EngineStorage);
	}

	// Token: 0x06002574 RID: 9588 RVA: 0x000E9E40 File Offset: 0x000E8040
	public override bool AdminFixUp(int tier)
	{
		if (!base.AdminFixUp(tier))
		{
			return false;
		}
		EngineStorage engineStorage = base.GetContainer() as EngineStorage;
		engineStorage.AdminAddParts(tier);
		this.RefreshPerformanceStats(engineStorage);
		return true;
	}

	// Token: 0x04001DEE RID: 7662
	[SerializeField]
	private VehicleModuleEngine.Engine engine;

	// Token: 0x04001DF4 RID: 7668
	private const float FORCE_MULTIPLIER = 12.75f;

	// Token: 0x04001DF5 RID: 7669
	private const float HEALTH_PERFORMANCE_FRACTION = 0.25f;

	// Token: 0x04001DF6 RID: 7670
	private const float LOW_PERFORMANCE_THRESHOLD = 0.5f;

	// Token: 0x04001DF7 RID: 7671
	private Sound badPerformanceLoop;

	// Token: 0x04001DF8 RID: 7672
	private SoundModulation.Modulator badPerformancePitchModulator;

	// Token: 0x04001DF9 RID: 7673
	private float prevSmokePercent;

	// Token: 0x04001DFA RID: 7674
	private const float MIN_FORCE_BIAS = 0.0002f;

	// Token: 0x04001DFB RID: 7675
	private const float MAX_FORCE_BIAS = 0.7f;

	// Token: 0x02000CB2 RID: 3250
	[Serializable]
	public class Engine
	{
		// Token: 0x0400437D RID: 17277
		[Header("Engine Stats")]
		public int engineKW;

		// Token: 0x0400437E RID: 17278
		public float idleFuelPerSec = 0.25f;

		// Token: 0x0400437F RID: 17279
		public float maxFuelPerSec = 0.25f;

		// Token: 0x04004380 RID: 17280
		[Header("Engine Audio")]
		public EngineAudioSet audioSet;

		// Token: 0x04004381 RID: 17281
		[Header("Engine FX")]
		public ParticleSystemContainer[] engineParticles;

		// Token: 0x04004382 RID: 17282
		public ParticleSystem[] exhaustSmoke;

		// Token: 0x04004383 RID: 17283
		public ParticleSystem[] exhaustBackfire;

		// Token: 0x04004384 RID: 17284
		public float exhaustSmokeMinOpacity = 0.1f;

		// Token: 0x04004385 RID: 17285
		public float exhaustSmokeMaxOpacity = 0.7f;

		// Token: 0x04004386 RID: 17286
		public float exhaustSmokeChangeRate = 0.5f;
	}
}
