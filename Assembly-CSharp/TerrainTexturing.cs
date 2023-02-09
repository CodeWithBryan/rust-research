using System;
using Rust;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

// Token: 0x0200067E RID: 1662
[ExecuteInEditMode]
public class TerrainTexturing : TerrainExtension
{
	// Token: 0x06002F9E RID: 12190 RVA: 0x000059DD File Offset: 0x00003BDD
	private void InitializeBasePyramid()
	{
	}

	// Token: 0x06002F9F RID: 12191 RVA: 0x000059DD File Offset: 0x00003BDD
	private void ReleaseBasePyramid()
	{
	}

	// Token: 0x06002FA0 RID: 12192 RVA: 0x000059DD File Offset: 0x00003BDD
	private void UpdateBasePyramid()
	{
	}

	// Token: 0x06002FA1 RID: 12193 RVA: 0x000059DD File Offset: 0x00003BDD
	private void InitializeCoarseHeightSlope()
	{
	}

	// Token: 0x06002FA2 RID: 12194 RVA: 0x000059DD File Offset: 0x00003BDD
	private void ReleaseCoarseHeightSlope()
	{
	}

	// Token: 0x06002FA3 RID: 12195 RVA: 0x000059DD File Offset: 0x00003BDD
	private void UpdateCoarseHeightSlope()
	{
	}

	// Token: 0x170003AC RID: 940
	// (get) Token: 0x06002FA4 RID: 12196 RVA: 0x0011C5AB File Offset: 0x0011A7AB
	public int ShoreMapSize
	{
		get
		{
			return this.shoreMapSize;
		}
	}

	// Token: 0x170003AD RID: 941
	// (get) Token: 0x06002FA5 RID: 12197 RVA: 0x0011C5B3 File Offset: 0x0011A7B3
	public Vector3[] ShoreMap
	{
		get
		{
			return this.shoreVectors;
		}
	}

	// Token: 0x06002FA6 RID: 12198 RVA: 0x0011C5BC File Offset: 0x0011A7BC
	private void InitializeShoreVector()
	{
		int num = Mathf.ClosestPowerOfTwo(this.terrain.terrainData.heightmapResolution) >> 3;
		int num2 = num * num;
		this.terrainSize = Mathf.Max(this.terrain.terrainData.size.x, this.terrain.terrainData.size.z);
		this.shoreMapSize = num;
		this.shoreDistances = new float[num * num];
		this.shoreVectors = new Vector3[num * num];
		for (int i = 0; i < num2; i++)
		{
			this.shoreDistances[i] = 10000f;
			this.shoreVectors[i] = Vector3.one;
		}
	}

	// Token: 0x06002FA7 RID: 12199 RVA: 0x0011C668 File Offset: 0x0011A868
	private void GenerateShoreVector()
	{
		using (TimeWarning.New("GenerateShoreVector", 500))
		{
			this.GenerateShoreVector(out this.shoreDistances, out this.shoreVectors);
		}
	}

	// Token: 0x06002FA8 RID: 12200 RVA: 0x0011C6B4 File Offset: 0x0011A8B4
	private void ReleaseShoreVector()
	{
		this.shoreDistances = null;
		this.shoreVectors = null;
	}

	// Token: 0x06002FA9 RID: 12201 RVA: 0x0011C6C4 File Offset: 0x0011A8C4
	private void GenerateShoreVector(out float[] distances, out Vector3[] vectors)
	{
		float num = this.terrainSize / (float)this.shoreMapSize;
		Vector3 position = this.terrain.GetPosition();
		int num2 = LayerMask.NameToLayer("Terrain");
		NativeArray<RaycastHit> results = new NativeArray<RaycastHit>(this.shoreMapSize * this.shoreMapSize, Allocator.TempJob, NativeArrayOptions.ClearMemory);
		NativeArray<RaycastCommand> commands = new NativeArray<RaycastCommand>(this.shoreMapSize * this.shoreMapSize, Allocator.TempJob, NativeArrayOptions.ClearMemory);
		for (int i = 0; i < this.shoreMapSize; i++)
		{
			for (int j = 0; j < this.shoreMapSize; j++)
			{
				float x = ((float)j + 0.5f) * num;
				float z = ((float)i + 0.5f) * num;
				Vector3 from = new Vector3(position.x, 0f, position.z) + new Vector3(x, 1000f, z);
				Vector3 down = Vector3.down;
				commands[i * this.shoreMapSize + j] = new RaycastCommand(from, down, float.MaxValue, -5, 1);
			}
		}
		RaycastCommand.ScheduleBatch(commands, results, 1, default(JobHandle)).Complete();
		byte[] array = new byte[this.shoreMapSize * this.shoreMapSize];
		distances = new float[this.shoreMapSize * this.shoreMapSize];
		vectors = new Vector3[this.shoreMapSize * this.shoreMapSize];
		int k = 0;
		int num3 = 0;
		while (k < this.shoreMapSize)
		{
			int l = 0;
			while (l < this.shoreMapSize)
			{
				bool flag = results[k * this.shoreMapSize + l].collider.gameObject.layer == num2;
				array[num3] = (flag ? byte.MaxValue : 0);
				distances[num3] = (float)(flag ? 256 : 0);
				l++;
				num3++;
			}
			k++;
		}
		byte b = 127;
		DistanceField.Generate(this.shoreMapSize, b, array, ref distances);
		DistanceField.ApplyGaussianBlur(this.shoreMapSize, distances, 0);
		DistanceField.GenerateVectors(this.shoreMapSize, distances, ref vectors);
		results.Dispose();
		commands.Dispose();
	}

