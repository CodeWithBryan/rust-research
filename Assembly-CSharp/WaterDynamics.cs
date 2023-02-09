using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x020006D3 RID: 1747
[ExecuteInEditMode]
public class WaterDynamics : MonoBehaviour
{
	// Token: 0x060030C9 RID: 12489 RVA: 0x0012C20A File Offset: 0x0012A40A
	private void RasterBindImage(WaterDynamics.Image image)
	{
		this.imageDesc = image.desc;
		this.imagePixels = image.pixels;
	}

	// Token: 0x060030CA RID: 12490 RVA: 0x0012C224 File Offset: 0x0012A424
	private void RasterBindTarget(WaterDynamics.Target target)
	{
		this.targetDesc = target.Desc;
		this.targetPixels = target.Pixels;
		this.targetDrawTileTable = target.DrawTileTable;
		this.targetDrawTileList = target.DrawTileList;
	}

	// Token: 0x060030CB RID: 12491 RVA: 0x0012C258 File Offset: 0x0012A458
	private void RasterInteraction(Vector2 pos, Vector2 scale, float rotation, float disp, float dist)
	{
		Vector2 a = this.targetDesc.WorldToRaster(pos);
		float f = -rotation * 0.017453292f;
		float s = Mathf.Sin(f);
		float c = Mathf.Cos(f);
		float num = Mathf.Min((float)this.imageDesc.width * scale.x, 1024f) * 0.5f;
		float num2 = Mathf.Min((float)this.imageDesc.height * scale.y, 1024f) * 0.5f;
		Vector2 vector = a + this.Rotate2D(new Vector2(-num, -num2), s, c);
		Vector2 vector2 = a + this.Rotate2D(new Vector2(num, -num2), s, c);
		Vector2 vector3 = a + this.Rotate2D(new Vector2(num, num2), s, c);
		Vector2 vector4 = a + this.Rotate2D(new Vector2(-num, num2), s, c);
		WaterDynamics.Point2D p = new WaterDynamics.Point2D(vector.x * 256f, vector.y * 256f);
		WaterDynamics.Point2D p2 = new WaterDynamics.Point2D(vector2.x * 256f, vector2.y * 256f);
		WaterDynamics.Point2D point2D = new WaterDynamics.Point2D(vector3.x * 256f, vector3.y * 256f);
		WaterDynamics.Point2D p3 = new WaterDynamics.Point2D(vector4.x * 256f, vector4.y * 256f);
		Vector2 uv = new Vector2(-0.5f, -0.5f);
		Vector2 uv2 = new Vector2((float)this.imageDesc.width - 0.5f, -0.5f);
		Vector2 vector5 = new Vector2((float)this.imageDesc.width - 0.5f, (float)this.imageDesc.height - 0.5f);
		Vector2 uv3 = new Vector2(-0.5f, (float)this.imageDesc.height - 0.5f);
		byte disp2 = (byte)(disp * 255f);
		byte dist2 = (byte)(dist * 255f);
		this.RasterizeTriangle(p, p2, point2D, uv, uv2, vector5, disp2, dist2);
		this.RasterizeTriangle(p, point2D, p3, uv, vector5, uv3, disp2, dist2);
	}

	// Token: 0x060030CC RID: 12492 RVA: 0x0012C46B File Offset: 0x0012A66B
	private float Frac(float x)
	{
		return x - (float)((int)x);
	}

	// Token: 0x060030CD RID: 12493 RVA: 0x0012C474 File Offset: 0x0012A674
	private Vector2 Rotate2D(Vector2 v, float s, float c)
	{
		Vector2 result;
		result.x = v.x * c - v.y * s;
		result.y = v.y * c + v.x * s;
		return result;
	}

	// Token: 0x060030CE RID: 12494 RVA: 0x0012C4B2 File Offset: 0x0012A6B2
	private int Min3(int a, int b, int c)
	{
		return Mathf.Min(a, Mathf.Min(b, c));
	}

