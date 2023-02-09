using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000491 RID: 1169
public class TrainTrackSpline : WorldSpline
{
	// Token: 0x17000300 RID: 768
	// (get) Token: 0x060025F0 RID: 9712 RVA: 0x000ECC49 File Offset: 0x000EAE49
	private bool HasNextTrack
	{
		get
		{
			return this.nextTracks.Count > 0;
		}
	}

	// Token: 0x17000301 RID: 769
	// (get) Token: 0x060025F1 RID: 9713 RVA: 0x000ECC59 File Offset: 0x000EAE59
	private bool HasPrevTrack
	{
		get
		{
			return this.prevTracks.Count > 0;
		}
	}

	// Token: 0x060025F2 RID: 9714 RVA: 0x000ECC69 File Offset: 0x000EAE69
	public void SetAll(Vector3[] points, Vector3[] tangents, TrainTrackSpline sourceSpline)
	{
		this.points = points;
		this.tangents = tangents;
		this.lutInterval = sourceSpline.lutInterval;
		this.isStation = sourceSpline.isStation;
		this.aboveGroundSpawn = sourceSpline.aboveGroundSpawn;
		this.hierarchy = sourceSpline.hierarchy;
	}

	// Token: 0x060025F3 RID: 9715 RVA: 0x000ECCAC File Offset: 0x000EAEAC
	public float GetSplineDistAfterMove(float prevSplineDist, Vector3 askerForward, float distMoved, TrainTrackSpline.TrackSelection trackSelection, out TrainTrackSpline onSpline, out bool atEndOfLine, TrainTrackSpline preferredAltA, TrainTrackSpline preferredAltB)
	{
		bool facingForward = this.IsForward(askerForward, prevSplineDist);
		return this.GetSplineDistAfterMove(prevSplineDist, distMoved, trackSelection, facingForward, out onSpline, out atEndOfLine, preferredAltA, preferredAltB);
	}

	// Token: 0x060025F4 RID: 9716 RVA: 0x000ECCD8 File Offset: 0x000EAED8
	private float GetSplineDistAfterMove(float prevSplineDist, float distMoved, TrainTrackSpline.TrackSelection trackSelection, bool facingForward, out TrainTrackSpline onSpline, out bool atEndOfLine, TrainTrackSpline preferredAltA, TrainTrackSpline preferredAltB)
	{
		WorldSplineData data = base.GetData();
		float num = facingForward ? (prevSplineDist + distMoved) : (prevSplineDist - distMoved);
		atEndOfLine = false;
		onSpline = this;
		if (num < 0f)
		{
			if (this.HasPrevTrack)
			{
				TrainTrackSpline.ConnectedTrackInfo trackSelection2 = this.GetTrackSelection(this.prevTracks, this.straightestPrevIndex, trackSelection, false, facingForward, preferredAltA, preferredAltB);
				float distMoved2 = facingForward ? num : (-num);
				if (trackSelection2.orientation == TrainTrackSpline.TrackOrientation.Same)
				{
					prevSplineDist = trackSelection2.track.GetLength();
				}
				else
				{
					prevSplineDist = 0f;
					facingForward = !facingForward;
				}
				return trackSelection2.track.GetSplineDistAfterMove(prevSplineDist, distMoved2, trackSelection, facingForward, out onSpline, out atEndOfLine, preferredAltA, preferredAltB);
			}
			atEndOfLine = true;
			num = 0f;
		}
		else if (num > data.Length)
		{
			if (this.HasNextTrack)
			{
				TrainTrackSpline.ConnectedTrackInfo trackSelection3 = this.GetTrackSelection(this.nextTracks, this.straightestNextIndex, trackSelection, true, facingForward, preferredAltA, preferredAltB);
				float distMoved3 = facingForward ? (num - data.Length) : (-(num - data.Length));
				if (trackSelection3.orientation == TrainTrackSpline.TrackOrientation.Same)
				{
					prevSplineDist = 0f;
				}
				else
				{
					prevSplineDist = trackSelection3.track.GetLength();
					facingForward = !facingForward;
				}
				return trackSelection3.track.GetSplineDistAfterMove(prevSplineDist, distMoved3, trackSelection, facingForward, out onSpline, out atEndOfLine, preferredAltA, preferredAltB);
			}
			atEndOfLine = true;
			num = data.Length;
		}
		return num;
	}

