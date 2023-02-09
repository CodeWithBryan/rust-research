using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000AE RID: 174
public class PoweredLightsDeployer : global::HeldEntity
{
	// Token: 0x06000FC4 RID: 4036 RVA: 0x00082824 File Offset: 0x00080A24
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PoweredLightsDeployer.OnRpcMessage", 0))
		{
			if (rpc == 447739874U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - AddPoint ");
				}
				using (TimeWarning.New("AddPoint", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(447739874U, "AddPoint", this, player))
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
							this.AddPoint(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in AddPoint");
					}
				}
				return true;
			}
			if (rpc == 1975273522U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - Finish ");
				}
				using (TimeWarning.New("Finish", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.IsActiveItem.Test(1975273522U, "Finish", this, player))
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
							this.Finish(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in Finish");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000FC5 RID: 4037 RVA: 0x00082B1C File Offset: 0x00080D1C
	public static bool CanPlayerUse(global::BasePlayer player)
	{
		return player.CanBuild() && !GamePhysics.CheckSphere(player.eyes.position, 0.1f, 536870912, QueryTriggerInteraction.Collide);
	}

	// Token: 0x1700015C RID: 348
	// (get) Token: 0x06000FC6 RID: 4038 RVA: 0x00082B48 File Offset: 0x00080D48
	// (set) Token: 0x06000FC7 RID: 4039 RVA: 0x00082B77 File Offset: 0x00080D77
	public AdvancedChristmasLights active
	{
		get
		{
			global::BaseEntity baseEntity = this.activeLights.Get(base.isServer);
			if (baseEntity)
			{
				return baseEntity.GetComponent<AdvancedChristmasLights>();
			}
			return null;
		}
		set
		{
			this.activeLights.Set(value);
		}
	}

	// Token: 0x06000FC8 RID: 4040 RVA: 0x00082B88 File Offset: 0x00080D88
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	public void AddPoint(global::BaseEntity.RPCMessage msg)
	{
		Vector3 vector = msg.read.Vector3();
		Vector3 vector2 = msg.read.Vector3();
		global::BasePlayer player = msg.player;
		if (this.GetItem() == null)
		{
			return;
		}
		if (this.GetItem().amount < 1)
		{
			return;
		}
		if (!base.IsVisible(vector, float.PositiveInfinity))
		{
			return;
		}
		if (!PoweredLightsDeployer.CanPlayerUse(player))
		{
			return;
		}
		if (Vector3.Distance(vector, player.eyes.position) > this.maxPlaceDistance)
		{
			return;
		}
		int num;
		if (this.active == null)
		{
			AdvancedChristmasLights component = GameManager.server.CreateEntity(this.poweredLightsPrefab.resourcePath, vector, Quaternion.LookRotation(vector2, player.eyes.HeadUp()), true).GetComponent<AdvancedChristmasLights>();
			component.Spawn();
			this.active = component;
			num = 1;
		}
		else
		{
			if (this.active.IsFinalized())
			{
				return;
			}
			float num2 = 0f;
			Vector3 vector3 = this.active.transform.position;
			if (this.active.points.Count > 0)
			{
				vector3 = this.active.points[this.active.points.Count - 1].point;
				num2 = Vector3.Distance(vector, vector3);
			}
			num2 = Mathf.Max(num2, this.lengthPerAmount);
			float num3 = (float)this.GetItem().amount * this.lengthPerAmount;
			if (num2 > num3)
			{
				num2 = num3;
				vector = vector3 + Vector3Ex.Direction(vector, vector3) * num2;
			}
			num2 = Mathf.Min(num3, num2);
			num = Mathf.CeilToInt(num2 / this.lengthPerAmount);
		}
		this.active.AddPoint(vector, vector2);
		base.SetFlag(global::BaseEntity.Flags.Reserved8, this.active != null, false, true);
		int iAmount = num;
		base.UseItemAmount(iAmount);
		this.active.AddLengthUsed(num);
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000FC9 RID: 4041 RVA: 0x00082D64 File Offset: 0x00080F64
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsActiveItem]
	public void Finish(global::BaseEntity.RPCMessage msg)
	{
		this.DoFinish();
	}

	// Token: 0x06000FCA RID: 4042 RVA: 0x00082D6C File Offset: 0x00080F6C
	public void DoFinish()
	{
		if (this.active)
		{
			this.active.FinishEditing();
		}
		this.active = null;
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000FCB RID: 4043 RVA: 0x00082D94 File Offset: 0x00080F94
	public override void OnHeldChanged()
	{
		this.DoFinish();
		this.active = null;
		base.OnHeldChanged();
	}

	// Token: 0x06000FCC RID: 4044 RVA: 0x00082DA9 File Offset: 0x00080FA9
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			info.msg.lightDeployer = Facepunch.Pool.Get<LightDeployer>();
			info.msg.lightDeployer.active = this.activeLights.uid;
		}
	}

	// Token: 0x04000A0E RID: 2574
	public GameObjectRef poweredLightsPrefab;

	// Token: 0x04000A0F RID: 2575
	public EntityRef activeLights;

	// Token: 0x04000A10 RID: 2576
	public MaterialReplacement guide;

	// Token: 0x04000A11 RID: 2577
	public GameObject guideObject;

	// Token: 0x04000A12 RID: 2578
	public float maxPlaceDistance = 5f;

	// Token: 0x04000A13 RID: 2579
	public float lengthPerAmount = 0.5f;
}
