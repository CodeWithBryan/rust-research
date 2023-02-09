using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200003A RID: 58
public class BaseLiquidVessel : AttackEntity
{
	// Token: 0x060003BA RID: 954 RVA: 0x0002F2C8 File Offset: 0x0002D4C8
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("BaseLiquidVessel.OnRpcMessage", 0))
		{
			if (rpc == 4013436649U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DoDrink ");
				}
				using (TimeWarning.New("DoDrink", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(4013436649U, "DoDrink", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg2 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.DoDrink(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in DoDrink");
					}
				}
				return true;
			}
			if (rpc == 2781345828U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SendFilling ");
				}
				using (TimeWarning.New("SendFilling", 0))
				{
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg3 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SendFilling(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in SendFilling");
					}
				}
				return true;
			}
			if (rpc == 3038767821U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - ThrowContents ");
				}
				using (TimeWarning.New("ThrowContents", 0))
				{
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg4 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ThrowContents(msg4);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in ThrowContents");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060003BB RID: 955 RVA: 0x0002F680 File Offset: 0x0002D880
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRepeating(new Action(this.FillCheck), 1f, 1f);
	}

	// Token: 0x060003BC RID: 956 RVA: 0x0002F6A4 File Offset: 0x0002D8A4
	public override void OnHeldChanged()
	{
		base.OnHeldChanged();
		if (base.IsDisabled())
		{
			this.StopFilling();
		}
		if (!this.hasLid)
		{
			this.DoThrow(base.transform.position, Vector3.zero);
			Item item = this.GetItem();
			if (item == null || item.contents == null)
			{
				return;
			}
			item.contents.SetLocked(base.IsDisabled());
			base.SendNetworkUpdateImmediate(false);
		}
	}

	// Token: 0x060003BD RID: 957 RVA: 0x0002F70E File Offset: 0x0002D90E
	public void SetFilling(bool isFilling)
	{
		base.SetFlag(BaseEntity.Flags.Open, isFilling, false, true);
		if (isFilling)
		{
			this.StartFilling();
		}
		else
		{
			this.StopFilling();
		}
		this.OnSetFilling(isFilling);
	}

	// Token: 0x060003BE RID: 958 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void OnSetFilling(bool flag)
	{
	}

	// Token: 0x060003BF RID: 959 RVA: 0x0002F734 File Offset: 0x0002D934
	public void StartFilling()
	{
		float num = UnityEngine.Time.realtimeSinceStartup - this.lastFillTime;
		this.StopFilling();
		base.InvokeRepeating(new Action(this.FillCheck), 0f, 0.3f);
		if (num > 1f)
		{
			LiquidContainer facingLiquidContainer = this.GetFacingLiquidContainer();
			if (facingLiquidContainer != null && facingLiquidContainer.GetLiquidItem() != null)
			{
				if (this.fillFromContainer.isValid)
				{
					Effect.server.Run(this.fillFromContainer.resourcePath, facingLiquidContainer.transform.position, Vector3.up, null, false);
				}
				base.ClientRPC(null, "CLIENT_StartFillingSoundsContainer");
			}
			else if (this.CanFillFromWorld())
			{
				if (this.fillFromWorld.isValid)
				{
					Effect.server.Run(this.fillFromWorld.resourcePath, base.GetOwnerPlayer(), 0U, Vector3.zero, Vector3.up, null, false);
				}
				base.ClientRPC(null, "CLIENT_StartFillingSoundsWorld");
			}
		}
		this.lastFillTime = UnityEngine.Time.realtimeSinceStartup;
	}

	// Token: 0x060003C0 RID: 960 RVA: 0x0002F81D File Offset: 0x0002DA1D
	public void StopFilling()
	{
		base.ClientRPC(null, "CLIENT_StopFillingSounds");
		base.CancelInvoke(new Action(this.FillCheck));
	}

	// Token: 0x060003C1 RID: 961 RVA: 0x0002F840 File Offset: 0x0002DA40
	public void FillCheck()
	{
		if (base.isClient)
		{
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		float f = (UnityEngine.Time.realtimeSinceStartup - this.lastFillTime) * this.fillMlPerSec;
		Vector3 pos = ownerPlayer.transform.position - new Vector3(0f, 1f, 0f);
		LiquidContainer facingLiquidContainer = this.GetFacingLiquidContainer();
		if (facingLiquidContainer == null && this.CanFillFromWorld())
		{
			this.AddLiquid(WaterResource.GetAtPoint(pos), Mathf.FloorToInt(f));
		}
		else if (facingLiquidContainer != null && facingLiquidContainer.HasLiquidItem())
		{
			int num = Mathf.CeilToInt((1f - this.HeldFraction()) * (float)this.MaxHoldable());
			if (num > 0)
			{
				Item liquidItem = facingLiquidContainer.GetLiquidItem();
				int num2 = Mathf.Min(Mathf.CeilToInt(f), Mathf.Min(liquidItem.amount, num));
				this.AddLiquid(liquidItem.info, num2);
				liquidItem.UseItem(num2);
				facingLiquidContainer.OpenTap(2f);
			}
		}
		this.lastFillTime = UnityEngine.Time.realtimeSinceStartup;
	}

	// Token: 0x060003C2 RID: 962 RVA: 0x0002F950 File Offset: 0x0002DB50
	public void LoseWater(int amount)
	{
		Item slot = this.GetItem().contents.GetSlot(0);
		if (slot != null)
		{
			slot.UseItem(amount);
			slot.MarkDirty();
			base.SendNetworkUpdateImmediate(false);
		}
	}

	// Token: 0x060003C3 RID: 963 RVA: 0x0002F988 File Offset: 0x0002DB88
	public void AddLiquid(ItemDefinition liquidType, int amount)
	{
		if (amount <= 0)
		{
			return;
		}
		Item item = this.GetItem();
		Item item2 = item.contents.GetSlot(0);
		ItemModContainer component = item.info.GetComponent<ItemModContainer>();
		if (item2 == null)
		{
			Item item3 = ItemManager.Create(liquidType, amount, 0UL);
			if (item3 != null)
			{
				item3.MoveToContainer(item.contents, -1, true, false, null, true);
				return;
			}
		}
		else
		{
			int num = Mathf.Clamp(item2.amount + amount, 0, component.maxStackSize);
			ItemDefinition itemDefinition = WaterResource.Merge(item2.info, liquidType);
			if (itemDefinition != item2.info)
			{
				item2.Remove(0f);
				item2 = ItemManager.Create(itemDefinition, num, 0UL);
				item2.MoveToContainer(item.contents, -1, true, false, null, true);
			}
			else
			{
				item2.amount = num;
			}
			item2.MarkDirty();
			base.SendNetworkUpdateImmediate(false);
		}
	}

	// Token: 0x060003C4 RID: 964 RVA: 0x0002FA54 File Offset: 0x0002DC54
	public int AmountHeld()
	{
		Item item = this.GetItem();
		if (item == null || item.contents == null)
		{
			return 0;
		}
		Item slot = item.contents.GetSlot(0);
		if (slot == null)
		{
			return 0;
		}
		return slot.amount;
	}

	// Token: 0x060003C5 RID: 965 RVA: 0x0002FA90 File Offset: 0x0002DC90
	public float HeldFraction()
	{
		Item item = this.GetItem();
		if (item == null || item.contents == null)
		{
			return 0f;
		}
		return (float)this.AmountHeld() / (float)this.MaxHoldable();
	}

	// Token: 0x060003C6 RID: 966 RVA: 0x0002FAC4 File Offset: 0x0002DCC4
	public int MaxHoldable()
	{
		Item item = this.GetItem();
		if (item == null || item.contents == null)
		{
			return 1;
		}
		return this.GetItem().info.GetComponent<ItemModContainer>().maxStackSize;
	}

	// Token: 0x060003C7 RID: 967 RVA: 0x0002FAFC File Offset: 0x0002DCFC
	public bool CanDrink()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return false;
		}
		if (!ownerPlayer.metabolism.CanConsume())
		{
			return false;
		}
		if (!this.canDrinkFrom)
		{
			return false;
		}
		Item item = this.GetItem();
		return item != null && item.contents != null && item.contents.itemList != null && item.contents.itemList.Count != 0;
	}

	// Token: 0x060003C8 RID: 968 RVA: 0x0002FB6D File Offset: 0x0002DD6D
	private bool IsWeaponBusy()
	{
		return UnityEngine.Time.realtimeSinceStartup < this.nextFreeTime;
	}

	// Token: 0x060003C9 RID: 969 RVA: 0x0002FB7C File Offset: 0x0002DD7C
	private void SetBusyFor(float dur)
	{
		this.nextFreeTime = UnityEngine.Time.realtimeSinceStartup + dur;
	}

	// Token: 0x060003CA RID: 970 RVA: 0x0002FB8B File Offset: 0x0002DD8B
	private void ClearBusy()
	{
		this.nextFreeTime = UnityEngine.Time.realtimeSinceStartup - 1f;
	}

	// Token: 0x060003CB RID: 971 RVA: 0x0002FBA0 File Offset: 0x0002DDA0
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	private void DoDrink(BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		Item item = this.GetItem();
		if (item == null)
		{
			return;
		}
		if (item.contents == null)
		{
			return;
		}
		if (!msg.player.metabolism.CanConsume())
		{
			return;
		}
		foreach (Item item2 in item.contents.itemList)
		{
			ItemModConsume component = item2.info.GetComponent<ItemModConsume>();
			if (!(component == null) && component.CanDoAction(item2, msg.player))
			{
				component.DoAction(item2, msg.player);
				break;
			}
		}
	}

	// Token: 0x060003CC RID: 972 RVA: 0x0002FC5C File Offset: 0x0002DE5C
	[BaseEntity.RPC_Server]
	private void ThrowContents(BaseEntity.RPCMessage msg)
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return;
		}
		this.DoThrow(ownerPlayer.eyes.position + ownerPlayer.eyes.BodyForward() * 1f, ownerPlayer.estimatedVelocity + ownerPlayer.eyes.BodyForward() * this.throwScale);
		Effect.server.Run(this.ThrowEffect3P.resourcePath, ownerPlayer.transform.position, ownerPlayer.eyes.BodyForward(), ownerPlayer.net.connection, false);
	}

	// Token: 0x060003CD RID: 973 RVA: 0x0002FCF8 File Offset: 0x0002DEF8
	public void DoThrow(Vector3 pos, Vector3 velocity)
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return;
		}
		Item item = this.GetItem();
		if (item == null)
		{
			return;
		}
		if (item.contents == null)
		{
			return;
		}
		Item slot = item.contents.GetSlot(0);
		if (slot != null && slot.amount > 0)
		{
			Vector3 vector = ownerPlayer.eyes.position + ownerPlayer.eyes.BodyForward() * 1f;
			WaterBall waterBall = GameManager.server.CreateEntity(this.thrownWaterObject.resourcePath, vector, Quaternion.identity, true) as WaterBall;
			if (waterBall)
			{
				waterBall.liquidType = slot.info;
				waterBall.waterAmount = slot.amount;
				waterBall.transform.position = vector;
				waterBall.SetVelocity(velocity);
				waterBall.Spawn();
			}
			slot.UseItem(slot.amount);
			slot.MarkDirty();
			base.SendNetworkUpdateImmediate(false);
		}
	}

	// Token: 0x060003CE RID: 974 RVA: 0x0002FDEC File Offset: 0x0002DFEC
	[BaseEntity.RPC_Server]
	private void SendFilling(BaseEntity.RPCMessage msg)
	{
		bool filling = msg.read.Bit();
		this.SetFilling(filling);
	}

	// Token: 0x060003CF RID: 975 RVA: 0x0002FE0C File Offset: 0x0002E00C
	public bool CanFillFromWorld()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		return ownerPlayer && !ownerPlayer.IsInWaterVolume(base.transform.position) && ownerPlayer.WaterFactor() >= 0.05f;
	}

	// Token: 0x060003D0 RID: 976 RVA: 0x0002FE4F File Offset: 0x0002E04F
	public bool CanThrow()
	{
		return this.HeldFraction() > this.minThrowFrac;
	}

	// Token: 0x060003D1 RID: 977 RVA: 0x0002FE60 File Offset: 0x0002E060
	public LiquidContainer GetFacingLiquidContainer()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return null;
		}
		RaycastHit hit;
		if (UnityEngine.Physics.Raycast(ownerPlayer.eyes.HeadRay(), out hit, 2f, 1236478737))
		{
			BaseEntity baseEntity = hit.GetEntity();
			if (baseEntity && !hit.collider.gameObject.CompareTag("Not Player Usable") && !hit.collider.gameObject.CompareTag("Usable Primary"))
			{
				baseEntity = baseEntity.ToServer<BaseEntity>();
				return baseEntity.GetComponent<LiquidContainer>();
			}
		}
		return null;
	}

	// Token: 0x040002F1 RID: 753
	[Header("Liquid Vessel")]
	public GameObjectRef thrownWaterObject;

	// Token: 0x040002F2 RID: 754
	public GameObjectRef ThrowEffect3P;

	// Token: 0x040002F3 RID: 755
	public SoundDefinition throwSound3P;

	// Token: 0x040002F4 RID: 756
	public GameObjectRef fillFromContainer;

	// Token: 0x040002F5 RID: 757
	public GameObjectRef fillFromWorld;

	// Token: 0x040002F6 RID: 758
	public SoundDefinition fillFromContainerStartSoundDef;

	// Token: 0x040002F7 RID: 759
	public SoundDefinition fillFromContainerSoundDef;

	// Token: 0x040002F8 RID: 760
	public SoundDefinition fillFromWorldStartSoundDef;

	// Token: 0x040002F9 RID: 761
	public SoundDefinition fillFromWorldSoundDef;

	// Token: 0x040002FA RID: 762
	public bool hasLid;

	// Token: 0x040002FB RID: 763
	public float throwScale = 10f;

	// Token: 0x040002FC RID: 764
	public bool canDrinkFrom;

	// Token: 0x040002FD RID: 765
	public bool updateVMWater;

	// Token: 0x040002FE RID: 766
	public float minThrowFrac;

	// Token: 0x040002FF RID: 767
	public bool useThrowAnim;

	// Token: 0x04000300 RID: 768
	public float fillMlPerSec = 500f;

	// Token: 0x04000301 RID: 769
	private float lastFillTime;

	// Token: 0x04000302 RID: 770
	private float nextFreeTime;
}
