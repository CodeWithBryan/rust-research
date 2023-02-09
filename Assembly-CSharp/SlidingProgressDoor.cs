using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x020004B4 RID: 1204
public class SlidingProgressDoor : ProgressDoor
{
	// Token: 0x060026CB RID: 9931 RVA: 0x000EFD00 File Offset: 0x000EDF00
	public override void Spawn()
	{
		base.Spawn();
		base.InvokeRepeating(new Action(this.ServerUpdate), 0f, 0.1f);
		if (this.vehiclePhysBox != null)
		{
			this.vehiclePhysBox.gameObject.SetActive(false);
		}
	}

	// Token: 0x060026CC RID: 9932 RVA: 0x000EFD4E File Offset: 0x000EDF4E
	public override void NoEnergy()
	{
		base.NoEnergy();
	}

	// Token: 0x060026CD RID: 9933 RVA: 0x000EFD56 File Offset: 0x000EDF56
	public override void AddEnergy(float amount)
	{
		this.lastEnergyTime = Time.time;
		base.AddEnergy(amount);
	}

	// Token: 0x060026CE RID: 9934 RVA: 0x000EFD6C File Offset: 0x000EDF6C
	public void ServerUpdate()
	{
		if (base.isServer)
		{
			if (this.lastServerUpdateTime == 0f)
			{
				this.lastServerUpdateTime = Time.realtimeSinceStartup;
			}
			float num = Time.realtimeSinceStartup - this.lastServerUpdateTime;
			this.lastServerUpdateTime = Time.realtimeSinceStartup;
			if (Time.time > this.lastEnergyTime + 0.333f)
			{
				float b = this.energyForOpen * num / this.secondsToClose;
				float num2 = Mathf.Min(this.storedEnergy, b);
				if (this.vehiclePhysBox != null)
				{
					this.vehiclePhysBox.gameObject.SetActive(num2 > 0f && this.storedEnergy > 0f);
					if (this.vehiclePhysBox.gameObject.activeSelf && this.vehiclePhysBox.ContentsCount > 0)
					{
						num2 = 0f;
					}
				}
				this.storedEnergy -= num2;
				this.storedEnergy = Mathf.Clamp(this.storedEnergy, 0f, this.energyForOpen);
				if (num2 > 0f)
				{
					foreach (global::IOEntity.IOSlot ioslot in this.outputs)
					{
						if (ioslot.connectedTo.Get(true) != null)
						{
							ioslot.connectedTo.Get(true).IOInput(this, this.ioType, -num2, ioslot.connectedToSlot);
						}
					}
				}
			}
			this.UpdateProgress();
		}
	}

	// Token: 0x060026CF RID: 9935 RVA: 0x000EFED4 File Offset: 0x000EE0D4
	public override void UpdateProgress()
	{
		Vector3 localPosition = this.doorObject.transform.localPosition;
		float t = this.storedEnergy / this.energyForOpen;
		Vector3 vector = Vector3.Lerp(this.closedPosition, this.openPosition, t);
		this.doorObject.transform.localPosition = vector;
		if (base.isServer)
		{
			bool flag = Vector3.Distance(localPosition, vector) > 0.01f;
			base.SetFlag(global::BaseEntity.Flags.Reserved1, flag, false, true);
			if (flag)
			{
				base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
			}
		}
	}

	// Token: 0x060026D0 RID: 9936 RVA: 0x000EFF53 File Offset: 0x000EE153
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		ProtoBuf.SphereEntity sphereEntity = info.msg.sphereEntity;
	}

	// Token: 0x060026D1 RID: 9937 RVA: 0x000EFF68 File Offset: 0x000EE168
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.sphereEntity = Pool.Get<ProtoBuf.SphereEntity>();
		info.msg.sphereEntity.radius = this.storedEnergy;
	}

	// Token: 0x04001F46 RID: 8006
	public Vector3 openPosition;

	// Token: 0x04001F47 RID: 8007
	public Vector3 closedPosition;

	// Token: 0x04001F48 RID: 8008
	public GameObject doorObject;

	// Token: 0x04001F49 RID: 8009
	public TriggerVehiclePush vehiclePhysBox;

	// Token: 0x04001F4A RID: 8010
	private float lastEnergyTime;

	// Token: 0x04001F4B RID: 8011
	private float lastServerUpdateTime;
}
