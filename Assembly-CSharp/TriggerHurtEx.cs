using System;
using System.Collections.Generic;
using System.Linq;
using Rust;
using UnityEngine;

// Token: 0x0200071B RID: 1819
public class TriggerHurtEx : TriggerBase, IServerComponent, IHurtTrigger
{
	// Token: 0x0600327F RID: 12927 RVA: 0x001381E0 File Offset: 0x001363E0
	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		if (!(baseEntity is BaseCombatEntity))
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x06003280 RID: 12928 RVA: 0x00138230 File Offset: 0x00136430
	internal void DoDamage(BaseEntity ent, TriggerHurtEx.HurtType type, List<DamageTypeEntry> damage, GameObjectRef effect, float multiply = 1f)
	{
		if (!this.damageEnabled)
		{
			return;
		}
		using (TimeWarning.New("TriggerHurtEx.DoDamage", 0))
		{
			if (damage != null && damage.Count > 0)
			{
				BaseCombatEntity baseCombatEntity = ent as BaseCombatEntity;
				if (baseCombatEntity)
				{
					HitInfo hitInfo = new HitInfo();
					hitInfo.damageTypes.Add(damage);
					hitInfo.damageTypes.ScaleAll(multiply);
					hitInfo.DoHitEffects = true;
					hitInfo.DidHit = true;
					hitInfo.Initiator = base.gameObject.ToBaseEntity();
					hitInfo.PointStart = base.transform.position;
					hitInfo.PointEnd = baseCombatEntity.transform.position;
					if (type == TriggerHurtEx.HurtType.Simple)
					{
						baseCombatEntity.Hurt(hitInfo);
					}
					else
					{
						baseCombatEntity.OnAttacked(hitInfo);
					}
				}
			}
			if (effect.isValid)
			{
				Effect.server.Run(effect.resourcePath, ent, StringPool.closest, base.transform.position, Vector3.up, null, false);
			}
		}
	}

