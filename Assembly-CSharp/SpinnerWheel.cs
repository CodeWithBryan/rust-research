using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000CC RID: 204
public class SpinnerWheel : Signage
{
	// Token: 0x06001200 RID: 4608 RVA: 0x00090760 File Offset: 0x0008E960
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SpinnerWheel.OnRpcMessage", 0))
		{
			if (rpc == 3019675107U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_AnyoneSpin ");
				}
				using (TimeWarning.New("RPC_AnyoneSpin", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(3019675107U, "RPC_AnyoneSpin", this, player, 3f))
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
							this.RPC_AnyoneSpin(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_AnyoneSpin");
					}
				}
				return true;
			}
			if (rpc == 1455840454U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
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
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001201 RID: 4609 RVA: 0x00003A54 File Offset: 0x00001C54
	public virtual bool AllowPlayerSpins()
	{
		return true;
	}

	// Token: 0x06001202 RID: 4610 RVA: 0x00090A60 File Offset: 0x0008EC60
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.spinnerWheel = Facepunch.Pool.Get<ProtoBuf.SpinnerWheel>();
		info.msg.spinnerWheel.spin = this.wheel.rotation.eulerAngles;
	}

	// Token: 0x06001203 RID: 4611 RVA: 0x00090AA8 File Offset: 0x0008ECA8
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.spinnerWheel != null)
		{
			Quaternion rotation = Quaternion.Euler(info.msg.spinnerWheel.spin);
			if (base.isServer)
			{
				this.wheel.transform.rotation = rotation;
			}
		}
	}

	// Token: 0x06001204 RID: 4612 RVA: 0x00090AF8 File Offset: 0x0008ECF8
	public virtual float GetMaxSpinSpeed()
	{
		return 720f;
	}

	// Token: 0x06001205 RID: 4613 RVA: 0x00090B00 File Offset: 0x0008ED00
	public virtual void Update_Server()
	{
		if (this.velocity > 0f)
		{
			float num = Mathf.Clamp(this.GetMaxSpinSpeed() * this.velocity, 0f, this.GetMaxSpinSpeed());
			this.velocity -= UnityEngine.Time.deltaTime * Mathf.Clamp(this.velocity / 2f, 0.1f, 1f);
			if (this.velocity < 0f)
			{
				this.velocity = 0f;
			}
			this.wheel.Rotate(Vector3.up, num * UnityEngine.Time.deltaTime, Space.Self);
			base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x06001206 RID: 4614 RVA: 0x000059DD File Offset: 0x00003BDD
	public void Update_Client()
	{
	}

	// Token: 0x06001207 RID: 4615 RVA: 0x00090BA0 File Offset: 0x0008EDA0
	public void Update()
	{
		if (base.isClient)
		{
			this.Update_Client();
		}
		if (base.isServer)
		{
			this.Update_Server();
		}
	}

	// Token: 0x06001208 RID: 4616 RVA: 0x00090BC0 File Offset: 0x0008EDC0
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_Spin(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!this.AllowPlayerSpins())
		{
			return;
		}
		if (this.AnyoneSpin() || rpc.player.CanBuild())
		{
			if (this.velocity > 15f)
			{
				return;
			}
			this.velocity += UnityEngine.Random.Range(4f, 7f);
		}
	}

	// Token: 0x06001209 RID: 4617 RVA: 0x00090C23 File Offset: 0x0008EE23
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_AnyoneSpin(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved3, rpc.read.Bit(), false, true);
	}

	// Token: 0x0600120A RID: 4618 RVA: 0x0002D546 File Offset: 0x0002B746
	public bool AnyoneSpin()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved3);
	}

	// Token: 0x04000B48 RID: 2888
	public Transform wheel;

	// Token: 0x04000B49 RID: 2889
	public float velocity;

	// Token: 0x04000B4A RID: 2890
	public Quaternion targetRotation = Quaternion.identity;

	// Token: 0x04000B4B RID: 2891
	[Header("Sound")]
	public SoundDefinition spinLoopSoundDef;

	// Token: 0x04000B4C RID: 2892
	public SoundDefinition spinStartSoundDef;

	// Token: 0x04000B4D RID: 2893
	public SoundDefinition spinAccentSoundDef;

	// Token: 0x04000B4E RID: 2894
	public SoundDefinition spinStopSoundDef;

	// Token: 0x04000B4F RID: 2895
	public float minTimeBetweenSpinAccentSounds = 0.3f;

	// Token: 0x04000B50 RID: 2896
	public float spinAccentAngleDelta = 180f;

	// Token: 0x04000B51 RID: 2897
	private Sound spinSound;

	// Token: 0x04000B52 RID: 2898
	private SoundModulation.Modulator spinSoundGain;
}
