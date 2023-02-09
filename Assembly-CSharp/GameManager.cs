using System;
using ConVar;
using Facepunch;
using Rust;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020004E3 RID: 1251
public class GameManager
{
	// Token: 0x060027BF RID: 10175 RVA: 0x000F3955 File Offset: 0x000F1B55
	public void Reset()
	{
		this.pool.Clear();
	}

	// Token: 0x060027C0 RID: 10176 RVA: 0x000F3962 File Offset: 0x000F1B62
	public GameManager(bool clientside, bool serverside)
	{
		this.Clientside = clientside;
		this.Serverside = serverside;
		this.preProcessed = new PrefabPreProcess(clientside, serverside, false);
		this.pool = new PrefabPoolCollection();
	}

	// Token: 0x060027C1 RID: 10177 RVA: 0x000F3994 File Offset: 0x000F1B94
	public GameObject FindPrefab(uint prefabID)
	{
		string text = StringPool.Get(prefabID);
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		return this.FindPrefab(text);
	}

	// Token: 0x060027C2 RID: 10178 RVA: 0x000F39B9 File Offset: 0x000F1BB9
	public GameObject FindPrefab(BaseEntity ent)
	{
		if (ent == null)
		{
			return null;
		}
		return this.FindPrefab(ent.PrefabName);
	}

	// Token: 0x060027C3 RID: 10179 RVA: 0x000F39D4 File Offset: 0x000F1BD4
	public GameObject FindPrefab(string strPrefab)
	{
		GameObject gameObject = this.preProcessed.Find(strPrefab);
		if (gameObject != null)
		{
			return gameObject;
		}
		gameObject = FileSystem.LoadPrefab(strPrefab);
		if (gameObject == null)
		{
			return null;
		}
		this.preProcessed.Process(strPrefab, gameObject);
		GameObject gameObject2 = this.preProcessed.Find(strPrefab);
		if (!(gameObject2 != null))
		{
			return gameObject;
		}
		return gameObject2;
	}

	// Token: 0x060027C4 RID: 10180 RVA: 0x000F3A34 File Offset: 0x000F1C34
	public GameObject CreatePrefab(string strPrefab, Vector3 pos, Quaternion rot, Vector3 scale, bool active = true)
	{
		GameObject gameObject = this.Instantiate(strPrefab, pos, rot);
		if (gameObject)
		{
			gameObject.transform.localScale = scale;
			if (active)
			{
				gameObject.AwakeFromInstantiate();
			}
		}
		return gameObject;
	}

	// Token: 0x060027C5 RID: 10181 RVA: 0x000F3A6C File Offset: 0x000F1C6C
	public GameObject CreatePrefab(string strPrefab, Vector3 pos, Quaternion rot, bool active = true)
	{
		GameObject gameObject = this.Instantiate(strPrefab, pos, rot);
		if (gameObject && active)
		{
			gameObject.AwakeFromInstantiate();
		}
		return gameObject;
	}

	// Token: 0x060027C6 RID: 10182 RVA: 0x000F3A98 File Offset: 0x000F1C98
	public GameObject CreatePrefab(string strPrefab, bool active = true)
	{
		GameObject gameObject = this.Instantiate(strPrefab, Vector3.zero, Quaternion.identity);
		if (gameObject && active)
		{
			gameObject.AwakeFromInstantiate();
		}
		return gameObject;
	}

	// Token: 0x060027C7 RID: 10183 RVA: 0x000F3ACC File Offset: 0x000F1CCC
	public GameObject CreatePrefab(string strPrefab, Transform parent, bool active = true)
	{
		GameObject gameObject = this.Instantiate(strPrefab, parent.position, parent.rotation);
		if (gameObject)
		{
			gameObject.transform.SetParent(parent, false);
			gameObject.Identity();
			if (active)
			{
				gameObject.AwakeFromInstantiate();
			}
		}
		return gameObject;
	}

	// Token: 0x060027C8 RID: 10184 RVA: 0x000F3B14 File Offset: 0x000F1D14
	public BaseEntity CreateEntity(string strPrefab, Vector3 pos = default(Vector3), Quaternion rot = default(Quaternion), bool startActive = true)
	{
		if (string.IsNullOrEmpty(strPrefab))
		{
			return null;
		}
		GameObject gameObject = this.CreatePrefab(strPrefab, pos, rot, startActive);
		if (gameObject == null)
		{
			return null;
		}
		BaseEntity component = gameObject.GetComponent<BaseEntity>();
		if (component == null)
		{
			Debug.LogError("CreateEntity called on a prefab that isn't an entity! " + strPrefab);
			UnityEngine.Object.Destroy(gameObject);
			return null;
		}
		if (component.CompareTag("CannotBeCreated"))
		{
			Debug.LogWarning("CreateEntity called on a prefab that has the CannotBeCreated tag set. " + strPrefab);
			UnityEngine.Object.Destroy(gameObject);
			return null;
		}
		return component;
	}

