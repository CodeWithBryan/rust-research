using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A16 RID: 2582
	[PostProcess(typeof(ColorGradingRenderer), "Unity/Color Grading", true)]
	[Serializable]
	public sealed class ColorGrading : PostProcessEffectSettings
	{
		// Token: 0x06003D65 RID: 15717 RVA: 0x00166E29 File Offset: 0x00165029
		public override bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return (this.gradingMode.value != GradingMode.External || (SystemInfo.supports3DRenderTextures && SystemInfo.supportsComputeShaders)) && this.enabled.value;
		}

		// Token: 0x04003689 RID: 13961
		[DisplayName("Mode")]
		[Tooltip("Select a color grading mode that fits your dynamic range and workflow. Use HDR if your camera is set to render in HDR and your target platform supports it. Use LDR for low-end mobiles or devices that don't support HDR. Use External if you prefer authoring a Log LUT in an external software.")]
		public GradingModeParameter gradingMode = new GradingModeParameter
		{
			value = GradingMode.HighDefinitionRange
		};

		// Token: 0x0400368A RID: 13962
		[DisplayName("Lookup Texture")]
		[Tooltip("A custom 3D log-encoded texture.")]
		public TextureParameter externalLut = new TextureParameter
		{
			value = null
		};

		// Token: 0x0400368B RID: 13963
		[DisplayName("Mode")]
		[Tooltip("Select a tonemapping algorithm to use at the end of the color grading process.")]
		public TonemapperParameter tonemapper = new TonemapperParameter
		{
			value = Tonemapper.None
		};

		// Token: 0x0400368C RID: 13964
		[DisplayName("Toe Strength")]
		[Range(0f, 1f)]
		[Tooltip("Affects the transition between the toe and the mid section of the curve. A value of 0 means no toe, a value of 1 means a very hard transition.")]
		public FloatParameter toneCurveToeStrength = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x0400368D RID: 13965
		[DisplayName("Toe Length")]
		[Range(0f, 1f)]
		[Tooltip("Affects how much of the dynamic range is in the toe. With a small value, the toe will be very short and quickly transition into the linear section, with a larger value, the toe will be longer.")]
		public FloatParameter toneCurveToeLength = new FloatParameter
		{
			value = 0.5f
		};

		// Token: 0x0400368E RID: 13966
		[DisplayName("Shoulder Strength")]
		[Range(0f, 1f)]
		[Tooltip("Affects the transition between the mid section and the shoulder of the curve. A value of 0 means no shoulder, a value of 1 means a very hard transition.")]
		public FloatParameter toneCurveShoulderStrength = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x0400368F RID: 13967
		[DisplayName("Shoulder Length")]
		[Min(0f)]
		[Tooltip("Affects how many F-stops (EV) to add to the dynamic range of the curve.")]
		public FloatParameter toneCurveShoulderLength = new FloatParameter
		{
			value = 0.5f
		};

		// Token: 0x04003690 RID: 13968
		[DisplayName("Shoulder Angle")]
		[Range(0f, 1f)]
		[Tooltip("Affects how much overshoot to add to the shoulder.")]
		public FloatParameter toneCurveShoulderAngle = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x04003691 RID: 13969
		[DisplayName("Gamma")]
		[Min(0.001f)]
		[Tooltip("Applies a gamma function to the curve.")]
		public FloatParameter toneCurveGamma = new FloatParameter
		{
			value = 1f
		};

		// Token: 0x04003692 RID: 13970
		[DisplayName("Lookup Texture")]
		[Tooltip("Custom lookup texture (strip format, for example 256x16) to apply before the rest of the color grading operators. If none is provided, a neutral one will be generated internally.")]
		public TextureParameter ldrLut = new TextureParameter
		{
			value = null,
			defaultState = TextureParameterDefault.Lut2D
		};

		// Token: 0x04003693 RID: 13971
		[DisplayName("Contribution")]
		[Range(0f, 1f)]
		[Tooltip("How much of the lookup texture will contribute to the color grading effect.")]
		public FloatParameter ldrLutContribution = new FloatParameter
		{
			value = 1f
		};

		// Token: 0x04003694 RID: 13972
		[DisplayName("Temperature")]
		[Range(-100f, 100f)]
		[Tooltip("Sets the white balance to a custom color temperature.")]
		public FloatParameter temperature = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x04003695 RID: 13973
		[DisplayName("Tint")]
		[Range(-100f, 100f)]
		[Tooltip("Sets the white balance to compensate for a green or magenta tint.")]
		public FloatParameter tint = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x04003696 RID: 13974
		[DisplayName("Color Filter")]
		[ColorUsage(false, true)]
		[Tooltip("Tint the render by multiplying a color.")]
		public ColorParameter colorFilter = new ColorParameter
		{
			value = Color.white
		};

		// Token: 0x04003697 RID: 13975
		[DisplayName("Hue Shift")]
		[Range(-180f, 180f)]
		[Tooltip("Shift the hue of all colors.")]
		public FloatParameter hueShift = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x04003698 RID: 13976
		[DisplayName("Saturation")]
		[Range(-100f, 100f)]
		[Tooltip("Pushes the intensity of all colors.")]
		public FloatParameter saturation = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x04003699 RID: 13977
		[DisplayName("Brightness")]
		[Range(-100f, 100f)]
		[Tooltip("Makes the image brighter or darker.")]
		public FloatParameter brightness = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x0400369A RID: 13978
		[DisplayName("Post-exposure (EV)")]
		[Tooltip("Adjusts the overall exposure of the scene in EV units. This is applied after the HDR effect and right before tonemapping so it won't affect previous effects in the chain.")]
		public FloatParameter postExposure = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x0400369B RID: 13979
		[DisplayName("Contrast")]
		[Range(-100f, 100f)]
		[Tooltip("Expands or shrinks the overall range of tonal values.")]
		public FloatParameter contrast = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x0400369C RID: 13980
		[DisplayName("Red")]
		[Range(-200f, 200f)]
		[Tooltip("Modify influence of the red channel in the overall mix.")]
		public FloatParameter mixerRedOutRedIn = new FloatParameter
		{
			value = 100f
		};

		// Token: 0x0400369D RID: 13981
		[DisplayName("Green")]
		[Range(-200f, 200f)]
		[Tooltip("Modify influence of the green channel in the overall mix.")]
		public FloatParameter mixerRedOutGreenIn = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x0400369E RID: 13982
		[DisplayName("Blue")]
		[Range(-200f, 200f)]
		[Tooltip("Modify influence of the blue channel in the overall mix.")]
		public FloatParameter mixerRedOutBlueIn = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x0400369F RID: 13983
		[DisplayName("Red")]
		[Range(-200f, 200f)]
		[Tooltip("Modify influence of the red channel in the overall mix.")]
		public FloatParameter mixerGreenOutRedIn = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x040036A0 RID: 13984
		[DisplayName("Green")]
		[Range(-200f, 200f)]
		[Tooltip("Modify influence of the green channel in the overall mix.")]
		public FloatParameter mixerGreenOutGreenIn = new FloatParameter
		{
			value = 100f
		};

		// Token: 0x040036A1 RID: 13985
		[DisplayName("Blue")]
		[Range(-200f, 200f)]
		[Tooltip("Modify influence of the blue channel in the overall mix.")]
		public FloatParameter mixerGreenOutBlueIn = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x040036A2 RID: 13986
		[DisplayName("Red")]
		[Range(-200f, 200f)]
		[Tooltip("Modify influence of the red channel in the overall mix.")]
		public FloatParameter mixerBlueOutRedIn = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x040036A3 RID: 13987
		[DisplayName("Green")]
		[Range(-200f, 200f)]
		[Tooltip("Modify influence of the green channel in the overall mix.")]
		public FloatParameter mixerBlueOutGreenIn = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x040036A4 RID: 13988
		[DisplayName("Blue")]
		[Range(-200f, 200f)]
		[Tooltip("Modify influence of the blue channel in the overall mix.")]
		public FloatParameter mixerBlueOutBlueIn = new FloatParameter
		{
			value = 100f
		};

		// Token: 0x040036A5 RID: 13989
		[DisplayName("Lift")]
		[Tooltip("Controls the darkest portions of the render.")]
		[Trackball(TrackballAttribute.Mode.Lift)]
		public Vector4Parameter lift = new Vector4Parameter
		{
			value = new Vector4(1f, 1f, 1f, 0f)
		};

		// Token: 0x040036A6 RID: 13990
		[DisplayName("Gamma")]
		[Tooltip("Power function that controls mid-range tones.")]
		[Trackball(TrackballAttribute.Mode.Gamma)]
		public Vector4Parameter gamma = new Vector4Parameter
		{
			value = new Vector4(1f, 1f, 1f, 0f)
		};

		// Token: 0x040036A7 RID: 13991
		[DisplayName("Gain")]
		[Tooltip("Controls the lightest portions of the render.")]
		[Trackball(TrackballAttribute.Mode.Gain)]
		public Vector4Parameter gain = new Vector4Parameter
		{
			value = new Vector4(1f, 1f, 1f, 0f)
		};

		// Token: 0x040036A8 RID: 13992
		public SplineParameter masterCurve = new SplineParameter
		{
			value = new Spline(new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 1f, 1f),
				new Keyframe(1f, 1f, 1f, 1f)
			}), 0f, false, new Vector2(0f, 1f))
		};

		// Token: 0x040036A9 RID: 13993
		public SplineParameter redCurve = new SplineParameter
		{
			value = new Spline(new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 1f, 1f),
				new Keyframe(1f, 1f, 1f, 1f)
			}), 0f, false, new Vector2(0f, 1f))
		};

		// Token: 0x040036AA RID: 13994
		public SplineParameter greenCurve = new SplineParameter
		{
			value = new Spline(new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 1f, 1f),
				new Keyframe(1f, 1f, 1f, 1f)
			}), 0f, false, new Vector2(0f, 1f))
		};

		// Token: 0x040036AB RID: 13995
		public SplineParameter blueCurve = new SplineParameter
		{
			value = new Spline(new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 1f, 1f),
				new Keyframe(1f, 1f, 1f, 1f)
			}), 0f, false, new Vector2(0f, 1f))
		};

		// Token: 0x040036AC RID: 13996
		public SplineParameter hueVsHueCurve = new SplineParameter
		{
			value = new Spline(new AnimationCurve(), 0.5f, true, new Vector2(0f, 1f))
		};

		// Token: 0x040036AD RID: 13997
		public SplineParameter hueVsSatCurve = new SplineParameter
		{
			value = new Spline(new AnimationCurve(), 0.5f, true, new Vector2(0f, 1f))
		};

		// Token: 0x040036AE RID: 13998
		public SplineParameter satVsSatCurve = new SplineParameter
		{
			value = new Spline(new AnimationCurve(), 0.5f, false, new Vector2(0f, 1f))
		};

		// Token: 0x040036AF RID: 13999
		public SplineParameter lumVsSatCurve = new SplineParameter
		{
			value = new Spline(new AnimationCurve(), 0.5f, false, new Vector2(0f, 1f))
		};
	}
}
