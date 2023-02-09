using System;
using System.Threading;
using UnityEngine;

// Token: 0x020002EB RID: 747
public class InvokeSpammer : MonoBehaviour
{
	// Token: 0x06001D56 RID: 7510 RVA: 0x000C8D8D File Offset: 0x000C6F8D
	private void Start()
	{
		SingletonComponent<InvokeHandler>.Instance.InvokeRepeating(new Action(this.TestInvoke), this.RepeatTime, this.RepeatTime);
	}

	// Token: 0x06001D57 RID: 7511 RVA: 0x000C8DB1 File Offset: 0x000C6FB1
	private void TestInvoke()
	{
		Thread.Sleep(this.InvokeMilliseconds);
	}

	// Token: 0x040016BC RID: 5820
	public int InvokeMilliseconds = 1;

	// Token: 0x040016BD RID: 5821
	public float RepeatTime = 0.6f;
}
