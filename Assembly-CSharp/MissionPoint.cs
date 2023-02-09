using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using UnityEngine;

// Token: 0x020005E7 RID: 1511
public class MissionPoint : MonoBehaviour
{
	// Token: 0x06002C59 RID: 11353 RVA: 0x00109D94 File Offset: 0x00107F94
	public static int TypeToIndex(int id)
	{
		return MissionPoint.type2index[id];
	}

	// Token: 0x06002C5A RID: 11354 RVA: 0x00109DA1 File Offset: 0x00107FA1
	public static int IndexToType(int idx)
	{
		return 1 << idx;
	}

	// Token: 0x06002C5B RID: 11355 RVA: 0x00109DA9 File Offset: 0x00107FA9
	public void Awake()
	{
		MissionPoint.all.Add(this);
	}

	// Token: 0x06002C5C RID: 11356 RVA: 0x00109DB6 File Offset: 0x00107FB6
	private void Start()
	{
		if (this.dropToGround)
		{
			SingletonComponent<InvokeHandler>.Instance.Invoke(new Action(this.DropToGround), 0.5f);
		}
	}

	// Token: 0x06002C5D RID: 11357 RVA: 0x00109DDC File Offset: 0x00107FDC
	private void DropToGround()
	{
		if (Rust.Application.isLoading)
		{
			SingletonComponent<InvokeHandler>.Instance.Invoke(new Action(this.DropToGround), 0.5f);
			return;
		}
		Vector3 position = base.transform.position;
		base.transform.DropToGround(false, 100f);
	}

	// Token: 0x06002C5E RID: 11358 RVA: 0x00109E2A File Offset: 0x0010802A
	public void OnDisable()
	{
		if (MissionPoint.all.Contains(this))
		{
			MissionPoint.all.Remove(this);
		}
	}

	// Token: 0x06002C5F RID: 11359 RVA: 0x000299AB File Offset: 0x00027BAB
	public virtual Vector3 GetPosition()
	{
		return base.transform.position;
	}

	// Token: 0x06002C60 RID: 11360 RVA: 0x00109E45 File Offset: 0x00108045
	public virtual Quaternion GetRotation()
	{
		return base.transform.rotation;
	}

	// Token: 0x06002C61 RID: 11361 RVA: 0x00109E54 File Offset: 0x00108054
	public static bool GetMissionPoints(ref List<MissionPoint> points, Vector3 near, float minDistance, float maxDistance, int flags, int exclusionFlags)
	{
		List<MissionPoint> list = Pool.GetList<MissionPoint>();
		foreach (MissionPoint missionPoint in MissionPoint.all)
		{
			if ((missionPoint.Flags & (MissionPoint.MissionPointEnum)flags) == (MissionPoint.MissionPointEnum)flags && (exclusionFlags == 0 || (missionPoint.Flags & (MissionPoint.MissionPointEnum)exclusionFlags) == (MissionPoint.MissionPointEnum)0))
			{
				float num = Vector3.Distance(missionPoint.transform.position, near);
				if (num <= maxDistance && num > minDistance)
				{
					if (BaseMission.blockedPoints.Count > 0)
					{
						bool flag = false;
						using (List<Vector3>.Enumerator enumerator2 = BaseMission.blockedPoints.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								if (Vector3.Distance(enumerator2.Current, missionPoint.transform.position) < 5f)
								{
									flag = true;
									break;
								}
							}
						}
						if (flag)
						{
							continue;
						}
					}
					list.Add(missionPoint);
				}
			}
		}
		if (list.Count == 0)
		{
			return false;
		}
		foreach (MissionPoint item in list)
		{
			points.Add(item);
		}
		Pool.FreeList<MissionPoint>(ref list);
		return true;
	}

	// Token: 0x04002426 RID: 9254
	public bool dropToGround = true;

	// Token: 0x04002427 RID: 9255
	public const int COUNT = 8;

	// Token: 0x04002428 RID: 9256
	public const int EVERYTHING = -1;

	// Token: 0x04002429 RID: 9257
	public const int NOTHING = 0;

	// Token: 0x0400242A RID: 9258
	public const int EASY_MONUMENT = 1;

	// Token: 0x0400242B RID: 9259
	public const int MED_MONUMENT = 2;

	// Token: 0x0400242C RID: 9260
	public const int HARD_MONUMENT = 4;

	// Token: 0x0400242D RID: 9261
	public const int ITEM_HIDESPOT = 8;

	// Token: 0x0400242E RID: 9262
	public const int UNDERWATER = 128;

	// Token: 0x0400242F RID: 9263
	public const int EASY_MONUMENT_IDX = 0;

	// Token: 0x04002430 RID: 9264
	public const int MED_MONUMENT_IDX = 1;

	// Token: 0x04002431 RID: 9265
	public const int HARD_MONUMENT_IDX = 2;

	// Token: 0x04002432 RID: 9266
	public const int ITEM_HIDESPOT_IDX = 3;

	// Token: 0x04002433 RID: 9267
	public const int FOREST_IDX = 4;

	// Token: 0x04002434 RID: 9268
	public const int ROADSIDE_IDX = 5;

	// Token: 0x04002435 RID: 9269
	public const int BEACH = 6;

	// Token: 0x04002436 RID: 9270
	public const int UNDERWATER_IDX = 7;

	// Token: 0x04002437 RID: 9271
	private static Dictionary<int, int> type2index = new Dictionary<int, int>
	{
		{
			1,
			0
		},
		{
			2,
			1
		},
		{
			4,
			2
		},
		{
			8,
			3
		},
		{
			128,
			7
		}
	};

	// Token: 0x04002438 RID: 9272
	public static List<MissionPoint> all = new List<MissionPoint>();

	// Token: 0x04002439 RID: 9273
	[global::InspectorFlags]
	public MissionPoint.MissionPointEnum Flags = (MissionPoint.MissionPointEnum)(-1);

	// Token: 0x02000D37 RID: 3383
	public enum MissionPointEnum
	{
		// Token: 0x04004588 RID: 17800
		EasyMonument = 1,
		// Token: 0x04004589 RID: 17801
		MediumMonument,
		// Token: 0x0400458A RID: 17802
		HardMonument = 4,
		// Token: 0x0400458B RID: 17803
		Item_Hidespot = 8,
		// Token: 0x0400458C RID: 17804
		Underwater = 128
	}
}
