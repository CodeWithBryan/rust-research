using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020008BA RID: 2234
public class BaseCommandBuffer : MonoBehaviour
{
	// Token: 0x060035FE RID: 13822 RVA: 0x00142EA8 File Offset: 0x001410A8
	protected CommandBuffer GetCommandBuffer(string name, Camera camera, CameraEvent cameraEvent)
	{
		Dictionary<int, CommandBuffer> dictionary;
		if (!this.cameras.TryGetValue(camera, out dictionary))
		{
			dictionary = new Dictionary<int, CommandBuffer>();
			this.cameras.Add(camera, dictionary);
		}
		CommandBuffer commandBuffer;
		if (dictionary.TryGetValue((int)cameraEvent, out commandBuffer))
		{
			commandBuffer.Clear();
		}
		else
		{
			commandBuffer = new CommandBuffer();
			commandBuffer.name = name;
			dictionary.Add((int)cameraEvent, commandBuffer);
			this.CleanupCamera(name, camera, cameraEvent);
			camera.AddCommandBuffer(cameraEvent, commandBuffer);
		}
		return commandBuffer;
	}

	// Token: 0x060035FF RID: 13823 RVA: 0x00142F14 File Offset: 0x00141114
	protected void CleanupCamera(string name, Camera camera, CameraEvent cameraEvent)
	{
		foreach (CommandBuffer commandBuffer in camera.GetCommandBuffers(cameraEvent))
		{
			if (commandBuffer.name == name)
			{
				camera.RemoveCommandBuffer(cameraEvent, commandBuffer);
			}
		}
	}

	// Token: 0x06003600 RID: 13824 RVA: 0x00142F54 File Offset: 0x00141154
	protected void CleanupCommandBuffer(Camera camera, CameraEvent cameraEvent)
	{
		Dictionary<int, CommandBuffer> dictionary;
		if (!this.cameras.TryGetValue(camera, out dictionary))
		{
			return;
		}
		CommandBuffer buffer;
		if (!dictionary.TryGetValue((int)cameraEvent, out buffer))
		{
			return;
		}
		camera.RemoveCommandBuffer(cameraEvent, buffer);
	}

	// Token: 0x06003601 RID: 13825 RVA: 0x00142F88 File Offset: 0x00141188
	protected void Cleanup()
	{
		foreach (KeyValuePair<Camera, Dictionary<int, CommandBuffer>> keyValuePair in this.cameras)
		{
			Camera key = keyValuePair.Key;
			Dictionary<int, CommandBuffer> value = keyValuePair.Value;
			if (key)
			{
				foreach (KeyValuePair<int, CommandBuffer> keyValuePair2 in value)
				{
					int key2 = keyValuePair2.Key;
					CommandBuffer value2 = keyValuePair2.Value;
					key.RemoveCommandBuffer((CameraEvent)key2, value2);
				}
			}
		}
	}

	// Token: 0x04003106 RID: 12550
	private Dictionary<Camera, Dictionary<int, CommandBuffer>> cameras = new Dictionary<Camera, Dictionary<int, CommandBuffer>>();
}