	// Token: 0x06003281 RID: 12929 RVA: 0x00138330 File Offset: 0x00136530
	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		if (ent == null)
		{
			return;
		}
		if (this.entityAddList == null)
		{
			this.entityAddList = new List<BaseEntity>();
		}
		this.entityAddList.Add(ent);
		base.Invoke(new Action(this.ProcessQueues), 0.1f);
	}

	// Token: 0x06003282 RID: 12930 RVA: 0x00138384 File Offset: 0x00136584
	internal override void OnEntityLeave(BaseEntity ent)
	{
		base.OnEntityLeave(ent);
		if (ent == null)
		{
			return;
		}
		if (this.entityLeaveList == null)
		{
			this.entityLeaveList = new List<BaseEntity>();
		}
		this.entityLeaveList.Add(ent);
		base.Invoke(new Action(this.ProcessQueues), 0.1f);
	}

	// Token: 0x06003283 RID: 12931 RVA: 0x001383D8 File Offset: 0x001365D8
	internal override void OnObjects()
	{
		base.InvokeRepeating(new Action(this.OnTick), this.repeatRate, this.repeatRate);
	}

	// Token: 0x06003284 RID: 12932 RVA: 0x001383F8 File Offset: 0x001365F8
	internal override void OnEmpty()
	{
		base.CancelInvoke(new Action(this.OnTick));
	}

	// Token: 0x06003285 RID: 12933 RVA: 0x0013840C File Offset: 0x0013660C
	private void OnTick()
	{
		this.ProcessQueues();
		if (this.entityInfo != null)
		{
			foreach (KeyValuePair<BaseEntity, TriggerHurtEx.EntityTriggerInfo> keyValuePair in this.entityInfo.ToArray<KeyValuePair<BaseEntity, TriggerHurtEx.EntityTriggerInfo>>())
			{
				if (keyValuePair.Key.IsValid())
				{
					Vector3 position = keyValuePair.Key.transform.position;
					float magnitude = (position - keyValuePair.Value.lastPosition).magnitude;
					if (magnitude > 0.01f)
					{
						keyValuePair.Value.lastPosition = position;
						this.DoDamage(keyValuePair.Key, this.hurtTypeOnMove, this.damageOnMove, this.effectOnMove, magnitude);
					}
					this.DoDamage(keyValuePair.Key, this.hurtTypeOnTimer, this.damageOnTimer, this.effectOnTimer, this.repeatRate);
				}
			}
		}
	}

	// Token: 0x06003286 RID: 12934 RVA: 0x001384F0 File Offset: 0x001366F0
	private void ProcessQueues()
	{
		if (this.entityAddList != null)
		{
			foreach (BaseEntity baseEntity in this.entityAddList)
			{
				if (baseEntity.IsValid())
				{
					this.DoDamage(baseEntity, this.hurtTypeOnEnter, this.damageOnEnter, this.effectOnEnter, 1f);
					if (this.entityInfo == null)
					{
						this.entityInfo = new Dictionary<BaseEntity, TriggerHurtEx.EntityTriggerInfo>();
					}
					if (!this.entityInfo.ContainsKey(baseEntity))
					{
						this.entityInfo.Add(baseEntity, new TriggerHurtEx.EntityTriggerInfo
						{
							lastPosition = baseEntity.transform.position
						});
					}
				}
			}
			this.entityAddList = null;
		}
		if (this.entityLeaveList != null)
		{
			foreach (BaseEntity baseEntity2 in this.entityLeaveList)
			{
				if (baseEntity2.IsValid())
				{
					this.DoDamage(baseEntity2, this.hurtTypeOnLeave, this.damageOnLeave, this.effectOnLeave, 1f);
					if (this.entityInfo != null)
					{
						this.entityInfo.Remove(baseEntity2);
						if (this.entityInfo.Count == 0)
						{
							this.entityInfo = null;
						}
					}
				}
			}
			this.entityLeaveList.Clear();
		}
	}

	// Token: 0x040028D1 RID: 10449
	public float repeatRate = 0.1f;

	// Token: 0x040028D2 RID: 10450
	[Header("On Enter")]
	public List<DamageTypeEntry> damageOnEnter;

	// Token: 0x040028D3 RID: 10451
	public GameObjectRef effectOnEnter;

	// Token: 0x040028D4 RID: 10452
	public TriggerHurtEx.HurtType hurtTypeOnEnter;

	// Token: 0x040028D5 RID: 10453
	[Header("On Timer (damage per second)")]
	public List<DamageTypeEntry> damageOnTimer;

	// Token: 0x040028D6 RID: 10454
	public GameObjectRef effectOnTimer;

	// Token: 0x040028D7 RID: 10455
	public TriggerHurtEx.HurtType hurtTypeOnTimer;

	// Token: 0x040028D8 RID: 10456
	[Header("On Move (damage per meter)")]
	public List<DamageTypeEntry> damageOnMove;

	// Token: 0x040028D9 RID: 10457
	public GameObjectRef effectOnMove;

	// Token: 0x040028DA RID: 10458
	public TriggerHurtEx.HurtType hurtTypeOnMove;

	// Token: 0x040028DB RID: 10459
	[Header("On Leave")]
	public List<DamageTypeEntry> damageOnLeave;

	// Token: 0x040028DC RID: 10460
	public GameObjectRef effectOnLeave;

	// Token: 0x040028DD RID: 10461
	public TriggerHurtEx.HurtType hurtTypeOnLeave;

	// Token: 0x040028DE RID: 10462
	public bool damageEnabled = true;

	// Token: 0x040028DF RID: 10463
	internal Dictionary<BaseEntity, TriggerHurtEx.EntityTriggerInfo> entityInfo;

	// Token: 0x040028E0 RID: 10464
	internal List<BaseEntity> entityAddList;

	// Token: 0x040028E1 RID: 10465
	internal List<BaseEntity> entityLeaveList;

	// Token: 0x02000DF8 RID: 3576
	public enum HurtType
	{
		// Token: 0x0400488F RID: 18575
		Simple,
		// Token: 0x04004890 RID: 18576
		IncludeBleedingAndScreenShake
	}

	// Token: 0x02000DF9 RID: 3577
	public class EntityTriggerInfo
	{
		// Token: 0x04004891 RID: 18577
		public Vector3 lastPosition;
	}
}
