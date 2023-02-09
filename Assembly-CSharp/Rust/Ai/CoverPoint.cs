using System;
using System.Collections;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x02000AF3 RID: 2803
	public class CoverPoint
	{
		// Token: 0x170005DC RID: 1500
		// (get) Token: 0x06004355 RID: 17237 RVA: 0x00186E86 File Offset: 0x00185086
		// (set) Token: 0x06004356 RID: 17238 RVA: 0x00186E8E File Offset: 0x0018508E
		public CoverPointVolume Volume { get; private set; }

		// Token: 0x170005DD RID: 1501
		// (get) Token: 0x06004357 RID: 17239 RVA: 0x00186E97 File Offset: 0x00185097
		// (set) Token: 0x06004358 RID: 17240 RVA: 0x00186EC1 File Offset: 0x001850C1
		public Vector3 Position
		{
			get
			{
				if (this.IsDynamic && this.SourceTransform != null)
				{
					return this.SourceTransform.position;
				}
				return this._staticPosition;
			}
			set
			{
				this._staticPosition = value;
			}
		}

		// Token: 0x170005DE RID: 1502
		// (get) Token: 0x06004359 RID: 17241 RVA: 0x00186ECA File Offset: 0x001850CA
		// (set) Token: 0x0600435A RID: 17242 RVA: 0x00186EF4 File Offset: 0x001850F4
		public Vector3 Normal
		{
			get
			{
				if (this.IsDynamic && this.SourceTransform != null)
				{
					return this.SourceTransform.forward;
				}
				return this._staticNormal;
			}
			set
			{
				this._staticNormal = value;
			}
		}

		// Token: 0x170005DF RID: 1503
		// (get) Token: 0x0600435B RID: 17243 RVA: 0x00186EFD File Offset: 0x001850FD
		// (set) Token: 0x0600435C RID: 17244 RVA: 0x00186F05 File Offset: 0x00185105
		public BaseEntity ReservedFor { get; set; }

		// Token: 0x170005E0 RID: 1504
		// (get) Token: 0x0600435D RID: 17245 RVA: 0x00186F0E File Offset: 0x0018510E
		public bool IsReserved
		{
			get
			{
				return this.ReservedFor != null;
			}
		}

		// Token: 0x170005E1 RID: 1505
		// (get) Token: 0x0600435E RID: 17246 RVA: 0x00186F1C File Offset: 0x0018511C
		// (set) Token: 0x0600435F RID: 17247 RVA: 0x00186F24 File Offset: 0x00185124
		public bool IsCompromised { get; set; }

		// Token: 0x170005E2 RID: 1506
		// (get) Token: 0x06004360 RID: 17248 RVA: 0x00186F2D File Offset: 0x0018512D
		// (set) Token: 0x06004361 RID: 17249 RVA: 0x00186F35 File Offset: 0x00185135
		public float Score { get; set; }

		// Token: 0x06004362 RID: 17250 RVA: 0x00186F3E File Offset: 0x0018513E
		public bool IsValidFor(BaseEntity entity)
		{
			return !this.IsCompromised && (this.ReservedFor == null || this.ReservedFor == entity);
		}

		// Token: 0x06004363 RID: 17251 RVA: 0x00186F66 File Offset: 0x00185166
		public CoverPoint(CoverPointVolume volume, float score)
		{
			this.Volume = volume;
			this.Score = score;
		}

		// Token: 0x06004364 RID: 17252 RVA: 0x00186F7C File Offset: 0x0018517C
		public void CoverIsCompromised(float cooldown)
		{
			if (this.IsCompromised)
			{
				return;
			}
			if (this.Volume != null)
			{
				this.Volume.StartCoroutine(this.StartCooldown(cooldown));
			}
		}

		// Token: 0x06004365 RID: 17253 RVA: 0x00186FA8 File Offset: 0x001851A8
		private IEnumerator StartCooldown(float cooldown)
		{
			this.IsCompromised = true;
			yield return CoroutineEx.waitForSeconds(cooldown);
			this.IsCompromised = false;
			yield break;
		}

		// Token: 0x06004366 RID: 17254 RVA: 0x00186FC0 File Offset: 0x001851C0
		public bool ProvidesCoverFromPoint(Vector3 point, float arcThreshold)
		{
			Vector3 normalized = (this.Position - point).normalized;
			return Vector3.Dot(this.Normal, normalized) < arcThreshold;
		}

		// Token: 0x04003BDF RID: 15327
		public CoverPoint.CoverType NormalCoverType;

		// Token: 0x04003BE0 RID: 15328
		public bool IsDynamic;

		// Token: 0x04003BE1 RID: 15329
		public Transform SourceTransform;

		// Token: 0x04003BE2 RID: 15330
		private Vector3 _staticPosition;

		// Token: 0x04003BE3 RID: 15331
		private Vector3 _staticNormal;

		// Token: 0x02000F32 RID: 3890
		public enum CoverType
		{
			// Token: 0x04004DA0 RID: 19872
			Full,
			// Token: 0x04004DA1 RID: 19873
			Partial,
			// Token: 0x04004DA2 RID: 19874
			None
		}
	}
}
