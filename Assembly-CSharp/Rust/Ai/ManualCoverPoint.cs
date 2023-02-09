using System;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x02000AF6 RID: 2806
	public class ManualCoverPoint : FacepunchBehaviour
	{
		// Token: 0x170005E4 RID: 1508
		// (get) Token: 0x06004374 RID: 17268 RVA: 0x000299AB File Offset: 0x00027BAB
		public Vector3 Position
		{
			get
			{
				return base.transform.position;
			}
		}

		// Token: 0x170005E5 RID: 1509
		// (get) Token: 0x06004375 RID: 17269 RVA: 0x00187695 File Offset: 0x00185895
		public float DirectionMagnitude
		{
			get
			{
				if (this.Volume != null)
				{
					return this.Volume.CoverPointRayLength;
				}
				return 1f;
			}
		}

		// Token: 0x06004376 RID: 17270 RVA: 0x001876B6 File Offset: 0x001858B6
		private void Awake()
		{
			if (base.transform.parent != null)
			{
				this.Volume = base.transform.parent.GetComponent<CoverPointVolume>();
			}
		}

		// Token: 0x06004377 RID: 17271 RVA: 0x001876E4 File Offset: 0x001858E4
		public CoverPoint ToCoverPoint(CoverPointVolume volume)
		{
			this.Volume = volume;
			if (this.IsDynamic)
			{
				CoverPoint coverPoint = new CoverPoint(this.Volume, this.Score);
				coverPoint.IsDynamic = true;
				coverPoint.SourceTransform = base.transform;
				coverPoint.NormalCoverType = this.NormalCoverType;
				Transform transform = base.transform;
				coverPoint.Position = ((transform != null) ? transform.position : Vector3.zero);
				return coverPoint;
			}
			Vector3 normalized = (base.transform.rotation * this.Normal).normalized;
			return new CoverPoint(this.Volume, this.Score)
			{
				IsDynamic = false,
				Position = base.transform.position,
				Normal = normalized,
				NormalCoverType = this.NormalCoverType
			};
		}

		// Token: 0x04003BF3 RID: 15347
		public bool IsDynamic;

		// Token: 0x04003BF4 RID: 15348
		public float Score = 2f;

		// Token: 0x04003BF5 RID: 15349
		public CoverPointVolume Volume;

		// Token: 0x04003BF6 RID: 15350
		public Vector3 Normal;

		// Token: 0x04003BF7 RID: 15351
		public CoverPoint.CoverType NormalCoverType;
	}
}
