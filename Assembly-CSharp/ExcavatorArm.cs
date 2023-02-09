using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200006E RID: 110
public class ExcavatorArm : global::BaseEntity
{
	// Token: 0x06000A85 RID: 2693 RVA: 0x0005EB68 File Offset: 0x0005CD68
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ExcavatorArm.OnRpcMessage", 0))
		{
			if (rpc == 2059417170U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_SetResourceTarget ");
				}
				using (TimeWarning.New("RPC_SetResourceTarget", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2059417170U, "RPC_SetResourceTarget", this, player, 3f))
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
							this.RPC_SetResourceTarget(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_SetResourceTarget");
					}
				}
				return true;
			}
			if (rpc == 2882020740U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_StopMining ");
				}
				using (TimeWarning.New("RPC_StopMining", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2882020740U, "RPC_StopMining", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage msg3 = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_StopMining(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in RPC_StopMining");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000A86 RID: 2694 RVA: 0x000028C8 File Offset: 0x00000AC8
	public bool IsPowered()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved8);
	}

	// Token: 0x06000A87 RID: 2695 RVA: 0x00006C79 File Offset: 0x00004E79
	public bool IsMining()
	{
		return base.IsOn();
	}

	// Token: 0x170000EB RID: 235
	// (get) Token: 0x06000A88 RID: 2696 RVA: 0x00031D65 File Offset: 0x0002FF65
	protected override float PositionTickRate
	{
		get
		{
			return 0.05f;
		}
	}

	// Token: 0x06000A89 RID: 2697 RVA: 0x000058B6 File Offset: 0x00003AB6
	public override float GetNetworkTime()
	{
		return UnityEngine.Time.fixedTime;
	}

	// Token: 0x06000A8A RID: 2698 RVA: 0x0005EE68 File Offset: 0x0005D068
	public void FixedUpdate()
	{
		if (base.isClient)
		{
			return;
		}
		bool flag = this.IsMining() && this.IsPowered();
		float num = flag ? 1f : 0f;
		this.currentTurnThrottle = Mathf.Lerp(this.currentTurnThrottle, num, UnityEngine.Time.fixedDeltaTime * (flag ? 0.333f : 1f));
		if (Mathf.Abs(num - this.currentTurnThrottle) < 0.025f)
		{
			this.currentTurnThrottle = num;
		}
		this.movedAmount += UnityEngine.Time.fixedDeltaTime * this.turnSpeed * this.currentTurnThrottle;
		float t = (Mathf.Sin(this.movedAmount) + 1f) / 2f;
		float num2 = Mathf.Lerp(this.yaw1, this.yaw2, t);
		if (num2 != this.lastMoveYaw)
		{
			this.lastMoveYaw = num2;
			base.transform.rotation = Quaternion.Euler(0f, num2, 0f);
			base.transform.hasChanged = true;
		}
	}

	// Token: 0x06000A8B RID: 2699 RVA: 0x0005EF64 File Offset: 0x0005D164
	public void BeginMining()
	{
		if (!this.IsPowered())
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.On, true, false, true);
		base.InvokeRepeating(new Action(this.ProduceResources), this.resourceProductionTickRate, this.resourceProductionTickRate);
		if (UnityEngine.Time.time > this.nextNotificationTime)
		{
			foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
			{
				if (!basePlayer.IsNpc && basePlayer.IsConnected)
				{
					basePlayer.ShowToast(GameTip.Styles.Server_Event, this.excavatorPhrase, Array.Empty<string>());
				}
			}
			this.nextNotificationTime = UnityEngine.Time.time + 60f;
		}
		ExcavatorServerEffects.SetMining(true, false);
		Analytics.Server.ExcavatorStarted();
		this.excavatorStartTime = this.GetNetworkTime();
	}

	// Token: 0x06000A8C RID: 2700 RVA: 0x0005F038 File Offset: 0x0005D238
	public void StopMining()
	{
		ExcavatorServerEffects.SetMining(false, false);
		base.CancelInvoke(new Action(this.ProduceResources));
		if (base.HasFlag(global::BaseEntity.Flags.On))
		{
			Analytics.Server.ExcavatorStopped(this.GetNetworkTime() - this.excavatorStartTime);
		}
		base.SetFlag(global::BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x06000A8D RID: 2701 RVA: 0x0005F078 File Offset: 0x0005D278
	public void ProduceResources()
	{
		float num = this.resourceProductionTickRate / this.timeForFullResources;
		float num2 = this.resourcesToMine[this.resourceMiningIndex].amount * num;
		this.pendingResources[this.resourceMiningIndex].amount += num2;
		foreach (ItemAmount itemAmount in this.pendingResources)
		{
			if (itemAmount.amount >= (float)this.outputPiles.Count)
			{
				int num3 = Mathf.FloorToInt(itemAmount.amount / (float)this.outputPiles.Count);
				itemAmount.amount -= (float)(num3 * 2);
				foreach (ExcavatorOutputPile excavatorOutputPile in this.outputPiles)
				{
					global::Item item = ItemManager.Create(this.resourcesToMine[this.resourceMiningIndex].itemDef, num3, 0UL);
					if (!item.MoveToContainer(excavatorOutputPile.inventory, -1, true, false, null, true))
					{
						item.Drop(excavatorOutputPile.GetDropPosition(), excavatorOutputPile.GetDropVelocity(), default(Quaternion));
					}
				}
			}
		}
	}

	// Token: 0x06000A8E RID: 2702 RVA: 0x0005F1BC File Offset: 0x0005D3BC
	public override void OnEntityMessage(global::BaseEntity from, string msg)
	{
		base.OnEntityMessage(from, msg);
		if (msg == "DieselEngineOn")
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved8, true, false, true);
			return;
		}
		if (msg == "DieselEngineOff")
		{
			base.SetFlag(global::BaseEntity.Flags.Reserved8, false, false, true);
			this.StopMining();
		}
	}

	// Token: 0x06000A8F RID: 2703 RVA: 0x0005F210 File Offset: 0x0005D410
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_SetResourceTarget(global::BaseEntity.RPCMessage msg)
	{
		string a = msg.read.String(256);
		if (a == "HQM")
		{
			this.resourceMiningIndex = 0;
		}
		else if (a == "Sulfur")
		{
			this.resourceMiningIndex = 1;
		}
		else if (a == "Stone")
		{
			this.resourceMiningIndex = 2;
		}
		else if (a == "Metal")
		{
			this.resourceMiningIndex = 3;
		}
		if (!base.IsOn())
		{
			this.BeginMining();
		}
	}

	// Token: 0x06000A90 RID: 2704 RVA: 0x000059DD File Offset: 0x00003BDD
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_StopMining(global::BaseEntity.RPCMessage msg)
	{
	}

	// Token: 0x06000A91 RID: 2705 RVA: 0x0005F292 File Offset: 0x0005D492
	public override void Spawn()
	{
		base.Spawn();
	}

	// Token: 0x06000A92 RID: 2706 RVA: 0x0005F29A File Offset: 0x0005D49A
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.Init();
		if (base.IsOn() && this.IsPowered())
		{
			this.BeginMining();
			return;
		}
		this.StopMining();
	}

	// Token: 0x06000A93 RID: 2707 RVA: 0x0005F2C8 File Offset: 0x0005D4C8
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity = Facepunch.Pool.Get<ProtoBuf.IOEntity>();
		info.msg.ioEntity.genericFloat1 = this.movedAmount;
		info.msg.ioEntity.genericInt1 = this.resourceMiningIndex;
	}

	// Token: 0x06000A94 RID: 2708 RVA: 0x0005F318 File Offset: 0x0005D518
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.movedAmount = info.msg.ioEntity.genericFloat1;
			this.resourceMiningIndex = info.msg.ioEntity.genericInt1;
		}
	}

	// Token: 0x06000A95 RID: 2709 RVA: 0x0005F365 File Offset: 0x0005D565
	public override void PostMapEntitySpawn()
	{
		base.PostMapEntitySpawn();
		this.Init();
	}

	// Token: 0x06000A96 RID: 2710 RVA: 0x0005F374 File Offset: 0x0005D574
	public void Init()
	{
		this.pendingResources = new ItemAmount[this.resourcesToMine.Length];
		for (int i = 0; i < this.resourcesToMine.Length; i++)
		{
			this.pendingResources[i] = new ItemAmount(this.resourcesToMine[i].itemDef, 0f);
		}
		List<ExcavatorOutputPile> list = Facepunch.Pool.GetList<ExcavatorOutputPile>();
		global::Vis.Entities<ExcavatorOutputPile>(base.transform.position, 200f, list, 512, QueryTriggerInteraction.Collide);
		this.outputPiles = new List<ExcavatorOutputPile>();
		foreach (ExcavatorOutputPile excavatorOutputPile in list)
		{
			if (!excavatorOutputPile.isClient)
			{
				this.outputPiles.Add(excavatorOutputPile);
			}
		}
		Facepunch.Pool.FreeList<ExcavatorOutputPile>(ref list);
	}

	// Token: 0x040006C3 RID: 1731
	public float yaw1;

	// Token: 0x040006C4 RID: 1732
	public float yaw2;

	// Token: 0x040006C5 RID: 1733
	public Transform wheel;

	// Token: 0x040006C6 RID: 1734
	public float wheelSpeed = 2f;

	// Token: 0x040006C7 RID: 1735
	public float turnSpeed = 0.1f;

	// Token: 0x040006C8 RID: 1736
	public Transform miningOffset;

	// Token: 0x040006C9 RID: 1737
	public GameObjectRef bounceEffect;

	// Token: 0x040006CA RID: 1738
	public LightGroupAtTime lights;

	// Token: 0x040006CB RID: 1739
	public Material conveyorMaterial;

	// Token: 0x040006CC RID: 1740
	public float beltSpeedMax = 0.1f;

	// Token: 0x040006CD RID: 1741
	public const global::BaseEntity.Flags Flag_HasPower = global::BaseEntity.Flags.Reserved8;

	// Token: 0x040006CE RID: 1742
	public List<ExcavatorOutputPile> outputPiles;

	// Token: 0x040006CF RID: 1743
	public SoundDefinition miningStartButtonSoundDef;

	// Token: 0x040006D0 RID: 1744
	[Header("Production")]
	public ItemAmount[] resourcesToMine;

	// Token: 0x040006D1 RID: 1745
	public float resourceProductionTickRate = 3f;

	// Token: 0x040006D2 RID: 1746
	public float timeForFullResources = 120f;

	// Token: 0x040006D3 RID: 1747
	private ItemAmount[] pendingResources;

	// Token: 0x040006D4 RID: 1748
	public Translate.Phrase excavatorPhrase;

	// Token: 0x040006D5 RID: 1749
	private float movedAmount;

	// Token: 0x040006D6 RID: 1750
	private float currentTurnThrottle;

	// Token: 0x040006D7 RID: 1751
	private float lastMoveYaw;

	// Token: 0x040006D8 RID: 1752
	private float excavatorStartTime;

	// Token: 0x040006D9 RID: 1753
	private float nextNotificationTime;

	// Token: 0x040006DA RID: 1754
	private int resourceMiningIndex;
}
