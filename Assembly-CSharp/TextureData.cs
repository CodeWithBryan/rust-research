using System;
using UnityEngine;

// Token: 0x0200062B RID: 1579
public struct TextureData
{
	// Token: 0x06002D77 RID: 11639 RVA: 0x001125D4 File Offset: 0x001107D4
	public TextureData(Texture2D tex)
	{
		if (tex != null)
		{
			this.width = tex.width;
			this.height = tex.height;
			this.colors = tex.GetPixels32();
			return;
		}
		this.width = 0;
		this.height = 0;
		this.colors = null;
	}

	// Token: 0x06002D78 RID: 11640 RVA: 0x00112624 File Offset: 0x00110824
	public Color32 GetColor(int x, int y)
	{
		return this.colors[y * this.width + x];
	}

	// Token: 0x06002D79 RID: 11641 RVA: 0x0011263B File Offset: 0x0011083B
	public int GetShort(int x, int y)
	{
		return (int)BitUtility.DecodeShort(this.GetColor(x, y));
	}

	// Token: 0x06002D7A RID: 11642 RVA: 0x0011264A File Offset: 0x0011084A
	public int GetInt(int x, int y)
	{
		return BitUtility.DecodeInt(this.GetColor(x, y));
	}

	// Token: 0x06002D7B RID: 11643 RVA: 0x00112659 File Offset: 0x00110859
	public float GetFloat(int x, int y)
	{
		return BitUtility.DecodeFloat(this.GetColor(x, y));
	}

	// Token: 0x06002D7C RID: 11644 RVA: 0x00112668 File Offset: 0x00110868
	public float GetHalf(int x, int y)
	{
		return BitUtility.Short2Float(this.GetShort(x, y));
	}

	// Token: 0x06002D7D RID: 11645 RVA: 0x00112677 File Offset: 0x00110877
	public Vector4 GetVector(int x, int y)
	{
		return BitUtility.DecodeVector(this.GetColor(x, y));
	}

	// Token: 0x06002D7E RID: 11646 RVA: 0x00112686 File Offset: 0x00110886
	public Vector3 GetNormal(int x, int y)
	{
		return BitUtility.DecodeNormal(this.GetColor(x, y));
	}

	// Token: 0x06002D7F RID: 11647 RVA: 0x0011269C File Offset: 0x0011089C
	public Color32 GetInterpolatedColor(float x, float y)
	{
		float num = x * (float)(this.width - 1);
		float num2 = y * (float)(this.height - 1);
		int num3 = Mathf.Clamp((int)num, 1, this.width - 2);
		int num4 = Mathf.Clamp((int)num2, 1, this.height - 2);
		int x2 = Mathf.Min(num3 + 1, this.width - 2);
		int y2 = Mathf.Min(num4 + 1, this.height - 2);
		Color a = this.GetColor(num3, num4);
		Color b = this.GetColor(x2, num4);
		Color a2 = this.GetColor(num3, y2);
		Color b2 = this.GetColor(x2, y2);
		float t = num - (float)num3;
		float t2 = num2 - (float)num4;
		Color a3 = Color.Lerp(a, b, t);
		Color b3 = Color.Lerp(a2, b2, t);
		return Color.Lerp(a3, b3, t2);
	}

	// Token: 0x06002D80 RID: 11648 RVA: 0x00112778 File Offset: 0x00110978
	public int GetInterpolatedInt(float x, float y)
	{
		float f = x * (float)(this.width - 1);
		float f2 = y * (float)(this.height - 1);
		int x2 = Mathf.Clamp(Mathf.RoundToInt(f), 1, this.width - 2);
		int y2 = Mathf.Clamp(Mathf.RoundToInt(f2), 1, this.height - 2);
		return this.GetInt(x2, y2);
	}

	// Token: 0x06002D81 RID: 11649 RVA: 0x001127D0 File Offset: 0x001109D0
	public int GetInterpolatedShort(float x, float y)
	{
		float f = x * (float)(this.width - 1);
		float f2 = y * (float)(this.height - 1);
		int x2 = Mathf.Clamp(Mathf.RoundToInt(f), 1, this.width - 2);
		int y2 = Mathf.Clamp(Mathf.RoundToInt(f2), 1, this.height - 2);
		return this.GetShort(x2, y2);
	}

