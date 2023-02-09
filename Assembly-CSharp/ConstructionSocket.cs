using System;
using UnityEngine;

// Token: 0x0200023E RID: 574
public class ConstructionSocket : Socket_Base
{
	// Token: 0x06001B1A RID: 6938 RVA: 0x000BDB74 File Offset: 0x000BBD74
	private void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.red;
		Gizmos.DrawLine(Vector3.zero, Vector3.forward * 0.6f);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(Vector3.zero, Vector3.right * 0.1f);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(Vector3.zero, Vector3.up * 0.1f);
		Gizmos.DrawIcon(base.transform.position, "light_circle_green.png", false);
	}

	// Token: 0x06001B1B RID: 6939 RVA: 0x000BDC10 File Offset: 0x000BBE10
	private void OnDrawGizmosSelected()
	{
		if (this.female)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.DrawWireCube(this.selectCenter, this.selectSize);
		}
	}

	// Token: 0x06001B1C RID: 6940 RVA: 0x000BDC3B File Offset: 0x000BBE3B
	public override bool TestTarget(Construction.Target target)
	{
		return base.TestTarget(target) && this.IsCompatible(target.socket);
	}

	// Token: 0x06001B1D RID: 6941 RVA: 0x000BDC54 File Offset: 0x000BBE54
	public override bool IsCompatible(Socket_Base socket)
	{
		if (!base.IsCompatible(socket))
		{
			return false;
		}
		ConstructionSocket constructionSocket = socket as ConstructionSocket;
		return !(constructionSocket == null) && constructionSocket.socketType != ConstructionSocket.Type.None && this.socketType != ConstructionSocket.Type.None && constructionSocket.socketType == this.socketType;
	}

	// Token: 0x06001B1E RID: 6942 RVA: 0x000BDCA4 File Offset: 0x000BBEA4
	public override bool CanConnect(Vector3 position, Quaternion rotation, Socket_Base socket, Vector3 socketPosition, Quaternion socketRotation)
	{
		if (!base.CanConnect(position, rotation, socket, socketPosition, socketRotation))
		{
			return false;
		}
		Matrix4x4 matrix4x = Matrix4x4.TRS(position, rotation, Vector3.one);
		Matrix4x4 matrix4x2 = Matrix4x4.TRS(socketPosition, socketRotation, Vector3.one);
		Vector3 a = matrix4x.MultiplyPoint3x4(this.worldPosition);
		Vector3 b = matrix4x2.MultiplyPoint3x4(socket.worldPosition);
		if (Vector3.Distance(a, b) > 0.01f)
		{
			return false;
		}
		Vector3 vector = matrix4x.MultiplyVector(this.worldRotation * Vector3.forward);
		Vector3 vector2 = matrix4x2.MultiplyVector(socket.worldRotation * Vector3.forward);
		float num = Vector3.Angle(vector, vector2);
		if (this.male && this.female)
		{
			num = Mathf.Min(num, Vector3.Angle(-vector, vector2));
		}
		if (socket.male && socket.female)
		{
			num = Mathf.Min(num, Vector3.Angle(vector, -vector2));
		}
		return num <= 1f;
	}

	// Token: 0x06001B1F RID: 6943 RVA: 0x000BDD9C File Offset: 0x000BBF9C
	public bool TestRestrictedAngles(Vector3 suggestedPos, Quaternion suggestedAng, Construction.Target target)
	{
		if (this.restrictPlacementAngle)
		{
			Quaternion rotation = Quaternion.Euler(0f, this.faceAngle, 0f) * suggestedAng;
			float num = target.ray.direction.XZ3D().DotDegrees(rotation * Vector3.forward);
			if (num > this.angleAllowed * 0.5f)
			{
				return false;
			}
			if (num < this.angleAllowed * -0.5f)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001B20 RID: 6944 RVA: 0x000BDE14 File Offset: 0x000BC014
	public override Construction.Placement DoPlacement(Construction.Target target)
	{
		if (!target.entity || !target.entity.transform)
		{
			return null;
		}
		if (!this.CanConnectToEntity(target))
		{
			return null;
		}
		ConstructionSocket constructionSocket = target.socket as ConstructionSocket;
		Vector3 worldPosition = target.GetWorldPosition();
		Quaternion worldRotation = target.GetWorldRotation(true);
		if (constructionSocket != null && !this.IsCompatible(constructionSocket))
		{
			return null;
		}
		if (this.rotationDegrees > 0 && (constructionSocket == null || !constructionSocket.restrictPlacementRotation))
		{
			Construction.Placement placement = new Construction.Placement();
			float num = float.MaxValue;
			float num2 = 0f;
			for (int i = 0; i < 360; i += this.rotationDegrees)
			{
				Quaternion lhs = Quaternion.Euler(0f, (float)(this.rotationOffset + i), 0f);
				Vector3 direction = target.ray.direction;
				Vector3 to = lhs * worldRotation * Vector3.up;
				float num3 = Vector3.Angle(direction, to);
				if (num3 < num)
				{
					num = num3;
					num2 = (float)i;
				}
			}
			for (int j = 0; j < 360; j += this.rotationDegrees)
			{
				Quaternion rhs = worldRotation * Quaternion.Inverse(this.rotation);
				Quaternion lhs2 = Quaternion.Euler(target.rotation);
				Quaternion rhs2 = Quaternion.Euler(0f, (float)(this.rotationOffset + j) + num2, 0f);
				Quaternion rotation = lhs2 * rhs2 * rhs;
				Vector3 b = rotation * this.position;
				placement.position = worldPosition - b;
				placement.rotation = rotation;
				if (this.CheckSocketMods(placement))
				{
					return placement;
				}
			}
		}
		Construction.Placement placement2 = new Construction.Placement();
		Quaternion rotation2 = worldRotation * Quaternion.Inverse(this.rotation);
		Vector3 b2 = rotation2 * this.position;
		placement2.position = worldPosition - b2;
		placement2.rotation = rotation2;
		if (!this.TestRestrictedAngles(worldPosition, worldRotation, target))
		{
			return null;
		}
		return placement2;
	}

	// Token: 0x06001B21 RID: 6945 RVA: 0x00003A54 File Offset: 0x00001C54
	protected virtual bool CanConnectToEntity(Construction.Target target)
	{
		return true;
	}

	// Token: 0x0400142D RID: 5165
	public ConstructionSocket.Type socketType;

	// Token: 0x0400142E RID: 5166
	public int rotationDegrees;

	// Token: 0x0400142F RID: 5167
	public int rotationOffset;

	// Token: 0x04001430 RID: 5168
	public bool restrictPlacementRotation;

	// Token: 0x04001431 RID: 5169
	public bool restrictPlacementAngle;

	// Token: 0x04001432 RID: 5170
	public float faceAngle;

	// Token: 0x04001433 RID: 5171
	public float angleAllowed = 150f;

	// Token: 0x04001434 RID: 5172
	[Range(0f, 1f)]
	public float support = 1f;

	// Token: 0x02000C34 RID: 3124
	public enum Type
	{
		// Token: 0x0400413A RID: 16698
		None,
		// Token: 0x0400413B RID: 16699
		Foundation,
		// Token: 0x0400413C RID: 16700
		Floor,
		// Token: 0x0400413D RID: 16701
		Misc,
		// Token: 0x0400413E RID: 16702
		Doorway,
		// Token: 0x0400413F RID: 16703
		Wall,
		// Token: 0x04004140 RID: 16704
		Block,
		// Token: 0x04004141 RID: 16705
		Ramp,
		// Token: 0x04004142 RID: 16706
		StairsTriangle,
		// Token: 0x04004143 RID: 16707
		Stairs,
		// Token: 0x04004144 RID: 16708
		FloorFrameTriangle,
		// Token: 0x04004145 RID: 16709
		Window,
		// Token: 0x04004146 RID: 16710
		Shutters,
		// Token: 0x04004147 RID: 16711
		WallFrame,
		// Token: 0x04004148 RID: 16712
		FloorFrame,
		// Token: 0x04004149 RID: 16713
		WindowDressing,
		// Token: 0x0400414A RID: 16714
		DoorDressing,
		// Token: 0x0400414B RID: 16715
		Elevator,
		// Token: 0x0400414C RID: 16716
		DoubleDoorDressing
	}
}
