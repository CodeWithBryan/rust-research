using System;
using UnityEngine;

// Token: 0x02000671 RID: 1649
public abstract class TerrainMap : TerrainExtension
{
	// Token: 0x06002EC6 RID: 11974 RVA: 0x00117FF4 File Offset: 0x001161F4
	public void ApplyFilter(float normX, float normZ, float radius, float fade, Action<int, int, float> action)
	{
		float num = TerrainMeta.OneOverSize.x * radius;
		float num2 = TerrainMeta.OneOverSize.x * fade;
		float num3 = (float)this.res * (num - num2);
		float num4 = (float)this.res * num;
		float num5 = normX * (float)this.res;
		float num6 = normZ * (float)this.res;
		int num7 = this.Index(normX - num);
		int num8 = this.Index(normX + num);
		int num9 = this.Index(normZ - num);
		int num10 = this.Index(normZ + num);
		if (num3 != num4)
		{
			for (int i = num9; i <= num10; i++)
			{
				for (int j = num7; j <= num8; j++)
				{
					float magnitude = new Vector2((float)j + 0.5f - num5, (float)i + 0.5f - num6).magnitude;
					float arg = Mathf.InverseLerp(num4, num3, magnitude);
					action(j, i, arg);
				}
			}
			return;
		}
		for (int k = num9; k <= num10; k++)
		{
			for (int l = num7; l <= num8; l++)
			{
				float arg2 = (float)((new Vector2((float)l + 0.5f - num5, (float)k + 0.5f - num6).magnitude < num4) ? 1 : 0);
				action(l, k, arg2);
			}
		}
	}

	// Token: 0x06002EC7 RID: 11975 RVA: 0x00118140 File Offset: 0x00116340
	public void ForEach(Vector3 worldPos, float radius, Action<int, int> action)
	{
		int num = this.Index(TerrainMeta.NormalizeX(worldPos.x - radius));
		int num2 = this.Index(TerrainMeta.NormalizeX(worldPos.x + radius));
		int num3 = this.Index(TerrainMeta.NormalizeZ(worldPos.z - radius));
		int num4 = this.Index(TerrainMeta.NormalizeZ(worldPos.z + radius));
		for (int i = num3; i <= num4; i++)
		{
			for (int j = num; j <= num2; j++)
			{
				action(j, i);
			}
		}
	}

	// Token: 0x06002EC8 RID: 11976 RVA: 0x001181C0 File Offset: 0x001163C0
	public void ForEach(float normX, float normZ, float normRadius, Action<int, int> action)
	{
		int num = this.Index(normX - normRadius);
		int num2 = this.Index(normX + normRadius);
		int num3 = this.Index(normZ - normRadius);
		int num4 = this.Index(normZ + normRadius);
		for (int i = num3; i <= num4; i++)
		{
			for (int j = num; j <= num2; j++)
			{
				action(j, i);
			}
		}
	}

	// Token: 0x06002EC9 RID: 11977 RVA: 0x0011821C File Offset: 0x0011641C
	public void ForEachParallel(Vector3 v0, Vector3 v1, Vector3 v2, Action<int, int> action)
	{
		Vector2i v3 = new Vector2i(this.Index(TerrainMeta.NormalizeX(v0.x)), this.Index(TerrainMeta.NormalizeZ(v0.z)));
		Vector2i v4 = new Vector2i(this.Index(TerrainMeta.NormalizeX(v1.x)), this.Index(TerrainMeta.NormalizeZ(v1.z)));
		Vector2i v5 = new Vector2i(this.Index(TerrainMeta.NormalizeX(v2.x)), this.Index(TerrainMeta.NormalizeZ(v2.z)));
		this.ForEachParallel(v3, v4, v5, action);
	}

