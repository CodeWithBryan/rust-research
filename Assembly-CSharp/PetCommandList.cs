using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000375 RID: 885
public class PetCommandList : PrefabAttribute
{
	// Token: 0x06001F17 RID: 7959 RVA: 0x000CEE49 File Offset: 0x000CD049
	protected override Type GetIndexedType()
	{
		return typeof(PetCommandList);
	}

	// Token: 0x06001F18 RID: 7960 RVA: 0x000CEE55 File Offset: 0x000CD055
	public List<PetCommandList.PetCommandDesc> GetCommandDescriptions()
	{
		return this.Commands;
	}

	// Token: 0x04001873 RID: 6259
	public List<PetCommandList.PetCommandDesc> Commands;

	// Token: 0x02000C61 RID: 3169
	[Serializable]
	public struct PetCommandDesc
	{
		// Token: 0x0400421F RID: 16927
		public PetCommandType CommandType;

		// Token: 0x04004220 RID: 16928
		public Translate.Phrase Title;

		// Token: 0x04004221 RID: 16929
		public Translate.Phrase Description;

		// Token: 0x04004222 RID: 16930
		public Sprite Icon;

		// Token: 0x04004223 RID: 16931
		public int CommandIndex;

		// Token: 0x04004224 RID: 16932
		public bool Raycast;

		// Token: 0x04004225 RID: 16933
		public int CommandWheelOrder;
	}
}
