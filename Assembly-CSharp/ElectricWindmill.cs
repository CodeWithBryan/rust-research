using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200012B RID: 299
public class ElectricWindmill : global::IOEntity
{
	// Token: 0x060015CE RID: 5582 RVA: 0x000A7A46 File Offset: 0x000A5C46
	public override int MaximalPowerOutput()
	{
		return this.maxPowerGeneration;
	}

	// Token: 0x060015CF RID: 5583 RVA: 0x00003A54 File Offset: 0x00001C54
	public override bool IsRootEntity()
	{
		return true;
	}

	// Token: 0x060015D0 RID: 5584 RVA: 0x000A7A50 File Offset: 0x000A5C50
	public float GetWindSpeedScale()
	{
		float num = Time.time / 600f;
		float num2 = base.transform.position.x / 512f;
		float num3 = base.transform.position.z / 512f;
		float num4 = Mathf.PerlinNoise(num2 + num, num3 + num * 0.1f);
		float height = TerrainMeta.HeightMap.GetHeight(base.transform.position);
		float num5 = base.transform.position.y - height;
		if (num5 < 0f)
		{
			num5 = 0f;
		}
		return Mathf.Clamp01(Mathf.InverseLerp(0f, 50f, num5) * 0.5f + num4);
	}

	// Token: 0x060015D1 RID: 5585 RVA: 0x000A7AFF File Offset: 0x000A5CFF
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
	}

	// Token: 0x060015D2 RID: 5586 RVA: 0x000A7B08 File Offset: 0x000A5D08
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRandomized(new Action(this.WindUpdate), 1f, 20f, 2f);
	}

	// Token: 0x060015D3 RID: 5587 RVA: 0x000A7B34 File Offset: 0x000A5D34
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			if (info.msg.ioEntity == null)
			{
				info.msg.ioEntity = Pool.Get<ProtoBuf.IOEntity>();
			}
			info.msg.ioEntity.genericFloat1 = Time.time;
			info.msg.ioEntity.genericFloat2 = this.serverWindSpeed;
		}
	}

	// Token: 0x060015D4 RID: 5588 RVA: 0x000A7B98 File Offset: 0x000A5D98
	public bool AmIVisible()
	{
		int num = 15;
		Vector3 a = base.transform.position + Vector3.up * 6f;
		if (!base.IsVisible(a + base.transform.up * (float)num, (float)(num + 1)))
		{
			return false;
		}
		Vector3 windAimDir = this.GetWindAimDir(Time.time);
		return base.IsVisible(a + windAimDir * (float)num, (float)(num + 1));
	}

	// Token: 0x060015D5 RID: 5589 RVA: 0x000A7C18 File Offset: 0x000A5E18
	public void WindUpdate()
	{
		this.serverWindSpeed = this.GetWindSpeedScale();
		if (!this.AmIVisible())
		{
			this.serverWindSpeed = 0f;
		}
		int num = Mathf.FloorToInt((float)this.maxPowerGeneration * this.serverWindSpeed);
		bool flag = this.currentEnergy != num;
		this.currentEnergy = num;
		if (flag)
		{
			this.MarkDirty();
		}
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x060015D6 RID: 5590 RVA: 0x000620BF File Offset: 0x000602BF
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (outputSlot != 0)
		{
			return 0;
		}
		return this.currentEnergy;
	}

	// Token: 0x060015D7 RID: 5591 RVA: 0x000A7C7C File Offset: 0x000A5E7C
	public Vector3 GetWindAimDir(float time)
	{
		float num = time / 3600f * 360f;
		int num2 = 10;
		Vector3 vector = new Vector3(Mathf.Sin(num * 0.017453292f) * (float)num2, 0f, Mathf.Cos(num * 0.017453292f) * (float)num2);
		return vector.normalized;
	}

	// Token: 0x04000E48 RID: 3656
	public Animator animator;

	// Token: 0x04000E49 RID: 3657
	public int maxPowerGeneration = 100;

	// Token: 0x04000E4A RID: 3658
	public Transform vaneRot;

	// Token: 0x04000E4B RID: 3659
	public SoundDefinition wooshSound;

	// Token: 0x04000E4C RID: 3660
	public Transform wooshOrigin;

	// Token: 0x04000E4D RID: 3661
	public float targetSpeed;

	// Token: 0x04000E4E RID: 3662
	private float serverWindSpeed;
}
