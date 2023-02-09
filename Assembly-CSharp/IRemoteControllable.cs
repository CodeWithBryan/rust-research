using System;
using UnityEngine;

// Token: 0x020000FA RID: 250
public interface IRemoteControllable
{
	// Token: 0x060014BC RID: 5308
	Transform GetEyes();

	// Token: 0x060014BD RID: 5309
	BaseEntity GetEnt();

	// Token: 0x060014BE RID: 5310
	bool Occupied();

	// Token: 0x060014BF RID: 5311
	void UpdateIdentifier(string newID, bool clientSend = false);

	// Token: 0x060014C0 RID: 5312
	string GetIdentifier();

	// Token: 0x060014C1 RID: 5313
	void RCSetup();

	// Token: 0x060014C2 RID: 5314
	void RCShutdown();

	// Token: 0x060014C3 RID: 5315
	bool CanControl();

	// Token: 0x170001AB RID: 427
	// (get) Token: 0x060014C4 RID: 5316
	bool RequiresMouse { get; }

	// Token: 0x060014C5 RID: 5317
	void UserInput(InputState inputState, BasePlayer player);

	// Token: 0x060014C6 RID: 5318
	void InitializeControl(BasePlayer controller);

	// Token: 0x060014C7 RID: 5319
	void StopControl();
}
