using System;
using UnityEngine;

// Token: 0x0200093C RID: 2364
public class RagdollEditor : SingletonComponent<RagdollEditor>
{
	// Token: 0x06003822 RID: 14370 RVA: 0x0014C131 File Offset: 0x0014A331
	private void OnGUI()
	{
		GUI.Box(new Rect((float)Screen.width * 0.5f - 2f, (float)Screen.height * 0.5f - 2f, 4f, 4f), "");
	}

	// Token: 0x06003823 RID: 14371 RVA: 0x0014C170 File Offset: 0x0014A370
	protected override void Awake()
	{
		base.Awake();
	}

	// Token: 0x06003824 RID: 14372 RVA: 0x0014C178 File Offset: 0x0014A378
	private void Update()
	{
		Camera.main.fieldOfView = 75f;
		if (Input.GetKey(KeyCode.Mouse1))
		{
			this.view.y = this.view.y + Input.GetAxisRaw("Mouse X") * 3f;
			this.view.x = this.view.x - Input.GetAxisRaw("Mouse Y") * 3f;
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		else
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
		Camera.main.transform.rotation = Quaternion.Euler(this.view);
		Vector3 vector = Vector3.zero;
		if (Input.GetKey(KeyCode.W))
		{
			vector += Vector3.forward;
		}
		if (Input.GetKey(KeyCode.S))
		{
			vector += Vector3.back;
		}
		if (Input.GetKey(KeyCode.A))
		{
			vector += Vector3.left;
		}
		if (Input.GetKey(KeyCode.D))
		{
			vector += Vector3.right;
		}
		Camera.main.transform.position += base.transform.rotation * vector * 0.05f;
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			this.StartGrab();
		}
		if (Input.GetKeyUp(KeyCode.Mouse0))
		{
			this.StopGrab();
		}
	}

	// Token: 0x06003825 RID: 14373 RVA: 0x0014C2C5 File Offset: 0x0014A4C5
	private void FixedUpdate()
	{
		if (Input.GetKey(KeyCode.Mouse0))
		{
			this.UpdateGrab();
		}
	}

	// Token: 0x06003826 RID: 14374 RVA: 0x0014C2DC File Offset: 0x0014A4DC
	private void StartGrab()
	{
		RaycastHit raycastHit;
		if (!Physics.Raycast(base.transform.position, base.transform.forward, out raycastHit, 100f))
		{
			return;
		}
		this.grabbedRigid = raycastHit.collider.GetComponent<Rigidbody>();
		if (this.grabbedRigid == null)
		{
			return;
		}
		this.grabPos = this.grabbedRigid.transform.worldToLocalMatrix.MultiplyPoint(raycastHit.point);
		this.grabOffset = base.transform.worldToLocalMatrix.MultiplyPoint(raycastHit.point);
	}

	// Token: 0x06003827 RID: 14375 RVA: 0x0014C374 File Offset: 0x0014A574
	private void UpdateGrab()
	{
		if (this.grabbedRigid == null)
		{
			return;
		}
		Vector3 a = base.transform.TransformPoint(this.grabOffset);
		Vector3 vector = this.grabbedRigid.transform.TransformPoint(this.grabPos);
		Vector3 a2 = a - vector;
		this.grabbedRigid.AddForceAtPosition(a2 * 100f, vector, ForceMode.Acceleration);
	}

	// Token: 0x06003828 RID: 14376 RVA: 0x0014C3D7 File Offset: 0x0014A5D7
	private void StopGrab()
	{
		this.grabbedRigid = null;
	}

	// Token: 0x04003248 RID: 12872
	private Vector3 view;

	// Token: 0x04003249 RID: 12873
	private Rigidbody grabbedRigid;

	// Token: 0x0400324A RID: 12874
	private Vector3 grabPos;

	// Token: 0x0400324B RID: 12875
	private Vector3 grabOffset;
}
