using System;

// Token: 0x02000389 RID: 905
public static class BaseEntityEx
{
	// Token: 0x06001FAD RID: 8109 RVA: 0x000D0E06 File Offset: 0x000CF006
	public static bool IsValidEntityReference<T>(this T obj) where T : class
	{
		return obj as BaseEntity != null;
	}

	// Token: 0x06001FAE RID: 8110 RVA: 0x000D0E1C File Offset: 0x000CF01C
	public static bool HasEntityInParents(this BaseEntity ent, BaseEntity toFind)
	{
		if (ent == null || toFind == null)
		{
			return false;
		}
		if (ent == toFind || ent.EqualNetID(toFind))
		{
			return true;
		}
		BaseEntity parentEntity = ent.GetParentEntity();
		while (parentEntity != null)
		{
			if (parentEntity == toFind || parentEntity.EqualNetID(toFind))
			{
				return true;
			}
			parentEntity = parentEntity.GetParentEntity();
		}
		return false;
	}
}
