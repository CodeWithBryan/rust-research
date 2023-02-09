using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000E4 RID: 228
public class VehicleModuleTaxi : VehicleModuleStorage
{
	// Token: 0x060013D1 RID: 5073 RVA: 0x0009BFD8 File Offset: 0x0009A1D8
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("VehicleModuleTaxi.OnRpcMessage", 0))
		{
			if (rpc == 2714639811U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_KickPassengers ");
				}
				using (TimeWarning.New("RPC_KickPassengers", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test(2714639811U, "RPC_KickPassengers", this, player, 3f))
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
							this.RPC_KickPassengers(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_KickPassengers");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x170001A7 RID: 423
	// (get) Token: 0x060013D2 RID: 5074 RVA: 0x0009C140 File Offset: 0x0009A340
	private Vector3 KickButtonPos
	{
		get
		{
			return this.kickButtonCollider.transform.position + this.kickButtonCollider.transform.rotation * this.kickButtonCollider.center;
		}
	}

	// Token: 0x060013D3 RID: 5075 RVA: 0x0009C178 File Offset: 0x0009A378
	private bool CanKickPassengers(BasePlayer player)
	{
		if (!base.IsOnAVehicle)
		{
			return false;
		}
		if (base.Vehicle.GetSpeed() > this.maxKickVelocity)
		{
			return false;
		}
		if (player == null)
		{
			return false;
		}
		if (!base.Vehicle.PlayerIsMounted(player))
		{
			return false;
		}
		Vector3 lhs = this.KickButtonPos - player.transform.position;
		return Vector3.Dot(lhs, player.transform.forward) < 0f && lhs.sqrMagnitude < 4f;
	}

	// Token: 0x060013D4 RID: 5076 RVA: 0x0009C200 File Offset: 0x0009A400
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.MaxDistance(3f)]
	public void RPC_KickPassengers(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (player == null)
		{
			return;
		}
		if (!this.CanKickPassengers(player))
		{
			return;
		}
		this.KickPassengers();
	}

	// Token: 0x060013D5 RID: 5077 RVA: 0x0009C230 File Offset: 0x0009A430
	private void KickPassengers()
	{
		if (!base.IsOnAVehicle)
		{
			return;
		}
		foreach (BaseVehicle.MountPointInfo mountPointInfo in this.mountPoints)
		{
			BaseMountable mountable = mountPointInfo.mountable;
			BasePlayer mounted = mountable.GetMounted();
			if (mounted != null && mountable.HasValidDismountPosition(mounted))
			{
				mountable.AttemptDismount(mounted);
			}
		}
	}

	// Token: 0x04000C7A RID: 3194
	[Header("Taxi")]
	[SerializeField]
	private SoundDefinition kickButtonSound;

	// Token: 0x04000C7B RID: 3195
	[SerializeField]
	private SphereCollider kickButtonCollider;

	// Token: 0x04000C7C RID: 3196
	[SerializeField]
	private float maxKickVelocity = 4f;
}
