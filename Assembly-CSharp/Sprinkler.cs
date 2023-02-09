using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;

// Token: 0x02000118 RID: 280
public class Sprinkler : IOEntity
{
	// Token: 0x170001B8 RID: 440
	// (get) Token: 0x06001591 RID: 5521 RVA: 0x000A6CC8 File Offset: 0x000A4EC8
	public override bool BlockFluidDraining
	{
		get
		{
			return this.currentFuelSource != null;
		}
	}

	// Token: 0x06001592 RID: 5522 RVA: 0x0004AF67 File Offset: 0x00049167
	public override int ConsumptionAmount()
	{
		return 2;
	}

	// Token: 0x06001593 RID: 5523 RVA: 0x000A6CD6 File Offset: 0x000A4ED6
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.UpdateHasPower(inputAmount, inputSlot);
		this.SetSprinklerState(inputAmount > 0);
	}

	// Token: 0x06001594 RID: 5524 RVA: 0x0003421C File Offset: 0x0003241C
	public override int CalculateCurrentEnergy(int inputAmount, int inputSlot)
	{
		return inputAmount;
	}

	// Token: 0x06001595 RID: 5525 RVA: 0x000A6CEC File Offset: 0x000A4EEC
	private void DoSplash()
	{
		using (TimeWarning.New("SprinklerSplash", 0))
		{
			int num = this.WaterPerSplash;
			if (this.updateSplashableCache > this.SplashFrequency * 4f || this.forceUpdateSplashables)
			{
				this.cachedSplashables.Clear();
				this.forceUpdateSplashables = false;
				this.updateSplashableCache = 0f;
				Vector3 position = this.Eyes.position;
				Vector3 up = base.transform.up;
				float num2 = Server.sprinklerEyeHeightOffset;
				float num3 = Vector3.Angle(up, Vector3.up) / 180f;
				num3 = Mathf.Clamp(num3, 0.2f, 1f);
				num2 *= num3;
				Vector3 startPosition = position + up * (Server.sprinklerRadius * 0.5f);
				Vector3 endPosition = position + up * num2;
				List<BaseEntity> list = Facepunch.Pool.GetList<BaseEntity>();
				global::Vis.Entities<BaseEntity>(startPosition, endPosition, Server.sprinklerRadius, list, 1236478737, QueryTriggerInteraction.Collide);
				if (list.Count > 0)
				{
					foreach (BaseEntity baseEntity in list)
					{
						ISplashable splashable;
						IOEntity entity;
						if (!baseEntity.isClient && (splashable = (baseEntity as ISplashable)) != null && !this.cachedSplashables.Contains(splashable) && splashable.WantsSplash(this.currentFuelType, num) && baseEntity.IsVisible(position, float.PositiveInfinity) && ((entity = (baseEntity as IOEntity)) == null || !base.IsConnectedTo(entity, IOEntity.backtracking, false)))
						{
							this.cachedSplashables.Add(splashable);
						}
					}
				}
				Facepunch.Pool.FreeList<BaseEntity>(ref list);
			}
			if (this.cachedSplashables.Count > 0)
			{
				int amount = num / this.cachedSplashables.Count;
				foreach (ISplashable splashable2 in this.cachedSplashables)
				{
					if (!splashable2.IsUnityNull<ISplashable>() && splashable2.WantsSplash(this.currentFuelType, amount))
					{
						int num4 = splashable2.DoSplash(this.currentFuelType, amount);
						num -= num4;
						if (num <= 0)
						{
							break;
						}
					}
				}
			}
			if (this.DecayPerSplash > 0f)
			{
				base.Hurt(this.DecayPerSplash);
			}
		}
	}

	// Token: 0x06001596 RID: 5526 RVA: 0x000A6F84 File Offset: 0x000A5184
	public void SetSprinklerState(bool wantsOn)
	{
		if (wantsOn)
		{
			this.TurnOn();
			return;
		}
		this.TurnOff();
	}

	// Token: 0x06001597 RID: 5527 RVA: 0x000A6F98 File Offset: 0x000A5198
	public void TurnOn()
	{
		if (base.IsOn())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		this.forceUpdateSplashables = true;
		if (!base.IsInvoking(new Action(this.DoSplash)))
		{
			base.InvokeRandomized(new Action(this.DoSplash), this.SplashFrequency * 0.5f, this.SplashFrequency, this.SplashFrequency * 0.2f);
		}
	}

	// Token: 0x06001598 RID: 5528 RVA: 0x000A7004 File Offset: 0x000A5204
	public void TurnOff()
	{
		if (!base.IsOn())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		if (base.IsInvoking(new Action(this.DoSplash)))
		{
			base.CancelInvoke(new Action(this.DoSplash));
		}
		this.currentFuelSource = null;
		this.currentFuelType = null;
	}

	// Token: 0x06001599 RID: 5529 RVA: 0x000A7058 File Offset: 0x000A5258
	public override void SetFuelType(ItemDefinition def, IOEntity source)
	{
		base.SetFuelType(def, source);
		this.currentFuelType = def;
		this.currentFuelSource = source;
	}

	// Token: 0x0600159A RID: 5530 RVA: 0x000A7070 File Offset: 0x000A5270
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk)
		{
			base.SetFlag(BaseEntity.Flags.On, false, false, false);
		}
	}

	// Token: 0x04000DEB RID: 3563
	public float SplashFrequency = 1f;

	// Token: 0x04000DEC RID: 3564
	public Transform Eyes;

	// Token: 0x04000DED RID: 3565
	public int WaterPerSplash = 1;

	// Token: 0x04000DEE RID: 3566
	public float DecayPerSplash = 0.8f;

	// Token: 0x04000DEF RID: 3567
	private ItemDefinition currentFuelType;

	// Token: 0x04000DF0 RID: 3568
	private IOEntity currentFuelSource;

	// Token: 0x04000DF1 RID: 3569
	private HashSet<ISplashable> cachedSplashables = new HashSet<ISplashable>();

	// Token: 0x04000DF2 RID: 3570
	private TimeSince updateSplashableCache;

	// Token: 0x04000DF3 RID: 3571
	private bool forceUpdateSplashables;
}
