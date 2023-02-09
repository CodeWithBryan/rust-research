using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A5B RID: 2651
	internal static class ShaderIDs
	{
		// Token: 0x040037AE RID: 14254
		internal static readonly int MainTex = Shader.PropertyToID("_MainTex");

		// Token: 0x040037AF RID: 14255
		internal static readonly int Jitter = Shader.PropertyToID("_Jitter");

		// Token: 0x040037B0 RID: 14256
		internal static readonly int Sharpness = Shader.PropertyToID("_Sharpness");

		// Token: 0x040037B1 RID: 14257
		internal static readonly int FinalBlendParameters = Shader.PropertyToID("_FinalBlendParameters");

		// Token: 0x040037B2 RID: 14258
		internal static readonly int HistoryTex = Shader.PropertyToID("_HistoryTex");

		// Token: 0x040037B3 RID: 14259
		internal static readonly int SMAA_Flip = Shader.PropertyToID("_SMAA_Flip");

		// Token: 0x040037B4 RID: 14260
		internal static readonly int SMAA_Flop = Shader.PropertyToID("_SMAA_Flop");

		// Token: 0x040037B5 RID: 14261
		internal static readonly int AOParams = Shader.PropertyToID("_AOParams");

		// Token: 0x040037B6 RID: 14262
		internal static readonly int AOColor = Shader.PropertyToID("_AOColor");

		// Token: 0x040037B7 RID: 14263
		internal static readonly int OcclusionTexture1 = Shader.PropertyToID("_OcclusionTexture1");

		// Token: 0x040037B8 RID: 14264
		internal static readonly int OcclusionTexture2 = Shader.PropertyToID("_OcclusionTexture2");

		// Token: 0x040037B9 RID: 14265
		internal static readonly int SAOcclusionTexture = Shader.PropertyToID("_SAOcclusionTexture");

		// Token: 0x040037BA RID: 14266
		internal static readonly int MSVOcclusionTexture = Shader.PropertyToID("_MSVOcclusionTexture");

		// Token: 0x040037BB RID: 14267
		internal static readonly int DepthCopy = Shader.PropertyToID("DepthCopy");

		// Token: 0x040037BC RID: 14268
		internal static readonly int LinearDepth = Shader.PropertyToID("LinearDepth");

		// Token: 0x040037BD RID: 14269
		internal static readonly int LowDepth1 = Shader.PropertyToID("LowDepth1");

		// Token: 0x040037BE RID: 14270
		internal static readonly int LowDepth2 = Shader.PropertyToID("LowDepth2");

		// Token: 0x040037BF RID: 14271
		internal static readonly int LowDepth3 = Shader.PropertyToID("LowDepth3");

		// Token: 0x040037C0 RID: 14272
		internal static readonly int LowDepth4 = Shader.PropertyToID("LowDepth4");

		// Token: 0x040037C1 RID: 14273
		internal static readonly int TiledDepth1 = Shader.PropertyToID("TiledDepth1");

		// Token: 0x040037C2 RID: 14274
		internal static readonly int TiledDepth2 = Shader.PropertyToID("TiledDepth2");

		// Token: 0x040037C3 RID: 14275
		internal static readonly int TiledDepth3 = Shader.PropertyToID("TiledDepth3");

		// Token: 0x040037C4 RID: 14276
		internal static readonly int TiledDepth4 = Shader.PropertyToID("TiledDepth4");

		// Token: 0x040037C5 RID: 14277
		internal static readonly int Occlusion1 = Shader.PropertyToID("Occlusion1");

		// Token: 0x040037C6 RID: 14278
		internal static readonly int Occlusion2 = Shader.PropertyToID("Occlusion2");

		// Token: 0x040037C7 RID: 14279
		internal static readonly int Occlusion3 = Shader.PropertyToID("Occlusion3");

		// Token: 0x040037C8 RID: 14280
		internal static readonly int Occlusion4 = Shader.PropertyToID("Occlusion4");

		// Token: 0x040037C9 RID: 14281
		internal static readonly int Combined1 = Shader.PropertyToID("Combined1");

		// Token: 0x040037CA RID: 14282
		internal static readonly int Combined2 = Shader.PropertyToID("Combined2");

		// Token: 0x040037CB RID: 14283
		internal static readonly int Combined3 = Shader.PropertyToID("Combined3");

		// Token: 0x040037CC RID: 14284
		internal static readonly int SSRResolveTemp = Shader.PropertyToID("_SSRResolveTemp");

		// Token: 0x040037CD RID: 14285
		internal static readonly int Noise = Shader.PropertyToID("_Noise");

		// Token: 0x040037CE RID: 14286
		internal static readonly int Test = Shader.PropertyToID("_Test");

		// Token: 0x040037CF RID: 14287
		internal static readonly int Resolve = Shader.PropertyToID("_Resolve");

		// Token: 0x040037D0 RID: 14288
		internal static readonly int History = Shader.PropertyToID("_History");

		// Token: 0x040037D1 RID: 14289
		internal static readonly int ViewMatrix = Shader.PropertyToID("_ViewMatrix");

		// Token: 0x040037D2 RID: 14290
		internal static readonly int InverseViewMatrix = Shader.PropertyToID("_InverseViewMatrix");

		// Token: 0x040037D3 RID: 14291
		internal static readonly int InverseProjectionMatrix = Shader.PropertyToID("_InverseProjectionMatrix");

		// Token: 0x040037D4 RID: 14292
		internal static readonly int ScreenSpaceProjectionMatrix = Shader.PropertyToID("_ScreenSpaceProjectionMatrix");

		// Token: 0x040037D5 RID: 14293
		internal static readonly int Params2 = Shader.PropertyToID("_Params2");

		// Token: 0x040037D6 RID: 14294
		internal static readonly int FogColor = Shader.PropertyToID("_FogColor");

		// Token: 0x040037D7 RID: 14295
		internal static readonly int FogParams = Shader.PropertyToID("_FogParams");

		// Token: 0x040037D8 RID: 14296
		internal static readonly int VelocityScale = Shader.PropertyToID("_VelocityScale");

		// Token: 0x040037D9 RID: 14297
		internal static readonly int MaxBlurRadius = Shader.PropertyToID("_MaxBlurRadius");

		// Token: 0x040037DA RID: 14298
		internal static readonly int RcpMaxBlurRadius = Shader.PropertyToID("_RcpMaxBlurRadius");

		// Token: 0x040037DB RID: 14299
		internal static readonly int VelocityTex = Shader.PropertyToID("_VelocityTex");

		// Token: 0x040037DC RID: 14300
		internal static readonly int Tile2RT = Shader.PropertyToID("_Tile2RT");

		// Token: 0x040037DD RID: 14301
		internal static readonly int Tile4RT = Shader.PropertyToID("_Tile4RT");

		// Token: 0x040037DE RID: 14302
		internal static readonly int Tile8RT = Shader.PropertyToID("_Tile8RT");

		// Token: 0x040037DF RID: 14303
		internal static readonly int TileMaxOffs = Shader.PropertyToID("_TileMaxOffs");

		// Token: 0x040037E0 RID: 14304
		internal static readonly int TileMaxLoop = Shader.PropertyToID("_TileMaxLoop");

		// Token: 0x040037E1 RID: 14305
		internal static readonly int TileVRT = Shader.PropertyToID("_TileVRT");

		// Token: 0x040037E2 RID: 14306
		internal static readonly int NeighborMaxTex = Shader.PropertyToID("_NeighborMaxTex");

		// Token: 0x040037E3 RID: 14307
		internal static readonly int LoopCount = Shader.PropertyToID("_LoopCount");

		// Token: 0x040037E4 RID: 14308
		internal static readonly int DepthOfFieldTemp = Shader.PropertyToID("_DepthOfFieldTemp");

		// Token: 0x040037E5 RID: 14309
		internal static readonly int DepthOfFieldTex = Shader.PropertyToID("_DepthOfFieldTex");

		// Token: 0x040037E6 RID: 14310
		internal static readonly int Distance = Shader.PropertyToID("_Distance");

		// Token: 0x040037E7 RID: 14311
		internal static readonly int LensCoeff = Shader.PropertyToID("_LensCoeff");

		// Token: 0x040037E8 RID: 14312
		internal static readonly int MaxCoC = Shader.PropertyToID("_MaxCoC");

		// Token: 0x040037E9 RID: 14313
		internal static readonly int RcpMaxCoC = Shader.PropertyToID("_RcpMaxCoC");

		// Token: 0x040037EA RID: 14314
		internal static readonly int RcpAspect = Shader.PropertyToID("_RcpAspect");

		// Token: 0x040037EB RID: 14315
		internal static readonly int CoCTex = Shader.PropertyToID("_CoCTex");

		// Token: 0x040037EC RID: 14316
		internal static readonly int TaaParams = Shader.PropertyToID("_TaaParams");

		// Token: 0x040037ED RID: 14317
		internal static readonly int AutoExposureTex = Shader.PropertyToID("_AutoExposureTex");

		// Token: 0x040037EE RID: 14318
		internal static readonly int HistogramBuffer = Shader.PropertyToID("_HistogramBuffer");

		// Token: 0x040037EF RID: 14319
		internal static readonly int Params = Shader.PropertyToID("_Params");

		// Token: 0x040037F0 RID: 14320
		internal static readonly int ScaleOffsetRes = Shader.PropertyToID("_ScaleOffsetRes");

		// Token: 0x040037F1 RID: 14321
		internal static readonly int BloomTex = Shader.PropertyToID("_BloomTex");

		// Token: 0x040037F2 RID: 14322
		internal static readonly int SampleScale = Shader.PropertyToID("_SampleScale");

		// Token: 0x040037F3 RID: 14323
		internal static readonly int Threshold = Shader.PropertyToID("_Threshold");

		// Token: 0x040037F4 RID: 14324
		internal static readonly int ColorIntensity = Shader.PropertyToID("_ColorIntensity");

		// Token: 0x040037F5 RID: 14325
		internal static readonly int Bloom_DirtTex = Shader.PropertyToID("_Bloom_DirtTex");

		// Token: 0x040037F6 RID: 14326
		internal static readonly int Bloom_Settings = Shader.PropertyToID("_Bloom_Settings");

		// Token: 0x040037F7 RID: 14327
		internal static readonly int Bloom_Color = Shader.PropertyToID("_Bloom_Color");

		// Token: 0x040037F8 RID: 14328
		internal static readonly int Bloom_DirtTileOffset = Shader.PropertyToID("_Bloom_DirtTileOffset");

		// Token: 0x040037F9 RID: 14329
		internal static readonly int ChromaticAberration_Amount = Shader.PropertyToID("_ChromaticAberration_Amount");

		// Token: 0x040037FA RID: 14330
		internal static readonly int ChromaticAberration_SpectralLut = Shader.PropertyToID("_ChromaticAberration_SpectralLut");

		// Token: 0x040037FB RID: 14331
		internal static readonly int Distortion_CenterScale = Shader.PropertyToID("_Distortion_CenterScale");

		// Token: 0x040037FC RID: 14332
		internal static readonly int Distortion_Amount = Shader.PropertyToID("_Distortion_Amount");

		// Token: 0x040037FD RID: 14333
		internal static readonly int Lut2D = Shader.PropertyToID("_Lut2D");

		// Token: 0x040037FE RID: 14334
		internal static readonly int Lut3D = Shader.PropertyToID("_Lut3D");

		// Token: 0x040037FF RID: 14335
		internal static readonly int Lut3D_Params = Shader.PropertyToID("_Lut3D_Params");

		// Token: 0x04003800 RID: 14336
		internal static readonly int Lut2D_Params = Shader.PropertyToID("_Lut2D_Params");

		// Token: 0x04003801 RID: 14337
		internal static readonly int UserLut2D_Params = Shader.PropertyToID("_UserLut2D_Params");

		// Token: 0x04003802 RID: 14338
		internal static readonly int PostExposure = Shader.PropertyToID("_PostExposure");

		// Token: 0x04003803 RID: 14339
		internal static readonly int ColorBalance = Shader.PropertyToID("_ColorBalance");

		// Token: 0x04003804 RID: 14340
		internal static readonly int ColorFilter = Shader.PropertyToID("_ColorFilter");

		// Token: 0x04003805 RID: 14341
		internal static readonly int HueSatCon = Shader.PropertyToID("_HueSatCon");

		// Token: 0x04003806 RID: 14342
		internal static readonly int Brightness = Shader.PropertyToID("_Brightness");

		// Token: 0x04003807 RID: 14343
		internal static readonly int ChannelMixerRed = Shader.PropertyToID("_ChannelMixerRed");

		// Token: 0x04003808 RID: 14344
		internal static readonly int ChannelMixerGreen = Shader.PropertyToID("_ChannelMixerGreen");

		// Token: 0x04003809 RID: 14345
		internal static readonly int ChannelMixerBlue = Shader.PropertyToID("_ChannelMixerBlue");

		// Token: 0x0400380A RID: 14346
		internal static readonly int Lift = Shader.PropertyToID("_Lift");

		// Token: 0x0400380B RID: 14347
		internal static readonly int InvGamma = Shader.PropertyToID("_InvGamma");

		// Token: 0x0400380C RID: 14348
		internal static readonly int Gain = Shader.PropertyToID("_Gain");

		// Token: 0x0400380D RID: 14349
		internal static readonly int Curves = Shader.PropertyToID("_Curves");

		// Token: 0x0400380E RID: 14350
		internal static readonly int CustomToneCurve = Shader.PropertyToID("_CustomToneCurve");

		// Token: 0x0400380F RID: 14351
		internal static readonly int ToeSegmentA = Shader.PropertyToID("_ToeSegmentA");

		// Token: 0x04003810 RID: 14352
		internal static readonly int ToeSegmentB = Shader.PropertyToID("_ToeSegmentB");

		// Token: 0x04003811 RID: 14353
		internal static readonly int MidSegmentA = Shader.PropertyToID("_MidSegmentA");

		// Token: 0x04003812 RID: 14354
		internal static readonly int MidSegmentB = Shader.PropertyToID("_MidSegmentB");

		// Token: 0x04003813 RID: 14355
		internal static readonly int ShoSegmentA = Shader.PropertyToID("_ShoSegmentA");

		// Token: 0x04003814 RID: 14356
		internal static readonly int ShoSegmentB = Shader.PropertyToID("_ShoSegmentB");

		// Token: 0x04003815 RID: 14357
		internal static readonly int Vignette_Color = Shader.PropertyToID("_Vignette_Color");

		// Token: 0x04003816 RID: 14358
		internal static readonly int Vignette_Center = Shader.PropertyToID("_Vignette_Center");

		// Token: 0x04003817 RID: 14359
		internal static readonly int Vignette_Settings = Shader.PropertyToID("_Vignette_Settings");

		// Token: 0x04003818 RID: 14360
		internal static readonly int Vignette_Mask = Shader.PropertyToID("_Vignette_Mask");

		// Token: 0x04003819 RID: 14361
		internal static readonly int Vignette_Opacity = Shader.PropertyToID("_Vignette_Opacity");

		// Token: 0x0400381A RID: 14362
		internal static readonly int Vignette_Mode = Shader.PropertyToID("_Vignette_Mode");

		// Token: 0x0400381B RID: 14363
		internal static readonly int Grain_Params1 = Shader.PropertyToID("_Grain_Params1");

		// Token: 0x0400381C RID: 14364
		internal static readonly int Grain_Params2 = Shader.PropertyToID("_Grain_Params2");

		// Token: 0x0400381D RID: 14365
		internal static readonly int GrainTex = Shader.PropertyToID("_GrainTex");

		// Token: 0x0400381E RID: 14366
		internal static readonly int Phase = Shader.PropertyToID("_Phase");

		// Token: 0x0400381F RID: 14367
		internal static readonly int GrainNoiseParameters = Shader.PropertyToID("_NoiseParameters");

		// Token: 0x04003820 RID: 14368
		internal static readonly int LumaInAlpha = Shader.PropertyToID("_LumaInAlpha");

		// Token: 0x04003821 RID: 14369
		internal static readonly int DitheringTex = Shader.PropertyToID("_DitheringTex");

		// Token: 0x04003822 RID: 14370
		internal static readonly int Dithering_Coords = Shader.PropertyToID("_Dithering_Coords");

		// Token: 0x04003823 RID: 14371
		internal static readonly int From = Shader.PropertyToID("_From");

		// Token: 0x04003824 RID: 14372
		internal static readonly int To = Shader.PropertyToID("_To");

		// Token: 0x04003825 RID: 14373
		internal static readonly int Interp = Shader.PropertyToID("_Interp");

		// Token: 0x04003826 RID: 14374
		internal static readonly int TargetColor = Shader.PropertyToID("_TargetColor");

		// Token: 0x04003827 RID: 14375
		internal static readonly int HalfResFinalCopy = Shader.PropertyToID("_HalfResFinalCopy");

		// Token: 0x04003828 RID: 14376
		internal static readonly int WaveformSource = Shader.PropertyToID("_WaveformSource");

		// Token: 0x04003829 RID: 14377
		internal static readonly int WaveformBuffer = Shader.PropertyToID("_WaveformBuffer");

		// Token: 0x0400382A RID: 14378
		internal static readonly int VectorscopeBuffer = Shader.PropertyToID("_VectorscopeBuffer");

		// Token: 0x0400382B RID: 14379
		internal static readonly int RenderViewportScaleFactor = Shader.PropertyToID("_RenderViewportScaleFactor");

		// Token: 0x0400382C RID: 14380
		internal static readonly int UVTransform = Shader.PropertyToID("_UVTransform");

		// Token: 0x0400382D RID: 14381
		internal static readonly int DepthSlice = Shader.PropertyToID("_DepthSlice");

		// Token: 0x0400382E RID: 14382
		internal static readonly int UVScaleOffset = Shader.PropertyToID("_UVScaleOffset");

		// Token: 0x0400382F RID: 14383
		internal static readonly int PosScaleOffset = Shader.PropertyToID("_PosScaleOffset");
	}
}
