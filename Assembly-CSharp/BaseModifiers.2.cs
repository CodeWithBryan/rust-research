using System;
using System.Collections.Generic;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x0200040B RID: 1035
public abstract class BaseModifiers<T> : EntityComponent<T> where T : BaseCombatEntity
{
	// Token: 0x170002A1 RID: 673
	// (get) Token: 0x060022B5 RID: 8885 RVA: 0x000DDF33 File Offset: 0x000DC133
	public int ActiveModifierCoount
	{
		get
		{
			return this.All.Count;
		}
	}

	// Token: 0x060022B6 RID: 8886 RVA: 0x000DDF40 File Offset: 0x000DC140
	public void Add(List<ModifierDefintion> modDefs)
	{
		foreach (ModifierDefintion def in modDefs)
		{
			this.Add(def);
		}
	}

	// Token: 0x060022B7 RID: 8887 RVA: 0x000DDF90 File Offset: 0x000DC190
	protected void Add(ModifierDefintion def)
	{
		Modifier modifier = new Modifier();
		modifier.Init(def.type, def.source, def.value, def.duration, def.duration);
		this.Add(modifier);
	}

	// Token: 0x060022B8 RID: 8888 RVA: 0x000DDFD0 File Offset: 0x000DC1D0
	protected void Add(Modifier modifier)
	{
		if (!this.CanAdd(modifier))
		{
			return;
		}
		int maxModifiersForSourceType = this.GetMaxModifiersForSourceType(modifier.Source);
		if (this.GetTypeSourceCount(modifier.Type, modifier.Source) >= maxModifiersForSourceType)
		{
			Modifier shortestLifeModifier = this.GetShortestLifeModifier(modifier.Type, modifier.Source);
			if (shortestLifeModifier == null)
			{
				return;
			}
			this.Remove(shortestLifeModifier);
		}
		this.All.Add(modifier);
		if (!this.totalValues.ContainsKey(modifier.Type))
		{
			this.totalValues.Add(modifier.Type, modifier.Value);
		}
		else
		{
			Dictionary<Modifier.ModifierType, float> dictionary = this.totalValues;
			Modifier.ModifierType type = modifier.Type;
			dictionary[type] += modifier.Value;
		}
		this.SetDirty(true);
	}

	// Token: 0x060022B9 RID: 8889 RVA: 0x000DE089 File Offset: 0x000DC289
	private bool CanAdd(Modifier modifier)
	{
		return !this.All.Contains(modifier);
	}

	// Token: 0x060022BA RID: 8890 RVA: 0x000DE09C File Offset: 0x000DC29C
	private int GetMaxModifiersForSourceType(Modifier.ModifierSource source)
	{
		if (source == Modifier.ModifierSource.Tea)
		{
			return 1;
		}
		return int.MaxValue;
	}

