using System;
using UnityEngine;

// Token: 0x020006CE RID: 1742
[ExecuteInEditMode]
public class WaterBody : MonoBehaviour
{
	// Token: 0x170003BA RID: 954
	// (get) Token: 0x060030B5 RID: 12469 RVA: 0x0012BEC8 File Offset: 0x0012A0C8
	// (set) Token: 0x060030B4 RID: 12468 RVA: 0x0012BEBF File Offset: 0x0012A0BF
	public Transform Transform { get; private set; }

	// Token: 0x060030B6 RID: 12470 RVA: 0x0012BED0 File Offset: 0x0012A0D0
	private void Awake()
	{
		this.Transform = base.transform;
	}

	// Token: 0x060030B7 RID: 12471 RVA: 0x0012BEDE File Offset: 0x0012A0DE
	private void OnEnable()
	{
		WaterSystem.RegisterBody(this);
	}

	// Token: 0x060030B8 RID: 12472 RVA: 0x0012BEE6 File Offset: 0x0012A0E6
	private void OnDisable()
	{
		WaterSystem.UnregisterBody(this);
	}

	// Token: 0x060030B9 RID: 12473 RVA: 0x0012BEF0 File Offset: 0x0012A0F0
	public void OnOceanLevelChanged(float newLevel)
	{
		if (!this.IsOcean)
		{
			return;
		}
		foreach (Collider collider in this.Triggers)
		{
			Vector3 position = collider.transform.position;
			position.y = newLevel;
			collider.transform.position = position;
		}
	}

	// Token: 0x04002794 RID: 10132
	public WaterBodyType Type = WaterBodyType.Lake;

	// Token: 0x04002795 RID: 10133
	public Renderer Renderer;

	// Token: 0x04002796 RID: 10134
	public Collider[] Triggers;

	// Token: 0x04002797 RID: 10135
	public bool IsOcean;

	// Token: 0x04002799 RID: 10137
	public WaterBody.FishingTag FishingType;

	// Token: 0x02000DD1 RID: 3537
	[Flags]
	public enum FishingTag
	{
		// Token: 0x040047DE RID: 18398
		MoonPool = 1,
		// Token: 0x040047DF RID: 18399
		River = 2,
		// Token: 0x040047E0 RID: 18400
		Ocean = 4,
		// Token: 0x040047E1 RID: 18401
		Swamp = 8
	}
}