	// Token: 0x060030CF RID: 12495 RVA: 0x0012C4C1 File Offset: 0x0012A6C1
	private int Max3(int a, int b, int c)
	{
		return Mathf.Max(a, Mathf.Max(b, c));
	}

	// Token: 0x060030D0 RID: 12496 RVA: 0x0012C4D0 File Offset: 0x0012A6D0
	private int EdgeFunction(WaterDynamics.Point2D a, WaterDynamics.Point2D b, WaterDynamics.Point2D c)
	{
		return (int)(((long)(b.x - a.x) * (long)(c.y - a.y) >> 8) - ((long)(b.y - a.y) * (long)(c.x - a.x) >> 8));
	}

	// Token: 0x060030D1 RID: 12497 RVA: 0x0012C51D File Offset: 0x0012A71D
	private bool IsTopLeft(WaterDynamics.Point2D a, WaterDynamics.Point2D b)
	{
		return (a.y == b.y && a.x < b.x) || a.y > b.y;
	}

	// Token: 0x060030D2 RID: 12498 RVA: 0x0012C54C File Offset: 0x0012A74C
	private void RasterizeTriangle(WaterDynamics.Point2D p0, WaterDynamics.Point2D p1, WaterDynamics.Point2D p2, Vector2 uv0, Vector2 uv1, Vector2 uv2, byte disp, byte dist)
	{
		int width = this.imageDesc.width;
		int widthShift = this.imageDesc.widthShift;
		int maxWidth = this.imageDesc.maxWidth;
		int maxHeight = this.imageDesc.maxHeight;
		int size = this.targetDesc.size;
		int tileCount = this.targetDesc.tileCount;
		int num = Mathf.Max(this.Min3(p0.x, p1.x, p2.x), 0);
		int num2 = Mathf.Max(this.Min3(p0.y, p1.y, p2.y), 0);
		int num3 = Mathf.Min(this.Max3(p0.x, p1.x, p2.x), this.targetDesc.maxSizeSubStep);
		int num4 = Mathf.Min(this.Max3(p0.y, p1.y, p2.y), this.targetDesc.maxSizeSubStep);
		int num5 = Mathf.Max(num >> 8 >> this.targetDesc.tileSizeShift, 0);
		int num6 = Mathf.Min(num3 >> 8 >> this.targetDesc.tileSizeShift, this.targetDesc.tileMaxCount);
		int num7 = Mathf.Max(num2 >> 8 >> this.targetDesc.tileSizeShift, 0);
		int num8 = Mathf.Min(num4 >> 8 >> this.targetDesc.tileSizeShift, this.targetDesc.tileMaxCount);
		for (int i = num7; i <= num8; i++)
		{
			int num9 = i * tileCount;
			for (int j = num5; j <= num6; j++)
			{
				int num10 = num9 + j;
				if (this.targetDrawTileTable[num10] == 0)
				{
					this.targetDrawTileTable[num10] = 1;
					this.targetDrawTileList.Add((ushort)num10);
				}
			}
		}
		num = (num + 255 & -256);
		num2 = (num2 + 255 & -256);
		int num11 = this.IsTopLeft(p1, p2) ? 0 : -1;
		int num12 = this.IsTopLeft(p2, p0) ? 0 : -1;
		int num13 = this.IsTopLeft(p0, p1) ? 0 : -1;
		WaterDynamics.Point2D c = new WaterDynamics.Point2D(num, num2);
		int num14 = this.EdgeFunction(p1, p2, c) + num11;
		int num15 = this.EdgeFunction(p2, p0, c) + num12;
		int num16 = this.EdgeFunction(p0, p1, c) + num13;
		int num17 = p1.y - p2.y;
		int num18 = p2.y - p0.y;
		int num19 = p0.y - p1.y;
		int num20 = p2.x - p1.x;
		int num21 = p0.x - p2.x;
		int num22 = p1.x - p0.x;
		float num23 = 16777216f / (float)this.EdgeFunction(p0, p1, p2);
		float num24 = uv0.x * 65536f;
		float num25 = uv0.y * 65536f;
		float num26 = (uv1.x - uv0.x) * num23;
		float num27 = (uv1.y - uv0.y) * num23;
		float num28 = (uv2.x - uv0.x) * num23;
		float num29 = (uv2.y - uv0.y) * num23;
		int num30 = (int)((float)num18 * 0.00390625f * num26 + (float)num19 * 0.00390625f * num28);
		int num31 = (int)((float)num18 * 0.00390625f * num27 + (float)num19 * 0.00390625f * num29);
		for (int k = num2; k <= num4; k += 256)
		{
			int num32 = num14;
			int num33 = num15;
			int num34 = num16;
			int num35 = (int)(num24 + num26 * 0.00390625f * (float)num33 + num28 * 0.00390625f * (float)num34);
			int num36 = (int)(num25 + num27 * 0.00390625f * (float)num33 + num29 * 0.00390625f * (float)num34);
			for (int l = num; l <= num3; l += 256)
			{
				if ((num32 | num33 | num34) >= 0)
				{
					int num37 = (num35 > 0) ? num35 : 0;
					object obj = (num36 > 0) ? num36 : 0;
					int num38 = num37 >> 16;
					object obj2 = obj;
					int num39 = obj2 >> 16;
					byte b = (byte)((num37 & 65535) >> 8);
					byte b2 = (obj2 & 65535) >> 8;
					num38 = ((num38 > 0) ? num38 : 0);
					num39 = ((num39 > 0) ? num39 : 0);
					num38 = ((num38 < maxWidth) ? num38 : maxWidth);
					num39 = ((num39 < maxHeight) ? num39 : maxHeight);
					int num40 = (num38 < maxWidth) ? 1 : 0;
					int num41 = (num39 < maxHeight) ? width : 0;
					int num42 = (num39 << widthShift) + num38;
					int num43 = num42 + num40;
					int num44 = num42 + num41;
					int num45 = num44 + num40;
					byte b3 = this.imagePixels[num42];
					byte b4 = this.imagePixels[num43];
					byte b5 = this.imagePixels[num44];
					byte b6 = this.imagePixels[num45];
					int num46 = (int)b3 + (b * (b4 - b3) >> 8);
					int num47 = (int)b5 + (b * (b6 - b5) >> 8);
					int num48 = num46 + ((int)b2 * (num47 - num46) >> 8);
					num48 = num48 * (int)disp >> 8;
					int num49 = (k >> 8) * size + (l >> 8);
					num48 = (int)this.targetPixels[num49] + num48;
					num48 = ((num48 < 255) ? num48 : 255);
					this.targetPixels[num49] = (byte)num48;
				}
				num32 += num17;
				num33 += num18;
				num34 += num19;
				num35 += num30;
				num36 += num31;
			}
			num14 += num20;
			num15 += num21;
			num16 += num22;
		}
	}

