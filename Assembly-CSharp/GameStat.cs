using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007B7 RID: 1975
public class GameStat : MonoBehaviour
{
	// Token: 0x04002BC1 RID: 11201
	public float refreshTime = 5f;

	// Token: 0x04002BC2 RID: 11202
	public Text title;

	// Token: 0x04002BC3 RID: 11203
	public Text globalStat;

	// Token: 0x04002BC4 RID: 11204
	public Text localStat;

	// Token: 0x04002BC5 RID: 11205
	private long globalValue;

	// Token: 0x04002BC6 RID: 11206
	private long localValue;

	// Token: 0x04002BC7 RID: 11207
	private float secondsSinceRefresh;

	// Token: 0x04002BC8 RID: 11208
	private float secondsUntilUpdate;

	// Token: 0x04002BC9 RID: 11209
	private float secondsUntilChange;

	// Token: 0x04002BCA RID: 11210
	public GameStat.Stat[] stats;

	// Token: 0x02000E1F RID: 3615
	[Serializable]
	public struct Stat
	{
		// Token: 0x0400494D RID: 18765
		public string statName;

		// Token: 0x0400494E RID: 18766
		public string statTitle;
	}
}
