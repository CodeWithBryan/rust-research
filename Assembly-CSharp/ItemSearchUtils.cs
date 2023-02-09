using System;
using System.Globalization;
using System.Linq;
using UnityEngine;

// Token: 0x02000831 RID: 2097
public static class ItemSearchUtils
{
	// Token: 0x06003492 RID: 13458 RVA: 0x0013E3FC File Offset: 0x0013C5FC
	public static IOrderedEnumerable<ItemDefinition> SearchForItems(string searchString, Func<ItemDefinition, bool> validFilter = null)
	{
		if (searchString == "")
		{
			searchString = "BALLS BALLS BALLS";
		}
		return from y in (from x in ItemManager.itemList
		where ItemSearchUtils.IsValidSearchResult(searchString, x) && (validFilter == null || validFilter(x))
		select x).Take(60)
		orderby ItemSearchUtils.ScoreSearchResult(searchString, y)
		select y;
	}

	// Token: 0x06003493 RID: 13459 RVA: 0x0013E468 File Offset: 0x0013C668
	private static bool IsValidSearchResult(string search, ItemDefinition target)
	{
		return ((target.isRedirectOf != null && target.redirectVendingBehaviour == ItemDefinition.RedirectVendingBehaviour.ListAsUniqueItem) || !target.hidden) && (target.shortname.Contains(search, CompareOptions.IgnoreCase) || target.displayName.translated.Contains(search, CompareOptions.IgnoreCase) || target.displayDescription.translated.Contains(search, CompareOptions.IgnoreCase));
	}

	// Token: 0x06003494 RID: 13460 RVA: 0x0013E4D0 File Offset: 0x0013C6D0
	private static float ScoreSearchResult(string search, ItemDefinition target)
	{
		float num = 0f;
		if (target.shortname.Equals(search, StringComparison.CurrentCultureIgnoreCase) || target.displayName.translated.Equals(search, StringComparison.CurrentCultureIgnoreCase))
		{
			num -= (float)(500 - search.Length);
		}
		float a = target.shortname.Contains(search, CompareOptions.IgnoreCase) ? ((float)search.Length / (float)target.shortname.Length) : 0f;
		float b = target.displayName.translated.Contains(search, CompareOptions.IgnoreCase) ? ((float)search.Length / (float)target.displayName.translated.Length) : 0f;
		float num2 = Mathf.Max(a, b);
		num -= 50f * num2;
		if (target.displayDescription.translated.Contains(search, CompareOptions.IgnoreCase))
		{
			num -= (float)search.Length;
		}
		return num;
	}
}
