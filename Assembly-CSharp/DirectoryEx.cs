using System;
using System.IO;
using System.Threading;

// Token: 0x020008ED RID: 2285
public static class DirectoryEx
{
	// Token: 0x06003697 RID: 13975 RVA: 0x00144BEC File Offset: 0x00142DEC
	public static void Backup(DirectoryInfo parent, params string[] names)
	{
		for (int i = 0; i < names.Length; i++)
		{
			names[i] = Path.Combine(parent.FullName, names[i]);
		}
		DirectoryEx.Backup(names);
	}

	// Token: 0x06003698 RID: 13976 RVA: 0x00144C20 File Offset: 0x00142E20
	public static bool MoveToSafe(this DirectoryInfo parent, string target, int retries = 10)
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

	// Token: 0x06003699 RID: 13977 RVA: 0x00144C60 File Offset: 0x00142E60
	public static void Backup(params string[] names)
	{
		for (int i = names.Length - 2; i >= 0; i--)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(names[i]);
			DirectoryInfo directoryInfo2 = new DirectoryInfo(names[i + 1]);
			if (directoryInfo.Exists)
			{
				if (directoryInfo2.Exists)
				{
					double totalHours = (DateTime.Now - directoryInfo2.LastWriteTime).TotalHours;
					int num = (i == 0) ? 0 : (1 << i - 1);
					if (totalHours >= (double)num)
					{
						directoryInfo2.Delete(true);
						directoryInfo.MoveToSafe(directoryInfo2.FullName, 10);
					}
				}
				else
				{
					if (!directoryInfo2.Parent.Exists)
					{
						directoryInfo2.Parent.Create();
					}
					directoryInfo.MoveToSafe(directoryInfo2.FullName, 10);
				}
			}
		}
	}

	// Token: 0x0600369A RID: 13978 RVA: 0x00144D14 File Offset: 0x00142F14
	public static void CopyAll(string sourceDirectory, string targetDirectory)
	{
		DirectoryInfo source = new DirectoryInfo(sourceDirectory);
		DirectoryInfo target = new DirectoryInfo(targetDirectory);
		DirectoryEx.CopyAll(source, target);
	}

	// Token: 0x0600369B RID: 13979 RVA: 0x00144D34 File Offset: 0x00142F34
	public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
	{
		if (source.FullName.ToLower() == target.FullName.ToLower())
		{
			return;
		}
		if (!source.Exists)
		{
			return;
		}
		if (!target.Exists)
		{
			target.Create();
		}
		foreach (FileInfo fileInfo in source.GetFiles())
		{
			FileInfo fileInfo2 = new FileInfo(Path.Combine(target.FullName, fileInfo.Name));
			fileInfo.CopyTo(fileInfo2.FullName, true);
			fileInfo2.CreationTime = fileInfo.CreationTime;
			fileInfo2.LastAccessTime = fileInfo.LastAccessTime;
			fileInfo2.LastWriteTime = fileInfo.LastWriteTime;
		}
		foreach (DirectoryInfo directoryInfo in source.GetDirectories())
		{
			DirectoryInfo directoryInfo2 = target.CreateSubdirectory(directoryInfo.Name);
			DirectoryEx.CopyAll(directoryInfo, directoryInfo2);
			directoryInfo2.CreationTime = directoryInfo.CreationTime;
			directoryInfo2.LastAccessTime = directoryInfo.LastAccessTime;
			directoryInfo2.LastWriteTime = directoryInfo.LastWriteTime;
		}
	}
}
