using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine;

namespace Facepunch
{
	// Token: 0x02000AAC RID: 2732
	public class FPNativeList<[IsUnmanaged] T> : Pool.IPooled where T : struct, ValueType
	{
		// Token: 0x17000588 RID: 1416
		// (get) Token: 0x0600416B RID: 16747 RVA: 0x00180349 File Offset: 0x0017E549
		public NativeArray<T> Array
		{
			get
			{
				return this._array;
			}
		}

		// Token: 0x17000589 RID: 1417
		// (get) Token: 0x0600416C RID: 16748 RVA: 0x00180351 File Offset: 0x0017E551
		public int Count
		{
			get
			{
				return this._length;
			}
		}

		// Token: 0x1700058A RID: 1418
		public T this[int index]
		{
			get
			{
				return this._array[index];
			}
			set
			{
				this._array[index] = value;
			}
		}

		// Token: 0x0600416F RID: 16751 RVA: 0x00180378 File Offset: 0x0017E578
		public void Add(T item)
		{
			this.EnsureCapacity(this._length + 1);
			int length = this._length;
			this._length = length + 1;
			this._array[length] = item;
		}

		// Token: 0x06004170 RID: 16752 RVA: 0x001803B0 File Offset: 0x0017E5B0
		public void Clear()
		{
			for (int i = 0; i < this._array.Length; i++)
			{
				this._array[i] = default(T);
			}
			this._length = 0;
		}

		// Token: 0x06004171 RID: 16753 RVA: 0x001803EF File Offset: 0x0017E5EF
		public void Resize(int count)
		{
			if (this._array.IsCreated)
			{
				this._array.Dispose();
			}
			this._array = new NativeArray<T>(count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this._length = count;
		}

		// Token: 0x06004172 RID: 16754 RVA: 0x00180420 File Offset: 0x0017E620
		public void EnsureCapacity(int requiredCapacity)
		{
			if (!this._array.IsCreated || this._array.Length < requiredCapacity)
			{
				int length = Mathf.Max(this._array.Length * 2, requiredCapacity);
				NativeArray<T> array = new NativeArray<T>(length, Allocator.Persistent, NativeArrayOptions.ClearMemory);
				if (this._array.IsCreated)
				{
					this._array.CopyTo(array.GetSubArray(0, this._array.Length));
					this._array.Dispose();
				}
				this._array = array;
			}
		}

		// Token: 0x06004173 RID: 16755 RVA: 0x001804A3 File Offset: 0x0017E6A3
		public void EnterPool()
		{
			if (this._array.IsCreated)
			{
				this._array.Dispose();
			}
			this._array = default(NativeArray<T>);
			this._length = 0;
		}

		// Token: 0x06004174 RID: 16756 RVA: 0x000059DD File Offset: 0x00003BDD
		public void LeavePool()
		{
		}

		// Token: 0x04003A5A RID: 14938
		private NativeArray<T> _array;

		// Token: 0x04003A5B RID: 14939
		private int _length;
	}
}
