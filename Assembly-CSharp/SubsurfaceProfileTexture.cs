using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006F3 RID: 1779
public class SubsurfaceProfileTexture
{
	// Token: 0x170003DC RID: 988
	// (get) Token: 0x06003185 RID: 12677 RVA: 0x00130558 File Offset: 0x0012E758
	public Texture2D Texture
	{
		get
		{
			if (!(this.texture == null))
			{
				return this.texture;
			}
			return this.CreateTexture();
		}
	}

	// Token: 0x06003186 RID: 12678 RVA: 0x00130575 File Offset: 0x0012E775
	public SubsurfaceProfileTexture()
	{
		this.AddProfile(SubsurfaceProfileData.Default, null);
	}

	// Token: 0x06003187 RID: 12679 RVA: 0x00130598 File Offset: 0x0012E798
	public int FindEntryIndex(SubsurfaceProfile profile)
	{
		for (int i = 0; i < this.entries.Count; i++)
		{
			if (this.entries[i].profile == profile)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06003188 RID: 12680 RVA: 0x001305D8 File Offset: 0x0012E7D8
	public int AddProfile(SubsurfaceProfileData data, SubsurfaceProfile profile)
	{
		int num = -1;
		for (int i = 0; i < this.entries.Count; i++)
		{
			if (this.entries[i].profile == profile)
			{
				num = i;
				this.entries[num] = new SubsurfaceProfileTexture.SubsurfaceProfileEntry(data, profile);
				break;
			}
		}
		if (num < 0)
		{
			num = this.entries.Count;
			this.entries.Add(new SubsurfaceProfileTexture.SubsurfaceProfileEntry(data, profile));
		}
		this.ReleaseTexture();
		return num;
	}

	// Token: 0x06003189 RID: 12681 RVA: 0x00130656 File Offset: 0x0012E856
	public void UpdateProfile(int id, SubsurfaceProfileData data)
	{
		if (id >= 0)
		{
			this.entries[id] = new SubsurfaceProfileTexture.SubsurfaceProfileEntry(data, this.entries[id].profile);
			this.ReleaseTexture();
		}
	}

	// Token: 0x0600318A RID: 12682 RVA: 0x00130685 File Offset: 0x0012E885
	public void RemoveProfile(int id)
	{
		if (id >= 0)
		{
			this.entries[id] = new SubsurfaceProfileTexture.SubsurfaceProfileEntry(SubsurfaceProfileData.Invalid, null);
			this.CheckReleaseTexture();
		}
	}

	// Token: 0x0600318B RID: 12683 RVA: 0x001306A8 File Offset: 0x0012E8A8
	public static Color ColorClamp(Color color, float min = 0f, float max = 1f)
	{
		Color result;
		result.r = Mathf.Clamp(color.r, min, max);
		result.g = Mathf.Clamp(color.g, min, max);
		result.b = Mathf.Clamp(color.b, min, max);
		result.a = Mathf.Clamp(color.a, min, max);
		return result;
	}

	// Token: 0x0600318C RID: 12684 RVA: 0x00130708 File Offset: 0x0012E908
	private Texture2D CreateTexture()
	{
		if (this.entries.Count > 0)
		{
			int num = 32;
			int num2 = Mathf.Max(this.entries.Count, 64);
			this.ReleaseTexture();
			this.texture = new Texture2D(num, num2, TextureFormat.RGBAHalf, false, true);
			this.texture.name = "SubsurfaceProfiles";
			this.texture.wrapMode = TextureWrapMode.Clamp;
			this.texture.filterMode = FilterMode.Bilinear;
			Color[] pixels = this.texture.GetPixels(0);
			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i] = Color.clear;
			}
			Color[] array = new Color[num];
			for (int j = 0; j < this.entries.Count; j++)
			{
				SubsurfaceProfileData data = this.entries[j].data;
				data.SubsurfaceColor = SubsurfaceProfileTexture.ColorClamp(data.SubsurfaceColor, 0f, 1f);
				data.FalloffColor = SubsurfaceProfileTexture.ColorClamp(data.FalloffColor, 0.009f, 1f);
				array[0] = data.SubsurfaceColor;
				array[0].a = 0f;
				SeparableSSS.CalculateKernel(array, 1, 13, data.SubsurfaceColor, data.FalloffColor);
				SeparableSSS.CalculateKernel(array, 14, 9, data.SubsurfaceColor, data.FalloffColor);
				SeparableSSS.CalculateKernel(array, 23, 6, data.SubsurfaceColor, data.FalloffColor);
				int num3 = num * (num2 - j - 1);
				for (int k = 0; k < 29; k++)
				{
					Color color = array[k] * new Color(1f, 1f, 1f, 0.33333334f);
					color.a *= data.ScatterRadius / 1024f;
					pixels[num3 + k] = color;
				}
			}
			this.texture.SetPixels(pixels, 0);
			this.texture.Apply(false, false);
			return this.texture;
		}
		return null;
	}

	// Token: 0x0600318D RID: 12685 RVA: 0x0013090C File Offset: 0x0012EB0C
	private void CheckReleaseTexture()
	{
		int num = 0;
		for (int i = 0; i < this.entries.Count; i++)
		{
			num += ((this.entries[i].profile == null) ? 1 : 0);
		}
		if (this.entries.Count == num)
		{
			this.ReleaseTexture();
		}
	}

	// Token: 0x0600318E RID: 12686 RVA: 0x00130965 File Offset: 0x0012EB65
	private void ReleaseTexture()
	{
		if (this.texture != null)
		{
			UnityEngine.Object.DestroyImmediate(this.texture);
			this.texture = null;
		}
	}

	// Token: 0x0400282D RID: 10285
	public const int SUBSURFACE_RADIUS_SCALE = 1024;

	// Token: 0x0400282E RID: 10286
	public const int SUBSURFACE_KERNEL_SIZE = 3;

	// Token: 0x0400282F RID: 10287
	private List<SubsurfaceProfileTexture.SubsurfaceProfileEntry> entries = new List<SubsurfaceProfileTexture.SubsurfaceProfileEntry>(16);

	// Token: 0x04002830 RID: 10288
	private Texture2D texture;

	// Token: 0x02000DE0 RID: 3552
	private struct SubsurfaceProfileEntry
	{
		// Token: 0x06004F81 RID: 20353 RVA: 0x0019F7D6 File Offset: 0x0019D9D6
		public SubsurfaceProfileEntry(SubsurfaceProfileData data, SubsurfaceProfile profile)
		{
			this.data = data;
			this.profile = profile;
		}

		// Token: 0x04004835 RID: 18485
		public SubsurfaceProfileData data;

		// Token: 0x04004836 RID: 18486
		public SubsurfaceProfile profile;
	}
}
