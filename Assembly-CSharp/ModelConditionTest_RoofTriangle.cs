using System;

// Token: 0x0200024C RID: 588
public class ModelConditionTest_RoofTriangle : ModelConditionTest
{
	// Token: 0x06001B52 RID: 6994 RVA: 0x000BED3C File Offset: 0x000BCF3C
	public override bool DoTest(BaseEntity ent)
	{
		EntityLink entityLink = ent.FindLink("roof/sockets/wall-female");
		return entityLink == null || entityLink.IsEmpty();
	}

	// Token: 0x04001464 RID: 5220
	private const string socket = "roof/sockets/wall-female";
}
