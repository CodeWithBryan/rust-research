﻿using System;
using System.IO;
using System.Threading;

// Token: 0x020008EE RID: 2286
public static class FileEx
{
	// Token: 0x0600369C RID: 13980 RVA: 0x00144E38 File Offset: 0x00143038
	public static void Backup(DirectoryInfo parent, params string[] names)
	{
		for (int i = 0; i < names.Length; i++)
		{
			names[i] = Path.Combine(parent.FullName, names[i]);
		}
		FileEx.Backup(names);
	}

	// Token: 0x0600369D RID: 13981 RVA: 0x00144E6C File Offset: 0x0014306C
	public static bool MoveToSafe(this FileInfo parent, string target, int retries = 10)
	{
		for (int i = 0; i < retries; i++)
		{
			try
			{
				parent.MoveTo(target);
			}
			catch (Exception)
			{
				Thread.Sleep(5);
				goto IL_19;
			}
			return true;
			IL_19:;
		}
		return false;
	}

	// Token: 0x0600369E RID: 13982 RVA: 0x00144EAC File Offset: 0x001430AC
	public static void Backup(params string[] names)
	{
		for (int i = names.Length - 2; i >= 0; i--)
		{
			FileInfo fileInfo = new FileInfo(names[i]);
			FileInfo fileInfo2 = new FileInfo(names[i + 1]);
			if (fileInfo.Exists)
			{
				if (fileInfo2.Exists)
				{
					double totalHours = (DateTime.Now - fileInfo2.LastWriteTime).TotalHours;
					int num = (i == 0) ? 0 : (1 << i - 1);
					if (totalHours >= (double)num)
					{
						fileInfo2.Delete();
						fileInfo.MoveToSafe(fileInfo2.FullName, 10);
					}
				}
				else
				{
					if (!fileInfo2.Directory.Exists)
					{
						fileInfo2.Directory.Create();
					}
					fileInfo.MoveToSafe(fileInfo2.FullName, 10);
				}
			}
		}
	}
}
