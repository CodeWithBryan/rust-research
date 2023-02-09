using System;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000C7 RID: 199
public class SlotMachine : BaseMountable
{
	// Token: 0x06001192 RID: 4498 RVA: 0x0008E014 File Offset: 0x0008C214
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SlotMachine.OnRpcMessage", 0))
		{
			if (rpc == 1251063754U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Deposit ");
				}
				using (TimeWarning.New("RPC_Deposit", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1251063754U, "RPC_Deposit", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Deposit(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_Deposit");
					}
				}
				return true;
			}
			if (rpc == 1455840454U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_Spin ");
				}
				using (TimeWarning.New("RPC_Spin", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1455840454U, "RPC_Spin", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpc3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_Spin(rpc3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_Spin");
					}
				}
				return true;
			}
			if (rpc == 3942337446U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_RequestMultiplierChange ");
				}
				using (TimeWarning.New("Server_RequestMultiplierChange", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(3942337446U, "Server_RequestMultiplierChange", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3942337446U, "Server_RequestMultiplierChange", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg2 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_RequestMultiplierChange(msg2);
						}
					}
					catch (Exception exception3)
					{
						Debug.LogException(exception3);
						player.Kick("RPC Error in Server_RequestMultiplierChange");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x17000173 RID: 371
	// (get) Token: 0x06001193 RID: 4499 RVA: 0x00004C84 File Offset: 0x00002E84
	private bool IsSpinning
	{
		get
		{
			return base.HasFlag(global::BaseEntity.Flags.Reserved2);
		}
	}

	// Token: 0x17000174 RID: 372
	// (get) Token: 0x06001194 RID: 4500 RVA: 0x0008E48C File Offset: 0x0008C68C
	// (set) Token: 0x06001195 RID: 4501 RVA: 0x0008E494 File Offset: 0x0008C694
	public int CurrentMultiplier { get; private set; } = 1;

	// Token: 0x06001196 RID: 4502 RVA: 0x0008E4A0 File Offset: 0x0008C6A0
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.slotMachine = Facepunch.Pool.Get<ProtoBuf.SlotMachine>();
		info.msg.slotMachine.oldResult1 = this.SpinResultPrevious1;
		info.msg.slotMachine.oldResult2 = this.SpinResultPrevious2;
		info.msg.slotMachine.oldResult3 = this.SpinResultPrevious3;
		info.msg.slotMachine.newResult1 = this.SpinResult1;
		info.msg.slotMachine.newResult2 = this.SpinResult2;
		info.msg.slotMachine.newResult3 = this.SpinResult3;
		info.msg.slotMachine.isSpinning = this.IsSpinning;
		info.msg.slotMachine.spinTime = this.SpinTime;
		info.msg.slotMachine.storageID = this.StorageInstance.uid;
		info.msg.slotMachine.multiplier = this.CurrentMultiplier;
	}

	// Token: 0x06001197 RID: 4503 RVA: 0x0008E5A8 File Offset: 0x0008C7A8
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.slotMachine != null)
		{
			this.SpinResultPrevious1 = info.msg.slotMachine.oldResult1;
			this.SpinResultPrevious2 = info.msg.slotMachine.oldResult2;
			this.SpinResultPrevious3 = info.msg.slotMachine.oldResult3;
			this.SpinResult1 = info.msg.slotMachine.newResult1;
			this.SpinResult2 = info.msg.slotMachine.newResult2;
			this.SpinResult3 = info.msg.slotMachine.newResult3;
			this.CurrentMultiplier = info.msg.slotMachine.multiplier;
			if (base.isServer)
			{
				this.SpinTime = info.msg.slotMachine.spinTime;
			}
			this.StorageInstance.uid = info.msg.slotMachine.storageID;
			if (info.fromDisk && base.isServer)
			{
				base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
			}
		}
	}

