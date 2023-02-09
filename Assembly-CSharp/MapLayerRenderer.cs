using System;
using System.Collections.Generic;
using System.Linq;
using Network;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020007C4 RID: 1988
public class MapLayerRenderer : SingletonComponent<MapLayerRenderer>
{
	// Token: 0x060033E9 RID: 13289 RVA: 0x0013C608 File Offset: 0x0013A808
	private void RenderDungeonsLayer()
	{
		ProceduralDynamicDungeon proceduralDynamicDungeon = MapLayerRenderer.FindDungeon(MainCamera.isValid ? MainCamera.position : Vector3.zero, 200f);
		MapLayer? currentlyRenderedLayer = this._currentlyRenderedLayer;
		MapLayer mapLayer = MapLayer.Dungeons;
		if (currentlyRenderedLayer.GetValueOrDefault() == mapLayer & currentlyRenderedLayer != null)
		{
			uint? currentlyRenderedDungeon = this._currentlyRenderedDungeon;
			uint? num;
			if (proceduralDynamicDungeon == null)
			{
				num = null;
			}
			else
			{
				Networkable net = proceduralDynamicDungeon.net;
				num = ((net != null) ? new uint?(net.ID) : null);
			}
			uint? num2 = num;
			if (currentlyRenderedDungeon.GetValueOrDefault() == num2.GetValueOrDefault() & currentlyRenderedDungeon != null == (num2 != null))
			{
				return;
			}
		}
		this._currentlyRenderedLayer = new MapLayer?(MapLayer.Dungeons);
		uint? currentlyRenderedDungeon2;
		if (proceduralDynamicDungeon == null)
		{
			currentlyRenderedDungeon2 = null;
		}
		else
		{
			Networkable net2 = proceduralDynamicDungeon.net;
			currentlyRenderedDungeon2 = ((net2 != null) ? new uint?(net2.ID) : null);
		}
		this._currentlyRenderedDungeon = currentlyRenderedDungeon2;
		using (CommandBuffer commandBuffer = this.BuildCommandBufferDungeons(proceduralDynamicDungeon))
		{
			this.RenderImpl(commandBuffer);
		}
	}

	// Token: 0x060033EA RID: 13290 RVA: 0x0013C720 File Offset: 0x0013A920
	private CommandBuffer BuildCommandBufferDungeons(ProceduralDynamicDungeon closest)
	{
		CommandBuffer commandBuffer = new CommandBuffer
		{
			name = "DungeonsLayer Render"
		};
		if (closest != null && closest.spawnedCells != null)
		{
			Matrix4x4 lhs = Matrix4x4.Translate(closest.mapOffset);
			foreach (ProceduralDungeonCell proceduralDungeonCell in closest.spawnedCells)
			{
				if (!(proceduralDungeonCell == null) && proceduralDungeonCell.mapRenderers != null && proceduralDungeonCell.mapRenderers.Length != 0)
				{
					foreach (MeshRenderer meshRenderer in proceduralDungeonCell.mapRenderers)
					{
						MeshFilter meshFilter;
						if (!(meshRenderer == null) && meshRenderer.TryGetComponent<MeshFilter>(out meshFilter))
						{
							Mesh sharedMesh = meshFilter.sharedMesh;
							int subMeshCount = sharedMesh.subMeshCount;
							Matrix4x4 matrix = lhs * meshRenderer.transform.localToWorldMatrix;
							for (int j = 0; j < subMeshCount; j++)
							{
								commandBuffer.DrawMesh(sharedMesh, matrix, this.renderMaterial, j);
							}
						}
					}
				}
			}
		}
		return commandBuffer;
	}

	// Token: 0x060033EB RID: 13291 RVA: 0x0013C84C File Offset: 0x0013AA4C
	public static ProceduralDynamicDungeon FindDungeon(Vector3 position, float maxDist = 200f)
	{
		ProceduralDynamicDungeon result = null;
		float num = 100000f;
		foreach (ProceduralDynamicDungeon proceduralDynamicDungeon in ProceduralDynamicDungeon.dungeons)
		{
			if (!(proceduralDynamicDungeon == null) && proceduralDynamicDungeon.isClient)
			{
				float num2 = Vector3.Distance(position, proceduralDynamicDungeon.transform.position);
				if (num2 <= maxDist && num2 <= num)
				{
					result = proceduralDynamicDungeon;
					num = num2;
				}
			}
		}
		return result;
	}

	// Token: 0x060033EC RID: 13292 RVA: 0x0013C8D4 File Offset: 0x0013AAD4
	private void RenderTrainLayer()
	{
		using (CommandBuffer commandBuffer = this.BuildCommandBufferTrainTunnels())
		{
			this.RenderImpl(commandBuffer);
		}
	}

	// Token: 0x060033ED RID: 13293 RVA: 0x0013C90C File Offset: 0x0013AB0C
	private CommandBuffer BuildCommandBufferTrainTunnels()
	{
		CommandBuffer commandBuffer = new CommandBuffer
		{
			name = "TrainLayer Render"
		};
		foreach (DungeonGridCell dungeonGridCell in TerrainMeta.Path.DungeonGridCells)
		{
			if (dungeonGridCell.MapRenderers != null && dungeonGridCell.MapRenderers.Length != 0)
			{
				foreach (MeshRenderer meshRenderer in dungeonGridCell.MapRenderers)
				{
					MeshFilter meshFilter;
					if (!(meshRenderer == null) && meshRenderer.TryGetComponent<MeshFilter>(out meshFilter))
					{
						Mesh sharedMesh = meshFilter.sharedMesh;
						int subMeshCount = sharedMesh.subMeshCount;
						Matrix4x4 localToWorldMatrix = meshRenderer.transform.localToWorldMatrix;
						for (int j = 0; j < subMeshCount; j++)
						{
							commandBuffer.DrawMesh(sharedMesh, localToWorldMatrix, this.renderMaterial, j);
						}
					}
				}
			}
		}
		return commandBuffer;
	}

