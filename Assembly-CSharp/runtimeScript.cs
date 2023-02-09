using System;
using EasyRoads3Dv3;
using UnityEngine;

// Token: 0x02000957 RID: 2391
public class runtimeScript : MonoBehaviour
{
	// Token: 0x0600386C RID: 14444 RVA: 0x0014CECC File Offset: 0x0014B0CC
	private void Start()
	{
		Debug.Log("Please read the comments at the top of the runtime script (/Assets/EasyRoads3D/Scripts/runtimeScript) before using the runtime API!");
		this.roadNetwork = new ERRoadNetwork();
		ERRoadType erroadType = new ERRoadType();
		erroadType.roadWidth = 6f;
		erroadType.roadMaterial = (Resources.Load("Materials/roads/road material") as Material);
		erroadType.layer = 1;
		erroadType.tag = "Untagged";
		Vector3[] markers = new Vector3[]
		{
			new Vector3(200f, 5f, 200f),
			new Vector3(250f, 5f, 200f),
			new Vector3(250f, 5f, 250f),
			new Vector3(300f, 5f, 250f)
		};
		this.road = this.roadNetwork.CreateRoad("road 1", erroadType, markers);
		this.road.AddMarker(new Vector3(300f, 5f, 300f));
		this.road.InsertMarker(new Vector3(275f, 5f, 235f));
		this.road.DeleteMarker(2);
		this.roadNetwork.BuildRoadNetwork();
		this.go = GameObject.CreatePrimitive(PrimitiveType.Cube);
	}

	// Token: 0x0600386D RID: 14445 RVA: 0x0014D014 File Offset: 0x0014B214
	private void Update()
	{
		if (this.roadNetwork != null)
		{
			float num = Time.deltaTime * this.speed;
			this.distance += num;
			Vector3 position = this.road.GetPosition(this.distance, ref this.currentElement);
			position.y += 1f;
			this.go.transform.position = position;
			this.go.transform.forward = this.road.GetLookatSmooth(this.distance, this.currentElement);
		}
	}

	// Token: 0x0600386E RID: 14446 RVA: 0x0014D0A4 File Offset: 0x0014B2A4
	private void OnDestroy()
	{
		if (this.roadNetwork != null && this.roadNetwork.isInBuildMode)
		{
			this.roadNetwork.RestoreRoadNetwork();
			Debug.Log("Restore Road Network");
		}
	}

	// Token: 0x040032BF RID: 12991
	public ERRoadNetwork roadNetwork;

	// Token: 0x040032C0 RID: 12992
	public ERRoad road;

	// Token: 0x040032C1 RID: 12993
	public GameObject go;

	// Token: 0x040032C2 RID: 12994
	public int currentElement;

	// Token: 0x040032C3 RID: 12995
	public float distance;

	// Token: 0x040032C4 RID: 12996
	public float speed = 5f;
}