	// Token: 0x060025F5 RID: 9717 RVA: 0x000ECE1C File Offset: 0x000EB01C
	public float GetDistance(Vector3 position, float maxError, out float minSplineDist)
	{
		WorldSplineData data = base.GetData();
		float num = maxError * maxError;
		Vector3 b = base.transform.InverseTransformPoint(position);
		float num2 = float.MaxValue;
		minSplineDist = 0f;
		int num3 = 0;
		int num4 = data.LUTValues.Count;
		if (data.Length > 40f)
		{
			int num5 = 0;
			while ((float)num5 < data.Length + 10f)
			{
				float num6 = Vector3.SqrMagnitude(data.GetPointCubicHermite((float)num5) - b);
				if (num6 < num2)
				{
					num2 = num6;
					minSplineDist = (float)num5;
				}
				num5 += 10;
			}
			num3 = Mathf.FloorToInt(Mathf.Max(0f, minSplineDist - 10f + 1f));
			num4 = Mathf.CeilToInt(Mathf.Min((float)data.LUTValues.Count, minSplineDist + 10f - 1f));
		}
		for (int i = num3; i < num4; i++)
		{
			WorldSplineData.LUTEntry lutentry = data.LUTValues[i];
			for (int j = 0; j < lutentry.points.Count; j++)
			{
				WorldSplineData.LUTEntry.LUTPoint lutpoint = lutentry.points[j];
				float num7 = Vector3.SqrMagnitude(lutpoint.pos - b);
				if (num7 < num2)
				{
					num2 = num7;
					minSplineDist = lutpoint.distance;
					if (num7 < num)
					{
						break;
					}
				}
			}
		}
		return Mathf.Sqrt(num2);
	}

	// Token: 0x060025F6 RID: 9718 RVA: 0x000ECF6F File Offset: 0x000EB16F
	public float GetLength()
	{
		return base.GetData().Length;
	}

	// Token: 0x060025F7 RID: 9719 RVA: 0x000ECF7C File Offset: 0x000EB17C
	public Vector3 GetPosition(float distance)
	{
		return base.GetPointCubicHermiteWorld(distance);
	}

	// Token: 0x060025F8 RID: 9720 RVA: 0x000ECF85 File Offset: 0x000EB185
	public Vector3 GetPositionAndTangent(float distance, Vector3 askerForward, out Vector3 tangent)
	{
		Vector3 pointAndTangentCubicHermiteWorld = base.GetPointAndTangentCubicHermiteWorld(distance, out tangent);
		if (Vector3.Dot(askerForward, tangent) < 0f)
		{
			tangent = -tangent;
		}
		return pointAndTangentCubicHermiteWorld;
	}