	// Token: 0x060030D3 RID: 12499
	[DllImport("RustNative", EntryPoint = "Water_RasterClearTile")]
	private static extern void RasterClearTile_Native(ref byte pixels, int offset, int stride, int width, int height);

	// Token: 0x060030D4 RID: 12500
	[DllImport("RustNative", EntryPoint = "Water_RasterBindImage")]
	private static extern void RasterBindImage_Native(ref WaterDynamics.ImageDesc desc, ref byte pixels);

	// Token: 0x060030D5 RID: 12501
	[DllImport("RustNative", EntryPoint = "Water_RasterBindTarget")]
	private static extern void RasterBindTarget_Native(ref WaterDynamics.TargetDesc desc, ref byte pixels, ref byte drawTileTable, ref ushort drawTileList, ref int drawTileCount);

	// Token: 0x060030D6 RID: 12502
	[DllImport("RustNative", EntryPoint = "Water_RasterInteraction")]
	private static extern void RasterInteraction_Native(Vector2 pos, Vector2 scale, float rotation, float disp, float dist);

	// Token: 0x060030D7 RID: 12503 RVA: 0x0012CAAA File Offset: 0x0012ACAA
	public static void SafeDestroy<T>(ref T obj) where T : UnityEngine.Object
	{
		if (obj != null)
		{
			UnityEngine.Object.DestroyImmediate(obj);
			obj = default(T);
		}
	}

