using System;
using UnityEngine.Rendering;

// Token: 0x020006E6 RID: 1766
public class CommandBufferDesc
{
	// Token: 0x170003D5 RID: 981
	// (get) Token: 0x06003163 RID: 12643 RVA: 0x0012FBEC File Offset: 0x0012DDEC
	// (set) Token: 0x06003164 RID: 12644 RVA: 0x0012FBF4 File Offset: 0x0012DDF4
	public CameraEvent CameraEvent { get; private set; }

	// Token: 0x170003D6 RID: 982
	// (get) Token: 0x06003165 RID: 12645 RVA: 0x0012FBFD File Offset: 0x0012DDFD
	// (set) Token: 0x06003166 RID: 12646 RVA: 0x0012FC05 File Offset: 0x0012DE05
	public int OrderId { get; private set; }

	// Token: 0x170003D7 RID: 983
	// (get) Token: 0x06003167 RID: 12647 RVA: 0x0012FC0E File Offset: 0x0012DE0E
	// (set) Token: 0x06003168 RID: 12648 RVA: 0x0012FC16 File Offset: 0x0012DE16
	public Action<CommandBuffer> FillDelegate { get; private set; }

	// Token: 0x06003169 RID: 12649 RVA: 0x0012FC1F File Offset: 0x0012DE1F
	public CommandBufferDesc(CameraEvent cameraEvent, int orderId, CommandBufferDesc.FillCommandBuffer fill)
	{
		this.CameraEvent = cameraEvent;
		this.OrderId = orderId;
		this.FillDelegate = new Action<CommandBuffer>(fill.Invoke);
	}

	// Token: 0x02000DDE RID: 3550
	// (Invoke) Token: 0x06004F7E RID: 20350
	public delegate void FillCommandBuffer(CommandBuffer cb);
}
