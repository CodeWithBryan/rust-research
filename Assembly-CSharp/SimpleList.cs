using System;

// Token: 0x0200091B RID: 2331
public class SimpleList<T>
{
	// Token: 0x17000431 RID: 1073
	// (get) Token: 0x06003786 RID: 14214 RVA: 0x00149A9C File Offset: 0x00147C9C
	public T[] Array
	{
		get
		{
			return this.array;
		}
	}

	// Token: 0x17000432 RID: 1074
	// (get) Token: 0x06003787 RID: 14215 RVA: 0x00149AA4 File Offset: 0x00147CA4
	public int Count
	{
		get
		{
			return this.count;
		}
	}

	// Token: 0x17000433 RID: 1075
	// (get) Token: 0x06003788 RID: 14216 RVA: 0x00149AAC File Offset: 0x00147CAC
	// (set) Token: 0x06003789 RID: 14217 RVA: 0x00149AB8 File Offset: 0x00147CB8
	public int Capacity
	{
		get
		{
			return this.array.Length;
		}
		set
		{
			if (value != this.array.Length)
			{
				if (value > 0)
				{
					T[] destinationArray = new T[value];
					if (this.count > 0)
					{
						System.Array.Copy(this.array, 0, destinationArray, 0, this.count);
					}
					this.array = destinationArray;
					return;
				}
				this.array = SimpleList<T>.emptyArray;
			}
		}
	}

	// Token: 0x17000434 RID: 1076
	public T this[int index]
	{
		get
		{
			return this.array[index];
		}
		set
		{
			this.array[index] = value;
		}
	}

	// Token: 0x0600378C RID: 14220 RVA: 0x00149B28 File Offset: 0x00147D28
	public SimpleList()
	{
		this.array = SimpleList<T>.emptyArray;
	}

	// Token: 0x0600378D RID: 14221 RVA: 0x00149B3B File Offset: 0x00147D3B
	public SimpleList(int capacity)
	{
		this.array = ((capacity == 0) ? SimpleList<T>.emptyArray : new T[capacity]);
	}

	// Token: 0x0600378E RID: 14222 RVA: 0x00149B5C File Offset: 0x00147D5C
	public void Add(T item)
	{
		if (this.count == this.array.Length)
		{
			this.EnsureCapacity(this.count + 1);
		}
		T[] array = this.array;
		int num = this.count;
		this.count = num + 1;
		array[num] = item;
	}

	// Token: 0x0600378F RID: 14223 RVA: 0x00149BA4 File Offset: 0x00147DA4
	public void Clear()
	{
		if (this.count > 0)
		{
			System.Array.Clear(this.array, 0, this.count);
			this.count = 0;
		}
	}

	// Token: 0x06003790 RID: 14224 RVA: 0x00149BC8 File Offset: 0x00147DC8
	public bool Contains(T item)
	{
		for (int i = 0; i < this.count; i++)
		{
			if (this.array[i].Equals(item))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003791 RID: 14225 RVA: 0x00149C0A File Offset: 0x00147E0A
	public void CopyTo(T[] array)
	{
		System.Array.Copy(this.array, 0, array, 0, this.count);
	}

	// Token: 0x06003792 RID: 14226 RVA: 0x00149C20 File Offset: 0x00147E20
	public void EnsureCapacity(int min)
	{
		if (this.array.Length < min)
		{
			int num = (this.array.Length == 0) ? 16 : (this.array.Length * 2);
			num = ((num < min) ? min : num);
			this.Capacity = num;
		}
	}

	// Token: 0x040031CE RID: 12750
	private const int defaultCapacity = 16;

	// Token: 0x040031CF RID: 12751
	private static readonly T[] emptyArray = new T[0];

	// Token: 0x040031D0 RID: 12752
	public T[] array;

	// Token: 0x040031D1 RID: 12753
	public int count;
}
