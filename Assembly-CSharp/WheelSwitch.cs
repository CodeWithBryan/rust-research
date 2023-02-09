using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000E8 RID: 232
public class WheelSwitch : global::IOEntity
{
	// Token: 0x06001429 RID: 5161 RVA: 0x0009ED7C File Offset: 0x0009CF7C
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("WheelSwitch.OnRpcMessage", 0))
		{
			if (rpc == 2223603322U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - BeginRotate ");
				}
				using (TimeWarning.New("BeginRotate", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2223603322U, "BeginRotate", this, player, 3f))
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
							this.BeginRotate(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in BeginRotate");
					}
				}
				return true;
			}
			if (rpc == 434251040U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - CancelRotate ");
				}
				using (TimeWarning.New("CancelRotate", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(434251040U, "CancelRotate", this, player, 3f))
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
							this.CancelRotate(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in CancelRotate");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600142A RID: 5162 RVA: 0x0009F07C File Offset: 0x0009D27C
	public override void ResetIOState()
	{
		this.CancelPlayerRotation();
	}

	// Token: 0x0600142B RID: 5163 RVA: 0x0009F084 File Offset: 0x0009D284
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void BeginRotate(global::BaseEntity.RPCMessage msg)
	{
		if (this.IsBeingRotated())
		{
			return;
		}
		base.SetFlag(this.BeingRotated, true, false, true);
		this.rotatorPlayer = msg.player;
		base.InvokeRepeating(new Action(this.RotateProgress), 0f, this.progressTickRate);
	}

	// Token: 0x0600142C RID: 5164 RVA: 0x0009F0D4 File Offset: 0x0009D2D4
	public void CancelPlayerRotation()
	{
		base.CancelInvoke(new Action(this.RotateProgress));
		base.SetFlag(this.BeingRotated, false, false, true);
		foreach (global::IOEntity.IOSlot ioslot in this.outputs)
		{
			if (ioslot.connectedTo.Get(true) != null)
			{
				ioslot.connectedTo.Get(true).IOInput(this, this.ioType, 0f, ioslot.connectedToSlot);
			}
		}
		this.rotatorPlayer = null;
	}

	// Token: 0x0600142D RID: 5165 RVA: 0x0009F15C File Offset: 0x0009D35C
	public void RotateProgress()
	{
		if (!this.rotatorPlayer || this.rotatorPlayer.IsDead() || this.rotatorPlayer.IsSleeping() || Vector3Ex.Distance2D(this.rotatorPlayer.transform.position, base.transform.position) > 2f)
		{
			this.CancelPlayerRotation();
			return;
		}
		float num = this.kineticEnergyPerSec * this.progressTickRate;
		foreach (global::IOEntity.IOSlot ioslot in this.outputs)
		{
			if (ioslot.connectedTo.Get(true) != null)
			{
				num = ioslot.connectedTo.Get(true).IOInput(this, this.ioType, num, ioslot.connectedToSlot);
			}
		}
		if (num == 0f)
		{
			this.SetRotateProgress(this.rotateProgress + 0.1f);
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600142E RID: 5166 RVA: 0x0009F23C File Offset: 0x0009D43C
	public void SetRotateProgress(float newValue)
	{
		float num = this.rotateProgress;
		this.rotateProgress = newValue;
		base.SetFlag(global::BaseEntity.Flags.Reserved4, num != newValue, false, true);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		base.CancelInvoke(new Action(this.StoppedRotatingCheck));
		base.Invoke(new Action(this.StoppedRotatingCheck), 0.25f);
	}

	// Token: 0x0600142F RID: 5167 RVA: 0x0009F29B File Offset: 0x0009D49B
	public void StoppedRotatingCheck()
	{
		base.SetFlag(global::BaseEntity.Flags.Reserved4, false, false, true);
	}

	// Token: 0x06001430 RID: 5168 RVA: 0x0009F07C File Offset: 0x0009D27C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	public void CancelRotate(global::BaseEntity.RPCMessage msg)
	{
		this.CancelPlayerRotation();
	}

	// Token: 0x06001431 RID: 5169 RVA: 0x0009F2AC File Offset: 0x0009D4AC
	public void Powered()
	{
		float inputAmount = this.kineticEnergyPerSec * this.progressTickRate;
		foreach (global::IOEntity.IOSlot ioslot in this.outputs)
		{
			if (ioslot.connectedTo.Get(true) != null)
			{
				inputAmount = ioslot.connectedTo.Get(true).IOInput(this, this.ioType, inputAmount, ioslot.connectedToSlot);
			}
		}
		this.SetRotateProgress(this.rotateProgress + 0.1f);
	}

	// Token: 0x06001432 RID: 5170 RVA: 0x0009F328 File Offset: 0x0009D528
	public override float IOInput(global::IOEntity from, global::IOEntity.IOType inputType, float inputAmount, int slot = 0)
	{
		if (inputAmount < 0f)
		{
			this.SetRotateProgress(this.rotateProgress + inputAmount);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
		if (inputType == global::IOEntity.IOType.Electric && slot == 1)
		{
			if (inputAmount == 0f)
			{
				base.CancelInvoke(new Action(this.Powered));
			}
			else
			{
				base.InvokeRepeating(new Action(this.Powered), 0f, this.progressTickRate);
			}
		}
		return Mathf.Clamp(inputAmount - 1f, 0f, inputAmount);
	}

	// Token: 0x06001433 RID: 5171 RVA: 0x0009F3A5 File Offset: 0x0009D5A5
	public bool IsBeingRotated()
	{
		return base.HasFlag(this.BeingRotated);
	}

	// Token: 0x06001434 RID: 5172 RVA: 0x0009F3B3 File Offset: 0x0009D5B3
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.sphereEntity == null)
		{
			return;
		}
		this.rotateProgress = info.msg.sphereEntity.radius;
	}

	// Token: 0x06001435 RID: 5173 RVA: 0x0009F3E0 File Offset: 0x0009D5E0
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.sphereEntity = Facepunch.Pool.Get<ProtoBuf.SphereEntity>();
		info.msg.sphereEntity.radius = this.rotateProgress;
	}

	// Token: 0x04000CBE RID: 3262
	public Transform wheelObj;

	// Token: 0x04000CBF RID: 3263
	public float rotateSpeed = 90f;

	// Token: 0x04000CC0 RID: 3264
	public global::BaseEntity.Flags BeingRotated = global::BaseEntity.Flags.Reserved1;

	// Token: 0x04000CC1 RID: 3265
	public global::BaseEntity.Flags RotatingLeft = global::BaseEntity.Flags.Reserved2;

	// Token: 0x04000CC2 RID: 3266
	public global::BaseEntity.Flags RotatingRight = global::BaseEntity.Flags.Reserved3;

	// Token: 0x04000CC3 RID: 3267
	public float rotateProgress;

	// Token: 0x04000CC4 RID: 3268
	public Animator animator;

	// Token: 0x04000CC5 RID: 3269
	public float kineticEnergyPerSec = 1f;

	// Token: 0x04000CC6 RID: 3270
	private global::BasePlayer rotatorPlayer;

	// Token: 0x04000CC7 RID: 3271
	private float progressTickRate = 0.1f;
}
