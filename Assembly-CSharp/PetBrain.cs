using System;
using Network;
using Rust;
using UnityEngine;

// Token: 0x020000A5 RID: 165
public class PetBrain : BaseAIBrain
{
	// Token: 0x06000F0E RID: 3854 RVA: 0x0007D1C8 File Offset: 0x0007B3C8
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PetBrain.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000F0F RID: 3855 RVA: 0x0007D208 File Offset: 0x0007B408
	public override void AddStates()
	{
		base.AddStates();
	}

	// Token: 0x06000F10 RID: 3856 RVA: 0x0007D210 File Offset: 0x0007B410
	public override void InitializeAI()
	{
		base.InitializeAI();
		base.ThinkMode = AIThinkMode.Interval;
		this.thinkRate = 0.25f;
		base.PathFinder = new HumanPathFinder();
		((HumanPathFinder)base.PathFinder).Init(this.GetBaseEntity());
		PetBrain.Count++;
	}

	// Token: 0x06000F11 RID: 3857 RVA: 0x0007D262 File Offset: 0x0007B462
	public override void OnDestroy()
	{
		base.OnDestroy();
		PetBrain.Count--;
	}

	// Token: 0x06000F12 RID: 3858 RVA: 0x0007D278 File Offset: 0x0007B478
	public override void Think(float delta)
	{
		base.Think(delta);
		if (PetBrain.DrownInDeepWater)
		{
			BaseCombatEntity baseCombatEntity = this.GetBaseEntity() as BaseCombatEntity;
			if (baseCombatEntity != null && baseCombatEntity.WaterFactor() > 0.85f && !baseCombatEntity.IsDestroyed)
			{
				baseCombatEntity.Hurt(delta * (baseCombatEntity.MaxHealth() / PetBrain.DrownTimer), DamageType.Drowned, null, true);
			}
		}
		this.EvaluateLoadDefaultDesignTriggers();
	}

	// Token: 0x06000F13 RID: 3859 RVA: 0x0007D2DC File Offset: 0x0007B4DC
	private bool EvaluateLoadDefaultDesignTriggers()
	{
		if (this.loadedDesignIndex == 0)
		{
			return true;
		}
		bool flag = false;
		if (PetBrain.IdleWhenOwnerOfflineOrDead)
		{
			flag = ((PetBrain.IdleWhenOwnerOfflineOrDead && base.OwningPlayer == null) || base.OwningPlayer.IsSleeping() || base.OwningPlayer.IsDead());
		}
		if (PetBrain.IdleWhenOwnerMounted && !flag)
		{
			flag = (base.OwningPlayer != null && base.OwningPlayer.isMounted);
		}
		if (base.OwningPlayer != null && Vector3.Distance(base.transform.position, base.OwningPlayer.transform.position) > PetBrain.ControlDistance)
		{
			flag = true;
		}
		if (flag)
		{
			base.LoadDefaultAIDesign();
			return true;
		}
		return false;
	}

	// Token: 0x06000F14 RID: 3860 RVA: 0x0007D398 File Offset: 0x0007B598
	public override void OnAIDesignLoadedAtIndex(int index)
	{
		base.OnAIDesignLoadedAtIndex(index);
		BaseEntity baseEntity = this.GetBaseEntity();
		if (baseEntity != null)
		{
			BasePlayer basePlayer = BasePlayer.FindByID(baseEntity.OwnerID);
			if (basePlayer != null)
			{
				basePlayer.SendClientPetStateIndex();
			}
			baseEntity.ClientRPC(null, "OnCommandGiven");
		}
	}

	// Token: 0x040009CE RID: 2510
	[Header("Audio")]
	public SoundDefinition CommandGivenVocalSFX;

	// Token: 0x040009CF RID: 2511
	[ServerVar]
	public static bool DrownInDeepWater = true;

	// Token: 0x040009D0 RID: 2512
	[ServerVar]
	public static bool IdleWhenOwnerOfflineOrDead = true;

	// Token: 0x040009D1 RID: 2513
	[ServerVar]
	public static bool IdleWhenOwnerMounted = true;

	// Token: 0x040009D2 RID: 2514
	[ServerVar]
	public static float DrownTimer = 15f;

	// Token: 0x040009D3 RID: 2515
	[ReplicatedVar]
	public static float ControlDistance = 100f;

	// Token: 0x040009D4 RID: 2516
	public static int Count;
}
