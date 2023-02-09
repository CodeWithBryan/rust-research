using System;
using UnityEngine;

// Token: 0x0200013C RID: 316
public class ArcadeEntityController : BaseMonoBehaviour
{
	// Token: 0x170001B9 RID: 441
	// (get) Token: 0x0600160F RID: 5647 RVA: 0x000A8C02 File Offset: 0x000A6E02
	// (set) Token: 0x06001610 RID: 5648 RVA: 0x000A8C0F File Offset: 0x000A6E0F
	public Vector3 heading
	{
		get
		{
			return this.arcadeEntity.heading;
		}
		set
		{
			this.arcadeEntity.heading = value;
		}
	}

	// Token: 0x170001BA RID: 442
	// (get) Token: 0x06001611 RID: 5649 RVA: 0x000A8C1D File Offset: 0x000A6E1D
	// (set) Token: 0x06001612 RID: 5650 RVA: 0x000A8C2F File Offset: 0x000A6E2F
	public Vector3 positionLocal
	{
		get
		{
			return this.arcadeEntity.transform.localPosition;
		}
		set
		{
			this.arcadeEntity.transform.localPosition = value;
		}
	}

	// Token: 0x170001BB RID: 443
	// (get) Token: 0x06001613 RID: 5651 RVA: 0x000A8C42 File Offset: 0x000A6E42
	// (set) Token: 0x06001614 RID: 5652 RVA: 0x000A8C54 File Offset: 0x000A6E54
	public Vector3 positionWorld
	{
		get
		{
			return this.arcadeEntity.transform.position;
		}
		set
		{
			this.arcadeEntity.transform.position = value;
		}
	}

	// Token: 0x04000EBA RID: 3770
	public BaseArcadeGame parentGame;

	// Token: 0x04000EBB RID: 3771
	public ArcadeEntity arcadeEntity;

	// Token: 0x04000EBC RID: 3772
	public ArcadeEntity sourceEntity;
}
