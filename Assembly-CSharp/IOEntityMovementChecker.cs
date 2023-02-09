using System;
using UnityEngine;

// Token: 0x020004AC RID: 1196
[RequireComponent(typeof(IOEntity))]
public class IOEntityMovementChecker : FacepunchBehaviour
{
	// Token: 0x060026AB RID: 9899 RVA: 0x000EFA25 File Offset: 0x000EDC25
	protected void Awake()
	{
		this.ioEntity = base.GetComponent<IOEntity>();
	}

	// Token: 0x060026AC RID: 9900 RVA: 0x000EFA33 File Offset: 0x000EDC33
	protected void OnEnable()
	{
		base.InvokeRepeating(new Action(this.CheckPosition), UnityEngine.Random.Range(0f, 0.25f), 0.25f);
	}

	// Token: 0x060026AD RID: 9901 RVA: 0x000EFA5B File Offset: 0x000EDC5B
	protected void OnDisable()
	{
		base.CancelInvoke(new Action(this.CheckPosition));
	}

	// Token: 0x060026AE RID: 9902 RVA: 0x000EFA70 File Offset: 0x000EDC70
	private void CheckPosition()
	{
		if (this.ioEntity.isClient)
		{
			return;
		}
		if (Vector3.SqrMagnitude(base.transform.position - this.prevPos) > 0.0025000002f)
		{
			this.prevPos = base.transform.position;
			if (this.ioEntity.HasConnections())
			{
				this.ioEntity.SendChangedToRoot(true);
				this.ioEntity.ClearConnections();
			}
		}
	}

	// Token: 0x04001F2C RID: 7980
	private IOEntity ioEntity;

	// Token: 0x04001F2D RID: 7981
	private Vector3 prevPos;

	// Token: 0x04001F2E RID: 7982
	private const float MAX_MOVE = 0.05f;

	// Token: 0x04001F2F RID: 7983
	private const float MAX_MOVE_SQR = 0.0025000002f;
}
