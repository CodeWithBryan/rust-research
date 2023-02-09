using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004B9 RID: 1209
public class RFManager
{
	// Token: 0x060026E9 RID: 9961 RVA: 0x000F0580 File Offset: 0x000EE780
	public static int ClampFrequency(int freq)
	{
		return Mathf.Clamp(freq, RFManager.minFreq, RFManager.maxFreq);
	}

	// Token: 0x060026EA RID: 9962 RVA: 0x000F0594 File Offset: 0x000EE794
	public static List<IRFObject> GetListenList(int frequency)
	{
		frequency = RFManager.ClampFrequency(frequency);
		List<IRFObject> list = null;
		if (!RFManager._listeners.TryGetValue(frequency, out list))
		{
			list = new List<IRFObject>();
			RFManager._listeners.Add(frequency, list);
		}
		return list;
	}

	// Token: 0x060026EB RID: 9963 RVA: 0x000F05D0 File Offset: 0x000EE7D0
	public static List<IRFObject> GetBroadcasterList(int frequency)
	{
		frequency = RFManager.ClampFrequency(frequency);
		List<IRFObject> list = null;
		if (!RFManager._broadcasters.TryGetValue(frequency, out list))
		{
			list = new List<IRFObject>();
			RFManager._broadcasters.Add(frequency, list);
		}
		return list;
	}

	// Token: 0x060026EC RID: 9964 RVA: 0x000F060C File Offset: 0x000EE80C
	public static void AddListener(int frequency, IRFObject obj)
	{
		frequency = RFManager.ClampFrequency(frequency);
		List<IRFObject> listenList = RFManager.GetListenList(frequency);
		if (listenList.Contains(obj))
		{
			Debug.Log("adding same listener twice");
			return;
		}
		listenList.Add(obj);
		RFManager.MarkFrequencyDirty(frequency);
	}

	// Token: 0x060026ED RID: 9965 RVA: 0x000F064C File Offset: 0x000EE84C
	public static void RemoveListener(int frequency, IRFObject obj)
	{
		frequency = RFManager.ClampFrequency(frequency);
		List<IRFObject> listenList = RFManager.GetListenList(frequency);
		if (listenList.Contains(obj))
		{
			listenList.Remove(obj);
		}
		obj.RFSignalUpdate(false);
	}

	// Token: 0x060026EE RID: 9966 RVA: 0x000F0680 File Offset: 0x000EE880
	public static void AddBroadcaster(int frequency, IRFObject obj)
	{
		frequency = RFManager.ClampFrequency(frequency);
		List<IRFObject> broadcasterList = RFManager.GetBroadcasterList(frequency);
		if (broadcasterList.Contains(obj))
		{
			return;
		}
		broadcasterList.Add(obj);
		RFManager.MarkFrequencyDirty(frequency);
	}

	// Token: 0x060026EF RID: 9967 RVA: 0x000F06B4 File Offset: 0x000EE8B4
	public static void RemoveBroadcaster(int frequency, IRFObject obj)
	{
		frequency = RFManager.ClampFrequency(frequency);
		List<IRFObject> broadcasterList = RFManager.GetBroadcasterList(frequency);
		if (broadcasterList.Contains(obj))
		{
			broadcasterList.Remove(obj);
		}
		RFManager.MarkFrequencyDirty(frequency);
	}

	// Token: 0x060026F0 RID: 9968 RVA: 0x000F06E7 File Offset: 0x000EE8E7
	public static bool IsReserved(int frequency)
	{
		return frequency >= RFManager.reserveRangeMin && frequency <= RFManager.reserveRangeMax;
	}

	// Token: 0x060026F1 RID: 9969 RVA: 0x000F06FC File Offset: 0x000EE8FC
	public static void ReserveErrorPrint(BasePlayer player)
	{
		player.ChatMessage(RFManager.reserveString);
	}

	// Token: 0x060026F2 RID: 9970 RVA: 0x000F0709 File Offset: 0x000EE909
	public static void ChangeFrequency(int oldFrequency, int newFrequency, IRFObject obj, bool isListener, bool isOn = true)
	{
		newFrequency = RFManager.ClampFrequency(newFrequency);
		if (isListener)
		{
			RFManager.RemoveListener(oldFrequency, obj);
			if (isOn)
			{
				RFManager.AddListener(newFrequency, obj);
				return;
			}
		}
		else
		{
			RFManager.RemoveBroadcaster(oldFrequency, obj);
			if (isOn)
			{
				RFManager.AddBroadcaster(newFrequency, obj);
			}
		}
	}

	// Token: 0x060026F3 RID: 9971 RVA: 0x000F073C File Offset: 0x000EE93C
	public static void MarkFrequencyDirty(int frequency)
	{
		frequency = RFManager.ClampFrequency(frequency);
		List<IRFObject> broadcasterList = RFManager.GetBroadcasterList(frequency);
		List<IRFObject> listenList = RFManager.GetListenList(frequency);
		bool flag = broadcasterList.Count > 0;
		bool flag2 = false;
		bool flag3 = false;
		foreach (IRFObject irfobject in listenList)
		{
			if (!irfobject.IsValidEntityReference<IRFObject>())
			{
				flag2 = true;
			}
			else
			{
				if (flag)
				{
					flag = false;
					foreach (IRFObject irfobject2 in broadcasterList)
					{
						if (!irfobject2.IsValidEntityReference<IRFObject>())
						{
							flag3 = true;
						}
						else if (Vector3.Distance(irfobject2.GetPosition(), irfobject.GetPosition()) <= irfobject2.GetMaxRange())
						{
							flag = true;
							break;
						}
					}
				}
				irfobject.RFSignalUpdate(flag);
			}
		}
		if (flag2)
		{
			Debug.LogWarning("Found null entries in the RF listener list for frequency " + frequency + "... cleaning up.");
			for (int i = listenList.Count - 1; i >= 0; i--)
			{
				if (listenList[i] == null)
				{
					listenList.RemoveAt(i);
				}
			}
		}
		if (flag3)
		{
			Debug.LogWarning("Found null entries in the RF broadcaster list for frequency " + frequency + "... cleaning up.");
			for (int j = broadcasterList.Count - 1; j >= 0; j--)
			{
				if (broadcasterList[j] == null)
				{
					broadcasterList.RemoveAt(j);
				}
			}
		}
	}

	// Token: 0x04001F62 RID: 8034
	public static Dictionary<int, List<IRFObject>> _listeners = new Dictionary<int, List<IRFObject>>();

	// Token: 0x04001F63 RID: 8035
	public static Dictionary<int, List<IRFObject>> _broadcasters = new Dictionary<int, List<IRFObject>>();

	// Token: 0x04001F64 RID: 8036
	public static int minFreq = 1;

	// Token: 0x04001F65 RID: 8037
	public static int maxFreq = 9999;

	// Token: 0x04001F66 RID: 8038
	private static int reserveRangeMin = 4760;

	// Token: 0x04001F67 RID: 8039
	private static int reserveRangeMax = 4790;

	// Token: 0x04001F68 RID: 8040
	public static string reserveString = string.Concat(new object[]
	{
		"Channels ",
		RFManager.reserveRangeMin,
		" to ",
		RFManager.reserveRangeMax,
		" are restricted."
	});
}
