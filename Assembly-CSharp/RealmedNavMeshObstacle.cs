using System;
using Rust.Ai;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020001F8 RID: 504
public class RealmedNavMeshObstacle : BasePrefab
{
	// Token: 0x06001A36 RID: 6710 RVA: 0x000BA880 File Offset: 0x000B8A80
	public override void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (bundling)
		{
			return;
		}
		base.PreProcess(process, rootObj, name, serverside, clientside, false);
		if (base.isServer && this.Obstacle)
		{
			if (AiManager.nav_disable)
			{
				process.RemoveComponent(this.Obstacle);
				this.Obstacle = null;
			}
			else if (AiManager.nav_obstacles_carve_state >= 2)
			{
				this.Obstacle.carving = true;
			}
			else if (AiManager.nav_obstacles_carve_state == 1)
			{
				this.Obstacle.carving = (this.Obstacle.gameObject.layer == 21);
			}
			else
			{
				this.Obstacle.carving = false;
			}
		}
		process.RemoveComponent(this);
	}

	// Token: 0x04001290 RID: 4752
	public NavMeshObstacle Obstacle;
}