	// Token: 0x060025F9 RID: 9721 RVA: 0x000ECFB4 File Offset: 0x000EB1B4
	public void AddTrackConnection(TrainTrackSpline track, TrainTrackSpline.TrackPosition p, TrainTrackSpline.TrackOrientation o)
	{
		List<TrainTrackSpline.ConnectedTrackInfo> list = (p == TrainTrackSpline.TrackPosition.Next) ? this.nextTracks : this.prevTracks;
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].track == track)
			{
				return;
			}
		}
		Vector3 position = (p == TrainTrackSpline.TrackPosition.Next) ? this.points[this.points.Length - 2] : this.points[0];
		Vector3 position2 = (p == TrainTrackSpline.TrackPosition.Next) ? this.points[this.points.Length - 1] : this.points[1];
		Vector3 from = base.transform.TransformPoint(position2) - base.transform.TransformPoint(position);
		Vector3 initialVector = TrainTrackSpline.GetInitialVector(track, p, o);
		float num = Vector3.SignedAngle(from, initialVector, Vector3.up);
		int num2 = 0;
		while (num2 < list.Count && list[num2].angle <= num)
		{
			num2++;
		}
		list.Insert(num2, new TrainTrackSpline.ConnectedTrackInfo(track, o, num));
		int num3 = int.MaxValue;
		for (int j = 0; j < list.Count; j++)
		{
			num3 = Mathf.Min(num3, list[j].track.hierarchy);
		}
		float num4 = float.MaxValue;
		int num5 = 0;
		for (int k = 0; k < list.Count; k++)
		{
			TrainTrackSpline.ConnectedTrackInfo connectedTrackInfo = list[k];
			if (connectedTrackInfo.track.hierarchy <= num3)
			{
				float num6 = Mathf.Abs(connectedTrackInfo.angle);
				if (num6 < num4)
				{
					num4 = num6;
					num5 = k;
					if (num4 == 0f)
					{
						break;
					}
				}
			}
		}
		if (p == TrainTrackSpline.TrackPosition.Next)
		{
			this.straightestNextIndex = num5;
			return;
		}
		this.straightestPrevIndex = num5;
	}

	// Token: 0x060025FA RID: 9722 RVA: 0x000ED15C File Offset: 0x000EB35C
	public void RegisterTrackUser(TrainTrackSpline.ITrainTrackUser user)
	{
		this.trackUsers.Add(user);
	}

	// Token: 0x060025FB RID: 9723 RVA: 0x000ED16B File Offset: 0x000EB36B
	public void DeregisterTrackUser(TrainTrackSpline.ITrainTrackUser user)
	{
		if (user == null)
		{
			return;
		}
		this.trackUsers.Remove(user);
	}

	// Token: 0x060025FC RID: 9724 RVA: 0x000ED180 File Offset: 0x000EB380
	public bool IsForward(Vector3 askerForward, float askerSplineDist)
	{
		WorldSplineData data = base.GetData();
		Vector3 tangentCubicHermiteWorld = base.GetTangentCubicHermiteWorld(askerSplineDist, data);
		return Vector3.Dot(askerForward, tangentCubicHermiteWorld) >= 0f;
	}

	// Token: 0x060025FD RID: 9725 RVA: 0x000ED1B0 File Offset: 0x000EB3B0
	public bool HasValidHazardWithin(TrainCar asker, float askerSplineDist, float minHazardDist, float maxHazardDist, TrainTrackSpline.TrackSelection trackSelection, float trackSpeed, TrainTrackSpline preferredAltA, TrainTrackSpline preferredAltB)
	{
		Vector3 askerForward = (trackSpeed >= 0f) ? asker.transform.forward : (-asker.transform.forward);
		bool movingForward = this.IsForward(askerForward, askerSplineDist);
		return this.HasValidHazardWithin(asker, askerForward, askerSplineDist, minHazardDist, maxHazardDist, trackSelection, movingForward, preferredAltA, preferredAltB);
	}

	// Token: 0x060025FE RID: 9726 RVA: 0x000ED200 File Offset: 0x000EB400
	public bool HasValidHazardWithin(TrainTrackSpline.ITrainTrackUser asker, Vector3 askerForward, float askerSplineDist, float minHazardDist, float maxHazardDist, TrainTrackSpline.TrackSelection trackSelection, bool movingForward, TrainTrackSpline preferredAltA, TrainTrackSpline preferredAltB)
	{
		WorldSplineData data = base.GetData();
		foreach (TrainTrackSpline.ITrainTrackUser trainTrackUser in this.trackUsers)
		{
			if (trainTrackUser != asker)
			{
				Vector3 rhs = trainTrackUser.Position - asker.Position;
				if (Vector3.Dot(askerForward, rhs) >= 0f)
				{
					float magnitude = rhs.magnitude;
					if (magnitude > minHazardDist && magnitude < maxHazardDist)
					{
						Vector3 worldVelocity = trainTrackUser.GetWorldVelocity();
						if (worldVelocity.sqrMagnitude < 4f || Vector3.Dot(worldVelocity, rhs) < 0f)
						{
							return true;
						}
					}
				}
			}
		}
		float num = movingForward ? (askerSplineDist + minHazardDist) : (askerSplineDist - minHazardDist);
		float num2 = movingForward ? (askerSplineDist + maxHazardDist) : (askerSplineDist - maxHazardDist);
		if (num2 < 0f)
		{
			if (this.HasPrevTrack)
			{
				TrainTrackSpline.ConnectedTrackInfo trackSelection2 = this.GetTrackSelection(this.prevTracks, this.straightestPrevIndex, trackSelection, false, movingForward, preferredAltA, preferredAltB);
				if (trackSelection2.orientation == TrainTrackSpline.TrackOrientation.Same)
				{
					askerSplineDist = trackSelection2.track.GetLength();
				}
				else
				{
					askerSplineDist = 0f;
					movingForward = !movingForward;
				}
				float minHazardDist2 = Mathf.Max(-num, 0f);
				float maxHazardDist2 = -num2;
				return trackSelection2.track.HasValidHazardWithin(asker, askerForward, askerSplineDist, minHazardDist2, maxHazardDist2, trackSelection, movingForward, preferredAltA, preferredAltB);
			}
		}
		else if (num2 > data.Length && this.HasNextTrack)
		{
			TrainTrackSpline.ConnectedTrackInfo trackSelection3 = this.GetTrackSelection(this.nextTracks, this.straightestNextIndex, trackSelection, true, movingForward, preferredAltA, preferredAltB);
			if (trackSelection3.orientation == TrainTrackSpline.TrackOrientation.Same)
			{
				askerSplineDist = 0f;
			}
			else
			{
				askerSplineDist = trackSelection3.track.GetLength();
				movingForward = !movingForward;
			}
			float minHazardDist3 = Mathf.Max(num - data.Length, 0f);
			float maxHazardDist3 = num2 - data.Length;
			return trackSelection3.track.HasValidHazardWithin(asker, askerForward, askerSplineDist, minHazardDist3, maxHazardDist3, trackSelection, movingForward, preferredAltA, preferredAltB);
		}
		return false;
	}

	// Token: 0x060025FF RID: 9727 RVA: 0x000ED404 File Offset: 0x000EB604
	public bool HasAnyUsers()
	{
		return this.trackUsers.Count > 0;
	}

	// Token: 0x06002600 RID: 9728 RVA: 0x000ED414 File Offset: 0x000EB614
	public bool HasAnyUsersOfType(TrainCar.TrainCarType carType)
	{
		using (HashSet<TrainTrackSpline.ITrainTrackUser>.Enumerator enumerator = this.trackUsers.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.CarType == carType)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06002601 RID: 9729 RVA: 0x000ED470 File Offset: 0x000EB670
	public bool HasConnectedTrack(TrainTrackSpline tts)
	{
		return this.HasConnectedNextTrack(tts) || this.HasConnectedPrevTrack(tts);
	}

	// Token: 0x06002602 RID: 9730 RVA: 0x000ED484 File Offset: 0x000EB684
	public bool HasConnectedNextTrack(TrainTrackSpline tts)
	{
		using (List<TrainTrackSpline.ConnectedTrackInfo>.Enumerator enumerator = this.nextTracks.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.track == tts)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06002603 RID: 9731 RVA: 0x000ED4E4 File Offset: 0x000EB6E4
	public bool HasConnectedPrevTrack(TrainTrackSpline tts)
	{
		using (List<TrainTrackSpline.ConnectedTrackInfo>.Enumerator enumerator = this.prevTracks.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.track == tts)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06002604 RID: 9732 RVA: 0x000ED544 File Offset: 0x000EB744
	private static Vector3 GetInitialVector(TrainTrackSpline track, TrainTrackSpline.TrackPosition p, TrainTrackSpline.TrackOrientation o)
	{
		Vector3 position;
		Vector3 position2;
		if (p == TrainTrackSpline.TrackPosition.Next)
		{
			if (o == TrainTrackSpline.TrackOrientation.Reverse)
			{
				position = track.points[track.points.Length - 1];
				position2 = track.points[track.points.Length - 2];
			}
			else
			{
				position = track.points[0];
				position2 = track.points[1];
			}
		}
		else if (o == TrainTrackSpline.TrackOrientation.Reverse)
		{
			position = track.points[1];
			position2 = track.points[0];
		}
		else
		{
			position = track.points[track.points.Length - 2];
			position2 = track.points[track.points.Length - 1];
		}
		return track.transform.TransformPoint(position2) - track.transform.TransformPoint(position);
	}

	// Token: 0x06002605 RID: 9733 RVA: 0x000ED60C File Offset: 0x000EB80C
	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		for (int i = 0; i < this.nextTracks.Count; i++)
		{
			Color splineColour = Color.white;
			if (this.straightestNextIndex != i && this.nextTracks.Count > 1)
			{
				if (i == 0)
				{
					splineColour = Color.green;
				}
				else if (i == this.nextTracks.Count - 1)
				{
					splineColour = Color.yellow;
				}
			}
			WorldSpline.DrawSplineGizmo(this.nextTracks[i].track, splineColour);
		}
		for (int j = 0; j < this.prevTracks.Count; j++)
		{
			Color splineColour2 = Color.white;
			if (this.straightestPrevIndex != j && this.prevTracks.Count > 1)
			{
				if (j == 0)
				{
					splineColour2 = Color.green;
				}
				else if (j == this.nextTracks.Count - 1)
				{
					splineColour2 = Color.yellow;
				}
			}
			WorldSpline.DrawSplineGizmo(this.prevTracks[j].track, splineColour2);
		}
	}

	// Token: 0x06002606 RID: 9734 RVA: 0x000ED6F8 File Offset: 0x000EB8F8
	private TrainTrackSpline.ConnectedTrackInfo GetTrackSelection(List<TrainTrackSpline.ConnectedTrackInfo> trackOptions, int straightestIndex, TrainTrackSpline.TrackSelection trackSelection, bool nextTrack, bool trainForward, TrainTrackSpline preferredAltA, TrainTrackSpline preferredAltB)
	{
		if (trackOptions.Count == 1)
		{
			return trackOptions[0];
		}
		foreach (TrainTrackSpline.ConnectedTrackInfo connectedTrackInfo in trackOptions)
		{
			if (connectedTrackInfo.track == preferredAltA || connectedTrackInfo.track == preferredAltB)
			{
				return connectedTrackInfo;
			}
		}
		bool flag = nextTrack ^ trainForward;
		if (trackSelection != TrainTrackSpline.TrackSelection.Left)
		{
			if (trackSelection != TrainTrackSpline.TrackSelection.Right)
			{
				return trackOptions[straightestIndex];
			}
			if (!flag)
			{
				return trackOptions[trackOptions.Count - 1];
			}
			return trackOptions[0];
		}
		else
		{
			if (!flag)
			{
				return trackOptions[0];
			}
			return trackOptions[trackOptions.Count - 1];
		}
		TrainTrackSpline.ConnectedTrackInfo result;
		return result;
	}

	// Token: 0x06002607 RID: 9735 RVA: 0x000ED7C0 File Offset: 0x000EB9C0
	public static bool TryFindTrackNear(Vector3 pos, float maxDist, out TrainTrackSpline splineResult, out float distResult)
	{
		splineResult = null;
		distResult = 0f;
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapSphere(pos, maxDist, list, 65536, QueryTriggerInteraction.Ignore);
		if (list.Count > 0)
		{
			List<TrainTrackSpline> list2 = Pool.GetList<TrainTrackSpline>();
			float num = float.MaxValue;
			foreach (Collider collider in list)
			{
				collider.GetComponentsInParent<TrainTrackSpline>(false, list2);
				if (list2.Count > 0)
				{
					foreach (TrainTrackSpline trainTrackSpline in list2)
					{
						float num2;
						float distance = trainTrackSpline.GetDistance(pos, 1f, out num2);
						if (distance < num)
						{
							num = distance;
							distResult = num2;
							splineResult = trainTrackSpline;
						}
					}
				}
			}
			Pool.FreeList<TrainTrackSpline>(ref list2);
		}
		Pool.FreeList<Collider>(ref list);
		return splineResult != null;
	}

	// Token: 0x04001EDD RID: 7901
	[Tooltip("Is this track spline part of a train station?")]
	public bool isStation;

	// Token: 0x04001EDE RID: 7902
	[Tooltip("Can above-ground trains spawn here?")]
	public bool aboveGroundSpawn;

	// Token: 0x04001EDF RID: 7903
	public int hierarchy;

	// Token: 0x04001EE0 RID: 7904
	public static List<TrainTrackSpline> SidingSplines = new List<TrainTrackSpline>();

	// Token: 0x04001EE1 RID: 7905
	private List<TrainTrackSpline.ConnectedTrackInfo> nextTracks = new List<TrainTrackSpline.ConnectedTrackInfo>();

	// Token: 0x04001EE2 RID: 7906
	private int straightestNextIndex;

	// Token: 0x04001EE3 RID: 7907
	private List<TrainTrackSpline.ConnectedTrackInfo> prevTracks = new List<TrainTrackSpline.ConnectedTrackInfo>();

	// Token: 0x04001EE4 RID: 7908
	private int straightestPrevIndex;

	// Token: 0x04001EE5 RID: 7909
	private HashSet<TrainTrackSpline.ITrainTrackUser> trackUsers = new HashSet<TrainTrackSpline.ITrainTrackUser>();

	// Token: 0x02000CBB RID: 3259
	public enum TrackSelection
	{
		// Token: 0x0400439E RID: 17310
		Default,
		// Token: 0x0400439F RID: 17311
		Left,
		// Token: 0x040043A0 RID: 17312
		Right
	}

	// Token: 0x02000CBC RID: 3260
	public enum TrackPosition
	{
		// Token: 0x040043A2 RID: 17314
		Next,
		// Token: 0x040043A3 RID: 17315
		Prev
	}

	// Token: 0x02000CBD RID: 3261
	public enum TrackOrientation
	{
		// Token: 0x040043A5 RID: 17317
		Same,
		// Token: 0x040043A6 RID: 17318
		Reverse
	}

	// Token: 0x02000CBE RID: 3262
	private class ConnectedTrackInfo
	{
		// Token: 0x06004D5C RID: 19804 RVA: 0x00197C9D File Offset: 0x00195E9D
		public ConnectedTrackInfo(TrainTrackSpline track, TrainTrackSpline.TrackOrientation orientation, float angle)
		{
			this.track = track;
			this.orientation = orientation;
			this.angle = angle;
		}

		// Token: 0x040043A7 RID: 17319
		public TrainTrackSpline track;

		// Token: 0x040043A8 RID: 17320
		public TrainTrackSpline.TrackOrientation orientation;

		// Token: 0x040043A9 RID: 17321
		public float angle;
	}

	// Token: 0x02000CBF RID: 3263
	public enum DistanceType
	{
		// Token: 0x040043AB RID: 17323
		SplineDistance,
		// Token: 0x040043AC RID: 17324
		WorldDistance
	}

	// Token: 0x02000CC0 RID: 3264
	public interface ITrainTrackUser
	{
		// Token: 0x1700062C RID: 1580
		// (get) Token: 0x06004D5D RID: 19805
		Vector3 Position { get; }

		// Token: 0x1700062D RID: 1581
		// (get) Token: 0x06004D5E RID: 19806
		float FrontWheelSplineDist { get; }

		// Token: 0x06004D5F RID: 19807
		Vector3 GetWorldVelocity();

		// Token: 0x1700062E RID: 1582
		// (get) Token: 0x06004D60 RID: 19808
		TrainCar.TrainCarType CarType { get; }
	}
}
