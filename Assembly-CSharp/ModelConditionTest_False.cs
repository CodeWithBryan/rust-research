using System;

// Token: 0x02000244 RID: 580
public class ModelConditionTest_False : ModelConditionTest
{
	// Token: 0x06001B32 RID: 6962 RVA: 0x000BE3A9 File Offset: 0x000BC5A9
	public override bool DoTest(BaseEntity ent)
	{
		return !this.reference.RunTests(ent);
	}

	// Token: 0x0400143B RID: 5179
	public ConditionalModel reference;
}
