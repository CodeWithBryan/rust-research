using System;

// Token: 0x0200024E RID: 590
public class ModelConditionTest_True : ModelConditionTest
{
	// Token: 0x06001B58 RID: 7000 RVA: 0x000BEE89 File Offset: 0x000BD089
	public override bool DoTest(BaseEntity ent)
	{
		return this.reference.RunTests(ent);
	}

	// Token: 0x04001469 RID: 5225
	public ConditionalModel reference;
}
