using System;
using UnityEngine;

// Token: 0x0200044B RID: 1099
public class Buoyancy : ListComponent<Buoyancy>, IServerComponent
{
	// Token: 0x170002BE RID: 702
	// (get) Token: 0x0600240A RID: 9226 RVA: 0x000E350A File Offset: 0x000E170A
	// (set) Token: 0x0600240B RID: 9227 RVA: 0x000E3512 File Offset: 0x000E1712
	public float timeOutOfWater { get; private set; }

	// Token: 0x0600240C RID: 9228 RVA: 0x000E351B File Offset: 0x000E171B
	public static string DefaultWaterImpact()
	{
		return "assets/bundled/prefabs/fx/impacts/physics/water-enter-exit.prefab";
	}

	// Token: 0x0600240D RID: 9229 RVA: 0x000E3522 File Offset: 0x000E1722
	private void Awake()
	{
		base.InvokeRandomized(new Action(this.CheckSleepState), 0.5f, 5f, 1f);
	}

	// Token: 0x0600240E RID: 9230 RVA: 0x000E3545 File Offset: 0x000E1745
	public void Sleep()
	{
		if (this.rigidBody != null)
		{
			this.rigidBody.Sleep();
		}
		base.enabled = false;
	}

	// Token: 0x0600240F RID: 9231 RVA: 0x000E3567 File Offset: 0x000E1767
	public void Wake()
	{
		if (this.rigidBody != null)
		{
			this.rigidBody.WakeUp();
		}
		base.enabled = true;
	}

	// Token: 0x06002410 RID: 9232 RVA: 0x000E358C File Offset: 0x000E178C
	public void CheckSleepState()
	{
		if (base.transform == null)
		{
			return;
		}
		if (this.rigidBody == null)
		{
			return;
		}
		bool flag = BaseNetworkable.HasCloseConnections(base.transform.position, 100f);
		if (base.enabled && (this.rigidBody.IsSleeping() || (!flag && this.timeInWater > 6f)))
		{
			base.Invoke(new Action(this.Sleep), 0f);
			return;
		}
		if (!base.enabled && (!this.rigidBody.IsSleeping() || (flag && this.timeInWater > 0f)))
		{
			base.Invoke(new Action(this.Wake), 0f);
		}
	}

	// Token: 0x06002411 RID: 9233 RVA: 0x000E3648 File Offset: 0x000E1848
	protected void DoCycle()
	{
		bool flag = this.submergedFraction > 0f;
		this.BuoyancyFixedUpdate();
		bool flag2 = this.submergedFraction > 0f;
		if (flag != flag2)
		{
			if (this.useUnderwaterDrag && this.rigidBody != null)
			{
				if (flag2)
				{
					this.defaultDrag = this.rigidBody.drag;
					this.defaultAngularDrag = this.rigidBody.angularDrag;
					this.rigidBody.drag = this.underwaterDrag;
					this.rigidBody.angularDrag = this.underwaterDrag;
				}
				else
				{
					this.rigidBody.drag = this.defaultDrag;
					this.rigidBody.angularDrag = this.defaultAngularDrag;
				}
			}
			if (this.SubmergedChanged != null)
			{
				this.SubmergedChanged(flag2);
			}
		}
	}

	// Token: 0x06002412 RID: 9234 RVA: 0x000E3714 File Offset: 0x000E1914
	public static void Cycle()
	{
		Buoyancy[] buffer = ListComponent<Buoyancy>.InstanceList.Values.Buffer;
		int count = ListComponent<Buoyancy>.InstanceList.Count;
		for (int i = 0; i < count; i++)
		{
			buffer[i].DoCycle();
		}
	}

	// Token: 0x06002413 RID: 9235 RVA: 0x000E3750 File Offset: 0x000E1950
	public Vector3 GetFlowDirection(Vector2 posUV)
	{
		if (TerrainMeta.WaterMap == null)
		{
			return Vector3.zero;
		}
		Vector3 normalFast = TerrainMeta.WaterMap.GetNormalFast(posUV);
		float scale = Mathf.Clamp01(Mathf.Abs(normalFast.y));
		normalFast.y = 0f;
		normalFast.FastRenormalize(scale);
		return normalFast;
	}

