using System;
using UnityEngine;

// Token: 0x02000588 RID: 1416
public class LegacyWeatherState
{
	// Token: 0x06002A76 RID: 10870 RVA: 0x00101473 File Offset: 0x000FF673
	public LegacyWeatherState(WeatherPreset preset)
	{
		this.preset = preset;
	}

	// Token: 0x17000342 RID: 834
	// (get) Token: 0x06002A77 RID: 10871 RVA: 0x00101482 File Offset: 0x000FF682
	// (set) Token: 0x06002A78 RID: 10872 RVA: 0x0010148F File Offset: 0x000FF68F
	public float Wind
	{
		get
		{
			return this.preset.Wind;
		}
		set
		{
			this.preset.Wind = value;
		}
	}

	// Token: 0x17000343 RID: 835
	// (get) Token: 0x06002A79 RID: 10873 RVA: 0x0010149D File Offset: 0x000FF69D
	// (set) Token: 0x06002A7A RID: 10874 RVA: 0x001014AA File Offset: 0x000FF6AA
	public float Rain
	{
		get
		{
			return this.preset.Rain;
		}
		set
		{
			this.preset.Rain = value;
		}
	}

	// Token: 0x17000344 RID: 836
	// (get) Token: 0x06002A7B RID: 10875 RVA: 0x001014B8 File Offset: 0x000FF6B8
	// (set) Token: 0x06002A7C RID: 10876 RVA: 0x001014CA File Offset: 0x000FF6CA
	public float Clouds
	{
		get
		{
			return this.preset.Clouds.Coverage;
		}
		set
		{
			this.preset.Clouds.Opacity = Mathf.Sign(value);
			this.preset.Clouds.Coverage = value;
		}
	}

	// Token: 0x17000345 RID: 837
	// (get) Token: 0x06002A7D RID: 10877 RVA: 0x001014F3 File Offset: 0x000FF6F3
	// (set) Token: 0x06002A7E RID: 10878 RVA: 0x00101505 File Offset: 0x000FF705
	public float Fog
	{
		get
		{
			return this.preset.Atmosphere.Fogginess;
		}
		set
		{
			this.preset.Atmosphere.Fogginess = value;
		}
	}

	// Token: 0x04002256 RID: 8790
	private WeatherPreset preset;
}
