using System;
using UnityEngine;

// Token: 0x02000498 RID: 1176
public class VehicleTerrainHandler
{
	// Token: 0x1700030C RID: 780
	// (get) Token: 0x06002634 RID: 9780 RVA: 0x000EE218 File Offset: 0x000EC418
	public bool IsOnSnowOrIce
	{
		get
		{
			return this.OnSurface == VehicleTerrainHandler.Surface.Snow || this.OnSurface == VehicleTerrainHandler.Surface.Ice;
		}
	}

	// Token: 0x06002635 RID: 9781 RVA: 0x000EE230 File Offset: 0x000EC430
	public VehicleTerrainHandler(BaseVehicle vehicle)
	{
		this.vehicle = vehicle;
	}

	// Token: 0x06002636 RID: 9782 RVA: 0x000EE289 File Offset: 0x000EC489
	public void FixedUpdate()
	{
		if (!this.vehicle.IsStationary() && this.timeSinceTerrainCheck > 0.25f)
		{
			this.DoTerrainCheck();
		}
	}

	// Token: 0x06002637 RID: 9783 RVA: 0x000EE2B0 File Offset: 0x000EC4B0
	private void DoTerrainCheck()
	{
		this.timeSinceTerrainCheck = UnityEngine.Random.Range(-0.025f, 0.025f);
		Transform transform = this.vehicle.transform;
		RaycastHit raycastHit;
		if (Physics.Raycast(transform.position + transform.up * 0.5f, -transform.up, out raycastHit, this.RayLength, 27328513, QueryTriggerInteraction.Ignore))
		{
			this.CurGroundPhysicsMatName = raycastHit.collider.GetMaterialAt(raycastHit.point).GetNameLower();
			if (this.GetOnRoad(this.CurGroundPhysicsMatName))
			{
				this.OnSurface = VehicleTerrainHandler.Surface.Road;
			}
			else if (this.CurGroundPhysicsMatName == "snow")
			{
				if (raycastHit.collider.CompareTag("TreatSnowAsIce"))
				{
					this.OnSurface = VehicleTerrainHandler.Surface.Ice;
				}
				else
				{
					this.OnSurface = VehicleTerrainHandler.Surface.Snow;
				}
			}
			else if (this.CurGroundPhysicsMatName == "sand")
			{
				this.OnSurface = VehicleTerrainHandler.Surface.Sand;
			}
			else if (this.CurGroundPhysicsMatName.Contains("zero friction"))
			{
				this.OnSurface = VehicleTerrainHandler.Surface.Frictionless;
			}
			else
			{
				this.OnSurface = VehicleTerrainHandler.Surface.Default;
			}
			this.IsGrounded = true;
			return;
		}
		this.CurGroundPhysicsMatName = "concrete";
		this.OnSurface = VehicleTerrainHandler.Surface.Default;
		this.IsGrounded = false;
	}

	// Token: 0x06002638 RID: 9784 RVA: 0x000EE3EC File Offset: 0x000EC5EC
	private bool GetOnRoad(string physicMat)
	{
		for (int i = 0; i < this.TerrainRoad.Length; i++)
		{
			if (this.TerrainRoad[i] == physicMat)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04001F06 RID: 7942
	public string CurGroundPhysicsMatName;

	// Token: 0x04001F07 RID: 7943
	public VehicleTerrainHandler.Surface OnSurface;

	// Token: 0x04001F08 RID: 7944
	public bool IsGrounded;

	// Token: 0x04001F09 RID: 7945
	public float RayLength = 1.5f;

	// Token: 0x04001F0A RID: 7946
	private readonly string[] TerrainRoad = new string[]
	{
		"rock",
		"concrete",
		"gravel",
		"metal",
		"path"
	};

	// Token: 0x04001F0B RID: 7947
	private const float SECONDS_BETWEEN_TERRAIN_SAMPLE = 0.25f;

	// Token: 0x04001F0C RID: 7948
	private TimeSince timeSinceTerrainCheck;

	// Token: 0x04001F0D RID: 7949
	private readonly BaseVehicle vehicle;

	// Token: 0x02000CC5 RID: 3269
	public enum Surface
	{
		// Token: 0x040043BD RID: 17341
		Default,
		// Token: 0x040043BE RID: 17342
		Road,
		// Token: 0x040043BF RID: 17343
		Snow,
		// Token: 0x040043C0 RID: 17344
		Ice,
		// Token: 0x040043C1 RID: 17345
		Sand,
		// Token: 0x040043C2 RID: 17346
		Frictionless
	}
}
