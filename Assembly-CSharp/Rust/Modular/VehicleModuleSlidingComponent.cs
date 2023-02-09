using System;
using UnityEngine;

namespace Rust.Modular
{
	// Token: 0x02000AE8 RID: 2792
	[Serializable]
	public class VehicleModuleSlidingComponent
	{
		// Token: 0x06004326 RID: 17190 RVA: 0x001863F1 File Offset: 0x001845F1
		public bool WantsOpenPos(BaseEntity parentEntity)
		{
			return parentEntity.HasFlag(this.flag_SliderOpen);
		}

		// Token: 0x06004327 RID: 17191 RVA: 0x001863FF File Offset: 0x001845FF
		public void Use(BaseVehicleModule parentModule)
		{
			parentModule.SetFlag(this.flag_SliderOpen, !this.WantsOpenPos(parentModule), false, true);
		}

		// Token: 0x06004328 RID: 17192 RVA: 0x00186419 File Offset: 0x00184619
		public void ServerUpdateTick(BaseVehicleModule parentModule)
		{
			this.CheckPosition(parentModule, Time.fixedDeltaTime);
		}

		// Token: 0x06004329 RID: 17193 RVA: 0x00186428 File Offset: 0x00184628
		private void CheckPosition(BaseEntity parentEntity, float dt)
		{
			bool flag = this.WantsOpenPos(parentEntity);
			if (flag && this.positionPercent == 1f)
			{
				return;
			}
			if (!flag && this.positionPercent == 0f)
			{
				return;
			}
			float num = flag ? (dt / this.moveTime) : (-(dt / this.moveTime));
			this.positionPercent = Mathf.Clamp01(this.positionPercent + num);
			foreach (VehicleModuleSlidingComponent.SlidingPart slidingPart in this.slidingParts)
			{
				if (!(slidingPart.transform == null))
				{
					slidingPart.transform.localPosition = Vector3.Lerp(slidingPart.closedPosition, slidingPart.openPosition, this.positionPercent);
				}
			}
		}

		// Token: 0x04003BB7 RID: 15287
		public string interactionColliderName = "MyCollider";

		// Token: 0x04003BB8 RID: 15288
		public BaseEntity.Flags flag_SliderOpen = BaseEntity.Flags.Reserved3;

		// Token: 0x04003BB9 RID: 15289
		public float moveTime = 1f;

		// Token: 0x04003BBA RID: 15290
		public VehicleModuleSlidingComponent.SlidingPart[] slidingParts;

		// Token: 0x04003BBB RID: 15291
		public SoundDefinition openSoundDef;

		// Token: 0x04003BBC RID: 15292
		public SoundDefinition closeSoundDef;

		// Token: 0x04003BBD RID: 15293
		private float positionPercent;

		// Token: 0x02000F30 RID: 3888
		[Serializable]
		public class SlidingPart
		{
			// Token: 0x04004D99 RID: 19865
			public Transform transform;

			// Token: 0x04004D9A RID: 19866
			public Vector3 openPosition;

			// Token: 0x04004D9B RID: 19867
			public Vector3 closedPosition;
		}
	}
}
