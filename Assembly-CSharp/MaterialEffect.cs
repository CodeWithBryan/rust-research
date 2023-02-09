using System;
using UnityEngine;

// Token: 0x020005DF RID: 1503
[CreateAssetMenu(menuName = "Rust/MaterialEffect")]
public class MaterialEffect : ScriptableObject
{
	// Token: 0x06002C2D RID: 11309 RVA: 0x001090C8 File Offset: 0x001072C8
	public MaterialEffect.Entry GetEntryFromMaterial(PhysicMaterial mat)
	{
		foreach (MaterialEffect.Entry entry in this.Entries)
		{
			if (entry.Material == mat)
			{
				return entry;
			}
		}
		return null;
	}

	// Token: 0x06002C2E RID: 11310 RVA: 0x00109100 File Offset: 0x00107300
	public MaterialEffect.Entry GetWaterEntry()
	{
		if (this.waterFootstepIndex == -1)
		{
			for (int i = 0; i < this.Entries.Length; i++)
			{
				if (this.Entries[i].Material.name == "Water")
				{
					this.waterFootstepIndex = i;
					break;
				}
			}
		}
		if (this.waterFootstepIndex != -1)
		{
			return this.Entries[this.waterFootstepIndex];
		}
		Debug.LogWarning("Unable to find water effect for :" + base.name);
		return null;
	}

	// Token: 0x06002C2F RID: 11311 RVA: 0x00109180 File Offset: 0x00107380
	public void SpawnOnRay(Ray ray, int mask, float length = 0.5f, Vector3 forward = default(Vector3), float speed = 0f)
	{
		RaycastHit raycastHit;
		if (!GamePhysics.Trace(ray, 0f, out raycastHit, length, mask, QueryTriggerInteraction.UseGlobal, null))
		{
			Effect.client.Run(this.DefaultEffect.resourcePath, ray.origin, ray.direction * -1f, forward, Effect.Type.Generic);
			if (this.DefaultSoundDefinition != null)
			{
				this.PlaySound(this.DefaultSoundDefinition, raycastHit.point, speed);
			}
			return;
		}
		WaterLevel.WaterInfo waterInfo = WaterLevel.GetWaterInfo(ray.origin, true, null, false);
		if (!waterInfo.isValid)
		{
			PhysicMaterial materialAt = raycastHit.collider.GetMaterialAt(raycastHit.point);
			MaterialEffect.Entry entryFromMaterial = this.GetEntryFromMaterial(materialAt);
			if (entryFromMaterial == null)
			{
				Effect.client.Run(this.DefaultEffect.resourcePath, raycastHit.point, raycastHit.normal, forward, Effect.Type.Generic);
				if (this.DefaultSoundDefinition != null)
				{
					this.PlaySound(this.DefaultSoundDefinition, raycastHit.point, speed);
					return;
				}
			}
			else
			{
				Effect.client.Run(entryFromMaterial.Effect.resourcePath, raycastHit.point, raycastHit.normal, forward, Effect.Type.Generic);
				if (entryFromMaterial.SoundDefinition != null)
				{
					this.PlaySound(entryFromMaterial.SoundDefinition, raycastHit.point, speed);
				}
			}
			return;
		}
		Vector3 vector = new Vector3(ray.origin.x, WaterSystem.GetHeight(ray.origin), ray.origin.z);
		MaterialEffect.Entry waterEntry = this.GetWaterEntry();
		if (this.submergedWaterDepth > 0f && waterInfo.currentDepth >= this.submergedWaterDepth)
		{
			waterEntry = this.submergedWaterEntry;
		}
		else if (this.deepWaterDepth > 0f && waterInfo.currentDepth >= this.deepWaterDepth)
		{
			waterEntry = this.deepWaterEntry;
		}
		if (waterEntry == null)
		{
			return;
		}
		Effect.client.Run(waterEntry.Effect.resourcePath, vector, Vector3.up, default(Vector3), Effect.Type.Generic);
		if (waterEntry.SoundDefinition != null)
		{
			this.PlaySound(waterEntry.SoundDefinition, vector, speed);
		}
	}

	// Token: 0x06002C30 RID: 11312 RVA: 0x000059DD File Offset: 0x00003BDD
	public void PlaySound(SoundDefinition definition, Vector3 position, float velocity = 0f)
	{
	}

	// Token: 0x04002400 RID: 9216
	public GameObjectRef DefaultEffect;

	// Token: 0x04002401 RID: 9217
	public SoundDefinition DefaultSoundDefinition;

	// Token: 0x04002402 RID: 9218
	public MaterialEffect.Entry[] Entries;

	// Token: 0x04002403 RID: 9219
	public int waterFootstepIndex = -1;

	// Token: 0x04002404 RID: 9220
	public MaterialEffect.Entry deepWaterEntry;

	// Token: 0x04002405 RID: 9221
	public float deepWaterDepth = -1f;

	// Token: 0x04002406 RID: 9222
	public MaterialEffect.Entry submergedWaterEntry;

	// Token: 0x04002407 RID: 9223
	public float submergedWaterDepth = -1f;

	// Token: 0x04002408 RID: 9224
	public bool ScaleVolumeWithSpeed;

	// Token: 0x04002409 RID: 9225
	public AnimationCurve SpeedGainCurve;

	// Token: 0x02000D2D RID: 3373
	[Serializable]
	public class Entry
	{
		// Token: 0x0400454B RID: 17739
		public PhysicMaterial Material;

		// Token: 0x0400454C RID: 17740
		public GameObjectRef Effect;

		// Token: 0x0400454D RID: 17741
		public SoundDefinition SoundDefinition;
	}
}
