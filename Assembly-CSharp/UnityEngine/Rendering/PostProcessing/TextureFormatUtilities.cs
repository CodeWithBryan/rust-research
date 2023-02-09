using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A5E RID: 2654
	public static class TextureFormatUtilities
	{
		// Token: 0x06003EF0 RID: 16112 RVA: 0x001714B8 File Offset: 0x0016F6B8
		static TextureFormatUtilities()
		{
			foreach (object obj in Enum.GetValues(typeof(RenderTextureFormat)))
			{
				if ((int)obj >= 0 && !TextureFormatUtilities.IsObsolete(obj))
				{
					bool value = SystemInfo.SupportsRenderTextureFormat((RenderTextureFormat)obj);
					TextureFormatUtilities.s_SupportedRenderTextureFormats[(int)obj] = value;
				}
			}
			TextureFormatUtilities.s_SupportedTextureFormats = new Dictionary<int, bool>();
			foreach (object obj2 in Enum.GetValues(typeof(TextureFormat)))
			{
				if ((int)obj2 >= 0 && !TextureFormatUtilities.IsObsolete(obj2))
				{
					bool value2 = SystemInfo.SupportsTextureFormat((TextureFormat)obj2);
					TextureFormatUtilities.s_SupportedTextureFormats[(int)obj2] = value2;
				}
			}
		}

		// Token: 0x06003EF1 RID: 16113 RVA: 0x0017172C File Offset: 0x0016F92C
		private static bool IsObsolete(object value)
		{
			ObsoleteAttribute[] array = (ObsoleteAttribute[])value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(ObsoleteAttribute), false);
			return array != null && array.Length != 0;
		}

		// Token: 0x06003EF2 RID: 16114 RVA: 0x0017176C File Offset: 0x0016F96C
		public static RenderTextureFormat GetUncompressedRenderTextureFormat(Texture texture)
		{
			Assert.IsNotNull<Texture>(texture);
			if (texture is RenderTexture)
			{
				return (texture as RenderTexture).format;
			}
			if (!(texture is Texture2D))
			{
				return RenderTextureFormat.Default;
			}
			TextureFormat format = ((Texture2D)texture).format;
			RenderTextureFormat result;
			if (!TextureFormatUtilities.s_FormatAliasMap.TryGetValue((int)format, out result))
			{
				throw new NotSupportedException("Texture format not supported");
			}
			return result;
		}

		// Token: 0x06003EF3 RID: 16115 RVA: 0x001717C4 File Offset: 0x0016F9C4
		internal static bool IsSupported(this RenderTextureFormat format)
		{
			bool result;
			TextureFormatUtilities.s_SupportedRenderTextureFormats.TryGetValue((int)format, out result);
			return result;
		}

		// Token: 0x06003EF4 RID: 16116 RVA: 0x001717E0 File Offset: 0x0016F9E0
		internal static bool IsSupported(this TextureFormat format)
		{
			bool result;
			TextureFormatUtilities.s_SupportedTextureFormats.TryGetValue((int)format, out result);
			return result;
		}

		// Token: 0x0400383B RID: 14395
		private static Dictionary<int, RenderTextureFormat> s_FormatAliasMap = new Dictionary<int, RenderTextureFormat>
		{
			{
				1,
				RenderTextureFormat.ARGB32
			},
			{
				2,
				RenderTextureFormat.ARGB4444
			},
			{
				3,
				RenderTextureFormat.ARGB32
			},
			{
				4,
				RenderTextureFormat.ARGB32
			},
			{
				5,
				RenderTextureFormat.ARGB32
			},
			{
				7,
				RenderTextureFormat.RGB565
			},
			{
				9,
				RenderTextureFormat.RHalf
			},
			{
				10,
				RenderTextureFormat.ARGB32
			},
			{
				12,
				RenderTextureFormat.ARGB32
			},
			{
				13,
				RenderTextureFormat.ARGB4444
			},
			{
				14,
				RenderTextureFormat.ARGB32
			},
			{
				15,
				RenderTextureFormat.RHalf
			},
			{
				16,
				RenderTextureFormat.RGHalf
			},
			{
				17,
				RenderTextureFormat.ARGBHalf
			},
			{
				18,
				RenderTextureFormat.RFloat
			},
			{
				19,
				RenderTextureFormat.RGFloat
			},
			{
				20,
				RenderTextureFormat.ARGBFloat
			},
			{
				22,
				RenderTextureFormat.ARGBHalf
			},
			{
				26,
				RenderTextureFormat.R8
			},
			{
				27,
				RenderTextureFormat.RGHalf
			},
			{
				24,
				RenderTextureFormat.ARGBHalf
			},
			{
				25,
				RenderTextureFormat.ARGB32
			},
			{
				28,
				RenderTextureFormat.ARGB32
			},
			{
				29,
				RenderTextureFormat.ARGB32
			},
			{
				30,
				RenderTextureFormat.ARGB32
			},
			{
				31,
				RenderTextureFormat.ARGB32
			},
			{
				32,
				RenderTextureFormat.ARGB32
			},
			{
				33,
				RenderTextureFormat.ARGB32
			},
			{
				34,
				RenderTextureFormat.ARGB32
			},
			{
				45,
				RenderTextureFormat.ARGB32
			},
			{
				46,
				RenderTextureFormat.ARGB32
			},
			{
				47,
				RenderTextureFormat.ARGB32
			},
			{
				48,
				RenderTextureFormat.ARGB32
			},
			{
				49,
				RenderTextureFormat.ARGB32
			},
			{
				50,
				RenderTextureFormat.ARGB32
			},
			{
				51,
				RenderTextureFormat.ARGB32
			},
			{
				52,
				RenderTextureFormat.ARGB32
			},
			{
				53,
				RenderTextureFormat.ARGB32
			}
		};

		// Token: 0x0400383C RID: 14396
		private static Dictionary<int, bool> s_SupportedRenderTextureFormats = new Dictionary<int, bool>();

		// Token: 0x0400383D RID: 14397
		private static Dictionary<int, bool> s_SupportedTextureFormats;
	}
}
