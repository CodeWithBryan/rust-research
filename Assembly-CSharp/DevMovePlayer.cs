using System;
using UnityEngine;

// Token: 0x02000301 RID: 769
public class DevMovePlayer : BaseMonoBehaviour
{
	// Token: 0x06001D99 RID: 7577 RVA: 0x000CA418 File Offset: 0x000C8618
	public void Awake()
	{
		this.randRun = UnityEngine.Random.Range(5f, 10f);
		this.player = base.GetComponent<BasePlayer>();
		if (this.Waypoints.Length != 0)
		{
			this.destination = this.Waypoints[0].position;
		}
		else
		{
			this.destination = base.transform.position;
		}
		if (this.player.isClient)
		{
			return;
		}
		if (this.player.eyes == null)
		{
			this.player.eyes = this.player.GetComponent<PlayerEyes>();
		}
		base.Invoke(new Action(this.LateSpawn), 1f);
	}

	// Token: 0x06001D9A RID: 7578 RVA: 0x000CA4C4 File Offset: 0x000C86C4
	public void LateSpawn()
	{
		Item item = ItemManager.CreateByName("rifle.semiauto", 1, 0UL);
		this.player.inventory.GiveItem(item, this.player.inventory.containerBelt);
		this.player.UpdateActiveItem(item.uid);
		this.player.health = 100f;
	}

	// Token: 0x06001D9B RID: 7579 RVA: 0x000CA522 File Offset: 0x000C8722
	public void SetWaypoints(Transform[] wps)
	{
		this.Waypoints = wps;
		this.destination = wps[0].position;
	}

	// Token: 0x06001D9C RID: 7580 RVA: 0x000CA53C File Offset: 0x000C873C
	public void Update()
	{
		if (this.player.isClient)
		{
			return;
		}
		if (!this.player.IsAlive() || this.player.IsWounded())
		{
			return;
		}
		if (Vector3.Distance(this.destination, base.transform.position) < 0.25f)
		{
			if (this.moveRandomly)
			{
				this.waypointIndex = UnityEngine.Random.Range(0, this.Waypoints.Length);
			}
			else
			{
				this.waypointIndex++;
			}
			if (this.waypointIndex >= this.Waypoints.Length)
			{
				this.waypointIndex = 0;
			}
		}
		if (this.Waypoints.Length <= this.waypointIndex)
		{
			return;
		}
		this.destination = this.Waypoints[this.waypointIndex].position;
		Vector3 normalized = (this.destination - base.transform.position).normalized;
		float running = Mathf.Sin(Time.time + this.randRun);
		float speed = this.player.GetSpeed(running, 0f, 0f);
		Vector3 position = base.transform.position;
		float range = 1f;
		LayerMask mask = 1537286401;
		RaycastHit raycastHit;
		if (TransformUtil.GetGroundInfo(base.transform.position + normalized * speed * Time.deltaTime, out raycastHit, range, mask, this.player.transform))
		{
			position = raycastHit.point;
		}
		base.transform.position = position;
		Vector3 normalized2 = (new Vector3(this.destination.x, 0f, this.destination.z) - new Vector3(this.player.transform.position.x, 0f, this.player.transform.position.z)).normalized;
		this.player.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x04001702 RID: 5890
	public BasePlayer player;

	// Token: 0x04001703 RID: 5891
	public Transform[] Waypoints;

	// Token: 0x04001704 RID: 5892
	public bool moveRandomly;

	// Token: 0x04001705 RID: 5893
	public Vector3 destination = Vector3.zero;

	// Token: 0x04001706 RID: 5894
	public Vector3 lookPoint = Vector3.zero;

	// Token: 0x04001707 RID: 5895
	private int waypointIndex;

	// Token: 0x04001708 RID: 5896
	private float randRun;
}
