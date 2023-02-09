using System;
using UnityEngine;

// Token: 0x020002B9 RID: 697
public class PlayAudioEx : MonoBehaviour
{
	// Token: 0x06001C75 RID: 7285 RVA: 0x000059DD File Offset: 0x00003BDD
	private void Start()
	{
	}

	// Token: 0x06001C76 RID: 7286 RVA: 0x000C3D74 File Offset: 0x000C1F74
	private void OnEnable()
	{
		AudioSource component = base.GetComponent<AudioSource>();
		if (component)
		{
			component.PlayDelayed(this.delay);
		}
	}

	// Token: 0x040015EB RID: 5611
	public float delay;
}
