using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x02000052 RID: 82
public class CCTV_RC : PoweredRemoteControlEntity
{
	// Token: 0x06000915 RID: 2325 RVA: 0x00055440 File Offset: 0x00053640
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("CCTV_RC.OnRpcMessage", 0))
		{
			if (rpc == 3353964129U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Server_SetDir ");
				}
				using (TimeWarning.New("Server_SetDir", 0))
				{
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
							this.Server_SetDir(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in Server_SetDir");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000916 RID: 2326 RVA: 0x0001F1CE File Offset: 0x0001D3CE
	public override int ConsumptionAmount()
	{
		return 5;
	}

	// Token: 0x06000917 RID: 2327 RVA: 0x00055564 File Offset: 0x00053764
	public override void ServerInit()
	{
		base.ServerInit();
		if (base.isClient)
		{
			return;
		}
		if (base.IsStatic())
		{
			this.pitchAmount = this.pitch.localEulerAngles.x;
			this.yawAmount = this.yaw.localEulerAngles.y;
			base.UpdateRCAccess(true);
		}
	}

	// Token: 0x06000918 RID: 2328 RVA: 0x000555BB File Offset: 0x000537BB
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.UpdateRotation(10000f);
	}

	// Token: 0x06000919 RID: 2329 RVA: 0x000555D0 File Offset: 0x000537D0
	public override void UserInput(InputState inputState, BasePlayer player)
	{
		if (!this.hasPTZ)
		{
			return;
		}
		float num = 1f;
		float num2 = Mathf.Clamp(-inputState.current.mouseDelta.y, -1f, 1f);
		float num3 = Mathf.Clamp(inputState.current.mouseDelta.x, -1f, 1f);
		this.pitchAmount = Mathf.Clamp(this.pitchAmount + num2 * num * this.turnSpeed, this.pitchClamp.x, this.pitchClamp.y);
		this.yawAmount = Mathf.Clamp(this.yawAmount + num3 * num * this.turnSpeed, this.yawClamp.x, this.yawClamp.y);
		Quaternion localRotation = Quaternion.Euler(this.pitchAmount, 0f, 0f);
		Quaternion localRotation2 = Quaternion.Euler(0f, this.yawAmount, 0f);
		this.pitch.transform.localRotation = localRotation;
		this.yaw.transform.localRotation = localRotation2;
		if (num2 != 0f || num3 != 0f)
		{
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x0600091A RID: 2330 RVA: 0x000556F8 File Offset: 0x000538F8
	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.rcEntity.aim.x = this.pitchAmount;
		info.msg.rcEntity.aim.y = this.yawAmount;
		info.msg.rcEntity.aim.z = 0f;
	}

	// Token: 0x0600091B RID: 2331 RVA: 0x0005575C File Offset: 0x0005395C
	[BaseEntity.RPC_Server]
	public void Server_SetDir(BaseEntity.RPCMessage msg)
	{
		if (base.IsStatic())
		{
			return;
		}
		BasePlayer player = msg.player;
		if (!player.CanBuild() || !player.IsBuildingAuthed())
		{
			return;
		}
		Vector3 vector = Vector3Ex.Direction(player.eyes.position, this.yaw.transform.position);
		vector = base.transform.InverseTransformDirection(vector);
		Vector3 vector2 = BaseMountable.ConvertVector(Quaternion.LookRotation(vector).eulerAngles);
		this.pitchAmount = vector2.x;
		this.yawAmount = vector2.y;
		this.pitchAmount = Mathf.Clamp(this.pitchAmount, this.pitchClamp.x, this.pitchClamp.y);
		this.yawAmount = Mathf.Clamp(this.yawAmount, this.yawClamp.x, this.yawClamp.y);
		Quaternion localRotation = Quaternion.Euler(this.pitchAmount, 0f, 0f);
		Quaternion localRotation2 = Quaternion.Euler(0f, this.yawAmount, 0f);
		this.pitch.transform.localRotation = localRotation;
		this.yaw.transform.localRotation = localRotation2;
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600091C RID: 2332 RVA: 0x0005588B File Offset: 0x00053A8B
	public override void InitializeControl(BasePlayer controller)
	{
		base.InitializeControl(controller);
		this.numViewers++;
		this.UpdateViewers();
	}

	// Token: 0x0600091D RID: 2333 RVA: 0x000558A8 File Offset: 0x00053AA8
	public override void StopControl()
	{
		base.StopControl();
		this.numViewers--;
		this.UpdateViewers();
	}

	// Token: 0x0600091E RID: 2334 RVA: 0x000558C4 File Offset: 0x00053AC4
	public void PingFromExternalViewer()
	{
		base.Invoke(new Action(this.ResetExternalViewer), 10f);
		this.externalViewer = true;
		this.UpdateViewers();
	}

	// Token: 0x0600091F RID: 2335 RVA: 0x000558EA File Offset: 0x00053AEA
	private void ResetExternalViewer()
	{
		this.externalViewer = false;
		this.UpdateViewers();
	}

	// Token: 0x06000920 RID: 2336 RVA: 0x000558F9 File Offset: 0x00053AF9
	public void UpdateViewers()
	{
		base.SetFlag(BaseEntity.Flags.Reserved5, this.externalViewer || this.numViewers > 0, false, true);
	}

	// Token: 0x06000921 RID: 2337 RVA: 0x0005591C File Offset: 0x00053B1C
	public void UpdateRotation(float delta)
	{
		Quaternion b = Quaternion.Euler(this.pitchAmount, 0f, 0f);
		Quaternion b2 = Quaternion.Euler(0f, this.yawAmount, 0f);
		float t = delta * (base.isServer ? this.serverLerpSpeed : this.clientLerpSpeed);
		this.pitch.transform.localRotation = Quaternion.Lerp(this.pitch.transform.localRotation, b, t);
		this.yaw.transform.localRotation = Quaternion.Lerp(this.yaw.transform.localRotation, b2, t);
	}

	// Token: 0x06000922 RID: 2338 RVA: 0x000559BC File Offset: 0x00053BBC
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.rcEntity != null)
		{
			this.pitchAmount = info.msg.rcEntity.aim.x;
			this.yawAmount = info.msg.rcEntity.aim.y;
		}
	}

	// Token: 0x040005F6 RID: 1526
	public Transform pivotOrigin;

	// Token: 0x040005F7 RID: 1527
	public Transform yaw;

	// Token: 0x040005F8 RID: 1528
	public Transform pitch;

	// Token: 0x040005F9 RID: 1529
	public Vector2 pitchClamp = new Vector2(-50f, 50f);

	// Token: 0x040005FA RID: 1530
	public Vector2 yawClamp = new Vector2(-50f, 50f);

	// Token: 0x040005FB RID: 1531
	public float turnSpeed = 25f;

	// Token: 0x040005FC RID: 1532
	public float serverLerpSpeed = 15f;

	// Token: 0x040005FD RID: 1533
	public float clientLerpSpeed = 10f;

	// Token: 0x040005FE RID: 1534
	private float pitchAmount;

	// Token: 0x040005FF RID: 1535
	private float yawAmount;

	// Token: 0x04000600 RID: 1536
	public bool hasPTZ = true;

	// Token: 0x04000601 RID: 1537
	public const BaseEntity.Flags Flag_HasViewer = BaseEntity.Flags.Reserved5;

	// Token: 0x04000602 RID: 1538
	private int numViewers;

	// Token: 0x04000603 RID: 1539
	private bool externalViewer;
}