	// Token: 0x06002414 RID: 9236 RVA: 0x000E37A4 File Offset: 0x000E19A4
	public void EnsurePointsInitialized()
	{
		if (this.points == null || this.points.Length == 0)
		{
			Rigidbody component = base.GetComponent<Rigidbody>();
			if (component != null)
			{
				BuoyancyPoint buoyancyPoint = new GameObject("BuoyancyPoint")
				{
					transform = 
					{
						parent = component.gameObject.transform,
						localPosition = component.centerOfMass
					}
				}.AddComponent<BuoyancyPoint>();
				buoyancyPoint.buoyancyForce = component.mass * -Physics.gravity.y;
				buoyancyPoint.buoyancyForce *= 1.32f;
				buoyancyPoint.size = 0.2f;
				this.points = new BuoyancyPoint[1];
				this.points[0] = buoyancyPoint;
			}
		}
		if (this.pointData == null || this.pointData.Length != this.points.Length)
		{
			this.pointData = new Buoyancy.BuoyancyPointData[this.points.Length];
			this.pointPositionArray = new Vector2[this.points.Length];
			this.pointPositionUVArray = new Vector2[this.points.Length];
			this.pointShoreVectorArray = new Vector3[this.points.Length];
			this.pointTerrainHeightArray = new float[this.points.Length];
			this.pointWaterHeightArray = new float[this.points.Length];
			for (int i = 0; i < this.points.Length; i++)
			{
				Transform transform = this.points[i].transform;
				Transform parent = transform.parent;
				transform.SetParent(base.transform);
				Vector3 localPosition = transform.localPosition;
				transform.SetParent(parent);
				this.pointData[i].transform = transform;
				this.pointData[i].localPosition = transform.localPosition;
				this.pointData[i].rootToPoint = localPosition;
			}
		}
	}

	// Token: 0x06002415 RID: 9237 RVA: 0x000E396C File Offset: 0x000E1B6C
	public void BuoyancyFixedUpdate()
	{
		if (TerrainMeta.WaterMap == null)
		{
			return;
		}
		this.EnsurePointsInitialized();
		if (this.rigidBody == null)
		{
			return;
		}
		if (this.buoyancyScale == 0f)
		{
			base.Invoke(new Action(this.Sleep), 0f);
			return;
		}
		float time = Time.time;
		float x = TerrainMeta.Position.x;
		float z = TerrainMeta.Position.z;
		float x2 = TerrainMeta.OneOverSize.x;
		float z2 = TerrainMeta.OneOverSize.z;
		Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
		for (int i = 0; i < this.pointData.Length; i++)
		{
			BuoyancyPoint buoyancyPoint = this.points[i];
			Vector3 vector = localToWorldMatrix.MultiplyPoint3x4(this.pointData[i].rootToPoint);
			this.pointData[i].position = vector;
			float x3 = (vector.x - x) * x2;
			float y = (vector.z - z) * z2;
			this.pointPositionArray[i] = new Vector2(vector.x, vector.z);
			this.pointPositionUVArray[i] = new Vector2(x3, y);
		}
		WaterSystem.GetHeightArray(this.pointPositionArray, this.pointPositionUVArray, this.pointShoreVectorArray, this.pointTerrainHeightArray, this.pointWaterHeightArray);
		int num = 0;
		for (int j = 0; j < this.points.Length; j++)
		{
			BuoyancyPoint buoyancyPoint2 = this.points[j];
			Vector3 position = this.pointData[j].position;
			Vector3 localPosition = this.pointData[j].localPosition;
			Vector2 posUV = this.pointPositionUVArray[j];
			float terrainHeight = this.pointTerrainHeightArray[j];
			float waterHeight = this.pointWaterHeightArray[j];
			if (this.ArtificialHeight != null)
			{
				waterHeight = this.ArtificialHeight.Value;
			}
			bool doDeepwaterChecks = this.ArtificialHeight == null;
			WaterLevel.WaterInfo buoyancyWaterInfo = WaterLevel.GetBuoyancyWaterInfo(position, posUV, terrainHeight, waterHeight, doDeepwaterChecks, this.forEntity);
			bool flag = false;
			if (position.y < buoyancyWaterInfo.surfaceLevel && buoyancyWaterInfo.isValid)
			{
				flag = true;
				num++;
				float currentDepth = buoyancyWaterInfo.currentDepth;
				float num2 = Mathf.InverseLerp(0f, buoyancyPoint2.size, currentDepth);
				float num3 = 1f + Mathf.PerlinNoise(buoyancyPoint2.randomOffset + time * buoyancyPoint2.waveFrequency, 0f) * buoyancyPoint2.waveScale;
				float num4 = buoyancyPoint2.buoyancyForce * this.buoyancyScale;
				Vector3 force = new Vector3(0f, num3 * num2 * num4, 0f);
				Vector3 flowDirection = this.GetFlowDirection(posUV);
				if (flowDirection.y < 0.9999f && flowDirection != Vector3.up)
				{
					num4 *= 0.25f;
					force.x += flowDirection.x * num4 * this.flowMovementScale;
					force.y += flowDirection.y * num4 * this.flowMovementScale;
					force.z += flowDirection.z * num4 * this.flowMovementScale;
				}
				this.rigidBody.AddForceAtPosition(force, position, ForceMode.Force);
			}
			if (buoyancyPoint2.doSplashEffects && ((!buoyancyPoint2.wasSubmergedLastFrame && flag) || (!flag && buoyancyPoint2.wasSubmergedLastFrame)) && this.doEffects && this.rigidBody.GetRelativePointVelocity(localPosition).magnitude > 1f)
			{
				string strName = (this.waterImpacts != null && this.waterImpacts.Length != 0 && this.waterImpacts[0].isValid) ? this.waterImpacts[0].resourcePath : Buoyancy.DefaultWaterImpact();
				Vector3 b = new Vector3(UnityEngine.Random.Range(-0.25f, 0.25f), 0f, UnityEngine.Random.Range(-0.25f, 0.25f));
				Effect.server.Run(strName, position + b, Vector3.up, null, false);
				buoyancyPoint2.nexSplashTime = Time.time + 0.25f;
			}
			buoyancyPoint2.wasSubmergedLastFrame = flag;
		}
		if (this.points.Length != 0)
		{
			this.submergedFraction = (float)num / (float)this.points.Length;
		}
		if (this.submergedFraction > this.requiredSubmergedFraction)
		{
			this.timeInWater += Time.fixedDeltaTime;
			this.timeOutOfWater = 0f;
			return;
		}
		this.timeOutOfWater += Time.fixedDeltaTime;
		this.timeInWater = 0f;
	}

