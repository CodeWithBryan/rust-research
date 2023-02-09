using System;
using UnityEngine;

// Token: 0x020007B9 RID: 1977
public abstract class Graph : MonoBehaviour
{
	// Token: 0x060033CB RID: 13259
	protected abstract float GetValue();

	// Token: 0x060033CC RID: 13260
	protected abstract Color GetColor(float value);

	// Token: 0x060033CD RID: 13261 RVA: 0x0013BF98 File Offset: 0x0013A198
	protected Vector3 GetVertex(float x, float y)
	{
		return new Vector3(x, y, 0f);
	}

	// Token: 0x060033CE RID: 13262 RVA: 0x0013BFA8 File Offset: 0x0013A1A8
	protected void Update()
	{
		if (this.values == null || this.values.Length != this.Resolution)
		{
			this.values = new float[this.Resolution];
		}
		this.max = 0f;
		for (int i = 0; i < this.values.Length - 1; i++)
		{
			this.max = Mathf.Max(this.max, this.values[i] = this.values[i + 1]);
		}
		this.max = Mathf.Max(this.max, this.CurrentValue = (this.values[this.values.Length - 1] = this.GetValue()));
	}

	// Token: 0x060033CF RID: 13263 RVA: 0x0013C058 File Offset: 0x0013A258
	protected void OnGUI()
	{
		if (Event.current.type != EventType.Repaint)
		{
			return;
		}
		if (this.values == null || this.values.Length == 0)
		{
			return;
		}
		float num = Mathf.Max(this.Area.width, this.ScreenFill.x * (float)Screen.width);
		float num2 = Mathf.Max(this.Area.height, this.ScreenFill.y * (float)Screen.height);
		float num3 = this.Area.x - this.Pivot.x * num + this.ScreenOrigin.x * (float)Screen.width;
		float num4 = this.Area.y - this.Pivot.y * num2 + this.ScreenOrigin.y * (float)Screen.height;
		GL.PushMatrix();
		this.Material.SetPass(0);
		GL.LoadPixelMatrix();
		GL.Begin(7);
		for (int i = 0; i < this.values.Length; i++)
		{
			float num5 = this.values[i];
			float num6 = num / (float)this.values.Length;
			float num7 = num2 * num5 / this.max;
			float num8 = num3 + (float)i * num6;
			float num9 = num4;
			GL.Color(this.GetColor(num5));
			GL.Vertex(this.GetVertex(num8 + 0f, num9 + num7));
			GL.Vertex(this.GetVertex(num8 + num6, num9 + num7));
			GL.Vertex(this.GetVertex(num8 + num6, num9 + 0f));
			GL.Vertex(this.GetVertex(num8 + 0f, num9 + 0f));
		}
		GL.End();
		GL.PopMatrix();
	}

	// Token: 0x04002BCB RID: 11211
	public Material Material;

	// Token: 0x04002BCC RID: 11212
	public int Resolution = 128;

	// Token: 0x04002BCD RID: 11213
	public Vector2 ScreenFill = new Vector2(0f, 0f);

	// Token: 0x04002BCE RID: 11214
	public Vector2 ScreenOrigin = new Vector2(0f, 0f);

	// Token: 0x04002BCF RID: 11215
	public Vector2 Pivot = new Vector2(0f, 0f);

	// Token: 0x04002BD0 RID: 11216
	public Rect Area = new Rect(0f, 0f, 128f, 32f);

	// Token: 0x04002BD1 RID: 11217
	internal float CurrentValue;

	// Token: 0x04002BD2 RID: 11218
	private int index;

	// Token: 0x04002BD3 RID: 11219
	private float[] values;

	// Token: 0x04002BD4 RID: 11220
	private float max;
}
