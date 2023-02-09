using System;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000340 RID: 832
public static class AIDesigns
{
	// Token: 0x06001E41 RID: 7745 RVA: 0x000CD148 File Offset: 0x000CB348
	public static ProtoBuf.AIDesign GetByNameOrInstance(string designName, ProtoBuf.AIDesign entityDesign)
	{
		if (entityDesign != null)
		{
			return entityDesign;
		}
		ProtoBuf.AIDesign byName = AIDesigns.GetByName(designName + "_custom");
		if (byName != null)
		{
			return byName;
		}
		return AIDesigns.GetByName(designName);
	}

	// Token: 0x06001E42 RID: 7746 RVA: 0x000CD176 File Offset: 0x000CB376
	public static void RefreshCache(string designName, ProtoBuf.AIDesign design)
	{
		if (!AIDesigns.designs.ContainsKey(designName))
		{
			return;
		}
		AIDesigns.designs[designName] = design;
	}

	// Token: 0x06001E43 RID: 7747 RVA: 0x000CD194 File Offset: 0x000CB394
	private static ProtoBuf.AIDesign GetByName(string designName)
	{
		ProtoBuf.AIDesign aidesign;
		AIDesigns.designs.TryGetValue(designName, out aidesign);
		if (aidesign != null)
		{
			return aidesign;
		}
		string text = "cfg/ai/" + designName;
		if (!File.Exists(text))
		{
			return null;
		}
		try
		{
			using (FileStream fileStream = File.Open(text, FileMode.Open))
			{
				aidesign = ProtoBuf.AIDesign.Deserialize(fileStream);
				if (aidesign == null)
				{
					return null;
				}
				AIDesigns.designs.Add(designName, aidesign);
			}
		}
		catch (Exception)
		{
			Debug.LogWarning("Error trying to find AI design by name: " + text);
			return null;
		}
		return aidesign;
	}

	// Token: 0x040017FF RID: 6143
	public const string DesignFolderPath = "cfg/ai/";

	// Token: 0x04001800 RID: 6144
	private static Dictionary<string, ProtoBuf.AIDesign> designs = new Dictionary<string, ProtoBuf.AIDesign>();
}
