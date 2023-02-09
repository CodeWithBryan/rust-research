using System;
using UnityEngine.EventSystems;

// Token: 0x020007B6 RID: 1974
public class FpStandaloneInputModule : StandaloneInputModule
{
	// Token: 0x170003FA RID: 1018
	// (get) Token: 0x060033C3 RID: 13251 RVA: 0x0013BED3 File Offset: 0x0013A0D3
	public PointerEventData CurrentData
	{
		get
		{
			if (!this.m_PointerData.ContainsKey(-1))
			{
				return new PointerEventData(EventSystem.current);
			}
			return this.m_PointerData[-1];
		}
	}
}
