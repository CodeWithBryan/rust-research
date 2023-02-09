using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x0200053B RID: 1339
public class ConvarControlledSpawnPopulationRail : ConvarControlledSpawnPopulation
{
	// Token: 0x060028D6 RID: 10454 RVA: 0x000F8C5C File Offset: 0x000F6E5C
	public override bool GetSpawnPosOverride(Prefab<Spawnable> prefab, ref Vector3 newPos, ref Quaternion newRot)
	{
		if (TrainTrackSpline.SidingSplines.Count <= 0)
		{
			return false;
		}
		TrainCar component = prefab.Object.GetComponent<TrainCar>();
		if (component == null)
		{
			Debug.LogError(base.GetType().Name + ": Train prefab has no TrainCar component: " + prefab.Object.name);
			return false;
		}
		int num = 0;
		using (List<TrainTrackSpline>.Enumerator enumerator = TrainTrackSpline.SidingSplines.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.HasAnyUsersOfType(TrainCar.TrainCarType.Engine))
				{
					num++;
				}
			}
		}
		bool flag = component.CarType == TrainCar.TrainCarType.Engine;
		int i = 0;
		while (i < 20)
		{
			i++;
			TrainTrackSpline trainTrackSpline = null;
			if (flag)
			{
				foreach (TrainTrackSpline trainTrackSpline2 in TrainTrackSpline.SidingSplines)
				{
					if (!trainTrackSpline2.HasAnyUsersOfType(TrainCar.TrainCarType.Engine))
					{
						trainTrackSpline = trainTrackSpline2;
						break;
					}
				}
			}
			if (trainTrackSpline == null)
			{
				int index = UnityEngine.Random.Range(0, TrainTrackSpline.SidingSplines.Count);
				trainTrackSpline = TrainTrackSpline.SidingSplines[index];
			}
			if (trainTrackSpline != null && this.TryGetRandomPointOnSpline(trainTrackSpline, component, out newPos, out newRot))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060028D7 RID: 10455 RVA: 0x000F8DB4 File Offset: 0x000F6FB4
	public override void OnPostFill(SpawnHandler spawnHandler)
	{
		List<Prefab<Spawnable>> list = Pool.GetList<Prefab<Spawnable>>();
		foreach (Prefab<Spawnable> prefab in this.Prefabs)
		{
			TrainCar component = prefab.Object.GetComponent<TrainCar>();
			if (component != null && component.CarType == TrainCar.TrainCarType.Engine)
			{
				list.Add(prefab);
			}
		}
		foreach (TrainTrackSpline trainTrackSpline in TrainTrackSpline.SidingSplines)
		{
			if (!trainTrackSpline.HasAnyUsersOfType(TrainCar.TrainCarType.Engine))
			{
				int num = UnityEngine.Random.Range(0, list.Count);
				Prefab<Spawnable> prefab2 = this.Prefabs[num];
				TrainCar component2 = prefab2.Object.GetComponent<TrainCar>();
				if (!(component2 == null))
				{
					int j = 0;
					while (j < 20)
					{
						j++;
						Vector3 pos;
						Quaternion rot;
						if (this.TryGetRandomPointOnSpline(trainTrackSpline, component2, out pos, out rot))
						{
							spawnHandler.Spawn(this, prefab2, pos, rot);
							break;
						}
					}
				}
			}
		}
		Pool.FreeList<Prefab<Spawnable>>(ref list);
	}

	// Token: 0x060028D8 RID: 10456 RVA: 0x000F8EBC File Offset: 0x000F70BC
	protected override int GetPrefabWeight(Prefab<Spawnable> prefab)
	{
		int num = prefab.Parameters ? prefab.Parameters.Count : 1;
		TrainCar component = prefab.Object.GetComponent<TrainCar>();
		if (component != null)
		{
			if (component.CarType == TrainCar.TrainCarType.Wagon)
			{
				num *= TrainCar.wagons_per_engine;
			}
		}
		else
		{
			Debug.LogError(base.GetType().Name + ": No TrainCar script on train prefab " + prefab.Object.name);
		}
		return num;
	}

	// Token: 0x060028D9 RID: 10457 RVA: 0x000F8F34 File Offset: 0x000F7134
	private bool TryGetRandomPointOnSpline(TrainTrackSpline spline, TrainCar trainCar, out Vector3 pos, out Quaternion rot)
	{
		float length = spline.GetLength();
		if (length < 65f)
		{
			pos = Vector3.zero;
			rot = Quaternion.identity;
			return false;
		}
		float distance = UnityEngine.Random.Range(60f, length - 60f);
		Vector3 forward;
		pos = spline.GetPointAndTangentCubicHermiteWorld(distance, out forward) + Vector3.up * 0.5f;
		rot = Quaternion.LookRotation(forward);
		float radius = trainCar.bounds.extents.Max();
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapSphere(pos, radius, list, 32768, QueryTriggerInteraction.Ignore);
		bool result = true;
		foreach (Collider collider in list)
		{
			if (!trainCar.ColliderIsPartOfTrain(collider))
			{
				result = false;
				break;
			}
		}
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	// Token: 0x04002129 RID: 8489
	private const float MIN_MARGIN = 60f;
}
