using System;
using UnityEngine;

// Token: 0x020002C8 RID: 712
public class ScaleBySpeed : MonoBehaviour
{
	// Token: 0x06001CB3 RID: 7347 RVA: 0x000C4D67 File Offset: 0x000C2F67
	private void Start()
	{
		this.prevPosition = base.transform.position;
	}

	// Token: 0x06001CB4 RID: 7348 RVA: 0x000C4D7C File Offset: 0x000C2F7C
	private void Update()
	{
		Vector3 position = base.transform.position;
		float num = (position - this.prevPosition).sqrMagnitude;
		float num2 = this.minScale;
		bool enabled = WaterSystem.GetHeight(position) > position.y - this.submergedThickness;
		if (num > 0.0001f)
		{
			num = Mathf.Sqrt(num);
			float value = Mathf.Clamp(num, this.minSpeed, this.maxSpeed) / (this.maxSpeed - this.minSpeed);
			num2 = Mathf.Lerp(this.minScale, this.maxScale, Mathf.Clamp01(value));
			if (this.component != null && this.toggleComponent)
			{
				this.component.enabled = enabled;
			}
		}
		else if (this.component != null && this.toggleComponent)
		{
			this.component.enabled = false;
		}
		base.transform.localScale = new Vector3(num2, num2, num2);
		this.prevPosition = position;
	}

	// Token: 0x04001652 RID: 5714
	public float minScale = 0.001f;

	// Token: 0x04001653 RID: 5715
	public float maxScale = 1f;

	// Token: 0x04001654 RID: 5716
	public float minSpeed;

	// Token: 0x04001655 RID: 5717
	public float maxSpeed = 1f;

	// Token: 0x04001656 RID: 5718
	public MonoBehaviour component;

	// Token: 0x04001657 RID: 5719
	public bool toggleComponent = true;

	// Token: 0x04001658 RID: 5720
	public bool onlyWhenSubmerged;

	// Token: 0x04001659 RID: 5721
	public float submergedThickness = 0.33f;

	// Token: 0x0400165A RID: 5722
	private Vector3 prevPosition = Vector3.zero;
}
