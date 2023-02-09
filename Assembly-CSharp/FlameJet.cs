using System;
using UnityEngine;

// Token: 0x020001BC RID: 444
public class FlameJet : MonoBehaviour
{
	// Token: 0x060017F2 RID: 6130 RVA: 0x000B1600 File Offset: 0x000AF800
	private void Initialize()
	{
		this.currentColor = this.startColor;
		this.tesselation = 0.1f;
		this.numSegments = Mathf.CeilToInt(this.maxLength / this.tesselation);
		this.spacing = this.maxLength / (float)this.numSegments;
		if (this.currentSegments.Length != this.numSegments)
		{
			this.currentSegments = new Vector3[this.numSegments];
		}
	}

	// Token: 0x060017F3 RID: 6131 RVA: 0x000B1671 File Offset: 0x000AF871
	private void Awake()
	{
		this.Initialize();
	}

	// Token: 0x060017F4 RID: 6132 RVA: 0x000B1679 File Offset: 0x000AF879
	public void LateUpdate()
	{
		if (this.on || this.currentColor.a > 0f)
		{
			this.UpdateLine();
		}
	}

	// Token: 0x060017F5 RID: 6133 RVA: 0x000B169B File Offset: 0x000AF89B
	public void SetOn(bool isOn)
	{
		this.on = isOn;
	}

	// Token: 0x060017F6 RID: 6134 RVA: 0x000B16A4 File Offset: 0x000AF8A4
	private float curve(float x)
	{
		return x * x;
	}

	// Token: 0x060017F7 RID: 6135 RVA: 0x000B16AC File Offset: 0x000AF8AC
	private void UpdateLine()
	{
		this.currentColor.a = Mathf.Lerp(this.currentColor.a, this.on ? 1f : 0f, Time.deltaTime * 40f);
		this.line.SetColors(this.currentColor, this.endColor);
		if (this.lastWorldSegments == null)
		{
			this.lastWorldSegments = new Vector3[this.numSegments];
		}
		int num = this.currentSegments.Length;
		for (int i = 0; i < num; i++)
		{
			float x = 0f;
			float y = 0f;
			if (this.lastWorldSegments != null && this.lastWorldSegments[i] != Vector3.zero && i > 0)
			{
				Vector3 a = base.transform.InverseTransformPoint(this.lastWorldSegments[i]);
				float f = (float)i / (float)this.currentSegments.Length;
				Vector3 vector = Vector3.Lerp(a, Vector3.zero, Time.deltaTime * this.drag);
				vector = Vector3.Lerp(Vector3.zero, vector, Mathf.Sqrt(f));
				x = vector.x;
				y = vector.y;
			}
			if (i == 0)
			{
				y = (x = 0f);
			}
			Vector3 vector2 = new Vector3(x, y, (float)i * this.spacing);
			this.currentSegments[i] = vector2;
			this.lastWorldSegments[i] = base.transform.TransformPoint(vector2);
		}
		this.line.positionCount = this.numSegments;
		this.line.SetPositions(this.currentSegments);
	}

	// Token: 0x04001134 RID: 4404
	public LineRenderer line;

	// Token: 0x04001135 RID: 4405
	public float tesselation = 0.025f;

	// Token: 0x04001136 RID: 4406
	private float length;

	// Token: 0x04001137 RID: 4407
	public float maxLength = 2f;

	// Token: 0x04001138 RID: 4408
	public float drag;

	// Token: 0x04001139 RID: 4409
	private int numSegments;

	// Token: 0x0400113A RID: 4410
	private float spacing;

	// Token: 0x0400113B RID: 4411
	public bool on;

	// Token: 0x0400113C RID: 4412
	private Vector3[] lastWorldSegments;

	// Token: 0x0400113D RID: 4413
	private Vector3[] currentSegments = new Vector3[0];

	// Token: 0x0400113E RID: 4414
	public Color startColor;

	// Token: 0x0400113F RID: 4415
	public Color endColor;

	// Token: 0x04001140 RID: 4416
	public Color currentColor;
}