	// Token: 0x060030D8 RID: 12504 RVA: 0x0012CAD8 File Offset: 0x0012ACD8
	public static T SafeDestroy<T>(T obj) where T : UnityEngine.Object
	{
		if (obj != null)
		{
			UnityEngine.Object.DestroyImmediate(obj);
		}
		return default(T);
	}

	// Token: 0x060030D9 RID: 12505 RVA: 0x0012CB07 File Offset: 0x0012AD07
	public static void SafeRelease<T>(ref T obj) where T : class, IDisposable
	{
		if (obj != null)
		{
			obj.Dispose();
			obj = default(T);
		}
	}

	// Token: 0x060030DA RID: 12506 RVA: 0x0012CB30 File Offset: 0x0012AD30
	public static T SafeRelease<T>(T obj) where T : class, IDisposable
	{
		if (obj != null)
		{
			obj.Dispose();
		}
		return default(T);
	}

	// Token: 0x170003BB RID: 955
	// (get) Token: 0x060030DC RID: 12508 RVA: 0x0012CB62 File Offset: 0x0012AD62
	// (set) Token: 0x060030DB RID: 12507 RVA: 0x0012CB59 File Offset: 0x0012AD59
	public bool IsInitialized { get; private set; }

	// Token: 0x060030DD RID: 12509 RVA: 0x0012CB6A File Offset: 0x0012AD6A
	public static void RegisterInteraction(WaterInteraction interaction)
	{
		WaterDynamics.interactions.Add(interaction);
	}

	// Token: 0x060030DE RID: 12510 RVA: 0x0012CB78 File Offset: 0x0012AD78
	public static void UnregisterInteraction(WaterInteraction interaction)
	{
		WaterDynamics.interactions.Remove(interaction);
	}

	// Token: 0x060030DF RID: 12511 RVA: 0x0012CB88 File Offset: 0x0012AD88
	private bool SupportsNativePath()
	{
		bool result = true;
		try
		{
			WaterDynamics.ImageDesc imageDesc = default(WaterDynamics.ImageDesc);
			byte[] array = new byte[1];
			WaterDynamics.RasterBindImage_Native(ref imageDesc, ref array[0]);
		}
		catch (EntryPointNotFoundException)
		{
			Debug.Log("[WaterDynamics] Fast native path not available. Reverting to managed fallback.");
			result = false;
		}
		return result;
	}

	// Token: 0x060030E0 RID: 12512 RVA: 0x0012CBD8 File Offset: 0x0012ADD8
	public void Initialize(Vector3 areaPosition, Vector3 areaSize)
	{
		this.target = new WaterDynamics.Target(this, areaPosition, areaSize);
		this.useNativePath = this.SupportsNativePath();
		this.IsInitialized = true;
	}

	// Token: 0x060030E1 RID: 12513 RVA: 0x0012CBFB File Offset: 0x0012ADFB
	public bool TryInitialize()
	{
		if (!this.IsInitialized && TerrainMeta.Data != null)
		{
			this.Initialize(TerrainMeta.Position, TerrainMeta.Data.size);
			return true;
		}
		return false;
	}

	// Token: 0x060030E2 RID: 12514 RVA: 0x0012CC2A File Offset: 0x0012AE2A
	public void Shutdown()
	{
		if (this.target != null)
		{
			this.target.Destroy();
			this.target = null;
		}
		this.IsInitialized = false;
	}

	// Token: 0x060030E3 RID: 12515 RVA: 0x0012CC4D File Offset: 0x0012AE4D
	public void OnEnable()
	{
		this.TryInitialize();
	}

	// Token: 0x060030E4 RID: 12516 RVA: 0x0012CC56 File Offset: 0x0012AE56
	public void OnDisable()
	{
		this.Shutdown();
	}

	// Token: 0x060030E5 RID: 12517 RVA: 0x0012CC5E File Offset: 0x0012AE5E
	public void Update()
	{
		if (!(WaterSystem.Instance == null))
		{
			if (this.IsInitialized)
			{
				return;
			}
			this.TryInitialize();
		}
	}

