using System;

// Token: 0x02000378 RID: 888
public interface ICassettePlayer
{
	// Token: 0x06001F33 RID: 7987
	void OnCassetteInserted(Cassette c);

	// Token: 0x06001F34 RID: 7988
	void OnCassetteRemoved(Cassette c);

	// Token: 0x17000260 RID: 608
	// (get) Token: 0x06001F35 RID: 7989
	BaseEntity ToBaseEntity { get; }
}
