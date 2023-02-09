using System;
using Rust.Workshop;
using UnityEngine;
using UnityEngine.Rendering;

namespace ConVar
{
	// Token: 0x02000A7C RID: 2684
	[ConsoleSystem.Factory("graphics")]
	public class Graphics : ConsoleSystem
	{
		// Token: 0x17000547 RID: 1351
		// (get) Token: 0x0600400C RID: 16396 RVA: 0x0017994A File Offset: 0x00177B4A
		// (set) Token: 0x0600400D RID: 16397 RVA: 0x00179951 File Offset: 0x00177B51
		[ClientVar(Help = "The currently selected quality level")]
		public static int quality
		{
			get
			{
				return QualitySettings.GetQualityLevel();
			}
			set
			{
				int shadowcascades = Graphics.shadowcascades;
				QualitySettings.SetQualityLevel(value, true);
				Graphics.shadowcascades = shadowcascades;
			}
		}

		// Token: 0x0600400E RID: 16398 RVA: 0x00179964 File Offset: 0x00177B64
		public static float EnforceShadowDistanceBounds(float distance)
		{
			if (QualitySettings.shadowCascades == 1)
			{
				distance = Mathf.Clamp(distance, 100f, 100f);
			}
			else if (QualitySettings.shadowCascades == 2)
			{
				distance = Mathf.Clamp(distance, 100f, 600f);
			}
			else
			{
				distance = Mathf.Clamp(distance, 100f, 1000f);
			}
			return distance;
		}

		// Token: 0x17000548 RID: 1352
		// (get) Token: 0x0600400F RID: 16399 RVA: 0x001799BC File Offset: 0x00177BBC
		// (set) Token: 0x06004010 RID: 16400 RVA: 0x001799C3 File Offset: 0x00177BC3
		[ClientVar(Saved = true)]
		public static float shadowdistance
		{
			get
			{
				return Graphics._shadowdistance;
			}
			set
			{
				Graphics._shadowdistance = value;
				QualitySettings.shadowDistance = Graphics.EnforceShadowDistanceBounds(Graphics._shadowdistance);
			}
		}

		// Token: 0x17000549 RID: 1353
		// (get) Token: 0x06004011 RID: 16401 RVA: 0x001799DA File Offset: 0x00177BDA
		// (set) Token: 0x06004012 RID: 16402 RVA: 0x001799E1 File Offset: 0x00177BE1
		[ClientVar(Saved = true)]
		public static int shadowcascades
		{
			get
			{
				return QualitySettings.shadowCascades;
			}
			set
			{
				QualitySettings.shadowCascades = value;
				QualitySettings.shadowDistance = Graphics.EnforceShadowDistanceBounds(Graphics.shadowdistance);
			}
		}

		// Token: 0x1700054A RID: 1354
		// (get) Token: 0x06004013 RID: 16403 RVA: 0x001799F8 File Offset: 0x00177BF8
		// (set) Token: 0x06004014 RID: 16404 RVA: 0x00179A00 File Offset: 0x00177C00
		[ClientVar(Saved = true)]
		public static int shadowquality
		{
			get
			{
				return Graphics._shadowquality;
			}
			set
			{
				Graphics._shadowquality = Mathf.Clamp(value, 0, 3);
				Graphics.shadowmode = Graphics._shadowquality + 1;
				bool flag = SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLCore;
				KeywordUtil.EnsureKeywordState("SHADOW_QUALITY_HIGH", !flag && Graphics._shadowquality == 2);
				KeywordUtil.EnsureKeywordState("SHADOW_QUALITY_VERYHIGH", !flag && Graphics._shadowquality == 3);
			}
		}

		// Token: 0x1700054B RID: 1355
		// (get) Token: 0x06004015 RID: 16405 RVA: 0x00179A60 File Offset: 0x00177C60
		// (set) Token: 0x06004016 RID: 16406 RVA: 0x00179A67 File Offset: 0x00177C67
		[ClientVar(Saved = true)]
		public static float fov
		{
			get
			{
				return Graphics._fov;
			}
			set
			{
				Graphics._fov = Mathf.Clamp(value, 70f, 90f);
			}
		}

		// Token: 0x1700054C RID: 1356
		// (get) Token: 0x06004017 RID: 16407 RVA: 0x00179A7E File Offset: 0x00177C7E
		// (set) Token: 0x06004018 RID: 16408 RVA: 0x00179A85 File Offset: 0x00177C85
		[ClientVar]
		public static float lodbias
		{
			get
			{
				return QualitySettings.lodBias;
			}
			set
			{
				QualitySettings.lodBias = Mathf.Clamp(value, 0.25f, 5f);
			}
		}

