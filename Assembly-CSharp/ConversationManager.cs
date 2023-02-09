using System;
using UnityEngine;

// Token: 0x0200018F RID: 399
public class ConversationManager : MonoBehaviour
{
	// Token: 0x02000BE8 RID: 3048
	public class Conversation : MonoBehaviour
	{
		// Token: 0x06004B6F RID: 19311 RVA: 0x00191EB1 File Offset: 0x001900B1
		public int GetSpeechNodeIndex(string name)
		{
			if (this.data == null)
			{
				return -1;
			}
			return this.data.GetSpeechNodeIndex(name);
		}

		// Token: 0x04004027 RID: 16423
		public ConversationData data;

		// Token: 0x04004028 RID: 16424
		public int currentSpeechNodeIndex;

		// Token: 0x04004029 RID: 16425
		public IConversationProvider provider;
	}
}
