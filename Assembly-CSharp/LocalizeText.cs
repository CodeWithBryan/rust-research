using System;
using Rust.Localization;
using UnityEngine;

// Token: 0x020008B1 RID: 2225
public class LocalizeText : MonoBehaviour, IClientComponent, ILocalize
{
	// Token: 0x17000410 RID: 1040
	// (get) Token: 0x060035ED RID: 13805 RVA: 0x00142E04 File Offset: 0x00141004
	// (set) Token: 0x060035EE RID: 13806 RVA: 0x00142E0C File Offset: 0x0014100C
	public string LanguageToken
	{
		get
		{
			return this.token;
		}
		set
		{
			this.token = value;
		}
	}

	// Token: 0x17000411 RID: 1041
	// (get) Token: 0x060035EF RID: 13807 RVA: 0x00142E15 File Offset: 0x00141015
	// (set) Token: 0x060035F0 RID: 13808 RVA: 0x00142E1D File Offset: 0x0014101D
	public string LanguageEnglish
	{
		get
		{
			return this.english;
		}
		set
		{
			this.english = value;
		}
	}

	// Token: 0x040030ED RID: 12525
	public string token;

	// Token: 0x040030EE RID: 12526
	[TextArea]
	public string english;

	// Token: 0x040030EF RID: 12527
	public string append;

	// Token: 0x040030F0 RID: 12528
	public LocalizeText.SpecialMode specialMode;

	// Token: 0x02000E4D RID: 3661
	public enum SpecialMode
	{
		// Token: 0x040049F0 RID: 18928
		None,
		// Token: 0x040049F1 RID: 18929
		AllUppercase,
		// Token: 0x040049F2 RID: 18930
		AllLowercase
	}
}
