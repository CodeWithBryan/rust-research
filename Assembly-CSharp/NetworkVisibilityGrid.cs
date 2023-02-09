using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ConVar;
using Network;
using Network.Visibility;
using Rust;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020005F7 RID: 1527
public class NetworkVisibilityGrid : MonoBehaviour, Provider
{
	// Token: 0x06002C9E RID: 11422 RVA: 0x0010B0FD File Offset: 0x001092FD
	private void Awake()
	{
		Debug.Assert(Network.Net.sv != null, "Network.Net.sv is NULL when creating Visibility Grid");
		Debug.Assert(Network.Net.sv.visibility == null, "Network.Net.sv.visibility is being set multiple times");
		Network.Net.sv.visibility = new Manager(this);
	}

	// Token: 0x06002C9F RID: 11423 RVA: 0x0010B138 File Offset: 0x00109338
	private void OnEnable()
	{
		this.halfGridSize = (float)this.gridSize / 2f;
		this.cellSize = (float)this.gridSize / (float)this.cellCount;
		this.halfCellSize = this.cellSize / 2f;
		this.numIDsPerLayer = this.cellCount * this.cellCount;
	}

	// Token: 0x06002CA0 RID: 11424 RVA: 0x0010B192 File Offset: 0x00109392
	private void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		if (Network.Net.sv != null && Network.Net.sv.visibility != null)
		{
			Network.Net.sv.visibility.Dispose();
			Network.Net.sv.visibility = null;
		}
	}

	// Token: 0x06002CA1 RID: 11425 RVA: 0x0010B1CC File Offset: 0x001093CC
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Vector3 position = base.transform.position;
		for (int i = 0; i <= this.cellCount; i++)
		{
			float num = -this.halfGridSize + (float)i * this.cellSize - this.halfCellSize;
			Gizmos.DrawLine(new Vector3(this.halfGridSize, position.y, num), new Vector3(-this.halfGridSize, position.y, num));
			Gizmos.DrawLine(new Vector3(num, position.y, this.halfGridSize), new Vector3(num, position.y, -this.halfGridSize));
		}
	}

	// Token: 0x06002CA2 RID: 11426 RVA: 0x0010B26D File Offset: 0x0010946D
	private int PositionToGrid(float value)
	{
		return Mathf.RoundToInt((value + this.halfGridSize) / this.cellSize);
	}

	// Token: 0x06002CA3 RID: 11427 RVA: 0x0010B283 File Offset: 0x00109483
	private float GridToPosition(int value)
	{
		return (float)value * this.cellSize - this.halfGridSize;
	}

	// Token: 0x06002CA4 RID: 11428 RVA: 0x0010B295 File Offset: 0x00109495
	private int PositionToLayer(float y)
	{
		if (y < this.tunnelsThreshold)
		{
			return 2;
		}
		if (y < this.cavesThreshold)
		{
			return 1;
		}
		if (y >= this.dynamicDungeonsThreshold)
		{
			return 10 + Mathf.FloorToInt((y - this.dynamicDungeonsThreshold) / this.dynamicDungeonsInterval);
		}
		return 0;
	}

	// Token: 0x06002CA5 RID: 11429 RVA: 0x0010B2CF File Offset: 0x001094CF
	private uint CoordToID(int x, int y, int layer)
	{
		return (uint)(layer * this.numIDsPerLayer + (x * this.cellCount + y) + this.startID);
	}

	// Token: 0x06002CA6 RID: 11430 RVA: 0x0010B2EC File Offset: 0x001094EC
	private uint GetID(Vector3 vPos)
	{
		int num = this.PositionToGrid(vPos.x);
		int num2 = this.PositionToGrid(vPos.z);
		int num3 = this.PositionToLayer(vPos.y);
		if (num < 0)
		{
			return 0U;
		}
		if (num >= this.cellCount)
		{
			return 0U;
		}
		if (num2 < 0)
		{
			return 0U;
		}
		if (num2 >= this.cellCount)
		{
			return 0U;
		}
		uint num4 = this.CoordToID(num, num2, num3);
		if ((ulong)num4 < (ulong)((long)this.startID))
		{
			Debug.LogError(string.Format("NetworkVisibilityGrid.GetID - group is below range {0} {1} {2} {3} {4}", new object[]
			{
				num,
				num2,
				num3,
				num4,
				this.cellCount
			}));
		}
		return num4;
	}

	// Token: 0x06002CA7 RID: 11431 RVA: 0x0010B3A0 File Offset: 0x001095A0
	[return: TupleElementNames(new string[]
	{
		"x",
		"y",
		"layer"
	})]
	private ValueTuple<int, int, int> DeconstructGroupId(int groupId)
	{
		groupId -= this.startID;
		int a;
		int item = Math.DivRem(groupId, this.numIDsPerLayer, out a);
		int item2;
		return new ValueTuple<int, int, int>(Math.DivRem(a, this.cellCount, out item2), item2, item);
	}

	// Token: 0x06002CA8 RID: 11432 RVA: 0x0010B3DC File Offset: 0x001095DC
	private Bounds GetBounds(uint uid)
	{
		ValueTuple<int, int, int> valueTuple = this.DeconstructGroupId((int)uid);
		int item = valueTuple.Item1;
		int item2 = valueTuple.Item2;
		int item3 = valueTuple.Item3;
		Vector3 vector = new Vector3(this.GridToPosition(item) - this.halfCellSize, 0f, this.GridToPosition(item2) - this.halfCellSize);
		Vector3 max = new Vector3(vector.x + this.cellSize, 0f, vector.z + this.cellSize);
		if (item3 == 0)
		{
			vector.y = this.cavesThreshold;
			max.y = this.dynamicDungeonsThreshold;
		}
		else if (item3 == 1)
		{
			vector.y = this.tunnelsThreshold;
			max.y = this.cavesThreshold - float.Epsilon;
		}
		else if (item3 == 2)
		{
			vector.y = -10000f;
			max.y = this.tunnelsThreshold - float.Epsilon;
		}
		else if (item3 >= 10)
		{
			int num = item3 - 10;
			vector.y = this.dynamicDungeonsThreshold + (float)num * this.dynamicDungeonsInterval + float.Epsilon;
			max.y = vector.y + this.dynamicDungeonsInterval;
		}
		else
		{
			Debug.LogError(string.Format("Cannot get bounds for unknown layer {0}!", item3), this);
		}
		return new Bounds
		{
			min = vector,
			max = max
		};
	}

	// Token: 0x06002CA9 RID: 11433 RVA: 0x0010B52F File Offset: 0x0010972F
	public void OnGroupAdded(Group group)
	{
		group.bounds = this.GetBounds(group.ID);
	}

	// Token: 0x06002CAA RID: 11434 RVA: 0x0010B543 File Offset: 0x00109743
	public bool IsInside(Group group, Vector3 vPos)
	{
		return false || group.ID == 0U || group.bounds.Contains(vPos) || group.bounds.SqrDistance(vPos) < this.switchTolerance;
	}

	// Token: 0x06002CAB RID: 11435 RVA: 0x0010B580 File Offset: 0x00109780
	public Group GetGroup(Vector3 vPos)
	{
		uint id = this.GetID(vPos);
		if (id == 0U)
		{
			return null;
		}
		Group group = Network.Net.sv.visibility.Get(id);
		if (!this.IsInside(group, vPos))
		{
			float num = group.bounds.SqrDistance(vPos);
			Debug.Log(string.Concat(new object[]
			{
				"Group is inside is all fucked ",
				id,
				"/",
				num,
				"/",
				vPos
			}));
		}
		return group;
	}

	// Token: 0x06002CAC RID: 11436 RVA: 0x0010B608 File Offset: 0x00109808
	public void GetVisibleFromFar(Group group, List<Group> groups)
	{
		int visibilityRadiusFarOverride = ConVar.Net.visibilityRadiusFarOverride;
		int radius = (visibilityRadiusFarOverride > 0) ? visibilityRadiusFarOverride : this.visibilityRadiusFar;
		this.GetVisibleFrom(group, groups, radius);
	}

	// Token: 0x06002CAD RID: 11437 RVA: 0x0010B634 File Offset: 0x00109834
	public void GetVisibleFromNear(Group group, List<Group> groups)
	{
		int visibilityRadiusNearOverride = ConVar.Net.visibilityRadiusNearOverride;
		int radius = (visibilityRadiusNearOverride > 0) ? visibilityRadiusNearOverride : this.visibilityRadiusNear;
		this.GetVisibleFrom(group, groups, radius);
	}

	// Token: 0x06002CAE RID: 11438 RVA: 0x0010B660 File Offset: 0x00109860
	private void GetVisibleFrom(Group group, List<Group> groups, int radius)
	{
		NetworkVisibilityGrid.<>c__DisplayClass34_0 CS$<>8__locals1;
		CS$<>8__locals1.groups = groups;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.groups.Add(Network.Net.sv.visibility.Get(0U));
		int id = (int)group.ID;
		if (id < this.startID)
		{
			return;
		}
		ValueTuple<int, int, int> valueTuple = this.DeconstructGroupId(id);
		int item = valueTuple.Item1;
		int item2 = valueTuple.Item2;
		int item3 = valueTuple.Item3;
		this.<GetVisibleFrom>g__AddLayers|34_0(item, item2, item3, ref CS$<>8__locals1);
		for (int i = 1; i <= radius; i++)
		{
			this.<GetVisibleFrom>g__AddLayers|34_0(item - i, item2, item3, ref CS$<>8__locals1);
			this.<GetVisibleFrom>g__AddLayers|34_0(item + i, item2, item3, ref CS$<>8__locals1);
			this.<GetVisibleFrom>g__AddLayers|34_0(item, item2 - i, item3, ref CS$<>8__locals1);
			this.<GetVisibleFrom>g__AddLayers|34_0(item, item2 + i, item3, ref CS$<>8__locals1);
			for (int j = 1; j < i; j++)
			{
				this.<GetVisibleFrom>g__AddLayers|34_0(item - i, item2 - j, item3, ref CS$<>8__locals1);
				this.<GetVisibleFrom>g__AddLayers|34_0(item - i, item2 + j, item3, ref CS$<>8__locals1);
				this.<GetVisibleFrom>g__AddLayers|34_0(item + i, item2 - j, item3, ref CS$<>8__locals1);
				this.<GetVisibleFrom>g__AddLayers|34_0(item + i, item2 + j, item3, ref CS$<>8__locals1);
				this.<GetVisibleFrom>g__AddLayers|34_0(item - j, item2 - i, item3, ref CS$<>8__locals1);
				this.<GetVisibleFrom>g__AddLayers|34_0(item + j, item2 - i, item3, ref CS$<>8__locals1);
				this.<GetVisibleFrom>g__AddLayers|34_0(item - j, item2 + i, item3, ref CS$<>8__locals1);
				this.<GetVisibleFrom>g__AddLayers|34_0(item + j, item2 + i, item3, ref CS$<>8__locals1);
			}
			this.<GetVisibleFrom>g__AddLayers|34_0(item - i, item2 - i, item3, ref CS$<>8__locals1);
			this.<GetVisibleFrom>g__AddLayers|34_0(item - i, item2 + i, item3, ref CS$<>8__locals1);
			this.<GetVisibleFrom>g__AddLayers|34_0(item + i, item2 - i, item3, ref CS$<>8__locals1);
			this.<GetVisibleFrom>g__AddLayers|34_0(item + i, item2 + i, item3, ref CS$<>8__locals1);
		}
	}

	// Token: 0x06002CB0 RID: 11440 RVA: 0x0010B887 File Offset: 0x00109A87
	[CompilerGenerated]
	private void <GetVisibleFrom>g__AddLayers|34_0(int groupX, int groupY, int groupLayer, ref NetworkVisibilityGrid.<>c__DisplayClass34_0 A_4)
	{
		this.<GetVisibleFrom>g__Add|34_1(groupX, groupY, groupLayer, ref A_4);
		if (groupLayer == 0)
		{
			this.<GetVisibleFrom>g__Add|34_1(groupX, groupY, 1, ref A_4);
		}
		if (groupLayer == 1)
		{
			this.<GetVisibleFrom>g__Add|34_1(groupX, groupY, 2, ref A_4);
			this.<GetVisibleFrom>g__Add|34_1(groupX, groupY, 0, ref A_4);
		}
	}

	// Token: 0x06002CB1 RID: 11441 RVA: 0x0010B8BC File Offset: 0x00109ABC
	[CompilerGenerated]
	private void <GetVisibleFrom>g__Add|34_1(int groupX, int groupY, int groupLayer, ref NetworkVisibilityGrid.<>c__DisplayClass34_0 A_4)
	{
		A_4.groups.Add(Network.Net.sv.visibility.Get(this.CoordToID(groupX, groupY, groupLayer)));
	}

	// Token: 0x0400246A RID: 9322
	public const int overworldLayer = 0;

	// Token: 0x0400246B RID: 9323
	public const int cavesLayer = 1;

	// Token: 0x0400246C RID: 9324
	public const int tunnelsLayer = 2;

	// Token: 0x0400246D RID: 9325
	public const int dynamicDungeonsFirstLayer = 10;

	// Token: 0x0400246E RID: 9326
	public int startID = 1024;

	// Token: 0x0400246F RID: 9327
	public int gridSize = 100;

	// Token: 0x04002470 RID: 9328
	public int cellCount = 32;

	// Token: 0x04002471 RID: 9329
	[FormerlySerializedAs("visibilityRadius")]
	public int visibilityRadiusFar = 2;

	// Token: 0x04002472 RID: 9330
	public int visibilityRadiusNear = 1;

	// Token: 0x04002473 RID: 9331
	public float switchTolerance = 20f;

	// Token: 0x04002474 RID: 9332
	public float cavesThreshold = -5f;

	// Token: 0x04002475 RID: 9333
	public float tunnelsThreshold = -50f;

	// Token: 0x04002476 RID: 9334
	public float dynamicDungeonsThreshold = 1000f;

	// Token: 0x04002477 RID: 9335
	public float dynamicDungeonsInterval = 100f;

	// Token: 0x04002478 RID: 9336
	private float halfGridSize;

	// Token: 0x04002479 RID: 9337
	private float cellSize;

	// Token: 0x0400247A RID: 9338
	private float halfCellSize;

	// Token: 0x0400247B RID: 9339
	private int numIDsPerLayer;
}
