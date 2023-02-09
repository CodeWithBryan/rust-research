using System;
using System.Collections.Generic;
using Rust.Interpolation;
using UnityEngine;

// Token: 0x020002BF RID: 703
public interface IPosLerpTarget : ILerpInfo
{
	// Token: 0x06001C87 RID: 7303
	float GetInterpolationInertia();

	// Token: 0x06001C88 RID: 7304
	Vector3 GetNetworkPosition();

	// Token: 0x06001C89 RID: 7305
	Quaternion GetNetworkRotation();

	// Token: 0x06001C8A RID: 7306
	void SetNetworkPosition(Vector3 pos);

	// Token: 0x06001C8B RID: 7307
	void SetNetworkRotation(Quaternion rot);

	// Token: 0x06001C8C RID: 7308
	void DrawInterpolationState(Interpolator<TransformSnapshot>.Segment segment, List<TransformSnapshot> entries);

	// Token: 0x06001C8D RID: 7309
	void LerpIdleDisable();
}
