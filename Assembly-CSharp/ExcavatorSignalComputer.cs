using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

// Token: 0x0200006F RID: 111
public class ExcavatorSignalComputer : BaseCombatEntity
{
	// Token: 0x06000A98 RID: 2712 RVA: 0x0005F488 File Offset: 0x0005D688
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ExcavatorSignalComputer.OnRpcMessage", 0))
		{
			if (rpc == 1824723998U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RequestSupplies ");
				}
				using (TimeWarning.New("RequestSupplies", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(1824723998U, "RequestSupplies", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(1824723998U, "RequestSupplies", this, player, 3f))
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
							this.RequestSupplies(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RequestSupplies");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000A99 RID: 2713 RVA: 0x0005F648 File Offset: 0x0005D848
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity = Facepunch.Pool.Get<ProtoBuf.IOEntity>();
		info.msg.ioEntity.genericFloat1 = this.chargePower;
		info.msg.ioEntity.genericFloat2 = ExcavatorSignalComputer.chargeNeededForSupplies;
	}

	// Token: 0x06000A9A RID: 2714 RVA: 0x0005F697 File Offset: 0x0005D897
	public override void ServerInit()
	{
		base.ServerInit();
		this.lastChargeTime = UnityEngine.Time.time;
		base.InvokeRepeating(new Action(this.ChargeThink), 0f, 1f);
	}

	// Token: 0x06000A9B RID: 2715 RVA: 0x0005F6C6 File Offset: 0x0005D8C6
	public override void PostServerLoad()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved8, false, false, true);
		base.SetFlag(global::BaseEntity.Flags.Reserved7, false, false, true);
	}

	// Token: 0x06000A9C RID: 2716 RVA: 0x0005F6E4 File Offset: 0x0005D8E4
	public void ChargeThink()
	{
		float num = this.chargePower;
		float num2 = UnityEngine.Time.time - this.lastChargeTime;
		this.lastChargeTime = UnityEngine.Time.time;
		if (this.IsPowered())
		{
			this.chargePower += num2;
		}
		this.chargePower = Mathf.Clamp(this.chargePower, 0f, ExcavatorSignalComputer.chargeNeededForSupplies);
		base.SetFlag(global::BaseEntity.Flags.Reserved7, this.chargePower >= ExcavatorSignalComputer.chargeNeededForSupplies, false, true);
		if (num != this.chargePower)
		{
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000A9D RID: 2717 RVA: 0x0005F76C File Offset: 0x0005D96C
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
		}
	}

	// Token: 0x06000A9E RID: 2718 RVA: 0x0005F7B8 File Offset: 0x0005D9B8
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	public void RequestSupplies(global::BaseEntity.RPCMessage rpc)
	{
		if (base.HasFlag(global::BaseEntity.Flags.Reserved7) && this.IsPowered() && this.chargePower >= ExcavatorSignalComputer.chargeNeededForSupplies)
		{
			global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.supplyPlanePrefab.resourcePath, default(Vector3), default(Quaternion), true);
			if (baseEntity)
			{
				Vector3 position = this.dropPoints[UnityEngine.Random.Range(0, this.dropPoints.Length)].position;
				Vector3 b = new Vector3(UnityEngine.Random.Range(-3f, 3f), 0f, UnityEngine.Random.Range(-3f, 3f));
				baseEntity.SendMessage("InitDropPosition", position + b, SendMessageOptions.DontRequireReceiver);
				baseEntity.Spawn();
			}
			this.chargePower -= ExcavatorSignalComputer.chargeNeededForSupplies;
			base.SetFlag(global::BaseEntity.Flags.Reserved7, false, false, true);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06000A9F RID: 2719 RVA: 0x000028C8 File Offset: 0x00000AC8
	public bool IsPowered()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved8);
	}

	// Token: 0x06000AA0 RID: 2720 RVA: 0x0005F8AC File Offset: 0x0005DAAC
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.chargePower = info.msg.ioEntity.genericFloat1;
		}
	}

	// Token: 0x040006DB RID: 1755
	public float chargePower;

	// Token: 0x040006DC RID: 1756
	public const global::BaseEntity.Flags Flag_Ready = global::BaseEntity.Flags.Reserved7;

	// Token: 0x040006DD RID: 1757
	public const global::BaseEntity.Flags Flag_HasPower = global::BaseEntity.Flags.Reserved8;

	// Token: 0x040006DE RID: 1758
	public GameObjectRef supplyPlanePrefab;

	// Token: 0x040006DF RID: 1759
	public Transform[] dropPoints;

	// Token: 0x040006E0 RID: 1760
	public Text statusText;

	// Token: 0x040006E1 RID: 1761
	public Text timerText;

	// Token: 0x040006E2 RID: 1762
	public static readonly Translate.Phrase readyphrase = new Translate.Phrase("excavator.signal.ready", "READY");

	// Token: 0x040006E3 RID: 1763
	public static readonly Translate.Phrase chargephrase = new Translate.Phrase("excavator.signal.charging", "COMSYS CHARGING");

	// Token: 0x040006E4 RID: 1764
	[ServerVar]
	public static float chargeNeededForSupplies = 600f;

	// Token: 0x040006E5 RID: 1765
	private float lastChargeTime;
}