	// Token: 0x06002FAA RID: 12202 RVA: 0x0011C8DC File Offset: 0x0011AADC
	public float GetCoarseDistanceToShore(Vector3 pos)
	{
		Vector2 uv;
		uv.x = (pos.x - TerrainMeta.Position.x) * TerrainMeta.OneOverSize.x;
		uv.y = (pos.z - TerrainMeta.Position.z) * TerrainMeta.OneOverSize.z;
		return this.GetCoarseDistanceToShore(uv);
	}

	// Token: 0x06002FAB RID: 12203 RVA: 0x0011C938 File Offset: 0x0011AB38
	public float GetCoarseDistanceToShore(Vector2 uv)
	{
		int num = this.shoreMapSize;
		int num2 = num - 1;
		float num3 = uv.x * (float)num2;
		float num4 = uv.y * (float)num2;
		int num5 = (int)num3;
		int num6 = (int)num4;
		float num7 = num3 - (float)num5;
		float num8 = num4 - (float)num6;
		num5 = ((num5 >= 0) ? num5 : 0);
		num6 = ((num6 >= 0) ? num6 : 0);
		num5 = ((num5 <= num2) ? num5 : num2);
		num6 = ((num6 <= num2) ? num6 : num2);
		int num9 = (num3 < (float)num2) ? 1 : 0;
		int num10 = (num4 < (float)num2) ? num : 0;
		int num11 = num6 * num + num5;
		int num12 = num11 + num9;
		int num13 = num11 + num10;
		int num14 = num13 + num9;
		float num15 = this.shoreDistances[num11];
		float num16 = this.shoreDistances[num12];
		float num17 = this.shoreDistances[num13];
		float num18 = this.shoreDistances[num14];
		float num19 = (num16 - num15) * num7 + num15;
		return ((num18 - num17) * num7 + num17 - num19) * num8 + num19;
	}

	// Token: 0x06002FAC RID: 12204 RVA: 0x0011CA1C File Offset: 0x0011AC1C
	public Vector3 GetCoarseVectorToShore(Vector3 pos)
	{
		Vector2 uv;
		uv.x = (pos.x - TerrainMeta.Position.x) * TerrainMeta.OneOverSize.x;
		uv.y = (pos.z - TerrainMeta.Position.z) * TerrainMeta.OneOverSize.z;
		return this.GetCoarseVectorToShore(uv);
	}

