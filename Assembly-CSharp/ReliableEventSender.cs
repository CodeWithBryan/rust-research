using System;
using UnityEngine;

// Token: 0x02000274 RID: 628
public class ReliableEventSender : StateMachineBehaviour
{
	// Token: 0x040014FA RID: 5370
	[Header("State Enter")]
	public string StateEnter;

	// Token: 0x040014FB RID: 5371
	[Header("Mid State")]
	public string MidStateEvent;

	// Token: 0x040014FC RID: 5372
	[Range(0f, 1f)]
	public float TargetEventTime;
}
