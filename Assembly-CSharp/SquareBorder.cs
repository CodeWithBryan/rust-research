using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000822 RID: 2082
public class SquareBorder : MonoBehaviour
{
	// Token: 0x06003477 RID: 13431 RVA: 0x0013DFE0 File Offset: 0x0013C1E0
	private void Update()
	{
		if (this._lastSize != this.Size)
		{
			this.Top.offsetMin = new Vector2(0f, -this.Size);
			this.Bottom.offsetMax = new Vector2(0f, this.Size);
			this.Left.offsetMin = new Vector2(0f, this.Size);
			this.Left.offsetMax = new Vector2(this.Size, -this.Size);
			this.Right.offsetMin = new Vector2(-this.Size, this.Size);
			this.Right.offsetMax = new Vector2(0f, -this.Size);
			this._lastSize = this.Size;
		}
		if (this._lastColor != this.Color)
		{
			this.TopImage.color = this.Color;
			this.BottomImage.color = this.Color;
			this.LeftImage.color = this.Color;
			this.RightImage.color = this.Color;
			this._lastColor = this.Color;
		}
	}

	// Token: 0x04002E32 RID: 11826
	public float Size;

	// Token: 0x04002E33 RID: 11827
	public Color Color;

	// Token: 0x04002E34 RID: 11828
	public RectTransform Top;

	// Token: 0x04002E35 RID: 11829
	public RectTransform Bottom;

	// Token: 0x04002E36 RID: 11830
	public RectTransform Left;

	// Token: 0x04002E37 RID: 11831
	public RectTransform Right;

	// Token: 0x04002E38 RID: 11832
	public Image TopImage;

	// Token: 0x04002E39 RID: 11833
	public Image BottomImage;

	// Token: 0x04002E3A RID: 11834
	public Image LeftImage;

	// Token: 0x04002E3B RID: 11835
	public Image RightImage;

	// Token: 0x04002E3C RID: 11836
	private float _lastSize;

	// Token: 0x04002E3D RID: 11837
	private Color _lastColor;
}
