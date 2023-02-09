using System;
using UnityEngine;

// Token: 0x02000275 RID: 629
public class AnimationEventForward : MonoBehaviour
{
	// Token: 0x06001BD7 RID: 7127 RVA: 0x000C132D File Offset: 0x000BF52D
	public void Event(string type)
	{
		this.targetObject.SendMessage(type);
	}

	// Token: 0x040014FD RID: 5373
	public GameObject targetObject;
}