		// Token: 0x06004019 RID: 16409 RVA: 0x000059DD File Offset: 0x00003BDD
		[ClientVar(ClientAdmin = true)]
		public static void dof_focus_target(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x0600401A RID: 16410 RVA: 0x00179A9C File Offset: 0x00177C9C
		[ClientVar]
		public static void dof_nudge(ConsoleSystem.Arg arg)
		{
			float @float = arg.GetFloat(0, 0f);
			Graphics.dof_focus_dist += @float;
			if (Graphics.dof_focus_dist < 0f)
			{
				Graphics.dof_focus_dist = 0f;
			}
		}

		// Token: 0x1700054D RID: 1357
		// (get) Token: 0x0600401B RID: 16411 RVA: 0x00179AD8 File Offset: 0x00177CD8
		// (set) Token: 0x0600401C RID: 16412 RVA: 0x00179ADF File Offset: 0x00177CDF
		[ClientVar(Saved = true)]
		public static int shaderlod
		{
			get
			{
				return Shader.globalMaximumLOD;
			}
			set
			{
				Shader.globalMaximumLOD = Mathf.Clamp(value, 100, 600);
			}
		}

		// Token: 0x1700054E RID: 1358
		// (get) Token: 0x0600401D RID: 16413 RVA: 0x00179AF3 File Offset: 0x00177CF3
		// (set) Token: 0x0600401E RID: 16414 RVA: 0x00179AFA File Offset: 0x00177CFA
		[ClientVar(Saved = true)]
		public static float uiscale
		{
			get
			{
				return Graphics._uiscale;
			}
			set
			{
				Graphics._uiscale = Mathf.Clamp(value, 0.5f, 1f);
			}
		}

		// Token: 0x1700054F RID: 1359
		// (get) Token: 0x0600401F RID: 16415 RVA: 0x00179B11 File Offset: 0x00177D11
		// (set) Token: 0x06004020 RID: 16416 RVA: 0x00179B18 File Offset: 0x00177D18
		[ClientVar(Saved = true)]
		public static int af
		{
			get
			{
				return Graphics._anisotropic;
			}
			set
			{
				value = Mathf.Clamp(value, 1, 16);
				Texture.SetGlobalAnisotropicFilteringLimits(1, value);
				if (value <= 1)
				{
					Texture.anisotropicFiltering = AnisotropicFiltering.Disable;
				}
				if (value > 1)
				{
					Texture.anisotropicFiltering = AnisotropicFiltering.Enable;
				}
				Graphics._anisotropic = value;
			}
		}

		// Token: 0x17000550 RID: 1360
		// (get) Token: 0x06004021 RID: 16417 RVA: 0x00179B46 File Offset: 0x00177D46
		// (set) Token: 0x06004022 RID: 16418 RVA: 0x00179B50 File Offset: 0x00177D50
		[ClientVar(Saved = true)]
		public static int parallax
		{
			get
			{
				return Graphics._parallax;
			}
			set
			{
				if (value != 1)
				{
					if (value != 2)
					{
						Shader.DisableKeyword("TERRAIN_PARALLAX_OFFSET");
						Shader.DisableKeyword("TERRAIN_PARALLAX_OCCLUSION");
					}
					else
					{
						Shader.DisableKeyword("TERRAIN_PARALLAX_OFFSET");
						Shader.EnableKeyword("TERRAIN_PARALLAX_OCCLUSION");
					}
				}
				else
				{
					Shader.EnableKeyword("TERRAIN_PARALLAX_OFFSET");
					Shader.DisableKeyword("TERRAIN_PARALLAX_OCCLUSION");
				}
				Graphics._parallax = value;
			}
		}

		// Token: 0x17000551 RID: 1361
		// (get) Token: 0x06004023 RID: 16419 RVA: 0x00179BAB File Offset: 0x00177DAB
		// (set) Token: 0x06004024 RID: 16420 RVA: 0x00179BB2 File Offset: 0x00177DB2
		[ClientVar]
		public static bool itemskins
		{
			get
			{
				return Rust.Workshop.WorkshopSkin.AllowApply;
			}
			set
			{
				Rust.Workshop.WorkshopSkin.AllowApply = value;
			}
		}

		// Token: 0x17000552 RID: 1362
		// (get) Token: 0x06004025 RID: 16421 RVA: 0x00179BBA File Offset: 0x00177DBA
		// (set) Token: 0x06004026 RID: 16422 RVA: 0x00179BC1 File Offset: 0x00177DC1
		[ClientVar]
		public static bool itemskinunload
		{
			get
			{
				return Rust.Workshop.WorkshopSkin.AllowUnload;
			}
			set
			{
				Rust.Workshop.WorkshopSkin.AllowUnload = value;
			}
		}

		// Token: 0x17000553 RID: 1363
		// (get) Token: 0x06004027 RID: 16423 RVA: 0x00179BC9 File Offset: 0x00177DC9
		// (set) Token: 0x06004028 RID: 16424 RVA: 0x00179BD0 File Offset: 0x00177DD0
		[ClientVar]
		public static float itemskintimeout
		{
			get
			{
				return Rust.Workshop.WorkshopSkin.DownloadTimeout;
			}
			set
			{
				Rust.Workshop.WorkshopSkin.DownloadTimeout = value;
			}
		}

		// Token: 0x04003944 RID: 14660
		private const float MinShadowDistance = 100f;

		// Token: 0x04003945 RID: 14661
		private const float MaxShadowDistance2Split = 600f;

		// Token: 0x04003946 RID: 14662
		private const float MaxShadowDistance4Split = 1000f;

		// Token: 0x04003947 RID: 14663
		private static float _shadowdistance = 1000f;

		// Token: 0x04003948 RID: 14664
		[ClientVar(Saved = true)]
		public static int shadowmode = 2;

		// Token: 0x04003949 RID: 14665
		[ClientVar(Saved = true)]
		public static int shadowlights = 1;

		// Token: 0x0400394A RID: 14666
		private static int _shadowquality = 1;

		// Token: 0x0400394B RID: 14667
		[ClientVar(Saved = true)]
		public static bool grassshadows = false;

		// Token: 0x0400394C RID: 14668
		[ClientVar(Saved = true)]
		public static bool contactshadows = false;

		// Token: 0x0400394D RID: 14669
		[ClientVar(Saved = true)]
		public static float drawdistance = 2500f;

		// Token: 0x0400394E RID: 14670
		private static float _fov = 75f;

		// Token: 0x0400394F RID: 14671
		[ClientVar]
		public static bool hud = true;

		// Token: 0x04003950 RID: 14672
		[ClientVar(Saved = true)]
		public static bool chat = true;

		// Token: 0x04003951 RID: 14673
		[ClientVar(Saved = true)]
		public static bool branding = true;

		// Token: 0x04003952 RID: 14674
		[ClientVar(Saved = true)]
		public static int compass = 1;

		// Token: 0x04003953 RID: 14675
		[ClientVar(Saved = true)]
		public static bool dof = false;

		// Token: 0x04003954 RID: 14676
		[ClientVar(Saved = true)]
		public static float dof_aper = 12f;

		// Token: 0x04003955 RID: 14677
		[ClientVar(Saved = true)]
		public static float dof_blur = 1f;

		// Token: 0x04003956 RID: 14678
		[ClientVar(Saved = true, Help = "0 = auto 1 = manual 2 = dynamic based on target")]
		public static int dof_mode = 0;

		// Token: 0x04003957 RID: 14679
		[ClientVar(Saved = true, Help = "distance from camera to focus on")]
		public static float dof_focus_dist = 10f;

		// Token: 0x04003958 RID: 14680
		[ClientVar(Saved = true)]
		public static float dof_focus_time = 0.2f;

		// Token: 0x04003959 RID: 14681
		[ClientVar(Saved = true, ClientAdmin = true)]
		public static bool dof_debug = false;

		// Token: 0x0400395A RID: 14682
		[ClientVar(Saved = true, Help = "Goes from 0 - 3, higher = more dof samples but slower perf")]
		public static int dof_kernel_count = 0;

		// Token: 0x0400395B RID: 14683
		public static BaseEntity dof_focus_target_entity = null;

		// Token: 0x0400395C RID: 14684
		[ClientVar(Saved = true, Help = "Whether to scale vm models with fov")]
		public static bool vm_fov_scale = true;

		// Token: 0x0400395D RID: 14685
		[ClientVar(Saved = true, Help = "FLips viewmodels horizontally (for left handed players)")]
		public static bool vm_horizontal_flip = false;

		// Token: 0x0400395E RID: 14686
		private static float _uiscale = 1f;

		// Token: 0x0400395F RID: 14687
		private static int _anisotropic = 1;

		// Token: 0x04003960 RID: 14688
		private static int _parallax = 0;
	}
}
