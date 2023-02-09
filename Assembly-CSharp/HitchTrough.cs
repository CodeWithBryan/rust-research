using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000106 RID: 262
public class HitchTrough : StorageContainer
{
	// Token: 0x0600151F RID: 5407 RVA: 0x000A5794 File Offset: 0x000A3994
	public global::Item GetFoodItem()
	{
		foreach (global::Item item in base.inventory.itemList)
		{
			if (item.info.category == ItemCategory.Food && item.info.GetComponent<ItemModConsumable>())
			{
				return item;
			}
		}
		return null;
	}

	// Token: 0x06001520 RID: 5408 RVA: 0x000A580C File Offset: 0x000A3A0C
	public bool ValidHitchPosition(Vector3 pos)
	{
		return this.GetClosest(pos, false, 1f) != null;
	}

	// Token: 0x06001521 RID: 5409 RVA: 0x000A5820 File Offset: 0x000A3A20
	public bool HasSpace()
	{
		HitchTrough.HitchSpot[] array = this.hitchSpots;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].IsOccupied(true))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001522 RID: 5410 RVA: 0x000A5850 File Offset: 0x000A3A50
	public HitchTrough.HitchSpot GetClosest(Vector3 testPos, bool includeOccupied = false, float maxRadius = -1f)
	{
		float num = 10000f;
		HitchTrough.HitchSpot result = null;
		for (int i = 0; i < this.hitchSpots.Length; i++)
		{
			float num2 = Vector3.Distance(testPos, this.hitchSpots[i].spot.position);
			if (num2 < num && (maxRadius == -1f || num2 <= maxRadius) && (includeOccupied || !this.hitchSpots[i].IsOccupied(true)))
			{
				num = num2;
				result = this.hitchSpots[i];
			}
		}
		return result;
	}

	// Token: 0x06001523 RID: 5411 RVA: 0x000A58C0 File Offset: 0x000A3AC0
	public void Unhitch(RidableHorse horse)
	{
		foreach (HitchTrough.HitchSpot hitchSpot in this.hitchSpots)
		{
			if (hitchSpot.GetHorse(base.isServer) == horse)
			{
				hitchSpot.SetOccupiedBy(null);
				horse.SetHitch(null);
			}
		}
	}

	// Token: 0x06001524 RID: 5412 RVA: 0x000A5908 File Offset: 0x000A3B08
	public int NumHitched()
	{
		int num = 0;
		HitchTrough.HitchSpot[] array = this.hitchSpots;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].IsOccupied(true))
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06001525 RID: 5413 RVA: 0x000A593C File Offset: 0x000A3B3C
	public bool AttemptToHitch(RidableHorse horse, HitchTrough.HitchSpot hitch = null)
	{
		if (horse == null)
		{
			return false;
		}
		if (hitch == null)
		{
			hitch = this.GetClosest(horse.transform.position, false, -1f);
		}
		if (hitch != null)
		{
			hitch.SetOccupiedBy(horse);
			horse.SetHitch(this);
			horse.transform.SetPositionAndRotation(hitch.spot.position, hitch.spot.rotation);
			horse.DismountAllPlayers();
			return true;
		}
		return false;
	}

	// Token: 0x06001526 RID: 5414 RVA: 0x000A59AC File Offset: 0x000A3BAC
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity = Pool.Get<ProtoBuf.IOEntity>();
		info.msg.ioEntity.genericEntRef1 = this.hitchSpots[0].horse.uid;
		info.msg.ioEntity.genericEntRef2 = this.hitchSpots[1].horse.uid;
	}

	// Token: 0x06001527 RID: 5415 RVA: 0x000A5A14 File Offset: 0x000A3C14
	public override void PostServerLoad()
	{
		foreach (HitchTrough.HitchSpot hitchSpot in this.hitchSpots)
		{
			this.AttemptToHitch(hitchSpot.GetHorse(true), hitchSpot);
		}
	}

	// Token: 0x06001528 RID: 5416 RVA: 0x000A5A4C File Offset: 0x000A3C4C
	public void UnhitchAll()
	{
		HitchTrough.HitchSpot[] array = this.hitchSpots;
		for (int i = 0; i < array.Length; i++)
		{
			RidableHorse horse = array[i].GetHorse(true);
			if (horse)
			{
				this.Unhitch(horse);
			}
		}
	}

	// Token: 0x06001529 RID: 5417 RVA: 0x000A5A87 File Offset: 0x000A3C87
	public override void DestroyShared()
	{
		if (base.isServer)
		{
			this.UnhitchAll();
		}
		base.DestroyShared();
	}

	// Token: 0x0600152A RID: 5418 RVA: 0x000A5A9D File Offset: 0x000A3C9D
	public override void OnKilled(HitInfo info)
	{
		base.OnKilled(info);
	}

	// Token: 0x0600152B RID: 5419 RVA: 0x000A5AA8 File Offset: 0x000A3CA8
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.hitchSpots[0].horse.uid = info.msg.ioEntity.genericEntRef1;
			this.hitchSpots[1].horse.uid = info.msg.ioEntity.genericEntRef2;
		}
	}

	// Token: 0x04000DB6 RID: 3510
	public HitchTrough.HitchSpot[] hitchSpots;

	// Token: 0x04000DB7 RID: 3511
	public float caloriesToDecaySeconds = 36f;

	// Token: 0x02000BD4 RID: 3028
	[Serializable]
	public class HitchSpot
	{
		// Token: 0x06004B4D RID: 19277 RVA: 0x00191BCD File Offset: 0x0018FDCD
		public RidableHorse GetHorse(bool isServer = true)
		{
			return this.horse.Get(isServer) as RidableHorse;
		}

		// Token: 0x06004B4E RID: 19278 RVA: 0x00191BE0 File Offset: 0x0018FDE0
		public bool IsOccupied(bool isServer = true)
		{
			return this.horse.IsValid(isServer);
		}

		// Token: 0x06004B4F RID: 19279 RVA: 0x00191BEE File Offset: 0x0018FDEE
		public void SetOccupiedBy(RidableHorse newHorse)
		{
			this.horse.Set(newHorse);
		}

		// Token: 0x04003FD0 RID: 16336
		public HitchTrough owner;

		// Token: 0x04003FD1 RID: 16337
		public Transform spot;

		// Token: 0x04003FD2 RID: 16338
		public EntityRef horse;
	}
}