	// Token: 0x060027C9 RID: 10185 RVA: 0x000F3B90 File Offset: 0x000F1D90
	private GameObject Instantiate(string strPrefab, Vector3 pos, Quaternion rot)
	{
		if (!strPrefab.IsLower())
		{
			Debug.LogWarning("Converting prefab name to lowercase: " + strPrefab);
			strPrefab = strPrefab.ToLower();
		}
		GameObject gameObject = this.FindPrefab(strPrefab);
		if (!gameObject)
		{
			Debug.LogError("Couldn't find prefab \"" + strPrefab + "\"");
			return null;
		}
		GameObject gameObject2 = this.pool.Pop(StringPool.Get(strPrefab), pos, rot);
		if (gameObject2 == null)
		{
			gameObject2 = Facepunch.Instantiate.GameObject(gameObject, pos, rot);
			gameObject2.name = strPrefab;
		}
		else
		{
			gameObject2.transform.localScale = gameObject.transform.localScale;
		}
		if (!this.Clientside && this.Serverside && gameObject2.transform.parent == null)
		{
			SceneManager.MoveGameObjectToScene(gameObject2, Rust.Server.EntityScene);
		}
		return gameObject2;
	}

	// Token: 0x060027CA RID: 10186 RVA: 0x000F3C58 File Offset: 0x000F1E58
	public static void Destroy(Component component, float delay = 0f)
	{
		if ((component as BaseEntity).IsValid())
		{
			Debug.LogError("Trying to destroy an entity without killing it first: " + component.name);
		}
		UnityEngine.Object.Destroy(component, delay);
	}

	// Token: 0x060027CB RID: 10187 RVA: 0x000F3C83 File Offset: 0x000F1E83
	public static void Destroy(GameObject instance, float delay = 0f)
	{
		if (!instance)
		{
			return;
		}
		if (instance.GetComponent<BaseEntity>().IsValid())
		{
			Debug.LogError("Trying to destroy an entity without killing it first: " + instance.name);
		}
		UnityEngine.Object.Destroy(instance, delay);
	}

	// Token: 0x060027CC RID: 10188 RVA: 0x000F3CB7 File Offset: 0x000F1EB7
	public static void DestroyImmediate(Component component, bool allowDestroyingAssets = false)
	{
		if ((component as BaseEntity).IsValid())
		{
			Debug.LogError("Trying to destroy an entity without killing it first: " + component.name);
		}
		UnityEngine.Object.DestroyImmediate(component, allowDestroyingAssets);
	}

	// Token: 0x060027CD RID: 10189 RVA: 0x000F3CE2 File Offset: 0x000F1EE2
	public static void DestroyImmediate(GameObject instance, bool allowDestroyingAssets = false)
	{
		if (instance.GetComponent<BaseEntity>().IsValid())
		{
			Debug.LogError("Trying to destroy an entity without killing it first: " + instance.name);
		}
		UnityEngine.Object.DestroyImmediate(instance, allowDestroyingAssets);
	}

	// Token: 0x060027CE RID: 10190 RVA: 0x000F3D10 File Offset: 0x000F1F10
	public void Retire(GameObject instance)
	{
		if (!instance)
		{
			return;
		}
		using (TimeWarning.New("GameManager.Retire", 0))
		{
			if (instance.GetComponent<BaseEntity>().IsValid())
			{
				Debug.LogError("Trying to retire an entity without killing it first: " + instance.name);
			}
			if (!Rust.Application.isQuitting && ConVar.Pool.enabled && instance.SupportsPooling())
			{
				this.pool.Push(instance);
			}
			else
			{
				UnityEngine.Object.Destroy(instance);
			}
		}
	}

	// Token: 0x04002003 RID: 8195
	public static GameManager server = new GameManager(false, true);

	// Token: 0x04002004 RID: 8196
	internal PrefabPreProcess preProcessed;

	// Token: 0x04002005 RID: 8197
	internal PrefabPoolCollection pool;

	// Token: 0x04002006 RID: 8198
	private bool Clientside;

	// Token: 0x04002007 RID: 8199
	private bool Serverside;
}
