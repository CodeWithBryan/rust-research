using System;
using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
	// Token: 0x020009D4 RID: 2516
	[ExecuteInEditMode]
	[AddComponentMenu("Image Effects/Cinematic/Tonemapping and Color Grading")]
	[ImageEffectAllowedInSceneView]
	public class TonemappingColorGrading : MonoBehaviour
	{
		// Token: 0x0400353D RID: 13629
		[SerializeField]
		[TonemappingColorGrading.SettingsGroup]
		private TonemappingColorGrading.EyeAdaptationSettings m_EyeAdaptation = TonemappingColorGrading.EyeAdaptationSettings.defaultSettings;

		// Token: 0x0400353E RID: 13630
		[SerializeField]
		[TonemappingColorGrading.SettingsGroup]
		private TonemappingColorGrading.TonemappingSettings m_Tonemapping = TonemappingColorGrading.TonemappingSettings.defaultSettings;

		// Token: 0x0400353F RID: 13631
		[SerializeField]
		[TonemappingColorGrading.SettingsGroup]
		private TonemappingColorGrading.ColorGradingSettings m_ColorGrading = TonemappingColorGrading.ColorGradingSettings.defaultSettings;

		// Token: 0x04003540 RID: 13632
		[SerializeField]
		[TonemappingColorGrading.SettingsGroup]
		private TonemappingColorGrading.LUTSettings m_Lut = TonemappingColorGrading.LUTSettings.defaultSettings;

		// Token: 0x04003541 RID: 13633
		[SerializeField]
		private Shader m_Shader;

		// Token: 0x02000E98 RID: 3736
		[AttributeUsage(AttributeTargets.Field)]
		public class SettingsGroup : Attribute
		{
		}

		// Token: 0x02000E99 RID: 3737
		public class IndentedGroup : PropertyAttribute
		{
		}

		// Token: 0x02000E9A RID: 3738
		public class ChannelMixer : PropertyAttribute
		{
		}

		// Token: 0x02000E9B RID: 3739
		public class ColorWheelGroup : PropertyAttribute
		{
			// Token: 0x0600510F RID: 20751 RVA: 0x001A3CFE File Offset: 0x001A1EFE
			public ColorWheelGroup()
			{
			}

			// Token: 0x06005110 RID: 20752 RVA: 0x001A3D19 File Offset: 0x001A1F19
			public ColorWheelGroup(int minSizePerWheel, int maxSizePerWheel)
			{
				this.minSizePerWheel = minSizePerWheel;
				this.maxSizePerWheel = maxSizePerWheel;
			}

			// Token: 0x04004B40 RID: 19264
			public int minSizePerWheel = 60;

			// Token: 0x04004B41 RID: 19265
			public int maxSizePerWheel = 150;
		}

		// Token: 0x02000E9C RID: 3740
		public class Curve : PropertyAttribute
		{
			// Token: 0x06005111 RID: 20753 RVA: 0x001A3D42 File Offset: 0x001A1F42
			public Curve()
			{
			}

			// Token: 0x06005112 RID: 20754 RVA: 0x001A3D55 File Offset: 0x001A1F55
			public Curve(float r, float g, float b, float a)
			{
				this.color = new Color(r, g, b, a);
			}

			// Token: 0x04004B42 RID: 19266
			public Color color = Color.white;
		}

		// Token: 0x02000E9D RID: 3741
		[Serializable]
		public struct EyeAdaptationSettings
		{
			// Token: 0x170006A0 RID: 1696
			// (get) Token: 0x06005113 RID: 20755 RVA: 0x001A3D78 File Offset: 0x001A1F78
			public static TonemappingColorGrading.EyeAdaptationSettings defaultSettings
			{
				get
				{
					return new TonemappingColorGrading.EyeAdaptationSettings
					{
						enabled = false,
						showDebug = false,
						middleGrey = 0.5f,
						min = -3f,
						max = 3f,
						speed = 1.5f
					};
				}
			}

			// Token: 0x04004B43 RID: 19267
			public bool enabled;

			// Token: 0x04004B44 RID: 19268
			[Min(0f)]
			[Tooltip("Midpoint Adjustment.")]
			public float middleGrey;

			// Token: 0x04004B45 RID: 19269
			[Tooltip("The lowest possible exposure value; adjust this value to modify the brightest areas of your level.")]
			public float min;

			// Token: 0x04004B46 RID: 19270
			[Tooltip("The highest possible exposure value; adjust this value to modify the darkest areas of your level.")]
			public float max;

			// Token: 0x04004B47 RID: 19271
			[Min(0f)]
			[Tooltip("Speed of linear adaptation. Higher is faster.")]
			public float speed;

			// Token: 0x04004B48 RID: 19272
			[Tooltip("Displays a luminosity helper in the GameView.")]
			public bool showDebug;
		}

		// Token: 0x02000E9E RID: 3742
		public enum Tonemapper
		{
			// Token: 0x04004B4A RID: 19274
			ACES,
			// Token: 0x04004B4B RID: 19275
			Curve,
			// Token: 0x04004B4C RID: 19276
			Hable,
			// Token: 0x04004B4D RID: 19277
			HejlDawson,
			// Token: 0x04004B4E RID: 19278
			Photographic,
			// Token: 0x04004B4F RID: 19279
			Reinhard,
			// Token: 0x04004B50 RID: 19280
			Neutral
		}

		// Token: 0x02000E9F RID: 3743
		[Serializable]
		public struct TonemappingSettings
		{
			// Token: 0x170006A1 RID: 1697
			// (get) Token: 0x06005114 RID: 20756 RVA: 0x001A3DD0 File Offset: 0x001A1FD0
			public static TonemappingColorGrading.TonemappingSettings defaultSettings
			{
				get
				{
					return new TonemappingColorGrading.TonemappingSettings
					{
						enabled = false,
						tonemapper = TonemappingColorGrading.Tonemapper.Neutral,
						exposure = 1f,
						curve = TonemappingColorGrading.CurvesSettings.defaultCurve,
						neutralBlackIn = 0.02f,
						neutralWhiteIn = 10f,
						neutralBlackOut = 0f,
						neutralWhiteOut = 10f,
						neutralWhiteLevel = 5.3f,
						neutralWhiteClip = 10f
					};
				}
			}

			// Token: 0x04004B51 RID: 19281
			public bool enabled;

			// Token: 0x04004B52 RID: 19282
			[Tooltip("Tonemapping technique to use. ACES is the recommended one.")]
			public TonemappingColorGrading.Tonemapper tonemapper;

			// Token: 0x04004B53 RID: 19283
			[Min(0f)]
			[Tooltip("Adjusts the overall exposure of the scene.")]
			public float exposure;

			// Token: 0x04004B54 RID: 19284
			[Tooltip("Custom tonemapping curve.")]
			public AnimationCurve curve;

			// Token: 0x04004B55 RID: 19285
			[Range(-0.1f, 0.1f)]
			public float neutralBlackIn;

			// Token: 0x04004B56 RID: 19286
			[Range(1f, 20f)]
			public float neutralWhiteIn;

			// Token: 0x04004B57 RID: 19287
			[Range(-0.09f, 0.1f)]
			public float neutralBlackOut;

			// Token: 0x04004B58 RID: 19288
			[Range(1f, 19f)]
			public float neutralWhiteOut;

			// Token: 0x04004B59 RID: 19289
			[Range(0.1f, 20f)]
			public float neutralWhiteLevel;

			// Token: 0x04004B5A RID: 19290
			[Range(1f, 10f)]
			public float neutralWhiteClip;
		}

		// Token: 0x02000EA0 RID: 3744
		[Serializable]
		public struct LUTSettings
		{
			// Token: 0x170006A2 RID: 1698
			// (get) Token: 0x06005115 RID: 20757 RVA: 0x001A3E58 File Offset: 0x001A2058
			public static TonemappingColorGrading.LUTSettings defaultSettings
			{
				get
				{
					return new TonemappingColorGrading.LUTSettings
					{
						enabled = false,
						texture = null,
						contribution = 1f
					};
				}
			}

			// Token: 0x04004B5B RID: 19291
			public bool enabled;

			// Token: 0x04004B5C RID: 19292
			[Tooltip("Custom lookup texture (strip format, e.g. 256x16).")]
			public Texture texture;

			// Token: 0x04004B5D RID: 19293
			[Range(0f, 1f)]
			[Tooltip("Blending factor.")]
			public float contribution;
		}

		// Token: 0x02000EA1 RID: 3745
		[Serializable]
		public struct ColorWheelsSettings
		{
			// Token: 0x170006A3 RID: 1699
			// (get) Token: 0x06005116 RID: 20758 RVA: 0x001A3E8C File Offset: 0x001A208C
			public static TonemappingColorGrading.ColorWheelsSettings defaultSettings
			{
				get
				{
					return new TonemappingColorGrading.ColorWheelsSettings
					{
						shadows = Color.white,
						midtones = Color.white,
						highlights = Color.white
					};
				}
			}

			// Token: 0x04004B5E RID: 19294
			[ColorUsage(false)]
			public Color shadows;

			// Token: 0x04004B5F RID: 19295
			[ColorUsage(false)]
			public Color midtones;

			// Token: 0x04004B60 RID: 19296
			[ColorUsage(false)]
			public Color highlights;
		}

		// Token: 0x02000EA2 RID: 3746
		[Serializable]
		public struct BasicsSettings
		{
			// Token: 0x170006A4 RID: 1700
			// (get) Token: 0x06005117 RID: 20759 RVA: 0x001A3EC8 File Offset: 0x001A20C8
			public static TonemappingColorGrading.BasicsSettings defaultSettings
			{
				get
				{
					return new TonemappingColorGrading.BasicsSettings
					{
						temperatureShift = 0f,
						tint = 0f,
						contrast = 1f,
						hue = 0f,
						saturation = 1f,
						value = 1f,
						vibrance = 0f,
						gain = 1f,
						gamma = 1f
					};
				}
			}

			// Token: 0x04004B61 RID: 19297
			[Range(-2f, 2f)]
			[Tooltip("Sets the white balance to a custom color temperature.")]
			public float temperatureShift;

			// Token: 0x04004B62 RID: 19298
			[Range(-2f, 2f)]
			[Tooltip("Sets the white balance to compensate for a green or magenta tint.")]
			public float tint;

			// Token: 0x04004B63 RID: 19299
			[Space]
			[Range(-0.5f, 0.5f)]
			[Tooltip("Shift the hue of all colors.")]
			public float hue;

			// Token: 0x04004B64 RID: 19300
			[Range(0f, 2f)]
			[Tooltip("Pushes the intensity of all colors.")]
			public float saturation;

			// Token: 0x04004B65 RID: 19301
			[Range(-1f, 1f)]
			[Tooltip("Adjusts the saturation so that clipping is minimized as colors approach full saturation.")]
			public float vibrance;

			// Token: 0x04004B66 RID: 19302
			[Range(0f, 10f)]
			[Tooltip("Brightens or darkens all colors.")]
			public float value;

			// Token: 0x04004B67 RID: 19303
			[Space]
			[Range(0f, 2f)]
			[Tooltip("Expands or shrinks the overall range of tonal values.")]
			public float contrast;

			// Token: 0x04004B68 RID: 19304
			[Range(0.01f, 5f)]
			[Tooltip("Contrast gain curve. Controls the steepness of the curve.")]
			public float gain;

			// Token: 0x04004B69 RID: 19305
			[Range(0.01f, 5f)]
			[Tooltip("Applies a pow function to the source.")]
			public float gamma;
		}

		// Token: 0x02000EA3 RID: 3747
		[Serializable]
		public struct ChannelMixerSettings
		{
			// Token: 0x170006A5 RID: 1701
			// (get) Token: 0x06005118 RID: 20760 RVA: 0x001A3F4C File Offset: 0x001A214C
			public static TonemappingColorGrading.ChannelMixerSettings defaultSettings
			{
				get
				{
					return new TonemappingColorGrading.ChannelMixerSettings
					{
						currentChannel = 0,
						channels = new Vector3[]
						{
							new Vector3(1f, 0f, 0f),
							new Vector3(0f, 1f, 0f),
							new Vector3(0f, 0f, 1f)
						}
					};
				}
			}

			// Token: 0x04004B6A RID: 19306
			public int currentChannel;

			// Token: 0x04004B6B RID: 19307
			public Vector3[] channels;
		}

		// Token: 0x02000EA4 RID: 3748
		[Serializable]
		public struct CurvesSettings
		{
			// Token: 0x170006A6 RID: 1702
			// (get) Token: 0x06005119 RID: 20761 RVA: 0x001A3FC8 File Offset: 0x001A21C8
			public static TonemappingColorGrading.CurvesSettings defaultSettings
			{
				get
				{
					return new TonemappingColorGrading.CurvesSettings
					{
						master = TonemappingColorGrading.CurvesSettings.defaultCurve,
						red = TonemappingColorGrading.CurvesSettings.defaultCurve,
						green = TonemappingColorGrading.CurvesSettings.defaultCurve,
						blue = TonemappingColorGrading.CurvesSettings.defaultCurve
					};
				}
			}

			// Token: 0x170006A7 RID: 1703
			// (get) Token: 0x0600511A RID: 20762 RVA: 0x001A4010 File Offset: 0x001A2210
			public static AnimationCurve defaultCurve
			{
				get
				{
					return new AnimationCurve(new Keyframe[]
					{
						new Keyframe(0f, 0f, 1f, 1f),
						new Keyframe(1f, 1f, 1f, 1f)
					});
				}
			}

			// Token: 0x04004B6C RID: 19308
			[TonemappingColorGrading.Curve]
			public AnimationCurve master;

			// Token: 0x04004B6D RID: 19309
			[TonemappingColorGrading.Curve(1f, 0f, 0f, 1f)]
			public AnimationCurve red;

			// Token: 0x04004B6E RID: 19310
			[TonemappingColorGrading.Curve(0f, 1f, 0f, 1f)]
			public AnimationCurve green;

			// Token: 0x04004B6F RID: 19311
			[TonemappingColorGrading.Curve(0f, 1f, 1f, 1f)]
			public AnimationCurve blue;
		}

		// Token: 0x02000EA5 RID: 3749
		public enum ColorGradingPrecision
		{
			// Token: 0x04004B71 RID: 19313
			Normal = 16,
			// Token: 0x04004B72 RID: 19314
			High = 32
		}

		// Token: 0x02000EA6 RID: 3750
		[Serializable]
		public struct ColorGradingSettings
		{
			// Token: 0x170006A8 RID: 1704
			// (get) Token: 0x0600511B RID: 20763 RVA: 0x001A4068 File Offset: 0x001A2268
			public static TonemappingColorGrading.ColorGradingSettings defaultSettings
			{
				get
				{
					return new TonemappingColorGrading.ColorGradingSettings
					{
						enabled = false,
						useDithering = false,
						showDebug = false,
						precision = TonemappingColorGrading.ColorGradingPrecision.Normal,
						colorWheels = TonemappingColorGrading.ColorWheelsSettings.defaultSettings,
						basics = TonemappingColorGrading.BasicsSettings.defaultSettings,
						channelMixer = TonemappingColorGrading.ChannelMixerSettings.defaultSettings,
						curves = TonemappingColorGrading.CurvesSettings.defaultSettings
					};
				}
			}

			// Token: 0x0600511C RID: 20764 RVA: 0x001A40CF File Offset: 0x001A22CF
			internal void Reset()
			{
				this.curves = TonemappingColorGrading.CurvesSettings.defaultSettings;
			}

			// Token: 0x04004B73 RID: 19315
			public bool enabled;

			// Token: 0x04004B74 RID: 19316
			[Tooltip("Internal LUT precision. \"Normal\" is 256x16, \"High\" is 1024x32. Prefer \"Normal\" on mobile devices.")]
			public TonemappingColorGrading.ColorGradingPrecision precision;

			// Token: 0x04004B75 RID: 19317
			[Space]
			[TonemappingColorGrading.ColorWheelGroup]
			public TonemappingColorGrading.ColorWheelsSettings colorWheels;

			// Token: 0x04004B76 RID: 19318
			[Space]
			[TonemappingColorGrading.IndentedGroup]
			public TonemappingColorGrading.BasicsSettings basics;

			// Token: 0x04004B77 RID: 19319
			[Space]
			[TonemappingColorGrading.ChannelMixer]
			public TonemappingColorGrading.ChannelMixerSettings channelMixer;

			// Token: 0x04004B78 RID: 19320
			[Space]
			[TonemappingColorGrading.IndentedGroup]
			public TonemappingColorGrading.CurvesSettings curves;

			// Token: 0x04004B79 RID: 19321
			[Space]
			[Tooltip("Use dithering to try and minimize color banding in dark areas.")]
			public bool useDithering;

			// Token: 0x04004B7A RID: 19322
			[Tooltip("Displays the generated LUT in the top left corner of the GameView.")]
			public bool showDebug;
		}
	}
}
