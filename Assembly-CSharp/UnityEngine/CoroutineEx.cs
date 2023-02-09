using System;
using System.Collections;
using System.Collections.Generic;
using Facepunch;

namespace UnityEngine
{
	// Token: 0x020009E0 RID: 2528
	public static class CoroutineEx
	{
		// Token: 0x06003B8E RID: 15246 RVA: 0x0015BE68 File Offset: 0x0015A068
		public static WaitForSeconds waitForSeconds(float seconds)
		{
			WaitForSeconds waitForSeconds;
			if (!CoroutineEx.waitForSecondsBuffer.TryGetValue(seconds, out waitForSeconds))
			{
				waitForSeconds = new WaitForSeconds(seconds);
				CoroutineEx.waitForSecondsBuffer.Add(seconds, waitForSeconds);
			}
			return waitForSeconds;
		}

		// Token: 0x06003B8F RID: 15247 RVA: 0x0015BE98 File Offset: 0x0015A098
		public static WaitForSecondsRealtimeEx waitForSecondsRealtime(float seconds)
		{
			WaitForSecondsRealtimeEx waitForSecondsRealtimeEx = Pool.Get<WaitForSecondsRealtimeEx>();
			waitForSecondsRealtimeEx.WaitTime = seconds;
			return waitForSecondsRealtimeEx;
		}

		// Token: 0x06003B90 RID: 15248 RVA: 0x0015BEA6 File Offset: 0x0015A0A6
		public static IEnumerator Combine(params IEnumerator[] coroutines)
		{
			for (;;)
			{
				bool flag = true;
				foreach (IEnumerator enumerator in coroutines)
				{
					if (enumerator != null && enumerator.MoveNext())
					{
						flag = false;
					}
				}
				if (flag)
				{
					break;
				}
				yield return CoroutineEx.waitForEndOfFrame;
			}
			yield break;
			yield break;
		}

		// Token: 0x04003553 RID: 13651
		public static WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

		// Token: 0x04003554 RID: 13652
		public static WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

		// Token: 0x04003555 RID: 13653
		private static Dictionary<float, WaitForSeconds> waitForSecondsBuffer = new Dictionary<float, WaitForSeconds>();
	}
}
