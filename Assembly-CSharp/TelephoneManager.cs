using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000385 RID: 901
public static class TelephoneManager
{
	// Token: 0x06001F90 RID: 8080 RVA: 0x000D05FC File Offset: 0x000CE7FC
	public static int GetUnusedTelephoneNumber()
	{
		int num = UnityEngine.Random.Range(10000000, 99990000);
		int num2 = 0;
		int num3 = 1000;
		while (TelephoneManager.allTelephones.ContainsKey(num) && num2 < num3)
		{
			num2++;
			num = UnityEngine.Random.Range(10000000, 99990000);
		}
		if (num2 == num3)
		{
			num = TelephoneManager.maxAssignedPhoneNumber + 1;
		}
		TelephoneManager.maxAssignedPhoneNumber = Mathf.Max(TelephoneManager.maxAssignedPhoneNumber, num);
		return num;
	}

	// Token: 0x06001F91 RID: 8081 RVA: 0x000D0668 File Offset: 0x000CE868
	public static void RegisterTelephone(PhoneController t, bool checkPhoneNumber = false)
	{
		if (checkPhoneNumber && TelephoneManager.allTelephones.ContainsKey(t.PhoneNumber) && TelephoneManager.allTelephones[t.PhoneNumber] != t)
		{
			t.PhoneNumber = TelephoneManager.GetUnusedTelephoneNumber();
		}
		if (!TelephoneManager.allTelephones.ContainsKey(t.PhoneNumber) && t.PhoneNumber != 0)
		{
			TelephoneManager.allTelephones.Add(t.PhoneNumber, t);
			TelephoneManager.maxAssignedPhoneNumber = Mathf.Max(TelephoneManager.maxAssignedPhoneNumber, t.PhoneNumber);
		}
	}

	// Token: 0x06001F92 RID: 8082 RVA: 0x000D06ED File Offset: 0x000CE8ED
	public static void DeregisterTelephone(PhoneController t)
	{
		if (TelephoneManager.allTelephones.ContainsKey(t.PhoneNumber))
		{
			TelephoneManager.allTelephones.Remove(t.PhoneNumber);
		}
	}

	// Token: 0x06001F93 RID: 8083 RVA: 0x000D0712 File Offset: 0x000CE912
	public static PhoneController GetTelephone(int number)
	{
		if (TelephoneManager.allTelephones.ContainsKey(number))
		{
			return TelephoneManager.allTelephones[number];
		}
		return null;
	}

	// Token: 0x06001F94 RID: 8084 RVA: 0x000D0730 File Offset: 0x000CE930
	public static PhoneController GetRandomTelephone(int ignoreNumber)
	{
		foreach (KeyValuePair<int, PhoneController> keyValuePair in TelephoneManager.allTelephones)
		{
			if (keyValuePair.Value.PhoneNumber != ignoreNumber)
			{
				return keyValuePair.Value;
			}
		}
		return null;
	}

	// Token: 0x06001F95 RID: 8085 RVA: 0x000D0798 File Offset: 0x000CE998
	public static int GetCurrentActiveCalls()
	{
		int num = 0;
		foreach (KeyValuePair<int, PhoneController> keyValuePair in TelephoneManager.allTelephones)
		{
			if (keyValuePair.Value.serverState != global::Telephone.CallState.Idle)
			{
				num++;
			}
		}
		if (num == 0)
		{
			return 0;
		}
		return num / 2;
	}

	// Token: 0x06001F96 RID: 8086 RVA: 0x000D0800 File Offset: 0x000CEA00
	public static void GetPhoneDirectory(int ignoreNumber, int page, int perPage, PhoneDirectory directory)
	{
		directory.entries = Pool.GetList<PhoneDirectory.DirectoryEntry>();
		int num = page * perPage;
		int num2 = 0;
		foreach (KeyValuePair<int, PhoneController> keyValuePair in TelephoneManager.allTelephones)
		{
			if (keyValuePair.Key != ignoreNumber && !string.IsNullOrEmpty(keyValuePair.Value.PhoneName))
			{
				num2++;
				if (num2 >= num)
				{
					PhoneDirectory.DirectoryEntry directoryEntry = Pool.Get<PhoneDirectory.DirectoryEntry>();
					directoryEntry.phoneName = keyValuePair.Value.GetDirectoryName();
					directoryEntry.phoneNumber = keyValuePair.Value.PhoneNumber;
					directory.entries.Add(directoryEntry);
					if (directory.entries.Count >= perPage)
					{
						directory.atEnd = false;
						return;
					}
				}
			}
		}
		directory.atEnd = true;
	}

	// Token: 0x06001F97 RID: 8087 RVA: 0x000D08DC File Offset: 0x000CEADC
	[ServerVar]
	public static void PrintAllPhones(ConsoleSystem.Arg arg)
	{
		TextTable textTable = new TextTable();
		textTable.AddColumns(new string[]
		{
			"Number",
			"Name",
			"Position"
		});
		foreach (KeyValuePair<int, PhoneController> keyValuePair in TelephoneManager.allTelephones)
		{
			Vector3 position = keyValuePair.Value.transform.position;
			textTable.AddRow(new string[]
			{
				keyValuePair.Key.ToString(),
				keyValuePair.Value.GetDirectoryName(),
				string.Format("{0} {1} {2}", position.x, position.y, position.z)
			});
		}
		arg.ReplyWith(textTable.ToString());
	}

	// Token: 0x040018DE RID: 6366
	public const int MaxPhoneNumber = 99990000;

	// Token: 0x040018DF RID: 6367
	public const int MinPhoneNumber = 10000000;

	// Token: 0x040018E0 RID: 6368
	[ServerVar]
	public static int MaxConcurrentCalls = 10;

	// Token: 0x040018E1 RID: 6369
	[ServerVar]
	public static int MaxCallLength = 120;

	// Token: 0x040018E2 RID: 6370
	private static Dictionary<int, PhoneController> allTelephones = new Dictionary<int, PhoneController>();

	// Token: 0x040018E3 RID: 6371
	private static int maxAssignedPhoneNumber = 99990000;
}