	// Token: 0x06002D82 RID: 11650 RVA: 0x00112828 File Offset: 0x00110A28
	public float GetInterpolatedFloat(float x, float y)
	{
		float num = x * (float)(this.width - 1);
		float num2 = y * (float)(this.height - 1);
		int num3 = Mathf.Clamp((int)num, 1, this.width - 2);
		int num4 = Mathf.Clamp((int)num2, 1, this.height - 2);
		int x2 = Mathf.Min(num3 + 1, this.width - 2);
		int y2 = Mathf.Min(num4 + 1, this.height - 2);
		float @float = this.GetFloat(num3, num4);
		float float2 = this.GetFloat(x2, num4);
		float float3 = this.GetFloat(num3, y2);
		float float4 = this.GetFloat(x2, y2);
		float t = num - (float)num3;
		float t2 = num2 - (float)num4;
		float a = Mathf.Lerp(@float, float2, t);
		float b = Mathf.Lerp(float3, float4, t);
		return Mathf.Lerp(a, b, t2);
	}

	// Token: 0x06002D83 RID: 11651 RVA: 0x001128E8 File Offset: 0x00110AE8
	public float GetInterpolatedHalf(float x, float y)
	{
		float num = x * (float)(this.width - 1);
		float num2 = y * (float)(this.height - 1);
		int num3 = Mathf.Clamp((int)num, 1, this.width - 2);
		int num4 = Mathf.Clamp((int)num2, 1, this.height - 2);
		int x2 = Mathf.Min(num3 + 1, this.width - 2);
		int y2 = Mathf.Min(num4 + 1, this.height - 2);
		float half = this.GetHalf(num3, num4);
		float half2 = this.GetHalf(x2, num4);
		float half3 = this.GetHalf(num3, y2);
		float half4 = this.GetHalf(x2, y2);
		float t = num - (float)num3;
		float t2 = num2 - (float)num4;
		float a = Mathf.Lerp(half, half2, t);
		float b = Mathf.Lerp(half3, half4, t);
		return Mathf.Lerp(a, b, t2);
	}

	// Token: 0x06002D84 RID: 11652 RVA: 0x001129A8 File Offset: 0x00110BA8
	public Vector4 GetInterpolatedVector(float x, float y)
	{
		float num = x * (float)(this.width - 1);
		float num2 = y * (float)(this.height - 1);
		int num3 = Mathf.Clamp((int)num, 1, this.width - 2);
		int num4 = Mathf.Clamp((int)num2, 1, this.height - 2);
		int x2 = Mathf.Min(num3 + 1, this.width - 2);
		int y2 = Mathf.Min(num4 + 1, this.height - 2);
		Vector4 vector = this.GetVector(num3, num4);
		Vector4 vector2 = this.GetVector(x2, num4);
		Vector4 vector3 = this.GetVector(num3, y2);
		Vector4 vector4 = this.GetVector(x2, y2);
		float t = num - (float)num3;
		float t2 = num2 - (float)num4;
		Vector4 a = Vector4.Lerp(vector, vector2, t);
		Vector4 b = Vector4.Lerp(vector3, vector4, t);
		return Vector4.Lerp(a, b, t2);
	}

	// Token: 0x06002D85 RID: 11653 RVA: 0x00112A68 File Offset: 0x00110C68
	public Vector3 GetInterpolatedNormal(float x, float y)
	{
		float num = x * (float)(this.width - 1);
		float num2 = y * (float)(this.height - 1);
		int num3 = Mathf.Clamp((int)num, 1, this.width - 2);
		int num4 = Mathf.Clamp((int)num2, 1, this.height - 2);
		int x2 = Mathf.Min(num3 + 1, this.width - 2);
		int y2 = Mathf.Min(num4 + 1, this.height - 2);
		Vector3 normal = this.GetNormal(num3, num4);
		Vector3 normal2 = this.GetNormal(x2, num4);
		Vector3 normal3 = this.GetNormal(num3, y2);
		Vector3 normal4 = this.GetNormal(x2, y2);
		float t = num - (float)num3;
		float t2 = num2 - (float)num4;
		Vector3 a = Vector3.Lerp(normal, normal2, t);
		Vector3 b = Vector3.Lerp(normal3, normal4, t);
		return Vector3.Lerp(a, b, t2);
	}

	// Token: 0x04002544 RID: 9540
	public int width;

	// Token: 0x04002545 RID: 9541
	public int height;

	// Token: 0x04002546 RID: 9542
	public Color32[] colors;
}
