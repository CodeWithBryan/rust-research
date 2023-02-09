using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace VLB
{
	// Token: 0x0200097A RID: 2426
	public static class Consts
	{
		// Token: 0x17000456 RID: 1110
		// (get) Token: 0x0600397D RID: 14717 RVA: 0x00153E61 File Offset: 0x00152061
		public static HideFlags ProceduralObjectsHideFlags
		{
			get
			{
				if (!Consts.ProceduralObjectsVisibleInEditor)
				{
					return HideFlags.HideAndDontSave;
				}
				return HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild | HideFlags.DontUnloadUnusedAsset;
			}
		}

		// Token: 0x0600397E RID: 14718 RVA: 0x00153E70 File Offset: 0x00152070
		// Note: this type is marked as 'beforefieldinit'.
		static Consts()
		{
			BlendMode[] array = new BlendMode[3];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.42A4001D1CFDC98C761C0CFE5497A75F739D92F8).FieldHandle);
			Consts.BlendingMode_SrcFactor = array;
			BlendMode[] array2 = new BlendMode[3];
			RuntimeHelpers.InitializeArray(array2, fieldof(<PrivateImplementationDetails>.D950356082C70AD4018410AD313BA99D655D4D4A).FieldHandle);
			Consts.BlendingMode_DstFactor = array2;
			bool[] array3 = new bool[3];
			array3[0] = true;
			array3[1] = true;
			Consts.BlendingMode_AlphaAsBlack = array3;
		}

		// Token: 0x040033DE RID: 13278
		private const string HelpUrlBase = "http://saladgamer.com/vlb-doc/";

		// Token: 0x040033DF RID: 13279
		public const string HelpUrlBeam = "http://saladgamer.com/vlb-doc/comp-lightbeam/";

		// Token: 0x040033E0 RID: 13280
		public const string HelpUrlDustParticles = "http://saladgamer.com/vlb-doc/comp-dustparticles/";

		// Token: 0x040033E1 RID: 13281
		public const string HelpUrlDynamicOcclusion = "http://saladgamer.com/vlb-doc/comp-dynocclusion/";

		// Token: 0x040033E2 RID: 13282
		public const string HelpUrlTriggerZone = "http://saladgamer.com/vlb-doc/comp-triggerzone/";

		// Token: 0x040033E3 RID: 13283
		public const string HelpUrlConfig = "http://saladgamer.com/vlb-doc/config/";

		// Token: 0x040033E4 RID: 13284
		public static readonly bool ProceduralObjectsVisibleInEditor = true;

		// Token: 0x040033E5 RID: 13285
		public static readonly Color FlatColor = Color.white;

		// Token: 0x040033E6 RID: 13286
		public const ColorMode ColorModeDefault = ColorMode.Flat;

		// Token: 0x040033E7 RID: 13287
		public const float Alpha = 1f;

		// Token: 0x040033E8 RID: 13288
		public const float SpotAngleDefault = 35f;

		// Token: 0x040033E9 RID: 13289
		public const float SpotAngleMin = 0.1f;

		// Token: 0x040033EA RID: 13290
		public const float SpotAngleMax = 179.9f;

		// Token: 0x040033EB RID: 13291
		public const float ConeRadiusStart = 0.1f;

		// Token: 0x040033EC RID: 13292
		public const MeshType GeomMeshType = MeshType.Shared;

		// Token: 0x040033ED RID: 13293
		public const int GeomSidesDefault = 18;

		// Token: 0x040033EE RID: 13294
		public const int GeomSidesMin = 3;

		// Token: 0x040033EF RID: 13295
		public const int GeomSidesMax = 256;

		// Token: 0x040033F0 RID: 13296
		public const int GeomSegmentsDefault = 5;

		// Token: 0x040033F1 RID: 13297
		public const int GeomSegmentsMin = 0;

		// Token: 0x040033F2 RID: 13298
		public const int GeomSegmentsMax = 64;

		// Token: 0x040033F3 RID: 13299
		public const bool GeomCap = false;

		// Token: 0x040033F4 RID: 13300
		public const AttenuationEquation AttenuationEquationDefault = AttenuationEquation.Quadratic;

		// Token: 0x040033F5 RID: 13301
		public const float AttenuationCustomBlending = 0.5f;

		// Token: 0x040033F6 RID: 13302
		public const float FadeStart = 0f;

		// Token: 0x040033F7 RID: 13303
		public const float FadeEnd = 3f;

		// Token: 0x040033F8 RID: 13304
		public const float FadeMinThreshold = 0.01f;

		// Token: 0x040033F9 RID: 13305
		public const float DepthBlendDistance = 2f;

		// Token: 0x040033FA RID: 13306
		public const float CameraClippingDistance = 0.5f;

		// Token: 0x040033FB RID: 13307
		public const float FresnelPowMaxValue = 10f;

		// Token: 0x040033FC RID: 13308
		public const float FresnelPow = 8f;

		// Token: 0x040033FD RID: 13309
		public const float GlareFrontal = 0.5f;

		// Token: 0x040033FE RID: 13310
		public const float GlareBehind = 0.5f;

		// Token: 0x040033FF RID: 13311
		public const float NoiseIntensityMin = 0f;

		// Token: 0x04003400 RID: 13312
		public const float NoiseIntensityMax = 1f;

		// Token: 0x04003401 RID: 13313
		public const float NoiseIntensityDefault = 0.5f;

		// Token: 0x04003402 RID: 13314
		public const float NoiseScaleMin = 0.01f;

		// Token: 0x04003403 RID: 13315
		public const float NoiseScaleMax = 2f;

		// Token: 0x04003404 RID: 13316
		public const float NoiseScaleDefault = 0.5f;

		// Token: 0x04003405 RID: 13317
		public static readonly Vector3 NoiseVelocityDefault = new Vector3(0.07f, 0.18f, 0.05f);

		// Token: 0x04003406 RID: 13318
		public const BlendingMode BlendingModeDefault = BlendingMode.Additive;

		// Token: 0x04003407 RID: 13319
		public static readonly BlendMode[] BlendingMode_SrcFactor;

		// Token: 0x04003408 RID: 13320
		public static readonly BlendMode[] BlendingMode_DstFactor;

		// Token: 0x04003409 RID: 13321
		public static readonly bool[] BlendingMode_AlphaAsBlack;

		// Token: 0x0400340A RID: 13322
		public const float DynOcclusionMinSurfaceRatioDefault = 0.5f;

		// Token: 0x0400340B RID: 13323
		public const float DynOcclusionMinSurfaceRatioMin = 50f;

		// Token: 0x0400340C RID: 13324
		public const float DynOcclusionMinSurfaceRatioMax = 100f;

		// Token: 0x0400340D RID: 13325
		public const float DynOcclusionMaxSurfaceDotDefault = 0.25f;

		// Token: 0x0400340E RID: 13326
		public const float DynOcclusionMaxSurfaceAngleMin = 45f;

		// Token: 0x0400340F RID: 13327
		public const float DynOcclusionMaxSurfaceAngleMax = 90f;

		// Token: 0x04003410 RID: 13328
		public const int ConfigGeometryLayerIDDefault = 1;

		// Token: 0x04003411 RID: 13329
		public const string ConfigGeometryTagDefault = "Untagged";

		// Token: 0x04003412 RID: 13330
		public const RenderQueue ConfigGeometryRenderQueueDefault = RenderQueue.Transparent;

		// Token: 0x04003413 RID: 13331
		public const bool ConfigGeometryForceSinglePassDefault = false;

		// Token: 0x04003414 RID: 13332
		public const int ConfigNoise3DSizeDefault = 64;

		// Token: 0x04003415 RID: 13333
		public const int ConfigSharedMeshSides = 24;

		// Token: 0x04003416 RID: 13334
		public const int ConfigSharedMeshSegments = 5;
	}
}
