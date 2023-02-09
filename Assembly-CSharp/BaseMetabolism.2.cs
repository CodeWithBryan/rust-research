using System;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x020003DD RID: 989
public abstract class BaseMetabolism<T> : EntityComponent<T> where T : BaseCombatEntity
{
	// Token: 0x0600218B RID: 8587 RVA: 0x000D793C File Offset: 0x000D5B3C
	public virtual void Reset()
	{
		this.calories.Reset();
		this.hydration.Reset();
		this.heartrate.Reset();
	}

	// Token: 0x0600218C RID: 8588 RVA: 0x000D795F File Offset: 0x000D5B5F
	protected virtual void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		this.owner = default(T);
	}

	// Token: 0x0600218D RID: 8589 RVA: 0x000D7975 File Offset: 0x000D5B75
	public virtual void ServerInit(T owner)
	{
		this.Reset();
		this.owner = owner;
	}

	// Token: 0x0600218E RID: 8590 RVA: 0x000D7984 File Offset: 0x000D5B84
	public virtual void ServerUpdate(BaseCombatEntity ownerEntity, float delta)
	{
		this.timeSinceLastMetabolism += delta;
		if (this.timeSinceLastMetabolism <= ConVar.Server.metabolismtick)
		{
			return;
		}
		if (this.owner && !this.owner.IsDead())
		{
			this.RunMetabolism(ownerEntity, this.timeSinceLastMetabolism);
			this.DoMetabolismDamage(ownerEntity, this.timeSinceLastMetabolism);
		}
		this.timeSinceLastMetabolism = 0f;
	}

	// Token: 0x0600218F RID: 8591 RVA: 0x000D79F8 File Offset: 0x000D5BF8
	protected virtual void DoMetabolismDamage(BaseCombatEntity ownerEntity, float delta)
	{
		if (this.calories.value <= 20f)
		{
			using (TimeWarning.New("Calories Hurt", 0))
			{
				ownerEntity.Hurt(Mathf.InverseLerp(20f, 0f, this.calories.value) * delta * 0.083333336f, DamageType.Hunger, null, true);
			}
		}
		if (this.hydration.value <= 20f)
		{
			using (TimeWarning.New("Hyration Hurt", 0))
			{
				ownerEntity.Hurt(Mathf.InverseLerp(20f, 0f, this.hydration.value) * delta * 0.13333334f, DamageType.Thirst, null, true);
			}
		}
	}

	// Token: 0x06002190 RID: 8592 RVA: 0x000D7ACC File Offset: 0x000D5CCC
	protected virtual void RunMetabolism(BaseCombatEntity ownerEntity, float delta)
	{
		if (this.calories.value > 200f)
		{
			ownerEntity.Heal(Mathf.InverseLerp(200f, 1000f, this.calories.value) * delta * 0.016666668f);
		}
		if (this.hydration.value > 200f)
		{
			ownerEntity.Heal(Mathf.InverseLerp(200f, 1000f, this.hydration.value) * delta * 0.016666668f);
		}
		this.hydration.MoveTowards(0f, delta * 0.008333334f);
		this.calories.MoveTowards(0f, delta * 0.016666668f);
		this.heartrate.MoveTowards(0.05f, delta * 0.016666668f);
	}

	// Token: 0x06002191 RID: 8593 RVA: 0x000D7B94 File Offset: 0x000D5D94
	public void ApplyChange(MetabolismAttribute.Type type, float amount, float time)
	{
		MetabolismAttribute metabolismAttribute = this.FindAttribute(type);
		if (metabolismAttribute == null)
		{
			return;
		}
		metabolismAttribute.Add(amount);
	}

	// Token: 0x06002192 RID: 8594 RVA: 0x000D7BB4 File Offset: 0x000D5DB4
	public bool ShouldDie()
	{
		return this.owner && this.owner.Health() <= 0f;
	}

	// Token: 0x06002193 RID: 8595 RVA: 0x000D7BE4 File Offset: 0x000D5DE4
	public virtual MetabolismAttribute FindAttribute(MetabolismAttribute.Type type)
	{
		switch (type)
		{
		case MetabolismAttribute.Type.Calories:
			return this.calories;
		case MetabolismAttribute.Type.Hydration:
			return this.hydration;
		case MetabolismAttribute.Type.Heartrate:
			return this.heartrate;
		default:
			return null;
		}
	}

	// Token: 0x040019F6 RID: 6646
	protected T owner;

	// Token: 0x040019F7 RID: 6647
	public MetabolismAttribute calories = new MetabolismAttribute();

	// Token: 0x040019F8 RID: 6648
	public MetabolismAttribute hydration = new MetabolismAttribute();

	// Token: 0x040019F9 RID: 6649
	public MetabolismAttribute heartrate = new MetabolismAttribute();

	// Token: 0x040019FA RID: 6650
	protected float timeSinceLastMetabolism;
}