	// Token: 0x060030E6 RID: 12518 RVA: 0x0012CC80 File Offset: 0x0012AE80
	private void ProcessInteractions()
	{
		foreach (WaterInteraction waterInteraction in WaterDynamics.interactions)
		{
			if (!(waterInteraction == null))
			{
				waterInteraction.UpdateTransform();
			}
		}
	}

	// Token: 0x060030E7 RID: 12519 RVA: 0x00026FFC File Offset: 0x000251FC
	public float SampleHeight(Vector3 pos)
	{
		return 0f;
	}

	// Token: 0x0400279D RID: 10141
	private const int maxRasterSize = 1024;

	// Token: 0x0400279E RID: 10142
	private const int subStep = 256;

	// Token: 0x0400279F RID: 10143
	private const int subShift = 8;

	// Token: 0x040027A0 RID: 10144
	private const int subMask = 255;

	// Token: 0x040027A1 RID: 10145
	private const float oneOverSubStep = 0.00390625f;

	// Token: 0x040027A2 RID: 10146
	private const float interp_subStep = 65536f;

	// Token: 0x040027A3 RID: 10147
	private const int interp_subShift = 16;

	// Token: 0x040027A4 RID: 10148
	private const int interp_subFracMask = 65535;

	// Token: 0x040027A5 RID: 10149
	private WaterDynamics.ImageDesc imageDesc;

	// Token: 0x040027A6 RID: 10150
	private byte[] imagePixels;

	// Token: 0x040027A7 RID: 10151
	private WaterDynamics.TargetDesc targetDesc;

	// Token: 0x040027A8 RID: 10152
	private byte[] targetPixels;

	// Token: 0x040027A9 RID: 10153
	private byte[] targetDrawTileTable;

	// Token: 0x040027AA RID: 10154
	private SimpleList<ushort> targetDrawTileList;

	// Token: 0x040027AB RID: 10155
	public bool ShowDebug;

	// Token: 0x040027AD RID: 10157
	public bool ForceFallback;

	// Token: 0x040027AE RID: 10158
	private WaterDynamics.Target target;

	// Token: 0x040027AF RID: 10159
	private bool useNativePath;

	// Token: 0x040027B0 RID: 10160
	private static HashSet<WaterInteraction> interactions = new HashSet<WaterInteraction>();

	// Token: 0x02000DD2 RID: 3538
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ImageDesc
	{
		// Token: 0x06004F5A RID: 20314 RVA: 0x0019EF44 File Offset: 0x0019D144
		public ImageDesc(Texture2D tex)
		{
			this.width = tex.width;
			this.height = tex.height;
			this.maxWidth = tex.width - 1;
			this.maxHeight = tex.height - 1;
			this.widthShift = (int)Mathf.Log((float)tex.width, 2f);
		}

		// Token: 0x06004F5B RID: 20315 RVA: 0x0019EF9D File Offset: 0x0019D19D
		public void Clear()
		{
			this.width = 0;
			this.height = 0;
			this.maxWidth = 0;
			this.maxHeight = 0;
			this.widthShift = 0;
		}

		// Token: 0x040047E2 RID: 18402
		public int width;

		// Token: 0x040047E3 RID: 18403
		public int height;

		// Token: 0x040047E4 RID: 18404
		public int maxWidth;

		// Token: 0x040047E5 RID: 18405
		public int maxHeight;

		// Token: 0x040047E6 RID: 18406
		public int widthShift;
	}

	// Token: 0x02000DD3 RID: 3539
	public class Image
	{
		// Token: 0x17000666 RID: 1638
		// (get) Token: 0x06004F5C RID: 20316 RVA: 0x0019EFC2 File Offset: 0x0019D1C2
		// (set) Token: 0x06004F5D RID: 20317 RVA: 0x0019EFCA File Offset: 0x0019D1CA
		public Texture2D texture { get; private set; }

