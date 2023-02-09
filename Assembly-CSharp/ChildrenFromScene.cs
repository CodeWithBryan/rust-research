using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200076D RID: 1901
public class ChildrenFromScene : MonoBehaviour
{
	// Token: 0x06003355 RID: 13141 RVA: 0x0013B5D2 File Offset: 0x001397D2
	private IEnumerator Start()
	{
		Debug.LogWarning("WARNING: CHILDRENFROMSCENE(" + this.SceneName + ") - WE SHOULDN'T BE USING THIS SHITTY COMPONENT NOW WE HAVE AWESOME PREFABS", base.gameObject);
		if (!SceneManager.GetSceneByName(this.SceneName).isLoaded)
		{
			yield return SceneManager.LoadSceneAsync(this.SceneName, LoadSceneMode.Additive);
		}
		Scene sceneByName = SceneManager.GetSceneByName(this.SceneName);
		foreach (GameObject gameObject in sceneByName.GetRootGameObjects())
		{
			gameObject.transform.SetParent(base.transform, false);
			gameObject.Identity();
			RectTransform rectTransform = gameObject.transform as RectTransform;
			if (rectTransform)
			{
				rectTransform.pivot = Vector2.zero;
				rectTransform.anchoredPosition = Vector2.zero;
				rectTransform.anchorMin = Vector2.zero;
				rectTransform.anchorMax = Vector2.one;
				rectTransform.sizeDelta = Vector2.one;
			}
			SingletonComponent[] componentsInChildren = gameObject.GetComponentsInChildren<SingletonComponent>(true);
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].SingletonSetup();
			}
			if (this.StartChildrenDisabled)
			{
				gameObject.SetActive(false);
			}
		}
		SceneManager.UnloadSceneAsync(sceneByName);
		yield break;
	}

	// Token: 0x040029DB RID: 10715
	public string SceneName;

	// Token: 0x040029DC RID: 10716
	public bool StartChildrenDisabled;
}
