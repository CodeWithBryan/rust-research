using System;

// Token: 0x02000243 RID: 579
public abstract class ModelConditionTest : PrefabAttribute
{
	// Token: 0x06001B2F RID: 6959
	public abstract bool DoTest(BaseEntity ent);

	// Token: 0x06001B30 RID: 6960 RVA: 0x000BE39D File Offset: 0x000BC59D
	protected override Type GetIndexedType()
	{
		return typeof(ModelConditionTest);
	}
}