	// Token: 0x06001198 RID: 4504 RVA: 0x000062DD File Offset: 0x000044DD
	public override float GetComfort()
	{
		return 1f;
	}

	// Token: 0x06001199 RID: 4505 RVA: 0x0008E6C0 File Offset: 0x0008C8C0
	public override void Spawn()
	{
		base.Spawn();
		if (!Rust.Application.isLoadingSave)
		{
			global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.StoragePrefab.resourcePath, default(Vector3), default(Quaternion), true);
			baseEntity.Spawn();
			baseEntity.SetParent(this, false, false);
			this.StorageInstance.Set(baseEntity);
		}
	}

	// Token: 0x0600119A RID: 4506 RVA: 0x0008E720 File Offset: 0x0008C920
	internal override void DoServerDestroy()
	{
		SlotMachineStorage slotMachineStorage = this.StorageInstance.Get(base.isServer) as SlotMachineStorage;
		if (slotMachineStorage.IsValid())
		{
			slotMachineStorage.DropItems(null);
		}
		base.DoServerDestroy();
	}

	// Token: 0x0600119B RID: 4507 RVA: 0x0008E75C File Offset: 0x0008C95C
	private int GetBettingAmount()
	{
		SlotMachineStorage component = this.StorageInstance.Get(base.isServer).GetComponent<SlotMachineStorage>();
		if (component == null)
		{
			return 0;
		}
		global::Item slot = component.inventory.GetSlot(0);
		if (slot != null)
		{
			return slot.amount;
		}
		return 0;
	}

	// Token: 0x0600119C RID: 4508 RVA: 0x0008E7A4 File Offset: 0x0008C9A4
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_Spin(global::BaseEntity.RPCMessage rpc)
	{
		if (this.IsSpinning)
		{
			return;
		}
		if (rpc.player != base.GetMounted())
		{
			return;
		}
		SlotMachineStorage component = this.StorageInstance.Get(base.isServer).GetComponent<SlotMachineStorage>();
		int num = (int)this.PayoutSettings.SpinCost.amount * this.CurrentMultiplier;
		if (this.GetBettingAmount() < num)
		{
			return;
		}
		if (rpc.player == null)
		{
			return;
		}
		global::BasePlayer player = rpc.player;
		this.CurrentSpinPlayer = player;
		global::Item slot = component.inventory.GetSlot(0);
		int amount = 0;
		if (slot != null)
		{
			if (slot.amount > num)
			{
				slot.MarkDirty();
				slot.amount -= num;
				amount = slot.amount;
			}
			else
			{
				slot.amount -= num;
				slot.RemoveFromContainer();
			}
		}
		component.UpdateAmount(amount);
		base.SetFlag(global::BaseEntity.Flags.Reserved2, true, false, true);
		this.SpinResultPrevious1 = this.SpinResult1;
		this.SpinResultPrevious2 = this.SpinResult2;
		this.SpinResultPrevious3 = this.SpinResult3;
		this.CalculateSpinResults();
		this.SpinTime = UnityEngine.Time.time;
		base.ClientRPC<sbyte, sbyte, sbyte>(null, "RPC_OnSpin", (sbyte)this.SpinResult1, (sbyte)this.SpinResult2, (sbyte)this.SpinResult3);
		base.Invoke(new Action(this.CheckPayout), this.SpinDuration);
	}

	// Token: 0x0600119D RID: 4509 RVA: 0x0008E8F8 File Offset: 0x0008CAF8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_Deposit(global::BaseEntity.RPCMessage rpc)
	{
		global::BasePlayer player = rpc.player;
		if (player == null)
		{
			return;
		}
		if (this.StorageInstance.IsValid(base.isServer))
		{
			this.StorageInstance.Get(base.isServer).GetComponent<StorageContainer>().PlayerOpenLoot(player, "", false);
		}
	}

	// Token: 0x0600119E RID: 4510 RVA: 0x0008E94C File Offset: 0x0008CB4C
	private void CheckPayout()
	{
		bool flag = false;
		if (this.PayoutSettings != null)
		{
			SlotMachinePayoutSettings.PayoutInfo payoutInfo;
			int num;
			if (this.CalculatePayout(out payoutInfo, out num))
			{
				int num2 = ((int)payoutInfo.Item.amount + num) * this.CurrentMultiplier;
				global::BaseEntity baseEntity = this.StorageInstance.Get(true);
				SlotMachineStorage slotMachineStorage;
				if (baseEntity != null && (slotMachineStorage = (baseEntity as SlotMachineStorage)) != null)
				{
					global::Item slot = slotMachineStorage.inventory.GetSlot(1);
					if (slot != null)
					{
						slot.amount += num2;
						slot.MarkDirty();
					}
					else
					{
						ItemManager.Create(payoutInfo.Item.itemDef, num2, 0UL).MoveToContainer(slotMachineStorage.inventory, 1, true, false, null, true);
					}
				}
				if (this.CurrentSpinPlayer.IsValid() && this.CurrentSpinPlayer == this._mounted)
				{
					this.CurrentSpinPlayer.ChatMessage(string.Format("You received {0}x {1} for slots payout!", num2, payoutInfo.Item.itemDef.displayName.english));
				}
				Analytics.Server.SlotMachineTransaction((int)this.PayoutSettings.SpinCost.amount * this.CurrentMultiplier, num2);
				if (payoutInfo.OverrideWinEffect != null && payoutInfo.OverrideWinEffect.isValid)
				{
					Effect.server.Run(payoutInfo.OverrideWinEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
				}
				else if (this.PayoutSettings.DefaultWinEffect != null && this.PayoutSettings.DefaultWinEffect.isValid)
				{
					Effect.server.Run(this.PayoutSettings.DefaultWinEffect.resourcePath, this, 0U, Vector3.zero, Vector3.zero, null, false);
				}
				if (payoutInfo.OverrideWinEffect != null && payoutInfo.OverrideWinEffect.isValid)
				{
					flag = true;
				}
			}
			else
			{
				Analytics.Server.SlotMachineTransaction((int)this.PayoutSettings.SpinCost.amount * this.CurrentMultiplier, 0);
			}
		}
		else
		{
			Debug.LogError(string.Format("Failed to process spin results: PayoutSettings != null {0} CurrentSpinPlayer.IsValid {1} CurrentSpinPlayer == mounted {2}", this.PayoutSettings != null, this.CurrentSpinPlayer.IsValid(), this.CurrentSpinPlayer == this._mounted));
		}
		if (!flag)
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
		}
		else
		{
			base.Invoke(new Action(this.DelayedSpinningReset), 4f);
		}
		this.CurrentSpinPlayer = null;
	}

	// Token: 0x0600119F RID: 4511 RVA: 0x0008EB9A File Offset: 0x0008CD9A
	private void DelayedSpinningReset()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved2, false, false, true);
	}

	// Token: 0x060011A0 RID: 4512 RVA: 0x0008EBAC File Offset: 0x0008CDAC
	private void CalculateSpinResults()
	{
		if (global::SlotMachine.ForcePayoutIndex != -1)
		{
			this.SpinResult1 = this.PayoutSettings.Payouts[global::SlotMachine.ForcePayoutIndex].Result1;
			this.SpinResult2 = this.PayoutSettings.Payouts[global::SlotMachine.ForcePayoutIndex].Result2;
			this.SpinResult3 = this.PayoutSettings.Payouts[global::SlotMachine.ForcePayoutIndex].Result3;
			return;
		}
		this.SpinResult1 = this.RandomSpinResult();
		this.SpinResult2 = this.RandomSpinResult();
		this.SpinResult3 = this.RandomSpinResult();
	}

	// Token: 0x060011A1 RID: 4513 RVA: 0x0008EC48 File Offset: 0x0008CE48
	private int RandomSpinResult()
	{
		int num = new System.Random(UnityEngine.Random.Range(0, 1000)).Next(0, this.PayoutSettings.TotalStops);
		int num2 = 0;
		int num3 = 0;
		foreach (int num4 in this.PayoutSettings.VirtualFaces)
		{
			if (num < num4 + num2)
			{
				return num3;
			}
			num2 += num4;
			num3++;
		}
		return 15;
	}

	// Token: 0x060011A2 RID: 4514 RVA: 0x0008ECB4 File Offset: 0x0008CEB4
	public override void OnPlayerDismounted(global::BasePlayer player)
	{
		base.OnPlayerDismounted(player);
		global::BaseEntity baseEntity = this.StorageInstance.Get(true);
		SlotMachineStorage slotMachineStorage;
		if (baseEntity != null && (slotMachineStorage = (baseEntity as SlotMachineStorage)) != null)
		{
			global::Item slot = slotMachineStorage.inventory.GetSlot(1);
			if (slot != null)
			{
				slot.MoveToContainer(player.inventory.containerMain, -1, true, false, null, true);
			}
		}
	}

	// Token: 0x060011A3 RID: 4515 RVA: 0x0008ED10 File Offset: 0x0008CF10
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	private void Server_RequestMultiplierChange(global::BaseEntity.RPCMessage msg)
	{
		if (msg.player != this._mounted)
		{
			return;
		}
		this.CurrentMultiplier = Mathf.Clamp(msg.read.Int32(), 1, 5);
		this.OnBettingScrapUpdated(this.GetBettingAmount());
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060011A4 RID: 4516 RVA: 0x0008ED5C File Offset: 0x0008CF5C
	public void OnBettingScrapUpdated(int amount)
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved1, (float)amount >= this.PayoutSettings.SpinCost.amount * (float)this.CurrentMultiplier, false, true);
	}

	// Token: 0x060011A5 RID: 4517 RVA: 0x0008ED8C File Offset: 0x0008CF8C
	private bool CalculatePayout(out SlotMachinePayoutSettings.PayoutInfo info, out int bonus)
	{
		info = default(SlotMachinePayoutSettings.PayoutInfo);
		bonus = 0;
		foreach (SlotMachinePayoutSettings.IndividualPayouts individualPayouts in this.PayoutSettings.FacePayouts)
		{
			if (individualPayouts.Result == this.SpinResult1)
			{
				bonus += (int)individualPayouts.Item.amount;
			}
			if (individualPayouts.Result == this.SpinResult2)
			{
				bonus += (int)individualPayouts.Item.amount;
			}
			if (individualPayouts.Result == this.SpinResult3)
			{
				bonus += (int)individualPayouts.Item.amount;
			}
			if (bonus > 0)
			{
				info.Item = new ItemAmount(individualPayouts.Item.itemDef, 0f);
			}
		}
		foreach (SlotMachinePayoutSettings.PayoutInfo payoutInfo in this.PayoutSettings.Payouts)
		{
			if (payoutInfo.Result1 == this.SpinResult1 && payoutInfo.Result2 == this.SpinResult2 && payoutInfo.Result3 == this.SpinResult3)
			{
				info = payoutInfo;
				return true;
			}
		}
		return bonus > 0;
	}

	// Token: 0x04000AEB RID: 2795
	[ServerVar]
	public static int ForcePayoutIndex = -1;

	// Token: 0x04000AEC RID: 2796
	[Header("Slot Machine")]
	public Transform Reel1;

	// Token: 0x04000AED RID: 2797
	public Transform Reel2;

	// Token: 0x04000AEE RID: 2798
	public Transform Reel3;

	// Token: 0x04000AEF RID: 2799
	public Transform Arm;

	// Token: 0x04000AF0 RID: 2800
	public AnimationCurve Curve;

	// Token: 0x04000AF1 RID: 2801
	public int Reel1Spins = 16;

	// Token: 0x04000AF2 RID: 2802
	public int Reel2Spins = 48;

	// Token: 0x04000AF3 RID: 2803
	public int Reel3Spins = 80;

	// Token: 0x04000AF4 RID: 2804
	public int MaxReelSpins = 96;

	// Token: 0x04000AF5 RID: 2805
	public float SpinDuration = 2f;

	// Token: 0x04000AF6 RID: 2806
	private int SpinResult1;

	// Token: 0x04000AF7 RID: 2807
	private int SpinResult2;

	// Token: 0x04000AF8 RID: 2808
	private int SpinResult3;

	// Token: 0x04000AF9 RID: 2809
	private int SpinResultPrevious1;

	// Token: 0x04000AFA RID: 2810
	private int SpinResultPrevious2;

	// Token: 0x04000AFB RID: 2811
	private int SpinResultPrevious3;

	// Token: 0x04000AFC RID: 2812
	private float SpinTime;

	// Token: 0x04000AFD RID: 2813
	public GameObjectRef StoragePrefab;

	// Token: 0x04000AFE RID: 2814
	public EntityRef StorageInstance;

	// Token: 0x04000AFF RID: 2815
	public SoundDefinition SpinSound;

	// Token: 0x04000B00 RID: 2816
	public SlotMachinePayoutDisplay PayoutDisplay;

	// Token: 0x04000B01 RID: 2817
	public SlotMachinePayoutSettings PayoutSettings;

	// Token: 0x04000B02 RID: 2818
	public Transform HandIkTarget;

	// Token: 0x04000B03 RID: 2819
	private const global::BaseEntity.Flags HasScrapForSpin = global::BaseEntity.Flags.Reserved1;

	// Token: 0x04000B04 RID: 2820
	private const global::BaseEntity.Flags IsSpinningFlag = global::BaseEntity.Flags.Reserved2;

	// Token: 0x04000B05 RID: 2821
	public Material PayoutIconMaterial;

	// Token: 0x04000B06 RID: 2822
	public bool UseTimeOfDayAdjustedSprite = true;

	// Token: 0x04000B07 RID: 2823
	public MeshRenderer[] PulseRenderers;

	// Token: 0x04000B08 RID: 2824
	public float PulseSpeed = 5f;

	// Token: 0x04000B09 RID: 2825
	[ColorUsage(true, true)]
	public Color PulseFrom;

	// Token: 0x04000B0A RID: 2826
	[ColorUsage(true, true)]
	public Color PulseTo;

	// Token: 0x04000B0C RID: 2828
	private global::BasePlayer CurrentSpinPlayer;

	// Token: 0x02000BB6 RID: 2998
	public enum SlotFaces
	{
		// Token: 0x04003F37 RID: 16183
		Scrap,
		// Token: 0x04003F38 RID: 16184
		Rope,
		// Token: 0x04003F39 RID: 16185
		Apple,
		// Token: 0x04003F3A RID: 16186
		LowGrade,
		// Token: 0x04003F3B RID: 16187
		Wood,
		// Token: 0x04003F3C RID: 16188
		Bandage,
		// Token: 0x04003F3D RID: 16189
		Charcoal,
		// Token: 0x04003F3E RID: 16190
		Gunpowder,
		// Token: 0x04003F3F RID: 16191
		Rust,
		// Token: 0x04003F40 RID: 16192
		Meat,
		// Token: 0x04003F41 RID: 16193
		Hammer,
		// Token: 0x04003F42 RID: 16194
		Sulfur,
		// Token: 0x04003F43 RID: 16195
		TechScrap,
		// Token: 0x04003F44 RID: 16196
		Frags,
		// Token: 0x04003F45 RID: 16197
		Cloth,
		// Token: 0x04003F46 RID: 16198
		LuckySeven
	}
}
