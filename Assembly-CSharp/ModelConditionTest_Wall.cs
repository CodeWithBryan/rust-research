using System;

// Token: 0x0200024F RID: 591
public class ModelConditionTest_Wall : ModelConditionTest
{
	// Token: 0x06001B5A RID: 7002 RVA: 0x000BEE97 File Offset: 0x000BD097
	public override bool DoTest(BaseEntity ent)
	{
		return !ModelConditionTest_WallTriangleLeft.CheckCondition(ent) && !ModelConditionTest_WallTriangleRight.CheckCondition(ent);
	}
}
