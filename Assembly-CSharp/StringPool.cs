using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000738 RID: 1848
public class StringPool
{
	// Token: 0x06003305 RID: 13061 RVA: 0x0013AF38 File Offset: 0x00139138
	private static void Init()
	{
		if (StringPool.initialized)
		{
			return;
		}
		StringPool.toString = new Dictionary<uint, string>();
		StringPool.toNumber = new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase);
		GameManifest gameManifest = FileSystem.Load<GameManifest>("Assets/manifest.asset", true);
		uint num = 0U;
		while ((ulong)num < (ulong)((long)gameManifest.pooledStrings.Length))
		{
			string str = gameManifest.pooledStrings[(int)num].str;
			uint hash = gameManifest.pooledStrings[(int)num].hash;
			string text;
			if (StringPool.toString.TryGetValue(hash, out text))
			{
				if (str != text)
				{
					Debug.LogWarning(string.Format("Hash collision: {0} already exists in string pool. `{1}` and `{2}` have the same hash.", hash, str, text));
				}
			}
			else
			{
				StringPool.toString.Add(hash, str);
				StringPool.toNumber.Add(str, hash);
			}
			num += 1U;
		}
		StringPool.initialized = true;
		StringPool.closest = StringPool.Get("closest");
	}

	// Token: 0x06003306 RID: 13062 RVA: 0x0013B00C File Offset: 0x0013920C
	public static string Get(uint i)
	{
		if (i == 0U)
		{
			return string.Empty;
		}
		StringPool.Init();
		string result;
		if (StringPool.toString.TryGetValue(i, out result))
		{
			return result;
		}
		Debug.LogWarning("StringPool.GetString - no string for ID" + i);
		return "";
	}

	// Token: 0x06003307 RID: 13063 RVA: 0x0013B054 File Offset: 0x00139254
	public static uint Get(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return 0U;
		}
		StringPool.Init();
		uint result;
		if (StringPool.toNumber.TryGetValue(str, out result))
		{
			return result;
		}
		Debug.LogWarning("StringPool.GetNumber - no number for string " + str);
		return 0U;
	}

	// Token: 0x06003308 RID: 13064 RVA: 0x0013B094 File Offset: 0x00139294
	public static uint Add(string str)
	{
		uint num = 0U;
		if (!StringPool.toNumber.TryGetValue(str, out num))
		{
			num = str.ManifestHash();
			StringPool.toString.Add(num, str);
			StringPool.toNumber.Add(str, num);
		}
		return num;
	}

	// Token: 0x04002985 RID: 10629
	private static Dictionary<uint, string> toString;

	// Token: 0x04002986 RID: 10630
	private static Dictionary<string, uint> toNumber;

	// Token: 0x04002987 RID: 10631
	private static bool initialized;

	// Token: 0x04002988 RID: 10632
	public static uint closest;
}
