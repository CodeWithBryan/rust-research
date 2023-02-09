using System;
using UnityEngine;

// Token: 0x020001E0 RID: 480
public class ScientistNPC : HumanNPC, IAIMounted
{
	// Token: 0x0600191D RID: 6429 RVA: 0x000B681C File Offset: 0x000B4A1C
	public void SetChatterType(ScientistNPC.RadioChatterType newType)
	{
		if (newType == this.radioChatterType)
		{
			return;
		}
		if (newType == ScientistNPC.RadioChatterType.Idle)
		{
			this.QueueRadioChatter();
			return;
		}
		base.CancelInvoke(new Action(this.PlayRadioChatter));
	}

	// Token: 0x0600191E RID: 6430 RVA: 0x000B6845 File Offset: 0x000B4A45
	public override void ServerInit()
	{
		base.ServerInit();
		this.SetChatterType(ScientistNPC.RadioChatterType.Idle);
		base.InvokeRandomized(new Action(this.IdleCheck), 0f, 20f, 1f);
	}

	// Token: 0x0600191F RID: 6431 RVA: 0x000B6875 File Offset: 0x000B4A75
	public void IdleCheck()
	{
		if (Time.time > this.lastAlertedTime + 20f)
		{
			this.SetChatterType(ScientistNPC.RadioChatterType.Idle);
		}
	}

	// Token: 0x06001920 RID: 6432 RVA: 0x000B6891 File Offset: 0x000B4A91
	public void QueueRadioChatter()
	{
		if (!this.IsAlive() || base.IsDestroyed)
		{
			return;
		}
		base.Invoke(new Action(this.PlayRadioChatter), UnityEngine.Random.Range(this.IdleChatterRepeatRange.x, this.IdleChatterRepeatRange.y));
	}

	// Token: 0x06001921 RID: 6433 RVA: 0x000B68D1 File Offset: 0x000B4AD1
	public override bool ShotTest(float targetDist)
	{
		bool result = base.ShotTest(targetDist);
		if (Time.time - this.lastGunShotTime < 5f)
		{
			this.Alert();
		}
		return result;
	}

	// Token: 0x06001922 RID: 6434 RVA: 0x000B68F3 File Offset: 0x000B4AF3
	public void Alert()
	{
		this.lastAlertedTime = Time.time;
		this.SetChatterType(ScientistNPC.RadioChatterType.Alert);
	}

	// Token: 0x06001923 RID: 6435 RVA: 0x000B6907 File Offset: 0x000B4B07
	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		this.Alert();
	}

	// Token: 0x06001924 RID: 6436 RVA: 0x000B6918 File Offset: 0x000B4B18
	public override void OnKilled(HitInfo info)
	{
		base.OnKilled(info);
		this.SetChatterType(ScientistNPC.RadioChatterType.NONE);
		if (this.DeathEffects.Length != 0)
		{
			Effect.server.Run(this.DeathEffects[UnityEngine.Random.Range(0, this.DeathEffects.Length)].resourcePath, this.ServerPosition, Vector3.up, null, false);
		}
		if (info != null && info.InitiatorPlayer != null && !info.InitiatorPlayer.IsNpc)
		{
			info.InitiatorPlayer.stats.Add(this.deathStatName, 1, (Stats)5);
		}
	}

	// Token: 0x06001925 RID: 6437 RVA: 0x000B69A0 File Offset: 0x000B4BA0
	public void PlayRadioChatter()
	{
		if (this.RadioChatterEffects.Length == 0)
		{
			return;
		}
		if (base.IsDestroyed || base.transform == null)
		{
			base.CancelInvoke(new Action(this.PlayRadioChatter));
			return;
		}
		Effect.server.Run(this.RadioChatterEffects[UnityEngine.Random.Range(0, this.RadioChatterEffects.Length)].resourcePath, this, StringPool.Get("head"), Vector3.zero, Vector3.zero, null, false);
		this.QueueRadioChatter();
	}

	// Token: 0x06001926 RID: 6438 RVA: 0x000B6A1C File Offset: 0x000B4C1C
	public override void EquipWeapon(bool skipDeployDelay = false)
	{
		base.EquipWeapon(skipDeployDelay);
		HeldEntity heldEntity = base.GetHeldEntity();
		if (heldEntity != null)
		{
			Item item = heldEntity.GetItem();
			if (item != null && item.contents != null)
			{
				if (UnityEngine.Random.Range(0, 3) == 0)
				{
					Item item2 = ItemManager.CreateByName("weapon.mod.flashlight", 1, 0UL);
					if (!item2.MoveToContainer(item.contents, -1, true, false, null, true))
					{
						item2.Remove(0f);
						return;
					}
					this.lightsOn = false;
					base.InvokeRandomized(new Action(base.LightCheck), 0f, 30f, 5f);
					base.LightCheck();
					return;
				}
				else
				{
					Item item3 = ItemManager.CreateByName("weapon.mod.lasersight", 1, 0UL);
					if (!item3.MoveToContainer(item.contents, -1, true, false, null, true))
					{
						item3.Remove(0f);
					}
					base.LightToggle(true);
					this.lightsOn = true;
				}
			}
		}
	}

	// Token: 0x06001927 RID: 6439 RVA: 0x000B6AFA File Offset: 0x000B4CFA
	public bool IsMounted()
	{
		return base.isMounted;
	}

	// Token: 0x06001928 RID: 6440 RVA: 0x000B6B02 File Offset: 0x000B4D02
	protected override string OverrideCorpseName()
	{
		return "Scientist";
	}

	// Token: 0x040011E2 RID: 4578
	public GameObjectRef[] RadioChatterEffects;

	// Token: 0x040011E3 RID: 4579
	public GameObjectRef[] DeathEffects;

	// Token: 0x040011E4 RID: 4580
	public string deathStatName = "kill_scientist";

	// Token: 0x040011E5 RID: 4581
	public Vector2 IdleChatterRepeatRange = new Vector2(10f, 15f);

	// Token: 0x040011E6 RID: 4582
	public ScientistNPC.RadioChatterType radioChatterType;

	// Token: 0x040011E7 RID: 4583
	protected float lastAlertedTime = -100f;

	// Token: 0x02000C09 RID: 3081
	public enum RadioChatterType
	{
		// Token: 0x04004067 RID: 16487
		NONE,
		// Token: 0x04004068 RID: 16488
		Idle,
		// Token: 0x04004069 RID: 16489
		Alert
	}
}
