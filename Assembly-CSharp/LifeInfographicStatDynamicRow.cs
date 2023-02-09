using System;
using Rust.UI;

// Token: 0x02000834 RID: 2100
public class LifeInfographicStatDynamicRow : LifeInfographicStat
{
	// Token: 0x06003497 RID: 13463 RVA: 0x0013E5A5 File Offset: 0x0013C7A5
	public void SetStatName(Translate.Phrase phrase)
	{
		this.StatName.SetPhrase(phrase);
	}

	// Token: 0x04002EA9 RID: 11945
	public RustText StatName;
}
