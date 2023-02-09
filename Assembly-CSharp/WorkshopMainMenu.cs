using System;

// Token: 0x02000869 RID: 2153
public class WorkshopMainMenu : SingletonComponent<WorkshopMainMenu>
{
	// Token: 0x04002FBE RID: 12222
	public static Translate.Phrase loading_workshop = new TokenisedPhrase("loading.workshop", "Loading Workshop");

	// Token: 0x04002FBF RID: 12223
	public static Translate.Phrase loading_workshop_setup = new TokenisedPhrase("loading.workshop.initializing", "Setting Up Scene");

	// Token: 0x04002FC0 RID: 12224
	public static Translate.Phrase loading_workshop_skinnables = new TokenisedPhrase("loading.workshop.skinnables", "Getting Skinnables");

	// Token: 0x04002FC1 RID: 12225
	public static Translate.Phrase loading_workshop_item = new TokenisedPhrase("loading.workshop.item", "Loading Item Data");
}