	// Token: 0x04001C91 RID: 7313
	public BuoyancyPoint[] points;

	// Token: 0x04001C92 RID: 7314
	public GameObjectRef[] waterImpacts;

	// Token: 0x04001C93 RID: 7315
	public Rigidbody rigidBody;

	// Token: 0x04001C94 RID: 7316
	public float buoyancyScale = 1f;

	// Token: 0x04001C95 RID: 7317
	public bool doEffects = true;

	// Token: 0x04001C96 RID: 7318
	public float flowMovementScale = 1f;

	// Token: 0x04001C97 RID: 7319
	public float requiredSubmergedFraction;

	// Token: 0x04001C98 RID: 7320
	public bool useUnderwaterDrag;

	// Token: 0x04001C99 RID: 7321
	[Range(0f, 3f)]
	public float underwaterDrag = 2f;

	// Token: 0x04001C9B RID: 7323
	public Action<bool> SubmergedChanged;

	// Token: 0x04001C9C RID: 7324
	public BaseEntity forEntity;

	// Token: 0x04001C9D RID: 7325
	[NonSerialized]
	public float submergedFraction;

	// Token: 0x04001C9E RID: 7326
	private Buoyancy.BuoyancyPointData[] pointData;

	// Token: 0x04001C9F RID: 7327
	private Vector2[] pointPositionArray;

	// Token: 0x04001CA0 RID: 7328
	private Vector2[] pointPositionUVArray;

	// Token: 0x04001CA1 RID: 7329
	private Vector3[] pointShoreVectorArray;

	// Token: 0x04001CA2 RID: 7330
	private float[] pointTerrainHeightArray;

	// Token: 0x04001CA3 RID: 7331
	private float[] pointWaterHeightArray;

	// Token: 0x04001CA4 RID: 7332
	private float defaultDrag;

	// Token: 0x04001CA5 RID: 7333
	private float defaultAngularDrag;

	// Token: 0x04001CA6 RID: 7334
	private float timeInWater;

	// Token: 0x04001CA7 RID: 7335
	public float? ArtificialHeight;

	// Token: 0x04001CA8 RID: 7336
	public float waveHeightScale = 0.5f;

	// Token: 0x02000CA2 RID: 3234
	private struct BuoyancyPointData
	{
		// Token: 0x04004349 RID: 17225
		public Transform transform;

		// Token: 0x0400434A RID: 17226
		public Vector3 localPosition;

		// Token: 0x0400434B RID: 17227
		public Vector3 rootToPoint;

		// Token: 0x0400434C RID: 17228
		public Vector3 position;
	}
}
