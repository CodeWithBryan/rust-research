using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000925 RID: 2341
public static class Bezier
{
	// Token: 0x060037DE RID: 14302 RVA: 0x0014B04B File Offset: 0x0014924B
	public static void ApplyLineSlack(ref Vector3[] positions, float[] slackLevels, int tesselationLevel)
	{
		Bezier.ApplyLineSlack(positions, slackLevels, ref positions, tesselationLevel);
	}

	// Token: 0x060037DF RID: 14303 RVA: 0x0014B058 File Offset: 0x00149258
	public static void ApplyLineSlack(Vector3[] positions, float[] slackLevels, ref Vector3[] result, int tesselationLevel)
	{
		List<Vector3> list = Pool.GetList<Vector3>();
		Bezier.ApplyLineSlack(positions, slackLevels, ref list, tesselationLevel);
		if (result.Length != list.Count)
		{
			result = new Vector3[list.Count];
		}
		list.CopyTo(result);
		Pool.FreeList<Vector3>(ref list);
	}

	// Token: 0x060037E0 RID: 14304 RVA: 0x0014B0A0 File Offset: 0x001492A0
	public static void ApplyLineSlack(Vector3[] positions, float[] slackLevels, ref List<Vector3> result, int tesselationLevel)
	{
		if (positions.Length < 2)
		{
			return;
		}
		if (slackLevels.Length == 0)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < slackLevels.Length; i++)
		{
			if (slackLevels[i] > 0f)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			result.AddRange(positions);
			return;
		}
		for (int j = 0; j < positions.Length; j++)
		{
			if (j < positions.Length - 1)
			{
				Vector3 vector = positions[j];
				Vector3 b = positions[j + 1];
				Vector3 vector2 = Vector3.Lerp(vector, b, 0.5f);
				if (j < slackLevels.Length)
				{
					vector2.y -= slackLevels[j];
				}
				result.Add(vector);
				for (int k = 0; k < tesselationLevel; k++)
				{
					float num = (float)k / (float)tesselationLevel;
					num = Mathx.RemapValClamped(num, 0f, 1f, 0.1f, 0.9f);
					Vector3 item = Vector3.Lerp(Vector3.Lerp(vector, vector2, num), Vector3.Lerp(vector2, b, num), num);
					result.Add(item);
				}
			}
			else
			{
				result.Add(positions[j]);
			}
		}
	}
}
