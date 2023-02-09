using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001C4 RID: 452
public class AIInformationCellContents<T> where T : AIPoint
{
	// Token: 0x170001D0 RID: 464
	// (get) Token: 0x0600180C RID: 6156 RVA: 0x000B1F40 File Offset: 0x000B0140
	public int Count
	{
		get
		{
			return this.Items.Count;
		}
	}

	// Token: 0x170001D1 RID: 465
	// (get) Token: 0x0600180D RID: 6157 RVA: 0x000B1F4D File Offset: 0x000B014D
	public bool Empty
	{
		get
		{
			return this.Items.Count == 0;
		}
	}

	// Token: 0x0600180E RID: 6158 RVA: 0x000B1F60 File Offset: 0x000B0160
	public void Init(Bounds cellBounds, GameObject root)
	{
		this.Clear();
		foreach (T t in root.GetComponentsInChildren<T>(true))
		{
			if (cellBounds.Contains(t.gameObject.transform.position))
			{
				this.Add(t);
			}
		}
	}

	// Token: 0x0600180F RID: 6159 RVA: 0x000B1FB6 File Offset: 0x000B01B6
	public void Clear()
	{
		this.Items.Clear();
	}

	// Token: 0x06001810 RID: 6160 RVA: 0x000B1FC3 File Offset: 0x000B01C3
	public void Add(T item)
	{
		this.Items.Add(item);
	}

	// Token: 0x06001811 RID: 6161 RVA: 0x000B1FD2 File Offset: 0x000B01D2
	public void Remove(T item)
	{
		this.Items.Remove(item);
	}

	// Token: 0x0400114F RID: 4431
	public HashSet<T> Items = new HashSet<T>();
}
