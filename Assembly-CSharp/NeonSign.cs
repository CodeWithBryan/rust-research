using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x0200009F RID: 159
public class NeonSign : Signage
{
	// Token: 0x06000EB9 RID: 3769 RVA: 0x0007AEC4 File Offset: 0x000790C4
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("NeonSign.OnRpcMessage", 0))
		{
			if (rpc == 2433901419U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - SetAnimationSpeed ");
				}
				using (TimeWarning.New("SetAnimationSpeed", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(2433901419U, "SetAnimationSpeed", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2433901419U, "SetAnimationSpeed", this, player, 3f))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							global::BaseEntity.RPCMessage rpcmessage = new global::BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SetAnimationSpeed(rpcmessage);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in SetAnimationSpeed");
					}
				}
				return true;
			}
			if (rpc == 1919786296U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - UpdateNeonColors ");
				}
				using (TimeWarning.New("UpdateNeonColors", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(1919786296U, "UpdateNeonColors", this, player, 5UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(1919786296U, "UpdateNeonColors", this, player, 3f))
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
							this.UpdateNeonColors(msg2);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in UpdateNeonColors");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000EBA RID: 3770 RVA: 0x0007B1FC File Offset: 0x000793FC
	public override int ConsumptionAmount()
	{
		return this.powerConsumption;
	}

	// Token: 0x06000EBB RID: 3771 RVA: 0x0007B204 File Offset: 0x00079404
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.neonSign != null)
		{
			if (this.frameLighting != null)
			{
				foreach (ProtoBuf.NeonSign.Lights lights in this.frameLighting)
				{
					Facepunch.Pool.Free<ProtoBuf.NeonSign.Lights>(ref lights);
				}
				Facepunch.Pool.FreeList<ProtoBuf.NeonSign.Lights>(ref this.frameLighting);
			}
			this.frameLighting = info.msg.neonSign.frameLighting;
			info.msg.neonSign.frameLighting = null;
			this.currentFrame = Mathf.Clamp(info.msg.neonSign.currentFrame, 0, this.paintableSources.Length);
			this.animationSpeed = Mathf.Clamp(info.msg.neonSign.animationSpeed, 0.5f, 5f);
		}
	}

	// Token: 0x06000EBC RID: 3772 RVA: 0x0007B2F4 File Offset: 0x000794F4
	public override void ServerInit()
	{
		base.ServerInit();
		this.animationLoopAction = new Action(this.SwitchToNextFrame);
	}

	// Token: 0x06000EBD RID: 3773 RVA: 0x0007B30E File Offset: 0x0007950E
	public override void ResetState()
	{
		base.ResetState();
		base.CancelInvoke(this.animationLoopAction);
	}

	// Token: 0x06000EBE RID: 3774 RVA: 0x0007B324 File Offset: 0x00079524
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.UpdateHasPower(inputAmount, inputSlot);
		if (this.paintableSources.Length <= 1)
		{
			return;
		}
		bool flag = base.HasFlag(global::BaseEntity.Flags.Reserved8);
		if (flag && !this.isAnimating)
		{
			if (this.currentFrame != 0)
			{
				this.currentFrame = 0;
				base.ClientRPC<int>(null, "SetFrame", this.currentFrame);
			}
			base.InvokeRepeating(this.animationLoopAction, this.animationSpeed, this.animationSpeed);
			this.isAnimating = true;
			return;
		}
		if (!flag && this.isAnimating)
		{
			base.CancelInvoke(this.animationLoopAction);
			this.isAnimating = false;
		}
	}

	// Token: 0x06000EBF RID: 3775 RVA: 0x0007B3BC File Offset: 0x000795BC
	private void SwitchToNextFrame()
	{
		int num = this.currentFrame;
		for (int i = 0; i < this.paintableSources.Length; i++)
		{
			this.currentFrame++;
			if (this.currentFrame >= this.paintableSources.Length)
			{
				this.currentFrame = 0;
			}
			if (this.textureIDs[this.currentFrame] != 0U)
			{
				break;
			}
		}
		if (this.currentFrame != num)
		{
			base.ClientRPC<int>(null, "SetFrame", this.currentFrame);
		}
	}

	// Token: 0x06000EC0 RID: 3776 RVA: 0x0007B434 File Offset: 0x00079634
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		List<ProtoBuf.NeonSign.Lights> list = Facepunch.Pool.GetList<ProtoBuf.NeonSign.Lights>();
		if (this.frameLighting != null)
		{
			foreach (ProtoBuf.NeonSign.Lights lights in this.frameLighting)
			{
				list.Add(lights.Copy());
			}
		}
		info.msg.neonSign = Facepunch.Pool.Get<ProtoBuf.NeonSign>();
		info.msg.neonSign.frameLighting = list;
		info.msg.neonSign.currentFrame = this.currentFrame;
		info.msg.neonSign.animationSpeed = this.animationSpeed;
	}

	// Token: 0x06000EC1 RID: 3777 RVA: 0x0007B4F0 File Offset: 0x000796F0
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void SetAnimationSpeed(global::BaseEntity.RPCMessage msg)
	{
		float num = Mathf.Clamp(msg.read.Float(), 0.5f, 5f);
		this.animationSpeed = num;
		if (this.isAnimating)
		{
			base.CancelInvoke(this.animationLoopAction);
			base.InvokeRepeating(this.animationLoopAction, this.animationSpeed, this.animationSpeed);
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000EC2 RID: 3778 RVA: 0x0007B554 File Offset: 0x00079754
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.CallsPerSecond(5UL)]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	public void UpdateNeonColors(global::BaseEntity.RPCMessage msg)
	{
		if (!this.CanUpdateSign(msg.player))
		{
			return;
		}
		int num = msg.read.Int32();
		if (num < 0 || num >= this.paintableSources.Length)
		{
			return;
		}
		this.EnsureInitialized();
		this.frameLighting[num].topLeft = global::NeonSign.ClampColor(msg.read.Color());
		this.frameLighting[num].topRight = global::NeonSign.ClampColor(msg.read.Color());
		this.frameLighting[num].bottomLeft = global::NeonSign.ClampColor(msg.read.Color());
		this.frameLighting[num].bottomRight = global::NeonSign.ClampColor(msg.read.Color());
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000EC3 RID: 3779 RVA: 0x0007B620 File Offset: 0x00079820
	private void EnsureInitialized()
	{
		if (this.frameLighting == null)
		{
			this.frameLighting = Facepunch.Pool.GetList<ProtoBuf.NeonSign.Lights>();
		}
		while (this.frameLighting.Count < this.paintableSources.Length)
		{
			ProtoBuf.NeonSign.Lights lights = Facepunch.Pool.Get<ProtoBuf.NeonSign.Lights>();
			lights.topLeft = Color.clear;
			lights.topRight = Color.clear;
			lights.bottomLeft = Color.clear;
			lights.bottomRight = Color.clear;
			this.frameLighting.Add(lights);
		}
	}

	// Token: 0x06000EC4 RID: 3780 RVA: 0x0007B695 File Offset: 0x00079895
	private static Color ClampColor(Color color)
	{
		return new Color(Mathf.Clamp01(color.r), Mathf.Clamp01(color.g), Mathf.Clamp01(color.b), Mathf.Clamp01(color.a));
	}

	// Token: 0x0400099D RID: 2461
	private const float FastSpeed = 0.5f;

	// Token: 0x0400099E RID: 2462
	private const float MediumSpeed = 1f;

	// Token: 0x0400099F RID: 2463
	private const float SlowSpeed = 2f;

	// Token: 0x040009A0 RID: 2464
	private const float MinSpeed = 0.5f;

	// Token: 0x040009A1 RID: 2465
	private const float MaxSpeed = 5f;

	// Token: 0x040009A2 RID: 2466
	[Header("Neon Sign")]
	public Light topLeft;

	// Token: 0x040009A3 RID: 2467
	public Light topRight;

	// Token: 0x040009A4 RID: 2468
	public Light bottomLeft;

	// Token: 0x040009A5 RID: 2469
	public Light bottomRight;

	// Token: 0x040009A6 RID: 2470
	public float lightIntensity = 2f;

	// Token: 0x040009A7 RID: 2471
	[Range(1f, 100f)]
	public int powerConsumption = 10;

	// Token: 0x040009A8 RID: 2472
	public Material activeMaterial;

	// Token: 0x040009A9 RID: 2473
	public Material inactiveMaterial;

	// Token: 0x040009AA RID: 2474
	private float animationSpeed = 1f;

	// Token: 0x040009AB RID: 2475
	private int currentFrame;

	// Token: 0x040009AC RID: 2476
	private List<ProtoBuf.NeonSign.Lights> frameLighting;

	// Token: 0x040009AD RID: 2477
	private bool isAnimating;

	// Token: 0x040009AE RID: 2478
	private Action animationLoopAction;

	// Token: 0x040009AF RID: 2479
	public AmbienceEmitter ambientSoundEmitter;

	// Token: 0x040009B0 RID: 2480
	public SoundDefinition switchSoundDef;
}
