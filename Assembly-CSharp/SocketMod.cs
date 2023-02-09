using System;

// Token: 0x02000256 RID: 598
public class SocketMod : PrefabAttribute
{
	// Token: 0x06001B71 RID: 7025 RVA: 0x00007074 File Offset: 0x00005274
	public virtual bool DoCheck(Construction.Placement place)
	{
		return false;
	}

	// Token: 0x06001B72 RID: 7026 RVA: 0x000059DD File Offset: 0x00003BDD
	public virtual void ModifyPlacement(Construction.Placement place)
	{
	}

	// Token: 0x06001B73 RID: 7027 RVA: 0x000BF4B0 File Offset: 0x000BD6B0
	protected override Type GetIndexedType()
	{
		return typeof(SocketMod);
	}

	// Token: 0x0400147C RID: 5244
	[NonSerialized]
	public Socket_Base baseSocket;

	// Token: 0x0400147D RID: 5245
	public Translate.Phrase FailedPhrase;
}
