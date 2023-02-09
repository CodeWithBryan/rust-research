using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000198 RID: 408
public class BasePath : MonoBehaviour
{
	// Token: 0x0600178D RID: 6029 RVA: 0x000059DD File Offset: 0x00003BDD
	public void Start()
	{
	}

	// Token: 0x0600178E RID: 6030 RVA: 0x000AFCBC File Offset: 0x000ADEBC
	private void AddChildren()
	{
		if (this.nodes != null)
		{
			this.nodes.Clear();
			this.nodes.AddRange(base.GetComponentsInChildren<BasePathNode>());
			foreach (BasePathNode basePathNode in this.nodes)
			{
				basePathNode.Path = this;
			}
		}
		if (this.interestZones != null)
		{
			this.interestZones.Clear();
			this.interestZones.AddRange(base.GetComponentsInChildren<PathInterestNode>());
		}
		if (this.speedZones != null)
		{
			this.speedZones.Clear();
			this.speedZones.AddRange(base.GetComponentsInChildren<PathSpeedZone>());
		}
	}

	// Token: 0x0600178F RID: 6031 RVA: 0x000AFD7C File Offset: 0x000ADF7C
	private void ClearChildren()
	{
		if (this.nodes != null)
		{
			foreach (BasePathNode basePathNode in this.nodes)
			{
				basePathNode.linked.Clear();
			}
		}
		this.nodes.Clear();
	}

	// Token: 0x06001790 RID: 6032 RVA: 0x000AFDE4 File Offset: 0x000ADFE4
	public static void AutoGenerateLinks(BasePath path)
	{
		path.AddChildren();
		foreach (BasePathNode basePathNode in path.nodes)
		{
			if (basePathNode.linked == null)
			{
				basePathNode.linked = new List<BasePathNode>();
			}
			else
			{
				basePathNode.linked.Clear();
			}
			foreach (BasePathNode basePathNode2 in path.nodes)
			{
				if (!(basePathNode == basePathNode2) && GamePhysics.LineOfSight(basePathNode.transform.position, basePathNode2.transform.position, 429990145, null) && GamePhysics.LineOfSight(basePathNode2.transform.position, basePathNode.transform.position, 429990145, null))
				{
					basePathNode.linked.Add(basePathNode2);
				}
			}
		}
	}

	// Token: 0x06001791 RID: 6033 RVA: 0x000AFEF4 File Offset: 0x000AE0F4
	public void GetNodesNear(Vector3 point, ref List<BasePathNode> nearNodes, float dist = 10f)
	{
		foreach (BasePathNode basePathNode in this.nodes)
		{
			if ((Vector3Ex.XZ(point) - Vector3Ex.XZ(basePathNode.transform.position)).sqrMagnitude <= dist * dist)
			{
				nearNodes.Add(basePathNode);
			}
		}
	}

	// Token: 0x06001792 RID: 6034 RVA: 0x000AFF70 File Offset: 0x000AE170
	public BasePathNode GetClosestToPoint(Vector3 point)
	{
		BasePathNode result = this.nodes[0];
		float num = float.PositiveInfinity;
		foreach (BasePathNode basePathNode in this.nodes)
		{
			if (!(basePathNode == null) && !(basePathNode.transform == null))
			{
				float sqrMagnitude = (point - basePathNode.transform.position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					result = basePathNode;
				}
			}
		}
		return result;
	}

	// Token: 0x06001793 RID: 6035 RVA: 0x000B0010 File Offset: 0x000AE210
	public PathInterestNode GetRandomInterestNodeAwayFrom(Vector3 from, float dist = 10f)
	{
		PathInterestNode pathInterestNode = null;
		int num = 0;
		while (pathInterestNode == null && num < 20)
		{
			pathInterestNode = this.interestZones[UnityEngine.Random.Range(0, this.interestZones.Count)];
			if ((pathInterestNode.transform.position - from).sqrMagnitude >= 100f)
			{
				break;
			}
			pathInterestNode = null;
			num++;
		}
		if (pathInterestNode == null)
		{
			pathInterestNode = this.interestZones[0];
		}
		return pathInterestNode;
	}

	// Token: 0x04001083 RID: 4227
	public List<BasePathNode> nodes;

	// Token: 0x04001084 RID: 4228
	public List<PathInterestNode> interestZones;

	// Token: 0x04001085 RID: 4229
	public List<PathSpeedZone> speedZones;
}
