using System;
using Network;
using UnityEngine;

// Token: 0x020000C1 RID: 193
public class SantaSleigh : BaseEntity
{
	// Token: 0x06001115 RID: 4373 RVA: 0x0008AB08 File Offset: 0x00088D08
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("SantaSleigh.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001116 RID: 4374 RVA: 0x000058B6 File Offset: 0x00003AB6
	public override float GetNetworkTime()
	{
		return Time.fixedTime;
	}

	// Token: 0x06001117 RID: 4375 RVA: 0x0008AB48 File Offset: 0x00088D48
	public void InitDropPosition(Vector3 newDropPosition)
	{
		this.dropPosition = newDropPosition;
		this.dropPosition.y = 0f;
	}

	// Token: 0x06001118 RID: 4376 RVA: 0x0008AB64 File Offset: 0x00088D64
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.dropPosition == Vector3.zero)
		{
			this.dropPosition = this.RandomDropPosition();
		}
		this.UpdateDropPosition(this.dropPosition);
		base.Invoke(new Action(this.SendHoHoHo), 0f);
	}

	// Token: 0x06001119 RID: 4377 RVA: 0x0008ABB8 File Offset: 0x00088DB8
	public void SendHoHoHo()
	{
		base.Invoke(new Action(this.SendHoHoHo), this.hohohospacing + UnityEngine.Random.Range(0f, this.hohoho_additional_spacing));
		base.ClientRPC(null, "ClientPlayHoHoHo");
	}

	// Token: 0x0600111A RID: 4378 RVA: 0x0008ABF0 File Offset: 0x00088DF0
	public Vector3 RandomDropPosition()
	{
		Vector3 vector = Vector3.zero;
		float num = 100f;
		float x = TerrainMeta.Size.x;
		do
		{
			vector = Vector3Ex.Range(-(x / 3f), x / 3f);
		}
		while (this.filter.GetFactor(vector, true) == 0f && (num -= 1f) > 0f);
		vector.y = 0f;
		return vector;
	}

	// Token: 0x0600111B RID: 4379 RVA: 0x0008AC5C File Offset: 0x00088E5C
	public void UpdateDropPosition(Vector3 newDropPosition)
	{
		float x = TerrainMeta.Size.x;
		float y = SantaSleigh.altitudeAboveTerrain;
		this.startPos = Vector3Ex.Range(-1f, 1f);
		this.startPos.y = 0f;
		this.startPos.Normalize();
		this.startPos *= x * 1.25f;
		this.startPos.y = y;
		this.endPos = this.startPos * -1f;
		this.endPos.y = this.startPos.y;
		this.startPos += newDropPosition;
		this.endPos += newDropPosition;
		this.secondsToTake = Vector3.Distance(this.startPos, this.endPos) / 25f;
		this.secondsToTake *= UnityEngine.Random.Range(0.95f, 1.05f);
		base.transform.SetPositionAndRotation(this.startPos, Quaternion.LookRotation(this.endPos - this.startPos));
		this.dropPosition = newDropPosition;
	}

	// Token: 0x0600111C RID: 4380 RVA: 0x0008AD88 File Offset: 0x00088F88
	private void FixedUpdate()
	{
		if (!base.isServer)
		{
			return;
		}
		Vector3 vector = base.transform.position;
		Quaternion rotation = base.transform.rotation;
		this.secondsTaken += Time.deltaTime;
		float num = Mathf.InverseLerp(0f, this.secondsToTake, this.secondsTaken);
		if (!this.dropped && num >= 0.5f)
		{
			this.dropped = true;
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.prefabDrop.resourcePath, this.dropOrigin.transform.position, default(Quaternion), true);
			if (baseEntity)
			{
				baseEntity.globalBroadcast = true;
				baseEntity.Spawn();
			}
		}
		vector = Vector3.Lerp(this.startPos, this.endPos, num);
		Vector3 normalized = (this.endPos - this.startPos).normalized;
		Vector3 vector2 = Vector3.zero;
		if (this.swimScale != Vector3.zero)
		{
			if (this.swimRandom == 0f)
			{
				this.swimRandom = UnityEngine.Random.Range(0f, 20f);
			}
			float num2 = Time.time + this.swimRandom;
			vector2 = new Vector3(Mathf.Sin(num2 * this.swimSpeed.x) * this.swimScale.x, Mathf.Cos(num2 * this.swimSpeed.y) * this.swimScale.y, Mathf.Sin(num2 * this.swimSpeed.z) * this.swimScale.z);
			vector2 = base.transform.InverseTransformDirection(vector2);
			vector += vector2 * this.appliedSwimScale;
		}
		rotation = Quaternion.LookRotation(normalized) * Quaternion.Euler(Mathf.Cos(Time.time * this.swimSpeed.y) * this.appliedSwimRotation, 0f, Mathf.Sin(Time.time * this.swimSpeed.x) * this.appliedSwimRotation);
		Vector3 vector3 = vector;
		float height = TerrainMeta.HeightMap.GetHeight(vector3 + base.transform.forward * 30f);
		float height2 = TerrainMeta.HeightMap.GetHeight(vector3);
		float num3 = Mathf.Max(height, height2);
		float b = Mathf.Max(SantaSleigh.desiredAltitude, num3 + SantaSleigh.altitudeAboveTerrain);
		vector3.y = Mathf.Lerp(base.transform.position.y, b, Time.fixedDeltaTime * 0.5f);
		vector = vector3;
		base.transform.hasChanged = true;
		if (num >= 1f)
		{
			base.Kill(BaseNetworkable.DestroyMode.None);
		}
		base.transform.SetPositionAndRotation(vector, rotation);
	}

	// Token: 0x0600111D RID: 4381 RVA: 0x0008B034 File Offset: 0x00089234
	[ServerVar]
	public static void drop(ConsoleSystem.Arg arg)
	{
		BasePlayer basePlayer = arg.Player();
		if (!basePlayer)
		{
			return;
		}
		Debug.Log("Santa Inbound");
		BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/misc/xmas/sleigh/santasleigh.prefab", default(Vector3), default(Quaternion), true);
		if (baseEntity)
		{
			baseEntity.GetComponent<SantaSleigh>().InitDropPosition(basePlayer.transform.position + new Vector3(0f, 10f, 0f));
			baseEntity.Spawn();
		}
	}

	// Token: 0x04000AB0 RID: 2736
	public GameObjectRef prefabDrop;

	// Token: 0x04000AB1 RID: 2737
	public SpawnFilter filter;

	// Token: 0x04000AB2 RID: 2738
	public Transform dropOrigin;

	// Token: 0x04000AB3 RID: 2739
	[ServerVar]
	public static float altitudeAboveTerrain = 50f;

	// Token: 0x04000AB4 RID: 2740
	[ServerVar]
	public static float desiredAltitude = 60f;

	// Token: 0x04000AB5 RID: 2741
	public Light bigLight;

	// Token: 0x04000AB6 RID: 2742
	public SoundPlayer hohoho;

	// Token: 0x04000AB7 RID: 2743
	public float hohohospacing = 4f;

	// Token: 0x04000AB8 RID: 2744
	public float hohoho_additional_spacing = 2f;

	// Token: 0x04000AB9 RID: 2745
	private Vector3 startPos;

	// Token: 0x04000ABA RID: 2746
	private Vector3 endPos;

	// Token: 0x04000ABB RID: 2747
	private float secondsToTake;

	// Token: 0x04000ABC RID: 2748
	private float secondsTaken;

	// Token: 0x04000ABD RID: 2749
	private bool dropped;

	// Token: 0x04000ABE RID: 2750
	private Vector3 dropPosition = Vector3.zero;

	// Token: 0x04000ABF RID: 2751
	public Vector3 swimScale;

	// Token: 0x04000AC0 RID: 2752
	public Vector3 swimSpeed;

	// Token: 0x04000AC1 RID: 2753
	private float swimRandom;

	// Token: 0x04000AC2 RID: 2754
	public float appliedSwimScale = 1f;

	// Token: 0x04000AC3 RID: 2755
	public float appliedSwimRotation = 20f;

	// Token: 0x04000AC4 RID: 2756
	private const string path = "assets/prefabs/misc/xmas/sleigh/santasleigh.prefab";
}
