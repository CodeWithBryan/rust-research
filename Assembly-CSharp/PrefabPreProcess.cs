using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using Rust.UI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using VLB;

// Token: 0x0200052D RID: 1325
public class PrefabPreProcess : IPrefabProcessor
{
	// Token: 0x06002899 RID: 10393 RVA: 0x000F6EB0 File Offset: 0x000F50B0
	public PrefabPreProcess(bool clientside, bool serverside, bool bundling = false)
	{
		this.isClientside = clientside;
		this.isServerside = serverside;
		this.isBundling = bundling;
	}

	// Token: 0x0600289A RID: 10394 RVA: 0x000F6F00 File Offset: 0x000F5100
	public GameObject Find(string strPrefab)
	{
		GameObject gameObject;
		if (!this.prefabList.TryGetValue(strPrefab, out gameObject))
		{
			return null;
		}
		if (gameObject == null)
		{
			this.prefabList.Remove(strPrefab);
			return null;
		}
		return gameObject;
	}

	// Token: 0x0600289B RID: 10395 RVA: 0x000F6F38 File Offset: 0x000F5138
	public bool NeedsProcessing(GameObject go)
	{
		if (go.CompareTag("NoPreProcessing"))
		{
			return false;
		}
		if (this.HasComponents<IPrefabPreProcess>(go.transform))
		{
			return true;
		}
		if (this.HasComponents<IPrefabPostProcess>(go.transform))
		{
			return true;
		}
		if (this.HasComponents<IEditorComponent>(go.transform))
		{
			return true;
		}
		if (!this.isClientside)
		{
			if (PrefabPreProcess.clientsideOnlyTypes.Any((Type type) => this.HasComponents(go.transform, type)))
			{
				return true;
			}
			if (this.HasComponents<IClientComponentEx>(go.transform))
			{
				return true;
			}
		}
		if (!this.isServerside)
		{
			if (PrefabPreProcess.serversideOnlyTypes.Any((Type type) => this.HasComponents(go.transform, type)))
			{
				return true;
			}
			if (this.HasComponents<IServerComponentEx>(go.transform))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600289C RID: 10396 RVA: 0x000F701C File Offset: 0x000F521C
	public void ProcessObject(string name, GameObject go, bool resetLocalTransform = true)
	{
		if (!this.isClientside)
		{
			foreach (Type t in PrefabPreProcess.clientsideOnlyTypes)
			{
				this.DestroyComponents(t, go, this.isClientside, this.isServerside);
			}
			foreach (IClientComponentEx clientComponentEx in this.FindComponents<IClientComponentEx>(go.transform))
			{
				clientComponentEx.PreClientComponentCull(this);
			}
		}
		if (!this.isServerside)
		{
			foreach (Type t2 in PrefabPreProcess.serversideOnlyTypes)
			{
				this.DestroyComponents(t2, go, this.isClientside, this.isServerside);
			}
			foreach (IServerComponentEx serverComponentEx in this.FindComponents<IServerComponentEx>(go.transform))
			{
				serverComponentEx.PreServerComponentCull(this);
			}
		}
		this.DestroyComponents(typeof(IEditorComponent), go, this.isClientside, this.isServerside);
		if (resetLocalTransform)
		{
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
		}
		List<Transform> list = this.FindComponents<Transform>(go.transform);
		list.Reverse();
		MeshColliderCookingOptions meshColliderCookingOptions = MeshColliderCookingOptions.CookForFasterSimulation | MeshColliderCookingOptions.EnableMeshCleaning | MeshColliderCookingOptions.WeldColocatedVertices;
		MeshColliderCookingOptions cookingOptions = MeshColliderCookingOptions.CookForFasterSimulation | MeshColliderCookingOptions.EnableMeshCleaning | MeshColliderCookingOptions.WeldColocatedVertices | MeshColliderCookingOptions.UseFastMidphase;
		MeshColliderCookingOptions meshColliderCookingOptions2 = (MeshColliderCookingOptions)(-1);
		foreach (MeshCollider meshCollider in this.FindComponents<MeshCollider>(go.transform))
		{
			if (meshCollider.cookingOptions == meshColliderCookingOptions || meshCollider.cookingOptions == meshColliderCookingOptions2)
			{
				meshCollider.cookingOptions = cookingOptions;
			}
		}
		foreach (IPrefabPreProcess prefabPreProcess in this.FindComponents<IPrefabPreProcess>(go.transform))
		{
			prefabPreProcess.PreProcess(this, go, name, this.isServerside, this.isClientside, this.isBundling);
		}
		foreach (Transform transform in list)
		{
			if (transform && transform.gameObject)
			{
				if (this.isServerside && transform.gameObject.CompareTag("Server Cull"))
				{
					this.RemoveComponents(transform.gameObject);
					this.NominateForDeletion(transform.gameObject);
				}
				if (this.isClientside)
				{
					bool flag = transform.gameObject.CompareTag("Client Cull");
					bool flag2 = transform != go.transform && transform.gameObject.GetComponent<BaseEntity>() != null;
					if (flag || flag2)
					{
						this.RemoveComponents(transform.gameObject);
						this.NominateForDeletion(transform.gameObject);
					}
				}
			}
		}
		this.RunCleanupQueue();
		foreach (IPrefabPostProcess prefabPostProcess in this.FindComponents<IPrefabPostProcess>(go.transform))
		{
			prefabPostProcess.PostProcess(this, go, name, this.isServerside, this.isClientside, this.isBundling);
		}
	}

	// Token: 0x0600289D RID: 10397 RVA: 0x000F739C File Offset: 0x000F559C
	public void Process(string name, GameObject go)
	{
		if (!UnityEngine.Application.isPlaying)
		{
			return;
		}
		if (go.CompareTag("NoPreProcessing"))
		{
			return;
		}
		GameObject hierarchyGroup = this.GetHierarchyGroup();
		GameObject gameObject = go;
		go = Instantiate.GameObject(gameObject, hierarchyGroup.transform);
		go.name = gameObject.name;
		if (this.NeedsProcessing(go))
		{
			this.ProcessObject(name, go, true);
		}
		this.AddPrefab(name, go);
	}

	// Token: 0x0600289E RID: 10398 RVA: 0x000F73FC File Offset: 0x000F55FC
	public void Invalidate(string name)
	{
		GameObject gameObject;
		if (this.prefabList.TryGetValue(name, out gameObject))
		{
			this.prefabList.Remove(name);
			if (gameObject != null)
			{
				UnityEngine.Object.DestroyImmediate(gameObject, true);
			}
		}
	}

	// Token: 0x0600289F RID: 10399 RVA: 0x000F7436 File Offset: 0x000F5636
	public GameObject GetHierarchyGroup()
	{
		if (this.isClientside && this.isServerside)
		{
			return HierarchyUtil.GetRoot("PrefabPreProcess - Generic", false, true);
		}
		if (this.isServerside)
		{
			return HierarchyUtil.GetRoot("PrefabPreProcess - Server", false, true);
		}
		return HierarchyUtil.GetRoot("PrefabPreProcess - Client", false, true);
	}

	// Token: 0x060028A0 RID: 10400 RVA: 0x000F7476 File Offset: 0x000F5676
	public void AddPrefab(string name, GameObject go)
	{
		go.SetActive(false);
		this.prefabList.Add(name, go);
	}

	// Token: 0x060028A1 RID: 10401 RVA: 0x000F748C File Offset: 0x000F568C
	private void DestroyComponents(Type t, GameObject go, bool client, bool server)
	{
		List<Component> list = new List<Component>();
		this.FindComponents(go.transform, list, t);
		list.Reverse();
		foreach (Component component in list)
		{
			RealmedRemove component2 = component.GetComponent<RealmedRemove>();
			if (!(component2 != null) || component2.ShouldDelete(component, client, server))
			{
				if (!component.gameObject.CompareTag("persist"))
				{
					this.NominateForDeletion(component.gameObject);
				}
				UnityEngine.Object.DestroyImmediate(component, true);
			}
		}
	}

	// Token: 0x060028A2 RID: 10402 RVA: 0x000F7530 File Offset: 0x000F5730
	private bool ShouldExclude(Transform transform)
	{
		return transform.GetComponent<BaseEntity>() != null;
	}

	// Token: 0x060028A3 RID: 10403 RVA: 0x000F7544 File Offset: 0x000F5744
	private bool HasComponents<T>(Transform transform)
	{
		if (transform.GetComponent<T>() != null)
		{
			return true;
		}
		foreach (object obj in transform)
		{
			Transform transform2 = (Transform)obj;
			if (!this.ShouldExclude(transform2) && this.HasComponents<T>(transform2))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060028A4 RID: 10404 RVA: 0x000F75BC File Offset: 0x000F57BC
	private bool HasComponents(Transform transform, Type t)
	{
		if (transform.GetComponent(t) != null)
		{
			return true;
		}
		foreach (object obj in transform)
		{
			Transform transform2 = (Transform)obj;
			if (!this.ShouldExclude(transform2) && this.HasComponents(transform2, t))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060028A5 RID: 10405 RVA: 0x000F7634 File Offset: 0x000F5834
	public List<T> FindComponents<T>(Transform transform)
	{
		List<T> list = new List<T>();
		this.FindComponents<T>(transform, list);
		return list;
	}

	// Token: 0x060028A6 RID: 10406 RVA: 0x000F7650 File Offset: 0x000F5850
	public void FindComponents<T>(Transform transform, List<T> list)
	{
		list.AddRange(transform.GetComponents<T>());
		foreach (object obj in transform)
		{
			Transform transform2 = (Transform)obj;
			if (!this.ShouldExclude(transform2))
			{
				this.FindComponents<T>(transform2, list);
			}
		}
	}

	// Token: 0x060028A7 RID: 10407 RVA: 0x000F76BC File Offset: 0x000F58BC
	public List<Component> FindComponents(Transform transform, Type t)
	{
		List<Component> list = new List<Component>();
		this.FindComponents(transform, list, t);
		return list;
	}

	// Token: 0x060028A8 RID: 10408 RVA: 0x000F76DC File Offset: 0x000F58DC
	public void FindComponents(Transform transform, List<Component> list, Type t)
	{
		list.AddRange(transform.GetComponents(t));
		foreach (object obj in transform)
		{
			Transform transform2 = (Transform)obj;
			if (!this.ShouldExclude(transform2))
			{
				this.FindComponents(transform2, list, t);
			}
		}
	}

	// Token: 0x060028A9 RID: 10409 RVA: 0x000F7748 File Offset: 0x000F5948
	public void RemoveComponent(Component c)
	{
		if (c == null)
		{
			return;
		}
		this.destroyList.Add(c);
	}

	// Token: 0x060028AA RID: 10410 RVA: 0x000F7760 File Offset: 0x000F5960
	public void RemoveComponents(GameObject gameObj)
	{
		foreach (Component component in gameObj.GetComponents<Component>())
		{
			if (!(component is Transform))
			{
				this.destroyList.Add(component);
			}
		}
	}

	// Token: 0x060028AB RID: 10411 RVA: 0x000F779A File Offset: 0x000F599A
	public void NominateForDeletion(GameObject gameObj)
	{
		this.cleanupList.Add(gameObj);
	}

	// Token: 0x060028AC RID: 10412 RVA: 0x000F77A8 File Offset: 0x000F59A8
	private void RunCleanupQueue()
	{
		foreach (Component obj in this.destroyList)
		{
			UnityEngine.Object.DestroyImmediate(obj, true);
		}
		this.destroyList.Clear();
		foreach (GameObject go in this.cleanupList)
		{
			this.DoCleanup(go);
		}
		this.cleanupList.Clear();
	}

	// Token: 0x060028AD RID: 10413 RVA: 0x000F7854 File Offset: 0x000F5A54
	private void DoCleanup(GameObject go)
	{
		if (go == null)
		{
			return;
		}
		if (go.GetComponentsInChildren<Component>(true).Length > 1)
		{
			return;
		}
		Transform parent = go.transform.parent;
		if (parent == null)
		{
			return;
		}
		if (parent.name.StartsWith("PrefabPreProcess - "))
		{
			return;
		}
		UnityEngine.Object.DestroyImmediate(go, true);
	}

	// Token: 0x04002104 RID: 8452
	public static Type[] clientsideOnlyTypes = new Type[]
	{
		typeof(IClientComponent),
		typeof(SkeletonSkinLod),
		typeof(ImageEffectLayer),
		typeof(NGSS_Directional),
		typeof(VolumetricDustParticles),
		typeof(VolumetricLightBeam),
		typeof(Cloth),
		typeof(MeshFilter),
		typeof(Renderer),
		typeof(AudioLowPassFilter),
		typeof(AudioSource),
		typeof(AudioListener),
		typeof(ParticleSystemRenderer),
		typeof(ParticleSystem),
		typeof(ParticleEmitFromParentObject),
		typeof(ImpostorShadows),
		typeof(Light),
		typeof(LODGroup),
		typeof(Animator),
		typeof(AnimationEvents),
		typeof(PlayerVoiceSpeaker),
		typeof(VoiceProcessor),
		typeof(PlayerVoiceRecorder),
		typeof(ParticleScaler),
		typeof(PostEffectsBase),
		typeof(TOD_ImageEffect),
		typeof(TOD_Scattering),
		typeof(TOD_Rays),
		typeof(Tree),
		typeof(Projector),
		typeof(HttpImage),
		typeof(EventTrigger),
		typeof(StandaloneInputModule),
		typeof(UIBehaviour),
		typeof(Canvas),
		typeof(CanvasRenderer),
		typeof(CanvasGroup),
		typeof(GraphicRaycaster)
	};

	// Token: 0x04002105 RID: 8453
	public static Type[] serversideOnlyTypes = new Type[]
	{
		typeof(IServerComponent),
		typeof(NavMeshObstacle)
	};

	// Token: 0x04002106 RID: 8454
	public bool isClientside;

	// Token: 0x04002107 RID: 8455
	public bool isServerside;

	// Token: 0x04002108 RID: 8456
	public bool isBundling;

	// Token: 0x04002109 RID: 8457
	internal Dictionary<string, GameObject> prefabList = new Dictionary<string, GameObject>(StringComparer.OrdinalIgnoreCase);

	// Token: 0x0400210A RID: 8458
	private List<Component> destroyList = new List<Component>();

	// Token: 0x0400210B RID: 8459
	private List<GameObject> cleanupList = new List<GameObject>();
}
