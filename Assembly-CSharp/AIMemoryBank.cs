using System;
using UnityEngine;

// Token: 0x02000348 RID: 840
public class AIMemoryBank<T>
{
	// Token: 0x06001E4F RID: 7759 RVA: 0x000CD5A7 File Offset: 0x000CB7A7
	public AIMemoryBank(MemoryBankType type, int slots)
	{
		this.Init(type, slots);
	}

	// Token: 0x06001E50 RID: 7760 RVA: 0x000CD5B7 File Offset: 0x000CB7B7
	public void Init(MemoryBankType type, int slots)
	{
		this.type = type;
		this.slotCount = slots;
		this.slots = new T[this.slotCount];
		this.slotSetTimestamps = new float[this.slotCount];
	}

	// Token: 0x06001E51 RID: 7761 RVA: 0x000CD5E9 File Offset: 0x000CB7E9
	public void Set(T item, int index)
	{
		if (index < 0 || index >= this.slotCount)
		{
			return;
		}
		this.slots[index] = item;
		this.slotSetTimestamps[index] = Time.realtimeSinceStartup;
	}

	// Token: 0x06001E52 RID: 7762 RVA: 0x000CD614 File Offset: 0x000CB814
	public T Get(int index)
	{
		if (index < 0 || index >= this.slotCount)
		{
			return default(T);
		}
		return this.slots[index];
	}

	// Token: 0x06001E53 RID: 7763 RVA: 0x000CD644 File Offset: 0x000CB844
	public float GetTimeSinceSet(int index)
	{
		if (index < 0 || index >= this.slotCount)
		{
			return 0f;
		}
		return Time.realtimeSinceStartup - this.slotSetTimestamps[index];
	}

	// Token: 0x06001E54 RID: 7764 RVA: 0x000CD668 File Offset: 0x000CB868
	public void Remove(int index)
	{
		if (index < 0 || index >= this.slotCount)
		{
			return;
		}
		this.slots[index] = default(T);
	}

	// Token: 0x04001835 RID: 6197
	private MemoryBankType type;

	// Token: 0x04001836 RID: 6198
	private T[] slots;

	// Token: 0x04001837 RID: 6199
	private float[] slotSetTimestamps;

	// Token: 0x04001838 RID: 6200
	private int slotCount;
}
