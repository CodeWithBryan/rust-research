using System;

// Token: 0x02000289 RID: 649
public class Client : SingletonComponent<Client>
{
	// Token: 0x04001538 RID: 5432
	public static Translate.Phrase loading_loading = new Translate.Phrase("loading.loading", "Loading");

	// Token: 0x04001539 RID: 5433
	public static Translate.Phrase loading_connecting = new Translate.Phrase("loading.connecting", "Connecting");

	// Token: 0x0400153A RID: 5434
	public static Translate.Phrase loading_connectionaccepted = new Translate.Phrase("loading.connectionaccepted", "Connection Accepted");

	// Token: 0x0400153B RID: 5435
	public static Translate.Phrase loading_connecting_negotiate = new Translate.Phrase("loading.connecting.negotiate", "Negotiating Connection");

	// Token: 0x0400153C RID: 5436
	public static Translate.Phrase loading_level = new Translate.Phrase("loading.loadinglevel", "Loading Level");

	// Token: 0x0400153D RID: 5437
	public static Translate.Phrase loading_skinnablewarmup = new Translate.Phrase("loading.skinnablewarmup", "Skinnable Warmup");

	// Token: 0x0400153E RID: 5438
	public static Translate.Phrase loading_preloadcomplete = new Translate.Phrase("loading.preloadcomplete", "Preload Complete");

	// Token: 0x0400153F RID: 5439
	public static Translate.Phrase loading_openingscene = new Translate.Phrase("loading.openingscene", "Opening Scene");

	// Token: 0x04001540 RID: 5440
	public static Translate.Phrase loading_clientready = new Translate.Phrase("loading.clientready", "Client Ready");

	// Token: 0x04001541 RID: 5441
	public static Translate.Phrase loading_prefabwarmup = new Translate.Phrase("loading.prefabwarmup", "Warming Prefabs [{0}/{1}]");
}
