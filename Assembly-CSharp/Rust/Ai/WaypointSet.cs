using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x02000B01 RID: 2817
	public class WaypointSet : MonoBehaviour, IServerComponent
	{
		// Token: 0x170005E9 RID: 1513
		// (get) Token: 0x060043AB RID: 17323 RVA: 0x00188970 File Offset: 0x00186B70
		// (set) Token: 0x060043AC RID: 17324 RVA: 0x00188978 File Offset: 0x00186B78
		public List<WaypointSet.Waypoint> Points
		{
			get
			{
				return this._points;
			}
			set
			{
				this._points = value;
			}
		}

		// Token: 0x170005EA RID: 1514
		// (get) Token: 0x060043AD RID: 17325 RVA: 0x00188981 File Offset: 0x00186B81
		public WaypointSet.NavModes NavMode
		{
			get
			{
				return this.navMode;
			}
		}

		// Token: 0x060043AE RID: 17326 RVA: 0x0018898C File Offset: 0x00186B8C
		private void OnDrawGizmos()
		{
			for (int i = 0; i < this.Points.Count; i++)
			{
				Transform transform = this.Points[i].Transform;
				if (transform != null)
				{
					if (this.Points[i].IsOccupied)
					{
						Gizmos.color = Color.red;
					}
					else
					{
						Gizmos.color = Color.cyan;
					}
					Gizmos.DrawSphere(transform.position, 0.25f);
					Gizmos.color = Color.cyan;
					if (i + 1 < this.Points.Count)
					{
						Gizmos.DrawLine(transform.position, this.Points[i + 1].Transform.position);
					}
					else if (this.NavMode == WaypointSet.NavModes.Loop)
					{
						Gizmos.DrawLine(transform.position, this.Points[0].Transform.position);
					}
					Gizmos.color = Color.magenta - new Color(0f, 0f, 0f, 0.5f);
					foreach (Transform transform2 in this.Points[i].LookatPoints)
					{
						Gizmos.DrawSphere(transform2.position, 0.1f);
						Gizmos.DrawLine(transform.position, transform2.position);
					}
				}
			}
		}

		// Token: 0x04003C3E RID: 15422
		[SerializeField]
		private List<WaypointSet.Waypoint> _points = new List<WaypointSet.Waypoint>();

		// Token: 0x04003C3F RID: 15423
		[SerializeField]
		private WaypointSet.NavModes navMode;

		// Token: 0x02000F38 RID: 3896
		public enum NavModes
		{
			// Token: 0x04004DBF RID: 19903
			Loop,
			// Token: 0x04004DC0 RID: 19904
			PingPong
		}

		// Token: 0x02000F39 RID: 3897
		[Serializable]
		public struct Waypoint
		{
			// Token: 0x04004DC1 RID: 19905
			public Transform Transform;

			// Token: 0x04004DC2 RID: 19906
			public float WaitTime;

			// Token: 0x04004DC3 RID: 19907
			public Transform[] LookatPoints;

			// Token: 0x04004DC4 RID: 19908
			[NonSerialized]
			public bool IsOccupied;
		}
	}
}
