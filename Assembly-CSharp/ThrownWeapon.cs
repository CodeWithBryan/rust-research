﻿using System;
using ConVar;
using Network;
using Rust;
using Rust.Ai;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000D8 RID: 216
public class ThrownWeapon : AttackEntity
{
	// Token: 0x060012BC RID: 4796 RVA: 0x00095E6C File Offset: 0x0009406C
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ThrownWeapon.OnRpcMessage", 0))
		{
			if (rpc == 1513023343U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DoDrop ");
				}
				using (TimeWarning.New("DoDrop", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(1513023343U, "DoDrop", this, player))
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
							this.DoDrop(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in DoDrop");
					}
				}
				return true;
			}
			if (rpc == 1974840882U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - DoThrow ");
				}
				using (TimeWarning.New("DoThrow", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test(1974840882U, "DoThrow", this, player))
						{
							return true;
						}
					}
					try
					{
						using (TimeWarning.New("Call", 0))
						{
							BaseEntity.RPCMessage msg3 = new BaseEntity.RPCMessage
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.DoThrow(msg3);
						}
					}
					catch (Exception exception2)
					{
						Debug.LogException(exception2);
						player.Kick("RPC Error in DoThrow");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x060012BD RID: 4797 RVA: 0x000303F4 File Offset: 0x0002E5F4
	public override Vector3 GetInheritedVelocity(BasePlayer player, Vector3 direction)
	{
		return player.GetInheritedThrowVelocity(direction);
	}

	// Token: 0x060012BE RID: 4798 RVA: 0x00096164 File Offset: 0x00094364
	public void ServerThrow(Vector3 targetPosition)
	{
		if (base.isClient)
		{
			return;
		}
		if (!base.HasItemAmount() || base.HasAttackCooldown())
		{
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return;
		}
		if (!this.canThrowUnderwater && ownerPlayer.IsHeadUnderwater())
		{
			return;
		}
		Vector3 position = ownerPlayer.eyes.position;
		Vector3 a = ownerPlayer.eyes.BodyForward();
		float d = 1f;
		base.SignalBroadcast(BaseEntity.Signal.Throw, string.Empty, null);
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.prefabToThrow.resourcePath, position, Quaternion.LookRotation((this.overrideAngle == Vector3.zero) ? (-a) : this.overrideAngle), true);
		if (baseEntity == null)
		{
			return;
		}
		baseEntity.creatorEntity = ownerPlayer;
		Vector3 vector = a + Quaternion.AngleAxis(10f, Vector3.right) * Vector3.up;
		float num = this.GetThrowVelocity(position, targetPosition, vector);
		if (float.IsNaN(num))
		{
			vector = a + Quaternion.AngleAxis(20f, Vector3.right) * Vector3.up;
			num = this.GetThrowVelocity(position, targetPosition, vector);
			if (float.IsNaN(num))
			{
				num = 5f;
			}
		}
		baseEntity.SetVelocity(vector * num * d);
		if (this.tumbleVelocity > 0f)
		{
			baseEntity.SetAngularVelocity(new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)) * this.tumbleVelocity);
		}
		baseEntity.Spawn();
		base.StartAttackCooldown(this.repeatDelay);
		base.UseItemAmount(1);
		TimedExplosive timedExplosive = baseEntity as TimedExplosive;
		if (timedExplosive != null)
		{
			float num2 = 0f;
			foreach (DamageTypeEntry damageTypeEntry in timedExplosive.damageTypes)
			{
				num2 += damageTypeEntry.amount;
			}
			Sense.Stimulate(new Sensation
			{
				Type = SensationType.ThrownWeapon,
				Position = ownerPlayer.transform.position,
				Radius = 50f,
				DamagePotential = num2,
				InitiatorPlayer = ownerPlayer,
				Initiator = ownerPlayer,
				UsedEntity = timedExplosive
			});
			return;
		}
		Sense.Stimulate(new Sensation
		{
			Type = SensationType.ThrownWeapon,
			Position = ownerPlayer.transform.position,
			Radius = 50f,
			DamagePotential = 0f,
			InitiatorPlayer = ownerPlayer,
			Initiator = ownerPlayer,
			UsedEntity = this
		});
	}

	// Token: 0x060012BF RID: 4799 RVA: 0x0009643C File Offset: 0x0009463C
	private float GetThrowVelocity(Vector3 throwPos, Vector3 targetPos, Vector3 aimDir)
	{
		Vector3 vector = targetPos - throwPos;
		float magnitude = new Vector2(vector.x, vector.z).magnitude;
		float y = vector.y;
		float magnitude2 = new Vector2(aimDir.x, aimDir.z).magnitude;
		float y2 = aimDir.y;
		float y3 = UnityEngine.Physics.gravity.y;
		return Mathf.Sqrt(0.5f * y3 * magnitude * magnitude / (magnitude2 * (magnitude2 * y - y2 * magnitude)));
	}

	// Token: 0x060012C0 RID: 4800 RVA: 0x000964C0 File Offset: 0x000946C0
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	private void DoThrow(BaseEntity.RPCMessage msg)
	{
		if (!base.HasItemAmount() || base.HasAttackCooldown())
		{
			return;
		}
		Vector3 vector = msg.read.Vector3();
		Vector3 normalized = msg.read.Vector3().normalized;
		float d = Mathf.Clamp01(msg.read.Float());
		if (msg.player.isMounted || msg.player.HasParent())
		{
			vector = msg.player.eyes.position;
		}
		else if (!base.ValidateEyePos(msg.player, vector))
		{
			return;
		}
		if (!this.canThrowUnderwater && msg.player.IsHeadUnderwater())
		{
			return;
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.prefabToThrow.resourcePath, vector, Quaternion.LookRotation((this.overrideAngle == Vector3.zero) ? (-normalized) : this.overrideAngle), true);
		if (baseEntity == null)
		{
			return;
		}
		baseEntity.creatorEntity = msg.player;
		baseEntity.skinID = this.skinID;
		baseEntity.SetVelocity(this.GetInheritedVelocity(msg.player, normalized) + normalized * this.maxThrowVelocity * d + msg.player.estimatedVelocity * 0.5f);
		if (this.tumbleVelocity > 0f)
		{
			baseEntity.SetAngularVelocity(new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)) * this.tumbleVelocity);
		}
		baseEntity.Spawn();
		this.SetUpThrownWeapon(baseEntity);
		base.StartAttackCooldown(this.repeatDelay);
		base.UseItemAmount(1);
		BasePlayer player = msg.player;
		if (player != null)
		{
			TimedExplosive timedExplosive = baseEntity as TimedExplosive;
			if (timedExplosive != null)
			{
				float num = 0f;
				foreach (DamageTypeEntry damageTypeEntry in timedExplosive.damageTypes)
				{
					num += damageTypeEntry.amount;
				}
				Sense.Stimulate(new Sensation
				{
					Type = SensationType.ThrownWeapon,
					Position = player.transform.position,
					Radius = 50f,
					DamagePotential = num,
					InitiatorPlayer = player,
					Initiator = player,
					UsedEntity = timedExplosive
				});
				return;
			}
			Sense.Stimulate(new Sensation
			{
				Type = SensationType.ThrownWeapon,
				Position = player.transform.position,
				Radius = 50f,
				DamagePotential = 0f,
				InitiatorPlayer = player,
				Initiator = player,
				UsedEntity = this
			});
		}
	}

	// Token: 0x060012C1 RID: 4801 RVA: 0x000967B0 File Offset: 0x000949B0
	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.IsActiveItem]
	private void DoDrop(BaseEntity.RPCMessage msg)
	{
		if (!base.HasItemAmount() || base.HasAttackCooldown())
		{
			return;
		}
		if (!this.canThrowUnderwater && msg.player.IsHeadUnderwater())
		{
			return;
		}
		Vector3 vector = msg.read.Vector3();
		Vector3 normalized = msg.read.Vector3().normalized;
		if (msg.player.isMounted || msg.player.HasParent())
		{
			vector = msg.player.eyes.position;
		}
		else if (!base.ValidateEyePos(msg.player, vector))
		{
			return;
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.prefabToThrow.resourcePath, vector, Quaternion.LookRotation(Vector3.up), true);
		if (baseEntity == null)
		{
			return;
		}
		RaycastHit hit;
		if (this.canStick && UnityEngine.Physics.SphereCast(new Ray(vector, normalized), 0.05f, out hit, 1.5f, 1236478737))
		{
			Vector3 point = hit.point;
			Vector3 normal = hit.normal;
			BaseEntity baseEntity2 = hit.GetEntity();
			Collider collider = hit.collider;
			if (baseEntity2 && baseEntity2 is StabilityEntity && baseEntity is TimedExplosive)
			{
				baseEntity2 = baseEntity2.ToServer<BaseEntity>();
				TimedExplosive timedExplosive = baseEntity as TimedExplosive;
				timedExplosive.onlyDamageParent = true;
				timedExplosive.DoStick(point, normal, baseEntity2, collider);
			}
			else
			{
				baseEntity.SetVelocity(normalized);
			}
		}
		else
		{
			baseEntity.SetVelocity(normalized);
		}
		baseEntity.creatorEntity = msg.player;
		baseEntity.skinID = this.skinID;
		baseEntity.Spawn();
		this.SetUpThrownWeapon(baseEntity);
		base.StartAttackCooldown(this.repeatDelay);
		base.UseItemAmount(1);
	}

	// Token: 0x060012C2 RID: 4802 RVA: 0x000059DD File Offset: 0x00003BDD
	protected virtual void SetUpThrownWeapon(BaseEntity ent)
	{
	}

	// Token: 0x04000BAC RID: 2988
	[Header("Throw Weapon")]
	public GameObjectRef prefabToThrow;

	// Token: 0x04000BAD RID: 2989
	public float maxThrowVelocity = 10f;

	// Token: 0x04000BAE RID: 2990
	public float tumbleVelocity;

	// Token: 0x04000BAF RID: 2991
	public Vector3 overrideAngle = Vector3.zero;

	// Token: 0x04000BB0 RID: 2992
	public bool canStick = true;

	// Token: 0x04000BB1 RID: 2993
	public bool canThrowUnderwater = true;
}