	// Token: 0x06002ECA RID: 11978 RVA: 0x001182B0 File Offset: 0x001164B0
	public void ForEachParallel(Vector2i v0, Vector2i v1, Vector2i v2, Action<int, int> action)
	{
		int x = Mathx.Min(v0.x, v1.x, v2.x);
		int x2 = Mathx.Max(v0.x, v1.x, v2.x);
		int y = Mathx.Min(v0.y, v1.y, v2.y);
		int y2 = Mathx.Max(v0.y, v1.y, v2.y);
		Vector2i base_min = new Vector2i(x, y);
		Vector2i a = new Vector2i(x2, y2);
		Vector2i base_count = a - base_min + Vector2i.one;
		Parallel.Call(delegate(int thread_id, int thread_count)
		{
			Vector2i min = base_min + base_count * thread_id / thread_count;
			Vector2i max = base_min + base_count * (thread_id + 1) / thread_count - Vector2i.one;
			this.ForEachInternal(v0, v1, v2, action, min, max);
		});
	}

	// Token: 0x06002ECB RID: 11979 RVA: 0x001183CC File Offset: 0x001165CC
	public void ForEach(Vector3 v0, Vector3 v1, Vector3 v2, Action<int, int> action)
	{
		Vector2i v3 = new Vector2i(this.Index(TerrainMeta.NormalizeX(v0.x)), this.Index(TerrainMeta.NormalizeZ(v0.z)));
		Vector2i v4 = new Vector2i(this.Index(TerrainMeta.NormalizeX(v1.x)), this.Index(TerrainMeta.NormalizeZ(v1.z)));
		Vector2i v5 = new Vector2i(this.Index(TerrainMeta.NormalizeX(v2.x)), this.Index(TerrainMeta.NormalizeZ(v2.z)));
		this.ForEach(v3, v4, v5, action);
	}

	// Token: 0x06002ECC RID: 11980 RVA: 0x00118460 File Offset: 0x00116660
	public void ForEach(Vector2i v0, Vector2i v1, Vector2i v2, Action<int, int> action)
	{
		Vector2i min = new Vector2i(int.MinValue, int.MinValue);
		Vector2i max = new Vector2i(int.MaxValue, int.MaxValue);
		this.ForEachInternal(v0, v1, v2, action, min, max);
	}

	// Token: 0x06002ECD RID: 11981 RVA: 0x0011849C File Offset: 0x0011669C
	private void ForEachInternal(Vector2i v0, Vector2i v1, Vector2i v2, Action<int, int> action, Vector2i min, Vector2i max)
	{
		int x = Mathf.Max(min.x, Mathx.Min(v0.x, v1.x, v2.x));
		int num = Mathf.Min(max.x, Mathx.Max(v0.x, v1.x, v2.x));
		int y = Mathf.Max(min.y, Mathx.Min(v0.y, v1.y, v2.y));
		int num2 = Mathf.Min(max.y, Mathx.Max(v0.y, v1.y, v2.y));
		int num3 = v0.y - v1.y;
		int num4 = v1.x - v0.x;
		int num5 = v1.y - v2.y;
		int num6 = v2.x - v1.x;
		int num7 = v2.y - v0.y;
		int num8 = v0.x - v2.x;
		Vector2i vector2i = new Vector2i(x, y);
		int num9 = (v2.x - v1.x) * (vector2i.y - v1.y) - (v2.y - v1.y) * (vector2i.x - v1.x);
		int num10 = (v0.x - v2.x) * (vector2i.y - v2.y) - (v0.y - v2.y) * (vector2i.x - v2.x);
		int num11 = (v1.x - v0.x) * (vector2i.y - v0.y) - (v1.y - v0.y) * (vector2i.x - v0.x);
		vector2i.y = y;
		while (vector2i.y <= num2)
		{
			int num12 = num9;
			int num13 = num10;
			int num14 = num11;
			vector2i.x = x;
			while (vector2i.x <= num)
			{
				if ((num12 | num13 | num14) >= 0)
				{
					action(vector2i.x, vector2i.y);
				}
				num12 += num5;
				num13 += num7;
				num14 += num3;
				vector2i.x++;
			}
			num9 += num6;
			num10 += num8;
			num11 += num4;
			vector2i.y++;
		}
	}

