using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x020000AB RID: 171
public class PlayerMetabolism : BaseMetabolism<global::BasePlayer>
{
	// Token: 0x06000F9C RID: 3996 RVA: 0x00081228 File Offset: 0x0007F428
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PlayerMetabolism.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000F9D RID: 3997 RVA: 0x00081268 File Offset: 0x0007F468
	public override void Reset()
	{
		base.Reset();
		this.poison.Reset();
		this.radiation_level.Reset();
		this.radiation_poison.Reset();
		this.temperature.Reset();
		this.oxygen.Reset();
		this.bleeding.Reset();
		this.wetness.Reset();
		this.dirtyness.Reset();
		this.comfort.Reset();
		this.pending_health.Reset();
		this.lastConsumeTime = float.NegativeInfinity;
		this.isDirty = true;
	}

	// Token: 0x06000F9E RID: 3998 RVA: 0x000812FB File Offset: 0x0007F4FB
	public override void ServerUpdate(BaseCombatEntity ownerEntity, float delta)
	{
		base.ServerUpdate(ownerEntity, delta);
		this.SendChangesToClient();
	}

	// Token: 0x06000F9F RID: 3999 RVA: 0x0008130C File Offset: 0x0007F50C
	internal bool HasChanged()
	{
		bool flag = this.isDirty;
		flag = (this.calories.HasChanged() || flag);
		flag = (this.hydration.HasChanged() || flag);
		flag = (this.heartrate.HasChanged() || flag);
		flag = (this.poison.HasChanged() || flag);
		flag = (this.radiation_level.HasChanged() || flag);
		flag = (this.radiation_poison.HasChanged() || flag);
		flag = (this.temperature.HasChanged() || flag);
		flag = (this.wetness.HasChanged() || flag);
		flag = (this.dirtyness.HasChanged() || flag);
		flag = (this.comfort.HasChanged() || flag);
		return this.pending_health.HasChanged() || flag;
	}

	// Token: 0x06000FA0 RID: 4000 RVA: 0x000813BC File Offset: 0x0007F5BC
	protected override void DoMetabolismDamage(BaseCombatEntity ownerEntity, float delta)
	{
		base.DoMetabolismDamage(ownerEntity, delta);
		if (this.temperature.value < -20f)
		{
			this.owner.Hurt(Mathf.InverseLerp(1f, -50f, this.temperature.value) * delta * 1f, DamageType.Cold, null, true);
		}
		else if (this.temperature.value < -10f)
		{
			this.owner.Hurt(Mathf.InverseLerp(1f, -50f, this.temperature.value) * delta * 0.3f, DamageType.Cold, null, true);
		}
		else if (this.temperature.value < 1f)
		{
			this.owner.Hurt(Mathf.InverseLerp(1f, -50f, this.temperature.value) * delta * 0.1f, DamageType.Cold, null, true);
		}
		if (this.temperature.value > 60f)
		{
			this.owner.Hurt(Mathf.InverseLerp(60f, 200f, this.temperature.value) * delta * 5f, DamageType.Heat, null, true);
		}
		if (this.oxygen.value < 0.5f)
		{
			this.owner.Hurt(Mathf.InverseLerp(0.5f, 0f, this.oxygen.value) * delta * 20f, DamageType.Drowned, null, false);
		}
		if (this.bleeding.value > 0f)
		{
			float num = delta * 0.33333334f;
			this.owner.Hurt(num, DamageType.Bleeding, null, true);
			this.bleeding.Subtract(num);
		}
		if (this.poison.value > 0f)
		{
			this.owner.Hurt(this.poison.value * delta * 0.1f, DamageType.Poison, null, true);
		}
		if (ConVar.Server.radiation && this.radiation_poison.value > 0f)
		{
			float num2 = (1f + Mathf.Clamp01(this.radiation_poison.value / 25f) * 5f) * (delta / 5f);
			this.owner.Hurt(num2, DamageType.Radiation, null, true);
			this.radiation_poison.Subtract(num2);
		}
	}

	// Token: 0x06000FA1 RID: 4001 RVA: 0x000815EA File Offset: 0x0007F7EA
	public bool SignificantBleeding()
	{
		return this.bleeding.value > 0f;
	}

