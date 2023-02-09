using System;
using System.Globalization;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000AA5 RID: 2725
	[ConsoleSystem.Factory("weather")]
	public class Weather : ConsoleSystem
	{
		// Token: 0x0600410B RID: 16651 RVA: 0x0017E8D4 File Offset: 0x0017CAD4
		[ClientVar]
		[ServerVar]
		public static void load(ConsoleSystem.Arg args)
		{
			if (!SingletonComponent<Climate>.Instance)
			{
				return;
			}
			string name = args.GetString(0, "");
			if (string.IsNullOrEmpty(name))
			{
				args.ReplyWith("Weather preset name invalid.");
				return;
			}
			WeatherPreset weatherPreset = Array.Find<WeatherPreset>(SingletonComponent<Climate>.Instance.WeatherPresets, (WeatherPreset x) => x.name.Contains(name, CompareOptions.IgnoreCase));
			if (weatherPreset == null)
			{
				args.ReplyWith("Weather preset not found: " + name);
				return;
			}
			SingletonComponent<Climate>.Instance.WeatherOverrides.Set(weatherPreset);
			if (args.IsServerside)
			{
				ServerMgr.SendReplicatedVars("weather.");
			}
		}

		// Token: 0x0600410C RID: 16652 RVA: 0x0017E97D File Offset: 0x0017CB7D
		[ClientVar]
		[ServerVar]
		public static void reset(ConsoleSystem.Arg args)
		{
			if (!SingletonComponent<Climate>.Instance)
			{
				return;
			}
			SingletonComponent<Climate>.Instance.WeatherOverrides.Reset();
			if (args.IsServerside)
			{
				ServerMgr.SendReplicatedVars("weather.");
			}
		}

		// Token: 0x0600410D RID: 16653 RVA: 0x0017E9B0 File Offset: 0x0017CBB0
		[ClientVar]
		[ServerVar]
		public static void report(ConsoleSystem.Arg args)
		{
			if (!SingletonComponent<Climate>.Instance)
			{
				return;
			}
			TextTable textTable = new TextTable();
			textTable.AddColumn(SingletonComponent<Climate>.Instance.WeatherStatePrevious.name);
			textTable.AddColumn("|");
			textTable.AddColumn(SingletonComponent<Climate>.Instance.WeatherStateTarget.name);
			textTable.AddColumn("|");
			textTable.AddColumn(SingletonComponent<Climate>.Instance.WeatherStateNext.name);
			int num = Mathf.RoundToInt(SingletonComponent<Climate>.Instance.WeatherStateBlend * 100f);
			if (num < 100)
			{
				textTable.AddRow(new string[]
				{
					"fading out (" + (100 - num) + "%)",
					"|",
					"fading in (" + num + "%)",
					"|",
					"up next"
				});
			}
			else
			{
				textTable.AddRow(new string[]
				{
					"previous",
					"|",
					"current",
					"|",
					"up next"
				});
			}
			args.ReplyWith(textTable.ToString() + Environment.NewLine + SingletonComponent<Climate>.Instance.WeatherState.ToString());
		}

		// Token: 0x1700056E RID: 1390
		// (get) Token: 0x0600410E RID: 16654 RVA: 0x0017EAF2 File Offset: 0x0017CCF2
		// (set) Token: 0x0600410F RID: 16655 RVA: 0x0017EB15 File Offset: 0x0017CD15
		[ReplicatedVar(Default = "1")]
		public static float clear_chance
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return 1f;
				}
				return SingletonComponent<Climate>.Instance.Weather.ClearChance;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.Weather.ClearChance = Mathf.Clamp01(value);
			}
		}

		// Token: 0x1700056F RID: 1391
		// (get) Token: 0x06004110 RID: 16656 RVA: 0x0017EB39 File Offset: 0x0017CD39
		// (set) Token: 0x06004111 RID: 16657 RVA: 0x0017EB5C File Offset: 0x0017CD5C
		[ReplicatedVar(Default = "0")]
		public static float dust_chance
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return 0f;
				}
				return SingletonComponent<Climate>.Instance.Weather.DustChance;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.Weather.DustChance = Mathf.Clamp01(value);
			}
		}

		// Token: 0x17000570 RID: 1392
		// (get) Token: 0x06004112 RID: 16658 RVA: 0x0017EB80 File Offset: 0x0017CD80
		// (set) Token: 0x06004113 RID: 16659 RVA: 0x0017EBA3 File Offset: 0x0017CDA3
		[ReplicatedVar(Default = "0")]
		public static float fog_chance
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return 0f;
				}
				return SingletonComponent<Climate>.Instance.Weather.FogChance;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.Weather.FogChance = Mathf.Clamp01(value);
			}
		}

		// Token: 0x17000571 RID: 1393
		// (get) Token: 0x06004114 RID: 16660 RVA: 0x0017EBC7 File Offset: 0x0017CDC7
		// (set) Token: 0x06004115 RID: 16661 RVA: 0x0017EBEA File Offset: 0x0017CDEA
		[ReplicatedVar(Default = "0")]
		public static float overcast_chance
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return 0f;
				}
				return SingletonComponent<Climate>.Instance.Weather.OvercastChance;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.Weather.OvercastChance = Mathf.Clamp01(value);
			}
		}

		// Token: 0x17000572 RID: 1394
		// (get) Token: 0x06004116 RID: 16662 RVA: 0x0017EC0E File Offset: 0x0017CE0E
		// (set) Token: 0x06004117 RID: 16663 RVA: 0x0017EC31 File Offset: 0x0017CE31
		[ReplicatedVar(Default = "0")]
		public static float storm_chance
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return 0f;
				}
				return SingletonComponent<Climate>.Instance.Weather.StormChance;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.Weather.StormChance = Mathf.Clamp01(value);
			}
		}

		// Token: 0x17000573 RID: 1395
		// (get) Token: 0x06004118 RID: 16664 RVA: 0x0017EC55 File Offset: 0x0017CE55
		// (set) Token: 0x06004119 RID: 16665 RVA: 0x0017EC78 File Offset: 0x0017CE78
		[ReplicatedVar(Default = "0")]
		public static float rain_chance
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return 0f;
				}
				return SingletonComponent<Climate>.Instance.Weather.RainChance;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.Weather.RainChance = Mathf.Clamp01(value);
			}
		}

		// Token: 0x17000574 RID: 1396
		// (get) Token: 0x0600411A RID: 16666 RVA: 0x0017EC9C File Offset: 0x0017CE9C
		// (set) Token: 0x0600411B RID: 16667 RVA: 0x0017ECBF File Offset: 0x0017CEBF
		[ReplicatedVar(Default = "-1")]
		public static float rain
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Rain;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Rain = value;
			}
		}

		// Token: 0x17000575 RID: 1397
		// (get) Token: 0x0600411C RID: 16668 RVA: 0x0017ECDE File Offset: 0x0017CEDE
		// (set) Token: 0x0600411D RID: 16669 RVA: 0x0017ED01 File Offset: 0x0017CF01
		[ReplicatedVar(Default = "-1")]
		public static float wind
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Wind;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Wind = value;
			}
		}

		// Token: 0x17000576 RID: 1398
		// (get) Token: 0x0600411E RID: 16670 RVA: 0x0017ED20 File Offset: 0x0017CF20
		// (set) Token: 0x0600411F RID: 16671 RVA: 0x0017ED43 File Offset: 0x0017CF43
		[ReplicatedVar(Default = "-1")]
		public static float thunder
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Thunder;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Thunder = value;
			}
		}

		// Token: 0x17000577 RID: 1399
		// (get) Token: 0x06004120 RID: 16672 RVA: 0x0017ED62 File Offset: 0x0017CF62
		// (set) Token: 0x06004121 RID: 16673 RVA: 0x0017ED85 File Offset: 0x0017CF85
		[ReplicatedVar(Default = "-1")]
		public static float rainbow
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Rainbow;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Rainbow = value;
			}
		}

		// Token: 0x17000578 RID: 1400
		// (get) Token: 0x06004122 RID: 16674 RVA: 0x0017EDA4 File Offset: 0x0017CFA4
		// (set) Token: 0x06004123 RID: 16675 RVA: 0x0017EDCC File Offset: 0x0017CFCC
		[ReplicatedVar(Default = "-1")]
		public static float fog
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.Fogginess;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.Fogginess = value;
			}
		}

		// Token: 0x17000579 RID: 1401
		// (get) Token: 0x06004124 RID: 16676 RVA: 0x0017EDF0 File Offset: 0x0017CFF0
		// (set) Token: 0x06004125 RID: 16677 RVA: 0x0017EE18 File Offset: 0x0017D018
		[ReplicatedVar(Default = "-1")]
		public static float atmosphere_rayleigh
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.RayleighMultiplier;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.RayleighMultiplier = value;
			}
		}

		// Token: 0x1700057A RID: 1402
		// (get) Token: 0x06004126 RID: 16678 RVA: 0x0017EE3C File Offset: 0x0017D03C
		// (set) Token: 0x06004127 RID: 16679 RVA: 0x0017EE64 File Offset: 0x0017D064
		[ReplicatedVar(Default = "-1")]
		public static float atmosphere_mie
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.MieMultiplier;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.MieMultiplier = value;
			}
		}

		// Token: 0x1700057B RID: 1403
		// (get) Token: 0x06004128 RID: 16680 RVA: 0x0017EE88 File Offset: 0x0017D088
		// (set) Token: 0x06004129 RID: 16681 RVA: 0x0017EEB0 File Offset: 0x0017D0B0
		[ReplicatedVar(Default = "-1")]
		public static float atmosphere_brightness
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.Brightness;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.Brightness = value;
			}
		}

		// Token: 0x1700057C RID: 1404
		// (get) Token: 0x0600412A RID: 16682 RVA: 0x0017EED4 File Offset: 0x0017D0D4
		// (set) Token: 0x0600412B RID: 16683 RVA: 0x0017EEFC File Offset: 0x0017D0FC
		[ReplicatedVar(Default = "-1")]
		public static float atmosphere_contrast
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.Contrast;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.Contrast = value;
			}
		}

		// Token: 0x1700057D RID: 1405
		// (get) Token: 0x0600412C RID: 16684 RVA: 0x0017EF20 File Offset: 0x0017D120
		// (set) Token: 0x0600412D RID: 16685 RVA: 0x0017EF48 File Offset: 0x0017D148
		[ReplicatedVar(Default = "-1")]
		public static float atmosphere_directionality
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.Directionality;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Atmosphere.Directionality = value;
			}
		}

		// Token: 0x1700057E RID: 1406
		// (get) Token: 0x0600412E RID: 16686 RVA: 0x0017EF6C File Offset: 0x0017D16C
		// (set) Token: 0x0600412F RID: 16687 RVA: 0x0017EF94 File Offset: 0x0017D194
		[ReplicatedVar(Default = "-1")]
		public static float cloud_size
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Size;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Size = value;
			}
		}

		// Token: 0x1700057F RID: 1407
		// (get) Token: 0x06004130 RID: 16688 RVA: 0x0017EFB8 File Offset: 0x0017D1B8
		// (set) Token: 0x06004131 RID: 16689 RVA: 0x0017EFE0 File Offset: 0x0017D1E0
		[ReplicatedVar(Default = "-1")]
		public static float cloud_opacity
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Opacity;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Opacity = value;
			}
		}

		// Token: 0x17000580 RID: 1408
		// (get) Token: 0x06004132 RID: 16690 RVA: 0x0017F004 File Offset: 0x0017D204
		// (set) Token: 0x06004133 RID: 16691 RVA: 0x0017F02C File Offset: 0x0017D22C
		[ReplicatedVar(Default = "-1")]
		public static float cloud_coverage
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Coverage;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Coverage = value;
			}
		}

		// Token: 0x17000581 RID: 1409
		// (get) Token: 0x06004134 RID: 16692 RVA: 0x0017F050 File Offset: 0x0017D250
		// (set) Token: 0x06004135 RID: 16693 RVA: 0x0017F078 File Offset: 0x0017D278
		[ReplicatedVar(Default = "-1")]
		public static float cloud_sharpness
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Sharpness;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Sharpness = value;
			}
		}

		// Token: 0x17000582 RID: 1410
		// (get) Token: 0x06004136 RID: 16694 RVA: 0x0017F09C File Offset: 0x0017D29C
		// (set) Token: 0x06004137 RID: 16695 RVA: 0x0017F0C4 File Offset: 0x0017D2C4
		[ReplicatedVar(Default = "-1")]
		public static float cloud_coloring
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Coloring;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Coloring = value;
			}
		}

		// Token: 0x17000583 RID: 1411
		// (get) Token: 0x06004138 RID: 16696 RVA: 0x0017F0E8 File Offset: 0x0017D2E8
		// (set) Token: 0x06004139 RID: 16697 RVA: 0x0017F110 File Offset: 0x0017D310
		[ReplicatedVar(Default = "-1")]
		public static float cloud_attenuation
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Attenuation;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Attenuation = value;
			}
		}

		// Token: 0x17000584 RID: 1412
		// (get) Token: 0x0600413A RID: 16698 RVA: 0x0017F134 File Offset: 0x0017D334
		// (set) Token: 0x0600413B RID: 16699 RVA: 0x0017F15C File Offset: 0x0017D35C
		[ReplicatedVar(Default = "-1")]
		public static float cloud_saturation
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Saturation;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Saturation = value;
			}
		}

		// Token: 0x17000585 RID: 1413
		// (get) Token: 0x0600413C RID: 16700 RVA: 0x0017F180 File Offset: 0x0017D380
		// (set) Token: 0x0600413D RID: 16701 RVA: 0x0017F1A8 File Offset: 0x0017D3A8
		[ReplicatedVar(Default = "-1")]
		public static float cloud_scattering
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Scattering;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Scattering = value;
			}
		}

		// Token: 0x17000586 RID: 1414
		// (get) Token: 0x0600413E RID: 16702 RVA: 0x0017F1CC File Offset: 0x0017D3CC
		// (set) Token: 0x0600413F RID: 16703 RVA: 0x0017F1F4 File Offset: 0x0017D3F4
		[ReplicatedVar(Default = "-1")]
		public static float cloud_brightness
		{
			get
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return -1f;
				}
				return SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Brightness;
			}
			set
			{
				if (!SingletonComponent<Climate>.Instance)
				{
					return;
				}
				SingletonComponent<Climate>.Instance.WeatherOverrides.Clouds.Brightness = value;
			}
		}

		// Token: 0x04003A31 RID: 14897
		[ServerVar]
		public static float wetness_rain = 0.4f;

		// Token: 0x04003A32 RID: 14898
		[ServerVar]
		public static float wetness_snow = 0.2f;
	}
}
