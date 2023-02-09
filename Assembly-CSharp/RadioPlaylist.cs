using System;
using UnityEngine;

// Token: 0x02000384 RID: 900
[CreateAssetMenu]
public class RadioPlaylist : ScriptableObject
{
	// Token: 0x040018DB RID: 6363
	public string Url;

	// Token: 0x040018DC RID: 6364
	public AudioClip[] Playlist;

	// Token: 0x040018DD RID: 6365
	public float PlaylistLength;
}
