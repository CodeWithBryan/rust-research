using System;
using UnityEngine;

// Token: 0x020001CB RID: 459
public class AITraversalArea : TriggerBase
{
	// Token: 0x0600185D RID: 6237 RVA: 0x000B3D63 File Offset: 0x000B1F63
	public void OnValidate()
	{
		this.movementArea.center = base.transform.position;
	}

	// Token: 0x0600185E RID: 6238 RVA: 0x000B3D7C File Offset: 0x000B1F7C
	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		if (!baseEntity.IsNpc)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x0600185F RID: 6239 RVA: 0x000B3DC9 File Offset: 0x000B1FC9
	public bool CanTraverse(BaseEntity ent)
	{
		return Time.time > this.nextFreeTime;
	}

	// Token: 0x06001860 RID: 6240 RVA: 0x000B3DD8 File Offset: 0x000B1FD8
	public Transform GetClosestEntry(Vector3 position)
	{
		float num = Vector3.Distance(position, this.entryPoint1.position);
		float num2 = Vector3.Distance(position, this.entryPoint2.position);
		if (num < num2)
		{
			return this.entryPoint1;
		}
		return this.entryPoint2;
	}

	// Token: 0x06001861 RID: 6241 RVA: 0x000B3E18 File Offset: 0x000B2018
	public Transform GetFarthestEntry(Vector3 position)
	{
		float num = Vector3.Distance(position, this.entryPoint1.position);
		float num2 = Vector3.Distance(position, this.entryPoint2.position);
		if (num > num2)
		{
			return this.entryPoint1;
		}
		return this.entryPoint2;
	}

	// Token: 0x06001862 RID: 6242 RVA: 0x000B3E58 File Offset: 0x000B2058
	public void SetBusyFor(float dur = 1f)
	{
		this.nextFreeTime = Time.time + dur;
	}

	// Token: 0x06001863 RID: 6243 RVA: 0x000B3DC9 File Offset: 0x000B1FC9
	public bool CanUse(Vector3 dirFrom)
	{
		return Time.time > this.nextFreeTime;
	}

	// Token: 0x06001864 RID: 6244 RVA: 0x000B3E67 File Offset: 0x000B2067
	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
	}

	// Token: 0x06001865 RID: 6245 RVA: 0x000B3E70 File Offset: 0x000B2070
	public AITraversalWaitPoint GetEntryPointNear(Vector3 pos)
	{
		Vector3 position = this.GetClosestEntry(pos).position;
		Vector3 position2 = this.GetFarthestEntry(pos).position;
		new BaseEntity[1];
		AITraversalWaitPoint result = null;
		float num = 0f;
		foreach (AITraversalWaitPoint aitraversalWaitPoint in this.waitPoints)
		{
			if (!aitraversalWaitPoint.Occupied())
			{
				Vector3 position3 = aitraversalWaitPoint.transform.position;
				float num2 = Vector3.Distance(position, position3);
				if (Vector3.Distance(position2, position3) >= num2)
				{
					float value = Vector3.Distance(position3, pos);
					float num3 = (1f - Mathf.InverseLerp(0f, 20f, value)) * 100f;
					if (num3 > num)
					{
						num = num3;
						result = aitraversalWaitPoint;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06001866 RID: 6246 RVA: 0x000B3F2A File Offset: 0x000B212A
	public bool EntityFilter(BaseEntity ent)
	{
		return ent.IsNpc && ent.isServer;
	}

	// Token: 0x06001867 RID: 6247 RVA: 0x000B3F3C File Offset: 0x000B213C
	internal override void OnEntityLeave(BaseEntity ent)
	{
		base.OnEntityLeave(ent);
	}

	// Token: 0x06001868 RID: 6248 RVA: 0x000B3F48 File Offset: 0x000B2148
	public void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawCube(this.entryPoint1.position + Vector3.up * 0.125f, new Vector3(0.5f, 0.25f, 0.5f));
		Gizmos.DrawCube(this.entryPoint2.position + Vector3.up * 0.125f, new Vector3(0.5f, 0.25f, 0.5f));
		Gizmos.color = new Color(0.2f, 1f, 0.2f, 0.5f);
		Gizmos.DrawCube(this.movementArea.center, this.movementArea.size);
		Gizmos.color = Color.magenta;
		AITraversalWaitPoint[] array = this.waitPoints;
		for (int i = 0; i < array.Length; i++)
		{
			GizmosUtil.DrawCircleY(array[i].transform.position, 0.5f);
		}
	}

	// Token: 0x0400117F RID: 4479
	public Transform entryPoint1;

	// Token: 0x04001180 RID: 4480
	public Transform entryPoint2;

	// Token: 0x04001181 RID: 4481
	public AITraversalWaitPoint[] waitPoints;

	// Token: 0x04001182 RID: 4482
	public Bounds movementArea;

	// Token: 0x04001183 RID: 4483
	public Transform activeEntryPoint;

	// Token: 0x04001184 RID: 4484
	public float nextFreeTime;
}
