using System;
using Facepunch.BurstCloth;
using UnityEngine;

// Token: 0x020000F3 RID: 243
public class BurstClothCollider : MonoBehaviour, IClientComponent
{
	// Token: 0x060014A7 RID: 5287 RVA: 0x000A36B4 File Offset: 0x000A18B4
	public CapsuleParams GetParams()
	{
		Vector3 position = base.transform.position;
		float d = this.Height / 2f;
		Vector3 a = base.transform.rotation * Vector3.up;
		Vector3 position2 = position + a * d;
		Vector3 position3 = position - a * d;
		return new CapsuleParams
		{
			Transform = base.transform,
			PointA = base.transform.InverseTransformPoint(position2),
			PointB = base.transform.InverseTransformPoint(position3),
			Radius = this.Radius
		};
	}

	// Token: 0x04000D3D RID: 3389
	public float Height;

	// Token: 0x04000D3E RID: 3390
	public float Radius;
}