		// Token: 0x06004F5E RID: 20318 RVA: 0x0019EFD3 File Offset: 0x0019D1D3
		public Image(Texture2D tex)
		{
			this.desc = new WaterDynamics.ImageDesc(tex);
			this.texture = tex;
			this.pixels = this.GetDisplacementPixelsFromTexture(tex);
		}

		// Token: 0x06004F5F RID: 20319 RVA: 0x0019EFFB File Offset: 0x0019D1FB
		public void Destroy()
		{
			this.desc.Clear();
			this.texture = null;
			this.pixels = null;
		}

		// Token: 0x06004F60 RID: 20320 RVA: 0x0019F018 File Offset: 0x0019D218
		private byte[] GetDisplacementPixelsFromTexture(Texture2D tex)
		{
			Color32[] pixels = tex.GetPixels32();
			byte[] array = new byte[pixels.Length];
			for (int i = 0; i < pixels.Length; i++)
			{
				array[i] = pixels[i].b;
			}
			return array;
		}

		// Token: 0x040047E7 RID: 18407
		public WaterDynamics.ImageDesc desc;

		// Token: 0x040047E9 RID: 18409
		public byte[] pixels;
	}

	// Token: 0x02000DD4 RID: 3540
	private struct Point2D
	{
		// Token: 0x06004F61 RID: 20321 RVA: 0x0019F053 File Offset: 0x0019D253
		public Point2D(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		// Token: 0x06004F62 RID: 20322 RVA: 0x0019F063 File Offset: 0x0019D263
		public Point2D(float x, float y)
		{
			this.x = (int)x;
			this.y = (int)y;
		}

		// Token: 0x040047EA RID: 18410
		public int x;

		// Token: 0x040047EB RID: 18411
		public int y;
	}

	// Token: 0x02000DD5 RID: 3541
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TargetDesc
	{
		// Token: 0x06004F63 RID: 20323 RVA: 0x0019F078 File Offset: 0x0019D278
		public TargetDesc(Vector3 areaPosition, Vector3 areaSize)
		{
			this.size = 512;
			this.maxSize = this.size - 1;
			this.maxSizeSubStep = this.maxSize * 256;
			this.areaOffset = new Vector2(areaPosition.x, areaPosition.z);
			this.areaToMapUV = new Vector2(1f / areaSize.x, 1f / areaSize.z);
			this.areaToMapXY = this.areaToMapUV * (float)this.size;
			this.tileSize = Mathf.NextPowerOfTwo(Mathf.Max(this.size, 4096)) / 256;
			this.tileSizeShift = (int)Mathf.Log((float)this.tileSize, 2f);
			this.tileCount = Mathf.CeilToInt((float)this.size / (float)this.tileSize);
			this.tileMaxCount = this.tileCount - 1;
		}

		// Token: 0x06004F64 RID: 20324 RVA: 0x0019F164 File Offset: 0x0019D364
		public void Clear()
		{
			this.areaOffset = Vector2.zero;
			this.areaToMapUV = Vector2.zero;
			this.areaToMapXY = Vector2.zero;
			this.size = 0;
			this.maxSize = 0;
			this.maxSizeSubStep = 0;
			this.tileSize = 0;
			this.tileSizeShift = 0;
			this.tileCount = 0;
			this.tileMaxCount = 0;
		}

		// Token: 0x06004F65 RID: 20325 RVA: 0x0019F1C4 File Offset: 0x0019D3C4
		public ushort TileOffsetToXYOffset(ushort tileOffset, out int x, out int y, out int offset)
		{
			int num = (int)tileOffset % this.tileCount;
			int num2 = (int)tileOffset / this.tileCount;
			x = num * this.tileSize;
			y = num2 * this.tileSize;
			offset = y * this.size + x;
			return tileOffset;
		}

		// Token: 0x06004F66 RID: 20326 RVA: 0x0019F207 File Offset: 0x0019D407
		public ushort TileOffsetToTileXYIndex(ushort tileOffset, out int tileX, out int tileY, out ushort tileIndex)
		{
			tileX = (int)tileOffset % this.tileCount;
			tileY = (int)tileOffset / this.tileCount;
			tileIndex = (ushort)(tileY * this.tileCount + tileX);
			return tileOffset;
		}

		// Token: 0x06004F67 RID: 20327 RVA: 0x0019F230 File Offset: 0x0019D430
		public Vector2 WorldToRaster(Vector2 pos)
		{
			Vector2 result;
			result.x = (pos.x - this.areaOffset.x) * this.areaToMapXY.x;
			result.y = (pos.y - this.areaOffset.y) * this.areaToMapXY.y;
			return result;
		}

		// Token: 0x06004F68 RID: 20328 RVA: 0x0019F288 File Offset: 0x0019D488
		public Vector3 WorldToRaster(Vector3 pos)
		{
			Vector2 v;
			v.x = (pos.x - this.areaOffset.x) * this.areaToMapXY.x;
			v.y = (pos.z - this.areaOffset.y) * this.areaToMapXY.y;
			return v;
		}

		// Token: 0x040047EC RID: 18412
		public int size;

		// Token: 0x040047ED RID: 18413
		public int maxSize;

		// Token: 0x040047EE RID: 18414
		public int maxSizeSubStep;

		// Token: 0x040047EF RID: 18415
		public Vector2 areaOffset;

		// Token: 0x040047F0 RID: 18416
		public Vector2 areaToMapUV;

		// Token: 0x040047F1 RID: 18417
		public Vector2 areaToMapXY;

		// Token: 0x040047F2 RID: 18418
		public int tileSize;

		// Token: 0x040047F3 RID: 18419
		public int tileSizeShift;

		// Token: 0x040047F4 RID: 18420
		public int tileCount;

		// Token: 0x040047F5 RID: 18421
		public int tileMaxCount;
	}

