using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

// Token: 0x020000ED RID: 237
public class ZiplineLaunchPoint : global::BaseEntity
{
	// Token: 0x0600147D RID: 5245 RVA: 0x000A1D18 File Offset: 0x0009FF18
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("ZiplineLaunchPoint.OnRpcMessage", 0))
		{
			if (rpc == 2256922575U && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log("SV_RPCMessage: " + player + " - MountPlayer ");
				}
				using (TimeWarning.New("MountPlayer", 0))
				{
					using (TimeWarning.New("Conditions", 0))
					{
						if (!global::BaseEntity.RPC_Server.CallsPerSecond.Test(2256922575U, "MountPlayer", this, player, 2UL))
						{
							return true;
						}
						if (!global::BaseEntity.RPC_Server.IsVisible.Test(2256922575U, "MountPlayer", this, player, 3f))
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
							this.MountPlayer(msg2);
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						player.Kick("RPC Error in MountPlayer");
					}
				}
				return true;
			}
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600147E RID: 5246 RVA: 0x000A1ED8 File Offset: 0x000A00D8
	public override void ResetState()
	{
		base.ResetState();
		this.ziplineTargets.Clear();
		this.linePoints = null;
	}

	// Token: 0x0600147F RID: 5247 RVA: 0x000A1EF4 File Offset: 0x000A00F4
	public override void PostMapEntitySpawn()
	{
		base.PostMapEntitySpawn();
		this.FindZiplineTarget(ref this.ziplineTargets);
		this.CalculateZiplinePoints(this.ziplineTargets, ref this.linePoints);
		if (this.ziplineTargets.Count == 0)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
			return;
		}
		if (Vector3.Distance(this.linePoints[0], this.linePoints[this.linePoints.Count - 1]) > 100f && this.ArrivalPointRef != null && this.ArrivalPointRef.isValid)
		{
			global::ZiplineArrivalPoint ziplineArrivalPoint = base.gameManager.CreateEntity(this.ArrivalPointRef.resourcePath, this.linePoints[this.linePoints.Count - 1], default(Quaternion), true) as global::ZiplineArrivalPoint;
			ziplineArrivalPoint.SetPositions(this.linePoints);
			ziplineArrivalPoint.Spawn();
		}
		this.UpdateBuildingBlocks();
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06001480 RID: 5248 RVA: 0x000A1FDC File Offset: 0x000A01DC
	private void FindZiplineTarget(ref List<Vector3> foundPositions)
	{
		foundPositions.Clear();
		Vector3 position = this.LineDeparturePoint.position;
		List<ZiplineTarget> list = Facepunch.Pool.GetList<ZiplineTarget>();
		GamePhysics.OverlapSphere<ZiplineTarget>(position + base.transform.forward * 200f, 200f, list, 1218511105, QueryTriggerInteraction.Ignore);
		float num = float.MaxValue;
		float num2 = 3f;
		foreach (ZiplineTarget ziplineTarget in list)
		{
			if (!ziplineTarget.IsChainPoint)
			{
				Vector3 position2 = ziplineTarget.transform.position;
				float num3 = Vector3.Dot((position2.WithY(position.y) - position).normalized, base.transform.forward);
				float num4 = Vector3.Distance(position, position2);
				if (num3 > 0.2f && ziplineTarget.IsValidPosition(position) && position.y + num2 > position2.y && num4 > 10f && num4 < num)
				{
					if (this.CheckLineOfSight(position, position2))
					{
						num = num4;
						ZiplineTarget ziplineTarget2 = ziplineTarget;
						foundPositions.Clear();
						foundPositions.Add(ziplineTarget2.transform.position);
					}
					else
					{
						foreach (ZiplineTarget ziplineTarget3 in list)
						{
							if (ziplineTarget3.IsChainPoint && ziplineTarget3.IsValidChainPoint(position, position2))
							{
								bool flag = this.CheckLineOfSight(position, ziplineTarget3.transform.position);
								bool flag2 = this.CheckLineOfSight(ziplineTarget3.transform.position, position2);
								if (flag && flag2)
								{
									num = num4;
									ZiplineTarget ziplineTarget2 = ziplineTarget;
									foundPositions.Clear();
									foundPositions.Add(ziplineTarget3.transform.position);
									foundPositions.Add(ziplineTarget2.transform.position);
								}
								else if (flag)
								{
									foreach (ZiplineTarget ziplineTarget4 in list)
									{
										if (!(ziplineTarget4 == ziplineTarget3) && ziplineTarget4.IsValidChainPoint(ziplineTarget3.Target.position, ziplineTarget.Target.position))
										{
											bool flag3 = this.CheckLineOfSight(ziplineTarget3.transform.position, ziplineTarget4.transform.position);
											bool flag4 = this.CheckLineOfSight(ziplineTarget4.transform.position, ziplineTarget.transform.position);
											if (flag3 && flag4)
											{
												num = num4;
												ZiplineTarget ziplineTarget2 = ziplineTarget;
												foundPositions.Clear();
												foundPositions.Add(ziplineTarget3.transform.position);
												foundPositions.Add(ziplineTarget4.transform.position);
												foundPositions.Add(ziplineTarget2.transform.position);
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06001481 RID: 5249 RVA: 0x000A2324 File Offset: 0x000A0524
	private bool CheckLineOfSight(Vector3 from, Vector3 to)
	{
		Vector3 vector = this.CalculateLineMidPoint(from, to) - Vector3.up * 0.5f;
		return GamePhysics.LineOfSightRadius(from, to, 1218511105, 0.5f, 2f, null) && GamePhysics.LineOfSightRadius(from, vector, 1218511105, 0.5f, 2f, null) && GamePhysics.LineOfSightRadius(vector, to, 1218511105, 0.5f, 2f, null);
	}

	// Token: 0x06001482 RID: 5250 RVA: 0x000A239C File Offset: 0x000A059C
	[global::BaseEntity.RPC_Server]
	[global::BaseEntity.RPC_Server.IsVisible(3f)]
	[global::BaseEntity.RPC_Server.CallsPerSecond(2UL)]
	private void MountPlayer(global::BaseEntity.RPCMessage msg)
	{
		if (base.IsBusy())
		{
			return;
		}
		if (msg.player == null)
		{
			return;
		}
		if (msg.player.Distance(this.LineDeparturePoint.position) > 3f)
		{
			return;
		}
		if (!this.IsPlayerFacingValidDirection(msg.player))
		{
			return;
		}
		if (this.ziplineTargets.Count == 0)
		{
			return;
		}
		Vector3 position = this.LineDeparturePoint.position;
		Quaternion lineStartRot = Quaternion.LookRotation((this.ziplineTargets[0].WithY(position.y) - position).normalized);
		Quaternion rot = Quaternion.LookRotation((position - msg.player.transform.position.WithY(position.y)).normalized);
		global::ZiplineMountable ziplineMountable = base.gameManager.CreateEntity(this.MountableRef.resourcePath, msg.player.transform.position + Vector3.up * 2.1f, rot, true) as global::ZiplineMountable;
		if (ziplineMountable != null)
		{
			this.CalculateZiplinePoints(this.ziplineTargets, ref this.linePoints);
			ziplineMountable.SetDestination(this.linePoints, position, lineStartRot);
			ziplineMountable.Spawn();
			ziplineMountable.MountPlayer(msg.player);
			if (msg.player.GetMounted() != ziplineMountable)
			{
				ziplineMountable.Kill(global::BaseNetworkable.DestroyMode.None);
			}
			base.SetFlag(global::BaseEntity.Flags.Busy, true, false, true);
			base.Invoke(new Action(this.ClearBusy), 2f);
		}
	}

	// Token: 0x06001483 RID: 5251 RVA: 0x0005E510 File Offset: 0x0005C710
	private void ClearBusy()
	{
		base.SetFlag(global::BaseEntity.Flags.Busy, false, false, true);
	}

	// Token: 0x06001484 RID: 5252 RVA: 0x000A2520 File Offset: 0x000A0720
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.zipline == null)
		{
			info.msg.zipline = Facepunch.Pool.Get<Zipline>();
		}
		info.msg.zipline.destinationPoints = Facepunch.Pool.GetList<VectorData>();
		foreach (Vector3 vector in this.ziplineTargets)
		{
			info.msg.zipline.destinationPoints.Add(new VectorData(vector.x, vector.y, vector.z));
		}
	}

	// Token: 0x06001485 RID: 5253 RVA: 0x000A25D4 File Offset: 0x000A07D4
	[ServerVar(ServerAdmin = true)]
	public static void report(ConsoleSystem.Arg arg)
	{
		float num = 0f;
		int num2 = 0;
		int num3 = 0;
		foreach (global::BaseNetworkable baseNetworkable in global::BaseNetworkable.serverEntities)
		{
			ZiplineLaunchPoint ziplineLaunchPoint;
			if ((ziplineLaunchPoint = (baseNetworkable as ZiplineLaunchPoint)) != null)
			{
				float lineLength = ziplineLaunchPoint.GetLineLength();
				num2++;
				num += lineLength;
			}
			else if (baseNetworkable is global::ZiplineArrivalPoint)
			{
				num3++;
			}
		}
		arg.ReplyWith(string.Format("{0} ziplines, total distance: {1:F2}, avg length: {2:F2}, arrival points: {3}", new object[]
		{
			num2,
			num,
			num / (float)num2,
			num3
		}));
	}

	// Token: 0x06001486 RID: 5254 RVA: 0x000A2690 File Offset: 0x000A0890
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.zipline != null)
		{
			this.ziplineTargets.Clear();
			foreach (VectorData v in info.msg.zipline.destinationPoints)
			{
				this.ziplineTargets.Add(v);
			}
		}
	}

	// Token: 0x06001487 RID: 5255 RVA: 0x000A2718 File Offset: 0x000A0918
	private void CalculateZiplinePoints(List<Vector3> targets, ref List<Vector3> points)
	{
		if (points != null || targets.Count == 0)
		{
			return;
		}
		Vector3[] array = new Vector3[targets.Count + 1];
		array[0] = this.LineDeparturePoint.position;
		for (int i = 0; i < targets.Count; i++)
		{
			array[i + 1] = targets[i];
		}
		float[] array2 = new float[array.Length];
		for (int j = 0; j < array2.Length; j++)
		{
			array2[j] = this.LineSlackAmount;
		}
		points = Facepunch.Pool.GetList<Vector3>();
		Bezier.ApplyLineSlack(array, array2, ref points, 25);
	}

	// Token: 0x06001488 RID: 5256 RVA: 0x000A27A4 File Offset: 0x000A09A4
	private Vector3 CalculateLineMidPoint(Vector3 start, Vector3 endPoint)
	{
		Vector3 result = Vector3.Lerp(start, endPoint, 0.5f);
		result.y -= this.LineSlackAmount;
		return result;
	}

	// Token: 0x06001489 RID: 5257 RVA: 0x000A27D0 File Offset: 0x000A09D0
	private void UpdateBuildingBlocks()
	{
		BoxCollider[] array = this.BuildingBlocks;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(false);
		}
		array = this.PointBuildingBlocks;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(false);
		}
		SpawnableBoundsBlocker[] spawnableBoundsBlockers = this.SpawnableBoundsBlockers;
		for (int i = 0; i < spawnableBoundsBlockers.Length; i++)
		{
			spawnableBoundsBlockers[i].gameObject.SetActive(false);
		}
		int num = 0;
		if (this.ziplineTargets.Count > 0)
		{
			Vector3 vector = Vector3.zero;
			int startIndex = 0;
			for (int j = 0; j < this.linePoints.Count; j++)
			{
				if (j != 0 && (!base.isClient || j != 1))
				{
					Vector3 vector2 = this.linePoints[j];
					Vector3 normalized = (vector2 - this.linePoints[j - 1].WithY(vector2.y)).normalized;
					if (vector != Vector3.zero && Vector3.Dot(normalized, vector) < 0.98f)
					{
						if (num < this.BuildingBlocks.Length)
						{
							this.<UpdateBuildingBlocks>g__SetUpBuildingBlock|24_0(this.BuildingBlocks[num], this.PointBuildingBlocks[num], this.SpawnableBoundsBlockers[num++], startIndex, j - 1);
						}
						startIndex = j - 1;
					}
					vector = normalized;
				}
			}
			if (num < this.BuildingBlocks.Length)
			{
				this.<UpdateBuildingBlocks>g__SetUpBuildingBlock|24_0(this.BuildingBlocks[num], this.PointBuildingBlocks[num], this.SpawnableBoundsBlockers[num], startIndex, this.linePoints.Count - 1);
			}
		}
	}

	// Token: 0x0600148A RID: 5258 RVA: 0x000A2969 File Offset: 0x000A0B69
	private bool IsPlayerFacingValidDirection(global::BasePlayer ply)
	{
		return Vector3.Dot(ply.eyes.HeadForward(), base.transform.forward) > 0.2f;
	}

	// Token: 0x0600148B RID: 5259 RVA: 0x000A2990 File Offset: 0x000A0B90
	public float GetLineLength()
	{
		if (this.linePoints == null)
		{
			return 0f;
		}
		float num = 0f;
		for (int i = 0; i < this.linePoints.Count - 1; i++)
		{
			num += Vector3.Distance(this.linePoints[i], this.linePoints[i + 1]);
		}
		return num;
	}

	// Token: 0x0600148D RID: 5261 RVA: 0x000A2A0C File Offset: 0x000A0C0C
	[CompilerGenerated]
	private void <UpdateBuildingBlocks>g__SetUpBuildingBlock|24_0(BoxCollider longCollider, BoxCollider pointCollider, SpawnableBoundsBlocker spawnBlocker, int startIndex, int endIndex)
	{
		Vector3 a = this.linePoints[startIndex];
		Vector3 b = this.linePoints[endIndex];
		Vector3 vector = Vector3.zero;
		Quaternion rotation = Quaternion.LookRotation((a - b).normalized, Vector3.up);
		Vector3 position = Vector3.Lerp(a, b, 0.5f);
		longCollider.transform.position = position;
		longCollider.transform.rotation = rotation;
		for (int i = startIndex; i < endIndex; i++)
		{
			Vector3 vector2 = longCollider.transform.InverseTransformPoint(this.linePoints[i]);
			if (vector2.y < vector.y)
			{
				vector = vector2;
			}
		}
		float num = Mathf.Abs(vector.y) + 2f;
		float z = Vector3.Distance(a, b);
		Vector3 vector3 = spawnBlocker.BoxCollider.size = new Vector3(0.5f, num, z) + Vector3.one;
		longCollider.size = vector3;
		BoxCollider boxCollider = spawnBlocker.BoxCollider;
		vector3 = new Vector3(0f, -(num * 0.5f), 0f);
		boxCollider.center = vector3;
		longCollider.center = vector3;
		longCollider.gameObject.SetActive(true);
		pointCollider.transform.position = this.linePoints[endIndex];
		pointCollider.gameObject.SetActive(true);
		spawnBlocker.gameObject.SetActive(true);
		if (base.isServer)
		{
			spawnBlocker.ClearTrees();
		}
	}

	// Token: 0x04000D00 RID: 3328
	public Transform LineDeparturePoint;

	// Token: 0x04000D01 RID: 3329
	public LineRenderer ZiplineRenderer;

	// Token: 0x04000D02 RID: 3330
	public Collider MountCollider;

	// Token: 0x04000D03 RID: 3331
	public BoxCollider[] BuildingBlocks;

	// Token: 0x04000D04 RID: 3332
	public BoxCollider[] PointBuildingBlocks;

	// Token: 0x04000D05 RID: 3333
	public SpawnableBoundsBlocker[] SpawnableBoundsBlockers;

	// Token: 0x04000D06 RID: 3334
	public GameObjectRef MountableRef;

	// Token: 0x04000D07 RID: 3335
	public float LineSlackAmount = 2f;

	// Token: 0x04000D08 RID: 3336
	public bool RegenLine;

	// Token: 0x04000D09 RID: 3337
	private List<Vector3> ziplineTargets = new List<Vector3>();

	// Token: 0x04000D0A RID: 3338
	private List<Vector3> linePoints;

	// Token: 0x04000D0B RID: 3339
	public GameObjectRef ArrivalPointRef;
}