	// Token: 0x06002ECE RID: 11982 RVA: 0x001186EC File Offset: 0x001168EC
	public void ForEachParallel(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Action<int, int> action)
	{
		Vector2i v4 = new Vector2i(this.Index(TerrainMeta.NormalizeX(v0.x)), this.Index(TerrainMeta.NormalizeZ(v0.z)));
		Vector2i v5 = new Vector2i(this.Index(TerrainMeta.NormalizeX(v1.x)), this.Index(TerrainMeta.NormalizeZ(v1.z)));
		Vector2i v6 = new Vector2i(this.Index(TerrainMeta.NormalizeX(v2.x)), this.Index(TerrainMeta.NormalizeZ(v2.z)));
		Vector2i v7 = new Vector2i(this.Index(TerrainMeta.NormalizeX(v3.x)), this.Index(TerrainMeta.NormalizeZ(v3.z)));
		this.ForEachParallel(v4, v5, v6, v7, action);
	}

	// Token: 0x06002ECF RID: 11983 RVA: 0x001187AC File Offset: 0x001169AC
	public void ForEachParallel(Vector2i v0, Vector2i v1, Vector2i v2, Vector2i v3, Action<int, int> action)
	{
		int x = Mathx.Min(v0.x, v1.x, v2.x, v3.x);
		int x2 = Mathx.Max(v0.x, v1.x, v2.x, v3.x);
		int y = Mathx.Min(v0.y, v1.y, v2.y, v3.y);
		int y2 = Mathx.Max(v0.y, v1.y, v2.y, v3.y);
		Vector2i base_min = new Vector2i(x, y);
		Vector2i vector2i = new Vector2i(x2, y2) - base_min + Vector2i.one;
		Vector2i size_x = new Vector2i(vector2i.x, 0);
		Vector2i size_y = new Vector2i(0, vector2i.y);
		Parallel.Call(delegate(int thread_id, int thread_count)
		{
			Vector2i min = base_min + size_y * thread_id / thread_count;
			Vector2i max = base_min + size_y * (thread_id + 1) / thread_count + size_x - Vector2i.one;
			this.ForEachInternal(v0, v1, v2, v3, action, min, max);
		});
	}

	// Token: 0x06002ED0 RID: 11984 RVA: 0x00118918 File Offset: 0x00116B18
	public void ForEach(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Action<int, int> action)
	{
		Vector2i v4 = new Vector2i(this.Index(TerrainMeta.NormalizeX(v0.x)), this.Index(TerrainMeta.NormalizeZ(v0.z)));
		Vector2i v5 = new Vector2i(this.Index(TerrainMeta.NormalizeX(v1.x)), this.Index(TerrainMeta.NormalizeZ(v1.z)));
		Vector2i v6 = new Vector2i(this.Index(TerrainMeta.NormalizeX(v2.x)), this.Index(TerrainMeta.NormalizeZ(v2.z)));
		Vector2i v7 = new Vector2i(this.Index(TerrainMeta.NormalizeX(v3.x)), this.Index(TerrainMeta.NormalizeZ(v3.z)));
		this.ForEach(v4, v5, v6, v7, action);
	}

	// Token: 0x06002ED1 RID: 11985 RVA: 0x001189D8 File Offset: 0x00116BD8
	public void ForEach(Vector2i v0, Vector2i v1, Vector2i v2, Vector2i v3, Action<int, int> action)
	{
		Vector2i min = new Vector2i(int.MinValue, int.MinValue);
		Vector2i max = new Vector2i(int.MaxValue, int.MaxValue);
		this.ForEachInternal(v0, v1, v2, v3, action, min, max);
	}

