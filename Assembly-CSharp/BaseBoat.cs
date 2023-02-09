using System;
using System.Collections.Generic;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x0200044A RID: 1098
public class BaseBoat : BaseVehicle
{
	// Token: 0x060023F9 RID: 9209 RVA: 0x000E2E9C File Offset: 0x000E109C
	public bool InDryDock()
	{
		return base.GetParentEntity() != null;
	}

	// Token: 0x060023FA RID: 9210 RVA: 0x000E2EAA File Offset: 0x000E10AA
	public override float MaxVelocity()
	{
		return 25f;
	}

	// Token: 0x060023FB RID: 9211 RVA: 0x000E2EB4 File Offset: 0x000E10B4
	public override void ServerInit()
	{
		base.ServerInit();
		this.rigidBody.isKinematic = false;
		if (this.rigidBody == null)
		{
			Debug.LogWarning("Boat rigidbody null");
			return;
		}
		if (this.centerOfMass == null)
		{
			Debug.LogWarning("boat COM null");
			return;
		}
		this.rigidBody.centerOfMass = this.centerOfMass.localPosition;
	}

	// Token: 0x060023FC RID: 9212 RVA: 0x000E2F1B File Offset: 0x000E111B
	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		if (base.IsDriver(player))
		{
			this.DriverInput(inputState, player);
		}
	}

	// Token: 0x060023FD RID: 9213 RVA: 0x000E2F30 File Offset: 0x000E1130
	public virtual void DriverInput(InputState inputState, BasePlayer player)
	{
		if (inputState.IsDown(BUTTON.FORWARD))
		{
			this.gasPedal = 1f;
		}
		else if (inputState.IsDown(BUTTON.BACKWARD))
		{
			this.gasPedal = -0.5f;
		}
		else
		{
			this.gasPedal = 0f;
		}
		if (inputState.IsDown(BUTTON.LEFT))
		{
			this.steering = 1f;
			return;
		}
		if (inputState.IsDown(BUTTON.RIGHT))
		{
			this.steering = -1f;
			return;
		}
		this.steering = 0f;
	}

	// Token: 0x060023FE RID: 9214 RVA: 0x00026D90 File Offset: 0x00024F90
	public void OnPoolDestroyed()
	{
		base.Kill(BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x060023FF RID: 9215 RVA: 0x0004A35B File Offset: 0x0004855B
	public void WakeUp()
	{
		if (this.rigidBody != null)
		{
			this.rigidBody.WakeUp();
			this.rigidBody.AddForce(Vector3.up * 0.1f, ForceMode.Impulse);
		}
	}

	// Token: 0x06002400 RID: 9216 RVA: 0x000E2FAA File Offset: 0x000E11AA
	protected override void OnServerWake()
	{
		if (this.buoyancy != null)
		{
			this.buoyancy.Wake();
		}
	}

	// Token: 0x06002401 RID: 9217 RVA: 0x000E2FC5 File Offset: 0x000E11C5
	public virtual bool EngineOn()
	{
		return base.HasDriver() && !base.IsFlipped();
	}

	// Token: 0x06002402 RID: 9218 RVA: 0x000E2FDC File Offset: 0x000E11DC
	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		if (!this.EngineOn())
		{
			this.gasPedal = 0f;
			this.steering = 0f;
		}
		base.VehicleFixedUpdate();
		bool flag = WaterLevel.Test(this.thrustPoint.position, true, this);
		if (this.gasPedal != 0f && flag && this.buoyancy.submergedFraction > 0.3f)
		{
			Vector3 force = (base.transform.forward + base.transform.right * this.steering * this.steeringScale).normalized * this.gasPedal * this.engineThrust;
			this.rigidBody.AddForceAtPosition(force, this.thrustPoint.position, ForceMode.Force);
		}
		if (this.AnyMounted() && base.IsFlipped())
		{
			this.DismountAllPlayers();
		}
	}

	// Token: 0x06002403 RID: 9219 RVA: 0x000E30CC File Offset: 0x000E12CC
	public static void WaterVehicleDecay(BaseCombatEntity entity, float decayTickRate, float timeSinceLastUsed, float outsideDecayMinutes, float deepWaterDecayMinutes)
	{
		if (entity.healthFraction == 0f)
		{
			return;
		}
		if (timeSinceLastUsed < 2700f)
		{
			return;
		}
		float overallWaterDepth = WaterLevel.GetOverallWaterDepth(entity.transform.position, true, null, false);
		float num = entity.IsOutside() ? outsideDecayMinutes : float.PositiveInfinity;
		if (overallWaterDepth > 12f)
		{
			float t = Mathf.InverseLerp(12f, 16f, overallWaterDepth);
			float num2 = Mathf.Lerp(0.1f, 1f, t);
			num = Mathf.Min(num, deepWaterDecayMinutes / num2);
		}
		if (!float.IsPositiveInfinity(num))
		{
			float num3 = decayTickRate / 60f / num;
			entity.Hurt(entity.MaxHealth() * num3, DamageType.Decay, entity, false);
		}
	}

	// Token: 0x06002404 RID: 9220 RVA: 0x000E3170 File Offset: 0x000E1370
	public virtual bool EngineInWater()
	{
		return TerrainMeta.WaterMap.GetHeight(this.thrustPoint.position) > this.thrustPoint.position.y;
	}

	// Token: 0x06002405 RID: 9221 RVA: 0x000E3199 File Offset: 0x000E1399
	public override float WaterFactorForPlayer(BasePlayer player)
	{
		if (TerrainMeta.WaterMap.GetHeight(player.eyes.position) >= player.eyes.position.y)
		{
			return 1f;
		}
		return 0f;
	}

	// Token: 0x06002406 RID: 9222 RVA: 0x000E31D4 File Offset: 0x000E13D4
	public static float GetWaterDepth(Vector3 pos)
	{
		if (UnityEngine.Application.isPlaying && !(TerrainMeta.WaterMap == null))
		{
			return TerrainMeta.WaterMap.GetDepth(pos);
		}
		RaycastHit raycastHit;
		if (!UnityEngine.Physics.Raycast(pos, Vector3.down, out raycastHit, 100f, 8388608))
		{
			return 100f;
		}
		return raycastHit.distance;
	}

	// Token: 0x06002407 RID: 9223 RVA: 0x000E3228 File Offset: 0x000E1428
	public static List<Vector3> GenerateOceanPatrolPath(float minDistanceFromShore = 50f, float minWaterDepth = 8f)
	{
		float x = TerrainMeta.Size.x;
		float num = x * 2f * 3.1415927f;
		float num2 = 30f;
		int num3 = Mathf.CeilToInt(num / num2);
		List<Vector3> list = new List<Vector3>();
		float num4 = x;
		float y = 0f;
		for (int i = 0; i < num3; i++)
		{
			float num5 = (float)i / (float)num3 * 360f;
			list.Add(new Vector3(Mathf.Sin(num5 * 0.017453292f) * num4, y, Mathf.Cos(num5 * 0.017453292f) * num4));
		}
		float d = 4f;
		float num6 = 200f;
		bool flag = true;
		int num7 = 0;
		while (num7 < AI.ocean_patrol_path_iterations && flag)
		{
			flag = false;
			for (int j = 0; j < num3; j++)
			{
				Vector3 vector = list[j];
				int index = (j == 0) ? (num3 - 1) : (j - 1);
				int index2 = (j == num3 - 1) ? 0 : (j + 1);
				Vector3 b = list[index2];
				Vector3 b2 = list[index];
				Vector3 origin = vector;
				Vector3 normalized = (Vector3.zero - vector).normalized;
				Vector3 vector2 = vector + normalized * d;
				if (Vector3.Distance(vector2, b) <= num6 && Vector3.Distance(vector2, b2) <= num6)
				{
					bool flag2 = true;
					int num8 = 16;
					for (int k = 0; k < num8; k++)
					{
						float num9 = (float)k / (float)num8 * 360f;
						Vector3 normalized2 = new Vector3(Mathf.Sin(num9 * 0.017453292f), y, Mathf.Cos(num9 * 0.017453292f)).normalized;
						Vector3 vector3 = vector2 + normalized2 * 1f;
						BaseBoat.GetWaterDepth(vector3);
						Vector3 direction = normalized;
						if (vector3 != Vector3.zero)
						{
							direction = (vector3 - vector2).normalized;
						}
						RaycastHit raycastHit;
						if (UnityEngine.Physics.SphereCast(origin, 3f, direction, out raycastHit, minDistanceFromShore, 1218511105))
						{
							flag2 = false;
							break;
						}
					}
					if (flag2)
					{
						flag = true;
						list[j] = vector2;
					}
				}
			}
			num7++;
		}
		if (flag)
		{
			Debug.LogWarning("Failed to generate ocean patrol path");
			return null;
		}
		List<int> list2 = new List<int>();
		LineUtility.Simplify(list, 5f, list2);
		List<Vector3> list3 = list;
		list = new List<Vector3>();
		foreach (int index3 in list2)
		{
			list.Add(list3[index3]);
		}
		Debug.Log("Generated ocean patrol path with node count: " + list.Count);
		return list;
	}

	// Token: 0x04001C89 RID: 7305
	public float engineThrust = 10f;

	// Token: 0x04001C8A RID: 7306
	public float steeringScale = 0.1f;

	// Token: 0x04001C8B RID: 7307
	public float gasPedal;

	// Token: 0x04001C8C RID: 7308
	public float steering;

	// Token: 0x04001C8D RID: 7309
	public Transform thrustPoint;

	// Token: 0x04001C8E RID: 7310
	public Transform centerOfMass;

	// Token: 0x04001C8F RID: 7311
	public Buoyancy buoyancy;

	// Token: 0x04001C90 RID: 7312
	[ServerVar]
	public static bool generate_paths = true;
}
