using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000B1 RID: 177
public class ProceduralLift : global::BaseEntity
{
	// Token: 0x06000FEF RID: 4079 RVA: 0x000834B0 File Offset: 0x000816B0
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ProceduralLift.OnRpcMessage", 0))
		{
			if (rpc == 2657791441U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - RPC_UseLift ");
				}
				using (TimeWarning.New("RPC_UseLift", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.MaxDistance.Test(2657791441U, "RPC_UseLift", this, player, 3f))
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
							this.RPC_UseLift(rpc2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in RPC_UseLift");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000FF0 RID: 4080 RVA: 0x00083618 File Offset: 0x00081818
	public override void Spawn()
	{
		base.Spawn();
		if (!Rust.Application.isLoadingSave)
		{
			global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.triggerPrefab.resourcePath, Vector3.zero, Quaternion.identity, true);
			baseEntity.Spawn();
			baseEntity.SetParent(this, this.triggerBone, false, false);
		}
	}

	// Token: 0x06000FF1 RID: 4081 RVA: 0x00083666 File Offset: 0x00081866
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.MaxDistance(3f)]
	private void RPC_UseLift(global::BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (base.IsBusy())
		{
			return;
		}
		this.MoveToFloor((this.floorIndex + 1) % this.stops.Length);
	}

	// Token: 0x06000FF2 RID: 4082 RVA: 0x00083696 File Offset: 0x00081896
	public override void ServerInit()
	{
		base.ServerInit();
		this.SnapToFloor(0);
	}

	// Token: 0x06000FF3 RID: 4083 RVA: 0x000836A5 File Offset: 0x000818A5
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.lift = Facepunch.Pool.Get<ProtoBuf.Lift>();
		info.msg.lift.floor = this.floorIndex;
	}

	// Token: 0x06000FF4 RID: 4084 RVA: 0x000836D4 File Offset: 0x000818D4
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		if (info.msg.lift != null)
		{
			if (this.floorIndex == -1)
			{
				this.SnapToFloor(info.msg.lift.floor);
			}
			else
			{
				this.MoveToFloor(info.msg.lift.floor);
			}
		}
		base.Load(info);
	}

	// Token: 0x06000FF5 RID: 4085 RVA: 0x0008372C File Offset: 0x0008192C
	private void ResetLift()
	{
		this.MoveToFloor(0);
	}

	// Token: 0x06000FF6 RID: 4086 RVA: 0x00083738 File Offset: 0x00081938
	private void MoveToFloor(int floor)
	{
		this.floorIndex = Mathf.Clamp(floor, 0, this.stops.Length - 1);
		if (base.isServer)
		{
			base.SetFlag(global::BaseEntity.Flags.Busy, true, false, true);
			base.SendNetworkUpdateImmediate(false);
			base.CancelInvoke(new Action(this.ResetLift));
		}
	}

	// Token: 0x06000FF7 RID: 4087 RVA: 0x0008378C File Offset: 0x0008198C
	private void SnapToFloor(int floor)
	{
		this.floorIndex = Mathf.Clamp(floor, 0, this.stops.Length - 1);
		ProceduralLiftStop proceduralLiftStop = this.stops[this.floorIndex];
		this.cabin.transform.position = proceduralLiftStop.transform.position;
		if (base.isServer)
		{
			base.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
			base.SendNetworkUpdateImmediate(false);
			base.CancelInvoke(new Action(this.ResetLift));
		}
	}

	// Token: 0x06000FF8 RID: 4088 RVA: 0x00083808 File Offset: 0x00081A08
	private void OnFinishedMoving()
	{
		if (base.isServer)
		{
			base.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
			base.SendNetworkUpdateImmediate(false);
			if (this.floorIndex != 0)
			{
				base.Invoke(new Action(this.ResetLift), this.resetDelay);
			}
		}
	}

	// Token: 0x06000FF9 RID: 4089 RVA: 0x00083848 File Offset: 0x00081A48
	protected void Update()
	{
		if (this.floorIndex < 0 || this.floorIndex > this.stops.Length - 1)
		{
			return;
		}
		ProceduralLiftStop proceduralLiftStop = this.stops[this.floorIndex];
		if (this.cabin.transform.position == proceduralLiftStop.transform.position)
		{
			return;
		}
		this.cabin.transform.position = Vector3.MoveTowards(this.cabin.transform.position, proceduralLiftStop.transform.position, this.movementSpeed * UnityEngine.Time.deltaTime);
		if (this.cabin.transform.position == proceduralLiftStop.transform.position)
		{
			this.OnFinishedMoving();
		}
	}

	// Token: 0x06000FFA RID: 4090 RVA: 0x000059DD File Offset: 0x00003BDD
	public void StartMovementSounds()
	{
	}

	// Token: 0x06000FFB RID: 4091 RVA: 0x000059DD File Offset: 0x00003BDD
	public void StopMovementSounds()
	{
	}

	// Token: 0x04000A1E RID: 2590
	public float movementSpeed = 1f;

	// Token: 0x04000A1F RID: 2591
	public float resetDelay = 5f;

	// Token: 0x04000A20 RID: 2592
	public ProceduralLiftCabin cabin;

	// Token: 0x04000A21 RID: 2593
	public ProceduralLiftStop[] stops;

	// Token: 0x04000A22 RID: 2594
	public GameObjectRef triggerPrefab;

	// Token: 0x04000A23 RID: 2595
	public string triggerBone;

	// Token: 0x04000A24 RID: 2596
	private int floorIndex = -1;

	// Token: 0x04000A25 RID: 2597
	public SoundDefinition startSoundDef;

	// Token: 0x04000A26 RID: 2598
	public SoundDefinition stopSoundDef;

	// Token: 0x04000A27 RID: 2599
	public SoundDefinition movementLoopSoundDef;

	// Token: 0x04000A28 RID: 2600
	private Sound movementLoopSound;
}
