using System;
using UnityEngine;

// Token: 0x020002AB RID: 683
public class LightGroupAtTime : FacepunchBehaviour
{
	// Token: 0x04001592 RID: 5522
	public float IntensityOverride = 1f;

	// Token: 0x04001593 RID: 5523
	public AnimationCurve IntensityScaleOverTime = new AnimationCurve
	{
		keys = new Keyframe[]
		{
			new Keyframe(0f, 1f),
			new Keyframe(8f, 0f),
			new Keyframe(12f, 0f),
			new Keyframe(19f, 1f),
			new Keyframe(24f, 1f)
		}
	};

	// Token: 0x04001594 RID: 5524
	public Transform SearchRoot;

	// Token: 0x04001595 RID: 5525
	[Header("Power Settings")]
	public bool requiresPower;

	// Token: 0x04001596 RID: 5526
	[Tooltip("Can NOT be entity, use new blank gameobject!")]
	public Transform powerOverrideTransform;

	// Token: 0x04001597 RID: 5527
	public LayerMask checkLayers = 1235288065;

	// Token: 0x04001598 RID: 5528
	public GameObject enableWhenLightsOn;

	// Token: 0x04001599 RID: 5529
	public float timeBetweenPowerLookup = 10f;
}