	// Token: 0x06000FA2 RID: 4002 RVA: 0x00081600 File Offset: 0x0007F800
	protected override void RunMetabolism(BaseCombatEntity ownerEntity, float delta)
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		float currentTemperature = this.owner.currentTemperature;
		float fTarget = this.owner.currentComfort;
		float currentCraftLevel = this.owner.currentCraftLevel;
		this.owner.SetPlayerFlag(global::BasePlayer.PlayerFlags.Workbench1, currentCraftLevel == 1f);
		this.owner.SetPlayerFlag(global::BasePlayer.PlayerFlags.Workbench2, currentCraftLevel == 2f);
		this.owner.SetPlayerFlag(global::BasePlayer.PlayerFlags.Workbench3, currentCraftLevel == 3f);
		this.owner.SetPlayerFlag(global::BasePlayer.PlayerFlags.SafeZone, this.owner.InSafeZone());
		if (activeGameMode == null || activeGameMode.allowTemperature)
		{
			float num = currentTemperature;
			num -= this.DeltaWet() * 34f;
			float num2 = Mathf.Clamp(this.owner.baseProtection.amounts[18] * 1.5f, -1f, 1f);
			float num3 = Mathf.InverseLerp(20f, -50f, currentTemperature);
			float num4 = Mathf.InverseLerp(20f, 30f, currentTemperature);
			num += num3 * 70f * num2;
			num += num4 * 10f * Mathf.Abs(num2);
			num += this.heartrate.value * 5f;
			this.temperature.MoveTowards(num, delta * 5f);
		}
		else
		{
			this.temperature.value = 25f;
		}
		if (this.temperature.value >= 40f)
		{
			fTarget = 0f;
		}
		this.comfort.MoveTowards(fTarget, delta / 5f);
		float num5 = 0.6f + 0.4f * this.comfort.value;
		if (this.calories.value > 100f && this.owner.healthFraction < num5 && this.radiation_poison.Fraction() < 0.25f && this.owner.SecondsSinceAttacked > 10f && !this.SignificantBleeding() && this.temperature.value >= 10f && this.hydration.value > 40f)
		{
			float num6 = Mathf.InverseLerp(this.calories.min, this.calories.max, this.calories.value);
			float num7 = 5f;
			float num8 = num7 * this.owner.MaxHealth() * 0.8f / 600f;
			num8 += num8 * num6 * 0.5f;
			float num9 = num8 / num7;
			num9 += num9 * this.comfort.value * 6f;
			ownerEntity.Heal(num9 * delta);
			this.calories.Subtract(num8 * delta);
			this.hydration.Subtract(num8 * delta * 0.2f);
		}
		float num10 = this.owner.estimatedSpeed2D / this.owner.GetMaxSpeed() * 0.75f;
		float fTarget2 = Mathf.Clamp(0.05f + num10, 0f, 1f);
		this.heartrate.MoveTowards(fTarget2, delta * 0.1f);
		if (!this.owner.IsGod())
		{
			float num11 = this.heartrate.Fraction() * 0.375f;
			this.calories.MoveTowards(0f, delta * num11);
			float num12 = 0.008333334f;
			num12 += Mathf.InverseLerp(40f, 60f, this.temperature.value) * 0.083333336f;
			num12 += this.heartrate.value * 0.06666667f;
			this.hydration.MoveTowards(0f, delta * num12);
		}
		bool b = this.hydration.Fraction() <= 0f || this.radiation_poison.value >= 100f;
		this.owner.SetPlayerFlag(global::BasePlayer.PlayerFlags.NoSprint, b);
		if (this.temperature.value > 40f)
		{
			this.hydration.Add(Mathf.InverseLerp(40f, 200f, this.temperature.value) * delta * -1f);
		}
		if (this.temperature.value < 10f)
		{
			float num13 = Mathf.InverseLerp(20f, -100f, this.temperature.value);
			this.heartrate.MoveTowards(Mathf.Lerp(0.2f, 1f, num13), delta * 2f * num13);
		}
		float num14 = this.owner.AirFactor();
		float num15 = (num14 > this.oxygen.value) ? 1f : 0.1f;
		this.oxygen.MoveTowards(num14, delta * num15);
		float f = 0f;
		float f2 = 0f;
		if (this.owner.IsOutside(this.owner.eyes.position))
		{
			f = Climate.GetRain(this.owner.eyes.position) * Weather.wetness_rain;
			f2 = Climate.GetSnow(this.owner.eyes.position) * Weather.wetness_snow;
		}
		bool flag = this.owner.baseProtection.amounts[4] > 0f;
		float num16 = this.owner.currentEnvironmentalWetness;
		num16 = Mathf.Clamp(num16, 0f, 0.8f);
		float num17 = this.owner.WaterFactor();
		if (!flag && num17 > 0f)
		{
			this.wetness.value = Mathf.Max(this.wetness.value, Mathf.Clamp(num17, this.wetness.min, this.wetness.max));
		}
		float num18 = Mathx.Max(this.wetness.value, f, f2, num16);
		num18 = Mathf.Min(num18, flag ? 0f : num18);
		this.wetness.MoveTowards(num18, delta * 0.05f);
		if (num17 < this.wetness.value && num16 <= 0f)
		{
			this.wetness.MoveTowards(0f, delta * 0.2f * Mathf.InverseLerp(0f, 100f, currentTemperature));
		}
		this.poison.MoveTowards(0f, delta * 0.5555556f);
		if (this.wetness.Fraction() > 0.4f && this.owner.estimatedSpeed > 0.25f && this.radiation_level.Fraction() == 0f)
		{
			this.radiation_poison.Subtract(this.radiation_poison.value * 0.2f * this.wetness.Fraction() * delta * 0.2f);
		}
		if (ConVar.Server.radiation && !this.owner.IsGod())
		{
			this.radiation_level.value = this.owner.radiationLevel;
			if (this.radiation_level.value > 0f)
			{
				this.radiation_poison.Add(this.radiation_level.value * delta);
			}
		}
		if (this.pending_health.value > 0f)
		{
			float num19 = Mathf.Min(1f * delta, this.pending_health.value);
			ownerEntity.Heal(num19);
			if (ownerEntity.healthFraction == 1f)
			{
				this.pending_health.value = 0f;
				return;
			}
			this.pending_health.Subtract(num19);
		}
	}

	// Token: 0x06000FA3 RID: 4003 RVA: 0x00081D60 File Offset: 0x0007FF60
	private float DeltaHot()
	{
		return Mathf.InverseLerp(20f, 100f, this.temperature.value);
	}

	// Token: 0x06000FA4 RID: 4004 RVA: 0x00081D7C File Offset: 0x0007FF7C
	private float DeltaCold()
	{
		return Mathf.InverseLerp(20f, -50f, this.temperature.value);
	}

	// Token: 0x06000FA5 RID: 4005 RVA: 0x00081D98 File Offset: 0x0007FF98
	private float DeltaWet()
	{
		return this.wetness.value;
	}

	// Token: 0x06000FA6 RID: 4006 RVA: 0x00081DA5 File Offset: 0x0007FFA5
	public void UseHeart(float frate)
	{
		if (this.heartrate.value > frate)
		{
			this.heartrate.Add(frate);
			return;
		}
		this.heartrate.value = frate;
	}

	// Token: 0x06000FA7 RID: 4007 RVA: 0x00081DD0 File Offset: 0x0007FFD0
	public void SendChangesToClient()
	{
		if (!this.HasChanged())
		{
			return;
		}
		this.isDirty = false;
		using (ProtoBuf.PlayerMetabolism playerMetabolism = this.Save())
		{
			base.baseEntity.ClientRPCPlayerAndSpectators<ProtoBuf.PlayerMetabolism>(null, base.baseEntity, "UpdateMetabolism", playerMetabolism);
		}
	}

	// Token: 0x06000FA8 RID: 4008 RVA: 0x00081E28 File Offset: 0x00080028
	public bool CanConsume()
	{
		return (!this.owner || !this.owner.IsHeadUnderwater()) && UnityEngine.Time.time - this.lastConsumeTime > 1f;
	}

	// Token: 0x06000FA9 RID: 4009 RVA: 0x00081E59 File Offset: 0x00080059
	public void MarkConsumption()
	{
		this.lastConsumeTime = UnityEngine.Time.time;
	}

	// Token: 0x06000FAA RID: 4010 RVA: 0x00081E68 File Offset: 0x00080068
	public ProtoBuf.PlayerMetabolism Save()
	{
		ProtoBuf.PlayerMetabolism playerMetabolism = Facepunch.Pool.Get<ProtoBuf.PlayerMetabolism>();
		playerMetabolism.calories = this.calories.value;
		playerMetabolism.hydration = this.hydration.value;
		playerMetabolism.heartrate = this.heartrate.value;
		playerMetabolism.temperature = this.temperature.value;
		playerMetabolism.radiation_level = this.radiation_level.value;
		playerMetabolism.radiation_poisoning = this.radiation_poison.value;
		playerMetabolism.wetness = this.wetness.value;
		playerMetabolism.dirtyness = this.dirtyness.value;
		playerMetabolism.oxygen = this.oxygen.value;
		playerMetabolism.bleeding = this.bleeding.value;
		playerMetabolism.comfort = this.comfort.value;
		playerMetabolism.pending_health = this.pending_health.value;
		if (this.owner)
		{
			playerMetabolism.health = this.owner.Health();
		}
		return playerMetabolism;
	}

	// Token: 0x06000FAB RID: 4011 RVA: 0x00081F68 File Offset: 0x00080168
	public void Load(ProtoBuf.PlayerMetabolism s)
	{
		this.calories.SetValue(s.calories);
		this.hydration.SetValue(s.hydration);
		this.comfort.SetValue(s.comfort);
		this.heartrate.value = s.heartrate;
		this.temperature.value = s.temperature;
		this.radiation_level.value = s.radiation_level;
		this.radiation_poison.value = s.radiation_poisoning;
		this.wetness.value = s.wetness;
		this.dirtyness.value = s.dirtyness;
		this.oxygen.value = s.oxygen;
		this.bleeding.value = s.bleeding;
		this.pending_health.value = s.pending_health;
		if (this.owner)
		{
			this.owner.health = s.health;
		}
	}

	// Token: 0x06000FAC RID: 4012 RVA: 0x00082060 File Offset: 0x00080260
	public override MetabolismAttribute FindAttribute(MetabolismAttribute.Type type)
	{
		switch (type)
		{
		case MetabolismAttribute.Type.Poison:
			return this.poison;
		case MetabolismAttribute.Type.Radiation:
			return this.radiation_poison;
		case MetabolismAttribute.Type.Bleeding:
			return this.bleeding;
		case MetabolismAttribute.Type.HealthOverTime:
			return this.pending_health;
		}
		return base.FindAttribute(type);
	}

	// Token: 0x040009F4 RID: 2548
	public const float HotThreshold = 40f;

	// Token: 0x040009F5 RID: 2549
	public const float ColdThreshold = 5f;

	// Token: 0x040009F6 RID: 2550
	public const float OxygenHurtThreshold = 0.5f;

	// Token: 0x040009F7 RID: 2551
	public const float OxygenDepleteTime = 10f;

	// Token: 0x040009F8 RID: 2552
	public const float OxygenRefillTime = 1f;

	// Token: 0x040009F9 RID: 2553
	public MetabolismAttribute temperature = new MetabolismAttribute();

	// Token: 0x040009FA RID: 2554
	public MetabolismAttribute poison = new MetabolismAttribute();

	// Token: 0x040009FB RID: 2555
	public MetabolismAttribute radiation_level = new MetabolismAttribute();

	// Token: 0x040009FC RID: 2556
	public MetabolismAttribute radiation_poison = new MetabolismAttribute();

	// Token: 0x040009FD RID: 2557
	public MetabolismAttribute wetness = new MetabolismAttribute();

	// Token: 0x040009FE RID: 2558
	public MetabolismAttribute dirtyness = new MetabolismAttribute();

	// Token: 0x040009FF RID: 2559
	public MetabolismAttribute oxygen = new MetabolismAttribute();

	// Token: 0x04000A00 RID: 2560
	public MetabolismAttribute bleeding = new MetabolismAttribute();

	// Token: 0x04000A01 RID: 2561
	public MetabolismAttribute comfort = new MetabolismAttribute();

	// Token: 0x04000A02 RID: 2562
	public MetabolismAttribute pending_health = new MetabolismAttribute();

	// Token: 0x04000A03 RID: 2563
	public bool isDirty;

	// Token: 0x04000A04 RID: 2564
	private float lastConsumeTime;
}
