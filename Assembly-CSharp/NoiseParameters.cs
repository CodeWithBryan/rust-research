using System;

// Token: 0x02000626 RID: 1574
[Serializable]
public struct NoiseParameters
{
	// Token: 0x06002D41 RID: 11585 RVA: 0x0010FB1A File Offset: 0x0010DD1A
	public NoiseParameters(int octaves, float frequency, float amplitude, float offset)
	{
		this.Octaves = octaves;
		this.Frequency = frequency;
		this.Amplitude = amplitude;
		this.Offset = offset;
	}

	// Token: 0x04002506 RID: 9478
	public int Octaves;

	// Token: 0x04002507 RID: 9479
	public float Frequency;

	// Token: 0x04002508 RID: 9480
	public float Amplitude;

	// Token: 0x04002509 RID: 9481
	public float Offset;
}