	// Token: 0x06002FAD RID: 12205 RVA: 0x0011CA78 File Offset: 0x0011AC78
	public Vector3 GetCoarseVectorToShore(Vector2 uv)
	{
		int num = this.shoreMapSize;
		int num2 = num - 1;
		float num3 = uv.x * (float)num2;
		float num4 = uv.y * (float)num2;
		int num5 = (int)num3;
		int num6 = (int)num4;
		float num7 = num3 - (float)num5;
		float num8 = num4 - (float)num6;
		num5 = ((num5 >= 0) ? num5 : 0);
		num6 = ((num6 >= 0) ? num6 : 0);
		num5 = ((num5 <= num2) ? num5 : num2);
		num6 = ((num6 <= num2) ? num6 : num2);
		int num9 = (num3 < (float)num2) ? 1 : 0;
		int num10 = (num4 < (float)num2) ? num : 0;
		int num11 = num6 * num + num5;
		int num12 = num11 + num9;
		int num13 = num11 + num10;
		int num14 = num13 + num9;
		Vector3 vector = this.shoreVectors[num11];
		Vector3 vector2 = this.shoreVectors[num12];
		Vector3 vector3 = this.shoreVectors[num13];
		Vector3 vector4 = this.shoreVectors[num14];
		Vector3 vector5;
		vector5.x = (vector2.x - vector.x) * num7 + vector.x;
		vector5.y = (vector2.y - vector.y) * num7 + vector.y;
		vector5.z = (vector2.z - vector.z) * num7 + vector.z;
		Vector3 vector6;
		vector6.x = (vector4.x - vector3.x) * num7 + vector3.x;
		vector6.y = (vector4.y - vector3.y) * num7 + vector3.y;
		vector6.z = (vector4.z - vector3.z) * num7 + vector3.z;
		float x = (vector6.x - vector5.x) * num8 + vector5.x;
		float y = (vector6.y - vector5.y) * num8 + vector5.y;
		float z = (vector6.z - vector5.z) * num8 + vector5.z;
		return new Vector3(x, y, z);
	}

	// Token: 0x170003AE RID: 942
	// (get) Token: 0x06002FAE RID: 12206 RVA: 0x0011CC70 File Offset: 0x0011AE70
	public static TerrainTexturing Instance
	{
		get
		{
			return TerrainTexturing.instance;
		}
	}

	// Token: 0x06002FAF RID: 12207 RVA: 0x0011CC77 File Offset: 0x0011AE77
	private void CheckInstance()
	{
		TerrainTexturing.instance = ((TerrainTexturing.instance != null) ? TerrainTexturing.instance : this);
	}

	// Token: 0x06002FB0 RID: 12208 RVA: 0x0011CC93 File Offset: 0x0011AE93
	private void Awake()
	{
		this.CheckInstance();
	}

	// Token: 0x06002FB1 RID: 12209 RVA: 0x0011CC9B File Offset: 0x0011AE9B
	public override void Setup()
	{
		this.InitializeShoreVector();
	}

	// Token: 0x06002FB2 RID: 12210 RVA: 0x0011CCA4 File Offset: 0x0011AEA4
	public override void PostSetup()
	{
		TerrainMeta component = base.GetComponent<TerrainMeta>();
		if (component == null || component.config == null)
		{
			Debug.LogError("[TerrainTexturing] Missing TerrainMeta or TerrainConfig not assigned.");
			return;
		}
		this.Shutdown();
		this.InitializeCoarseHeightSlope();
		this.GenerateShoreVector();
		this.initialized = true;
	}

	// Token: 0x06002FB3 RID: 12211 RVA: 0x0011CCF3 File Offset: 0x0011AEF3
	private void Shutdown()
	{
		this.ReleaseBasePyramid();
		this.ReleaseCoarseHeightSlope();
		this.ReleaseShoreVector();
		this.initialized = false;
	}

	// Token: 0x06002FB4 RID: 12212 RVA: 0x0011CC93 File Offset: 0x0011AE93
	private void OnEnable()
	{
		this.CheckInstance();
	}

	// Token: 0x06002FB5 RID: 12213 RVA: 0x0011CD0E File Offset: 0x0011AF0E
	private void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		this.Shutdown();
	}

	// Token: 0x06002FB6 RID: 12214 RVA: 0x0011CD1E File Offset: 0x0011AF1E
	private void Update()
	{
		if (!this.initialized)
		{
			return;
		}
		this.UpdateBasePyramid();
		this.UpdateCoarseHeightSlope();
	}

	// Token: 0x0400266F RID: 9839
	private const int ShoreVectorDownscale = 3;

	// Token: 0x04002670 RID: 9840
	private const int ShoreVectorBlurPasses = 0;

	// Token: 0x04002671 RID: 9841
	private float terrainSize;

	// Token: 0x04002672 RID: 9842
	private int shoreMapSize;

	// Token: 0x04002673 RID: 9843
	private float[] shoreDistances;

	// Token: 0x04002674 RID: 9844
	private Vector3[] shoreVectors;

	// Token: 0x04002675 RID: 9845
	public bool debugFoliageDisplacement;

	// Token: 0x04002676 RID: 9846
	private bool initialized;

	// Token: 0x04002677 RID: 9847
	private static TerrainTexturing instance;
}
