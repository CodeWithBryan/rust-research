using System;
using System.Linq;
using ConVar;
using UnityEngine;

// Token: 0x0200092B RID: 2347
public class Poolable : MonoBehaviour, IClientComponent, IPrefabPostProcess
{
	// Token: 0x17000437 RID: 1079
	// (get) Token: 0x060037EB RID: 14315 RVA: 0x0014B3FC File Offset: 0x001495FC
	public int ClientCount
	{
		get
		{
			if (base.GetComponent<LootPanel>() != null)
			{
				return 1;
			}
			if (base.GetComponent<DecorComponent>() != null)
			{
				return 100;
			}
			if (base.GetComponent<BuildingBlock>() != null)
			{
				return 100;
			}
			if (base.GetComponent<Door>() != null)
			{
				return 100;
			}
			if (base.GetComponent<Projectile>() != null)
			{
				return 100;
			}
			if (base.GetComponent<Gib>() != null)
			{
				return 100;
			}
			return 10;
		}
	}

	// Token: 0x17000438 RID: 1080
	// (get) Token: 0x060037EC RID: 14316 RVA: 0x00007074 File Offset: 0x00005274
	public int ServerCount
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x060037ED RID: 14317 RVA: 0x0014B470 File Offset: 0x00149670
	public void PostProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (bundling)
		{
			return;
		}
		this.Initialize(StringPool.Get(name));
	}

	// Token: 0x060037EE RID: 14318 RVA: 0x0014B484 File Offset: 0x00149684
	public void Initialize(uint id)
	{
		this.prefabID = id;
		this.behaviours = base.gameObject.GetComponentsInChildren(typeof(Behaviour), true).OfType<Behaviour>().ToArray<Behaviour>();
		this.rigidbodies = base.gameObject.GetComponentsInChildren<Rigidbody>(true);
		this.colliders = base.gameObject.GetComponentsInChildren<Collider>(true);
		this.lodgroups = base.gameObject.GetComponentsInChildren<LODGroup>(true);
		this.renderers = base.gameObject.GetComponentsInChildren<Renderer>(true);
		this.particles = base.gameObject.GetComponentsInChildren<ParticleSystem>(true);
		if (this.behaviours.Length == 0)
		{
			this.behaviours = Array.Empty<Behaviour>();
		}
		if (this.rigidbodies.Length == 0)
		{
			this.rigidbodies = Array.Empty<Rigidbody>();
		}
		if (this.colliders.Length == 0)
		{
			this.colliders = Array.Empty<Collider>();
		}
		if (this.lodgroups.Length == 0)
		{
			this.lodgroups = Array.Empty<LODGroup>();
		}
		if (this.renderers.Length == 0)
		{
			this.renderers = Array.Empty<Renderer>();
		}
		if (this.particles.Length == 0)
		{
			this.particles = Array.Empty<ParticleSystem>();
		}
		this.behaviourStates = ArrayEx.New<bool>(this.behaviours.Length);
		this.rigidbodyStates = ArrayEx.New<bool>(this.rigidbodies.Length);
		this.colliderStates = ArrayEx.New<bool>(this.colliders.Length);
		this.lodgroupStates = ArrayEx.New<bool>(this.lodgroups.Length);
		this.rendererStates = ArrayEx.New<bool>(this.renderers.Length);
	}

	// Token: 0x060037EF RID: 14319 RVA: 0x0014B5F0 File Offset: 0x001497F0
	public void EnterPool()
	{
		if (base.transform.parent != null)
		{
			base.transform.SetParent(null, false);
		}
		if (Pool.mode <= 1)
		{
			if (base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(false);
				return;
			}
		}
		else
		{
			this.SetBehaviourEnabled(false);
			this.SetComponentEnabled(false);
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x060037F0 RID: 14320 RVA: 0x0014B666 File Offset: 0x00149866
	public void LeavePool()
	{
		if (Pool.mode > 1)
		{
			this.SetComponentEnabled(true);
		}
	}

	// Token: 0x060037F1 RID: 14321 RVA: 0x0014B678 File Offset: 0x00149878
	public void SetBehaviourEnabled(bool state)
	{
		try
		{
			if (!state)
			{
				for (int i = 0; i < this.behaviours.Length; i++)
				{
					Behaviour behaviour = this.behaviours[i];
					this.behaviourStates[i] = behaviour.enabled;
					behaviour.enabled = false;
				}
				for (int j = 0; j < this.particles.Length; j++)
				{
					ParticleSystem particleSystem = this.particles[j];
					particleSystem.Stop();
					particleSystem.Clear();
				}
			}
			else
			{
				for (int k = 0; k < this.particles.Length; k++)
				{
					ParticleSystem particleSystem2 = this.particles[k];
					if (particleSystem2.playOnAwake)
					{
						particleSystem2.Play();
					}
				}
				for (int l = 0; l < this.behaviours.Length; l++)
				{
					this.behaviours[l].enabled = this.behaviourStates[l];
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Pooling error: ",
				base.name,
				" (",
				ex.Message,
				")"
			}));
		}
	}

	// Token: 0x060037F2 RID: 14322 RVA: 0x0014B790 File Offset: 0x00149990
	public void SetComponentEnabled(bool state)
	{
		try
		{
			if (!state)
			{
				for (int i = 0; i < this.renderers.Length; i++)
				{
					Renderer renderer = this.renderers[i];
					this.rendererStates[i] = renderer.enabled;
					renderer.enabled = false;
				}
				for (int j = 0; j < this.lodgroups.Length; j++)
				{
					LODGroup lodgroup = this.lodgroups[j];
					this.lodgroupStates[j] = lodgroup.enabled;
					lodgroup.enabled = false;
				}
				for (int k = 0; k < this.colliders.Length; k++)
				{
					Collider collider = this.colliders[k];
					this.colliderStates[k] = collider.enabled;
					collider.enabled = false;
				}
				for (int l = 0; l < this.rigidbodies.Length; l++)
				{
					Rigidbody rigidbody = this.rigidbodies[l];
					this.rigidbodyStates[l] = rigidbody.isKinematic;
					rigidbody.isKinematic = true;
					rigidbody.detectCollisions = false;
				}
			}
			else
			{
				for (int m = 0; m < this.renderers.Length; m++)
				{
					this.renderers[m].enabled = this.rendererStates[m];
				}
				for (int n = 0; n < this.lodgroups.Length; n++)
				{
					this.lodgroups[n].enabled = this.lodgroupStates[n];
				}
				for (int num = 0; num < this.colliders.Length; num++)
				{
					this.colliders[num].enabled = this.colliderStates[num];
				}
				for (int num2 = 0; num2 < this.rigidbodies.Length; num2++)
				{
					Rigidbody rigidbody2 = this.rigidbodies[num2];
					rigidbody2.isKinematic = this.rigidbodyStates[num2];
					rigidbody2.detectCollisions = true;
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Pooling error: ",
				base.name,
				" (",
				ex.Message,
				")"
			}));
		}
	}

	// Token: 0x0400320D RID: 12813
	[HideInInspector]
	public uint prefabID;

	// Token: 0x0400320E RID: 12814
	[HideInInspector]
	public Behaviour[] behaviours;

	// Token: 0x0400320F RID: 12815
	[HideInInspector]
	public Rigidbody[] rigidbodies;

	// Token: 0x04003210 RID: 12816
	[HideInInspector]
	public Collider[] colliders;

	// Token: 0x04003211 RID: 12817
	[HideInInspector]
	public LODGroup[] lodgroups;

	// Token: 0x04003212 RID: 12818
	[HideInInspector]
	public Renderer[] renderers;

	// Token: 0x04003213 RID: 12819
	[HideInInspector]
	public ParticleSystem[] particles;

	// Token: 0x04003214 RID: 12820
	[HideInInspector]
	public bool[] behaviourStates;

	// Token: 0x04003215 RID: 12821
	[HideInInspector]
	public bool[] rigidbodyStates;

	// Token: 0x04003216 RID: 12822
	[HideInInspector]
	public bool[] colliderStates;

	// Token: 0x04003217 RID: 12823
	[HideInInspector]
	public bool[] lodgroupStates;

	// Token: 0x04003218 RID: 12824
	[HideInInspector]
	public bool[] rendererStates;
}
