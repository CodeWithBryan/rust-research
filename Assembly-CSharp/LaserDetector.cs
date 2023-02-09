using System;
using System.Collections.Generic;

// Token: 0x0200010A RID: 266
public class LaserDetector : BaseDetector
{
	// Token: 0x06001552 RID: 5458 RVA: 0x000A6198 File Offset: 0x000A4398
	public override void OnObjects()
	{
		using (HashSet<BaseEntity>.Enumerator enumerator = this.myTrigger.entityContents.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.IsVisible(base.transform.position + base.transform.forward * 0.1f, 4f))
				{
					base.OnObjects();
					break;
				}
			}
		}
	}
}
