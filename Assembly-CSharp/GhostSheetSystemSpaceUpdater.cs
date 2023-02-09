using System;
using EZhex1991.EZSoftBone;
using UnityEngine;

// Token: 0x020000F5 RID: 245
public class GhostSheetSystemSpaceUpdater : MonoBehaviour, IClientComponent
{
	// Token: 0x060014AC RID: 5292 RVA: 0x000A378F File Offset: 0x000A198F
	public void Awake()
	{
		this.ezSoftBones = base.GetComponents<EZSoftBone>();
		this.player = (base.gameObject.ToBaseEntity() as BasePlayer);
	}

	// Token: 0x060014AD RID: 5293 RVA: 0x000A37B4 File Offset: 0x000A19B4
	public void Update()
	{
		if (this.ezSoftBones == null || this.ezSoftBones.Length == 0 || this.player == null)
		{
			return;
		}
		BaseMountable mounted = this.player.GetMounted();
		if (mounted != null)
		{
			this.SetSimulateSpace(mounted.transform, false);
			return;
		}
		BaseEntity parentEntity = this.player.GetParentEntity();
		if (parentEntity != null)
		{
			this.SetSimulateSpace(parentEntity.transform, true);
			return;
		}
		this.SetSimulateSpace(null, true);
	}

	// Token: 0x060014AE RID: 5294 RVA: 0x000A3830 File Offset: 0x000A1A30
	private void SetSimulateSpace(Transform transform, bool collisionEnabled)
	{
		for (int i = 0; i < this.ezSoftBones.Length; i++)
		{
			EZSoftBone ezsoftBone = this.ezSoftBones[i];
			ezsoftBone.simulateSpace = transform;
			ezsoftBone.collisionEnabled = collisionEnabled;
		}
	}

	// Token: 0x04000D48 RID: 3400
	private EZSoftBone[] ezSoftBones;

	// Token: 0x04000D49 RID: 3401
	private BasePlayer player;
}