	// Token: 0x02000DD6 RID: 3542
	public class Target
	{
		// Token: 0x17000667 RID: 1639
		// (get) Token: 0x06004F69 RID: 20329 RVA: 0x0019F2E5 File Offset: 0x0019D4E5
		public WaterDynamics.TargetDesc Desc
		{
			get
			{
				return this.desc;
			}
		}

		// Token: 0x17000668 RID: 1640
		// (get) Token: 0x06004F6A RID: 20330 RVA: 0x0019F2ED File Offset: 0x0019D4ED
		public byte[] Pixels
		{
			get
			{
				return this.pixels;
			}
		}

		// Token: 0x17000669 RID: 1641
		// (get) Token: 0x06004F6B RID: 20331 RVA: 0x0019F2F5 File Offset: 0x0019D4F5
		public byte[] DrawTileTable
		{
			get
			{
				return this.drawTileTable;
			}
		}

		// Token: 0x1700066A RID: 1642
		// (get) Token: 0x06004F6C RID: 20332 RVA: 0x0019F2FD File Offset: 0x0019D4FD
		public SimpleList<ushort> DrawTileList
		{
			get
			{
				return this.drawTileList;
			}
		}

		// Token: 0x06004F6D RID: 20333 RVA: 0x0019F305 File Offset: 0x0019D505
		public Target(WaterDynamics owner, Vector3 areaPosition, Vector3 areaSize)
		{
			this.owner = owner;
			this.desc = new WaterDynamics.TargetDesc(areaPosition, areaSize);
		}

		// Token: 0x06004F6E RID: 20334 RVA: 0x0019F321 File Offset: 0x0019D521
		public void Destroy()
		{
			this.desc.Clear();
		}

		// Token: 0x06004F6F RID: 20335 RVA: 0x0019F32E File Offset: 0x0019D52E
		private Texture2D CreateDynamicTexture(int size)
		{
			return new Texture2D(size, size, TextureFormat.ARGB32, false, true)
			{
				filterMode = FilterMode.Bilinear,
				wrapMode = TextureWrapMode.Clamp
			};
		}