	// Token: 0x060033EE RID: 13294 RVA: 0x0013CA00 File Offset: 0x0013AC00
	private void RenderUnderwaterLabs(int floor)
	{
		using (CommandBuffer commandBuffer = this.BuildCommandBufferUnderwaterLabs(floor))
		{
			this.RenderImpl(commandBuffer);
		}
	}

	// Token: 0x060033EF RID: 13295 RVA: 0x0013CA38 File Offset: 0x0013AC38
	public int GetUnderwaterLabFloorCount()
	{
		if (this._underwaterLabFloorCount != null)
		{
			return this._underwaterLabFloorCount.Value;
		}
		List<DungeonBaseInfo> dungeonBaseEntrances = TerrainMeta.Path.DungeonBaseEntrances;
		int value;
		if (dungeonBaseEntrances == null || dungeonBaseEntrances.Count <= 0)
		{
			value = 0;
		}
		else
		{
			value = dungeonBaseEntrances.Max((DungeonBaseInfo l) => l.Floors.Count);
		}
		this._underwaterLabFloorCount = new int?(value);
		return this._underwaterLabFloorCount.Value;
	}

	// Token: 0x060033F0 RID: 13296 RVA: 0x0013CAB4 File Offset: 0x0013ACB4
	private CommandBuffer BuildCommandBufferUnderwaterLabs(int floor)
	{
		CommandBuffer commandBuffer = new CommandBuffer
		{
			name = "UnderwaterLabLayer Render"
		};
		foreach (DungeonBaseInfo dungeonBaseInfo in TerrainMeta.Path.DungeonBaseEntrances)
		{
			if (dungeonBaseInfo.Floors.Count > floor)
			{
				foreach (DungeonBaseLink dungeonBaseLink in dungeonBaseInfo.Floors[floor].Links)
				{
					if (dungeonBaseLink.MapRenderers != null && dungeonBaseLink.MapRenderers.Length != 0)
					{
						foreach (MeshRenderer meshRenderer in dungeonBaseLink.MapRenderers)
						{
							MeshFilter meshFilter;
							if (!(meshRenderer == null) && meshRenderer.TryGetComponent<MeshFilter>(out meshFilter))
							{
								Mesh sharedMesh = meshFilter.sharedMesh;
								int subMeshCount = sharedMesh.subMeshCount;
								Matrix4x4 localToWorldMatrix = meshRenderer.transform.localToWorldMatrix;
								for (int j = 0; j < subMeshCount; j++)
								{
									commandBuffer.DrawMesh(sharedMesh, localToWorldMatrix, this.renderMaterial, j);
								}
							}
						}
					}
				}
			}
		}
		return commandBuffer;
	}

	// Token: 0x060033F1 RID: 13297 RVA: 0x0013CC0C File Offset: 0x0013AE0C
	public void Render(MapLayer layer)
	{
		if (layer < MapLayer.TrainTunnels)
		{
			return;
		}
		if (layer == MapLayer.Dungeons)
		{
			this.RenderDungeonsLayer();
			return;
		}
		MapLayer? currentlyRenderedLayer = this._currentlyRenderedLayer;
		if (layer == currentlyRenderedLayer.GetValueOrDefault() & currentlyRenderedLayer != null)
		{
			return;
		}
		this._currentlyRenderedLayer = new MapLayer?(layer);
		if (layer == MapLayer.TrainTunnels)
		{
			this.RenderTrainLayer();
			return;
		}
		if (layer >= MapLayer.Underwater1 && layer <= MapLayer.Underwater8)
		{
			this.RenderUnderwaterLabs(layer - MapLayer.Underwater1);
		}
	}

	// Token: 0x060033F2 RID: 13298 RVA: 0x0013CC70 File Offset: 0x0013AE70
	private void RenderImpl(CommandBuffer cb)
	{
		double num = World.Size * 1.5;
		this.renderCamera.orthographicSize = (float)num / 2f;
		this.renderCamera.RemoveAllCommandBuffers();
		this.renderCamera.AddCommandBuffer(this.cameraEvent, cb);
		this.renderCamera.Render();
		this.renderCamera.RemoveAllCommandBuffers();
	}

	// Token: 0x060033F3 RID: 13299 RVA: 0x0013CCD5 File Offset: 0x0013AED5
	public static MapLayerRenderer GetOrCreate()
	{
		if (SingletonComponent<MapLayerRenderer>.Instance != null)
		{
			return SingletonComponent<MapLayerRenderer>.Instance;
		}
		return GameManager.server.CreatePrefab("assets/prefabs/engine/maplayerrenderer.prefab", Vector3.zero, Quaternion.identity, true).GetComponent<MapLayerRenderer>();
	}

	// Token: 0x04002C06 RID: 11270
	private uint? _currentlyRenderedDungeon;

	// Token: 0x04002C07 RID: 11271
	private int? _underwaterLabFloorCount;

	// Token: 0x04002C08 RID: 11272
	public Camera renderCamera;

	// Token: 0x04002C09 RID: 11273
	public CameraEvent cameraEvent;

	// Token: 0x04002C0A RID: 11274
	public Material renderMaterial;

	// Token: 0x04002C0B RID: 11275
	private MapLayer? _currentlyRenderedLayer;
}
