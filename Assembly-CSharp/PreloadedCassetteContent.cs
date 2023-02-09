using System;
using UnityEngine;

// Token: 0x02000383 RID: 899
public class PreloadedCassetteContent : ScriptableObject
{
	// Token: 0x06001F8B RID: 8075 RVA: 0x000D0510 File Offset: 0x000CE710
	public SoundDefinition GetSoundContent(int index, PreloadedCassetteContent.PreloadType type)
	{
		switch (type)
		{
		case PreloadedCassetteContent.PreloadType.Short:
			return this.GetDefinition(index, this.ShortTapeContent);
		case PreloadedCassetteContent.PreloadType.Medium:
			return this.GetDefinition(index, this.MediumTapeContent);
		case PreloadedCassetteContent.PreloadType.Long:
			return this.GetDefinition(index, this.LongTapeContent);
		default:
			return null;
		}
	}

	// Token: 0x06001F8C RID: 8076 RVA: 0x000D055C File Offset: 0x000CE75C
	private SoundDefinition GetDefinition(int index, SoundDefinition[] array)
	{
		index = Mathf.Clamp(index, 0, array.Length);
		return array[index];
	}

	// Token: 0x06001F8D RID: 8077 RVA: 0x000D0570 File Offset: 0x000CE770
	public SoundDefinition GetSoundContent(uint id)
	{
		foreach (SoundDefinition soundDefinition in this.ShortTapeContent)
		{
			if (StringPool.Get(soundDefinition.name) == id)
			{
				return soundDefinition;
			}
		}
		foreach (SoundDefinition soundDefinition2 in this.MediumTapeContent)
		{
			if (StringPool.Get(soundDefinition2.name) == id)
			{
				return soundDefinition2;
			}
		}
		foreach (SoundDefinition soundDefinition3 in this.LongTapeContent)
		{
			if (StringPool.Get(soundDefinition3.name) == id)
			{
				return soundDefinition3;
			}
		}
		return null;
	}

	// Token: 0x040018D8 RID: 6360
	public SoundDefinition[] ShortTapeContent;

	// Token: 0x040018D9 RID: 6361
	public SoundDefinition[] MediumTapeContent;

	// Token: 0x040018DA RID: 6362
	public SoundDefinition[] LongTapeContent;

	// Token: 0x02000C65 RID: 3173
	public enum PreloadType
	{
		// Token: 0x0400422F RID: 16943
		Short,
		// Token: 0x04004230 RID: 16944
		Medium,
		// Token: 0x04004231 RID: 16945
		Long
	}
}
