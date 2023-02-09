using System;
using System.IO;
using System.Linq;
using UnityEngine;

// Token: 0x020008C5 RID: 2245
public class ChildrenScreenshot : MonoBehaviour
{
	// Token: 0x06003627 RID: 13863 RVA: 0x00143578 File Offset: 0x00141778
	[ContextMenu("Create Screenshots")]
	public void CreateScreenshots()
	{
		RenderTexture renderTexture = new RenderTexture(this.width, this.height, 0);
		GameObject gameObject = new GameObject();
		Camera camera = gameObject.AddComponent<Camera>();
		camera.targetTexture = renderTexture;
		camera.orthographic = false;
		camera.fieldOfView = this.fieldOfView;
		camera.nearClipPlane = 0.1f;
		camera.farClipPlane = 2000f;
		camera.cullingMask = LayerMask.GetMask(new string[]
		{
			"TransparentFX"
		});
		camera.clearFlags = CameraClearFlags.Color;
		camera.backgroundColor = new Color(0f, 0f, 0f, 0f);
		camera.renderingPath = RenderingPath.DeferredShading;
		Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
		foreach (Transform transform in base.transform.Cast<Transform>())
		{
			this.PositionCamera(camera, transform.gameObject);
			int layer = transform.gameObject.layer;
			transform.gameObject.SetLayerRecursive(1);
			camera.Render();
			transform.gameObject.SetLayerRecursive(layer);
			string text = transform.GetRecursiveName("");
			text = text.Replace('/', '.');
			RenderTexture.active = renderTexture;
			texture2D.ReadPixels(new Rect(0f, 0f, (float)renderTexture.width, (float)renderTexture.height), 0, 0, false);
			RenderTexture.active = null;
			byte[] bytes = texture2D.EncodeToPNG();
			string path = string.Format(this.folder, text, transform.name);
			string directoryName = Path.GetDirectoryName(path);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			File.WriteAllBytes(path, bytes);
		}
		UnityEngine.Object.DestroyImmediate(texture2D, true);
		UnityEngine.Object.DestroyImmediate(renderTexture, true);
		UnityEngine.Object.DestroyImmediate(gameObject, true);
	}

	// Token: 0x06003628 RID: 13864 RVA: 0x00143754 File Offset: 0x00141954
	public void PositionCamera(Camera cam, GameObject obj)
	{
		Bounds bounds = new Bounds(obj.transform.position, Vector3.zero * 0.1f);
		bool flag = true;
		foreach (Renderer renderer in obj.GetComponentsInChildren<Renderer>())
		{
			if (flag)
			{
				bounds = renderer.bounds;
				flag = false;
			}
			else
			{
				bounds.Encapsulate(renderer.bounds);
			}
		}
		float d = bounds.size.magnitude * 0.5f / Mathf.Tan(cam.fieldOfView * 0.5f * 0.017453292f);
		cam.transform.position = bounds.center + obj.transform.TransformVector(this.offsetAngle.normalized) * d;
		cam.transform.LookAt(bounds.center);
	}

	// Token: 0x0400311B RID: 12571
	public Vector3 offsetAngle = new Vector3(0f, 0f, 1f);

	// Token: 0x0400311C RID: 12572
	public int width = 512;

	// Token: 0x0400311D RID: 12573
	public int height = 512;

	// Token: 0x0400311E RID: 12574
	public float fieldOfView = 70f;

	// Token: 0x0400311F RID: 12575
	[Tooltip("0 = full recursive name, 1 = object name")]
	public string folder = "screenshots/{0}.png";
}
