using System;
using UnityEngine;

namespace VLB
{
	// Token: 0x02000985 RID: 2437
	public static class Noise3D
	{
		// Token: 0x17000459 RID: 1113
		// (get) Token: 0x06003999 RID: 14745 RVA: 0x001549EC File Offset: 0x00152BEC
		public static bool isSupported
		{
			get
			{
				if (!Noise3D.ms_IsSupportedChecked)
				{
					Noise3D.ms_IsSupported = (SystemInfo.graphicsShaderLevel >= 35);
					if (!Noise3D.ms_IsSupported)
					{
						Debug.LogWarning(Noise3D.isNotSupportedString);
					}
					Noise3D.ms_IsSupportedChecked = true;
				}
				return Noise3D.ms_IsSupported;
			}
		}

		// Token: 0x1700045A RID: 1114
		// (get) Token: 0x0600399A RID: 14746 RVA: 0x00154A22 File Offset: 0x00152C22
		public static bool isProperlyLoaded
		{
			get
			{
				return Noise3D.ms_NoiseTexture != null;
			}
		}

		// Token: 0x1700045B RID: 1115
		// (get) Token: 0x0600399B RID: 14747 RVA: 0x00154A2F File Offset: 0x00152C2F
		public static string isNotSupportedString
		{
			get
			{
				return string.Format("3D Noise requires higher shader capabilities (Shader Model 3.5 / OpenGL ES 3.0), which are not available on the current platform: graphicsShaderLevel (current/required) = {0} / {1}", SystemInfo.graphicsShaderLevel, 35);
			}
		}

		// Token: 0x0600399C RID: 14748 RVA: 0x00154A4C File Offset: 0x00152C4C
		[RuntimeInitializeOnLoadMethod]
		private static void OnStartUp()
		{
			Noise3D.LoadIfNeeded();
		}

		// Token: 0x0600399D RID: 14749 RVA: 0x00154A54 File Offset: 0x00152C54
		public static void LoadIfNeeded()
		{
			if (!Noise3D.isSupported)
			{
				return;
			}
			if (Noise3D.ms_NoiseTexture == null)
			{
				Noise3D.ms_NoiseTexture = Noise3D.LoadTexture3D(Config.Instance.noise3DData, Config.Instance.noise3DSize);
				if (Noise3D.ms_NoiseTexture)
				{
					Noise3D.ms_NoiseTexture.hideFlags = HideFlags.HideAndDontSave;
				}
			}
			Shader.SetGlobalTexture("_VLB_NoiseTex3D", Noise3D.ms_NoiseTexture);
			Shader.SetGlobalVector("_VLB_NoiseGlobal", Config.Instance.globalNoiseParam);
		}

		// Token: 0x0600399E RID: 14750 RVA: 0x00154AD0 File Offset: 0x00152CD0
		private static Texture3D LoadTexture3D(TextAsset textData, int size)
		{
			if (textData == null)
			{
				Debug.LogErrorFormat("Fail to open Noise 3D Data", Array.Empty<object>());
				return null;
			}
			byte[] bytes = textData.bytes;
			Debug.Assert(bytes != null);
			int num = Mathf.Max(0, size * size * size);
			if (bytes.Length != num)
			{
				Debug.LogErrorFormat("Noise 3D Data file has not the proper size {0}x{0}x{0}", new object[]
				{
					size
				});
				return null;
			}
			Texture3D texture3D = new Texture3D(size, size, size, TextureFormat.Alpha8, false);
			Color[] array = new Color[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = new Color32(0, 0, 0, bytes[i]);
			}
			texture3D.SetPixels(array);
			texture3D.Apply();
			return texture3D;
		}

		// Token: 0x0400343D RID: 13373
		private static bool ms_IsSupportedChecked;

		// Token: 0x0400343E RID: 13374
		private static bool ms_IsSupported;

		// Token: 0x0400343F RID: 13375
		private static Texture3D ms_NoiseTexture;

		// Token: 0x04003440 RID: 13376
		private const HideFlags kHideFlags = HideFlags.HideAndDontSave;

		// Token: 0x04003441 RID: 13377
		private const int kMinShaderLevel = 35;
	}
}
