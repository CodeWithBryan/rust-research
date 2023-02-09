using System;
using UnityEngine;

// Token: 0x020001DD RID: 477
public class HumanPathFinder : BasePathFinder
{
	// Token: 0x06001912 RID: 6418 RVA: 0x000B642E File Offset: 0x000B462E
	public void Init(BaseEntity npc)
	{
		this.npc = npc;
	}

	// Token: 0x06001913 RID: 6419 RVA: 0x000B6438 File Offset: 0x000B4638
	public override AIMovePoint GetBestRoamPoint(Vector3 anchorPos, Vector3 currentPos, Vector3 currentDirection, float anchorClampDistance, float lookupMaxRange = 20f)
	{
		AIInformationZone aiinformationZone = null;
		HumanNPC humanNPC;
		if ((humanNPC = (this.npc as HumanNPC)) != null)
		{
			if (humanNPC.VirtualInfoZone != null)
			{
				aiinformationZone = humanNPC.VirtualInfoZone;
			}
			else
			{
				aiinformationZone = humanNPC.GetInformationZone(currentPos);
			}
		}
		if (aiinformationZone == null)
		{
			return null;
		}
		return this.GetBestRoamPoint(aiinformationZone, anchorPos, currentPos, currentDirection, anchorClampDistance, lookupMaxRange);
	}

	// Token: 0x06001914 RID: 6420 RVA: 0x000B6490 File Offset: 0x000B4690
	private AIMovePoint GetBestRoamPoint(AIInformationZone aiZone, Vector3 anchorPos, Vector3 currentPos, Vector3 currentDirection, float clampDistance, float lookupMaxRange)
	{
		if (aiZone == null)
		{
			return null;
		}
		bool flag = clampDistance > -1f;
		float num = float.NegativeInfinity;
		AIPoint aipoint = null;
		int num2;
		AIPoint[] movePointsInRange = aiZone.GetMovePointsInRange(anchorPos, lookupMaxRange, out num2);
		if (movePointsInRange == null || num2 <= 0)
		{
			return null;
		}
		for (int i = 0; i < num2; i++)
		{
			AIPoint aipoint2 = movePointsInRange[i];
			if (aipoint2.transform.parent.gameObject.activeSelf)
			{
				float num3 = Mathf.Abs(currentPos.y - aipoint2.transform.position.y);
				bool flag2 = currentPos.y < WaterSystem.OceanLevel;
				if (flag2 || ((flag2 || aipoint2.transform.position.y >= WaterSystem.OceanLevel) && (currentPos.y < WaterSystem.OceanLevel || num3 <= 5f)))
				{
					float num4 = 0f;
					float value = Vector3.Dot(currentDirection, Vector3Ex.Direction2D(aipoint2.transform.position, currentPos));
					num4 += Mathf.InverseLerp(-1f, 1f, value) * 100f;
					if (!aipoint2.InUse())
					{
						num4 += 1000f;
					}
					num4 += (1f - Mathf.InverseLerp(1f, 10f, num3)) * 100f;
					float num5 = Vector3.Distance(currentPos, aipoint2.transform.position);
					if (num5 <= 1f)
					{
						num4 -= 3000f;
					}
					if (flag)
					{
						float num6 = Vector3.Distance(anchorPos, aipoint2.transform.position);
						if (num6 <= clampDistance)
						{
							num4 += 1000f;
							num4 += (1f - Mathf.InverseLerp(0f, clampDistance, num6)) * 200f * UnityEngine.Random.Range(0.8f, 1f);
						}
					}
					else if (num5 > 3f)
					{
						num4 += Mathf.InverseLerp(3f, lookupMaxRange, num5) * 50f;
					}
					if (num4 > num)
					{
						aipoint = aipoint2;
						num = num4;
					}
				}
			}
		}
		return aipoint as AIMovePoint;
	}

	// Token: 0x040011E0 RID: 4576
	private BaseEntity npc;
}