	// Token: 0x060022BB RID: 8891 RVA: 0x000DE0A8 File Offset: 0x000DC2A8
	private int GetTypeSourceCount(Modifier.ModifierType type, Modifier.ModifierSource source)
	{
		int num = 0;
		foreach (Modifier modifier in this.All)
		{
			if (modifier.Type == type && modifier.Source == source)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x060022BC RID: 8892 RVA: 0x000DE110 File Offset: 0x000DC310
	private Modifier GetShortestLifeModifier(Modifier.ModifierType type, Modifier.ModifierSource source)
	{
		Modifier modifier = null;
		foreach (Modifier modifier2 in this.All)
		{
			if (modifier2.Type == type && modifier2.Source == source)
			{
				if (modifier == null)
				{
					modifier = modifier2;
				}
				else if (modifier2.TimeRemaining < modifier.TimeRemaining)
				{
					modifier = modifier2;
				}
			}
		}
		return modifier;
	}

	// Token: 0x060022BD RID: 8893 RVA: 0x000DE188 File Offset: 0x000DC388
	private void Remove(Modifier modifier)
	{
		if (!this.All.Contains(modifier))
		{
			return;
		}
		this.All.Remove(modifier);
		Dictionary<Modifier.ModifierType, float> dictionary = this.totalValues;
		Modifier.ModifierType type = modifier.Type;
		dictionary[type] -= modifier.Value;
		this.SetDirty(true);
	}

	// Token: 0x060022BE RID: 8894 RVA: 0x000DE1DB File Offset: 0x000DC3DB
	public void RemoveAll()
	{
		this.All.Clear();
		this.totalValues.Clear();
		this.SetDirty(true);
	}

	// Token: 0x060022BF RID: 8895 RVA: 0x000DE1FC File Offset: 0x000DC3FC
	public float GetValue(Modifier.ModifierType type, float defaultValue = 0f)
	{
		float result;
		if (this.totalValues.TryGetValue(type, out result))
		{
			return result;
		}
		return defaultValue;
	}

	// Token: 0x060022C0 RID: 8896 RVA: 0x000DE21C File Offset: 0x000DC41C
	public float GetVariableValue(Modifier.ModifierType type, float defaultValue)
	{
		float result;
		if (this.modifierVariables.TryGetValue(type, out result))
		{
			return result;
		}
		return defaultValue;
	}

	// Token: 0x060022C1 RID: 8897 RVA: 0x000DE23C File Offset: 0x000DC43C
	public void SetVariableValue(Modifier.ModifierType type, float value)
	{
		float num;
		if (this.modifierVariables.TryGetValue(type, out num))
		{
			this.modifierVariables[type] = value;
			return;
		}
		this.modifierVariables.Add(type, value);
	}

	// Token: 0x060022C2 RID: 8898 RVA: 0x000DE274 File Offset: 0x000DC474
	public void RemoveVariable(Modifier.ModifierType type)
	{
		this.modifierVariables.Remove(type);
	}

	// Token: 0x060022C3 RID: 8899 RVA: 0x000DE283 File Offset: 0x000DC483
	protected virtual void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		this.owner = default(T);
	}

	// Token: 0x060022C4 RID: 8900 RVA: 0x000DE299 File Offset: 0x000DC499
	protected void SetDirty(bool flag)
	{
		this.dirty = flag;
	}

	// Token: 0x060022C5 RID: 8901 RVA: 0x000DE2A2 File Offset: 0x000DC4A2
	public virtual void ServerInit(T owner)
	{
		this.owner = owner;
		this.ResetTicking();
		this.RemoveAll();
	}

	// Token: 0x060022C6 RID: 8902 RVA: 0x000DE2B7 File Offset: 0x000DC4B7
	public void ResetTicking()
	{
		this.lastTickTime = UnityEngine.Time.realtimeSinceStartup;
		this.timeSinceLastTick = 0f;
	}

	// Token: 0x060022C7 RID: 8903 RVA: 0x000DE2D0 File Offset: 0x000DC4D0
	public virtual void ServerUpdate(BaseCombatEntity ownerEntity)
	{
		float num = UnityEngine.Time.realtimeSinceStartup - this.lastTickTime;
		this.lastTickTime = UnityEngine.Time.realtimeSinceStartup;
		this.timeSinceLastTick += num;
		if (this.timeSinceLastTick <= ConVar.Server.modifierTickRate)
		{
			return;
		}
		if (this.owner != null && !this.owner.IsDead())
		{
			this.TickModifiers(ownerEntity, this.timeSinceLastTick);
		}
		this.timeSinceLastTick = 0f;
	}

	// Token: 0x060022C8 RID: 8904 RVA: 0x000DE350 File Offset: 0x000DC550
	protected virtual void TickModifiers(BaseCombatEntity ownerEntity, float delta)
	{
		for (int i = this.All.Count - 1; i >= 0; i--)
		{
			Modifier modifier = this.All[i];
			modifier.Tick(ownerEntity, delta);
			if (modifier.Expired)
			{
				this.Remove(modifier);
			}
		}
	}

	// Token: 0x04001B1E RID: 6942
	public List<Modifier> All = new List<Modifier>();

	// Token: 0x04001B1F RID: 6943
	protected Dictionary<Modifier.ModifierType, float> totalValues = new Dictionary<Modifier.ModifierType, float>();

	// Token: 0x04001B20 RID: 6944
	protected Dictionary<Modifier.ModifierType, float> modifierVariables = new Dictionary<Modifier.ModifierType, float>();

	// Token: 0x04001B21 RID: 6945
	protected T owner;

	// Token: 0x04001B22 RID: 6946
	protected bool dirty = true;

	// Token: 0x04001B23 RID: 6947
	protected float timeSinceLastTick;

	// Token: 0x04001B24 RID: 6948
	protected float lastTickTime;
}