		// Token: 0x06004F70 RID: 20336 RVA: 0x0019F348 File Offset: 0x0019D548
		private RenderTexture CreateRenderTexture(int size)
		{
			RenderTextureFormat format = SystemInfoEx.SupportsRenderTextureFormat(RenderTextureFormat.RHalf) ? RenderTextureFormat.RHalf : RenderTextureFormat.RFloat;
			RenderTexture renderTexture = new RenderTexture(size, size, 0, format, RenderTextureReadWrite.Linear);
			renderTexture.filterMode = FilterMode.Bilinear;
			renderTexture.wrapMode = TextureWrapMode.Clamp;
			renderTexture.Create();
			return renderTexture;
		}

		// Token: 0x06004F71 RID: 20337 RVA: 0x0019F384 File Offset: 0x0019D584
		public void ClearTiles()
		{
			for (int i = 0; i < this.clearTileList.Count; i++)
			{
				int num;
				int num2;
				int num3;
				this.desc.TileOffsetToXYOffset(this.clearTileList[i], out num, out num2, out num3);
				int num4 = Mathf.Min(num + this.desc.tileSize, this.desc.size) - num;
				int num5 = Mathf.Min(num2 + this.desc.tileSize, this.desc.size) - num2;
				if (this.owner.useNativePath)
				{
					WaterDynamics.RasterClearTile_Native(ref this.pixels[0], num3, this.desc.size, num4, num5);
				}
				else
				{
					for (int j = 0; j < num5; j++)
					{
						Array.Clear(this.pixels, num3, num4);
						num3 += this.desc.size;
					}
				}
			}
		}

		// Token: 0x06004F72 RID: 20338 RVA: 0x0019F46C File Offset: 0x0019D66C
		public void ProcessTiles()
		{
			for (int i = 0; i < this.clearTileList.Count; i++)
			{
				int num2;
				int num3;
				ushort num4;
				ushort num = this.desc.TileOffsetToTileXYIndex(this.clearTileList[i], out num2, out num3, out num4);
				this.clearTileTable[(int)num] = 0;
				this.clearTileList[i] = ushort.MaxValue;
			}
			this.clearTileList.Clear();
			for (int j = 0; j < this.drawTileList.Count; j++)
			{
				int num2;
				int num3;
				ushort num4;
				ushort num5 = this.desc.TileOffsetToTileXYIndex(this.drawTileList[j], out num2, out num3, out num4);
				if (this.clearTileTable[(int)num4] == 0)
				{
					this.clearTileTable[(int)num4] = 1;
					this.clearTileList.Add(num4);
				}
				this.drawTileTable[(int)num5] = 0;
				this.drawTileList[j] = ushort.MaxValue;
			}
			this.drawTileList.Clear();
		}

		// Token: 0x06004F73 RID: 20339 RVA: 0x000059DD File Offset: 0x00003BDD
		public void UpdateTiles()
		{
		}

		// Token: 0x06004F74 RID: 20340 RVA: 0x000059DD File Offset: 0x00003BDD
		public void Prepare()
		{
		}

		// Token: 0x06004F75 RID: 20341 RVA: 0x000059DD File Offset: 0x00003BDD
		public void Update()
		{
		}

		// Token: 0x06004F76 RID: 20342 RVA: 0x000059DD File Offset: 0x00003BDD
		public void UpdateGlobalShaderProperties()
		{
		}

		// Token: 0x040047F6 RID: 18422
		public WaterDynamics owner;

		// Token: 0x040047F7 RID: 18423
		public WaterDynamics.TargetDesc desc;

		// Token: 0x040047F8 RID: 18424
		private byte[] pixels;

		// Token: 0x040047F9 RID: 18425
		private byte[] clearTileTable;

		// Token: 0x040047FA RID: 18426
		private SimpleList<ushort> clearTileList;

		// Token: 0x040047FB RID: 18427
		private byte[] drawTileTable;

		// Token: 0x040047FC RID: 18428
		private SimpleList<ushort> drawTileList;

		// Token: 0x040047FD RID: 18429
		private const int MaxInteractionOffset = 100;

		// Token: 0x040047FE RID: 18430
		private Vector3 prevCameraWorldPos;

		// Token: 0x040047FF RID: 18431
		private Vector2i interactionOffset;
	}
}
