using System;
using ProtoBuf;
using UnityEngine;

// Token: 0x020001A4 RID: 420
public class MapMarkerCH47 : MapMarker
{
	// Token: 0x060017C8 RID: 6088 RVA: 0x000B0F24 File Offset: 0x000AF124
	private float GetRotation()
	{
		global::BaseEntity parentEntity = base.GetParentEntity();
		if (!parentEntity)
		{
			return 0f;
		}
		Vector3 forward = parentEntity.transform.forward;
		forward.y = 0f;
		forward.Normalize();
		return Mathf.Atan2(forward.x, -forward.z) * 57.29578f + 180f;
	}

	// Token: 0x060017C9 RID: 6089 RVA: 0x000B0F83 File Offset: 0x000AF183
	public override AppMarker GetAppMarkerData()
	{
		AppMarker appMarkerData = base.GetAppMarkerData();
		appMarkerData.rotation = this.GetRotation();
		return appMarkerData;
	}

	// Token: 0x040010CF RID: 4303
	private GameObject createdMarker;
}
