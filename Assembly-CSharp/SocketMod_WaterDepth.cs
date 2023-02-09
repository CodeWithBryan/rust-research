using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000263 RID: 611
public class SocketMod_WaterDepth : SocketMod
{
	// Token: 0x06001B9F RID: 7071 RVA: 0x000C04C0 File Offset: 0x000BE6C0
	public override bool DoCheck(Construction.Placement place)
	{
		Vector3 vector = place.position + place.rotation * this.worldPosition;
		if (!this.AllowWaterVolumes)
		{
			List<WaterVolume> list = Pool.GetList<WaterVolume>();
			Vis.Components<WaterVolume>(vector, 0.5f, list, 262144, QueryTriggerInteraction.Collide);
			int count = list.Count;
			Pool.FreeList<WaterVolume>(ref list);
			if (count > 0)
			{
				Construction.lastPlacementError = "Failed Check: WaterDepth_VolumeCheck (" + this.hierachyName + ")";
				return false;
			}
		}
		vector.y = WaterSystem.GetHeight(vector) - 0.1f;
		float overallWaterDepth = WaterLevel.GetOverallWaterDepth(vector, false, null, false);
		if (overallWaterDepth > this.MinimumWaterDepth && overallWaterDepth < this.MaximumWaterDepth)
		{
			return true;
		}
		if (overallWaterDepth <= this.MinimumWaterDepth)
		{
			Construction.lastPlacementError = SocketMod_WaterDepth.TooShallowPhrase.translated;
		}
		else
		{
			Construction.lastPlacementError = SocketMod_WaterDepth.TooDeepPhrase.translated;
		}
		return false;
	}

	// Token: 0x040014A8 RID: 5288
	public float MinimumWaterDepth = 2f;

	// Token: 0x040014A9 RID: 5289
	public float MaximumWaterDepth = 4f;

	// Token: 0x040014AA RID: 5290
	public bool AllowWaterVolumes;

	// Token: 0x040014AB RID: 5291
	public static Translate.Phrase TooDeepPhrase = new Translate.Phrase("error_toodeep", "Water is too deep");

	// Token: 0x040014AC RID: 5292
	public static Translate.Phrase TooShallowPhrase = new Translate.Phrase("error_shallow", "Water is too shallow");
}