	// Token: 0x06002ED2 RID: 11986 RVA: 0x00118A18 File Offset: 0x00116C18
	private void ForEachInternal(Vector2i v0, Vector2i v1, Vector2i v2, Vector2i v3, Action<int, int> action, Vector2i min, Vector2i max)
	{
		int x = Mathf.Max(min.x, Mathx.Min(v0.x, v1.x, v2.x, v3.x));
		int num = Mathf.Min(max.x, Mathx.Max(v0.x, v1.x, v2.x, v3.x));
		int y = Mathf.Max(min.y, Mathx.Min(v0.y, v1.y, v2.y, v3.y));
		int num2 = Mathf.Min(max.y, Mathx.Max(v0.y, v1.y, v2.y, v3.y));
		int num3 = v0.y - v1.y;
		int num4 = v1.x - v0.x;
		int num5 = v1.y - v2.y;
		int num6 = v2.x - v1.x;
		int num7 = v2.y - v0.y;
		int num8 = v0.x - v2.x;
		int num9 = v3.y - v2.y;
		int num10 = v2.x - v3.x;
		int num11 = v2.y - v1.y;
		int num12 = v1.x - v2.x;
		int num13 = v1.y - v3.y;
		int num14 = v3.x - v1.x;
		Vector2i vector2i = new Vector2i(x, y);
		int num15 = (v2.x - v1.x) * (vector2i.y - v1.y) - (v2.y - v1.y) * (vector2i.x - v1.x);
		int num16 = (v0.x - v2.x) * (vector2i.y - v2.y) - (v0.y - v2.y) * (vector2i.x - v2.x);
		int num17 = (v1.x - v0.x) * (vector2i.y - v0.y) - (v1.y - v0.y) * (vector2i.x - v0.x);
		int num18 = (v1.x - v2.x) * (vector2i.y - v2.y) - (v1.y - v2.y) * (vector2i.x - v2.x);
		int num19 = (v3.x - v1.x) * (vector2i.y - v1.y) - (v3.y - v1.y) * (vector2i.x - v1.x);
		int num20 = (v2.x - v3.x) * (vector2i.y - v3.y) - (v2.y - v3.y) * (vector2i.x - v3.x);
		vector2i.y = y;
		while (vector2i.y <= num2)
		{
			int num21 = num15;
			int num22 = num16;
			int num23 = num17;
			int num24 = num18;
			int num25 = num19;
			int num26 = num20;
			vector2i.x = x;
			while (vector2i.x <= num)
			{
				if ((num21 | num22 | num23) >= 0 || (num24 | num25 | num26) >= 0)
				{
					action(vector2i.x, vector2i.y);
				}
				num21 += num5;
				num22 += num7;
				num23 += num3;
				num24 += num11;
				num25 += num13;
				num26 += num9;
				vector2i.x++;
			}
			num15 += num6;
			num16 += num8;
			num17 += num4;
			num18 += num12;
			num19 += num14;
			num20 += num10;
			vector2i.y++;
		}
	}

	// Token: 0x06002ED3 RID: 11987 RVA: 0x00118DD8 File Offset: 0x00116FD8
	public void ForEach(int x_min, int x_max, int z_min, int z_max, Action<int, int> action)
	{
		for (int i = z_min; i <= z_max; i++)
		{
			for (int j = x_min; j <= x_max; j++)
			{
				action(j, i);
			}
		}
	}

	// Token: 0x06002ED4 RID: 11988 RVA: 0x00118E08 File Offset: 0x00117008
	public void ForEach(Action<int, int> action)
	{
		for (int i = 0; i < this.res; i++)
		{
			for (int j = 0; j < this.res; j++)
			{
				action(j, i);
			}
		}
	}

	// Token: 0x06002ED5 RID: 11989 RVA: 0x00118E40 File Offset: 0x00117040
	public int Index(float normalized)
	{
		int num = (int)(normalized * (float)this.res);
		if (num < 0)
		{
			return 0;
		}
		if (num <= this.res - 1)
		{
			return num;
		}
		return this.res - 1;
	}

	// Token: 0x06002ED6 RID: 11990 RVA: 0x00118E73 File Offset: 0x00117073
	public float Coordinate(int index)
	{
		return ((float)index + 0.5f) / (float)this.res;
	}

	// Token: 0x0400262B RID: 9771
	internal int res;
}
